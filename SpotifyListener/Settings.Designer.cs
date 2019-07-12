namespace SpotifyListener
{
    partial class Settings
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
            this.SpotifyGroupBox = new System.Windows.Forms.GroupBox();
            this.AccessTokenTextBox = new System.Windows.Forms.TextBox();
            this.AccessTokenRevealButton = new System.Windows.Forms.Button();
            this.RevealKeyButton = new System.Windows.Forms.Button();
            this.RefreshTokenTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.DiscordGroupBox = new System.Windows.Forms.GroupBox();
            this.DiscordRichPresenceEnable = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.DiscordPlayState = new System.Windows.Forms.RichTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.DiscordPauseState = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.DiscordPlayDetail = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.DiscordPauseDetail = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ResetButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.trackbar_BlurRadial = new System.Windows.Forms.TrackBar();
            this.NetworkEnable = new System.Windows.Forms.CheckBox();
            this.WebServiceListeningEnable = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.trackbar_VolumeScale = new System.Windows.Forms.TrackBar();
            this.label13 = new System.Windows.Forms.Label();
            this.AlbumColorMode = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.RenderModeCombobox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.RenderStyleCombobox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.RenderFPS = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.ColorDensity = new System.Windows.Forms.TrackBar();
            this.ReverseLEDRender = new System.Windows.Forms.CheckBox();
            this.ColorSettingsButton = new System.Windows.Forms.Button();
            this.ChromaSDKEnable = new System.Windows.Forms.CheckBox();
            this.DisableDesktop = new System.Windows.Forms.CheckBox();
            this.SpotifyGroupBox.SuspendLayout();
            this.DiscordGroupBox.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_BlurRadial)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_VolumeScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorDensity)).BeginInit();
            this.SuspendLayout();
            // 
            // SpotifyGroupBox
            // 
            this.SpotifyGroupBox.Controls.Add(this.AccessTokenTextBox);
            this.SpotifyGroupBox.Controls.Add(this.AccessTokenRevealButton);
            this.SpotifyGroupBox.Controls.Add(this.RevealKeyButton);
            this.SpotifyGroupBox.Controls.Add(this.RefreshTokenTextbox);
            this.SpotifyGroupBox.Controls.Add(this.label2);
            this.SpotifyGroupBox.Controls.Add(this.label1);
            this.SpotifyGroupBox.Location = new System.Drawing.Point(13, 13);
            this.SpotifyGroupBox.Name = "SpotifyGroupBox";
            this.SpotifyGroupBox.Size = new System.Drawing.Size(616, 128);
            this.SpotifyGroupBox.TabIndex = 0;
            this.SpotifyGroupBox.TabStop = false;
            this.SpotifyGroupBox.Text = "Spotify";
            // 
            // AccessTokenTextBox
            // 
            this.AccessTokenTextBox.Location = new System.Drawing.Point(5, 95);
            this.AccessTokenTextBox.Name = "AccessTokenTextBox";
            this.AccessTokenTextBox.PasswordChar = '*';
            this.AccessTokenTextBox.ReadOnly = true;
            this.AccessTokenTextBox.Size = new System.Drawing.Size(516, 20);
            this.AccessTokenTextBox.TabIndex = 6;
            // 
            // AccessTokenRevealButton
            // 
            this.AccessTokenRevealButton.Location = new System.Drawing.Point(527, 90);
            this.AccessTokenRevealButton.Name = "AccessTokenRevealButton";
            this.AccessTokenRevealButton.Size = new System.Drawing.Size(75, 23);
            this.AccessTokenRevealButton.TabIndex = 5;
            this.AccessTokenRevealButton.Text = "Reveal Key";
            this.AccessTokenRevealButton.UseVisualStyleBackColor = true;
            this.AccessTokenRevealButton.Click += new System.EventHandler(this.AccessTokenRevealButton_Click);
            // 
            // RevealKeyButton
            // 
            this.RevealKeyButton.Location = new System.Drawing.Point(526, 29);
            this.RevealKeyButton.Name = "RevealKeyButton";
            this.RevealKeyButton.Size = new System.Drawing.Size(75, 23);
            this.RevealKeyButton.TabIndex = 4;
            this.RevealKeyButton.Text = "Reveal Key";
            this.RevealKeyButton.UseVisualStyleBackColor = true;
            this.RevealKeyButton.Click += new System.EventHandler(this.RevealKeyButton_Click);
            // 
            // RefreshTokenTextbox
            // 
            this.RefreshTokenTextbox.Location = new System.Drawing.Point(6, 32);
            this.RefreshTokenTextbox.Name = "RefreshTokenTextbox";
            this.RefreshTokenTextbox.PasswordChar = '*';
            this.RefreshTokenTextbox.Size = new System.Drawing.Size(516, 20);
            this.RefreshTokenTextbox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Access Token :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Refresh Token :";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(742, 574);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(76, 37);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DiscordGroupBox
            // 
            this.DiscordGroupBox.Controls.Add(this.DiscordRichPresenceEnable);
            this.DiscordGroupBox.Controls.Add(this.label7);
            this.DiscordGroupBox.Controls.Add(this.DiscordPlayState);
            this.DiscordGroupBox.Controls.Add(this.label5);
            this.DiscordGroupBox.Controls.Add(this.DiscordPauseState);
            this.DiscordGroupBox.Controls.Add(this.label6);
            this.DiscordGroupBox.Controls.Add(this.DiscordPlayDetail);
            this.DiscordGroupBox.Controls.Add(this.label4);
            this.DiscordGroupBox.Controls.Add(this.DiscordPauseDetail);
            this.DiscordGroupBox.Controls.Add(this.label3);
            this.DiscordGroupBox.Location = new System.Drawing.Point(13, 148);
            this.DiscordGroupBox.Name = "DiscordGroupBox";
            this.DiscordGroupBox.Size = new System.Drawing.Size(616, 306);
            this.DiscordGroupBox.TabIndex = 2;
            this.DiscordGroupBox.TabStop = false;
            this.DiscordGroupBox.Text = "Discord";
            // 
            // DiscordRichPresenceEnable
            // 
            this.DiscordRichPresenceEnable.AutoSize = true;
            this.DiscordRichPresenceEnable.Location = new System.Drawing.Point(10, 30);
            this.DiscordRichPresenceEnable.Name = "DiscordRichPresenceEnable";
            this.DiscordRichPresenceEnable.Size = new System.Drawing.Size(171, 17);
            this.DiscordRichPresenceEnable.TabIndex = 3;
            this.DiscordRichPresenceEnable.Text = "Enable Discord Rich Presence";
            this.DiscordRichPresenceEnable.UseVisualStyleBackColor = true;
            this.DiscordRichPresenceEnable.CheckedChanged += new System.EventHandler(this.DiscordRichPresenceEnable_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label7.Location = new System.Drawing.Point(6, 299);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 18);
            this.label7.TabIndex = 10;
            // 
            // DiscordPlayState
            // 
            this.DiscordPlayState.Location = new System.Drawing.Point(10, 198);
            this.DiscordPlayState.Name = "DiscordPlayState";
            this.DiscordPlayState.Size = new System.Drawing.Size(222, 89);
            this.DiscordPlayState.TabIndex = 9;
            this.DiscordPlayState.Text = "";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Discord Play State :";
            // 
            // DiscordPauseState
            // 
            this.DiscordPauseState.Location = new System.Drawing.Point(256, 199);
            this.DiscordPauseState.Name = "DiscordPauseState";
            this.DiscordPauseState.Size = new System.Drawing.Size(236, 89);
            this.DiscordPauseState.TabIndex = 7;
            this.DiscordPauseState.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(253, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Discord Pause State :";
            // 
            // DiscordPlayDetail
            // 
            this.DiscordPlayDetail.Location = new System.Drawing.Point(10, 80);
            this.DiscordPlayDetail.Name = "DiscordPlayDetail";
            this.DiscordPlayDetail.Size = new System.Drawing.Size(222, 89);
            this.DiscordPlayDetail.TabIndex = 5;
            this.DiscordPlayDetail.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Discord Play Detail :";
            // 
            // DiscordPauseDetail
            // 
            this.DiscordPauseDetail.Location = new System.Drawing.Point(256, 80);
            this.DiscordPauseDetail.Name = "DiscordPauseDetail";
            this.DiscordPauseDetail.Size = new System.Drawing.Size(236, 89);
            this.DiscordPauseDetail.TabIndex = 3;
            this.DiscordPauseDetail.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(253, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Discord Pause Detail :";
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(851, 574);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(76, 37);
            this.ResetButton.TabIndex = 4;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.DisableDesktop);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.trackbar_BlurRadial);
            this.groupBox3.Controls.Add(this.NetworkEnable);
            this.groupBox3.Controls.Add(this.WebServiceListeningEnable);
            this.groupBox3.Location = new System.Drawing.Point(10, 460);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(620, 163);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Preferences";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 114);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(125, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "Background Blur Radial :";
            // 
            // trackbar_BlurRadial
            // 
            this.trackbar_BlurRadial.Location = new System.Drawing.Point(134, 114);
            this.trackbar_BlurRadial.Minimum = 1;
            this.trackbar_BlurRadial.Name = "trackbar_BlurRadial";
            this.trackbar_BlurRadial.Size = new System.Drawing.Size(355, 45);
            this.trackbar_BlurRadial.TabIndex = 2;
            this.trackbar_BlurRadial.Value = 1;
            // 
            // NetworkEnable
            // 
            this.NetworkEnable.AutoSize = true;
            this.NetworkEnable.Enabled = false;
            this.NetworkEnable.Location = new System.Drawing.Point(259, 25);
            this.NetworkEnable.Name = "NetworkEnable";
            this.NetworkEnable.Size = new System.Drawing.Size(187, 17);
            this.NetworkEnable.TabIndex = 1;
            this.NetworkEnable.Text = "Allow Application To Use Network";
            this.NetworkEnable.UseVisualStyleBackColor = true;
            this.NetworkEnable.Visible = false;
            this.NetworkEnable.CheckedChanged += new System.EventHandler(this.NetworkEnable_CheckedChanged);
            // 
            // WebServiceListeningEnable
            // 
            this.WebServiceListeningEnable.AutoSize = true;
            this.WebServiceListeningEnable.Enabled = false;
            this.WebServiceListeningEnable.Location = new System.Drawing.Point(6, 25);
            this.WebServiceListeningEnable.Name = "WebServiceListeningEnable";
            this.WebServiceListeningEnable.Size = new System.Drawing.Size(232, 17);
            this.WebServiceListeningEnable.TabIndex = 0;
            this.WebServiceListeningEnable.Text = "Enable application control over web service";
            this.WebServiceListeningEnable.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.trackbar_VolumeScale);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.AlbumColorMode);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.RenderModeCombobox);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.RenderStyleCombobox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.RenderFPS);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.ColorDensity);
            this.groupBox2.Controls.Add(this.ReverseLEDRender);
            this.groupBox2.Controls.Add(this.ColorSettingsButton);
            this.groupBox2.Controls.Add(this.ChromaSDKEnable);
            this.groupBox2.Location = new System.Drawing.Point(635, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(366, 361);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Razer Chroma SDK";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 304);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(78, 13);
            this.label15.TabIndex = 27;
            this.label15.Text = "Volume Scale :";
            // 
            // trackbar_VolumeScale
            // 
            this.trackbar_VolumeScale.Location = new System.Drawing.Point(87, 304);
            this.trackbar_VolumeScale.Minimum = 1;
            this.trackbar_VolumeScale.Name = "trackbar_VolumeScale";
            this.trackbar_VolumeScale.Size = new System.Drawing.Size(164, 45);
            this.trackbar_VolumeScale.TabIndex = 26;
            this.trackbar_VolumeScale.Value = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 218);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(102, 13);
            this.label13.TabIndex = 25;
            this.label13.Text = "Album Color Mode : ";
            // 
            // AlbumColorMode
            // 
            this.AlbumColorMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AlbumColorMode.FormattingEnabled = true;
            this.AlbumColorMode.Items.AddRange(new object[] {
            "Dominant",
            "Average"});
            this.AlbumColorMode.Location = new System.Drawing.Point(114, 214);
            this.AlbumColorMode.Name = "AlbumColorMode";
            this.AlbumColorMode.Size = new System.Drawing.Size(232, 21);
            this.AlbumColorMode.TabIndex = 24;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 187);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(81, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "Render Mode : ";
            // 
            // RenderModeCombobox
            // 
            this.RenderModeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RenderModeCombobox.FormattingEnabled = true;
            this.RenderModeCombobox.Items.AddRange(new object[] {
            "Static",
            "Adaptive"});
            this.RenderModeCombobox.Location = new System.Drawing.Point(114, 184);
            this.RenderModeCombobox.Name = "RenderModeCombobox";
            this.RenderModeCombobox.Size = new System.Drawing.Size(232, 21);
            this.RenderModeCombobox.TabIndex = 22;
            this.RenderModeCombobox.SelectedIndexChanged += new System.EventHandler(this.RenderModeCombobox_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 157);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Render Style  :";
            // 
            // RenderStyleCombobox
            // 
            this.RenderStyleCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RenderStyleCombobox.FormattingEnabled = true;
            this.RenderStyleCombobox.Items.AddRange(new object[] {
            "Custom - Progression + Volume",
            "Custom - Peak Volume Meter",
            "Custom - Symmetric Peak Volume Meter",
            "Album Cover - Progression + Volume",
            "Album Cover - Peak Volume Meter",
            "Album Cover - Symmetric Peak Volume Meter",
            "Chroma - Peak Volume Meter"});
            this.RenderStyleCombobox.Location = new System.Drawing.Point(114, 154);
            this.RenderStyleCombobox.Name = "RenderStyleCombobox";
            this.RenderStyleCombobox.Size = new System.Drawing.Size(232, 21);
            this.RenderStyleCombobox.TabIndex = 20;
            this.RenderStyleCombobox.SelectedIndexChanged += new System.EventHandler(this.RenderStyleCombobox_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(3, 116);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(199, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "(Recommed : 60 fps, Maximum : 144 fps)";
            // 
            // RenderFPS
            // 
            this.RenderFPS.Location = new System.Drawing.Point(151, 93);
            this.RenderFPS.Name = "RenderFPS";
            this.RenderFPS.Size = new System.Drawing.Size(66, 20);
            this.RenderFPS.TabIndex = 12;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 96);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(142, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Render Frame-Per-Second : ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 258);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Color Density :";
            // 
            // ColorDensity
            // 
            this.ColorDensity.Location = new System.Drawing.Point(87, 253);
            this.ColorDensity.Minimum = 1;
            this.ColorDensity.Name = "ColorDensity";
            this.ColorDensity.Size = new System.Drawing.Size(164, 45);
            this.ColorDensity.TabIndex = 6;
            this.ColorDensity.Value = 1;
            // 
            // ReverseLEDRender
            // 
            this.ReverseLEDRender.AutoSize = true;
            this.ReverseLEDRender.Location = new System.Drawing.Point(6, 55);
            this.ReverseLEDRender.Name = "ReverseLEDRender";
            this.ReverseLEDRender.Size = new System.Drawing.Size(211, 17);
            this.ReverseLEDRender.TabIndex = 4;
            this.ReverseLEDRender.Text = "Reverse LED Strip Render (For Mouse)";
            this.ReverseLEDRender.UseVisualStyleBackColor = true;
            // 
            // ColorSettingsButton
            // 
            this.ColorSettingsButton.Location = new System.Drawing.Point(257, 253);
            this.ColorSettingsButton.Name = "ColorSettingsButton";
            this.ColorSettingsButton.Size = new System.Drawing.Size(75, 23);
            this.ColorSettingsButton.TabIndex = 1;
            this.ColorSettingsButton.Text = "Colors...";
            this.ColorSettingsButton.UseVisualStyleBackColor = true;
            this.ColorSettingsButton.Click += new System.EventHandler(this.ColorSettingsButton_Click);
            // 
            // ChromaSDKEnable
            // 
            this.ChromaSDKEnable.AutoSize = true;
            this.ChromaSDKEnable.Location = new System.Drawing.Point(6, 25);
            this.ChromaSDKEnable.Name = "ChromaSDKEnable";
            this.ChromaSDKEnable.Size = new System.Drawing.Size(154, 17);
            this.ChromaSDKEnable.TabIndex = 0;
            this.ChromaSDKEnable.Text = "Enable Razer Chroma SDK";
            this.ChromaSDKEnable.UseVisualStyleBackColor = true;
            // 
            // DynamicDesktop
            // 
            this.DisableDesktop.AutoSize = true;
            this.DisableDesktop.Location = new System.Drawing.Point(6, 59);
            this.DisableDesktop.Name = "DynamicDesktop";
            this.DisableDesktop.Size = new System.Drawing.Size(211, 17);
            this.DisableDesktop.TabIndex = 4;
            this.DisableDesktop.Text = "Disable track background on minimized";
            this.DisableDesktop.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 640);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.DiscordGroupBox);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.SpotifyGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Text = "Settings";
            this.SpotifyGroupBox.ResumeLayout(false);
            this.SpotifyGroupBox.PerformLayout();
            this.DiscordGroupBox.ResumeLayout(false);
            this.DiscordGroupBox.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_BlurRadial)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_VolumeScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorDensity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox SpotifyGroupBox;
        private System.Windows.Forms.TextBox RefreshTokenTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button RevealKeyButton;
        private System.Windows.Forms.GroupBox DiscordGroupBox;
        private System.Windows.Forms.RichTextBox DiscordPauseDetail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox DiscordPlayState;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox DiscordPauseState;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RichTextBox DiscordPlayDetail;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox WebServiceListeningEnable;
        private System.Windows.Forms.CheckBox DiscordRichPresenceEnable;
        private System.Windows.Forms.CheckBox NetworkEnable;
        private System.Windows.Forms.Button AccessTokenRevealButton;
        private System.Windows.Forms.TextBox AccessTokenTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox AlbumColorMode;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox RenderModeCombobox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox RenderStyleCombobox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox RenderFPS;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TrackBar ColorDensity;
        private System.Windows.Forms.CheckBox ReverseLEDRender;
        private System.Windows.Forms.Button ColorSettingsButton;
        private System.Windows.Forms.CheckBox ChromaSDKEnable;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TrackBar trackbar_BlurRadial;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TrackBar trackbar_VolumeScale;
        private System.Windows.Forms.CheckBox DisableDesktop;
    }
}