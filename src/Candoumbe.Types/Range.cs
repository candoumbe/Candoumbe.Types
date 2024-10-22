// "Copyright (c) Cyrille NDOUMBE.
// Licenced under GNU General Public Licence, version 3.0"

using System;
using System.Collections.Generic;

namespace Candoumbe.Types;

#if !NET5_0_OR_GREATER
/// <summary>
/// It's quite common to see comparisons where a value is checked against a range of values. Ranges are most of the time handled by a pair of values,
/// and you check against them both. <see cref="Range{TBound}"/> instead uses a single object to represent the range as a whole, and then provides the relevant operations
/// to check if values fall in the <see cref="Range{TBound}"/> and to compare <see cref="Range{TBound}"/>s.
/// </summary>
public abstract class Range<TBound> : IRange<Range<TBound>, TBound>, ICanRepresentEmpty<Range<TBound>, TBound>
    where TBound : IComparable<TBound>
#else
/// <summary>
/// Builds a new instance of <see cref="Range{TBound}"/> which spans from <see cref="Start"/> to <see cref="End"/>
/// </summary>
/// <param name="Start">The lowest bound</param>    
/// <param name="End">The highest bound</param>
/// <typeparam name="TBound">Type of the <see cref="Start"/> and <see cref="End"/> bounds.</typeparam>
public abstract record Range<TBound>(TBound Start, TBound End) : IRange<Range<TBound>, TBound>, ICanRepresentEmpty<Range<TBound>, TBound>
    where TBound : IComparable<TBound>
#endif
{
#if !NET5_0_OR_GREATER
    /// <summary>
    /// Builds a new <see cref="Range{TBound}"/> instance
    /// </summary>
    /// <param name="start">start of the interval</param>
    /// <param name="end">end of the interval</param>
    protected Range(TBound start, TBound end)
    {
        Start = start;
        End = end;
    }

    /// <inheritdoc />
    public TBound Start { get; }

    /// <inheritdoc />
    public TBound End { get; }

    ///<inheritdoc/>
#if !NETSTANDARD2_1
    public override int GetHashCode()
    {
        int hashCode = -1676728671;
        hashCode = (hashCode * -1521134295) + EqualityComparer<TBound>.Default.GetHashCode(Start);
        hashCode = (hashCode * -1521134295) + EqualityComparer<TBound>.Default.GetHashCode(End);
        return hashCode;
    }
#else
    public override int GetHashCode() => HashCode.Combine(Start, End);
#endif

    /// <summary>
    /// Override <c>==</c> operator
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns><see langword="true"/> when both ranges are equal and <see langword="false"/> otherwise. </returns>
    public static bool operator ==(Range<TBound> left, Range<TBound> right) => EqualityComparer<Range<TBound>>.Default.Equals(left, right);

    /// <summary>
    /// Override <c>!=</c> operator
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns><see langword="true"/> when <paramref name="left"/> and <paramref name="right"/> values are not equal, and <see langword="false"/> otherwise. </returns>
    public static bool operator !=(Range<TBound> left, Range<TBound> right) => !(left == right);
#endif

    /// <inheritdoc />
    public bool IsEmpty() => Start.Equals(End);

    /// <summary>
    /// Checks if the current instance overlaps with <paramref name="other"/>
    /// </summary>
    /// <param name="other">The other instance</param>
    /// <returns><see langword="true"/> if the current instance overlaps <paramref name="other"/> and <see langword="false"/> otherwise.</returns>
    public virtual bool Overlaps(Range<TBound> other) => other is not null && ( Start.CompareTo(other.Start), Start.CompareTo(other.End), End.CompareTo(other.Start), End.CompareTo(other.End) ) switch
    {
        (> 0, < 0, _, _) => true,
        // current :   |-------|
        // other   :        |-------|
        (_, _, > 0, < 0) => true,
        // current :   |---------------|
        // other   :        |-------|
        (<= 0, _, _, >= 0) => true,
        // current :   |
        // other   :   |
        _ => other is not null && IsEmpty() && other.IsEmpty() && Start.Equals(other.Start),
    };

    /// <summary>
    /// Checks if the current instance is contiguous with <paramref name="other"/>.
    /// </summary>
    /// <param name="other"></param>
    /// <returns><see langword="true"/> when current instance and <paramref name="other"/> are contiguous and <see langword="false"/> otherwise.</returns>
    public virtual bool IsContiguousWith(Range<TBound> other) => End.Equals(other.Start) || Start.Equals(other.End);

    ///<inheritdoc/>
    public override string ToString() => $"[{Start} - {End}]";

    /// <inheritdoc/>
    public virtual bool Equals(Range<TBound> other) => other is not null && Equals(Start, other.Start) && Equals(End, other.End);

    /// <inheritdoc/>
    public int CompareTo(Range<TBound> other)
        => other switch
        {
            null => -1,
            _ => Start.CompareTo(other.Start) switch
            {
                0 => End.CompareTo(other.End),
                int value => value
            }
        };
}

//// <summary>