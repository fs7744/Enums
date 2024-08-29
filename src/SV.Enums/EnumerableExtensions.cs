using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{
    public static partial class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this List<T> source, int pageSize)
        {
            if (source.Count <= pageSize)
            {
                yield return source;
            }
            else
            {
                var totalCount = (int)Math.Ceiling(source.Count * 1.0 / pageSize);
                for (int i = 0; i <= totalCount; i++)
                {
                    yield return source.Skip(pageSize * i).Take(pageSize);
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> Page<T>(this IEnumerable<T> source, int pageSize)
        {
            if (source is List<T> s)
            {
                return s.Chunk(pageSize);
            }
            else
            {
                return source.Chunk(pageSize);
            }
        }

#if NET6_0_OR_GREATER
#else
        public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
        {
            return ChunkIterator(source, size);
        }

        private static IEnumerable<TSource[]> ChunkIterator<TSource>(IEnumerable<TSource> source, int size)
        {
            using IEnumerator<TSource> e = source.GetEnumerator();
            while (e.MoveNext())
            {
                TSource[] chunk = new TSource[size];
                chunk[0] = e.Current;

                int i = 1;
                for (; i < chunk.Length && e.MoveNext(); i++)
                {
                    chunk[i] = e.Current;
                }

                if (i == chunk.Length)
                {
                    yield return chunk;
                }
                else
                {
                    Array.Resize(ref chunk, i);
                    yield return chunk;
                    yield break;
                }
            }
        }
#endif

        public static bool IsNullOrEmpty<T>(this List<T> source)
        {
            return source == null || source.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this T[] source)
        {
            return source == null || source.Length == 0; ;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null
                || (source is IList<T> s && s.Count == 0)
                || !source.GetEnumerator().MoveNext();
        }

        public static bool IsNotNullOrEmpty<T>(this List<T> source)
        {
            return source != null && source.Count > 0;
        }

        public static bool IsNotNullOrEmpty<T>(this T[] source)
        {
            return source != null && source.Length > 0;
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source != null &&
                ((source is IList<T> s && s.Count > 0)
                || source.GetEnumerator().MoveNext());
        }

        public static List<T> AsList<T>(this IEnumerable<T> source) => source switch
        {
            null => null!,
            List<T> list => list,
            _ => Enumerable.ToList(source),
        };

        public static ICollection<T> AsCollection<T>(this IEnumerable<T> source) => source switch
        {
            null => null!,
            T[] array => array,
            List<T> list => list,
            _ => Enumerable.ToArray(source),
        };

        public static IAsyncEnumerable<TValue> AsyncEmpty<TValue>()
        {
            return EmptyAsyncEnumerator<TValue>.Instance;
        }
    }

    internal sealed class EmptyAsyncEnumerator<TValue> : IAsyncEnumerator<TValue>, IAsyncEnumerable<TValue>
    {
        private static readonly ValueTask<bool> f =
#if NET5_0_OR_GREATER
            ValueTask.FromResult(false);

#else
            new ValueTask<bool>(false);
#endif

        public static readonly EmptyAsyncEnumerator<TValue> Instance = new();

        public TValue Current => default!;

        public ValueTask DisposeAsync()
        {
            return
#if NET5_0_OR_GREATER
                ValueTask.CompletedTask;
#else
                default;
#endif
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return f;
        }

        public IAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return this;
        }
    }
}