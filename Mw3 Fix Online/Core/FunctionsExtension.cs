using System;
using System.Linq;

namespace Mw3_Fix_Online.Core
{
    public static class FunctionsExtension
    {
        public static byte[] HexStringToByteArray(this string text)
        {
            return Enumerable.Range(0, text.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(text.Substring(x, 2), text.Length))
                         .ToArray();
        }
    }
}