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
    public class StaticResourceBoolConverter : StyleConverter
    {
        public StaticResourceBoolConverter():base()
        {

        }
        #region IValueConverter Members

        /// <summary>
        /// Du lieu truyen vao co dang:
        /// PropertyName(true or false) StaticResourceName1 : StaticResourceName2
        /// (neu true => chon StaticResourceName1 
        /// neu false => StaticResourceName2)
        /// (Giong toan tu ba ngoi.)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = (bool)value;
            string trueResName = null;
            string falseResName = null;

            string inputParam = parameter as string;
            if(inputParam != null)
            {
                string[] arr = inputParam.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if(arr != null && arr.Length >=2 )
                {
                    trueResName = arr[0];
                    falseResName = arr[1];
                }
            }

            string selectedStyleName = null;

            if (StaticResource != null)
            {
                selectedStyleName = (b? trueResName: falseResName);
                if(!string.IsNullOrWhiteSpace(selectedStyleName))
                {
                    return StaticResource[selectedStyleName] as Style;
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("StaticResourceBoolConverter not implemented");
        }

        #endregion
    }
}
