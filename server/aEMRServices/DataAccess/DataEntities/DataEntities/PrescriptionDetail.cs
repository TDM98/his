using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

/*
 * 20180920 #001 TBL:   Chinh sua chuc nang bug mantis ID 0000061, kiem tra tung property trong dataentities 
 *                      neu co su thay doi IsDataChanged = true
 * 20181012 #002 TTM:   Create new some Property: AppoimentID, IssueID, CanEdit, ReasonCanEdit, IsEdit
 * 20181101 #003 TTM:   Bổ sung thêm RefGenDrugCatID_1.
 */
namespace DataEntities
{
    public partial class PrescriptionDetail : EntityBase
    {

        public bool IsDataChanged { get; set; }

        #region Factory Method

        public PrescriptionDetail() 
        {
            SelectedDrugForPrescription = new GetDrugForSellVisitor();
            ObjPrescriptionDetailSchedules = new ObservableCollection<PrescriptionDetailSchedules>();

        }

        /// Create a new PrescriptionDetail object.

        /// <param name="prescriptDetailID">Initial value of the PrescriptDetailID property.</param>
        /// <param name="strength">Initial value of the Strength property.</param>
        /// <param name="mDose">Initial value of the MDose property.</param>
        /// <param name="v_Units">Initial value of the V_Units property.</param>
        /// <param name="dayRpts">Initial value of the DayRpts property.</param>
        /// <param name="qty">Initial value of the Qty property.</param>
        public static PrescriptionDetail CreatePrescriptionDetail(long prescriptDetailID, String strength, Single mDose, Int64 v_Units, double dayRpts, double qty)
        {
            PrescriptionDetail prescriptionDetail = new PrescriptionDetail();
            prescriptionDetail.PrescriptDetailID = prescriptDetailID;
            prescriptionDetail.Strength = strength;
            prescriptionDetail.MDose = mDose;
            prescriptionDetail.V_Units = v_Units;
            prescriptionDetail.DayRpts = dayRpts;
            prescriptionDetail.Qty = qty;
            return prescriptionDetail;
        }

        #endregion

        #region Primitive Properties
        /*▼====: #001*/
        private bool _IsObjectBeingUsedByClient = false;
        [DataMemberAttribute()]
        public bool IsObjectBeingUsedByClient
        {
            get
            {
                return _IsObjectBeingUsedByClient;
            }
            set
            {
                if (_IsObjectBeingUsedByClient != value)
                {
                    _IsObjectBeingUsedByClient = value;
                    RaisePropertyChanged("IsObjectBeingUsedByClient");
                }
            }
        }
        /*▲====: #001*/
        [DataMemberAttribute()]
        public long PrescriptDetailID
        {
            get
            {
                return _PrescriptDetailID;
            }
            set
            {
                if (_PrescriptDetailID != value)
                {
                    OnPrescriptDetailIDChanging(value);
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _PrescriptDetailID != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _PrescriptDetailID = value;
                    RaisePropertyChanged("PrescriptDetailID");
                    OnPrescriptDetailIDChanged();
                }
            }
        }
        private long _PrescriptDetailID;
        partial void OnPrescriptDetailIDChanging(long value);
        partial void OnPrescriptDetailIDChanged();

        [Required(ErrorMessage = "Bạn phải chọn thuốc")]
        [DataMemberAttribute()]
        public Nullable<long> DrugID
        {
            get
            {
                return _DrugID;
            }
            set
            {
                OnDrugIDChanging(value);
                ValidateProperty("DrugID", value);
                _DrugID = value;
                RaisePropertyChanged("DrugID");
                OnDrugIDChanged();
            }
        }
        private Nullable<long> _DrugID;
        partial void OnDrugIDChanging(Nullable<long> value);
        partial void OnDrugIDChanged();


        


        [DataMemberAttribute()]
        public long PrescriptID
        {
            get
            {
                return _PrescriptID;
            }
            set
            {
                OnPrescriptIDChanging(value);
                _PrescriptID = value;
                RaisePropertyChanged("PrescriptID");
                OnPrescriptIDChanged();
            }
        }
        private long _PrescriptID;
        partial void OnPrescriptIDChanging(long value);
        partial void OnPrescriptIDChanged();


