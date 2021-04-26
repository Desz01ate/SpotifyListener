using Listener.Core.Framework.Helpers;
using ListenerX.Visualization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Classes
{
    public class Settings : ISettings, INotifyPropertyChanged
    {
        private bool enableArtworkWallpaper = true;
        private string activePlayerModule = "Spotify";
        private bool enableRgbRender = false;
        private uint rgbRenderFps = 60;
        private int rgbRenderStyle = 0;
        private float rgbRenderAmplitude = 100;
        private float rgbRenderBackgroundMultiplier = 20;
        private bool rgbRenderAverageSpectrum = false;
        private ScalingStrategy rgbRenderScalingStrategy = ScalingStrategy.Sqrt;

        public bool EnableArtworkWallpaper
        {
            get => enableArtworkWallpaper;
            set
            {
                enableArtworkWallpaper = value;
                NotifyPropertyChanged(nameof(EnableArtworkWallpaper));
            }
        }

        public string ActivePlayerModule
        {
            get => activePlayerModule;
            set
            {
                activePlayerModule = value;
                NotifyPropertyChanged(nameof(ActivePlayerModule));
            }
        }
        public bool EnableRgbRender
        {
            get => enableRgbRender;
            set
            {
                enableRgbRender = value;
                NotifyPropertyChanged(nameof(EnableRgbRender));
            }
        }
        public uint RgbRenderFps
        {
            get => rgbRenderFps; set
            {
                rgbRenderFps = value;
                NotifyPropertyChanged(nameof(RgbRenderFps));
            }
        }
        public int RgbRenderStyle
        {
            get => rgbRenderStyle; set
            {
                rgbRenderStyle = value;
                NotifyPropertyChanged(nameof(RgbRenderStyle));
            }
        }
        public float RgbRenderAmplitude
        {
            get => rgbRenderAmplitude; set
            {
                rgbRenderAmplitude = value;
                NotifyPropertyChanged(nameof(RgbRenderAmplitude));
            }
        }
        public float RgbRenderBackgroundMultiplier
        {
            get => rgbRenderBackgroundMultiplier; set
            {
                rgbRenderBackgroundMultiplier = value;
                NotifyPropertyChanged(nameof(RgbRenderBackgroundMultiplier));
            }
        }
        public bool RgbRenderAverageSpectrum
        {
            get => rgbRenderAverageSpectrum; set
            {
                rgbRenderAverageSpectrum = value;
                NotifyPropertyChanged(nameof(RgbRenderAverageSpectrum));
            }
        }
        public ScalingStrategy RgbRenderScalingStrategy
        {
            get => rgbRenderScalingStrategy; set
            {
                rgbRenderScalingStrategy = value;
                NotifyPropertyChanged(nameof(RgbRenderScalingStrategy));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SaveChanges()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(SettingsPath, json, Encoding.UTF8);
        }

        public static ISettings LoadSettings()
        {
            ISettings settings;
            if (!File.Exists(SettingsPath))
            {
                settings = new Settings();
            }
            else
            {
                var json = File.ReadAllText(SettingsPath);
                settings = JsonConvert.DeserializeObject<Settings>(json);
            }
            return settings;
        }

        private static string SettingsPath => System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "configuration.json");
    }
}
