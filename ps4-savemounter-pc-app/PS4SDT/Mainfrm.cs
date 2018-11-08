using libdebug;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PS4SDT
{
	public class Mainfrm : Form
	{
		private PS4DBG ps4;

		private int pid;

		private ulong stub;

		private ulong libSceUserServiceBase;

		private ulong libSceSaveDataBase;

		private string mp;

		private uint mm = 1u;

		private IContainer components;

		private Button FindIP_Button;

		private TextBox IP_TextBox;

		private Button Connect_Button;

		private Button RefreshProc_Button;

		private ComboBox Processes_ComboBox;

		private Button Setup_Button;

		private Button FindDirs_Button;

		private ComboBox Dirs_ComboBox;

		private Button Mount_Button;

		private Button Unmount_Button;

		private Button Q_Button;

		private ComboBox MountMode_ComboBox;

		public Mainfrm()
		{
			InitializeComponent();
			MountMode_ComboBox.SelectedIndex = 0;
		}

		private void FindIP_Button_Click(object sender, EventArgs e)
		{
			try
			{
				IP_TextBox.Text = PS4DBG.FindPlayStation();
			}
			catch
			{
				MessageBox.Show("Couldn't find ps4", "Failed");
			}
		}

		private void Connect_Button_Click(object sender, EventArgs e)
		{
			ps4 = new PS4DBG(IP_TextBox.Text);
			ps4.Connect();
		}

		private void RefreshProc_Button_Click(object sender, EventArgs e)
		{
			if (ps4.IsConnected)
			{
				Processes_ComboBox.DataSource = ps4.GetProcessList().processes;
			}
		}

		private void Setup_Button_Click(object sender, EventArgs e)
		{
			if (pid != 0)
			{
				ProcessMap processMaps = ps4.GetProcessMaps(pid);
				libSceUserServiceBase = processMaps.FindEntry("libSceUserService.sprx").start;
				ulong? num = processMaps.FindEntry("libSceSaveData.sprx")?.start;
				if (num.HasValue)
				{
					libSceSaveDataBase = num.Value;
				}
				if (processMaps.FindEntry("(NoName)clienthandler") == null)
				{
					stub = ps4.InstallRPC(pid);
				}
				else
				{
					stub = processMaps.FindEntry("(NoName)clienthandler").start;
				}
			}
		}

		private void FindDirs_Button_Click(object sender, EventArgs e)
		{
			if (pid != 0 && ps4.GetProcessMaps(pid).FindEntry("(NoName)clienthandler") != null)
			{
				ulong num = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
				SceSaveDataDirNameSearchCond sceSaveDataDirNameSearchCond = default(SceSaveDataDirNameSearchCond);
				sceSaveDataDirNameSearchCond.userId = InitialUser();
				SceSaveDataDirNameSearchCond searchCond = sceSaveDataDirNameSearchCond;
				SceSaveDataDirNameSearchResult sceSaveDataDirNameSearchResult = default(SceSaveDataDirNameSearchResult);
				sceSaveDataDirNameSearchResult.dirNames = num;
				sceSaveDataDirNameSearchResult.dirNamesNum = 1024u;
				SceSaveDataDirNameSearchResult searchResult = sceSaveDataDirNameSearchResult;
				Dirs_ComboBox.DataSource = Find(searchCond, searchResult);
				ps4.FreeMemory(pid, num, Marshal.SizeOf(typeof(SceSaveDataDirName)) * 1024);
			}
		}

		private void Mount_Button_Click(object sender, EventArgs e)
		{
			if (Dirs_ComboBox.Items.Count != 0)
			{
				ulong num = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirName)));
				SceSaveDataDirName sceSaveDataDirName = default(SceSaveDataDirName);
				sceSaveDataDirName.data = Dirs_ComboBox.Text;
				SceSaveDataDirName value = sceSaveDataDirName;
				SceSaveDataMount2 sceSaveDataMount = default(SceSaveDataMount2);
				sceSaveDataMount.userId = InitialUser();
				sceSaveDataMount.dirName = num;
				sceSaveDataMount.blocks = 32768uL;
				sceSaveDataMount.mountMode = mm;
				SceSaveDataMount2 mount = sceSaveDataMount;
				SceSaveDataMountResult mountResult = default(SceSaveDataMountResult);
				ps4.WriteMemory(pid, num, value);
				mp = Mount(mount, mountResult);
				ps4.FreeMemory(pid, num, Marshal.SizeOf(typeof(SceSaveDataDirName)));
			}
		}

		private void Unmount_Button_Click(object sender, EventArgs e)
		{
			if (mp != null)
			{
				SceSaveDataMountPoint sceSaveDataMountPoint = default(SceSaveDataMountPoint);
				sceSaveDataMountPoint.data = mp;
				SceSaveDataMountPoint mountPoint = sceSaveDataMountPoint;
				Unmount(mountPoint);
			}
		}

		private void Processes_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			pid = ((Process)Processes_ComboBox.SelectedItem).pid;
		}

		private int InitialUser()
		{
			ulong num = ps4.AllocateMemory(pid, 4);
			ps4.Call(pid, stub, libSceUserServiceBase + 13232, num);
			int result = ps4.ReadMemory<int>(pid, num);
			ps4.FreeMemory(pid, num, 4);
			return result;
		}

		private string[] Find(SceSaveDataDirNameSearchCond searchCond, SceSaveDataDirNameSearchResult searchResult)
		{
			ulong num = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
			ulong num2 = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));
			ps4.WriteMemory(pid, num, searchCond);
			ps4.WriteMemory(pid, num2, searchResult);
			if (ps4.Call(pid, stub, libSceSaveDataBase + 154784, num, num2) == 0L)
			{
				searchResult = ps4.ReadMemory<SceSaveDataDirNameSearchResult>(pid, num2);
				string[] array = new string[searchResult.hitNum];
				for (uint num3 = 0u; num3 < searchResult.hitNum; num3++)
				{
					array[num3] = ps4.ReadMemory<string>(pid, searchResult.dirNames + num3 * 32);
				}
				ps4.FreeMemory(pid, num, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
				ps4.FreeMemory(pid, num2, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));
				return array;
			}
			ps4.FreeMemory(pid, num, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchCond)));
			ps4.FreeMemory(pid, num2, Marshal.SizeOf(typeof(SceSaveDataDirNameSearchResult)));
			return new string[0];
		}

		private string Mount(SceSaveDataMount2 mount, SceSaveDataMountResult mountResult)
		{
			ulong num = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMount2)));
			ulong num2 = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMountResult)));
			ps4.WriteMemory(pid, num, mount);
			ps4.WriteMemory(pid, num2, mountResult);
			if (ps4.Call(pid, stub, libSceSaveDataBase + 150496, num, num2) == 0L)
			{
				mountResult = ps4.ReadMemory<SceSaveDataMountResult>(pid, num2);
				ps4.FreeMemory(pid, num, Marshal.SizeOf(typeof(SceSaveDataMount2)));
				ps4.FreeMemory(pid, num2, Marshal.SizeOf(typeof(SceSaveDataMountResult)));
				return mountResult.mountPoint.data;
			}
			ps4.FreeMemory(pid, num, Marshal.SizeOf(typeof(SceSaveDataMount2)));
			ps4.FreeMemory(pid, num2, Marshal.SizeOf(typeof(SceSaveDataMountResult)));
			return "";
		}

		private void Unmount(SceSaveDataMountPoint mountPoint)
		{
			ulong num = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMountPoint)));
			ps4.WriteMemory(pid, num, mountPoint);
			ps4.Call(pid, stub, libSceSaveDataBase + 151744, num);
			ps4.FreeMemory(pid, num, Marshal.SizeOf(typeof(SceSaveDataMountPoint)));
			mp = null;
		}

		private string TransferMount(SceSaveDataTransferringMount mount, SceSaveDataMountResult mountResult)
		{
			ulong num = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataTransferringMount)));
			ulong num2 = ps4.AllocateMemory(pid, Marshal.SizeOf(typeof(SceSaveDataMountResult)));
			ps4.WriteMemory(pid, num, mount);
			ps4.WriteMemory(pid, num2, mountResult);
			if (ps4.Call(pid, stub, libSceSaveDataBase + 151408, num, num2) == 0L)
			{
				mountResult = ps4.ReadMemory<SceSaveDataMountResult>(pid, num2);
				ps4.FreeMemory(pid, num, Marshal.SizeOf(typeof(SceSaveDataTransferringMount)));
				ps4.FreeMemory(pid, num2, Marshal.SizeOf(typeof(SceSaveDataMountResult)));
				return mountResult.mountPoint.data;
			}
			ps4.FreeMemory(pid, num, Marshal.SizeOf(typeof(SceSaveDataTransferringMount)));
			ps4.FreeMemory(pid, num2, Marshal.SizeOf(typeof(SceSaveDataMountResult)));
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
				mm = 1u;
				break;
			case 1:
				mm = 16u;
				break;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			FindIP_Button = new System.Windows.Forms.Button();
			IP_TextBox = new System.Windows.Forms.TextBox();
			Connect_Button = new System.Windows.Forms.Button();
			RefreshProc_Button = new System.Windows.Forms.Button();
			Processes_ComboBox = new System.Windows.Forms.ComboBox();
			Setup_Button = new System.Windows.Forms.Button();
			FindDirs_Button = new System.Windows.Forms.Button();
			Dirs_ComboBox = new System.Windows.Forms.ComboBox();
			Mount_Button = new System.Windows.Forms.Button();
			Unmount_Button = new System.Windows.Forms.Button();
			Q_Button = new System.Windows.Forms.Button();
			MountMode_ComboBox = new System.Windows.Forms.ComboBox();
			SuspendLayout();
			FindIP_Button.Location = new System.Drawing.Point(12, 12);
			FindIP_Button.Name = "FindIP_Button";
			FindIP_Button.Size = new System.Drawing.Size(129, 21);
			FindIP_Button.TabIndex = 0;
			FindIP_Button.Text = "Find Playstation";
			FindIP_Button.UseVisualStyleBackColor = true;
			FindIP_Button.Click += new System.EventHandler(FindIP_Button_Click);
			IP_TextBox.Location = new System.Drawing.Point(147, 13);
			IP_TextBox.Name = "IP_TextBox";
			IP_TextBox.Size = new System.Drawing.Size(108, 20);
			IP_TextBox.TabIndex = 1;
			Connect_Button.Location = new System.Drawing.Point(261, 13);
			Connect_Button.Name = "Connect_Button";
			Connect_Button.Size = new System.Drawing.Size(129, 21);
			Connect_Button.TabIndex = 2;
			Connect_Button.Text = "Connnect";
			Connect_Button.UseVisualStyleBackColor = true;
			Connect_Button.Click += new System.EventHandler(Connect_Button_Click);
			RefreshProc_Button.Location = new System.Drawing.Point(12, 39);
			RefreshProc_Button.Name = "RefreshProc_Button";
			RefreshProc_Button.Size = new System.Drawing.Size(182, 21);
			RefreshProc_Button.TabIndex = 3;
			RefreshProc_Button.Text = "Refresh Processes";
			RefreshProc_Button.UseVisualStyleBackColor = true;
			RefreshProc_Button.Click += new System.EventHandler(RefreshProc_Button_Click);
			Processes_ComboBox.FormattingEnabled = true;
			Processes_ComboBox.Location = new System.Drawing.Point(200, 39);
			Processes_ComboBox.Name = "Processes_ComboBox";
			Processes_ComboBox.Size = new System.Drawing.Size(211, 21);
			Processes_ComboBox.TabIndex = 4;
			Processes_ComboBox.SelectedIndexChanged += new System.EventHandler(Processes_ComboBox_SelectedIndexChanged);
			Setup_Button.Location = new System.Drawing.Point(12, 66);
			Setup_Button.Name = "Setup_Button";
			Setup_Button.Size = new System.Drawing.Size(400, 23);
			Setup_Button.TabIndex = 5;
			Setup_Button.Text = "Setup";
			Setup_Button.UseVisualStyleBackColor = true;
			Setup_Button.Click += new System.EventHandler(Setup_Button_Click);
			FindDirs_Button.Location = new System.Drawing.Point(12, 97);
			FindDirs_Button.Name = "FindDirs_Button";
			FindDirs_Button.Size = new System.Drawing.Size(182, 21);
			FindDirs_Button.TabIndex = 6;
			FindDirs_Button.Text = "Find Dirs";
			FindDirs_Button.UseVisualStyleBackColor = true;
			FindDirs_Button.Click += new System.EventHandler(FindDirs_Button_Click);
			Dirs_ComboBox.FormattingEnabled = true;
			Dirs_ComboBox.Location = new System.Drawing.Point(200, 97);
			Dirs_ComboBox.Name = "Dirs_ComboBox";
			Dirs_ComboBox.Size = new System.Drawing.Size(211, 21);
			Dirs_ComboBox.TabIndex = 7;
			Mount_Button.Location = new System.Drawing.Point(12, 124);
			Mount_Button.Name = "Mount_Button";
			Mount_Button.Size = new System.Drawing.Size(182, 21);
			Mount_Button.TabIndex = 8;
			Mount_Button.Text = "Mount";
			Mount_Button.UseVisualStyleBackColor = true;
			Mount_Button.Click += new System.EventHandler(Mount_Button_Click);
			Unmount_Button.Location = new System.Drawing.Point(13, 151);
			Unmount_Button.Name = "Unmount_Button";
			Unmount_Button.Size = new System.Drawing.Size(399, 23);
			Unmount_Button.TabIndex = 9;
			Unmount_Button.Text = "Unmount";
			Unmount_Button.UseVisualStyleBackColor = true;
			Unmount_Button.Click += new System.EventHandler(Unmount_Button_Click);
			Q_Button.Location = new System.Drawing.Point(391, 13);
			Q_Button.Name = "Q_Button";
			Q_Button.Size = new System.Drawing.Size(21, 21);
			Q_Button.TabIndex = 10;
			Q_Button.Text = "?";
			Q_Button.UseVisualStyleBackColor = true;
			Q_Button.Click += new System.EventHandler(Q_Button_Click);
			MountMode_ComboBox.FormattingEnabled = true;
			MountMode_ComboBox.Items.AddRange(new object[2]
			{
				"READ ONLY",
				"READ/WRITE"
			});
			MountMode_ComboBox.Location = new System.Drawing.Point(200, 124);
			MountMode_ComboBox.Name = "MountMode_ComboBox";
			MountMode_ComboBox.Size = new System.Drawing.Size(211, 21);
			MountMode_ComboBox.TabIndex = 11;
			MountMode_ComboBox.SelectedIndexChanged += new System.EventHandler(MountMode_ComboBox_SelectedIndexChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(424, 186);
			base.Controls.Add(MountMode_ComboBox);
			base.Controls.Add(Q_Button);
			base.Controls.Add(Unmount_Button);
			base.Controls.Add(Mount_Button);
			base.Controls.Add(Dirs_ComboBox);
			base.Controls.Add(FindDirs_Button);
			base.Controls.Add(Setup_Button);
			base.Controls.Add(Processes_ComboBox);
			base.Controls.Add(RefreshProc_Button);
			base.Controls.Add(Connect_Button);
			base.Controls.Add(IP_TextBox);
			base.Controls.Add(FindIP_Button);
			base.Name = "Mainfrm";
			Text = "Playstation 4 Save Mounter";
			ResumeLayout(performLayout: false);
			PerformLayout();
		}
	}
}
