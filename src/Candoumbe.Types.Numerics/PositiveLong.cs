using System;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Candoumbe.Types.Numerics;
#if NET7_0_OR_GREATER
#endif

/// <summary>
/// A numeric type which value is guaranteed to always be greater than <c>0</c>.
/// <para>
/// This type can be useful when a value must be strictly greater than <c>0</c> but using <see langword="uint"/> would make an API harder
/// to integrate with existing ecosystem.
/// </para>
/// </summary>
public record PositiveLong : PositiveNumberBase<long, PositiveLong>
#if NET7_0_OR_GREATER
        , IAdditionOperators<PositiveLong, PositiveLong, PositiveLong>
        , IAdditionOperators<PositiveLong, NonNegativeLong, PositiveLong>
        , ISubtractionOperators<PositiveLong, PositiveLong, PositiveLong>
        , IMultiplyOperators<PositiveLong, PositiveLong, PositiveLong>
        , IDivisionOperators<PositiveLong, PositiveLong, NonNegativeLong>
        , IMultiplyOperators<PositiveLong, NonNegativeLong, NonNegativeLong>
        , IComparisonOperators<PositiveLong, PositiveLong, bool>
        , IMinMaxValue<PositiveLong>
        , IMultiplicativeIdentity<PositiveLong, PositiveLong>

#endif
{
#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// The multiplicative identity of the current type
        /// </summary>
#endif
        public static PositiveLong MultiplicativeIdentity => One;

        /// <summary>
        /// The one value
        /// </summary>
        public static PositiveLong One => From(1);

        private PositiveLong(long value)
        {
                if (value < 1)
                {
                        throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(value)} must be greater than 1.");
                }
                Value = value;
        }

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Gets the maximum value for the current type
        /// </summary>
#endif
        public static PositiveLong MaxValue => From(long.MaxValue);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Gets the minimum value for the current type
        /// </summary>
#endif
        public static PositiveLong MinValue => One;

        /// <summary>
        /// Builds a new <see cref="PositiveLong"/> with the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>a <see cref="PositiveLong"/> which initial value is <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="value"/> &lt; <c>1</c>.</exception>
        public static PositiveLong From(long value) => new PositiveLong(value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Adds two <see cref="PositiveLong"/> values together and computes their sum
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns>the sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
#endif
        public static PositiveLong operator +(PositiveLong left, PositiveLong right)
        {
                return (left.Value, right.Value) switch
                {
                        (long.MaxValue, _) => right.Value switch
                        {
                                1 => One,
                                _ => right - One
                        },
                        (_, long.MaxValue) => left.Value switch
                        {
                                1 => One,
                                _ => left - One
                        },

                        _ => (left.Value + right.Value) switch
                        {
                                var result when result < 1 => From(1 + Math.Abs(result)),
                                var result => From(result)
                        }
                };
        }

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Adds two values together and computes their sum
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns>the sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        /// <exception cref="OverflowException"></exception>
#endif
        public static PositiveLong operator checked +(PositiveLong left, PositiveLong right)
        {
                try
                {
                        return From(left.Value + right.Value);
                }
                catch (ArgumentOutOfRangeException)
                {
                        throw new OverflowException($@"adding ""{left}"" and ""{right}"" result is outside of {nameof(PositiveLong)} values");
                }
        }

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
        public static PositiveLong operator +(PositiveLong left, NonNegativeLong right) => From(left.Value + right.Value);

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
        public static PositiveLong operator -(PositiveLong left, PositiveLong right)
                => (left.Value - right.Value) switch
                {
                        var value when value == 0 => MaxValue,
                        var value when value < 0 => From(MaxValue.Value - Math.Abs(value)),
                        var result => From(result)
                };

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
        public static PositiveLong operator checked -(PositiveLong left, PositiveLong right)
                => (left.Value - right.Value) switch
                {
                        < 1 => throw new OverflowException(),
                        var result => From(result)
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
        public static PositiveLong operator *(PositiveLong left, PositiveLong right)
                => From(left.Value * right.Value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Divides two values together to compute their fraction.
        /// </summary>
        /// <param name="left">The left value</param>
        /// <param name="right">The right value</param>
        /// <returns>The result of <paramref name="left"/> divided by <paramref name="right"/>.</returns>
#endif
        public static NonNegativeLong operator /(PositiveLong left, PositiveLong right)
                => NonNegativeLong.From(left.Value / right.Value);

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
        public static NonNegativeLong operator *(PositiveLong left, NonNegativeLong right)
                => From(left.Value * right.Value);

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
        public static bool operator >(PositiveLong left, PositiveLong right) => left.Value > right.Value;

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
        public static bool operator <(PositiveLong left, PositiveLong right) => left.Value < right.Value;

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
        public static bool operator >=(PositiveLong left, PositiveLong right) => left.Value >= right.Value;

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
        public static bool operator <=(PositiveLong left, PositiveLong right) => left.Value <= right.Value;

        ///<inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Implicitely cast to <see cref="long"/> type
        /// </summary>
        /// <param name="x"></param>
        public static implicit operator long(PositiveLong x) => x.Value;

        /// <summary>
        /// Implicitely cast to <see cref="ulong"/> type
        /// </summary>
        /// <param name="x"></param>
        public static implicit operator ulong(PositiveLong x) => (ulong)x.Value;

        /// <summary>
        /// Implicitely cast to <see cref="decimal"/> type
        /// </summary>
        /// <param name="x"></param>
        public static implicit operator decimal(PositiveLong x) => x.Value;

        /// <summary>
        /// Implicitely cast to <see cref="NonNegativeLong"/> type
        /// </summary>
        /// <param name="x"></param>
        public static implicit operator NonNegativeLong(PositiveLong x) => NonNegativeLong.From(x.Value);
}