using System;

namespace UtilsLite
{
    public static class Interval
    {
        public static Interval<T> CreateInclusive<T>(T beginning, T end) where T : IComparable =>
            new Interval<T>(beginning, true, end, true);

        public static Interval<T> CreateExclusive<T>(T beginning, T end) where T : IComparable =>
            new Interval<T>(beginning, false, end, false);

        public static Interval<T> CreateCustom<T>(T beginning, bool beginningInclusive, T end, bool endInclusive) where T : IComparable =>
            new Interval<T>(beginning, beginningInclusive, end, endInclusive);
    }

    public class Interval<T> where T : IComparable
    {
        public T Beginning { get; }
        public bool BeginningInclusive { get; }
        public T End { get; }
        public bool EndInclusive { get; }

        public Interval(T beginning, bool beginningInclusive, T end, bool endInclusive)
        {
            Beginning = beginning;
            BeginningInclusive = beginningInclusive;
            End = end;
            EndInclusive = endInclusive;
        }

        public bool Includes(T value)
        {
            var beginCompare = Beginning.CompareTo(value);
            var endCompare = End.CompareTo(value);

            var beginOk = beginCompare < 0 || (BeginningInclusive && beginCompare == 0);
            var endOk = endCompare > 0 || (EndInclusive && endCompare == 0);

            return beginOk && endOk;
        }
    }
}