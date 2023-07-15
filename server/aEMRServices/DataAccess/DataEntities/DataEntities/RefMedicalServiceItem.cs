using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel.DataAnnotations;
/*
 * 20180508 #001 TBLD: Added HIApproved
 * 20210803 #002 TNHX: Added NgoaiDinhSuat/ V_DVKTPhanTuyen/ UseAnalgesic
 * 20221007 #003 TNHX: Added IsAllowDrugHIForOutPatient
 * 20230327 #004 BLQ: Thêm check pp sinh và check có thiết bị
*/
namespace DataEntities
{
    public partial class RefMedicalServiceItem : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefMedicalServiceItem object.

        /// <param name="medServiceID">Initial value of the MedServiceID property.</param>
        /// <param name="medServiceCode">Initial value of the MedServiceCode property.</param>
        /// <param name="medServiceName">Initial value of the MedServiceName property.</param>
        /// <param name="v_TimeUnit">Initial value of the V_TimeUnit property.</param>
        /// <param name="hIPrice">Initial value of the HIPrice property.</param>
        /// <param name="childrenUnderSixPrice">Initial value of the ChildrenUnderSixPrice property.</param>
        /// <param name="hIAllowedPrice">Initial value of the HIAllowedPrice property.</param>
        /// <param name="effectiveDate">Initial value of the EffectiveDate property.</param>
        public static RefMedicalServiceItem CreateRefMedicalServiceItem(long medServiceID, String medServiceCode, String medServiceName, Int64 v_TimeUnit, Decimal hIPrice, Decimal childrenUnderSixPrice, Decimal hIAllowedPrice, DateTime effectiveDate)
        {
            RefMedicalServiceItem refMedicalServiceItem = new RefMedicalServiceItem();
            refMedicalServiceItem.MedServiceID = medServiceID;
            refMedicalServiceItem.MedServiceCode = medServiceCode;
            refMedicalServiceItem.MedServiceName = medServiceName;
            refMedicalServiceItem.ChildrenUnderSixPrice = childrenUnderSixPrice;
            refMedicalServiceItem.HIAllowedPrice = hIAllowedPrice;
            refMedicalServiceItem.ExpiryDate = effectiveDate;
            return refMedicalServiceItem;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public long MedServiceID
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
                    ////ReportPropertyChanging("MedServiceID");
                    _MedServiceID = value;
                    RaisePropertyChanged("MedServiceID");
                    OnMedServiceIDChanged();
                }
            }
        }
        private long _MedServiceID;
        partial void OnMedServiceIDChanging(long value);
        partial void OnMedServiceIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
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
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Staff ObjStaff
        {
            get { return _ObjStaff; }
            set
            {
                if (_ObjStaff != value)
                {
                    OnObjStaffChanging(value);
                    _ObjStaff = value;
                    RaisePropertyChanged("ObjStaff");
                    OnObjStaffChanged();
                }
            }
        }
        private Staff _ObjStaff;
        partial void OnObjStaffChanging(Staff value);
        partial void OnObjStaffChanged();



        [DataMemberAttribute()]
        public Nullable<long> DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                ////ReportPropertyChanging("DeptID");
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private Nullable<long> _DeptID;
        partial void OnDeptIDChanging(Nullable<long> value);
        partial void OnDeptIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> MedicalServiceTypeID
        {
            get
            {
                return _MedicalServiceTypeID;
            }
            set
            {
                OnMedicalServiceTypeIDChanging(value);
                _MedicalServiceTypeID = value;
                RaisePropertyChanged("MedicalServiceTypeID");
                OnMedicalServiceTypeIDChanged();
            }
        }
        private Nullable<long> _MedicalServiceTypeID;
        partial void OnMedicalServiceTypeIDChanging(Nullable<long> value);
        partial void OnMedicalServiceTypeIDChanged();

        [DataMemberAttribute()]
        private RefMedicalServiceType _ObjMedicalServiceTypeID;
        public RefMedicalServiceType ObjMedicalServiceTypeID
        {
            get { return _ObjMedicalServiceTypeID; }
            set
            {
                OnObjMedicalServiceTypeIDChanging(value);
                _ObjMedicalServiceTypeID = value;
                RaisePropertyChanged("ObjMedicalServiceTypeID");
                OnObjMedicalServiceTypeIDChanged();
            }
        }
        partial void OnObjMedicalServiceTypeIDChanging(RefMedicalServiceType value);
        partial void OnObjMedicalServiceTypeIDChanged();



        [DataMemberAttribute()]
        public String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                OnDescriptionChanging(value);
                ////ReportPropertyChanging("Description");
                _Description = value;
                RaisePropertyChanged("Description");
                OnDescriptionChanged();
            }
        }
        private string _Description = "";
        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();


        [DataMemberAttribute()]
        public Nullable<long> PartnerShipID
        {
            get
            {
                return _PartnerShipID;
            }
            set
            {
                OnPartnerShipIDChanging(value);
                ////ReportPropertyChanging("PartnerShipID");
                _PartnerShipID = value;
                RaisePropertyChanged("PartnerShipID");
                OnPartnerShipIDChanged();
            }
        }
        private Nullable<long> _PartnerShipID;
        partial void OnPartnerShipIDChanging(Nullable<long> value);
        partial void OnPartnerShipIDChanged();




        [Required(ErrorMessage = "Nhập Mã Dịch Vụ!")]
        [DataMemberAttribute()]
        public String MedServiceCode
        {
            get
            {
                return _MedServiceCode;
            }
            set
            {
                OnMedServiceCodeChanging(value);
                ValidateProperty("MedServiceCode", value);
                _MedServiceCode = value;
                RaisePropertyChanged("MedServiceCode");
                OnMedServiceCodeChanged();
            }
        }
        private String _MedServiceCode;
        partial void OnMedServiceCodeChanging(String value);
        partial void OnMedServiceCodeChanged();




        [Required(ErrorMessage = "Nhập Tên Dịch Vụ!")]
        [DataMemberAttribute()]
        public String MedServiceName
        {
            get
            {
                return _MedServiceName;
            }
            set
            {
                OnMedServiceNameChanging(value);
                ValidateProperty("MedServiceName", value);
                _MedServiceName = value;
                RaisePropertyChanged("MedServiceName");
                OnMedServiceNameChanged();
            }
        }
        private String _MedServiceName;
        partial void OnMedServiceNameChanging(String value);
        partial void OnMedServiceNameChanged();


        //[DataMemberAttribute()]
        //public Int64? V_TimeUnit
        //{
        //    get
        //    {
        //        return _V_TimeUnit;
        //    }
        //    set
        //    {
        //        OnV_TimeUnitChanging(value);
        //        ////ReportPropertyChanging("V_TimeUnit");
        //        _V_TimeUnit = value;
        //        RaisePropertyChanged("V_TimeUnit");
        //        OnV_TimeUnitChanged();
        //    }
        //}
        //private Int64? _V_TimeUnit;
        //partial void OnV_TimeUnitChanging(Int64? value);
        //partial void OnV_TimeUnitChanged();


        //[DataMemberAttribute()]        
        //public Lookup ObjV_TimeUnit
        //{
        //    get { return _ObjV_TimeUnit; }
        //    set 
        //    {
        //        if (_ObjV_TimeUnit != value)
        //        {
        //            OnObjV_TimeUnitChanging(value);
        //            _ObjV_TimeUnit = value;
        //            RaisePropertyChanged("ObjV_TimeUnit");
        //            OnObjV_TimeUnitChanged();
        //        }
        //    }
        //}
        //private Lookup _ObjV_TimeUnit;
        //partial void OnObjV_TimeUnitChanging(Lookup value);
        //partial void OnObjV_TimeUnitChanged();


        [DataMemberAttribute]
        public Location ObjLocation
        {
            get { return _ObjLocation; }
            set
            {
                if (_ObjLocation != value)
                {
                    OnObjLocationChanging(value);
                    _ObjLocation = value;
                    RaisePropertyChanged("ObjLocation");
                    OnObjLocationChanged();
                }
            }
        }
        private Location _ObjLocation;
        partial void OnObjLocationChanging(Location value);
        partial void OnObjLocationChanged();


        [DataMemberAttribute()]
        public Nullable<Double> VATRate
        {
            get
            {
                return _VATRate;
            }
            set
            {
                OnVATRateChanging(value);
                ////ReportPropertyChanging("VATRate");
                _VATRate = value;
                RaisePropertyChanged("VATRate");
                OnVATRateChanged();
            }
        }
        private Nullable<Double> _VATRate;
        partial void OnVATRateChanging(Nullable<Double> value);
        partial void OnVATRateChanged();



        [DataMemberAttribute()]
        public Decimal ChildrenUnderSixPrice
        {
            get
            {
                return _ChildrenUnderSixPrice;
            }
            set
            {
                OnChildrenUnderSixPriceChanging(value);
                ////ReportPropertyChanging("ChildrenUnderSixPrice");
                _ChildrenUnderSixPrice = value;
                RaisePropertyChanged("ChildrenUnderSixPrice");
                OnChildrenUnderSixPriceChanged();
            }
        }
        private Decimal _ChildrenUnderSixPrice;
        partial void OnChildrenUnderSixPriceChanging(Decimal value);
        partial void OnChildrenUnderSixPriceChanged();

        [DataMemberAttribute()]
        public Decimal PriceForHIPatient
        {
            get
            {
                return _PriceForHIPatient;
            }
            set
            {
                OnPriceForHIPatientChanging(value);
                ////ReportPropertyChanging("PriceForHIPatient");
                _PriceForHIPatient = value;
                RaisePropertyChanged("PriceForHIPatient");
                OnPriceForHIPatientChanged();
            }
        }
        private Decimal _PriceForHIPatient;
        partial void OnPriceForHIPatientChanging(Decimal value);
        partial void OnPriceForHIPatientChanged();



        [Required(ErrorMessage = "Nhập Ngày Hết Hạn!")]
        [DataMemberAttribute()]
        public Nullable<DateTime> ExpiryDate
        {
            get
            {
                return _expiryDate;
            }
            set
            {
                ValidateProperty("ExpiryDate", value);
                _expiryDate = value;
                RaisePropertyChanged("ExpiryDate");
            }
        }
        private Nullable<DateTime> _expiryDate;

        [DataMemberAttribute()]
        public Nullable<Boolean> IsExpiredDate
        {
            get
            {
                return _isExpiredDate;
            }
            set
            {
                _isExpiredDate = value;
                RaisePropertyChanged("IsExpiredDate");
            }
        }
        private Nullable<Boolean> _isExpiredDate;





        [DataMemberAttribute()]
        public Nullable<Boolean> ByRequest
        {
            get
            {
                return _ByRequest;
            }
            set
            {
                OnByRequestChanging(value);
                ////ReportPropertyChanging("ByRequest");
                _ByRequest = value;
                RaisePropertyChanged("ByRequest");
                OnByRequestChanged();
            }
        }
        private Nullable<Boolean> _ByRequest;
        partial void OnByRequestChanging(Nullable<Boolean> value);
        partial void OnByRequestChanged();


        private Nullable<bool> _IsActive = false;
        public Nullable<bool> IsActive
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
        partial void OnIsActiveChanging(Nullable<Boolean> value);
        partial void OnIsActiveChanged();

        private bool _IsDeleted;
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
        partial void OnIsDeletedChanging(Boolean value);
        partial void OnIsDeletedChanged();


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


        [DataMemberAttribute()]
        public Lookup ObjV_RefMedServiceItemsUnit
        {
            get { return _ObjV_RefMedServiceItemsUnit; }
            set
            {
                if (_ObjV_RefMedServiceItemsUnit != value)
                {
                    OnObjV_RefMedServiceItemsUnitChanging(value);
                    _ObjV_RefMedServiceItemsUnit = value;
                    RaisePropertyChanged("ObjV_RefMedServiceItemsUnit");
                    if (ObjV_RefMedServiceItemsUnit != null)
                    {
                        V_RefMedServiceItemsUnit = ObjV_RefMedServiceItemsUnit.LookupID;
                    }
                    OnObjV_RefMedServiceItemsUnitChanged();
                }
            }
        }
        private Lookup _ObjV_RefMedServiceItemsUnit;
        partial void OnObjV_RefMedServiceItemsUnitChanging(Lookup value);
        partial void OnObjV_RefMedServiceItemsUnitChanged();



        [DataMemberAttribute()]
        public Nullable<Byte> ServiceMainTime
        {
            get
            {
                return _ServiceMainTime;
            }
            set
            {
                OnServiceMainTimeChanging(value);
                ////ReportPropertyChanging("ServiceMainTime");
                _ServiceMainTime = value;
                RaisePropertyChanged("ServiceMainTime");
                OnServiceMainTimeChanged();
            }
        }
        private Nullable<Byte> _ServiceMainTime;
        partial void OnServiceMainTimeChanging(Nullable<Byte> value);
        partial void OnServiceMainTimeChanged();


        // Hpt 11/11/2015: Thêm các thuộc tính hỗ trợ phân loại giá dịch vụ
        [Required(ErrorMessage = "Chọn Loại giá dịch vụ!")]
        [DataMemberAttribute()]
        public Int32 V_NewPriceType
        {
            get { return _V_NewPriceType; }
            set
            {
                if (_V_NewPriceType != value)
                {
                    OnV_NewPriceTypeChanging(value);
                    ValidateProperty("V_NewPriceType", value);
                    _V_NewPriceType = value;
                    RaisePropertyChanged("V_NewPriceType");
                    OnV_NewPriceTypeChanged();
                }
            }
        }
        private Int32 _V_NewPriceType;
        partial void OnV_NewPriceTypeChanging(Int32 value);
        partial void OnV_NewPriceTypeChanged();


        [DataMemberAttribute()]
        public bool IsPackageService
        {
            get { return _IsPackageService; }
            set
            {
                if (_IsPackageService != value)
                {
                    _IsPackageService = value;
                    RaisePropertyChanged("IsPackageService");
                }
            }
        }
        private bool _IsPackageService;

        [DataMemberAttribute()]
        private long _PtRegDetailID;
        public long PtRegDetailID
        {
            get { return _PtRegDetailID; }
            set
            {
                if (_PtRegDetailID != value)
                {
                    _PtRegDetailID = value;
                    RaisePropertyChanged("PtRegDetailID");
                }
            }
        }

        [DataMemberAttribute()]
        private long _V_CatastropheType;
        public long V_CatastropheType
        {
            get { return _V_CatastropheType; }
            set
            {
                if (_V_CatastropheType != value)
                {
                    _V_CatastropheType = value;
                    RaisePropertyChanged("V_CatastropheType");
                }
            }
        }
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Nullable<Int64> HITTypeID
        {
            get
            {
                return _HITTypeID;
            }
            set
            {
                if (_HITTypeID != value)
                {
                    OnHITTypeIDChanging(value);
                    _HITTypeID = value;
                    RaisePropertyChanged("HITTypeID");
                    OnHITTypeIDChanged();
                }
            }
        }
        private Nullable<Int64> _HITTypeID;
        partial void OnHITTypeIDChanging(Nullable<Int64> value);
        partial void OnHITTypeIDChanged();


        [DataMemberAttribute()]
        public HITransactionType ObjHITTypeID
        {
            get
            {
                return _ObjHITTypeID;
            }
            set
            {
                if (_ObjHITTypeID != value)
                {
                    OnObjHITTypeIDChanging(value);
                    _ObjHITTypeID = value;
                    RaisePropertyChanged("ObjHITTypeID");
                    OnObjHITTypeIDChanged();
                }
            }
        }
        private HITransactionType _ObjHITTypeID;
        partial void OnObjHITTypeIDChanging(HITransactionType value);
        partial void OnObjHITTypeIDChanged();


        //
        public ObservableCollection<DeptLocation> allDeptLocation
        {
            get
            {
                return _allDeptLocation;
            }
            set
            {
                if (_allDeptLocation != value)
                {
                    OnallDeptLocationChanging(value);
                    _allDeptLocation = value;
                    RaisePropertyChanged("allDeptLocation");
                    OnallDeptLocationChanged();
                }
            }
        }
        private ObservableCollection<DeptLocation> _allDeptLocation;
        partial void OnallDeptLocationChanging(ObservableCollection<DeptLocation> value);
        partial void OnallDeptLocationChanged();


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_BEDLOCAT_REL_RM30_REFMEDIC", "BedLocation")]
        public ObservableCollection<BedLocation> BedLocations
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_BLOODDON_REL_BB01_REFMEDIC", "BloodDonations")]
        public ObservableCollection<BloodDonation> BloodDonations
        {
            get;
            set;
        }


        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_CHAINMED_REL_HOSFM_REFMEDIC", "ChainMedicalServices")]
        public ObservableCollection<ChainMedicalService> ChainMedicalServices
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HISERVIC_REL_HOSFM_REFMEDIC", "HIServices")]
        public ObservableCollection<HIService> HIServices
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTR_REL_PTINF_REFMEDIC", "PatientRegistrationDetails")]
        public ObservableCollection<PatientRegistrationDetail> PatientRegistrationDetails
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFMEDIC_REL_HOSFM_PCLEXAMT", "PCLExamTypes")]
        public PCLExamType PCLExamType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFMEDIC_REL_HOSFM_REFDEPAR", "RefDepartments")]
        public RefDepartment RefDepartment
        {
            get;
            set;
        }

        private RefMedicalServiceType _refMedicalServiceType;

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFMEDIC_REL_HOSFM_REFMEDIC", "RefMedicalServiceTypes")]
        public RefMedicalServiceType RefMedicalServiceType
        {
            get { return _refMedicalServiceType; }
            set
            {
                _refMedicalServiceType = value;
                RaisePropertyChanged("RefMedicalServiceType");
                if (_refMedicalServiceType != null)
                {
                    MedicalServiceTypeID = _refMedicalServiceType.MedicalServiceTypeID;
                }
            }
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFMEDIC_REL_HOSFM_RESEARCH", "ResearchPartnerShip")]
        public ResearchPartnerShip ResearchPartnerShip
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_RESOURCE_REL_RM09_REFMEDIC", "ResourcesForMedicalServices")]
        public ObservableCollection<ResourcesForMedicalService> ResourcesForMedicalServices
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SCHEDULE_REL_PTAPP_REFMEDIC", "ScheduledJob")]
        public ObservableCollection<ScheduledJob> ScheduledJobs
        {
            get;
            set;
        }



        #endregion

        public string GetCode()
        {
            return _MedServiceCode;
        }

        public override string ToString()
        {
            return _MedServiceName;
        }

        #region For check choose from dataList with checkbox
        [DataMemberAttribute()]
        public Nullable<Boolean> IsCheckedInDataList
        {
            get
            {
                return _IsCheckedInDataList;
            }
            set
            {
                if (_IsCheckedInDataList != value)
                {
                    OnIsCheckedInDataListChanging(value);
                    _IsCheckedInDataList = value;
                    RaisePropertyChanged("IsCheckedInDataList");
                    OnIsCheckedInDataListChanged();
                }
            }
        }
        private Nullable<Boolean> _IsCheckedInDataList;
        partial void OnIsCheckedInDataListChanging(Nullable<Boolean> value);
        partial void OnIsCheckedInDataListChanged();
        #endregion


        #region Config Cho Phép Hẹn Bệnh
        [DataMemberAttribute()]
        public Nullable<Int64> V_AppointmentType
        {
            get
            {
                return _V_AppointmentType;
            }
            set
            {
                if (_V_AppointmentType != value)
                {
                    OnV_AppointmentTypeChanging(value);
                    _V_AppointmentType = value;
                    RaisePropertyChanged("V_AppointmentType");
                    OnV_AppointmentTypeChanged();
                }
            }
        }
        private Nullable<Int64> _V_AppointmentType = 0;
        partial void OnV_AppointmentTypeChanging(Nullable<Int64> value);
        partial void OnV_AppointmentTypeChanged();
        #endregion

        //Hpt 24/08/2016: Vv Cho phép người dùng điều chỉnh mã dùng chung theo công văn 5084
        private string _HICode5084;
        public string HICode5084
        {
            get
            {
                return _HICode5084;
            }
            set
            {
                _HICode5084 = value;
                RaisePropertyChanged("HICode5084");
            }
        }

        private int _IsAllowToPayAfter;
        [DataMemberAttribute()]
        public int IsAllowToPayAfter
        {
            get
            {
                return _IsAllowToPayAfter;
            }
            set
            {
                if (_IsAllowToPayAfter != value)
                {
                    _IsAllowToPayAfter = value;
                    RaisePropertyChanged("IsAllowToPayAfter");
                }
            }
        }

        private string _MedServiceName_Ax;
        public string MedServiceName_Ax
        {
            get
            {
                return _MedServiceName_Ax;
            }
            set
            {
                _MedServiceName_Ax = value;
                RaisePropertyChanged("MedServiceName_Ax");
            }
        }

        private string _HICode;
        public string HICode
        {
            get
            {
                return _HICode;
            }
            set
            {
                _HICode = value;
                RaisePropertyChanged("HICode");
            }
        }
        private Nullable<DateTime> _UpdatedTime;
        public Nullable<DateTime> UpdatedTime
        {
            get
            {
                return _UpdatedTime;
            }
            set
            {
                _UpdatedTime = value;
                RaisePropertyChanged("UpdatedTime");
            }
        }

        private Int64 _UpdatedStaffID;
        public Int64 UpdatedStaffID
        {
            get
            {
                return _UpdatedStaffID;
            }
            set
            {
                _UpdatedStaffID = value;
                RaisePropertyChanged("UpdatedStaffID");
            }
        }
        /*TMA*/
        [Required(ErrorMessage = "Phẫu thuật - Thủ thuật")]
        [DataMemberAttribute()]
        public Int64 V_Surgery_Tips_Item
        {
            get { return _V_Surgery_Tips_Item; }
            set
            {
                if (_V_Surgery_Tips_Item != value)
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


        [DataMemberAttribute()]
        public Lookup ObjV_Surgery_Tips_Item
        {
            get { return _ObjV_Surgery_Tips_Item; }
            set
            {
                if (_ObjV_Surgery_Tips_Item != value)
                {
                    OnObjV_Surgery_Tips_ItemChanging(value);
                    _ObjV_Surgery_Tips_Item = value;
                    RaisePropertyChanged("ObjV_Surgery_Tips_Item");
                    if (ObjV_Surgery_Tips_Item != null)
                    {
                        V_Surgery_Tips_Item = ObjV_Surgery_Tips_Item.LookupID;
                    }
                    OnObjV_Surgery_Tips_ItemChanged();
                }
            }
        }
        private Lookup _ObjV_Surgery_Tips_Item;
        partial void OnObjV_Surgery_Tips_ItemChanging(Lookup value);
        partial void OnObjV_Surgery_Tips_ItemChanged();


        [Required(ErrorMessage = "Chọn Loại Hình Phẫu -Thủ Thuật!")]
        [DataMemberAttribute()]
        public Int64 V_Surgery_Tips_Type
        {
            get { return _V_Surgery_Tips_Type; }
            set
            {
                if (_V_Surgery_Tips_Type != value)
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


        [DataMemberAttribute()]
        public Lookup ObjV_Surgery_Tips_Type
        {
            get { return _ObjV_Surgery_Tips_Type; }
            set
            {
                if (_ObjV_Surgery_Tips_Type != value)
                {
                    OnObjV_Surgery_Tips_TypeChanging(value);
                    _ObjV_Surgery_Tips_Type = value;
                    RaisePropertyChanged("ObjV_Surgery_Tips_Type");
                    if (ObjV_Surgery_Tips_Type != null)
                    {
                        V_Surgery_Tips_Type = ObjV_Surgery_Tips_Type.LookupID;
                    }
                    OnObjV_Surgery_Tips_TypeChanged();
                }
            }
        }
        private Lookup _ObjV_Surgery_Tips_Type;
        partial void OnObjV_Surgery_Tips_TypeChanging(Lookup value);
        partial void OnObjV_Surgery_Tips_TypeChanged();
        /*TMA*/
        /*▼====: #001*/
        [DataMemberAttribute()]
        public Nullable<Boolean> HIApproved
        {
            get
            {
                return _HIApproved;
            }
            set
            {
                OnHIApprovedChanging(value);
                _HIApproved = value;
                RaisePropertyChanged("HIApproved");
                OnHIApprovedChanged();
            }
        }
        private Nullable<Boolean> _HIApproved = false;
        partial void OnHIApprovedChanging(Nullable<Boolean> value);
        partial void OnHIApprovedChanged();
        /*▲====: #001*/

        [DataMemberAttribute()]
        public decimal HIPayRatio
        {
            get
            {
                return _HIPayRatio;
            }
            set
            {
                _HIPayRatio = value;
                RaisePropertyChanged("HIPayRatio");
            }
        }
        private decimal _HIPayRatio = 1;

        private bool _IsMedicalExamination;
        [DataMemberAttribute]
        public bool IsMedicalExamination
        {
            get
            {
                return _IsMedicalExamination;
            }
            set
            {
                _IsMedicalExamination = value;
                RaisePropertyChanged("IsMedicalExamination");
            }
        }
        private Int64 _V_SpecialistType;
        [DataMemberAttribute]
        public Int64 V_SpecialistType
        {
            get
            {
                return _V_SpecialistType;
            }
            set
            {
                if (_V_SpecialistType != value)
                {
                    _V_SpecialistType = value;
                    RaisePropertyChanged("V_SpecialistType");
                }
            }
        }
        private string _GenderType;
        [DataMemberAttribute]
        public string GenderType
        {
            get { return _GenderType; }
            set
            {
                if (_GenderType != value)
                {
                    _GenderType = value;
                    RaisePropertyChanged("GenderType");
                }
            }
        }

        //▼====: #002
        private long? _V_DVKTPhanTuyen;
        [DataMemberAttribute]
        public long? V_DVKTPhanTuyen
        {
            get { return _V_DVKTPhanTuyen; }
            set
            {
                if (_V_DVKTPhanTuyen != value)
                {
                    _V_DVKTPhanTuyen = value;
                    RaisePropertyChanged("V_DVKTPhanTuyen");
                }
            }
        }

        private Lookup _ObjV_DVKTPhanTuyen;
        [DataMemberAttribute]
        public Lookup ObjV_DVKTPhanTuyen
        {
            get { return _ObjV_DVKTPhanTuyen; }
            set
            {
                if (_ObjV_DVKTPhanTuyen != value)
                {
                    _ObjV_DVKTPhanTuyen = value;
                    RaisePropertyChanged("ObjV_DVKTPhanTuyen");
                }
            }
        }

        private bool _NgoaiDinhSuat;
        [DataMemberAttribute]
        public bool NgoaiDinhSuat
        {
            get { return _NgoaiDinhSuat; }
            set
            {
                if (_NgoaiDinhSuat != value)
                {
                    _NgoaiDinhSuat = value;
                    RaisePropertyChanged("NgoaiDinhSuat");
                }
            }
        }

        private bool _UseAnalgesic;
        [DataMemberAttribute]
        public bool UseAnalgesic
        {
            get { return _UseAnalgesic; }
            set
            {
                if (_UseAnalgesic != value)
                {
                    _UseAnalgesic = value;
                    RaisePropertyChanged("UseAnalgesic");
                }
            }
        }
        //▲====: #002
        private bool _InCategoryCOVID;
        [DataMemberAttribute]
        public bool InCategoryCOVID
        {
            get { return _InCategoryCOVID; }
            set
            {
                if (_InCategoryCOVID != value)
                {
                    _InCategoryCOVID = value;
                    RaisePropertyChanged("InCategoryCOVID");
                }
            }
        }

        //▼====: #003
        private bool _IsAllowDrugHIForOutPatient;
        [DataMemberAttribute]
        public bool IsAllowDrugHIForOutPatient
        {
            get { return _IsAllowDrugHIForOutPatient; }
            set
            {
                if (_IsAllowDrugHIForOutPatient != value)
                {
                    _IsAllowDrugHIForOutPatient = value;
                    RaisePropertyChanged("IsAllowDrugHIForOutPatient");
                }
            }
        }
        //▲====: #003
        //▼====: #004
        private bool _IsBirthMethod;
        [DataMemberAttribute]
        public bool IsBirthMethod
        {
            get { return _IsBirthMethod; }
            set
            {
                if (_IsBirthMethod != value)
                {
                    _IsBirthMethod = value;
                    RaisePropertyChanged("IsBirthMethod");
                }
            }
        }
        private bool _IsHaveEquip;
        [DataMemberAttribute]
        public bool IsHaveEquip
        {
            get { return _IsHaveEquip; }
            set
            {
                if (_IsHaveEquip != value)
                {
                    _IsHaveEquip = value;
                    RaisePropertyChanged("IsHaveEquip");
                }
            }
        }
        //▲====: #004
    }
}
