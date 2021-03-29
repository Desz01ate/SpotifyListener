using Listener.Core.Framework.Helpers;
using Listener.Core.Framework.Players;
using Listener.ImageProcessing;
using ListenerX.Helpers;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ListenerX.Classes
{
    public class Wallpaper : IDisposable
    {
        enum Style
        {
            Tiled,
            Centered,
            Stretched
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        public const uint SPI_SETDESKWALLPAPER = 0x14;
        public const uint SPIF_UPDATEINIFILE = 0x01;
        public const uint SPIF_SENDWININICHANGE = 0x02;

        private readonly string _fontFamily;
        private readonly string OriginalBackgroundImagePath;
        private readonly uint OriginalAccentColor, OriginalColorizationAfterglow, OriginalColorizationColor;
        private readonly uint OriginalStartColorMenu, OriginalAccentColorMenu;
        private readonly byte[] OriginalAccentPallete;

        private bool disposed = false;
        private string _temporaryWaitForDeleteFiles = "";

        public Wallpaper(string fontFamily)
        {
            var backgroundImagePathCache = Properties.Settings.Default.BackgroundImagePath;

            if (!string.IsNullOrWhiteSpace(backgroundImagePathCache))
            {
                OriginalBackgroundImagePath = backgroundImagePathCache;
            }
            else
            {
                using RegistryKey desktopKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                var path = (string)desktopKey.GetValue("Wallpaper");
                OriginalBackgroundImagePath = path;
                Properties.Settings.Default.BackgroundImagePath = path;
                Properties.Settings.Default.Save();
            }

            this._fontFamily = fontFamily;

            using var dwmKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", true);
            this.OriginalAccentColor = DwordConversion((int)dwmKey.GetValue("AccentColor"));//4282927692
            //this.OriginalColorizationAfterglow = DwordConversion((int)dwmKey.GetValue("ColorizationAfterglow")); //3293334088
            //this.OriginalColorizationColor = DwordConversion((int)dwmKey.GetValue("ColorizationColor")); //3293334088
            //this.OriginalStartColorMenu = DwordConversion((int)explorerAccentKey.GetValue("StartColorMenu"));
            //this.OriginalAccentColorMenu = DwordConversion((int)explorerAccentKey.GetValue("AccentColorMenu"));
            //this.OriginalAccentPallete = (byte[])explorerAccentKey.GetValue("AccentPalette");

            //Disable();
        }

        public void Enable(IPlayerHost player)
        {
            using var image = CalculateBackgroundImage(player, (int)System.Windows.SystemParameters.PrimaryScreenWidth, (int)System.Windows.SystemParameters.PrimaryScreenHeight);
            var color = player.AlbumArtwork.GetDominantColors(1).First();

            DeleteTempFile();
            SetDesktopWallpaper(image, Wallpaper.Style.Centered);
            SetAeroColor(color);
        }

        public void Disable()
        {
            DeleteTempFile();
            SetDesktopWallpaper(this.OriginalBackgroundImagePath, Wallpaper.Style.Stretched);
            SetAeroColor(this.OriginalAccentColor);
        }

        private bool SetDesktopWallpaper(string path, Style style)
        {
            using RegistryKey desktopKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            var values = style switch
            {
                Style.Stretched => ("2", "0"),
                Style.Centered => ("1", "0"),
                Style.Tiled => ("1", "1"),
                _ => throw new NotSupportedException(style.ToString())
            };
            desktopKey.SetValue("WallpaperStyle", values.Item1);
            desktopKey.SetValue("TileWallpaper", values.Item2);
            Task.Run(() => SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE));
            return true;
        }

        private void SetAeroColor(Color color)
        {
            SetAeroColor(Color.FromArgb(color.A, color.B, color.G, color.R).ToUint());
        }
        private void SetAeroColor(uint hex)
        {
            unchecked
            {
                SetAeroColor((int)hex);
            }
        }
        private void SetAeroColor(int hex)
        {
            if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 2)
            {
                using var dwmKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", true);
                //using var explorerAccentKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent", true);
                dwmKey.SetValue("AccentColor", hex, RegistryValueKind.DWord);
                //if (image == null)
                //{
                //    dwmKey.SetValue("AccentColor", (int)this.OriginalAccentColor, RegistryValueKind.DWord);
                //    //dwmKey.SetValue("ColorizationAfterglow", (int)this.OriginalColorizationAfterglow, RegistryValueKind.DWord);
                //    //dwmKey.SetValue("ColorizationColor", (int)this.OriginalColorizationColor, RegistryValueKind.DWord);
                //    //explorerAccentKey.SetValue("StartColorMenu", (int)this.OriginalStartColorMenu, RegistryValueKind.DWord);
                //    //explorerAccentKey.SetValue("AccentColorMenu", (int)this.OriginalAccentColorMenu, RegistryValueKind.DWord);
                //}
                //else
                //{
                //    var color = this.player.AlbumArtwork.GetDominantColors(1).First();
                //    //due to windows is weird shit, arrange ARGB as ABGR instead so we need to swap rgb position.
                //    var swappedColor = Color.FromArgb(color.A, color.B, color.G, color.R).ToUint();
                //    dwmKey.SetValue("AccentColor", (int)swappedColor, RegistryValueKind.DWord);
                //    //dwmKey.SetValue("ColorizationAfterglow", (int)swappedColor, RegistryValueKind.DWord);
                //    //dwmKey.SetValue("ColorizationColor", (int)swappedColor, RegistryValueKind.DWord);
                //    //explorerAccentKey.SetValue("StartColorMenu", (int)swappedColor, RegistryValueKind.DWord);
                //    //explorerAccentKey.SetValue("AccentColorMenu", (int)swappedColor, RegistryValueKind.DWord);
                //}
            }
        }

        private bool SetDesktopWallpaper(Image image, Style style)
        {
            if (image == null)
            {
                return false;
            }
            _temporaryWaitForDeleteFiles = CacheFileManager.GetTempPath().Replace("tmp", "jpg");

            File.Create(_temporaryWaitForDeleteFiles).Close();
            image.Save(_temporaryWaitForDeleteFiles, image.RawFormat);

            return SetDesktopWallpaper(_temporaryWaitForDeleteFiles, style);
        }

        private uint DwordConversion(int value)
        {
            var binary = Convert.ToString(value, 2);
            var converted = Convert.ToUInt32(binary, 2);
            return converted;
        }

        private Image CalculateBackgroundImage(Image highlightImg, Image backgroundImg, string track, string album,
            string artist, float width, float height)
        {
            var screenWidth = width;
            var screenHeight = height;
            backgroundImg = backgroundImg.Resize((int)screenWidth, (int)screenHeight);
            using var g = Graphics.FromImage(backgroundImg);

            var highlightX = (int)((screenWidth - highlightImg.Width) / 2);
            var highlightY = (int)((screenHeight - highlightImg.Height) / 2) - (int)(screenHeight * 0.12);
            g.DrawImage(highlightImg, highlightX, highlightY);

            var fontSize = width * 0.0052592592592593f;//0.0185185185185185f;
            using var font = new Font(_fontFamily, fontSize, FontStyle.Regular);
            using var trackFont = new Font(_fontFamily, fontSize * 1.3f, FontStyle.Bold);

            var trackMeasure = g.MeasureString(track, trackFont);
            var albumMeasure = g.MeasureString(album, font);
            var artistMeasure = g.MeasureString(artist, font);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.DrawString(track, trackFont, Brushes.White, (int)((screenWidth - trackMeasure.Width) / 2),
                highlightY + highlightImg.Height + (int)(screenHeight * 0.07));
            g.DrawString(album, font, Brushes.White, (int)((screenWidth - albumMeasure.Width) / 2),
                highlightY + highlightImg.Height + (int)(screenHeight * 0.15));
            g.DrawString(artist, font, Brushes.White, (int)((screenWidth - artistMeasure.Width) / 2),
                highlightY + highlightImg.Height + (int)(screenHeight * 0.2));

            return backgroundImg;
        }

        private Image CalculateBackgroundImage(IPlayerHost player, int width, int height)
        {
            var highlightSize = (int)Math.Round(height * 0.555);

            var artwork = player.AlbumArtwork;
            using var background = ImageProcessing.CalculateBackgroundSource(
                artwork,
                width,
                height,
                10
            );
            Image highlight;
            if (artwork.Width == artwork.Height)
            {
                highlight = artwork.Resize(highlightSize, highlightSize);
            }
            else
            {
                if (artwork.Width < artwork.Height)
                {
                    var factor = (float)highlightSize / artwork.Height;
                    highlight = artwork.Resize((int)(artwork.Width * factor), highlightSize);
                }
                else
                {
                    var factor = (float)highlightSize / artwork.Width;
                    highlight = artwork.Resize(highlightSize, (int)(artwork.Height * factor));
                }

            }
            using (highlight)
            {
                var image = CalculateBackgroundImage(
                    highlight,
                    background,
                    player.Track,
                    player.Album,
                    player.Artist,
                    width,
                    height);
                return image;

            }

        }

        public string GetWallpaperImage(IPlayerHost player)
        {
            if (File.Exists(_temporaryWaitForDeleteFiles))
                return _temporaryWaitForDeleteFiles;

            var fileName = "10." + RegularExpressionHelpers.AlphabetCleaner($"{player.Track}-{player.Album}-{player.Artist}") + ".jpg";
            if (!CacheFileManager.IsFileExists(fileName))
            {
                using var image = this.CalculateBackgroundImage(player, (int)System.Windows.SystemParameters.PrimaryScreenWidth, (int)System.Windows.SystemParameters.PrimaryScreenHeight);
                var bytes = image.ToByteArray(ImageFormat.Jpeg);
                CacheFileManager.SaveCache(fileName, bytes);
            }

            return CacheFileManager.GetFullCachePath(fileName);
        }

        private void DeleteTempFile()
        {
            if (File.Exists(_temporaryWaitForDeleteFiles))
            {
                File.Delete(_temporaryWaitForDeleteFiles);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.Disable();
            }
            this.disposed = true;
        }


        public void Dispose()
        {
            Properties.Settings.Default.BackgroundImagePath = string.Empty;
            Properties.Settings.Default.Save();
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}