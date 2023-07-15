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
using System.Windows.Media.Imaging;
using System.IO;

namespace aEMR.Common.Converters
{
    public class ImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage bmp = new BitmapImage(new Uri("..Images/Icon/word_icon.jpg", UriKind.RelativeOrAbsolute));
             return bmp;


            //BitmapImage Imagebitmage =null; 
            //if (value == null)
            //{
            //    Imagebitmage = new BitmapImage(new Uri(Application.Current.Host.Source, "../Images/Icon/word_icon.jpg"));
            //    return Imagebitmage;
            //}
            //else
            //{
            //    //string val = value.ToString();
            //    //if (val == ".bmp" || val == ".jpeg" || val == ".jpg" || val == ".jpe" || val == ".jfif" || val == ".gif" || val == ".png")
            //    //{
            //    //    return "/ApplicationStrings;component/Images/Icon/BlueFolderXP.jpg";
            //    //}
            //    //else if (val == ".txt")
            //    //{
            //    //    return "";
            //    //}
            //    //else if (val == ".pdf")
            //    //{
            //    //    return "";
            //    //}
            //    //else if (val == ".doc" || val == ".docx")
            //    //{
            //    //    return "/ApplicationStrings;component/Images/Icon/word_icon.jpg";
            //    //}
            //    //else if (val == ".xls" || val == ".xlsx")
            //    //{
            //    //    return "";
            //    //}
            //    //else if (val == ".rar" || val == ".zip")
            //    //{
            //    //    return "";
            //    //}
            //    //else if (val == ".csv")
            //    //{
            //    //    return "";
            //    //}
            //    //else if (val == ".avi" || val == ".mp4" || val == ".mov" || val == ".wmv")
            //    //{
            //    //    return "";
            //    //}
            //    //else
            //    {
            //        Imagebitmage = new BitmapImage(new Uri(Application.Current.Host.Source, "../Images/Icon/word_icon.jpg"));
            //        return Imagebitmage;// "/ApplicationStrings;component/Images/Icon/word_icon.jpg";
            //    }
            //}
           

        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new BitmapImage(new Uri("../Images/Icon/word_icon.jpg", UriKind.Relative));
          
        }

        #endregion
    }
}
