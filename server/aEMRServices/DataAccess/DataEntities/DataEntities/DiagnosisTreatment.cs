using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

/*
 * 20180920 #001 TBL:   Chinh sua chuc nang bug mantis ID 0000061, kiem tra tung property trong dataentities 
 *                      neu co su thay doi IsDataChanged = true
 * 20181027 #002 TBL:   BM 0000130. Added V_TreatmentType and LookupTreatment
 * 20210701 #003 TNHX: Thêm cột user bsi mượn OfficialAccountID
 * 20220622 #004 QTD:  Thêm cột đánh dấu bệnh nặng
 * 20220725 #005 DatTB: Thêm thông tin nhịp thở RespiratoryRate vào DiagnosisTreatment_InPt.
 */
namespace DataEntities
{
    public partial class DiagnosisTreatment : NotifyChangedBase, IEditableObject
    {
        public DiagnosisTreatment()
            : base()
        {

        }

        #region IEditableObject Members
        private DiagnosisTreatment _tempDiagnosisTreatment;
        public void BeginEdit()
        {
            _tempDiagnosisTreatment = (DiagnosisTreatment)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempDiagnosisTreatment)
                CopyFrom(_tempDiagnosisTreatment);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(DiagnosisTreatment p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        #region  BackupObject
        private DiagnosisTreatment _bkObject;
        public DiagnosisTreatment BKObject
        {
            get
            {
                return _bkObject;
            }
            set
            {
                if (_bkObject != null)
                {
                    _bkObject = value;
                }
            }
        }
        public void CreateBackupObject()
        {
            _bkObject = new DiagnosisTreatment();

            _bkObject.DTItemID = this.DTItemID;
            _bkObject.ServiceRecID = this.ServiceRecID;
            _bkObject.Diagnosis = this.Diagnosis;
            _bkObject.OrientedTreatment = this.OrientedTreatment;
            _bkObject.DoctorComments = this.DoctorComments;
            _bkObject.MDRptTemplateID = this.MDRptTemplateID;

            if (this.PatientServiceRecord != null)
            {
                _bkObject.PatientServiceRecord = new PatientServiceRecord();
                _bkObject.PatientServiceRecord.ServiceRecID = this.PatientServiceRecord.ServiceRecID;
                _bkObject.PatientServiceRecord.PtRegistrationID = this.PatientServiceRecord.PtRegistrationID;
                _bkObject.PatientServiceRecord.StaffID = this.PatientServiceRecord.StaffID;
                if (this.PatientServiceRecord.Staff != null)
                {
                    _bkObject.PatientServiceRecord.Staff = new Staff();
                    _bkObject.PatientServiceRecord.Staff.FullName = this.PatientServiceRecord.Staff.FullName;
                    _bkObject.PatientServiceRecord.Staff.SPhoneNumber = this.PatientServiceRecord.Staff.SPhoneNumber;
                    _bkObject.PatientServiceRecord.PatientRecID = this.PatientServiceRecord.PatientRecID;
                    _bkObject.PatientServiceRecord.V_ProcessingType = this.PatientServiceRecord.V_ProcessingType;
                    _bkObject.PatientServiceRecord.V_Behaving = this.PatientServiceRecord.V_Behaving;
                }

                if (this.PatientServiceRecord.LookupProcessingType != null)
                {
                    _bkObject.PatientServiceRecord.LookupProcessingType = new Lookup();
                    _bkObject.PatientServiceRecord.LookupProcessingType.LookupID = this.PatientServiceRecord.LookupProcessingType.LookupID;
                    _bkObject.PatientServiceRecord.LookupProcessingType.ObjectValue = this.PatientServiceRecord.LookupProcessingType.ObjectValue;
                }

                if (this.PatientServiceRecord.LookupBehaving != null)
                {
                    _bkObject.PatientServiceRecord.LookupBehaving = new Lookup();
                    _bkObject.PatientServiceRecord.LookupBehaving.LookupID = this.PatientServiceRecord.LookupBehaving.LookupID;
                    _bkObject.PatientServiceRecord.LookupBehaving.ObjectValue = this.PatientServiceRecord.LookupBehaving.ObjectValue;
                }
                if (this.PatientServiceRecord.PatientMedicalRecord != null)
                {
                    _bkObject.PatientServiceRecord.PatientMedicalRecord = new PatientMedicalRecord();
                    _bkObject.PatientServiceRecord.PatientMedicalRecord.PatientRecID = this.PatientServiceRecord.PatientMedicalRecord.PatientRecID;
                    _bkObject.PatientServiceRecord.PatientMedicalRecord.PatientID = this.PatientServiceRecord.PatientMedicalRecord.PatientID;
                    _bkObject.PatientServiceRecord.PatientMedicalRecord.NationalMedicalCode = this.PatientServiceRecord.PatientMedicalRecord.NationalMedicalCode;
                    _bkObject.PatientServiceRecord.PatientMedicalRecord.CreatedDate = this.PatientServiceRecord.PatientMedicalRecord.CreatedDate;
                }
            }
            _bkObject.LatestPrescriptID = this.LatestPrescriptID;
            _bkObject.PrescriptID = this.PrescriptID;
        }
        #endregion

        #region Factory Method


        /// Create a new DiagnosisTreatment object.

        /// <param name="dTItemID">Initial value of the DTItemID property.</param>
        public static DiagnosisTreatment CreateDiagnosisTreatment(long dTItemID)
        {
            DiagnosisTreatment diagnosisTreatment = new DiagnosisTreatment();
            diagnosisTreatment.DTItemID = dTItemID;
            return diagnosisTreatment;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long DTItemID
        {
            get
            {
                return _DTItemID;
            }
            set
            {
                if (_DTItemID != value)
                {
                    OnDTItemIDChanging(value);
                    _DTItemID = value;
                    RaisePropertyChanged("DTItemID");
                    OnDTItemIDChanged();
                }
            }
        }
        private long _DTItemID;
        partial void OnDTItemIDChanging(long value);
        partial void OnDTItemIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                OnServiceRecIDChanging(value);
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private Nullable<long> _ServiceRecID;
        partial void OnServiceRecIDChanging(Nullable<long> value);
        partial void OnServiceRecIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                OnPtRegDetailIDChanging(value);
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
                OnPtRegDetailIDChanged();
            }
        }
        private Nullable<long> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<long> value);
        partial void OnPtRegDetailIDChanged();


