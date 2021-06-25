using System;
using System.Drawing;
using VirtualGrid.Enums;
using VirtualGrid.Interfaces;

namespace ListenerX.Extensions
{
    public static class VirtualLedGridExtensions
    {
        public class VirtualGridRendererImpl : IDisposable
        {
            private readonly IVirtualLedGrid grid;
            private Bitmap bitmap;
            private Graphics graphics;
            private readonly Font font;
            internal VirtualGridRendererImpl(IVirtualLedGrid grid, int boxWidth, int boxHeight)
            {
                this.grid = grid;
                var totalWidth = (grid.ColumnCount + 1) * boxWidth;
                var totalHeight = (grid.RowCount + 1) * boxHeight;
                var fontSize = 13 * (boxWidth / 100.0f);
                font = new Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular);
                bitmap = GetGridBoilerplate(totalWidth, totalHeight, boxWidth, boxHeight);
                graphics = System.Drawing.Graphics.FromImage(bitmap);
            }

            public Image VisualizeRenderingGrid(int boxWidth = 50, int boxHeight = 50)
            {
                var fontSize = 13 * (boxWidth / 100.0f);

                var totalWidth = (grid.ColumnCount + 1) * boxWidth;
                var totalHeight = (grid.RowCount + 1) * boxHeight;


                var bitmap = GetGridBoilerplate(totalWidth, totalHeight, boxWidth, boxHeight);
                using var g = System.Drawing.Graphics.FromImage(bitmap);
                using var font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular);

                foreach (var key in grid)
                {
                    using var block = new System.Drawing.Bitmap(boxWidth, boxHeight);
                    using var _g = System.Drawing.Graphics.FromImage(block);
                    using System.Drawing.Brush color = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(key.Color.R, key.Color.G, key.Color.B));
                    _g.FillRectangle(color, new System.Drawing.RectangleF(0, 0, boxWidth, boxHeight));
                    g.DrawImage(block, (key.Index.X + 1) * boxWidth, (key.Index.Y + 1) * boxHeight);
                }

                return bitmap;
            }

            public Image VisualizeRenderingGrid2(int boxWidth = 50, int boxHeight = 50)
            {
                foreach (var key in grid)
                {
                    var x = key.Index.X + 1;
                    var y = key.Index.Y + 1;
                    using System.Drawing.Brush color = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(key.Color.R, key.Color.G, key.Color.B));
                    graphics.FillRectangle(color, new System.Drawing.RectangleF(x * boxWidth, y * boxHeight, boxWidth, boxHeight));
                }
                return bitmap;
            }

            private Bitmap GetGridBoilerplate(int totalWidth, int totalHeight, int boxWidth, int boxHeight)
            {
                var boilerplate = new Bitmap(totalWidth, totalHeight);
                using Font font = new Font("Microsoft Sans Serif", 13 * (boxWidth / 100.0f), System.Drawing.FontStyle.Regular);
                using var g = System.Drawing.Graphics.FromImage(boilerplate);
                for (var x = 0; x < totalWidth; x++)
                {
                    using var block = new System.Drawing.Bitmap(boxWidth, boxHeight);
                    using var _g = System.Drawing.Graphics.FromImage(block);
                    _g.FillRectangle(System.Drawing.Brushes.Black, new System.Drawing.RectangleF(0, 0, boxWidth, boxHeight));
                    var text = (x - 1).ToString();
                    if (text == "-1")
                        text = "";
                    var textMeasure = _g.MeasureString(text, font);
                    _g.DrawString(text, font, System.Drawing.Brushes.Green, (int)((boxWidth - textMeasure.Width) / 2), (int)((boxHeight - textMeasure.Height) / 2));
                    g.DrawImage(block, x * boxWidth, 0);
                }

                for (var y = 0; y < totalHeight; y++)
                {
                    using var block = new System.Drawing.Bitmap(boxWidth, boxHeight);
                    using var _g = System.Drawing.Graphics.FromImage(block);
                    _g.FillRectangle(System.Drawing.Brushes.Black, new System.Drawing.RectangleF(0, 0, boxWidth, boxHeight));
                    var text = (y - 1).ToString();
                    if (text == "-1")
                        text = "";
                    var textMeasure = _g.MeasureString(text, font);
                    _g.DrawString(text, font, System.Drawing.Brushes.Green, (int)((boxWidth - textMeasure.Width) / 2), (int)((boxHeight - textMeasure.Height) / 2));
                    g.DrawImage(block, 0, y * boxHeight);
                }
                return boilerplate;
            }

            private bool disposed = false;

            protected virtual void Dispose(bool disposing)
            {
                if (disposing & !disposed)
                {
                    this.bitmap?.Dispose();
                    this.graphics?.Dispose();
                    this.font?.Dispose();
                }
                this.disposed = true;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        public static VirtualGridRendererImpl CreateGridRendererInstance(IVirtualLedGrid grid, int boxWidth, int boxHeight)
        {
            return new VirtualGridRendererImpl(grid, boxWidth, boxHeight);
        }

        public static void Set(this IVirtualLedGrid grid, VirtualGrid.Color[][] colors, double brightness)
        {
            for (var y = 0; y < colors.GetLength(0); y++)
            {
                var row = colors[y];
                for (var x = 0; x < row.Length; x++)
                {
                    grid[x, y] = row[x].ChangeBrightnessLevel(brightness);
                }
            }
        }
    }
}
