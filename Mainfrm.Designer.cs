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
            this.FindIP_Button = new System.Windows.Forms.Button();
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
            this.SuspendLayout();
            // 
            // FindIP_Button
            // 
            this.FindIP_Button.Location = new System.Drawing.Point(12, 12);
            this.FindIP_Button.Name = "FindIP_Button";
            this.FindIP_Button.Size = new System.Drawing.Size(129, 21);
            this.FindIP_Button.TabIndex = 0;
            this.FindIP_Button.Text = "Find Playstation";
            this.FindIP_Button.UseVisualStyleBackColor = true;
            this.FindIP_Button.Click += new System.EventHandler(this.FindIP_Button_Click);
            // 
            // IP_TextBox
            // 
            this.IP_TextBox.Location = new System.Drawing.Point(147, 13);
            this.IP_TextBox.Name = "IP_TextBox";
            this.IP_TextBox.Size = new System.Drawing.Size(108, 20);
            this.IP_TextBox.TabIndex = 1;
            // 
            // Connect_Button
            // 
            this.Connect_Button.Location = new System.Drawing.Point(261, 13);
            this.Connect_Button.Name = "Connect_Button";
            this.Connect_Button.Size = new System.Drawing.Size(129, 21);
            this.Connect_Button.TabIndex = 2;
            this.Connect_Button.Text = "Connnect";
            this.Connect_Button.UseVisualStyleBackColor = true;
            this.Connect_Button.Click += new System.EventHandler(this.Connect_Button_Click);
            // 
            // RefreshProc_Button
            // 
            this.RefreshProc_Button.Location = new System.Drawing.Point(12, 39);
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
            this.Processes_ComboBox.Location = new System.Drawing.Point(200, 39);
            this.Processes_ComboBox.Name = "Processes_ComboBox";
            this.Processes_ComboBox.Size = new System.Drawing.Size(211, 21);
            this.Processes_ComboBox.TabIndex = 4;
            this.Processes_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Processes_ComboBox_SelectedIndexChanged);
            // 
            // Setup_Button
            // 
            this.Setup_Button.Location = new System.Drawing.Point(12, 66);
            this.Setup_Button.Name = "Setup_Button";
            this.Setup_Button.Size = new System.Drawing.Size(400, 23);
            this.Setup_Button.TabIndex = 5;
            this.Setup_Button.Text = "Setup";
            this.Setup_Button.UseVisualStyleBackColor = true;
            this.Setup_Button.Click += new System.EventHandler(this.Setup_Button_Click);
            // 
            // FindDirs_Button
            // 
            this.FindDirs_Button.Location = new System.Drawing.Point(12, 97);
            this.FindDirs_Button.Name = "FindDirs_Button";
            this.FindDirs_Button.Size = new System.Drawing.Size(182, 21);
            this.FindDirs_Button.TabIndex = 6;
            this.FindDirs_Button.Text = "Find Dirs";
            this.FindDirs_Button.UseVisualStyleBackColor = true;
            this.FindDirs_Button.Click += new System.EventHandler(this.FindDirs_Button_Click);
            // 
            // Dirs_ComboBox
            // 
            this.Dirs_ComboBox.FormattingEnabled = true;
            this.Dirs_ComboBox.Location = new System.Drawing.Point(200, 97);
            this.Dirs_ComboBox.Name = "Dirs_ComboBox";
            this.Dirs_ComboBox.Size = new System.Drawing.Size(211, 21);
            this.Dirs_ComboBox.TabIndex = 7;
            // 
            // Mount_Button
            // 
            this.Mount_Button.Location = new System.Drawing.Point(12, 124);
            this.Mount_Button.Name = "Mount_Button";
            this.Mount_Button.Size = new System.Drawing.Size(182, 21);
            this.Mount_Button.TabIndex = 8;
            this.Mount_Button.Text = "Mount";
            this.Mount_Button.UseVisualStyleBackColor = true;
            this.Mount_Button.Click += new System.EventHandler(this.Mount_Button_Click);
            // 
            // Unmount_Button
            // 
            this.Unmount_Button.Location = new System.Drawing.Point(13, 151);
            this.Unmount_Button.Name = "Unmount_Button";
            this.Unmount_Button.Size = new System.Drawing.Size(399, 23);
            this.Unmount_Button.TabIndex = 9;
            this.Unmount_Button.Text = "Unmount";
            this.Unmount_Button.UseVisualStyleBackColor = true;
            this.Unmount_Button.Click += new System.EventHandler(this.Unmount_Button_Click);
            // 
            // Q_Button
            // 
            this.Q_Button.Location = new System.Drawing.Point(391, 13);
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
            this.MountMode_ComboBox.Location = new System.Drawing.Point(200, 124);
            this.MountMode_ComboBox.Name = "MountMode_ComboBox";
            this.MountMode_ComboBox.Size = new System.Drawing.Size(211, 21);
            this.MountMode_ComboBox.TabIndex = 11;
            this.MountMode_ComboBox.SelectedIndexChanged += new System.EventHandler(this.MountMode_ComboBox_SelectedIndexChanged);
            // 
            // Mainfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 186);
            this.Controls.Add(this.MountMode_ComboBox);
            this.Controls.Add(this.Q_Button);
            this.Controls.Add(this.Unmount_Button);
            this.Controls.Add(this.Mount_Button);
            this.Controls.Add(this.Dirs_ComboBox);
            this.Controls.Add(this.FindDirs_Button);
            this.Controls.Add(this.Setup_Button);
            this.Controls.Add(this.Processes_ComboBox);
            this.Controls.Add(this.RefreshProc_Button);
            this.Controls.Add(this.Connect_Button);
            this.Controls.Add(this.IP_TextBox);
            this.Controls.Add(this.FindIP_Button);
            this.Name = "Mainfrm";
            this.Text = "Playstation 4 Save Mounter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button FindIP_Button;
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
    }
}

