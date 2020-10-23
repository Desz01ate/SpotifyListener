using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Listener.ImageProcessing
{
    public static class ImageProcessing
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public static Color InverseColor(this Color c)
        {
            return Color.FromArgb((int)(Color.FromArgb(c.R, c.G, c.B).ToArgb() ^ 0xFFFFFFFu));
        }

        public static Color ChangeColorDensity(this Color c, double multiplier, double alpha = 255)
        {
            return Color.FromArgb((byte)alpha, (byte)(c.R * multiplier), (byte)(c.G * multiplier), (byte)(c.B * multiplier));
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
        public static byte[] ToByteArray(this Image image, ImageFormat imageFormat = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, imageFormat ?? image.RawFormat);
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
        public static Image Cut(this Bitmap image, double width, double height)
        {
            var diffOffset = height / width;
            if (!(0 < diffOffset && diffOffset < 1))
                throw new ArgumentException("width and height offset is out of range (height divide by width must be inside of range 0~1).");

            var newImage = new Bitmap(image.Width, (int)(image.Width * diffOffset), PixelFormat.Format32bppArgb);
            var drawer = Graphics.FromImage(newImage);
            var offsetX = 0;
            var offsetY = (int)(image.Width * (1 - diffOffset) / 2);
            drawer.DrawImage(image, 0, 0, new Rectangle(offsetX, offsetY, image.Width, image.Width), GraphicsUnit.Pixel);
            return newImage;
        }
        public static Image Blur(this Image image, int radial)
        {
            if (image.Width == 1 && image.Height == 1)
                return image;
            var result = GaussianBlur.Blur(image as Bitmap, radial);
            return result;
        }
        public static Image Resize(this Image img, int outputWidth, int outputHeight)
        {

            if (img == null || (img.Width == outputWidth && img.Height == outputHeight)) return img;
            Bitmap outputImage;
            Graphics graphics;
            outputImage = new Bitmap(outputWidth, outputHeight, PixelFormat.Format32bppArgb);
            graphics = Graphics.FromImage(outputImage);
            graphics.DrawImage(img, new Rectangle(0, 0, outputWidth, outputHeight), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            return outputImage;
        }
        public static Color ContrastColor(this Color c)
        {
            int d = 0;
            double a = 1 - (0.299 * c.R + 0.587 * c.G + 0.114 * c.B) / 255;
            if (a >= 0.5)
                d = 255;
            return Color.FromArgb(d, d, d);
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
        public static Icon ToIcon(this Bitmap image)
        {
            IntPtr HIcon = image.GetHicon();
            //Icon result = Icon.FromHandle(HIcon);
            using (var result = Icon.FromHandle(HIcon))
            {
                return result;
            }
        }
        public static BitmapImage ToBitmapImage(this Bitmap src, ImageFormat format)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                src.Save(memory, format);
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
        public static BitmapImage ToBitmapImage(this Image src, ImageFormat format)
        {
            using var bitmap = new Bitmap(src);
            var result = ToBitmapImage(bitmap, format);
            return result;
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

        public static Image CalculateBackgroundSource(Image AlbumArtwork, double width, double height, int blurRadial)
        {
            if (AlbumArtwork == null) return null;
            using var background = new Bitmap(AlbumArtwork);
            using var cutBg = background.Cut(width, height);
            var opacBg = cutBg.SetOpacity(0.6d, System.Drawing.Color.Black);
            var blurBg = opacBg.Blur(blurRadial);
            return blurBg;
        }
    }
}
