using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VirtualGrid.Enums;
using VirtualGrid.Interfaces;

namespace VirtualGrid
{
    /// <summary>
    /// A virtual LED grid that represent the physical setup.
    /// </summary>
    public sealed class VirtualLedGrid : IVirtualLedGrid
    {
        class VirtualKey : IVirtualKey
        {
            public (int X, int Y) Index { get; }

            public string FriendlyName { get; }

            public Color Color { get; set; }

            public KeyType Type { get; }

            public int KeyCode { get; }

            public VirtualKey(string friendlyName, int x, int y, int internalKeyCode, KeyType keyType, Color color = default)
            {
                this.Index = (x, y);
                this.FriendlyName = friendlyName;
                this.Type = keyType;
                this.Color = color;
                this.KeyCode = internalKeyCode;
            }

            public override string ToString()
            {
                return this.FriendlyName + $"({Index.X},{Index.Y})";
            }
        }

        private readonly IVirtualKey[,] grid;

        /// <summary>
        /// Indexer for accesing color of the virtual grid using specific index.
        /// </summary>
        /// <param name="column">Specific column.</param>
        /// <param name="row">Specific row.</param>
        /// <returns><seealso cref="Color"/></returns>
        public Color this[int column, int row]
        {
            get
            {
                return grid[column, row].Color;
            }

            set
            {
                grid[column, row].Color = value;
            }
        }

        /// <summary>
        /// Row count for virtual grid.
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// Column count for virtual grid.
        /// </summary>
        public int ColumnCount { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="keyboard"></param>
        /// <param name="mouse"></param>
        /// <param name="mousepad"></param>
        /// <param name="headset"></param>
        /// <param name="chromaExtra"></param>
        public VirtualLedGrid(System.Enum[,] keys)
        {
            var xDimension = keys.GetLength(1);
            var yDimension = keys.GetLength(0);
            this.ColumnCount = xDimension;
            this.RowCount = yDimension;
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
            var grid = new VirtualKey[xDimension, yDimension];
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional
            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    var @enum = keys[y, x];
                    KeyType keyType;
                    if (@enum is KeyboardLed key)
                    {
                        if (key == KeyboardLed.Invalid)
                            keyType = KeyType.Invalid;
                        else
                            keyType = KeyType.Keyboard;
                    }
                    else if (@enum is MouseLed)
                    {
                        keyType = KeyType.Mouse;
                    }
                    else if (@enum is MousepadLed)
                    {
                        keyType = KeyType.Mousepad;
                    }
                    else if (@enum is HeadsetLed)
                    {
                        keyType = KeyType.Headset;
                    }
                    else if (@enum is ExtraDeviceLed)
                    {
                        keyType = KeyType.ExtraDevice;
                    }
                    else
                    {
                        throw new NotSupportedException(@enum.GetType().FullName);
                    }

                    var abstractKey = new VirtualKey(@enum.ToString(), x, y, Convert.ToInt32(@enum), keyType);
                    grid[x, y] = abstractKey;
                }
            }
            this.grid = grid;
        }

        /// <summary>
        /// Set a single color to all keys.
        /// </summary>
        /// <param name="color"></param>
        public void Set(Color color)
        {
            foreach (var key in this.grid)
            {
                key.Color = color;
            }
        }

        /// <summary>
        /// Set per-key color.
        /// </summary>
        /// <param name="colors"></param>
        public void Set(Color[][] colors)
        {
            if (colors == null)
                throw new ArgumentNullException(nameof(colors));

            for (var y = 0; y < colors.GetLength(0); y++)
            {
                var row = colors[y];
                for (var x = 0; x < row.Length; x++)
                {
                    this[x, y] = row[x];
                }
            }
        }

