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
        public static Color DominantColor2(this Image img)
        {
            var bitmap = (Bitmap)img;
            var colorIncidence = new Dictionary<int, int>();
            for (var x = 0; x < bitmap.Size.Width; x++)
                for (var y = 0; y < bitmap.Size.Height; y++)
                {
                    var pixelColor = bitmap.GetPixel(x, y).ToArgb();
                    if (colorIncidence.Keys.Contains(pixelColor))
                        colorIncidence[pixelColor]++;
                    else
                        colorIncidence.Add(pixelColor, 1);
                }
            return Color.FromArgb(colorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value).First().Key);

        }

        public static Color NearestColor(this Color c)
        {
            var inputRed = Convert.ToDouble(c.R);
            var inputGreen = Convert.ToDouble(c.G);
            var inputBlue = Convert.ToDouble(c.B);
            var colors = new List<Color>();
            foreach (var knownColor in Enum.GetValues(typeof(KnownColor)))
            {
                var color = Color.FromKnownColor((KnownColor)knownColor);
                if (!color.IsSystemColor)
                    colors.Add(color);
            }
            var nearestColor = Color.Empty;
            var distance = 500.0;
            foreach (var color in colors)
            {
                // Compute Euclidean distance between the two colors
                var testRed = Math.Pow(Convert.ToDouble(color.R) - inputRed, 2.0);
                var testGreen = Math.Pow(Convert.ToDouble(color.G) - inputGreen, 2.0);
                var testBlue = Math.Pow(Convert.ToDouble(color.B) - inputBlue, 2.0);
                var tempDistance = Math.Sqrt(testBlue + testGreen + testRed);
                if (tempDistance == 0.0)
                    return color;
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    nearestColor = color;
                }
            }
            return nearestColor;
        }

        public static byte[] ToByteArray(this Image image, ImageFormat imageFormat = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, imageFormat ?? image.RawFormat);
                return memoryStream.ToArray();
            }
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
            if (image.Width == 1 && image.Height == 1 || radial == 0)
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
            //outputImage.SetResolution(300, 300);
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

        public static uint ToUint(this Color c)
        {
            return (uint)(((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B) & 0xffffffffL);
        }
        public static Image CalculateBackgroundSource(Image AlbumArtwork, double width, double height, int blurRadial)
        {
            if (AlbumArtwork == null) return null;
            using var background = new Bitmap(AlbumArtwork);
            using var cutBg = background.Cut(width, height);
            var opacBg = cutBg.SetOpacity(0.6d, System.Drawing.Color.Black);
            var blurBg = opacBg.Blur(10);
            return blurBg;
        }

        public static IList<Color> GetDominantColors(this Image img, int k = 1)
        {
            const int maxResizedDimension = 200;
            Size resizedSize;
            if (img.Width > img.Height)
            {
                resizedSize = new Size(maxResizedDimension, (int)Math.Floor((img.Height / (img.Width * 1.0f)) * maxResizedDimension));
            }
            else
            {
                resizedSize = new Size((int)Math.Floor((img.Width / (img.Width * 1.0f)) * maxResizedDimension), maxResizedDimension);
            }

            using var resized = new Bitmap(img, resizedSize);
            var colors = new List<Color>(resized.Width * resized.Height);
            for (int x = 0; x < resized.Width; x++)
            {
                for (int y = 0; y < resized.Height; y++)
                {
                    colors.Add(resized.GetPixel(x, y));
                }
            }

            var dominantColours = KMeansClusteringCalculator.Calculate(k, colors);
            return dominantColours;
        }
    }
}
