
using eHCMSLanguage;
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
    public class ExamRegStatusConverter : CheckIfNullConverter
    {
        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AllLookupValues.ExamRegStatus status = (AllLookupValues.ExamRegStatus) value;
            switch (status)
            {
                    //case AllLookupValues.ExamRegStatus.DANG_KY_KHAM:
                    //return "Đăng ký khám";

                    //case AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN:
                    //return "Đang khám";


                    //case AllLookupValues.ExamRegStatus.HOAN_TAT:
                    //return "Thực hiện xong";

                    //case AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI:
                    //return "Hủy - Trả tiền lại";

                    //default:
                    //return "Chưa xác định";
                case AllLookupValues.ExamRegStatus.DANG_KY_KHAM:
                    return "ĐKK";

                case AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN:
                    return eHCMSResources.R0304_G1_K;


                case AllLookupValues.ExamRegStatus.HOAN_TAT:
                    return "HT";

                case AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI:
                    return "H-TTL";

                case AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI:
                    return "X-TTL";

                default:
                    return "CXĐ";
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("Chua code");
        }

        #endregion
    }
    public class OutDrugInvStatusConverter : CheckIfNullConverter
    {
        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AllLookupValues.V_OutDrugInvStatus status = (AllLookupValues.V_OutDrugInvStatus)value;
            switch (status)
            {
                case AllLookupValues.V_OutDrugInvStatus.SAVE:
                    return "Đã lưu";

                case AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED:
                    return eHCMSResources.K2810_G1_DaLayThuoc;


                case AllLookupValues.V_OutDrugInvStatus.PAID:
                    return "Đã trả tiền";

                case AllLookupValues.V_OutDrugInvStatus.REFUNDED:
                    return "Đã hoàn tiền";

                case AllLookupValues.V_OutDrugInvStatus.RETURN:
                    return eHCMSResources.G1666_G1_TraThuoc;

                case AllLookupValues.V_OutDrugInvStatus.CANCELED:
                    return "Hủy - Trả tiền lại";

                default:
                    return eHCMSResources.Z1116_G1_ChuaXacDinh;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("Chua code");
        }

        #endregion
    }
}
