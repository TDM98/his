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
    public class CanReturnBedAllocConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var obj = value as BedPatientAllocs;
            if(obj == null)
            {
                return  false;
            }
            return obj.BedPatientID > 0 && obj.CheckOutDate == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Chua lam");
        }

        #endregion
    }
}
