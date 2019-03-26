using IF.Lastfm.Core.Api;
using System;
using System.Threading.Tasks;

namespace SpotifyListener
{
    internal class LastFMApi
    {
        internal static async Task<string> GetImageUrlByInfoAsync(string album, string artist, string track)
        {
            try
            {
                var client = new LastfmClient(Properties.Settings.Default.LastFMApiKey, Properties.Settings.Default.LastFMApiSecret);
                var response = await client.Album.GetInfoAsync(artist, album, true);
                return response.Content.Images.ExtraLarge.ToString();
            }
            catch
            {
                return string.Empty;//
            }
        }
    }
}