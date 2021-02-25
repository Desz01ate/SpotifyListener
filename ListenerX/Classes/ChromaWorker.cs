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
using Listener.Plugin.Razer.Interfaces;

namespace ListenerX
{
    namespace ChromaExtension
    {
        public sealed partial class ChromaWorker : IDisposable
        {
            private ColoreColor _primaryColor { get; set; } = ColoreColor.White;
            private ColoreColor _secondaryColor { get; set; } = ColoreColor.Black;
            private readonly IChroma _chromaInterface;

            private AutoshiftCirculaQueue<ColoreColor> _albumColors;

            public readonly IVirtualLedGrid FullGridArray;

            public readonly bool IsError;

            private ColoreColor[][] _albumBackgroundSource;

            private static Lazy<ChromaWorker> _instance = new Lazy<ChromaWorker>(() => new ChromaWorker(), true);
            public static ChromaWorker Instance => _instance.Value;


            private ChromaWorker()
            {
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

            internal void SetEffect(IRazerEffect effect, double[] spectrumValues, double playingPosition)
            {
                effect.SetEffect(this.FullGridArray, this._primaryColor, this._secondaryColor, this._albumColors, this._albumBackgroundSource, spectrumValues, playingPosition, Properties.Settings.Default.BackgroundBrightness);
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
    }
}