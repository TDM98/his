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
using System.Windows.Data;
using System.Collections;
using System.Collections.ObjectModel;

namespace aEMR.Common.Converters
{
    public class CollectionEmptyVisibilityConverter : CollectionEmptyConverter
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = (bool)base.Convert(value, targetType, parameter, culture);
            Visibility retVal = Visibility.Visible;
            if (parameter != null && parameter.ToString().Trim().ToLower() == "collapsed")
            {
                retVal = Visibility.Collapsed;
            }
            if(!b)
            {
                if(retVal == Visibility.Collapsed)
                {
                    retVal = Visibility.Visible;
                }
                else
                {
                    retVal = Visibility.Collapsed;
                }
                
            }
            return retVal;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return base.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
