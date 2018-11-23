using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using libdebug;

namespace PS4SDT
{
    public partial class Mainfrm : Form
    {
        PS4DBG ps4;
        public Mainfrm()
        {
            InitializeComponent();
            MountMode_ComboBox.SelectedIndex = 0;
        }

        private void SetStatus(string msg)
        {
            Status_Label.Text = $"STATUS: {msg}";
        }
        private int pid;
        private ulong stub;
        private ulong libSceUserServiceBase = 0x0;
        private ulong libSceSaveDataBase = 0x0;
        private string mp;
        private uint mm = 0x1;
        private void Connect_Button_Click(object sender, EventArgs e)
        {
            ps4 = new PS4DBG(IP_TextBox.Text);
            ps4.Connect();
            SetStatus("Connected");
        }

        private void RefreshProc_Button_Click(object sender, EventArgs e)
        {
            if(ps4.IsConnected)
            Processes_ComboBox.DataSource = ps4.GetProcessList().processes;
            SetStatus("Refreshed Processes");
        }

        private void Setup_Button_Click(object sender, EventArgs e)
        {
            if (pid == 0)
            {
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
            SetStatus("Setup Done :)");
        }

        private void FindDirs_Button_Click(object sender, EventArgs e)
        {
            if (pid == 0)
            {
                return;
            }
            var pm = ps4.GetProcessMaps(pid);
            if (pm.FindEntry("(NoName)clienthandler") == null)
            {
                return;
            }
            var dirNameAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
            var paramAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataParam)) * 1024);
            SceSaveDataDirNameSearchCond searchCond = new SceSaveDataDirNameSearchCond
            {
                userId = InitialUser()
            };
            SceSaveDataDirNameSearchResult searchResult = new SceSaveDataDirNameSearchResult
            {
                dirNames = dirNameAddr,
                dirNamesNum = 1024,
                param = paramAddr,
            };
            Dirs_ComboBox.DataSource = Find(searchCond, searchResult);
            ps4.FreeMemory(pid, dirNameAddr, Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
            ps4.FreeMemory(pid, paramAddr, Marshal.SizeOf(typeof(SceSaveDataParam)) * 1024);
            if (Dirs_ComboBox.Items.Count > 0)
            {
                SetStatus($"Found {Dirs_ComboBox.Items.Count} Save Directories :D");
            }
            else
            {
                SetStatus("Found 0 Save Directories :(");
            }
        }

        private void Mount_Button_Click(object sender, EventArgs e)
        {
            if (Dirs_ComboBox.Items.Count == 0)
            {
                return;
            }
            var dirNameAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirName)));
            SceSaveDataDirName dirName = new SceSaveDataDirName
            {
                data = Dirs_ComboBox.Text
            };

            SceSaveDataMount2 mount = new SceSaveDataMount2
            {
                userId = InitialUser(),
                dirName = dirNameAddr,
                blocks = 32768,
                mountMode = mm,

            };
            SceSaveDataMountResult mountResult = new SceSaveDataMountResult
            {

            };
            ps4.WriteMemory(pid, dirNameAddr, dirName);
            mp = Mount(mount, mountResult);

            ps4.FreeMemory(pid, dirNameAddr, Marshal.SizeOf(typeof(SceSaveDataDirName)));
            if (mp != "")
            {
                SetStatus("Save Mounted");
            }
            else
            {
                SetStatus("Mouting Failed");
            }
        }

        private void Unmount_Button_Click(object sender, EventArgs e)
        {
            if (mp == null)
            {
                return;
            }
            SceSaveDataMountPoint mountPoint = new SceSaveDataMountPoint
            {
                data = mp,
            };

            Unmount(mountPoint);
            SetStatus("Save Unmounted");
        }

        private void Processes_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            pid = ((Process) Processes_ComboBox.SelectedItem).pid;
        }


        


        private int InitialUser()
        {
            var bufferAddr = ps4.AllocateMemory(pid, sizeof(int));

            ps4.Call(pid, stub, libSceUserServiceBase + offsets.sceUserServiceGetInitialUser, bufferAddr);

            var id = ps4.ReadMemory<int>(pid, bufferAddr);

            ps4.FreeMemory(pid, bufferAddr, sizeof(int));

            return id;
        }

        private SearchEntry[] Find(SceSaveDataDirNameSearchCond searchCond, SceSaveDataDirNameSearchResult searchResult)
        {
            var searchCondAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
            var searchResultAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));

            ps4.WriteMemory(pid, searchCondAddr, searchCond);
            ps4.WriteMemory(pid, searchResultAddr, searchResult);

            if (ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataDirNameSearch, searchCondAddr, searchResultAddr) == 0)
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

            if (ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataMount2, mountAddr, mountResultAddr) == 0)
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

            ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataUmount, mountPointAddr);

            ps4.FreeMemory(pid, mountPointAddr, Marshal.SizeOf(typeof(SceSaveDataMountPoint)));
            mp = null;
        }

        private string TransferMount(SceSaveDataTransferringMount mount, SceSaveDataMountResult mountResult)
        {
            var mountAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataTransferringMount)));
            var mountResultAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMountResult)));

            ps4.WriteMemory(pid, mountAddr, mount);
            ps4.WriteMemory(pid, mountResultAddr, mountResult);

            if (ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataTransferringMount, mountAddr, mountResultAddr) == 0)
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

        private void Q_Button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Aida & ChendoChap - save tool\r\ngolden - ps4debug\r\nLightningMods - tester", "Credits");
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
        private void MountMode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (MountMode_ComboBox.SelectedIndex)
            {
                case 0:
                    mm = 0x01;
                    break;
                case 1:
                    mm = 0x8 | 0x2;
                    break;
            }
        }

        private void Dirs_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Dirs_ComboBox.SelectedItem.GetType() != typeof(SearchEntry))
            {
                return;
            }
            Title_TextBox.Text = ((SearchEntry)Dirs_ComboBox.SelectedItem).title;
            Subtitle_TextBox.Text = ((SearchEntry)Dirs_ComboBox.SelectedItem).subtitle;
            Detail_TextBox.Text = ((SearchEntry)Dirs_ComboBox.SelectedItem).detail;
            Date_TextBox.Text = ((SearchEntry)Dirs_ComboBox.SelectedItem).time;
        }
    }
}
