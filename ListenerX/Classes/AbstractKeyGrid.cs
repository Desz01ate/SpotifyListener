using Colore.Data;
using Colore.Effects.Keyboard;
using Colore.Effects.Mouse;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListenerX;
using Colore.Effects.ChromaLink;
using Colore.Effects.Mousepad;
using Colore.Effects.Headset;

namespace ListenerX.Classes
{
    public sealed class AbstractKeyGrid : IEnumerable<IAbstractKey>
    {
        class AbstractKey : IAbstractKey
        {
            public (int X, int Y) Index { get; }

            public string FriendlyName { get; }

            public Color Color { get; set; }

            public KeyType Type { get; }

            public int internalKeyCode { get; }

            public AbstractKey(string friendlyName, int x, int y, int internalKeyCode, KeyType keyType, Color color = default)
            {
                this.Index = (x, y);
                this.FriendlyName = friendlyName;
                this.Type = keyType;
                this.Color = color;
                this.internalKeyCode = internalKeyCode;
            }
            public override string ToString()
            {
                return this.FriendlyName + $"({Index.X},{Index.Y})";
            }
        }

        private readonly IAbstractKey[,] grid;
        public IAbstractKey this[int x, int y]
        {
            get
            {
                return grid[x, y];
            }
        }

        public readonly int RowCount, ColumnCount;
        private AbstractKeyGrid(object[,] physicalGrid)
        {
            var xDim = physicalGrid.GetLength(1);
            var yDim = physicalGrid.GetLength(0);
            ColumnCount = xDim;
            RowCount = yDim;
            var grid = new AbstractKey[xDim, yDim];
            for (var x = 0; x < xDim; x++)
            {
                for (var y = 0; y < yDim; y++)
                {
                    var e = physicalGrid[y, x];
                    KeyType keyType;
                    if (e is Key key)
                    {
                        if (key == Key.Invalid)
                            keyType = KeyType.Invalid;
                        else
                            keyType = KeyType.Keyboard;
                    }
                    else if (e is GridLed)
                    {
                        keyType = KeyType.Mouse;
                    }
                    else if (e is MousepadLed)
                    {
                        keyType = KeyType.Mousepad;
                    }
                    else if (e is HeadsetLed)
                    {
                        keyType = KeyType.Headset;
                    }
                    else if (e is ChromaLinkLed)
                    {
                        keyType = KeyType.ChromaLink;
                    }
                    else
                    {
                        throw new NotSupportedException(e.GetType().FullName);
                    }
                    var abstractKey = new AbstractKey(e.ToString(), x, y, (int)e, keyType);
                    grid[x, y] = abstractKey;
                }
                this.grid = grid;
            }
        }

        public IEnumerable<(Key Key, (int X, int Y) Index, Color Color)> EnumerateKeyboardKeys(bool includeInvalidKeys = false)
        {
            foreach (var key in this.grid)
            {
                if (key.Type == KeyType.Keyboard || (includeInvalidKeys && key.Type == KeyType.Invalid))
                    yield return ((Key)key.internalKeyCode, key.Index, key.Color);
            }
        }

        public IEnumerable<(GridLed Key, (int X, int Y) Index, Color Color)> EnumerateMouseKeys(bool includeInvalidKeys = false)
        {
            foreach (var key in this.grid)
            {
                if (key.Type == KeyType.Mouse || (includeInvalidKeys && key.Type == KeyType.Invalid))
                    yield return ((GridLed)key.internalKeyCode, key.Index, key.Color);
            }
        }

        public IEnumerable<(GridLed Key, (int X, int Y) Index, Color Color)> EnumerateChromaLinkKeys()
        {
            foreach (var key in this.grid)
            {
                if (key.Type == KeyType.ChromaLink)
                    yield return ((GridLed)key.internalKeyCode, key.Index, key.Color);
            }
        }

