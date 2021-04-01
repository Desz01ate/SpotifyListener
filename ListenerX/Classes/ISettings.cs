using ListenerX.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Classes
{
    public interface ISettings
    {
        bool EnableArtworkWallpaper { get; }
        string ActivePlayerModule { get; }
        bool EnableRgbRender { get; }
        uint RgbRenderFps { get; }
        int RgbRenderStyle { get; }
        float RgbRenderAmplitude { get; }
        float RgbRenderBackgroundMultiplier { get; }
        bool RgbRenderAverageSpectrum { get; }
        ScalingStrategy RgbRenderScalingStrategy { get; }
    }
}
