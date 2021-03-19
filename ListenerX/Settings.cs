using Listener.Core.Framework.Helpers;
using ListenerX.Components;
using ListenerX.Cscore;
using ListenerX.Helpers;
using ListenerX.Visualization;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace ListenerX
{
    public partial class Settings : Form
    {
        private readonly VirtualKeyboardComponent virtualKeyboard;
        private bool firstLaunch = true;
        private VirtualKeyboard virtualKeyboardDisplayPanel;
        public Settings()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.listenerx;

            this.RenderStyleCombobox.Items.AddRange(ActivatorHelpers.Effects.Select(x => x.EffectName).ToArray());
            this.RenderStyleCombobox.SelectedIndex = Properties.Settings.Default.RenderStyle;

            ChromaSDKEnable.Checked = Properties.Settings.Default.ChromaSDKEnable;
            RenderFPS.Text = Properties.Settings.Default.RenderFPS.ToString();
            cb_EnableArtworkWallpaper.Checked = Properties.Settings.Default.ArtworkWallpaperEnable;

            trackbar_Amplitude.Value = Math.Min(trackbar_Amplitude.Maximum, (int)(Properties.Settings.Default.Amplitude * 10));
            trackbar_VolumeScale_Scroll(trackbar_Amplitude, null);

            trackbar_BgBrightness.Value = Math.Min(trackbar_BgBrightness.Maximum, (int)(Properties.Settings.Default.BackgroundBrightness * 10));
            trackbar_BgBrightness_Scroll(trackbar_BgBrightness, null);

            var strategies = Enum.GetValues(typeof(ScalingStrategy));
            foreach (var s in strategies)
            {
                ScalingStrategy.Items.Add(s);
            }
            ScalingStrategy.SelectedIndex = Properties.Settings.Default.ScalingStrategy;

            //this.lbl_Metadata.Text = $"Active module : {ActivatorHelpers.Metadata.ModuleName} {ActivatorHelpers.Metadata.VersionName}";

            var devices = OutputDevice.GetDevices().ToArray();
            var activeIndex = Array.FindIndex(devices, x => x.Item1 == OutputDevice.ActiveDevice.DeviceId);
            this.cb_OutputDevice.Items.AddRange(devices.Select(x => x.Item2).ToArray());
            this.cb_OutputDevice.SelectedIndex = activeIndex;
            this.cb_OutputDevice.SelectedIndexChanged += (s, e) =>
            {
                var index = (s as ComboBox).SelectedIndex;
                OutputDevice.ChangeActiveDevice(devices[index].Item1);
            };


            this.virtualKeyboard = new VirtualKeyboardComponent();
            this.virtualKeyboard.SizeMode = PictureBoxSizeMode.AutoSize;
            this.virtualKeyboard.OnImageChanged += VirtualKeyboard_OnImageChanged;
            this.virtualKeyboard.Start();

            this.FormClosing += Settings_FormClosing;

            firstLaunch = false;
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.virtualKeyboardDisplayPanel?.Dispose();
        }

        private void VirtualKeyboard_OnImageChanged(object sender, EventArgs e)
        {
            this.visualizer.Image = this.virtualKeyboard.Image;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
        }

        private int SafeConvertRenderFps(string text)
        {
            if (int.TryParse(text, out int result))
            {
                if (result <= 0)
                    result = 60;
                else if (result > 144)
                    result = 144;
                return result;
            }
            throw new FormatException(nameof(text));
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to reset all settings?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();
            }
        }

        private void ChromaSDKEnable_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void RenderStyleCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RenderStyle = RenderStyleCombobox.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void RenderModeCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        private void ClearCache_Click(object sender, EventArgs e)
        {
            CacheFileManager.ClearCache();
            CacheSize.Text = CacheFileManager.GetCacheSize();
        }

        private void Settings_Load_1(object sender, EventArgs e)
        {
            this.CacheSize.Text = CacheFileManager.GetCacheSize();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void cb_EnableArtworkWallpaper_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ArtworkWallpaperEnable = cb_EnableArtworkWallpaper.Checked;
            Properties.Settings.Default.Save();
        }

        private void trackbar_VolumeScale_Scroll(object sender, EventArgs e)
        {
            txt_volScale.Text = trackbar_Amplitude.Value * 10 + "%";
            Properties.Settings.Default.Amplitude = trackbar_Amplitude.Value / 10.0d;
            Properties.Settings.Default.Save();
        }

        private void trackbar_BgBrightness_Scroll(object sender, EventArgs e)
        {
            txt_BgBrightness.Text = trackbar_BgBrightness.Value * 10 + "%";
            Properties.Settings.Default.BackgroundBrightness = trackbar_BgBrightness.Value / 10.0d;
            Properties.Settings.Default.Save();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.virtualKeyboard?.Dispose();
            base.OnClosing(e);
        }

        private void visualizer_Click(object sender, EventArgs e)
        {
            if (virtualKeyboardDisplayPanel != null)
            {
                virtualKeyboardDisplayPanel.BringToFront();
                return;
            }

            virtualKeyboardDisplayPanel = new VirtualKeyboard(this.virtualKeyboard);
            virtualKeyboardDisplayPanel.FormClosing += (s, e) => virtualKeyboardDisplayPanel = null;
            virtualKeyboardDisplayPanel.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseAverage = this.checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ScalingStrategy = this.ScalingStrategy.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        private void ChromaSDKEnable_CheckedChanged_1(object sender, EventArgs e)
        {
            if (!firstLaunch)
            {
                MessageBox.Show($"Razer Chroma SDK enable/disable required application to restart in order to take effect.", "ListenerX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Properties.Settings.Default.ChromaSDKEnable = ChromaSDKEnable.Checked;
            Properties.Settings.Default.Save();
        }

        private void RenderFPS_TextChanged(object sender, EventArgs e)
        {
            var fps = SafeConvertRenderFps(this.RenderFPS.Text);
            Properties.Settings.Default.RenderFPS = fps;
            Properties.Settings.Default.Save();
        }
    }
}