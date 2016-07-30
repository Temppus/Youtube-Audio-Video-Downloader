using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader
{
    public static class StringExtensions
    {
        public static string ToSafeFileName(this string s)
        {
            return s
                .Replace("\\", "")
                .Replace("/", "")
                .Replace("\"", "")
                .Replace("*", "")
                .Replace(":", "")
                .Replace("?", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "");
        }

        public static string ReplaceLastOccurrence(this string sourceStr, string matchStr, string replaceStr)
        {
            int place = sourceStr.LastIndexOf(matchStr);

            if (place == -1)
                return sourceStr;

            return sourceStr.Remove(place, matchStr.Length).Insert(place, replaceStr);
        }
    }
}
