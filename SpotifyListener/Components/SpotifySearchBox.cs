using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SpotifyListener
{
    public class SpotifySearchBox : AutoCompleteBox
    {
        private static readonly SolidColorBrush Active = new SolidColorBrush(Colors.SlateGray) { Opacity = 0.1f };
        private static readonly SolidColorBrush Inactive = new SolidColorBrush(Colors.SlateGray) { Opacity = 0.01 };
        public SpotifySearchBox()
        {
            //var border = new Border();
            //border.BorderBrush = System.Windows.Media.Brushes.Black;
            //border.BorderThickness = new Thickness(2, 2, 2, 2);
            //border.CornerRadius = new CornerRadius(8, 8, 8, 8);
            this.BorderThickness = new Thickness(0, 0, 0, 0);
            this.Foreground = new SolidColorBrush(Colors.White);
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            this.Background = Active;
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.Background = Inactive;
            base.OnMouseLeave(e);
        }
        private Dictionary<string, (SpotifyAPI.Web.Enums.SearchType, string)> _dict { get; set; }
        protected override void OnTextChanged(RoutedEventArgs e)
        {

            base.OnTextChanged(e);
        }
        private string GetKey(FullTrack x)
        {
            return $@"{x.Name} - {x.Album.Name} by {x.Artists.FirstOrDefault()?.Name}";
        }
        public void SetInternalValue(IEnumerable<(string track, SpotifyAPI.Web.Enums.SearchType searchType, string uri)> tracks)
        {
            var dict = new Dictionary<string, (SpotifyAPI.Web.Enums.SearchType, string)>();
            foreach (var track in tracks)
            {
                var key = track.track;
                if (!dict.ContainsKey(key))
                {
                    dict.Add(key, (track.searchType, track.uri));
                }
            }
            this.ItemsSource = dict.Keys;
            _dict = dict;
        }
        public (SpotifyAPI.Web.Enums.SearchType searchType, string uri) GetTrackUrl(string trackName)
        {
            if (_dict.TryGetValue(trackName, out var result))
            {
                return result;
            }
            return (SpotifyAPI.Web.Enums.SearchType.All, string.Empty);
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
        }
    }
}
