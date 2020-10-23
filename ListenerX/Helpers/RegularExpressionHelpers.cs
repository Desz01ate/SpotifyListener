using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ListenerX.Helpers
{
    public static class RegularExpressionHelpers
    {
        public static Func<string, string> AlphabetCleaner = (input) => Regex.Replace(input, @"[^0-9a-zA-Z:_%]+", "", RegexOptions.Compiled);
    }
}
