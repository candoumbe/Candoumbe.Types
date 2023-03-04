using Bogus;

using Candoumbe.Types.Numerics;

using FluentAssertions;

using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Candoumbe.Types.UnitTests.Numerics;

[UnitTest]
[Feature(nameof(Numerics))]
public class NonNegativeIntegerTests
{
    private static readonly Faker Faker = new();
    private readonly ITestOutputHelper _outputHelper;
    private static readonly string[] StandardNumericFormats = {"c", "d", "e", "f", "g", "n", "p", "r", "x"};

    public NonNegativeIntegerTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void Given_default_instance_Then_underlying_should_be_zero()
    {
        // Act
        NonNegativeInteger numeric = default;

        // Assert
        numeric.Value.Should().Be(0);
    }

    [Fact]
    public void MinValue_Should_Be_Equal_To_Zero()
    {
        // Act
        NonNegativeInteger minValue = NonNegativeInteger.MinValue;

        // Assert
        minValue.Should().Be(NonNegativeInteger.Zero);
    }

    [Fact]
    public void MaxValue_Should_Be_Equal_To_intMaxValue()
    {
        // Act
        NonNegativeInteger maxValue = NonNegativeInteger.MaxValue;

        // Assert
        maxValue.Value.Should().Be(int.MaxValue);
    }

    [Property]
    public void Given_desiredValue_is_negative_When_calling_Create_Then_ArgumentOutOfRangeException_should_be_thrown(NegativeInt negativeInt)
    {
        // Act
        Action createNonNegativeInteger = () => NonNegativeInteger.From(negativeInt.Item);

        // Assert
        createNonNegativeInteger.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Property]
    public void Given_any_NonNegativeInteger_When_adding_AdditiveIdentity_Then_result_should_be_initial_value(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        NonNegativeInteger result = initialValue + NonNegativeInteger.AdditiveIdentity;

        // Assert
        result.Should().Be(initialValue);
    }

    [Property]
    public void Given_two_NonNegativeInteger_When_adding_them_together_Then_result_should_be_their_sum(NonNegativeInt leftValueGenerator, NonNegativeInt rightValueGenerator)
    {
        // Arrange
        NonNegativeInteger left = NonNegativeInteger.From(leftValueGenerator.Item);
        NonNegativeInteger right = NonNegativeInteger.From(rightValueGenerator.Item);

        // Act
        NonNegativeInteger result = left + right;

        // Assert
        _ = (left.Value + right.Value) switch
        {
            < 0 => result.Value.Should().Be(0),
            _ => result.Value.Should().Be(left.Value + right.Value)
        };
    }

    [Property]
    public void Given_left_NonNegativeInteger_and_right_is_an_integer_When_Adding_right_from_left_Then_result_should_be_NonNegativeInteger(NonNegativeInt leftValueGenerator, int right)
    {
        // Arrange
        NonNegativeInteger left = NonNegativeInteger.From(leftValueGenerator.Item);

        // Act
        NonNegativeInteger result = left + right;

        // Assert
        _ = (left.Value + right) switch
        {
            < 0 => result.Value.Should().Be(0),
            int actual => result.Value.Should().Be(actual)
        };
    }

    [Property]
    public void Given_left_and_right_NonNegativeInteger_When_substracting_right_from_left_Then_result_should_be_their_difference(NonNegativeInt leftValueGenerator, NonNegativeInt rightValueGenerator)
    {
        // Arrange
        NonNegativeInteger left = NonNegativeInteger.From(leftValueGenerator.Item);
        NonNegativeInteger right = NonNegativeInteger.From(rightValueGenerator.Item);

        // Act
        NonNegativeInteger result = left - right;

        // Assert
        _ = (left.Value - right.Value) switch
        {
            < 0 => result.Value.Should().Be(0),
            int actual => result.Value.Should().Be(actual)
        };
    }

