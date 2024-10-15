using System;

namespace Candoumbe.Types;

/// <summary>
/// Marker interface for intervals that can represent an interval in such way that for any <typeparamref name="TBound"/> value,
/// <see cref="TInterval."/>
/// </summary>
/// <typeparam name="TInterval">Type of the interval</typeparam>
/// <typeparam name="TBound">Type of the boundaries of the interval</typeparam>
public interface ICanRepresentInfinite<TInterval, TBound>
    where TInterval : IRange<TInterval, TBound>, IComparable<TInterval> where TBound : IComparable<TBound>

{
    /// <summary>
    /// The infinite <typeparamref name="TBound"/> interval per <typeparamref name="TInterval"/>'s definition.
    /// Such interval overlaps any <typeparamref name="TBound"/> value and <typeparamref name="TInterval"/>.
    /// </summary>
    static TInterval Infinite { get; }

    /// <summary>
    /// Checks if the current instance is infinite.
    /// </summary>
    /// <returns></returns>
    bool IsInfinite();
}