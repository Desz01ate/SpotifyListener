using Listener.Core.Framework.Players;
using System;
using System.Windows;

namespace SpotifyListener
{
    /// <summary>
    /// Interaction logic for LyricsDisplay.xaml
    /// </summary>
    public partial class LyricsDisplay : Window
    {
        private readonly IPlayerHost music;
        private readonly Action callback;
        public LyricsDisplay(IPlayerHost music, double x, double y, Action callback = null)
        {
            this.callback = callback;
            this.Closing += delegate
            {
                callback?.Invoke();
            };

            InitializeComponent();
            this.music = music;
            this.DataContext = this.music;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = x;
            this.Top = y;
            //this.Title = $"{track} by {artist}";
            //this.txt_Lyrics.Document.Blocks.Add(new Paragraph(new Run(lyrics)));
        }
    }
}
