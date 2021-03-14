using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AuraServiceLib;
using Listener.Plugin.ChromaEffect.Enums;
using Listener.Plugin.ChromaEffect.Interfaces;

namespace ListenerX.Classes.Adapter
{
    enum RogStrixKeyboardMap
    {
        GhostKey = -1,
        Invalid0_1 = 0,
        Invalid0_2 = 1,
        VolumeDown = 2,
        VolumeUp = 3,
        MicMute = 4,
        FanToggle = 5,
        RogKey = 6,
        Invalid0_3 = 7,
        Invalid0_4 = 8,
        Invalid0_5 = 9,
        Invalid0_6 = 10,
        Invalid0_7 = 11,
        Invalid0_8 = 12,
        Invalid0_9 = 13,
        Invalid0_10 = 14,
        Invalid0_11 = 15,
        Invalid0_12 = 16,
        Invalid0_13 = 17,
        Invalid0_14 = 18,
        Invalid0_15 = 19,
        Invalid0_16 = 20,
        Escape = 21,
        Invalid1_1 = 22,
        F1 = 23,
        F2 = 24,
        F3 = 25,
        F4 = 26,
        Invalid1_2 = 27,
        F5 = 28,
        F6 = 29,
        F7 = 30,
        F8 = 31,
        Invalid1_3 = 32,
        F9 = 33,
        F10 = 34,
        F11 = 35,
        F12 = 36,
        DeleteInsert = 37,
        Invalid1_4 = 38,
        Invalid1_5 = 39,
        Invalid1_6 = 40,
        Invalid1_7 = 41,
        TildeGraveAccent = 42,
        D1 = 43,
        D2 = 44,
        D3 = 45,
        D4 = 46,
        D5 = 47,
        D6 = 48,
        D7 = 49,
        D8 = 50,
        D9 = 51,
        D0 = 52,
        OemMinus = 53,
        OemEquals = 54,
        BackspaceLed0 = 55,
        BackspaceLed1 = 56,
        BackspaceLed2 = 57,
        MultimediaPlay = 58,
        Invalid2_1 = 59,
        Invalid2_2 = 60,
        Invalid2_3 = 61,
        Invalid2_4 = 62,
        Tab = 63,
        Q = 64,
        W = 65,
        E = 66,
        R = 67,
        T = 68,
        Y = 69,
        U = 70,
        I = 71,
        O = 72,
        P = 73,
        BracketLeft = 74,
        BracketRight = 75,
        OemBackslash = 76,
        Invalid3_1 = 77,
        Invalid3_2 = 78,
        MultimediaPause = 79,
        Invalid3_3 = 80,
        Invalid3_4 = 81,
        Invalid3_5 = 82,
        Invalid3_6 = 83,
        CapsLock = 84,
        A = 85,
        S = 86,
        D = 87,
        F = 88,
        G = 89,
        H = 90,
        J = 91,
        K = 92,
        L = 93,
        OemSemicolon = 94,
        OemApostrophe = 95,
        Invalid4_1 = 96,
        OemEnterLed0 = 97,
        OemEnterLed1 = 98,
        OemEnterLed2 = 99,
        MultimediaPrevious = 100,
        Invalid4_2 = 101,
        Invalid4_3 = 102,
        Invalid4_4 = 103,
        Invalid4_5 = 104,
        LeftShift = 105,
        Invalid5_1 = 106,
        Z = 107,
        X = 108,
        C = 109,
        V = 110,
        B = 111,
        N = 112,
        M = 113,
        OemComma = 114,
        OemPeriod = 115,
        OemSlash = 116,
        Invalid5_2 = 117,
        RightShiftLed0 = 118,
        RightShiftLed1 = 119,
        RightShiftLed2 = 120,
        MultimediaNext = 121,
        Invalid5_3 = 122,
        Invalid5_4 = 123,
        Invalid5_5 = 124,
        Invalid5_6 = 125,
        LeftCtrl = 126,
        OemFunction = 127,
        LeftWindows = 128,
        LeftAlt = 129,
        SpaceLed0 = 130,
        SpaceLed1 = 131,
        SpaceLed2 = 132,
        SpaceLed3 = 133,

