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
    public class PaperReferalToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var referal = value as PaperReferal;
            if (referal == null || referal.Hospital == null)
            {
                return "";
            }
            return string.Format("({0}){1}", referal.Hospital.HICode, referal.Hospital.HosName);
        }
        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("PaperReferalToStringConverter: Method Not Implemented");
        }

        #endregion
    }
}
