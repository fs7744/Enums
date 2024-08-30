using SV;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SV;

// note:
//  - I really don't want to make my own custom dictionary.
//  - However, this is faster than FrozonDictionary<TKey, TValue>, so I have no choice but to prepare it.

/// <summary>
/// Provides a read-only dictionary that contents are fixed at the time of instance creation.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
/// <remarks>
/// Reference:<br/>
/// <a href="https://github.com/xin9le/FastEnum/blob/main/src/libs/FastEnum.Core/Internals/FastDictionary.cs"></a>
/// </remarks>
public sealed class FastReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
     where TKey : notnull
{
    #region Fields

    private Entry[] _buckets;
    private int _size;
    const float _loadFactor = 0.75f;
    const int initialSize = 4;
    private static readonly IEqualityComparer<TKey> comparer = EqualityComparer<TKey>.Default;

    #endregion Fields

    #region Constructors

    public FastReadOnlyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        var size = source.TryGetNonEnumeratedCount(out var count) ? count : initialSize;
        var bucketSize = CalculateCapacity(size, _loadFactor);
        this._buckets = (bucketSize is 0) ? [] : new Entry[bucketSize];

        foreach (var x in source)
        {
            if (!this.TryAddInternal(x.Key, x.Value, out _))
                throw new ArgumentException($"Key was already exists. Key:{x.Key}");
        }
    }

    #endregion Constructors

    #region Add

    private bool TryAddInternal(TKey key, TValue value, out TValue resultingValue)
    {
        var nextCapacity = CalculateCapacity(this._size + 1, _loadFactor);
        if (this._buckets.Length < nextCapacity)
        {
            //--- rehash
            var nextBucket = new Entry[nextCapacity];
            for (int i = 0; i < this._buckets.Length; i++)
            {
                var e = this._buckets[i];
                while (e is not null)
                {
                    var newEntry = new Entry(e.Key, e.Value, e.Hash);
                    addToBuckets(nextBucket, key, newEntry, default, out _);
                    e = e.Next;
                }
            }

            var success = addToBuckets(nextBucket, key, null, value, out resultingValue);
            this._buckets = nextBucket;
            if (success)
                this._size++;

            return success;
        }
        else
        {
            var success = addToBuckets(this._buckets, key, null, value, out resultingValue);
            if (success)
                this._size++;

            return success;
        }

        #region Local Functions

        //--- please pass 'key + newEntry' or 'key + value'.
        bool addToBuckets(Entry[] buckets, TKey newKey, Entry newEntry, TValue value, out TValue resultingValue)
        {
            var hash = newEntry?.Hash ?? comparer.GetHashCode(newKey);
            var index = hash & (buckets.Length - 1);
            if (buckets[index] is null)
            {
                if (newEntry is null)
                {
                    resultingValue = value;
                    buckets[index] = new Entry(newKey, resultingValue, hash);
                }
                else
                {
                    resultingValue = newEntry.Value;
                    buckets[index] = newEntry;
                }
            }
            else
            {
                var lastEntry = buckets[index];
                while (true)
                {
                    if (comparer.Equals(lastEntry.Key, newKey))
                    {
                        resultingValue = lastEntry.Value;
                        return false;
                    }

                    if (lastEntry.Next is null)
                    {
                        if (newEntry is null)
                        {
                            resultingValue = value;
                            lastEntry.Next = new Entry(newKey, resultingValue, hash);
                        }
                        else
                        {
                            resultingValue = newEntry.Value;
                            lastEntry.Next = newEntry;
                        }
                        break;
                    }

                    lastEntry = lastEntry.Next;
                }
            }
            return true;
        }

        #endregion Local Functions
    }

    private static int CalculateCapacity(int collectionSize, float loadFactor)
    {
        var initialCapacity = (int)(collectionSize / loadFactor);
        var capacity = 1;
        while (capacity < initialCapacity)
            capacity <<= 1;

        if (capacity < 8)
            return 8;

        return capacity;
    }

    #endregion Add

    #region IReadOnlyDictionary<TKey, TValue>

    public TValue this[TKey key]
        => this.TryGetValue(key, out var value)
        ? value
        : throw new KeyNotFoundException();

    public IEnumerable<TKey> Keys
        => AllKV().Select(i => i.Key);

    public IEnumerable<TValue> Values
        => AllKV().Select(i => i.Value);

    public int Count
        => this._size;

    [MethodImpl(Enums.Optimization)]
    public bool ContainsKey(TKey key)
        => this.TryGetValue(key, out _);

    [MethodImpl(Enums.Optimization)]
    public bool TryGetValue(TKey key, out TValue value)
    {
        var hash = comparer.GetHashCode(key);
        var index = hash & (this._buckets.Length - 1);
        var next = this._buckets[index];
        while (next is not null)
        {
            if (comparer.Equals(next.Key, key))
            {
                value = next.Value!;
                return true;
            }
            next = next.Next;
        }
        value = default;
        return false;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => AllKV().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => AllKV().GetEnumerator();

    public IEnumerable<KeyValuePair<TKey, TValue>> AllKV()
    {
        if (_buckets != null)
        {
            foreach (var item in _buckets)
            {
                var next = item;
                while (next is not null)
                {
                    yield return new KeyValuePair<TKey, TValue>(next.Key, next.Value);
                    next = next.Next;
                }
            }
        }
    }

    #endregion IReadOnlyDictionary<TKey, TValue>

    #region Inner Classes

    private sealed class Entry(TKey key, TValue value, int hash)
    {
        public readonly TKey Key = key;
        public readonly TValue Value = value;
        public readonly int Hash = hash;
        public Entry Next;
    }

    #endregion Inner Classes
}