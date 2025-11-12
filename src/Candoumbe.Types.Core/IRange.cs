using System;

namespace Candoumbe.Types;

/// <summary>
/// It's quite common to see comparisons where a value is checked against an interval of values. Ranges are most of the time handled by a pair of values,
/// and you check against them both. <see cref="IRange{TInterval,TBound}"/> instead uses a single object to represent the range as a whole, and then provides the relevant operations
/// to check if values fall in the <see cref="IRange{TInterval,TBound}"/> and to compare <see cref="IRange{TInterval,TBound}"/>s.
/// </summary>
public interface IRange<TInterval, TBound> : IComparable<TInterval>, IEquatable<TInterval>
    where TInterval : IRange<TInterval, TBound>
    where TBound : IComparable<TBound>
{
    /// <summary>
    /// Start of the interval
    /// </summary>
    TBound Start { get; }

    /// <summary>
    /// End of the interval
    /// </summary>
    TBound End { get; }

    /// <summary>
    /// Checks if the current instance overlaps with <paramref name="other"/>
    /// </summary>
    /// <param name="other">The other instance</param>
    /// <returns><see langword="true"/> if the current instance overlaps <paramref name="other"/> and <see langword="false"/> otherwise.</returns>
#if NET
    public bool Overlaps(TBound other) => (Start.CompareTo(other), other.CompareTo(End)) switch
    {
        ( <= 0, <= 0) => true,
        _             => false
    };
    #else
    bool Overlaps(TBound other);
#endif

    /// <summary>
    /// Checks if the current instance overlaps with <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The other instance</param>
    /// <returns><see langword="true"/> if the current instance overlaps <paramref name="other"/> and <see langword="false"/> otherwise.</returns>
#if NET
    public bool Overlaps(TInterval other) => Overlaps(other.Start) || Overlaps(other.End);
#else
    bool Overlaps(TInterval other);
#endif
}