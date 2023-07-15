using System;
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
    public class StringSplitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return "";
            }
            if (parameter.ToString() == "dd")
            {
                return value.ToString().Substring(0,2);
            }
            else if (parameter.ToString() == "MM")
            {
                return value.ToString().Substring(3, 2);
            }
            else if (parameter.ToString() == "yyyy")
            {
                return value.ToString().Substring(6, 4);
            }
            else
            {
                return "";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("StringSplitConverter not implemented");
        }
    }
}
