using System.Collections.Generic;
using VirtualGrid;
using VirtualGrid.Interfaces;

namespace Listener.Plugin.ChromaEffect.Interfaces
{
    public interface IChromaEffect
    {
        /// <summary>
        /// Effect name description.
        /// </summary>
        string EffectName { get; }

        /// <summary>
        /// Specified how much spectrum range should be supply.
        /// </summary>
        int RequiredSpectrumRange { get; }

        /// <summary>
        /// Set effect to virtual grid.
        /// </summary>
        /// <param name="virtualGrid">Virtual grid sent by client.</param>
        /// <param name="albumColor">Current album color.</param>
        /// <param name="spectrumValues">Current spectrum values.</param>
        /// <param name="position">Current playing position.</param>
        void SetEffect(IVirtualLedGrid virtualGrid,
                       Color firstColor,
                       Color secondaryColor,
                       ICollection<Color> albumColor,
                       Color[][] albumArtworkColor,
                       double[] spectrumValues,
                       double position,
                       double brightnessMultiplier);
    }
}
