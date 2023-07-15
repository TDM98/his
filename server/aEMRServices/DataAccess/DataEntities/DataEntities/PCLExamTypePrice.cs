using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    [DataContract]
    public partial class PCLExamTypePrice : NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new PCLExamTypePrice object.
     
        /// <param name="pCLExamTypePriceID">Initial value of the PCLExamTypePriceID property.</param>
        /// <param name="pCLExamTypeID">Initial value of the PCLExamTypeID property.</param>
        /// <param name="recCreatedDate">Initial value of the RecCreatedDate property.</param>
        /// <param name="normalPrice">Initial value of the NormalPrice property.</param>
        /// <param name="effectiveDate">Initial value of the EffectiveDate property.</param>
        /// <param name="isActive">Initial value of the IsActive property.</param>
        public static PCLExamTypePrice CreatePCLExamTypePrice(Int64 pCLExamTypePriceID, Int64 pCLExamTypeID, DateTime recCreatedDate, Decimal normalPrice, DateTime effectiveDate, Boolean isActive)
        {
            PCLExamTypePrice pCLExamTypePrice = new PCLExamTypePrice();
            pCLExamTypePrice.PCLExamTypePriceID = pCLExamTypePriceID;
            pCLExamTypePrice.PCLExamTypeID = pCLExamTypeID;
            pCLExamTypePrice.RecCreatedDate = recCreatedDate;
            pCLExamTypePrice.NormalPrice = normalPrice;
            pCLExamTypePrice.EffectiveDate = effectiveDate;
            pCLExamTypePrice.IsActive = isActive;
            return pCLExamTypePrice;
        }

        #endregion
        #region Primitive Properties

     
        [DataMemberAttribute()]
        public Int64 PCLExamTypePriceID
        {
            get
            {
                return _PCLExamTypePriceID;
            }
            set
            {
                if (_PCLExamTypePriceID != value)
                {
                    OnPCLExamTypePriceIDChanging(value);
                    _PCLExamTypePriceID =value;
                    RaisePropertyChanged("PCLExamTypePriceID");
                    OnPCLExamTypePriceIDChanged();
                }
            }
        }
        private Int64 _PCLExamTypePriceID;
        partial void OnPCLExamTypePriceIDChanging(Int64 value);
        partial void OnPCLExamTypePriceIDChanged();

       
        [DataMemberAttribute()]
        public Int64 PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                OnPCLExamTypeIDChanging(value);
                _PCLExamTypeID =value;
                RaisePropertyChanged("PCLExamTypeID");
                OnPCLExamTypeIDChanged();
            }
        }
        private Int64 _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(Int64 value);
        partial void OnPCLExamTypeIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> PCLExamTypePriceListID
        {
            get { return _PCLExamTypePriceListID; }
            set
            {
                if (_PCLExamTypePriceListID != value)
                {
                    OnPCLExamTypePriceListIDChanging(value);
                    _PCLExamTypePriceListID = value;
                    RaisePropertyChanged("PCLExamTypePriceListID");
                    OnPCLExamTypePriceListIDChanged();
                }
            }
        }
        private Nullable<Int64> _PCLExamTypePriceListID;
        partial void OnPCLExamTypePriceListIDChanging(Nullable<Int64> value);
        partial void OnPCLExamTypePriceListIDChanged();


        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                OnRecCreatedDateChanging(value);
                _RecCreatedDate =value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID =value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();


        [DataMemberAttribute()]        
        public Nullable<Int64> ApprovedStaffID
        {
            get { return _ApprovedStaffID; }
            set
            {
                if (_ApprovedStaffID != value)
                {
                    OnApprovedStaffIDChanging(value);
                    _ApprovedStaffID = value;
                    RaisePropertyChanged("ApprovedStaffID");
                    OnApprovedStaffIDChanged();
                }
            }
        }
        private Nullable<Int64> _ApprovedStaffID;
        partial void OnApprovedStaffIDChanging(Nullable<Int64> value);
        partial void OnApprovedStaffIDChanged();


        //[Range(1.0, 99999999999.0, ErrorMessage = "Đơn Giá Phải >=1")]
        //[Required(ErrorMessage = "Nhập Đơn Giá!")]
        [DataMemberAttribute()]
        public decimal NormalPrice
        {
            get { return _NormalPrice; }
            set
            {
                OnNormalPriceChanging(value);
                ValidateProperty("NormalPrice", value);
                _NormalPrice = value;
                RaisePropertyChanged("NormalPrice");
                OnNormalPriceChanged();
            }
        }
        private decimal _NormalPrice;
        partial void OnNormalPriceChanging(decimal value);
        partial void OnNormalPriceChanged();


        [DataMemberAttribute()]
        public decimal SuggestPrice
        {
            get { return _SuggestPrice; }
            set
            {
                if (_SuggestPrice != value)
                {
                    OnSuggestPriceChanging(value);
                    ValidateProperty("SuggestPrice", value);
                    _SuggestPrice = value;
                    RaisePropertyChanged("SuggestPrice");
                    OnSuggestPriceChanged();
                }
            }
        }
        private decimal _SuggestPrice;
        partial void OnSuggestPriceChanging(decimal value);
        partial void OnSuggestPriceChanged();

        [Range(0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public Nullable<decimal> PriceForHIPatient
        {
            get { return _PriceForHIPatient; }
            set
            {
                OnPriceForHIPatientChanging(value);
                ValidateProperty("PriceForHIPatient", value);
                _PriceForHIPatient = value;
                RaisePropertyChanged("PriceForHIPatient");
                OnPriceForHIPatientChanged();
            }
        }
        private Nullable<decimal> _PriceForHIPatient;
        partial void OnPriceForHIPatientChanging(Nullable<decimal> value);
        partial void OnPriceForHIPatientChanged();

        [Range(0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public Nullable<decimal> HIAllowedPrice
        {
            get { return _HIAllowedPrice; }
            set
            {
                OnHIAllowedPriceChanging(value);
                ValidateProperty("HIAllowedPrice", value);
                _HIAllowedPrice = value;
                RaisePropertyChanged("HIAllowedPrice");
                OnHIAllowedPriceChanged();
            }
        }
        private Nullable<decimal> _HIAllowedPrice;
        partial void OnHIAllowedPriceChanging(Nullable<decimal> value);
        partial void OnHIAllowedPriceChanged();


        [Required(ErrorMessage = "Nhập Ngày Áp Dụng!")]
        [DataMemberAttribute()]
        public Nullable<DateTime> EffectiveDate
        {
            get { return _EffectiveDate; }
            set
            {
                OnEffectiveDateChanging(value);
                ValidateProperty("EffectiveDate", value);
                _EffectiveDate = value;
                RaisePropertyChanged("EffectiveDate");
                OnEffectiveDateChanged();
            }
        }
        private Nullable<DateTime> _EffectiveDate;
        partial void OnEffectiveDateChanging(Nullable<DateTime> value);
        partial void OnEffectiveDateChanged();

       
        [DataMemberAttribute()]
        public Nullable<DateTime> EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                OnEndDateChanging(value);
                _EndDate =value;
                RaisePropertyChanged("EndDate");
                OnEndDateChanged();
            }
        }
        private Nullable<DateTime> _EndDate;
        partial void OnEndDateChanging(Nullable<DateTime> value);
        partial void OnEndDateChanged();

       
        [DataMemberAttribute()]
        public Boolean IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive =value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private Boolean _IsActive;
        partial void OnIsActiveChanging(Boolean value);
        partial void OnIsActiveChanged();

       
        [DataMemberAttribute()]
        public Nullable<Boolean> IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                OnIsDeletedChanging(value);
                _IsDeleted =value;
                RaisePropertyChanged("IsDeleted");
                OnIsDeletedChanged();
            }
        }
        private Nullable<Boolean> _IsDeleted;
        partial void OnIsDeletedChanging(Nullable<Boolean> value);
        partial void OnIsDeletedChanged();

        #endregion

        #region Navigate

        [DataMemberAttribute()]
        public string PCLExamTypeCode
        {
            get { return _PCLExamTypeCode; }
            set 
            { 
                if(_PCLExamTypeCode!=value)
                {
                    OnPCLExamTypeCodeChanging(value);
                    _PCLExamTypeCode = value;
                    RaisePropertyChanged("PCLExamTypeCode");
                    OnPCLExamTypeCodeChanged();
                }
            }
        }
        private string _PCLExamTypeCode;
        partial void OnPCLExamTypeCodeChanging(string value);
        partial void OnPCLExamTypeCodeChanged();

        [DataMemberAttribute()]        
        public string PCLExamTypeName
        {
            get { return _PCLExamTypeName; }
            set 
            {
                if (_PCLExamTypeName != value)
                {
                    OnPCLExamTypeNameChanging(value);
                    _PCLExamTypeName = value;
                    RaisePropertyChanged("PCLExamTypeName");
                    OnPCLExamTypeNameChanged();
                }
            }
        }
        private string _PCLExamTypeName;
        partial void OnPCLExamTypeNameChanging(string value);
        partial void OnPCLExamTypeNameChanged();

        [DataMemberAttribute()]        
        public Staff ObjStaffID
        {
            get { return _ObjStaffID; }
            set 
            {
                if (_ObjStaffID != value)
                {
                    OnObjStaffIDChanging(value);
                    _ObjStaffID = value;
                    RaisePropertyChanged("ObjStaffID");
                    OnObjStaffIDChanged();
                }
            }
        }
        private Staff _ObjStaffID;
        partial void OnObjStaffIDChanging(Staff value);
        partial void OnObjStaffIDChanged();
        
        
        
        [DataMemberAttribute()]
        public Staff ObjApprovedStaffID
        {
            get { return _ObjApprovedStaffID; }
            set 
            {
                if (_ObjApprovedStaffID != value)
                {
                    OnObjApprovedStaffIDChanging(value);
                    _ObjApprovedStaffID = value;
                    RaisePropertyChanged("ObjApprovedStaffID");
                    OnObjApprovedStaffIDChanged();
                }
            }
        }
        private Staff _ObjApprovedStaffID;
        partial void OnObjApprovedStaffIDChanging(Staff value);
        partial void OnObjApprovedStaffIDChanged();

        #endregion

        #region ext Prop
        
        
        [DataMemberAttribute()]
        public Nullable<bool> CanEdit
        {
            get { return _CanEdit; }
            set
            {
                if (_CanEdit != value)
                {
                    OnCanEditChanging(value);
                    _CanEdit = value;
                    RaisePropertyChanged("CanEdit");
                    OnCanEditChanged();
                }
            }
        }
        private Nullable<bool> _CanEdit;
        partial void OnCanEditChanging(Nullable<bool> value);
        partial void OnCanEditChanged();


        
        
        [DataMemberAttribute()]
        public Nullable<bool> CanDelete
        {
            get { return _CanDelete; }
            set
            {
                if (_CanDelete != value)
                {
                    OnCanDeleteChanging(value);
                    _CanDelete = value;
                    RaisePropertyChanged("CanDelete");
                    OnCanDeleteChanged();
                }
            }
        }
        private Nullable<bool> _CanDelete;
        partial void OnCanDeleteChanging(Nullable<bool> value);
        partial void OnCanDeleteChanged();


        
        
        [DataMemberAttribute()]
        public string PriceType
        {
            get { return _PriceType; }
            set
            {
                if (_PriceType != value)
                {
                    OnPriceTypeChanging(value);
                    _PriceType = value;
                    RaisePropertyChanged("PriceType");
                    OnPriceTypeChanged();
                }
            }
        }
        private string _PriceType;
        partial void OnPriceTypeChanging(string value);
        partial void OnPriceTypeChanged();
        #endregion

        #region Ext cho Lưới Tạo Bảng Giá Hàng Loạt
        [DataMemberAttribute()]
        public decimal NormalPrice_Old
        {
            get { return _NormalPrice_Old; }
            set
            {
                if (_NormalPrice_Old != value)
                {
                    OnNormalPrice_OldChanging(value);
                    _NormalPrice_Old = value;
                    RaisePropertyChanged("NormalPrice_Old");
                    OnNormalPrice_OldChanged();
                }
            }
        }
        private decimal _NormalPrice_Old;
        partial void OnNormalPrice_OldChanging(decimal value);
        partial void OnNormalPrice_OldChanged();

        [DataMemberAttribute()]
        public Nullable<decimal> PriceForHIPatient_Old
        {
            get { return _PriceForHIPatient_Old; }
            set
            {
                if (_PriceForHIPatient_Old != value)
                {
                    OnPriceForHIPatient_OldChanging(value);
                    _PriceForHIPatient_Old = value;
                    RaisePropertyChanged("PriceForHIPatient_Old");
                    OnPriceForHIPatient_OldChanged();
                }
            }
        }
        private Nullable<decimal> _PriceForHIPatient_Old;
        partial void OnPriceForHIPatient_OldChanging(Nullable<decimal> value);
        partial void OnPriceForHIPatient_OldChanged();

        [DataMemberAttribute()]
        public Nullable<decimal> HIAllowedPrice_Old
        {
            get { return _HIAllowedPrice_Old; }
            set
            {
                if (_HIAllowedPrice_Old != value)
                {
                    OnHIAllowedPrice_OldChanging(value);
                    _HIAllowedPrice_Old = value;
                    RaisePropertyChanged("HIAllowedPrice_Old");
                    OnHIAllowedPrice_OldChanged();
                }
            }
        }
        private Nullable<decimal> _HIAllowedPrice_Old;
        partial void OnHIAllowedPrice_OldChanging(Nullable<decimal> value);
        partial void OnHIAllowedPrice_OldChanged();

        [DataMemberAttribute()]
        public Nullable<double> VATRate_Old
        {
            get { return _VATRate_Old; }
            set
            {
                if (_VATRate_Old != value)
                {
                    OnVATRate_OldChanging(value);
                    _VATRate_Old = value;
                    RaisePropertyChanged("VATRate_Old");
                    OnVATRate_OldChanged();
                }
            }
        }
        private Nullable<double> _VATRate_Old;
        partial void OnVATRate_OldChanging(Nullable<double> value);
        partial void OnVATRate_OldChanged();

        //1: Insert, 2: Update, 3: Delete: 0: No Change
        public enum RowStateValue
        {
            Insert = 1,
            Update = 2,
            Delete = 3,
            NoChange = 0
        }

        [DataMemberAttribute()]
        public RowStateValue RowState
        {
            get { return _RowState; }
            set
            {
                OnRowStateChanging(value);
                _RowState = value;
                RaisePropertyChanged("RowState");
                OnRowStateChanged();
            }
        }
        private RowStateValue _RowState;
        partial void OnRowStateChanging(RowStateValue value);
        partial void OnRowStateChanged();

        #endregion
    }
}