    [Property]
    public void Given_left_NonNegativeInteger_and_right_is_an_integer_When_substracting_right_from_left_Then_result_should_be_NonNegativeInteger(NonNegativeInt leftValueGenerator, int right)
    {
        // Arrange
        NonNegativeInteger left = NonNegativeInteger.From(leftValueGenerator.Item);

        // Act
        NonNegativeInteger result = left - right;

        // Assert
        _ = (left.Value - right) switch
        {
            < 0 => result.Value.Should().Be(0),
            int actual => result.Value.Should().Be(actual)
        };
    }

    [Property]
    public void Given_NonNegativeInteger_When_casting_to_int_Then_result_should_equal_the_inner_value(NonNegativeInt nonNegativeInt)
    {
        // Arrange
        int expected = nonNegativeInt.Item;
        NonNegativeInteger nonNegativeInteger = NonNegativeInteger.From(expected);

        // Act
        int actual = nonNegativeInteger;

        // Assert
        actual.Should().Be(nonNegativeInteger.Value)
                       .And
                       .Be(expected);
    }

    [Property]
    public void Two_NonNegativeInteger_are_equal_when_their_value_are_equal(NonNegativeInt leftValueGenerator, NonNegativeInt rightValueGenerator)
    {
        // Arrange
        NonNegativeInteger left = NonNegativeInteger.From(leftValueGenerator.Item);
        NonNegativeInteger right = NonNegativeInteger.From(rightValueGenerator.Item);

        // Act
        bool actual = left == right;

        // Assert
        actual.Should().Be(left.Value == right.Value);
    }

