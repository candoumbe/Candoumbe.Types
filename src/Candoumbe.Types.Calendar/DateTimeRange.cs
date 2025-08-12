// "Copyright (c) Cyrille NDOUMBE.
// Licenced under GNU General Public Licence, version 3.0"

using System;

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Candoumbe.Types.Calendar;

/// <summary>
/// The implementation of <see cref="Range{DateTime}"/> that can be used to model a <see cref="DateTime"/> interval that
/// spans from <see cref="Range{DateTime}.Start"/> to <see cref="Range{DateTime}.End"/> (inclusive).
/// </summary>
#if !NET5_0_OR_GREATER
public class DateTimeRange : Range<DateTime>, IFormattable
    , ICanRepresentEmpty<DateTimeRange, DateTime>
    , IRange<DateTimeRange, DateTime>
#else
public record DateTimeRange : Range<DateTime>, IFormattable
#endif
#if NET7_0_OR_GREATER
    , IAdditionOperators<DateTimeRange, DateTimeRange, DateTimeRange>
    , IAdditiveIdentity<DateTimeRange, DateTimeRange>
#endif
{
    /// <summary>
    /// A <see cref="DateTimeRange"/> that cannot contains other <see cref="DateTimeRange"/> range.
    /// </summary>
    public static DateTimeRange Empty => new(DateTime.MinValue, DateTime.MinValue);

#if NET7_0_OR_GREATER
    ///<inheritdoc/>
#else
    /// <summary>
    /// Gets the additive identity for <see cref="DateTimeRange"/>
    /// </summary>
#endif
    public static DateTimeRange AdditiveIdentity => Empty;

    /// <summary>
    /// A <see cref="DateTimeRange"/> that overlaps any other <see cref="DateTimeRange"/> (except <see cref="Empty"/>)
    /// </summary>
    public static readonly DateTimeRange Infinite = new(DateTime.MinValue, DateTime.MaxValue);

    /// <summary>
    /// Builds a new <see cref="DateTimeRange"/> that span from <paramref name="start"/> to <paramref name="end"/>.
    /// </summary>
    /// <param name="start">lower bound</param>
    /// <param name="end">Upper bound</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> is after <paramref name="end"/>.</exception>
    public DateTimeRange(DateTime start, DateTime end) : base(start, end)
    {
        if (start > end)
        {
            throw new ArgumentOutOfRangeException(nameof(start), $"{nameof(start)} cannot be after {nameof(end)}");
        }
    }

    ///<inheritdoc/>
    public int CompareTo(DateTimeRange other) => other switch
    {
        null => -1,
        _ => Start.CompareTo(other.Start) switch
        {
            int and 0 => End.CompareTo(other.End),
            int value => value
        }
    };

    ///<inheritdoc/>
    public string ToString(string format, IFormatProvider provider)
        => (Start == End)
            ? Start.ToString(format, provider)
            :$"{Start.ToString(format, provider)} - {End.ToString(format, provider)}";

    /// <summary>
    /// Builds a new <see cref="DateTimeRange"/> that spans from <see cref="DateTime.MinValue"/> up to the specified <paramref name="date"/>
    /// </summary>
    /// <param name="date">The desired upper limit</param>
    /// <returns>a <see cref="DateTimeRange"/> that spans up to <paramref name="date"/>.</returns>
    public static DateTimeRange UpTo(DateTime date) => new(DateTime.MinValue, date);

    /// <summary>
    /// Builds a new <see cref="DateTimeRange"/> that spans from <paramref name="date"/> to <see cref="DateTime.MaxValue"/>.
    /// </summary>
    /// <param name="date">The desired upper limit</param>
    /// <returns>a <see cref="DateTimeRange"/> that spans from <paramref name="date"/>.</returns>
    public static DateTimeRange DownTo(DateTime date) => new(date, DateTime.MaxValue);

    /// <summary>
    /// Expands the current so that the resulting <see cref="DateTimeRange"/> spans over both current and <paramref name="other"/>.
    /// </summary>
    /// <param name="other">The other <see cref="DateTimeRange"/> to span over</param>
    /// <returns>A new <see cref="DateTimeRange"/> than spans over both current and <paramref name="other"/> range</returns>
    /// <exception cref="InvalidOperationException">if current instance does not overlap or is not continuous with <paramref name="other"/>.</exception>
    public DateTimeRange Merge(DateTimeRange other)
    {
        DateTimeRange result = Empty;
        if (other.IsEmpty())
        {
            result = this;
        }
        else if (Overlaps(other) || IsContiguousWith(other))
        {
#if NET5_0_OR_GREATER
            result = this with { Start = GetMinimum(Start, other.Start), End = GetMaximum(other.End, End) };
#else
            result = new(GetMinimum(Start, other.Start), GetMaximum(other.End, End));
#endif
        }
        else
        {
            throw new InvalidOperationException($"Cannot build a {nameof(DateTimeRange)} as union of '{this}' and {other}");
        }

        return result;
    }

    ///<inheritdoc/>
    public static DateTimeRange operator +(DateTimeRange left, DateTimeRange right) => left?.Merge(right);

    /// <summary>
    /// Returns the oldest <see cref="DateTime"/> between <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    private static DateTime GetMinimum(DateTime left, DateTime right) => left.CompareTo(right) switch
    {
        < 0 => left,
        _ => right
    };

    /// <summary>
    /// Returns the furthest <see cref="DateTime"/> between <paramref name="left"/> and <paramref name="right"/>.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    private static DateTime GetMaximum(DateTime left, DateTime right) => left.CompareTo(right) switch
    {
        > 0 => left,
        _ => right
    };

    /// <summary>
    /// Computes  <see cref="DateTimeRange"/> value that is common between the current instance and <paramref name="other"/>.
    /// </summary>
    /// <remarks>
    /// This methods relies on <see cref="Range{T}.Overlaps(Range{T})"/> to see if there can be a intersection with <paramref name="other"/>.
    /// </remarks>
    /// <param name="other">The other instance to test</param>
    /// <returns>a <see cref="DateTimeRange"/> that represent the overlap between the current instance and <paramref name="other"/> or <see cref="Empty"/> when no intersection found.</returns>
    public DateTimeRange Intersect(DateTimeRange other)
    {
        DateTimeRange result = Empty;

        if (Overlaps(other))
        {
#if NET5_0_OR_GREATER
            result = this with { Start = GetMaximum(Start, other.Start), End = GetMinimum(End, other.End) };
#else
            result = new(GetMaximum(Start, other.Start), GetMinimum(End, other.End));
#endif
        }

        return result;
    }

    /// <summary>
    /// Checks if the current instance spans over or is contiguous with any other <see cref="DateTimeRange"/>
    /// </summary>
    /// <returns><see langword="true"/> if the current instance span over any other <see cref="DateTimeRange"/> and <see langword="false"/> otherwise.</returns>
    public bool IsInfinite() => (Start, End) == (Infinite.Start, Infinite.End);

#if !NET5_0_OR_GREATER
    /// <summary>
    /// Checks if <paramref name="left"/> is not equal to <paramref name="right"/>.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/> and <see langword="false"/> otherwise.</returns>
    public static bool operator ==(DateTimeRange left, DateTimeRange right) => left?.Equals(right) ?? false;

    /// <summary>
    /// Checks if <paramref name="left"/> is not equal to <paramref name="right"/>.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/> and <see langword="false"/> otherwise.</returns>
    public static bool operator !=(DateTimeRange left, DateTimeRange right) => !(left == right);
#endif

    ///<inheritdoc/>
    public bool Overlaps(DateTimeRange other)
        => (IsInfinite() && other.IsEmpty())
           || (IsEmpty() && other.IsInfinite())
           || base.Overlaps(other);

    /// <inheritdoc />
    public bool Overlaps(DateTime other) => Start <= other && other <= End;

#if !NET5_0_OR_GREATER
    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as DateTimeRange);
#endif

    ///<inheritdoc/>
    public override int GetHashCode()
#if NET6_0_OR_GREATER
        => HashCode.Combine(Start, End);
#else
    {
        int hashCode = -1245466969;
        hashCode = (hashCode * -1521134295) + base.GetHashCode();
        hashCode = (hashCode * -1521134295) + Start.GetHashCode();
        hashCode = (hashCode * -1521134295) + End.GetHashCode();
        return hashCode;
    }
#endif

    ///<inheritdoc/>
    public virtual bool Equals(DateTimeRange other) => other is not null &&
               base.Equals(other) &&
               Start == other.Start &&
               End == other.End;
}