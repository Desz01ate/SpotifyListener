using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using GeometryAbstract = System.Windows.Media.Geometry;
namespace ListenerX.Resources.Geometry
{
    public static class GeometryCollection
    {
        public static readonly GeometryAbstract PlayButton = GeometryAbstract.Parse(GeometryShapeConstants.PlayButtonShape);
        public static readonly GeometryAbstract PauseButton = GeometryAbstract.Parse(GeometryShapeConstants.PauseButtonShape);
        public static readonly GeometryAbstract BackButton = GeometryAbstract.Parse(GeometryShapeConstants.BackButtonShape);
        public static readonly GeometryAbstract NextButton = GeometryAbstract.Parse(GeometryShapeConstants.NextButtonShape);
        public static readonly GeometryAbstract ShuffleButton = GeometryAbstract.Parse(GeometryShapeConstants.ShuffleButtonShape);
        public static readonly GeometryAbstract VolumeButton = GeometryAbstract.Parse(GeometryShapeConstants.VolumeButtonShape);
        public static readonly GeometryAbstract SettingsButton = GeometryAbstract.Parse(GeometryShapeConstants.SettingsButtonShape);
        public static readonly GeometryAbstract DevicesButton = GeometryAbstract.Parse(GeometryShapeConstants.DevicesButtonShape);
        public static readonly GeometryAbstract SearchButton = GeometryAbstract.Parse(GeometryShapeConstants.SearchButtonShape);
        public static readonly GeometryAbstract LyricsButton = GeometryAbstract.Parse(GeometryShapeConstants.LyricsButtonShape);
        public static readonly GeometryAbstract MinimizeButton = GeometryAbstract.Parse(GeometryShapeConstants.MinimizeButtonShape);
        public static readonly GeometryAbstract CloseButton = GeometryAbstract.Parse(GeometryShapeConstants.CloseButtonShape);
    }
}
