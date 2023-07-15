using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using aEMR.Common.Utilities;
using DataEntities;
/*
 * 20181219 #001: BM 0005443: Tạo mới converter phục vụ màn hình lập phiếu lĩnh nhà thuốc.
 */
namespace aEMR.Common.Converters
{

    public class EnumValueToStringConverter : IValueConverter
    {

        #region IValueConverter Members

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var enumVal = value as Enum;
            if (enumVal != null)
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
    }

    public class PtPCLReqObjToStrConverter : IValueConverter
    {
        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PatientPCLRequest pclReq = value as PatientPCLRequest;
            if (pclReq != null)
            {
                return string.Format("PCL Request {0}", pclReq.PCLRequestNumID);
            }
            return string.Empty;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
    //▼====== #001
    public class ReqDrugHIStore_SummaryLine_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string theParam = parameter as string;
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var itemsInRow = (ReadOnlyObservableCollection<Object>)value;
                Decimal sumQty = 0;
                foreach (RequestDrugInwardForHiStoreDetails theItem in itemsInRow)
                {
                    if (theParam == "ReqQty")
                    {
                        sumQty += theItem.ReqQty;
                    }
                    else if (theParam == "ApprovedQty")
                    {
                        sumQty += theItem.ApprovedQty;
                    }
                    else if (theParam == "RemainingQty")
                    {
                        sumQty = theItem.RemainingQty;
                        break;
                    }

                }
                return sumQty.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
    //▲====== #001
    public class ReqDrugClinicDept_SummaryLine_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string theParam = parameter as string;
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var itemsInRow = (ReadOnlyObservableCollection<Object>)value;
                Decimal sumQty = 0;
                foreach (ReqOutwardDrugClinicDeptPatient theItem in itemsInRow)
                {
                    if (theParam == "ReqQty")
                    {
                        sumQty += theItem.ReqQty;
                    }
                    else if (theParam == "ApprovedQty")
                    {
                        sumQty += theItem.ApprovedQty;
                    }
                    else if (theParam == "RemainingQty")
                    {
                        // TxD 10/07/2018 For the Remaining Qty JUST get the Value on 1 Row is enough (because it's repeated for all rows in the SAME Group) then return
                        sumQty = theItem.RemainingQty;
                        break;
                    }
                    
                }
                return sumQty.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class ReqDrugClinicDept_ReqQty_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var items = (ReadOnlyObservableCollection<Object>)value;
                Decimal total = 0;
                foreach (ReqOutwardDrugClinicDeptPatient gi in items)
                {
                    total += gi.ReqQty;
                }
                return total.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class ReqDrugClinicDept_RemainingQty_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ReadOnlyObservableCollection<Object>)
            {
                var items = (ReadOnlyObservableCollection<Object>)value;
                Decimal remQty = 0;
                foreach (ReqOutwardDrugClinicDeptPatient gi in items)
                {
                    remQty = gi.RemainingQty;
                    break;
                }
                return remQty.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

}
