﻿// "Copyright (c) Cyrille NDOUMBE.
// Licenced under GNU General Public Licence, version 3.0"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif
using System.Text;

namespace Candoumbe.Types.Calendar;

/// <summary>
/// A type that optimize the storage of several <see cref="DateTimeRange"/>.
/// </summary>
public class MultiDateTimeRange : IEquatable<MultiDateTimeRange>, IEnumerable<DateTimeRange>
#if NET7_0_OR_GREATER
    , IAdditionOperators<MultiDateTimeRange, MultiDateTimeRange, MultiDateTimeRange>
    , IAdditionOperators<MultiDateTimeRange, DateTimeRange, MultiDateTimeRange>
    , IUnaryNegationOperators<MultiDateTimeRange, MultiDateTimeRange>
#endif
{
    private readonly ISet<DateTimeRange> _ranges;

    /// <summary>
    /// A <see cref="MultiDateTimeRange"/> that contains no <see cref="DateTimeRange"/>.
    /// </summary>
    public static MultiDateTimeRange Empty => new();

    /// <summary>
    /// A <see cref="MultiDateTimeRange"/> that overlaps any other <see cref="MultiDateTimeRange"/>.
    /// </summary>
    public static MultiDateTimeRange Infinite => new(DateTimeRange.Infinite);

    /// <summary>
    /// Builds a new <see cref="MultiDateTimeRange"/> instance
    /// </summary>
    /// <param name="ranges"></param>
    public MultiDateTimeRange(params DateTimeRange[] ranges)
    {
#if !NETSTANDARD2_1_OR_GREATER || !NET5_0_OR_GREATER
        _ranges = new HashSet<DateTimeRange>();
#else
        _ranges = new HashSet<DateTimeRange>(ranges.Length); 
#endif
        foreach (DateTimeRange range in ranges.OrderBy(x => x.Start))
        {
            Add(range);
        }
    }

    /// <inheritdoc/>
    public IEnumerator<DateTimeRange> GetEnumerator() => _ranges.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Adds <paramref name="range"/>.
    /// </summary>
    /// <remarks>
    /// The algorithm will first try to find if any other <see cref="DateTimeRange"/> overlaps or abuts with <paramref name="range"/> and if so,
    /// will swap that element with the result of <c>range.Union(element)</c>
    /// </remarks>
    /// <param name="range">a range to add to the current instance</param>
    /// <exception cref="ArgumentNullException">if <paramref name="range"/> is <see langword="null"/>.</exception>
    public void Add(DateTimeRange range)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(range);
#else
        if (range is null)
        {
            throw new ArgumentNullException(nameof(range));
        }
#endif

        if (range.IsInfinite())
        {
            _ranges.Clear();
            _ranges.Add(range);
        }
        else
        {
            DateTimeRange[] previous = _ranges.Where(item => item.IsContiguousWith(range) || item.Overlaps(range))
                                              .OrderBy(x => x.Start)
                                              .ToArray();
            if (previous.Length != 0)
            {
                previous.ForEach(item => _ranges.Remove(item));
                DateTimeRange union = previous.Aggregate(range, (a, b) => a.Merge(b));
                _ranges.Add(union);
            }
            else
            {
                _ranges.Add(range);
            }
        }
    }

#if NET7_0_OR_GREATER
    ///<inheritdoc/>
