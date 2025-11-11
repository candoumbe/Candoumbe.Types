using System;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Candoumbe.Types.Numerics;

/// <summary>
/// Base class for creating numeric types that can only hold a certain range of values.
/// </summary>
/// <typeparam name="TNumber">The numeric type that the current type will restrict values for.</typeparam>
public abstract record Number<TNumber> : IComparable<Number<TNumber>>
    where TNumber : notnull, IComparable<TNumber>
{
    /// <summary>
    /// Gets the underlying <typeparamref name="TNumber"/> value
    /// </summary>
    /// <remarks>The value is guaranteed to be within the range of valid values defined by the current type.</remarks>
    public TNumber Value
    {
        get;
#if NET7_0_OR_GREATER
        protected init;
#else
            protected set;
#endif
    } = default;

    /// <summary>
    /// Builds a new <see cref="Number{TNumber}"/> that contains the default value of <typeparamref name="TNumber"/>.
    /// </summary>
    protected Number() => Value = default;

    /// <summary>
    /// Builds a new <see cref="Number{TNumber}"/> initialized with the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    protected Number(TNumber value) => Value = value;

    ///<inheritdoc/>
    public int CompareTo(Number<TNumber> other) => Value.CompareTo(other.Value);

    ///<inheritdoc/>
    public virtual bool Equals(Number<TNumber> other) => other is not null && Equals(Value, other.Value);

    ///<inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Compares two <see cref="Number{TNumber}"/> instances.
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <returns><c>true</c> when <paramref name="left"/> is less than <paramref name="right"/> and <c>false</c> otherwise. </returns>
    public static bool operator <(Number<TNumber> left, TNumber right)
        => left.Value.CompareTo(right) < 0;

    ///<inheritdoc/>
    public static bool operator >(Number<TNumber> left, TNumber right)
        => left.Value.CompareTo(right) > 0;

    ///<inheritdoc/>
    public static bool operator >=(Number<TNumber> left, TNumber right)
        => left.Value.CompareTo(right) >= 0;

    ///<inheritdoc/>
    public static bool operator <=(Number<TNumber> left, TNumber right)
        => left.Value.CompareTo(right) <= 0;
}