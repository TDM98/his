using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public interface ITotalPrice
    {
        decimal TotalInvoicePrice
        {
            get;
            set;
        }
        decimal TotalPriceDifference
        {
            get;
            set;
        }
        decimal TotalHIPayment
        {
            get;
            set;
        }
        decimal TotalCoPayment
        {
            get;
            set;
        }
        decimal TotalPatientPayment
        {
            get;
            set;
        }
    }
}
