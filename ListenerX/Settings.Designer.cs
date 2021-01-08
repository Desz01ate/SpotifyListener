namespace ListenerX
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
            this.SaveButton = new System.Windows.Forms.Label();
            this.ResetButton = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cb_EnableArtworkWallpaper = new System.Windows.Forms.CheckBox();
            this.ClearCache = new System.Windows.Forms.Label();
            this.CacheSize = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txt_volScale = new System.Windows.Forms.Label();
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
            this.ColorSettingsButton = new System.Windows.Forms.Label();
            this.ChromaSDKEnable = new System.Windows.Forms.CheckBox();
            this.lbl_Metadata = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_VolumeScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorDensity)).BeginInit();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.SaveButton.Location = new System.Drawing.Point(605, 380);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(76, 37);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.ForeColor = System.Drawing.Color.Red;
            this.ResetButton.Location = new System.Drawing.Point(699, 380);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(76, 37);
            this.ResetButton.TabIndex = 4;
            this.ResetButton.Text = "Reset";
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cb_EnableArtworkWallpaper);
            this.groupBox3.Controls.Add(this.ClearCache);
            this.groupBox3.Controls.Add(this.CacheSize);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(12, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(372, 361);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Preferences";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // cb_EnableArtworkWallpaper
            // 
            this.cb_EnableArtworkWallpaper.AutoSize = true;
            this.cb_EnableArtworkWallpaper.Location = new System.Drawing.Point(10, 55);
            this.cb_EnableArtworkWallpaper.Name = "cb_EnableArtworkWallpaper";
            this.cb_EnableArtworkWallpaper.Size = new System.Drawing.Size(145, 17);
            this.cb_EnableArtworkWallpaper.TabIndex = 8;
            this.cb_EnableArtworkWallpaper.Text = "Enable artwork wallpaper";
            this.cb_EnableArtworkWallpaper.UseVisualStyleBackColor = true;
            this.cb_EnableArtworkWallpaper.CheckedChanged += new System.EventHandler(this.cb_EnableArtworkWallpaper_CheckedChanged);
            // 
            // ClearCache
            // 
            this.ClearCache.ForeColor = System.Drawing.Color.Red;
            this.ClearCache.Location = new System.Drawing.Point(169, 26);
            this.ClearCache.Name = "ClearCache";
            this.ClearCache.Size = new System.Drawing.Size(41, 13);
            this.ClearCache.TabIndex = 7;
            this.ClearCache.Text = "Clear";
            this.ClearCache.Click += new System.EventHandler(this.ClearCache_Click);
            // 
            // CacheSize
            // 
            this.CacheSize.AutoSize = true;
            this.CacheSize.Location = new System.Drawing.Point(90, 26);
            this.CacheSize.Name = "CacheSize";
            this.CacheSize.Size = new System.Drawing.Size(13, 13);
            this.CacheSize.TabIndex = 5;
            this.CacheSize.Text = "$";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Data caching :";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txt_volScale);
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
            this.groupBox2.Location = new System.Drawing.Point(390, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(366, 361);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Razer Chroma SDK";
            // 
            // txt_volScale
            // 
            this.txt_volScale.AutoSize = true;
            this.txt_volScale.Location = new System.Drawing.Point(254, 304);
            this.txt_volScale.Name = "txt_volScale";
            this.txt_volScale.Size = new System.Drawing.Size(37, 13);
            this.txt_volScale.TabIndex = 28;
            this.txt_volScale.Text = "NaN%";
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
            this.trackbar_VolumeScale.Maximum = 20;
            this.trackbar_VolumeScale.Minimum = 1;
            this.trackbar_VolumeScale.Name = "trackbar_VolumeScale";
            this.trackbar_VolumeScale.Size = new System.Drawing.Size(164, 45);
            this.trackbar_VolumeScale.TabIndex = 26;
            this.trackbar_VolumeScale.Value = 1;
            this.trackbar_VolumeScale.Scroll += new System.EventHandler(this.trackbar_VolumeScale_Scroll);
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
            "Album Cover - Progression + Volume",
            "Album Cover - Peak Volume Meter",
            "Album Cover - Symmetric Peak Volume Meter",
            "Chroma - Peak Volume Meter",
            "Chroma - Symmetric Peak Volume Meter"});
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
            this.ColorSettingsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.ColorSettingsButton.Location = new System.Drawing.Point(257, 257);
            this.ColorSettingsButton.Name = "ColorSettingsButton";
            this.ColorSettingsButton.Size = new System.Drawing.Size(75, 23);
            this.ColorSettingsButton.TabIndex = 1;
            this.ColorSettingsButton.Text = "Colors...";
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
            // lbl_Metadata
            // 
            this.lbl_Metadata.AutoSize = true;
            this.lbl_Metadata.Location = new System.Drawing.Point(12, 380);
            this.lbl_Metadata.Name = "lbl_Metadata";
            this.lbl_Metadata.Size = new System.Drawing.Size(13, 13);
            this.lbl_Metadata.TabIndex = 7;
            this.lbl_Metadata.Text = "$";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 405);
            this.Controls.Add(this.lbl_Metadata);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.SaveButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load_1);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_VolumeScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorDensity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label SaveButton;
        private System.Windows.Forms.Label ResetButton;
        private System.Windows.Forms.GroupBox groupBox3;
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
        private System.Windows.Forms.Label ColorSettingsButton;
        private System.Windows.Forms.CheckBox ChromaSDKEnable;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TrackBar trackbar_VolumeScale;
        private System.Windows.Forms.Label txt_volScale;
        private System.Windows.Forms.Label ClearCache;
        private System.Windows.Forms.Label CacheSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_Metadata;
        private System.Windows.Forms.CheckBox cb_EnableArtworkWallpaper;
    }
}