using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpotifyListener.Effects
{
    public static class BitmapHelper
    {
        public static Image CalculateBackgroundSource(Image AlbumArtwork)
        {
            if (AlbumArtwork == null) return null;
            var width = MainWindow.Context.InitWidth;
            var height = MainWindow.Context.InitHeight;
            using var background = new Bitmap(AlbumArtwork);
            using var cutBg = background.Cut(width, height);
            var opacBg = cutBg.SetOpacity(0.6d, System.Drawing.Color.Black);
            var blurBg = opacBg.Blur(Properties.Settings.Default.BlurRadial);
            return blurBg;

        }
    }
}
