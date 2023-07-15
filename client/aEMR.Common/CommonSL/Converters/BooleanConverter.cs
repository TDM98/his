using DataEntities;
using System;
using System.Windows;
using System.Windows.Data;
namespace aEMR.Common.Converters
{
    public class IntToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (int.TryParse(value.ToString(), out int d))
                {
                    return d > 0;
                }
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("IntToBooleanConverter: Not Implemented");
        }
        #endregion
    }
    public class LongToBooleanConverter : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long d;
            if (value != null)
            {
                if (long.TryParse(value.ToString(), out d))
                {
                    return d > 0;
                }
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("LongToBooleanThanConverter: Not Implemented");
        }
        #endregion
    }
    public class LongToNotBooleanConverter : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long d;
            if (value != null)
            {
                if (long.TryParse(value.ToString(), out d))
                {
                    return !(d > 0);
                }
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("LongToNotBooleanConverter: Not Implemented");
        }
        #endregion
    }
    public class LongToNotBooleanConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long CurrentLongValue;
            if (value != null)
            {
                if (long.TryParse(value.ToString(), out CurrentLongValue))
                {
                    return (CurrentLongValue <= 0);
                }
            }
            return true;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("LongToNotBooleanConverter: Not Implemented");
        }
    }
    public class EnumBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }
        #endregion
    }
    public class ConditionToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null && value != null && parameter.ToString().Equals(value.ToString()))
                return true;
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    public class V_ReportStatusPrescriptionToBooleanConverter : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long d;
            if (value != null)
            {
                if (long.TryParse(value.ToString(), out d))
                {
                    return d == (long)AllLookupValues.V_ReportStatusPrescription.NotReported;
                }
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("LongToBooleanThanConverter: Not Implemented");
        }
        #endregion
    }
    public class V_ReportStatusPrescriptionToBooleanConverter2 : IValueConverter
    {

        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long d;
            if (value != null)
            {
                if (long.TryParse(value.ToString(), out d))
                {
                    return d == (long)AllLookupValues.V_ReportStatusPrescription.Reported;
                }
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("LongToBooleanThanConverter: Not Implemented");
        }
        #endregion
    }
}
