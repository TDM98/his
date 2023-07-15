using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure.Utils
{
    public static class InvoiceNoFormatUtils
    {

        public static string BuildInvoiceNo(long invoicepk)
        {
            var value = string.Empty;

            value = invoicepk.ToString("D6");

            return value;
        }

    }
}
