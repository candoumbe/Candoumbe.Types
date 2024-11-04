// "Copyright (c) Cyrille NDOUMBE.
// Licenced under GNU General Public Licence, version 3.0"

using System;
using Bogus;
using Candoumbe.Types.Calendar;
using Candoumbe.Types.UnitTests.Generators;
using FluentAssertions;
using FluentAssertions.Extensions;
using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Candoumbe.Types.UnitTests.Calendar;

[UnitTest]
public class DateTimeRangeTests(ITestOutputHelper outputHelper)
{
    private static readonly Faker Faker = new();

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_start_gt_end_Constructor_should_feed_Properties_accordingly(DateTime start)
    {
        // Arrange
        DateTime end = start.AddDays(1);

        // Act
        Action action = () => _ = new DateTimeRange(start: end, end: start);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>("start cannot be greater than end");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_start_and_end_Constructor_should_feed_Properties_accordingly(DateTime start)
    {
        // Arrange
        DateTime end = Faker.Date.Future(refDate: start);

        // Act
        DateTimeRange range = new(start, end);

        // Assert
        range.Start.Should().Be(start);
        range.End.Should().Be(end);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_two_non_empty_DateTimeRange_that_are_equals_Overlaps_should_return_true(DateTime reference)
    {
        // Arrange
        DateTime end = reference;
        DateTime start = Faker.Date.Recent(refDate: reference);

        DateTimeRange first = new(start, end);
        DateTimeRange other = new(start, end);

        // Act
        bool overlaps = first.Overlaps(other);

        // Assert
        overlaps.Should()
                .BeTrue("Two DateTime ranges that are equal overlaps");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_current_instance_is_not_null_When_comparing_to_null_Then_result_should_be_negative(NonNull<DateTimeRange> dateTimeRangeGenerator)
    {
        // Arrange
        DateTimeRange dateTimeRange = dateTimeRangeGenerator.Item;

        // Act
        int actual = dateTimeRange.CompareTo(null);

        // Assert
        actual.Should().Be(-1);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_current_instance_is_not_null_When_comparing_to_itself_Then_result_should_be_zero(NonNull<DateTimeRange> dateTimeRangeGenerator)
    {
        // Arrange
        DateTimeRange dateTimeRange = dateTimeRangeGenerator.Item;

        // Act
        int actual = dateTimeRange.CompareTo(dateTimeRange);

        // Assert
        actual.Should().Be(0);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public Property Given_two_DateTimeRange_instances_Overlaps_should_be_symmetric(DateTimeRange left, DateTimeRange right)
    {
        outputHelper.WriteLine($"{nameof(left)}: {left}");
        outputHelper.WriteLine($"{nameof(right)}: {right}");

        return (left.Overlaps(right) == right.Overlaps(left)).ToProperty();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]

    public void Given_two_non_empty_DateTimeRange_instances_when_first_ends_where_other_starts_Abuts_should_return_true(DateTime reference)
    {
        // Arrange
        DateTime start = Faker.Date.Recent(refDate: reference);
        DateTime end = reference;

        DateTimeRange current = new(start, reference);
        DateTimeRange other = new(reference, end);

        // Act
        bool isContiguous = current.IsContiguousWith(other);

        // Assert
        isContiguous.Should()
            .BeTrue();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public Property Given_two_DateTimeRange_instances_IsContiguous_should_be_symetric(DateTimeRange left, DateTimeRange right)
    {
        outputHelper.WriteLine($"{nameof(left)}: {left}");
        outputHelper.WriteLine($"{nameof(right)}: {right}");

        return (left.IsContiguousWith(right) == right.IsContiguousWith(left)).ToProperty();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_DateTimeRange_instance_When_Start_eq_End_IsEmpty_should_be_True(DateTime reference)
    {
        // Arrange
        DateTimeRange current = new(reference, reference);

        // Act
        bool isEmpty = current.IsEmpty();

        // Assert
        isEmpty.Should()
               .BeTrue();
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_DateTime_value_UpTo_should_build_a_DateTimeRange_up_to_that_value(DateTime reference)
    {
        // Act
        DateTimeRange range = DateTimeRange.UpTo(reference);

        // Assert
        range.Start.Should()
                   .Be(DateTime.MinValue);
        range.End.Should()
                 .Be(reference);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_DateTime_value_DownTo_should_build_a_DateTimeRange_down_to_that_value(DateTime reference)
    {
        // Act
        DateTimeRange range = DateTimeRange.DownTo(reference);

        // Assert
        range.Start.Should()
                   .Be(reference);
        range.End.Should()
                 .Be(DateTime.MaxValue);
    }

    [Property]
    public void Given_DateTimeRange_When_another_DateTimeRange_does_not_overlaps_nor_is_contiguous_Then_Merge_should_throw_InvalidOperationException()
    {
        // Arrange
        DateTimeRange current = new DateTimeRange(1.January(2019), 10.January(2019));
        DateTimeRange other = new DateTimeRange(11.January(2019), 15.January(2019));

        // Act
        Action action = () => current.Merge(other);

        // Assert
        action.Should()
              .Throw<InvalidOperationException>("the two dates do not overlap").Which.Message
              .Should().NotBeNullOrEmpty("the message can be usefull for troubleshooting purposes");
    }

    public static TheoryData<DateTimeRange, DateTimeRange, bool> OverlapsCases
    {
        get
        {
            return new TheoryData<DateTimeRange, DateTimeRange, bool>()
            {
                /*
                 * first: |---------------|
                 * other:         |---------------|
                 */
                {
                    new DateTimeRange(1.April(1832), 5.April(1945)),
                    new DateTimeRange(3.April(1888), 5.April(1950)),
                    true
                },
                /*
                 * first: |---------------|
                 * other:                     |---------------|
                 */
                {
                    new DateTimeRange(1.April(1832), 5.April(1945)),
                    new DateTimeRange(3.July(1970), 5.April(1980)),
                    false
                },
                /*
                 * first: |---------------|
                 * other:                 |---------------|
                 */
                {
                    new DateTimeRange(1.April(1832), 5.April(1945)),
                    new DateTimeRange(5.April(1945), 5.April(1950)),
                    false
                },
                /*
                 * first:         |--------|
                 * other:      |---------------|
                 */
                {
                    new DateTimeRange(1.April(1832), 5.April(1945)),
                    new DateTimeRange(14.July(1789), 5.April(1950)),
                    true
                },

                /*
                 * first:    |
                 * other:      |---------------|
                 */
                {
                    new DateTimeRange(1.April(1430), 1.April(1430)),
                    new DateTimeRange(14.July(1789), 5.April(1950)),
                    false
                },

                /*
                 * first:          |
                 * other:      |---------------|
                 */
                {
                    new DateTimeRange(17.July(1859), 17.July(1859)),
                    new DateTimeRange(14.July(1789), 5.April(1950)),
                    true
                },
                /*
                 * first:      |
                 * other:      |---------------|
                 */
                {
                    new DateTimeRange(14.July(1859), 14.July(1859)),
                    new DateTimeRange(14.July(1789), 5.April(1950)),
                    true
                },
                /*
                 * first:                      |
                 * other:      |---------------|
                 */
                {
                    new DateTimeRange(5.July(1859), 5.July(1950)),
                    new DateTimeRange(14.July(1789), 5.April(1950)),
                    true
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(OverlapsCases))]
    public void Given_two_instances_Overlaps_should_behave_as_expected(DateTimeRange left, DateTimeRange right, bool expected)
    {
        // Act
        bool actual = left.Overlaps(right);

        // Assert
        actual.Should().Be(expected);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public Property Overlaps_should_be_symetric(DateTimeRange left, DateTimeRange right)
        => (left.Overlaps(right) == right.Overlaps(left)).ToProperty();

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_AllTime_when_testing_overlap_with_any_other_DateTimeRange_Overlaps_should_be_true(DateTimeRange other)
    {
        // Act
        bool actual = DateTimeRange.Infinite.Overlaps(other);

        // Assert
        actual.Should()
              .BeTrue($"AllTime range overlaps every other {nameof(DateTimeRange)}s");
    }

    public static TheoryData<DateTimeRange, DateTimeRange, DateTimeRange> MergeCases
    {
        get =>
            new()
            {
                /*
                 * current   : |---------------|
                 * other     :         |---------------|
                 * expected  : |-----------------------|
                 */
                {
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    new DateTimeRange(4.January(1990), 8.January(1990)),
                    new DateTimeRange(1.January(1990), 8.January(1990))
                },

                /*
                 * current   :         |---------------|
                 * other     : |---------------|
                 * expected  : |-----------------------|
                 */
                {
                    new DateTimeRange(4.January(1990), 8.January(1990)),
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    new DateTimeRange(1.January(1990), 8.January(1990))
                },

                /*
                 * current   :                 |---------------|
                 * other     : |---------------|
                 * expected  : |-------------------------------|
                 */
                {
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    new DateTimeRange(6.January(1990), 8.January(1990)),
                    new DateTimeRange(1.January(1990), 8.January(1990))
                },

                /*
                 * current     : |---------------|
                 * other       :                 |---------------|
                 * expected    : |-------------------------------|
                 */
                {
                    new DateTimeRange(6.January(1990), 8.January(1990)),
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    new DateTimeRange(1.January(1990), 8.January(1990))
                },

                /*
                 * current     : |---------------------|
                 * other       :         |---------|
                 * expected    : |---------------------|
                 */
                {
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    new DateTimeRange(3.January(1990), 5.January(1990)),
                    new DateTimeRange(1.January(1990), 6.January(1990))
                },

                /*
                 * current     : |---------------------|
                 * other       :         |
                 * expected    : |---------------------|
                 */
                {
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    new DateTimeRange(4.January(1990), 4.January(1990)),
                    new DateTimeRange(1.January(1990), 6.January(1990))
                }
            };
    }

    [Theory]
    [MemberData(nameof(MergeCases))]
    public void Given_two_instances_Merge_should_behave_as_expected(DateTimeRange current, DateTimeRange other, DateTimeRange expected)
    {
        // Act
        DateTimeRange actual = current.Merge(other);
        outputHelper.WriteLine($"Result: {actual}");

        // Assert
        actual.Should().Be(expected);
    }

    public static TheoryData<DateTimeRange, DateTimeRange, DateTimeRange> IntersectCases
    {
        get
        {
            return new TheoryData<DateTimeRange, DateTimeRange, DateTimeRange>()
            {
                /*
                 * current  :     |
                 * other    :  |
                 * expected :  |
                 */
                {
                    DateTimeRange.Empty,
                    DateTimeRange.Empty,
                    DateTimeRange.Empty
                },
                /*
                 * current   :  |-----------|
                 * other     :          |------------|
                 * expected  :          |---|
                 */
                {
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    new DateTimeRange(4.January(1990), 8.February(1990)),
                    new DateTimeRange(4.January(1990), 6.January(1990))
                },
                /*
                 * current   :          |------------|
                 * other     :  |-----------|
                 * expected  :          |---|
                 */
                {
                    new DateTimeRange(3.January(1990), 5.January(1990)),
                    new DateTimeRange(1.January(1990), 4.January(1990)),
                    new DateTimeRange(3.January(1990), 4.January(1990))
                },
                /*
                 * current   :  |-----------|
                 * other     :      |-----|
                 * expected  :      |-----|
                 */
                {
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    new DateTimeRange(3.January(1990), 5.January(1990)),
                    new DateTimeRange(3.January(1990), 5.January(1990))
                },

                /*
                 * current   :  |----|
                 * other     :          |------------|
                 * expected  :  |
                 */
                {
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    new DateTimeRange(18.February(1990), 25.July(1990)),
                    DateTimeRange.Empty
                },
                /*
                 * current   :  |----|
                 * other     :  ----------------------
                 * expected  :  |----|
                 */
                {
                    new DateTimeRange(1.January(1990), 6.January(1990)),
                    DateTimeRange.Infinite,
                    new DateTimeRange(1.January(1990), 6.January(1990))
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(IntersectCases))]
    public void Given_two_instances_Intersect_should_return_the_intersection(DateTimeRange current, DateTimeRange other, DateTimeRange expected)
    {
        // Act
        DateTimeRange intersection = current.Intersect(other);

        // Assert
        intersection.Should()
                    .NotBeNull().And
                    .Be(expected);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public Property Intersect_should_be_symetric(DateTimeRange left, DateTimeRange right)
        => (left.Intersect(right) == right.Intersect(left)).ToProperty();

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Empty_should_be_the_neutral_element_of_DateTimeRange(DateTimeRange range)
    {
        // Act
        DateTimeRange result = range.Merge(DateTimeRange.Empty);

        // Assert
        result.Should()
              .Be(range);
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_DateTimeRange_is_empty_When_value_is_anything_Overlaps_should_returns_Inconclusive(DateTime date)
    {
        // Arrange
        DateTimeRange empty = DateTimeRange.Empty;

        // Act
        bool actual = empty.Overlaps(date);

        // Assert
        actual.Should().Be(empty.Start == date, $"The {nameof(DateTimeRange)} is empty");
    }

    [Property]
    public void Given_DateTimeRange_is_infinite_When_value_anything_Overlaps_should_returns_Yes(DateTime date)
    {
        // Arrange
        DateTimeRange infinite = DateTimeRange.Infinite;

        // Act
        bool actual = infinite.Overlaps(date);

        // Assert
        actual.Should().BeTrue($"The {nameof(DateTimeRange.Infinite)} contains all {nameof(DateTime)} values");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public Property Given_a_range_When_testing_against_a_DateTime_that_is_between_lower_and_upper_bound_Then_Overlaps_should_return_true(NonNull<DateTimeRange> rangeGenerator)
    {
        // Arrange
        DateTimeRange range = rangeGenerator.Item;
        DateTime value = Faker.Date.Between(range.Start, range.Start);

        // Assert
        return range.Overlaps(value).ToProperty();
    }

    [Fact]
    public void Given_DateTimeRange_is_not_empty_and_not_infinite_When_value_is_not_between_Start_and_End_Overlaps_should_returns_No()
    {
        // Arrange
        DateTime start = 12.July(2012);
        DateTime end = 16.July(2012);

        DateTimeRange dateRange = new(start, end);

        DateTime value = Faker.PickRandom(Faker.Date.Recent(refDate: start.AddDays(-1)),
                                          Faker.Date.Past(refDate: start.AddDays(-1)),
                                          Faker.Date.Soon(refDate: end.AddDays(1)),
                                          Faker.Date.Future(refDate: end.AddDays(1)));

        // Act
        bool actual = dateRange.Overlaps(value);

        // Assert
        actual.Should().BeFalse($"{dateRange} does not contains {value} value");
    }

    [Property(Arbitrary = [typeof(ValueGenerators)])]
    public void Given_a_non_empty_DateTimeRange_When_merging_with_AdditiveIdentity_Then_the_result_should_be_the_initial_range(NonNull<DateTimeRange> rangeGenerator)
    {
        // Arrange
        DateTimeRange range = rangeGenerator.Item;

        // Act
        DateTimeRange actual = range.Merge(DateTimeRange.AdditiveIdentity);

        // Assert
        actual.Should().Be(range);

    }
}