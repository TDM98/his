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
    public class DrugInvoiceNameConverter : IValueConverter
    {

        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var inv = value as OutwardDrugInvoice;
            if (inv == null)
            {
                return "";
            }
            if(inv.ReturnID.GetValueOrDefault(-1) > 0)
            {
                return string.Format("Phiếu trả {0}", inv.OutInvID);
            }
            else
            {
                return string.Format("Phiếu xuất {0}", inv.OutInvID);
            }
        }

        #endregion


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool visible = false;
            if (value != null)
            {
                visible = (bool)value;
            }
            return visible ? eHCMSResources.K2799_G1_DaHuy : "";
        }

        #endregion


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }

    public class ReportConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool visible = false;
            if (value != null)
            {
                visible = (bool)value;
            }
            return visible ? eHCMSResources.K2780_G1_DaBC : eHCMSResources.K2239_G1_ChuaBC;
        }

        #endregion


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }

    public class PrintConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool visible = false;
            if (value != null)
            {
                visible = (bool)value;
            }
            return visible ? "" : "In";
        }

        #endregion


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }

    public class PaymentStatusConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value.ToString().Trim() == "50002")
                {
                    return eHCMSResources.N0181_G1_NhaThuoc;
                }
            }
            return "";
        }

        #endregion


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }

    public class ReportTypeConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool visible = false;
            if (value != null)
            {
                visible = (bool)value;
            }
            return visible ? "Phiếu Hủy" : eHCMSResources.R0511_G1_phThu;
        }

        #endregion


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }
}
