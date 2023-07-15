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
    public class CollectionEmptyConverter : IValueConverter
    {

        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
            {
                return true;
            }
            return ((ICollection)value).Count == 0;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("CollectionEmptyConverter: Not Implemented.");
        }

        #endregion
    }
}
