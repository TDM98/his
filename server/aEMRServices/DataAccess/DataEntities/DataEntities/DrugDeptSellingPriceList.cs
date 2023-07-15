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
    public partial class DrugDeptSellingPriceList : NotifyChangedBase
    {

        [DataMemberAttribute()]
        private Int64 _V_MedProductType;
        public Int64 V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                RaisePropertyChanged("V_MedProductType");
            }
        }

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
        [StringLength(50, MinimumLength = 0, ErrorMessage = "Tiêu Đề Bảng Giá <= 50 Ký Tự")]
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
        public ObservableCollection<DrugDeptSellingItemPrices> _ObjDrugDeptSellingItemPrices;
        public ObservableCollection<DrugDeptSellingItemPrices> ObjDrugDeptSellingItemPrices
        {
            get
            {
                return _ObjDrugDeptSellingItemPrices;
            }
            set
            {
                if (_ObjDrugDeptSellingItemPrices != value)
                {
                    _ObjDrugDeptSellingItemPrices = value;
                    RaisePropertyChanged("ObjDrugDeptSellingItemPrices");
                }
            }
        }
        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(ObjDrugDeptSellingItemPrices);
        }
        public string ConvertDetailsListToXml(IEnumerable<DrugDeptSellingItemPrices> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (DrugDeptSellingItemPrices item in ObjList)
                {
                    sb.Append("<DrugDeptSellingItemPrices>");
                    sb.AppendFormat("<DrugDeptSellingItemPriceID>{0}</DrugDeptSellingItemPriceID>", item.DrugDeptSellingItemPriceID);
                    sb.AppendFormat("<GenMedProductID>{0}</GenMedProductID>", item.GenMedProductID);
                    sb.AppendFormat("<DrugDeptSellingPriceListID>{0}</DrugDeptSellingPriceListID>", item.DrugDeptSellingPriceListID);
                    //sb.AppendFormat("<RecCreatedDate>{0}</RecCreatedDate>", item.RecCreatedDate.ToString("MM-dd-yyyy HH:mm:ss"));
                    sb.AppendFormat("<StaffID>{0}</StaffID>", item.StaffID);
                    sb.AppendFormat("<ApprovedStaffID>{0}</ApprovedStaffID>", item.ApprovedStaffID);
                    sb.AppendFormat("<InCost>{0}</InCost>", item.InCost);
                    sb.AppendFormat("<NormalPrice>{0}</NormalPrice>", item.NormalPrice);
                    sb.AppendFormat("<PriceForHIPatient>{0}</PriceForHIPatient>", item.PriceForHIPatient);
                    sb.AppendFormat("<HIAllowedPrice>{0}</HIAllowedPrice>", item.HIAllowedPrice);
                    sb.AppendFormat("<EffectiveDate>{0}</EffectiveDate>", EffectiveDate.ToString("MM-dd-yyyy HH:mm:ss"));
                    sb.AppendFormat("<EndDate>{0}</EndDate>", item.EndDate);
                    sb.AppendFormat("<IsActive>{0}</IsActive>", item.IsActive);
                    sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", item.IsDeleted);
                    sb.AppendFormat("<inviIDBefore>{0}</inviIDBefore>", item.inviIDBefore);
                    sb.AppendFormat("<inviID>{0}</inviID>", item.inviID);
                    sb.AppendFormat("<StaffNotes>{0}</StaffNotes>", item.StaffNotes);
                    sb.Append("</DrugDeptSellingItemPrices>");
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
