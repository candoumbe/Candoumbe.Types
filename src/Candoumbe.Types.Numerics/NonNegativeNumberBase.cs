using System;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Candoumbe.Types.Numerics;

/// <summary>
/// Base class for creating numeric types that can only be positive
/// </summary>
/// <typeparam name="TNumber">The numeric type that the current type will restrict to only non-negative values.</typeparam>
/// <typeparam name="TSelf">The non-negative number type</typeparam>
public abstract record NonNegativeNumberBase<TNumber, TSelf> : Number<TNumber>, IComparable<TSelf>
#if NET7_0_OR_GREATER
    where TNumber : ISignedNumber<TNumber>, IComparable<TNumber>
    where TSelf : NonNegativeNumberBase<TNumber, TSelf>, IMinMaxValue<TSelf>, IComparisonOperators<TSelf, TSelf, bool>, IParsable<TSelf>, ISpanParsable<TSelf>,
    IAdditiveIdentity<TSelf, TSelf>
#else
        where TNumber : IComparable<TNumber>
        where TSelf : NonNegativeNumberBase<TNumber, TSelf>
#endif
{

    ///<inheritdoc/>
    public int CompareTo(TSelf other) => Value.CompareTo(other.Value);


    ///<inheritdoc/>
    public static bool operator <(NonNegativeNumberBase<TNumber, TSelf> left, TNumber right)
        => left.Value.CompareTo(right) < 0;

    ///<inheritdoc/>
    public static bool operator >(NonNegativeNumberBase<TNumber, TSelf> left, TNumber right)
        => left.Value.CompareTo(right) > 0;

    ///<inheritdoc/>
    public static bool operator >=(NonNegativeNumberBase<TNumber, TSelf> left, TNumber right)
        => left.Value.CompareTo(right) >= 0;

    ///<inheritdoc/>
    public static bool operator <=(NonNegativeNumberBase<TNumber, TSelf> left, TNumber right)
        => left.Value.CompareTo(right) <= 0;



    ///<inheritdoc/>
    public static bool operator <(NonNegativeNumberBase<TNumber, TSelf> left, TSelf right)
        => left.Value.CompareTo(right.Value) < 0;

    ///<inheritdoc/>
    public static bool operator >(NonNegativeNumberBase<TNumber, TSelf> left, TSelf right)
        => left.Value.CompareTo(right.Value) > 0;

    ///<inheritdoc/>
    public static bool operator >=(NonNegativeNumberBase<TNumber, TSelf> left, TSelf right)
        => left.Value.CompareTo(right.Value) >= 0;

    ///<inheritdoc/>
    public static bool operator <=(NonNegativeNumberBase<TNumber, TSelf> left, TSelf right)
        => left.Value.CompareTo(right.Value) <= 0;

    ///<inheritdoc/>
    public static bool operator ==(NonNegativeNumberBase<TNumber, TSelf> left, TSelf right)
        => left.Value.CompareTo(right.Value) == 0;

    ///<inheritdoc/>
    public static bool operator !=(NonNegativeNumberBase<TNumber, TSelf> left, TSelf right)
        => left.Value.CompareTo(right.Value) != 0;
}