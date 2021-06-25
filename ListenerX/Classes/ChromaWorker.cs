using Listener.Core.Framework.DataStructure;
using Listener.ImageProcessing;
using Listener.Plugin.ChromaEffect.Interfaces;
using ListenerX.Classes;
using ListenerX.Extensions;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtualGrid.Asus;
using VirtualGrid.Interfaces;
using VirtualGrid.Razer;
using ChromaColor = VirtualGrid.Color;

namespace ListenerX
{
    namespace ChromaExtension
    {
        public sealed partial class ChromaWorker : IDisposable
        {
            private ChromaColor _primaryColor { get; set; } = ChromaColor.White;
            private ChromaColor _secondaryColor { get; set; } = ChromaColor.Black;
            private ChromaColor[][] _albumBackgroundSource;
            private readonly IVirtualLedGrid _virtualGrid;
            private readonly IArrangeMediator _physicalDeviceMediator;
            private readonly ISettings _settings;
            private AutoshiftCirculaQueue<ChromaColor> _albumColors;

            public readonly bool IsError;
            public ChromaWorker(IVirtualLedGrid virtualGrid,
                                IArrangeMediator arrangeMediator,
                                ISettings settings)
            {
                this._albumColors = AutoshiftCirculaQueue<ChromaColor>.Empty;
                this._virtualGrid = virtualGrid;
                this._settings = settings;
                try
                {
                    this._physicalDeviceMediator = arrangeMediator;
                    this._physicalDeviceMediator.Attach<RazerKeyboardAdapter>(0, 1);
                    this._physicalDeviceMediator.Attach<RazerMousepadAdapter>(0, 0);
                    this._physicalDeviceMediator.Attach<RazerMouseAdapter>(23, 1);
                    this._physicalDeviceMediator.Attach<RazerHeadsetAdapter>(25, 7);
                    this._physicalDeviceMediator.Attach<RazerChromaLinkAdapter>(12, 9);
                    this._physicalDeviceMediator.Attach<AsusRogStrix_G15_2021_Adapter>(0, 0);
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
                    await this._physicalDeviceMediator.ApplyAsync(cancellationToken);
                }
            }

            public void SDKDisable()
            {
                this._virtualGrid.Set(ChromaColor.Black);
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

            public void LoadColor(Image image, Color color1, Color color2)
            {

                var width = this._virtualGrid.ColumnCount;
                var height = this._virtualGrid.RowCount;

                using var source = (Bitmap)ImageProcessing.Cut((Bitmap)image, width, height);
                using var resizedImage = (Bitmap)source.Resize(width, height);
                this._albumBackgroundSource = resizedImage.GetPixels().ToChromaColors();
                LoadColor(color1, color2);
            }

            internal void SetEffect(IChromaEffect effect, double[] spectrumValues, double playingPosition)
            {
                effect.SetEffect(this._virtualGrid, this._primaryColor, this._secondaryColor, this._albumColors, this._albumBackgroundSource, spectrumValues, playingPosition, _settings.RgbRenderBackgroundMultiplier / 100.0);
            }

            private bool disposed = false;
            protected void Dispose(bool disposing)
            {
                if (this.disposed)
                    return;

                if (disposing)
                {
                    this._physicalDeviceMediator?.Dispose();
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