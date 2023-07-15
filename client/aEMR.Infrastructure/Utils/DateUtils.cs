using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure.Utils
{
    public static class DateUtils
    {
        public static DateTime StartOfDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        public static DateTime NullDate()
        {
            return new DateTime(1980, 01, 01);
        }

        public static DateTime StartOfDate(DateTime? date)
        {
            if (date.HasValue)
            {
                return StartOfDate(date.Value);
            }

            return date.Value;
        }

        public static DateTime EndOfDate(DateTime? date)
        {
            if (date.HasValue)
            {
                return EndOfDate(date.Value);
            }

            return date.Value;
        }

        public static DateTime EndOfDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        public static bool IsBetweenDay(DateTime date, DateTime startDate, DateTime endDate)
        {
            return (date.TimeOfDay > StartOfDate(startDate).TimeOfDay && date.TimeOfDay < EndOfDate(endDate).TimeOfDay);
        }
    }
}
