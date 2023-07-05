using System;
using System.Collections.Generic;

public static class EnumerableExtensions
{
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (keySelector == null)
        {
            throw new ArgumentNullException(nameof(keySelector));
        }

        using (var enumerator = source.GetEnumerator())
        {
            if (!enumerator.MoveNext())
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            var minElement = enumerator.Current;
            var minKey = keySelector(minElement);

            while (enumerator.MoveNext())
            {
                var currentElement = enumerator.Current;
                var currentKey = keySelector(currentElement);

                if (Comparer<TKey>.Default.Compare(currentKey, minKey) < 0)
                {
                    minElement = currentElement;
                    minKey = currentKey;
                }
            }

            return minElement;
        }
    }
}
