using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using DataEntities;
using eHCMSCommon.Utilities;

namespace aEMR.Common.Converters
{

    public class EnumValueToStringConverter : IValueConverter
    {

        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var enumVal = value as Enum;
            if(enumVal != null)
            {
                return Helpers.GetEnumDescription(enumVal);    
            }
            return string.Empty;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("EnumValueToStringConverter: ConvertBack not implemented");
        }

        #endregion

        //public string GetEnumDescription(Enum value)
        //{
        //    FieldInfo fi = value.GetType().GetField(value.ToString());
        //    var attributes = (DescriptionAttribute[])fi.GetCustomAttributes
        //      (typeof(DescriptionAttribute), false);
        //    return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        //}
    }
}
