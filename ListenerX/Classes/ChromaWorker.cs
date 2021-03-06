﻿using Listener.ImageProcessing;
using System;
using System.Linq;
using ListenerX.Classes;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using ListenerX.Extensions;
using ChromaColor = VirtualGrid.Color;
using Listener.Plugin.ChromaEffect.Interfaces;
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
            private readonly IArrangeMediator _physicalDeviceMediator;
            private readonly IVirtualLedGrid _virtualGrid;
            private readonly ISettings _settings;
            private AutoshiftCirculaQueue<ChromaColor> _albumColors;

            public readonly bool IsError;
            public ChromaWorker(IVirtualLedGrid virtualGrid, ISettings settings)
            {
                this._albumColors = AutoshiftCirculaQueue<ChromaColor>.Empty;
                this._virtualGrid = virtualGrid;
                this._settings = settings;
                try
                {
                    this._physicalDeviceMediator = new VirtualGrid.PhysicalDeviceMediator(this._virtualGrid);
                    this._physicalDeviceMediator.Attach<RazerAdapter>();
                    this._physicalDeviceMediator.Attach<AsusRogStrix_G15_2021_Adapter>();
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