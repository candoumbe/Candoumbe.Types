using System;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Candoumbe.Types.Numerics;

/// <summary>
/// Base class for creating numeric types that can only be positive
/// </summary>
/// <typeparam name="TNumber">The numeric type that the current type will restrict to only positive values.</typeparam>
/// <typeparam name="TSelf">The positive number type</typeparam>
public abstract record PositiveNumberBase<TNumber, TSelf> : Number<TNumber>, IComparable<TSelf>
#if NET7_0_OR_GREATER
    where TNumber : ISignedNumber<TNumber>, IComparable<TNumber>
    where TSelf : PositiveNumberBase<TNumber, TSelf>, IMinMaxValue<TSelf>, IComparisonOperators<TSelf, TSelf, bool>
#else
        where TNumber : IComparable<TNumber>
        where TSelf : PositiveNumberBase<TNumber, TSelf>
#endif
{
    /// <summary>
    /// Gets the underlying value
    /// </summary>
    /// <remarks>The value is garantied to be &gt; or equal to <c>0</c></remarks>
    TNumber Value { get; }

    ///<inheritdoc/>
    public int CompareTo(TSelf other) => Value.CompareTo(other.Value);
}