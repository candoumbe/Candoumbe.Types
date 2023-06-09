using Bogus;

using Candoumbe.Types.Numerics;
using Candoumbe.Types.UnitTests.Generators;

using FluentAssertions;

using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

using System;
using System.Globalization;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Candoumbe.Types.UnitTests.Numerics;

[UnitTest]
[Feature(nameof(Numerics))]
public class NonNegativeLongTests
{
    private static readonly Faker Faker = new();
    private readonly ITestOutputHelper _outputHelper;
    private static readonly string[] StandardNumericFormats = { "c", "d", "e", "f", "g", "n", "p", "r", "x" };

    public NonNegativeLongTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Property(Arbitrary = new[] { typeof(ValueGenerators.NumericsTypes) })]
    public void Given_default_instance_Then_underlying_should_be_zero(NonNegativeLong initial)
    {
        // Arrange
        long expected = initial.Value;

        // Act
        NonNegativeLong numeric = NonNegativeLong.From(initial.Value);

        // Assert
        numeric.Should().Be(initial);
        numeric.Value.Should().Be(expected);
    }

    [Fact]
    public void MinValue_Should_Be_Equal_To_Zero()
    {
        // Act
        NonNegativeLong minValue = NonNegativeLong.MinValue;

        // Assert
        minValue.Should().Be(NonNegativeLong.Zero);
    }

    [Fact]
    public void MaxValue_Should_Be_Equal_To_intMaxValue()
    {
        // Act
        NonNegativeLong maxValue = NonNegativeLong.MaxValue;

        // Assert
        maxValue.Value.Should().Be(long.MaxValue);
    }

    [Property]
    public void Given_desiredValue_is_negative_When_calling_Create_Then_ArgumentOutOfRangeException_should_be_thrown(NegativeInt negativeInt)
    {
        // Act
        Action createNonNegativeLong = () => NonNegativeLong.From(negativeInt.Item);

        // Assert
        createNonNegativeLong.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Property]
    public void Given_any_NonNegativeLong_When_adding_AdditiveIdentity_Then_result_should_be_initial_value(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        NonNegativeLong result = initialValue + NonNegativeLong.AdditiveIdentity;

        // Assert
        result.Should().Be(initialValue + 1);
    }

    [Property]
    public void Given_two_NonNegativeLong_When_adding_them_together_Then_result_should_be_their_sum(NonNegativeInt leftValueGenerator, NonNegativeInt rightValueGenerator)
    {
        // Arrange
        NonNegativeLong left = NonNegativeLong.From(leftValueGenerator.Item);
        NonNegativeLong right = NonNegativeLong.From(rightValueGenerator.Item);

        // Act
        NonNegativeLong result = left + right;

        // Assert
        _ = (left.Value + right.Value) switch
        {
            < 0 => result.Value.Should().Be(0),
            _ => result.Value.Should().Be(left.Value + right.Value)
        };
    }

    [Property]
    public void Given_left_NonNegativeLong_and_right_is_an_integer_When_Adding_right_from_left_Then_result_should_be_NonNegativeLong(NonNegativeInt leftValueGenerator, long right)
    {
        // Arrange
        NonNegativeLong left = NonNegativeLong.From(leftValueGenerator.Item);

        // Act
        NonNegativeLong result = left + right;

        // Assert
        _ = (left.Value + right) switch
        {
            < 0 => result.Value.Should().Be(0),
            long actual => result.Value.Should().Be(actual)
        };
    }

    [Property]
    public void Given_left_and_right_NonNegativeLong_When_substracting_right_from_left_Then_result_should_be_their_difference(NonNegativeInt leftValueGenerator, NonNegativeInt rightValueGenerator)
    {
        // Arrange
        NonNegativeLong left = NonNegativeLong.From(leftValueGenerator.Item);
        NonNegativeLong right = NonNegativeLong.From(rightValueGenerator.Item);

        // Act
        NonNegativeLong result = left - right;

        // Assert
        _ = (left.Value - right.Value) switch
        {
            < 0 => result.Value.Should().Be(0),
            long actual => result.Value.Should().Be(actual)
        };
    }

    [Property]
    public void Given_left_NonNegativeLong_and_right_is_an_integer_When_substracting_right_from_left_Then_result_should_be_NonNegativeLong(NonNegativeInt leftValueGenerator, long right)
    {
        // Arrange
        NonNegativeLong left = NonNegativeLong.From(leftValueGenerator.Item);

        // Act
        NonNegativeLong result = left - right;

        // Assert
        _ = (left.Value - right) switch
        {
            < 0 => result.Value.Should().Be(0),
            long actual => result.Value.Should().Be(actual)
        };
    }

    [Property]
    public void Given_NonNegativeLong_When_casting_to_int_Then_result_should_equal_the_inner_value(NonNegativeInt nonNegativeInt)
    {
        // Arrange
        long expected = nonNegativeInt.Item;
        NonNegativeLong nonNegativeLong = NonNegativeLong.From(expected);

        // Act
        long actual = nonNegativeLong;

        // Assert
        actual.Should().Be(nonNegativeLong.Value)
                       .And
                       .Be(expected);
    }

    [Property]
    public void Two_NonNegativeLong_are_equal_when_their_value_are_equal(NonNegativeInt leftValueGenerator, NonNegativeInt rightValueGenerator)
    {
        // Arrange
        NonNegativeLong left = NonNegativeLong.From(leftValueGenerator.Item);
        NonNegativeLong right = NonNegativeLong.From(rightValueGenerator.Item);

        // Act
        bool actual = left == right;

        // Assert
        actual.Should().Be(left.Value == right.Value);
    }

    [Property]
    public void Given_left_is_NonNegativeLong_and_right_is_an_int_When_left_value_equal_right_Then_left_and_right_should_be_equal(NonNegativeInt leftValueGenerator, long right)
    {
        // Arrange
        NonNegativeLong left = NonNegativeLong.From(leftValueGenerator.Item);

        // Act
        bool actual = left == right;

        // Assert
        actual.Should().Be(left.Value == right);
    }

    [Property]
    public void Given_a_NonNegativeLong_When_adding_one_Then_the_result_should_be_greater_than_initialValue(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        NonNegativeLong actual = initialValue + 1;

        // Assert
        actual.Value.Should().Be(initialValue.Value switch
        {
            long.MaxValue => long.MaxValue,
            long value => value + 1
        });
    }

    [Property]
    public void Given_a_NonNegativeLong_When_subtracting_one_Then_the_result_should_be_less_than_initialValue(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong left = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        NonNegativeLong actual = left - 1;

        // Assert
        actual.Value.Should().Be(left.Value switch
        {
            0 => 0,
            long value => value - 1
        });
    }

    [Property]
    public void Given_a_NonNegativeLong_When_implicitly_casting_to_a_long_Then_value_should_be_equal_to_initialValue(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        long actual = initialValue;

        // Assert
        actual.Should().Be(initialValue.Value);
    }

    [Property]
    public Property Given_a_NonNegativeLong_When_implicitly_casting_to_a_ulong_Then_value_should_be_equal_to_initialValue(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        ulong actual = initialValue;

        // Assert
        return (actual == initialValue).ToProperty();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_Abs_Then_result_should_be_the_value(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        NonNegativeLong actual = NonNegativeLong.Abs(initialValue);

        // Assert
        actual.Should().Be(initialValue);
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsCanonical_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsCanonical(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsOddLong_on_it_Then_result_should_be_true_when_inner_integer_is_odd(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsOddInteger(initialValue);

        // Assert
        actual.Should().Be(initialValue.Value % 2 != 0);
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsEvenLong_on_it_Then_result_should_be_true_when_inner_integer_is_even(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsEvenInteger(initialValue);

        // Assert
        actual.Should().Be(initialValue.Value % 2 == 0);
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsComplexNumber_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsComplexNumber(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsFinite_on_it_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsFinite(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsImaginary_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsImaginaryNumber(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsInfinity_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsInfinity(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsLong_on_it_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsLong(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsNaN_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsNaN(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsNegative_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsNegative(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsPositiveInfinity_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsPositiveInfinity(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsPositive_on_it_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsPositive(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsRealNumber_on_it_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsRealNumber(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeLong_When_calling_IsZero_on_it_Then_result_should_be_same_as_initialValue_eq_NonNegativeLongZero(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeLong.IsZero(initialValue);

        // Assert
        actual.Should().Be(initialValue == NonNegativeLong.Zero);
    }

    [Property]
    public void Given_two_NonNegativeLong_When_calling_MaxMagnitude_Then_result_should_return_the_greater_of_the_two(NonNegativeInt xValueGenerator, NonNegativeInt yValueGenerator)
    {
        // Arrange
        NonNegativeLong x = NonNegativeLong.From(xValueGenerator.Item);
        NonNegativeLong y = NonNegativeLong.From(yValueGenerator.Item);

        // Act
        NonNegativeLong actual = NonNegativeLong.MaxMagnitude(x, y);

        // Assert
        _ = (x.Value < y.Value) switch
        {
            true => actual.Should().Be(y),
            _ => actual.Should().Be(x)
        };
    }

    [Property]
    public void Given_two_NonNegativeLong_When_calling_MaxMagnitudeNumber_Then_result_should_return_the_greater_of_the_two(NonNegativeInt xValueGenerator, NonNegativeInt yValueGenerator)
    {
        // Arrange
        NonNegativeLong x = NonNegativeLong.From(xValueGenerator.Item);
        NonNegativeLong y = NonNegativeLong.From(yValueGenerator.Item);

        // Act
        NonNegativeLong actual = NonNegativeLong.MaxMagnitudeNumber(x, y);

        // Assert
        _ = (x.Value < y.Value) switch
        {
            true => actual.Should().Be(y),
            _ => actual.Should().Be(x)
        };
    }

    [Property]
    public void Given_two_NonNegativeLong_When_calling_MinMagnitude_Then_result_should_return_the_smaller_of_the_two(NonNegativeInt xValueGenerator, NonNegativeInt yValueGenerator)
    {
        // Arrange
        NonNegativeLong x = NonNegativeLong.From(xValueGenerator.Item);
        NonNegativeLong y = NonNegativeLong.From(yValueGenerator.Item);

        // Act
        NonNegativeLong actual = NonNegativeLong.MinMagnitude(x, y);

        // Assert
        _ = (x.Value < y.Value) switch
        {
            true => actual.Should().Be(x),
            _ => actual.Should().Be(y)
        };
    }

    [Property]
    public void Given_two_NonNegativeLong_When_calling_MinMagnitudeNumber_Then_result_should_return_the_smaller_of_the_two(NonNegativeInt xValueGenerator, NonNegativeInt yValueGenerator)
    {
        // Arrange
        NonNegativeLong x = NonNegativeLong.From(xValueGenerator.Item);
        NonNegativeLong y = NonNegativeLong.From(yValueGenerator.Item);

        // Act
        NonNegativeLong actual = NonNegativeLong.MinMagnitudeNumber(x, y);

        // Assert
        _ = (x.Value < y.Value) switch
        {
            true => actual.Should().Be(x),
            _ => actual.Should().Be(y)
        };
    }

    [Property]
    public void Given_a_string_representing_a_NonNegativeLong_and_the_format_When_calling_Parse_Then_the_result_should_be_equal_to_the_original_value(NonNegativeInt initialValueGenerator, CultureInfo culture)
    {
        // Arrange
        NonNegativeLong initialValue = NonNegativeLong.From(initialValueGenerator.Item);
        string initial = initialValue.ToString();

        // Act
        NonNegativeLong actual = NonNegativeLong.Parse(initial, culture.NumberFormat);

        // Assert
        actual.Should().Be(initialValue);
    }

    [Property]
    public void Given_a_string_representing_a_value_outside_NonNegativeLong_Ranges_When_calling_Parse_Then_OverflowException_should_be_thrown(CultureInfo culture)
    {
        // Arrange
        long value = Faker.Random.Long(max: ((long)NonNegativeLong.MinValue) - 1);

        string initial = value.ToString(culture.NumberFormat);

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        Action parsingValueThatIsOutsideNonNegativeLongRange = () => NonNegativeLong.Parse(initial, culture.NumberFormat);

        // Assert
        parsingValueThatIsOutsideNonNegativeLongRange.Should()
                                                     .Throw<OverflowException>($@"original value ""{initial}"" is outside the  [{NonNegativeLong.MinValue} - {NonNegativeLong.MaxValue}] range value");
    }

    [Property]
    public void Given_a_string_representing_a_value_outside_range_of_NonNegativeLong_values_When_calling_TryParse_Then_should_be_false(CultureInfo culture)
    {
        // Arrange
        long value = Faker.Random.Long(max: ((long)NonNegativeLong.MinValue) - 1);

        string initial = value.ToString(culture.NumberFormat);

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        bool actual = NonNegativeLong.TryParse(initial, culture.NumberFormat, out _);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_string_representing_a_value_inside_range_of_NonNegativeLong_values_When_calling_TryParse_Then_should_be_true(CultureInfo culture)
    {
        // Arrange
        long value = Faker.PickRandom(NonNegativeLong.MinValue, NonNegativeLong.MaxValue);

        string initial = value.ToString(culture.NumberFormat);

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        bool actual = NonNegativeLong.TryParse(initial, culture.NumberFormat, out _);

        // Assert
        actual.Should().BeTrue();
    }

#if NET7_0_OR_GREATER
    [Property]
    public void Given_a_ReadOnlySpan_representing_a_value_outside_range_of_NonNegativeLong_values_When_calling_TryParse_Then_should_be_false(CultureInfo culture)
    {
        // Arrange
        long value = Faker.Random.Long(max: ((long)NonNegativeLong.MinValue) - 1);

        ReadOnlySpan<char> initial = value.ToString(culture.NumberFormat).AsSpan();

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        bool actual = NonNegativeLong.TryParse(initial, culture.NumberFormat, out _);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_ReadOnlySpan_representing_a_value_outside_range_of_NonNegativeLong_values_When_calling_TryParse_Then_should_be_true(CultureInfo culture)
    {
        // Arrange
        string format = $"{Faker.PickRandom(StandardNumericFormats)}{Faker.PickRandom(1, 9)}";
        const ulong value = ulong.MaxValue;

        ReadOnlySpan<char> initial = value.ToString(culture.NumberFormat).AsSpan();

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        bool actual = NonNegativeLong.TryParse(initial, culture.NumberFormat, out _);

        // Assert
        actual.Should().BeFalse($"The value to parse is outside the range of {nameof(NonNegativeLong)} values");
    }
#endif

    [Property(Arbitrary = new[] { typeof(ValueGenerators) })]
    public Property Given_an_existing_NonNegativeInteger_When_multiplying_by_multiplicative_identity_Then_the_result_should_be_equal_to_the_initial_value(NonNegativeLong initial)
    => ((initial * NonNegativeLong.MultiplicativeIdentity) == initial).ToProperty();

    [Property(Arbitrary = new[] { typeof(ValueGenerators) })]
    public void Given_an_existing_NonNegativeInteger_When_adding_additive_identity_Then_the_result_should_be_equal_to_the_initial_value(NonNegativeLong initial)
        => ((initial + NonNegativeLong.AdditiveIdentity) == initial).ToProperty();


    [Property]
    public void Given_a_NonNegativeLong_When_implicitely_casting_to_int32_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initial = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        long actual = initial;

        // Assert
        actual.Should().Be(initial.Value);
    }

    [Property]
    public void Given_a_NonNegativeLong_When_implicitely_casting_to_long_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initial = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        long actual = initial;

        // Assert
        actual.Should().Be(initial.Value);
    }

    [Property]
    public void Given_a_NonNegativeLong_When_implicitely_casting_to_decimal_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initial = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        decimal actual = initial;

        // Assert
        actual.Should().Be(initial);
    }

    [Property]
    public void Given_a_NonNegativeLong_When_implicitely_casting_to_uint_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        NonNegativeLong initial = NonNegativeLong.From(initialValueGenerator.Item);

        // Act
        uint actual = initial;

        // Assert
        actual.Should().Be(initial);
    }

    [Property]
    public void Given_a_NonNegativeLong_When_adding_a_non_negative_value_Then_result_should_be_gte_the_original_value(PositiveInt positiveIntGenerator, NonNegativeInt nonNegativeIntGenerator)
    {
        // Arrange
        PositiveLong positiveLong = PositiveLong.From(positiveIntGenerator.Item);
        NonNegativeLong nonNegativeLong = NonNegativeLong.From(nonNegativeIntGenerator.Item);

        // Act
        NonNegativeLong actual = nonNegativeLong + positiveLong;

        // Assert
        actual.Should().Be(NonNegativeLong.From(positiveLong.Value + nonNegativeLong.Value));
    }

    [Property]
    public void Given_a_NonNegativeLong_When_multiplying_with_a_PositiveLong_Then_result_should_be_a_NonNegativeValue(PositiveInt positiveIntGenerator, NonNegativeInt nonNegativeIntGenerator)
    {
        // Arrange
        PositiveLong positiveLong = PositiveLong.From(positiveIntGenerator.Item);
        NonNegativeLong nonNegativeLong = NonNegativeLong.From(nonNegativeIntGenerator.Item);

        // Act
        NonNegativeLong actual = nonNegativeLong * positiveLong;

        // Assert
        actual.Should().Be(NonNegativeLong.From(positiveLong.Value * nonNegativeLong.Value));
    }
}
