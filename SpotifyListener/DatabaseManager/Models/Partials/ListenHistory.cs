using System;
namespace SpotifyListener.DatabaseManager.Models
{
    //You can get Utilities package via nuget : Install-Package Deszolate.Utilities.Lite
    //[Utilities.Attributes.SQL.Table("ListenHistory")]
    public partial class ListenHistory
    {
        public ListenHistory(SpotifyPlayer player)
        {
            this.Track = player.Track;
            this.Album = player.Album;
            this.Artist = player.Artist;
            this.Genre = player.Genre;
            this.Type = player.Type;
            this.Url = player.URL;
            this.PlayOn = DateTime.Now;
        }
        public ListenHistory()
        {

        }
    }
}

