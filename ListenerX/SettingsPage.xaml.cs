using Listener.Core.Framework.Helpers;
using Listener.ImageProcessing;
using ListenerX.Classes;
using ListenerX.Components;
using ListenerX.Helpers;
using ListenerX.Visualization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VirtualGrid.Interfaces;

namespace ListenerX
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Window
    {
        private readonly Settings settings;
        private readonly ModuleActivator moduleActivator;
        private readonly IVirtualLedGrid virtualGrid;
        private readonly VirtualKeyboardComponent virtualKeyboardRenderer;
        private readonly bool Init;
        public SettingsPage(Settings settings,
                            ModuleActivator moduleActivator,
                            IVirtualLedGrid virtualGrid)
        {
            InitializeComponent();
            this.settings = settings;
            this.moduleActivator = moduleActivator;
            this.virtualGrid = virtualGrid;
            this.DataContext = this.settings;

            var providers = this.moduleActivator.Players.Select(x => x.Key);
            this.list_music_provider.ItemsSource = providers;

            var devices = RealTimePlayback.EnumerateLoopbackDevices().ToArray();
            var activeIndex = Array.FindIndex(devices, x => x.IsDefaultDevice);
            this.list_output_devices.ItemsSource = devices.Select(x => x.Device.FriendlyName).ToArray();
            this.list_output_devices.SelectedIndex = activeIndex;
            this.list_output_devices.SelectionChanged += (s, e) =>
            {
                var idx = (s as ComboBox).SelectedIndex;
                RealTimePlayback.InitLoopbackCapture(devices[idx].Device, settings);
            };

            this.list_render_style.ItemsSource = this.moduleActivator.Effects.Select(x => x.EffectName);
            this.list_scaling_strategy.ItemsSource = Enum.GetValues(typeof(ScalingStrategy));
            this.txt_cache_mb.Content = Math.Round(CacheFileManager.GetCacheSize(), 2);

            this.virtualKeyboardRenderer = new VirtualKeyboardComponent(virtualGrid);
            this.virtualKeyboardRenderer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.virtualKeyboardRenderer.OnImageChanged += VirtualKeyboardRenderer_OnImageChanged;
            this.virtualKeyboardRenderer.Start();
            this.Init = true;
        }

        private void VirtualKeyboardRenderer_OnImageChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() =>
            {
                this.img_preview_effect.Source = virtualKeyboardRenderer.Image.ToBitmapImage(ImageFormat.Jpeg);
            }));
        }

        private void WallpaperEnableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var isCheck = (sender as CheckBox).IsChecked ?? false;
            var text = isCheck ? "Enable" : "Disable";
            this.txt_wallpaper_status.Content = text;
        }

        private void LightningEnableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var isCheck = (sender as CheckBox).IsChecked ?? false;
            var text = isCheck ? "Enable" : "Disable";
            this.txt_lightviz_status.Content = text;
            if (isCheck != this.settings.EnableRgbRender)
                MessageBox.Show($"Lightning visualization {text} require application restart in order to take effect.", "ListenerX", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AverageLowpassEnableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var isCheck = (sender as CheckBox).IsChecked ?? false;
            this.txt_avg_lowpass_status.Content = isCheck ? "Enable" : "Disable";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.virtualKeyboardRenderer?.Dispose();
            this.settings.SaveChanges();
            base.OnClosing(e);
        }

        private void ClearButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CacheFileManager.ClearCache();
            this.txt_cache_mb.Content = Math.Round(CacheFileManager.GetCacheSize(), 2);
        }

        private void list_music_provider_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count != 0)
            {
                var selectedProvider = (sender as ComboBox).Text;
                MessageBox.Show($"Music provider has been changed to {selectedProvider} and require application restart in order to take effect.", "ListenerX", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}