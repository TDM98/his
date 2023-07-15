using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using Service.Core.Common;

namespace DataEntities
{
    public partial class OutwardDrugClinicDeptTemplateItem : EntityBase
    {
        #region Factory Method

        public static OutwardDrugClinicDeptTemplateItem CreateOutwardDrugClinicDeptTemplateItem(Int64 outwardDrugClinicDeptTemplateItemID, long genMedProductID, decimal reqOutQuantity)
        {
            OutwardDrugClinicDeptTemplateItem outwardDrugClinicDeptTemplateItem = new OutwardDrugClinicDeptTemplateItem();
            outwardDrugClinicDeptTemplateItem.OutwardDrugClinicDeptTemplateItemID = outwardDrugClinicDeptTemplateItemID;
            outwardDrugClinicDeptTemplateItem.GenMedProductID = genMedProductID;
            outwardDrugClinicDeptTemplateItem.ReqOutQuantity = reqOutQuantity;
            return outwardDrugClinicDeptTemplateItem;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 OutwardDrugClinicDeptTemplateItemID
        {
            get
            {
                return _OutwardDrugClinicDeptTemplateItemID;
            }
            set
            {
                if (_OutwardDrugClinicDeptTemplateItemID != value)
                {
                    OnOutwardDrugClinicDeptTemplateItemIDChanging(value);
                    _OutwardDrugClinicDeptTemplateItemID = value;
                    RaisePropertyChanged("OutwardDrugClinicDeptTemplateItemID");
                    OnOutwardDrugClinicDeptTemplateItemIDChanged();
                }
            }
        }
        private Int64 _OutwardDrugClinicDeptTemplateItemID;
        partial void OnOutwardDrugClinicDeptTemplateItemIDChanging(Int64 value);
        partial void OnOutwardDrugClinicDeptTemplateItemIDChanged();



        [DataMemberAttribute()]
        public long GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                if (_GenMedProductID != value)
                {
                    OnDrugIDChanging(value);
                    ValidateProperty("GenMedProductID", value);
                    _GenMedProductID = value;
                    RaisePropertyChanged("GenMedProductID");
                    OnDrugIDChanged();
                }
            }
        }
        private long _GenMedProductID;
        partial void OnDrugIDChanging(Nullable<long> value);
        partial void OnDrugIDChanged();


        [DataMemberAttribute()]
        public decimal ReqOutQuantity
        {
            get
            {
                return _ReqOutQuantity;
            }
            set
            {
                OnReqOutQuantityChanging(value);
                _ReqOutQuantity = value;
                RaisePropertyChanged("ReqOutQuantity");
                OnReqOutQuantityChanged();
    
            }
        }
        private decimal _ReqOutQuantity;
        partial void OnReqOutQuantityChanging(decimal value);
        partial void OnReqOutQuantityChanged();


        [DataMemberAttribute()]
        public decimal ReqOutQuantity_Orig
        {
            get
            {
                return _ReqOutQuantity_Orig;
            }
            set
            {
                _ReqOutQuantity_Orig = value;
                RaisePropertyChanged("ReqOutQuantity_Orig");

            }
        }
        private decimal _ReqOutQuantity_Orig;


        [DataMemberAttribute()]
        public string ItemNote
        {
            get
            {
                return _ItemNote;
            }
            set
            {
                _ItemNote = value;
                RaisePropertyChanged("ItemNote");

            }
        }
        private string _ItemNote;

        [DataMemberAttribute()]
        public string ItemNote_Orig
        {
            get
            {
                return _ItemNote_Orig;
            }
            set
            {
                _ItemNote_Orig = value;
                RaisePropertyChanged("ItemNote_Orig");

            }
        }
        private string _ItemNote_Orig;
        #endregion

        #region Navigation Properties

        private RefGenMedProductDetails _RefGenericDrugDetail;
        [DataMemberAttribute()]
        public RefGenMedProductDetails RefGenericDrugDetail
        {
            get
            {
                return _RefGenericDrugDetail;
            }
            set
            {
                if (_RefGenericDrugDetail != value)
                {
                    _RefGenericDrugDetail = value;
                    if (_RefGenericDrugDetail != null)
                    {
                        _GenMedProductID = _RefGenericDrugDetail.GenMedProductID;
                    }
                    else
                    {
                        _GenMedProductID = 0;
                    }
                    RaisePropertyChanged("RefGenericDrugDetail");
                }
            }
        }

        private long _V_RecordState = (long)AllLookupValues.V_RecordState.ADD;
        [DataMemberAttribute()]
        public long V_RecordState
        {
            get
            {
                return _V_RecordState;
            }
            set
            {
                if (_V_RecordState != value)
                {
                    _V_RecordState = value;
                    RaisePropertyChanged("V_RecordState");
                }
            }
        }

        #endregion

    }
}
