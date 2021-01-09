using Colore;
using Colore.Effects.Headset;
using Colore.Effects.Keyboard;
using Colore.Effects.Mouse;
using Colore.Effects.Mousepad;
using Listener.Core.Framework.Players;
using Listener.ImageProcessing;
using System;
using System.Diagnostics;
using System.Linq;
using ListenerX.Classes;
using ListenerX.Foundation.Struct;
using ColoreColor = Colore.Data.Color;
using System.Threading.Tasks;
using System.Threading;
using Utilities.Shared;

namespace ListenerX
{
    namespace ChromaExtension
    {
        public sealed class ChromaWorker : IDisposable
        {
            public ColoreColor PrimaryColor { get; private set; } = Properties.Settings.Default.Volume.ToColoreColor();
            private ColoreColor SecondaryColor { get; set; } = ColoreColor.Black;
            private CustomKeyboardEffect KeyboardGrid = CustomKeyboardEffect.Create();
            private CustomMouseEffect MouseGrid = CustomMouseEffect.Create();
            private CustomMousepadEffect MousepadGrid = CustomMousepadEffect.Create();
            private CustomHeadsetEffect HeadsetGrid = CustomHeadsetEffect.Create();
            private IChroma Chroma;
            public static ChromaWorker Instance { get; } = new ChromaWorker();
            public bool IsError { get; private set; }

            private ChromaWorker()
            {
                try
                {
                    Chroma = ColoreProvider.CreateNativeAsync().Result;
                }
                catch (Exception ex)
                {
                    IsError = true;
                    Debug.WriteLine(ex);
                }
            }
            /// <summary>
            /// Apply effects to devices
            /// </summary>
            public async Task ApplyAsync(CancellationToken cancellationToken = default)
            {
                await Chroma.Keyboard.SetCustomAsync(KeyboardGrid);
                await Chroma.Mouse.SetGridAsync(MouseGrid);
                await Chroma.Headset.SetCustomAsync(HeadsetGrid);
                await Chroma.Mousepad.SetCustomAsync(MousepadGrid);
                await Chroma.ChromaLink.SetAllAsync(ColoreColor.Red);
                KeyboardGrid.Clear();
                MouseGrid.Clear();
                HeadsetGrid.Clear();
                MousepadGrid.Clear();
            }
            public delegate void CustomApplyEffects(ref CustomMouseEffect mouse, ref CustomKeyboardEffect keyboard, ref CustomMousepadEffect mousepad, ref CustomHeadsetEffect headset);

            public void SDKDisable()
            {
                MouseGrid.Set(ColoreColor.Black);
                KeyboardGrid.Set(ColoreColor.Black);
                MousepadGrid.Set(ColoreColor.Black);
                HeadsetGrid.Set(ColoreColor.Black);
                ApplyAsync();
            }
            /// <summary>
            /// Call this method to refresh the color properties
            /// </summary>
            /// <param name="player">player instance</param>
            /// <param name="density">density for adaptive color</param>
            public void LoadColor(StandardColor color)
            {
                var standardColor = color;
                var razerColor = new ChromaDevicesColor();
                razerColor.Standard = standardColor.Standard.ToColoreColor();
                razerColor.Complemented = standardColor.Complemented.ToColoreColor();

                //VolumeColor = razerColor.Complemented;
                //BackgroundColor = razerColor.Standard;
                PrimaryColor = razerColor.Standard;
                SecondaryColor = razerColor.Complemented;
            }

            public ChromaWorker PeakVolumeEffects(float volume, bool splitRender = false)
            {
                if (splitRender)
                {
                    this.MouseGrid.SetPeakVolumeSymmetric(this.PrimaryColor, volume);
                    this.KeyboardGrid.SetPeakVolumeSymmetric(this.PrimaryColor, volume);
                }
                else
                {
                    this.MouseGrid.SetPeakVolume(this.PrimaryColor, this.SecondaryColor, volume);
                    this.KeyboardGrid.SetPeakVolume(this.PrimaryColor, this.SecondaryColor, volume);
                }
                this.HeadsetGrid.SetPeakVolume(this.PrimaryColor);
                this.MousepadGrid.SetPeakVolume(this.PrimaryColor);
                return this;
            }

