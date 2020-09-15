using SpotifyListener.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotifyListener
{
    /// <summary>
    /// Interaction logic for LyricsDisplay.xaml
    /// </summary>
    public partial class LyricsDisplay : Window
    {
        private readonly IMusic music;
        public LyricsDisplay(IMusic music)
        {
            InitializeComponent();
            this.music = music;
            this.DataContext = this.music;
            //this.Title = $"{track} by {artist}";
            //this.txt_Lyrics.Document.Blocks.Add(new Paragraph(new Run(lyrics)));
        }
    }
}
