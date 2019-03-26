using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotifyListener
{
    public partial class Settings : Form
    {
        public static bool AlbumCoverRenderEnable = false;
        public static bool RenderPeakVolumeEnable = false;
        public static bool RenderPeakVolumeSymmetricEnable = false;
        public static bool Adaptive = false;
        public Settings()
        {
            InitializeComponent();
            //Due to Facebook updated policy (https://developers.facebook.com/blog/post/2018/04/24/new-facebook-platform-product-changes-policy-updates/), now publish_actions is deprecated so this feature might be remove soon as well
            SpotifyGroupBox.Enabled = true;
#if WIN32
            DiscordRichPresenceEnable.Visible = false;
            DiscordGroupBox.Text = "Discord RPC is not working on x86 system";
            DiscordGroupBox.ForeColor = Color.Red;
#endif
            Adaptive = Properties.Settings.Default.AdaptiveDensity;
            AlbumCoverRenderEnable = Properties.Settings.Default.AlbumCoverRenderEnable;
            RenderPeakVolumeEnable = Properties.Settings.Default.RenderPeakVolumeEnable;
            RenderPeakVolumeSymmetricEnable = Properties.Settings.Default.SymmetricRenderEnable;
            RenderStyleCombobox.SelectedIndex = Properties.Settings.Default.RenderStyleIndex;
            RenderModeCombobox.SelectedIndex = Adaptive ? 1 : 0;

            RefreshTokenTextbox.Text = Properties.Settings.Default.RefreshToken;
            AccessTokenTextBox.Text = Properties.Settings.Default.AccessToken;

            DiscordPlayDetail.Text = Properties.Settings.Default.DiscordPlayDetail;
            DiscordPlayState.Text = Properties.Settings.Default.DiscordPlayState;
            DiscordPauseDetail.Text = Properties.Settings.Default.DiscordPauseDetail;
            DiscordPauseState.Text = Properties.Settings.Default.DiscordPauseState;
            ChromaSDKEnable.Checked = Properties.Settings.Default.ChromaSDKEnable;
            DiscordRichPresenceEnable.Checked = Properties.Settings.Default.DiscordRichPresenceEnable;
            ReverseLEDRender.Checked = Properties.Settings.Default.ReverseLEDRender;
            ColorDensity.Value = Properties.Settings.Default.Density;
            RenderFPS.Text = Properties.Settings.Default.RenderFPS.ToString();
            NetworkEnable.Checked = Properties.Settings.Default.NetworkEnable;
            AlbumColorMode.SelectedIndex = Properties.Settings.Default.AlbumColorMode;

            ChromaSDKEnable_CheckedChanged(null, EventArgs.Empty);
            DiscordRichPresenceEnable_CheckedChanged(null, EventArgs.Empty);
            NetworkEnable_CheckedChanged(NetworkEnable, EventArgs.Empty);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RefreshToken = RefreshTokenTextbox.Text;

            Properties.Settings.Default.AdaptiveDensity = Adaptive;
            Properties.Settings.Default.RenderPeakVolumeEnable = RenderPeakVolumeEnable;
            Properties.Settings.Default.AlbumCoverRenderEnable = AlbumCoverRenderEnable;
            Properties.Settings.Default.SymmetricRenderEnable = RenderPeakVolumeSymmetricEnable;
            Properties.Settings.Default.RenderStyleIndex = (byte)RenderStyleCombobox.SelectedIndex;


            Properties.Settings.Default.AccessToken = RefreshTokenTextbox.Text;
            Properties.Settings.Default.DiscordPlayDetail = DiscordPlayDetail.Text;
            Properties.Settings.Default.DiscordPlayState = DiscordPlayState.Text;
            Properties.Settings.Default.DiscordPauseDetail = DiscordPauseDetail.Text;
            Properties.Settings.Default.DiscordPauseState = DiscordPauseState.Text;

            Properties.Settings.Default.ChromaSDKEnable = ChromaSDKEnable.Checked;
            Properties.Settings.Default.DiscordRichPresenceEnable = DiscordRichPresenceEnable.Checked;
            Properties.Settings.Default.ReverseLEDRender = ReverseLEDRender.Checked;
            Properties.Settings.Default.Density = ColorDensity.Value;
            Properties.Settings.Default.RenderFPS = SafeConvertRenderFps(RenderFPS.Text);
            Properties.Settings.Default.NetworkEnable = NetworkEnable.Checked;
            Properties.Settings.Default.AlbumColorMode = (byte)AlbumColorMode.SelectedIndex;
            Properties.Settings.Default.Save();
            Extension.Restart();
            Dispose();
        }

        private int SafeConvertRenderFps(string text)
        {
            int.TryParse(text, out int result);
            if (result <= 0)
                result = 60;
            else if (result > 144)
                result = 144;
            return result;
        }

        private async void RevealKeyButton_Click(object sender, EventArgs e)
        {
            RefreshTokenTextbox.PasswordChar = '\0';
            await Task.Delay(5000);
            RefreshTokenTextbox.PasswordChar = '*';
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            new ColorSettings().Show();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to reset all settings?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.Save();
                Extension.Restart();
            }
        }

        private void DiscordRichPresenceEnable_CheckedChanged(object sender, EventArgs e)
        {
            DiscordPlayState.Enabled = DiscordRichPresenceEnable.Checked;
            DiscordPlayDetail.Enabled = DiscordRichPresenceEnable.Checked;
            DiscordPauseState.Enabled = DiscordRichPresenceEnable.Checked;
            DiscordPauseDetail.Enabled = DiscordRichPresenceEnable.Checked;
        }

        private void ChromaSDKEnable_CheckedChanged(object sender, EventArgs e)
        {
            ColorSettingsButton.Enabled = ChromaSDKEnable.Checked;
            ReverseLEDRender.Enabled = ChromaSDKEnable.Checked;
            ColorDensity.Enabled = ChromaSDKEnable.Checked;
        }
        private void NetworkEnable_CheckedChanged(object sender, EventArgs e)
        {
            var state = ((CheckBox)sender).Checked;
            if (!state)
            {
                DiscordRichPresenceEnable.Checked = false;
            }
            DiscordRichPresenceEnable.Enabled = state;
        }

        private void RenderStyleCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlbumCoverRenderEnable = false;
            RenderPeakVolumeEnable = false;
            RenderPeakVolumeSymmetricEnable = false;
            var selectedIndex = ((ComboBox)sender).SelectedIndex;
            /*
               Custom - Progression + Volume
               Custom - Peak Volume Meter
               Custom - Symmetric Peak Volume Meter
               Album Cover - Progression + Volume
               Album Cover - Peak Volume Meter
               Album Cover - Symmetric Peak Volume Meter
            */
            switch (selectedIndex)
            {
                case 0:
                    break;
                case 1:
                    RenderPeakVolumeEnable = true;
                    break;
                case 2:
                    RenderPeakVolumeEnable = true;
                    RenderPeakVolumeSymmetricEnable = true;
                    break;
                case 3:
                    AlbumCoverRenderEnable = true;
                    break;
                case 4:
                    AlbumCoverRenderEnable = true;
                    RenderPeakVolumeEnable = true;
                    break;
                case 5:
                    AlbumCoverRenderEnable = true;
                    RenderPeakVolumeEnable = true;
                    RenderPeakVolumeSymmetricEnable = true;
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

        private async void AccessTokenRevealButton_Click(object sender, EventArgs e)
        {
            AccessTokenTextBox.PasswordChar = '\0';
            await Task.Delay(5000);
            AccessTokenTextBox.PasswordChar = '*';
        }
    }
}