        /// <summary>
        /// Create default virtual grid with predefined layout.
        /// </summary>
        /// <returns></returns>
        public static IVirtualLedGrid CreateDefaultGrid()
        {
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
            var grid = new System.Enum[,]{
                            { MousepadLed.Led0,MousepadLed.Led1,MousepadLed.Led2,MousepadLed.Led3,MousepadLed.Led4,MousepadLed.Led5,MousepadLed.Led6,MousepadLed.Led7,MousepadLed.Led8,MousepadLed.Led9,MousepadLed.Led10,MousepadLed.Led11,MousepadLed.Led12,MousepadLed.Led13,MousepadLed.Led14,KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Invalid,MouseLed.LeftSide1, KeyboardLed.Invalid, KeyboardLed.Invalid, KeyboardLed.Invalid, KeyboardLed.Invalid, KeyboardLed.Invalid ,MouseLed.RightSide1 },
                            { KeyboardLed.Invalid, KeyboardLed.Escape, KeyboardLed.Invalid, KeyboardLed.F1,KeyboardLed.F2,KeyboardLed.F3, KeyboardLed.F4,KeyboardLed.F5, KeyboardLed.F6, KeyboardLed.F7, KeyboardLed.F8, KeyboardLed.F9, KeyboardLed.F10, KeyboardLed.F11, KeyboardLed.F12,KeyboardLed.PrintScreen,KeyboardLed.Scroll,KeyboardLed.Pause, KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Logo,KeyboardLed.Invalid, MouseLed.LeftSide2,KeyboardLed.Invalid, KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Invalid, KeyboardLed.Invalid, MouseLed.RightSide2 },
                            { KeyboardLed.Macro1, KeyboardLed.OemTilde, KeyboardLed.D1, KeyboardLed.D2, KeyboardLed.D3, KeyboardLed.D4, KeyboardLed.D5, KeyboardLed.D6, KeyboardLed.D7, KeyboardLed.D8, KeyboardLed.D9, KeyboardLed.D0,KeyboardLed.OemMinus, KeyboardLed.OemEquals, KeyboardLed.Backspace,KeyboardLed.Insert,KeyboardLed.Home,KeyboardLed.PageUp,KeyboardLed.NumLock,KeyboardLed.NumDivide,KeyboardLed.NumMultiply,KeyboardLed.NumSubtract, MouseLed.LeftSide3,KeyboardLed.Invalid, KeyboardLed.Invalid,MouseLed.ScrollWheel, KeyboardLed.Invalid,  KeyboardLed.Invalid ,MouseLed.RightSide3},
                            { KeyboardLed.Macro2,KeyboardLed.Tab,KeyboardLed.Q,KeyboardLed.W,KeyboardLed.E,KeyboardLed.R,KeyboardLed.T,KeyboardLed.Y,KeyboardLed.U,KeyboardLed.I,KeyboardLed.O,KeyboardLed.P,KeyboardLed.OemLeftBracket,KeyboardLed.OemRightBracket,KeyboardLed.OemBackslash, KeyboardLed.Delete,KeyboardLed.End,KeyboardLed.PageDown,KeyboardLed.Num7,KeyboardLed.Num8,KeyboardLed.Num9,KeyboardLed.NumAdd, MouseLed.LeftSide4,KeyboardLed.Invalid, KeyboardLed.Invalid, KeyboardLed.Invalid, KeyboardLed.Invalid, KeyboardLed.Invalid,MouseLed.RightSide4, },
                            { KeyboardLed.Macro3,KeyboardLed.CapsLock,KeyboardLed.A,KeyboardLed.S,KeyboardLed.D,KeyboardLed.F,KeyboardLed.G,KeyboardLed.H,KeyboardLed.J,KeyboardLed.K,KeyboardLed.L,KeyboardLed.OemSemicolon,KeyboardLed.OemApostrophe, KeyboardLed.Invalid,KeyboardLed.Enter,KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Num4,KeyboardLed.Num5,KeyboardLed.Num6,KeyboardLed.Invalid ,MouseLed.LeftSide5, KeyboardLed.Invalid, KeyboardLed.Invalid,MouseLed.Backlight, KeyboardLed.Invalid, KeyboardLed.Invalid,MouseLed.RightSide5, },
                            { KeyboardLed.Macro4,KeyboardLed.LeftShift,KeyboardLed.Invalid, KeyboardLed.Z,KeyboardLed.X,KeyboardLed.C,KeyboardLed.V,KeyboardLed.B,KeyboardLed.N,KeyboardLed.M,KeyboardLed.OemComma,KeyboardLed.OemPeriod,KeyboardLed.OemSlash,KeyboardLed.Invalid, KeyboardLed.RightShift, KeyboardLed.Invalid,KeyboardLed.Up,KeyboardLed.Invalid,KeyboardLed.Num1,KeyboardLed.Num2,KeyboardLed.Num3,KeyboardLed.NumEnter,MouseLed.LeftSide6, ExtraDeviceLed.ExtraLed0,ExtraDeviceLed.ExtraLed1,ExtraDeviceLed.ExtraLed2,ExtraDeviceLed.ExtraLed3,ExtraDeviceLed.ExtraLed4 ,MouseLed.RightSide6 },
                            { KeyboardLed.Macro5, KeyboardLed.LeftControl,KeyboardLed.LeftWindows,KeyboardLed.LeftAlt,KeyboardLed.Invalid, KeyboardLed.Invalid, KeyboardLed.Invalid,KeyboardLed.Space,KeyboardLed.Invalid,KeyboardLed.Invalid,KeyboardLed.Invalid, KeyboardLed.RightAlt,KeyboardLed.Function,KeyboardLed.RightMenu,KeyboardLed.RightControl,KeyboardLed.Left,KeyboardLed.Down,KeyboardLed.Right,KeyboardLed.Invalid,KeyboardLed.Num0,KeyboardLed.NumDecimal,KeyboardLed.Invalid,MouseLed.LeftSide7, KeyboardLed.Invalid,HeadsetLed.Left,MouseLed.Logo,HeadsetLed.Right, KeyboardLed.Invalid,MouseLed.RightSide7 },
                        };
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional
            return new VirtualLedGrid(grid);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IVirtualKey> GetEnumerator()
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
    }
}
