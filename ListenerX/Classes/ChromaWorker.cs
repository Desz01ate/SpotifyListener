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
        public sealed partial class ChromaWorker : IDisposable
        {
            private ColoreColor PrimaryColor { get; set; } = ColoreColor.White;
            private ColoreColor SecondaryColor { get; set; } = ColoreColor.Black;
            private CustomKeyboardEffect KeyboardGrid = CustomKeyboardEffect.Create();
            private CustomMouseEffect MouseGrid = CustomMouseEffect.Create();
            private CustomMousepadEffect MousepadGrid = CustomMousepadEffect.Create();
            private CustomHeadsetEffect HeadsetGrid = CustomHeadsetEffect.Create();
            private IChroma Chroma;
            public static ChromaWorker Instance { get; } = new ChromaWorker();
            public bool IsError { get; private set; }

            private readonly AbstractKeyGrid FullGridArray;
            private ChromaWorker()
            {
                try
                {
                    this.Chroma = ColoreProvider.CreateNativeAsync().Result;
                    this.FullGridArray = AbstractKeyGrid.GetDefaultGrid();
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
                foreach (var key in this.FullGridArray.EnumerateKeyboardKeys())
                {
                    KeyboardGrid[key.Key] = key.Color;
                }
                foreach (var key in this.FullGridArray.EnumerateMouseKeys())
                {
                    MouseGrid[key.Key] = key.Color;
                }
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

            public ChromaWorker VisualizeVolumeEffects(double[] spectrumValues)
            {
                //var keyboardSpectrum = spectrumValues.Take(22).ToArray();
                //var mouseSpectrum = spectrumValues.TakeLast(3).ToArray();
                //this.KeyboardGrid.SetVisualizeSpectrum(this.PrimaryColor, this.SecondaryColor, keyboardSpectrum);
                //this.MouseGrid.SetVisualizeSpectrum(this.PrimaryColor, this.SecondaryColor, mouseSpectrum);
                this.SetVisualizeSpectrum(spectrumValues);
                return this;
            }

            public ChromaWorker VisualizeVolumeChromaEffects(double[] spectrumValues)
            {
                this.SetChromaVisualizeSpectrum(spectrumValues);
                return this;
            }

            public ChromaWorker PlayingPositionEffects(double volume, double position)
            {
                if (double.IsNaN(position) || double.IsInfinity(position))
                    return this;
                var backgroundColor = this.PrimaryColor.ChangeBrightnessLevel(0.2);
                this.KeyboardGrid.Set(backgroundColor);
                this.MouseGrid.Set(backgroundColor);
                this.SetPlayingPosition(volume, position);
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

        public sealed partial class ChromaWorker
        {
            private static readonly AutoshiftCirculaQueue<ColoreColor> colors = new AutoshiftCirculaQueue<ColoreColor>(RGBCollection.HIGH_RESOLUTION_RGB, 500);
            private void SetChromaVisualizeSpectrum(
                double[] spectrumValues)
            {
                for (var x = 0; x < this.FullGridArray.ColumnCount; x++)
                {

                    var color = colors.ElementAt(x);
                    var background = color.ChangeBrightnessLevel(Properties.Settings.Default.BackgroundBrightness);

                    foreach (var key in this.FullGridArray.Where(e => e.Index.X == x))
                    {
                        key.Color = background;
                    }

                    var c = spectrumValues[x];
                    var absSpectrum = this.FullGridArray.RowCount - (int)Math.Round((this.FullGridArray.RowCount * (c / 100.0d)), 0);
                    for (var y = this.FullGridArray.RowCount - 1; y >= absSpectrum; y--)
                    {
                        var key = this.FullGridArray[x, y];
                        key.Color = color;
                    }
                }
            }

            private void SetVisualizeSpectrum(
                double[] spectrumValues)
            {
                for (var x = 0; x < this.FullGridArray.ColumnCount; x++)
                {

                    var color = this.PrimaryColor;
                    var background = this.SecondaryColor.ChangeBrightnessLevel(Properties.Settings.Default.BackgroundBrightness);

                    foreach (var key in this.FullGridArray.Where(e => e.Index.X == x))
                    {
                        key.Color = background;
                    }

                    var c = spectrumValues[x];
                    var absSpectrum = this.FullGridArray.RowCount - (int)Math.Round((this.FullGridArray.RowCount * (c / 100.0d)), 0);
                    for (var y = this.FullGridArray.RowCount - 1; y >= absSpectrum; y--)
                    {
                        var key = this.FullGridArray[x, y];
                        key.Color = color;
                    }
                }
            }

            private void SetPlayingPosition(double volume, double position)
            {
                this.FullGridArray.SetAll(PrimaryColor.ChangeBrightnessLevel(Properties.Settings.Default.BackgroundBrightness));
                var keyboardGrid = this.FullGridArray.EnumerateKeyboardKeys(true);
                var keyboardRowCount = keyboardGrid.Max(e => e.Index.Y) + 1;
                for (var rowIdx = 0; rowIdx < keyboardRowCount; rowIdx++)
                {
                    var row = keyboardGrid.Where(e => e.Index.Y == rowIdx && e.Index.X < 22).ToArray();
                    var pos = (int)Math.Round(position * ((double)(row.Length - 1) / 10), 0);
                    var key = row[pos];
                    this.FullGridArray[key.Index.X, key.Index.Y].Color = PrimaryColor;
                    if (0 < pos - 1 && pos + 1 < row.Length)
                    {
                        var leftKey = row[pos - 1];
                        var rightKey = row[pos + 1];

                        this.FullGridArray[leftKey.Index.X, leftKey.Index.Y].Color = SecondaryColor;
                        this.FullGridArray[rightKey.Index.X, rightKey.Index.Y].Color = SecondaryColor;
                    }
                }

                var mouseGrid = this.FullGridArray.EnumerateMouseKeys(true).ToArray();
                var maxMouseY = mouseGrid.Max(e => e.Index.Y) + 1;
                var currentPlayPosition = (int)Math.Round(position * ((double)(maxMouseY - 1) / 10), 0);
                //var mouseStrip = !switchStripSide ? 22 : 28;
                this.FullGridArray[22, currentPlayPosition].Color = PrimaryColor;

                var volumeScale = maxMouseY - (int)Math.Round((volume / 100d) * maxMouseY, 0);
                for (var y = maxMouseY - 1; y >= volumeScale; y--)
                {
                    this.FullGridArray[28, y].Color = PrimaryColor;
                }
            }
        }


    }
    static class RGBCollection
    {
        public static readonly ColoreColor[] VERY_HIGH_RESOLUTION_RGB = new ColoreColor[] {
//RED->GREEN
new ColoreColor(255,0  ,0),
new ColoreColor(255,16 ,0),
new ColoreColor(255,32 ,0),
new ColoreColor(255,48, 0),
new ColoreColor(255,64 ,0),
new ColoreColor(255,80 ,0),
new ColoreColor(255,96 ,0),
new ColoreColor(255,112,0),
new ColoreColor(255,128,0),
new ColoreColor(255,144,0),
new ColoreColor(255,160,0),
new ColoreColor(255,176,0),
new ColoreColor(255,192,0),
new ColoreColor(255,208,0),
new ColoreColor(255,224,0),
new ColoreColor(255,240,0),
new ColoreColor(255,255,0),
new ColoreColor(240,255,0),
new ColoreColor(224,255,0),
new ColoreColor(208,255,0),
new ColoreColor(192,255,0),
new ColoreColor(176,255,0),
new ColoreColor(160,255,0),
new ColoreColor(144,255,0),
new ColoreColor(128,255,0),
new ColoreColor(112,255,0),
new ColoreColor(96 ,255,0),
new ColoreColor(80 ,255,0),
new ColoreColor(64 ,255,0),
new ColoreColor(48 ,255,0),
new ColoreColor(32 ,255,0),
new ColoreColor(16 ,255,0),
//GREEN->BLUE
new ColoreColor(0  ,255, 0),
new ColoreColor(0  ,255,16),
new ColoreColor(0  ,255,32),
new ColoreColor(0  ,255,48),
new ColoreColor(0  ,255,64),
new ColoreColor(0  ,255,80),
new ColoreColor(0,255,96 ),
new ColoreColor(0,255,112),
new ColoreColor(0,255,128),
new ColoreColor(0,255,144),
new ColoreColor(0,255,160),
new ColoreColor(0,255,176),
new ColoreColor(0,255,192),
new ColoreColor(0,255,208),
new ColoreColor(0,255,224),
new ColoreColor(0,255,240),
new ColoreColor(0,255,255),
new ColoreColor(0,240,255),
new ColoreColor(0,224,255),
new ColoreColor(0,208,255),
new ColoreColor(0,192,255),
new ColoreColor(0,176,255),
new ColoreColor(0,160,255),
new ColoreColor(0,144,255),
new ColoreColor(0,128,255),
new ColoreColor(0,112,255),
new ColoreColor(0,96 ,255),
new ColoreColor(0,80,255),
new ColoreColor(0,64 ,255),
new ColoreColor(0,48,255),
new ColoreColor(0,32 ,255),
new ColoreColor(0,16,255),
//BLUE->RED
new ColoreColor(0  ,0  ,255),
new ColoreColor(16,0,255),
new ColoreColor(32 ,0  ,255),
new ColoreColor(48,0,255),
new ColoreColor(64 ,0  ,255),
new ColoreColor(80,0,255),
new ColoreColor(96 ,0  ,255),
new ColoreColor(112,0,255),
new ColoreColor(128,0  ,255),
new ColoreColor(144,0,255),
new ColoreColor(160,0  ,255),
new ColoreColor(176,0,255),
new ColoreColor(192,0  ,255),
new ColoreColor(208,0,255),
new ColoreColor(224,0  ,255),
new ColoreColor(240,0,255),
new ColoreColor(255,0  ,255),
new ColoreColor(255,0,240),
new ColoreColor(255,0  ,224),
new ColoreColor(255,0,208),
new ColoreColor(255,0  ,192),
new ColoreColor(255,0,176),
new ColoreColor(255,0  ,160),
new ColoreColor(255,0,144),
new ColoreColor(255,0  ,128),
new ColoreColor(255,0,112),
new ColoreColor(255,0  ,96 ),
new ColoreColor(255,0,80),
new ColoreColor(255,0  ,64 ),
new ColoreColor(255,0,48),
new ColoreColor(255,0  ,32 ),
new ColoreColor(255,0,16)
//new ColoreColor(32 ,0  ,224),
//new ColoreColor(64 ,0  ,192),
//new ColoreColor(96 ,0  ,160),
//new ColoreColor(128,0  ,128),
//new ColoreColor(160,0  ,96 ),
//new ColoreColor(192,0  ,64 ),
//new ColoreColor(224,0  ,32 ),
        };
        public static readonly ColoreColor[] HIGH_RESOLUTION_RGB = new ColoreColor[] {
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
new ColoreColor(255,0  ,32 ),
new ColoreColor(255,0,0)
//END LOOP SET



//new ColoreColor(32 ,0  ,224),
//new ColoreColor(64 ,0  ,192),
//new ColoreColor(96 ,0  ,160),
//new ColoreColor(128,0  ,128),
//new ColoreColor(160,0  ,96 ),
//new ColoreColor(192,0  ,64 ),
//new ColoreColor(224,0  ,32 ),
        };
    }
}