using System;

namespace Devmonster.Core.LoggerFluent.ReferenceFunction;

internal static class StringExtensionMethods
{
    /// <summary>
    /// Trims a string to set length
    /// </summary>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string TrimToLength(this string text, int length)
    {
        return text?.Substring(0, Math.Min(text.Length, length)) ?? "";
    }
}

