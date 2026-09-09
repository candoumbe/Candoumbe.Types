using System;

namespace Candoumbe.Types.Core;

#if NET
    /// <summary>
    /// Marker interface for intervals that can represent an interval in such way that, for any given <typeparamref name="TBound"/> value,
    /// For example, if an interval represents a range of numbers, an infinite interval would overlap with any specific number.
    /// <see cref="Infinite"/> overlaps any <typeparamref name="TInterval"/> value.
    /// </summary>
    /// <typeparam name="TInterval">Type of the interval</typeparam>
    /// <typeparam name="TBound">Type of the boundaries of the interval</typeparam>
#else
    /// <summary>
    /// Marker interface for intervals that can represent an interval in such way that, for any given <typeparamref name="TBound"/> value,
    /// For example, if an interval represents a range of numbers, an infinite interval would overlap with any specific number.
    /// </summary>
    /// <typeparam name="TInterval">Type of the interval</typeparam>
    /// <typeparam name="TBound">Type of the boundaries of the interval</typeparam>
#endif
public interface ICanRepresentInfinite<TInterval, TBound>
    where TInterval : IRange<TInterval, TBound>, IComparable<TInterval> where TBound : IComparable<TBound>

{
#if NET
    /// <summary>
    /// The infinite <typeparamref name="TBound"/> interval per <typeparamref name="TInterval"/>'s definition.
    /// Such an interval overlaps any <typeparamref name="TBound"/> value and <typeparamref name="TInterval"/>.
    /// </summary>
    static TInterval Infinite { get; }
#endif

    /// <summary>
    /// Checks if the current instance is infinite.
    /// </summary>
    /// <returns><see langword="true"/> if the current <typeparamref name="TInterval"/> overlaps with any other <typeparamref name="TInterval"/> and <see langword="false"/> otherwise.</returns>
    bool IsInfinite();
}