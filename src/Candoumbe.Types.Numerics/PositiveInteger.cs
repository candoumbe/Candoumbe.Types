using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Candoumbe.Types.Numerics;
#if NET7_0_OR_GREATER
#endif

/// <summary>
/// A numeric type which <see cref="int"/> value is guarantied to always be greater than <c>0</c>.
/// <para>
/// This type can be useful when a value must be strictly greater than <c>0</c> but using <see langword="uint"/> would make an API harder
/// to integrate with existing ecosystem.
/// </para>
/// </summary>
public record PositiveInteger :
    PositiveNumberBase<int, PositiveInteger>
#if NET7_0_OR_GREATER
        , IAdditionOperators<PositiveInteger, PositiveInteger, PositiveInteger>
        , IAdditionOperators<PositiveInteger, NonNegativeInteger, PositiveInteger>
        , ISubtractionOperators<PositiveInteger, PositiveInteger, PositiveInteger>
        , IMultiplyOperators<PositiveInteger, PositiveInteger, PositiveInteger>
        , IDivisionOperators<PositiveInteger, PositiveInteger, NonNegativeInteger>
        , IMultiplyOperators<PositiveInteger, NonNegativeInteger, NonNegativeInteger>
        , IComparisonOperators<PositiveInteger, PositiveInteger, bool>
        , IMinMaxValue<PositiveInteger>
        , IMultiplicativeIdentity<PositiveInteger, PositiveInteger>
        , ISpanParsable<PositiveInteger>
        , ISpanFormattable
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
    /// The one value
    /// </summary>
    public static PositiveInteger One => From(1);

    private PositiveInteger(int value)
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
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="value"/> &lt; <c>1</c>.</exception>
    public static PositiveInteger From(int value) => new(value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
    /// <summary>
    /// Adds two <see cref="PositiveInteger"/> values together and computes their sum
    /// </summary>
    /// <param name="left">The left value</param>
    /// <param name="right">The right value</param>
    /// <returns>the sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
#endif
    public static PositiveInteger operator +(PositiveInteger left, PositiveInteger right)
    {
        return ( left.Value, right.Value ) switch
        {
            (int.MaxValue, _) => right.Value switch
            {
                1 => One,
                _ => right - One
            },
            (_, int.MaxValue) => left.Value switch
            {
                1 => One,
                _ => left - One
            },

            _ => ( left.Value + right.Value ) switch
            {
                int result when result < 1 => From(1 + Math.Abs(result)),
                int result => From(result)
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
    public static PositiveInteger operator checked +(PositiveInteger left, PositiveInteger right)
    {
        try
        {
            return From(left.Value + right.Value);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new OverflowException(
                $@"adding ""{left}"" and ""{right}"" result is outside of {nameof(PositiveInteger)} values");
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
    public static PositiveInteger operator +(PositiveInteger left, NonNegativeInteger right) =>
        From(left.Value + right.Value);

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
    /// <summary>
    /// Subtracts <paramref name="right"/> from <paramref name="left"/>.
    /// </summary>
    /// <param name="left">The left value</param>
    /// <param name="right">The right value</param>
    /// <returns>the result of the difference of <paramref name="right"/> from <paramref name="left"/>.</returns>
#endif
    public static PositiveInteger operator -(PositiveInteger left, PositiveInteger right)
        => ( left.Value - right.Value ) switch
        {
            0 => MaxValue,
            int value and < 0 => From(MaxValue.Value - Math.Abs(value)),
            int result => From(result)
        };

#if NET7_0_OR_GREATER
        ///<inheritdoc/>
#else
    /// <summary>
    /// Subtracts <paramref name="right"/> from <paramref name="left"/>.
    /// </summary>
    /// <param name="left">The left value</param>
    /// <param name="right">The right value</param>
    /// <returns>the result of the difference of <paramref name="right"/> from <paramref name="left"/>.</returns>
#endif
    public static PositiveInteger operator checked -(PositiveInteger left, PositiveInteger right)
        => ( left.Value - right.Value ) switch
        {
            < 1 => throw new OverflowException(),
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
        => From(left.Value * right.Value);

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
    public static PositiveInteger operator checked *(PositiveInteger left, PositiveInteger right)
        => ( left.Value * right.Value ) switch
        {
            int result when result < left.Value && result < right.Value => throw new OverflowException(
                $@"multiplying ""{left}"" and ""{right}"" result is outside of {nameof(PositiveInteger)} values"),
            int result => From(result)
        };

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
    public static NonNegativeInteger operator /(PositiveInteger left, PositiveInteger right)
        => NonNegativeInteger.From(left.Value / right.Value);

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
    public static NonNegativeInteger operator *(PositiveInteger left, NonNegativeInteger right)
        => right switch
        {
            { Value: 0 } => right,
            { Value: 1 } => left,
            _ => From(left.Value * right.Value)
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

#if NETSTANDARD
    /// <summary>
    /// Converts the value of the current <see cref="PositiveInteger"/> object to its equivalent string representation using the specified format and culture-specific format information...
    /// </summary>
    /// <param name="format">A numeric format string.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <returns>A string representation of the value of the current <see cref="PositiveInteger"/> object as specified by <paramref name="format"/> and <paramref name="provider"/>.</returns>
    public string ToString(string format, IFormatProvider provider)
#else
    /// <inheritdoc />
    public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string format, IFormatProvider provider)
#endif
        => Value.ToString(format, provider);

#if NETSTANDARD
    /// <summary>
    /// Converts the value of the current <see cref="PositiveInteger"/> object to its equivalent string representation using the specified format and culture-specific format information...
    /// </summary>
    ///
#else
    /// <inheritdoc />
#endif
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider) => Value.TryFormat(destination, out charsWritten, format, provider);

    /// <summary>
    /// Implicitly cast to <see cref="int"/> type
    /// </summary>
    /// <param name="x"></param>
    public static implicit operator int(PositiveInteger x) => x.Value;

    /// <summary>
    /// Implicitly cast to <see cref="long"/> type
    /// </summary>
    /// <param name="x"></param>
    public static implicit operator long(PositiveInteger x) => x.Value;

    /// <summary>
    /// Implicitly cast to <see cref="ulong"/> type
    /// </summary>
    /// <param name="x"></param>
    public static implicit operator ulong(PositiveInteger x) => (ulong)x.Value;

    /// <summary>
    /// Implicitly cast to <see cref="decimal"/> type
    /// </summary>
    /// <param name="x"></param>
    public static implicit operator decimal(PositiveInteger x) => x.Value;

    /// <summary>
    /// Implicitly cast to <see cref="uint"/> type
    /// </summary>
    /// <param name="x"></param>
    public static implicit operator uint(PositiveInteger x) => (uint)x.Value;

    /// <summary>
    /// Implicitly cast to <see cref="NonNegativeInteger"/> type
    /// </summary>
    /// <param name="x"></param>
    public static implicit operator NonNegativeInteger(PositiveInteger x) => NonNegativeInteger.From(x.Value);

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Parses a string into a value.
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s"/>.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in correct format.</exception>
    /// <exception cref="OverflowException"><paramref name="s"/> is not representable by <see cref="PositiveInteger"/>.</exception>
#else
    /// <inheritdoc/>
#endif
    public static PositiveInteger Parse(string s, IFormatProvider provider)
    {
        int value = int.Parse(s, provider);
        return MinValue <= value && value <= MaxValue
                   ? From(value)
                   : throw new OverflowException($"""
                                                  "{s}" represents a value that is outside range of {nameof(PositiveInteger)} values
                                                  """);
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Tries to parse a string into a value.
    /// </summary>
    /// <param name="s">The string to parse</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s"/>.</param>
    /// <param name="result">The parsed value</param>
    /// <returns><see langword="true"/> if <paramref name="s"/> was parsed successfully and <paramref name="result"/> contains the parsed value, <see langword="false"/> otherwise.</returns>
#else
    /// <inheritdoc/>
#endif
    public static bool TryParse([NotNullWhen(true)] string s,
                                IFormatProvider provider,
                                [NotNullWhen(true)] out PositiveInteger result)
    {
#if NETSTANDARD2_1_OR_GREATER
        bool parsingDone = int.TryParse(s, NumberStyles.None, provider, out int value) && MinValue <= value && value <= MaxValue;
#else
        bool parsingDone = int.TryParse(s, provider, out int value) && MinValue <= value && value <= MaxValue;
#endif
        result = null;
        if (parsingDone)
        {
            result = From(value);
        }

        return parsingDone;
    }

#if NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// Parses a span of characters into a value.
    /// </summary>
    /// <param name="s">The span of characters to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s"/>.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in correct format.</exception>
    /// <exception cref="OverflowException"><paramref name="s"/> is not representable by <see cref="PositiveInteger"/>.</exception>
#else
    /// <inheritdoc />
#endif
    public static PositiveInteger Parse(ReadOnlySpan<char> s, IFormatProvider provider)
    {
        int value = int.Parse(s, NumberStyles.Integer, provider);
        return value < MinValue || value > MaxValue
#if NETSTANDARD2_1_OR_GREATER
                   ? throw new OverflowException($"""
                                                  "{s.ToString()}" represents a value that is outside range of {nameof(PositiveInteger)} values
                                                  """)
#else
                   ? throw new OverflowException($@"""{s}"" represents a value that is outside range of {nameof(PositiveInteger)} values")
#endif
                   : From(value);
    }


#if NETSTANDARD2_1_OR_GREATER
/// <summary>
/// Tries to parse a span of characters into a value.
/// </summary>
/// <param name="s">The span of characters to parse.</param>
/// <param name="provider">An object that provides culture-specific formatting information about <paramref name="s"/>.</param>
/// <param name="result">The parsed value</param>
/// <returns><see langword="true"/> if <paramref name="s"/> was parsed successfully and <paramref name="result"/> contains the parsed value, <see langword="false"/> otherwise.</returns>
/// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
#else
    /// <inheritdoc />
#endif
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, [NotNullWhen(true)] out PositiveInteger result)
    {
        bool parsingDone = int.TryParse(s, NumberStyles.Integer, provider, out int value) && MinValue <= value && value <= MaxValue;
        result = null;
        if (parsingDone)
        {
            result = From(value);
        }

        return parsingDone;
    }
}