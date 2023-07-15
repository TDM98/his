using aEMR.Common.Utilities;
using DataEntities;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace aEMR.Common.Converters
{
    public class EnumConverter : IValueConverter
    {

        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType.IsAssignableFrom(typeof(Boolean)) && targetType.IsAssignableFrom(typeof(String)))
                throw new ArgumentException("EnumConverter can only convert to boolean or string.");
            if (targetType == typeof(String))
                return value.ToString();
            //return String.Compare(value.ToString(), (String)parameter, StringComparison.InvariantCultureIgnoreCase) == 0;
            //Cho phép dùng toán tử OR (|)
            string param = (string)parameter;
            if(param.IndexOf('|') < 0)
            {
                return String.Compare(value.ToString(), param, StringComparison.InvariantCultureIgnoreCase) == 0;
            }
            string[] allParams = param.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if(allParams != null && allParams.Length > 0)
            {
                bool match = false;
                foreach (string s in allParams)
                {
                    match = String.Compare(value.ToString(), s.Trim(), StringComparison.InvariantCultureIgnoreCase) == 0;
                    if(match)
                    {
                        break;
                    }
                }
                return match;
            }
            return false;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType.IsAssignableFrom(typeof(Boolean)) && targetType.IsAssignableFrom(typeof(String)))
                throw new ArgumentException("EnumConverter can only convert back value from a string or a boolean.");
            if (!targetType.IsEnum)
                throw new ArgumentException("EnumConverter can only convert value to an Enum Type.");
            if (value.GetType() == typeof(String))
            {
                return Enum.Parse(targetType, (String)value, true);
            }
            else
            {
                //We have a boolean, as for binding to a checkbox. we use parameter
                if ((Boolean)value)
                    return Enum.Parse(targetType, (String)parameter, true);
            }
            return null;

        }

        #endregion

    }
    public class ColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush color = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
            switch ((AllLookupValues.V_Color)value)
            {
                case AllLookupValues.V_Color.Normal:
                    color = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
                    return color;
                case AllLookupValues.V_Color.Pink:
                    color = new SolidColorBrush(Color.FromArgb(200, 224, 130, 228));
                    return color;

                default: return color;
            }
        }

        /// <summary>
        /// Converts a value <see cref="Visibility"/> value to a boolean value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((AllLookupValues.V_Color)value);
        }
    }
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Enum))
            {
                return null;
            }
            return Helpers.GetEnumDescription((Enum)value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class EnumFileStorageStatusToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return (FileStorageStatus)value != FileStorageStatus.OutStore;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("EnumFileStorageStatusToBooleanConverter: Not Implemented");
        }
        #endregion
    }
}