#else
    /// <summary>
    /// Adds two values together and computes their sum
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns>The result of <paramref name="left"/> <c>+</c> <paramref name="right"/>.</returns>
#endif
    public static MultiDateTimeRange operator +(MultiDateTimeRange left, DateTimeRange right)
    {
        left.Add(right);

        return left;
    }

    /// <summary>
    /// Builds a <see cref="MultiDateTimeRange"/> instance that represents the union of the current instance with <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The other instance to add</param>
    /// <exception cref="ArgumentNullException">if <paramref name="other"/> is <see langword="null"/></exception>
    /// <returns>a <see cref="MultiDateTimeRange"/> that represents the union of the current instance with <paramref name="other"/>.</returns>
    public MultiDateTimeRange Merge(MultiDateTimeRange other) => new([.. _ranges, .. other._ranges]);

    /// <summary>
    /// Performs a "union" operation between <paramref name="left"/> and <paramref name="right"/> elements.
    /// </summary>
    /// <param name="left">The left element of the operator</param>
    /// <param name="right">The right element of the operator</param>
    /// <returns>a <see cref="MultiDateTimeRange"/> that represents <paramref name="left"/> and <paramref name="right"/> values.</returns>
    public static MultiDateTimeRange operator +(MultiDateTimeRange left, MultiDateTimeRange right) => left.Merge(right);

    /// <summary>
    /// Tests if the current instance contains one or more <see cref="DateTimeRange"/> which, once combined, overlap the specified <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range to test</param>
    /// <returns><see langword="true"/> if the current instance contains <see cref="DateTimeRange"/>s which, once combined, overlap <paramref name="range"/> and <see langword="false"/> otherwise.</returns>
    public bool Overlaps(DateTimeRange range)
    {
        bool covers;

        if (_ranges.Count == 0)
        {
            covers = false;
        }
        else
        {
            covers = _ranges
#if !NETSTANDARD1_0
        .AsParallel()
#endif
                            .Any(item => (item.Overlaps(range) && item.Start <= range.Start && range.End <= item.End)
                                         || item == range);
        }

        return covers;
    }

    /// <summary>
    /// Checks if the current instance contains one or more <see cref="DateTimeRange"/>s which, once combined, cover the specified <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The range to test</param>
    /// <returns><see langword="true"/> if the current instance contains <see cref="DateTimeRange"/>s which, once combined, cover <paramref name="other"/> and <see langword="false"/> otherwise.</returns>
    public bool Overlaps(MultiDateTimeRange other) => other is not null
                                                    && other._ranges.AsParallel().All(Overlaps)
                                                    && _ranges.AsParallel().All(other.Overlaps);

    ///<inheritdoc/>
    public override string ToString()
    {
        string representation = "{empty}";

        if (IsInfinite())
        {
            representation = "{infinite}";
        }
        else if (!IsEmpty())
        {
            StringBuilder sb = new();

            foreach (DateTimeRange item in _ranges)
            {
                if (sb.Length > 0)
                {
                    sb.Append(',');
                }
                sb.Append(item);
            }

            representation = sb.Insert(0, "{").Append('}').ToString();
        }

        return representation;
    }

    /// <summary>
    /// Computes the complement of the current instance.
    /// </summary>
    public MultiDateTimeRange Complement()
    {
        MultiDateTimeRange complement;

        if (IsInfinite())
        {
            complement = Empty;
        }
        else if (IsEmpty())
        {
            complement = Infinite;
        }
        else
        {
            complement = new();
            switch (_ranges)
            {
                case { Count: 1 }:
                    {
                        DateTimeRange current = _ranges.ElementAt(0);
                        complement.Add(DateTimeRange.UpTo(current.Start));
                        complement.Add(DateTimeRange.DownTo(current.End));
                    }
                    break;
                default:
                    {
                        DateTimeRange[] ranges = _ranges.OrderBy(range => range.Start)
                                                        .ToArray();
                        for (int i = 0; i < ranges.Length; i++)
                        {
                            DateTimeRange current = ranges[i];
                            switch (i)
                            {
                                case 0:
                                    complement.Add(DateTimeRange.UpTo(current.Start));
                                    break;
                                case int _ when i <= ranges.Length - 2:
                                    {
                                        DateTimeRange previous = ranges[i - 1];
                                        DateTimeRange next = ranges[i + 1];
                                        complement.Add(new DateTimeRange(previous.End, current.Start));
                                        complement.Add(new DateTimeRange(current.End, next.Start));
                                    }
                                    break;
                                default:
                                    {
                                        DateTimeRange previous = ranges[i - 1];
                                        complement.Add(new DateTimeRange(previous.End, current.Start));
                                        if (current.End < DateTime.MaxValue)
                                        {
                                            complement.Add(DateTimeRange.DownTo(current.End));
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        return complement;
    }

    ///<inheritdoc/>
    public static MultiDateTimeRange operator -(MultiDateTimeRange range) => range.Complement();

    /// <summary>
    /// Checks if the current instance is empty.
    /// </summary>
    /// <returns><see langword="true"/> when the current instance cannot cover any other </returns>
    public bool IsEmpty() => _ranges.Count == 0 || _ranges.All(range => range.IsEmpty());

    /// <summary>
    /// Checks if the current instance is "infinite" which, if <see langword="true" />, means that it overlaps any other <see cref="DateTimeRange"/>.
    /// </summary>
    /// <returns></returns>
    public bool IsInfinite() => _ranges.AtLeastOnce() && _ranges.All(range => range.IsInfinite());

    ///<inheritdoc/>
    public bool Equals(MultiDateTimeRange other)
    {
        bool equals = false;
        if (other is not null)
        {
            equals = ReferenceEquals(this, other)
                     || (IsEmpty() && other.IsEmpty())
                     || (Overlaps(other) && other.Overlaps(this));
        }
        return equals;
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as MultiDateTimeRange);

    ///<inheritdoc/>
    public override int GetHashCode()
    {
        HashCode hashCode = new();

        foreach (DateTimeRange range in _ranges)
        {
            hashCode.Add(range);
        }

        return hashCode.ToHashCode();
    }
}