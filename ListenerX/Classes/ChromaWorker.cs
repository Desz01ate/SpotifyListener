using Listener.ImageProcessing;
using System;
using System.Diagnostics;
using System.Linq;
using ListenerX.Classes;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using ListenerX.Extensions;
using ChromaColor = VirtualGrid.Color;
using Listener.Plugin.ChromaEffect.Interfaces;
using System.Collections.Generic;
using VirtualGrid.Interfaces;
using VirtualGrid.Razer;
using VirtualGrid.Asus;
using Listener.Core.Framework.DataStructure;

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
                this.FullGridArray = VirtualGrid.VirtualLedGrid.CreateDefaultGrid();
                try
                {
                    var adapters = new List<IPhysicalDeviceAdapter>();
                    this._deviceAdapters = adapters;
                    adapters.Add(RazerAdapter.Instance);
                    adapters.Add(AsusRogStrix_G15_2021_Adapter.Instance);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //Asus SDK can sometime don't want to play by the rules.
                    if (ex is System.TypeInitializationException || ex is System.Runtime.InteropServices.COMException)
                    {
                        return;
                    }
                    else
                    {
                        this.IsError = true;
                    }
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