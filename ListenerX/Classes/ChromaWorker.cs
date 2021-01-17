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
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Colore.Effects.ChromaLink;

namespace ListenerX
{
    namespace ChromaExtension
    {
        public sealed partial class ChromaWorker : IDisposable
        {
            private ColoreColor primaryColor { get; set; } = ColoreColor.White;
            private ColoreColor secondaryColor { get; set; } = ColoreColor.Black;
            private CustomKeyboardEffect keyboardGrid = CustomKeyboardEffect.Create();
            private CustomMouseEffect mouseGrid = CustomMouseEffect.Create();
            private CustomMousepadEffect mousepadGrid = CustomMousepadEffect.Create();
            private CustomHeadsetEffect headsetGrid = CustomHeadsetEffect.Create();
            private CustomChromaLinkEffect chromaLinkGrid = CustomChromaLinkEffect.Create();
            private readonly IChroma chromaInterface;
            private readonly AutoshiftCirculaQueue<ColoreColor> rainbowColors;
            private AutoshiftCirculaQueue<ColoreColor> albumColors;

            private ColoreColor[][] albumBackgroundSource;

            private static ChromaWorker _instance;
            public static ChromaWorker Instance
            {
                get
                {
                    return _instance ??= new ChromaWorker();
                }
            }
            public bool IsError { get; private set; }

