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
        public static Color AverageColor(this Image img)
        {
            using (var bitmap = (Bitmap)img)
            {
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
                return result;
            }

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
        }
        public static Image SetOpacity(this Image image, double opacity, Color color)
        {
            if (opacity < 0 || 1 < opacity)
                return image;
            var alpha = 255 * opacity;
            using (var g = Graphics.FromImage(image))
            {
                using (var brush = new SolidBrush(Color.FromArgb((int)alpha, color)))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, image.Width, image.Height));
                }

            }
            return image;
        }
        public static Image Cut(this Image image, double width, double height)
        {
            var diffOffset = height / width;
            if (!(0 < diffOffset && diffOffset < 1)) throw new Exception("width and height offset is out of range (height divide by width must inside of range 0~1).");

            var scale = 96;// (int)((((1920 - System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) * 0.0000732421875) + 0.05) * System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width);//96;
            var baseBitmap = (Bitmap)image;
            baseBitmap.SetResolution(scale, scale);
            var newImage = new Bitmap(baseBitmap.Width, (int)(baseBitmap.Width * diffOffset));
            var drawer = Graphics.FromImage(newImage);
            var offsetX = 0;//(int)(image.Height * (1 - diffOffset) / 2);
            var offsetY = (int)(baseBitmap.Width * (1 - diffOffset) / 2);
            drawer.DrawImage(baseBitmap, 0, 0, new Rectangle(offsetX, offsetY, baseBitmap.Width, baseBitmap.Width), GraphicsUnit.Pixel);
            return newImage;
        }
        public static Image Blur(this Image image, int radial)
        {
            if (image.Width == 1 && image.Height == 1)
                return image;
            var result = GaussianBlur.GaussianBlur.Blur(image as Bitmap, radial);
            return result;
        }
        public static Image Resize(this Image img, int outputWidth, int outputHeight)
        {

            if (img == null || (img.Width == outputWidth && img.Height == outputHeight)) return img;
            Bitmap outputImage;
            Graphics graphics;
            try
            {
                outputImage = new Bitmap(outputWidth, outputHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
                graphics = Graphics.FromImage(outputImage);
                graphics.DrawImage(img, new Rectangle(0, 0, outputWidth, outputHeight),new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                return outputImage;
            }
            catch
            {
                throw;
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
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmapimage.Freeze();
                return bitmapimage;
            }
        }
        public static BitmapImage ToBitmapImage(this Image src)
        {
            return ToBitmapImage(src as Bitmap);
        }
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        public static System.Windows.Media.Brush ToSafeMemoryBrush(this Bitmap src)
        {
            IntPtr hBitMap = src.GetHbitmap();
            System.Windows.Media.ImageBrush b = new System.Windows.Media.ImageBrush(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitMap, IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
            DeleteObject(hBitMap);
            return b;
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
        public static string ToHex(this Color c)
        {
            return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
        }
    }
}


namespace GaussianBlur
{
    public class GaussianBlur
    {


        private readonly int[] _alpha;
        private readonly int[] _red;
        private readonly int[] _green;
        private readonly int[] _blue;

        private readonly int _width;
        private readonly int _height;
        private readonly ParallelOptions _pOptions = new ParallelOptions { MaxDegreeOfParallelism = 16 };
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
        public static Bitmap Blur(Bitmap image, int radial)
        {
            var gb = new GaussianBlur(image);
            return gb.Process(radial);
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
