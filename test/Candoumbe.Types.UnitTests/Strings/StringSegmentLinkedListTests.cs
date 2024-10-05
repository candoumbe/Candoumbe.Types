using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Bogus;
using Candoumbe.Types.Strings;
using FluentAssertions;
using FluentAssertions.Execution;
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
    private static readonly Faker Faker = new();

    [Property]
    public void Given_non_empty_string_segment_Then_constructor_should_initialize_properties(NonEmptyString stringGenerator)
    {
        // Arrange
        StringSegment initialSegment = new StringSegment("initial");

        // Act
        StringSegmentLinkedList linkedList = new StringSegmentLinkedList(initialSegment);

        // Assert
        linkedList.Count.Should().Be(1);
    }

    [Property]
    public void Given_index_is_negative_When_calling_InsertAt_Then_throw_ArgumentOutOfRangeException(NegativeInt negativeIndexGenerator)
    {
        // Arrange
        StringSegment initialSegment = new StringSegment(Faker.Lorem.Word());
        StringSegmentLinkedList linkedList = new StringSegmentLinkedList(initialSegment);
        StringSegment newSegment = new StringSegment(Faker.Lorem.Word());
        int negativeIndex = negativeIndexGenerator.Item;

        // Act & Assert
        Action insertAtWithNegativeIndex = () => linkedList.InsertAt(negativeIndex, newSegment);

        insertAtWithNegativeIndex.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Property]
    public void Given_index_is_beyond_valid_values_When_calling_InsertAt_Then_throw_ArgumentOutOfRangeException(NonEmptyArray<NonEmptyString> stringsGenerator)
    {
        // Arrange
        StringSegment initialSegment = new StringSegment(stringsGenerator.Item[0].Item);
        StringSegmentLinkedList linkedList = new StringSegmentLinkedList(initialSegment);
        StringSegment newSegment = Faker.PickRandom(stringsGenerator.Item).Item;
        int indexOutsideOfRange = stringsGenerator.Item.Length + 1;

        // Act & Assert
        Action insertAtWithNegativeIndex = () => linkedList.InsertAt(indexOutsideOfRange, newSegment);

        insertAtWithNegativeIndex.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Property]
    public void Given_list_is_not_empty_when_the_value_to_add_is_empty_Then_the_list_is_not_modified(NonEmptyString stringGenerator)
    {
        // Arrange
        StringSegmentLinkedList list = new StringSegmentLinkedList(stringGenerator.Item);

        // Act
        list.Append(StringSegment.Empty);

        // Assert
        list.Count.Should().Be(1);
    }

    public static TheoryData<StringSegmentLinkedList, (int index, StringSegment value), (int length, string value)> InsertAtCases
        => new()
        {
            {
                new("Hello"),
                ( 1, " world" ),
                ("Hello world".Length, "Hello world")
            },
            {
                new("Hello"),
                ( 0, "world " ),
                ("world Hello".Length, "world Hello")
            },
            {
                new("Hello", "world"),
                ( 1, " " ),
                ("Hello world".Length, "Hello world")
            }
        };

    [Theory]
    [MemberData(nameof(InsertAtCases))]
    public void Given_a_initial_list_When_inserting_Then_a_value_content_is_as_expected(StringSegmentLinkedList initialList, (int index, StringSegment value) insertion, (int length, string value) expected)
    {
        // Act
        initialList.InsertAt(insertion.index, insertion.value);

        // Assert
        using AssertionScope assertionScope = new ();
        initialList.ToStringValue().Should().Be(expected.value);
        initialList.GetTotalLength().Should().Be(expected.length);
    }

    public static TheoryData<StringSegmentLinkedList, IReadOnlyList<StringSegment>, (int length, string value)> AppendCases
    {
        get
        {
            TheoryData<StringSegmentLinkedList, IReadOnlyList<StringSegment>, (int length, string value)> data = new TheoryData<StringSegmentLinkedList, IReadOnlyList<StringSegment>, (int length, string value)>();
            data.Add(new("Hello"),
                    [" ", "world"],
                    ( "Hello world".Length, "Hello world" )
            );
            data.Add(new("Hello"),
                    ["wonderful", string.Empty, " ", "world"],
                    ( "Hellowonderful world".Length, "Hellowonderful world" )
            );
            {
                StringSegment source = "abcdef";
                data.Add(new (source.Subsegment(0, 1)), [ source ] , (7, "aabcdef"));
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(AppendCases))]
    public void Given_a_initial_list_When_appending_value_Then_the_list_state_is_as_expected(StringSegmentLinkedList initialList, IEnumerable<StringSegment> values, (int length, string value) expected)
    {
        // Act
        foreach (StringSegment value in values)
        {
            initialList.Append(value);
        }

        // Assert
        using AssertionScope assertionScope = new();
        initialList.ToStringValue().Should().Be(expected.value);
        initialList.GetTotalLength().Should().Be(expected.length);
    }

    public static TheoryData<StringSegmentLinkedList,(int index, StringSegmentLinkedList list), (int length, string value)> InsertAtListCases
    {
        get
        {
            TheoryData<StringSegmentLinkedList, (int, StringSegmentLinkedList), (int length, string value)> data = new TheoryData<StringSegmentLinkedList, (int index, StringSegmentLinkedList), (int length, string value)>();
            data.Add(new("Hello"),
                (1 , new StringSegmentLinkedList(" ", "world")),
                ( "Hello world".Length, "Hello world" )
            );
            data.Add(new("Hello"),
                (1, new("wonderful", string.Empty, " ", "world")),
                ( "Hellowonderful world".Length, "Hellowonderful world" )
            );
            {
                StringSegment source = "abcdef";
                data.Add(new (
                    source.Subsegment(0, 1)),
                        (0, new (source)) ,
                        (7, "abcdefa"));
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(InsertAtListCases))]
    public void Given_a_initial_list_When_appending_list_Then_the_list_state_is_as_expected(StringSegmentLinkedList initialList, (int index, StringSegmentLinkedList other) insertion, (int length, string value) expected)
    {
        // Act
        initialList.InsertAt(insertion.index, insertion.other);

        // Assert
        using AssertionScope assertionScope = new();
        initialList.ToStringValue().Should().Be(expected.value);
        initialList.GetTotalLength().Should().Be(expected.length);
    }

    public static TheoryData<StringSegmentLinkedList, (char oldChar, char newChar), string> ReplaceCharByCharCases
        => new()
        {
            { new StringSegmentLinkedList("Hello"), ('e', 'a'), "Hallo" },
            { new StringSegmentLinkedList("Hello"), ('l', 'r'), "Herro" },
        };

    [Theory]
    [MemberData(nameof(ReplaceCharByCharCases))]
    public void Given_a_initial_list_When_replacing_a_character_that_exists_in_one_of_its_node_Then_the_string_should_be_updated(StringSegmentLinkedList initialList, (char oldChar, char newChar) replacement, string expected)
    {
        // Act
        StringSegmentLinkedList actualList = initialList.Replace(replacement.oldChar, replacement.newChar);

        // Assert
        actualList.ToStringValue().Should().Be(expected);
    }

    public static TheoryData<StringSegmentLinkedList, (char oldChar, string newString), string> ReplaceCharByStringCases
        => new()
        {
            { new StringSegmentLinkedList("Hello"), ('e', "a"), "Hallo" },
            { new StringSegmentLinkedList("Hello"), ('H', "Tr"), "Trello" },
        };

    [Theory]
    [MemberData(nameof(ReplaceCharByStringCases))]
    public void Given_a_initial_list_When_replacing_a_character_that_exists_in_one_of_its_node_Then_the_string_should_be_updated(StringSegmentLinkedList initialList, (char oldChar, string newString) replacement, string expected)
    {
        // Act
        StringSegmentLinkedList actualList = initialList.Replace(replacement.oldChar, replacement.newString);

        // Assert
        actualList.ToStringValue().Should().Be(expected);
    }

    public static TheoryData<StringSegmentLinkedList, (string oldString, string newString), string> ReplaceStringByStringCases
        => new()
        {
            { new StringSegmentLinkedList("Hello"), ("e", "a"), "Hallo" },
            { new StringSegmentLinkedList("Hello"), ("llo", "ro"), "Hero" },
            { new StringSegmentLinkedList("Hello"), ("ll", "r"), "Hero" },
        };

    [Theory]
    [MemberData(nameof(ReplaceStringByStringCases))]
    public void Given_a_non_initial_list_When_replacing_a_character_that_exists_in_one_of_its_node_Then_the_string_should_be_updated(StringSegmentLinkedList initialList, (string oldString, string newString) replacement, string expected)
    {
        // Act
        StringSegmentLinkedList actualList = initialList.Replace(replacement.oldString, replacement.newString);

        // Assert
        actualList.ToStringValue().Should().Be(expected);
    }

    [Property]
    public void Given_the_initial_list_is_empty_When_I_add_an_non_empty_string_Then_the_list_should_only_contains_the_new_element(NonEmptyString stringGenerator)
    {
        // Arrange
        StringSegmentLinkedList initialList = new(StringSegment.Empty);
        StringSegment segment = stringGenerator.Item;

        // Act
        StringSegmentLinkedList actualList = initialList.Append(segment);

        // Assert
        actualList.Count.Should().Be(2);
        actualList.GetTotalLength().Should().Be(segment.Length);
    }

    [Fact]
    public void Given_initial_empty_list_Then_Count_should_return_zero()
    {
        // Arrange
        StringSegmentLinkedList initialList = new(StringSegment.Empty);

        // Act
        initialList.Count.Should().Be(1);
        initialList.GetTotalLength().Should().Be(0);
    }
}