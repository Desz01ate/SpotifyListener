using AuraServiceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtualGrid.Asus.Enums;
using VirtualGrid.Enums;
using VirtualGrid.Interfaces;

namespace VirtualGrid.Asus
{
    /// <summary>
    /// An adapter to convert <see cref="IVirtualLedGrid"/> into Asus Rog Strix G15 (2021) compatible effect.
    /// </summary>
    public class AsusRogStrix_G15_2021_Adapter : IPhysicalDeviceAdapter
    {
        private readonly IAuraSdk2 _sdk;
        private readonly IAuraSyncDevice _notebookKeyboard;

        private static IPhysicalDeviceAdapter _adapter;
        /// <summary>
        /// An instance of Razer Adapter.
        /// </summary>
        public static IPhysicalDeviceAdapter Instance = _adapter ??= new AsusRogStrix_G15_2021_Adapter();

        private AsusRogStrix_G15_2021_Adapter()
        {
            var sdk = (IAuraSdk2)new AuraSdk();
            sdk.SwitchMode();
            var devices = sdk.Enumerate(528384);
            this._sdk = sdk;
            this._notebookKeyboard = devices.Count > 0 ? devices[0] : null;
        }

        public Task ApplyAsync(IVirtualLedGrid virtualGrid, CancellationToken cancellationToken = default)
        {
            if (_sdk == null || this._notebookKeyboard == null)
                return Task.CompletedTask;

            foreach (var key in virtualGrid.Where(x => x.Type != KeyType.Headset))
            {
                var color = ToUint(key.Color);
                switch (key.FriendlyName)
                {
                    case "Enter":
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.OemEnterLed0].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.OemEnterLed1].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.OemEnterLed2].Color = color;
                        break;
                    case "RightShift":
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.RightShiftLed0].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.RightShiftLed1].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.RightShiftLed2].Color = color;
                        break;
                    case "Backspace":
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.BackspaceLed0].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.BackspaceLed1].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.BackspaceLed2].Color = color;
                        break;
                    case "Space":
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.SpaceLed0].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.SpaceLed1].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.SpaceLed2].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardKey.SpaceLed3].Color = color;
                        break;
                    default:
                        var asusKey = KeyConvert(key.FriendlyName);
                        if (asusKey == RogStrixKeyboardKey.GhostKey)
                            continue;

                        var led = _notebookKeyboard.Lights[(int)asusKey];
                        if (led == null)
                            continue;
                        led.Color = color;
                        break;
                }

            }
            _notebookKeyboard.Apply();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this._sdk?.ReleaseControl(0);
        }

        private static RogStrixKeyboardKey KeyConvert(string name)
        {
            if (Maps.RogStrix_G15_2021Map.TryGetValue(name, out var v))
                return (RogStrixKeyboardKey)v;
            return RogStrixKeyboardKey.GhostKey;
        }


        private static uint ToUint(Color color)
        {
            var value = (color.R << 16) | (color.G << 8) | (color.B);
            return (uint)value;
        }
    }
}
