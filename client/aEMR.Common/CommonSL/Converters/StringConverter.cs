using eHCMSLanguage;
using System;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace aEMR.Common.Converters
{
    public class StringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                if (value.ToString() == "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                if (value.ToString() == "")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        #endregion
    }
    public class ResourceStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || string.IsNullOrEmpty(parameter.ToString()))
            {
                return null;
            }
            try
            {
                ResourceManager mResourceManager = new ResourceManager("eHCMSLanguage.eHCMSResources", typeof(eHCMSResources).Assembly);
                string ParameterString = mResourceManager.GetString(parameter.ToString());
                if (value.GetType().Equals(typeof(decimal)))
                {
                    return string.Format(ParameterString, System.Convert.ToDecimal(value).ToString("#,0.##"));
                }
                return string.Format(ParameterString, value);
            }
            catch
            {
                return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GenderStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            if (value.ToString() == "F")
            {
                return "Nữ";
            }
            else if (value.ToString() == "M")
            {
                return "Nam";
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                if (value.ToString() == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                if (value.ToString() == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion
    }
}