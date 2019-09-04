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
