using System;
using System.Linq;
using System.Text;

namespace Listener.Utilities
{
    public static class Extensions
    {
        public static string TruncateString(string s)
        {
            var n = Encoding.Unicode.GetByteCount(s);
            if (n <= 127) return s;
            s = s.Substring(0, 64);

            while (Encoding.Unicode.GetByteCount(s) > 123)
                s = s.Substring(0, s.Length - 1);

            return s + "...";
        }
        public static string RenderString(string template, IPlayerHost track)
        {
            return template.Replace("%artist", track.Artist).Replace("%track", track.Track).Replace("%playlist_type", "").Replace("%playlist_name", track.Album).Replace("%album", track.Album);
        }
        public static string ToMinutes(this long elapsedMilliseconds)
        {
            var ts = TimeSpan.FromSeconds(elapsedMilliseconds);
            return string.Format("{0:0}:{1:00}", ts.Minutes, ts.Seconds);
        }
        public static string ToMinutes(this int elapsedMilliseconds)
        {
            var ts = TimeSpan.FromMilliseconds(elapsedMilliseconds);
            return string.Format("{0:0}:{1:00}", ts.Minutes, ts.Seconds);
        }
        public static object GetProgression(int scale, long position, long duration)
        {
            var percentage = (((double)position / duration) * (scale * 10)) / 10;
            var repeating = Math.Round(percentage, 0);
            return $"[{new string('-', (int)repeating > 0 ? (int)repeating - 1 : 0)}▓{new string('-', scale - (int)repeating)}]";
        }
        public static string UnknownLength_Substring(this string s, int length)
        {
            if (s.Length <= length)
                return s;
            return s.Substring(0, length);
        }
        public static object GetOnlyDigit(string afterSlash)
        {
            var digits = Enumerable.Range(0, 10).ToArray();
            var exception = new[] { "id" };
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < afterSlash.Length; i++)
            {
                var current = afterSlash[i].ToString();
                var count = false;
                for (var e = 0; e < exception.Length; e++)
                {
                    var exc = exception[e];
                    if (exc.Contains(current))
                    {
                        stringBuilder.Append(current);
                        count = true;
                        break;
                    }
                }
                for (var d = 0; d < digits.Length; d++)
                {
                    var digit = digits[d];
                    if (current == digit.ToString())
                    {
                        stringBuilder.Append(current);
                        count = true;
                        break;
                    }
                }
                if (!count)
                    return stringBuilder;
            }
            return string.Empty;
        }

        public static int GetIndexOfAt(this string baseString, char c, int occurrent)
        {
            var count = 0;
            var stringArray = baseString.ToArray();
            for (var i = 0; i < stringArray.Length; i++)
            {
                if (stringArray[i] == c)
                {
                    count++;
                    if (count == occurrent)
                        return i;
                }
            }
            return -1;
        }
    }
}