        [DataMemberAttribute()]
        public String Strength
        {
            get
            {
                return _Strength;
            }
            set
            {
                OnStrengthChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _Strength != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _Strength = value;
                RaisePropertyChanged("Strength");
                OnStrengthChanged();
            }
        }
        private String _Strength;
        partial void OnStrengthChanging(String value);
        partial void OnStrengthChanged();

        [DataMemberAttribute()]
        public Boolean BeOfHIMedicineList
        {
            get
            {
                return _BeOfHIMedicineList;
            }
            set
            {
                OnBeOfHIMedicineListChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _BeOfHIMedicineList != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _BeOfHIMedicineList = value;
                if (_BeOfHIMedicineList)
                {
                    IsDrugNotInCat = false;
                }
                RaisePropertyChanged("BeOfHIMedicineList");
                OnBeOfHIMedicineListChanged();
            }
        }
        private Boolean _BeOfHIMedicineList;
        partial void OnBeOfHIMedicineListChanging(Boolean value);
        partial void OnBeOfHIMedicineListChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Int64> V_DrugUsage
        {
            get
            {
                return _V_DrugUsage;
            }
            set
            {
                OnV_DrugUsageChanging(value);
                ValidateProperty("V_DrugUsage", value);
                _V_DrugUsage = value;
                RaisePropertyChanged("V_DrugUsage");
                OnV_DrugUsageChanged();
            }
        }
        private Nullable<Int64> _V_DrugUsage;
        partial void OnV_DrugUsageChanging(Nullable<Int64> value);
        partial void OnV_DrugUsageChanged();

        
        [DataMemberAttribute()]
        public Int64 V_DrugType
        {
            get
            {
                return _V_DrugType;
            }
            set
            {
                OnV_DrugTypeChanging(value);
                ValidateProperty("V_DrugType", value);
                _V_DrugType = value;
                RaisePropertyChanged("V_DrugType");
                switch (_V_DrugType)
                {
                    case (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG:                        
                        isHICheck = true;
                        isEditDosage = true;
                        BackGroundColor = "#F8F8F8";
                    break;

                    case (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN:                        
                        isHICheck = true;
                        isEditDosage = false;
                    break;
                    
                    case (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN:                        
                        isHICheck = true;
                        isEditDosage = false;
                    break;
                }
                OnV_DrugTypeChanged();
            }
        }
        private Int64 _V_DrugType=(long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG;
        partial void OnV_DrugTypeChanging(Int64 value);
        partial void OnV_DrugTypeChanged();


        //
        // Txd 25/09/2013 Modified the following 4 Fields MDose, ADose, EDose and NDose to GET RID
        // of the Updating of QTY automatically by calling SetValueFollowNgayDung
        //

       // [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Single MDose
        {
            get
            {
                return _MDose;
            }
            set
            {
                OnMDoseChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _MDose != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _MDose = value;
                RaisePropertyChanged("MDose");
                OnMDoseChanged();
                
            }
        }
        private Single _MDose;
        partial void OnMDoseChanging(Single value);
        partial void OnMDoseChanged();

      //  [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> ADose
        {
            get
            {
                return _ADose;
            }
            set
            {
                OnADoseChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _ADose != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _ADose = value;
                RaisePropertyChanged("ADose");
                OnADoseChanged();
            }
        }
        private Nullable<Single> _ADose;
        partial void OnADoseChanging(Nullable<Single> value);
        partial void OnADoseChanged();

        //[Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> EDose
        {
            get
            {
                return _EDose;
            }
            set
            {
                OnEDoseChanging(value);
                // ValidateProperty("EDose", value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _EDose != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _EDose = value;
                RaisePropertyChanged("EDose");
                OnEDoseChanged();
            }
        }
        private Nullable<Single> _EDose;
        partial void OnEDoseChanging(Nullable<Single> value);
        partial void OnEDoseChanged();

       // [Range(0.0, 99999999999.0, ErrorMessage = "Liều dùng không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public Nullable<Single> NDose
        {
            get
            {
                return _NDose;
            }
            set
            {
                OnNDoseChanging(value);
                //  ValidateProperty("NDose", value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _NDose != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _NDose = value;
                RaisePropertyChanged("NDose");
                OnNDoseChanged();
            }
        }
        private Nullable<Single> _NDose;
        partial void OnNDoseChanging(Nullable<Single> value);
        partial void OnNDoseChanged();

        [DataMemberAttribute()]
        public Int64 V_Units
        {
            get
            {
                return _V_Units;
            }
            set
            {
                OnV_UnitsChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _V_Units != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _V_Units = value;
                RaisePropertyChanged("V_Units");
                OnV_UnitsChanged();
            }
        }
        private Int64 _V_Units;
        partial void OnV_UnitsChanging(Int64 value);
        partial void OnV_UnitsChanged();


        //
        // Txd 25/09/2013 Modified the following 4 Fields MDose, ADose, EDose and NDose to GET RID
        // of the Updating of QTY automatically by calling SetValueFollowNgayDung
        //

        [DataMemberAttribute()]
        public double DayRpts
        {
            get
            {
                return _DayRpts;
            }
            set
            {
                OnDayRptsChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _DayRpts != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _DayRpts = value;
                RaisePropertyChanged("DayRpts");
                OnDayRptsChanged();

            }
        }
        private double _DayRpts;
        partial void OnDayRptsChanging(double value);
        partial void OnDayRptsChanged();

        [DataMemberAttribute()]
        public double DayExtended
        {
            get
            {
                return _DayExtended;
            }
            set
            {
                OnDayExtendedChanging(value);
                _DayExtended = value;
                RaisePropertyChanged("DayExtended");
                OnDayExtendedChanged();

            }
        }
        private double _DayExtended;
        partial void OnDayExtendedChanging(double value);
        partial void OnDayExtendedChanged();

        [DataMemberAttribute()]
        public int RealDay
        {
            get
            {
                return _RealDay;
            }
            set
            {
                OnRealDayChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _RealDay != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _RealDay = value;
                RaisePropertyChanged("RealDay");
                OnRealDayChanged();
            }
        }
        private int _RealDay;
        partial void OnRealDayChanging(double value);
        partial void OnRealDayChanged();

        [Range(0.0, 99999999999.0, ErrorMessage = "Số lượng không được nhỏ hơn 0")]
        [DataMemberAttribute()]
        public double Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                if (_Qty != value)
                {
                    OnQtyChanging(value);
                    ValidateProperty("Qty", value);
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _Qty != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _Qty = value;
                    RaisePropertyChanged("Qty");
                    OnQtyChanged();
                }
            }
        }
        private double _Qty;
        partial void OnQtyChanging(double value);
        partial void OnQtyChanged();

        [DataMemberAttribute()]
        public double QtyMaxAllowed
        {
            get
            {
                return _QtyMaxAllowed;
            }
            set
            {
                if (_QtyMaxAllowed != value)
                {
                    OnQtyMaxAllowedChanging(value);
                    _QtyMaxAllowed = value;
                    RaisePropertyChanged("QtyMaxAllowed");
                    OnQtyMaxAllowedChanged();
                }
            }
        }
        private double _QtyMaxAllowed;
        partial void OnQtyMaxAllowedChanging(double value);
        partial void OnQtyMaxAllowedChanged();


        [DataMemberAttribute()]
        public Nullable<Single> QtyForDay
        {
            get
            {
                return _QtyForDay;
            }
            set
            {
                OnQtyForDayChanging(value);
                _QtyForDay = value;
                RaisePropertyChanged("QtyForDay");
                OnQtyForDayChanged();
            }
        }
        private Nullable<Single> _QtyForDay;
        partial void OnQtyForDayChanging(Nullable<Single> value);
        partial void OnQtyForDayChanged();


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedMon
        {
            get
            {
                return _QtySchedMon;
            }
            set
            {
                OnQtySchedMonChanging(value);
                _QtySchedMon = value;
                RaisePropertyChanged("QtySchedMon");
                OnQtySchedMonChanged();
            }
        }
        private Nullable<Single> _QtySchedMon;
        partial void OnQtySchedMonChanging(Nullable<Single> value);
        partial void OnQtySchedMonChanged();

        [DataMemberAttribute()]
        public Nullable<Single> QtySchedTue
        {
            get
            {
                return _QtySchedTue;
            }
            set
            {
                OnQtySchedTueChanging(value);
                _QtySchedTue = value;
                RaisePropertyChanged("QtySchedTue");
                OnQtySchedTueChanged();
            }
        }
        private Nullable<Single> _QtySchedTue;
        partial void OnQtySchedTueChanging(Nullable<Single> value);
        partial void OnQtySchedTueChanged();


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedWed
        {
            get
            {
                return _QtySchedWed;
            }
            set
            {
                OnQtySchedWedChanging(value);
                _QtySchedWed = value;
                RaisePropertyChanged("QtySchedWed");
                OnQtySchedWedChanged();
            }
        }
        private Nullable<Single> _QtySchedWed;
        partial void OnQtySchedWedChanging(Nullable<Single> value);
        partial void OnQtySchedWedChanged();


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedThu
        {
            get
            {
                return _QtySchedThu;
            }
            set
            {
                OnQtySchedThuChanging(value);
                _QtySchedThu = value;
                RaisePropertyChanged("QtySchedThu");
                OnQtySchedThuChanged();
            }
        }
        private Nullable<Single> _QtySchedThu;
        partial void OnQtySchedThuChanging(Nullable<Single> value);
        partial void OnQtySchedThuChanged();


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedFri
        {
            get
            {
                return _QtySchedFri;
            }
            set
            {
                OnQtySchedFriChanging(value);
                _QtySchedFri = value;
                RaisePropertyChanged("QtySchedFri");
                OnQtySchedFriChanged();
            }
        }
        private Nullable<Single> _QtySchedFri;
        partial void OnQtySchedFriChanging(Nullable<Single> value);
        partial void OnQtySchedFriChanged();


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedSat
        {
            get
            {
                return _QtySchedSat;
            }
            set
            {
                OnQtySchedSatChanging(value);
                _QtySchedSat = value;
                RaisePropertyChanged("QtySchedSat");
                OnQtySchedSatChanged();
            }
        }
        private Nullable<Single> _QtySchedSat;
        partial void OnQtySchedSatChanging(Nullable<Single> value);
        partial void OnQtySchedSatChanged();


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedSun
        {
            get
            {
                return _QtySchedSun;
            }
            set
            {
                OnQtySchedSunChanging(value);
                _QtySchedSun = value;
                RaisePropertyChanged("QtySchedSun");
                OnQtySchedSunChanged();
            }
        }
        private Nullable<Single> _QtySchedSun;
        partial void OnQtySchedSunChanging(Nullable<Single> value);
        partial void OnQtySchedSunChanged();


        [DataMemberAttribute()]
        public Nullable<byte> SchedBeginDOW
        {
            get
            {
                return _SchedBeginDOW;
            }
            set
            {
                OnSchedBeginDOWChanging(value);
                _SchedBeginDOW = value;
                RaisePropertyChanged("SchedBeginDOW");
                OnSchedBeginDOWChanged();
            }
        }
        private Nullable<byte> _SchedBeginDOW;
        partial void OnSchedBeginDOWChanging(Nullable<byte> value);
        partial void OnSchedBeginDOWChanged();



        [DataMemberAttribute()]
        public String V_DrugInstruction
        {
            get
            {
                return _V_DrugInstruction;
            }
            set
            {
                OnV_DrugInstructionChanging(value);
                _V_DrugInstruction = value;
                RaisePropertyChanged("V_DrugInstruction");
                OnV_DrugInstructionChanged();
            }
        }
        private String _V_DrugInstruction;
        partial void OnV_DrugInstructionChanging(String value);
        partial void OnV_DrugInstructionChanged();



        [DataMemberAttribute()]
        public String DrugInstructionNotes
        {
            get
            {
                return _DrugInstructionNotes;
            }
            set
            {
                OnDrugInstructionNotesChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _DrugInstructionNotes != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _DrugInstructionNotes = value;
                RaisePropertyChanged("DrugInstructionNotes");
                OnDrugInstructionNotesChanged();
            }
        }
        private String _DrugInstructionNotes;
        partial void OnDrugInstructionNotesChanging(String value);
        partial void OnDrugInstructionNotesChanged();

        [DataMemberAttribute()]
        public ObservableCollection<PrescriptionDetailSchedules> ObjPrescriptionDetailSchedules
        {
            get { return _ObjPrescriptionDetailSchedules; }
            set
            {
                if(_ObjPrescriptionDetailSchedules!=value)
                {
                    OnObjPrescriptionDetailSchedulesChanging(value);
                    _ObjPrescriptionDetailSchedules = value;
                    RaisePropertyChanged("ObjPrescriptionDetailSchedules");
                    OnObjPrescriptionDetailSchedulesChanged();
                }
            }
        }
        private ObservableCollection<PrescriptionDetailSchedules> _ObjPrescriptionDetailSchedules;
        partial void OnObjPrescriptionDetailSchedulesChanging(ObservableCollection<PrescriptionDetailSchedules> value);
        partial void OnObjPrescriptionDetailSchedulesChanged();


        [DataMemberAttribute()]
        public Boolean HasSchedules
        {
            get
            {
                return (V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN);
            }
            
        }

        //▼====== #002
        [DataMemberAttribute()]
        public long AppointmentID
        {
            get
            {
                return _AppointmentID;
            }
            set
            {
                if (_AppointmentID != value)
                {
                    OnAppointmentIDChanging(value);
                    _AppointmentID = value;
                    RaisePropertyChanged("AppointmentID");
                    OnAppointmentIDChanged();
                }
            }
        }
        private long _AppointmentID;
        partial void OnAppointmentIDChanging(double value);
        partial void OnAppointmentIDChanged();

        [DataMemberAttribute()]
        public long IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                if (_IssueID != value)
                {
                    OnIssueIDChanging(value);
                    _IssueID = value;
                    RaisePropertyChanged("IssueID");
                    OnIssueIDChanged();
                }
            }
        }
        private long _IssueID;
        partial void OnIssueIDChanging(double value);
        partial void OnIssueIDChanged();

