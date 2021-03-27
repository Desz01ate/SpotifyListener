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
            this.ModuleSelector = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cb_OutputDevice = new System.Windows.Forms.ComboBox();
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
            this.ResetButton.Location = new System.Drawing.Point(816, 579);
            this.ResetButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(89, 43);
            this.ResetButton.TabIndex = 4;
            this.ResetButton.Text = "Reset";
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ModuleSelector);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.cb_OutputDevice);
            this.groupBox3.Controls.Add(this.cb_EnableArtworkWallpaper);
            this.groupBox3.Controls.Add(this.ClearCache);
            this.groupBox3.Controls.Add(this.CacheSize);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(14, 15);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox3.Size = new System.Drawing.Size(434, 539);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Preferences";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // ModuleSelector
            // 
            this.ModuleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModuleSelector.FormattingEnabled = true;
            this.ModuleSelector.Location = new System.Drawing.Point(110, 92);
            this.ModuleSelector.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ModuleSelector.Name = "ModuleSelector";
            this.ModuleSelector.Size = new System.Drawing.Size(316, 23);
            this.ModuleSelector.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 95);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "Active Module :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 133);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "Output Device :";
            // 
            // cb_OutputDevice
            // 
            this.cb_OutputDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_OutputDevice.FormattingEnabled = true;
            this.cb_OutputDevice.Location = new System.Drawing.Point(111, 129);
            this.cb_OutputDevice.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cb_OutputDevice.Name = "cb_OutputDevice";
            this.cb_OutputDevice.Size = new System.Drawing.Size(316, 23);
            this.cb_OutputDevice.TabIndex = 9;
            // 
            // cb_EnableArtworkWallpaper
            // 
            this.cb_EnableArtworkWallpaper.AutoSize = true;
            this.cb_EnableArtworkWallpaper.Location = new System.Drawing.Point(12, 63);
            this.cb_EnableArtworkWallpaper.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cb_EnableArtworkWallpaper.Name = "cb_EnableArtworkWallpaper";
            this.cb_EnableArtworkWallpaper.Size = new System.Drawing.Size(158, 19);
            this.cb_EnableArtworkWallpaper.TabIndex = 8;
            this.cb_EnableArtworkWallpaper.Text = "Enable artwork wallpaper";
            this.cb_EnableArtworkWallpaper.UseVisualStyleBackColor = true;
            this.cb_EnableArtworkWallpaper.CheckedChanged += new System.EventHandler(this.cb_EnableArtworkWallpaper_CheckedChanged);
            // 
            // ClearCache
            // 
            this.ClearCache.ForeColor = System.Drawing.Color.Red;
            this.ClearCache.Location = new System.Drawing.Point(197, 30);
            this.ClearCache.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ClearCache.Name = "ClearCache";
            this.ClearCache.Size = new System.Drawing.Size(48, 15);
            this.ClearCache.TabIndex = 7;
            this.ClearCache.Text = "Clear";
            this.ClearCache.Click += new System.EventHandler(this.ClearCache_Click);
            // 
            // CacheSize
            // 
            this.CacheSize.AutoSize = true;
            this.CacheSize.Location = new System.Drawing.Point(105, 30);
            this.CacheSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.CacheSize.Name = "CacheSize";
            this.CacheSize.Size = new System.Drawing.Size(13, 15);
            this.CacheSize.TabIndex = 5;
            this.CacheSize.Text = "$";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
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
            this.groupBox2.Location = new System.Drawing.Point(455, 15);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox2.Size = new System.Drawing.Size(430, 539);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "RGB Support";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 338);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 15);
            this.label4.TabIndex = 34;
            this.label4.Text = "Scaling Strategy :";
            // 
            // ScalingStrategy
            // 
            this.ScalingStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ScalingStrategy.FormattingEnabled = true;
            this.ScalingStrategy.Location = new System.Drawing.Point(133, 335);
            this.ScalingStrategy.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ScalingStrategy.Name = "ScalingStrategy";
            this.ScalingStrategy.Size = new System.Drawing.Size(270, 23);
            this.ScalingStrategy.TabIndex = 33;
            this.ScalingStrategy.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(10, 280);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(125, 19);
            this.checkBox1.TabIndex = 32;
            this.checkBox1.Text = "Use Average Mode";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 398);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "Effect preview :";
            // 
            // txt_BgBrightness
            // 
            this.txt_BgBrightness.AutoSize = true;
            this.txt_BgBrightness.Location = new System.Drawing.Point(343, 237);
            this.txt_BgBrightness.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_BgBrightness.Name = "txt_BgBrightness";
            this.txt_BgBrightness.Size = new System.Drawing.Size(41, 15);
            this.txt_BgBrightness.TabIndex = 31;
            this.txt_BgBrightness.Text = "NaN%";
            // 
            // visualizer
            // 
            this.visualizer.Location = new System.Drawing.Point(6, 419);
            this.visualizer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.visualizer.Name = "visualizer";
            this.visualizer.Size = new System.Drawing.Size(420, 110);
            this.visualizer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.visualizer.TabIndex = 9;
            this.visualizer.TabStop = false;
            this.visualizer.Click += new System.EventHandler(this.visualizer_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 237);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 15);
            this.label3.TabIndex = 30;
            this.label3.Text = "Background Brightness :";
            // 
            // trackbar_BgBrightness
            // 
            this.trackbar_BgBrightness.LargeChange = 1;
            this.trackbar_BgBrightness.Location = new System.Drawing.Point(148, 237);
            this.trackbar_BgBrightness.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackbar_BgBrightness.Name = "trackbar_BgBrightness";
            this.trackbar_BgBrightness.Size = new System.Drawing.Size(191, 45);
            this.trackbar_BgBrightness.TabIndex = 29;
            this.trackbar_BgBrightness.Scroll += new System.EventHandler(this.trackbar_BgBrightness_Scroll);
            // 
            // txt_volScale
            // 
            this.txt_volScale.AutoSize = true;
            this.txt_volScale.Location = new System.Drawing.Point(343, 178);
            this.txt_volScale.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txt_volScale.Name = "txt_volScale";
            this.txt_volScale.Size = new System.Drawing.Size(41, 15);
            this.txt_volScale.TabIndex = 28;
            this.txt_volScale.Text = "NaN%";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(7, 178);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(69, 15);
            this.label15.TabIndex = 27;
            this.label15.Text = "Amplitude :";
            // 
            // trackbar_Amplitude
            // 
            this.trackbar_Amplitude.LargeChange = 10;
            this.trackbar_Amplitude.Location = new System.Drawing.Point(148, 178);
            this.trackbar_Amplitude.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackbar_Amplitude.Maximum = 50;
            this.trackbar_Amplitude.Name = "trackbar_Amplitude";
            this.trackbar_Amplitude.Size = new System.Drawing.Size(191, 45);
            this.trackbar_Amplitude.SmallChange = 5;
            this.trackbar_Amplitude.TabIndex = 26;
            this.trackbar_Amplitude.Value = 1;
            this.trackbar_Amplitude.Scroll += new System.EventHandler(this.trackbar_VolumeScale_Scroll);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 133);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 15);
            this.label9.TabIndex = 21;
            this.label9.Text = "Render Style  :";
            // 
            // RenderStyleCombobox
            // 
            this.RenderStyleCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RenderStyleCombobox.FormattingEnabled = true;
            this.RenderStyleCombobox.Location = new System.Drawing.Point(133, 129);
            this.RenderStyleCombobox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.RenderStyleCombobox.Name = "RenderStyleCombobox";
            this.RenderStyleCombobox.Size = new System.Drawing.Size(270, 23);
            this.RenderStyleCombobox.TabIndex = 20;
            this.RenderStyleCombobox.SelectedIndexChanged += new System.EventHandler(this.RenderStyleCombobox_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Location = new System.Drawing.Point(4, 85);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(223, 15);
            this.label8.TabIndex = 13;
            this.label8.Text = "(Recommed : 60 fps, Maximum : 144 fps)";
            // 
            // RenderFPS
            // 
            this.RenderFPS.Location = new System.Drawing.Point(147, 59);
            this.RenderFPS.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.RenderFPS.Name = "RenderFPS";
            this.RenderFPS.Size = new System.Drawing.Size(76, 23);
            this.RenderFPS.TabIndex = 12;
            this.RenderFPS.TextChanged += new System.EventHandler(this.RenderFPS_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 62);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(118, 15);
            this.label11.TabIndex = 11;
            this.label11.Text = "Render frame speed :";
            // 
            // ChromaSDKEnable
            // 
            this.ChromaSDKEnable.AutoSize = true;
            this.ChromaSDKEnable.Location = new System.Drawing.Point(7, 29);
            this.ChromaSDKEnable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChromaSDKEnable.Name = "ChromaSDKEnable";
            this.ChromaSDKEnable.Size = new System.Drawing.Size(126, 19);
            this.ChromaSDKEnable.TabIndex = 0;
            this.ChromaSDKEnable.Text = "Enable RGB Render";
            this.ChromaSDKEnable.UseVisualStyleBackColor = true;
            this.ChromaSDKEnable.CheckedChanged += new System.EventHandler(this.ChromaSDKEnable_CheckedChanged_1);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 613);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ResetButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
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
        private System.Windows.Forms.CheckBox cb_EnableArtworkWallpaper;
        private System.Windows.Forms.Label txt_BgBrightness;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackbar_BgBrightness;
        private System.Windows.Forms.PictureBox visualizer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox ScalingStrategy;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cb_OutputDevice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox ModuleSelector;
        private System.Windows.Forms.Label label6;
    }
}