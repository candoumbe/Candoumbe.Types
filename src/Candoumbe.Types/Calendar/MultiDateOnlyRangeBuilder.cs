#if NET6_0_OR_GREATER
using System;
using System.Linq;

namespace Candoumbe.Types.Calendar;

internal static class MultiRangeBuilder
{
    internal static MultiDateOnlyRange CreateForDateOnly(ReadOnlySpan<DateOnlyRange> ranges)
    {
        switch (ranges)
        {
            case []:
            case [DateOnlyRange element] when element.IsEmpty():
                return MultiDateOnlyRange.Empty;
            default:
            {
                int i = 0;
                bool keepLooking = false;
                bool containsEmpty = false;
                bool containsInfinite = false;
                bool containsNonEmpty = false;
                DateOnlyRange current;
                do
                {
                    current = ranges[i];
                    containsInfinite = current.IsInfinite();
                    containsNonEmpty = !(current.IsEmpty() || current.IsInfinite());
                    i++;
                    keepLooking = (containsEmpty || containsNonEmpty) && i < ranges.Length - 1;
                } while (i < ranges.Length && keepLooking);

                return (containsInfinite, containsEmpty, containsNonEmpty) switch
                {
                    (true, _, _) => MultiDateOnlyRange.Infinite,
                    (_, true , false) => MultiDateOnlyRange.Empty,
                    _ => new MultiDateOnlyRange([.. ranges]),
                };
            }
        }
    }

    internal static MultiTimeOnlyRange CreateForTimeOnly(ReadOnlySpan<TimeOnlyRange> ranges)
    {
        switch (ranges)
        {
            case []:
            case [TimeOnlyRange element] when element.IsEmpty():
                return MultiTimeOnlyRange.Empty;
            default:
            {
                int i = 0;
                bool keepLooking = false;
                bool containsEmpty = false;
                bool containsInfinite = false;
                bool containsNonEmpty = false;
                TimeOnlyRange current;
                do
                {
                    current = ranges[i];
                    containsInfinite = current.IsInfinite();
                    containsNonEmpty = !(current.IsEmpty() || current.IsInfinite());
                    i++;
                    keepLooking = (containsEmpty || containsNonEmpty) && i < ranges.Length - 1;
                } while (i < ranges.Length && keepLooking);

                return (containsInfinite, containsEmpty, containsNonEmpty) switch
                {
                    (true, _, _) => MultiTimeOnlyRange.Infinite,
                    (_, true , false) => MultiTimeOnlyRange.Empty,
                    _ => new MultiTimeOnlyRange([.. ranges]),
                };
            }
        }
    }
    
    internal static MultiDateTimeRange CreateForTimeOnly(ReadOnlySpan<DateTimeRange> ranges)
    {
        switch (ranges)
        {
            case []:
            case [DateTimeRange element] when element.IsEmpty():
                return MultiDateTimeRange.Empty;
            default:
            {
                int i = 0;
                bool keepLooking = false;
                bool containsEmpty = false;
                bool containsInfinite = false;
                bool containsNonEmpty = false;
                DateTimeRange current;
                do
                {
                    current = ranges[i];
                    containsInfinite = current.IsInfinite();
                    containsNonEmpty = !(current.IsEmpty() || current.IsInfinite());
                    i++;
                    keepLooking = (containsEmpty || containsNonEmpty) && i < ranges.Length - 1;
                } while (i < ranges.Length && keepLooking);

                return (containsInfinite, containsEmpty, containsNonEmpty) switch
                {
                    (true, _, _) => MultiDateTimeRange.Infinite,
                    (_, true , false) => MultiDateTimeRange.Empty,
                    _ => new MultiDateTimeRange([.. ranges]),
                };
            }
        }
    }
}


#endif