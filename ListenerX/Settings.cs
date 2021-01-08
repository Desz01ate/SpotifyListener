using Listener.Core.Framework.Helpers;
using ListenerX.Classes;
using ListenerX.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListenerX
{
    public partial class Settings : Form
    {
        private bool AlbumCoverRenderEnable = false;
        private bool RenderPeakVolumeEnable = false;
        private bool RenderPeakVolumeSymmetricEnable = false;
        private bool ChromaPeakEnable = false;
        private bool ChromaEnableChanged = false;
        private bool Adaptive = false;
        private bool ArtworkWallpaperEnabled = false;
        public Settings()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.spotify;
            //Due to Facebook updated policy (https://developers.facebook.com/blog/post/2018/04/24/new-facebook-platform-product-changes-policy-updates/), now publish_actions is deprecated so this feature might be remove soon as well
            Adaptive = Properties.Settings.Default.AdaptiveDensity;
            AlbumCoverRenderEnable = Properties.Settings.Default.AlbumCoverRenderEnable;
            RenderPeakVolumeEnable = Properties.Settings.Default.RenderPeakVolumeEnable;
            RenderPeakVolumeSymmetricEnable = Properties.Settings.Default.SymmetricRenderEnable;
            ChromaPeakEnable = Properties.Settings.Default.PeakChroma;
            RenderStyleCombobox.SelectedIndex = Properties.Settings.Default.RenderStyleIndex;
            RenderModeCombobox.SelectedIndex = Adaptive ? 1 : 0;
            ChromaSDKEnable.Checked = Properties.Settings.Default.ChromaSDKEnable;
            ChromaSDKEnable.CheckedChanged += ChromaSDKEnable_CheckedChanged;
            ReverseLEDRender.Checked = Properties.Settings.Default.ReverseLEDRender;
            ColorDensity.Value = Properties.Settings.Default.Density;
            RenderFPS.Text = Properties.Settings.Default.RenderFPS.ToString();
            AlbumColorMode.SelectedIndex = Properties.Settings.Default.AlbumColorMode;
            cb_EnableArtworkWallpaper.Checked = Properties.Settings.Default.ArtworkWallpaperEnable;

            ChromaSDKEnable_CheckedChanged(null, EventArgs.Empty);
            trackbar_VolumeScale.Value = Properties.Settings.Default.VolumeScale;
            trackbar_VolumeScale_Scroll(trackbar_VolumeScale, null);

            this.lbl_Metadata.Text = $"Active module : {ActivatorHelpers.Metadata.ModuleName} {ActivatorHelpers.Metadata.VersionName}";

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {

            Properties.Settings.Default.AdaptiveDensity = Adaptive;
            Properties.Settings.Default.RenderPeakVolumeEnable = RenderPeakVolumeEnable;
            Properties.Settings.Default.AlbumCoverRenderEnable = AlbumCoverRenderEnable;
            Properties.Settings.Default.SymmetricRenderEnable = RenderPeakVolumeSymmetricEnable;
            Properties.Settings.Default.PeakChroma = ChromaPeakEnable;
            Properties.Settings.Default.RenderStyleIndex = (byte)RenderStyleCombobox.SelectedIndex;

            Properties.Settings.Default.ChromaSDKEnable = ChromaSDKEnable.Checked;
            Properties.Settings.Default.ReverseLEDRender = ReverseLEDRender.Checked;
            Properties.Settings.Default.Density = ColorDensity.Value;

            Properties.Settings.Default.AlbumColorMode = (byte)AlbumColorMode.SelectedIndex;
            Properties.Settings.Default.VolumeScale = trackbar_VolumeScale.Value;

            Properties.Settings.Default.ArtworkWallpaperEnable = this.ArtworkWallpaperEnabled;
            Properties.Settings.Default.Save();

            var safeConvertFps = SafeConvertRenderFps(RenderFPS.Text);
            if (Properties.Settings.Default.RenderFPS != safeConvertFps)
            {
                Properties.Settings.Default.RenderFPS = safeConvertFps;
                Properties.Settings.Default.Save();
            }
            if (this.ChromaEnableChanged)
            {
                MessageBox.Show($"Razer Chroma SDK enable/disable required application to restart in order to take effect.", "ListenerX", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Dispose();
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

        private void Button1_Click(object sender, EventArgs e)
        {
            using var colorSettings = new ColorSettings();
            colorSettings.ShowDialog();
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
            if (sender != null)
            {
                ChromaEnableChanged = true;
            }

            ColorSettingsButton.Enabled = ChromaSDKEnable.Checked;
            ReverseLEDRender.Enabled = ChromaSDKEnable.Checked;
            ColorDensity.Enabled = ChromaSDKEnable.Checked;
        }

        private void RenderStyleCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlbumCoverRenderEnable = false;
            RenderPeakVolumeEnable = false;
            ChromaPeakEnable = false;
            RenderPeakVolumeSymmetricEnable = false;
            RenderFPS.Enabled = true;
            var selectedIndex = ((ComboBox)sender).SelectedIndex;
            /*
               Album Cover - Progression + Volume
               Album Cover - Peak Volume Meter
               Album Cover - Symmetric Peak Volume Meter
               Fixed - Chroma Peak Meter
            */
            switch (selectedIndex)
            {
                case 0:
                    AlbumCoverRenderEnable = true;
                    ChromaPeakEnable = false;
                    break;
                case 1:
                    AlbumCoverRenderEnable = true;
                    RenderPeakVolumeEnable = true;
                    ChromaPeakEnable = false;
                    break;
                case 2:
                    AlbumCoverRenderEnable = true;
                    RenderPeakVolumeEnable = true;
                    RenderPeakVolumeSymmetricEnable = true;
                    ChromaPeakEnable = false;
                    break;
                case 3:
                    AlbumCoverRenderEnable = false;
                    RenderPeakVolumeEnable = true;
                    RenderPeakVolumeSymmetricEnable = false;
                    ChromaPeakEnable = true;
                    RenderFPS.Enabled = false;
                    RenderFPS.Text = "60";
                    break;
                case 4:
                    AlbumCoverRenderEnable = false;
                    RenderPeakVolumeEnable = true;
                    RenderPeakVolumeSymmetricEnable = true;
                    ChromaPeakEnable = true;
                    RenderFPS.Enabled = false;
                    RenderFPS.Text = "60";
                    break;
            }
        }

        private void RenderModeCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Adaptive = ((ComboBox)sender).SelectedIndex == 0 ? false : true;
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }


        private void ColorSettingsButton_Click(object sender, EventArgs e)
        {
            new ColorSettings().ShowDialog();
        }

        private void trackbar_VolumeScale_Scroll(object sender, EventArgs e)
        {
            txt_volScale.Text = trackbar_VolumeScale.Value * 10 + "%";
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
            this.ArtworkWallpaperEnabled = cb_EnableArtworkWallpaper.Checked;
        }
    }
}