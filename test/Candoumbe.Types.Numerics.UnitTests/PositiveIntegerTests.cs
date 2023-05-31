using System;
using Bogus;
using Candoumbe.Types.Numerics.UnitTests.Generators;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace Candoumbe.Types.Numerics.UnitTests;

public class PositiveIntegerTests
{
    private readonly ITestOutputHelper _outputHelper;
    private static readonly Faker Faker = new();

    public PositiveIntegerTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Property]
    public void Given_input_is_less_than_1_When_calling_From_to_build_an_instance_Then_an_exception_should_be_thrown()
    {
        // Arrange
        int input = Faker.Random.Int(max: 0);

        // Act
        Action callingFrom = () => PositiveInteger.From(input);

        // Assert
        callingFrom.Should()
                   .ThrowExactly<ArgumentOutOfRangeException>($"{input} is out of [{PositiveLong.MinValue} - {PositiveLong.MaxValue}] range of values")
                   .Where(ex => !string.IsNullOrWhiteSpace(ex.Message), "the message of the exception helps understanding the issue")
                   .Where(ex => Equals(ex.ActualValue, input), "having the actual value that caused the exception can help when debbuging");
    }

    [Property(Arbitrary = new[] { typeof(ValueGenerators) })]
    public void Given_two_positive_integers_When_comparing_them_Then_the_result_should_be_the_same_as_comparing_their_underlying_values(PositiveInteger left, PositiveInteger right)
    {
        // Assert
        (left <= right).Should().Be(left.Value <= right.Value);
        (left < right).Should().Be(left.Value < right.Value);
        (left >= right).Should().Be(left.Value >= right.Value);
        (left > right).Should().Be(left.Value > right.Value);
        (left == right).Should().Be(left.Value == right.Value);
        (left != right).Should().Be(left.Value != right.Value);
    }

    [Property(Arbitrary = new[] { typeof(ValueGenerators) })]
    public void Given_two_positive_integers_and_in_a_checked_When_adding_those_values_together_Then_the_result_should_be_withing_range_of_positive_values(PositiveInteger left, PositiveInteger right)
    {
        // Act
        PositiveInteger result = checked(left + right);

        // Assert
        result.Should()
              .BeInRange(PositiveInteger.MinValue, PositiveInteger.MaxValue);
        result.Should().Be(checked(right + left));
    }

    [Property]
    public void Given_two_PositiveIntegers_When_doing_Subtraction_with_unchecked_Then_ShouldReturnCorrectResult(PositiveInt leftValueGenerator, PositiveInt rightValueGenerator)
    {
        // Arrange
        PositiveInteger left = PositiveInteger.From(Math.Max(1, leftValueGenerator.Item));
        PositiveInteger right = PositiveInteger.From(Math.Max(1, rightValueGenerator.Item));

        _outputHelper.WriteLine($"{nameof(left)} : '{left}'");
        _outputHelper.WriteLine($"{nameof(right)} : '{right}'");

        // Act
        PositiveInteger result = unchecked(left - right);

        // Assert
        object _ = (leftValueGenerator.Item - rightValueGenerator.Item) switch
        {
            int value when value >= 1 => result.Should().Be(PositiveInteger.From(value), $"a positive integer can never hold a value less than {PositiveInteger.MinValue.Value}"),
            int value when value < 1 => result.Should().Be(PositiveInteger.From(PositiveInteger.MaxValue.Value - Math.Abs(value)), $"a positive integer can never hold a value less than {PositiveInteger.MinValue.Value}"),
            int value => result.Value.Should().Be(value)
        };
    }

    [Property(Arbitrary = new[] { typeof(ValueGenerators) })]
    public void Given_two_PositiveIntegers_and_checked_context_When_multiplying_them_together_Then_the_result_should_be_correct(PositiveInteger left, PositiveInteger right)
    {
        // Act
        PositiveInteger result = checked(left * right);

        // Assert
        result.Should()
              .BeInRange(PositiveInteger.MinValue, PositiveInteger.MaxValue)
              .And.Be(right * left, "The multîplication is commutative");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_PositiveInteger_When_multiplying_by_NonNegativeZero_Then_result_should_be_NonNegativeZero(PositiveInteger left)
    {
        // Arrange
        NonNegativeInteger zero = NonNegativeInteger.Zero;

        // Act
        NonNegativeInteger actual = left * zero;

        // Assert
        actual.Should().Be(NonNegativeInteger.Zero);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_PositiveInteger_When_multiplying_by_NonNegativeInteger_Then_result_should_be_a_NonNegativeInteger_which_value_is_left_times_right(PositiveInteger left, NonNegativeInteger right)
    {
        // Arrange
        NonNegativeInteger expected = NonNegativeInteger.From(left * right);

        // Act
        NonNegativeInteger actual = left * right;

        // Assert
        actual.Should().Be(expected);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_PositiveInteger_When_multiplying_by_NonNegative_identity_Then_result_should_be_the_left_value(PositiveInteger left)
    {
        // Arrange
        NonNegativeInteger identity = NonNegativeInteger.MultiplicativeIdentity;

        // Act
        NonNegativeInteger actual = left * identity;

        // Assert
        actual.Should().Be(NonNegativeInteger.From(left));
    }

    [Property]
    public void Given_PositiveIntegers_When_LessThan_Then_ShouldReturnCorrectResult(int left, int right)
    {
        // Arrange
        left = Math.Max(1, left);
        right = Math.Max(1, right);
        PositiveInteger positiveA = PositiveInteger.From(left);
        PositiveInteger positiveB = PositiveInteger.From(right);

        // Act
        bool result = positiveA < positiveB;

        // Assert
        result.Should().Be(left < right);
    }

    [Property]
    public void Given_PositiveIntegers_When_LessThanOrEqual_Then_ShouldReturnCorrectResult(PositiveInt leftValue, PositiveInt rightValue)
    {
        // Arrange
        PositiveInteger left = PositiveInteger.From(leftValue.Item);
        PositiveInteger right = PositiveInteger.From(rightValue.Item);

        // Act
        bool result = left <= right;

        // Assert
        result.Should().Be(left.Value <= right.Value);
    }

    [Property]
    public void Given_PositiveIntegers_When_GreaterThan_Then_ShouldReturnCorrectResult(PositiveInt leftValue, PositiveInt rightValue)
    {
        // Arrange
        PositiveInteger left = PositiveInteger.From(leftValue.Item);
        PositiveInteger right = PositiveInteger.From(rightValue.Item);

        // Act
        bool result = left > right;

        // Assert
        result.Should().Be(left.Value > right.Value);
    }

    [Property]
    public void Given_PositiveIntegers_When_GreaterThanOrEqual_Then_ShouldReturnCorrectResult(PositiveInt leftValue, PositiveInt rightValue)
    {
        // Arrange
        PositiveInteger left = PositiveInteger.From(leftValue.Item);
        PositiveInteger right = PositiveInteger.From(rightValue.Item);

        // Act
        bool result = left >= right;

        // Assert
        result.Should().Be(left.Value >= right.Value);
    }

    [Property]
    public void Given_a_PositiveInteger_When_implicitely_casting_to_int32_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        PositiveInteger initial = PositiveInteger.From(initialValueGenerator.Item);

        // Act
        int actual = initial;

        // Assert
        actual.Should().Be(initial.Value);
    }

    [Property]
    public void Given_a_PositiveInteger_When_implicitely_casting_to_long_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        PositiveInteger initial = PositiveInteger.From(initialValueGenerator.Item);

        // Act
        long actual = initial;

        // Assert
        actual.Should().Be(initial.Value);
    }

    [Property]
    public void Given_a_PositiveInteger_When_implicitly_casting_to_decimal_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        PositiveInteger initial = PositiveInteger.From(initialValueGenerator.Item);

        // Act
        decimal actual = initial;

        // Assert
        actual.Should().Be(initial.Value);
    }

    [Property]
    public void Given_a_PositiveInteger_When_implicitely_casting_to_uint_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        PositiveInteger initial = PositiveInteger.From(initialValueGenerator.Item);

        // Act
        uint actual = initial;

        // Assert
        actual.Should().Be(initial);
    }

    [Property]
    public void Given_a_PositiveInteger_When_implicitely_casting_to_ulong_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        PositiveInteger initial = PositiveInteger.From(initialValueGenerator.Item);

        // Act
        ulong actual = initial;

        // Assert
        actual.Should().Be(initial);
    }

    [Property]
    public void Given_a_PositiveInteger_When_adding_a_non_negative_value_Then_result_should_be_gte_the_original_value(PositiveInt positiveIntGenerator, NonNegativeInt nonNegativeIntGenerator)
    {
        // Arrange
        PositiveInteger positiveInteger = PositiveInteger.From(positiveIntGenerator.Item);
        NonNegativeInteger nonNegativeInteger = NonNegativeInteger.From(nonNegativeIntGenerator.Item);

        // Act
        PositiveInteger actual = positiveInteger + nonNegativeInteger;

        // Assert
        actual.Should().Be(PositiveInteger.From(positiveInteger.Value + nonNegativeInteger.Value));
    }

    [Property]
    public void Given_a_PositiveInteger_When_dividing_by_a_PositiveInteger_Then_result_should_be_NonNegativeInteger(PositiveInt leftGenerator, PositiveInt rightGenerator)
    {
        // Arrange
        PositiveInteger left = PositiveInteger.From(leftGenerator.Item);
        PositiveInteger right = PositiveInteger.From(rightGenerator.Item);

        // Act
        NonNegativeInteger actual = left / right;

        // Assert
        actual.Should().Be(NonNegativeInteger.From(left.Value / right.Value));
    }

    [Property]
    public void Given_a_PositiveInteger_is_MaxValue_and_checked_state_When_adding_a_PositiveInteger_Then_OverflowException_should_be_thrown(PositiveInt rightGenerator)
    {
        // Arrange
        PositiveInteger left = PositiveInteger.MaxValue;
        PositiveInteger right = PositiveInteger.From(rightGenerator.Item);

        // Act
        Action addingPositiveValueUsingCheckedKeyword = () => _ = checked(left + right);

        // Assert
        addingPositiveValueUsingCheckedKeyword.Should().Throw<OverflowException>($"the result is outside of [{PositiveInteger.MinValue} - {PositiveInteger.MaxValue}] range");
    }

    [Property]
    public void Given_a_PositiveInteger_is_MinValue_and_checked_state_When_subtracting_any_PositiveInteger_Then_OverflowException_should_be_thrown(PositiveInt rightGenerator)
    {
        // Arrange
        PositiveInteger left = PositiveInteger.MinValue;
        PositiveInteger right = PositiveInteger.From(rightGenerator.Item);

        _outputHelper.WriteLine($"{nameof(left)}: '{left}'");
        _outputHelper.WriteLine($"{nameof(right)}: '{right}'");

        // Act
        Action subtractingPositiveValueUsingCheckedKeyword = () => _ = checked(left - right);

        // Assert
        subtractingPositiveValueUsingCheckedKeyword.Should().Throw<OverflowException>($"the result would be outside of [{PositiveInteger.MinValue} - {PositiveInteger.MaxValue}] range");
    }

    public static IEnumerable<object[]> SubtractionInCheckedContextCases
    {
        get
        {
            yield return new object[]
            {
                PositiveInteger.MinValue,
                PositiveInteger.MinValue,
                PositiveInteger.MaxValue
            };

            yield return new object[]
            {
                PositiveInteger.One,
                PositiveInteger.One,
                PositiveInteger.MaxValue
            };

            yield return new object[]
            {
                PositiveInteger.One,
                PositiveInteger.From(2),
                PositiveInteger.From(PositiveInteger.MaxValue.Value - 1)
            };
        }
    }

    [Theory]
    [MemberData(nameof(SubtractionInCheckedContextCases))]
    public void Given_two_PositiveInteger_When_substracting_Then_result_should_be_expected(PositiveInteger left, PositiveInteger right, PositiveInteger expected)
    {
        // Act
        PositiveInteger actual = unchecked(left - right);

        // Assert
        actual.Should().Be(expected);
    }

    [Property(Arbitrary = new[] { typeof(ValueGenerators) })]
    public Property Given_an_existing_PositiveInteger_When_multiplying_by_multiplicative_identity_Then_the_result_should_be_equal_to_the_initial_value(PositiveInteger initial)
        => ((initial * PositiveInteger.MultiplicativeIdentity) == initial).ToProperty();
}