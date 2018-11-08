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

        private int pid;
        private ulong stub;
        private ulong libSceUserServiceBase = 0x0;
        private ulong libSceSaveDataBase = 0x0;
        private string mp;
        private uint mm = 0x1;
        private void FindIP_Button_Click(object sender, EventArgs e)
        {
            try
            {
                IP_TextBox.Text = PS4DBG.FindPlayStation();
            }
            catch
            {
                MessageBox.Show("Couldn't find ps4","Failed");
            }
        }

        private void Connect_Button_Click(object sender, EventArgs e)
        {
            ps4 = new PS4DBG(IP_TextBox.Text);
            ps4.Connect();
        }

        private void RefreshProc_Button_Click(object sender, EventArgs e)
        {
            if(ps4.IsConnected)
            Processes_ComboBox.DataSource = ps4.GetProcessList().processes;
        }

        private void Setup_Button_Click(object sender, EventArgs e)
        {
            if (pid == 0)
            {
                return;
            }
            var pm = ps4.GetProcessMaps(pid);
            libSceUserServiceBase = pm.FindEntry("libSceUserService.sprx").start;
            var   a   = pm.FindEntry("libSceSaveData.sprx")?.start;
            if (a != null)
            {


                libSceSaveDataBase = (ulong) a;
            }

            if (pm.FindEntry("(NoName)clienthandler") == null)
            {
                stub = ps4.InstallRPC(pid);
                return;
            }

            stub = pm.FindEntry("(NoName)clienthandler").start;
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
            SceSaveDataDirNameSearchCond searchCond = new SceSaveDataDirNameSearchCond
            {
                userId = InitialUser()
            };
            SceSaveDataDirNameSearchResult searchResult = new SceSaveDataDirNameSearchResult
            {
                dirNames = dirNameAddr,
                dirNamesNum = 1024
            };
            Dirs_ComboBox.DataSource = Find(searchCond, searchResult);
            ps4.FreeMemory(pid, dirNameAddr, Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
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

        private string[] Find(SceSaveDataDirNameSearchCond searchCond, SceSaveDataDirNameSearchResult searchResult)
        {
            var searchCondAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
            var searchResultAddr = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));

            ps4.WriteMemory(pid, searchCondAddr, searchCond);
            ps4.WriteMemory(pid, searchResultAddr, searchResult);

            if (ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataDirNameSearch, searchCondAddr, searchResultAddr) == 0)
            {
                searchResult = ps4.ReadMemory<SceSaveDataDirNameSearchResult>(pid, searchResultAddr);
                string[] dirs = new string[searchResult.hitNum];
                for (uint i = 0; i < searchResult.hitNum; i++)
                {
                    dirs[i] = ps4.ReadMemory<string>(pid, searchResult.dirNames + i * 32);
                }
                ps4.FreeMemory(pid, searchCondAddr, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
                ps4.FreeMemory(pid, searchResultAddr, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));
                return dirs;
            }

            ps4.FreeMemory(pid, searchCondAddr, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
            ps4.FreeMemory(pid, searchResultAddr, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));

            return new string[0];

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

        private void MountMode_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (MountMode_ComboBox.SelectedIndex)
            {
                case 0:
                    mm = 0x01;
                    break;
                case 1:
                    mm = 0x2 || 0x8;
                    break;
            }
        }
    }
}
