using Listener.ImageProcessing;
using System;
using System.Diagnostics;
using System.Linq;
using ListenerX.Classes;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using ListenerX.Extensions;
using Utilities.Shared;
using Colore;
using Colore.Effects.Virtual;
using ColoreColor = Colore.Data.Color;


namespace ListenerX
{
    namespace ChromaExtension
    {
        public sealed partial class ChromaWorker : IDisposable
        {
            private ColoreColor _primaryColor { get; set; } = ColoreColor.White;
            private ColoreColor _secondaryColor { get; set; } = ColoreColor.Black;
            private readonly IChroma _chromaInterface;
            private readonly AutoshiftCirculaQueue<ColoreColor> _rainbowColors;
            private AutoshiftCirculaQueue<ColoreColor> _albumColors;
            public readonly IVirtualLedGrid FullGridArray;

            public readonly bool IsError;

            private ColoreColor[][] _albumBackgroundSource;

            private static ChromaWorker _instance;
            public static ChromaWorker Instance => _instance ??= new ChromaWorker();


            private ChromaWorker()
            {
                this._rainbowColors = new AutoshiftCirculaQueue<ColoreColor>(ColorProcessing.GenerateRainbowSinusoidal().Select(x => new ColoreColor(x.Item1, x.Item2, x.Item3)), 500);
                this._albumColors = AutoshiftCirculaQueue<ColoreColor>.Empty;
                try
                {
                    this._chromaInterface = ColoreProvider.CreateNativeAsync().Result;
                    this.FullGridArray = this._chromaInterface.VirtualLedGrid;
                }
                catch (Exception ex)
                {
                    this.FullGridArray = VirtualLedGrid.CreateDefaultGrid();
                    this.IsError = true;
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
                    await this.FullGridArray.ApplyAsync();
                }
            }

            public void SDKDisable()
            {
                this.FullGridArray.Set(ColoreColor.Black);
                ApplyAsync().Wait();
            }
            /// <summary>
            /// Call this method to refresh the color properties
            /// </summary>
            /// <param name="player">player instance</param>
            /// <param name="density">density for adaptive color</param>
            public void LoadColor(Color primaryColor, Color secondaryColor)
            {
                this._primaryColor = primaryColor.ToColoreColor();
                this._secondaryColor = secondaryColor.ToColoreColor();

                var gradients = ColorProcessing.GenerateGradients(new[] { primaryColor, secondaryColor }, true);
                this._albumColors?.Dispose();
                this._albumColors = new AutoshiftCirculaQueue<ColoreColor>(gradients.Select(ColoreColorExtensions.ToColoreColor), 500);
            }

            public void LoadColor(Image image)
            {
                var colors = image.GetDominantColors(2);

                var width = this.FullGridArray.ColumnCount;
                var height = this.FullGridArray.RowCount;

                using var source = (Bitmap)ImageProcessing.Cut((Bitmap)image, width, height);
                using var resizedImage = (Bitmap)source.Resize(width, height);
                this._albumBackgroundSource = resizedImage.GetPixels().ToColoreColors();
                LoadColor(colors[0], colors[1]);
            }
            public ChromaWorker VisualizeVolumeEffects(double[] spectrumValues)
            {
                this.SetVisualizeSpectrum(this._albumColors, spectrumValues);
                return this;
            }

            public ChromaWorker VisualizeVolumeChromaEffects(double[] spectrumValues)
            {
                this.SetVisualizeSpectrum(this._rainbowColors, spectrumValues);
                return this;
            }

            public ChromaWorker VisualizeVolumeBackgroundEffects(double[] spectrumValues)
            {
                this.SetVisualizeAlbumBackground(this._albumColors, spectrumValues);
                return this;
            }
            public ChromaWorker PlayingPositionEffects(double[] spectrumValues, double position)
            {
                if (double.IsNaN(position) || double.IsInfinity(position))
                    return this;
                this.SetPlayingPosition(this._albumColors, spectrumValues, position);
                return this;
            }

            public ChromaWorker PlayingPositionBackgroundEffects(double[] spectrumValues, double position)
            {
                if (double.IsNaN(position) || double.IsInfinity(position))
                    return this;
                this.SetPlayingPosition(spectrumValues, position);
                return this;
            }

            private bool disposed = false;
            protected void Dispose(bool disposing)
            {
                if (disposing && !disposed)
                {
                    this._chromaInterface?.Dispose();
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
                        this.FullGridArray[x, y] = foreground;
                    }
                }
            }

            private void SetVisualizeAlbumBackground(
                ICollection<ColoreColor> colors,
                double[] spectrumValues)
            {
                if (this._albumBackgroundSource == null)
                    return;
                this.FullGridArray.Set(this._albumBackgroundSource, Properties.Settings.Default.BackgroundBrightness);

                for (var x = 0; x < this.FullGridArray.ColumnCount; x++)
                {
                    var foreground = colors.ElementAt(colors.Count - 1 - x);

                    var c = spectrumValues[x];
                    var absSpectrum = this.FullGridArray.RowCount - (int)Math.Round((this.FullGridArray.RowCount * (c / 100.0d)), 0);
                    for (var y = this.FullGridArray.RowCount - 1; y >= absSpectrum; y--)
                    {
                        //var foreground = this._albumBackgroundSource[y][x];
                        this.FullGridArray[x, y] = foreground;
                    }
                }
            }

