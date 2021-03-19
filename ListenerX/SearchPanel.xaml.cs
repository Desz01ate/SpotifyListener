using Listener.Core.Framework.Players;
using ListenerX.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SearchType = Listener.Core.Framework.Events.SearchType;

namespace ListenerX
{
    /// <summary>
    /// Interaction logic for SearchPanel.xaml
    /// </summary>
    public partial class SearchPanel : Window
    {
        private readonly IStreamablePlayerHost Player;
        private readonly Geometry playPath;
        private readonly Action Callback;
        public SearchPanel(IStreamablePlayerHost player, Action callback = null)
        {
            Player = player;
            InitializeComponent();
            this.cb_searchBox.Focus();
            Callback = callback;
            playPath = (Geometry)this.Resources["playPath"];
            this.cb_searchBox.TextChanged += async (s, e) =>
            {
                var q = cb_searchBox.Text;
                string query = default;
                SearchType searchType = SearchType.All;
                if (q.Contains(":"))
                {
                    var data = q.Split(':');
                    var qtype = data[0].ToLower();
                    if (qtype == "t" || qtype == "track")
                    {
                        searchType = SearchType.Track;
                    }
                    else if (qtype == "ab" || qtype == "album")
                    {
                        searchType = SearchType.Album;
                    }
                    else if (qtype == "a" || qtype == "artist")
                    {
                        searchType = SearchType.Artist;
                    }
                    else if (qtype == "p" || qtype == "playlist")
                    {
                        searchType = SearchType.Playlist;
                    }
                    query = data[1];
                }
                else
                {
                    query = q;
                }
                if (string.IsNullOrWhiteSpace(q) || string.IsNullOrWhiteSpace(query))
                {
                    this.Height = 300;
                    grid_searchResult.Children.Clear();
                    return;
                }
                var result = (await Player.SearchAsync(query, searchType, 10)).ToArray();
                if (result == null) return;
                do
                {
                    grid_searchResult.Children.Clear();

                } while (grid_searchResult.Children.Count > 0);
                for (var i = 0; i < result.Count(); i++)
                {
                    var element = result[i];
                    var button = new PathButton();
                    button.Data = playPath;
                    button.Margin = new Thickness(10, (i + 1) * 50 - 2, 0, 0);
                    button.Width = 20;
                    button.Height = 20;
                    button.HorizontalAlignment = HorizontalAlignment.Left;
                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.Background = Brushes.Transparent;
                    button.BorderBrush = Brushes.Transparent;
                    button.BorderThickness = new Thickness(0, 0, 0, 0);
                    button.Fill = Brushes.Gray;
                    button.ActiveColor = Brushes.Green;
                    button.InactiveColor = Brushes.Gray;
                    button.Style = (Style)this.Resources["PathButtonStyle"];
                    button.Click += async delegate
                    {
                        switch (searchType)
                        {
                            case SearchType.Artist:
                                OpenerHelpers.Open(element.Uri.AbsoluteUri);
                                break;
                            case SearchType.Track:
                            case SearchType.All:
                                await Player.PlayTrackAsync(element.Uri.AbsoluteUri);
                                break;
                            default:
                                await Player.PlayAsync(element.Uri.AbsoluteUri);
                                break;
                        }
                    };
                    grid_searchResult.Children.Add(button);
                    var text = new TextBlock();
                    text.Text = element.Track;
                    text.Margin = new Thickness(50, (i + 1) * 50, 0, 0);
                    text.Width = 1920;
                    text.HorizontalAlignment = HorizontalAlignment.Left;
                    grid_searchResult.Children.Add(text);
                }
                if (grid_searchResult.Children.Count == 0)
                {
                    this.Height = 300;
                }
                else
                {
                    this.Height = grid_searchResult.Children.Count * (20) + 50;
                }
                //cb_searchBox.SetInternalValue(result);
            };
            this.Closing += delegate
            {
                Callback?.Invoke();
            };
        }
    }
}
