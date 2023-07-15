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
    public class IntConverterMin1 : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
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

                    int cost = System.Convert.ToInt32(value);
                    CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (cost <= 0)
                    {
                        retVal = "1";
                    }
                    else
                    {
                        //retVal = cost.ToString("#,###.#", culture);
                        retVal = cost.ToString("#,###.###", currentCulture);
                    }
                }
                return retVal;
            }
            catch
            {
                return "1";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int retVal = 1;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return retVal;
            }
            try
            {
                retVal = System.Convert.ToInt32(value);
            }
            catch
            {
            }
            return retVal;
        }

        #endregion
    }
}