        Invalid6_1 = 134,
        RightAlt = 135,
        Invalid6_2 = 136,
        RightCtrl = 137,
        Invalid6_3 = 138,
        ArrowUp = 139,
        Invalid6_4 = 140,
        Invalid6_5 = 141,
        PrintScreen = 142,
        Invalid6_6 = 143,
        Invalid6_7 = 144,
        Invalid6_8 = 145,
        Invalid6_9 = 146,

        Invalid7_1 = 147,
        Invalid7_2 = 148,
        Invalid7_3 = 149,
        Invalid7_4 = 150,
        Invalid7_5 = 151,
        Invalid7_6 = 152,
        Invalid7_7 = 153,
        Invalid7_8 = 154,
        Invalid7_9 = 155,
        Invalid7_10 = 156,
        Invalid7_11 = 157,
        Invalid7_12 = 158,

        ArrowLeft = 159,
        ArrowDown = 160,
        ArrowRight = 161,

        Invalid7_13 = 162,
        Invalid7_14 = 163,
        Invalid7_15 = 164,
        Invalid7_16 = 165,
        Invalid7_17 = 166,

        Invalid8_1 = 167,
        Invalid8_2 = 168,
        LedBar5 = 169,
        LedBar4 = 170,
        LedBar3 = 171,
        LedBar2 = 172,
        LedBar1 = 173,
        LedBar0 = 174,
        Invalid8_9 = 175,
        Invalid8_10 = 176,
        Invalid8_11 = 177,
        Invalid8_12 = 178,
        Invalid8_13 = 179,
        Invalid8_14 = 180,
        Invalid8_15 = 181,
        Invalid8_16 = 182,
        Invalid8_17 = 183,
        Invalid8_18 = 184,
        Invalid8_19 = 185,
        Invalid8_20 = 186,
    }

    public class AsusSdkAdapter : IPhysicalDeviceAdapter
    {
        private readonly IAuraSdk2 _sdk;
        private readonly IAuraSyncDevice _notebookKeyboard;
        public AsusSdkAdapter()
        {
            var sdk = (IAuraSdk2)new AuraSdk();
            sdk.SwitchMode();
            var devices = sdk.Enumerate(528384);
            this._sdk = sdk;
            this._notebookKeyboard = devices.Count > 0 ? devices[0] : null;
        }

