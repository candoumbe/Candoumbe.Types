namespace Candoumbe.Types.UnitTests.Numerics;

using Candoumbe.Types.Numerics;

using FluentAssertions;

using FsCheck;
using FsCheck.Xunit;

using System;

public class PositiveIntegerTests
{
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
    public void Given_PositiveIntegers_When_CompareTo_Then_ShouldReturnCorrectResult(PositiveInt leftValue, PositiveInt rightValue)
    {
        // Arrange
        PositiveInteger left = PositiveInteger.From(leftValue.Item);
        PositiveInteger right = PositiveInteger.From(rightValue.Item);

        // Act
        int result = left.CompareTo(right);

        // Assert
        result.Should().Be(left.Value.CompareTo(right.Value));
    }
}