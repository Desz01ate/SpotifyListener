using Microsoft.Win32;
using SpotifyListener.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener
{
    public class Wallpaper : IDisposable
    {
        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }
        string OriginalBackgroundImagePath { get; }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        public static readonly uint SPI_SETDESKWALLPAPER = 0x14;
        public static readonly uint SPIF_UPDATEINIFILE = 0x01;
        public static readonly uint SPIF_SENDWININICHANGE = 0x02;
        private static string BAK_IMAGE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IMGBAK.bak");
        public readonly float FontSize;
        public readonly string FontFamily;
        public IMusic Player;
        public Wallpaper(float fontSize, string fontFamily)
        {
            OriginalBackgroundImagePath = BAK_IMAGE;
            FontSize = fontSize;
            FontFamily = fontFamily;
            Disable();
        }
        public Wallpaper(string backgroundImagePath, float fontSize, string fontFamily)
        {
            OriginalBackgroundImagePath = backgroundImagePath;
            FontSize = fontSize;
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
                };
                byte[] path = (byte[])Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop").GetValue("TranscodedImageCache");
                var wallpaper_file_path = System.Text.Encoding.Unicode.GetString(SliceMe(path, 24)).TrimEnd("\0".ToCharArray());
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
            using (var image = CalculateBackgroundImage())
            {
                Set(image, Wallpaper.Style.Stretched);
            }
        }
        public void Disable()
        {
            Set(null, Wallpaper.Style.Stretched, OriginalBackgroundImagePath);
        }
        private bool Set(Image image, Style style, string path = "")
        {
            bool Success = false;
            string TempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            try
            {
                if (image != null)
                {
                    File.Create(TempPath).Close();
                    image.Save(TempPath, ImageFormat.Bmp);
                }

                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                switch (style)
                {
                    case Style.Stretched:
                        key.SetValue(@"WallpaperStyle", 2.ToString());

                        key.SetValue(@"TileWallpaper", 0.ToString());

                        break;

                    case Style.Centered:
                        key.SetValue(@"WallpaperStyle", 1.ToString());

                        key.SetValue(@"TileWallpaper", 0.ToString());

                        break;

                    default:
                    case Style.Tiled:
                        key.SetValue(@"WallpaperStyle", 1.ToString());

                        key.SetValue(@"TileWallpaper", 1.ToString());

                        break;

                }
                if (!string.IsNullOrWhiteSpace(path))
                {
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                }
                else
                {
                    SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, TempPath, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                }
                Success = true;

            }
            catch
            {
                //ex.HandleException();
            }
            finally
            {
                if (File.Exists(TempPath))
                {
                    File.Delete(TempPath);
                }
                image?.Dispose();
            }
            return Success;
        }
        internal void SetPlayerBase(IMusic music)
        {
            Player = music;
        }
        private Image CalculateBackgroundImage(Image highlightImage, Image backgroundimage, string track, string album, string artist)
        {
            var image = backgroundimage;
            var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            image = image.Resize((int)screenWidth, (int)screenHeight);
            using (highlightImage)
            using (backgroundimage)
            using (var g = Graphics.FromImage(image))
            {
                var highlightX = (int)((screenWidth - highlightImage.Width) / 2);
                var highlightY = (int)((screenHeight - highlightImage.Height) / 2) - (int)(screenHeight * 0.12);
                g.DrawImage(highlightImage, highlightX, highlightY);

                using (var font = new Font(FontFamily, FontSize, FontStyle.Regular))
                {
                    using (var trackFont = new Font(FontFamily, FontSize * 1.3f, FontStyle.Bold))
                    {
                        var trackMeasure = g.MeasureString(track, trackFont);
                        var albumMeasure = g.MeasureString(album, font);
                        var artistMeasure = g.MeasureString(artist, font);
                        g.DrawString(track, trackFont, Brushes.White, (int)((screenWidth - trackMeasure.Width) / 2), highlightY + highlightImage.Height + (int)(screenHeight * 0.07));
                        g.DrawString(album, font, Brushes.White, (int)((screenWidth - albumMeasure.Width) / 2), highlightY + highlightImage.Height + (int)(screenHeight * 0.15));
                        g.DrawString(artist, font, Brushes.White, (int)((screenWidth - artistMeasure.Width) / 2), highlightY + highlightImage.Height + (int)(screenHeight * 0.2));
                    }
                }
            }
            return image;
        }
        private Image CalculateBackgroundImage()
        {
            var artwork = (Player.AlbumArtwork.Clone() as Image);
            var background = Effects.Bitmap.CalculateBackgroundSource(artwork.Clone() as Image);
            using (background)
            {
                using (artwork)
                {
                    var highlightSize = (int)Math.Round(System.Windows.SystemParameters.PrimaryScreenHeight * 0.555);
                    var image = CalculateBackgroundImage(
                        artwork.Resize(highlightSize, highlightSize),
                        background,
                        Player.Track, Player.Album, Player.Artist);

                    return image;
                }
            }
        }
        public void SaveWallpaperToFile(string filePath)
        {
            using (var image = CalculateBackgroundImage())
            {
                image.Save(filePath);
            }
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Disable();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
