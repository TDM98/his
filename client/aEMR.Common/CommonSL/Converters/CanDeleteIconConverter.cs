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
    /// <summary>
    /// Lấy icon cho 1 item tùy theo biến CanDelete.
    /// Nếu 1 item có CanDelete = true thì trả về icon delete.png
    /// Nếu 1 item có CanDelete = false thì trả về icon cannotdelete.png
    /// </summary>
    public class CanDeleteIconConverter : EnumConverter
    {

        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool canDelete = (bool) value;
            if(canDelete)
            {
                return "/aEMR.CommonViews;component/Assets/Images/Delete.png";
            }
            else
            {
                return "/aEMR.CommonViews;component/Assets/Images/cannotdelete.png";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class CanEditIconConverter : EnumConverter
    {
        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool canDelete = (bool)value;
            if (canDelete)
            {
                return "/aEMR.CommonViews;component/Assets/Images/edit-icon.png";
            }
            else
            {
                return "/aEMR.CommonViews;component/Assets/Images/lock-edit-icon.png";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class CanDeleteIconConverter_V2 : EnumConverter
    {
        #region IValueConverter Members
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                AllLookupValues.ExamRegStatus mExamRegStatus = (AllLookupValues.ExamRegStatus)Enum.Parse(typeof(AllLookupValues.ExamRegStatus), value.ToString(), false);
                if (mExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM || mExamRegStatus == AllLookupValues.ExamRegStatus.KHONG_XAC_DINH || mExamRegStatus == AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI)
                {
                    return "/aEMR.CommonViews;component/Assets/Images/Delete.png";
                }
                else if (mExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
                {
                    return "/aEMR.CommonViews;component/Assets/Images/tick.png";
                }
                else
                {
                    return "/aEMR.CommonViews;component/Assets/Images/cannotdelete.png";
                }
            }
            catch
            {
                return "/aEMR.CommonViews;component/Assets/Images/cannotdelete.png";
            }
        }
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
        #endregion
    }
}