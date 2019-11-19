using SpotifyAPI.Web.Models;
using SpotifyListener.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener.DatabaseManager.Models
{
    public class ListenHistory
    {
        public string Track { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public DateTime PlayOn { get; set; }
        public int ListeningSeconds { get; set; }
        [Obsolete("Preserve for table creation only.")]
        public ListenHistory() { }
        public ListenHistory(IMusic track)
        {
            this.Track = track.Track;
            this.Album = track.Album;
            this.Artist = track.Artist;
            this.Genre = track.Genre;
            this.Type = track.Type;
            this.Url = track.URL;
            this.PlayOn = DateTime.Now;
        }
    }
}
