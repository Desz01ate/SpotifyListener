using Microsoft.Win32;
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
        public Image TrackImage { get; private set; }
        private Image PersistenceTrackImage { get; set; }
        [DllImport("user32.dll")]
        public static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        public static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        public static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        public static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;
        private static string BAK_IMAGE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IMGBAK.bak");
        public Wallpaper()
        {
            OriginalBackgroundImagePath = BAK_IMAGE;
            Disable();
        }
        public Wallpaper(string backgroundImagePath)
        {
            OriginalBackgroundImagePath = backgroundImagePath;
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
            //Set(TrackImage, Wallpaper.Style.Stretched);

            Set(TrackImage, Wallpaper.Style.Stretched);

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
        public void CalculateBackgroundImage(Image highlightImage, Image backgroundimage, string fontFamily, float fontSize, string track, string album, string artist, Func<Image, Image> backgroundApplyFunction = null, Func<Image, Image> highlightApplyFunction = null)
        {
            TrackImage?.Dispose();
            PersistenceTrackImage?.Dispose();
            var image = backgroundimage;
            var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            if (backgroundApplyFunction != null)
            {
                image = backgroundApplyFunction(image);
            }
            if (highlightApplyFunction != null)
            {
                highlightImage = highlightApplyFunction(highlightImage);
            }

            image = image.Resize((int)screenWidth, (int)screenHeight);

            using (var g = Graphics.FromImage(image))
            {
                var highlightX = (int)((screenWidth - highlightImage.Width) / 2);
                var highlightY = (int)((screenHeight - highlightImage.Height) / 2) - (int)(screenHeight * 0.12);
                g.DrawImage(highlightImage, highlightX, highlightY);

                var font = new Font(fontFamily, fontSize, FontStyle.Bold);
                var trackFont = new Font(fontFamily, fontSize * 1.3f, FontStyle.Bold);
                var trackMeasure = g.MeasureString(track, trackFont);
                var albumMeasure = g.MeasureString(album, font);
                var artistMeasure = g.MeasureString(artist, font);
                g.DrawString(track, trackFont, Brushes.White, (int)((screenWidth - trackMeasure.Width) / 2), highlightY + highlightImage.Height + (int)(screenHeight * 0.07));
                g.DrawString(album, font, Brushes.White, (int)((screenWidth - albumMeasure.Width) / 2), highlightY + highlightImage.Height + (int)(screenHeight * 0.15));
                g.DrawString(artist, font, Brushes.White, (int)((screenWidth - artistMeasure.Width) / 2), highlightY + highlightImage.Height + (int)(screenHeight * 0.2));
                font.Dispose();
                trackFont.Dispose();
            }
            highlightImage.Dispose();
            backgroundimage.Dispose();
            TrackImage = image;
            PersistenceTrackImage = image.Clone() as Image;
        }
        public void SaveWallpaperToFile(string filePath)
        {
            PersistenceTrackImage.Save(filePath);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Disable();
                TrackImage?.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
