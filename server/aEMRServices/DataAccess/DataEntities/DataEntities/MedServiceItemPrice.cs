using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class MedServiceItemPrice : NotifyChangedBase
    {
        [DataMemberAttribute()]        
        public Int64 MedServItemPriceID
        {
            get { return _MedServItemPriceID; }
            set {
                if (_MedServItemPriceID != value)
                {
                    OnMedServItemPriceIDChanging(value);
                    _MedServItemPriceID = value;
                    RaisePropertyChanged("MedServItemPriceID");
                    OnMedServItemPriceIDChanged();
                }
            }
        }
        private Int64 _MedServItemPriceID;
        partial void OnMedServItemPriceIDChanging(Int64 value);
        partial void OnMedServItemPriceIDChanged();


        [DataMemberAttribute()]
        public Int64 MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                if (_MedServiceID != value)
                {
                    OnMedServiceIDChanging(value);
                    _MedServiceID = value;
                    RaisePropertyChanged("MedServiceID");
                    OnMedServiceIDChanged();
                }
            }
        }
        private Int64 _MedServiceID;
        partial void OnMedServiceIDChanging(Int64 value);
        partial void OnMedServiceIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> MedServiceItemPriceListID
        {
            get { return _MedServiceItemPriceListID; }
            set
            {
                if (_MedServiceItemPriceListID != value)
                {
                    OnMedServiceItemPriceListIDChanging(value);
                    _MedServiceItemPriceListID = value;
                    RaisePropertyChanged("MedServiceItemPriceListID");
                    OnMedServiceItemPriceListIDChanged();
                }
            }
        }
        private Nullable<Int64> _MedServiceItemPriceListID;
        partial void OnMedServiceItemPriceListIDChanging(Nullable<Int64> value);
        partial void OnMedServiceItemPriceListIDChanged();


        [DataMemberAttribute()]        
        public Int64 StaffID
        {
            get { return _StaffID; }
            set {
                if (_StaffID != value)
                {
                    OnStaffIDChanging(value);
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                    OnStaffIDChanged();
                }
            }
        }
        private Int64 _StaffID;
        partial void OnStaffIDChanging(Int64 value);
        partial void OnStaffIDChanged();

        
        [DataMemberAttribute()]                
        public Nullable<Int64> ApprovedStaffID
        {
            get { return _ApprovedStaffID; }
            set {
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


        [Range(0.0, 99999999999.0, ErrorMessage = "Không Được < 0")]
        [DataMemberAttribute()]
        public Nullable<double> VATRate
        {
            get { return _VATRate; }
            set
            {
                if (_VATRate != value)
                {
                    OnVATRateChanging(value);
                    ValidateProperty("VATRate", value);
                    _VATRate = value;
                    RaisePropertyChanged("VATRate");
                    OnVATRateChanged();
                }
            }
        }
        private Nullable<double> _VATRate;
        partial void OnVATRateChanging(Nullable<double> value);
        partial void OnVATRateChanged();



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


        ////Hàm kiểm tra
        //public static ValidationResult ValidCustom_IsNumeric(object objValue, ValidationContext context)
        //{
        //    if (objValue != null && objValue != DBNull.Value)
        //    {
        //        if (IsNumeric(objValue) == false)
        //        {
        //            return new ValidationResult("Vui Lòng Nhập Số!", new string[] { "NormalPrice", "VATRate", "PriceForHIPatient", "HIAllowedPrice" });
        //        }
        //    }
        //    return ValidationResult.Success;
        //}
        //private static bool IsNumeric(object Expression)
        //{
        //    bool isNum;
        //    double retNum;
        //    isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        //    return isNum;
        //}
        ////Hàm kiểm tra


        [Range(0, 99999999999.0, ErrorMessage = "Phải >= 0")]
        [DataMemberAttribute()]
        public Nullable<decimal> PriceForHIPatient
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
        private Nullable<decimal> _PriceForHIPatient;
        partial void OnPriceForHIPatientChanging(Nullable<decimal> value);
        partial void OnPriceForHIPatientChanged();

       
        
        [DataMemberAttribute()]                                
        public Nullable<decimal> PriceDifference
        {
            get { return _PriceDifference; }
            set
            {
                if (_PriceDifference != value)
                {
                    OnPriceDifferenceChanging(value);
                    _PriceDifference = value;
                    RaisePropertyChanged("PriceDifference"); ;
                    OnPriceDifferenceChanged();
                }
            }
        }
        private Nullable<decimal> _PriceDifference;
        partial void OnPriceDifferenceChanging(Nullable<decimal> value);
        partial void OnPriceDifferenceChanged();


         [Range(0, 99999999999.0, ErrorMessage = "Phải >= 0")]
        [DataMemberAttribute()]
        public Nullable<decimal> HIAllowedPrice
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
        private Nullable<decimal> _HIAllowedPrice;
        partial void OnHIAllowedPriceChanging(Nullable<decimal> value);
        partial void OnHIAllowedPriceChanged();


        
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
        public bool IsActive
        {
            get { return _IsActive; }
            set {
                if (_IsActive != value)
                {
                    OnIsActiveChanging(value);
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                    OnIsActiveChanged();
                }
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();

        
        [DataMemberAttribute()]        
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set {
                if (_IsDeleted != value)
                {
                    OnIsDeletedChanging(value);
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                    OnIsDeletedChanged();
                }
            }
        }
        private bool _IsDeleted;
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();

        
        [DataMemberAttribute()]        
        private string _Note;
        public string Note
        {
            get { return _Note; }
            set 
            {
                if (_Note != value)
                {
                    OnNoteChanging(value);
                    _Note = value;
                    RaisePropertyChanged("Note");
                    OnNoteChanged();
                }
            }
        }
        partial void OnNoteChanging(string value);
        partial void OnNoteChanged();


        [DataMemberAttribute()]
        private Int32 _V_NewPriceType;
        public Int32 V_NewPriceType
        {
            get { return _V_NewPriceType; }
            set
            {
                if (_V_NewPriceType != value)
                {
                    OnNewPriceTypeChanging(value);
                    _V_NewPriceType = value;
                    RaisePropertyChanged("V_NewPriceType");
                    OnNewPriceTypeChanging();
                }
            }
        }
        partial void OnNewPriceTypeChanging(Int32 value);
        partial void OnNewPriceTypeChanging();


        #region Navigate        

        [DataMemberAttribute()]        
        public DeptMedServiceItems ObjDeptMedServiceItems
        {
            get { return _ObjDeptMedServiceItems; }
            set 
            {
                if (_ObjDeptMedServiceItems != value)
                {
                    OnObjDeptMedServiceItemsChanging(value);
                    _ObjDeptMedServiceItems = value;
                    RaisePropertyChanged("ObjDeptMedServiceItems");
                    OnObjDeptMedServiceItemsChanged();
                }
            }
        }
        private DeptMedServiceItems _ObjDeptMedServiceItems;
        partial void OnObjDeptMedServiceItemsChanging(DeptMedServiceItems value);
        partial void OnObjDeptMedServiceItemsChanged();


        [DataMemberAttribute()]                
        public RefMedicalServiceItem ObjMedServiceID
        {
            get { return _ObjMedServiceID; }
            set 
            {
                if (_ObjMedServiceID != value)
                {
                    OnObjMedServiceIDChanging(value);
                    _ObjMedServiceID = value;
                    RaisePropertyChanged("ObjMedServiceID");
                    OnObjMedServiceIDChanged();
                }

            }
        }
        private RefMedicalServiceItem _ObjMedServiceID;
        partial void OnObjMedServiceIDChanging(RefMedicalServiceItem value);
        partial void OnObjMedServiceIDChanged();


        [DataMemberAttribute()]        
        private Staff _ObjStaffID;
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
                
        
        [DataMemberAttribute()]        
        public Nullable<bool> CanEdit
        {
            get { return _CanEdit; }
            set 
            { 
                if(_CanEdit!=value)
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

        
        
        [DataMemberAttribute()]  
        public bool ServicesIsInUse
        {
            get 
            { 
                return _ServicesIsInUse; 
            }
            set 
            {
                if (_ServicesIsInUse != value)
                {
                    OnServicesIsInUseChanging(value);
                    _ServicesIsInUse = value;
                    RaisePropertyChanged("ServicesIsInUse");
                    OnServicesIsInUseChanged();
                }
            }
        }
        private bool _ServicesIsInUse;
        partial void OnServicesIsInUseChanging(bool value);
        partial void OnServicesIsInUseChanged();


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
