using Colore;
using Colore.Effects.Headset;
using Colore.Effects.Keyboard;
using Colore.Effects.Mouse;
using Colore.Effects.Mousepad;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColoreColor = Colore.Data.Color;

namespace SpotifyListener
{
    namespace ChromaExtension
    {
        class ChromaWrapper
        {
            public ColoreColor BackgroundColor_Playing { get; private set; } = Properties.Settings.Default.Background_Playing.ToColoreColor();
            public ColoreColor BackgroundColor_Pause { get; private set; } = Properties.Settings.Default.Background_Pause.ToColoreColor();
            public ColoreColor PositionColor_Foreground { get; private set; } = Properties.Settings.Default.Position_Foreground.ToColoreColor();
            public ColoreColor PositionColor_Background { get; private set; } = Properties.Settings.Default.Position_Background.ToColoreColor();
            public ColoreColor VolumeColor { get; private set; } = Properties.Settings.Default.Volume.ToColoreColor();
            public ColoreColor BackgroundColor { get; set; } = ColoreColor.Black;
            public KeyboardCustom KeyboardGrid = KeyboardCustom.Create();
            public MouseCustom MouseGrid = MouseCustom.Create();
            public MousepadCustom MousepadGrid = MousepadCustom.Create();
            public HeadsetCustom HeadsetGrid = HeadsetCustom.Create();
            private IChroma Chroma;
            public static ChromaWrapper GetInstance { get; } = new ChromaWrapper();
            public bool IsError { get; private set; }

