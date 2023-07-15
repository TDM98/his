using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;

namespace aEMR.Common.Converters
{
    public class ShortDateConverter : IValueConverter
    {
        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            return ((DateTime)value).ToShortDateString();
        }
        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return null;
            }

            try
            {
                return DateTime.Parse(value.ToString());
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError(ex.ToString());
                throw new Exception("Ngày tháng không hợp lệ");
            }
            
        }

        #endregion
    }

    public class StringToDateConverter : IValueConverter
    {
        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            return ((DateTime)value).ToString("dd/MM/yyyy");
        }
        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return null;
            }
            CultureInfo provider = new CultureInfo("vi-VN");
            try
            {
                return DateTime.Parse(value.ToString(), provider);
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError(ex.ToString());
                return null;
            }
        }

        #endregion
    }
}
