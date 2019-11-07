using SpotifyListener.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ColoreColor = Colore.Data.Color;

namespace SpotifyListener
{
    static class Extension
    {
        public static double CalculateRelativeValue(this ProgressBar control)
        {
            double absolutePosition = Mouse.GetPosition(control).X / control.ActualWidth;
            double relativePosition = absolutePosition * control.Maximum;
            return relativePosition;
        }
        public static string ImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "AlbumImages");

        public static string TruncateString(string s)
        {
            var n = Encoding.Unicode.GetByteCount(s);
            if (n <= 127) return s;
            s = s.Substring(0, 64);

            while (Encoding.Unicode.GetByteCount(s) > 123)
                s = s.Substring(0, s.Length - 1);

            return s + "...";
        }
        public static string RenderString(string template, IMusic track)
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
            var index = 0;
            var count = 0;
            var stringArray = baseString.ToArray();
            for (var i = 0; i < stringArray.Length; i++)
            {
                if (stringArray[i] == c)
                {
                    index = i;
                    count++;
                    if (count == occurrent)
                        return i;
                }
            }
            return -1;
        }
        public static void Restart(this Window context)
        {
            context.Close();
            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\SpotifyListener.exe");
        }

        public static void BringToFront(this FrameworkElement element)
        {
            if (element == null) return;

            if (!(element.Parent is Panel parent)) return;

            var maxZ = parent.Children.OfType<UIElement>()
              .Where(x => x != element)
              .Select(x => Panel.GetZIndex(x))
              .Max();
            Panel.SetZIndex(element, maxZ + 1);
        }
        public static T[] Slice<T>(this ReadOnlySpan<T> span, int startIndex, int length) => span.Slice(startIndex, length).ToArray();
        public static T[] Slice<T>(this T[] array, int startIndex, int length) => Slice<T>((ReadOnlySpan<T>)array, startIndex, length);
        public static T[] Add<T>(this T[] baseArray, T element)
        {
            Array.Resize(ref baseArray, baseArray.Length + 1);
            baseArray[baseArray.Length - 1] = element;
            return baseArray;
        }
    }
}
