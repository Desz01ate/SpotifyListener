using ListenerX.Components;

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
            this.ResetButton = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cb_EnableArtworkWallpaper = new System.Windows.Forms.CheckBox();
            this.ClearCache = new System.Windows.Forms.Label();
            this.CacheSize = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ScalingStrategy = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_BgBrightness = new System.Windows.Forms.Label();
            this.visualizer = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.trackbar_BgBrightness = new System.Windows.Forms.TrackBar();
            this.txt_volScale = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.trackbar_Amplitude = new System.Windows.Forms.TrackBar();
            this.label9 = new System.Windows.Forms.Label();
            this.RenderStyleCombobox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.RenderFPS = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.ChromaSDKEnable = new System.Windows.Forms.CheckBox();
            this.lbl_Metadata = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.visualizer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_BgBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_Amplitude)).BeginInit();
            this.SuspendLayout();
            // 
            // ResetButton
            // 
            this.ResetButton.ForeColor = System.Drawing.Color.Red;
            this.ResetButton.Location = new System.Drawing.Point(699, 502);
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
            this.groupBox3.Size = new System.Drawing.Size(372, 467);
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
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.ScalingStrategy);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txt_BgBrightness);
            this.groupBox2.Controls.Add(this.visualizer);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.trackbar_BgBrightness);
            this.groupBox2.Controls.Add(this.txt_volScale);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.trackbar_Amplitude);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.RenderStyleCombobox);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.RenderFPS);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.ChromaSDKEnable);
            this.groupBox2.Location = new System.Drawing.Point(390, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(369, 467);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Razer Chroma SDK";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 293);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 34;
            this.label4.Text = "Scaling Strategy :";
            // 
            // ScalingStrategy
            // 
            this.ScalingStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ScalingStrategy.FormattingEnabled = true;
            this.ScalingStrategy.Location = new System.Drawing.Point(114, 290);
            this.ScalingStrategy.Name = "ScalingStrategy";
            this.ScalingStrategy.Size = new System.Drawing.Size(232, 21);
            this.ScalingStrategy.TabIndex = 33;
            this.ScalingStrategy.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(9, 243);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(118, 17);
            this.checkBox1.TabIndex = 32;
            this.checkBox1.Text = "Use Average Mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 345);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Effect preview :";
            // 
            // txt_BgBrightness
            // 
            this.txt_BgBrightness.AutoSize = true;
            this.txt_BgBrightness.Location = new System.Drawing.Point(294, 205);
            this.txt_BgBrightness.Name = "txt_BgBrightness";
            this.txt_BgBrightness.Size = new System.Drawing.Size(37, 13);
            this.txt_BgBrightness.TabIndex = 31;
            this.txt_BgBrightness.Text = "NaN%";
            // 
            // visualizer
            // 
            this.visualizer.Location = new System.Drawing.Point(5, 363);
            this.visualizer.Name = "visualizer";
            this.visualizer.Size = new System.Drawing.Size(360, 95);
            this.visualizer.TabIndex = 9;
            this.visualizer.TabStop = false;
            this.visualizer.Click += new System.EventHandler(this.visualizer_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 205);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Background Brightness :";
            // 
            // trackbar_BgBrightness
            // 
            this.trackbar_BgBrightness.LargeChange = 1;
            this.trackbar_BgBrightness.Location = new System.Drawing.Point(127, 205);
            this.trackbar_BgBrightness.Name = "trackbar_BgBrightness";
            this.trackbar_BgBrightness.Size = new System.Drawing.Size(164, 45);
            this.trackbar_BgBrightness.TabIndex = 29;
            this.trackbar_BgBrightness.Scroll += new System.EventHandler(this.trackbar_BgBrightness_Scroll);
            // 
            // txt_volScale
            // 
            this.txt_volScale.AutoSize = true;
            this.txt_volScale.Location = new System.Drawing.Point(294, 154);
            this.txt_volScale.Name = "txt_volScale";
            this.txt_volScale.Size = new System.Drawing.Size(37, 13);
            this.txt_volScale.TabIndex = 28;
            this.txt_volScale.Text = "NaN%";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 154);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 13);
            this.label15.TabIndex = 27;
            this.label15.Text = "Amplitude :";
            // 
            // trackbar_Amplitude
            // 
            this.trackbar_Amplitude.LargeChange = 10;
            this.trackbar_Amplitude.Location = new System.Drawing.Point(127, 154);
            this.trackbar_Amplitude.Maximum = 50;
            this.trackbar_Amplitude.Name = "trackbar_Amplitude";
            this.trackbar_Amplitude.Size = new System.Drawing.Size(164, 45);
            this.trackbar_Amplitude.SmallChange = 5;
            this.trackbar_Amplitude.TabIndex = 26;
            this.trackbar_Amplitude.Value = 1;
            this.trackbar_Amplitude.Scroll += new System.EventHandler(this.trackbar_VolumeScale_Scroll);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 115);
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
            "Album Cover (Gradients) - Progression + Volume",
            "Album Cover - Progression + Volume",
            "Album Cover (Gradients)  - Spectrum Vitualizer",
            "Album Cover - Spectrum Vitualizer",
            "Chroma - Spectrum Vitualizer"});
            this.RenderStyleCombobox.Location = new System.Drawing.Point(114, 112);
            this.RenderStyleCombobox.Name = "RenderStyleCombobox";
            this.RenderStyleCombobox.Size = new System.Drawing.Size(232, 21);
            this.RenderStyleCombobox.TabIndex = 20;
            this.RenderStyleCombobox.SelectedIndexChanged += new System.EventHandler(this.RenderStyleCombobox_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(3, 74);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(199, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "(Recommed : 60 fps, Maximum : 144 fps)";
            // 
            // RenderFPS
            // 
            this.RenderFPS.Location = new System.Drawing.Point(126, 51);
            this.RenderFPS.Name = "RenderFPS";
            this.RenderFPS.Size = new System.Drawing.Size(66, 20);
            this.RenderFPS.TabIndex = 12;
            this.RenderFPS.TextChanged += new System.EventHandler(this.RenderFPS_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 54);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(109, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Render frame speed :";
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
            this.ChromaSDKEnable.CheckedChanged += new System.EventHandler(this.ChromaSDKEnable_CheckedChanged_1);
            // 
            // lbl_Metadata
            // 
            this.lbl_Metadata.AutoSize = true;
            this.lbl_Metadata.Location = new System.Drawing.Point(12, 502);
            this.lbl_Metadata.Name = "lbl_Metadata";
            this.lbl_Metadata.Size = new System.Drawing.Size(13, 13);
            this.lbl_Metadata.TabIndex = 7;
            this.lbl_Metadata.Text = "$";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 531);
            this.Controls.Add(this.lbl_Metadata);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ResetButton);
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
            ((System.ComponentModel.ISupportInitialize)(this.visualizer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_BgBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_Amplitude)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label ResetButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox RenderStyleCombobox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox RenderFPS;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox ChromaSDKEnable;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TrackBar trackbar_Amplitude;
        private System.Windows.Forms.Label txt_volScale;
        private System.Windows.Forms.Label ClearCache;
        private System.Windows.Forms.Label CacheSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_Metadata;
        private System.Windows.Forms.CheckBox cb_EnableArtworkWallpaper;
        private System.Windows.Forms.Label txt_BgBrightness;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackbar_BgBrightness;
        private System.Windows.Forms.PictureBox visualizer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox ScalingStrategy;
        private System.Windows.Forms.Label label4;
    }
}