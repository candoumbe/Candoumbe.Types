using System;

namespace Candoumbe.Types;

/// <summary>
/// Marker interface for intervals that can represent an interval in such way that, for any given <typeparamref name="TBound"/> value,
/// <see cref="Infinite"/> overlaps any <typeparamref name="TInterval"/> value.
/// </summary>
/// <typeparam name="TInterval">Type of the interval</typeparam>
/// <typeparam name="TBound">Type of the boundaries of the interval</typeparam>
public interface ICanRepresentInfinite<TInterval, TBound>
    where TInterval : IRange<TInterval, TBound>, IComparable<TInterval> where TBound : IComparable<TBound>

{
    /// <summary>
    /// The infinite <typeparamref name="TBound"/> interval per <typeparamref name="TInterval"/>'s definition.
    /// Such an interval overlaps any <typeparamref name="TBound"/> value and <typeparamref name="TInterval"/>.
    /// </summary>
    static TInterval Infinite { get; }

    /// <summary>
    /// Checks if the current instance is infinite.
    /// </summary>
    /// <returns><see langword="true"/> if the current <typeparamref name="TInterval"/> overlaps with any other <typeparamref name="TInterval"/> and <see langword="false"/> otherwise.</returns>
    bool IsInfinite();
}