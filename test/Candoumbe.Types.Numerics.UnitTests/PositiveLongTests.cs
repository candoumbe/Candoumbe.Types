using System;
using System.Collections.Generic;
using Bogus;
using FluentAssertions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace Candoumbe.Types.Numerics.UnitTests;

public class PositiveLongTests
{
    private readonly ITestOutputHelper _outputHelper;
    private static readonly Faker Faker = new();

    public PositiveLongTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }


    [Property]
    public void Given_input_is_less_than_1_When_calling_From_to_build_an_instance_Then_an_exception_should_be_thrown()
    {
        // Arrange
        long input = Faker.Random.Long(max: 0);

        // Act
        Action callingFrom = () => PositiveLong.From(input);

        // Assert
        callingFrom.Should()
                   .ThrowExactly<ArgumentOutOfRangeException>($"{input} is out of [{PositiveLong.MinValue} - {PositiveLong.MaxValue}] range of values")
                   .Where(ex => !string.IsNullOrWhiteSpace(ex.Message), "the message always should be set for diagnostic purpose.");
    }

    [Property]
    public void Given_PositiveLongs_When_Addition_Then_ShouldReturnCorrectResult(long left, long right)
    {
        // Arrange
        left = Math.Max(1, left);
        right = Math.Max(1, right);
        PositiveLong positiveA = PositiveLong.From(left);
        PositiveLong positiveB = PositiveLong.From(right);

        // Act
        PositiveLong result = checked(positiveA + positiveB);

        // Assert
        result.Value.Should().Be(left + right);
    }

    [Property]
    public void Given_two_PositiveLongs_When_doing_Subtraction_with_unchecked_Then_ShouldReturnCorrectResult()
    {
        // Arrange
        Prop.ForAll<long, long>((leftValueGenerator, rightValueGenerator) =>
        {
            PositiveLong left = PositiveLong.From(Math.Max(1, leftValueGenerator));
            PositiveLong right = PositiveLong.From(Math.Max(1, rightValueGenerator));

            _outputHelper.WriteLine($"{nameof(left)} : '{left}'");
            _outputHelper.WriteLine($"{nameof(right)} : '{right}'");

            // Act
            PositiveLong result = unchecked(left - right);

            // Assert
            object _ = (leftValueGenerator - rightValueGenerator) switch
            {
                long value when value >= 1 => result.Should().Be(PositiveLong.From(value), $"a positive integer can never hold a value less than {PositiveLong.MinValue.Value}"),
                long value when value < 1 => result.Should().Be(PositiveLong.From(PositiveLong.MaxValue.Value - Math.Abs(value)), $"a positive integer can never hold a value less than {PositiveLong.MinValue.Value}"),
                long value => result.Value.Should().Be(value)
            };
        });
    }

    [Property]
    public void Given_PositiveLongs_When_Multiplication_Then_ShouldReturnCorrectResult(long left, long right)
    {
        // Arrange
        left = Math.Max(1, left);
        right = Math.Max(1, right);
        PositiveLong positiveA = PositiveLong.From(left);
        PositiveLong positiveB = PositiveLong.From(right);

        // Act
        PositiveLong result = checked(positiveA * positiveB);

        // Assert
        result.Value.Should().Be(left * right);
    }

    [Property]
    public void Given_PositiveLongs_When_LessThan_Then_ShouldReturnCorrectResult(long left, long right)
    {
        // Arrange
        left = Math.Max(1, left);
        right = Math.Max(1, right);
        PositiveLong positiveA = PositiveLong.From(left);
        PositiveLong positiveB = PositiveLong.From(right);

        // Act
        bool result = positiveA < positiveB;

        // Assert
        result.Should().Be(left < right);
    }

    [Property]
    public void Given_PositiveLongs_When_LessThanOrEqual_Then_ShouldReturnCorrectResult(PositiveInt leftValue, PositiveInt rightValue)
    {
        // Arrange
        PositiveLong left = PositiveLong.From(leftValue.Item);
        PositiveLong right = PositiveLong.From(rightValue.Item);

        // Act
        bool result = left <= right;

        // Assert
        result.Should().Be(left.Value <= right.Value);
    }

    [Property]
    public void Given_PositiveLongs_When_GreaterThan_Then_ShouldReturnCorrectResult(PositiveInt leftValue, PositiveInt rightValue)
    {
        // Arrange
        PositiveLong left = PositiveLong.From(leftValue.Item);
        PositiveLong right = PositiveLong.From(rightValue.Item);

        // Act
        bool result = left > right;

        // Assert
        result.Should().Be(left.Value > right.Value);
    }

    [Property]
    public void Given_PositiveLongs_When_GreaterThanOrEqual_Then_ShouldReturnCorrectResult(PositiveInt leftValue, PositiveInt rightValue)
    {
        // Arrange
        PositiveLong left = PositiveLong.From(leftValue.Item);
        PositiveLong right = PositiveLong.From(rightValue.Item);

        // Act
        bool result = left >= right;

        // Assert
        result.Should().Be(left.Value >= right.Value);
    }

    [Property]
    public void Given_a_PositiveLong_When_implicitely_casting_to_int32_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        PositiveLong initial = PositiveLong.From(initialValueGenerator.Item);

        // Act
        long actual = initial;

        // Assert
        actual.Should().Be(initial.Value);
    }

    [Property]
    public void Given_a_PositiveLong_When_implicitely_casting_to_long_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        PositiveLong initial = PositiveLong.From(initialValueGenerator.Item);

        // Act
        long actual = initial;

        // Assert
        actual.Should().Be(initial.Value);
    }

    [Property]
    public void Given_a_PositiveLong_When_implicitely_casting_to_decimal_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        PositiveLong initial = PositiveLong.From(initialValueGenerator.Item);

        // Act
        decimal actual = initial;

        // Assert
        actual.Should().Be(initial.Value);
    }

    [Property]
    public void Given_a_PositiveLong_When_implicitely_casting_to_ulong_Then_result_should_equal_the_original_value(PositiveInt initialValueGenerator)
    {
        // Arrange
        PositiveLong initial = PositiveLong.From(initialValueGenerator.Item);

        // Act
        ulong actual = initial;

        // Assert
        actual.Should().Be(initial);
    }

    [Property]
    public void Given_a_PositiveLong_When_adding_a_non_negative_value_Then_result_should_be_gte_the_original_value(PositiveInt positiveIntGenerator, NonNegativeInt nonNegativeIntGenerator)
    {
        // Arrange
        PositiveLong positiveLong = PositiveLong.From(positiveIntGenerator.Item);
        NonNegativeLong nonNegativeLong = NonNegativeLong.From(nonNegativeIntGenerator.Item);

        // Act
        PositiveLong actual = positiveLong + nonNegativeLong;

        // Assert
        actual.Should().Be(PositiveLong.From(positiveLong.Value + nonNegativeLong.Value));
    }

    [Property]
    public void Given_a_PositiveLong_When_dividing_by_a_PositiveLong_Then_result_should_be_NonNegativeLong(PositiveInt leftGenerator, PositiveInt rightGenerator)
    {
        // Arrange
        PositiveLong left = PositiveLong.From(leftGenerator.Item);
        PositiveLong right = PositiveLong.From(rightGenerator.Item);

        // Act
        NonNegativeLong actual = left / right;

        // Assert
        actual.Should().Be(NonNegativeLong.From(left.Value / right.Value));
    }

    [Property]
    public void Given_a_PositiveLong_is_MaxValue_and_checked_state_When_adding_a_PositiveLong_Then_OverflowException_should_be_thrown(PositiveInt rightGenerator)
    {
        // Arrange
        PositiveLong left = PositiveLong.MaxValue;
        PositiveLong right = PositiveLong.From(rightGenerator.Item);

        // Act
        Action addingPositiveValueInCheckedContext = () => _ = checked(left + right);

        // Assert
        addingPositiveValueInCheckedContext.Should().Throw<OverflowException>($"the result is outside of [{PositiveLong.MinValue} - {PositiveLong.MaxValue}] range");
    }

    [Property]
    public void Given_a_PositiveLong_is_MinValue_and_checked_state_When_subtracting_any_PositiveLong_Then_OverflowException_should_be_thrown(PositiveInt rightGenerator)
    {
        // Arrange
        PositiveLong left = PositiveLong.MinValue;
        PositiveLong right = PositiveLong.From(rightGenerator.Item);

        _outputHelper.WriteLine($"{nameof(left)}: '{left}'");
        _outputHelper.WriteLine($"{nameof(right)}: '{right}'");

        // Act
        Action subtractingPositiveValueUsingCheckedKeyword = () => _ = checked(left - right);

        // Assert
        subtractingPositiveValueUsingCheckedKeyword.Should().Throw<OverflowException>($"the result would be outside of [{PositiveLong.MinValue} - {PositiveLong.MaxValue}] range");
    }

    public static IEnumerable<object[]> SubtractionInCheckedContextCases
    {
        get
        {
            yield return
            [
                PositiveLong.MinValue,
                PositiveLong.MinValue,
                PositiveLong.MaxValue
            ];

            yield return
            [
                PositiveLong.One,
                PositiveLong.One,
                PositiveLong.MaxValue
            ];

            yield return
            [
                PositiveLong.One,
                PositiveLong.From(2),
                PositiveLong.From(PositiveLong.MaxValue.Value - 1)
            ];
        }
    }

    [Theory]
    [MemberData(nameof(SubtractionInCheckedContextCases))]
    public void Given_unchecked_When_subtracting_left_from_right_Then_result_should_be_expected(PositiveLong left, PositiveLong right, PositiveLong expected)
    {
        // Act
        PositiveLong actual = unchecked(left - right);

        // Assert
        actual.Should().Be(expected);
    }
}