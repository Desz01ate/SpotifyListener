using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener.Classes
{
    public static class CacheFileManager
    {
        private const string CACHE_DIR = "cache";
        static CacheFileManager()
        {
            if (!Directory.Exists(CACHE_DIR))
            {
                Directory.CreateDirectory(CACHE_DIR);
            }
        }
        public static bool IsFileExists(string fileName) => File.Exists(Path.Combine(CACHE_DIR, fileName));

        public static FileStream GetFileCache(string fileName)
        {
            if (IsFileExists(fileName))
            {
                return File.OpenRead(Path.Combine(CACHE_DIR, fileName));
            }
            return default;
        }
        public static bool TryGetFileCache(string fileName, out FileStream fileStream)
        {
            if (IsFileExists(fileName))
            {
                fileStream = File.OpenRead(Path.Combine(CACHE_DIR, fileName));
                return true;
            }
            fileStream = null;
            return false;
        }
        public static void SaveCache(string fileName, Stream stream)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using var fs = File.Create(Path.Combine(CACHE_DIR, fileName));
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fs);
        }
        public static void SaveCache(string fileName, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            using var fs = File.Create(Path.Combine(CACHE_DIR, fileName));
            fs.Write(data, 0, data.Length);
        }
    }
}
