using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class PCLExamTypePriceList : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Int64 PCLExamTypePriceListID
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
        private Int64 _PCLExamTypePriceListID;
        partial void OnPCLExamTypePriceListIDChanging(Int64 value);
        partial void OnPCLExamTypePriceListIDChanged();
        
        
        private DateTime _RecCreatedDate;
        public DateTime RecCreatedDate
        {
            get { return _RecCreatedDate; }
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
        //Check CanEditCanDelete
        public Nullable<bool> CanEdit
        {
            get { return IsActive; }

        }
        //neu co bang gia tuong lai,thi so sanh ngay effec voi ngay hien hanh
        public Nullable<bool> CanDelete
        {
            get
            {
                if (IsActive)
                {
                    return false;
                }
                else
                {
                    if (AxHelper.CompareDate(EffectiveDate.GetValueOrDefault(DateTime.Now), DateTime.Now) == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                };
            }

        }

        [DataMemberAttribute()]
        public string PriceListType
        {
            get
            {
                if (IsActive)
                {
                    return "PriceList-InUse";
                }
                else
                {
                    if (AxHelper.CompareDate(EffectiveDate.GetValueOrDefault(DateTime.Now), DateTime.Now) == 1)
                    {
                        return "PriceList-InFuture";
                    }
                    else
                    {
                        return "PriceList-Old";
                    }
                }
            }

        }
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
