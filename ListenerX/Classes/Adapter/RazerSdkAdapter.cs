using Colore;
using Colore.Data;
using Colore.Effects.ChromaLink;
using Colore.Effects.Headset;
using Colore.Effects.Keyboard;
using Colore.Effects.Mouse;
using Colore.Effects.Mousepad;
using Listener.Plugin.ChromaEffect.Enums;
using Listener.Plugin.ChromaEffect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListenerX.Classes.Adapter
{
    public class RazerSdkAdapter : IPhysicalDeviceAdapter
    {
        private readonly IChroma _chromaInterface;
        public RazerSdkAdapter()
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
                    case KeyType.ChromaLink:
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

        private static Color ToColoreColor(Listener.Plugin.ChromaEffect.Implementation.Color color)
        {
            return new Color(color.Value);
        }

        public void Dispose()
        {
            this._chromaInterface?.Dispose();
        }
    }
}
