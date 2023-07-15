using System;
using System.Globalization;
using System.Windows.Data;
namespace aEMR.Common.Converters
{
    public class DecimalConverter : IValueConverter
    {
        #region IValueConverter Members
        public bool IsVND = true;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            try
            {
                if (value == null)
                {
                    retVal = "0";
                }
                else
                {

                    decimal cost = System.Convert.ToDecimal(value);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost == 0)
                    {
                        retVal = "0";
                    }
                    else
                    {
                        if (IsVND)
                        {
                            retVal = cost.ToString("#,0.##", currentCulture);
                        }
                        else
                        {
                            retVal = cost.ToString("#,0.####", currentCulture);
                        }
                    }
                }
                return retVal;
            }
            catch
            {
                return "0";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {
                retVal = System.Convert.ToDecimal(value);
            }
            catch
            {
            }
            return retVal;
        }
        #endregion
    }
    public class DecimalConverterMin1 : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            try
            {
                if (value == null)
                {
                    retVal = "1";
                }
                else
                {
                    decimal cost = System.Convert.ToDecimal(value);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost == 0)
                    {
                        retVal = "1";
                    }
                    else
                    {
                        retVal = cost.ToString("#,0.##", currentCulture);
                    }
                }
                return retVal;
            }
            catch
            {
                return "1";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {
                retVal = System.Convert.ToDecimal(value);
            }
            catch
            {
            }
            return retVal;
        }
        #endregion
    }
    public class NullableDecimalConverter : IValueConverter
    {
        #region IValueConverter Members
        public bool IsVND = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = null;
            try
            {
                if (value == null)
                {
                    return retVal;
                }

                decimal cost = System.Convert.ToDecimal(value);
                CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                if (cost == 0)
                {
                    retVal = "0";
                }
                else
                {
                    if (IsVND)
                    {
                        retVal = cost.ToString("#,0.##", currentCulture);
                    }
                    else
                    {
                        retVal = cost.ToString("#,0.####", currentCulture);
                    }
                }
                return retVal;
            }
            catch
            {
                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {

                retVal = System.Convert.ToDecimal(value);
            }
            catch
            {
            }
            return retVal;
        }

        #endregion
    }
    public class NullableDecimalConverter2 : IValueConverter
    {
        public bool IsVND = true;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = null;
            try
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    return retVal;
                }
                decimal cost = System.Convert.ToDecimal(value);
                CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                if (cost == 0)
                {
                    retVal = "0";
                }
                else
                {
                    if (IsVND)
                    {
                        retVal = cost.ToString("#,0.##", currentCulture);
                    }
                    else
                    {
                        retVal = cost.ToString("#,0.####", currentCulture);
                    }
                }
                return retVal;
            }
            catch
            {
                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal? retVal = null;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {
                retVal = System.Convert.ToDecimal(value);
            }
            catch
            {
            }
            return retVal;
        }
    }
    public class IntConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            try
            {
                if (value == null)
                {
                    retVal = "0";
                }
                else
                {

                    decimal cost = Math.Round(System.Convert.ToDecimal(value), 0);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost == 0)
                    {
                        retVal = "0";
                    }
                    else
                    {
                        //retVal = cost.ToString("#,###.#", culture);
                        retVal = cost.ToString("#,0.####", currentCulture);
                    }
                }
                return retVal;
            }
            catch
            {
                return "0";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {

                retVal = Math.Round(System.Convert.ToDecimal(value), 0);
            }
            catch
            {
            }
            return retVal;
        }

        #endregion
    }
    public class ByteConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            if (value is byte?)
            {
                byte? byteValue = (byte?)value;
                if (byteValue.HasValue)
                {
                    return byteValue.Value.ToString();
                }
            }

            return 0;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            if (value is string)
            {
                byte number;
                if (Byte.TryParse((string)value, out number))
                {
                    return number;
                }
            }

            return null;
        }
    }
    public class DecimalConverter0 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            try
            {
                if (value == null)
                {
                    retVal = "0";
                }
                else
                {
                    decimal cost = Math.Round(System.Convert.ToDecimal(value), 0);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost == 0)
                    {
                        retVal = "0";
                    }
                    else
                    {
                        retVal = cost.ToString("#,0", currentCulture);
                    }
                }
                return retVal;
            }
            catch
            {
                return "0";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {

                retVal = System.Convert.ToDecimal(value);
            }
            catch
            {
            }
            return retVal;
        }
    }
    public class DecimalConverter2 : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            try
            {
                if (value == null)
                {
                    retVal = "0";
                }
                else
                {

                    decimal cost = Math.Round(System.Convert.ToDecimal(value), 2);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost == 0)
                    {
                        retVal = "0";
                    }
                    else
                    {
                        retVal = cost.ToString("#,0.##", currentCulture);
                    }
                }
                return retVal;
            }
            catch
            {
                return "0";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {

                retVal = System.Convert.ToDecimal(value);
            }
            catch
            {
            }
            return retVal;
        }

        #endregion
    }
    public class DecimalConverter3 : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            try
            {
                if (value == null)
                {
                    retVal = "0";
                }
                else
                {
                    //KMx: Phải làm tròn lên 3 số sau dấu thập phân, nếu không sẽ bị lỗi 0.125, làm tròn thành 0.13 (30/12/2014 16:16.)
                    //decimal cost = Math.Round(System.Convert.ToDecimal(value), 2);
                    decimal cost = Math.Round(System.Convert.ToDecimal(value), 3);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost == 0)
                    {
                        retVal = "0";
                    }
                    else
                    {
                        retVal = cost.ToString("#,0.###", currentCulture);
                    }
                }
                return retVal;
            }
            catch
            {
                return "0";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {

                retVal = System.Convert.ToDecimal(value);
            }
            catch
            {
            }
            return retVal;
        }

        #endregion
    }
    public class DecimalConverter4 : IValueConverter
    {
        #region IValueConverter Members
        public bool IsVND = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            try
            {
                if (value == null)
                {
                    retVal = "0";
                }
                else
                {

                    decimal cost = System.Convert.ToDecimal(value);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost == 0)
                    {
                        retVal = "0";
                    }
                    else
                    {
                        retVal = cost.ToString("#,0.####", currentCulture);
                    }
                }
                return retVal;
            }
            catch
            {
                return "0";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {

                retVal = System.Convert.ToDecimal(value);
            }
            catch
            {
            }
            return retVal;
        }

        #endregion
    }
    public class DecimalConverterRound : IValueConverter
    {
        #region IValueConverter Members

        public bool IsVND = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            try
            {
                if (value == null)
                {
                    retVal = "0";
                }
                else
                {

                    decimal cost = System.Convert.ToDecimal(value);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost == 0)
                    {
                        retVal = "0";
                    }
                    else
                    {
                        if (IsVND)
                        {
                            //retVal = Math.Round(cost, 0).ToString("#,0", currentCulture);
                            retVal = Math.Round(cost, 0, System.MidpointRounding.AwayFromZero).ToString("#,0", currentCulture); //20210922 QTD Fix làm tròn với giá trị 0.5
                        }
                        else
                        {
                            retVal = cost.ToString("#,0.####", currentCulture);
                        }
                    }
                }
                return retVal;
            }
            catch
            {
                return "0";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {

                retVal = System.Convert.ToDecimal(value);
            }
            catch
            {
            }
            return retVal;
        }

        #endregion
    }
    public class DecimalCurrencyConverter : IValueConverter
    {
        #region IValueConverter Members
        public bool IsVND = true;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retVal = string.Empty;
            try
            {
                if (value == null)
                {
                    retVal = "0";
                }
                else
                {
                    decimal cost = System.Convert.ToDecimal(value);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost == 0)
                    {
                        retVal = "0";
                    }
                    else
                    {
                        if (IsVND)
                        {
                            retVal = cost.ToString("#,0.##", currentCulture);
                        }
                        else
                        {
                            retVal = cost.ToString("#,0.####", currentCulture);
                        }
                    }
                }
                return retVal;
            }
            catch
            {
                return "0";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal retVal = 0m;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {
                retVal = Math.Round(System.Convert.ToDecimal(value), IsVND ? 2 : 4);
            }
            catch
            {
            }
            return retVal;
        }
        #endregion
    }
    public class DecimalToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is decimal))
            {
                return 0;
            }
            return Math.Round(System.Convert.ToDecimal(value) * 100, 2).ToString("#");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string))
            {
                return 0;
            }
            try
            {
                decimal mPercentValue = System.Convert.ToDecimal(value);
                if (mPercentValue < 0 || mPercentValue > 100)
                {
                    return 0;
                }
                return Math.Round(mPercentValue / 100, 2);
            }
            catch
            {
                return 0;
            }
        }
    }
}