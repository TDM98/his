using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

/*
 * 20180920 #001 TBL:   Chinh sua chuc nang bug mantis ID 0000061, kiem tra tung property trong dataentities 
 *                      neu co su thay doi IsDataChanged = true
 * 20180925 #002 TBL: Kiem tra toa khong co thuoc khi luu     
 * 20181102 #003 TBL: Added PrescriptionIssueCode
 * 20220823 #004 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
 * 20220908 #005 DatTB: Chuẩn bị thuốc trước: chỉnh sửa các mục phản hồi sau khi test
 * 20220929 #005 DatTB:
 * + Thêm textbox tìm bệnh nhân theo tên/mã/stt
 * + Thêm đối tượng ưu tiên
 */
namespace DataEntities
{
    [DataContract]
    public partial class Prescription : NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new Prescription object.
     
        /// <param name="prescriptID">Initial value of the PrescriptID property.</param>
        public static Prescription CreatePrescription(long prescriptID)
        {
            Prescription prescription = new Prescription();
            prescription.PrescriptID = prescriptID;
            return prescription;
        }

        #endregion

        #region Primitive Properties

                         
        [DataMemberAttribute()]
        public long PrescriptID
        {
            get
            {
                return _PrescriptID;
            }
            set
            {
                if (_PrescriptID != value)
                {
                    OnPrescriptIDChanging(value);
                    _PrescriptID = value;
                    RaisePropertyChanged("PrescriptID");
                    OnPrescriptIDChanged();
                }
            }
        }
        private long _PrescriptID;
        partial void OnPrescriptIDChanging(long value);
        partial void OnPrescriptIDChanged();


        [DataMemberAttribute()]
        public long? Issue_HisID
        {
            get
            {
                return _Issue_HisID; 
            }
            set
            {
                _Issue_HisID = value;
                RaisePropertyChanged("Issue_HisID");
            }
        }
        private long? _Issue_HisID;
     

