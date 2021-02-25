using Colore.Effects.Virtual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColoreColor = Colore.Data.Color;

namespace Listener.Plugin.Razer.Interfaces
{
    public interface IRazerEffect
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
                       ColoreColor firstColor,
                       ColoreColor secondaryColor,
                       ICollection<ColoreColor> albumColor,
                       ColoreColor[][] albumArtworkColor,
                       double[] spectrumValues,
                       double position,
                       double brightnessMultiplier);
    }
}
