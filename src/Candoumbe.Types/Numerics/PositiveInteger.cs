﻿using System;

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Candoumbe.Types.Numerics
{
    /// <summary>
    /// A numeric type which value is garantied to never be less than <c>0</c>
    /// </summary>
    public record PositiveInteger : IEquatable<PositiveInteger>, IComparable<PositiveInteger>
#if NET7_0_OR_GREATER
        , IAdditionOperators<PositiveInteger, PositiveInteger, PositiveInteger>
        , ISubtractionOperators<PositiveInteger, PositiveInteger, PositiveInteger>
        , IMultiplyOperators<PositiveInteger, PositiveInteger, PositiveInteger>
        , IComparisonOperators<PositiveInteger, PositiveInteger, bool>
        , IMinMaxValue<PositiveInteger>
        , IMultiplicativeIdentity<PositiveInteger, PositiveInteger>

#endif
    {


#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// The multiplicative identity of the current type
        /// </summary> 
#endif
        public static PositiveInteger MultiplicativeIdentity => One;

        /// <summary>
        /// The zero value
        /// </summary>
        public static PositiveInteger Zero => new(0);

        /// <summary>
        /// The one value
        /// </summary>
        public static PositiveInteger One => new(1);

        private PositiveInteger(int value)
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(value)} must be greater than 1.");
            }
            Value = value;
        }

        /// <summary>
        /// Gets the underlying value
        /// </summary>
        /// <remarks>The value is garantied to be &gt; or equal to <c>0</c></remarks>
        public int Value { get; }

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Gets the maximum value for the current type
        /// </summary>
#endif
        public static PositiveInteger MaxValue => From(int.MaxValue);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Gets the minimum value for the current type
        /// </summary>
#endif
        public static PositiveInteger MinValue => One;

        /// <summary>
        /// Builds a new <see cref="PositiveInteger"/> with the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>a <see cref="PositiveInteger"/> which initial value is <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="value"/> is <see langword="null"/>.</exception>
        public static PositiveInteger From(int value) => new PositiveInteger(value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Adds two values together and computes their sum
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns>the sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
#endif
        public static PositiveInteger operator +(PositiveInteger left, PositiveInteger right) => new PositiveInteger(left.Value + right.Value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Substracts <paramref name="right"/> from <paramref name="left"/>.
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns>the result of the difference of <paramref name="right"/> from <paramref name="left"/>.</returns>
#endif
        public static PositiveInteger operator -(PositiveInteger left, PositiveInteger right)
            => (left.Value - right.Value) switch
            {
                < 1 => One,
                int result => From(result)
            };

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Multiplies two values together to compute their product.
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns>The result of <paramref name="left"/> multiplied by <paramref name="right"/>.</returns>
#endif
        public static PositiveInteger operator *(PositiveInteger left, PositiveInteger right)
            => new PositiveInteger(left.Value * right.Value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Substracts <paramref name="right"/> from <paramref name="left"/>.
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns>the result of the difference of <paramref name="right"/> from <paramref name="left"/>.</returns>
#endif
        public static bool operator >(PositiveInteger left, PositiveInteger right) => left.Value > right.Value;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Compares <paramref name="right"/> and <paramref name="left"/> to decide which is the less.
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/> and <see langword="false"/> otherwise.</returns>
#endif
        public static bool operator <(PositiveInteger left, PositiveInteger right) => left.Value < right.Value;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Compares <paramref name="right"/> and <paramref name="left"/> to if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greather than or equal to <paramref name="right"/> and <see langword="false"/> otherwise.</returns>
#endif
        public static bool operator >=(PositiveInteger left, PositiveInteger right) => left.Value >= right.Value;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Compares <paramref name="right"/> and <paramref name="left"/> to decide if <paramref name="left"/> is less than or equal to <paramref name="right"/> .
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/> and <see langword="false"/> otherwise.</returns>
#endif
        public static bool operator <=(PositiveInteger left, PositiveInteger right) => left.Value <= right.Value;

        ///<inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        ///<inheritdoc/>
        public int CompareTo(PositiveInteger other)
        {
            return other is null
                ? throw new ArgumentNullException(nameof(other), $"{nameof(other)} cannot be null")
                : Value.CompareTo(other.Value);
        }
    }
}