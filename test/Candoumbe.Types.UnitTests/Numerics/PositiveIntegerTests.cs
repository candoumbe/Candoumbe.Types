namespace Candoumbe.Types.UnitTests.Numerics;

using Candoumbe.Types.Numerics;

using FluentAssertions;

using FsCheck;
using FsCheck.Xunit;

using System;

public class PositiveIntegerTests
{
    [Property]
    public void Given_PositiveIntegers_When_Addition_Then_ShouldReturnCorrectResult(PositiveInt leftValue, PositiveInt rightValue)
    {
        // Arrange
        int left = leftValue.Get;
        int right = rightValue.Get;
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
        left = Math.Max(0, left);
        right = Math.Max(0, right);
        PositiveInteger positiveA = PositiveInteger.From(left);
        PositiveInteger positiveB = PositiveInteger.From(right);

        // Act
        PositiveInteger result = positiveA - positiveB;

        // Assert
        object _ = (left - right) switch
        {
            < 0 => result.Value.Should().Be(0, "a positive integer can never hold a negative value"),
            int value => result.Value.Should().Be(value)
        };
    }

    [Property]
    public void Given_PositiveIntegers_When_Multiplication_Then_ShouldReturnCorrectResult(int left, int right)
    {
        // Arrange
        left = Math.Max(0, left);
        right = Math.Max(0, right);
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
        left = Math.Max(0, left);
        right = Math.Max(0, right);
        PositiveInteger positiveA = PositiveInteger.From(left);
        PositiveInteger positiveB = PositiveInteger.From(right);

        // Act
        bool result = positiveA < positiveB;

        // Assert
        result.Should().Be(left < right);
    }

    [Property]
    public void Given_PositiveIntegers_When_LessThanOrEqual_Then_ShouldReturnCorrectResult(int left, int right)
    {
        // Arrange
        left = Math.Max(0, left);
        right = Math.Max(0, right);
        PositiveInteger positiveA = PositiveInteger.From(left);
        PositiveInteger positiveB = PositiveInteger.From(right);

        // Act
        bool result = positiveA <= positiveB;

        // Assert
        result.Should().Be(left <= right);
    }

    [Property]
    public void Given_PositiveIntegers_When_GreaterThan_Then_ShouldReturnCorrectResult(int left, int right)
    {
        // Arrange
        left = Math.Max(0, left);
        right = Math.Max(0, right);
        PositiveInteger positiveA = PositiveInteger.From(left);
        PositiveInteger positiveB = PositiveInteger.From(right);

        // Act
        bool result = positiveA > positiveB;

        // Assert
        result.Should().Be(left > right);
    }

    [Property]
    public void Given_PositiveIntegers_When_GreaterThanOrEqual_Then_ShouldReturnCorrectResult(int left, int right)
    {
        // Arrange
        left = Math.Max(0, left);
        right = Math.Max(0, right);
        PositiveInteger positiveA = PositiveInteger.From(left);
        PositiveInteger positiveB = PositiveInteger.From(right);

        // Act
        bool result = positiveA >= positiveB;

        // Assert
        result.Should().Be(left >= right);
    }

}