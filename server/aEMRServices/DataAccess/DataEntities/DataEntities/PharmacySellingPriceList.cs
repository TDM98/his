using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Text;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class PharmacySellingPriceList : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public Int64 PharmacySellingPriceListID
        {
            get { return _PharmacySellingPriceListID; }
            set
            {
                if (_PharmacySellingPriceListID != value)
                {
                    OnPharmacySellingPriceListIDChanging(value);
                    _PharmacySellingPriceListID = value;
                    RaisePropertyChanged("PharmacySellingPriceListID");
                    OnPharmacySellingPriceListIDChanged();
                }
            }
        }
        private Int64 _PharmacySellingPriceListID;
        partial void OnPharmacySellingPriceListIDChanging(Int64 value);
        partial void OnPharmacySellingPriceListIDChanged();


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
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();


        [Required(ErrorMessage = "Nhập Tiêu Đề Bảng Giá!")]
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

        [Required(ErrorMessage = "Nhập Ngày Áp Dụng!")]
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
                    if (AxHelper.CompareDate(EffectiveDate, DateTime.Now) == 1)
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
                    if (AxHelper.CompareDate(EffectiveDate, DateTime.Now) == 1)
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


        [DataMemberAttribute()]
        public ObservableCollection<PharmacySellingItemPrices> _ObjPharmacySellingItemPrices;
        public ObservableCollection<PharmacySellingItemPrices> ObjPharmacySellingItemPrices
        {
            get
            {
                return _ObjPharmacySellingItemPrices;
            }
            set
            {
                if (_ObjPharmacySellingItemPrices != value)
                {
                    _ObjPharmacySellingItemPrices = value;
                    RaisePropertyChanged("ObjPharmacySellingItemPrices");
                }
            }
        }
        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(ObjPharmacySellingItemPrices);
        }
        public string ConvertDetailsListToXml(IEnumerable<PharmacySellingItemPrices> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PharmacySellingItemPrices item in ObjList)
                {
                    sb.Append("<PharmacySellingItemPrices>");
                    sb.AppendFormat("<PharmacySellingItemPriceID>{0}</PharmacySellingItemPriceID>", item.PharmacySellingItemPriceID);
                    sb.AppendFormat("<DrugID>{0}</DrugID>", item.DrugID);
                    sb.AppendFormat("<PharmacySellingPriceListID>{0}</PharmacySellingPriceListID>", item.PharmacySellingPriceListID);
                    sb.AppendFormat("<RecCreatedDate>{0}</RecCreatedDate>", item.RecCreatedDate);
                    sb.AppendFormat("<StaffID>{0}</StaffID>", item.StaffID);
                    sb.AppendFormat("<ApprovedStaffID>{0}</ApprovedStaffID>", item.ApprovedStaffID);
                    sb.AppendFormat("<InCost>{0}</InCost>", item.InCost);
                    sb.AppendFormat("<NormalPrice>{0}</NormalPrice>", item.NormalPrice);
                    sb.AppendFormat("<PriceForHIPatient>{0}</PriceForHIPatient>", item.PriceForHIPatient);
                    sb.AppendFormat("<HIAllowedPrice>{0}</HIAllowedPrice>", item.HIAllowedPrice);
                    sb.AppendFormat("<EffectiveDate>{0}</EffectiveDate>", EffectiveDate);
                    sb.AppendFormat("<EndDate>{0}</EndDate>", item.EndDate);
                    sb.AppendFormat("<IsActive>{0}</IsActive>", item.IsActive);
                    sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", item.IsDeleted);
                    sb.AppendFormat("<inviIDBefore>{0}</inviIDBefore>", item.inviIDBefore);
                    sb.AppendFormat("<inviID>{0}</inviID>", item.inviID);
                    sb.AppendFormat("<StaffNotes>{0}</StaffNotes>", item.StaffNotes);
                    sb.Append("</PharmacySellingItemPrices>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #endregion

    }
}
