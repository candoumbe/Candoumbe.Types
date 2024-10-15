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
    /// The static default representation of an empty interval of <typeparamref name="TInterval"/>.
    /// </summary>
    static TInterval Empty { get; }

    /// <summary>
    /// Checks if <paramref name="range"/> is empty (by <typeparamref name="TInterval"/>'s definition)
    /// </summary>
    /// <param name="range"></param>
    /// <returns><see langword="true"/> if range is empty or <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">if <paramref name="range"/> is <see langword="null"/></exception>
    bool IsEmpty();
}