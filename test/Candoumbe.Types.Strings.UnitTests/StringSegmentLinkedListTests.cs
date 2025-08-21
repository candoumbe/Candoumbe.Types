using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bogus;
using Candoumbe.MiscUtilities.Comparers;
using Candoumbe.Types.Strings.UnitTests.Generators;
using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using FsCheck.Experimental;
using FsCheck.Xunit;
using Microsoft.Extensions.Primitives;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using ReplacePredicateByReadOnlyMemory = (System.Func<char, bool> Predicate, System.ReadOnlyMemory<char> NewValue);
using ReplacePredicateByChar = (System.Func<char, bool> Predicate, char NewValue);

namespace Candoumbe.Types.Strings.UnitTests;

[UnitTest]
public class StringSegmentLinkedListTests(ITestOutputHelper outputHelper)
{
    private static readonly Faker s_faker = new();

    [Property]
    public void Given_non_empty_string_segment_Then_constructor_should_initialize_properties(NonEmptyString stringGenerator)
    {
        // Arrange
        StringSegment initialSegment ="initial";

        // Act
        StringSegmentLinkedList linkedList = new(initialSegment);

        // Assert
        linkedList.Count.Should().Be(1);
    }

    [Property]
    public void Given_index_is_negative_When_calling_InsertAt_Then_throw_ArgumentOutOfRangeException(NegativeInt negativeIndexGenerator)
    {
        // Arrange
        StringSegment initialSegment = new StringSegment(s_faker.Lorem.Word());
        StringSegmentLinkedList linkedList = new StringSegmentLinkedList(initialSegment);
        StringSegment newSegment = new StringSegment(s_faker.Lorem.Word());
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
        StringSegment newSegment = s_faker.PickRandom(stringsGenerator.Item).Item;
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
            TheoryData<StringSegmentLinkedList, IReadOnlyList<StringSegment>, (int length, string value)> data = new()
            {
                { new("Hello"), [" ", "world"], ( "Hello world".Length, "Hello world" ) },
                {
                    new("Hello"), ["wonderful", string.Empty, " ", "world"],
                    ( "Hellowonderful world".Length, "Hellowonderful world" )
                }
            };
            {
                StringSegment source = "abcdef";
                data.Add(new StringSegmentLinkedList(source.Subsegment(0, 1)), [ source ] , (7, "aabcdef"));
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(AppendCases))]
    public void Given_a_initial_list_When_appending_value_Then_the_list_state_is_as_expected(StringSegmentLinkedList initialList, IReadOnlyList<StringSegment> values, (int length, string value) expected)
    {
        // Act
        foreach (StringSegment value in values)
        {
            initialList.Append(value.AsSpan());
        }

        // Assert
        using AssertionScope assertionScope = new();
        initialList.ToStringValue().Should().Be(expected.value);
        initialList.GetTotalLength().Should().Be(expected.length);
    }

