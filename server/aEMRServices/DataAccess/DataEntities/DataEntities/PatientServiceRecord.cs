using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PatientServiceRecord : NotifyChangedBase, IEditableObject
    {
        #region Factory Method


        /// Create a new PatientServiceRecord object.

        /// <param name="serviceRecID">Initial value of the ServiceRecID property.</param>
        /// <param name="examDate">Initial value of the ExamDate property.</param>
        public static PatientServiceRecord CreatePatientServiceRecord(long serviceRecID, DateTime examDate)
        {
            PatientServiceRecord patientServiceRecord = new PatientServiceRecord();
            patientServiceRecord.ServiceRecID = serviceRecID;
            patientServiceRecord.ExamDate = examDate;
            return patientServiceRecord;
        }

        #endregion

        #region Primitive Properties
        private DateTime? _dateModified;
        [DataMemberAttribute()]
        public DateTime? DateModified
        {
            get
            {
                return _dateModified;
            }
            set
            {
                if (_dateModified != value)
                {
                    _dateModified = value;
                    RaisePropertyChanged("DateModified");
                }
            }
        }
        [DataMemberAttribute()]
        public long ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                if (_ServiceRecID != value)
                {
                    OnServiceRecIDChanging(value);
                    _ServiceRecID = value;
                    RaisePropertyChanged("ServiceRecID");
                    OnServiceRecIDChanged();
                }
            }
        }
        private long _ServiceRecID;
        partial void OnServiceRecIDChanging(long value);
        partial void OnServiceRecIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                OnPtRegistrationIDChanging(value);
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
                OnPtPtRegistrationIDChanged();
            }
        }
        private Nullable<Int64> _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(Nullable<Int64> value);
        partial void OnPtPtRegistrationIDChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegDetailID
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
                OnPtPtRegDetailIDChanged();
            }
        }
        private Nullable<Int64> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<Int64> value);
        partial void OnPtPtRegDetailIDChanged();



        [DataMemberAttribute()]
        public string PtRegistrationCode
        {
            get
            {
                return _PtRegistrationCode;
            }
            set
            {

                _PtRegistrationCode = value;
                RaisePropertyChanged("PtRegistrationCode");
            }
        }
        private string _PtRegistrationCode;

        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                ////ReportPropertyChanging("StaffID");
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PatientRecID
        {
            get
            {
                return _PatientRecID;
            }
            set
            {
                OnPatientRecIDChanging(value);
                _PatientRecID = value;
                RaisePropertyChanged("PatientRecID");
                OnPatientRecIDChanged();
            }
        }
        private Nullable<long> _PatientRecID;
        partial void OnPatientRecIDChanging(Nullable<long> value);
        partial void OnPatientRecIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PatientMedicalFileID
        {
            get
            {
                return _PatientMedicalFileID;
            }
            set
            {
                OnPatientMedicalFileIDChanging(value);
                _PatientMedicalFileID = value;
                RaisePropertyChanged("PatientMedicalFileID");
                OnPatientMedicalFileIDChanged();
            }
        }
        private Nullable<long> _PatientMedicalFileID;
        partial void OnPatientMedicalFileIDChanging(Nullable<long> value);
        partial void OnPatientMedicalFileIDChanged();

        [DataMemberAttribute()]
        public DateTime ExamDate
        {
            get
            {
                return _ExamDate;
            }
            set
            {
                OnExamDateChanging(value);
                _ExamDate = value;
                RaisePropertyChanged("ExamDate");
                OnExamDateChanged();
            }
        }
        private DateTime _ExamDate;
        partial void OnExamDateChanging(DateTime value);
        partial void OnExamDateChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_ProcessingType
        {
            get
            {
                return _V_ProcessingType;
            }
            set
            {
                OnV_ProcessingTypeChanging(value);
                _V_ProcessingType = value;
                RaisePropertyChanged("V_ProcessingType");
                OnV_ProcessingTypeChanged();
            }
        }
        private Nullable<Int64> _V_ProcessingType;
        partial void OnV_ProcessingTypeChanging(Nullable<Int64> value);
        partial void OnV_ProcessingTypeChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_Behaving
        {
            get
            {
                return _V_Behaving;
            }
            set
            {
                OnV_BehavingChanging(value);
                _V_Behaving = value;
                RaisePropertyChanged("V_Behaving");
                OnV_BehavingChanged();
            }
        }
        private Nullable<Int64> _V_Behaving;
        partial void OnV_BehavingChanging(Nullable<Int64> value);
        partial void OnV_BehavingChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_DiagnosisType
        {
            get
            {
                return _V_DiagnosisType;
            }
            set
            {
                OnV_DiagnosisTypeChanging(value);
                _V_DiagnosisType = value;
                RaisePropertyChanged("V_DiagnosisType");
                OnV_DiagnosisTypeChanged();
            }
        }
        private Nullable<Int64> _V_DiagnosisType;
        partial void OnV_DiagnosisTypeChanging(Nullable<Int64> value);
        partial void OnV_DiagnosisTypeChanged();
        #endregion

        #region Navigation Properties

        private ObservableCollection<DiagnosisTreatment> _DiagnosisTreatments;
        [DataMemberAttribute()]
        public ObservableCollection<DiagnosisTreatment> DiagnosisTreatments
        {
            get
            {
                return _DiagnosisTreatments;
            }
            set
            {
                if (_DiagnosisTreatments != value)
                {
                    _DiagnosisTreatments = value;
                    RaisePropertyChanged("DiagnosisTreatments");
                }
            }
        }

     
        private ObservableCollection<Examination> _Examinations;
        [DataMemberAttribute()]
        public ObservableCollection<Examination> Examinations
        {
            get
            {
                return _Examinations;
            }
            set
            {
                if (_Examinations != value)
                {
                    _Examinations = value;
                    RaisePropertyChanged("Examinations");
                }
            }
        }

     
        private ObservableCollection<OutwardInvoiceBlood> _OutwardInvoiceBloods;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardInvoiceBlood> OutwardInvoiceBloods
        {
            get
            {
                return _OutwardInvoiceBloods;
            }
            set
            {
                if (_OutwardInvoiceBloods != value)
                {
                    _OutwardInvoiceBloods = value;
                    RaisePropertyChanged("OutwardInvoiceBloods");
                }
            }
        }

        private PatientMedicalRecord _PatientMedicalRecord;
        [DataMemberAttribute()]
        public PatientMedicalRecord PatientMedicalRecord
        {
            get
            {
                return _PatientMedicalRecord;
            }
            set
            {
                if (_PatientMedicalRecord != value)
                {
                    _PatientMedicalRecord = value;
                    RaisePropertyChanged("PatientMedicalRecord");
                }
            }
        }

        private PatientMedicalFile _patientMedicalFile;
        [DataMemberAttribute()]
        public PatientMedicalFile patientMedicalFile
        {
            get
            {
                return _patientMedicalFile;
            }
            set
            {
                if (_patientMedicalFile != value)
                {
                    _patientMedicalFile = value;
                    RaisePropertyChanged("patientMedicalFile");
                }
            }
        }

     
        private ObservableCollection<PatientPCLRequest> _PatientPCLAppointments;
        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLRequest> PatientPCLAppointments
        {
            get
            {
                return _PatientPCLAppointments;
            }
            set
            {
                if (_PatientPCLAppointments != value)
                {
                    _PatientPCLAppointments = value;
                    RaisePropertyChanged("PatientPCLAppointments");
                }
            }
        }

     
        
     
        private ObservableCollection<PatientPCLImagingResult> _PatientPCLExamResults;
        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLImagingResult> PatientPCLExamResults
        {
            get
            {
                return _PatientPCLExamResults;
            }
            set
            {
                if (_PatientPCLExamResults != value)
                {
                    _PatientPCLExamResults = value;
                    RaisePropertyChanged("PatientPCLExamResults");
                }
            }
        }

     
        
     
        private PatientRegistration _PatientRegistration;
        [DataMemberAttribute()]
        public PatientRegistration PatientRegistration
        {
            get
            {
                return _PatientRegistration;
            }
            set
            {
                if (_PatientRegistration != value)
                {
                    _PatientRegistration = value;
                    RaisePropertyChanged("PatientRegistration");
                }
            }
        }

     
        
     
        private Staff _Staff;
        [DataMemberAttribute()]
        public Staff Staff
        {
            get
            {
                return _Staff;
            }
            set
            {
                if (_Staff != value)
                {
                    _Staff = value;
                    RaisePropertyChanged("Staff");
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

     
        
     
        private ObservableCollection<RefItem> _RefItems;
        [DataMemberAttribute()]
        public ObservableCollection<RefItem> RefItems
        {
            get
            {
                return _RefItems;
            }
            set
            {
                if (_RefItems != value)
                {
                    _RefItems = value;
                    RaisePropertyChanged("RefItems");
                }
            }
        }

     
        
     
        private ObservableCollection<Surgery> _Surgeries;
        [DataMemberAttribute()]
        public ObservableCollection<Surgery> Surgeries
        {
            get
            {
                return _Surgeries;
            }
            set
            {
                if (_Surgeries != value)
                {
                    _Surgeries = value;
                    RaisePropertyChanged("Surgeries");
                }
            }
        }

     
        
     
        private Lookup _LookupProcessingType;
        [DataMemberAttribute()]
        public Lookup LookupProcessingType
        {
            get
            {
                return _LookupProcessingType;
            }
            set
            {
                if (_LookupProcessingType != value)
                {
                    _LookupProcessingType = value;
                    if (_LookupProcessingType != null)
                        V_ProcessingType = _LookupProcessingType.LookupID;
                    RaisePropertyChanged("LookupProcessingType");
                }
            }
        }

     
        
     
        private Lookup _LookupBehaving;
        [DataMemberAttribute()]
        public Lookup LookupBehaving
        {
            get
            {
                return _LookupBehaving;
            }
            set
            {
                if (_LookupBehaving != value)
                {
                    _LookupBehaving = value;
                    if(_LookupBehaving!=null)
                        V_Behaving = _LookupBehaving.LookupID;
                    RaisePropertyChanged("LookupBehaving");
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
                    if (_LookupBehaving != null)
                        V_DiagnosisType = _LookupDiagnosis.LookupID;
                    RaisePropertyChanged("LookupDiagnosis");
                }
            }
        }
        #endregion
        
        #region IEditableObject Members
        private PatientServiceRecord _tempPatientServiceRecord;
        public void BeginEdit()
        {
            _tempPatientServiceRecord = (PatientServiceRecord)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPatientServiceRecord)
                CopyFrom(_tempPatientServiceRecord);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PatientServiceRecord p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        public override bool Equals(object obj)
        {
            PatientServiceRecord cond = obj as PatientServiceRecord;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ServiceRecID == cond.ServiceRecID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <summary>
        /// 31-08-2012 Dinh
        /// Thêm trạng thái để phân biệt nội trú và ngoại trú
        /// </summary>
        private RegistrationType _RegistrationType;
        [DataMemberAttribute()]
        public RegistrationType RegistrationType
        {
            get
            {
                return _RegistrationType;
            }
            set
            {
                _RegistrationType = value;
                RaisePropertyChanged("RegistrationType");
            }
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
        private bool _IsUpdateSmallProcedureOnly = false;
        [DataMemberAttribute()]
        public bool IsUpdateSmallProcedureOnly
        {
            get
            {
                return _IsUpdateSmallProcedureOnly;
            }
            set
            {
                _IsUpdateSmallProcedureOnly = value;
                RaisePropertyChanged("IsUpdateSmallProcedureOnly");
            }
        }
    }
}
