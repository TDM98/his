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
using eHCMSLanguage;

namespace aEMR.Common.Converters
{
    public class FormularPriceConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "%";
            else
            {
                bool val = (bool)value;
                if (val)
                {
                    return "%";
                }
                   
                else
                {
                    return eHCMSResources.G1616_G1_VND.ToUpper();
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "%";
            else
            {
                bool val = (bool)value;
                if (val)
                {
                    return "%";
                }

                else
                {
                    return eHCMSResources.G1616_G1_VND.ToUpper();
                }
            }
        }

        #endregion
    }
}
