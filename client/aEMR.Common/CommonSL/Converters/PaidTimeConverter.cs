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
    /// Dùng để hiển convert PaidTime sang chuỗi.
    /// Nếu có paidtime => Đã trả tiền
    /// Nếu chưa có => Chưa trả tiền
    /// </summary>
    public class PaidTimeConverter : EnumConverter
    {

        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MedRegItemBase item = value as MedRegItemBase;
            if(item == null)
            {
                return "";
            }
            if(item.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM
                || item.ExamRegStatus == AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN
                || item.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
            {
                if (item.PaidTime == null)
                {
                    return "CTT";// "Chưa trả tiền";
                }
                return string.Format("ĐTT");//"Đã trả tiền");
            }
            else if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            {
                if (item.RefundTime == null)
                {
                    return "CHT";// "Chưa hoàn tiền";
                }
                return string.Format("ĐHT");//"Đã hoàn tiền");
            }
            return "";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