        [DataMemberAttribute()]
        public override bool CanEdit
        {
            get
            {
                return _CanEdit;
            }
            set
            {
                OnCanEditChanging(value);
                _CanEdit = value;
                RaisePropertyChanged("CanEdit");
                OnCanEditChanged();
            }
        }
        private bool _CanEdit;
        partial void OnCanEditChanging(Boolean value);
        partial void OnCanEditChanged();

        [DataMemberAttribute()]
        public string ReasonCanEdit
        {
            get
            {
                return _ReasonCanEdit;
            }
            set
            {
                OnReasonCanEditChanging(value);
                _ReasonCanEdit = value;
                RaisePropertyChanged("ReasonCanEdit");
                OnReasonCanEditChanged();
            }
        }
        private string _ReasonCanEdit;
        partial void OnReasonCanEditChanging(String value);
        partial void OnReasonCanEditChanged();

        [DataMemberAttribute()]
        public bool IsAllowEditNDay
        {
            get
            {
                return _IsAllowEditNDay;
            }
            set
            {
                OnIsAllowEditNDayChanging(value);
                _IsAllowEditNDay = value;
                RaisePropertyChanged("IsAllowEditNDay");
                OnIsAllowEditNDayChanged();
            }
        }
        private bool _IsAllowEditNDay;
        partial void OnIsAllowEditNDayChanging(bool value);
        partial void OnIsAllowEditNDayChanged();
        //▲====== #002
        //▼====== #003
        [DataMemberAttribute()]
        public long? RefGenDrugCatID_1
        {
            get
            {
                return _RefGenDrugCatID_1;
            }
            set
            {
                _RefGenDrugCatID_1 = value;
                RaisePropertyChanged("RefGenDrugCatID_1");
            }
        }
        private long? _RefGenDrugCatID_1;
        //▲====== #003
        #endregion

