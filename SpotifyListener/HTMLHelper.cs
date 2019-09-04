using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpotifyListener
{
    static class HTMLHelper
    {
        public static Dictionary<string, string> UrlMemoized { get; set; } = new Dictionary<string, string>();
        public static string Image_source => "<img.+?src=[\"'](.+?)[\"'].+?>";
        private static async Task<string> HTMLAgilityPackParserAsync(string searchingKeyword, string XPath, params string[] searchingContent)
        {
            var url = $"https://www.google.co.th/search?q={searchingKeyword.Replace(' ', '+').Replace('&', '-')}";
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(url);
                var input = System.Text.Encoding.UTF8.GetString(response);
                var doc = new HtmlDocument();
                byte[] byteArray = Encoding.ASCII.GetBytes(input);
                var ts = new MemoryStream(byteArray);
                doc.Load(ts);
                var root = doc.DocumentNode;
                var hrefs = root.SelectNodes(XPath).Select(p => p.GetAttributeValue("href", "not found"));
                Parallel.ForEach(hrefs, (href, state) =>
                {
                    bool valid = true;
                    for (var i = 0; i < searchingContent.Length; i++)
                    {
                        if (!href.Contains(searchingContent[i]))
                        {
                            valid = false;
                            break;
                        }
                        if (href.Contains("webcache.googleusercontent.com"))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid)
                    {
                        url = href;
                        state.Stop();
                    }
                });
                return url;
            }
            catch
            {
                throw new Exception("Raised by HTMLHelper.HTMLAgilityPackParser");
            }
        }
        private static async Task<string> ReFormatAsync(string href)
        {
            try
            {
                var baseString = @"https://itunes.apple.com/th";
                baseString += href.Substring(href.IndexOf(@"/album/"));
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(baseString);
                var source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                if (source.Contains("We are unable to find iTunes on your computer"))
                    return href;
                return baseString;
            }
            catch
            {
                return href;
            }
        }
        private static async Task<List<string>> GetImagesUrlAsync(string searchingKeyword, string regexPattern = "<img.+?src=[\"'](.+?)[\"'].+?>")
        {
            var url = $@"https://www.google.co.th/search?q={searchingKeyword.Replace(' ', '+').Replace('&', '-')}&source=lnms&tbm=isch&sa=X&ved=0ahUKEwjl0-jRz8zZAhUIjJQKHRhpAwQQ_AUICigB&biw=500&bih=500";
            var ResultList = new List<string>();
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(url);
                var source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                var result = Regex.Matches(source, regexPattern, RegexOptions.IgnoreCase);
                for (var i = 1; i < result.Count; i++) //skip index 0, it's checksum
                {
                    ResultList.Add(result[i].Groups[1].Value); //get src group from img
                }
            }
            catch
            {
                throw new Exception();
            }
            return ResultList;
        }
        public static async Task<string> GetMusicURLAsync(string name, string album, string artist)
        {
            if (UrlMemoized.TryGetValue(album, out string lookup))
                return lookup;
            var url = await HTMLAgilityPackParserAsync($"{album} {artist} itunes.apple.com/th", "//a[contains(@href,'album')]", "itunes.apple.com");
            url = UrlCleaning(url);
            if (!url.Contains(@"/th/"))
                url = await ReFormatAsync(url);
            try
            {
                UrlMemoized.Add(album, url);
            }
            catch
            {

            }
            return url;
        }
        public static async Task<Image> GetImage(string track, string album, string artist, string path, bool networkAllow)
        {
            try
            {
                //\u0E00-\u0E7F is unicode for Thai language
                if (!File.Exists(path))
                {
                    if (networkAllow)
                    {
                        var images = await LastFMApi.GetImageUrlByInfoAsync(album, artist, track);
                        if (images == string.Empty)
                            images = (await GetImagesUrlAsync($"{track} {album} {artist} album artwork -youtube", Image_source))[0];
                        var client = new HttpClient();
                        var byteArray = await client.GetByteArrayAsync(images);
                        Image image = (Image)((new ImageConverter()).ConvertFrom(byteArray));
                        image.Save(path);
                        image.Dispose();
                        client.Dispose();
                        images = null;
                    }
                    else
                    {
                        var r = new Random();
                        var bitmap = new Bitmap(1, 1);
                        bitmap.SetPixel(0, 0, Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)));
                        return bitmap;
                    }
                }
                var result = new MemoryStream(File.ReadAllBytes(path));
                return Image.FromStream(result);
                //can't release this stream manual, causing another GDI+ processing to be error. Let's the GC handle this alone
            }
            catch
            {
                var r = new Random();
                var bitmap = new Bitmap(1, 1);
                bitmap.SetPixel(0, 0, Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)));
                return bitmap;
            }
        }
        public static async Task<Image> GetImage(string track, string album, string artist)
        {
            try
            {
                //\u0E00-\u0E7F is unicode for Thai language

                var imageURL = await LastFMApi.GetImageUrlByInfoAsync(album, artist, track);
                if (imageURL == string.Empty)
                    imageURL = (await GetImagesUrlAsync($"{track} {album} {artist} album artwork -youtube", Image_source))[0];
                using (var client = new HttpClient())
                {
                    var byteArray = await client.GetByteArrayAsync(imageURL);
                    using (Image image = (Image)((new ImageConverter()).ConvertFrom(byteArray)))
                    {
                        return image;
                    }
                }
                //can't release this stream manual, causing another GDI+ processing to be error. Let's the GC handle this alone
            }
            catch
            {
                var r = new Random();
                using (var bitmap = new Bitmap(1, 1))
                {
                    bitmap.SetPixel(0, 0, Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)));
                    return bitmap;
                }
            }
        }
        private static string UrlCleaning(string url)
        {
            var httpIndex = url.IndexOf("h");
            var slashIndex = Extension.GetIndexOfAt(url, '/', 7);
            var afterSlash = url.Substring(slashIndex + 1);
            var songDigit = Extension.GetOnlyDigit(afterSlash);
            url = url.Substring(httpIndex, slashIndex - url.Substring(0, httpIndex).Length + 1) + songDigit;
            return url;
        }
    }
}
