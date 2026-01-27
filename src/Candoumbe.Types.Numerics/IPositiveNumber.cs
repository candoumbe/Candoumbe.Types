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
    where TNumber : notnull, ISignedNumber<TNumber>, IComparable<TNumber>
    where TSelf : PositiveNumberBase<TNumber, TSelf>, IMinMaxValue<TSelf>, IComparisonOperators<TSelf, TSelf, bool>
#else
        where TNumber : IComparable<TNumber>
        where TSelf : PositiveNumberBase<TNumber, TSelf>
#endif
{

#if NET7_0_OR_GREATER
    /// <summary>
    /// Builds a new <see cref="PositiveNumberBase{TNumber, TSelf}"/> initialized with the specified <typeparamref name="TNumber"/> <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value</param>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="value"/> is &lt; <see cref="INumberBase{TNumber}.Zero"/></exception>
#else
    /// <summary>
    /// Builds a new <see cref="PositiveNumberBase{TNumber, TSelf}"/> initialized with the specified <typeparamref name="TNumber"/> <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value</param>
#endif
    protected PositiveNumberBase(TNumber value) : base(value)
    {
#if NET7_0_OR_GREATER
        if (value.CompareTo(TNumber.Zero) < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Value must be greater than {TNumber.Zero}.");
        }
#endif
    }

    ///<inheritdoc/>
    public int CompareTo(TSelf other) => Value.CompareTo(other.Value);
}