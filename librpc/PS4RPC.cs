/* golden */
/* 2/12/2018 */

using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace librpc
{
    public class PS4RPC
    {
        private Socket sock;
        private IPEndPoint enp;
        public bool IsConnected
        {
            get;
			private set;
        }

        private const int RPC_PORT = 733;
        private const uint RPC_PACKET_MAGIC = 0xBDAABBCC;
        private const int RPC_MAX_DATA_LEN = 8192;

        /** commands **/
        private enum RPC_CMDS : uint
        {
            RPC_PROC_READ = 0xBD000001,
            RPC_PROC_WRITE = 0xBD000002,
            RPC_PROC_LIST = 0xBD000003,
            RPC_PROC_INFO = 0xBD000004,
            RPC_PROC_INTALL = 0xBD000005,
            RPC_PROC_CALL = 0xBD000006,
            RPC_PROC_ELF = 0xBD000007,
            RPC_END = 0xBD000008,
            RPC_REBOOT = 0xBD000009,
            RPC_KERN_BASE = 0xBD00000A,
            RPC_KERN_READ = 0xBD00000B,
            RPC_KERN_WRITE = 0xBD00000C
        };

        /** packet sizes **/
        private const int RPC_PACKET_SIZE = 12;
        private const int RPC_PROC_READ_SIZE = 16;
        private const int RPC_PROC_WRITE_SIZE = 16;
        private const int RPC_PROC_LIST_SIZE = 36;
        private const int RPC_PROC_INFO1_SIZE = 4;
        private const int RPC_PROC_INFO2_SIZE = 60;
        private const int RPC_PROC_INSTALL1_SIZE = 4;
        private const int RPC_PROC_INSTALL2_SIZE = 12;
        private const int RPC_PROC_CALL1_SIZE = 68;
        private const int RPC_PROC_CALL2_SIZE = 12;
        private const int RPC_PROC_ELF_SIZE = 8;
        private const int RPC_KERN_BASE_SIZE = 8;
        private const int RPC_KERN_READ_SIZE = 12;
        private const int RPC_KERN_WRITE_SIZE = 12;

        /** status **/
        private enum RPC_STATUS : uint
        {
            RPC_SUCCESS = 0x80000000,
            RPC_TOO_MUCH_DATA = 0xF0000001,
            RPC_READ_ERROR = 0xF0000002,
            RPC_WRITE_ERROR = 0xF0000003,
            RPC_LIST_ERROR = 0xF0000004,
            RPC_INFO_ERROR = 0xF0000005,
            RPC_INFO_NO_MAP = 0x80000006,
            RPC_NO_PROC = 0xF0000007,
            RPC_INSTALL_ERROR = 0xF0000008,
            RPC_CALL_ERROR = 0xF0000009,
            RPC_ELF_ERROR = 0xF000000A,
        };

        /** messages **/
        private static Dictionary<RPC_STATUS, string> StatusMessages = new Dictionary<RPC_STATUS, string>()
        {
            { RPC_STATUS.RPC_SUCCESS, "success"},
            { RPC_STATUS.RPC_TOO_MUCH_DATA, "too much data"},
            { RPC_STATUS.RPC_READ_ERROR, "read error"},
            { RPC_STATUS.RPC_WRITE_ERROR, "write error"},
            { RPC_STATUS.RPC_LIST_ERROR, "process list error"},
            { RPC_STATUS.RPC_INFO_ERROR, "process information error"},
            { RPC_STATUS.RPC_NO_PROC, "no such process error"},
            { RPC_STATUS.RPC_INSTALL_ERROR, "could not install rpc" },
            { RPC_STATUS.RPC_CALL_ERROR, "could not call address" },
            { RPC_STATUS.RPC_ELF_ERROR, "could not map elf" }
        };
        private const string NotConnectedErrorMessage = "librpc: not connected";
        private const string TooManyArgumentsErrorMessage = "librpc: too many call arguments";
		
        /// <summary>
        /// Initializes PS4RPC class
        /// </summary>
        /// <param name="addr">PlayStation 4 address</param>
        public PS4RPC(IPAddress addr)
        {
            enp = new IPEndPoint(addr, RPC_PORT);
            sock = new Socket(enp.AddressFamily, SocketType.Stream, ProtocolType.Tcp){NoDelay = true, ReceiveTimeout = 5* 1000, SendTimeout = 5 * 1000};
        }

        /// <summary>
        /// Initializes PS4RPC class
        /// </summary>
        /// <param name="ip">PlayStation 4 ip address</param>
        public PS4RPC(string ip)
        {
            IPAddress addr;
            try
            {
                addr = IPAddress.Parse(ip);
            }
            catch (FormatException ex)
            {
                throw ex;
            }

            enp = new IPEndPoint(addr, RPC_PORT);
            sock = new Socket(enp.AddressFamily, SocketType.Stream, ProtocolType.Tcp){NoDelay = true, ReceiveTimeout = 5* 1000, SendTimeout = 5 * 1000};
        }

        private static string GetNullTermString(byte[] data, int offset)
        {
            int length = Array.IndexOf<byte>(data, 0, offset) - offset;
            if (length < 0)
            {
                length = data.Length - offset;
            }

            return Encoding.ASCII.GetString(data, offset, length);
        }

        private static byte[] SubArray(byte[] data, int offset, int length)
        {
            byte[] bytes = new byte[length];
            Buffer.BlockCopy(data, offset, bytes, 0, length);
            return bytes;
        }
        private static object GetObjectFromBytes(byte[] buffer, Type type)
        {
            int size = Marshal.SizeOf(type);

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(buffer, 0, ptr, size);
            object r = Marshal.PtrToStructure(ptr, type);

            Marshal.FreeHGlobal(ptr);

            return r;
        }

        private static byte[] GetBytesFromObject(object obj)
        {
            int size = Marshal.SizeOf(obj);

            byte[] bytes = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(obj, ptr, false);
            Marshal.Copy(ptr, bytes, 0, size);

            Marshal.FreeHGlobal(ptr);

            return bytes;
        }

        private static bool IsFatalStatus(RPC_STATUS status)
        {
            // if status first nibble starts with F
            return (uint)status >> 28 == 15;
        }

        /// <summary>
        /// Connects to PlayStation 4
        /// </summary>
        public void Connect()
        {
            if (!IsConnected)
            {
                sock.Connect(enp);
                IsConnected = true;
            }
        }

        /// <summary>
        /// Disconnects from PlayStation 4
        /// </summary>
        public void Disconnect()
        {
            SendCMDPacket(RPC_CMDS.RPC_END, 0);
            sock.Close();
            IsConnected = false;
        }

        private void SendPacketData(int length, params object[] fields)
        {
            MemoryStream rs = new MemoryStream();
            foreach (object field in fields)
            {
                byte[] bytes = null;

				switch (field)
				{
					case char _:
						bytes = BitConverter.GetBytes((char)field);
						break;
	
					case byte _:
						bytes = BitConverter.GetBytes((byte)field);
						break;
	
					case short _:
						bytes = BitConverter.GetBytes((short)field);
						break;
	
					case ushort _:
						bytes = BitConverter.GetBytes((ushort)field);
						break;
	
					case int _:
						bytes = BitConverter.GetBytes((int)field);
						break;
	
					case uint _:
						bytes = BitConverter.GetBytes((uint)field);
						break;
	
					case long _:
						bytes = BitConverter.GetBytes((long)field);
						break;
	
					case ulong _:
						bytes = BitConverter.GetBytes((ulong)field);
						break;
	
					case byte[] _:
						bytes = (byte[])field;
						break;
				}

               if (bytes != null) rs.Write(bytes, 0, bytes.Length);
            }

            SendData(rs.ToArray(), length);
            rs.Dispose();
        }

        private void SendCMDPacket(RPC_CMDS cmd, int length)
        {
            SendPacketData(RPC_PACKET_SIZE, RPC_PACKET_MAGIC, (uint)cmd, length);
        }

        private RPC_STATUS ReceiveRPCStatus()
        {
            byte[] status = new byte[4];
            sock.Receive(status, 4, SocketFlags.None);
            return (RPC_STATUS)BitConverter.ToUInt32(status, 0);
        }

        private RPC_STATUS CheckRPCStatus()
        {
            RPC_STATUS status = ReceiveRPCStatus();
            if (IsFatalStatus(status))
            {
                StatusMessages.TryGetValue(status, out string value);
                throw new Exception($"librpc: {value}");
            }

            return status;
        }

        private void SendData(byte[] data, int length)
        {
            int left = length;
            int offset = 0;
			int sent;
            while (left > 0)
            {				
                if (left > RPC_MAX_DATA_LEN)
                {
                    byte[] bytes = SubArray(data, offset, RPC_MAX_DATA_LEN);
                    sent = sock.Send(bytes, RPC_MAX_DATA_LEN, SocketFlags.None);
                    offset += sent;
                    left -= sent;
                }
                else
                {
                    byte[] bytes = SubArray(data, offset, left);
                    sent = sock.Send(bytes, left, SocketFlags.None);
                    offset += sent;
                    left -= sent;
                }
            }
        }

        private byte[] ReceiveData(int length)
        {
            MemoryStream s = new MemoryStream();

            int left = length;
            while (left > 0)
            {
                byte[] b = new byte[RPC_MAX_DATA_LEN];
                int recv = sock.Receive(b, RPC_MAX_DATA_LEN, SocketFlags.None);
                s.Write(b, 0, recv);
                left -= recv;
            }

            byte[] data = s.ToArray();

            s.Dispose();

            return data;
        }

        /// <summary>
        /// Read memory
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="address">Memory address</param>
        /// <param name="length">Data length</param>
        /// <returns></returns>
        public byte[] ReadMemory(int pid, ulong address, int length)
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            SendCMDPacket(RPC_CMDS.RPC_PROC_READ, RPC_PROC_READ_SIZE);
            SendPacketData(RPC_PROC_READ_SIZE, pid, address, length);
            CheckRPCStatus();
            return ReceiveData(length);
        }

        /// <summary>
        /// Write memory
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="address">Memory address</param>
        /// <param name="data">Data</param>
        public void WriteMemory(int pid, ulong address, byte[] data)
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            if (data.Length > RPC_MAX_DATA_LEN)
            {
                // write RPC_MAX_DATA_LEN
                byte[] nowdata = SubArray(data, 0, RPC_MAX_DATA_LEN);

                SendCMDPacket(RPC_CMDS.RPC_PROC_WRITE, RPC_PROC_WRITE_SIZE);
                SendPacketData(RPC_PROC_WRITE_SIZE, pid, address, RPC_MAX_DATA_LEN);
                CheckRPCStatus();
                SendData(nowdata, RPC_MAX_DATA_LEN);
                CheckRPCStatus();

                // call WriteMemory again with rest of it
                int nextlength = data.Length - RPC_MAX_DATA_LEN;
                ulong nextaddr = address + RPC_MAX_DATA_LEN;
                byte[] nextdata = SubArray(data, RPC_MAX_DATA_LEN, nextlength);
                WriteMemory(pid, nextaddr, nextdata);
            }
            else if (data.Length > 0)
            {
                SendCMDPacket(RPC_CMDS.RPC_PROC_WRITE, RPC_PROC_WRITE_SIZE);
                SendPacketData(RPC_PROC_WRITE_SIZE, pid, address, data.Length);
                CheckRPCStatus();
                SendData(data, data.Length);
                CheckRPCStatus();
            }
        }

        /// <summary>
        /// Get kernel base address
        /// </summary>
        /// <returns></returns>
        public ulong KernelBase()
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            SendCMDPacket(RPC_CMDS.RPC_KERN_BASE, 0);
            CheckRPCStatus();
            return BitConverter.ToUInt64(ReceiveData(RPC_KERN_BASE_SIZE), 0);

        }

        /// <summary>
        /// Read memory from kernel
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <param name="length">Data length</param>
        /// <returns></returns>
        public byte[] KernelReadMemory(ulong address, int length)
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            SendCMDPacket(RPC_CMDS.RPC_KERN_READ, RPC_KERN_READ_SIZE);
            SendPacketData(RPC_KERN_READ_SIZE, address, length);
            CheckRPCStatus();
            return ReceiveData(length);
        }

        /// <summary>
        /// Write memory in kernel
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <param name="data">Data</param>
        public void KernelWriteMemory(ulong address, byte[] data)
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            if (data.Length > RPC_MAX_DATA_LEN)
            {
                // write RPC_MAX_DATA_LEN
                byte[] nowdata = SubArray(data, 0, RPC_MAX_DATA_LEN);

                SendCMDPacket(RPC_CMDS.RPC_KERN_WRITE, RPC_KERN_WRITE_SIZE);
                SendPacketData(RPC_KERN_WRITE_SIZE, address, RPC_MAX_DATA_LEN);
                CheckRPCStatus();
                SendData(nowdata, RPC_MAX_DATA_LEN);
                CheckRPCStatus();

                // call WriteMemory again with rest of it
                int nextlength = data.Length - RPC_MAX_DATA_LEN;
                ulong nextaddr = address + RPC_MAX_DATA_LEN;
                byte[] nextdata = SubArray(data, RPC_MAX_DATA_LEN, nextlength);
                KernelWriteMemory(nextaddr, nextdata);
            }
            else if (data.Length > 0)
            {
                SendCMDPacket(RPC_CMDS.RPC_KERN_WRITE, RPC_KERN_WRITE_SIZE);
                SendPacketData(RPC_KERN_WRITE_SIZE, address, data.Length);
                CheckRPCStatus();
                SendData(data, data.Length);
                CheckRPCStatus();
            }
        }

        /// <summary>
        /// Get current process list
        /// </summary>
        /// <returns></returns>
        public ProcessList GetProcessList()
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            SendCMDPacket(RPC_CMDS.RPC_PROC_LIST, 0);
            CheckRPCStatus();

            // recv count
            byte[] bnumber = new byte[4];
            sock.Receive(bnumber, 4, SocketFlags.None);
            int number = BitConverter.ToInt32(bnumber, 0);

            // recv data
            byte[] data = ReceiveData(number * RPC_PROC_LIST_SIZE);

            // parse data
            string[] procnames = new string[number];
            int[] pids = new int[number];
            for (int i = 0; i < number; i++)
            {
                int offset = i * RPC_PROC_LIST_SIZE;
                procnames[i] = GetNullTermString(data, offset);
                pids[i] = BitConverter.ToInt32(data, offset + 32);
            }

            return new ProcessList(number, procnames, pids);
        }

        /// <summary>
        /// Get process information (memory map)
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <returns></returns>
        public ProcessInfo GetProcessInfo(int pid)
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            SendCMDPacket(RPC_CMDS.RPC_PROC_INFO, RPC_PROC_INFO1_SIZE);
            SendPacketData(RPC_PROC_INFO1_SIZE, pid);

            RPC_STATUS status = CheckRPCStatus();
            if (status == RPC_STATUS.RPC_INFO_NO_MAP)
            {
                return new ProcessInfo(pid, null);
            }

            // recv count
            byte[] bnumber = new byte[4];
            sock.Receive(bnumber, 4, SocketFlags.None);
            int number = BitConverter.ToInt32(bnumber, 0);

            // recv data
            byte[] data = ReceiveData(number * RPC_PROC_INFO2_SIZE);

            // parse data
            MemoryEntry[] entries = new MemoryEntry[number];
            for (int i = 0; i < number; i++)
            {
                int offset = i * RPC_PROC_INFO2_SIZE;
                entries[i] = new MemoryEntry
				{
                    name = GetNullTermString(data, offset),
                    start = BitConverter.ToUInt64(data, offset + 32),
                    end = BitConverter.ToUInt64(data, offset + 40),
                    offset = BitConverter.ToUInt64(data, offset + 48),
                    prot = BitConverter.ToUInt32(data, offset + 56)                
				};
            }

            return new ProcessInfo(pid, entries);
        }

        /// <summary>
        /// Install RPC into a process, this returns a stub address that you should pass into call functions
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <returns></returns>
        public ulong InstallRPC(int pid)
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            SendCMDPacket(RPC_CMDS.RPC_PROC_INTALL, RPC_PROC_INSTALL1_SIZE);
            SendPacketData(RPC_PROC_INSTALL1_SIZE, pid);
            CheckRPCStatus();
            byte[] data = ReceiveData(RPC_PROC_INSTALL2_SIZE);
            return BitConverter.ToUInt64(data, 4);
        }

        /// <summary>
        /// Call function (returns rax)
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="rpcstub">Stub address from InstallRPC</param>
        /// <param name="address">Address to call</param>
        /// <param name="args">Arguments array</param>
        /// <returns></returns>
        public ulong Call(int pid, ulong rpcstub, ulong address, params object[] args)
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            SendCMDPacket(RPC_CMDS.RPC_PROC_CALL, RPC_PROC_CALL1_SIZE);

            MemoryStream rs = new MemoryStream();
            rs.Write(BitConverter.GetBytes(pid), 0, sizeof(int));
            rs.Write(BitConverter.GetBytes(rpcstub), 0, sizeof(ulong));
            rs.Write(BitConverter.GetBytes(address), 0, sizeof(ulong));

            int num = 0;
            foreach (object arg in args)
            {
                byte[] bytes = new byte[8];

				switch (arg)
				{
				    case char _:
				    {
				        byte[] tmp = BitConverter.GetBytes((char) arg);
				        Buffer.BlockCopy(tmp, 0, bytes, 0, sizeof(char));

				        byte[] pad = new byte[sizeof(ulong) - sizeof(char)];
				        Buffer.BlockCopy(pad, 0, bytes, sizeof(char), pad.Length);
				        break;
				    }
				    case byte _:
				    {
				        byte[] tmp = BitConverter.GetBytes((byte) arg);
				        Buffer.BlockCopy(tmp, 0, bytes, 0, sizeof(byte));

				        byte[] pad = new byte[sizeof(ulong) - sizeof(byte)];
				        Buffer.BlockCopy(pad, 0, bytes, sizeof(byte), pad.Length);
				        break;
				    }
				    case short _:
				    {
				        byte[] tmp = BitConverter.GetBytes((short) arg);
				        Buffer.BlockCopy(tmp, 0, bytes, 0, sizeof(short));

				        byte[] pad = new byte[sizeof(ulong) - sizeof(short)];
				        Buffer.BlockCopy(pad, 0, bytes, sizeof(short), pad.Length);
				        break;
				    }
				    case ushort _:
				    {
				        byte[] tmp = BitConverter.GetBytes((ushort) arg);
				        Buffer.BlockCopy(tmp, 0, bytes, 0, sizeof(ushort));

				        byte[] pad = new byte[sizeof(ulong) - sizeof(ushort)];
				        Buffer.BlockCopy(pad, 0, bytes, sizeof(ushort), pad.Length);
				        break;
				    }
				    case int _:
				    {
				        byte[] tmp = BitConverter.GetBytes((int) arg);
				        Buffer.BlockCopy(tmp, 0, bytes, 0, sizeof(int));

				        byte[] pad = new byte[sizeof(ulong) - sizeof(int)];
				        Buffer.BlockCopy(pad, 0, bytes, sizeof(int), pad.Length);
				        break;
				    }
				    case uint _:
				    {
				        byte[] tmp = BitConverter.GetBytes((uint) arg);
				        Buffer.BlockCopy(tmp, 0, bytes, 0, sizeof(uint));

				        byte[] pad = new byte[sizeof(ulong) - sizeof(uint)];
				        Buffer.BlockCopy(pad, 0, bytes, sizeof(uint), pad.Length);
				        break;
				    }
				    case long _:
				    {
				        byte[] tmp = BitConverter.GetBytes((long) arg);
				        Buffer.BlockCopy(tmp, 0, bytes, 0, sizeof(long));
				        break;
				    }
				    case ulong _:
				    {
				        byte[] tmp = BitConverter.GetBytes((ulong) arg);
				        Buffer.BlockCopy(tmp, 0, bytes, 0, sizeof(ulong));
				        break;
				    }
				}

                rs.Write(bytes, 0, bytes.Length);
                num++;
            }

            if (num > 6)
            {
                throw new Exception(TooManyArgumentsErrorMessage);
            }
            if (num < 6)
            {
                for (int i = 0; i < (6 - num); i++)
                {
                    rs.Write(BitConverter.GetBytes((ulong)0), 0, sizeof(ulong));
                }
            }

            SendData(rs.ToArray(), RPC_PROC_CALL1_SIZE);
            rs.Dispose();

            CheckRPCStatus();

            byte[] data = ReceiveData(RPC_PROC_CALL2_SIZE);
            return BitConverter.ToUInt64(data, 4);
        }

        /// <summary>
        /// Load an elf into a process
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="elf">Elf bytes</param>
        public void LoadElf(int pid, byte[] elf)
        {
            SendCMDPacket(RPC_CMDS.RPC_PROC_ELF, RPC_PROC_ELF_SIZE);
            SendPacketData(RPC_PROC_ELF_SIZE, pid, elf.Length);
            SendData(elf, elf.Length);
            CheckRPCStatus();
        }

        /// <summary>
        /// Load an elf into a process
        /// </summary>
        /// <param name="pid">Process ID</param>
        /// <param name="filename">Elf file path</param>
        public void LoadElf(int pid, string filename)
        {
            LoadElf(pid, File.ReadAllBytes(filename));
        }

        /// <summary>
        /// Reboot console
        /// </summary>
        public void Reboot()
        {
            if (!IsConnected)
            {
                throw new Exception(NotConnectedErrorMessage);
            }

            SendCMDPacket(RPC_CMDS.RPC_REBOOT, 0);
            sock.Close();
            IsConnected = false;
        }

        public T ReadMemory<T>(int pid, ulong address)
        {
            if (typeof(T) == typeof(string))
            {
                string str = "";
                ulong i = 0;

                while (true)
                {
                    byte value = ReadMemory(pid, address + i, sizeof(byte))[0];
                    if (value == 0)
                    {
                        break;
                    }
                    str += Convert.ToChar(value);
                    i++;
                }

                return (T)(object)str;
            }

            if (typeof(T) == typeof(byte[]))
            {
                throw new NotSupportedException("byte arrays are not supported, use ReadMemory(int pid, ulong address, int size)");
            }

            return (T)GetObjectFromBytes(ReadMemory(pid, address, Marshal.SizeOf(typeof(T))), typeof(T));
        }

        public void WriteMemory<T>(int pid, ulong address, T value)
        {
            if (typeof(T) == typeof(string))
            {
                WriteMemory(pid, address, Encoding.ASCII.GetBytes((string)(object)value + (char)0x0));
                return;
            }

            if (typeof(T) == typeof(byte[]))
            {
                WriteMemory(pid, address, (byte[])(object)value);
                return;
            }

            WriteMemory(pid, address, GetBytesFromObject(value));
        }
    }
}
