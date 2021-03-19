using Colore;
using Colore.Effects.ChromaLink;
using Colore.Effects.Headset;
using Colore.Effects.Keyboard;
using Colore.Effects.Mouse;
using Colore.Effects.Mousepad;
using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualGrid.Enums;
using VirtualGrid.Interfaces;

namespace VirtualGrid.Razer
{
    /// <summary>
    /// An adapter to convert <see cref="IVirtualLedGrid"/> into Razer-compatible effects.
    /// </summary>
    public class RazerAdapter : IPhysicalDeviceAdapter
    {
        private readonly IChroma _chromaInterface;

        private static IPhysicalDeviceAdapter _adapter;
        /// <summary>
        /// An instance of Razer Adapter.
        /// </summary>
        public static IPhysicalDeviceAdapter Instance = _adapter ??= new RazerAdapter();

        private RazerAdapter()
        {
            this._chromaInterface = ColoreProvider.CreateNativeAsync().Result;
        }

        public async Task ApplyAsync(IVirtualLedGrid virtualGrid, CancellationToken cancellationToken = default)
        {
            var keyboardGrid = CustomKeyboardEffect.Create();
            var mouseGrid = CustomMouseEffect.Create();
            var mousepadGrid = CustomMousepadEffect.Create();
            var headsetGrid = CustomHeadsetEffect.Create();
            var chromaLinkGrid = CustomChromaLinkEffect.Create();
            foreach (var k in virtualGrid)
            {
                switch (k.Type)
                {
                    case KeyType.Invalid:
                        break;
                    case KeyType.Keyboard:
                        var kbVal = (Key)Enum.Parse(typeof(Key), k.FriendlyName);
                        keyboardGrid[kbVal] = ToColoreColor(k.Color);
                        break;
                    case KeyType.Mouse:
                        var mouseVal = (GridLed)Enum.Parse(typeof(GridLed), k.FriendlyName);
                        mouseGrid[mouseVal] = ToColoreColor(k.Color);
                        break;
                    case KeyType.Mousepad:
                        mousepadGrid[k.KeyCode] = ToColoreColor(k.Color);
                        break;
                    case KeyType.Headset:
                        headsetGrid[k.KeyCode] = ToColoreColor(k.Color);
                        break;
                    case KeyType.ExtraDevice:
                        chromaLinkGrid[k.KeyCode] = ToColoreColor(k.Color);
                        break;
                }
            }

            await this._chromaInterface.Keyboard.SetCustomAsync(keyboardGrid);
            await this._chromaInterface.Mouse.SetGridAsync(mouseGrid);
            await this._chromaInterface.Mousepad.SetCustomAsync(mousepadGrid);
            await this._chromaInterface.Headset.SetCustomAsync(headsetGrid);
            await this._chromaInterface.ChromaLink.SetCustomAsync(chromaLinkGrid);
        }

        private static Colore.Data.Color ToColoreColor(Color color)
        {
            return new Colore.Data.Color(color.Value);
        }

        public void Dispose()
        {
            this._chromaInterface?.Dispose();
        }
    }
}
