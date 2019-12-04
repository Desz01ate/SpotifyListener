using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SpotifyListener
{
    public class PathButton : Button
    {
        static DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(Geometry), typeof(PathButton), new FrameworkPropertyMetadata(new PropertyChangedCallback(Data_Changed)));
        public Brush InactiveColor { get; set; }
        public Brush ActiveColor { get; set; }
        public PathButton()
        {
            ActiveColor = new SolidColorBrush(Colors.Gold);
            InactiveColor = new SolidColorBrush(Colors.White);
        }
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
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (this.Content is Path path)
            {
                path.StrokeThickness = 1;
                path.Stroke = ActiveColor;
            }
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (this.Content is Path path)
            {
                path.StrokeThickness = 0;
                path.Stroke = InactiveColor;
            }
        }
        private void SetData()
        {
            Path path = new Path
            {
                Data = Data,
                Stretch = Stretch.Uniform,
                Stroke = InactiveColor,
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
