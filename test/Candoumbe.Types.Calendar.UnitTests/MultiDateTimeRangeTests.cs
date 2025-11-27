// "Copyright (c) Cyrille NDOUMBE.
// Licenced under GNU General Public Licence, version 3.0"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Candoumbe.Types.Calendar.UnitTests.Generators;
using FluentAssertions;
using FluentAssertions.Extensions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Candoumbe.Types.Calendar.UnitTests;

[UnitTest]
public class MultiDateTimeRangeTests(ITestOutputHelper outputHelper)
{
    public static TheoryData<IEnumerable<DateTimeRange>, Expression<Func<MultiDateTimeRange, bool>>> ConstructorCases
    {
        get
        {
            return new TheoryData<IEnumerable<DateTimeRange>, Expression<Func<MultiDateTimeRange, bool>>>
            {
                {
                    [],
                    ranges => ranges.Exactly(0)
                },
                /*
                 * inputs :  ------------------------
                 *                 |--------|
                 *
                 * ranges : ------------------------
                 */
                {
                    [
                        DateTimeRange.Infinite,
                        new DateTimeRange(8.April(2014), 16.April(2014))
                    ],
                    ranges => ranges.Once()
                              && ranges.Once(range => range.IsInfinite())
                },

                /*
                 * inputs :       |--------|
                 *          ------------------------
                 *
                 * ranges : ------------------------
                 */
                {
                    [
                        new DateTimeRange(8.April(2014), 16.April(2014)),
                        DateTimeRange.Infinite
                    ],
                    ranges => ranges.Once()
                              && ranges.Once(range => range == DateTimeRange.Infinite)
                },

                /*
                 * inputs :       |--------|
                 *           |--------|
                 *
                 * ranges :  |-------------|
                 */
                {
                    [
                        new DateTimeRange(8.April(2014), 16.April(2014)),
                        new DateTimeRange(5.April(2014), 12.April(2014)),
                    ],
                    ranges => ranges.Once()
                              && ranges.Once(range => range == new DateTimeRange(5.April(2014), 16.April(2014)))
                },
                /*
                 * inputs :  |--------|
                 *                |--------|
                 * ranges :  |-------------|
                 */
                {
                    [
                        new DateTimeRange(5.April(2014), 12.April(2014)),
                        new DateTimeRange(8.April(2014), 16.April(2014))
                    ],
                    ranges => ranges.Once()
                              && ranges.Once(range => range == new DateTimeRange(5.April(2014), 16.April(2014)))
                },

                /*
                 * inputs :  |--|
                 *                |------|
                 * ranges :  |--|
                 *                |------|
                 */
                {
                    [
                        new DateTimeRange(5.April(2014), 12.April(2014)),
                        new DateTimeRange(14.April(2014), 16.April(2014)),
                    ],
                    ranges => ranges.Exactly(2)
                              && ranges.Once(range => range == new DateTimeRange(5.April(2014), 12.April(2014)))
                              && ranges.Once(range => range == new DateTimeRange(14.April(2014), 16.April(2014)))

                },

                /*
                 * inputs :  |--|
                 *             |----|
                 *                |------|
                 * ranges :  |-----------|
                 */
                {
                    [
                        new DateTimeRange(5.April(2014), 12.April(2014)),
                        new DateTimeRange(9.April(2014), 14.April(2014)),
                        new DateTimeRange(12.April(2014), 18.April(2014)),
                    ],
                     ranges => ranges.Once()
                        && ranges.Once(range => range == new DateTimeRange(5.April(2014), 18.April(2014)))
                },
                /*
                 * inputs :  |--|
                 *                   |------|
                 *              |----|
                 * ranges :  |--------------|
                 */
                {
                    [
                        new DateTimeRange(1.April(2014), 9.April(2014)),
                        new DateTimeRange(14.April(2014), 18.April(2014)),
                        new DateTimeRange(9.April(2014), 14.April(2014)),
                    ],
                    ranges => ranges.Once()
                              && ranges.Once(range => range == new DateTimeRange(1.April(2014), 18.April(2014)))
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(ConstructorCases))]
    public void Given_non_empty_array_of_DateTimeRange_Constructor_should_merge_them(DateTimeRange[] dateTimeRanges, Expression<Func<MultiDateTimeRange, bool>> rangeExpectation)
    {
        // Act
        MultiDateTimeRange range = new(dateTimeRanges);

        // Assert
        range.Should()
             .Match(rangeExpectation);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_two_date_only_ranges_that_overlaps_each_other_When_adding_them_using_Add_Then_should_pack_into_one_Range_only(NonNull<DateTimeRange> leftSource, NonNull<DateTimeRange> rightSource)
    {
        // Arrange
        DateTimeRange left = leftSource.Item;
        DateTimeRange right = rightSource.Item;
        MultiDateTimeRange range =
        [
            left,
            // Act
            right
            // Assert
        ];

        // Act
        range.Add(right);

        // Assert
        _ = (left.IsContiguousWith(right) || left.Overlaps(right)) switch
        {
            true => range.Should()
                         .HaveCount(1).And
                         .ContainSingle(range => range == left.Merge(right)),
            _ => range.Should()
                .HaveCount(2).And
                .Contain(left).And
                .Contain(right)
        };
    }

    [Property(Arbitrary = [typeof(ValueGenerators)], Replay = "(16035462008899407285,4231346714582972547)")]
    public void Given_an_instance_that_one_range_eq_Infinite_When_adding_any_other_range_Should_result_in_a_noop_call(NonEmptyArray<DateTimeRange> ranges)
    {
        // Arrange
        MultiDateTimeRange sut = MultiDateTimeRange.Infinite;

        // Act
        ranges.Item.ForEach(range => sut.Add(range));

        // Assert
        sut.IsEmpty().Should().BeFalse();
        sut.IsInfinite().Should().BeTrue($"The initial {nameof(MultiDateTimeRange)} already contains infinite");
        sut.Should()
           .HaveCount(1, "The only range is infinite").And
           .Contain(range => range == DateTimeRange.Infinite);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_one_MultiDateTimeRange_when_calling_union_with_an_other_MultiDateTimeRange_Then_the_result_should_covers_all_DateTimeRange_from_initial_MultiDateTimeRange(NonNull<MultiDateTimeRange> leftSource, NonNull<MultiDateTimeRange> rightSource)
    {
        // Arrange
        MultiDateTimeRange left = leftSource.Item;
        MultiDateTimeRange right = rightSource.Item;

        outputHelper.WriteLine($"{nameof(left)} : {left}");
        outputHelper.WriteLine($"{nameof(right)} : {right}");

        // Act
        MultiDateTimeRange union = left.Merge(right);

        // Assert
        outputHelper.WriteLine($"Union : {union}");
        foreach (DateTimeRange range in left.Concat(right))
        {
            union.Overlaps(range).Should().BeTrue();
        }

        DateTimeRange[] ranges =  [ ..union];

        ranges.ForEach((range, index) =>
        {
            for (int i = 0; i < ranges.Length; i++)
            {
                if (i != index)
                {
                    bool overlaps = range.Overlaps(ranges[i]);
                    bool abuts = range.IsContiguousWith(ranges[i]);

                    overlaps.Should().BeFalse($"{nameof(MultiDateTimeRange)} internal storage is optimized to not hold two {nameof(DateTimeRange)}s that overlap each other");
                    abuts.Should().BeFalse($"{nameof(MultiDateTimeRange)} internal storage is optimized to not hold two {nameof(DateTimeRange)}s that abuts each other");
                }
            }
        });
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_two_non_null_instances_when_calling_plus_operator_should_have_same_result_as_calling_Union_method(NonNull<MultiDateTimeRange> leftSource, NonNull<MultiDateTimeRange> rightSource)
    {
        // Arrange
        MultiDateTimeRange left = leftSource.Item;
        MultiDateTimeRange right = rightSource.Item;

        outputHelper.WriteLine($"{nameof(left)} : {left}");
        outputHelper.WriteLine($"{nameof(right)} : {right}");
        MultiDateTimeRange expected = left.Merge(right);

        // Act
        MultiDateTimeRange actual = left + right;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    public static IEnumerable<object[]> CoversCases
    {
        get
        {
            /*
             * multirange : ----------------------
             * current    : ---------------------- 
             * expected   : true
             */
            yield return
            [
                new MultiDateTimeRange(DateTimeRange.Infinite),
                DateTimeRange.Infinite,
                true
            ];

            /*
             * multirange :       |--------|
             *              |--|
             * current    :   |-----|
             * expected   : false
             */
            yield return
            [
                new MultiDateTimeRange(new DateTimeRange(6.April(2014), 9.April(2014)),
                                       new DateTimeRange(10.April(2014), 12.April(2014))),
                new DateTimeRange(8.April(2014), 11.April(2014)),
                false
            ];
        }
    }

    [Theory]
    [MemberData(nameof(CoversCases))]
    public void Given_non_empty_MultiDateTimeRange_instance_when_DateTimeRange_is_not_empty_Covers_should_behave_as_expected(MultiDateTimeRange multiDateTimeRange, DateTimeRange dateOnlyRange, bool expected)
    {
        // Act
        bool actual = multiDateTimeRange.Overlaps(dateOnlyRange);

        // Assert
        actual.Should().Be(expected);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_non_null_MultiDateTimeRange_instance_When_adding_to_its_complement_Union_should_return_Infinite(MultiDateTimeRange original)
    {
        // Arrange
        MultiDateTimeRange complement = original.Complement();
        outputHelper.WriteLine($"Complement of {original} is {complement}");

        // Act
        MultiDateTimeRange actual = original + complement;
        outputHelper.WriteLine($"Union of {original} and {complement} is {actual}");

        // Assert
        actual.Should().BeEquivalentTo(MultiDateTimeRange.Infinite);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)], Replay = "(13215877118328040669,378035299666480085)")]
    public void Given_MultiDateTimeRange_When_calling_Complement_on_the_complement_of_initial_value_Then_result_should_be_eq_to_the_initial_value(MultiDateTimeRange range)
    {
        // Arrange
        MultiDateTimeRange complement = range.Complement();
        outputHelper.WriteLine($"Complement is {complement}");

        // Act
        MultiDateTimeRange actual = complement.Complement();

        // Assert
        actual.Should().BeEquivalentTo(range);
    }

    public static IEnumerable<object[]> UnionCases
    {
        get
        {
            yield return
            [
                MultiDateTimeRange.Infinite,
                MultiDateTimeRange.Empty,
                MultiDateTimeRange.Infinite
            ];

            yield return
            [
                new MultiDateTimeRange(new DateTimeRange(27.March(1900), 6.January(2071)),
                                       new DateTimeRange(17.March(2079), 2.February(2084))),
                new MultiDateTimeRange(DateTimeRange.UpTo(27.March(1900)),
                                       new DateTimeRange(6.January(2071), 17.March(2079)),
                                       DateTimeRange.DownTo(2.February(2084))),
                MultiDateTimeRange.Infinite
            ];
        }
    }

    [Theory]
    [MemberData(nameof(UnionCases))]
    public void Given_two_MultiDateTimeRange_instances_Union_should_behave_as_expected(MultiDateTimeRange left, MultiDateTimeRange right, MultiDateTimeRange expected)
    {
        // Act
        MultiDateTimeRange actual = left + right;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_any_MultiDateTimeRange_instance_When_adding_its_complement_Then_result_should_be_Infinite(MultiDateTimeRange value)
    {
        // Arrange
        MultiDateTimeRange complement = value.Complement();
        outputHelper.WriteLine($"Complement is {complement}");

        // Act
        MultiDateTimeRange result = value + complement;

        // Assert
        result.Should().BeEquivalentTo(MultiDateTimeRange.Infinite);
    }

    [Fact]
    public void Given_non_null_instance_When_adding_a_DateTimeRange_that_is_infinite_Then_IsInfinite_should_return_true()
    {
        // Arrange
        MultiDateTimeRange range = [];

        // Act
        range.Add(DateTimeRange.Infinite);

        // Assert
        range.IsInfinite().Should().BeTrue();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_an_empty_range_When_Then_Overlaps_should_return_always_returns_false(DateTimeRange range)
    {
        // Arrange
        MultiDateTimeRange planning = [];

        // Act
        bool overlaps = planning.Overlaps(range);

        // Assert
        overlaps.Should().BeFalse();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_an_instance_that_is_not_null_When_adding_a_DateOnlyRange_that_is_infinite_Then_IsInfinite_should_return_true(NonEmptyArray<DateOnlyRange> ranges)
    {
        // Arrange
        MultiDateOnlyRange range = [.. ranges.Item];

        // Act
        range.Add(DateOnlyRange.Infinite);

        // Assert
        range.IsInfinite().Should()
            .BeTrue();
        range.Should().HaveCount(1);
    }

    [Fact]
    public void Given_an_instance_that_contains_no_DateOnlyRange_Then_IsEmpty_should_return_true()
    {
        // Arrange
        MultiDateOnlyRange range = [];

        // Assert
        range.IsEmpty().Should().BeTrue();
    }

    public static TheoryData<MultiDateTimeRange, string, string> ToStringCases
        => new()
        {
            {
                MultiDateTimeRange.Infinite,
                "{infinite}",
                "The planning is an infinite range."
            },
            {
                MultiDateTimeRange.Empty,
                "{empty}",
                "The planning is empty"
            },
            {
                [DateTimeRange.UpTo(18.February(2018)), DateTimeRange.DownTo(18.February(2018))],
                "{infinite}",
                "The combined interval represent an infinite range"
            }
        };

    [Theory]
    [MemberData(nameof(ToStringCases))]
    public void Given_an_instance_that_is_not_null_Then_ToString_should_produce_expected_output(MultiDateTimeRange planning, string expected, string reason)
    {
        // Act
        string actual = planning.ToString();

        // Assert
        actual.Should().Be(expected);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_current_instance_is_infinite_When_Adding_any_other_value_Then_result_should_be_infinite(DateTimeRange other)
    {
        // Arrange
        MultiDateTimeRange current = MultiDateTimeRange.Infinite;

        // Act
        MultiDateTimeRange actual = current + other;

        // Assert
        actual.Should()
            .BeEquivalentTo(MultiDateTimeRange.Infinite)
            .And.HaveCount(1);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_an_instance_that_is_not_null_When_calling_Complement_Then_the_result_should_be_equivalent_to_using_the_minus_operator(NonNull<MultiDateTimeRange> multiDateTimeRangeGenerator)
    {
        // Arrange
        MultiDateTimeRange range = multiDateTimeRangeGenerator.Item;
        MultiDateTimeRange expected = range.Complement();

        // Act
        MultiDateTimeRange actual = -range;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Overlaps_returns_false_when_no_overlapping_date_time_ranges()
    {
        // Arrange
        MultiDateTimeRange first =
        [
            new DateTimeRange(1.January(2022), 5.January(2022)),
            new DateTimeRange(10.January(2022), 15.January(2022))
        ];
        MultiDateTimeRange other =
        [
            new DateTimeRange(15.January(2022), 20.January(2022)),
            new DateTimeRange(25.January(2022), 30.January(2022))
        ];

        // Act
        bool result = first.Overlaps(other);

        // Assert
        result.Should().BeFalse();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Overlaps_returns_false_when_other_is_null(DateTime startTime)
    {
        // Arrange
        DateTime endTime = startTime + 1.Hours();
        MultiDateTimeRange multiRange = [new DateTimeRange(startTime, endTime)];

        // Act
        bool result = multiRange.Overlaps((MultiDateTimeRange)null);

        // Assert
        result.Should().BeFalse();
    }

    public static TheoryData<MultiDateTimeRange, object, bool, string> EqualsCases
        => new TheoryData<MultiDateTimeRange, object, bool, string> ()
        {
            {


                /*
                 * multirange : |
                 * other    : |
                 * expected   : true
                 */
                MultiDateTimeRange.Empty,
                MultiDateTimeRange.Empty,
                true,
                "Both plannings represent empty ranges"
            },
            {
                /*
                 * multirange : |----------------------|
                 * other      : |----|
                 *                        |------------|
                 *                   |----|
                 * 
                 * expected   : true
                 */
                [
                    new DateTimeRange(1.January(2012), 5.January(2012))
                ],
                new MultiDateTimeRange
                {
                    new DateTimeRange(1.January(2012), 2.January(2012)),
                    new DateTimeRange(3.January(2012), 5.January(2012)),
                    new DateTimeRange(2.January(2012), 3.January(2012)),
                },
                true,
                "The current multirange covers each ranges of the other instance"
            }
        };

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void Given_two_instances_of_MultiDateTimeRange_When_calling_Equals_Then_it_should_return_expected_result(MultiDateTimeRange first, object other, bool expected, string reason)
    {
        outputHelper.WriteLine($"{first} equal to {other}");

        // Act
        bool actual = first.Equals(other);

        // Assert
        actual.Should().Be(expected, reason);
    }
}