            public ChromaWorker VisualizeVolumeEffects(double[] spectrumValues)
            {
                var keyboardSpectrum = spectrumValues.Take(22).ToArray();
                var mouseSpectrum = spectrumValues.TakeLast(3).ToArray();
                this.KeyboardGrid.SetVisualizeSpectrum(this.PrimaryColor, this.SecondaryColor, keyboardSpectrum);
                this.MouseGrid.SetVisualizeSpectrum(this.PrimaryColor, this.SecondaryColor, mouseSpectrum);
                return this;
            }

            public ChromaWorker VisualizeVolumeChromaEffects(double[] spectrumValues)
            {
                //this.MouseGrid.Set(ColoreColor.Black);
                //this.KeyboardGrid.Set(ColoreColor.Black);
                var keyboardSpectrum = spectrumValues.Take(22).ToArray();
                var mouseSpectrum = spectrumValues.TakeLast(3).ToArray();
                this.KeyboardGrid.SetChromaVisualizeSpectrum(keyboardSpectrum);
                this.MouseGrid.SetChromaVisualizeSpectrum(mouseSpectrum);
                this.HeadsetGrid.SetPeakVolume(this.PrimaryColor);
                this.MousepadGrid.SetPeakVolume(this.PrimaryColor);
                return this;
            }

            public ChromaWorker PlayingPositionEffects(IPlayerHost player, float volume, bool swapLedRender = false)
            {
                if (double.IsNaN(player.CalculatedPosition) || double.IsInfinity(player.CalculatedPosition))
                    return this;
                var backgroundColor = this.PrimaryColor.ChangeColorDensity(0.2);
                this.KeyboardGrid.Set(backgroundColor);
                this.MouseGrid.Set(backgroundColor);
                this.MouseGrid.SetPlayingPosition(this.SecondaryColor, player.CalculatedPosition, swapLedRender);
                this.MouseGrid.SetVolumeScale(this.SecondaryColor, volume, swapLedRender);
                this.KeyboardGrid.SetPlayingPosition(this.PrimaryColor, this.SecondaryColor, player.CalculatedPosition);

                //this.KeyboardGrid.SetVolumeScale(this.SecondaryColor, player.Volume);
                //this.KeyboardGrid.SetPlayingTime(TimeSpan.FromMilliseconds(player.Position_ms));
                this.MousepadGrid.SetPeakVolume(this.PrimaryColor);
                this.HeadsetGrid.SetPeakVolume(this.PrimaryColor);
                return this;
            }

