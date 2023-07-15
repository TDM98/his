using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using aEMR.Infrastructure.Utils;

namespace aEMR.Infrastructure.Converters
{
    public class InvoicenoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? InvoiceNoFormatUtils.BuildInvoiceNo(0) : InvoiceNoFormatUtils.BuildInvoiceNo(TypeUtils.ParserLong(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
