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
using DataEntities;

namespace aEMR.Common.Converters
{
    public class OutstandingItemFontWeightConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            OutstandingItemState state = (OutstandingItemState)value;

            if (state == OutstandingItemState.IsNew)
            {
                return "SemiBold";
            }
            else if (state == OutstandingItemState.IsSelected)
            {
                return "Bold";
            }
            return "Normal";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
