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
using DataEntities;

namespace aEMR.Common.Converters
{
    public class BedAllocIconConverter : IValueConverter
    {

        //public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    bool canDelete = (bool) value;
        //    if(canDelete)
        //    {
        //        return "/eHCMSCal;component/Assets/Images/Delete.png";
        //    }
        //    else
        //    {
        //        return "/eHCMSCal;component/Assets/Images/cannotdelete.png";
        //    }
        //}

        //public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    return null;
        //}

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var bedAlloc = value as BedPatientAllocs;
            if(bedAlloc == null)
            {
                return "/eHCMSCal;component/Assets/Images/bedalloc_notavailable.png";
            }
            if(bedAlloc.IsActive)
            {
                return "/eHCMSCal;component/Assets/Images/Bed4.png";
            }
            else
            {
                return "/eHCMSCal;component/Assets/Images/Bed6.jpg";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
