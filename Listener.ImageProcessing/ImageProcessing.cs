using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        public static Image Cut(this Bitmap image, int width, int height)
        {
            var diffOffset = (float)height / width;
            //if (!(0 < diffOffset && diffOffset < 1))
            //    throw new ArgumentException("width and height offset is out of range (height divide by width must be inside of range 0~1).");

            var actualHeight = (int)(image.Width * diffOffset);
            var newImage = new Bitmap(image.Width, actualHeight, PixelFormat.Format32bppArgb);
            //var newImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using var g = Graphics.FromImage(newImage);
            var offsetX = 0;
            var offsetY = (int)(image.Width * (1 - diffOffset) / 2);
            var rect = new Rectangle(offsetX, offsetY, image.Width, actualHeight);
            g.DrawImage(image, 0, 0, rect, GraphicsUnit.Pixel);
            return newImage;
        }
        public static Image Blur(this Image image, int radial)
        {
            if (image.Width == 1 && image.Height == 1)
                return image;
            var result = GaussianBlur.Blur(image as Bitmap, radial);
            return result;
        }
        public static Image Resize(this Image image, int width, int height)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (image.Width == width && image.Height == height)
                return image;

            //var outputImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            //using var graphics = Graphics.FromImage(outputImage);
            //graphics.DrawImage(image, new Rectangle(0, 0, outputWidth, outputHeight), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
            //return outputImage;

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            //destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            //destImage.SetResolution(299, 299);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return destImage;
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
            using Bitmap bitmap = new Bitmap(src);
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

        public static Image CalculateBackgroundSource(Image AlbumArtwork, int width, int height, int blurRadial = 10, double opacity = 0.6d)
        {
            if (AlbumArtwork == null) return null;

            using var cutBg = ((Bitmap)AlbumArtwork).Cut(width, height);
            using var opacBg = cutBg.SetOpacity(opacity, System.Drawing.Color.Black);
            var blurBg = opacBg.Blur(blurRadial);
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

        public static Color[][] GetPixels(this Bitmap bmp)
        {
            var result = new Color[bmp.Height][];
            for (var y = 0; y < bmp.Height; y++)
            {
                var row = new Color[bmp.Width];
                for (var x = 0; x < bmp.Width; x++)
                {
                    row[x] = bmp.GetPixel(x, y);
                }
                result[y] = row;
            }
            return result;
        }
    }
}
