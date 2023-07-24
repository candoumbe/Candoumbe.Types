namespace Candoumbe.Types.UnitTests.Numerics;

using Bogus;

using Candoumbe.Types.Numerics;
using Candoumbe.Types.UnitTests.Generators;

using FluentAssertions;

using FsCheck;
using FsCheck.Xunit;

using System;

public class PositiveIntegerTests
{
    private readonly static Faker Faker = new();

    [Property]
    public void Given_PositiveIntegers_When_Addition_Then_ShouldReturnCorrectResult(int left, int right)
    {
        // Arrange
        left = Math.Max(1, left);
        right = Math.Max(1, right);
        PositiveInteger positiveA = PositiveInteger.From(left);
        PositiveInteger positiveB = PositiveInteger.From(right);

        // Act
        PositiveInteger result = positiveA + positiveB;

        // Assert
        result.Value.Should().Be(left + right);
    }

    [Property]
    public void Given_PositiveIntegers_When_Subtraction_Then_ShouldReturnCorrectResult(int left, int right)
    {
        // Arrange
        left = Math.Max(1, left);
        right = Math.Max(1, right);
        PositiveInteger positiveA = PositiveInteger.From(left);
        PositiveInteger positiveB = PositiveInteger.From(right);

        // Act
        PositiveInteger result = positiveA - positiveB;

        // Assert
        object _ = (left - right) switch
        {
            < 1 => result.Value.Should().Be(PositiveInteger.MinValue.Value, $"a positive integer can never hold a value less than {PositiveInteger.MinValue.Value}"),
            int value => result.Value.Should().Be(value)
        };
    }

    [Property]
    public void Given_PositiveIntegers_When_Multiplication_Then_ShouldReturnCorrectResult(int left, int right)
    {
        // Arrange
        left = Math.Max(1, left);
        right = Math.Max(1, right);
        PositiveInteger positiveA = PositiveInteger.From(left);
        PositiveInteger positiveB = PositiveInteger.From(right);

        // Act
        PositiveInteger result = positiveA * positiveB;

        // Assert
        result.Value.Should().Be(left * right);
    }

    [Property(Arbitrary = new[] { typeof(ValueGenerators) })]
    public void Given_PositiveInteger_When_multiplying_by_NonNegativeZero_Then_result_should_be_NonNegativeZero(PositiveInteger left)
    {
        // Arrange
        NonNegativeInteger zero = NonNegativeInteger.Zero;

        // Act
        NonNegativeInteger actual = left * zero;

        // Assert
        actual.Should().Be(NonNegativeInteger.Zero);
    }

    [Property(Arbitrary = new[] { typeof(ValueGenerators) })]
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
    public void Given_a_PositiveInteger_When_implicitely_casting_to_decimal_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
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
}