        public async Task ApplyAsync(IVirtualLedGrid virtualGrid, CancellationToken cancellationToken = default)
        {
            if (_sdk == null || this._notebookKeyboard == null)
                return;

            foreach (var key in virtualGrid.Where(x => x.Type != KeyType.Headset))
            {
                var color = ToRGBNetColor(key.Color);
                switch (key.FriendlyName)
                {
                    case "Enter":
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.OemEnterLed0].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.OemEnterLed1].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.OemEnterLed2].Color = color;
                        break;
                    case "RightShift":
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.RightShiftLed0].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.RightShiftLed1].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.RightShiftLed2].Color = color;
                        break;
                    case "Backspace":
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.BackspaceLed0].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.BackspaceLed1].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.BackspaceLed2].Color = color;
                        break;
                    case "Space":
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.SpaceLed0].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.SpaceLed1].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.SpaceLed2].Color = color;
                        _notebookKeyboard.Lights[(int)RogStrixKeyboardMap.SpaceLed3].Color = color;
                        break;
                    default:
                        var asusKey = KeyConvert(key.FriendlyName);
                        if (asusKey == RogStrixKeyboardMap.GhostKey)
                            continue;

                        var led = _notebookKeyboard.Lights[(int)asusKey];
                        if (led == null)
                            continue;
                        led.Color = color;
                        break;
                }

            }
            _notebookKeyboard.Apply();
        }

        public void Dispose()
        {
            this._sdk?.ReleaseControl(0);
        }

        private static readonly IReadOnlyDictionary<string, RogStrixKeyboardMap> KeysMap = new Dictionary<string, RogStrixKeyboardMap>() {
{ "Z", RogStrixKeyboardMap.Z },
{ "Y", RogStrixKeyboardMap.Y },
{ "X", RogStrixKeyboardMap.X },
{ "W", RogStrixKeyboardMap.W },
{ "V", RogStrixKeyboardMap.V },
{ "Up", RogStrixKeyboardMap.ArrowUp },
{ "U", RogStrixKeyboardMap.U },
{ "Tab", RogStrixKeyboardMap.Tab },
{ "T", RogStrixKeyboardMap.T },
{ "Space", RogStrixKeyboardMap.SpaceLed0 },
{ "Scroll", RogStrixKeyboardMap.GhostKey },
{ "S", RogStrixKeyboardMap.S },
{ "RightShift", RogStrixKeyboardMap.RightShiftLed0 },
{ "RightMenu", RogStrixKeyboardMap.GhostKey },
{ "RightControl", RogStrixKeyboardMap.RightCtrl },
{ "RightAlt", RogStrixKeyboardMap.RightAlt },
{ "Right", RogStrixKeyboardMap.ArrowRight },
{ "R", RogStrixKeyboardMap.R },
{ "Q", RogStrixKeyboardMap.Q },
{ "PrintScreen", RogStrixKeyboardMap.GhostKey },
{ "Pause", RogStrixKeyboardMap.GhostKey },
{ "PageUp", RogStrixKeyboardMap.GhostKey },
{ "PageDown", RogStrixKeyboardMap.GhostKey },
{ "P", RogStrixKeyboardMap.P },
{ "OemTilde", RogStrixKeyboardMap.TildeGraveAccent },
{ "OemSlash", RogStrixKeyboardMap.OemSlash },
{ "OemSemicolon", RogStrixKeyboardMap.OemSemicolon },
{ "OemRightBracket", RogStrixKeyboardMap.BracketRight },
{ "OemPeriod", RogStrixKeyboardMap.OemPeriod },
{ "OemMinus", RogStrixKeyboardMap.OemMinus },
{ "OemLeftBracket", RogStrixKeyboardMap.BracketLeft },
{ "OemEquals", RogStrixKeyboardMap.OemEquals },
{ "OemComma", RogStrixKeyboardMap.OemComma },
{ "OemBackslash", RogStrixKeyboardMap.OemBackslash },
{ "OemApostrophe", RogStrixKeyboardMap.OemApostrophe },
{ "O", RogStrixKeyboardMap.O },
{ "N", RogStrixKeyboardMap.N },
{ "M", RogStrixKeyboardMap.M },
{ "Logo", RogStrixKeyboardMap.GhostKey },
{ "LeftShift", RogStrixKeyboardMap.LeftShift },
{ "Left", RogStrixKeyboardMap.ArrowLeft },
{ "L", RogStrixKeyboardMap.L },
{ "K", RogStrixKeyboardMap.K },
{ "J", RogStrixKeyboardMap.J },
{ "Insert", RogStrixKeyboardMap.GhostKey },
{ "I", RogStrixKeyboardMap.I },
{ "Home", RogStrixKeyboardMap.GhostKey },
{ "H", RogStrixKeyboardMap.H },
{ "G", RogStrixKeyboardMap.G },
{ "Function", RogStrixKeyboardMap.GhostKey },
{ "Macro5", RogStrixKeyboardMap.LeftCtrl },
{ "LeftControl", RogStrixKeyboardMap.OemFunction },
{ "LeftWindows", RogStrixKeyboardMap.LeftWindows },
{ "LeftAlt", RogStrixKeyboardMap.LeftAlt },
{ "F9", RogStrixKeyboardMap.F9 },
{ "F8", RogStrixKeyboardMap.F8 },
{ "F7", RogStrixKeyboardMap.F7 },
{ "F6", RogStrixKeyboardMap.F6 },
{ "F5", RogStrixKeyboardMap.F5 },
{ "F4", RogStrixKeyboardMap.F4 },
{ "F3", RogStrixKeyboardMap.F3 },
{ "F2", RogStrixKeyboardMap.F2 },
{ "F12", RogStrixKeyboardMap.F12 },
{ "F11", RogStrixKeyboardMap.F11 },
{ "F10", RogStrixKeyboardMap.F10 },
{ "F1", RogStrixKeyboardMap.F1 },
{ "F", RogStrixKeyboardMap.F },
{ "Escape", RogStrixKeyboardMap.Escape },
{ "Enter", RogStrixKeyboardMap.OemEnterLed0 },
{ "End", RogStrixKeyboardMap.GhostKey },
{ "E", RogStrixKeyboardMap.E },
{ "Down", RogStrixKeyboardMap.ArrowDown },
{ "Delete", RogStrixKeyboardMap.GhostKey },
{ "D9", RogStrixKeyboardMap.D9 },
{ "D8", RogStrixKeyboardMap.D8 },
{ "D7", RogStrixKeyboardMap.D7 },
{ "D6", RogStrixKeyboardMap.D6 },
{ "D5", RogStrixKeyboardMap.D5 },
{ "D4", RogStrixKeyboardMap.D4 },
{ "D3", RogStrixKeyboardMap.D3 },
{ "D2", RogStrixKeyboardMap.D2 },
{ "D1", RogStrixKeyboardMap.D1 },
{ "D0", RogStrixKeyboardMap.D0 },
{ "D", RogStrixKeyboardMap.D },
{ "CapsLock", RogStrixKeyboardMap.CapsLock },
{ "C", RogStrixKeyboardMap.C },
{ "Backspace", RogStrixKeyboardMap.BackspaceLed0 },
{ "B", RogStrixKeyboardMap.B },
{ "A", RogStrixKeyboardMap.A },
{ "LinkLed4", RogStrixKeyboardMap.LedBar5 },
{ "LinkLed3", RogStrixKeyboardMap.LedBar4 },
{ "LinkLed2", RogStrixKeyboardMap.LedBar2 },
{ "LinkLed1", RogStrixKeyboardMap.LedBar1 },
{ "LinkLed0", RogStrixKeyboardMap.LedBar0 },
            { "LeftSide2", RogStrixKeyboardMap.DeleteInsert },
            { "LeftSide3", RogStrixKeyboardMap.MultimediaPlay },
            { "LeftSide4", RogStrixKeyboardMap.MultimediaPause },
            { "LeftSide5", RogStrixKeyboardMap.MultimediaPrevious },
            { "LeftSide6", RogStrixKeyboardMap.MultimediaNext },
            { "LeftSide7", RogStrixKeyboardMap.PrintScreen },
            { "Led3", RogStrixKeyboardMap.VolumeDown },
            { "Led4", RogStrixKeyboardMap.VolumeUp },
            { "Led5", RogStrixKeyboardMap.MicMute },
            { "Led6", RogStrixKeyboardMap.FanToggle },
            { "Led7", RogStrixKeyboardMap.RogKey }


        };

        private static RogStrixKeyboardMap KeyConvert(string name)
        {
            if (KeysMap.TryGetValue(name, out var v))
                return (RogStrixKeyboardMap)v;
            return RogStrixKeyboardMap.GhostKey;
        }


        private static uint ToRGBNetColor(Listener.Plugin.ChromaEffect.Implementation.Color color)
        {
            return Listener.ImageProcessing.ColorProcessing.ToUint(color.B, color.G, color.R);
        }
    }
}
