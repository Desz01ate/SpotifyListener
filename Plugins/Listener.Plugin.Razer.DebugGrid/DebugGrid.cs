using Listener.Plugin.ChromaEffect.Interfaces;
using System.Collections.Generic;
using System.Linq;
using VirtualGrid;
using VirtualGrid.Enums;
using VirtualGrid.Interfaces;

namespace Listener.Plugin.Razer.DebugGrid
{
    public class DebugGrid : IChromaEffect
    {
        public string EffectName => "Debug Grid";

        public int RequiredSpectrumRange => 29;

        public void SetEffect(IVirtualLedGrid virtualGrid, Color firstColor, Color secondaryColor, ICollection<Color> albumColor, Color[][] albumArtworkColor, double[] spectrumValues, double position, double brightnessMultiplier)
        {
            virtualGrid.Set(Color.Black);

            var keyboardGrid = virtualGrid.Where(x => x.Type == KeyType.Keyboard);
            foreach (var key in keyboardGrid)
            {
                key.Color = Color.Red;
            }

            var mouseGrid = virtualGrid.Where(x => x.Type == KeyType.Mouse);
            foreach (var key in mouseGrid)
            {
                key.Color = Color.Blue;
            }

            var mousepadGrid = virtualGrid.Where(x => x.Type == KeyType.Mousepad);
            foreach (var key in mousepadGrid)
            {
                key.Color = Color.Purple;
            }

            var extraKeyGrid = virtualGrid.Where(x => x.Type == KeyType.ExtraDevice);
            foreach (var key in extraKeyGrid)
            {
                key.Color = Color.Green;
            }

            var headsetGrid = virtualGrid.Where(x => x.Type == KeyType.Headset);
            foreach (var key in headsetGrid)
            {
                key.Color = Color.White;
            }
        }
    }
}