            private readonly AbstractKeyGrid FullGridArray;
            private ChromaWorker()
            {
                try
                {
                    this.FullGridArray = AbstractKeyGrid.GetDefaultGrid();
                    this.chromaInterface = ColoreProvider.CreateNativeAsync().Result;
                    this.rainbowColors = new AutoshiftCirculaQueue<ColoreColor>(ColorProcessing.GenerateRainbowSinusoidal().Select(x => new ColoreColor(x.Item1, x.Item2, x.Item3)), 500);
                    this.albumColors = AutoshiftCirculaQueue<ColoreColor>.Empty; //default initializer.
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
                if (!this.IsError)
                {
                    foreach (var key in this.FullGridArray.EnumerateKeyboardKeys())
                    {
                        keyboardGrid[key.Key] = key.Color;
                    }
                    foreach (var key in this.FullGridArray.EnumerateMouseKeys())
                    {
                        mouseGrid[key.Key] = key.Color;
                    }

                    await chromaInterface.Keyboard.SetCustomAsync(keyboardGrid);
                    await chromaInterface.Mouse.SetGridAsync(mouseGrid);
                    await chromaInterface.Headset.SetCustomAsync(headsetGrid);
                    await chromaInterface.Mousepad.SetCustomAsync(mousepadGrid);
                    await chromaInterface.ChromaLink.SetCustomAsync(chromaLinkGrid);
                    //this.chromaLinkGrid[]
                }
                headsetGrid.Clear();
                mousepadGrid.Clear();
            }

            public void SDKDisable()
            {
                mouseGrid.Set(ColoreColor.Black);
                keyboardGrid.Set(ColoreColor.Black);
                mousepadGrid.Set(ColoreColor.Black);
                headsetGrid.Set(ColoreColor.Black);
                ApplyAsync().Wait();
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
                primaryColor = razerColor.Standard;
                secondaryColor = razerColor.Complemented;

                var gradients = ColorProcessing.GenerateGradients(new[] { color.Standard, color.Complemented }, true);
                this.albumColors?.Dispose();
                this.albumColors = new AutoshiftCirculaQueue<ColoreColor>(gradients.Select(ColoreColorProcessor.ToColoreColor), 500);
            }

            public void LoadColor(Image image)
            {
                var colors = image.GetDominantColors(2);
                var standardRenderColor = new StandardColor();
                standardRenderColor.Standard = colors[0];
                standardRenderColor.Complemented = colors[1];

                //using var resizedImage = (Bitmap)image.Resize(this.FullGridArray.ColumnCount, this.FullGridArray.RowCount);

                var width = this.FullGridArray.ColumnCount;
                var height = this.FullGridArray.RowCount;

                using var source = (Bitmap)ImageProcessing.Cut((Bitmap)image, width, height);
                using var resizedImage = (Bitmap)source.Resize(width, height);
                this.albumBackgroundSource = resizedImage.GetPixels().ToColoreColors();
                LoadColor(standardRenderColor);
            }
            public ChromaWorker VisualizeVolumeEffects(double[] spectrumValues)
            {
                this.SetVisualizeSpectrum(this.albumColors, spectrumValues);
                return this;
            }

            public ChromaWorker VisualizeVolumeChromaEffects(double[] spectrumValues)
            {
                this.SetVisualizeSpectrum(this.rainbowColors, spectrumValues);
                return this;
            }

            public ChromaWorker VisualizeVolumeBackgroundEffects(double[] spectrumValues)
            {
                this.SetVisualizeAlbumBackground(this.albumColors, spectrumValues);
                return this;
            }
            public ChromaWorker PlayingPositionEffects(double volume, double position)
            {
                if (double.IsNaN(position) || double.IsInfinity(position))
                    return this;
                var backgroundColor = this.primaryColor.ChangeBrightnessLevel(0.2);
                this.keyboardGrid.Set(backgroundColor);
                this.mouseGrid.Set(backgroundColor);
                this.SetPlayingPosition(this.albumColors, volume, position);
                return this;
            }

            public ChromaWorker PlayingPositionBackgroundEffects(double volume, double position)
            {
                if (double.IsNaN(position) || double.IsInfinity(position))
                    return this;
                this.SetPlayingPosition(volume, position);
                return this;
            }

            private bool disposed = false;
            protected void Dispose(bool disposing)
            {
                if (disposing && !disposed)
                {
                    this.chromaInterface?.Dispose();
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
            private void SetVisualizeSpectrum(
                ICollection<ColoreColor> colors,
                double[] spectrumValues)
            {
                for (var x = 0; x < this.FullGridArray.ColumnCount; x++)
                {
                    var foreground = colors.ElementAt(colors.Count - 1 - x);
                    var background = foreground.ChangeBrightnessLevel(Properties.Settings.Default.BackgroundBrightness);//colors.ElementAt(x).ChangeBrightnessLevel(Properties.Settings.Default.BackgroundBrightness);
                    foreach (var key in this.FullGridArray.Where(e => e.Index.X == x))
                    {
                        key.Color = background;
                    }

                    var c = spectrumValues[x];
                    var absSpectrum = this.FullGridArray.RowCount - (int)Math.Round((this.FullGridArray.RowCount * (c / 100.0d)), 0);
                    for (var y = this.FullGridArray.RowCount - 1; y >= absSpectrum; y--)
                    {
                        var key = this.FullGridArray[x, y];
                        key.Color = foreground;
                    }
                }
                var lastColor = colors.Last();
                this.chromaLinkGrid.Set(lastColor);
                this.mousepadGrid.Set(lastColor);
                this.headsetGrid.Set(lastColor);
            }

            private void SetVisualizeAlbumBackground(
                ICollection<ColoreColor> colors,
                double[] spectrumValues)
            {
                if (this.albumBackgroundSource == null)
                    return;
                this.FullGridArray.Set(this.albumBackgroundSource, Properties.Settings.Default.BackgroundBrightness);

                for (var x = 0; x < this.FullGridArray.ColumnCount; x++)
                {
                    var foreground = colors.ElementAt(colors.Count - 1 - x);

                    var c = spectrumValues[x];
                    var absSpectrum = this.FullGridArray.RowCount - (int)Math.Round((this.FullGridArray.RowCount * (c / 100.0d)), 0);
                    for (var y = this.FullGridArray.RowCount - 1; y >= absSpectrum; y--)
                    {
                        var key = this.FullGridArray[x, y];
                        key.Color = foreground;
                    }
                }
                var lastColor = colors.Last();
                this.chromaLinkGrid.Set(lastColor);
                this.mousepadGrid.Set(lastColor);
                this.headsetGrid.Set(lastColor);
            }

            private void SetPlayingPosition(ICollection<ColoreColor> colors, double volume, double position)
            {
                for (var x = 0; x < this.FullGridArray.ColumnCount; x++)
                {
                    var background = colors.ElementAt(x).ChangeBrightnessLevel(Properties.Settings.Default.BackgroundBrightness);
                    foreach (var key in this.FullGridArray.Where(e => e.Index.X == x))
                    {
                        key.Color = background;
                    }
                }

                var keyboardGrid = this.FullGridArray.EnumerateKeyboardKeys(true);
                var keyboardRowCount = keyboardGrid.Max(e => e.Index.Y) + 1;
                for (var rowIdx = 0; rowIdx < keyboardRowCount; rowIdx++)
                {
                    var row = keyboardGrid.Where(e => e.Index.Y == rowIdx && e.Index.X < 22).ToArray();
                    var pos = (int)Math.Round(position * ((double)(row.Length - 1) / 10), 0);
                    var key = row[pos];
                    this.FullGridArray[key.Index.X, key.Index.Y].Color = primaryColor;
                    if (0 < pos - 1 && pos + 1 < row.Length)
                    {
                        var leftKey = row[pos - 1];
                        var rightKey = row[pos + 1];

                        var adjacentColor = primaryColor.ChangeBrightnessLevel(0.5);
                        this.FullGridArray[leftKey.Index.X, leftKey.Index.Y].Color = adjacentColor;
                        this.FullGridArray[rightKey.Index.X, rightKey.Index.Y].Color = adjacentColor;
                    }
                }

                var mouseGrid = this.FullGridArray.EnumerateMouseKeys(true).ToArray();
                var maxMouseY = mouseGrid.Max(e => e.Index.Y) + 1;
                var currentPlayPosition = (int)Math.Round(position * ((double)(maxMouseY - 1) / 10), 0);
                this.FullGridArray[22, currentPlayPosition].Color = primaryColor;

                var volumeScale = maxMouseY - (int)Math.Round((volume / 100d) * maxMouseY, 0);
                for (var y = maxMouseY - 1; y >= volumeScale; y--)
                {
                    this.FullGridArray[28, y].Color = primaryColor;
                }
                var lastColor = colors.Last();
                this.chromaLinkGrid.Set(lastColor);
                this.mousepadGrid.Set(lastColor);
                this.headsetGrid.Set(lastColor);
            }

            private void SetPlayingPosition(double volume, double position)
            {
                if (this.albumBackgroundSource == null)
                    return;
                this.FullGridArray.Set(this.albumBackgroundSource, Properties.Settings.Default.BackgroundBrightness);

                var keyboardGrid = this.FullGridArray.EnumerateKeyboardKeys(true);
                var keyboardRowCount = keyboardGrid.Max(e => e.Index.Y) + 1;
                for (var rowIdx = 0; rowIdx < keyboardRowCount; rowIdx++)
                {
                    var row = keyboardGrid.Where(e => e.Index.Y == rowIdx && e.Index.X < 22).ToArray();
                    var pos = (int)Math.Round(position * ((double)(row.Length - 1) / 10), 0);
                    var key = row[pos];
                    this.FullGridArray[key.Index.X, key.Index.Y].Color = primaryColor;
                    if (0 < pos - 1 && pos + 1 < row.Length)
                    {
                        var leftKey = row[pos - 1];
                        var rightKey = row[pos + 1];

                        var adjacentColor = primaryColor.ChangeBrightnessLevel(0.5);
                        this.FullGridArray[leftKey.Index.X, leftKey.Index.Y].Color = adjacentColor;
                        this.FullGridArray[rightKey.Index.X, rightKey.Index.Y].Color = adjacentColor;
                    }
                }

                var mouseGrid = this.FullGridArray.EnumerateMouseKeys(true).ToArray();
                var maxMouseY = mouseGrid.Max(e => e.Index.Y) + 1;
                var currentPlayPosition = (int)Math.Round(position * ((double)(maxMouseY - 1) / 10), 0);
                this.FullGridArray[22, currentPlayPosition].Color = primaryColor;

                var volumeScale = maxMouseY - (int)Math.Round((volume / 100d) * maxMouseY, 0);
                for (var y = maxMouseY - 1; y >= volumeScale; y--)
                {
                    this.FullGridArray[28, y].Color = primaryColor;
                }
                var lastColor = this.albumBackgroundSource.Last().Last();
                this.chromaLinkGrid.Set(lastColor);
                this.mousepadGrid.Set(lastColor);
                this.headsetGrid.Set(lastColor);
            }
        }


    }
}