using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using Service.Core.Common;

namespace DataEntities
{
    public partial class PharmacyEstimationForPODetailExt : EntityBase
    {
        [DataMemberAttribute()]
        public Int64 PharmacyEstimatePoDetailID
        {
            get
            {
                return _PharmacyEstimatePoDetailID;
            }
            set
            {
                if (_PharmacyEstimatePoDetailID != value)
                {
                    OnPharmacyEstimatePoDetailIDChanging(value);
                    _PharmacyEstimatePoDetailID = value;
                    RaisePropertyChanged("PharmacyEstimatePoDetailID");
                    OnPharmacyEstimatePoDetailIDChanged();
                }
            }
        }
        private Int64 _PharmacyEstimatePoDetailID;
        partial void OnPharmacyEstimatePoDetailIDChanging(Int64 value);
        partial void OnPharmacyEstimatePoDetailIDChanged();

        private EntityState _EntityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }

        [DataMemberAttribute()]
        public PharmacyEstimationForPODetail PharmacyEstimationForPODetail
        {
            get
            {
                return _PharmacyEstimationForPODetail;
            }
            set
            {
                _PharmacyEstimationForPODetail = value;
                RaisePropertyChanged("PharmacyEstimationForPODetail");
            }
        }
        private PharmacyEstimationForPODetail _PharmacyEstimationForPODetail;

        [DataMemberAttribute()]
        public string DrugCode
        {
            get
            {
                return _DrugCode;
            }
            set
            {
                if (_DrugCode != value)
                {
                    _DrugCode = value;
                    RaisePropertyChanged("DrugCode");
                }
            }
        }
        private string _DrugCode;

        //////suy nghi sua lai nhu vay hay khong?
        //[DataMemberAttribute()]
        //private RefGenericDrugDetail _RefGenMedProductDetails;
        //public RefGenericDrugDetail RefGenMedProductDetails
        //{
        //    get
        //    {
        //        return _RefGenMedProductDetails;
        //    }
        //    set
        //    {
        //        _RefGenMedProductDetails = value;
        //        RaisePropertyChanged("RefGenMedProductDetails");
        //    }
        //}
    }
}
