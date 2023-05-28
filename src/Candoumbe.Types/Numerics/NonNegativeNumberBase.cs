using System;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif


namespace Candoumbe.Types.Numerics
{
    /// <summary>
    /// Base class for creating numeric types that can only be positive
    /// </summary>
    /// <typeparam name="TNumber">The numeric type that the current type will restrict to only non negative values.</typeparam>
    /// <typeparam name="TSelf">The non negative number type</typeparam>
    public abstract record NonNegativeNumberBase<TNumber, TSelf> : IComparable<TSelf>
#if NET7_0_OR_GREATER
        where TNumber : ISignedNumber<TNumber>, IComparable<TNumber>
        where TSelf : NonNegativeNumberBase<TNumber, TSelf>, IMinMaxValue<TSelf>, IComparisonOperators<TSelf, TSelf, bool>
#else
        where TNumber : IComparable<TNumber>
        where TSelf : NonNegativeNumberBase<TNumber, TSelf>
#endif
    {
        /// <summary>
        /// Gets the underlying value
        /// </summary>
        /// <remarks>The value is garantied to be &gt; or equal to <c>0</c></remarks>
        TNumber Value { get; }

        ///<inheritdoc/>
        public int CompareTo(TSelf other) => Value.CompareTo(other.Value);

        ///<inheritdoc/>
        public virtual bool Equals(NonNegativeNumberBase<TNumber, TSelf> other) => Equals(Value, other.Value);

        ///<inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        /////<inheritdoc/>
        //public static bool operator <(NonNegativeNumberBase<TNumber, TSelf> left, TNumber right)
        //    => left.CompareTo(right) < 0;

        /////<inheritdoc/>
        //public static bool operator >(NonNegativeNumberBase<TNumber, TSelf> left, TNumber right)
        //    => left.CompareTo(right) > 0;

        /////<inheritdoc/>
        //public static bool operator >=(NonNegativeNumberBase<TNumber, TSelf> left, TNumber right)
        //    => left.CompareTo(right) >= 0;

        /////<inheritdoc/>
        //public static bool operator <=(NonNegativeNumberBase<TNumber, TSelf> left, TNumber right)
        //    => left.CompareTo(right) <= 0;

#if NET7_0_OR_GREATER

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
            => left.Value != right.Value;
#endif
    }
}