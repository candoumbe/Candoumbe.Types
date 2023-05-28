using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Candoumbe.Types.Numerics
{
    /// <summary>
    /// A numeric type that can only hold an <see langword="int" /> value &gt;= <c>0</c>.
    /// </summary>
    public record NonNegativeInteger :
        NonNegativeNumberBase<int, NonNegativeInteger>,
        IEquatable<NonNegativeInteger>
#if NET7_0_OR_GREATER
        , IAdditiveIdentity<NonNegativeInteger, NonNegativeInteger>
        , IMinMaxValue<NonNegativeInteger>
        , IAdditionOperators<NonNegativeInteger, NonNegativeInteger, NonNegativeInteger>
        , IAdditionOperators<NonNegativeInteger, int, NonNegativeInteger>
        , ISubtractionOperators<NonNegativeInteger, NonNegativeInteger, NonNegativeInteger>
        , ISubtractionOperators<NonNegativeInteger, int, NonNegativeInteger>
        , IEqualityOperators<NonNegativeInteger, NonNegativeInteger, bool>
        , IEqualityOperators<NonNegativeInteger, int, bool>
        , IMultiplyOperators<NonNegativeInteger, NonNegativeInteger, NonNegativeInteger>
        , IMultiplyOperators<NonNegativeInteger, PositiveInteger, NonNegativeInteger>
        , IComparisonOperators<NonNegativeInteger, NonNegativeInteger, bool>

#endif
    {
        /// <summary>
        /// The underlying <see langword="int"/> value
        /// </summary>
        public int Value
        {
            get;
#if NET6_0_OR_GREATER
            init;
#endif
        }

        private NonNegativeInteger(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new <see cref="NonNegativeInteger"/> with the specified value
        /// </summary>
        /// <param name="value">The desired value</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="value"/> is &lt; 0</exception>
        public static NonNegativeInteger From(int value)
        {
            return value < 0
                ? throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(value)} cannot be negative")
                : new NonNegativeInteger(value);
        }

        /// <summary>
        /// The zero
        /// </summary>
        public static NonNegativeInteger Zero => From(0);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Gets additive identity of the current type
        /// </summary>
#endif
        public static NonNegativeInteger AdditiveIdentity => Zero;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Gets the maximum value of the current type
        /// </summary>
#endif
        public static NonNegativeInteger MaxValue => From(int.MaxValue);

        ///<inheritdoc/>
        public static NonNegativeInteger MinValue => Zero;

        /// <inheritdoc/>
        public static NonNegativeInteger One => throw new NotImplementedException();

        /// <inheritdoc/>
        public static int Radix => throw new NotImplementedException();

        /// <inheritdoc/>
        public static NonNegativeInteger MultiplicativeIdentity => throw new NotImplementedException();

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Computes the sum of two <see cref="NonNegativeInteger"/> values.
        /// </summary>
        /// <param name="left">The value from which <paramref name="right"/> is subtracted</param>
        /// <param name="right">The value to subtract from <paramref name="left"/>.</param>
        /// <returns>the sum of left and right.</returns>
#endif
        public static NonNegativeInteger operator +(NonNegativeInteger left, NonNegativeInteger right)
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
        public static NonNegativeInteger operator +(NonNegativeInteger left, int right)
            => From((left.Value + right) switch
            {
                < 0 => 0,
                int value => value
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
        public static NonNegativeInteger operator -(NonNegativeInteger left, NonNegativeInteger right)
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
        public static NonNegativeInteger operator -(NonNegativeInteger left, int right)
            => From((left.Value - right) switch
            {
                < 0 => 0,
                int value => value
            });

        /// <summary>
        /// Implicit cast to <see langword="int"/>
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator int(NonNegativeInteger value) => value.Value;

        /// <summary>
        /// Implicit cast to <see langword="long"/>
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator long(NonNegativeInteger value) => value.Value;

        /// <summary>
        /// Implicit cast to <see langword="ulong"/> type
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator ulong(NonNegativeInteger value) => (ulong)value.Value;

        /// <summary>
        /// Implicit cast to <see langword="decimal"/> type
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator decimal(NonNegativeInteger value) => value.Value;

        /// <summary>
        /// Implicit cast to <see langword="decimal"/> type
        /// </summary>
        /// <param name="value">The value to cast</param>
        public static implicit operator uint(NonNegativeInteger value) => (uint)value.Value;

        ///<inheritdoc/>
        public static bool operator <(NonNegativeInteger left, NonNegativeInteger right) => left.Value < right.Value;

        ///<inheritdoc/>
        public static bool operator >(NonNegativeInteger left, NonNegativeInteger right) => left.Value > right.Value;

        ///<inheritdoc/>
        public static bool operator >=(NonNegativeInteger left, NonNegativeInteger right) => left.Value >= right.Value;

        ///<inheritdoc/>
        public static bool operator <=(NonNegativeInteger left, NonNegativeInteger right) => left.Value <= right.Value;

        ///<inheritdoc/>
        public static bool operator ==(NonNegativeInteger left, int right) => left.Value == right;

        ///<inheritdoc/>
        public static bool operator !=(NonNegativeInteger left, int right) => !(left.Value == right);

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
        /// <exception cref="OverflowException">The result of <paramref name="left"/> <c>*</c> <paramref name="right"/> falls outside of <see cref="NonNegativeInteger"/> values.</exception>
#endif
        public static NonNegativeInteger operator *(NonNegativeInteger left, NonNegativeInteger right) => From(left.Value * right.Value);

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
        /// <exception cref="OverflowException">The result of <paramref name="left"/> <c>*</c> <paramref name="right"/> falls outside of <see cref="NonNegativeInteger"/> values.</exception>
#endif
        public static NonNegativeInteger operator *(NonNegativeInteger left, PositiveInteger right) => From(left.Value * right.Value);

        /// <inheritdoc/>
        public static NonNegativeInteger operator --(NonNegativeInteger value)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public static NonNegativeInteger operator /(NonNegativeInteger left, NonNegativeInteger right)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public static NonNegativeInteger operator ++(NonNegativeInteger value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public static NonNegativeInteger operator +(NonNegativeInteger value)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();

        ///<inheritdoc/>
        public static NonNegativeInteger Abs(NonNegativeInteger value) => value;

        ///<inheritdoc/>
        public static bool IsCanonical(NonNegativeInteger value) => true;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
        public static bool IsOddInteger(NonNegativeInteger value)
                    => int.IsOddInteger(value.Value);
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is an odd integral number
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is odd</returns>
        public static bool IsOddInteger(NonNegativeInteger value)
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
        public static bool IsEvenInteger(NonNegativeInteger value) => !IsOddInteger(value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is an even integral number
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
#endif
        public static bool IsComplexNumber(NonNegativeInteger value) => false;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is a finite <see cref="NonNegativeInteger"/>
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="true"/></returns>
#endif
        public static bool IsFinite(NonNegativeInteger value) => true;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is an imaginary number.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
#endif
        public static bool IsImaginaryNumber(NonNegativeInteger value) => false;

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is infinity.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
#endif
        public static bool IsInfinity(NonNegativeInteger value) => false;

        /// <summary>
        /// Determines if <paramref name="value"/> is an integer.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="true"/></returns>
        public static bool IsInteger(NonNegativeInteger value) => true;

        /// <summary>
        /// Determines if <paramref name="value"/> is not a number.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
        public static bool IsNaN(NonNegativeInteger value) => false;

        /// <summary>
        /// Determines if <paramref name="value"/> is not a number.
        /// </summary>
        /// <param name="value">the value to test</param>
        /// <returns><see langword="false"/></returns>
        public static bool IsNegative(NonNegativeInteger value) => false;

        /// <inheritdoc/>
        public static bool IsNegativeInfinity(NonNegativeInteger value) => throw new NotImplementedException();
        /// <inheritdoc/>
        public static bool IsNormal(NonNegativeInteger value) => throw new NotImplementedException();

        /// <inheritdoc/>
        public static bool IsPositive(NonNegativeInteger value) => true;

        /// <inheritdoc/>
        public static bool IsPositiveInfinity(NonNegativeInteger value) => false;

        /// <inheritdoc/>
        public static bool IsRealNumber(NonNegativeInteger value) => true;

        /// <inheritdoc/>
        public static bool IsSubnormal(NonNegativeInteger value) => throw new NotImplementedException();

#if NET7_0_OR_GREATER
        /// <inheritdoc/>
#else
        /// <summary>
        /// Determines if <paramref name="value"/> is <see cref="Zero"/>.
        /// </summary>
        /// <param name="value">The value to be checked</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is <see cref="Zero"/> and <see langword="false"/> otherwise</returns>
#endif
        public static bool IsZero(NonNegativeInteger value) => value == Zero;

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
        public static NonNegativeInteger MaxMagnitude(NonNegativeInteger x, NonNegativeInteger y)
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
        public static NonNegativeInteger MaxMagnitudeNumber(NonNegativeInteger x, NonNegativeInteger y) => MaxMagnitude(x, y);

        /// <inheritdoc/>
        public static NonNegativeInteger MinMagnitude(NonNegativeInteger x, NonNegativeInteger y) => MaxMagnitude(x, y) == x ? y : x;

        /// <inheritdoc/>
        public static NonNegativeInteger MinMagnitudeNumber(NonNegativeInteger x, NonNegativeInteger y) => MinMagnitude(x, y);

        /// <inheritdoc/>
        public static NonNegativeInteger Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider) => throw new NotImplementedException();

        /// <inheritdoc/>
        public static NonNegativeInteger Parse(string s, NumberStyles style, IFormatProvider provider) => throw new NotImplementedException();

        /// <inheritdoc/>
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, [MaybeNullWhen(false)] out NonNegativeInteger result) => throw new NotImplementedException();

        /// <inheritdoc/>
        public static bool TryParse([NotNullWhen(true)] string s, NumberStyles style, IFormatProvider provider, [MaybeNullWhen(false)] out NonNegativeInteger result)
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
        public static NonNegativeInteger Parse(ReadOnlySpan<char> s, IFormatProvider provider) => From(int.Parse(s, provider));

        /// <inheritdoc/>
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, [MaybeNullWhen(false)] out NonNegativeInteger result)
        {
            bool successfullyParsed = false;
            try
            {
                result = From(int.Parse(s, provider));
                successfullyParsed = true;
            }
            catch
            {
                result = default;
            }

            return successfullyParsed;
        }
#endif

        /// <inheritdoc/>
        public static NonNegativeInteger Parse(string s, IFormatProvider provider)
        {
            long value = long.Parse(s, provider);
            return value < MinValue || value > MaxValue
                ? throw new OverflowException($@"""{s}"" represents a value that is outside range of {nameof(NonNegativeInteger)} values")
                : From((int)value);
        }

        /// <inheritdoc/>
        public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, [MaybeNullWhen(false)] out NonNegativeInteger result)
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
