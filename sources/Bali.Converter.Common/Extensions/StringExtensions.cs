namespace Bali.Converter.Common.Extensions
{
    using System.IO;

    public static class StringExtensions
    {
        public static string RemoveIllegalChars(this string str)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                str = str.Replace(c.ToString(), "");
            }

            return str;
        }
    }
}
