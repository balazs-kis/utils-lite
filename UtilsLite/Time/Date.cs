using System;

namespace UtilsLite.Time
{
    public class Date : IEquatable<Date>, IComparable<Date>, IComparable
    {
        private readonly DateTime _dateTime;

        public long Ticks => _dateTime.Ticks;
        public int Month => _dateTime.Month;
        public int DayOfYear => _dateTime.DayOfYear;
        public DayOfWeek DayOfWeek => _dateTime.DayOfWeek;
        public int Day => _dateTime.Day;
        public int Year => _dateTime.Year;


        private Date(DateTime dateTime)
        {
            _dateTime = dateTime.Date;
        }


        public static Date FromDateTime(DateTime d) => new Date(d);

        public Date AddDays(int days) => new Date(_dateTime.AddDays(days));

        public Date AddMonths(int months) => new Date(_dateTime.AddMonths(months));

        public Date AddYears(int years) => new Date(_dateTime.AddYears(years));


        public bool Equals(Date other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _dateTime.Equals(other._dateTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Date)obj);
        }

        public int CompareTo(Date other)
        {
            return _dateTime.CompareTo(other._dateTime);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new InvalidOperationException($"{nameof(Date)}: obj is null");
            }

            if (!(obj is Date))
            {
                throw new InvalidOperationException($"{nameof(Date)}: obj is not Date");
            }

            var other = (Date)obj;
            return CompareTo(other);
        }

        public override int GetHashCode()
        {
            return _dateTime.GetHashCode();
        }


        public static bool operator ==(Date d1, Date d2) => d1?.Equals(d2) ?? d2 is null;

        public static bool operator !=(Date d1, Date d2) => !(d1 == d2);

        public static bool operator <(Date left, Date right) => left is null ? !(right is null) : left.CompareTo(right) < 0;

        public static bool operator <=(Date left, Date right) => left is null || left.CompareTo(right) <= 0;

        public static bool operator >(Date left, Date right) => !(left is null) && left.CompareTo(right) > 0;

        public static bool operator >=(Date left, Date right) => left is null ? right is null : left.CompareTo(right) >= 0;


        public override string ToString() =>
            _dateTime.ToString();

        public string ToString(string format) =>
            _dateTime.ToString(format);

        public string ToString(IFormatProvider provider) =>
            _dateTime.ToString(provider);

        public string ToString(string format, IFormatProvider provider) =>
            _dateTime.ToString(format, provider);
    }
}