using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public interface IChargeableItemPrice
    {
        decimal NormalPrice
        {
            get;
            set;
        }
        decimal HIPatientPrice
        {
            get;
            set;
        }
        decimal? HIAllowedPrice
        {
            get;set;
        }
        ChargeableItemType ChargeableItemType
        {
            get; set;
        }

        //gia ngay tai thoi diem xem lai,neu co chinh sua thi lay lai gia nay
        decimal NormalPriceNew
        {
            get;
            set;
        }
        decimal HIPatientPriceNew
        {
            get;
            set;
        }
        decimal? HIAllowedPriceNew
        {
            get;
            set;
        }
    }
}
