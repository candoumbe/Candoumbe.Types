﻿// "Copyright (c) Cyrille NDOUMBE.
// Licenced under GNU General Public Licence, version 3.0"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Candoumbe.Types.Calendar.UnitTests.Generators;
using FluentAssertions;
using FluentAssertions.Extensions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Candoumbe.Types.Calendar.UnitTests;

[UnitTest]
public class MultiDateOnlyRangeTests(ITestOutputHelper outputHelper)
{
    public static TheoryData<IReadOnlyList<DateOnlyRange>, Expression<Func<MultiDateOnlyRange, bool>>> ConstructorCases
    {
        get =>
            new()
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
                        DateOnlyRange.Infinite,
                        new DateOnlyRange(DateOnly.FromDateTime(8.April(2014)), DateOnly.FromDateTime(16.April(2014)))
                    ],
                    ranges => ranges.Once() && ranges.Once(range => range == DateOnlyRange.Infinite)
                },

                /*
                 * inputs :       |--------|
                 *          ------------------------
                 *
                 * ranges : ------------------------
                 */
                {
                    [
                        new DateOnlyRange(DateOnly.FromDateTime(8.April(2014)), DateOnly.FromDateTime(16.April(2014))),
                        DateOnlyRange.Infinite
                    ],
                    ranges => ranges.Once() && ranges.Once(range => range == DateOnlyRange.Infinite)
                },
                /*
                 * inputs :       |--------|
                 *           |--------|
                 *
                 * ranges :  |-------------|
                 */
                {
                    [
                        new DateOnlyRange(DateOnly.FromDateTime(8.April(2014)), DateOnly.FromDateTime(16.April(2014))),
                        new DateOnlyRange(DateOnly.FromDateTime(5.April(2014)), DateOnly.FromDateTime(12.April(2014))),
                    ],
                    ranges => ranges.Once()
                              && ranges.Once(range => range == new DateOnlyRange(DateOnly.FromDateTime(5.April(2014)),
                                  DateOnly.FromDateTime(16.April(2014))))
                },

                /*
                 * inputs :  |--------|
                 *                |--------|
                 *
                 * ranges :  |-------------|
                 */
                {
                    [
                        new DateOnlyRange(DateOnly.FromDateTime(5.April(2014)), DateOnly.FromDateTime(12.April(2014))),
                        new DateOnlyRange(DateOnly.FromDateTime(8.April(2014)), DateOnly.FromDateTime(16.April(2014))),
                    ],
                    ranges => ranges.Once()
                              && ranges.Once(range => range == new DateOnlyRange(DateOnly.FromDateTime(5.April(2014)),
                                  DateOnly.FromDateTime(16.April(2014))))
                },
                /*
                 * inputs :  |--|
                 *                |------|
                 *
                 * ranges :  |--|
                 *                |------|
                 */
                {
                    [
                        new DateOnlyRange(DateOnly.FromDateTime(5.April(2014)), DateOnly.FromDateTime(12.April(2014))),
                        new DateOnlyRange(DateOnly.FromDateTime(14.April(2014)), DateOnly.FromDateTime(16.April(2014))),
                    ],
                    ( ranges => ranges.Exactly(2)
                                && ranges.Once(range => range == new DateOnlyRange(DateOnly.FromDateTime(5.April(2014)),
                                    DateOnly.FromDateTime(12.April(2014))))
                                && ranges.Once(range =>
                                    range == new DateOnlyRange(DateOnly.FromDateTime(14.April(2014)),
                                        DateOnly.FromDateTime(16.April(2014))))
                    )
                },
                /*
                 * inputs :  |--|
                 *             |----|
                 *                |------|
                 * ranges :  |-----------|
                 */
                {
                    [
                        new DateOnlyRange(DateOnly.FromDateTime(5.April(2014)), DateOnly.FromDateTime(12.April(2014))),
                        new DateOnlyRange(DateOnly.FromDateTime(9.April(2014)), DateOnly.FromDateTime(14.April(2014))),
                        new DateOnlyRange(DateOnly.FromDateTime(12.April(2014)), DateOnly.FromDateTime(18.April(2014))),
                    ],
                    ranges => ranges.Once()
                              && ranges.Once(range => range == new DateOnlyRange(DateOnly.FromDateTime(5.April(2014)),
                                  DateOnly.FromDateTime(18.April(2014))))
                }
            };
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_a_non_null_instance_When_adding_a_null_DateOnlyRange_Then_Add_should_throw_ArgumentNullException(MultiDateOnlyRange multiDateOnlyRange)
    {
        // Act
        Action addingNull = () => multiDateOnlyRange.Add(null);

        // Assert
        addingNull.Should().Throw<ArgumentNullException>("cannot add null value")
                  .Where(ex => !string.IsNullOrWhiteSpace(ex.ParamName));
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_an_array_of_DateOnlyRanges_When_building_a_new_instance_Then_Constructor_should_order_them_from_(NonEmptyArray<DateOnlyRange> ranges)
    {
        // Arranges
        DateOnlyRange[] values = ranges.Get;

        // Act
        MultiDateOnlyRange multiDateOnlyRange = [.. values];

        // Assert
        multiDateOnlyRange.Should()
                          .OnlyHaveUniqueItems("the constructor handles duplicates").And
                          .BeInAscendingOrder(x => x.Start, "the constructor reorders values added if necessary");
        foreach (DateOnlyRange range in values)
        {
            multiDateOnlyRange.Overlaps(range).Should()
                                              .BeTrue("the instance must covers every ranges that was used to build it");
        }
    }

    [Theory]
    [MemberData(nameof(ConstructorCases))]
    public void Given_non_empty_array_of_DateOnlyRange_Constructor_should_merge_them(DateOnlyRange[] dateOnlyRanges, Expression<Func<MultiDateOnlyRange, bool>> rangeExpectation)
    {
        // Act
        MultiDateOnlyRange range = [.. dateOnlyRanges];

        // Assert
        range.Should()
             .Match(rangeExpectation).And
             .OnlyHaveUniqueItems("the constructor handles duplicates").And
             .BeInAscendingOrder(x => x.Start, "the constructor reorders values added if necessary");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_two_date_only_ranges_that_overlaps_each_other_When_adding_them_using_Add_Then_should_merge_them_into_one_Range_only(NonNull<DateOnlyRange> leftSource, NonNull<DateOnlyRange> rightSource)
    {
        // Arrange
        DateOnlyRange left = leftSource.Item;
        DateOnlyRange right = rightSource.Item;
        MultiDateOnlyRange range = [ left ];

        // Act
        range.Add(right);

        // Assert
        _ = (left.IsEmpty(), right.IsEmpty(), left.IsContiguousWith(right) || left.Overlaps(right)) switch
        {
            (true, false, true) => range.Should()
                                        .HaveCount(1).And
                                        .Contain(right),
            (false, true, true) => range.Should()
                                        .HaveCount(1).And
                                        .Contain(left),
            (false, true, false) => range.Should()
                                         .HaveCount(2).And
                                         .Contain(left).And
                                         .Contain(right),
            (false, false, true) => range.Should()
                                         .HaveCount(1, "left and right are either contiguous or overlap each other").And
                                         .Contain(left.Merge(right)),
            (true, true, _) => range.Should().BeEmpty(),
            _ => range.Should()
                      .HaveCount(2).And
                      .ContainSingle(range => range == right).And
                      .ContainSingle(range => range == left)
        };
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_an_instance_that_one_range_eq_Infinite_When_adding_any_other_range_Should_result_in_a_noop_call(NonEmptyArray<DateOnlyRange> ranges)
    {
        // Arrange
        MultiDateOnlyRange sut = new MultiDateOnlyRange(DateOnlyRange.Infinite);

        // Act
        ranges.Item.ForEach(sut.Add);

        // Assert
        sut.IsEmpty().Should().BeFalse();
        sut.IsInfinite().Should().BeTrue($"The initial {nameof(MultiDateOnlyRange)} already contains infinite");
        sut.Should()
           .HaveCount(1, "The only range is infinite").And
           .Contain(range => range.IsInfinite());
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_one_MultiDateOnlyRange_when_calling_union_with_an_other_MultiDateOnlyRange_Then_the_result_should_covers_all_DateOnlyRange_from_initial_MultiDateOnlyRange(NonNull<MultiDateOnlyRange> leftSource, NonNull<MultiDateOnlyRange> rightSource)
    {
        // Arrange
        MultiDateOnlyRange left = leftSource.Item;
        MultiDateOnlyRange right = rightSource.Item;

        outputHelper.WriteLine($"{nameof(left)} : {left}");
        outputHelper.WriteLine($"{nameof(right)} : {right}");

        // Act
        MultiDateOnlyRange union = left.Merge(right);

        // Assert
        outputHelper.WriteLine($"Union : {union}");
        foreach (DateOnlyRange range in left.Concat(right))
        {
            union.Overlaps(range).Should().BeTrue();
        }

        DateOnlyRange[] ranges = [.. union];

        ranges.ForEach((range, index) =>
        {
            for (int i = 0; i < ranges.Length; i++)
            {
                if (i != index)
                {
                    bool overlaps = range.Overlaps(ranges[i]);
                    bool abuts = range.IsContiguousWith(ranges[i]);

                    overlaps.Should().BeFalse($"{nameof(MultiDateOnlyRange)} internal storage is optimized to not hold two {nameof(DateOnlyRange)}s that overlap each other");
                    abuts.Should().BeFalse($"{nameof(MultiDateOnlyRange)} internal storage is optimized to not hold two {nameof(DateOnlyRange)}s that abuts each other");
                }
            }
        });
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_two_non_null_instances_when_calling_plus_operator_should_have_same_result_as_calling_Union_method(NonNull<MultiDateOnlyRange> leftSource, NonNull<MultiDateOnlyRange> rightSource)
    {
        // Arrange
        MultiDateOnlyRange left = leftSource.Item;
        MultiDateOnlyRange right = rightSource.Item;

        outputHelper.WriteLine($"{nameof(left)} : {left}");
        outputHelper.WriteLine($"{nameof(right)} : {right}");
        MultiDateOnlyRange expected = left.Merge(right);

        // Act
        MultiDateOnlyRange actual = left + right;

        // Assert
        actual.Should().Equal(expected);
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
                new MultiDateOnlyRange(DateOnlyRange.Infinite),
                DateOnlyRange.Infinite,
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
                new MultiDateOnlyRange(new DateOnlyRange(DateOnly.FromDateTime(6.April(2014)), DateOnly.FromDateTime(9.April(2014))),
                                       new DateOnlyRange(DateOnly.FromDateTime(10.April(2014)), DateOnly.FromDateTime(12.April(2014)))),
                new DateOnlyRange(DateOnly.FromDateTime(8.April(2014)), DateOnly.FromDateTime(11.April(2014))),
                false
            ];
        }
    }

    [Theory]
    [MemberData(nameof(CoversCases))]
    public void Given_non_empty_MultiDateOnlyRange_instance_when_DateOnlyRange_is_not_empty_Covers_should_behave_as_expected(MultiDateOnlyRange multiDateOnlyRange, DateOnlyRange dateOnlyRange, bool expected)
    {
        // Act
        bool actual = multiDateOnlyRange.Overlaps(dateOnlyRange);

        // Assert
        actual.Should().Be(expected);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_non_null_MultiDateOnlyRange_instance_When_adding_to_its_complement_Union_should_return_Infinite(MultiDateOnlyRange original)
    {
        // Arrange
        MultiDateOnlyRange complement = original.Complement();
        outputHelper.WriteLine($"Complement of {original} is {complement}");

        // Act
        MultiDateOnlyRange actual = original + complement;
        outputHelper.WriteLine($"Union of {original} and {complement} is {actual}");

        // Assert
        actual.Should().BeEquivalentTo(MultiDateOnlyRange.Infinite);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_MultiDateOnlyRange_When_calling_Complement_on_the_complement_of_initial_value_Then_result_should_be_eq_to_the_initial_value(MultiDateOnlyRange range)
    {
        // Arrange
        MultiDateOnlyRange complement = range.Complement();

        // Act
        MultiDateOnlyRange actual = complement.Complement();

        // Assert
        actual.Should().BeEquivalentTo(range);
    }

    public static IEnumerable<object[]> UnionCases
    {
        get
        {
            yield return
            [
                MultiDateOnlyRange.Infinite,
                MultiDateOnlyRange.Empty,
                MultiDateOnlyRange.Infinite
            ];

            yield return
            [
                new MultiDateOnlyRange(new DateOnlyRange(DateOnly.FromDateTime(27.March(1900)), DateOnly.FromDateTime(6.January(2071))),
                                       new DateOnlyRange(DateOnly.FromDateTime(17.March(2079)), DateOnly.FromDateTime(2.February(2084)))),
                new MultiDateOnlyRange(DateOnlyRange.UpTo(DateOnly.FromDateTime(27.March(1900))),
                                       new DateOnlyRange(DateOnly.FromDateTime(6.January(2071)), DateOnly.FromDateTime(17.March(2079))),
                                       DateOnlyRange.DownTo(DateOnly.FromDateTime(2.February(2084)))),
                MultiDateOnlyRange.Infinite
            ];
        }
    }

    [Theory]
    [MemberData(nameof(UnionCases))]
    public void Given_two_MultiDateOnlyRange_instances_Union_should_behave_as_expected(MultiDateOnlyRange left, MultiDateOnlyRange right, MultiDateOnlyRange expected)
    {
        // Act
        MultiDateOnlyRange actual = left + right;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_left_is_not_null_When_right_is_null_Then_Merge_should_throw_ArgumentNullException(NonNull<MultiDateOnlyRange> rangeGenerator)
    {
        // Arrange
        MultiDateOnlyRange left = rangeGenerator.Get;

        // Act
        Action mergingWithNull = () => _ = left.Merge(null);

        // Assert
        mergingWithNull.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("other");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_left_is_not_null_When_right_is_null_Then_Plus_operator_should_throw_ArgumentNullException(NonNull<MultiDateOnlyRange> rangeGenerator)
    {
        // Arrange
        MultiDateOnlyRange left = rangeGenerator.Get;

        // Act
        Action mergingWithNull = () => _ = left + (MultiDateOnlyRange)null;

        // Assert
        mergingWithNull.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("right");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_right_is_not_null_When_left_is_null_Then_Plus_operator_should_throw_ArgumentNullException(NonNull<MultiDateOnlyRange> rangeGenerator)
    {
        // Arrange
        MultiDateOnlyRange right = rangeGenerator.Get;

        // Act
        Action mergingWithNull = () => _ = null + right;

        // Assert
        mergingWithNull.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("left");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_any_MultiDateOnlyRange_instance_When_adding_its_complement_Then_result_should_be_Infinite(MultiDateOnlyRange value)
    {
        // Arrange
        MultiDateOnlyRange complement = value.Complement();
        outputHelper.WriteLine($"Complement is {complement}");

        // Act
        MultiDateOnlyRange result = value + complement;

        // Assert
        result.Should()
            .BeEquivalentTo(MultiDateOnlyRange.Infinite)
            .And.HaveCount(1,"+ operator should merge complementary values");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_an_instance_that_is_not_null_When_adding_a_DateOnlyRange_that_is_infinite_Then_IsInfinite_should_return_true(NonEmptyArray<DateOnlyRange> ranges)
    {
        // Arrange
        MultiDateOnlyRange range = [.. ranges.Get];

        // Act
        range.Add(DateOnlyRange.Infinite);

        // Assert
        range.IsInfinite().Should().BeTrue();
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

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_an_instance_that_is_not_null_Then_ToString_should_produce_expected_output(MultiDateOnlyRange range)
    {
        // Arrange
        string expected;
        if (range.IsInfinite())
        {
            expected = "{infinite}";
        }
        else if (range.IsEmpty())
        {
            expected = "{empty}";
        }
        else
        {
            StringBuilder sb = new();
            int i = 0;
            foreach (DateOnlyRange item in range)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }

                sb.Append(item);
                i++;
            }

            expected = sb.Insert(0, '{').Append('}').ToString();
        }

        // Act
        string actual = range.ToString();

        // Assert
        actual.Should().Be(expected);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_current_instance_is_infinite_When_Adding_any_other_value_Then_result_should_be_infinite(DateOnlyRange other)
    {
        // Arrange
        MultiDateOnlyRange current = MultiDateOnlyRange.Infinite;

        // Act
        MultiDateOnlyRange actual = current + other;

        // Assert
        actual.Should()
            .BeEquivalentTo(MultiDateOnlyRange.Infinite)
            .And.HaveCount(1);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public Property Given_a_non_null_range_When_calling_Complement_Then_the_result_should_be_equivalent_to_calling_the_unary_operator(NonNull<MultiDateOnlyRange> rangeGenerator)
    {
        // Arrange
        MultiDateOnlyRange range = rangeGenerator.Item;

        // Assert
        return (range.Complement() == (-range)).ToProperty();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public Property Equals_should_behave_the_same_way_as_equality_operator(NonNull<MultiDateOnlyRange> leftGenerator, NonNull<MultiDateOnlyRange> rightGenerator)
    {
        // Arrange
        MultiDateOnlyRange left = leftGenerator.Item;
        MultiDateOnlyRange right = rightGenerator.Item;

        // Act
        return (left.Equals(right) == (left == right)).ToProperty();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_left_is_not_null_When_right_is_null_Then_Equal_operator_should_return_false(NonNull<MultiDateOnlyRange> leftGenerator)
    {
        // Act
        MultiDateOnlyRange left = leftGenerator.Item;

        // Act
        bool actual = left == null;

        // Assert
        actual.Should().BeFalse();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_left_is_null_When_right_is_not_null_Then_Equal_operator_should_return_false(NonNull<MultiDateOnlyRange> rightGenerator)
    {
        // Act
        MultiDateOnlyRange left = null;
        MultiDateOnlyRange right = rightGenerator.Item;

        // Act
        bool actual = left == right;

        // Assert
        actual.Should().BeFalse();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_left_is_null_When_right_is_null_Then_Equal_operator_should_return_true()
    {
        // Act
        MultiDateOnlyRange left = null;
        MultiDateOnlyRange right = null;

        // Act
        bool actual = left == right;

        // Assert
        actual.Should().BeTrue();
    }
}