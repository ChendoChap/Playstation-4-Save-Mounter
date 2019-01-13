using libdebug;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace PS4Saves
{
    public partial class Main : Form
    {
        PS4DBG ps4 = new PS4DBG();
        private int pid;
        private ulong stub;
        private ulong libSceUserServiceBase = 0x0;
        private ulong libSceSaveDataBase = 0x0;
        private int user = 0x0;
        string mp = "";
        bool log = false;
        
        public Main()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2 && args[1] == "-log")
            {
                log = true;
            }

            if (File.Exists("ip"))
            {
                ipTextBox.Text = File.ReadAllText("ip");
            }
        }
        public static string FormatSize(double size)
        {
            const long BytesInKilobytes = 1024;
            const long BytesInMegabytes = BytesInKilobytes * 1024;
            const long BytesInGigabytes = BytesInMegabytes * 1024;
            double value;
            string str;
            if (size < BytesInGigabytes)
            {
                value = size / BytesInMegabytes;
                str = "MB";
            }
            else
            {
                value = size /BytesInGigabytes;
                str = "GB";
            }
            return String.Format("{0:0.##} {1}", value, str);
        }
        private void sizeTrackBar_Scroll(object sender, EventArgs e)
        {
            sizeToolTip.SetToolTip(sizeTrackBar, FormatSize((double)(sizeTrackBar.Value * 32768)));
        }
        private void SetStatus(string msg)
        {
            statusLabel.Text = $"Status: {msg}";
        }
        private void WriteLog(string msg)
        {
            if(log)
            {

                msg = $"|{msg}|";
                var a = msg.Length / 2;
                for (var i = 0; i < 48 - a; i++)
                {
                    msg = msg.Insert(0, " ");
                }

                for (var i = msg.Length; i < 96; i++)
                {
                    msg += " ";
                }

                var dateAndTime = DateTime.Now;
                var logStr = $"|{dateAndTime:MM/dd/yyyy} {dateAndTime:hh:mm:ss tt}| |{msg}|";

                if (File.Exists(@"log.txt"))
                {
                    File.AppendAllText(@"log.txt",
                        $"{logStr}{Environment.NewLine}");
                }
                else
                {
                    using (var sw = File.CreateText(@"log.txt"))
                    {
                        sw.WriteLine(logStr);
                    }
                }

                Console.WriteLine(logStr);
            }
        }
        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                ps4 = new PS4DBG(ipTextBox.Text);
                ps4.Connect();
                if (!ps4.IsConnected)
                {
                    throw new Exception();
                }
                SetStatus("Connected");
                if (!File.Exists("ip"))
                {
                    File.WriteAllText("ip", ipTextBox.Text);
                }
                else
                {
                    using (var sw = File.CreateText(@"log.txt"))
                    {
                        sw.Write(ipTextBox.Text);
                    }
                }
            }
            catch
            {
                SetStatus("Failed To Connect");
            }
        }

        private Process[] filter(ProcessList list)
        {
            List<Process> procs = new List<Process>();
            for(int i = 0; i < list.processes.Length; i++)
            {
                if (notSceCheck(list.processes[i].name))
                {
                    procs.Add(list.processes[i]);
                }
            }
            return procs.ToArray();
        }

        private string[] sceProcesses = { "SceVencProxy.elf", "SceVdecProxy.elf", "fs_cleaner.elf", "GnmCompositor.elf", "orbis_audiod.elf", "SceSysCore.elf", "SceSysAvControl.elf", "mini-syscore.elf" };
        private bool notSceCheck(string name)
        {
            if (name == "eboot.bin")
            {
                return true;
            }

            if (name.EndsWith(".elf"))
            {
                for (int i = 0; i < sceProcesses.Length; i++)
                {
                    if (name == sceProcesses[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private void processesButton_Click(object sender, EventArgs e)
        {
            if (!ps4.IsConnected)
            {
                SetStatus("Not Connected");
                return;
            }
            processesComboBox.DataSource = filter(ps4.GetProcessList());
            SetStatus("Refreshed Processes");
        }

        private void setupButton_Click(object sender, EventArgs e)
        {
            if (pid == 0)
            {
                SetStatus("No Process Selected");
                return;
            }
            var pm = ps4.GetProcessMaps(pid);
            var tmp = pm.FindEntry("libSceSaveData.sprx")?.start;
            if (tmp == null)
            {
                MessageBox.Show("savedata lib not found", "Error");
                return;
            }
            libSceSaveDataBase = (ulong)tmp;

            tmp = pm.FindEntry("libSceUserService.sprx")?.start;
            if (tmp == null)
            {
                MessageBox.Show("user service lib not found", "Error");
                return;
            }
            libSceUserServiceBase = (ulong)tmp;

            stub = pm.FindEntry("(NoName)clienthandler") == null ? ps4.InstallRPC(pid) : pm.FindEntry("(NoName)clienthandler").start;

            var ids = GetLoginList();
            List<User> users = new List<User>();
            for(int i = 0; i < ids.Length; i++)
            {
                if(ids[i] == -1)
                {
                    continue;
                }
                users.Add(new User {id = ids[i], name = GetUserName(ids[i]) });
            }
            userComboBox.DataSource = users.ToArray();

            var ret = ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataInitialize3);
            WriteLog($"sceSaveDataInitialize3 ret = 0x{ret:X}");
            
           
            //PATCHES
            //SCE_ PATCHES
            ps4.WriteMemory(pid, libSceSaveDataBase + 0x32998, (byte)0x00); // 'sce_' patch
            ps4.WriteMemory(pid, libSceSaveDataBase + 0x31699, (byte)0x00); // 'sce_sdmemory' patch
            ps4.WriteMemory(pid, libSceSaveDataBase + 0x01119, (byte)0x30); // '_' patch

            var l = ps4.GetProcessList();
            var s = l.FindProcess("SceShellCore");
            var m = ps4.GetProcessMaps(s.pid);
            var ex = m.FindEntry("executable");
            //SHELLCORE PATCHES
            ps4.WriteMemory(s.pid, ex.start + 0xD42843, (byte)0x00); // 'sce_sdmemory' patch
            ps4.WriteMemory(s.pid, ex.start + 0x7E4DC0, new byte[]{0x48, 0x31, 0xC0, 0xC3}); //verify keystone patch
            SetStatus("Setup Done :)");
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (pid == 0)
            {
                SetStatus("No Process Selected");
                return;
            }
            var pm = ps4.GetProcessMaps(pid);
            if (pm.FindEntry("(NoName)clienthandler") == null)
            {
                SetStatus("RPC Stub Not Found");
                return;
            }
            var dirNameAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
            var paramAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataParam)) * 1024);
            SceSaveDataDirNameSearchCond searchCond = new SceSaveDataDirNameSearchCond
            {
                userId = GetUser()
            };
            SceSaveDataDirNameSearchResult searchResult = new SceSaveDataDirNameSearchResult
            {
                dirNames = dirNameAddr,
                dirNamesNum = 1024,
                param = paramAddr,
            };
            dirsComboBox.DataSource = Find(searchCond, searchResult);
            ps4.FreeMemory(pid, dirNameAddr, Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
            ps4.FreeMemory(pid, paramAddr, Marshal.SizeOf(typeof(SceSaveDataParam)) * 1024);
            if (dirsComboBox.Items.Count > 0)
            {
                SetStatus($"Found {dirsComboBox.Items.Count} Save Directories :D");
            }
            else
            {
                SetStatus("Found 0 Save Directories :(");
            }
        }

        private void mountButton_Click(object sender, EventArgs e)
        {
            if (dirsComboBox.Items.Count == 0)
            {
                return;
            }
            var dirNameAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirName)));
            SceSaveDataDirName dirName = new SceSaveDataDirName
            {
                data = dirsComboBox.Text
            };

            SceSaveDataMount2 mount = new SceSaveDataMount2
            {
                userId = GetUser(),
                dirName = dirNameAddr,
                blocks = 32768,
                mountMode = 0x8 | 0x2,

            };
            SceSaveDataMountResult mountResult = new SceSaveDataMountResult
            {

            };
            ps4.WriteMemory(pid, dirNameAddr, dirName);
            mp = Mount2(mount, mountResult);

            ps4.FreeMemory(pid, dirNameAddr, Marshal.SizeOf(typeof(SceSaveDataDirName)));
            if (mp != "")
            {
                SetStatus($"Save Mounted in {mp}");
            }
            else
            {
                SetStatus("Mounting Failed");
            }
        }

        private void unmountButton_Click(object sender, EventArgs e)
        {
            if (mp == "")
            {
                SetStatus("No save mounted");
                return;
            }
            SceSaveDataMountPoint mountPoint = new SceSaveDataMountPoint
            {
                data = mp,
            };

            Unmount(mountPoint);
            mp = null;
            SetStatus("Save Unmounted");
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            if (pid == 0)
            {
                SetStatus("No Process Selected");
                return;
            }
            var pm = ps4.GetProcessMaps(pid);
            if (pm.FindEntry("(NoName)clienthandler") == null)
            {
                SetStatus("RPC Stub Not Found");
                return;
            }
            if (nameTextBox.Text == "")
            {
                SetStatus("No Save Name");
                return;
            }
            var dirNameAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirName)));
            SceSaveDataDirName dirName = new SceSaveDataDirName
            {
                data = nameTextBox.Text
            };

            SceSaveDataMount2 mount = new SceSaveDataMount2
            {
                userId = GetUser(),
                dirName = dirNameAddr,
                blocks = (ulong) sizeTrackBar.Value,
                mountMode = 4 | 2 | 8,

            };
            SceSaveDataMountResult mountResult = new SceSaveDataMountResult
            {

            };
            ps4.WriteMemory(pid, dirNameAddr, dirName);
            var mp = Mount2(mount, mountResult);
            ps4.FreeMemory(pid, dirNameAddr, Marshal.SizeOf(typeof(SceSaveDataDirName)));
            if (mp != "")
            {
                SetStatus("Save Created");
                SceSaveDataMountPoint mountPoint = new SceSaveDataMountPoint
                {
                    data = mp,
                };
                Unmount(mountPoint);
            }
            else
            {
                SetStatus("Save Creation Failed");
            }
        }

        private int GetUser()
        {
            if(user != 0)
            {
                return user;
            }
            else
            {
                return InitialUser();
            }
        }

        private int InitialUser()
        {
            var bufferAddr = ps4.AllocateMemory(pid, sizeof(int));

            ps4.Call(pid, stub, libSceUserServiceBase + offsets.sceUserServiceGetInitialUser, bufferAddr);

            var id = ps4.ReadMemory<int>(pid, bufferAddr);

            ps4.FreeMemory(pid, bufferAddr, sizeof(int));

            return id;
        }

        private int[] GetLoginList()
        {
            var bufferAddr = ps4.AllocateMemory(pid, sizeof(int) * 4);
            ps4.Call(pid, stub, libSceUserServiceBase + offsets.sceUserServiceGetLoginUserIdList, bufferAddr);

            var id = ps4.ReadMemory(pid, bufferAddr, sizeof(int) * 4);
            var size = id.Length / sizeof(int);
            var ints = new int[size];
            for (var index = 0; index < size; index++)
            {
                ints[index] = BitConverter.ToInt32(id, index * sizeof(int));
            }
            ps4.FreeMemory(pid, bufferAddr, sizeof(int));

            return ints;
        }

        private string GetUserName(int userid)
        {
            var bufferAddr = ps4.AllocateMemory(pid, 17);
            ps4.Call(pid, stub, libSceUserServiceBase + offsets.sceUserServiceGetUserName, userid, bufferAddr, 17);
            var str = ps4.ReadMemory<string>(pid, bufferAddr);
            ps4.FreeMemory(pid, bufferAddr, 17);
            return str;
        }

        private SearchEntry[] Find(SceSaveDataDirNameSearchCond searchCond, SceSaveDataDirNameSearchResult searchResult)
        {
            var searchCondAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
            var searchResultAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));

            ps4.WriteMemory(pid, searchCondAddr, searchCond);
            ps4.WriteMemory(pid, searchResultAddr, searchResult);
            var ret = ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataDirNameSearch, searchCondAddr, searchResultAddr);
            WriteLog($"sceSaveDataDirNameSearch ret = 0x{ret:X}");
            if ( ret == 0)
            {
                searchResult = ps4.ReadMemory<SceSaveDataDirNameSearchResult>(pid, searchResultAddr);
                SearchEntry[] sEntries = new SearchEntry[searchResult.hitNum];
                for (uint i = 0; i < searchResult.hitNum; i++)
                {
                    SceSaveDataParam tmp = ps4.ReadMemory<SceSaveDataParam>(pid, searchResult.param + i * (uint)Marshal.SizeOf(typeof(SceSaveDataParam)));
                    sEntries[i] = new SearchEntry
                    {
                        dirName = ps4.ReadMemory<string>(pid, searchResult.dirNames + i * 32),
                        title = tmp.title,
                        subtitle = tmp.subTitle,
                        detail = tmp.detail,
                        time = new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(tmp.mtime).ToString(),
                    };
                }
                ps4.FreeMemory(pid, searchCondAddr, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
                ps4.FreeMemory(pid, searchResultAddr, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));
                return sEntries;
            }

            ps4.FreeMemory(pid, searchCondAddr, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
            ps4.FreeMemory(pid, searchResultAddr, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));

            return new SearchEntry[0];

        }

        private string Mount2(SceSaveDataMount2 mount, SceSaveDataMountResult mountResult)
        {
            var mountAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMount2)));
            var mountResultAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMountResult)));

            ps4.WriteMemory(pid, mountAddr, mount);
            ps4.WriteMemory(pid, mountResultAddr, mountResult);
            var ret = ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataMount2, mountAddr, mountResultAddr);
            WriteLog($"sceSaveDataMount2 ret = 0x{ret:X}");
            if (ret == 0)
            {
                mountResult = ps4.ReadMemory<SceSaveDataMountResult>(pid, mountResultAddr);

                ps4.FreeMemory(pid, mountAddr, Marshal.SizeOf(typeof(SceSaveDataMount2)));
                ps4.FreeMemory(pid, mountResultAddr, Marshal.SizeOf(typeof(SceSaveDataMountResult)));

                return mountResult.mountPoint.data;
            }

            ps4.FreeMemory(pid, mountAddr, Marshal.SizeOf(typeof(SceSaveDataMount2)));
            ps4.FreeMemory(pid, mountResultAddr, Marshal.SizeOf(typeof(SceSaveDataMountResult)));

            return "";
        }

        private void Unmount(SceSaveDataMountPoint mountPoint)
        {
            var mountPointAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMountPoint)));

            ps4.WriteMemory(pid, mountPointAddr, mountPoint);
            var ret = ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataUmount, mountPointAddr);
            WriteLog($"sceSaveDataUmount ret = 0x{ret:X}");
            ps4.FreeMemory(pid, mountPointAddr, Marshal.SizeOf(typeof(SceSaveDataMountPoint)));
            mp = null;
        }

        private string Mount(SceSaveDataMount mount, SceSaveDataMountResult mountResult)
        {
            var mountAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMount)));
            var mountResultAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMountResult)));
            ps4.WriteMemory(pid, mountAddr, mount);
            ps4.WriteMemory(pid, mountResultAddr, mountResult);

            var ret = ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataMount, mountAddr, mountResultAddr);
            WriteLog($"sceSaveDataMount ret = 0x{ret:X}");
            if (ret == 0)
            {
                mountResult = ps4.ReadMemory<SceSaveDataMountResult>(pid, mountResultAddr);

                ps4.FreeMemory(pid, mountAddr, Marshal.SizeOf(typeof(SceSaveDataMount)));
                ps4.FreeMemory(pid, mountResultAddr, Marshal.SizeOf(typeof(SceSaveDataMountResult)));

                return mountResult.mountPoint.data;
            }

            ps4.FreeMemory(pid, mountAddr, Marshal.SizeOf(typeof(SceSaveDataMount)));
            ps4.FreeMemory(pid, mountResultAddr, Marshal.SizeOf(typeof(SceSaveDataMountResult)));

            return "";
        }
        private string TransferMount(SceSaveDataTransferringMount mount, SceSaveDataMountResult mountResult)
        {
            var mountAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataTransferringMount)));
            var mountResultAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMountResult)));

            ps4.WriteMemory(pid, mountAddr, mount);
            ps4.WriteMemory(pid, mountResultAddr, mountResult);
            var ret = ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataTransferringMount, mountAddr, mountResultAddr);
            WriteLog($"sceSaveDataTransferringMount ret = 0x{ret:X}");
            if (ret == 0)
            {
                mountResult = ps4.ReadMemory<SceSaveDataMountResult>(pid, mountResultAddr);

                ps4.FreeMemory(pid, mountAddr, Marshal.SizeOf(typeof(SceSaveDataTransferringMount)));
                ps4.FreeMemory(pid, mountResultAddr, Marshal.SizeOf(typeof(SceSaveDataMountResult)));

                return mountResult.mountPoint.data;
            }

            ps4.FreeMemory(pid, mountAddr, Marshal.SizeOf(typeof(SceSaveDataTransferringMount)));
            ps4.FreeMemory(pid, mountResultAddr, Marshal.SizeOf(typeof(SceSaveDataMountResult)));

            return "";
        }

        class SearchEntry
        {
            public string dirName;
            public string title;
            public string subtitle;
            public string detail;
            public string time;
            public override string ToString()
            {
                return dirName;
            }
        }

        private void dirsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            titleTextBox.Text = ((SearchEntry)dirsComboBox.SelectedItem).title;
            subtitleTextBox.Text = ((SearchEntry)dirsComboBox.SelectedItem).subtitle;
            detailsTextBox.Text = ((SearchEntry)dirsComboBox.SelectedItem).detail;
            dateTextBox.Text = ((SearchEntry)dirsComboBox.SelectedItem).time;
        }

        private void processesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pid = ((Process)processesComboBox.SelectedItem).pid;
        }

        private void userComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            user = ((User)userComboBox.SelectedItem).id;
        }

        class User
        {
            public int id;
            public string name;

            public override string ToString()
            {
                return name;
            }
        }

        private void tSearchButton_Click(object sender, EventArgs e)
        {
            if (pid == 0)
            {
                SetStatus("No Process Selected");
                return;
            }
            var pm = ps4.GetProcessMaps(pid);
            if (pm.FindEntry("(NoName)clienthandler") == null)
            {
                SetStatus("RPC Stub Not Found");
                return;
            }

            titleIdTextBox.Text = titleIdTextBox.Text.ToUpper().Replace("-", "").Replace(" ","");
            if (titleIdTextBox.Text.Length != 9)
            {
                SetStatus("Invalid Title");
                return;
            }
            var dirNameAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
            var paramAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataParam)) * 1024);
            var titleidAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataTitleId)));
            SceSaveDataTitleId titleid = new SceSaveDataTitleId
            {
                data = titleIdTextBox.Text
            };
            SceSaveDataDirNameSearchCond searchCond = new SceSaveDataDirNameSearchCond
            {
                userId = GetUser(),
                titleId = titleidAddr
            };
            SceSaveDataDirNameSearchResult searchResult = new SceSaveDataDirNameSearchResult
            {
                dirNames = dirNameAddr,
                dirNamesNum = 1024,
                param = paramAddr,
            };
            ps4.WriteMemory(pid, titleidAddr, titleid);
            tDirsComboBox.DataSource = Find(searchCond, searchResult);
            ps4.FreeMemory(pid, dirNameAddr, Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
            ps4.FreeMemory(pid, paramAddr, Marshal.SizeOf(typeof(SceSaveDataParam)) * 1024);
            ps4.FreeMemory(pid, titleidAddr, Marshal.SizeOf(typeof(SceSaveDataTitleId)));
            if (tDirsComboBox.Items.Count > 0)
            {
                SetStatus($"Found {tDirsComboBox.Items.Count} Save Directories :D");
            }
            else
            {
                SetStatus("Found 0 Save Directories :(");
            }
        }

        private void tMountButton_Click(object sender, EventArgs e)
        {
            if (tDirsComboBox.Items.Count == 0)
            {
                return;
            }
            var dirNameAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirName)));
            var fingerprintAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataFingerprint)));
            var titleidAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataTitleId)));
            SceSaveDataDirName dirName = new SceSaveDataDirName
            {
                data = tDirsComboBox.Text
            };
            SceSaveDataFingerprint fingerprint = new SceSaveDataFingerprint
            {
                //verify keystone patch is applied
                data = "0000000000000000000000000000000000000000000000000000000000000000"
            };
            SceSaveDataTitleId titleid = new SceSaveDataTitleId
            {
                data = titleIdTextBox.Text
            };
            SceSaveDataMount mount = new SceSaveDataMount
            {
                userId = GetUser(),
                titleId = titleidAddr,
                dirName = dirNameAddr,
                fingerprint = fingerprintAddr,
                mountMode  = 0x01,

            };

            SceSaveDataMountResult mountResult = new SceSaveDataMountResult();
            ps4.WriteMemory(pid, dirNameAddr, dirName);
            ps4.WriteMemory(pid, fingerprintAddr, fingerprint);
            ps4.WriteMemory(pid, titleidAddr, titleid);
            mp = Mount(mount, mountResult);

            ps4.FreeMemory(pid, dirNameAddr, Marshal.SizeOf(typeof(SceSaveDataDirName)));
            ps4.FreeMemory(pid, fingerprintAddr, Marshal.SizeOf(typeof(SceSaveDataFingerprint)));
            ps4.FreeMemory(pid, titleidAddr, Marshal.SizeOf(typeof(SceSaveDataTitleId)));
            if (mp != "")
            {
                SetStatus($"Save Mounted in {mp}");
            }
            else
            {
                SetStatus("T Mounting Failed");
            }
        }

        private void tUnmountButton_Click(object sender, EventArgs e)
        {
            if (mp == "")
            {
                SetStatus("No save mounted");
                return;
            }
            SceSaveDataMountPoint mountPoint = new SceSaveDataMountPoint
            {
                data = mp,
            };

            Unmount(mountPoint);
            mp = null;
            SetStatus("Save Unmounted");
        }

        private void tDirsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            titleTextBox.Text = ((SearchEntry)tDirsComboBox.SelectedItem).title;
            subtitleTextBox.Text = ((SearchEntry)tDirsComboBox.SelectedItem).subtitle;
            detailsTextBox.Text = ((SearchEntry)tDirsComboBox.SelectedItem).detail;
            dateTextBox.Text = ((SearchEntry)tDirsComboBox.SelectedItem).time;
        }
    }
}
