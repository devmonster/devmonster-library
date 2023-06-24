
using System;

namespace Devmonster.Core.Extensions.String
{
    public static class StringExtensions
    {
        /// <summary>
        /// Trims the string to specified length if the number of characters exceed the length
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length">Maximum number of characters the string may contain</param>
        /// <returns></returns>
        public static string TrimToLength(this string text, int length)
        {
            return text.Substring(0, Math.Min(text.Length, length));
        }

        /// <summary>
        /// Truncates the middle of the string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startLength"></param>
        /// <param name="endLength"></param>
        /// <returns></returns>
        public static string TruncateMiddle(this string text, int startLength, int endLength)
        {
            if (startLength + endLength >= text.Length) return text;

            return text.TrimToLength(startLength) + "..." + text.Substring(text.Length - endLength, endLength);
        }

        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }            
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        public static string Coalesce(string value, string valueIfEmpty)
        {
            if (!string.IsNullOrEmpty(value.Trim())) { return value; }
            return valueIfEmpty;
        }

    }
}
