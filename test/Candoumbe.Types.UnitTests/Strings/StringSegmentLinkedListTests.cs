using System;
using System.Linq;
using System.Text;
using Bogus;
using Candoumbe.Types.Strings;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.Extensions.Primitives;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Candoumbe.Types.UnitTests.Strings;

[UnitTest]
public class StringSegmentLinkedListTests(ITestOutputHelper outputHelper)
{
    private static readonly Faker Faker = new ();
        
    [Fact]
    public void GivenAddLast_WhenAddingNodeToEnd_ThenNodeIsAddedToEnd()
    {
        // Arrange
        StringSegment initialSegment = new StringSegment("initial");
        StringSegmentLinkedList linkedList = new StringSegmentLinkedList(initialSegment);
        StringSegment newSegment = new StringSegment("new");

        // Act
        linkedList.AddLast(newSegment);

        // Assert
        linkedList.Count.Should().Be(2);
        linkedList.Head.Next.Should().NotBeNull();
        linkedList.Head.Next.Value.Should().Be(newSegment);
    }

    [Property]
    public void GivenInsertAt_WhenInvalidIndex_ThenThrowsException()
    {
        // Arrange
        StringSegment initialSegment = new StringSegment("initial");
        StringSegmentLinkedList linkedList = new StringSegmentLinkedList(initialSegment);
        StringSegment newSegment = new StringSegment("new");

        // Act & Assert
        Action insertAtWithNegativeIndex = () => linkedList.InsertAt(-1, newSegment);
        Action insertAtWithIndexOutsideLinkedListIndex = () => linkedList.InsertAt(2, newSegment);
            
        insertAtWithNegativeIndex.Should().Throw<ArgumentOutOfRangeException>();
        insertAtWithIndexOutsideLinkedListIndex.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Property]
    public void Given_list_GetTotalLength_WhenCalculatingTotalLength_ThenReturnsCorrectValue(NonEmptyString initialStringGenerator, NonEmptySet<NonEmptyString> stringToAddGenerator)
    {
        // Arrange
        string[] stringsToAdd = stringToAddGenerator.Select(stringGenerator => stringGenerator.Item)
            .ToArray();
        string initialString = initialStringGenerator.Item;
        StringSegmentLinkedList linkedList = new (initialString);
        int expectedLength = initialString.Length;

        foreach (string stringToAdd in stringsToAdd)
        {
            outputHelper.WriteLine($"String to add : '{stringToAdd}', Length: {stringToAdd.Length}");
            const int offset = 0;
            int length = Faker.Random.Int(offset, stringToAdd.Length - 1);
            StringSegment newSegment = new StringSegment(stringToAdd, offset, length);
                
            outputHelper.WriteLine($"Segment added : '{newSegment.Value}'");
                
            expectedLength += newSegment.Length;
                
            linkedList.AddLast(newSegment);
        }
            
        // Act
        int totalLength = linkedList.GetTotalLength();

        // Assert
        totalLength.Should().Be(expectedLength);
    }

    [Property]
    public void Given_list_When_adding_nodes_Then_ToStringValue_returns_expected_string(NonEmptyString initialStringGenerator, NonEmptySet<NonEmptyString> stringToAddGenerator)
    {
        // Arrange
        string[] stringsToAdd = stringToAddGenerator.Select(stringGenerator => stringGenerator.Item)
            .ToArray();
        string initialString = initialStringGenerator.Item;
        StringSegmentLinkedList linkedList = new (initialString);
        StringBuilder sb = new(initialString);
        
        foreach (string stringToAdd in stringsToAdd)
        {
            outputHelper.WriteLine($"String to add : '{stringToAdd}', Length: {stringToAdd.Length}");
            const int offset = 0;
            int length = Faker.Random.Int(offset, stringToAdd.Length - 1);
            StringSegment newSegment = new StringSegment(stringToAdd, offset, length);
                
            outputHelper.WriteLine($"Segment added : '{newSegment.Value}'");

            linkedList.AddLast(newSegment);
            sb.Append(newSegment.AsSpan(0, newSegment.Length));
        }

        string expected = sb.ToString();

        // Act
        string actual = linkedList.ToStringValue();

        // Assert
        actual.Should().Be(expected);
    }
}