            private bool disposed = false;
            protected void Dispose(bool disposing)
            {
                if (disposing && !disposed)
                {
                    this.Chroma?.Dispose();
                }
                disposed = true;
            }
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
        static class ChromaEffectsExtension
        {
            static class Constant
            {
                public static readonly Key[] NumpadKeys = new[] { Key.Num0, Key.Num1, Key.Num2, Key.Num3, Key.Num4, Key.Num5, Key.Num6, Key.Num7, Key.Num8, Key.Num9 };
                public static readonly Key[] DPadKeys = new[] { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 };
                public static readonly Key[] FunctionKeys = new[] { Key.Escape, Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12 };
                public static readonly Key[] FunctionKeys_Reverse = FunctionKeys.Reverse().ToArray();
                public static readonly Key[] MacroKeys = new[] { Key.Macro1, Key.Macro2, Key.Macro3, Key.Macro4, Key.Macro5 };
                public static readonly Key[] AllKeys = Enum.GetValues(typeof(Key)).Cast<Key>().Where(x => x != Key.Invalid).ToArray();
                public static readonly Key[][] KeyboardKeys;
                public static readonly Key[][] KeyboardKeys_Vertical;
                //public static readonly GridLed[] AllMouseLED = Enum.GetValues(typeof(GridLed)).Cast<GridLed>().ToArray(); //for future uses
                public static readonly GridLed[] LeftStrip = new[] { GridLed.LeftSide1, GridLed.LeftSide2, GridLed.LeftSide3, GridLed.LeftSide4, GridLed.LeftSide5, GridLed.LeftSide6, GridLed.LeftSide7 };
                public static readonly GridLed[] RightStrip = new[] { GridLed.RightSide1, GridLed.RightSide2, GridLed.RightSide3, GridLed.RightSide4, GridLed.RightSide5, GridLed.RightSide6, GridLed.RightSide7 };
                public static readonly GridLed[] LeftStrip_Reverse = new[] { GridLed.LeftSide7, GridLed.LeftSide6, GridLed.LeftSide5, GridLed.LeftSide4, GridLed.LeftSide3, GridLed.LeftSide2, GridLed.LeftSide1 };
                public static readonly GridLed[] RightStrip_Reverse = new[] { GridLed.RightSide7, GridLed.RightSide6, GridLed.RightSide5, GridLed.RightSide4, GridLed.RightSide3, GridLed.RightSide2, GridLed.RightSide1 };
                static Constant()
                {
                    var keyboardArrays = new Key[][]{
                       new Key[22] { Key.Invalid, Key.Escape, Key.Invalid, Key.F1,Key.F2,Key.F3, Key.F4,Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12,Key.PrintScreen,Key.Scroll,Key.Pause, Key.Invalid,Key.Invalid,Key.Logo,Key.Invalid },
                       new Key[22] { Key.Macro1, Key.OemTilde, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0,Key.OemMinus, Key.OemEquals, Key.Backspace,Key.Insert,Key.Home,Key.PageUp,Key.NumLock,Key.NumDivide,Key.NumMultiply,Key.NumSubtract},
                       new Key[22]{ Key.Macro2,Key.Tab,Key.Q,Key.W,Key.E,Key.R,Key.T,Key.Y,Key.U,Key.I,Key.O,Key.P,Key.OemLeftBracket,Key.OemRightBracket,Key.OemBackslash, Key.Delete,Key.End,Key.PageDown,Key.Num7,Key.Num8,Key.Num9,Key.NumAdd},
                       new Key[22]{ Key.Macro3,Key.CapsLock,Key.A,Key.S,Key.D,Key.F,Key.G,Key.H,Key.J,Key.K,Key.L,Key.OemSemicolon,Key.OemApostrophe, Key.Invalid,Key.Enter,Key.Invalid,Key.Invalid,Key.Invalid,Key.Num4,Key.Num5,Key.Num6,Key.Invalid},
                       new Key[22]{ Key.Macro4,Key.LeftShift,Key.Invalid, Key.Z,Key.X,Key.C,Key.V,Key.B,Key.N,Key.M,Key.OemComma,Key.OemPeriod,Key.OemSlash,Key.Invalid, Key.RightShift, Key.Invalid,Key.Up,Key.Invalid,Key.Num1,Key.Num2,Key.Num3,Key.NumEnter},
                       new Key[22] {Key.Macro5, Key.LeftControl,Key.LeftWindows,Key.LeftAlt,Key.Invalid, Key.Invalid, Key.Invalid,Key.Space,Key.Invalid,Key.Invalid,Key.Invalid, Key.RightAlt,Key.Function,Key.RightMenu,Key.RightControl,Key.Left,Key.Down,Key.Right,Key.Invalid,Key.Num0,Key.NumDecimal,Key.Invalid},
                       //new Key[22] { Key.Invalid, Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid, Key.Invalid, Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid, Key.Logo, Key.Invalid, Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid, Key.Invalid, Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid,Key.Invalid }, // ghost row to compensate old model logo lightning
                    };
                    KeyboardKeys = keyboardArrays;
                    var verticalKeyboardArrays = new Key[22][];
                    for (var x = 0; x < 22; x++)
                    {
                        var row = new Key[6];
                        for (var y = 0; y < keyboardArrays.Length; y++)
                        {
                            row[y] = keyboardArrays[y][x];
                        }
                        verticalKeyboardArrays[x] = row;
                    }
                    KeyboardKeys_Vertical = verticalKeyboardArrays;
                }
            }
            const float BACKGROUND_MULT = 0.05f;
            static readonly ColoreColor[] rotationColors = new ColoreColor[] {
//RED->GREEN
new ColoreColor(255,0  ,0),
new ColoreColor(255,32 ,0),
new ColoreColor(255,64 ,0),
new ColoreColor(255,96 ,0),
new ColoreColor(255,128,0),
new ColoreColor(255,160,0),
new ColoreColor(255,192,0),
new ColoreColor(255,224,0),
new ColoreColor(255,255,0),
new ColoreColor(224,255,0),
new ColoreColor(192,255,0),
new ColoreColor(160,255,0),
new ColoreColor(128,255,0),
new ColoreColor(96 ,255,0),
new ColoreColor(64 ,255,0),
new ColoreColor(32 ,255,0),
//GREEN->BLUE
new ColoreColor(0,255,0),
new ColoreColor(0,255,32 ),
new ColoreColor(0,255,64 ),
new ColoreColor(0,255,96 ),
new ColoreColor(0,255,128),
new ColoreColor(0,255,160),
new ColoreColor(0,255,192),
new ColoreColor(0,255,224),
new ColoreColor(0,255,255),
new ColoreColor(0,224,255),
new ColoreColor(0,192,255),
new ColoreColor(0,160,255),
new ColoreColor(0,128,255),
new ColoreColor(0,96 ,255),
new ColoreColor(0,64 ,255),
new ColoreColor(0,32 ,255),
//BLUE->RED
new ColoreColor(0  ,0  ,255),
new ColoreColor(32 ,0  ,255),
new ColoreColor(64 ,0  ,255),
new ColoreColor(96 ,0  ,255),
new ColoreColor(128,0  ,255),
new ColoreColor(160,0  ,255),
new ColoreColor(192,0  ,255),
new ColoreColor(224,0  ,255),
new ColoreColor(255,0  ,255),
new ColoreColor(255,0  ,224),
new ColoreColor(255,0  ,192),
new ColoreColor(255,0  ,160),
new ColoreColor(255,0  ,128),
new ColoreColor(255,0  ,96 ),
new ColoreColor(255,0  ,64 ),
new ColoreColor(255,0  ,32 )
//new ColoreColor(32 ,0  ,224),
//new ColoreColor(64 ,0  ,192),
//new ColoreColor(96 ,0  ,160),
//new ColoreColor(128,0  ,128),
//new ColoreColor(160,0  ,96 ),
//new ColoreColor(192,0  ,64 ),
//new ColoreColor(224,0  ,32 ),
        };
            static CircularQueue<ColoreColor> colors = new CircularQueue<ColoreColor>(rotationColors);
            public static void SetPeakVolume(this ref CustomMouseEffect MouseGrid, ColoreColor VolumeColor, ColoreColor SecondaryColor, float volume)
            {
                var absolutePosition = Math.Round((volume * Constant.LeftStrip.Length), 0);
                MouseGrid.Set(ColoreColor.Black);
                for (var i = 0; i < absolutePosition; i++)
                {
                    var color = VolumeColor.ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length);
                    if (i > 0)
                        MouseGrid[GridLed.Logo] = SecondaryColor.ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length);
                    if (i == Constant.LeftStrip.Length - 1)
                        MouseGrid[GridLed.ScrollWheel] = SecondaryColor.ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length);
                    MouseGrid[Constant.RightStrip_Reverse[i]] = color;
                    MouseGrid[Constant.LeftStrip_Reverse[i]] = color;
                }
            }
            private static int changeRate = 0;
            public static void SetChromaPeakVolume(this ref CustomMouseEffect MouseGrid, float volume)
            {

                var absolutePosition = Math.Round((volume * Constant.LeftStrip.Length), 0);
                for (var i = 0; i < absolutePosition; i++)
                {
                    var color = colors.Peek().ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length);
                    if (i > 0)
                        MouseGrid[GridLed.Logo] = ColoreColor.White.ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length);
                    if (i == Constant.LeftStrip.Length - 1)
                        MouseGrid[GridLed.ScrollWheel] = color;
                    MouseGrid[Constant.RightStrip_Reverse[i]] = color;
                    MouseGrid[Constant.LeftStrip_Reverse[i]] = color;
                }
                var speed = (int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0);
                changeRate += speed;
                if (changeRate >= 44)
                {
                    colors.ShiftLeft();
                    changeRate = 0;
                }
            }
            public static void SetPeakVolume(this ref CustomKeyboardEffect KeyboardGrid, ColoreColor VolumeColor, ColoreColor SecondaryColor, float volume)
            {
                KeyboardGrid.Set(ColoreColor.Black);
                for (var outerIdx = 0; outerIdx < Constant.KeyboardKeys_Vertical.Length; outerIdx++)
                {
                    var currentRow = Constant.KeyboardKeys_Vertical[outerIdx];
                    var absolutePosition = currentRow.Length - Math.Round((volume * currentRow.Length), 0);
                    for (var i = currentRow.Length - 1; i >= absolutePosition; i--)
                    {
                        var key = currentRow[i];
                        if (key == Key.Invalid)
                            continue;
                        var color = VolumeColor.ChangeColorDensity(1.0 / (i + 1));
                        KeyboardGrid[key] = color;
                        KeyboardGrid[Key.Logo] = SecondaryColor.ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length);
                    }
                }
            }
            public static void SetChromaPeakVolume(this ref CustomKeyboardEffect KeyboardGrid, float volume)
            {
                for (var outerIdx = 0; outerIdx < Constant.KeyboardKeys_Vertical.Length; outerIdx++)
                {
                    var currentRow = Constant.KeyboardKeys_Vertical[outerIdx];
                    var absolutePosition = currentRow.Length - Math.Round((volume * currentRow.Length), 0);
                    for (var i = currentRow.Length - 1; i >= absolutePosition; i--)
                    {
                        var key = currentRow[i];
                        if (key == Key.Invalid)
                            continue;
                        var index = (int)(i / (float)currentRow.Length * 7);
                        var color = colors.ElementAt(index);
                        KeyboardGrid[key] = color;
                        KeyboardGrid[Key.Logo] = ColoreColor.White.ChangeColorDensity((double)(i + 1) / (double)currentRow.Length);
                    }
                }
            }
            public static void SetPeakVolume(this ref CustomMousepadEffect MousepadGrid, ColoreColor VolumeColor)
            {
                MousepadGrid.Set(VolumeColor);
            }
            public static void SetPeakVolume(this ref CustomHeadsetEffect HeadsetGrid, ColoreColor VolumeColor)
            {
                HeadsetGrid.Set(VolumeColor);
            }
            public static void SetVolumeScale(this ref CustomMouseEffect MouseGrid, ColoreColor VolumeColor, float volume, bool switchStripSide = false)
            {
                var mouseStrip = !switchStripSide ? Constant.RightStrip_Reverse : Constant.LeftStrip_Reverse;
                for (var i = 0; i < (volume * mouseStrip.Length); i++)
                {
                    MouseGrid[mouseStrip[i]] = VolumeColor;
                }
            }
            public static void SetVolumeScale(this ref CustomKeyboardEffect KeyboardGrid, ColoreColor VolumeColor, int volume)
            {
                for (var i = 0; i < (volume * Constant.DPadKeys.Length) / 100; i++)
                {
                    KeyboardGrid[Constant.DPadKeys[i]] = VolumeColor;
                }
            }
            public static void SetPeakVolumeSymmetric(this ref CustomMouseEffect MouseGrid, ColoreColor VolumeColor, float volume)
            {
                var fullLength = Constant.RightStrip.Length;
                var startPosition = (fullLength / 2);
                var absolutePosition = Math.Round((volume * fullLength), 0) / 2;
                for (var i = startPosition; i < startPosition + absolutePosition; i++)
                {
                    var color = VolumeColor.ChangeColorDensity((double)(i + 1) / (double)(fullLength));
                    if (i == fullLength - 1)
                    {
                        MouseGrid[GridLed.ScrollWheel] = color;
                        MouseGrid[GridLed.Logo] = color;
                    }
                    MouseGrid[Constant.RightStrip_Reverse[i]] = color; //VolumeColor;
                    MouseGrid[Constant.LeftStrip_Reverse[i]] = color; //VolumeColor;
                    MouseGrid[Constant.RightStrip_Reverse[fullLength - i - 1]] = color; //VolumeColor;
                    MouseGrid[Constant.LeftStrip_Reverse[fullLength - i - 1]] = color; //VolumeColor;
                }
                /*
                for (var i = startPosition; i > startPosition - absolutePosition; i--)
                {
                    MouseGrid[Constant.RightStrip_Reverse[i]] = VolumeColor;
                    MouseGrid[Constant.LeftStrip_Reverse[i]] = VolumeColor;
                }*/
            }
            public static void SetPeakVolumeSymmetric(this ref CustomKeyboardEffect KeyboardGrid, ColoreColor VolumeColor, float volume)
            {
                foreach (var row in Constant.KeyboardKeys)
                {
                    var fullLength = row.Length;
                    var startPosition = (fullLength / 2);
                    var absolutePosition = Math.Round((volume * fullLength), 0) / 2;
                    for (var i = startPosition; i < startPosition + absolutePosition; i++)
                    {
                        var right = row[i];
                        var left = row[fullLength - i - 1];
                        //var color = VolumeColor.ChangeColorDensity((double)(i + 1) / (double)(startPosition + absolutePosition));
                        var color = VolumeColor.ChangeColorDensity((fullLength - i - 1) * (1f / startPosition));
                        if (left != Key.Invalid)
                            KeyboardGrid[left] = color;//VolumeColor;
                        if (right != Key.Invalid)
                            KeyboardGrid[right] = color;// VolumeColor;
                    }
                }
            }

            public static void SetChromaPeakVolumeSymmetric(this ref CustomMouseEffect MouseGrid, float volume)
            {
                var fullLength = Constant.RightStrip.Length;
                var startPosition = (fullLength / 2);
                var absolutePosition = Math.Round((volume * fullLength), 0) / 2;
                for (var i = startPosition; i < startPosition + absolutePosition; i++)
                {
                    var color = colors.ElementAt(i);
                    if (i == fullLength - 1)
                    {
                        MouseGrid[GridLed.ScrollWheel] = color;
                        MouseGrid[GridLed.Logo] = color;
                    }
                    MouseGrid[Constant.RightStrip_Reverse[i]] = color; //VolumeColor;
                    MouseGrid[Constant.LeftStrip_Reverse[i]] = color; //VolumeColor;
                    MouseGrid[Constant.RightStrip_Reverse[fullLength - i - 1]] = color; //VolumeColor;
                    MouseGrid[Constant.LeftStrip_Reverse[fullLength - i - 1]] = color; //VolumeColor;
                }
                var speed = (int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0);
                changeRate += speed;
                if (changeRate >= 44)
                {
                    colors.ShiftLeft();
                    changeRate = 0;
                }
            }

            public static void SetChromaPeakVolumeSymmetric(this ref CustomKeyboardEffect KeyboardGrid, float volume)
            {
                foreach (var row in Constant.KeyboardKeys)
                {
                    var fullLength = row.Length;
                    var startPosition = (fullLength / 2);
                    var absolutePosition = Math.Round((volume * fullLength), 0) / 2;
                    for (var i = startPosition; i < startPosition + absolutePosition; i++)
                    {
                        var color = colors.ElementAt(i);
                        var right = row[i];
                        var left = row[fullLength - i - 1];
                        //var color = VolumeColor.ChangeColorDensity((double)(i + 1) / (double)(startPosition + absolutePosition));
                        if (left != Key.Invalid)
                            KeyboardGrid[left] = color;//VolumeColor;
                        if (right != Key.Invalid)
                            KeyboardGrid[right] = color;// VolumeColor;
                    }
                }
            }
            public static void SetPlayingTime(this ref CustomKeyboardEffect KeyboardGrid, TimeSpan time)
            {
                KeyboardGrid[Constant.NumpadKeys[time.Minutes]] = ColoreColor.Red;
                //                                                                                    ie.        47           -             7          = 40/10 = 4
                //use a 'lossy' property of integer to round all floating point, best practice should be (currentTime.Seconds - (currentTime.Seconds % 10))/10
                KeyboardGrid[Constant.NumpadKeys[time.Seconds / 10]] = ColoreColor.Green;
                KeyboardGrid[Constant.NumpadKeys[time.Seconds % 10]] = ColoreColor.Blue;
            }
            public static void SetPlayingPosition(this ref CustomMouseEffect mouseGrid, ColoreColor primaryColor, double position, bool switchStripSide = false)
            {
                var mouseStrip = !switchStripSide ? Constant.LeftStrip : Constant.RightStrip;
                var currentPlayPosition = (int)Math.Round(position * ((double)(mouseStrip.Length - 1) / 10), 0);
                //var background = secondaryColor.ChangeColorDensity(0.5);
                //for (var i = 0; i < mouseStrip.Length; i++)
                //{
                //    mouseGrid[mouseStrip[i]] = background;
                //}
                mouseGrid[mouseStrip[currentPlayPosition]] = primaryColor;
            }
            public static void SetPlayingPosition(this ref CustomKeyboardEffect keyboardGrid, ColoreColor primaryColor, ColoreColor secondaryColor, double position)
            {
                //secondaryColor = ColoreColor.Red;
                foreach (var row in Constant.KeyboardKeys)
                {
                    var pos = (int)Math.Round(position * ((double)(row.Length - 1) / 10), 0);
                    var key = row[pos];
                    if (key != Key.Invalid)
                    {
                        keyboardGrid[key] = primaryColor;
                    }
                    if (0 < pos - 1 && pos + 1 < row.Length)
                    {
                        var leftKey = row[pos - 1];
                        var rightKey = row[pos + 1];

                        if (leftKey != Key.Invalid) //left gradient
                        {
                            keyboardGrid[leftKey] = secondaryColor;
                        }
                        if (rightKey != Key.Invalid)
                        {
                            keyboardGrid[rightKey] = secondaryColor;
                        }
                    }

                }
            }

            public static void SetVisualizeSpectrum(this ref CustomKeyboardEffect keyboardGrid, ColoreColor primaryColor, ColoreColor secondaryColor, double[] spectrumValues)
            {
                keyboardGrid.Set(secondaryColor.ChangeColorDensity(BACKGROUND_MULT));
                //for (var i = 0; i < 22; i++)
                //{
                //    var absoluteSpectrum = (int)(6.0d * spectrumValues[i]);
                //    for (var h = 6; h >= absoluteSpectrum; h--)
                //    {
                //        var row = Constant.KeyboardKeys[h];
                //        var key = row[i];
                //    }
                //}
                for (var i = 0; i < Constant.KeyboardKeys_Vertical.Length; i++)
                {
                    var col = Constant.KeyboardKeys_Vertical[i];
                    var c = spectrumValues[i];
                    var absSpectrum = 6 - (int)Math.Round((col.Length * (c / 100.0d)), 0);
                    for (var row = 5; row >= absSpectrum; row--)
                    {
                        var key = col[row];
                        if (key == Key.Invalid)
                            continue;
                        keyboardGrid[col[row]] = primaryColor;
                    }
                }
            }

            public static void SetVisualizeSpectrum(this ref CustomMouseEffect mouseGrid, ColoreColor primaryColor, ColoreColor secondaryColor, double[] spectrumValues)
            {
                mouseGrid.Set(secondaryColor.ChangeColorDensity(0.05));

                var leftAbs = (int)Math.Round(Constant.LeftStrip.Length * (spectrumValues[0] / 100.0d), 2);
                var middleAbs = (int)Math.Round(Constant.LeftStrip.Length * (spectrumValues[2] / 100.0d), 2);
                var rightAbs = (int)Math.Round(Constant.RightStrip.Length * (spectrumValues[2] / 100.0d), 2);
                if (leftAbs >= 1)
                {
                    var leftKey = Constant.LeftStrip_Reverse[leftAbs - 1];
                    mouseGrid[led: leftKey] = primaryColor;
                }
                if (rightAbs >= 1)
                {
                    var rightKey = Constant.RightStrip_Reverse[rightAbs - 1];
                    mouseGrid[led: rightKey] = primaryColor;
                }
                if (middleAbs >= 6)
                {
                    mouseGrid[GridLed.ScrollWheel] = primaryColor;
                }
                if (middleAbs > 0)
                {

                    mouseGrid[GridLed.Logo] = primaryColor;
                }
            }

            public static void SetChromaVisualizeSpectrum(this ref CustomKeyboardEffect keyboardGrid, double[] spectrumValues)
            {
                for (var i = 0; i < Constant.KeyboardKeys_Vertical.Length; i++)
                {
                    var color = colors.ElementAt(i);
                    var background = color.ChangeColorDensity(0.05);
                    var col = Constant.KeyboardKeys_Vertical[i];

                    foreach (var key in col)
                    {
                        if (key != Key.Invalid)
                            keyboardGrid[key] = background;
                    }
                    var c = spectrumValues[i];
                    var absSpectrum = 6 - (int)Math.Round((col.Length * (c / 100.0d)), 0);
                    for (var row = 5; row >= absSpectrum; row--)
                    {
                        var key = col[row];
                        if (key == Key.Invalid)
                            continue;
                        keyboardGrid[col[row]] = color;
                    }
                }
            }


            public static void SetChromaVisualizeSpectrum(this ref CustomMouseEffect mouseGrid, double[] spectrumValues)
            {
                var leftAbs = (int)Math.Round(Constant.LeftStrip.Length * (spectrumValues[0] / 100.0d), 2);
                var middleAbs = (int)Math.Round(Constant.LeftStrip.Length * (spectrumValues[2] / 100.0d), 2);
                var rightAbs = (int)Math.Round(Constant.RightStrip.Length * (spectrumValues[2] / 100.0d), 2);
                var c1 = colors.ElementAt(22);
                var bgC1 = c1.ChangeColorDensity(BACKGROUND_MULT);
                foreach (var key in Constant.LeftStrip_Reverse)
                {
                    mouseGrid[key] = bgC1;
                }
                if (leftAbs >= 1)
                {
                    var leftKey = Constant.LeftStrip_Reverse[leftAbs - 1];
                    mouseGrid[led: leftKey] = c1;
                }
                var c3 = colors.ElementAt(24);
                var bgC3 = c3.ChangeColorDensity(BACKGROUND_MULT);
                foreach (var key in Constant.RightStrip_Reverse)
                {
                    mouseGrid[key] = bgC3;
                }
                if (rightAbs >= 1)
                {
                    var rightKey = Constant.RightStrip_Reverse[rightAbs - 1];
                    mouseGrid[led: rightKey] = c3;
                }
                var c2 = colors.ElementAt(23);
                var bgC2 = c2.ChangeColorDensity(BACKGROUND_MULT);
                mouseGrid[GridLed.Logo] = bgC2;
                mouseGrid[GridLed.ScrollWheel] = bgC2;
                if (middleAbs >= 6)
                {
                    mouseGrid[GridLed.ScrollWheel] = c2;
                }
                if (middleAbs > 0)
                {

                    mouseGrid[GridLed.Logo] = c2;
                }

                var speed = (int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0);
                changeRate += speed;
                if (changeRate >= 44)
                {
                    colors.ShiftLeft();
                    changeRate = 0;
                }
            }

        }
    }

}