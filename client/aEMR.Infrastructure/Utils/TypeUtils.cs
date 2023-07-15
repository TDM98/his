using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure.Utils
{
    public static class TypeUtils
    {
        public static string GetValue(string data)
        {
            return string.IsNullOrEmpty(data) ? string.Empty : data;
        }

        public static int GetValue(int? data)
        {
            return data.HasValue ? data.Value : 0;
        }

        public static double GetValue(double? data)
        {
            return data != null && data.HasValue ? data.Value : 0;
        }
        public static Decimal GetValue(Decimal? data)
        {
            return Math.Round(data != null && data.HasValue ? data.Value : 0, 2);
        }

        public static float GetValueDivide(float? data)
        {
            return data.HasValue ? data.Value : 1;
        }

        public static int GetValueDivide(int? data)
        {
            return data.HasValue ? data.Value : 1;
        }

        public static long GetValue(long? data)
        {
            return data.HasValue ? data.Value : 0;
        }

        public static double GetValueDivide(double? data)
        {
            return data.HasValue ? data.Value : 1;
        }


        public static bool GetValue(bool? data)
        {
            return data.HasValue && data.Value;
        }

        public static bool GetValue(bool? data, bool valueIsNull)
        {
            return data.HasValue ? data.Value : valueIsNull;
        }

        public static int ParserInt(string data)
        {
            var result = 0;
            int.TryParse(data, out result);
            return result;
        }

        public static long ParserLong(string data)
        {
            var result = 0L;
            long.TryParse(data, out result);
            return result;
        }

        public static long ParserLong(object data)
        {
            var result = 0L;
            long.TryParse("" + data, out result);
            return result;
        }

        public static DateTime GetValue(DateTime? data)
        {
            var date = DateTime.Parse("01/01/1980");

            if (data.HasValue)
            {
                date = data.Value;
            }

            return date;
        }

        public static double ParseDouble(string number)
        {
            var result = 0.0;
            double.TryParse(number, NumberStyles.Any, null, out result);
            return result;
        }

        public static Decimal ParseDecimal(string number)
        {
            Decimal result = 0;
            Decimal.TryParse(number, NumberStyles.Any, null, out result);
            return result;
        }
        public static Decimal ParseDecimal(object number)
        {
            Decimal result = 0L;
            Decimal.TryParse("" + number, out result);
            return result;
        }

    }
}
