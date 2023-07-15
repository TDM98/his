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
    public partial class DrugDeptSellingItemPrices : NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public Int64 DrugDeptSellingItemPriceID
        {
            get
            {
                return _DrugDeptSellingItemPriceID;
            }
            set
            {
                if (_DrugDeptSellingItemPriceID != value)
                {
                    OnDrugDeptSellingItemPriceIDChanging(value);
                    _DrugDeptSellingItemPriceID = value;
                    RaisePropertyChanged("DrugDeptSellingItemPriceID");
                    OnDrugDeptSellingItemPriceIDChanged();
                }
            }
        }
        private Int64 _DrugDeptSellingItemPriceID;
        partial void OnDrugDeptSellingItemPriceIDChanging(Int64 value);
        partial void OnDrugDeptSellingItemPriceIDChanged();

       
        [DataMemberAttribute()]
        public Int64 GenMedProductID
        {
            get
            {
                return _GenMedProductID;
            }
            set
            {
                if (_GenMedProductID != value)
                {
                    OnGenMedProductIDChanging(value);
                    _GenMedProductID = value;
                    RaisePropertyChanged("GenMedProductID");
                    OnGenMedProductIDChanged();
                }
            }
        }
        private Int64 _GenMedProductID;
        partial void OnGenMedProductIDChanging(Int64 value);
        partial void OnGenMedProductIDChanged();


        [DataMemberAttribute()]
        public Int64 DrugDeptSellingPriceListID
        {
            get { return _DrugDeptSellingPriceListID; }
            set
            {
                if (_DrugDeptSellingPriceListID != value)
                {
                    OnDrugDeptSellingPriceListIDChanging(value);
                    _DrugDeptSellingPriceListID = value;
                    RaisePropertyChanged("DrugDeptSellingPriceListID");
                    OnDrugDeptSellingPriceListIDChanged();
                }
            }
        }
        private Int64 _DrugDeptSellingPriceListID;
        partial void OnDrugDeptSellingPriceListIDChanging(Int64 value);
        partial void OnDrugDeptSellingPriceListIDChanged();


        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                if (_RecCreatedDate != value)
                {
                    OnRecCreatedDateChanging(value);
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                    OnRecCreatedDateChanged();
                }
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();

        [DataMemberAttribute()]
        public Int64 StaffID
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
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();


        [DataMemberAttribute()]        
        public Int64 ApprovedStaffID
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
        private Int64 _ApprovedStaffID;
        partial void OnApprovedStaffIDChanging(Int64 value);
        partial void OnApprovedStaffIDChanged();

        
        [DataMemberAttribute()]
        public Int64 inviIDBefore
        {
            get { return _inviIDBefore; }
            set
            {
                if (_inviIDBefore != value)
                {
                    OninviIDBeforeChanging(value);
                    _inviIDBefore = value;
                    RaisePropertyChanged("inviIDBefore");
                    OninviIDBeforeChanged();
                }
            }
        }
        private Int64 _inviIDBefore;
        partial void OninviIDBeforeChanging(Int64 value);
        partial void OninviIDBeforeChanged();

        [DataMemberAttribute()]
        public decimal InCostBefore
        {
            get { return _InCostBefore; }
            set
            {
                if (_InCostBefore != value)
                {
                    OnInCostBeforeChanging(value);
                    _InCostBefore = value;
                    RaisePropertyChanged("InCostBefore");
                    OnInCostBeforeChanged();
                }
            }
        }
        private decimal _InCostBefore;
        partial void OnInCostBeforeChanging(decimal value);
        partial void OnInCostBeforeChanged();
        
        
        [DataMemberAttribute()]
        public Int64 inviID
        {
            get { return _inviID; }
            set
            {
                if (_inviID != value)
                {
                    OninviIDChanging(value);
                    _inviID = value;
                    RaisePropertyChanged("inviID");
                    OninviIDChanged();
                }
            }
        }
        private Int64 _inviID;
        partial void OninviIDChanging(Int64 value);
        partial void OninviIDChanged();

        [DataMemberAttribute()]
        public decimal InCost
        {
            get { return _InCost; }
            set
            {
                if (_InCost != value)
                {
                    OnInCostChanging(value);
                    _InCost = value;
                    RaisePropertyChanged("InCost");
                    OnInCostChanged();
                }
            }
        }
        private decimal _InCost;
        partial void OnInCostChanging(decimal value);
        partial void OnInCostChanged();


        [DataMemberAttribute()]
        public String PercentProfit
        {
            get { return _PercentProfit; }
            set
            {
                if (_PercentProfit != value)
                {
                    OnPercentProfitChanging(value);
                    _PercentProfit = value;
                    RaisePropertyChanged("PercentProfit");
                    OnPercentProfitChanged();
                }
            }
        }
        private String _PercentProfit;
        partial void OnPercentProfitChanging(String value);
        partial void OnPercentProfitChanged();



        //[Range(1.0, 99999999999.0, ErrorMessage = "Đơn Giá Phải >=1")]
        //[Required(ErrorMessage = "Nhập Đơn Giá!")]
        [DataMemberAttribute()]
        public decimal NormalPrice
        {
            get { return _NormalPrice; }
            set
            {
                if (_NormalPrice != value)
                {
                    OnNormalPriceChanging(value);
                    ValidateProperty("NormalPrice", value);
                    _NormalPrice = value;
                    RaisePropertyChanged("NormalPrice");
                    OnNormalPriceChanged();
                }
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
        public decimal PriceForHIPatient
        {
            get { return _PriceForHIPatient; }
            set
            {
                if (_PriceForHIPatient != value)
                {
                    OnPriceForHIPatientChanging(value);
                    ValidateProperty("PriceForHIPatient", value);
                    _PriceForHIPatient = value;
                    RaisePropertyChanged("PriceForHIPatient");
                    OnPriceForHIPatientChanged();
                }
            }
        }
        private decimal _PriceForHIPatient;
        partial void OnPriceForHIPatientChanging(decimal value);
        partial void OnPriceForHIPatientChanged();


        [Range(0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public decimal HIAllowedPrice
        {
            get { return _HIAllowedPrice; }
            set
            {
                if (_HIAllowedPrice != value)
                {
                    OnHIAllowedPriceChanging(value);
                    ValidateProperty("HIAllowedPrice", value);
                    _HIAllowedPrice = value;
                    RaisePropertyChanged("HIAllowedPrice");
                    OnHIAllowedPriceChanged();
                }
            }
        }
        private decimal _HIAllowedPrice;
        partial void OnHIAllowedPriceChanging(decimal value);
        partial void OnHIAllowedPriceChanged();


        [Required(ErrorMessage = "Nhập Ngày Áp Dụng!")]
        [DataMemberAttribute()]
        public Nullable<DateTime> EffectiveDate
        {
            get { return _EffectiveDate; }
            set
            {
                if (_EffectiveDate != value)
                {
                    OnEffectiveDateChanging(value);
                    ValidateProperty("EffectiveDate", value);
                    _EffectiveDate = value;
                    RaisePropertyChanged("EffectiveDate");
                    OnEffectiveDateChanged();
                }
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
                if (_EndDate != value)
                {
                    OnEndDateChanging(value);
                    _EndDate = value;
                    RaisePropertyChanged("EndDate");
                    OnEndDateChanged();
                }
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
                if (_IsActive != value)
                {
                    OnIsActiveChanging(value);
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                    OnIsActiveChanged();
                }
            }
        }
        private Boolean _IsActive;
        partial void OnIsActiveChanging(Boolean value);
        partial void OnIsActiveChanged();

       
        [DataMemberAttribute()]
        public Boolean IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted != value)
                {
                    OnIsDeletedChanging(value);
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                    OnIsDeletedChanged();
                }
            }
        }
        private Boolean _IsDeleted;
        partial void OnIsDeletedChanging(Boolean value);
        partial void OnIsDeletedChanged();


        [DataMemberAttribute()]
        public string Notes
        {
            get { return _Notes; }
            set
            {
                if (_Notes != value)
                {
                    _Notes = value;
                    RaisePropertyChanged("Notes");
                }
            }
        }
        private string _Notes;

        [DataMemberAttribute()]
        public string StaffNotes
        {
            get { return _StaffNotes; }
            set
            {
                if (_StaffNotes != value)
                {
                    _StaffNotes = value;
                    RaisePropertyChanged("StaffNotes");
                }
            }
        }
        private string _StaffNotes;

        #endregion

        #region Navigate

        [DataMemberAttribute()]
        public string BrandName
        {
            get { return _BrandName; }
            set
            {
                if (_BrandName != value)
                {
                    OnBrandNameChanging(value);
                    _BrandName = value;
                    RaisePropertyChanged("BrandName");
                    OnBrandNameChanged();
                }
            }
        }
        private string _BrandName;
        partial void OnBrandNameChanging(string value);
        partial void OnBrandNameChanged();

        
        [DataMemberAttribute()]
        public string GenericName
        {
            get { return _GenericName; }
            set
            {
                if (_GenericName != value)
                {
                    OnGenericNameChanging(value);
                    _GenericName = value;
                    RaisePropertyChanged("GenericName");
                    OnGenericNameChanged();
                }
            }
        }
        private string _GenericName;
        partial void OnGenericNameChanging(string value);
        partial void OnGenericNameChanged();

        [DataMemberAttribute()]
        public string Code
        {
            get { return _Code; }
            set
            {
                if (_Code != value)
                {
                    _Code = value;
                    RaisePropertyChanged("Code");
                }
            }
        }
        private string _Code;

        [DataMemberAttribute()]
        public string HICode
        {
            get { return _HICode; }
            set
            {
                if (_HICode != value)
                {
                    _HICode = value;
                    RaisePropertyChanged("HICode");
                }
            }
        }
        private string _HICode;

        [DataMemberAttribute()]
        public Nullable<Boolean> InsuranceCover
        {
            get
            {
                return _InsuranceCover;
            }
            set
            {
                OnInsuranceCoverChanging(value);
                _InsuranceCover = value;
                RaisePropertyChanged("InsuranceCover");
                OnInsuranceCoverChanged();
            }
        }
        private Nullable<Boolean> _InsuranceCover;
        partial void OnInsuranceCoverChanging(Nullable<Boolean> value);
        partial void OnInsuranceCoverChanged();


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
        public decimal PriceForHIPatient_Old
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
        private decimal _PriceForHIPatient_Old;
        partial void OnPriceForHIPatient_OldChanging(decimal value);
        partial void OnPriceForHIPatient_OldChanged();

        [DataMemberAttribute()]
        public decimal HIAllowedPrice_Old
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
        private decimal _HIAllowedPrice_Old;
        partial void OnHIAllowedPrice_OldChanging(decimal value);
        partial void OnHIAllowedPrice_OldChanged();
        

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

        #region List

        #endregion
    }
}