        [DataMemberAttribute()]
        public String Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                OnDiagnosisChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _Diagnosis != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
                OnDiagnosisChanged();
            }
        }
        private String _Diagnosis = "";
        partial void OnDiagnosisChanging(String value);
        partial void OnDiagnosisChanged();

        [DataMemberAttribute()]
        public String DiagnosisFinal
        {
            get
            {
                return _DiagnosisFinal;
            }
            set
            {
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _DiagnosisFinal != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _DiagnosisFinal = value;
                RaisePropertyChanged("DiagnosisFinal");
            }
        }
        private String _DiagnosisFinal;
        [DataMemberAttribute()]
        public String DiagnosisOther
        {
            get
            {
                return _DiagnosisOther;
            }
            set
            {
                OnDiagnosisOtherChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _DiagnosisOther != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _DiagnosisOther = value;
                RaisePropertyChanged("DiagnosisOther");
                OnDiagnosisOtherChanged();
            }
        }
        private String _DiagnosisOther;
        partial void OnDiagnosisOtherChanging(String value);
        partial void OnDiagnosisOtherChanged();
        [DataMemberAttribute()]
        public String ReasonHospitalStay
        {
            get
            {
                return _ReasonHospitalStay;
            }
            set
            {
                OnReasonHospitalStayChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _ReasonHospitalStay != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _ReasonHospitalStay = value;
                RaisePropertyChanged("ReasonHospitalStay");
                OnReasonHospitalStayChanged();
            }
        }
        private String _ReasonHospitalStay;
        partial void OnReasonHospitalStayChanging(String value);
        partial void OnReasonHospitalStayChanged();
        [DataMemberAttribute()]
        public String AdmissionCriteriaList
        {
            get
            {
                return _AdmissionCriteriaList;
            }
            set
            {
                OnAdmissionCriteriaListChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _AdmissionCriteriaList != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _AdmissionCriteriaList = value;
                RaisePropertyChanged("AdmissionCriteriaList");
                OnAdmissionCriteriaListChanged();
            }
        }
        private String _AdmissionCriteriaList;
        partial void OnAdmissionCriteriaListChanging(String value);
        partial void OnAdmissionCriteriaListChanged();

        [DataMemberAttribute()]
        public String OrientedTreatment
        {
            get
            {
                return _OrientedTreatment;
            }
            set
            {
                OnOrientedTreatmentChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _OrientedTreatment != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _OrientedTreatment = value;
                RaisePropertyChanged("OrientedTreatment");
                OnOrientedTreatmentChanged();
            }
        }
        private String _OrientedTreatment = "";
        partial void OnOrientedTreatmentChanging(String value);
        partial void OnOrientedTreatmentChanged();

        [DataMemberAttribute()]
        public String DoctorComments
        {
            get
            {
                return _DoctorComments;
            }
            set
            {
                OnDoctorCommentsChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _DoctorComments != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _DoctorComments = value;
                RaisePropertyChanged("DoctorComments");
                OnDoctorCommentsChanged();
            }
        }
        private String _DoctorComments;
        partial void OnDoctorCommentsChanging(String value);
        partial void OnDoctorCommentsChanged();


        //MDRptTemplateID
        [DataMemberAttribute()]
        public long MDRptTemplateID
        {
            get
            {
                return _MDRptTemplateID;
            }
            set
            {
                OnMDRptTemplateIDChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _MDRptTemplateID != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _MDRptTemplateID = value;
                RaisePropertyChanged("MDRptTemplateID");
                OnMDRptTemplateIDChanged();
            }
        }
        private long _MDRptTemplateID;
        partial void OnMDRptTemplateIDChanging(long value);
        partial void OnMDRptTemplateIDChanged();


        [DataMemberAttribute()]
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                OnDoctorStaffIDChanging(value);
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
                OnDoctorStaffIDChanged();
            }
        }
        private long _DoctorStaffID;
        partial void OnDoctorStaffIDChanging(long value);
        partial void OnDoctorStaffIDChanged();


        [DataMemberAttribute()]
        public long DoctorAccountBorrowedID
        {
            get
            {
                return _DoctorAccountBorrowedID;
            }
            set
            {
                _DoctorAccountBorrowedID = value;
                RaisePropertyChanged("DoctorAccountBorrowedID");
            }
        }
        private long _DoctorAccountBorrowedID;

        [DataMemberAttribute()]
        public Staff ObjDoctorStaffID
        {
            get { return _ObjDoctorStaffID; }
            set
            {
                if (_ObjDoctorStaffID != value)
                {
                    OnObjDoctorStaffIDChanging(value);
                    _ObjDoctorStaffID = value;
                    RaisePropertyChanged("ObjDoctorStaffID");
                    OnObjDoctorStaffIDChanged();
                }
            }
        }
        private Staff _ObjDoctorStaffID;
        partial void OnObjDoctorStaffIDChanging(Staff value);
        partial void OnObjDoctorStaffIDChanged();


        [DataMemberAttribute()]
        public long DeptLocationID
        {
            get
            {
                return _DeptLocationID;
            }
            set
            {
                OnDeptLocationIDChanging(value);
                _DeptLocationID = value;
                RaisePropertyChanged("DeptLocationID");
                OnDeptLocationIDChanged();
            }
        }
        private long _DeptLocationID;
        partial void OnDeptLocationIDChanging(long value);
        partial void OnDeptLocationIDChanged();



        [DataMemberAttribute()]
        public Nullable<long> LatestPrescriptID
        {
            get
            {
                return _LatestPrescriptID;
            }
            set
            {
                OnLatestPrescriptIDChanging(value);
                _LatestPrescriptID = value;
                RaisePropertyChanged("LatestPrescriptID");
                OnLatestPrescriptIDChanged();
            }
        }
        private Nullable<long> _LatestPrescriptID;
        partial void OnLatestPrescriptIDChanging(Nullable<long> value);
        partial void OnLatestPrescriptIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PrescriptID
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
        private Nullable<long> _PrescriptID;
        partial void OnPrescriptIDChanging(Nullable<long> value);
        partial void OnPrescriptIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                OnIssueIDChanging(value);
                _IssueID = value;
                RaisePropertyChanged("IssueID");
                OnIssueIDChanged();
            }
        }
        private Nullable<long> _IssueID;
        partial void OnIssueIDChanging(Nullable<long> value);
        partial void OnIssueIDChanged();


        [DataMemberAttribute()]
        private string _DiagnosisForDrug;
        public String DiagnosisForDrug
        {
            get
            {
                return _DiagnosisForDrug;
            }
            set
            {
                if (_DiagnosisForDrug != value)
                {
                    _DiagnosisForDrug = value;
                    RaisePropertyChanged("DiagnosisForDrug");
                }
            }
        }


        [DataMemberAttribute()]
        private String _Treatment = "";
        public String Treatment
        {
            get
            {
                return _Treatment;
            }
            set
            {
                if (_Treatment != value)
                {
                    /*▼====: #001*/
                    if ((IsObjectBeingUsedByClient) && _Treatment != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #001*/
                    _Treatment = value;
                    RaisePropertyChanged("Treatment");
                }
            }
        }


        [DataMemberAttribute()]
        private string _DiagnosisPriorityView;
        public String DiagnosisPriorityView
        {
            get
            {
                return _DiagnosisPriorityView;
            }
            set
            {
                if (_DiagnosisPriorityView != value)
                {
                    _DiagnosisPriorityView = value;
                    RaisePropertyChanged("DiagnosisPriorityView");
                }
            }
        }


        private string _ModifierDoctorNames = "";
        [DataMemberAttribute()]
        public String ModifierDoctorNames
        {
            get
            {
                return _ModifierDoctorNames;
            }
            set
            {
                if (_ModifierDoctorNames != value)
                {
                    _ModifierDoctorNames = value;
                    RaisePropertyChanged("ModifierDoctorNames");
                }
            }
        }

        private DateTime _DiagnosisDate;
        [DataMemberAttribute()]
        public DateTime DiagnosisDate
        {
            get
            {
                return _DiagnosisDate;
            }
            set
            {
                if (_DiagnosisDate != value)
                {
                    _DiagnosisDate = value;
                    RaisePropertyChanged("DiagnosisDate");
                }
            }
        }

        private long _V_DiagnosisType;
        [DataMemberAttribute()]
        public long V_DiagnosisType
        {
            get
            {
                return _V_DiagnosisType;
            }
            set
            {
                if (_V_DiagnosisType != value)
                {
                    _V_DiagnosisType = value;
                    RaisePropertyChanged("V_DiagnosisType");
                }
            }
        }

        private Lookup _LookupDiagnosis;
        [DataMemberAttribute()]
        public Lookup LookupDiagnosis
        {
            get
            {
                return _LookupDiagnosis;
            }
            set
            {
                if (_LookupDiagnosis != value)
                {
                    _LookupDiagnosis = value;
                    RaisePropertyChanged("LookupDiagnosis");
                }
            }
        }
        /*▼====: #002*/
        private long _V_TreatmentType;
        [DataMemberAttribute()]
        public long V_TreatmentType
        {
            get
            {
                return _V_TreatmentType;
            }
            set
            {
                if (_V_TreatmentType != value)
                {
                    if ((IsObjectBeingUsedByClient) && _V_TreatmentType != value)
                    {
                        IsDataChanged = true;
                    }
                    _V_TreatmentType = value;
                    RaisePropertyChanged("V_TreatmentType");
                }
            }
        }
        /*▲====: #002*/
        //▼====: #003
        [DataMemberAttribute()]
        public long? UserOfficialAccountID
        {
            get
            {
                return _UserOfficialAccountID;
            }
            set
            {
                _UserOfficialAccountID = value;
                RaisePropertyChanged("UserOfficialAccountID");
            }
        }
        private long? _UserOfficialAccountID;
        //▲====: #003
        #endregion

        #region Navigation Properties

        private PatientServiceRecord _PatientServiceRecord;
        [DataMemberAttribute()]
        public PatientServiceRecord PatientServiceRecord
        {
            get
            {
                return _PatientServiceRecord;
            }
            set
            {
                if (_PatientServiceRecord != value)
                {
                    _PatientServiceRecord = value;
                    RaisePropertyChanged("PatientServiceRecord");
                }
            }
        }



        #endregion
        /*▼====: #001*/
        public bool IsDataChanged = false;

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
        public override bool Equals(object obj)
        {
            DiagnosisTreatment cond = obj as DiagnosisTreatment;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.DTItemID == cond.DTItemID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private bool _IsDirty = false;
        public bool IsDirty
        {
            get
            {
                return _IsDirty;
            }
            set
            {
                if (_IsDirty != value)
                {
                    _IsDirty = value;
                    RaisePropertyChanged("IsDirty");
                }
            }
        }

        protected override void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);
            //bool cond1 = (propertyName == "Diagnosis" && this.Diagnosis.Length > 0);
            //bool cond2 = (propertyName == "OrientedTreatment" && this.OrientedTreatment.Length > 0);
            //bool cond3 = (propertyName == "DoctorComments" && this.DoctorComments.Length > 0);
            IsDirty = true;
        }


        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }

        private string _ICD10List;
        [DataMemberAttribute()]
        public string ICD10List
        {
            get
            {
                return _ICD10List;
            }
            set
            {
                if (_ICD10List != value)
                {
                    _ICD10List = value;
                    RaisePropertyChanged("ICD10List");
                }
            }
        }

        private string _ICD10SubList;
        [DataMemberAttribute()]
        public string ICD10SubList
        {
            get
            {
                return _ICD10SubList;
            }
            set
            {
                if (_ICD10SubList != value)
                {
                    _ICD10SubList = value;
                    RaisePropertyChanged("ICD10SubList");
                }
            }
        }


        private RefDepartment _department;
        [DataMemberAttribute()]
        public RefDepartment Department
        {
            get { return _department; }
            set
            {
                if (_department != value)
                {
                    _department = value;
                    RaisePropertyChanged("Department");
                }
            }
        }
        private Guid? _inPtDeptGuid = null;
        [DataMemberAttribute()]
        public Guid? InPtDeptGuid
        {
            get
            {
                return _inPtDeptGuid;
            }
            set
            {
                _inPtDeptGuid = value;
                RaisePropertyChanged("InPtDeptGuid");
            }
        }

        private bool _HeartFailureType;
        [DataMemberAttribute()]
        public bool HeartFailureType
        {
            get
            {
                return _HeartFailureType;
            }
            set
            {
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _HeartFailureType != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _HeartFailureType = value;
                RaisePropertyChanged("HeartFailureType");
            }
        }

        //==== #001
        private long _IntPtDiagDrInstructionID;
        [DataMemberAttribute()]
        public long IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                _IntPtDiagDrInstructionID = value;
                RaisePropertyChanged("IntPtDiagDrInstructionID");
            }
        }
        private decimal _Pulse;
        [DataMemberAttribute()]
        public decimal Pulse
        {
            get
            {
                return _Pulse;
            }
            set
            {
                _Pulse = value;
                RaisePropertyChanged("Pulse");
            }
        }
        private decimal _BloodPressure;
        [DataMemberAttribute()]
        public decimal BloodPressure
        {
            get
            {
                return _BloodPressure;
            }
            set
            {
                _BloodPressure = value;
                RaisePropertyChanged("BloodPressure");
            }
        }

        //▼==== #005
        private decimal _RespiratoryRate;
        [DataMemberAttribute()]
        public decimal RespiratoryRate
        {
            get
            {
                return _RespiratoryRate;
            }
            set
            {
                _RespiratoryRate = value;
                RaisePropertyChanged("RespiratoryRate");
            }
        }
        //▲==== #005

        private decimal _LowerBloodPressure;
        [DataMemberAttribute()]
        public decimal LowerBloodPressure
        {
            get
            {
                return _LowerBloodPressure;
            }
            set
            {
                _LowerBloodPressure = value;
                RaisePropertyChanged("LowerBloodPressure");
            }
        }
        private decimal _Temperature;
        [DataMemberAttribute()]
        public decimal Temperature
        {
            get
            {
                return _Temperature;
            }
            set
            {
                _Temperature = value;
                RaisePropertyChanged("Temperature");
            }
        }
        private decimal _SpO2;
        [DataMemberAttribute()]
        public decimal SpO2
        {
            get
            {
                return _SpO2;
            }
            set
            {
                _SpO2 = value;
                RaisePropertyChanged("SpO2");
            }
        }
        //==== #001

        private string _KLSTriGiac;
        private string _KLSNiemMac;
        private string _KLSKetMac;
        private string _KLSTuyenGiap;
        private string _KLSHachBachHuyet;
        private string _KLSPhoi;
        private string _KLSTim;
        private string _KLSBung;
        private string _KLSTMH;

        [DataMemberAttribute()]
        public string KLSTriGiac
        {
            get => _KLSTriGiac; set
            {
                if ((IsObjectBeingUsedByClient) && _KLSTriGiac != value)
                {
                    IsDataChanged = true;
                }
                _KLSTriGiac = value;
                RaisePropertyChanged("KLSTriGiac");
            }
        }
        [DataMemberAttribute()]
        public string KLSNiemMac
        {
            get => _KLSNiemMac; set
            {
                if ((IsObjectBeingUsedByClient) && _KLSNiemMac != value)
                {
                    IsDataChanged = true;
                }
                _KLSNiemMac = value;
                RaisePropertyChanged("KLSNiemMac");
            }
        }
        [DataMemberAttribute()]
        public string KLSKetMac
        {
            get => _KLSKetMac; set
            {
                if ((IsObjectBeingUsedByClient) && _KLSKetMac != value)
                {
                    IsDataChanged = true;
                }
                _KLSKetMac = value;
                RaisePropertyChanged("KLSKetMac");
            }
        }
        [DataMemberAttribute()]
        public string KLSTuyenGiap
        {
            get => _KLSTuyenGiap; set
            {
                if ((IsObjectBeingUsedByClient) && _KLSTuyenGiap != value)
                {
                    IsDataChanged = true;
                }
                _KLSTuyenGiap = value;
                RaisePropertyChanged("KLSTuyenGiap");
            }
        }
        [DataMemberAttribute()]
        public string KLSHachBachHuyet
        {
            get => _KLSHachBachHuyet; set
            {
                if ((IsObjectBeingUsedByClient) && _KLSHachBachHuyet != value)
                {
                    IsDataChanged = true;
                }
                _KLSHachBachHuyet = value;
                RaisePropertyChanged("KLSHachBachHuyet");
            }
        }
        [DataMemberAttribute()]
        public string KLSPhoi
        {
            get => _KLSPhoi; set
            {
                if ((IsObjectBeingUsedByClient) && _KLSPhoi != value)
                {
                    IsDataChanged = true;
                }
                _KLSPhoi = value;
                RaisePropertyChanged("KLSPhoi");
            }
        }
        [DataMemberAttribute()]
        public string KLSTim
        {
            get => _KLSTim; set
            {
                if ((IsObjectBeingUsedByClient) && _KLSTim != value)
                {
                    IsDataChanged = true;
                }
                _KLSTim = value;
                RaisePropertyChanged("KLSTim");
            }
        }
        [DataMemberAttribute()]
        public string KLSBung
        {
            get => _KLSBung; set
            {
                if ((IsObjectBeingUsedByClient) && _KLSBung != value)
                {
                    IsDataChanged = true;
                }
                _KLSBung = value;
                RaisePropertyChanged("KLSBung");
            }
        }
        [DataMemberAttribute()]
        public string KLSTMH
        {
            get => _KLSTMH; set
            {
                if ((IsObjectBeingUsedByClient) && _KLSTMH != value)
                {
                    IsDataChanged = true;
                }
                _KLSTMH = value;
                RaisePropertyChanged("KLSTMH");
            }
        }

        [DataMemberAttribute]
        public string TreatmentType
        {
            get => _TreatmentType; set
            {
                _TreatmentType = value;
                RaisePropertyChanged("TreatmentType");
            }
        }
        private string _TreatmentType;

        private InPatientAdmDisDetails _AdmissionInfo;
        [DataMemberAttribute]
        public InPatientAdmDisDetails AdmissionInfo
        {
            get
            {
                return _AdmissionInfo;
            }
            set
            {
                if (_AdmissionInfo != value)
                {
                    _AdmissionInfo = value;
                }
                RaisePropertyChanged("AdmissionInfo");
            }
        }

        private bool _MarkedDelete;
        public bool MarkedDelete
        {
            get
            {
                return _MarkedDelete;
            }
            set
            {
                if (_MarkedDelete != value)
                {
                    _MarkedDelete = value;
                    RaisePropertyChanged("MarkedDelete");
                }
            }
        }
        private string _ICD10Code;
        [DataMemberAttribute()]
        public string ICD10Code
        {
            get
            {
                return _ICD10Code;
            }
            set
            {
                if (_ICD10Code != value)
                {
                    _ICD10Code = value;
                    RaisePropertyChanged("ICD10Code");
                }
            }
        }

        private DiagnosisDeptDetail _DeptDetail;
        [DataMemberAttribute]
        public DiagnosisDeptDetail DeptDetail
        {
            get
            {
                return _DeptDetail;
            }
            set
            {
                if (_DeptDetail == value)
                {
                    return;
                }
                _DeptDetail = value;
                RaisePropertyChanged("DeptDetail");
            }
        }
        private long _MedSecretaryID;
        [DataMemberAttribute()]
        public long MedSecretaryID
        {
            get
            {
                return _MedSecretaryID;
            }
            set
            {
                if (_MedSecretaryID != value)
                {
                    _MedSecretaryID = value;
                    RaisePropertyChanged("MedSecretaryID");
                }
            }
        }
        private int _VersionNumber;
        [DataMemberAttribute()]
        public int VersionNumber
        {
            get
            {
                return _VersionNumber;
            }
            set
            {
                if (_VersionNumber != value)
                {
                    _VersionNumber = value;
                    RaisePropertyChanged("VersionNumber");
                }
            }
        }
        private bool _IsAdmission;
        [DataMemberAttribute]
        public bool IsAdmission
        {
            get
            {
                return _IsAdmission;
            }
            set
            {
                if (_IsAdmission == value)
                {
                    return;
                }
                _IsAdmission = value;
                RaisePropertyChanged("IsAdmission");
            }
        }
        private long? _ConfirmedForInPatientDeptDetailID;
        [DataMemberAttribute]
        public long? ConfirmedForInPatientDeptDetailID
        {
            get
            {
                return _ConfirmedForInPatientDeptDetailID;
            }
            set
            {
                if (_ConfirmedForInPatientDeptDetailID == value)
                {
                    return;
                }
                _ConfirmedForInPatientDeptDetailID = value;
                RaisePropertyChanged("ConfirmedForInPatientDeptDetailID");
            }
        }
        private long? _ConfirmedForPrescriptID;
        [DataMemberAttribute]
        public long? ConfirmedForPrescriptID
        {
            get
            {
                return _ConfirmedForPrescriptID;
            }
            set
            {
                if (_ConfirmedForPrescriptID == value)
                {
                    return;
                }
                _ConfirmedForPrescriptID = value;
                RaisePropertyChanged("ConfirmedForPrescriptID");
            }
        }
        private int? _ConfimedForPreAndDischarge;
        [DataMemberAttribute]
        public int? ConfimedForPreAndDischarge
        {
            get
            {
                return _ConfimedForPreAndDischarge;
            }
            set
            {
                if (_ConfimedForPreAndDischarge == value)
                {
                    return;
                }
                _ConfimedForPreAndDischarge = value;
                RaisePropertyChanged("ConfimedForPreAndDischarge");
            }
        }

        private bool _IsTreatmentCOVID;
        [DataMemberAttribute]
        public bool IsTreatmentCOVID
        {
            get
            {
                return _IsTreatmentCOVID;
            }
            set
            {
                if (_IsTreatmentCOVID == value)
                {
                    return;
                }
                _IsTreatmentCOVID = value;
                RaisePropertyChanged("IsTreatmentCOVID");
            }
        }
        //▼====: #004
        private bool _IsSevereIllness;
        public bool IsSevereIllness
        {
            get
            {
                return _IsSevereIllness;
            }
            set
            {
                if (_IsSevereIllness == value)
                {
                    return;
                }
                _IsSevereIllness = value;
                RaisePropertyChanged("IsSevereIllness");
            }
        }
        //▲====: #004
        private string _DiseaseStage;
        [DataMemberAttribute()]
        public string DiseaseStage
        {
            get { return _DiseaseStage; }
            set
            {
                if (_DiseaseStage == value)
                {
                    return;
                }
                if ((IsObjectBeingUsedByClient) && _DiseaseStage != value)
                {
                    IsDataChanged = true;
                }
                _DiseaseStage = value;
                RaisePropertyChanged("DiseaseStage");
            }
        }
    }
    public class DiagnosisDeptDetail : NotifyChangedBase
    {
        private long _InPatientDeptDetailID;
        private string _DeptName;
        private string _LocationName;
        [DataMemberAttribute]
        public long InPatientDeptDetailID
        {
            get
            {
                return _InPatientDeptDetailID;
            }
            set
            {
                if (_InPatientDeptDetailID == value)
                {
                    return;
                }
                _InPatientDeptDetailID = value;
                RaisePropertyChanged("InPatientDeptDetailID");
                RaisePropertyChanged("GroupText");
            }
        }
        [DataMemberAttribute]
        public string DeptName
        {
            get
            {
                return _DeptName;
            }
            set
            {
                if (_DeptName == value)
                {
                    return;
                }
                _DeptName = value;
                RaisePropertyChanged("DeptName");
                RaisePropertyChanged("GroupText");
            }
        }
        [DataMemberAttribute]
        public string LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                if (_LocationName == value)
                {
                    return;
                }
                _LocationName = value;
                RaisePropertyChanged("LocationName");
            }
        }
        private Int16 _DeptEntrySeqNum;
        [DataMemberAttribute()]
        public Int16 DeptEntrySeqNum
        {
            get { return _DeptEntrySeqNum; }
            set
            {
                _DeptEntrySeqNum = value;
                RaisePropertyChanged("DeptEntrySeqNum");
            }
        }
        
        public string GroupText
        {
            get
            {
                return string.Format("[{0}] {1}", InPatientDeptDetailID, DeptName);
            }
        }
        public string GroupText2
        {
            get
            {
                return string.Format("[{0}] {1}", DeptEntrySeqNum, DeptName);
            }
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(DiagnosisDeptDetail))
            {
                return false;
            }
            return InPatientDeptDetailID.Equals(((DiagnosisDeptDetail)obj).InPatientDeptDetailID)
                && DeptName.Equals(((DiagnosisDeptDetail)obj).DeptName)
                && LocationName.Equals(((DiagnosisDeptDetail)obj).LocationName);
        }
        public override string ToString()
        {
            return DeptName;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}