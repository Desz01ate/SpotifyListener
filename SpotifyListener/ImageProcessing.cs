using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ColoreColor = Colore.Data.Color;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace SpotifyListener
{

    static class ImageProcessing
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        private static Dictionary<string, Color> DominantColorMemoized = new Dictionary<string, Color>();
        private static Dictionary<string, Color> AverageColorMemoized = new Dictionary<string, Color>();
        public static Color InverseColor(this Color c)
        {
            return Color.FromArgb((int)(Color.FromArgb(c.R, c.G, c.B).ToArgb() ^ 0xFFFFFFFu));
        }
        public static ColoreColor ComplementColor(this ColoreColor c)
        {
            return Color.FromArgb((int)(Color.FromArgb(c.R, c.G, c.B).ToArgb() ^ 0xFFFFFFFu)).ToColoreColor();
        }
        public static Color ChangeColorDensity(this Color c, double multiplier, double alpha = 255)
        {
            return Color.FromArgb((byte)alpha, (byte)(c.R * multiplier), (byte)(c.G * multiplier), (byte)(c.B * multiplier));
        }
        public static ColoreColor ChangeColorDensity(this ColoreColor c, double multiplier)
        {
            var R = (byte)(c.R * multiplier);
            var G = (byte)(c.G * multiplier);
            var B = (byte)(c.B * multiplier);
            //R = R > 255 ? 255 : R;
            //R = G > 255 ? 255 : G;
            //R = B > 255 ? 255 : B;
            return Color.FromArgb(R, G, B).ToColoreColor();
        }
        public static ColoreColor ToColoreColor(this Color c)
        {
            return new ColoreColor(c.R, c.G, c.B);
        }
        public static Color AverageColor(this Image img, string path = "")
        {
            var key = "";
            if (!string.IsNullOrEmpty(path))
            {
                key = CryptographicExtension.CalculateMD5(path);
                if (AverageColorMemoized.TryGetValue(key, out var savedColor))
                    return savedColor;

            }
            var bitmap = (Bitmap)img;
            var startX = 0;
            var startY = 0;
            int r = 0, g = 0, b = 0, total = 0;
            for (int x = startX; x < bitmap.Size.Width; x++)
            {
                for (int y = startY; y < bitmap.Size.Height; y++)
                {
                    Color clr = bitmap.GetPixel(x, y);
                    r += clr.R;
                    g += clr.G;
                    b += clr.B;
                    total++;
                }
            }
            //Calculate average
            r /= total;
            g /= total;
            b /= total;
            var result = Color.FromArgb(r, g, b);
            if (!string.IsNullOrEmpty(key))
                AverageColorMemoized.Add(CryptographicExtension.CalculateMD5(path), result);
            return result;
        }
        public static byte[] ToByteArray(this Image image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, image.RawFormat);
                return memoryStream.ToArray();
            }
        }
        public static Color DominantColor(this Image img, string path = "")
        {
            var colorThief = new ColorThiefDotNet.ColorThief();
            var dominant = colorThief.GetColor((Bitmap)img);
            return Color.FromArgb(dominant.Color.R, dominant.Color.G, dominant.Color.B);
            var key = "";
            if (!string.IsNullOrEmpty(path))
            {
                key = CryptographicExtension.CalculateMD5(path);
                if (DominantColorMemoized.TryGetValue(key, out var savedColor))
                    return savedColor;

            }
            var bitmap = (Bitmap)img;
            var startX = 0;
            var startY = 0;
            var colorIncidence = new Dictionary<int, int>();
            for (var x = startX; x < bitmap.Size.Width; x++)
                for (var y = startY; y < bitmap.Size.Height; y++)
                {
                    var pixelColor = bitmap.GetPixel(x, y).ToArgb();
                    if (colorIncidence.Keys.Contains(pixelColor))
                        colorIncidence[pixelColor]++;
                    else
                        colorIncidence.Add(pixelColor, 1);
                }

            var result = Color.FromArgb(colorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).First().Key);
            if (!string.IsNullOrEmpty(key))
                DominantColorMemoized.Add(key, result);
            return result;
        }
        public static Image SetOpacity(this Image image, double opacity, Color color)
        {
            if (opacity < 0 || 1 < opacity)
                return image;
            //var bitmap = new Bitmap((int)image.Width,(int)image.Height);
            //using (var graphics = Graphics.FromImage(bmp))
            //{
            //    var matrix = new ColorMatrix
            //    {
            //        Matrix33 = opacity
            //    };
            //    var attributes = new ImageAttributes();
            //    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            //    graphics.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            //}
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var alpha = 255 * opacity;
            using (var g = Graphics.FromImage(image))
            {
                using (var brush = new SolidBrush(System.Drawing.Color.FromArgb((int)alpha, color)))
                {
                    g.FillRectangle(brush, rect);
                }

            }
            return image;
        }

        public static System.Windows.Media.ImageSource Blur(this Image image, Int32 blurSize, double diffOffset = -1)
        {
            if (image.Width == 1 && image.Height == 1)
                return ((Bitmap)image).ToBitmapImage();
            try
            {
                var scale = 96;// (int)((((1920 - System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) * 0.0000732421875) + 0.05) * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width);//96;
                var baseBitmap = (Bitmap)image;
                baseBitmap.SetResolution(scale, scale);

                if (0 < diffOffset && diffOffset < 1)
                {
                    var newImage = new Bitmap(baseBitmap.Width, (int)(baseBitmap.Width * diffOffset));
                    var drawer = Graphics.FromImage(newImage);
                    drawer.DrawImage(baseBitmap, 0, 0, new Rectangle(0, (int)((baseBitmap.Width * (1 - diffOffset)) / 2), baseBitmap.Width, baseBitmap.Width), GraphicsUnit.Pixel);
                    image = newImage;
                }
                var blur = new GaussianBlur.GaussianBlur(image as Bitmap);
                var result = blur.Process(blurSize);
                try
                {
                    return result.ToBitmapImage();
                }
                finally
                {
                    result.Dispose();
                    blur = null;
                }
            }
            catch
            {
                return ((Bitmap)image).ToBitmapImage();
            }
        }
        public static Image Resize(this Image img, int outputWidth, int outputHeight)
        {
            if (img.Width == outputWidth && img.Height == outputHeight) return img;
            Bitmap outputImage = null;
            Graphics graphics = null;
            try
            {
                outputImage = new Bitmap(outputWidth, outputHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
                graphics = Graphics.FromImage(outputImage);
                graphics.DrawImage(img, new Rectangle(0, 0, outputWidth, outputHeight),
                new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);

                return outputImage;
            }
            catch
            {
                return img;
            }
        }
        public static Color ContrastColor(this Color c)
        {
            int d = 0;
            double a = 1 - (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255;
            if (a >= 0.5)
                d = 255;
            return Color.FromArgb(d, d, d);
        }
        public static ColoreColor ContrastColor(this ColoreColor c)
        {
            ColoreColor color = ColoreColor.Black;
            double a = 1 - (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255;
            if (a >= 0.5)
                color = ColoreColor.White;
            return color;
        }
        public static Color SoftColor(this Color c)
        {
            var rgb = new[] { c.R, c.G, c.B };
            if (rgb[0] < 10 || rgb[0] > 230 && rgb[1] < 10 || rgb[1] > 230 && rgb[2] < 10 || rgb[2] > 230)
            {
                return ContrastColor(c);
            }
            var max = rgb.Max();
            /* not the best code and also not a performance-wise, rather for a fucking lazy sake of me */
            for (var index = 0; index < rgb.Length; index++)
            {
                if (rgb[index] == max)
                {
                    var half = rgb[index] / 2;
                    if (index == 0)
                    {
                        var r = rgb[0];
                        var g = rgb[1] + half > 255 ? 255 : rgb[1] + half;
                        var b = rgb[2] + half > 255 ? 255 : rgb[2] + half;
                        return Color.FromArgb(r, g, b);
                    }
                    else if (index == 1)
                    {
                        var r = rgb[0] + half > 255 ? 255 : rgb[0] + half;
                        var g = rgb[1];
                        var b = rgb[2] + half > 255 ? 255 : rgb[2] + half;
                        return Color.FromArgb(r, g, b);
                    }
                    else if (index == 2)
                    {
                        var r = rgb[0] + half > 255 ? 255 : rgb[0] + half;
                        var g = rgb[1] + half > 255 ? 255 : rgb[1] + half;
                        var b = rgb[2];
                        return Color.FromArgb(r, g, b);
                    }
                }
            }
            /* fallback */
            return c;
        }
        public static ColoreColor SoftColor(this ColoreColor c)
        {
            var rgb = new[] { c.R, c.G, c.B };
            if (rgb[0] == rgb[1] && rgb[1] == rgb[2])
            {
                return ContrastColor(c);
            }
            var max = rgb.Max();
            /* not the best code and also not a performance-wise, rather for a fucking lazy sake of me */
            for (var index = 0; index < rgb.Length; index++)
            {
                if (rgb[index] == max)
                {
                    var half = rgb[index] / 2;
                    if (index == 0)
                    {
                        var r = rgb[0];
                        var g = rgb[1] + half > 255 ? 255 : rgb[1] + half;
                        var b = rgb[2] + half > 255 ? 255 : rgb[2] + half;
                        return new ColoreColor(r, g, b);
                    }
                    else if (index == 1)
                    {
                        var r = rgb[0] + half > 255 ? 255 : rgb[0] + half;
                        var g = rgb[1];
                        var b = rgb[2] + half > 255 ? 255 : rgb[2] + half;
                        return new ColoreColor(r, g, b);
                    }
                    else if (index == 2)
                    {
                        var r = rgb[0] + half > 255 ? 255 : rgb[0] + half;
                        var g = rgb[1] + half > 255 ? 255 : rgb[1] + half;
                        var b = rgb[2];
                        return new ColoreColor(r, g, b);
                    }
                }
            }
            /* fallback */
            return c;
        }
        public static Icon ToIcon(this Bitmap image)
        {
            IntPtr HIcon = image.GetHicon();
            //Icon result = Icon.FromHandle(HIcon);
            using (var result = Icon.FromHandle(HIcon))
            {
                return result;
            }
        }
        public static BitmapImage ToBitmapImage(this Bitmap src)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                src.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                try
                {

                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    bitmapimage.Freeze();
                    return bitmapimage;
                }
                finally
                {
                    bitmapimage = null;
                }
            }
        }
        public static Image ToImage(this System.Windows.Media.ImageSource imageSrc)
        {
            var encoder = new JpegBitmapEncoder
            {
                QualityLevel = 100
            };
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSrc));
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                var image = Image.FromStream(stream);
                return image;
            }
        }
        public static System.Windows.Media.ImageSource ToImageSource(this Image image)
        {
            var bitmap = new Bitmap(image);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   System.Windows.Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }
        public static string ToHex(this Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
    }
    public class Wallpaper
    {
        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }
        string OriginalBackgroundImagePath { get; }
        public Image OriginalBackgroundImage { get; }
        public Image TrackImage { get; private set; }
        [DllImport("user32.dll")]
        public static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        public static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        public static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        public static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;
        public Wallpaper() { }
        public Wallpaper(string backgroundImagePath)
        {
            OriginalBackgroundImage = Image.FromFile(backgroundImagePath);
            OriginalBackgroundImagePath = backgroundImagePath;
        }
        public void Enable()
        {
            Set(TrackImage, Wallpaper.Style.Stretched);
        }
        public void DisableTemporary()
        {
            Set(OriginalBackgroundImage, Wallpaper.Style.Stretched);
        }
        public void Disable()
        {
            Set(OriginalBackgroundImage, Wallpaper.Style.Stretched, OriginalBackgroundImagePath);
        }
        //public void SetTrackImage(Image image)
        //{
        //    TrackImage = image;
        //}
        private bool Set(string filePath, Style style)
        {
            bool Success = false;
            try
            {
                Image i = System.Drawing.Image.FromFile(Path.GetFullPath(filePath));

                Set(i, style);

                Success = true;

            }
            catch //(Exception ex)
            {
                //ex.HandleException();
            }
            return Success;
        }
        private bool Set(Image image, Style style, string path = "")
        {
            bool Success = false;
            string TempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            File.Create(TempPath).Close();
            try
            {
                image.Save(TempPath, ImageFormat.Bmp);

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
            }
            return Success;
        }
        public void CalculateBackgroundImage(Image highlightImage, Image backgroundimage, string fontFamily, float fontSize, string track, string album, string artist, Func<Image, Image> backgroundApplyFunction = null, Func<Image, Image> highlightApplyFunction = null)
        {
            var image = backgroundimage;
            var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            if (backgroundApplyFunction != null)
            {
                image = backgroundApplyFunction(image);//image.SetOpacity(0.6f);
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

                var trackMeasure = g.MeasureString(track, font);
                var albumMeasure = g.MeasureString(album, font);
                var artistMeasure = g.MeasureString(artist, font);
                g.DrawString(track, font, Brushes.White, (int)((screenWidth - trackMeasure.Width) / 2), highlightY + highlightImage.Height + (int)(screenHeight * 0.1));
                g.DrawString(album, font, Brushes.White, (int)((screenWidth - albumMeasure.Width) / 2), highlightY + highlightImage.Height + (int)(screenHeight * 0.15));
                g.DrawString(artist, font, Brushes.White, (int)((screenWidth - artistMeasure.Width) / 2), highlightY + highlightImage.Height + (int)(screenHeight * 0.2));
            }
            TrackImage = image;
        }
    }
}


