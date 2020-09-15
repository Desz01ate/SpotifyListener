using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpotifyListener.Classes
{
    public static class LyricsHelpers
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly Func<string, string> replacer = (x) => Regex.Replace(x, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled).ToLower();
        public static async ValueTask<string> GetLyricsAsync(string artist, string song)
        {
            artist = replacer(artist);
            song = replacer(song);
            if (CacheFileManager.TryGetFileCache($"{artist}{song}", out var fs))
            {
                using (fs)
                {
                    using var stream = new StreamReader(fs);
                    return await stream.ReadToEndAsync();
                }
            }
            var url = $"https://www.azlyrics.com/lyrics/{artist}/{song}.html";
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return "";

            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var lyrics = doc.DocumentNode.SelectNodes("//div").OrderByDescending(x => x.InnerText.Length).Select(x => x.InnerText).FirstOrDefault();
            lyrics = lyrics.Trim();
            CacheFileManager.SaveCache($"{artist}{song}", lyrics);
            return lyrics ?? "";
        }
    }
}
