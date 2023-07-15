using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DeptMedServiceItems : NotifyChangedBase
    {

        [DataMemberAttribute()]        
        public Int64 DeptMedServItemID
        {
            get 
            { 
                return _DeptMedServItemID; 
            }
            set 
            {
                if (_DeptMedServItemID != value)
                {
                    OnDeptMedServItemIDChanging(value);
                    _DeptMedServItemID = value;
                    RaisePropertyChanged("DeptMedServItemID");
                    OnDeptMedServItemIDChanged();
                }
            }
        }
        private Int64 _DeptMedServItemID;
        partial void OnDeptMedServItemIDChanging(Int64 value);
        partial void OnDeptMedServItemIDChanged();

        [DataMemberAttribute()]                
        public Int64 DeptID
        {
            get { return _DeptID; }
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
        private Int64 _DeptID;
        partial void OnDeptIDChanging(Int64 value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]                        
        public Int64 MedServiceID
        {
            get { return _MedServiceID; }
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
        

        //[DataMemberAttribute()]
        //private bool _IsAllowRegistrationExam;
        //public bool IsAllowRegistrationExam
        //{
        //    get 
        //    { 
        //        return _IsAllowRegistrationExam; 
        //    }
        //    set 
        //    {
        //        if (_IsAllowRegistrationExam != value)
        //        {
        //            OnIsAllowRegistrationExamChanging(value);
        //            _IsAllowRegistrationExam = value;
        //            RaisePropertyChanged("IsAllowRegistrationExam");
        //            OnIsAllowRegistrationExamChanged();
        //        }
        //    }
        //}
        //partial void OnIsAllowRegistrationExamChanging(bool value);
        //partial void OnIsAllowRegistrationExamChanged();


        #region Navigate
        [DataMemberAttribute()]                        
        public RefDepartments ObjDeptID
        {
            get { return _ObjDeptID; }
            set {
                if (_ObjDeptID != value)
                {
                    OnObjDeptIDChanging(value);
                    _ObjDeptID = value;
                    RaisePropertyChanged("ObjDeptID");
                    OnObjDeptIDChanged();
                }
            }
        }
        private RefDepartments _ObjDeptID;
        partial void OnObjDeptIDChanging(RefDepartments value);
        partial void OnObjDeptIDChanged();

        [DataMemberAttribute()]                                
        public RefMedicalServiceItem ObjRefMedicalServiceItem
        {
            get { return _ObjRefMedicalServiceItem; }
            set {
                if (_ObjRefMedicalServiceItem != value)
                {
                    OnObjRefMedicalServiceItemChanging(value);
                    _ObjRefMedicalServiceItem = value;
                    RaisePropertyChanged("ObjRefMedicalServiceItem");
                    OnObjRefMedicalServiceItemChanged();
                }
            }
        }
        private RefMedicalServiceItem _ObjRefMedicalServiceItem;
        partial void OnObjRefMedicalServiceItemChanging(RefMedicalServiceItem value);
        partial void OnObjRefMedicalServiceItemChanged();

        [DataMemberAttribute()]                                
        private MedServiceItemPrice _ObjMedServiceItemPrice;
        public MedServiceItemPrice ObjMedServiceItemPrice
        {
            get { return _ObjMedServiceItemPrice; }
            set 
            {
                if (_ObjMedServiceItemPrice != value)
                {
                    OnObjMedServiceItemPriceChanging(value);
                    _ObjMedServiceItemPrice = value;
                    RaisePropertyChanged("ObjMedServiceItemPrice");
                    OnObjMedServiceItemPriceChanged();
                }
            }
        }
        partial void OnObjMedServiceItemPriceChanging(MedServiceItemPrice value);
        partial void OnObjMedServiceItemPriceChanged();


        [Required(ErrorMessage = "Chọn Đơn Vị Tính!")]
        [DataMemberAttribute()]
        public Int64 V_RefMedServiceItemsUnit
        {
            get { return _V_RefMedServiceItemsUnit; }
            set
            {
                if (_V_RefMedServiceItemsUnit != value)
                {
                    OnV_RefMedServiceItemsUnitChanging(value);
                    ValidateProperty("V_RefMedServiceItemsUnit", value);
                    _V_RefMedServiceItemsUnit = value;
                    RaisePropertyChanged("V_RefMedServiceItemsUnit");
                    OnV_RefMedServiceItemsUnitChanged();
                }
            }
        }
        private Int64 _V_RefMedServiceItemsUnit;
        partial void OnV_RefMedServiceItemsUnitChanging(Int64 value);
        partial void OnV_RefMedServiceItemsUnitChanged();


        #endregion


        #region Config Cho Phép Hẹn Bệnh
        [DataMemberAttribute()]
        public ApptService ObjApptService
        {
            get
            {
                return _ObjApptService;
            }
            set
            {
                if (_ObjApptService != value)
                {
                    OnObjApptServiceChanging(value);
                    _ObjApptService = value;
                    RaisePropertyChanged("ObjApptService");
                    OnObjApptServiceChanged();
                }
            }
        }
        private ApptService _ObjApptService;
        partial void OnObjApptServiceChanging(ApptService value);
        partial void OnObjApptServiceChanged();
        
        #endregion
        /*TMA*/
        [Required(ErrorMessage = "Chọn Loại Hình!")]
        [DataMemberAttribute()]
        public Int64 V_Surgery_Tips_Type
        {
            get { return _V_Surgery_Tips_Type; }
            set
            {
                if (
                    _V_Surgery_Tips_Type != value)
                {
                    OnV_Surgery_Tips_TypeChanging(value);
                    ValidateProperty("V_Surgery_Tips_Type", value);
                    _V_Surgery_Tips_Type = value;
                    RaisePropertyChanged("V_Surgery_Tips_Type");
                    OnV_Surgery_Tips_TypeChanged();
                }
            }
        }
        private Int64 _V_Surgery_Tips_Type;
        partial void OnV_Surgery_Tips_TypeChanging(Int64 value);
        partial void OnV_Surgery_Tips_TypeChanged();

        [Required(ErrorMessage = "")]
        [DataMemberAttribute()]
        public Int64 V_Surgery_Tips_Item
        {
            get { return _V_Surgery_Tips_Item; }
            set
            {
                if (
                    _V_Surgery_Tips_Item != value)
                {
                    OnV_Surgery_Tips_ItemChanging(value);
                    ValidateProperty("V_Surgery_Tips_Item", value);
                    _V_Surgery_Tips_Item = value;
                    RaisePropertyChanged("V_Surgery_Tips_Item");
                    OnV_Surgery_Tips_ItemChanged();
                }
            }
        }
        private Int64 _V_Surgery_Tips_Item;
        partial void OnV_Surgery_Tips_ItemChanging(Int64 value);
        partial void OnV_Surgery_Tips_ItemChanged();
        /*TMA*/
    }

}
