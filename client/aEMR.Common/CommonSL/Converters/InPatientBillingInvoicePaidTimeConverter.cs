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
    /// <summary>
    /// Dùng để hiển convert PaidTime sang chuỗi.
    /// Nếu có paidtime => Đã trả tiền
    /// Nếu chưa có => Chưa trả tiền
    /// Thêm trạng thái của billing invoice (hủy, đã lưu...)
    /// </summary>
    public class InPatientBillingInvoicePaidTimeConverter : EnumConverter
    {
        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            InPatientBillingInvoice item = value as InPatientBillingInvoice;
            if (item == null)
            {
                return "";
            }


            if (item.TotalPatientPayment > item.TotalPatientPaid + item.TotalSupportFund)
            {
                if (item.TotalPatientPaid + item.TotalSupportFund <= 0)
                {
                    return eHCMSResources.Z1566_G1_ChuaDongTien;
                }
                else
                {
                    return eHCMSResources.Z1567_G1_ChuaDongDuTien;
                }

            }
            else
            {
                // TxD 05/12/2014 Added the following status for Finalization
                if (item.PaidTime != null)
                {
                    return eHCMSResources.Z1568_G1_DaQToan;
                }

                return eHCMSResources.Z1569_G1_DaDongDuTien;
            }
        }

        //public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{

        //    InPatientBillingInvoice item = value as InPatientBillingInvoice;
        //    if(item == null)
        //    {
        //        return "";
        //    }
        //    string strStatus = "";
        //    string strPaymentStatus = "";
        //    if(item.V_InPatientBillingInvStatus == AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI)
        //    {
        //        strStatus = eHCMSResources.T1723_G1_Huy;
        //    }
        //    else
        //    {
        //        if (item.InPatientBillingInvID > 0)
        //        {
        //            strStatus = "Đã lưu"; 
        //        }
        //        else
        //        {
        //            strStatus = eHCMSResources.Z0013_G1_Moi;
        //        }
        //    }
        //    if(item.V_InPatientBillingInvStatus == AllLookupValues.V_InPatientBillingInvStatus.NEW)
        //    {
        //        if (item.PaidTime == null)
        //        {
        //            strPaymentStatus = "Chưa trả tiền";
        //        }
        //        else
        //        {
        //            strPaymentStatus = "Đã trả tiền";   
        //        }
        //    }
        //    else if (item.V_InPatientBillingInvStatus == AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI)
        //    {
        //        if (item.RefundTime == null)
        //        {
        //            strPaymentStatus = "Chưa hoàn tiền";
        //        }
        //        else
        //        {
        //            strPaymentStatus = "Đã hoàn tiền";    
        //        }
        //    }
        //    return string.Join(" - ", strStatus, strPaymentStatus);
        //}

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
