using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace libdebug
{
	public class PS4DBG
	{
		public enum CMDS : uint
		{
			CMD_VERSION = 3170893825u,
			CMD_PROC_LIST = 3182034945u,
			CMD_PROC_READ = 3182034946u,
			CMD_PROC_WRITE = 3182034947u,
			CMD_PROC_MAPS = 3182034948u,
			CMD_PROC_INTALL = 3182034949u,
			CMD_PROC_CALL = 3182034950u,
			CMD_PROC_ELF = 3182034951u,
			CMD_PROC_PROTECT = 3182034952u,
			CMD_PROC_SCAN = 3182034953u,
			CMD_PROC_INFO = 3182034954u,
			CMD_PROC_ALLOC = 3182034955u,
			CMD_PROC_FREE = 3182034956u,
			CMD_DEBUG_ATTACH = 3183149057u,
			CMD_DEBUG_DETACH = 3183149058u,
			CMD_DEBUG_BREAKPT = 3183149059u,
			CMD_DEBUG_WATCHPT = 3183149060u,
			CMD_DEBUG_THREADS = 3183149061u,
			CMD_DEBUG_STOPTHR = 3183149062u,
			CMD_DEBUG_RESUMETHR = 3183149063u,
			CMD_DEBUG_GETREGS = 3183149064u,
			CMD_DEBUG_SETREGS = 3183149065u,
			CMD_DEBUG_GETFPREGS = 3183149066u,
			CMD_DEBUG_SETFPREGS = 3183149067u,
			CMD_DEBUG_GETDBGREGS = 3183149068u,
			CMD_DEBUG_SETDBGREGS = 3183149069u,
			CMD_DEBUG_STOPGO = 3183149072u,
			CMD_DEBUG_THRINFO = 3183149073u,
			CMD_DEBUG_SINGLESTEP = 3183149074u,
			CMD_KERN_BASE = 3184263169u,
			CMD_KERN_READ = 3184263170u,
			CMD_KERN_WRITE = 3184263171u,
			CMD_CONSOLE_REBOOT = 3185377281u,
			CMD_CONSOLE_END = 3185377282u,
			CMD_CONSOLE_PRINT = 3185377283u,
			CMD_CONSOLE_NOTIFY = 3185377284u,
			CMD_CONSOLE_INFO = 3185377285u
		}

		public enum CMD_STATUS : uint
		{
			CMD_SUCCESS = 0x80000000,
			CMD_ERROR = 4026531841u,
			CMD_TOO_MUCH_DATA = 4026531842u,
			CMD_DATA_NULL = 4026531843u,
			CMD_ALREADY_DEBUG = 4026531844u,
			CMD_INVALID_INDEX = 4026531845u
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct CMDPacket
		{
			public uint magic;

			public uint cmd;

			public uint datalen;
		}

		public enum VM_PROTECTIONS : uint
		{
			VM_PROT_NONE = 0u,
			VM_PROT_READ = 1u,
			VM_PROT_WRITE = 2u,
			VM_PROT_EXECUTE = 4u,
			VM_PROT_DEFAULT = 3u,
			VM_PROT_ALL = 7u,
			VM_PROT_NO_CHANGE = 8u,
			VM_PROT_COPY = 0x10,
			VM_PROT_WANTS_COPY = 0x10
		}

		public enum WATCHPT_LENGTH : uint
		{
			DBREG_DR7_LEN_1 = 0u,
			DBREG_DR7_LEN_2 = 1u,
			DBREG_DR7_LEN_4 = 3u,
			DBREG_DR7_LEN_8 = 2u
		}

		public enum WATCHPT_BREAKTYPE : uint
		{
			DBREG_DR7_EXEC = 0u,
			DBREG_DR7_WRONLY = 1u,
			DBREG_DR7_RDWR = 3u
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct DebuggerInterruptPacket
		{
			public uint lwpid;

			public uint status;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
			public string tdname;

			public regs reg64;

			public fpregs savefpu;

			public dbregs dbreg64;
		}

		public delegate void DebuggerInterruptCallback(uint lwpid, uint status, string tdname, regs regs, fpregs fpregs, dbregs dbregs);

		public enum ScanValueType : byte
		{
			valTypeUInt8,
			valTypeInt8,
			valTypeUInt16,
			valTypeInt16,
			valTypeUInt32,
			valTypeInt32,
			valTypeUInt64,
			valTypeInt64,
			valTypeFloat,
			valTypeDouble,
			valTypeArrBytes,
			valTypeString
		}

		public enum ScanCompareType : byte
		{
			ExactValue,
			FuzzyValue,
			BiggerThan,
			SmallerThan,
			ValueBetween,
			IncreasedValue,
			IncreasedValueBy,
			DecreasedValue,
			DecreasedValueBy,
			ChangedValue,
			UnchangedValue,
			UnknownInitialValue
		}

		private const int CMD_CONSOLE_PRINT_PACKET_SIZE = 4;

		private const int CMD_CONSOLE_NOTIFY_PACKET_SIZE = 8;

		private Socket sock;

		private IPEndPoint enp;

		private Thread debugThread;

		private const string LIBRARY_VERSION = "1.2";

		private const int PS4DBG_PORT = 744;

		private const int PS4DBG_DEBUG_PORT = 755;

		private const int NET_MAX_LENGTH = 8192;

		private const int BROADCAST_PORT = 1010;

		private const uint BROADCAST_MAGIC = 4294945450u;

		private const uint CMD_PACKET_MAGIC = 4289379276u;

		public static uint MAX_BREAKPOINTS = 10u;

		public static uint MAX_WATCHPOINTS = 4u;

		private const int CMD_PACKET_SIZE = 12;

		private const int CMD_DEBUG_ATTACH_PACKET_SIZE = 4;

		private const int CMD_DEBUG_BREAKPT_PACKET_SIZE = 16;

		private const int CMD_DEBUG_WATCHPT_PACKET_SIZE = 24;

		private const int CMD_DEBUG_STOPTHR_PACKET_SIZE = 4;

		private const int CMD_DEBUG_RESUMETHR_PACKET_SIZE = 4;

		private const int CMD_DEBUG_GETREGS_PACKET_SIZE = 4;

		private const int CMD_DEBUG_SETREGS_PACKET_SIZE = 8;

		private const int CMD_DEBUG_STOPGO_PACKET_SIZE = 4;

		private const int CMD_DEBUG_THRINFO_PACKET_SIZE = 4;

		private const int DEBUG_INTERRUPT_SIZE = 1184;

		private const int DEBUG_THRINFO_SIZE = 40;

		private const int DEBUG_REGS_SIZE = 176;

		private const int DEBUG_FPREGS_SIZE = 832;

		private const int DEBUG_DBGREGS_SIZE = 128;

		private const int CMD_KERN_READ_PACKET_SIZE = 12;

		private const int CMD_KERN_WRITE_PACKET_SIZE = 12;

		private const int KERN_BASE_SIZE = 8;

		private const int CMD_PROC_READ_PACKET_SIZE = 16;

		private const int CMD_PROC_WRITE_PACKET_SIZE = 16;

		private const int CMD_PROC_MAPS_PACKET_SIZE = 4;

		private const int CMD_PROC_INSTALL_PACKET_SIZE = 4;

		private const int CMD_PROC_CALL_PACKET_SIZE = 68;

		private const int CMD_PROC_ELF_PACKET_SIZE = 8;

		private const int CMD_PROC_PROTECT_PACKET_SIZE = 20;

		private const int CMD_PROC_SCAN_PACKET_SIZE = 10;

		private const int CMD_PROC_INFO_PACKET_SIZE = 4;

		private const int CMD_PROC_ALLOC_PACKET_SIZE = 8;

		private const int CMD_PROC_FREE_PACKET_SIZE = 16;

		private const int PROC_LIST_ENTRY_SIZE = 36;

		private const int PROC_MAP_ENTRY_SIZE = 58;

		private const int PROC_INSTALL_SIZE = 8;

		private const int PROC_CALL_SIZE = 12;

		private const int PROC_PROC_INFO_SIZE = 188;

		private const int PROC_ALLOC_SIZE = 8;

		public bool IsConnected
		{
			get;
			private set;
		}

		public bool IsDebugging
		{
			get;
			private set;
		}

		public void Reboot()
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_CONSOLE_REBOOT, 0);
			IsConnected = false;
		}

		public void Print(string str)
		{
			CheckConnected();
			string text = str + "\0";
			SendCMDPacket(CMDS.CMD_CONSOLE_PRINT, 4, text.Length);
			SendData(Encoding.ASCII.GetBytes(text), text.Length);
			CheckStatus();
		}

		public void Notify(int messageType, string message)
		{
			CheckConnected();
			string text = message + "\0";
			SendCMDPacket(CMDS.CMD_CONSOLE_NOTIFY, 8, messageType, text.Length);
			SendData(Encoding.ASCII.GetBytes(text), text.Length);
			CheckStatus();
		}

		public void GetConsoleInformation()
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_CONSOLE_INFO, 0);
			CheckStatus();
		}

		private static string ConvertASCII(byte[] data, int offset)
		{
			int num = Array.IndexOf(data, (byte)0, offset) - offset;
			if (num < 0)
			{
				num = data.Length - offset;
			}
			return Encoding.ASCII.GetString(data, offset, num);
		}

		private static byte[] SubArray(byte[] data, int offset, int length)
		{
			byte[] array = new byte[length];
			Buffer.BlockCopy(data, offset, array, 0, length);
			return array;
		}

		private static object GetObjectFromBytes(byte[] buffer, Type type)
		{
			int num = Marshal.SizeOf(type);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(buffer, 0, intPtr, num);
			object result = Marshal.PtrToStructure(intPtr, type);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		private static byte[] GetBytesFromObject(object obj)
		{
			int num = Marshal.SizeOf(obj);
			byte[] array = new byte[num];
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(obj, intPtr, fDeleteOld: false);
			Marshal.Copy(intPtr, array, 0, num);
			Marshal.FreeHGlobal(intPtr);
			return array;
		}

		private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
		{
			byte[] addressBytes = address.GetAddressBytes();
			byte[] addressBytes2 = subnetMask.GetAddressBytes();
			byte[] array = new byte[addressBytes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)(addressBytes[i] | (addressBytes2[i] ^ 0xFF));
			}
			return new IPAddress(array);
		}

		private void SendCMDPacket(CMDS cmd, int length, params object[] fields)
		{
			CMDPacket cMDPacket = default(CMDPacket);
			cMDPacket.magic = 4289379276u;
			cMDPacket.cmd = (uint)cmd;
			cMDPacket.datalen = (uint)length;
			CMDPacket cMDPacket2 = cMDPacket;
			byte[] array = null;
			if (length > 0)
			{
				MemoryStream memoryStream = new MemoryStream();
				foreach (object obj in fields)
				{
					byte[] array2 = null;
					object obj2 = obj;
					if (obj2 != null)
					{
						object obj3;
						byte[] array3;
						if ((obj3 = obj2) is char)
						{
							char value = (char)obj3;
							array2 = BitConverter.GetBytes(value);
						}
						else if ((obj3 = obj2) is byte)
						{
							byte value2 = (byte)obj3;
							array2 = BitConverter.GetBytes(value2);
						}
						else if ((obj3 = obj2) is short)
						{
							short value3 = (short)obj3;
							array2 = BitConverter.GetBytes(value3);
						}
						else if ((obj3 = obj2) is ushort)
						{
							ushort value4 = (ushort)obj3;
							array2 = BitConverter.GetBytes(value4);
						}
						else if ((obj3 = obj2) is int)
						{
							int value5 = (int)obj3;
							array2 = BitConverter.GetBytes(value5);
						}
						else if ((obj3 = obj2) is uint)
						{
							uint value6 = (uint)obj3;
							array2 = BitConverter.GetBytes(value6);
						}
						else if ((obj3 = obj2) is long)
						{
							long value7 = (long)obj3;
							array2 = BitConverter.GetBytes(value7);
						}
						else if ((obj3 = obj2) is ulong)
						{
							ulong value8 = (ulong)obj3;
							array2 = BitConverter.GetBytes(value8);
						}
						else if ((array3 = (obj2 as byte[])) != null)
						{
							array2 = array3;
						}
					}
					if (array2 != null)
					{
						memoryStream.Write(array2, 0, array2.Length);
					}
				}
				array = memoryStream.ToArray();
				memoryStream.Dispose();
			}
			SendData(GetBytesFromObject(cMDPacket2), 12);
			if (array != null)
			{
				SendData(array, length);
			}
		}

		private void SendData(byte[] data, int length)
		{
			int num = length;
			int num2 = 0;
			int num3 = 0;
			while (num > 0)
			{
				if (num > 8192)
				{
					byte[] buffer = SubArray(data, num2, 8192);
					num3 = sock.Send(buffer, 8192, SocketFlags.None);
				}
				else
				{
					byte[] buffer2 = SubArray(data, num2, num);
					num3 = sock.Send(buffer2, num, SocketFlags.None);
				}
				num2 += num3;
				num -= num3;
			}
		}

		private byte[] ReceiveData(int length)
		{
			MemoryStream memoryStream = new MemoryStream();
			int num = length;
			int num2 = 0;
			while (num > 0)
			{
				byte[] buffer = new byte[8192];
				num2 = sock.Receive(buffer, 8192, SocketFlags.None);
				memoryStream.Write(buffer, 0, num2);
				num -= num2;
			}
			byte[] result = memoryStream.ToArray();
			memoryStream.Dispose();
			GC.Collect();
			return result;
		}

		private CMD_STATUS ReceiveStatus()
		{
			byte[] array = new byte[4];
			sock.Receive(array, 4, SocketFlags.None);
			return (CMD_STATUS)BitConverter.ToUInt32(array, 0);
		}

		private void CheckStatus()
		{
			CMD_STATUS cMD_STATUS = ReceiveStatus();
			if (cMD_STATUS != CMD_STATUS.CMD_SUCCESS)
			{
				uint num = (uint)cMD_STATUS;
				throw new Exception("libdbg status " + num.ToString("X"));
			}
		}

		private void CheckConnected()
		{
			if (!IsConnected)
			{
				throw new Exception("libdbg: not connected");
			}
		}

		private void CheckDebugging()
		{
			if (!IsDebugging)
			{
				throw new Exception("libdbg: not debugging");
			}
		}

		public PS4DBG(IPAddress addr)
		{
			enp = new IPEndPoint(addr, 744);
			sock = new Socket(enp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		}

		public PS4DBG(string ip)
		{
			IPAddress iPAddress = null;
			try
			{
				iPAddress = IPAddress.Parse(ip);
			}
			catch (FormatException ex)
			{
				throw ex;
			}
			enp = new IPEndPoint(iPAddress, 744);
			sock = new Socket(enp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
		}

		public static string FindPlayStation()
		{
			UdpClient udpClient = new UdpClient();
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
			udpClient.EnableBroadcast = true;
			udpClient.Client.ReceiveTimeout = 4000;
			byte[] bytes = BitConverter.GetBytes(4294945450u);
			IPAddress iPAddress = null;
			IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			foreach (IPAddress iPAddress2 in addressList)
			{
				if (iPAddress2.AddressFamily == AddressFamily.InterNetwork)
				{
					iPAddress = iPAddress2;
				}
			}
			if (iPAddress == null)
			{
				throw new Exception("libdbg broadcast error: could not get host ip");
			}
			udpClient.Send(bytes, bytes.Length, new IPEndPoint(GetBroadcastAddress(iPAddress, IPAddress.Parse("255.255.255.0")), 1010));
			if (BitConverter.ToUInt32(udpClient.Receive(ref remoteEP), 0) != 4294945450u)
			{
				throw new Exception("libdbg broadcast error: wrong magic on udp server");
			}
			return remoteEP.Address.ToString();
		}

		public void Connect()
		{
			if (!IsConnected)
			{
				sock.NoDelay = true;
				sock.ReceiveBufferSize = 8192;
				sock.SendBufferSize = 8192;
				sock.ReceiveTimeout = 10000;
				sock.Connect(enp);
				IsConnected = true;
			}
		}

		public void Disconnect()
		{
			SendCMDPacket(CMDS.CMD_CONSOLE_END, 0);
			sock.Shutdown(SocketShutdown.Both);
			sock.Close();
			IsConnected = false;
		}

		public string GetLibraryDebugVersion()
		{
			return "1.2";
		}

		public string GetConsoleDebugVersion()
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_VERSION, 0);
			byte[] array = new byte[4];
			sock.Receive(array, 4, SocketFlags.None);
			int num = BitConverter.ToInt32(array, 0);
			byte[] array2 = new byte[num];
			sock.Receive(array2, num, SocketFlags.None);
			return ConvertASCII(array2, 0);
		}

		private void DebuggerThread(object obj)
		{
			DebuggerInterruptCallback debuggerInterruptCallback = (DebuggerInterruptCallback)obj;
			IPAddress iPAddress = IPAddress.Parse("0.0.0.0");
			IPEndPoint localEP = new IPEndPoint(iPAddress, 755);
			Socket socket = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket.Bind(localEP);
			socket.Listen(0);
			IsDebugging = true;
			Socket socket2 = socket.Accept();
			socket2.NoDelay = true;
			socket2.Blocking = false;
			while (IsDebugging)
			{
				if (socket2.Available == 1184)
				{
					byte[] buffer = new byte[1184];
					if (socket2.Receive(buffer, 1184, SocketFlags.None) == 1184)
					{
						DebuggerInterruptPacket debuggerInterruptPacket = (DebuggerInterruptPacket)GetObjectFromBytes(buffer, typeof(DebuggerInterruptPacket));
						debuggerInterruptCallback(debuggerInterruptPacket.lwpid, debuggerInterruptPacket.status, debuggerInterruptPacket.tdname, debuggerInterruptPacket.reg64, debuggerInterruptPacket.savefpu, debuggerInterruptPacket.dbreg64);
					}
				}
				Thread.Sleep(100);
			}
			socket.Close();
		}

		public void AttachDebugger(int pid, DebuggerInterruptCallback callback)
		{
			CheckConnected();
			if (IsDebugging || debugThread != null)
			{
				throw new Exception("libdbg: debugger already running?");
			}
			IsDebugging = false;
			debugThread = new Thread(DebuggerThread);
			debugThread.Start(callback);
			while (!IsDebugging)
			{
				Thread.Sleep(100);
			}
			SendCMDPacket(CMDS.CMD_DEBUG_ATTACH, 4, pid);
			CheckStatus();
		}

		public void DetachDebugger()
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_DEBUG_DETACH, 0);
			CheckStatus();
			if (IsDebugging && debugThread != null)
			{
				IsDebugging = false;
				debugThread.Join();
				debugThread = null;
			}
		}

		public void ProcessStop()
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_STOPGO, 4, 1);
			CheckStatus();
		}

		public void ProcessKill()
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_STOPGO, 4, 2);
			CheckStatus();
		}

		public void ProcessResume()
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_STOPGO, 4, 0);
			CheckStatus();
		}

		public void ChangeBreakpoint(int index, bool enabled, ulong address)
		{
			CheckConnected();
			CheckDebugging();
			if (index >= MAX_BREAKPOINTS)
			{
				throw new Exception("libdbg: breakpoint index out of range");
			}
			SendCMDPacket(CMDS.CMD_DEBUG_BREAKPT, 16, index, Convert.ToInt32(enabled), address);
			CheckStatus();
		}

		public void ChangeWatchpoint(int index, bool enabled, WATCHPT_LENGTH length, WATCHPT_BREAKTYPE breaktype, ulong address)
		{
			CheckConnected();
			CheckDebugging();
			if (index >= MAX_WATCHPOINTS)
			{
				throw new Exception("libdbg: watchpoint index out of range");
			}
			SendCMDPacket(CMDS.CMD_DEBUG_WATCHPT, 24, index, Convert.ToInt32(enabled), (uint)length, (uint)breaktype, address);
			CheckStatus();
		}

		public uint[] GetThreadList()
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_THREADS, 0);
			CheckStatus();
			byte[] array = new byte[4];
			sock.Receive(array, 4, SocketFlags.None);
			int num = BitConverter.ToInt32(array, 0);
			byte[] value = ReceiveData(num * 4);
			uint[] array2 = new uint[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = BitConverter.ToUInt32(value, i * 4);
			}
			return array2;
		}

		public ThreadInfo GetThreadInfo(uint lwpid)
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_THRINFO, 4, lwpid);
			CheckStatus();
			return (ThreadInfo)GetObjectFromBytes(ReceiveData(40), typeof(ThreadInfo));
		}

		public void StopThread(uint lwpid)
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_STOPTHR, 4, lwpid);
			CheckStatus();
		}

		public void ResumeThread(uint lwpid)
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_RESUMETHR, 4, lwpid);
			CheckStatus();
		}

		public regs GetRegisters(uint lwpid)
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_GETREGS, 4, lwpid);
			CheckStatus();
			return (regs)GetObjectFromBytes(ReceiveData(176), typeof(regs));
		}

		public void SetRegisters(uint lwpid, regs regs)
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_SETREGS, 8, lwpid, 176);
			CheckStatus();
			SendData(GetBytesFromObject(regs), 176);
			CheckStatus();
		}

		public fpregs GetFloatRegisters(uint lwpid)
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_GETFPREGS, 4, lwpid);
			CheckStatus();
			return (fpregs)GetObjectFromBytes(ReceiveData(832), typeof(fpregs));
		}

		public void SetFloatRegisters(uint lwpid, fpregs fpregs)
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_SETFPREGS, 8, lwpid, 832);
			CheckStatus();
			SendData(GetBytesFromObject(fpregs), 832);
			CheckStatus();
		}

		public dbregs GetDebugRegisters(uint lwpid)
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_GETDBGREGS, 4, lwpid);
			CheckStatus();
			return (dbregs)GetObjectFromBytes(ReceiveData(128), typeof(dbregs));
		}

		public void SetDebugRegisters(uint lwpid, dbregs dbregs)
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_SETDBGREGS, 8, lwpid, 128);
			CheckStatus();
			SendData(GetBytesFromObject(dbregs), 128);
			CheckStatus();
		}

		public void SingleStep()
		{
			CheckConnected();
			CheckDebugging();
			SendCMDPacket(CMDS.CMD_DEBUG_SINGLESTEP, 0);
			CheckStatus();
		}

		public ulong KernelBase()
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_KERN_BASE, 0);
			CheckStatus();
			return BitConverter.ToUInt64(ReceiveData(8), 0);
		}

		public byte[] KernelReadMemory(ulong address, int length)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_KERN_READ, 12, address, length);
			CheckStatus();
			return ReceiveData(length);
		}

		public void KernelWriteMemory(ulong address, byte[] data)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_KERN_WRITE, 12, address, data.Length);
			CheckStatus();
			SendData(data, data.Length);
			CheckStatus();
		}

		public ProcessList GetProcessList()
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_LIST, 0);
			CheckStatus();
			byte[] array = new byte[4];
			sock.Receive(array, 4, SocketFlags.None);
			int num = BitConverter.ToInt32(array, 0);
			byte[] array2 = ReceiveData(num * 36);
			string[] array3 = new string[num];
			int[] array4 = new int[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = i * 36;
				array3[i] = ConvertASCII(array2, num2);
				array4[i] = BitConverter.ToInt32(array2, num2 + 32);
			}
			return new ProcessList(num, array3, array4);
		}

		public byte[] ReadMemory(int pid, ulong address, int length)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_READ, 16, pid, address, length);
			CheckStatus();
			return ReceiveData(length);
		}

		public void WriteMemory(int pid, ulong address, byte[] data)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_WRITE, 16, pid, address, data.Length);
			CheckStatus();
			SendData(data, data.Length);
			CheckStatus();
		}

		public ProcessMap GetProcessMaps(int pid)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_MAPS, 4, pid);
			CheckStatus();
			byte[] array = new byte[4];
			sock.Receive(array, 4, SocketFlags.None);
			int num = BitConverter.ToInt32(array, 0);
			byte[] array2 = ReceiveData(num * 58);
			MemoryEntry[] array3 = new MemoryEntry[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = i * 58;
				array3[i] = new MemoryEntry
				{
					name = ConvertASCII(array2, num2),
					start = BitConverter.ToUInt64(array2, num2 + 32),
					end = BitConverter.ToUInt64(array2, num2 + 40),
					offset = BitConverter.ToUInt64(array2, num2 + 48),
					prot = BitConverter.ToUInt16(array2, num2 + 56)
				};
			}
			return new ProcessMap(pid, array3);
		}

		public ulong InstallRPC(int pid)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_INTALL, 4, pid);
			CheckStatus();
			return BitConverter.ToUInt64(ReceiveData(8), 0);
		}

		public ulong Call(int pid, ulong rpcstub, ulong address, params object[] args)
		{
			CheckConnected();
			CMDPacket cMDPacket = default(CMDPacket);
			cMDPacket.magic = 4289379276u;
			cMDPacket.cmd = 3182034950u;
			cMDPacket.datalen = 68u;
			CMDPacket cMDPacket2 = cMDPacket;
			SendData(GetBytesFromObject(cMDPacket2), 12);
			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(BitConverter.GetBytes(pid), 0, 4);
			memoryStream.Write(BitConverter.GetBytes(rpcstub), 0, 8);
			memoryStream.Write(BitConverter.GetBytes(address), 0, 8);
			int num = 0;
			foreach (object obj in args)
			{
				byte[] array = new byte[8];
				object obj2 = obj;
				if (obj2 != null)
				{
					object obj3;
					if ((obj3 = obj2) is char)
					{
						char value = (char)obj3;
						Buffer.BlockCopy(BitConverter.GetBytes(value), 0, array, 0, 2);
						byte[] array2 = new byte[6];
						Buffer.BlockCopy(array2, 0, array, 2, array2.Length);
					}
					else if ((obj3 = obj2) is byte)
					{
						byte value2 = (byte)obj3;
						Buffer.BlockCopy(BitConverter.GetBytes(value2), 0, array, 0, 1);
						byte[] array3 = new byte[7];
						Buffer.BlockCopy(array3, 0, array, 1, array3.Length);
					}
					else if ((obj3 = obj2) is short)
					{
						short value3 = (short)obj3;
						Buffer.BlockCopy(BitConverter.GetBytes(value3), 0, array, 0, 2);
						byte[] array4 = new byte[6];
						Buffer.BlockCopy(array4, 0, array, 2, array4.Length);
					}
					else if ((obj3 = obj2) is ushort)
					{
						ushort value4 = (ushort)obj3;
						Buffer.BlockCopy(BitConverter.GetBytes(value4), 0, array, 0, 2);
						byte[] array5 = new byte[6];
						Buffer.BlockCopy(array5, 0, array, 2, array5.Length);
					}
					else if ((obj3 = obj2) is int)
					{
						int value5 = (int)obj3;
						Buffer.BlockCopy(BitConverter.GetBytes(value5), 0, array, 0, 4);
						byte[] array6 = new byte[4];
						Buffer.BlockCopy(array6, 0, array, 4, array6.Length);
					}
					else if ((obj3 = obj2) is uint)
					{
						uint value6 = (uint)obj3;
						Buffer.BlockCopy(BitConverter.GetBytes(value6), 0, array, 0, 4);
						byte[] array7 = new byte[4];
						Buffer.BlockCopy(array7, 0, array, 4, array7.Length);
					}
					else if ((obj3 = obj2) is long)
					{
						long value7 = (long)obj3;
						Buffer.BlockCopy(BitConverter.GetBytes(value7), 0, array, 0, 8);
					}
					else if ((obj3 = obj2) is ulong)
					{
						ulong value8 = (ulong)obj3;
						Buffer.BlockCopy(BitConverter.GetBytes(value8), 0, array, 0, 8);
					}
				}
				memoryStream.Write(array, 0, array.Length);
				num++;
			}
			if (num > 6)
			{
				throw new Exception("libdbg: too many arguments");
			}
			if (num < 6)
			{
				for (int j = 0; j < 6 - num; j++)
				{
					memoryStream.Write(BitConverter.GetBytes(0uL), 0, 8);
				}
			}
			SendData(memoryStream.ToArray(), 68);
			memoryStream.Dispose();
			CheckStatus();
			return BitConverter.ToUInt64(ReceiveData(12), 4);
		}

		public void LoadElf(int pid, byte[] elf)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_ELF, 8, pid, (uint)elf.Length);
			CheckStatus();
			SendData(elf, elf.Length);
			CheckStatus();
		}

		public void LoadElf(int pid, string filename)
		{
			LoadElf(pid, File.ReadAllBytes(filename));
		}

		public List<ulong> ScanProcess<T>(int pid, ScanCompareType compareType, T value, T extraValue = default(T))
		{
			CheckConnected();
			int num = 0;
			byte[] array = null;
			if (value != null)
			{
				ScanValueType scanValueType;
				byte[] array2;
				T val;
				string text;
				if (((object)(val = value)) is bool)
				{
					bool flag = (bool)(object)val;
					bool value2 = flag;
					scanValueType = ScanValueType.valTypeUInt8;
					num = 1;
					array2 = BitConverter.GetBytes(value2);
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((bool)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is sbyte)
				{
					sbyte b = (sbyte)(object)val;
					sbyte value3 = b;
					scanValueType = ScanValueType.valTypeInt8;
					array2 = BitConverter.GetBytes(value3);
					num = 1;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((sbyte)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is byte)
				{
					byte b2 = (byte)(object)val;
					byte value4 = b2;
					scanValueType = ScanValueType.valTypeUInt8;
					array2 = BitConverter.GetBytes(value4);
					num = 1;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((byte)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is short)
				{
					short num2 = (short)(object)val;
					short value5 = num2;
					scanValueType = ScanValueType.valTypeInt16;
					array2 = BitConverter.GetBytes(value5);
					num = 2;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((short)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is ushort)
				{
					ushort num3 = (ushort)(object)val;
					ushort value6 = num3;
					scanValueType = ScanValueType.valTypeUInt16;
					array2 = BitConverter.GetBytes(value6);
					num = 2;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((ushort)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is int)
				{
					int num4 = (int)(object)val;
					int value7 = num4;
					scanValueType = ScanValueType.valTypeInt32;
					array2 = BitConverter.GetBytes(value7);
					num = 4;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((int)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is uint)
				{
					uint num5 = (uint)(object)val;
					uint value8 = num5;
					scanValueType = ScanValueType.valTypeUInt32;
					array2 = BitConverter.GetBytes(value8);
					num = 4;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((uint)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is long)
				{
					long num6 = (long)(object)val;
					long value9 = num6;
					scanValueType = ScanValueType.valTypeInt64;
					array2 = BitConverter.GetBytes(value9);
					num = 8;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((long)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is ulong)
				{
					ulong num7 = (ulong)(object)val;
					ulong value10 = num7;
					scanValueType = ScanValueType.valTypeUInt64;
					array2 = BitConverter.GetBytes(value10);
					num = 8;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((ulong)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is float)
				{
					float num8 = (float)(object)val;
					float value11 = num8;
					scanValueType = ScanValueType.valTypeFloat;
					array2 = BitConverter.GetBytes(value11);
					num = 4;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((float)(object)extraValue);
					}
				}
				else if (((object)(val = value)) is double)
				{
					double num9 = (double)(object)val;
					double value12 = num9;
					scanValueType = ScanValueType.valTypeDouble;
					array2 = BitConverter.GetBytes(value12);
					num = 8;
					if (extraValue != null)
					{
						array = BitConverter.GetBytes((double)(object)extraValue);
					}
				}
				else if ((text = (value as string)) == null)
				{
					byte[] array3;
					if ((array3 = (value as byte[])) == null)
					{
						goto IL_03f2;
					}
					byte[] array4 = array3;
					scanValueType = ScanValueType.valTypeArrBytes;
					array2 = array4;
					num = array2.Length;
				}
				else
				{
					string s = text;
					scanValueType = ScanValueType.valTypeString;
					array2 = Encoding.ASCII.GetBytes(s);
					num = array2.Length;
				}
				SendCMDPacket(CMDS.CMD_PROC_SCAN, 10, pid, (byte)scanValueType, (byte)compareType, (extraValue == null) ? num : (num * 2));
				CheckStatus();
				SendData(array2, num);
				if (array != null)
				{
					SendData(array, num);
				}
				CheckStatus();
				int receiveTimeout = sock.ReceiveTimeout;
				sock.ReceiveTimeout = 2147483647;
				List<ulong> list = new List<ulong>();
				while (true)
				{
					ulong num10 = BitConverter.ToUInt64(ReceiveData(8), 0);
					if (num10 == ulong.MaxValue)
					{
						break;
					}
					list.Add(num10);
				}
				sock.ReceiveTimeout = receiveTimeout;
				return list;
			}
			goto IL_03f2;
			IL_03f2:
			throw new NotSupportedException("Requested scan value type is not supported! (Feed in Byte[] instead.)");
		}

		public void ChangeProtection(int pid, ulong address, uint length, VM_PROTECTIONS newProt)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_PROTECT, 20, pid, address, length, (uint)newProt);
			CheckStatus();
		}

		public ProcessInfo GetProcessInfo(int pid)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_INFO, 4, pid);
			CheckStatus();
			return (ProcessInfo)GetObjectFromBytes(ReceiveData(188), typeof(ProcessInfo));
		}

		public ulong AllocateMemory(int pid, int length)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_ALLOC, 8, pid, length);
			CheckStatus();
			return BitConverter.ToUInt64(ReceiveData(8), 0);
		}

		public void FreeMemory(int pid, ulong address, int length)
		{
			CheckConnected();
			SendCMDPacket(CMDS.CMD_PROC_FREE, 16, pid, address, length);
			CheckStatus();
		}

		public T ReadMemory<T>(int pid, ulong address)
		{
			if (typeof(T) == typeof(string))
			{
				string text = "";
				ulong num = 0uL;
				while (true)
				{
					byte b = ReadMemory(pid, address + num, 1)[0];
					if (b == 0)
					{
						break;
					}
					text += Convert.ToChar(b).ToString();
					num++;
				}
				return (T)(object)text;
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
				WriteMemory(pid, address, Encoding.ASCII.GetBytes((string)(object)value + "\0"));
			}
			else if (typeof(T) == typeof(byte[]))
			{
				WriteMemory(pid, address, (byte[])(object)value);
			}
			else
			{
				WriteMemory(pid, address, GetBytesFromObject(value));
			}
		}
	}
}
