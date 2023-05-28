using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Candoumbe.Types.Numerics
{
    /// <summary>
    /// A numeric type that can only hold an <see langword="long" /> value &gt;= <c>0</c>.
    /// </summary>
    /// <remarks>
    /// This type is fully interoperable with <see cref="long"/> type.
    /// </remarks>
    public record NonNegativeLong :
        NonNegativeNumberBase<long, NonNegativeLong>,
        IEquatable<NonNegativeLong>

#if NET7_0_OR_GREATER
        , IAdditiveIdentity<NonNegativeLong, NonNegativeLong>
        , IMinMaxValue<NonNegativeLong>
        , IAdditionOperators<NonNegativeLong, NonNegativeLong, NonNegativeLong>
        , IAdditionOperators<NonNegativeLong, long, NonNegativeLong>
        , ISubtractionOperators<NonNegativeLong, NonNegativeLong, NonNegativeLong>
        , ISubtractionOperators<NonNegativeLong, long, NonNegativeLong>
        , IEqualityOperators<NonNegativeLong, NonNegativeLong, bool>
        , IEqualityOperators<NonNegativeLong, long, bool>
        , IMultiplyOperators<NonNegativeLong, NonNegativeLong, NonNegativeLong>
        , IMultiplyOperators<NonNegativeLong, PositiveLong, NonNegativeLong>
        , IComparisonOperators<NonNegativeLong, NonNegativeLong, bool>


#endif
    {
        /// <summary>
        /// The underlying <see langword="long"/> value
        /// </summary>
        public long Value
        {
            get;
#if NET6_0_OR_GREATER
            init;
#endif
        }

        private NonNegativeLong(long value)
        {
            Value = value;
        }

        /// <summary>
        /// Builds a new <see cref="NonNegativeLong"/> with the specified value
        /// </summary>
        /// <param name="value">The desired value</param>
        /// <returns>A <see cref="NonNegativeLong"/> which holds the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="value"/> is &lt; <c>0L</c></exception>
        public static NonNegativeLong From(long value)
        {
            return value < 0
                ? throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(value)} cannot be negative")
                : new NonNegativeLong(value);
        }

        /// <summary>
        /// The zero value of the current type
        /// </summary>
        public static NonNegativeLong Zero => From(0);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Gets additive identity of the current type
        /// </summary>
#endif
        public static NonNegativeLong AdditiveIdentity => Zero;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Gets the maximum value of the current type
        /// </summary>
#endif
        public static NonNegativeLong MaxValue => From(long.MaxValue);

        ///<inheritdoc/>
        public static NonNegativeLong MinValue => Zero;
        /// <inheritdoc/>
        public static NonNegativeLong One => throw new NotImplementedException();
        /// <inheritdoc/>
        public static long Radix => throw new NotImplementedException();
        /// <inheritdoc/>
        public static NonNegativeLong MultiplicativeIdentity => throw new NotImplementedException();
#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Computes the sum of two <see cref="NonNegativeLong"/> values.
        /// </summary>
        /// <param name="left">The value from which <paramref name="right"/> is subtracted</param>
        /// <param name="right">The value to subtract from <paramref name="left"/>.</param>
        /// <returns>the sum of left and right.</returns>
#endif
        public static NonNegativeLong operator +(NonNegativeLong left, NonNegativeLong right)
            => From(left.Value + right.Value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Adds two values together to compute their sum.
        /// </summary>
        /// <param name="left">The value from which <paramref name="right"/> is subtracted</param>
        /// <param name="right">The value to subtract from <paramref name="left"/>.</param>
        /// <returns>the sum of left and right.</returns>
#endif
        public static NonNegativeLong operator +(NonNegativeLong left, long right)
            => From((left.Value + right) switch
            {
                < 0 => 0,
                long value => value
            });

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Subtracts <paramref name="right"/> to <paramref name="left"/> and computes their difference
        /// </summary>
        /// <param name="left">The value from which <paramref name="right"/> is subtracted</param>
        /// <param name="right">The value to subtract from <paramref name="left"/>.</param>
        /// <returns>the result of the subtraction.</returns>
#endif
        public static NonNegativeLong operator -(NonNegativeLong left, NonNegativeLong right)
            => left - right.Value;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Subtracts <paramref name="right"/> to <paramref name="left"/> and computes their difference
        /// </summary>
        /// <param name="left">The value from which <paramref name="right"/> is subtracted</param>
        /// <param name="right">The value to subtract from <paramref name="left"/>.</param>
        /// <returns>the result of the subtraction.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> is <see langword="null"/></exception>
#endif
        public static NonNegativeLong operator -(NonNegativeLong left, long right)
            => From((left.Value - right) switch
            {
                < 0 => 0,
                long value => value
            });

        /// <summary>
        /// Implicit cast to <see langword="long"/>
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator long(NonNegativeLong value) => value.Value;

        /// <summary>
        /// Implicit cast to <see langword="ulong"/> type
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator ulong(NonNegativeLong value) => (ulong)value.Value;

        /// <summary>
        /// Implicit cast to <see langword="decimal"/> type
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator decimal(NonNegativeLong value) => value.Value;

        /// <summary>
        /// Implicit cast to <see langword="decimal"/> type
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator uint(NonNegativeLong value) => (uint)value.Value;

        ///<inheritdoc/>
        public static bool operator ==(NonNegativeLong left, long right) => left.Value == right;

        ///<inheritdoc/>
        public static bool operator !=(NonNegativeLong left, long right) => !(left.Value == right);

        ///<inheritdoc/>
        public static bool operator <(NonNegativeLong left, NonNegativeLong right) => left.Value < right.Value;

        ///<inheritdoc/>
        public static bool operator >(NonNegativeLong left, NonNegativeLong right) => left.Value > right.Value;

        ///<inheritdoc/>
        public static bool operator <=(NonNegativeLong left, NonNegativeLong right) => left.Value <= right.Value;

        ///<inheritdoc/>
        public static bool operator >=(NonNegativeLong left, NonNegativeLong right) => left.Value >= right.Value;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Multiplies <paramref name="left"/> by <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The value to multiply by <paramref name="right"/></param>
        /// <param name="right">The value to multiply by <paramref name="left"/>.</param>
        /// <returns>the result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> is <see langword="null"/></exception>
        /// <exception cref="OverflowException">The result of <paramref name="left"/> <c>*</c> <paramref name="right"/> falls outside of <see cref="NonNegativeLong"/> values.</exception>
#endif
        public static NonNegativeLong operator *(NonNegativeLong left, NonNegativeLong right) => From(left.Value * right.Value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Multiplies <paramref name="left"/> by <paramref name="right"/> and computes their product.
        /// </summary>
        /// <param name="left">The value to multiply by <paramref name="right"/></param>
        /// <param name="right">The value to multiply by <paramref name="left"/>.</param>
        /// <returns>the product.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="left"/> is <see langword="null"/></exception>
        /// <exception cref="OverflowException">The result of <paramref name="left"/> <c>*</c> <paramref name="right"/> falls outside of <see cref="NonNegativeLong"/> values.</exception>
#endif
        public static NonNegativeLong operator *(NonNegativeLong left, PositiveLong right) => From(left.Value * right.Value);

        /// <inheritdoc/>
        public static NonNegativeLong operator --(NonNegativeLong value)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public static NonNegativeLong operator /(NonNegativeLong left, NonNegativeLong right)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public static NonNegativeLong operator ++(NonNegativeLong value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public static NonNegativeLong operator +(NonNegativeLong value)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        ///<inheritdoc/>
        public static NonNegativeLong Abs(NonNegativeLong value) => value;

        ///<inheritdoc/>
        public static bool IsCanonical(NonNegativeLong value) => true;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
        public static bool IsOddInteger(NonNegativeLong value)
                    => long.IsOddInteger(value.Value);
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is an odd integral number
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is odd</returns>
        public static bool IsOddInteger(NonNegativeLong value)
                    => value.Value % 2 != 0;
#endif

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is an even integral number
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is even.</returns>
#endif
        public static bool IsEvenInteger(NonNegativeLong value) => !IsOddInteger(value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is an even integral number
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
#endif
        public static bool IsComplexNumber(NonNegativeLong value) => false;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is a finite <see cref="NonNegativeLong"/>
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="true"/></returns>
#endif
        public static bool IsFinite(NonNegativeLong value) => true;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is an imaginary number.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
#endif
        public static bool IsImaginaryNumber(NonNegativeLong value) => false;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is infinity.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
#endif
        public static bool IsInfinity(NonNegativeLong value) => false;

        /// <summary>
        /// Determines if <paramref name="value"/> is an integer.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="true"/></returns>
        public static bool IsLong(NonNegativeLong value) => true;

        /// <summary>
        /// Determines if <paramref name="value"/> is not a number.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
        public static bool IsNaN(NonNegativeLong value) => false;

        /// <summary>
        /// Determines if <paramref name="value"/> is not a number.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
        public static bool IsNegative(NonNegativeLong value) => false;

        /// <inheritdoc/>
        public static bool IsNegativeInfinity(NonNegativeLong value) => throw new NotImplementedException();
        /// <inheritdoc/>
        public static bool IsNormal(NonNegativeLong value) => throw new NotImplementedException();

        /// <inheritdoc/>
        public static bool IsPositive(NonNegativeLong value) => true;

        /// <inheritdoc/>
        public static bool IsPositiveInfinity(NonNegativeLong value) => false;

        /// <inheritdoc/>
        public static bool IsRealNumber(NonNegativeLong value) => true;

        /// <inheritdoc/>
        public static bool IsSubnormal(NonNegativeLong value) => throw new NotImplementedException();

#if NET7_0_OR_GREATER
        /// <inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is <see cref="Zero"/>.
        /// </summary>
        /// <param name="value">The value to be checked</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is <see cref="Zero"/> and <see langword="false"/> otherwise</returns>
#endif
        public static bool IsZero(NonNegativeLong value) => value == Zero;

#if NET7_0_OR_GREATER
        /// <inheritdoc/>
#else
        /// <summary>
        /// Compares two values to determine which is greater
        /// </summary>
        /// <param name="x">The value to compare to <paramref name="y"/>.</param>
        /// <param name="y">The value to compare to <paramref name="x"/>.</param>
        /// <returns><paramref name="x"/> if <paramref name="x"/> is greater than <paramref name="y"/>; otherwise <paramref name="y"/>.</returns>
#endif
        public static NonNegativeLong MaxMagnitude(NonNegativeLong x, NonNegativeLong y)
            => (x > y) switch
            {
                true => x,
                _ => y
            };

#if NET7_0_OR_GREATER
        /// <inheritdoc/>
#else
        /// <summary>
        /// Compares two values to determine which is greater
        /// </summary>
        /// <param name="x">The value to compare to <paramref name="y"/>.</param>
        /// <param name="y">The value to compare to <paramref name="x"/>.</param>
        /// <returns><paramref name="x"/> if <paramref name="x"/> is greater than <paramref name="y"/>; otherwise <paramref name="y"/>.</returns>
#endif
        public static NonNegativeLong MaxMagnitudeNumber(NonNegativeLong x, NonNegativeLong y) => MaxMagnitude(x, y);

        /// <inheritdoc/>
        public static NonNegativeLong MinMagnitude(NonNegativeLong x, NonNegativeLong y) => MaxMagnitude(x, y) == x ? y : x;

        /// <inheritdoc/>
        public static NonNegativeLong MinMagnitudeNumber(NonNegativeLong x, NonNegativeLong y) => MinMagnitude(x, y);

        /// <inheritdoc/>
        public static NonNegativeLong Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider) => throw new NotImplementedException();

        /// <inheritdoc/>
        public static NonNegativeLong Parse(string s, NumberStyles style, IFormatProvider provider) => throw new NotImplementedException();

        /// <inheritdoc/>
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, [MaybeNullWhen(false)] out NonNegativeLong result) => throw new NotImplementedException();

        /// <inheritdoc/>
        public static bool TryParse([NotNullWhen(true)] string s, NumberStyles style, IFormatProvider provider, [MaybeNullWhen(false)] out NonNegativeLong result)
            => throw new NotImplementedException();

#if NET7_0_OR_GREATER
        /// <inheritdoc/>
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider)
            => Value.TryFormat(destination, out charsWritten, format, provider);
#endif

        /// <inheritdoc/>
        public string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();

#if NET7_0_OR_GREATER
        /// <inheritdoc/>
        public static NonNegativeLong Parse(ReadOnlySpan<char> s, IFormatProvider provider) => From(long.Parse(s, provider));

        /// <inheritdoc/>
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, [MaybeNullWhen(false)] out NonNegativeLong result)
        {
            bool successfullyParsed = false;
            try
            {
                result = From(long.Parse(s, provider));
                successfullyParsed = true;
            }
            catch
            {
                result = default;
            }

            return successfullyParsed;
        }
#endif

        /// <summary>
        /// Parses a <see cref="NonNegativeLong"/> out of a string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        /// <exception cref="OverflowException"></exception>
        public static NonNegativeLong Parse(string s, IFormatProvider provider)
        {
            try
            {
                long value = long.Parse(s, provider);
                return value < MinValue || value > MaxValue
                    ? throw new OverflowException($@"""{s}"" represents a value that is outside range of {nameof(NonNegativeLong)} values")
                    : From(value);
            }
            catch (FormatException)
            {
                throw;
            }
        }

        /// <inheritdoc/>
        public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out NonNegativeLong result)
        {
            bool parsingDone = false;

            try
            {
                result = Parse(s, provider);
                parsingDone = true;
            }
            catch
            {
                result = default;
            }

            return parsingDone;
        }
    }
}
