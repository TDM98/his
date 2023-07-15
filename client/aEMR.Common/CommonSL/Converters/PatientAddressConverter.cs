using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using DataEntities;


namespace aEMR.Common.Converters
{
    public class PatientAddressConverter : IValueConverter
    {
        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Patient p = value as Patient;
            if (p == null)
            {
                return String.Empty;
            }
            string cityProvinceName = string.Empty;
            if (p.CitiesProvince != null)
            {
                cityProvinceName = p.CitiesProvince.CityProvinceName;
            }
            ConcatStringWithDelimiterConverter converter = new ConcatStringWithDelimiterConverter();
            return converter.Convert(new string[] { p.PatientStreetAddress, p.PatientSurburb, cityProvinceName , p.PatientNotes}, targetType, parameter, culture);
        }
        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("PatientAddressConverter: Method Not Implemented");
        }

        #endregion
    }
}