        #region Navigation Properties


        private ObservableCollection<RefDisposableMedicalResource> _RefDisposableMedicalResources;
        [DataMemberAttribute()]
        public ObservableCollection<RefDisposableMedicalResource> RefDisposableMedicalResources
        {
            get
            {
                return _RefDisposableMedicalResources;
            }
            set
            {
                if (_RefDisposableMedicalResources != value)
                {
                    _RefDisposableMedicalResources = value;
                    RaisePropertyChanged("RefDisposableMedicalResources");
                }
            }
        }


        //Adding


        private Lookup _LookupDrugUsage;
        [DataMemberAttribute()]
        public Lookup LookupDrugUsage
        {
            get
            {
                return _LookupDrugUsage;
            }
            set
            {
                if (_LookupDrugUsage != value)
                {
                    _LookupDrugUsage = value;
                    RaisePropertyChanged("LookupDrugUsage");
                }
            }
        }


        private Lookup _LookupUnits;
        [DataMemberAttribute()]
        public Lookup LookupUnits
        {
            get
            {
                return _LookupUnits;
            }
            set
            {
                if (_LookupUnits != value)
                {
                    _LookupUnits = value;
                    RaisePropertyChanged("LookupUnits");
                }
            }
        }


        #endregion

        public override bool Equals(object obj)
        {
            PrescriptionDetail seletedStore = obj as PrescriptionDetail;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PrescriptDetailID == seletedStore.PrescriptDetailID;
        }

