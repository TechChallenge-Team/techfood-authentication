using System;
using System.Collections.Generic;
using System.Linq;

namespace TechFood.Application.Common.Extensions;

public static class LinqExtensions
{
    public static bool AnyOrEmpty<TSource>(this IEnumerable<TSource> source)
        => source?.Any()
        ?? false;

    public static bool AnyOrEmpty<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        => source?.Any(predicate)
        ?? false;
}