            private void SetPlayingPosition(ICollection<ColoreColor> colors, double[] spectrumValues, double position)
            {
                for (var x = 0; x < this.FullGridArray.ColumnCount; x++)
                {
                    var background = colors.ElementAt(x).ChangeBrightnessLevel(Properties.Settings.Default.BackgroundBrightness);
                    foreach (var key in this.FullGridArray.Where(e => e.Index.X == x))
                    {
                        key.Color = background;
                    }
                }

                var keyboardGrid = this.FullGridArray.Where(x => x.Type == Colore.Effects.Virtual.KeyType.Keyboard || x.Type == Colore.Effects.Virtual.KeyType.Mousepad || x.Type == Colore.Effects.Virtual.KeyType.Invalid);
                var keyboardRowCount = keyboardGrid.Max(e => e.Index.Y) + 1;
                for (var rowIdx = 0; rowIdx < keyboardRowCount; rowIdx++)
                {
                    var row = keyboardGrid.Where(e => e.Index.Y == rowIdx && e.Index.X < 22).ToArray();
                    var pos = (int)Math.Round(position * ((double)(row.Length - 1) / 10), 0);
                    var key = row[pos];
                    this.FullGridArray[key.Index.X, key.Index.Y] = _primaryColor;
                    if (0 < pos - 1 && pos + 1 < row.Length)
                    {
                        var leftKey = row[pos - 1];
                        var rightKey = row[pos + 1];

                        var adjacentColor = _primaryColor.ChangeBrightnessLevel(0.5);
                        this.FullGridArray[leftKey.Index.X, leftKey.Index.Y] = adjacentColor;
                        this.FullGridArray[rightKey.Index.X, rightKey.Index.Y] = adjacentColor;
                    }
                }

                var maxMouseY = this.FullGridArray.Where(x => 22 < x.Index.X && x.Index.X < 29).Max(x => x.Index.Y) + 1;
                var currentPlayPosition = (int)Math.Round(position * ((double)(maxMouseY - 1) / 10), 0);
                this.FullGridArray[22, currentPlayPosition] = _primaryColor;

                var vizColor = this._albumColors.First();
                for (var x = 23; x < 29; x++)
                {
                    var volume = spectrumValues[x - 23];
                    var absPosition = maxMouseY - (int)Math.Round((volume / 100d) * maxMouseY, 0);
                    for (var y = maxMouseY - 1; y >= absPosition; y--)
                    {
                        this.FullGridArray[x, y] = vizColor;
                    }
                }
            }

            private void SetPlayingPosition(double[] spectrumValues, double position)
            {
                if (this._albumBackgroundSource == null)
                    return;
                this.FullGridArray.Set(this._albumBackgroundSource, Properties.Settings.Default.BackgroundBrightness);

                var keyboardGrid = this.FullGridArray.Where(x => x.Type == Colore.Effects.Virtual.KeyType.Keyboard || x.Type == Colore.Effects.Virtual.KeyType.Mousepad || x.Type == Colore.Effects.Virtual.KeyType.Invalid);
                var keyboardRowCount = keyboardGrid.Max(e => e.Index.Y) + 1;
                for (var rowIdx = 0; rowIdx < keyboardRowCount; rowIdx++)
                {
                    var row = keyboardGrid.Where(e => e.Index.Y == rowIdx && e.Index.X < 22).ToArray();
                    var pos = (int)Math.Round(position * ((double)(row.Length - 1) / 10), 0);
                    var key = row[pos];
                    this.FullGridArray[key.Index.X, key.Index.Y] = _primaryColor;
                    if (0 < pos - 1 && pos + 1 < row.Length)
                    {
                        var leftKey = row[pos - 1];
                        var rightKey = row[pos + 1];

                        var adjacentColor = _primaryColor.ChangeBrightnessLevel(0.5);
                        this.FullGridArray[leftKey.Index.X, leftKey.Index.Y] = adjacentColor;
                        this.FullGridArray[rightKey.Index.X, rightKey.Index.Y] = adjacentColor;
                    }
                }

                var maxMouseY = this.FullGridArray.Where(x => 22 < x.Index.X && x.Index.X < 29).Max(x => x.Index.Y) + 1;
                var currentPlayPosition = (int)Math.Round(position * ((double)(maxMouseY - 1) / 10), 0);
                this.FullGridArray[22, currentPlayPosition] = _primaryColor;

                var vizColor = this._albumColors.First();
                for (var x = 23; x < 29; x++)
                {
                    var volume = spectrumValues[x - 23];
                    var absPosition = maxMouseY - (int)Math.Round((volume / 100d) * maxMouseY, 0);
                    for (var y = maxMouseY - 1; y >= absPosition; y--)
                    {
                        this.FullGridArray[x, y] = vizColor;
                    }
                }
            }
        }
    }
}