        public IEnumerator<IAbstractKey> GetEnumerator()
        {
            foreach (var key in this.grid)
            {
                yield return key;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Set(Color color)
        {
            foreach (var key in this.grid)
            {
                key.Color = color;
            }
        }

        public void Set(Color[][] colors, double brightness)
        {
            for (var y = 0; y < colors.GetLength(0); y++)
            {
                var row = colors[y];
                for (var x = 0; x < row.Length; x++)
                {
                    var key = this[x, y];
                    key.Color = row[x].ChangeBrightnessLevel(brightness);
                }
            }
        }

        public void VisualizeGrid()
        {
            var boxWidth = 100;
            var boxHeight = 100;

            var totalWidth = (this.ColumnCount + 1) * boxWidth;
            var totalHeight = (this.RowCount + 1) * boxHeight;


            using var bitmap = new System.Drawing.Bitmap(totalWidth, totalHeight);
            using var g = System.Drawing.Graphics.FromImage(bitmap);
            using var font = new System.Drawing.Font("Microsoft Sans Serif", 13, System.Drawing.FontStyle.Regular);

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


            foreach (var key in this)
            {
                //continue;
                using var block = new System.Drawing.Bitmap(boxWidth, boxHeight);
                using var _g = System.Drawing.Graphics.FromImage(block);
                System.Drawing.Brush color;
                string text;
                switch (key.Type)
                {
                    case KeyType.Invalid:
                        color = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 0, 0));
                        text = "";
                        break;
                    case KeyType.Keyboard:
                        color = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(152, 251, 152));
                        text = key.FriendlyName;
                        break;
                    case KeyType.Mouse:
                        color = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(147, 112, 219));
                        text = key.FriendlyName;
                        break;
                    default:
                        throw new NotSupportedException(key.Type.ToString());
                }
                using (color)
                {
                    _g.FillRectangle(color, new System.Drawing.RectangleF(0, 0, boxWidth, boxHeight));

                    var textMeasure = g.MeasureString(text, font);
                    _g.DrawString(text, font, System.Drawing.Brushes.Black, (int)((boxWidth - textMeasure.Width) / 2), (int)((boxHeight - textMeasure.Height) / 2));
                    g.DrawImage(block, (key.Index.X + 1) * boxWidth, (key.Index.Y + 1) * boxHeight);
                }
            }

