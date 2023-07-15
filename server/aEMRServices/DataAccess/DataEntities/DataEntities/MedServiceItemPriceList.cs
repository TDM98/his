using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class MedServiceItemPriceList : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Int64 MedServiceItemPriceListID
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
        private Int64 _MedServiceItemPriceListID;
        partial void OnMedServiceItemPriceListIDChanging(Int64 value);
        partial void OnMedServiceItemPriceListIDChanged();


        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                if (_DeptID != value)
                {
                    OnDeptIDChanging(value);
                    _DeptID = value;
                    RaisePropertyChanged("DeptID");
                    OnDeptIDChanged();
                }
            }
        }
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();


        [DataMemberAttribute()]
        public long MedicalServiceTypeID
        {
            get
            {
                return _MedicalServiceTypeID;
            }
            set
            {
                if (_MedicalServiceTypeID != value)
                {
                    OnMedicalServiceTypeIDChanging(value);
                    _MedicalServiceTypeID = value;
                    RaisePropertyChanged("MedicalServiceTypeID");
                    OnMedicalServiceTypeIDChanged();
                }
            }
        }
        private long _MedicalServiceTypeID;
        partial void OnMedicalServiceTypeIDChanging(long value);
        partial void OnMedicalServiceTypeIDChanged();


        private DateTime _RecCreatedDate;
        public DateTime RecCreatedDate
        {
            get { return _RecCreatedDate; }
            set 
            { 
                if(_RecCreatedDate!=value)
                {
                    OnRecCreatedDateChanging(value);
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                    OnRecCreatedDateChanged();
                }
            }
        }
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();


        //[Required(ErrorMessage = "Nhập Tiêu Đề Bảng Giá!")]
        //[StringLength(50, MinimumLength = 0, ErrorMessage = "Tiêu Đề Bảng Giá <= 50 Ký Tự")]
        [DataMemberAttribute()]
        public String PriceListTitle
        {
            get
            {
                return _PriceListTitle;
            }
            set
            {
                OnPriceListTitleChanging(value);
                ValidateProperty("PriceListTitle", value);
                _PriceListTitle = value;
                RaisePropertyChanged("PriceListTitle");
                OnPriceListTitleChanged();
            }
        }
        private String _PriceListTitle;
        partial void OnPriceListTitleChanging(String value);
        partial void OnPriceListTitleChanged();


        [DataMemberAttribute()]
        public Int64 StaffID
        {
            get { return _StaffID; }
            set
            {
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


        [DataMemberAttribute()]
        public DateTime EffectiveDate
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
        private DateTime _EffectiveDate;
        partial void OnEffectiveDateChanging(DateTime value);
        partial void OnEffectiveDateChanged();

        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get { return _IsDeleted; }
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
        private bool _IsDeleted;
        partial void OnIsDeletedChanging(bool value);
        partial void OnIsDeletedChanged();

        [DataMemberAttribute()]
        public bool IsActive
        {
            get { return _IsActive; }
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
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();
        #region Navigate

        [DataMemberAttribute]
        public RefDepartments ObjRefDepartments
        {
            get { return _ObjRefDepartments; }
            set
            {
                if(_ObjRefDepartments!=value)
                {
                    OnObjRefDepartmentsChanging(value);
                    _ObjRefDepartments = value;
                    RaisePropertyChanged("ObjRefDepartments");
                    OnObjRefDepartmentsChanged();
                }
            }
        }
        private RefDepartments _ObjRefDepartments;
        partial void OnObjRefDepartmentsChanging(RefDepartments value);
        partial void OnObjRefDepartmentsChanged();


        [DataMemberAttribute]
        public RefMedicalServiceType ObjRefMedicalServiceType
        {
            get { return _ObjRefMedicalServiceType; }
            set
            {
                if(_ObjRefMedicalServiceType!=value)
                {
                    OnObjRefMedicalServiceTypeChanging(value);
                    _ObjRefMedicalServiceType = value;
                    RaisePropertyChanged("ObjRefMedicalServiceType");
                    OnObjRefMedicalServiceTypeChanged();
                }
            }
        }
        private RefMedicalServiceType _ObjRefMedicalServiceType;
        partial void OnObjRefMedicalServiceTypeChanging(RefMedicalServiceType value);
        partial void OnObjRefMedicalServiceTypeChanged();

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


        //Check CanEditCanDelete
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
        public string PriceListType
        {
            get { return _PriceListType; }
            set
            {
                if (_PriceListType != value)
                {
                    OnPriceListTypeChanging(value);
                    _PriceListType = value;
                    RaisePropertyChanged("PriceListType");
                    OnPriceListTypeChanged();
                }
            }
        }
        private string _PriceListType;
        partial void OnPriceListTypeChanging(string value);
        partial void OnPriceListTypeChanged();
        //Check CanEditCanDelete

        #endregion

        [DataMemberAttribute()]
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    OnIsCheckedChanging(value);
                    _IsChecked = value;
                    RaisePropertyChanged("IsChecked");
                    OnIsCheckedChanged();
                }
            }
        }
        private bool _IsChecked;
        partial void OnIsCheckedChanging(bool value);
        partial void OnIsCheckedChanged();
    }
}
