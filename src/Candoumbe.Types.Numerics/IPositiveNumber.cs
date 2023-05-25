#if NET7_0_OR_GREATER
using System;
using System.Numerics;
#endif

namespace Candoumbe.Types.Numerics
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Base class for creating numeric types that can only be positive
    /// </summary>
    /// <typeparam name="TNumber">The numeric type that the current type will restrict to only positive values.</typeparam>
    /// <typeparam name="TSelf">The positive number type</typeparam>
    public abstract record PositiveNumberBase<TNumber, TSelf> : IComparable<TSelf>
        where TNumber : ISignedNumber<TNumber>, IComparable<TNumber>
        where TSelf : PositiveNumberBase<TNumber, TSelf>, IMinMaxValue<TSelf>, IComparisonOperators<TSelf, TSelf, bool>
    {
        /// <summary>
        /// Gets the underlying value
        /// </summary>
        /// <remarks>The value is garantied to be &gt; or equal to <c>0</c></remarks>
        TNumber Value { get; }

        ///<inheritdoc/>
        public int CompareTo(TSelf other) => Value.CompareTo(other.Value);
    }
#endif
}