namespace GaussianBlur
{
    public class GaussianBlur : IDisposable
    {


        private readonly int[] _alpha;
        private readonly int[] _red;
        private readonly int[] _green;
        private readonly int[] _blue;

        private readonly int _width;
        private readonly int _height;
        private readonly ParallelOptions _pOptions = new ParallelOptions { MaxDegreeOfParallelism = 16 };
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public GaussianBlur(Bitmap image)
        {
            var rct = new Rectangle(0, 0, image.Width, image.Height);
            var source = new int[rct.Width * rct.Height];
            var cloneImage = image.Clone() as Bitmap;
            var bits = cloneImage.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(bits.Scan0, source, 0, source.Length);
            cloneImage.UnlockBits(bits);

            _width = cloneImage.Width;
            _height = cloneImage.Height;

            _alpha = new int[_width * _height];
            _red = new int[_width * _height];
            _green = new int[_width * _height];
            _blue = new int[_width * _height];

            Parallel.For(0, source.Length, _pOptions, i =>
            {
                _alpha[i] = (int)((source[i] & 0xff000000) >> 24);
                _red[i] = (source[i] & 0xff0000) >> 16;
                _green[i] = (source[i] & 0x00ff00) >> 8;
                _blue[i] = (source[i] & 0x0000ff);
            });
        }

