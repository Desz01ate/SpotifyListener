using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SpotifyListener
{
    public class PathButton : Button
    {
        public static DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(PathButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(Data_Changed)));
        public Brush Fill
        {
            get
            {
                return ((Path)Content).Fill;
            }
            set
            {
                ((Path)Content).Fill = value;
            }
        }
        public Geometry Data
        {
            get
            {
                return (Geometry)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        private static void Data_Changed(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            PathButton thisClass = (PathButton)o;
            thisClass.SetData();
        }
        private void SetData()
        {
            Path path = new Path
            {
                Data = Data,
                Stretch = Stretch.Uniform,
                Stroke = this.Foreground,
                StrokeThickness = 0
            };
            this.Content = path;
        }
        protected override void OnClick()
        {
            base.OnClick();
        }
    }
}
