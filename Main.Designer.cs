namespace PS4Saves
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.setupButton = new System.Windows.Forms.Button();
            this.userComboBox = new System.Windows.Forms.ComboBox();
            this.dirsComboBox = new System.Windows.Forms.ComboBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.mountButton = new System.Windows.Forms.Button();
            this.unmountButton = new System.Windows.Forms.Button();
            this.connectionGroupBox = new System.Windows.Forms.GroupBox();
            this.getGamesButton = new System.Windows.Forms.Button();
            this.gamesComboBox = new System.Windows.Forms.ComboBox();
            this.payloadButton = new System.Windows.Forms.Button();
            this.ipLabel = new System.Windows.Forms.Label();
            this.createGroupBox = new System.Windows.Forms.GroupBox();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.sizeTrackBar = new System.Windows.Forms.TrackBar();
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.createButton = new System.Windows.Forms.Button();
            this.mountGroupBox = new System.Windows.Forms.GroupBox();
            this.infoGroupBox = new System.Windows.Forms.GroupBox();
            this.dateTextBox = new System.Windows.Forms.TextBox();
            this.dateLabel = new System.Windows.Forms.Label();
            this.detailsTextBox = new System.Windows.Forms.TextBox();
            this.detailsLabel = new System.Windows.Forms.Label();
            this.subtitleTextBox = new System.Windows.Forms.TextBox();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.sizeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusLabel = new System.Windows.Forms.Label();
            this.connectionGroupBox.SuspendLayout();
            this.createGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).BeginInit();
            this.mountGroupBox.SuspendLayout();
            this.infoGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ipTextBox
            // 
            this.ipTextBox.Location = new System.Drawing.Point(70, 19);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(117, 20);
            this.ipTextBox.TabIndex = 0;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(284, 19);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(90, 20);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // setupButton
            // 
            this.setupButton.Location = new System.Drawing.Point(7, 45);
            this.setupButton.Name = "setupButton";
            this.setupButton.Size = new System.Drawing.Size(181, 21);
            this.setupButton.TabIndex = 3;
            this.setupButton.Text = "Setup";
            this.setupButton.UseVisualStyleBackColor = true;
            this.setupButton.Click += new System.EventHandler(this.setupButton_Click);
            // 
            // userComboBox
            // 
            this.userComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.userComboBox.FormattingEnabled = true;
            this.userComboBox.Location = new System.Drawing.Point(193, 45);
            this.userComboBox.Name = "userComboBox";
            this.userComboBox.Size = new System.Drawing.Size(181, 21);
            this.userComboBox.TabIndex = 4;
            this.userComboBox.SelectedIndexChanged += new System.EventHandler(this.userComboBox_SelectedIndexChanged);
            // 
            // dirsComboBox
            // 
            this.dirsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dirsComboBox.FormattingEnabled = true;
            this.dirsComboBox.Location = new System.Drawing.Point(193, 19);
            this.dirsComboBox.Name = "dirsComboBox";
            this.dirsComboBox.Size = new System.Drawing.Size(180, 21);
            this.dirsComboBox.TabIndex = 1;
            this.dirsComboBox.SelectedIndexChanged += new System.EventHandler(this.dirsComboBox_SelectedIndexChanged);
            // 
            // searchButton
            // 
            this.searchButton.Location = new System.Drawing.Point(6, 19);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(181, 21);
            this.searchButton.TabIndex = 0;
            this.searchButton.Text = "Search";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // mountButton
            // 
            this.mountButton.Location = new System.Drawing.Point(6, 48);
            this.mountButton.Name = "mountButton";
            this.mountButton.Size = new System.Drawing.Size(181, 23);
            this.mountButton.TabIndex = 2;
            this.mountButton.Text = "Mount";
            this.mountButton.UseVisualStyleBackColor = true;
            this.mountButton.Click += new System.EventHandler(this.mountButton_Click);
            // 
            // unmountButton
            // 
            this.unmountButton.Location = new System.Drawing.Point(192, 48);
            this.unmountButton.Name = "unmountButton";
            this.unmountButton.Size = new System.Drawing.Size(181, 23);
            this.unmountButton.TabIndex = 3;
            this.unmountButton.Text = "Unmount";
            this.unmountButton.UseVisualStyleBackColor = true;
            this.unmountButton.Click += new System.EventHandler(this.unmountButton_Click);
            // 
            // connectionGroupBox
            // 
            this.connectionGroupBox.Controls.Add(this.getGamesButton);
            this.connectionGroupBox.Controls.Add(this.gamesComboBox);
            this.connectionGroupBox.Controls.Add(this.payloadButton);
            this.connectionGroupBox.Controls.Add(this.ipLabel);
            this.connectionGroupBox.Controls.Add(this.ipTextBox);
            this.connectionGroupBox.Controls.Add(this.connectButton);
            this.connectionGroupBox.Controls.Add(this.setupButton);
            this.connectionGroupBox.Controls.Add(this.userComboBox);
            this.connectionGroupBox.Location = new System.Drawing.Point(7, 12);
            this.connectionGroupBox.Name = "connectionGroupBox";
            this.connectionGroupBox.Size = new System.Drawing.Size(379, 105);
            this.connectionGroupBox.TabIndex = 0;
            this.connectionGroupBox.TabStop = false;
            this.connectionGroupBox.Text = "Connection";
            // 
            // getGamesButton
            // 
            this.getGamesButton.Location = new System.Drawing.Point(6, 72);
            this.getGamesButton.Name = "getGamesButton";
            this.getGamesButton.Size = new System.Drawing.Size(181, 21);
            this.getGamesButton.TabIndex = 5;
            this.getGamesButton.Text = "Get Games";
            this.getGamesButton.UseVisualStyleBackColor = true;
            this.getGamesButton.Click += new System.EventHandler(this.getGamesButton_Click);
            // 
            // gamesComboBox
            // 
            this.gamesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gamesComboBox.FormattingEnabled = true;
            this.gamesComboBox.Location = new System.Drawing.Point(192, 72);
            this.gamesComboBox.Name = "gamesComboBox";
            this.gamesComboBox.Size = new System.Drawing.Size(181, 21);
            this.gamesComboBox.TabIndex = 6;
            this.gamesComboBox.SelectedIndexChanged += new System.EventHandler(this.gamesComboBox_SelectedIndexChanged);
            // 
            // payloadButton
            // 
            this.payloadButton.Location = new System.Drawing.Point(192, 19);
            this.payloadButton.Name = "payloadButton";
            this.payloadButton.Size = new System.Drawing.Size(90, 20);
            this.payloadButton.TabIndex = 1;
            this.payloadButton.Text = "Send Payload";
            this.payloadButton.UseVisualStyleBackColor = true;
            this.payloadButton.Click += new System.EventHandler(this.payloadButton_Click);
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(6, 22);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(58, 13);
            this.ipLabel.TabIndex = 6;
            this.ipLabel.Text = "ip address:";
            // 
            // createGroupBox
            // 
            this.createGroupBox.Controls.Add(this.sizeLabel);
            this.createGroupBox.Controls.Add(this.sizeTrackBar);
            this.createGroupBox.Controls.Add(this.nameLabel);
            this.createGroupBox.Controls.Add(this.nameTextBox);
            this.createGroupBox.Controls.Add(this.createButton);
            this.createGroupBox.Location = new System.Drawing.Point(7, 210);
            this.createGroupBox.Name = "createGroupBox";
            this.createGroupBox.Size = new System.Drawing.Size(379, 129);
            this.createGroupBox.TabIndex = 2;
            this.createGroupBox.TabStop = false;
            this.createGroupBox.Text = "Create New Saves";
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.Location = new System.Drawing.Point(6, 48);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(76, 13);
            this.sizeLabel.TabIndex = 9;
            this.sizeLabel.Text = "max save size:";
            // 
            // sizeTrackBar
            // 
            this.sizeTrackBar.Location = new System.Drawing.Point(117, 48);
            this.sizeTrackBar.Maximum = 32768;
            this.sizeTrackBar.Minimum = 96;
            this.sizeTrackBar.Name = "sizeTrackBar";
            this.sizeTrackBar.Size = new System.Drawing.Size(257, 45);
            this.sizeTrackBar.TabIndex = 1;
            this.sizeTrackBar.Value = 96;
            this.sizeTrackBar.Scroll += new System.EventHandler(this.sizeTrackBar_Scroll);
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(6, 25);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(105, 13);
            this.nameLabel.TabIndex = 7;
            this.nameLabel.Text = "save directory name:";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(117, 22);
            this.nameTextBox.MaxLength = 31;
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(256, 20);
            this.nameTextBox.TabIndex = 0;
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(5, 99);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(367, 23);
            this.createButton.TabIndex = 2;
            this.createButton.Text = "Create Save";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // mountGroupBox
            // 
            this.mountGroupBox.Controls.Add(this.searchButton);
            this.mountGroupBox.Controls.Add(this.dirsComboBox);
            this.mountGroupBox.Controls.Add(this.mountButton);
            this.mountGroupBox.Controls.Add(this.unmountButton);
            this.mountGroupBox.Location = new System.Drawing.Point(7, 123);
            this.mountGroupBox.Name = "mountGroupBox";
            this.mountGroupBox.Size = new System.Drawing.Size(379, 81);
            this.mountGroupBox.TabIndex = 1;
            this.mountGroupBox.TabStop = false;
            this.mountGroupBox.Text = "Mount Existing Saves";
            // 
            // infoGroupBox
            // 
            this.infoGroupBox.Controls.Add(this.dateTextBox);
            this.infoGroupBox.Controls.Add(this.dateLabel);
            this.infoGroupBox.Controls.Add(this.detailsTextBox);
            this.infoGroupBox.Controls.Add(this.detailsLabel);
            this.infoGroupBox.Controls.Add(this.subtitleTextBox);
            this.infoGroupBox.Controls.Add(this.subtitleLabel);
            this.infoGroupBox.Controls.Add(this.titleTextBox);
            this.infoGroupBox.Controls.Add(this.titleLabel);
            this.infoGroupBox.Location = new System.Drawing.Point(392, 12);
            this.infoGroupBox.Name = "infoGroupBox";
            this.infoGroupBox.Size = new System.Drawing.Size(396, 327);
            this.infoGroupBox.TabIndex = 3;
            this.infoGroupBox.TabStop = false;
            this.infoGroupBox.Text = "Save Info";
            // 
            // dateTextBox
            // 
            this.dateTextBox.Location = new System.Drawing.Point(9, 294);
            this.dateTextBox.Name = "dateTextBox";
            this.dateTextBox.ReadOnly = true;
            this.dateTextBox.Size = new System.Drawing.Size(381, 20);
            this.dateTextBox.TabIndex = 3;
            // 
            // dateLabel
            // 
            this.dateLabel.AutoSize = true;
            this.dateLabel.Location = new System.Drawing.Point(6, 278);
            this.dateLabel.Name = "dateLabel";
            this.dateLabel.Size = new System.Drawing.Size(31, 13);
            this.dateLabel.TabIndex = 6;
            this.dateLabel.Text = "date:";
            // 
            // detailsTextBox
            // 
            this.detailsTextBox.Location = new System.Drawing.Point(9, 166);
            this.detailsTextBox.Multiline = true;
            this.detailsTextBox.Name = "detailsTextBox";
            this.detailsTextBox.ReadOnly = true;
            this.detailsTextBox.Size = new System.Drawing.Size(381, 109);
            this.detailsTextBox.TabIndex = 2;
            // 
            // detailsLabel
            // 
            this.detailsLabel.AutoSize = true;
            this.detailsLabel.Location = new System.Drawing.Point(6, 150);
            this.detailsLabel.Name = "detailsLabel";
            this.detailsLabel.Size = new System.Drawing.Size(40, 13);
            this.detailsLabel.TabIndex = 4;
            this.detailsLabel.Text = "details:";
            // 
            // subtitleTextBox
            // 
            this.subtitleTextBox.Location = new System.Drawing.Point(9, 101);
            this.subtitleTextBox.Multiline = true;
            this.subtitleTextBox.Name = "subtitleTextBox";
            this.subtitleTextBox.ReadOnly = true;
            this.subtitleTextBox.Size = new System.Drawing.Size(381, 46);
            this.subtitleTextBox.TabIndex = 1;
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.Location = new System.Drawing.Point(6, 85);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(43, 13);
            this.subtitleLabel.TabIndex = 2;
            this.subtitleLabel.Text = "subtitle:";
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new System.Drawing.Point(9, 35);
            this.titleTextBox.Multiline = true;
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.ReadOnly = true;
            this.titleTextBox.Size = new System.Drawing.Size(381, 47);
            this.titleTextBox.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Location = new System.Drawing.Point(6, 19);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(26, 13);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "title:";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(4, 342);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(40, 13);
            this.statusLabel.TabIndex = 13;
            this.statusLabel.Text = "Status:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 362);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.infoGroupBox);
            this.Controls.Add(this.mountGroupBox);
            this.Controls.Add(this.createGroupBox);
            this.Controls.Add(this.connectionGroupBox);
            this.Name = "Main";
            this.Text = "Playstation 4 Save Mounter 1.5 [ps4debug]";
            this.connectionGroupBox.ResumeLayout(false);
            this.connectionGroupBox.PerformLayout();
            this.createGroupBox.ResumeLayout(false);
            this.createGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sizeTrackBar)).EndInit();
            this.mountGroupBox.ResumeLayout(false);
            this.infoGroupBox.ResumeLayout(false);
            this.infoGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button setupButton;
        private System.Windows.Forms.ComboBox userComboBox;
        private System.Windows.Forms.ComboBox dirsComboBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.Button mountButton;
        private System.Windows.Forms.Button unmountButton;
        private System.Windows.Forms.GroupBox connectionGroupBox;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.GroupBox createGroupBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.GroupBox mountGroupBox;
        private System.Windows.Forms.GroupBox infoGroupBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TrackBar sizeTrackBar;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.ToolTip sizeToolTip;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.TextBox dateTextBox;
        private System.Windows.Forms.Label dateLabel;
        private System.Windows.Forms.TextBox detailsTextBox;
        private System.Windows.Forms.Label detailsLabel;
        private System.Windows.Forms.TextBox subtitleTextBox;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button payloadButton;
        private System.Windows.Forms.Button getGamesButton;
        private System.Windows.Forms.ComboBox gamesComboBox;
    }
}