        public Bitmap Process(int radial)
        {
            var newAlpha = new int[_width * _height];
            var newRed = new int[_width * _height];
            var newGreen = new int[_width * _height];
            var newBlue = new int[_width * _height];
            var dest = new int[_width * _height];

            Parallel.Invoke(
                () => GaussBlur_4(_alpha, newAlpha, radial),
                () => GaussBlur_4(_red, newRed, radial),
                () => GaussBlur_4(_green, newGreen, radial),
                () => GaussBlur_4(_blue, newBlue, radial));

            Parallel.For(0, dest.Length, _pOptions, i =>
            {
                if (newAlpha[i] > 255) newAlpha[i] = 255;
                if (newRed[i] > 255) newRed[i] = 255;
                if (newGreen[i] > 255) newGreen[i] = 255;
                if (newBlue[i] > 255) newBlue[i] = 255;

                if (newAlpha[i] < 0) newAlpha[i] = 0;
                if (newRed[i] < 0) newRed[i] = 0;
                if (newGreen[i] < 0) newGreen[i] = 0;
                if (newBlue[i] < 0) newBlue[i] = 0;

                dest[i] = (int)((uint)(newAlpha[i] << 24) | (uint)(newRed[i] << 16) | (uint)(newGreen[i] << 8) | (uint)newBlue[i]);
            });

            var image = new Bitmap(_width, _height);
            var rct = new Rectangle(0, 0, image.Width, image.Height);
            var bits2 = image.LockBits(rct, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(dest, 0, bits2.Scan0, dest.Length);
            image.UnlockBits(bits2);
            try
            {
                return image;
            }
            finally
            {
                newAlpha = null;
                newRed = null;
                newBlue = null;
                newGreen = null;
                bits2 = null;
                dest = null;
                image = null;
            }
        }

        private void GaussBlur_4(int[] source, int[] dest, int r)
        {
            var bxs = BoxesForGauss(r, 3);
            BoxBlur_4(source, dest, _width, _height, (bxs[0] - 1) / 2);
            BoxBlur_4(dest, source, _width, _height, (bxs[1] - 1) / 2);
            BoxBlur_4(source, dest, _width, _height, (bxs[2] - 1) / 2);
        }

        private int[] BoxesForGauss(int sigma, int n)
        {
            var wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1);
            var wl = (int)Math.Floor(wIdeal);
            if (wl % 2 == 0) wl--;
            var wu = wl + 2;

            var mIdeal = (double)(12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            var m = Math.Round(mIdeal);

            var sizes = new List<int>();
            for (var i = 0; i < n; i++) sizes.Add(i < m ? wl : wu);
            return sizes.ToArray();
        }

        private void BoxBlur_4(int[] source, int[] dest, int w, int h, int r)
        {
            for (var i = 0; i < source.Length; i++) dest[i] = source[i];
            BoxBlurH_4(dest, source, w, h, r);
            BoxBlurT_4(source, dest, w, h, r);
        }

        private void BoxBlurH_4(int[] source, int[] dest, int w, int h, int r)
        {
            var iar = (double)1 / (r + r + 1);
            Parallel.For(0, h, _pOptions, i =>
            {
                var ti = i * w;
                var li = ti;
                var ri = ti + r;
                var fv = source[ti];
                var lv = source[ti + w - 1];
                var val = (r + 1) * fv;
                for (var j = 0; j < r; j++) val += source[ti + j];
                for (var j = 0; j <= r; j++)
                {
                    val += source[ri++] - fv;
                    dest[ti++] = (int)Math.Round(val * iar);
                }
                for (var j = r + 1; j < w - r; j++)
                {
                    val += source[ri++] - dest[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }
                for (var j = w - r; j < w; j++)
                {
                    val += lv - source[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }
            });
        }

        private void BoxBlurT_4(int[] source, int[] dest, int w, int h, int r)
        {
            var iar = (double)1 / (r + r + 1);
            Parallel.For(0, w, _pOptions, i =>
            {
                var ti = i;
                var li = ti;
                var ri = ti + r * w;
                var fv = source[ti];
                var lv = source[ti + w * (h - 1)];
                var val = (r + 1) * fv;
                for (var j = 0; j < r; j++) val += source[ti + j * w];
                for (var j = 0; j <= r; j++)
                {
                    val += source[ri] - fv;
                    dest[ti] = (int)Math.Round(val * iar);
                    ri += w;
                    ti += w;
                }
                for (var j = r + 1; j < h - r; j++)
                {
                    val += source[ri] - source[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ri += w;
                    ti += w;
                }
                for (var j = h - r; j < h; j++)
                {
                    val += lv - source[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ti += w;
                }
            });
        }
    }
}
