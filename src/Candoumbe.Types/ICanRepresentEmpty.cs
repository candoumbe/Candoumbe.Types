using System;

namespace Candoumbe.Types;

/// <summary>
/// Defines contract for intervals that can be empty.
/// </summary>
/// <typeparam name="TInterval">The type of the interval onto which the interface will be applied</typeparam>
/// <typeparam name="TBound">Type of the interval boundaries</typeparam>
public interface ICanRepresentEmpty<in TInterval, TBound> where TInterval : IRange<TInterval, TBound> where TBound : IComparable<TBound>
{
    /// <summary>
    /// Checks if the current instance is empty (by <typeparamref name="TInterval"/>'s definition).
    /// </summary>
    /// <returns><see langword="true"/> if range is empty or <see langword="false"/> otherwise.</returns>
    bool IsEmpty();
}