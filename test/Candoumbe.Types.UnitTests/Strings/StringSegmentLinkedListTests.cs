using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Bogus;
using Candoumbe.MiscUtilities.Comparers;
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
    
            /// <summary>
        /// The <c>_</c> character
        /// </summary>
        private const char Underscore = '_';

        /// <summary>
        /// The <c>*</c> character
        /// </summary>
        private const char Asterisk = '*';

        /// <summary>
        /// The <c>=</c> character
        /// </summary>
        private const char EqualSign = '=';

        /// <summary>
        /// The <c>(</c> character
        /// </summary>
        private const char LeftParenthesis = '(';

        /// <summary>
        /// The <c>)</c> character
        /// </summary>
        private const char RightParenthesis = ')';

        /// <summary>
        /// The <c>[</c> character
        /// </summary>
        private const char LeftSquareBracket = '[';

        /// <summary>
        /// The <c>{</c> character
        /// </summary>
        private const char LeftCurlyBracket = '{';

        /// <summary>
        /// The <c>}</c> character
        /// </summary>
        private const char RightCurlyBracket = '}';

        /// <summary>
        /// The <c>]</c> character
        /// </summary>
        private const char RightSquareBracket = ']';

        /// <summary>
        /// The <c>-</c> character
        /// </summary>
        private const char Hyphen = '-';

        /// <summary>
        /// The <c>\</c> character
        /// </summary>
        public const char BackSlash = '\\';

        /// <summary>
        /// The <c>|</c> character
        /// </summary>
        private const char Pipe = '|';

        /// <summary>
        /// The <c>,</c> character
        /// </summary>
        private const char Comma = ',';

        /// <summary>
        /// The <c>!</c> character
        /// </summary>
        private const char Bang = '!';

        /// <summary>
        /// The <c>"</c> character
        /// </summary>
        public const char DoubleQuote = '"';

        /// <summary>
        /// The <c>"</c> character as a string value
        /// </summary>
        internal const string DoubleQuoteString = @"\""";

        /// <summary>
        /// The <c>&#38;</c> character
        /// </summary>
        private const char Ampersand = '&';

        /// <summary>
        /// The <c>.</c> character.
        /// </summary>
        private const char Dot = '.';

        /// <summary>
        /// The space character
        /// </summary>
        private const char Space = ' ';

        /// <summary>
        /// The character to use to escape a special character.
        /// </summary>
        public const char EscapedCharacter = BackSlash;

        /// <summary>
        /// List of characters that have a special meaning and should be escaped
        /// </summary>
        public static readonly char[] SpecialCharacters =
        [
            Asterisk,
            EqualSign,
            LeftParenthesis,
            RightParenthesis,
            LeftSquareBracket,
            RightSquareBracket,
            BackSlash,
            Pipe,
            Comma,
            Bang,
            DoubleQuote,
            Ampersand,
            RightCurlyBracket,
            LeftCurlyBracket,
            ':',
            Hyphen,
            Dot,
            Space
        ];

        internal static readonly IReadOnlyDictionary<char, ReadOnlyMemory<char>> EscapedSpecialCharacters = SpecialCharacters
            .ToDictionary(chr => chr, chr => new ReadOnlyMemory<char>([BackSlash, chr]))
#if NET8_0_OR_GREATER
            .ToFrozenDictionary()
