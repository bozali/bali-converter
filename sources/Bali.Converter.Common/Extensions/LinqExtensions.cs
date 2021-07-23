namespace Bali.Converter.Common.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class LinqExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var i in enumerable)
            {
                action?.Invoke(i);
            }
        }
    }
}
