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
using System.Text;

namespace aEMR.Common.Converters
{
    public class StringSplitCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //quy dinh: paramater 2 tro la code 2, 3 :la code 3
            if (value == null || parameter == null)
            {
                return "";
            }
            if (parameter.ToString().Length < 2)
            {
                return "";
            }
            return Function(parameter,value);

        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("StringSplitConverter not implemented");
        }

        public string Function(object para, object value)
        {
            int temp = System.Convert.ToInt16(para.ToString().Substring(0, 1));
            int pos= System.Convert.ToInt16(para.ToString().Substring(1, 1));
            if (temp > value.ToString().Length)
            {
                if (pos < temp - value.ToString().Length)
                {
                    return "0";
                }
                else
                {
                    return value.ToString().Substring(value.ToString().Length - temp + pos,1);
                }
            }
            else
            {
                return value.ToString().Substring(pos,1);
            }
        }
    }
}
