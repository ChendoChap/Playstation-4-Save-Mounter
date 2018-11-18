using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using librpc;

namespace PS4SDT
{
    public partial class Mainfrm : Form
    {
        PS4RPC ps4;
        public Mainfrm()
        {
            InitializeComponent();
            MountMode_ComboBox.SelectedIndex = 0;
        }

        private int pid;
        private ulong stub;
        private ulong libSceUserServiceBase = 0x0;
        private ulong libSceSaveDataBase = 0x0;
        private ulong libSceLibcInternalBase = 0x0;
        private string mp;
        private uint mm = 0x1;
        
        private void Connect_Button_Click(object sender, EventArgs e)
        {
            ps4 = new PS4RPC(IP_TextBox.Text);
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
            var pi = ps4.GetProcessInfo(pid);

            var tmp = pi.FindEntry("libSceSaveData.sprx")?.start;
            if (tmp == null)
            {
                MessageBox.Show("savedata lib not found", "Error");
                return;
            }
            libSceSaveDataBase = (ulong)tmp;

            tmp = pi.FindEntry("libSceUserService.sprx")?.start;
            if (tmp == null)
            {
                MessageBox.Show("user service lib not found", "Error");
                return;
            }
            libSceUserServiceBase = (ulong)tmp;

            tmp = pi.FindEntry("libSceLibcInternal.sprx")?.start;
            if (tmp == null)
            {
                MessageBox.Show("libcinternal not found", "Error");
                return;
            }
            libSceLibcInternalBase = (ulong)tmp;


            if (pi.FindEntry("(NoName)rpchandler") == null)
            {
                stub = ps4.InstallRPC(pid);
                return;
            }

            stub = pi.FindEntry("(NoName)rpchandler").start;
        }

        private void FindDirs_Button_Click(object sender, EventArgs e)
        {
            if (pid == 0)
            {
                return;
            }
            var pi = ps4.GetProcessInfo(pid);
            if (pi.FindEntry("(NoName)rpchandler") == null)
            {
                return;
            }
            var dirNameAddr = malloc(Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
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
            free(dirNameAddr);
        }

        private void Mount_Button_Click(object sender, EventArgs e)
        {
            if (Dirs_ComboBox.Items.Count == 0)
            {
                return;
            }
            var dirNameAddr = malloc(Marshal.SizeOf(typeof(SceSaveDataDirName)));
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

            free(dirNameAddr);

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


        private ulong malloc(int size, bool zero = true)
        {
            //what's a calloc?
            var addr = ps4.Call(pid, stub, libSceLibcInternalBase + offsets.malloc, size);
            if (zero)
            {
                ps4.WriteMemory(pid, addr, new byte[size]);
            }
            return addr;
        }

        private void free(ulong addr)
        {
            ps4.Call(pid, stub, libSceLibcInternalBase + offsets.free, addr);
        }

        private int InitialUser()
        {
            var bufferAddr = malloc(sizeof(int));

            ps4.Call(pid, stub, libSceUserServiceBase + offsets.sceUserServiceGetInitialUser, bufferAddr);

            var id = ps4.ReadMemory<int>(pid, bufferAddr);

            free(bufferAddr);

            return id;
        }

        private string[] Find(SceSaveDataDirNameSearchCond searchCond, SceSaveDataDirNameSearchResult searchResult)
        {
            var searchCondAddr = malloc(Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
            var searchResultAddr = malloc(Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));

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
                free(searchCondAddr);
                free(searchResultAddr);
                return dirs;
            }

            free(searchCondAddr);
            free(searchResultAddr);

            return new string[0];

        }

        private string Mount(SceSaveDataMount2 mount, SceSaveDataMountResult mountResult)
        {
            var mountAddr = malloc(Marshal.SizeOf(typeof(SceSaveDataMount2)));
            var mountResultAddr = malloc(Marshal.SizeOf(typeof(SceSaveDataMountResult)));

            ps4.WriteMemory(pid, mountAddr, mount);
            ps4.WriteMemory(pid, mountResultAddr, mountResult);

            if (ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataMount2, mountAddr, mountResultAddr) == 0)
            {
                mountResult = ps4.ReadMemory<SceSaveDataMountResult>(pid, mountResultAddr);

                free(mountAddr);
                free(mountResultAddr);

                return mountResult.mountPoint.data;
            }

            free(mountAddr);
            free(mountResultAddr);

            return "";
        }

        private void Unmount(SceSaveDataMountPoint mountPoint)
        {
            var mountPointAddr = malloc(Marshal.SizeOf(typeof(SceSaveDataMountPoint)));

            ps4.WriteMemory(pid, mountPointAddr, mountPoint);

            ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataUmount, mountPointAddr);

            free(mountPointAddr);
            mp = null;
        }

        private string TransferMount(SceSaveDataTransferringMount mount, SceSaveDataMountResult mountResult)
        {
            var mountAddr = malloc(Marshal.SizeOf(typeof(SceSaveDataTransferringMount)));
            var mountResultAddr = malloc(Marshal.SizeOf(typeof(SceSaveDataMountResult)));

            ps4.WriteMemory(pid, mountAddr, mount);
            ps4.WriteMemory(pid, mountResultAddr, mountResult);

            if (ps4.Call(pid, stub, libSceSaveDataBase + offsets.sceSaveDataTransferringMount, mountAddr, mountResultAddr) == 0)
            {
                mountResult = ps4.ReadMemory<SceSaveDataMountResult>(pid, mountResultAddr);

                free(mountAddr);
                free(mountResultAddr);

                return mountResult.mountPoint.data;
            }

            free(mountAddr);
            free(mountResultAddr);

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
                    mm = 10u;
                    break;
            }
        }
    }
}
