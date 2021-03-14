using Listener.ImageProcessing;
using System;
using System.Diagnostics;
using System.Linq;
using ListenerX.Classes;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using ListenerX.Extensions;
using Colore;
using ChromaColor = Listener.Plugin.ChromaEffect.Implementation.Color;
using Listener.Plugin.ChromaEffect.Interfaces;
using System.Collections.Generic;
using ListenerX.Classes.Adapter;

namespace ListenerX
{
    namespace ChromaExtension
    {
        public sealed partial class ChromaWorker : IDisposable
        {
            private ChromaColor _primaryColor { get; set; } = ChromaColor.White;
            private ChromaColor _secondaryColor { get; set; } = ChromaColor.Black;

            private ChromaColor[][] _albumBackgroundSource;

            private readonly IReadOnlyList<IPhysicalDeviceAdapter> _deviceAdapters;

            private AutoshiftCirculaQueue<ChromaColor> _albumColors;

            public readonly IVirtualLedGrid FullGridArray;

            public readonly bool IsError;


            private static Lazy<ChromaWorker> _instance = new Lazy<ChromaWorker>(() => new ChromaWorker(), true);
            public static ChromaWorker Instance => _instance.Value;


            private ChromaWorker()
            {
                this._albumColors = AutoshiftCirculaQueue<ChromaColor>.Empty;
                this.FullGridArray = Listener.Plugin.ChromaEffect.Implementation.VirtualLedGrid.CreateDefaultGrid();
                try
                {
                    var adapters = new List<IPhysicalDeviceAdapter>();
                    adapters.Add(new RazerSdkAdapter());
                    adapters.Add(new AsusSdkAdapter());
                    this._deviceAdapters = adapters;
                }
                catch (Exception ex)
                {
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
                    foreach (var adapter in _deviceAdapters)
                    {
                        await adapter?.ApplyAsync(FullGridArray);
                    }
                }
            }

            public void SDKDisable()
            {
                this.FullGridArray.Set(ChromaColor.Black);
                ApplyAsync().Wait();
            }
            /// <summary>
            /// Call this method to refresh the color properties
            /// </summary>
            /// <param name="player">player instance</param>
            /// <param name="density">density for adaptive color</param>
            public void LoadColor(Color primaryColor, Color secondaryColor)
            {
                this._primaryColor = primaryColor.ToChromaColor();
                this._secondaryColor = secondaryColor.ToChromaColor();

                var gradients = ColorProcessing.GenerateGradients(new[] { primaryColor, secondaryColor }, true);
                this._albumColors?.Dispose();
                this._albumColors = new AutoshiftCirculaQueue<ChromaColor>(gradients.Select(ColorExtensions.ToChromaColor), 500);
            }

            public void LoadColor(Image image)
            {
                var colors = image.GetDominantColors(2);

                var width = this.FullGridArray.ColumnCount;
                var height = this.FullGridArray.RowCount;

                using var source = (Bitmap)ImageProcessing.Cut((Bitmap)image, width, height);
                using var resizedImage = (Bitmap)source.Resize(width, height);
                this._albumBackgroundSource = resizedImage.GetPixels().ToChromaColors();
                LoadColor(colors[0], colors[1]);
            }

            internal void SetEffect(IChromaEffect effect, double[] spectrumValues, double playingPosition)
            {
                effect.SetEffect(this.FullGridArray, this._primaryColor, this._secondaryColor, this._albumColors, this._albumBackgroundSource, spectrumValues, playingPosition, Properties.Settings.Default.BackgroundBrightness);
            }

            private bool disposed = false;
            protected void Dispose(bool disposing)
            {
                if (disposing && !disposed)
                {
                    foreach (var adapter in _deviceAdapters)
                        adapter?.Dispose();
                }
                disposed = true;
            }
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

        }
    }
}