        #region IEditableObject Members
        private PrescriptionDetail _tempPrescriptionDetail;
        public void BeginEdit()
        {
            _tempPrescriptionDetail = (PrescriptionDetail)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPrescriptionDetail)
                CopyFrom(_tempPrescriptionDetail);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PrescriptionDetail p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region Thuộc tính cho bảng Ngoài danh mục
        [DataMemberAttribute()]
        public String BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                OnBrandNameChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _BrandName != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _BrandName = value;
                
                //if (IsDrugNotInCat )//== (long)AllLookupValues.V_DrugType.THUOC_NGOAIDANH_MUC)
                //{
                //    if(!BrandName.Contains(" *"))
                //    {
                //        BrandName += " *";
                //    }
                //}
                RaisePropertyChanged("BrandName");
                OnBrandNameChanged();
            }
        }
        private String _BrandName="";

        partial void OnBrandNameChanging(String value);
        partial void OnBrandNameChanged();


        [DataMemberAttribute()]
        public String Content
        {
            get
            {
                return _Content;
            }
            set
            {
                OnContentChanging(value);
                _Content = value;
                RaisePropertyChanged("Content");
                OnContentChanged();
            }
        }
        private String _Content;
        partial void OnContentChanging(String value);
        partial void OnContentChanged();


        [DataMemberAttribute()]
        public String UnitName
        {
            get
            {
                return _UnitName;
            }
            set
            {
                OnUnitNameChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _UnitName != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _UnitName = value;
                RaisePropertyChanged("UnitName");
                OnUnitNameChanged();
            }
        }
        private String _UnitName;
        partial void OnUnitNameChanging(String value);
        partial void OnUnitNameChanged();


        [DataMemberAttribute()]
        public String UnitUse
        {
            get
            {
                return _UnitUse;
            }
            set
            {
                OnUnitUseChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _UnitUse != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _UnitUse = value;
                RaisePropertyChanged("UnitUse");
                OnUnitUseChanged();
            }
        }
        private String _UnitUse;
        partial void OnUnitUseChanging(String value);
        partial void OnUnitUseChanged();


        [DataMemberAttribute()]
        public String Administration
        {
            get
            {
                return _Administration;
            }
            set
            {
                OnAdministrationChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _Administration != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _Administration = value;
                RaisePropertyChanged("Administration");
                OnAdministrationChanged();
            }
        }
        private String _Administration;
        partial void OnAdministrationChanging(String value);
        partial void OnAdministrationChanged();


        [DataMemberAttribute()]
        public Boolean IsDrugNotInCat
        {
            get
            {
                return _IsDrugNotInCat;
            }
            set
            {
                if(_IsDrugNotInCat != value)
                {
                    OnIsDrugNotInCatChanging(value);
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _IsDrugNotInCat != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _IsDrugNotInCat = value;
                    RaisePropertyChanged("IsDrugNotInCat");
                    if (IsDrugNotInCat)
                    {
                        BeOfHIMedicineList = false;
                        this.DayRpts += this.DayExtended;
                        this.DayExtended = 0;
                        //if (BrandName != null && !BrandName.Contains(" *"))
                        //{
                        //    BrandName += " *";
                        //}

                        //if (SelectedDrugForPrescription != null && SelectedDrugForPrescription.BrandName != null
                        //    && !SelectedDrugForPrescription.BrandName.Contains(" *"))
                        //{
                        //    SelectedDrugForPrescription.BrandName += " *";
                        //}
                    }
                    else
                    {
                        if (BrandName != null && BrandName.Contains(" *"))
                        {
                            BrandName = BrandName.Replace(" *", "");
                        }

                        if (SelectedDrugForPrescription != null && SelectedDrugForPrescription.BrandName != null
                            && SelectedDrugForPrescription.BrandName.Contains(" *"))
                        {
                            BrandName = BrandName.Replace(" *", "");
                        }
                    }
                    OnIsDrugNotInCatChanged();
                }                
            }
        }
        private Boolean _IsDrugNotInCat;
        partial void OnIsDrugNotInCatChanging(Boolean value);
        partial void OnIsDrugNotInCatChanged();

        private RefGenericDrugDetail _RefGenericDrugDetail;
        [DataMemberAttribute()]
        public RefGenericDrugDetail RefGenericDrugDetail
        {
            get => _RefGenericDrugDetail; set
            {
                _RefGenericDrugDetail = value;
                RaisePropertyChanged("RefGenericDrugDetail");
            }
        }

        private bool _IsContraIndicator;
        [DataMemberAttribute()]
        public bool IsContraIndicator
        {
            get => _IsContraIndicator;
            set
            {
                if(_IsContraIndicator != value)
                {
                    _IsContraIndicator = value;
                    RaisePropertyChanged("IsContraIndicator");
                }
            }
        }
        [DataMemberAttribute()]
        public ObservableCollection<Lookup> ListRouteOfAdministration
        {
            get
            {
                return _ListRouteOfAdministration;
            }
            set
            {
                _ListRouteOfAdministration = value;
                RaisePropertyChanged("ListRouteOfAdministration");
            }
        }
        private ObservableCollection<Lookup> _ListRouteOfAdministration;

        private string _UsageDistance;
        [DataMemberAttribute]
        public string UsageDistance
        {
            get
            {
                return _UsageDistance;
            }
            set
            {
                if (_UsageDistance == value)
                {
                    return;
                }
                _UsageDistance = value;
                RaisePropertyChanged("UsageDistance");
            }
        }
        private long _V_RouteOfAdministration;
        [DataMemberAttribute]
        public long V_RouteOfAdministration
        {
            get
            {
                return _V_RouteOfAdministration;
            }
            set
            {
                if (_V_RouteOfAdministration == value)
                {
                    return;
                }
                _V_RouteOfAdministration = value;
                //IsTruyenTinhMach = V_RouteOfAdministration == 61319 ? true  : false;
                RaisePropertyChanged("V_RouteOfAdministration");
            }
        }
        #endregion
    }
}