﻿using Colore;
using Colore.Effects.ChromaLink;
using Colore.Effects.Headset;
using Colore.Effects.Keyboard;
using Colore.Effects.Mouse;
using Colore.Effects.Mousepad;
using ListenerX.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Extensions
{
    public static class AbstractKeyGridExtensions
    {
        public static System.Drawing.Image VisualizeRenderingGrid(this AbstractKeyGrid grid, int boxWidth = 50, int boxHeight = 50)
        {
            var fontSize = 13 * (boxWidth / 100.0f);

            var totalWidth = (grid.ColumnCount + 1) * boxWidth;
            var totalHeight = (grid.RowCount + 1) * boxHeight;


            var bitmap = new System.Drawing.Bitmap(totalWidth, totalHeight);
            using var g = System.Drawing.Graphics.FromImage(bitmap);
            using var font = new System.Drawing.Font("Microsoft Sans Serif", fontSize, System.Drawing.FontStyle.Regular);

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

            foreach (var key in grid)
            {
                using var block = new System.Drawing.Bitmap(boxWidth, boxHeight);
                using var _g = System.Drawing.Graphics.FromImage(block);
                using System.Drawing.Brush color = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(key.Color.R, key.Color.G, key.Color.B));
                string text;
                switch (key.Type)
                {
                    case KeyType.Invalid:
                        text = "";
                        break;
                    case KeyType.Keyboard:
                    case KeyType.Mouse:
                    case KeyType.Mousepad:
                    case KeyType.Headset:
                    case KeyType.ChromaLink:
                        text = key.FriendlyName;
                        break;
                    default:
                        throw new NotSupportedException(key.Type.ToString());
                }
                _g.FillRectangle(color, new System.Drawing.RectangleF(0, 0, boxWidth, boxHeight));

                var textMeasure = g.MeasureString(text, font);
                _g.DrawString(text, font, System.Drawing.Brushes.White, (int)((boxWidth - textMeasure.Width) / 2), (int)((boxHeight - textMeasure.Height) / 2));
                g.DrawImage(block, (key.Index.X + 1) * boxWidth, (key.Index.Y + 1) * boxHeight);

            }

            //var tempPath = System.IO.Path.GetTempFileName().Replace(".tmp", ".jpg");
            //bitmap.Save(tempPath);
            //System.Diagnostics.Process.Start(tempPath);

            return bitmap;
        }

        public static async Task ApplyAsync(this AbstractKeyGrid grid, IChroma chroma)
        {
            var keyboardGrid = CustomKeyboardEffect.Create();
            var mouseGrid = CustomMouseEffect.Create();
            var mousepadGrid = CustomMousepadEffect.Create();
            var headsetGrid = CustomHeadsetEffect.Create();
            var chromaLinkGrid = CustomChromaLinkEffect.Create();
            foreach (var k in grid)
            {
                switch (k.Type)
                {
                    case KeyType.Invalid:
                        break;
                    case KeyType.Keyboard:
                        keyboardGrid[(Key)k.KeyCode] = k.Color;
                        break;
                    case KeyType.Mouse:
                        mouseGrid[(GridLed)k.KeyCode] = k.Color;
                        break;
                    case KeyType.Mousepad:
                        mousepadGrid[k.KeyCode] = k.Color;
                        break;
                    case KeyType.Headset:
                        headsetGrid[k.KeyCode] = k.Color;
                        break;
                    case KeyType.ChromaLink:
                        chromaLinkGrid[k.KeyCode] = k.Color;
                        break;
                }
            }
            await chroma.Keyboard.SetCustomAsync(keyboardGrid);
            await chroma.Mouse.SetGridAsync(mouseGrid);
            await chroma.Mousepad.SetCustomAsync(mousepadGrid);
            await chroma.Headset.SetCustomAsync(headsetGrid);
            await chroma.ChromaLink.SetCustomAsync(chromaLinkGrid);
        }
    }
}
