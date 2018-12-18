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
        PS4DBG ps4;
        private int pid;
        private ulong stub;
        private ulong libSceUserServiceBase = 0x0;
        private ulong libSceSaveDataBase = 0x0;
        private int user = 0x0;
        string mp = null;
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

        private void processesButton_Click(object sender, EventArgs e)
        {
            if (!ps4.IsConnected)
            {
                SetStatus("Not Connected");
                return;
            }
            processesComboBox.DataSource = ps4.GetProcessList().processes;
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

            if (pm.FindEntry("(NoName)clienthandler") == null)
            {
                stub = ps4.InstallRPC(pid);
                return;
            }
            stub = pm.FindEntry("(NoName)clienthandler").start;

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

            //PATCHES
			/* shows sce saves but doesn't mount them
            ps4.WriteMemory(pid, libSceSaveDataBase + 0x32998, "////");
			*/

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
            mp = Mount(mount, mountResult);

            ps4.FreeMemory(pid, dirNameAddr, Marshal.SizeOf(typeof(SceSaveDataDirName)));
            if (mp != "")
            {
                SetStatus($"Save Mounted in {mp}");
            }
            else
            {
                SetStatus("Mouting Failed");
            }
        }

        private void unmountButton_Click(object sender, EventArgs e)
        {
            if (mp == null)
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
            var mp = Mount(mount, mountResult);
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

        private string Mount(SceSaveDataMount2 mount, SceSaveDataMountResult mountResult)
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
    }
}
