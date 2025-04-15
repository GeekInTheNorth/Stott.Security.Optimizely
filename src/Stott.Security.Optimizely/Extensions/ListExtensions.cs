using System.Collections.Generic;

namespace Stott.Security.Optimizely.Extensions;

internal static class ListExtensions
{
    internal static void TryAdd<TItem>(this List<TItem>? list, TItem? item)
        where TItem : class
    {
        if (list is not null && item is not null)
        {
            list.Add(item);
        }
    }
}