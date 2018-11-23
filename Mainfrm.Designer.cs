namespace PS4SDT
{
    partial class Mainfrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.IP_TextBox = new System.Windows.Forms.TextBox();
            this.Connect_Button = new System.Windows.Forms.Button();
            this.RefreshProc_Button = new System.Windows.Forms.Button();
            this.Processes_ComboBox = new System.Windows.Forms.ComboBox();
            this.Setup_Button = new System.Windows.Forms.Button();
            this.FindDirs_Button = new System.Windows.Forms.Button();
            this.Dirs_ComboBox = new System.Windows.Forms.ComboBox();
            this.Mount_Button = new System.Windows.Forms.Button();
            this.Unmount_Button = new System.Windows.Forms.Button();
            this.Q_Button = new System.Windows.Forms.Button();
            this.MountMode_ComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Title_TextBox = new System.Windows.Forms.TextBox();
            this.Title_Label = new System.Windows.Forms.Label();
            this.Subtitle_Label = new System.Windows.Forms.Label();
            this.Subtitle_TextBox = new System.Windows.Forms.TextBox();
            this.Detail_Label = new System.Windows.Forms.Label();
            this.Detail_TextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Date_Label = new System.Windows.Forms.Label();
            this.Date_TextBox = new System.Windows.Forms.TextBox();
            this.Status_Label = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // IP_TextBox
            // 
            this.IP_TextBox.Location = new System.Drawing.Point(6, 19);
            this.IP_TextBox.Name = "IP_TextBox";
            this.IP_TextBox.Size = new System.Drawing.Size(231, 20);
            this.IP_TextBox.TabIndex = 1;
            // 
            // Connect_Button
            // 
            this.Connect_Button.Location = new System.Drawing.Point(243, 19);
            this.Connect_Button.Name = "Connect_Button";
            this.Connect_Button.Size = new System.Drawing.Size(129, 21);
            this.Connect_Button.TabIndex = 2;
            this.Connect_Button.Text = "Connnect";
            this.Connect_Button.UseVisualStyleBackColor = true;
            this.Connect_Button.Click += new System.EventHandler(this.Connect_Button_Click);
            // 
            // RefreshProc_Button
            // 
            this.RefreshProc_Button.Location = new System.Drawing.Point(5, 45);
            this.RefreshProc_Button.Name = "RefreshProc_Button";
            this.RefreshProc_Button.Size = new System.Drawing.Size(182, 21);
            this.RefreshProc_Button.TabIndex = 3;
            this.RefreshProc_Button.Text = "Refresh Processes";
            this.RefreshProc_Button.UseVisualStyleBackColor = true;
            this.RefreshProc_Button.Click += new System.EventHandler(this.RefreshProc_Button_Click);
            // 
            // Processes_ComboBox
            // 
            this.Processes_ComboBox.FormattingEnabled = true;
            this.Processes_ComboBox.Location = new System.Drawing.Point(193, 45);
            this.Processes_ComboBox.Name = "Processes_ComboBox";
            this.Processes_ComboBox.Size = new System.Drawing.Size(201, 21);
            this.Processes_ComboBox.TabIndex = 4;
            this.Processes_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Processes_ComboBox_SelectedIndexChanged);
            // 
            // Setup_Button
            // 
            this.Setup_Button.Location = new System.Drawing.Point(5, 72);
            this.Setup_Button.Name = "Setup_Button";
            this.Setup_Button.Size = new System.Drawing.Size(389, 23);
            this.Setup_Button.TabIndex = 5;
            this.Setup_Button.Text = "Setup";
            this.Setup_Button.UseVisualStyleBackColor = true;
            this.Setup_Button.Click += new System.EventHandler(this.Setup_Button_Click);
            // 
            // FindDirs_Button
            // 
            this.FindDirs_Button.Location = new System.Drawing.Point(5, 103);
            this.FindDirs_Button.Name = "FindDirs_Button";
            this.FindDirs_Button.Size = new System.Drawing.Size(182, 21);
            this.FindDirs_Button.TabIndex = 6;
            this.FindDirs_Button.Text = "Find Save Directories";
            this.FindDirs_Button.UseVisualStyleBackColor = true;
            this.FindDirs_Button.Click += new System.EventHandler(this.FindDirs_Button_Click);
            // 
            // Dirs_ComboBox
            // 
            this.Dirs_ComboBox.FormattingEnabled = true;
            this.Dirs_ComboBox.Location = new System.Drawing.Point(193, 103);
            this.Dirs_ComboBox.Name = "Dirs_ComboBox";
            this.Dirs_ComboBox.Size = new System.Drawing.Size(201, 21);
            this.Dirs_ComboBox.TabIndex = 7;
            this.Dirs_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Dirs_ComboBox_SelectedIndexChanged);
            // 
            // Mount_Button
            // 
            this.Mount_Button.Location = new System.Drawing.Point(5, 130);
            this.Mount_Button.Name = "Mount_Button";
            this.Mount_Button.Size = new System.Drawing.Size(182, 21);
            this.Mount_Button.TabIndex = 8;
            this.Mount_Button.Text = "Mount Save";
            this.Mount_Button.UseVisualStyleBackColor = true;
            this.Mount_Button.Click += new System.EventHandler(this.Mount_Button_Click);
            // 
            // Unmount_Button
            // 
            this.Unmount_Button.Location = new System.Drawing.Point(5, 157);
            this.Unmount_Button.Name = "Unmount_Button";
            this.Unmount_Button.Size = new System.Drawing.Size(389, 23);
            this.Unmount_Button.TabIndex = 9;
            this.Unmount_Button.Text = "Unmount Save";
            this.Unmount_Button.UseVisualStyleBackColor = true;
            this.Unmount_Button.Click += new System.EventHandler(this.Unmount_Button_Click);
            // 
            // Q_Button
            // 
            this.Q_Button.Location = new System.Drawing.Point(373, 19);
            this.Q_Button.Name = "Q_Button";
            this.Q_Button.Size = new System.Drawing.Size(21, 21);
            this.Q_Button.TabIndex = 10;
            this.Q_Button.Text = "?";
            this.Q_Button.UseVisualStyleBackColor = true;
            this.Q_Button.Click += new System.EventHandler(this.Q_Button_Click);
            // 
            // MountMode_ComboBox
            // 
            this.MountMode_ComboBox.FormattingEnabled = true;
            this.MountMode_ComboBox.Items.AddRange(new object[] {
            "READ ONLY",
            "READ/WRITE"});
            this.MountMode_ComboBox.Location = new System.Drawing.Point(193, 130);
            this.MountMode_ComboBox.Name = "MountMode_ComboBox";
            this.MountMode_ComboBox.Size = new System.Drawing.Size(201, 21);
            this.MountMode_ComboBox.TabIndex = 11;
            this.MountMode_ComboBox.SelectedIndexChanged += new System.EventHandler(this.MountMode_ComboBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Date_Label);
            this.groupBox1.Controls.Add(this.Date_TextBox);
            this.groupBox1.Controls.Add(this.Detail_Label);
            this.groupBox1.Controls.Add(this.Detail_TextBox);
            this.groupBox1.Controls.Add(this.Subtitle_Label);
            this.groupBox1.Controls.Add(this.Subtitle_TextBox);
            this.groupBox1.Controls.Add(this.Title_Label);
            this.groupBox1.Controls.Add(this.Title_TextBox);
            this.groupBox1.Location = new System.Drawing.Point(412, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 220);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "save data parameters";
            // 
            // Title_TextBox
            // 
            this.Title_TextBox.Location = new System.Drawing.Point(6, 42);
            this.Title_TextBox.Name = "Title_TextBox";
            this.Title_TextBox.ReadOnly = true;
            this.Title_TextBox.Size = new System.Drawing.Size(214, 20);
            this.Title_TextBox.TabIndex = 0;
            // 
            // Title_Label
            // 
            this.Title_Label.AutoSize = true;
            this.Title_Label.Location = new System.Drawing.Point(6, 26);
            this.Title_Label.Name = "Title_Label";
            this.Title_Label.Size = new System.Drawing.Size(26, 13);
            this.Title_Label.TabIndex = 1;
            this.Title_Label.Text = "title:";
            // 
            // Subtitle_Label
            // 
            this.Subtitle_Label.AutoSize = true;
            this.Subtitle_Label.Location = new System.Drawing.Point(6, 69);
            this.Subtitle_Label.Name = "Subtitle_Label";
            this.Subtitle_Label.Size = new System.Drawing.Size(43, 13);
            this.Subtitle_Label.TabIndex = 3;
            this.Subtitle_Label.Text = "subtitle:";
            // 
            // Subtitle_TextBox
            // 
            this.Subtitle_TextBox.Location = new System.Drawing.Point(6, 85);
            this.Subtitle_TextBox.Name = "Subtitle_TextBox";
            this.Subtitle_TextBox.ReadOnly = true;
            this.Subtitle_TextBox.Size = new System.Drawing.Size(214, 20);
            this.Subtitle_TextBox.TabIndex = 2;
            // 
            // Detail_Label
            // 
            this.Detail_Label.AutoSize = true;
            this.Detail_Label.Location = new System.Drawing.Point(6, 108);
            this.Detail_Label.Name = "Detail_Label";
            this.Detail_Label.Size = new System.Drawing.Size(35, 13);
            this.Detail_Label.TabIndex = 5;
            this.Detail_Label.Text = "detail:";
            // 
            // Detail_TextBox
            // 
            this.Detail_TextBox.Location = new System.Drawing.Point(6, 124);
            this.Detail_TextBox.Multiline = true;
            this.Detail_TextBox.Name = "Detail_TextBox";
            this.Detail_TextBox.ReadOnly = true;
            this.Detail_TextBox.Size = new System.Drawing.Size(214, 51);
            this.Detail_TextBox.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Status_Label);
            this.groupBox2.Controls.Add(this.IP_TextBox);
            this.groupBox2.Controls.Add(this.Connect_Button);
            this.groupBox2.Controls.Add(this.MountMode_ComboBox);
            this.groupBox2.Controls.Add(this.RefreshProc_Button);
            this.groupBox2.Controls.Add(this.Q_Button);
            this.groupBox2.Controls.Add(this.Processes_ComboBox);
            this.groupBox2.Controls.Add(this.Unmount_Button);
            this.groupBox2.Controls.Add(this.Setup_Button);
            this.groupBox2.Controls.Add(this.Mount_Button);
            this.groupBox2.Controls.Add(this.FindDirs_Button);
            this.groupBox2.Controls.Add(this.Dirs_ComboBox);
            this.groupBox2.Location = new System.Drawing.Point(6, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 220);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            // 
            // Date_Label
            // 
            this.Date_Label.AutoSize = true;
            this.Date_Label.Location = new System.Drawing.Point(6, 178);
            this.Date_Label.Name = "Date_Label";
            this.Date_Label.Size = new System.Drawing.Size(31, 13);
            this.Date_Label.TabIndex = 7;
            this.Date_Label.Text = "date:";
            // 
            // Date_TextBox
            // 
            this.Date_TextBox.Location = new System.Drawing.Point(6, 194);
            this.Date_TextBox.Name = "Date_TextBox";
            this.Date_TextBox.ReadOnly = true;
            this.Date_TextBox.Size = new System.Drawing.Size(214, 20);
            this.Date_TextBox.TabIndex = 6;
            // 
            // Status_Label
            // 
            this.Status_Label.AutoSize = true;
            this.Status_Label.Location = new System.Drawing.Point(3, 197);
            this.Status_Label.Name = "Status_Label";
            this.Status_Label.Size = new System.Drawing.Size(40, 13);
            this.Status_Label.TabIndex = 12;
            this.Status_Label.Text = "Status:";
            // 
            // Mainfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 229);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Mainfrm";
            this.Text = "Playstation 4 Save Mounter 1.2 [ps4debug]";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox IP_TextBox;
        private System.Windows.Forms.Button Connect_Button;
        private System.Windows.Forms.Button RefreshProc_Button;
        private System.Windows.Forms.ComboBox Processes_ComboBox;
        private System.Windows.Forms.Button Setup_Button;
        private System.Windows.Forms.Button FindDirs_Button;
        private System.Windows.Forms.ComboBox Dirs_ComboBox;
        private System.Windows.Forms.Button Mount_Button;
        private System.Windows.Forms.Button Unmount_Button;
        private System.Windows.Forms.Button Q_Button;
        private System.Windows.Forms.ComboBox MountMode_ComboBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label Detail_Label;
        private System.Windows.Forms.TextBox Detail_TextBox;
        private System.Windows.Forms.Label Subtitle_Label;
        private System.Windows.Forms.TextBox Subtitle_TextBox;
        private System.Windows.Forms.Label Title_Label;
        private System.Windows.Forms.TextBox Title_TextBox;
        private System.Windows.Forms.Label Date_Label;
        private System.Windows.Forms.TextBox Date_TextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label Status_Label;
    }
}