    public static TheoryData<StringSegmentLinkedList, (int index, StringSegmentLinkedList list), (int length, string value)> InsertAtListCases
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
            { new StringSegmentLinkedList("Hello"), ('H', 'h'), "hello" },
            { new StringSegmentLinkedList("Hello"), ('l', 'r'), "Herro" },
            { new StringSegmentLinkedList("Hello"), ('e', 'a'), "Hallo" },
            { new StringSegmentLinkedList("Hello"), ('o', 'a'), "Hella" },
            { new StringSegmentLinkedList("Hello", "world"), ('o', 'a'), "Hellawarld" },
            { new StringSegmentLinkedList("killer", "ConcreteUnbranded", "Steel", "CarcopyAuto"), ('o', 'a'), "killerCancreteUnbrandedSteelCarcapyAuta" },
            { new StringSegmentLinkedList("killer", "ConcreteUnbranded", "Steel", "CarcopyAuto"), ('o', 'ꂕ'), "killerCꂕncreteUnbrandedSteelCarcꂕpyAutꂕ" }
        };

    [Theory]
    [MemberData(nameof(ReplaceCharByCharCases))]
    public void Given_a_initial_list_When_replacing_a_character_that_exists_in_one_of_its_node_Then_the_string_should_be_updated(StringSegmentLinkedList initialList, (char oldChar, char newChar) replacement, string expected)
    {
        // Act
        StringSegmentLinkedList actualList = initialList.Replace(replacement.oldChar, replacement.newChar);

        // Assert
        string actual = actualList.ToStringValue();
        actual.Should().Be(expected);
    }

    public static TheoryData<StringSegmentLinkedList, (char oldChar, string newString), string> ReplaceCharByStringCases
        => new()
        {
            { new StringSegmentLinkedList("Hello"), ('e', "a"), "Hallo" },
            { new StringSegmentLinkedList("Hello"), ('H', "Tr"), "Trello" },
            { new StringSegmentLinkedList("Hello", "world"), ('o', "a"), "Hellawarld" },
            { new StringSegmentLinkedList("killer", "ConcreteUnbranded", "Steel", "CarcopyAuto"), ('o', "ꂕ"), "killerCꂕncreteUnbrandedSteelCarcꂕpyAutꂕ" }
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
            { new StringSegmentLinkedList("Hello", "world"), ( "e", "a" ), "Halloworld" },
            { new StringSegmentLinkedList("Hello"), ( "llo", "ro" ), "Hero" },
            { new StringSegmentLinkedList("Hello"), ( "ll", "r" ), "Hero" },
            { new StringSegmentLinkedList("Hel", "lo"), ( "ll", "r" ), "Hero" },
            { new StringSegmentLinkedList("Hello", "world"), ( "o", "a" ), "Hellawarld" },
            { new StringSegmentLinkedList("killer", "ConcreteUnbranded", "Steel", "CarcopyAuto"), ("o", "ꂕ"), "killerCꂕncreteUnbrandedSteelCarcꂕpyAutꂕ" }
        };

    [Theory]
    [MemberData(nameof(ReplaceStringByStringCases))]
    public void Given_a_non_initial_list_When_replacing_a_character_that_exists_in_one_of_its_node_Then_the_string_should_be_updated(StringSegmentLinkedList initialList, (string oldString, string newString) replacement, string expected)
    {
        // Act
        StringSegmentLinkedList actualList = initialList.Replace(replacement.oldString, replacement.newString);

        // Assert
        string actual = actualList.ToStringValue();
        actual.Should().Be(expected);
    }

    [Property]
    public void Given_the_initial_list_is_empty_When_I_add_an_non_empty_string_Then_the_list_should_only_contains_the_new_element(NonEmptyString stringGenerator)
    {
        // Arrange
        StringSegmentLinkedList initialList = new(StringSegment.Empty);
        string segment = stringGenerator.Item;

        // Act
        StringSegmentLinkedList actualList = initialList.Append(segment);

        // Assert
        actualList.Count.Should().Be(1);
        actualList.GetTotalLength().Should().Be(segment.Length);
    }

    [Fact]
    public void Given_initial_empty_list_Then_Count_should_return_zero()
    {
        // Arrange
        StringSegmentLinkedList initialList = new(StringSegment.Empty);

        // Act
        initialList.Count.Should().Be(0);
        initialList.GetTotalLength().Should().Be(0);
    }

    public static TheoryData<StringSegmentLinkedList, Func<ReadOnlyMemory<char>, bool>, Expression<Func<StringSegmentLinkedList, bool>>> RemoveByPredicateCases
        => new()
        {
            {
                new StringSegmentLinkedList("Hel", "lo"),
                segment => segment.StartsWith("lo".ToCharArray()),
                list => list.ToStringValue() == "Hel"
            },
            {
                new StringSegmentLinkedList("Hel", "lo"),
                segment => segment.StartsWith("He".ToCharArray()),
                list => list.ToStringValue() == "lo"
            },
            {
                new StringSegmentLinkedList("Hel"),
                segment => segment.StartsWith("Hel".ToCharArray()),
                list => list.ToStringValue() == string.Empty
            }
        };

    [Theory]
    [MemberData(nameof(RemoveByPredicateCases))]
    public void Given_a_initial_list_When_removing_node_with_specified_predicate_Then_the_resulting_list_should_match_expectation(StringSegmentLinkedList initialList, Func<ReadOnlyMemory<char>, bool> nodeToRemovePredicate, Expression<Func<StringSegmentLinkedList, bool>> resultExpectation)
    {
        // Act
        StringSegmentLinkedList actual = initialList.RemoveBy(nodeToRemovePredicate);

        // Assert
        actual.Should().Match(resultExpectation);
    }

    public static TheoryData<StringSegmentLinkedList, StringSegmentLinkedList, StringSegmentLinkedList> AppendListToAnotherListCases
        => new()
        {
            {
                new StringSegmentLinkedList(),
                new StringSegmentLinkedList(),
                new StringSegmentLinkedList()
            },
            {
                new StringSegmentLinkedList("one","two"),
                new StringSegmentLinkedList(),
                new StringSegmentLinkedList("one", "two")
            },
            {
                new StringSegmentLinkedList(),
                new StringSegmentLinkedList("one","two"),
                new StringSegmentLinkedList("one","two")
            },
            {
                new StringSegmentLinkedList("one","two"),
                new StringSegmentLinkedList("three", "four", "five", "six", "seven", "eight", "nine"),
                new StringSegmentLinkedList("one", "two", "three", "four", "five", "six", "seven", "eight", "nine")
            }
        };

    [Theory]
    [MemberData(nameof(AppendListToAnotherListCases))]
    public void Given_an_initial_list_When_appending_another_list_Then_the_resulting_list_should_match_expectation(StringSegmentLinkedList first, StringSegmentLinkedList second, StringSegmentLinkedList expected)
    {
        // Act
        object actual = first.Append(second);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Given_initial_list_not_null_When_appending_null_Then_Append_should_throw_ArgumentNullException()
    {
        // Arrange
        StringSegmentLinkedList initialList = [];

        // Act
        Action appendingNullList = () => initialList.Append((StringSegmentLinkedList)null);

        // Assert
        appendingNullList.Should().Throw<ArgumentNullException>();
    }

    [Property]
    public void Given_two_lists_which_contains_characters_When_both_are_in_the_same_order_and_result_in_the_same_string_Then_both_lists_should_be_considered_equal(NonEmptyString valueGenerator)
    {
        // Arrange
        string value = valueGenerator.Get;
        int leftChunkSize = Random.Shared.Next(1, value.Length);
        int rightChunkSize = Random.Shared.Next(1, value.Length);

        IReadOnlyList<char[]> leftChunks = value.Chunk(leftChunkSize).ToArray();
        IReadOnlyList<char[]> rightChunks = value.Chunk(rightChunkSize).ToArray();

        outputHelper.WriteLine($"'left chunks : '{leftChunks.Jsonify()}'");
        outputHelper.WriteLine($"'right chunks : '{rightChunks.Jsonify()}'");

        StringSegmentLinkedList left = new ();
        foreach (char[] leftChunk in leftChunks)
        {
            left.Append(leftChunk);
        }

        StringSegmentLinkedList right = new ();
        foreach (char[] rightChunk in rightChunks)
        {
            right.Append(rightChunk);
        }

        string leftValue = $"[{string.Join(',', left.Select(node => $"[{node}]"))}]";
        string rightValue = $"[{string.Join(',', right.Select(node => $"[{node}]"))}]";
        outputHelper.WriteLine($"left is : {leftValue}");
        outputHelper.WriteLine($"right is: {rightValue}");

        // Act
        bool actual = left.Equals(right);

        // Assert
        actual.Should().BeTrue($"'{leftValue}' is equivalent to '{rightValue}'");
    }

    [Fact]
    public void Given_two_lists_When_both_are_empty_Then_they_should_be_considered_equal()
    {
        // Arrange
        StringSegmentLinkedList left = [];
        StringSegmentLinkedList right = [];

        // Act
        bool actual = left.Equals(right);

        // Assert
        actual.Should().BeTrue();
    }

    public static TheoryData<StringSegmentLinkedList, StringSegmentLinkedList, IEqualityComparer<char>, bool, string> IsEquivalentToCases
    {
        get
        {
            TheoryData<StringSegmentLinkedList, StringSegmentLinkedList, IEqualityComparer<char>, bool, string> data = new()
            {
                { [], [], CharComparer.Ordinal, true, $"Both lists are empty (comparer : {nameof(CharComparer.Ordinal)})" },
                { [], [], CharComparer.InvariantCultureIgnoreCase, true, $"Both lists are empty (comparer : {nameof(CharComparer.InvariantCultureIgnoreCase)})" },
                {new StringSegmentLinkedList("A", "lazy fox"),
                         new StringSegmentLinkedList("A", "lazy fox"),
                         CharComparer.InvariantCultureIgnoreCase,
                         true,
                         "Both lists contains 'A' and 'lazy fox'"
                         },
                {
                    new StringSegmentLinkedList("A", "lazy fox"),
                    new StringSegmentLinkedList("A", "lazy Fox"),
                    CharComparer.Ordinal,
                    false,
                    $"Both lists contains 'A' and 'lazy fox' but the casing is not the same and comparer is {nameof(CharComparer.Ordinal)} only."
                },
                {
                    new StringSegmentLinkedList("A", "lazy fox"),
                    new StringSegmentLinkedList("A", "lazy Fox"),
                    CharComparer.InvariantCultureIgnoreCase,
                    true,
                    $"Both lists contains 'A' and 'lazy fox' but the casing is not the same and is comparer is {nameof(CharComparer.InvariantCultureIgnoreCase)} only."
                },
                {
                    new StringSegmentLinkedList("A lazy fox"),
                    new StringSegmentLinkedList("A"),
                    CharComparer.InvariantCultureIgnoreCase,
                    false,
                    "Both lists contains starts with 'A' but the right list contains only one character whereas the left is longer"
                },
                {
                    new StringSegmentLinkedList("A"),
                    new StringSegmentLinkedList("A lazy fox"),
                    CharComparer.InvariantCultureIgnoreCase,
                    false,
                    "Both lists starts with the same value but the left list contains only one value whereas the right is longer"
                },
                {
                    new StringSegmentLinkedList("A lazy ", "fox"),
                    new StringSegmentLinkedList("A lazy fox"),
                    CharComparer.Ordinal,
                    true,
                    "Both lists would result with the same string values but were not initialize in the same way"
                }
            };

            {
                StringSegmentLinkedList left = new("aH");
                left = left.Append("&");

                StringSegmentLinkedList right = new("aH&");

                data.Add(left, right, null, true, "Both list would produces the same string output");
                data.Add(right, left, null, true, "Both list would produces the same string output");
            }
            {
                StringSegmentLinkedList left = new("0");
                StringSegmentLinkedList right = new("+0");

                data.Add(left, right, null, false, "['0'] is not equivalent to ['+0']");
                data.Add(left, right, null, false, "['+0'] is not equivalent to ['0']");
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(IsEquivalentToCases))]
    public void Given_left_and_right_lists_Equals_should_behave_as_expected(StringSegmentLinkedList left, StringSegmentLinkedList right, IEqualityComparer<char> comparer, bool expectedResult, string reason)
    {
        // Arrange
        string leftValue = $"[{string.Join(',', left.Select(node => $"[{node}]"))}]";
        string rightValue = $"[{string.Join(',', right.Select(node => $"[{node}]"))}]";
        outputHelper.WriteLine($"left is : {leftValue}");
        outputHelper.WriteLine($"right is: {rightValue}");
        // Act
        bool actual = left.Equals(right, comparer);

        //Assert
        actual.Should().Be(expectedResult, reason);
    }

    public static TheoryData<StringSegmentLinkedList, StringSegmentLinkedList, bool, string> EqualsCases
    {
        get
        {
            TheoryData<StringSegmentLinkedList, StringSegmentLinkedList, bool, string> data = new()
            {
                { [], null, false, "The other list is null" },
                { [], [], true, $"Both lists are empty (comparer : {nameof(CharComparer.Ordinal)})" },
                { [], [], true, $"Both lists are empty (comparer : {nameof(CharComparer.InvariantCultureIgnoreCase)})" },
                {
                    new StringSegmentLinkedList("A", "lazy fox"),
                    new StringSegmentLinkedList("A", "lazy fox"),
                    true,
                    "Both lists contains 'A' and 'lazy fox'"
                },
                {
                    new StringSegmentLinkedList("A", "lazy fox"),
                    new StringSegmentLinkedList("A", "lazy Fox"),
                    false,
                    $"Both lists contains 'A' and 'lazy fox' but the casing is not the same and comparer is {nameof(CharComparer.Ordinal)} only."
                },
                {
                    new StringSegmentLinkedList("A", "lazy fox"),
                    new StringSegmentLinkedList("A", "lazy Fox"),
                    false,
                    $"comparison is done with {nameof(CharComparer.InvariantCultureIgnoreCase)} only."
                },
                {
                    new StringSegmentLinkedList("A lazy fox"),
                    new StringSegmentLinkedList("A"),
                    false,
                    "Both lists contains starts with 'A' but the right list contains only one character whereas the left is longer"
                },
                {
                    new StringSegmentLinkedList("A"),
                    new StringSegmentLinkedList("A lazy fox"),
                    false,
                    "Both lists starts with the same value but the left list contains only one value whereas the right is longer"
                },
                {
                    new StringSegmentLinkedList("A lazy ", "fox"),
                    new StringSegmentLinkedList("A lazy fox"),
                    true,
                    "Both lists would result with the same string values but were not initialize in the same way"
                }
            };

            {
                StringSegmentLinkedList left = new("aH");
                left = left.Append("&");

                StringSegmentLinkedList right = new("aH&");

                data.Add(left, right, true, "Both list would produces the same string output");
                data.Add(right, left, true, "Both list would produces the same string output");
            }
            {
                StringSegmentLinkedList left = new("0");
                StringSegmentLinkedList right = new("+0");

                data.Add(left, right, false, "['0'] is not equivalent to ['+0']");
                data.Add(left, right, false, "['+0'] is not equivalent to ['0']");
            }

            return data;
        }
    }


    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void Given_left_and_right_lists_Equals_should_behave_as_expected(StringSegmentLinkedList left, StringSegmentLinkedList right, bool expectedResult, string reason)
    {
        // Arrange
        string leftValue = $"[{string.Join(',', left.Select(node => $"[{node}]"))}]";
        string rightValue = right switch
        {
            not null => $"[{string.Join(',', right.Select(node => $"[{node}]"))}]",
            null     => string.Empty
        };
        outputHelper.WriteLine($"left is : {leftValue}");
        outputHelper.WriteLine($"right is: {rightValue}");
        // Act
        bool actual = left.Equals(right);

        //Assert
        actual.Should().Be(expectedResult, reason);
    }

    public static TheoryData<StringSegmentLinkedList, ReplacePredicateByChar, StringSegmentLinkedList> ReplacePredicateByCharCases
        => new()
        {
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                (chr => chr is 'A' or 'a',                 'E'),
                new StringSegmentLinkedList("E", "lEzy fox")
            },
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                (chr => chr is 'W' or 'w', 'E'),
                new StringSegmentLinkedList("A", "lazy fox")
            },
            {
                new StringSegmentLinkedList("killer", "ConcreteUnbranded", "Steel", "CarcopyAuto"),
                (chr => chr is 'o', 'a'),
                new StringSegmentLinkedList("killer", "CancreteUnbranded", "Steel", "CarcapyAuta")
            }
        };

    [Theory]
    [MemberData(nameof(ReplacePredicateByCharCases))]
    public void Given_a_StringSegmentLinkedList_When_replacing_a_char_using_a_predicate_Then_the_result_should_match_expectation(StringSegmentLinkedList input, ReplacePredicateByChar replacement, StringSegmentLinkedList expected)
    {
        // Act
        StringSegmentLinkedList actual = input.Replace(replacement.Predicate, replacement.NewValue);

        // Assert
        string actualStr = actual.ToStringValue();
        outputHelper.WriteLine($"{nameof(actualStr)}: '{actualStr}'");
        actualStr.Should().Be(expected.ToStringValue());
    }

    public static TheoryData<StringSegmentLinkedList, ReplacePredicateByReadOnlyMemory, StringSegmentLinkedList> ReplacePredicateByReadOnlySpanCases
        => new()
        {
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                (chr => chr is 'A' or 'a', new ReadOnlyMemory<char>(['E'])),
                new StringSegmentLinkedList("E", "lEzy fox")
            },
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                (chr => chr is 'W' or 'w',  new ReadOnlyMemory<char>(['E'])),
                new StringSegmentLinkedList("A", "lazy fox")
            }
        };

    [Theory]
    [MemberData(nameof(ReplacePredicateByReadOnlySpanCases))]
    public void Given_a_StringSegmentLinkedList_When_replacing_a_char_using_a_predicate_Then_the_result_should_match_expectation(StringSegmentLinkedList input, ReplacePredicateByReadOnlyMemory replacement, StringSegmentLinkedList expected)
    {
        // Act
        StringSegmentLinkedList actual = input.Replace(replacement.Predicate, replacement.NewValue.Span);

        // Assert
        string actualStr = actual.ToStringValue();
        outputHelper.WriteLine($"{nameof(actualStr)}: '{actualStr}'");
        actualStr.Should().Be(expected.ToStringValue());
    }

    public static TheoryData<StringSegmentLinkedList, Func<char, bool>, IReadOnlyDictionary<char, ReadOnlyMemory<char>>, StringSegmentLinkedList> ReplaceCharByCharWithPredicateAndReplaceFunctionCases
        => new()
        {
            // one match only in each node
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                chr => chr is 'A' or 'a',
                new Dictionary<char, ReadOnlyMemory<char>>
                {
                    ['A'] = "a".AsMemory(),
                    ['a'] = "A".AsMemory()
                },
                new StringSegmentLinkedList("a", "lAzy fox")
            },
            // No match
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                chr => chr is 'W' or 'w',
                new Dictionary<char, ReadOnlyMemory<char>>
                {
                    ['W'] = "a".AsMemory(),
                    ['z'] = "A".AsMemory()
                },
                new StringSegmentLinkedList("A", "lazy fox")
            },
            // More than one matching and all substitutions match
            {
                new StringSegmentLinkedList("A bad", "very lazy fox"),
                chr => chr is 'a' or 'e' or 'A',
                new Dictionary<char, ReadOnlyMemory<char>>
                {
                    ['A'] = new(['a']),
                    ['a'] = new(['A']),
                    ['e'] = new(['E'])
                },
                new StringSegmentLinkedList("a bAd", "vEry lAzy fox")
            },
            // Several matches and one miss
            {
                new StringSegmentLinkedList("A bad", "very lazy fox"),
                chr => chr is 'a' or 'e' or 'A',
                new Dictionary<char, ReadOnlyMemory<char>>
                {
                    ['A'] = new(['a']),
                    ['W'] = new(['A']),
                    ['e'] = new(['E']),
                    ['a'] = new(['A']),
                },
                new StringSegmentLinkedList("a bAd", "vEry lAzy fox")
            },
            // Several matches and one miss
            {
                new StringSegmentLinkedList("Aaaaaa bad", "very lazy fox"),
                chr => chr is 'a' or 'e' or 'A',
                new Dictionary<char, ReadOnlyMemory<char>>
                {
                    ['A'] = new(['a']),
                    ['e'] = new(['E']),
                    ['a'] = new(['A']),
                },
                new StringSegmentLinkedList("aAAAAA bAd", "vEry lAzy fox")
            }
        };

    [Theory]
    [MemberData(nameof(ReplaceCharByCharWithPredicateAndReplaceFunctionCases))]
    public void Given_a_StringSegmentLinkedList_When_replacing_a_char_using_a_predicate_and_dictionary_of_replacements_Then_the_result_should_match_expectation(StringSegmentLinkedList input, Func<char, bool> predicate, IReadOnlyDictionary<char, ReadOnlyMemory<char>> replacement, StringSegmentLinkedList expected)
    {
        // Act
        StringSegmentLinkedList actual = input.Replace(predicate, replacement);

        // Assert
        string actualStr = actual.ToStringValue();
        outputHelper.WriteLine($"{nameof(actualStr)}: '{actualStr}'");
        actualStr.Should().Be(expected.ToStringValue());
    }

    [Property(Skip = "StringSegmentLinkedList does not handle state properly")]
    public Property StringSegmentList_should_works_consistently()
        => new StringSegmentLinkedListSpecification().ToProperty();


    [Property]
    public void Given_an_empty_StringSegmentLinkedList_When_checking_if_it_contains_an_empty_string_Then_the_result_should_be_True()
    {
        // Arrange
        StringSegmentLinkedList emptyList = [];

        // Act
        bool actual = emptyList.Contains(string.Empty);

        // Assert
        actual.Should().Be(string.Empty.Contains(string.Empty));
    }

    public static TheoryData<StringSegmentLinkedList, ReadOnlyMemory<char>, IEqualityComparer<char>, bool> ContainsCases
        => new()
        {
            {
                new StringSegmentLinkedList("Hello", "world"),
                "world".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList("Hello", "world"),
                "low".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList(@"Hello\*", "world"),
                @"\*".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList(@"Hello\*", "world"),
                @"\*m".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList("Hello").Append("world"),
                "low".AsMemory(),
                CharComparer.Ordinal,
                true
            }
        };

    [Theory]
    [MemberData(nameof(ContainsCases))]
    public void Given_a_StringSegmentLinkedList_Then_Contains_should_behave_as_expected(StringSegmentLinkedList list, ReadOnlyMemory<char> search, IEqualityComparer<char> comparer, bool expected)
    {
        // Act
        bool actual = list.Contains(search.Span, comparer);

        // Assert
        actual.Should().Be(expected);
    }

    public static TheoryData<StringSegmentLinkedList, ReadOnlyMemory<char>, IEqualityComparer<char>, bool> StartsWithCases
        => new()
        {
            {
                new StringSegmentLinkedList("Hello", "world"),
                "world".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList("Hello", "world"),
                "low".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList(@"Hello\*", "world"),
                @"\*".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList(@"Hello\*", "world"),
                @"\*m".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList("Hello").Append("world"),
                "low".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList("Hello").Append("world"),
                "Hel".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList("Hello").Append("world"),
                "Hellow".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList("Hello").Append("world"),
                "Hello".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList("Hello"),
                "HelloWorld".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList("Hello").Append("Wo").Append("r").Append("ld"),
                "HelloWorld".AsMemory(),
                CharComparer.Ordinal,
                true
            }
        };

    [Theory]
    [MemberData(nameof(StartsWithCases))]
    public void Given_a_StringSegmentLinkedList_Then_StartsWith_should_behave_as_expected(StringSegmentLinkedList list, ReadOnlyMemory<char> search, IEqualityComparer<char> comparer, bool expected)
    {
        // Act
        bool actual = list.StartsWith(search.Span, comparer);

        // Assert
        actual.Should().Be(expected);
    }

    public static TheoryData<StringSegmentLinkedList, ReadOnlyMemory<char>, IEqualityComparer<char>, bool> EndsWithCases
        => new()
        {
            {
                new StringSegmentLinkedList("Hello", "world"),
                "world".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList("Hello", "world"),
                "low".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList("Hello").Append("world"),
                "world".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList("Hello").Append("world"),
                "oworld".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList("Hello").Append("world"),
                "low".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList("Hello"),
                "HelloWorld".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList("Hello").Append("Wo").Append("r").Append("ld"),
                "World".AsMemory(),
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList("Hello").Append("wo").Append("r").Append("ld"),
                "World".AsMemory(),
                CharComparer.Ordinal,
                false
            },
            {
                new StringSegmentLinkedList("Hello").Append("wo").Append("r").Append("ld"),
                "World".AsMemory(),
                CharComparer.InvariantCultureIgnoreCase,
                true
            },
            {
                new StringSegmentLinkedList(),
                ReadOnlyMemory<char>.Empty,
                CharComparer.Ordinal,
                true
            },
            {
                new StringSegmentLinkedList(),
                "a".AsMemory(),
                CharComparer.Ordinal,
                false
            }
        };

    [Theory]
    [MemberData(nameof(EndsWithCases))]
    public void Given_a_StringSegmentLinkedList_Then_EndsWith_should_behave_as_expected(StringSegmentLinkedList list, ReadOnlyMemory<char> search, IEqualityComparer<char> comparer, bool expected)
    {
        // Act
        bool actual = list.EndsWith(search.Span, comparer);

        // Assert
        actual.Should().Be(expected);
    }


    [Property(Arbitrary = [typeof(StringSegmentLinkedListGenerator)])]
    public void Given_any_StringSegmentLinkedList_then_GetHashcode_should_never_throw(StringSegmentLinkedList list)
    {
        // Act
        Action computeGetHashCode = () => _ = list.GetHashCode();

        // Assert
        computeGetHashCode.Should().NotThrow();
    }

    [Property(Arbitrary = [typeof(StringSegmentLinkedListGenerator)])]
    public void Given_any_StringSegmentLinkedList_When_Inserting_any_value_at_a_negative_index_Then_an_ArgumentOutOfRangeException_should_be_thrown(StringSegmentLinkedList list, NegativeInt indexGenerator, NonEmptyString valueGenerator)
    {
        // Arrange
        int index = indexGenerator.Item;
        string value = valueGenerator.Item;

        // Act
        Action insertingValueAtNegativeIndex = () => list.InsertAt(index, value);

        // Assert
        insertingValueAtNegativeIndex.Should().Throw<ArgumentOutOfRangeException>()
            .Which.Message.Should()
            .StartWithEquivalentOf("Index is out of range.");
    }
}