#endif
            ;
    
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
            { new StringSegmentLinkedList("Hello"), ('H', 'h'), "hello" },
            { new StringSegmentLinkedList("Hello"), ('l', 'r'), "Herro" },
            { new StringSegmentLinkedList("Hello"), ('e', 'a'), "Hallo" },
            { new StringSegmentLinkedList("Hello"), ('o', 'a'), "Hella" },
            { new StringSegmentLinkedList("Hello", "world"), ('o', 'a'), "Hellawarld" },
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
            { new StringSegmentLinkedList("Hello", "world"), ('o', "a"), "Hellawarld" }
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
            { new StringSegmentLinkedList("Hello", "world"), ("e", "a"), "Halloworld" },
            { new StringSegmentLinkedList("Hello"), ("llo", "ro"), "Hero" },
            { new StringSegmentLinkedList("Hello"), ("ll", "r"), "Hero" },
            { new StringSegmentLinkedList("Hel", "lo"), ("ll", "r"), "Hero" },
            { new StringSegmentLinkedList("Hello", "world"), ("o", "a"), "Hellawarld" }
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

    public static TheoryData<StringSegmentLinkedList, StringSegmentLinkedList> AppendListToAnotherListCases
        => new()
        {
            {
                new StringSegmentLinkedList(),
                new StringSegmentLinkedList()
            },
            {
                new StringSegmentLinkedList("one","two"),
                new StringSegmentLinkedList()
            },
            {
                new StringSegmentLinkedList(),
                new StringSegmentLinkedList("one","two")
            },
            {
                new StringSegmentLinkedList("one","two"),
                new StringSegmentLinkedList("three", "four", "five", "six", "seven", "eight", "nine")
            }
        };

    [Theory]
    [MemberData(nameof(AppendListToAnotherListCases))]
    public void Given_an_initial_list_When_appending_another_list_Then_the_resulting_list_should_match_expectation(StringSegmentLinkedList first, StringSegmentLinkedList second)
    {
        // Act
        StringSegmentLinkedList actual = first.Append(second);

        // Assert
        actual.Should().BeEquivalentTo([..first, ..second]);
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

        // Act
        bool actual = right.Equals(left);

        // Assert
        actual.Should().BeTrue();
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

    public static TheoryData<StringSegmentLinkedList, StringSegmentLinkedList, IEqualityComparer<char>, bool, string> EqualsCases
        => new()
        {
            {
                [],
                [],
                CharComparer.InvariantCultureIgnoreCase,
                true,
                "Both lists are empty"
            },
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                new StringSegmentLinkedList("A", "lazy", " ", "fox"),
                CharComparer.Ordinal,
                true,
                "Both lists contains 'A' and 'lazy fox'"
            },
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                new StringSegmentLinkedList("A", "lazy", " ", "Fox"),
                CharComparer.Ordinal,
                false,
                "Both lists contains 'A' and 'lazy fox' but the casing is not the same and StringComparison is Ordinal only."
            },
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                new StringSegmentLinkedList("A", "lazy", " ", "Fox"),
                CharComparer.InvariantCultureIgnoreCase,
                true,
                "Both lists contains 'A' and 'lazy fox' but the casing is not the same and StringComparison is OrdinalIgnoreCase only."
            }
        };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void Given_left_and_right_lists_Equals_should_behave_as_expected(StringSegmentLinkedList left, StringSegmentLinkedList right, IEqualityComparer<char> comparer, bool expectedResult, string reason)
    {
        // Act
        bool actual = left.IsEquivalentTo(right, comparer);

        //Assert
        actual.Should().Be(expectedResult, reason);
    }

    public static TheoryData<StringSegmentLinkedList, Func<char, bool>, char, StringSegmentLinkedList> ReplaceCharByCharWithPredicateCases
        => new()
        {
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                chr => chr is 'A' or 'a',
                'E',
                new StringSegmentLinkedList("E", "lEzy fox")
            },
            {
                new StringSegmentLinkedList("A", "lazy fox"),
                chr => chr is 'W' or 'w',
                'E',
                new StringSegmentLinkedList("A", "lazy fox")
            }
        };

    [Theory]
    [MemberData(nameof(ReplaceCharByCharWithPredicateCases))]
    public void Given_a_StringSegmentLinkedList_When_replacing_a_char_using_a_predicate_Then_the_result_should_match_expectation(StringSegmentLinkedList input, Func<char, bool> predicate, char replacement, StringSegmentLinkedList expected)
    {
        // Act
        StringSegmentLinkedList actual = input.Replace(predicate, [replacement]);

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
            // More than one matching and all replacements matches
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
    public void Given_a_StringSegmentLinkedList_When_replacing_a_char_using_a_predicate_Then_the_result_should_match_expectation(StringSegmentLinkedList input, Func<char, bool> predicate, IReadOnlyDictionary<char, ReadOnlyMemory<char>> replacement, StringSegmentLinkedList expected)
    {
        // Act
        StringSegmentLinkedList actual = input.Replace(predicate, replacement);

        // Assert
        string actualStr = actual.ToStringValue();
        outputHelper.WriteLine($"{nameof(actualStr)}: '{actualStr}'");

        actualStr.Should().Be(expected.ToStringValue());
    }
}