    [Property]
    public void Given_left_is_NonNegativeInteger_and_right_is_an_int_When_left_value_equal_right_Then_left_and_right_should_be_equal(NonNegativeInt leftValueGenerator, int right)
    {
        // Arrange
        NonNegativeInteger left = NonNegativeInteger.From(leftValueGenerator.Item);

        // Act
        bool actual = left == right;

        // Assert
        actual.Should().Be(left.Value == right);
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_adding_one_Then_the_result_should_be_greater_than_initialValue(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        NonNegativeInteger actual = initialValue + 1;

        // Assert
        actual.Value.Should().Be(initialValue.Value switch
        {
            int.MaxValue => int.MaxValue,
            int value => value + 1
        });
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_subtracting_one_Then_the_result_should_be_less_than_initialValue(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger left = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        NonNegativeInteger actual = left - 1;

        // Assert
        actual.Value.Should().Be(left.Value switch
        {
            0 => 0,
            int value => value - 1
        });
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_implicitly_casting_to_a_long_Then_value_should_be_equal_to_initialValue(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        long actual = initialValue;

        // Assert
        actual.Should().Be(initialValue.Value);
    }

    [Property]
    public Property Given_a_NonNegativeInteger_When_implicitly_casting_to_a_ulong_Then_value_should_be_equal_to_initialValue(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        ulong actual = initialValue;

        // Assert
        return (actual == initialValue).ToProperty();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_Abs_Then_result_should_be_the_value(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        NonNegativeInteger actual = NonNegativeInteger.Abs(initialValue);

        // Assert
        actual.Should().Be(initialValue);
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsCanonical_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsCanonical(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsOddInteger_on_it_Then_result_should_be_true_when_inner_integer_is_odd(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsOddInteger(initialValue);

        // Assert
        actual.Should().Be(initialValue.Value % 2 != 0);
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsEvenInteger_on_it_Then_result_should_be_true_when_inner_integer_is_even(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsEvenInteger(initialValue);

        // Assert
        actual.Should().Be(initialValue.Value % 2 == 0);
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsComplexNumber_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsComplexNumber(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsFinite_on_it_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsFinite(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsImaginary_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsImaginaryNumber(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsInfinity_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsInfinity(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsInteger_on_it_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsInteger(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsNaN_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsNaN(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsNegative_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsNegative(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsPositiveInfinity_on_it_Then_result_should_be_false(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsPositiveInfinity(initialValue);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsPositive_on_it_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsPositive(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsRealNumber_on_it_Then_result_should_be_true(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsRealNumber(initialValue);

        // Assert
        actual.Should().BeTrue();
    }

    [Property]
    public void Given_a_NonNegativeInteger_When_calling_IsZero_on_it_Then_result_should_be_same_as_initialValue_eq_NonNegativeIntegerZero(NonNegativeInt initialValueGenerator)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);

        // Act
        bool actual = NonNegativeInteger.IsZero(initialValue);

        // Assert
        actual.Should().Be(initialValue == NonNegativeInteger.Zero);
    }

    [Property]
    public void Given_two_NonNegativeInteger_When_calling_MaxMagnitude_Then_result_should_return_the_greater_of_the_two(NonNegativeInt xValueGenerator, NonNegativeInt yValueGenerator)
    {
        // Arrange
        NonNegativeInteger x = NonNegativeInteger.From(xValueGenerator.Item);
        NonNegativeInteger y = NonNegativeInteger.From(yValueGenerator.Item);

        // Act
        NonNegativeInteger actual = NonNegativeInteger.MaxMagnitude(x, y);

        // Assert
        _ = (x.Value < y.Value) switch
        {
            true => actual.Should().Be(y),
            _ => actual.Should().Be(x)
        };
    }

    [Property]
    public void Given_two_NonNegativeInteger_When_calling_MaxMagnitudeNumber_Then_result_should_return_the_greater_of_the_two(NonNegativeInt xValueGenerator, NonNegativeInt yValueGenerator)
    {
        // Arrange
        NonNegativeInteger x = NonNegativeInteger.From(xValueGenerator.Item);
        NonNegativeInteger y = NonNegativeInteger.From(yValueGenerator.Item);

        // Act
        NonNegativeInteger actual = NonNegativeInteger.MaxMagnitudeNumber(x, y);

        // Assert
        _ = (x.Value < y.Value) switch
        {
            true => actual.Should().Be(y),
            _ => actual.Should().Be(x)
        };
    }

    [Property]
    public void Given_two_NonNegativeInteger_When_calling_MinMagnitude_Then_result_should_return_the_smaller_of_the_two(NonNegativeInt xValueGenerator, NonNegativeInt yValueGenerator)
    {
        // Arrange
        NonNegativeInteger x = NonNegativeInteger.From(xValueGenerator.Item);
        NonNegativeInteger y = NonNegativeInteger.From(yValueGenerator.Item);

        // Act
        NonNegativeInteger actual = NonNegativeInteger.MinMagnitude(x, y);

        // Assert
        _ = (x.Value < y.Value) switch
        {
            true => actual.Should().Be(x),
            _ => actual.Should().Be(y)
        };
    }

    [Property]
    public void Given_two_NonNegativeInteger_When_calling_MinMagnitudeNumber_Then_result_should_return_the_smaller_of_the_two(NonNegativeInt xValueGenerator, NonNegativeInt yValueGenerator)
    {
        // Arrange
        NonNegativeInteger x = NonNegativeInteger.From(xValueGenerator.Item);
        NonNegativeInteger y = NonNegativeInteger.From(yValueGenerator.Item);

        // Act
        NonNegativeInteger actual = NonNegativeInteger.MinMagnitudeNumber(x, y);

        // Assert
        _ = (x.Value < y.Value) switch
        {
            true => actual.Should().Be(x),
            _ => actual.Should().Be(y)
        };
    }

    [Property]
    public void Given_a_string_representing_a_NonNegativeInteger_and_the_format_When_calling_Parse_Then_the_result_should_be_equal_to_the_original_value(NonNegativeInt initialValueGenerator, CultureInfo culture)
    {
        // Arrange
        NonNegativeInteger initialValue = NonNegativeInteger.From(initialValueGenerator.Item);
        string initial = initialValue.ToString();

        // Act
        NonNegativeInteger actual = NonNegativeInteger.Parse(initial, culture.NumberFormat);

        // Assert
        actual.Should().Be(initialValue);
    }

    [Property]
    public void Given_a_string_representing_a_value_outside_NonNegativeInteger_Ranges_When_calling_Parse_Then_OverflowException_should_be_thrown(CultureInfo culture)
    {
        // Arrange
        long value = Faker.PickRandom( new [] { 
            Faker.Random.Long(max: ((long)NonNegativeInteger.MinValue) - 1),
            Faker.Random.Long(min: ((long)NonNegativeInteger.MaxValue) + 1)
        });

        string initial = value.ToString(culture.NumberFormat);

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        Action parsingValueThatIsOutsideNonNegativeIntegerRange = () => NonNegativeInteger.Parse(initial, culture.NumberFormat);

        // Assert
        parsingValueThatIsOutsideNonNegativeIntegerRange.Should()
                                                        .Throw<OverflowException>($@"original value ""{initial}"" is outside the  [{NonNegativeInteger.MinValue} - {NonNegativeInteger.MaxValue}] range value");
    }

    [Property]
    public void Given_a_string_representing_a_value_outside_range_of_NonNegativeInteger_values_When_calling_TryParse_Then_should_be_false(CultureInfo culture)
    {
        // Arrange
        long value = Faker.PickRandom(new[] {
            Faker.Random.Long(max: ((long)NonNegativeInteger.MinValue) - 1),
            Faker.Random.Long(min: ((long)NonNegativeInteger.MaxValue) + 1)
        });

        string initial = value.ToString(culture.NumberFormat);

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        bool actual = NonNegativeInteger.TryParse(initial, culture.NumberFormat, out _);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_string_representing_a_value_inside_range_of_NonNegativeInteger_values_When_calling_TryParse_Then_should_be_true(CultureInfo culture)
    {
        // Arrange
        long value = Faker.PickRandom(NonNegativeInteger.MinValue, NonNegativeInteger.MaxValue);

        string initial = value.ToString(culture.NumberFormat);

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        bool actual = NonNegativeInteger.TryParse(initial, culture.NumberFormat, out _);

        // Assert
        actual.Should().BeTrue();
    }

#if NET7_0_OR_GREATER
    [Property]
    public void Given_a_ReadOnlySpan_representing_a_value_outside_range_of_NonNegativeInteger_values_When_calling_TryParse_Then_should_be_false(CultureInfo culture)
    {
        // Arrange
        long value = Faker.PickRandom(new[] {
            Faker.Random.Long(max: ((long)NonNegativeInteger.MinValue) - 1),
            Faker.Random.Long(min: ((long)NonNegativeInteger.MaxValue) + 1)
        });

        ReadOnlySpan<char> initial = value.ToString(culture.NumberFormat).AsSpan();

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        bool actual = NonNegativeInteger.TryParse(initial, culture.NumberFormat, out _);

        // Assert
        actual.Should().BeFalse();
    }

    [Property]
    public void Given_a_ReadOnlySpan_representing_a_value_inside_range_of_NonNegativeInteger_values_When_calling_TryParse_Then_should_be_true(CultureInfo culture)
    {
        // Arrange
        string format = $"{Faker.PickRandom(StandardNumericFormats)}{Faker.PickRandom(1, 9)}";
        long value = Faker.PickRandom(NonNegativeInteger.MinValue, NonNegativeInteger.MaxValue);

        ReadOnlySpan<char> initial = value.ToString(culture.NumberFormat).AsSpan();

        _outputHelper.WriteLine($"{nameof(initial)} : '{initial}'");

        // Act
        bool actual = NonNegativeInteger.TryParse(initial, culture.NumberFormat, out _);

        // Assert
        actual.Should().BeTrue();
    } 
#endif
}