        [DataMemberAttribute()]
        public Nullable<Int64> ConsultantID
        {
            get
            {
                return _ConsultantID;
            }
            set
            {
                OnConsultantIDChanging(value);
                _ConsultantID = value;
                RaisePropertyChanged("ConsultantID");
                OnConsultantIDChanged();
            }
        }
        private Nullable<Int64> _ConsultantID;
        partial void OnConsultantIDChanging(Nullable<Int64> value);
        partial void OnConsultantIDChanged();

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
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
                OnDiagnosisChanged();
            }
        }
        private String _Diagnosis="";
        partial void OnDiagnosisChanging(String value);
        partial void OnDiagnosisChanged();


        [DataMemberAttribute()]
        public DiagnosisTreatment ObjDiagnosisTreatment
        {
            get
            {
                return _ObjDiagnosisTreatment;
            }
            set
            {
                OnObjDiagnosisTreatmentChanging(value);
                _ObjDiagnosisTreatment = value;
                if (ObjDiagnosisTreatment!=null
                    && ObjDiagnosisTreatment.DiagnosisFinal!=null)
                {
                    Diagnosis = ObjDiagnosisTreatment.DiagnosisFinal;
                    ICD10List = Diagnosis;
                }
                
                RaisePropertyChanged("ObjDiagnosisTreatment");
                OnObjDiagnosisTreatmentChanged();
            }
        }
        private DiagnosisTreatment _ObjDiagnosisTreatment;
        partial void OnObjDiagnosisTreatmentChanging(DiagnosisTreatment value);
        partial void OnObjDiagnosisTreatmentChanged();


        
     
        [DataMemberAttribute()]
        public String DoctorAdvice
        {
            get
            {
                return _DoctorAdvice;
            }
            set
            {
                OnDoctorAdviceChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _DoctorAdvice != value)
                {
                    IsDataChanged = true;
                }            
                /*▲====: #001*/
                _DoctorAdvice = value;
                RaisePropertyChanged("DoctorAdvice");
                OnDoctorAdviceChanged();
            }
        }
        private String _DoctorAdvice="";
        partial void OnDoctorAdviceChanging(String value);
        partial void OnDoctorAdviceChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Boolean> ForOutPatient
        {
            get
            {
                return _ForOutPatient;
            }
            set
            {
                OnForOutPatientChanging(value);
                _ForOutPatient = value;
                RaisePropertyChanged("ForOutPatient");
                OnForOutPatientChanged();
            }
        }
        private Nullable<Boolean> _ForOutPatient;
        partial void OnForOutPatientChanging(Nullable<Boolean> value);
        partial void OnForOutPatientChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Boolean> HaveBeenConsulted
        {
            get
            {
                return _HaveBeenConsulted;
            }
            set
            {
                OnHaveBeenConsultedChanging(value);
                _HaveBeenConsulted = value;
                RaisePropertyChanged("HaveBeenConsulted");
                OnHaveBeenConsultedChanged();
            }
        }
        private Nullable<Boolean> _HaveBeenConsulted;
        partial void OnHaveBeenConsultedChanging(Nullable<Boolean> value);
        partial void OnHaveBeenConsultedChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> V_PrescriptionType
        {
            get
            {
                return _V_PrescriptionType;
            }
            set
            {
                OnV_PrescriptionTypeChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _V_PrescriptionType != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _V_PrescriptionType = value;
                RaisePropertyChanged("V_PrescriptionType");
                OnV_PrescriptionTypeChanged();
            }
        }
        private Nullable<Int64> _V_PrescriptionType;
        partial void OnV_PrescriptionTypeChanging(Nullable<Int64> value);
        partial void OnV_PrescriptionTypeChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> V_PrescriptionIssuedCase
        {
            get
            {
                return _V_PrescriptionIssuedCase;
            }
            set
            {
                OnV_PrescriptionIssuedCaseChanging(value);
                _V_PrescriptionIssuedCase = value;
                RaisePropertyChanged("V_PrescriptionIssuedCase");
                OnV_PrescriptionIssuedCaseChanged();
            }
        }
        private Nullable<Int64> _V_PrescriptionIssuedCase;
        partial void OnV_PrescriptionIssuedCaseChanging(Nullable<Int64> value);
        partial void OnV_PrescriptionIssuedCaseChanged();

        #endregion

        #region Navigation Properties

        private PatientAppointment _Appointment;
        [DataMemberAttribute()]
        public PatientAppointment Appointment
        {
            get
            {
                return _Appointment;
            }
            set
            {
                if (_Appointment != value)
                {
                    _Appointment = value;
                    RaisePropertyChanged("Appointment");
                }
            }
        }

        private ObservableCollection<OutwardDMedRscr> _OutwardDMedRscrs;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDMedRscr> OutwardDMedRscrs
        {
            get
            {
                return _OutwardDMedRscrs;
            }
            set
            {
                if (_OutwardDMedRscrs != value)
                {
                    _OutwardDMedRscrs = value;
                    RaisePropertyChanged("OutwardDMedRscrs");
                }
            }
        }

        private ObservableCollection<OutwardDrugInvoice> _OutwardDrugInvoices;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDrugInvoice> OutwardDrugInvoices
        {
            get
            {
                return _OutwardDrugInvoices;
            }
            set
            {
                if (_OutwardDrugInvoices != value)
                {
                    _OutwardDrugInvoices = value;
                    RaisePropertyChanged("OutwardDrugInvoices");
                }
            }
        }
        /*▼====: #002*/
        private bool IsPrescriptionDetailsEmpty
        {
            get
            {                
                if (PrescriptionDetails == null)
                {
                    return true;
                }
                if (PrescriptionDetails.Count == 0 || (PrescriptionDetails.Count == 1 && PrescriptionDetails[0].DrugID.GetValueOrDefault() == 0))
                {
                    return true;
                }
                return false;
            }
        }
        /*▲====: #002*/
        /*▼====: #001*/
        private bool IsPrescriptionDetailsChanged
        {
            get
            {
                if (PrescriptionDetails != null)
                {
                    foreach (var pdItem in PrescriptionDetails)
                    {
                        if (pdItem.DrugID > 0 || (pdItem.IsDrugNotInCat)) //TBL: Truong hop thuoc ngoai danh muc se co DrugID = 0 nen them dk pdItem.IsDrugNotInCat
                        {
                            if (pdItem.IsDataChanged)
                                return true;
                        }
                    }
                }
                return false;
            }
        }
        /*▲====: #001*/
        private ObservableCollection<PrescriptionDetail> _PrescriptionDetails;
        [DataMemberAttribute()]
        public ObservableCollection<PrescriptionDetail> PrescriptionDetails
        {
            get
            {
                return _PrescriptionDetails;
            }
            set
            {
                if (_PrescriptionDetails != value)
                {                                        
                    _PrescriptionDetails = value;
                    RaisePropertyChanged("PrescriptionDetails");
                    if (PrescriptionDetails == null ||
                        PrescriptionDetails.Count < 1)
                    {
                        PreNoDrug = true;
                    }
                    else
                    {
                        PreNoDrug = false;
                    }                    
                }
            }
        }
        
     
        private PrescriptionIssueHistory _PrescriptionIssueHistory;
        [DataMemberAttribute()]
        public PrescriptionIssueHistory PrescriptionIssueHistory
        {
            get
            {
                return _PrescriptionIssueHistory;
            }
            set
            {
                if (_PrescriptionIssueHistory != value)
                {
                    _PrescriptionIssueHistory = value;
                    RaisePropertyChanged("PrescriptionIssueHistory");
                }
            }
        }
     
        private ObservableCollection<PrescriptionIssueHistory> _PrescriptionIssueHistories;
        [DataMemberAttribute()]
        public ObservableCollection<PrescriptionIssueHistory> PrescriptionIssueHistories
        {
            get
            {
                return _PrescriptionIssueHistories;
            }
            set
            {
                if (_PrescriptionIssueHistories != value)
                {
                    _PrescriptionIssueHistories = value;
                    RaisePropertyChanged("PrescriptionIssueHistories");
                }
            }
        }

     
        private Staff _ConsultantDoctor;
        [DataMemberAttribute()]
        public Staff ConsultantDoctor
        {
            get
            {
                return _ConsultantDoctor;
            }
            set
            {
                if (_ConsultantDoctor != value)
                {
                    _ConsultantDoctor = value;
                    RaisePropertyChanged("ConsultantDoctor");
                }
            }
        }


        private Staff _SecretaryStaff;
        [DataMemberAttribute()]
        public Staff SecretaryStaff
        {
            get
            {
                return _SecretaryStaff;
            }
            set
            {
                if (_SecretaryStaff != value)
                {
                    _SecretaryStaff = value;
                    RaisePropertyChanged("SecretaryStaff");
                }
            }
        }
        
     
        private Lookup _LookupPrescriptionType;
        [DataMemberAttribute()]
        public Lookup LookupPrescriptionType
        {
            get
            {
                return _LookupPrescriptionType;
            }
            set
            {
                if (_LookupPrescriptionType != value)
                {
                    _LookupPrescriptionType = value;
                    RaisePropertyChanged("LookupPrescriptionType");
                }
            }
        }
        

        [DataMemberAttribute()]
        public Int64 CreatorStaffID
        {
            get
            {
                return _CreatorStaffID;
            }
            set
            {
                if(_CreatorStaffID!=value)
                {
                    OnCreatorStaffIDChanging(value);
                    _CreatorStaffID = value;
                    RaisePropertyChanged("CreatorStaffID");
                    OnCreatorStaffIDChanged();
                }
            }
        }
        private Int64 _CreatorStaffID;
        partial void OnCreatorStaffIDChanging(Int64 value);
        partial void OnCreatorStaffIDChanged();


        private Staff _ObjCreatorStaffID;
        [DataMemberAttribute()]
        public Staff ObjCreatorStaffID
        {
            get
            {
                return _ObjCreatorStaffID;
            }
            set
            {
                if(_ObjCreatorStaffID!=value)
                {
                    OnObjCreatorStaffIDChanging(value);
                    _ObjCreatorStaffID = value;
                    RaisePropertyChanged("ObjCreatorStaffID");
                    OnObjCreatorStaffIDChanged();
                }
            }
        }
        partial void OnObjCreatorStaffIDChanging(Staff value);
        partial void OnObjCreatorStaffIDChanged();


        private Nullable<Int64> _ModifierStaffID;
        [DataMemberAttribute()]
        public Nullable<Int64> ModifierStaffID
        {
            get
            {
                return _ModifierStaffID;
            }
            set
            {
                OnModifierStaffIDChanging(value);
                _ModifierStaffID = value;
                RaisePropertyChanged("ModifierStaffID");
                OnModifierStaffIDChanged();
            }
        }
        partial void OnModifierStaffIDChanging(Nullable<Int64> value);
        partial void OnModifierStaffIDChanged();


        
        [DataMemberAttribute()]
        public Staff ObjModifierStaffID
        {
            get
            {
                return _ObjModifierStaffID;
            }
            set
            {
                if (_ObjModifierStaffID != value)
                {
                    OnObjModifierStaffIDChanging(value);
                    _ObjModifierStaffID = value;
                    RaisePropertyChanged("ObjModifierStaffID");
                    OnObjModifierStaffIDChanged();
                }
            }
        }
        private Staff _ObjModifierStaffID;
        partial void OnObjModifierStaffIDChanging(Staff value);
        partial void OnObjModifierStaffIDChanged();

        
        [DataMemberAttribute()]
        public DateTime RecDateCreated
        {
            get { return _RecDateCreated; }
            set
            {
                if(_RecDateCreated!=value)
                {
                    OnRecDateCreatedChanging(value);
                    _RecDateCreated = value;
                    RaisePropertyChanged("RecDateCreated");
                    OnRecDateCreatedChanged();
                }
            }
        }
        private DateTime _RecDateCreated;
        partial void OnRecDateCreatedChanging(DateTime value);
        partial void OnRecDateCreatedChanged();

        [DataMemberAttribute()]
        public int SelNDay
        {
            get { return _SelNDay; }
            set
            {
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _SelNDay != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _SelNDay = value;
            }
        }
        private int _SelNDay = 0;

        private DateTime? _SelApptDate = null;
        public DateTime? SelApptDate
        {
            get { return _SelApptDate; }
            set
            {
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _SelApptDate != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _SelApptDate = value;
            }
        }

        [DataMemberAttribute()]
        public Nullable<int> NDay
        {
            get { return _NDay; }
            set
            {
                if(_NDay!=value)
                {
                    OnNDayChanging(value);
                    if (_NDay.HasValue)
                    {
                        _NDayPreValue = _NDay.Value;
                        /*▼====: #001*/
                        if ((IsObjectBeingUsedByClient) && _NDay != value)
                        {
                            IsDataChanged = true;
                        }
                        /*▲====: #001*/
                    }
                    _NDay = value;
                    RaisePropertyChanged("NDay");
                    OnNDayChanged();
                }
            }
        }
        private Nullable<int> _NDay;
        partial void OnNDayChanging(Nullable<int> value);
        partial void OnNDayChanged();

        public int NDayPreValue
        {
            get { return _NDayPreValue; }
        }
        public int _NDayPreValue = 0;

        #endregion

        [DataMemberAttribute()]
        public Boolean HasAppointment
        {
            get
            {
                return _HasAppointment;
            }
            set
            {
                OnHasAppointmentChanging(value);
                /*▼====: #001*/
                if ((IsObjectBeingUsedByClient) && _HasAppointment != value)
                {
                    IsDataChanged = true;
                }
                /*▲====: #001*/
                _HasAppointment = value;
                RaisePropertyChanged("HasAppointment");
                OnHasAppointmentChanged();
            }
        }
        private Boolean _HasAppointment;
        partial void OnHasAppointmentChanging(Boolean value);
        partial void OnHasAppointmentChanged();

        [DataMemberAttribute()]
        public bool PreNoDrug
        {
            get
            {
                return _PreNoDrug;
            }
            set
            {
                OnPreNoDrugChanging(value);
                _PreNoDrug = value;
                RaisePropertyChanged("PreNoDrug");
                OnPreNoDrugChanged();
            }
        }
        private bool _PreNoDrug;
        partial void OnPreNoDrugChanging(bool value);
        partial void OnPreNoDrugChanged();


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


        private long _RefGenDrugCatID_1;
        [DataMemberAttribute()]
        public long RefGenDrugCatID_1
        {
            get
            {
                return _RefGenDrugCatID_1;
            }
            set
            {
                if (_RefGenDrugCatID_1 != value)
                {
                    _RefGenDrugCatID_1 = value;
                    RaisePropertyChanged("RefGenDrugCatID_1");
                }
            }
        }

        private string _CategoryName;
        [DataMemberAttribute()]
        public string CategoryName
        {
            get
            {
                return _CategoryName;
            }
            set
            {
                if (_CategoryName != value)
                {
                    _CategoryName = value;
                    RaisePropertyChanged("CategoryName");
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

        private bool _IsEmptyPrescription = false;
        [DataMemberAttribute]
        public bool IsEmptyPrescription
        {
            get { return _IsEmptyPrescription; }
            set
            {
                if (_IsEmptyPrescription != value)
                {
                    _IsEmptyPrescription = value;
                    RaisePropertyChanged("IsEmptyPrescription");
                }
            }
        }
        /*▼====: #003*/
        private string _PrescriptionIssueCode;
        [DataMemberAttribute]
        public string PrescriptionIssueCode
        {
            get { return _PrescriptionIssueCode; }
            set
            {
                if (_PrescriptionIssueCode != value)
                {
                    _PrescriptionIssueCode = value;
                    RaisePropertyChanged("PrescriptionIssueCode");
                }
            }
        }
        /*▲====: #003*/

        private bool _IsWarningSimilar;
        [DataMemberAttribute]
        public bool IsWarningSimilar
        {
            get { return _IsWarningSimilar; }
            set
            {
                if (_IsWarningSimilar != value)
                {
                    _IsWarningSimilar = value;
                    RaisePropertyChanged("IsWarningSimilar");
                }
            }
        }

        private bool _IsWarningInteraction;
        [DataMemberAttribute]
        public bool IsWarningInteraction
        {
            get { return _IsWarningInteraction; }
            set
            {
                if (_IsWarningInteraction != value)
                {
                    _IsWarningInteraction = value;
                    RaisePropertyChanged("IsWarningInteraction");
                }
            }
        }

        private string _MedNameUseOnlyForCheckConsultation;
        [DataMemberAttribute()]
        public string MedNameUseOnlyForCheckConsultation
        {
            get
            {
                return _MedNameUseOnlyForCheckConsultation;
            }
            set
            {
                if (_MedNameUseOnlyForCheckConsultation != value)
                {
                    _MedNameUseOnlyForCheckConsultation = value;
                    RaisePropertyChanged("MedNameUseOnlyForCheckConsultation");
                }
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

        private string _Reason;
        [DataMemberAttribute()]
        public string Reason
        {
            get
            {
                return _Reason;
            }
            set
            {
                if (_Reason != value)
                {
                    _Reason = value;
                    RaisePropertyChanged("Reason");
                }
            }
        }

        private bool _IsOutsideRegimen;
        [DataMemberAttribute]
        public bool IsOutsideRegimen
        {
            get { return _IsOutsideRegimen; }
            set
            {
                if (_IsOutsideRegimen != value)
                {
                    _IsOutsideRegimen = value;
                    RaisePropertyChanged("IsOutsideRegimen");
                }
            }
        }

        private long _ConfirmedDTItemID;
        [DataMemberAttribute]
        public long ConfirmedDTItemID
        {
            get
            {
                return _ConfirmedDTItemID;
            }
            set
            {
                if (_ConfirmedDTItemID == value)
                {
                    return;
                }
                _ConfirmedDTItemID = value;
                RaisePropertyChanged("ConfirmedDTItemID");
            }
        }

        private Int32 _PercentExceed;
        [DataMemberAttribute]
        public Int32 PercentExceed
        {
            get
            {
                return _PercentExceed;
            }
            set
            {
                if (_PercentExceed == value)
                {
                    return;
                }
                _PercentExceed = value;
                RaisePropertyChanged("PercentExceed");
            }
        }

        //▼==== #004
        private bool _IsWaiting;
        [DataMemberAttribute]
        public bool IsWaiting
        {
            get { return _IsWaiting; }
            set
            {
                if (_IsWaiting != value)
                {
                    _IsWaiting = value;
                    RaisePropertyChanged("IsWaiting");
                }
            }
        }

        private int _CountPrint;
        [DataMemberAttribute]
        public int CountPrint
        {
            get { return _CountPrint; }
            set
            {
                if (_CountPrint != value)
                {
                    _CountPrint = value;
                    RaisePropertyChanged("CountPrint");
                }
            }
        }

        private long _HIReportID;
        [DataMemberAttribute]
        public long HIReportID
        {
            get { return _HIReportID; }
            set
            {
                if (_HIReportID != value)
                {
                    _HIReportID = value;
                    RaisePropertyChanged("HIReportID");
                }
            }
        }

        private bool _PreCheck;
        [DataMemberAttribute]
        public bool PreCheck
        {
            get { return _PreCheck; }
            set
            {
                if (_PreCheck != value)
                {
                    _PreCheck = value;
                    RaisePropertyChanged("PreCheck");
                }
            }
        }

        private Location _Location;
        [DataMemberAttribute]
        public Location Location
        {
            get { return _Location; }
            set
            {
                if (_Location != value)
                {
                    _Location = value;
                    RaisePropertyChanged("Location");
                }
            }
        }
        //▲==== #004
        //▼==== #005
        private string _StoreServiceSeqNumStr;
        [DataMemberAttribute]
        public string StoreServiceSeqNumStr
        {
            get { return _StoreServiceSeqNumStr; }
            set
            {
                if (_StoreServiceSeqNumStr != value)
                {
                    _StoreServiceSeqNumStr = value;
                    RaisePropertyChanged("StoreServiceSeqNumStr");
                }
            }
        }
        //▲==== #005
        //▼==== #006
        private bool _IsPri;
        [DataMemberAttribute]
        public bool IsPri
        {
            get { return _IsPri; }
            set
            {
                if (_IsPri != value)
                {
                    _IsPri = value;
                    RaisePropertyChanged("IsPri");
                }
            }
        }
        //▲==== #006
    }
}