            public void SetIndividualKeys()
            {
                KeyboardGrid[Key.Up] = ColoreColor.Pink;
                KeyboardGrid[Key.Down] = ColoreColor.Pink;
                KeyboardGrid[Key.Left] = ColoreColor.Pink;
                KeyboardGrid[Key.Right] = ColoreColor.Pink;
                KeyboardGrid[Key.OemEquals] = ColoreColor.Purple;
                KeyboardGrid[Key.OemMinus] = ColoreColor.Purple;
                KeyboardGrid[Key.R] = ColoreColor.Blue;
                KeyboardGrid[Key.O] = ColoreColor.Blue;
                KeyboardGrid[Key.H] = ColoreColor.Blue;
                KeyboardGrid[Key.S] = ColoreColor.Blue;
            }
            private ChromaWrapper()
            {
                try
                {
                    Chroma = ColoreProvider.CreateNativeAsync().Result;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    IsError = true;
                }
            }
            /// <summary>
            /// Apply effects to devices
            /// </summary>
            public async void Apply()
            {
                await Chroma.Keyboard.SetCustomAsync(KeyboardGrid);
                await Chroma.Mouse.SetGridAsync(MouseGrid);
                await Chroma.Headset.SetCustomAsync(HeadsetGrid);
                await Chroma.Mousepad.SetCustomAsync(MousepadGrid);
                KeyboardGrid.Clear();
                MouseGrid.Clear();
                HeadsetGrid.Clear();
                MousepadGrid.Clear();
            }
            public void SetDevicesBackground()
            {
                MouseGrid.Set(BackgroundColor);
                KeyboardGrid.Set(BackgroundColor);
                MousepadGrid.Set(BackgroundColor);
                HeadsetGrid.Set(BackgroundColor);
            }
            public void SDKDisable()
            {
                MouseGrid.Set(ColoreColor.Black);
                KeyboardGrid.Set(ColoreColor.Black);
                MousepadGrid.Set(ColoreColor.Black);
                HeadsetGrid.Set(ColoreColor.Black);
                Apply();
            }
            /// <summary>
            /// Call this method to refresh the color properties
            /// </summary>
            /// <param name="player">player instance</param>
            /// <param name="density">density for adaptive color</param>
            public virtual void LoadColor(Music player, double density)
            {
                if (Properties.Settings.Default.AlbumCoverRenderEnable)
                {
                    PositionColor_Background = player.Album_RazerColor.Standard;
                    PositionColor_Foreground = player.Album_RazerColor.Complemented;
                    VolumeColor = player.Album_RazerColor.Complemented;
                    BackgroundColor = player.Album_RazerColor.Standard;
                }
                else
                {
                    BackgroundColor_Playing = Properties.Settings.Default.Background_Playing.ToColoreColor();
                    BackgroundColor_Pause = Properties.Settings.Default.Background_Pause.ToColoreColor();
                    PositionColor_Foreground = Properties.Settings.Default.Position_Foreground.ToColoreColor();
                    PositionColor_Background = Properties.Settings.Default.Position_Background.ToColoreColor();
                    VolumeColor = Properties.Settings.Default.Volume.ToColoreColor();
                    BackgroundColor = !player.IsPlaying ? BackgroundColor_Pause : BackgroundColor_Playing;

                }
                if (Properties.Settings.Default.PeakChroma) BackgroundColor = ColoreColor.Black;
                density = density < 0.1 ? 0.1 : density;
                BackgroundColor = BackgroundColor.ChangeColorDensity(density);
            }
        }
        static class ChromaEffectsExtension
        {
            static class Constant
            {
                public static readonly Key[] NumpadKeys = new[] { Key.Num0, Key.Num1, Key.Num2, Key.Num3, Key.Num4, Key.Num5, Key.Num6, Key.Num7, Key.Num8, Key.Num9 };
                public static readonly Key[] DPadKeys = new[] { Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9, Key.D0 };
                public static readonly Key[] FunctionKeys = new[] { Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6, Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12 };
                public static readonly Key[] FucntionKeys_Reverse = FunctionKeys.Reverse().ToArray();
                public static readonly Key[] MacroKeys = new[] { Key.Macro1, Key.Macro2, Key.Macro3, Key.Macro4, Key.Macro5 };
                public static readonly Key[] AllKeys = Enum.GetValues(typeof(Key)).Cast<Key>().Where(x => x != Key.Invalid).ToArray();
                public static readonly Key[][] KeyboardKeysArray = new Key[][]{
                       FunctionKeys,
                       new Key[]{ Key.Q,Key.W,Key.E,Key.R,Key.T,Key.Y,Key.U,Key.I,Key.O,Key.P,Key.OemLeftBracket,Key.OemRightBracket},
                       new Key[]{ Key.A,Key.S,Key.D,Key.F,Key.G,Key.H,Key.J,Key.K,Key.L,Key.OemSemicolon,Key.OemApostrophe},
                       new Key[]{ Key.Z,Key.X,Key.C,Key.V,Key.B,Key.N,Key.M,Key.OemComma,Key.OemPeriod,Key.OemSlash},
                };
                //public static readonly GridLed[] AllMouseLED = Enum.GetValues(typeof(GridLed)).Cast<GridLed>().ToArray(); //for future uses
                public static readonly GridLed[] LeftStrip = new[] { GridLed.LeftSide1, GridLed.LeftSide2, GridLed.LeftSide3, GridLed.LeftSide4, GridLed.LeftSide5, GridLed.LeftSide6, GridLed.LeftSide7 };
                public static readonly GridLed[] RightStrip = new[] { GridLed.RightSide1, GridLed.RightSide2, GridLed.RightSide3, GridLed.RightSide4, GridLed.RightSide5, GridLed.RightSide6, GridLed.RightSide7 };
                public static readonly GridLed[] LeftStrip_Reverse = new[] { GridLed.LeftSide7, GridLed.LeftSide6, GridLed.LeftSide5, GridLed.LeftSide4, GridLed.LeftSide3, GridLed.LeftSide2, GridLed.LeftSide1 };
                public static readonly GridLed[] RightStrip_Reverse = new[] { GridLed.RightSide7, GridLed.RightSide6, GridLed.RightSide5, GridLed.RightSide4, GridLed.RightSide3, GridLed.RightSide2, GridLed.RightSide1 };
            }
            static ColoreColor[] colors = new ColoreColor[] {
                ColoreColor.Red,
                ColoreColor.Orange,
                ColoreColor.Yellow,
                ColoreColor.Green,
                ColoreColor.Blue,
                ColoreColor.Purple,
                ColoreColor.Blue,
                ColoreColor.Green,
                ColoreColor.Yellow,
                ColoreColor.Orange,
                ColoreColor.Red
            };
            public static void SetPeakVolume(this ref MouseCustom MouseGrid, ColoreColor VolumeColor, float volume)
            {
                var absolutePosition = Math.Round((volume * Constant.LeftStrip.Length), 0);
                for (var i = 0; i < absolutePosition; i++)
                {
                    var color = VolumeColor.ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length);
                    if (i > 0)
                        MouseGrid[GridLed.Logo] = color;
                    if (i == Constant.LeftStrip.Length - 1)
                        MouseGrid[GridLed.ScrollWheel] = color;
                    MouseGrid[Constant.RightStrip_Reverse[i]] = color;
                    MouseGrid[Constant.LeftStrip_Reverse[i]] = color;
                }
            }
            private static int changeRate = 0;
            public static void SetChromaPeakVolume(this ref MouseCustom MouseGrid, float volume)
            {
                var absolutePosition = Math.Round((volume * Constant.LeftStrip.Length), 0);
                var currentDensity = (double)(absolutePosition) / (double)Constant.LeftStrip.Length;
                for (var i = 0; i < absolutePosition; i++)
                {
                    var color = colors[i];//.ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length);
                    if (i > 0)
                        MouseGrid[GridLed.Logo] = ColoreColor.White;//.ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length); ;
                    if (i == Constant.LeftStrip.Length - 1)
                        MouseGrid[GridLed.ScrollWheel] = ColoreColor.White;//color;//.ChangeColorDensity((double)(i + 1) / (double)Constant.LeftStrip.Length); ;
                    MouseGrid[Constant.RightStrip_Reverse[i]] = color;
                    MouseGrid[Constant.LeftStrip_Reverse[i]] = color;
                }
                changeRate += (int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0);
                if (changeRate >= 44)
                {
                    colors = colors.SubArray(1, colors.Length - 1).Add(colors[0]);
                    changeRate = 0;
                }

            }
            public static void SetPeakVolume(this ref KeyboardCustom KeyboardGrid, ColoreColor VolumeColor, float volume)
            {
                var absolutePosition = Math.Round((volume * Constant.FunctionKeys.Length), 0);
                for (var i = 0; i < absolutePosition; i++)
                {
                    var color = VolumeColor.ChangeColorDensity((double)(i + 1) / (double)Constant.FunctionKeys.Length);
                    KeyboardGrid[Constant.FunctionKeys[i]] = color;// VolumeColor;
                }
            }
            public static void SetChromaPeakVolume(this ref KeyboardCustom KeyboardGrid, float volume)
            {
                var absolutePosition = Math.Round((volume * Constant.FunctionKeys.Length), 0);
                for (var i = 0; i < absolutePosition; i++)
                {
                    var index = (int)(((float)i / (float)Constant.FunctionKeys.Length) * 7);
                    var color = colors[index];
                    KeyboardGrid[Constant.FunctionKeys[i]] = color;// VolumeColor;
                }
            }
            public static void SetPeakVolume(this ref MousepadCustom MousepadGrid, ColoreColor VolumeColor)
            {
                MousepadGrid.Set(VolumeColor);
            }
            public static void SetPeakVolume(this ref HeadsetCustom HeadsetGrid, ColoreColor VolumeColor)
            {
                HeadsetGrid.Set(VolumeColor);
            }
            public static void SetVolumeScale(this ref MouseCustom MouseGrid, ColoreColor VolumeColor, int volume, bool switchStripSide = false)
            {
                var mouseStrip = !switchStripSide ? Constant.RightStrip_Reverse : Constant.LeftStrip_Reverse;
                for (var i = 0; i < (volume * mouseStrip.Length) / 100; i++)
                {
                    MouseGrid[mouseStrip[i]] = VolumeColor;
                }
            }
            public static void SetVolumeScale(this ref KeyboardCustom KeyboardGrid, ColoreColor VolumeColor, int volume)
            {
                for (var i = 0; i < (volume * Constant.DPadKeys.Length) / 100; i++)
                {
                    KeyboardGrid[Constant.DPadKeys[i]] = VolumeColor;
                }
            }
            public static void SetPeakVolumeSymmetric(this ref MouseCustom MouseGrid, ColoreColor VolumeColor, float volume)
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
            public static void SetPeakVolumeSymmetric(this ref KeyboardCustom KeyboardGrid, ColoreColor VolumeColor, float volume)
            {
                var fullLength = Constant.FunctionKeys.Length;
                var startPosition = (fullLength / 2);
                var absolutePosition = Math.Round((volume * fullLength), 0) / 2;
                for (var i = startPosition; i < startPosition + absolutePosition; i++)
                {
                    var color = VolumeColor.ChangeColorDensity((double)(i + 1) / (double)(startPosition + absolutePosition));
                    KeyboardGrid[Constant.FunctionKeys[i]] = color;//VolumeColor;
                    KeyboardGrid[Constant.FunctionKeys[fullLength - i - 1]] = color;// VolumeColor;
                }
            }
            public static void SetPlayingTime(this ref KeyboardCustom KeyboardGrid, TimeSpan time)
            {
                KeyboardGrid[Constant.NumpadKeys[time.Minutes]] = ColoreColor.Red;
                //                                                                                    ie.        47           -             7          = 40/10 = 4
                //use a 'lossy' property of integer to round all floating point, best practice should be (currentTime.Seconds - (currentTime.Seconds % 10))/10
                KeyboardGrid[Constant.NumpadKeys[time.Seconds / 10]] = ColoreColor.Green;
                KeyboardGrid[Constant.NumpadKeys[time.Seconds % 10]] = ColoreColor.Blue;
            }
            public static void SetPlayingPosition(this ref MouseCustom MouseGrid, ColoreColor ForegroundColor, ColoreColor BackgroundColor, double position, bool switchStripSide = false)
            {
                var mouseStrip = !switchStripSide ? Constant.LeftStrip : Constant.RightStrip;
                var currentPlayPosition = (int)Math.Round(position * ((double)(mouseStrip.Length - 1) / 10), 0);
                for (var i = 0; i < currentPlayPosition + 1; i++)
                {
                    MouseGrid[mouseStrip[i]] = BackgroundColor;
                }
                MouseGrid[mouseStrip[currentPlayPosition]] = ForegroundColor;
            }
            public static void SetPlayingPosition(this ref KeyboardCustom KeyboardGrid, ColoreColor ForegroundColor, ColoreColor BackgroundColor, double position, bool switchStripSide = false)
            {
                var currentPlayPosition = (int)Math.Round(position * ((double)(Constant.FunctionKeys.Length - 1) / 10), 0);
                for (var i = 0; i < currentPlayPosition + 1; i++)
                {
                    KeyboardGrid[Constant.FunctionKeys[i]] = BackgroundColor;
                }
                KeyboardGrid[Constant.FunctionKeys[currentPlayPosition]] = ForegroundColor;
            }
        }
    }

}
