using System;
using System.Linq;
using System.Windows.Data;
using DataEntities;


namespace aEMR.Common.Converters
{
    public enum MidpointRounding
    {
        ToEven,
        AwayFromZero
    }

    public static class MathExt
    {
        public static decimal Round(decimal d, MidpointRounding mode)
        {
            return MathExt.Round(d, 0, mode);
        }

        //MidpointRounding = ToEven : 1.x nếu x <= .5 thì làm tròn xuống. Nhưng có 1 số trường hợp .5 lại làm tròn lên, không nên sử dụng cái này.
        //MidpointRounding = AwayFromZero: 1.x nếu x >= .5 thì làm tròn lên. Đã test và sử dụng được (22/08/2014 10:04).
        public static decimal Round(decimal d, int decimals, MidpointRounding mode)
        {
            if (mode == MidpointRounding.ToEven)
            {
                return decimal.Round(d, decimals);
            }
            else
            {
                decimal factor = Convert.ToDecimal(Math.Pow(10, decimals));
                int sign = Math.Sign(d);
                return Decimal.Truncate(d * factor + 0.5m * sign) / factor;
            }
        }
    }
}

namespace aEMR.Common.Converters
{
    public class InvoiceItemViewGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            CollectionViewGroup cvg = value as CollectionViewGroup;
            string param = parameter as string;
            switch (param)
            {
                case "Qty":
                    return new DecimalConverter().Convert(cvg.Items.Sum(x => (x as IInvoiceItem).Qty),null,null,null);

                case "InvoicePrice":
                    return new DecimalConverter().Convert(cvg.Items.Where(AvailableItems).Sum(x => (x as IInvoiceItem).TotalInvoicePrice),null,null,null);

                case "PriceDifference":
                    return new DecimalConverter().Convert(cvg.Items.Where(AvailableItems).Sum(x => (x as IInvoiceItem).TotalPriceDifference),null,null,null);

                case "HIPayment":
                    //return new DecimalConverter().Convert(cvg.Items.Where(AvailableItems).Sum(x => (x as IInvoiceItem).TotalHIPayment),null,null,null);
                    return new DecimalConverter().Convert(MathExt.Round(cvg.Items.Where(AvailableItems).Sum(x => (x as IInvoiceItem).TotalHIPayment), MidpointRounding.AwayFromZero), null, null, null);

                case "PatientCoPayment":
                    return new DecimalConverter().Convert(cvg.Items.Where(AvailableItems).Sum(x => (x as IInvoiceItem).TotalCoPayment),null,null,null);

                case "PatientPayment":
                    //return new DecimalConverter().Convert(cvg.Items.Where(AvailableItems).Sum(x => (x as IInvoiceItem).TotalPatientPayment),null,null,null) ;
                    //KMx: Bệnh nhân trả phải = Round(InvoicePrice) - Round(HIPayment). Nếu không sẽ ra số lẻ, không đồng bộ với nhà thuốc (07/08/2014 15:53).
                    decimal InvoicePrice = MathExt.Round(cvg.Items.Where(AvailableItems).Sum(x => (x as IInvoiceItem).TotalInvoicePrice), MidpointRounding.AwayFromZero);
                    decimal HIPayment = MathExt.Round(cvg.Items.Where(AvailableItems).Sum(x => (x as IInvoiceItem).TotalHIPayment), MidpointRounding.AwayFromZero);

                    return new DecimalConverter().Convert(InvoicePrice - HIPayment, null, null, null);

                case "HIAllowedPrice":
                    return new DecimalConverter().Convert(cvg.Items.Where(AvailableItems).Sum(x => { return (x as IInvoiceItem).TotalHIPayment + (x as IInvoiceItem).TotalCoPayment; }),null,null,null);
                default:
                    return 0;
            }
        }

        private bool AvailableItems(object obj)
        {
            IInvoiceItem inv = obj as IInvoiceItem;
            if (inv != null)
            {
                return inv.RecordState != RecordState.DELETED && inv.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;  
            }
            return false;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PaymentViewGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            CollectionViewGroup cvg = value as CollectionViewGroup;
            string param = parameter as string;
            switch (param)
            {
                case "PatientAmount":
                    return new DecimalConverter().Convert(cvg.Items.Sum(x => (x as ReportOutPatientCashReceipt_Payments).CurReportOutPatientCashReceipt.PatientAmount), null, null, null);
               
                default:
                    return 0;
            }
        }

     
        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


      
    }

}
