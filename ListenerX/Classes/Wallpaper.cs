using Listener.Core.Framework.Helpers;
using Listener.Core.Framework.Players;
using Listener.ImageProcessing;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ListenerX.Classes
{
    public class Wallpaper : IDisposable
    {
        public enum Style
        {
            Tiled,
            Centered,
            Stretched
        }

        string OriginalBackgroundImagePath { get; }

        private uint? OriginalAccentColor, OriginalColorizationAfterglow, OriginalColorizationColor;
        private uint? OriginalStartColorMenu, OriginalAccentColorMenu;
        private byte[] OriginalAccentPallete;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        public static readonly uint SPI_SETDESKWALLPAPER = 0x14;
        public static readonly uint SPIF_UPDATEINIFILE = 0x01;
        public static readonly uint SPIF_SENDWININICHANGE = 0x02;
        private static readonly string BAK_IMAGE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IMGBAK.bak");
        private readonly string FontFamily;
        private IPlayerHost Player;
        private string temporaryWaitForDeleteFiles = "";

        public Wallpaper(string fontFamily)
        {
            OriginalBackgroundImagePath = BAK_IMAGE;
            FontFamily = fontFamily;
            Disable();
        }

        public Wallpaper(string backgroundImagePath, string fontFamily)
        {
            OriginalBackgroundImagePath = backgroundImagePath;
            FontFamily = fontFamily;
            try
            {
                File.Copy(backgroundImagePath, BAK_IMAGE, true);
            }
            catch
            {
            }
        }

        public static bool TryGetWallpaper(out string imagePath)
        {
            try
            {
                byte[] SliceMe(byte[] source, int pos)
                {
                    byte[] dest = new byte[source.Length - pos];
                    Array.Copy(source, pos, dest, 0, dest.Length);
                    return dest;
                }

                ;
                byte[] path = (byte[])Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop")
                    .GetValue("TranscodedImageCache");
                var wallpaper_file_path = System.Text.Encoding.Unicode.GetString(SliceMe(path, 24))
                    .TrimEnd("\0".ToCharArray());
                imagePath = wallpaper_file_path;
                return File.Exists(wallpaper_file_path);
            }
            catch
            {
                imagePath = null;
                return false;
            }
        }

        public void Enable()
        {
            using var image = CalculateBackgroundImage((int)System.Windows.SystemParameters.PrimaryScreenWidth, (int)System.Windows.SystemParameters.PrimaryScreenHeight);
            Set(image, Wallpaper.Style.Stretched);
        }

        public void Disable()
        {
            Set(null, Wallpaper.Style.Stretched, OriginalBackgroundImagePath);
        }

        private bool Set(Image image, Style style, string path = "")
        {
            DeleteTempFile();
            bool success = false;
            string tempPath = CacheFileManager.GetTempPath().Replace("tmp", "jpg");
            using RegistryKey desktopKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            temporaryWaitForDeleteFiles = tempPath;
            try
            {
                if (image != null)
                {
                    File.Create(tempPath).Close();
                    image.Save(tempPath, image.RawFormat);
                }
                switch (style)
                {
                    case Style.Stretched:
                        desktopKey.SetValue(@"WallpaperStyle", "2");
                        desktopKey.SetValue(@"TileWallpaper", "0");
                        break;
                    case Style.Centered:
                        desktopKey.SetValue(@"WallpaperStyle", "1");
                        desktopKey.SetValue(@"TileWallpaper", "0");
                        break;
                    default:
                    case Style.Tiled:
                        desktopKey.SetValue(@"WallpaperStyle", "1");
                        desktopKey.SetValue(@"TileWallpaper", "1");
                        break;
                }

                if (!string.IsNullOrWhiteSpace(path))
                {
                    Task.Run(() =>
                        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
                            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE));
                }
                else
                {
                    Task.Run(() => SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempPath,
                        SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE));
                }
                if (Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 2)
                {
                    using var dwmKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", true);
                    //using var explorerAccentKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent", true);
                    if (!OriginalAccentColor.HasValue)
                    {
                        this.OriginalAccentColor = DwordConversion((int)dwmKey.GetValue("AccentColor"));//4282927692
                        //this.OriginalColorizationAfterglow = DwordConversion((int)dwmKey.GetValue("ColorizationAfterglow")); //3293334088
                        //this.OriginalColorizationColor = DwordConversion((int)dwmKey.GetValue("ColorizationColor")); //3293334088
                        //this.OriginalStartColorMenu = DwordConversion((int)explorerAccentKey.GetValue("StartColorMenu"));
                        //this.OriginalAccentColorMenu = DwordConversion((int)explorerAccentKey.GetValue("AccentColorMenu"));
                        //this.OriginalAccentPallete = (byte[])explorerAccentKey.GetValue("AccentPalette");
                    }
                    unchecked
                    {
                        if (image == null)
                        {
                            dwmKey.SetValue("AccentColor", (int)this.OriginalAccentColor, RegistryValueKind.DWord);
                            //dwmKey.SetValue("ColorizationAfterglow", (int)this.OriginalColorizationAfterglow, RegistryValueKind.DWord);
                            //dwmKey.SetValue("ColorizationColor", (int)this.OriginalColorizationColor, RegistryValueKind.DWord);
                            //explorerAccentKey.SetValue("StartColorMenu", (int)this.OriginalStartColorMenu, RegistryValueKind.DWord);
                            //explorerAccentKey.SetValue("AccentColorMenu", (int)this.OriginalAccentColorMenu, RegistryValueKind.DWord);
                        }
                        else
                        {
                            var color = this.Player.AlbumArtwork.GetDominantColors(1).First();
                            //due to windows is weird shit, arrange ARGB as ABGR instead so we need to swap rgb position.
                            var swappedColor = Color.FromArgb(color.A, color.B, color.G, color.R).ToUint();
                            dwmKey.SetValue("AccentColor", (int)swappedColor, RegistryValueKind.DWord);
                            //dwmKey.SetValue("ColorizationAfterglow", (int)swappedColor, RegistryValueKind.DWord);
                            //dwmKey.SetValue("ColorizationColor", (int)swappedColor, RegistryValueKind.DWord);
                            //explorerAccentKey.SetValue("StartColorMenu", (int)swappedColor, RegistryValueKind.DWord);
                            //explorerAccentKey.SetValue("AccentColorMenu", (int)swappedColor, RegistryValueKind.DWord);
                        }
                    }
                }
                success = true;
            }
            finally
            {
                //pass
            }

            return success;
            uint DwordConversion(int value)
            {
                var binary = Convert.ToString(value, 2);
                var converted = Convert.ToUInt32(binary, 2);
                return converted;
            }
        }

        internal void SetPlayerBase(IPlayerHost music)
        {
            Player = music;
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
            using var font = new Font(FontFamily, fontSize, FontStyle.Regular);
            using var trackFont = new Font(FontFamily, fontSize * 1.3f, FontStyle.Bold);

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

        private Image CalculateBackgroundImage(int width, int height)
        {

            var highlightSize = (int)Math.Round(height * 0.555);

            var artwork = Player.AlbumArtwork;
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
                    Player.Track,
                    Player.Album,
                    Player.Artist,
                    width,
                    height);
                return image;

            }

        }

        public string GetWallpaperImage()
        {
            return temporaryWaitForDeleteFiles;
            //var image = CalculateBackgroundImage(width, height);
            //return image;
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Disable();
                DeleteTempFile();
            }
        }

        private void DeleteTempFile()
        {
            if (File.Exists(temporaryWaitForDeleteFiles))
            {
                File.Delete(temporaryWaitForDeleteFiles);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}