            var tempPath = System.IO.Path.GetTempFileName().Replace(".tmp", ".jpg");
            bitmap.Save(tempPath);
            System.Diagnostics.Process.Start(tempPath);
        }

        public System.Drawing.Image VisualizeRenderingGrid(int boxWidth = 50, int boxHeight = 50)
        {
            var fontSize = 13 * (boxWidth / 100.0f);

            var totalWidth = (this.ColumnCount + 1) * boxWidth;
            var totalHeight = (this.RowCount + 1) * boxHeight;


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

            foreach (var key in this)
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

        static AbstractKeyGrid defaultGrid, keyboardGrid, mouseGrid;
        public static AbstractKeyGrid ActiveGrid { get; private set; } = GetDefaultGrid();
        public static AbstractKeyGrid GetDefaultGrid()
        {
            if (defaultGrid == null)
            {
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
                var grid = new object[,]{
                            { Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,GridLed.LeftSide1, Key.Invalid, MousepadLed.Led0, MousepadLed.Led1, MousepadLed.Led2, MousepadLed.Led3 ,GridLed.RightSide1 }, //ghost row
                            { Key.Invalid, Key.Escape, Key.Invalid, Key.F1,Key.F2,Key.F3, Key.F4,Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12,Key.PrintScreen,Key.Scroll,Key.Pause, Key.Invalid,Key.Invalid,Key.Logo,Key.Invalid, GridLed.LeftSide2,Key.Invalid, MousepadLed.Led13,MousepadLed.Led14,Key.Invalid, MousepadLed.Led4, GridLed.RightSide2 },
                            { Key.Macro1, Key.OemTilde, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0,Key.OemMinus, Key.OemEquals, Key.Backspace,Key.Insert,Key.Home,Key.PageUp,Key.NumLock,Key.NumDivide,Key.NumMultiply,Key.NumSubtract, GridLed.LeftSide3,Key.Invalid, MousepadLed.Led12,GridLed.ScrollWheel, Key.Invalid,  MousepadLed.Led5 ,GridLed.RightSide3},
                            { Key.Macro2,Key.Tab,Key.Q,Key.W,Key.E,Key.R,Key.T,Key.Y,Key.U,Key.I,Key.O,Key.P,Key.OemLeftBracket,Key.OemRightBracket,Key.OemBackslash, Key.Delete,Key.End,Key.PageDown,Key.Num7,Key.Num8,Key.Num9,Key.NumAdd, GridLed.LeftSide4,Key.Invalid, MousepadLed.Led11,GridLed.Backlight, Key.Invalid, MousepadLed.Led6,GridLed.RightSide4, },
                            { Key.Macro3,Key.CapsLock,Key.A,Key.S,Key.D,Key.F,Key.G,Key.H,Key.J,Key.K,Key.L,Key.OemSemicolon,Key.OemApostrophe, Key.Invalid,Key.Enter,Key.Invalid,Key.Invalid,Key.Invalid,Key.Num4,Key.Num5,Key.Num6,Key.Invalid ,GridLed.LeftSide5, Key.Invalid, MousepadLed.Led10,MousepadLed.Led9, MousepadLed.Led8, MousepadLed.Led7,GridLed.RightSide5, },
                            { Key.Macro4,Key.LeftShift,Key.Invalid, Key.Z,Key.X,Key.C,Key.V,Key.B,Key.N,Key.M,Key.OemComma,Key.OemPeriod,Key.OemSlash,Key.Invalid, Key.RightShift, Key.Invalid,Key.Up,Key.Invalid,Key.Num1,Key.Num2,Key.Num3,Key.NumEnter,GridLed.LeftSide6, ChromaLinkLed.LinkLed0,ChromaLinkLed.LinkLed1,ChromaLinkLed.LinkLed2,ChromaLinkLed.LinkLed3,ChromaLinkLed.LinkLed4 ,GridLed.RightSide6 },
                            { Key.Macro5, Key.LeftControl,Key.LeftWindows,Key.LeftAlt,Key.Invalid, Key.Invalid, Key.Invalid,Key.Space,Key.Invalid,Key.Invalid,Key.Invalid, Key.RightAlt,Key.Function,Key.RightMenu,Key.RightControl,Key.Left,Key.Down,Key.Right,Key.Invalid,Key.Num0,Key.NumDecimal,Key.Invalid,GridLed.LeftSide7, Key.Invalid,Key.Invalid,GridLed.Logo,Key.Invalid,Key.Invalid,GridLed.RightSide7 },
                        };
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional
                defaultGrid = new AbstractKeyGrid(grid);
            }
            ActiveGrid = defaultGrid;
            return defaultGrid;
        }

        public static AbstractKeyGrid GetKeyboardGrid()
        {
            if (keyboardGrid == null)
            {
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
                var grid = new object[,]{
                            { Key.Invalid, Key.Escape, Key.Invalid, Key.F1,Key.F2,Key.F3, Key.F4,Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12,Key.PrintScreen,Key.Scroll,Key.Pause, Key.Invalid,Key.Invalid,Key.Logo,Key.Invalid, },
                            { Key.Macro1, Key.OemTilde, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0,Key.OemMinus, Key.OemEquals, Key.Backspace,Key.Insert,Key.Home,Key.PageUp,Key.NumLock,Key.NumDivide,Key.NumMultiply,Key.NumSubtract,},
                            { Key.Macro2,Key.Tab,Key.Q,Key.W,Key.E,Key.R,Key.T,Key.Y,Key.U,Key.I,Key.O,Key.P,Key.OemLeftBracket,Key.OemRightBracket,Key.OemBackslash, Key.Delete,Key.End,Key.PageDown,Key.Num7,Key.Num8,Key.Num9,Key.NumAdd,},
                            { Key.Macro3,Key.CapsLock,Key.A,Key.S,Key.D,Key.F,Key.G,Key.H,Key.J,Key.K,Key.L,Key.OemSemicolon,Key.OemApostrophe, Key.Invalid,Key.Enter,Key.Invalid,Key.Invalid,Key.Invalid,Key.Num4,Key.Num5,Key.Num6,Key.Invalid },
                            { Key.Macro4,Key.LeftShift,Key.Invalid, Key.Z,Key.X,Key.C,Key.V,Key.B,Key.N,Key.M,Key.OemComma,Key.OemPeriod,Key.OemSlash,Key.Invalid, Key.RightShift, Key.Invalid,Key.Up,Key.Invalid,Key.Num1,Key.Num2,Key.Num3,Key.NumEnter},
                            {Key.Macro5, Key.LeftControl,Key.LeftWindows,Key.LeftAlt,Key.Invalid, Key.Invalid, Key.Invalid,Key.Space,Key.Invalid,Key.Invalid,Key.Invalid, Key.RightAlt,Key.Function,Key.RightMenu,Key.RightControl,Key.Left,Key.Down,Key.Right,Key.Invalid,Key.Num0,Key.NumDecimal,Key.Invalid }
                        };
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional
                keyboardGrid = new AbstractKeyGrid(grid);
            }
            ActiveGrid = keyboardGrid;
            return keyboardGrid;
        }

        public static AbstractKeyGrid GetMouseGrid()
        {
            if (mouseGrid == null)
            {
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
                var grid = new object[,]{
                            { GridLed.LeftSide1, Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid, GridLed.RightSide1  },
                            { GridLed.LeftSide2, Key.Invalid,Key.Invalid,GridLed.ScrollWheel,Key.Invalid,Key.Invalid,GridLed.RightSide2 },
                            { GridLed.LeftSide3, Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,GridLed.RightSide3},
                            { GridLed.LeftSide4, Key.Invalid,Key.Invalid,GridLed.Backlight,Key.Invalid,Key.Invalid,GridLed.RightSide4},
                            { GridLed.LeftSide5, Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,GridLed.RightSide5},
                            { GridLed.LeftSide6, Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,GridLed.RightSide6 },
                            { GridLed.LeftSide7, Key.Invalid,Key.Invalid,GridLed.Logo,Key.Invalid,Key.Invalid,GridLed.RightSide7 },
                            { Key.Invalid , GridLed.Bottom1, GridLed.Bottom2,GridLed.Bottom3,GridLed.Bottom4,GridLed.Bottom5,Key.Invalid }
            };
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional
                mouseGrid = new AbstractKeyGrid(grid);
            }
            ActiveGrid = mouseGrid;
            return mouseGrid;
        }
    }
}
