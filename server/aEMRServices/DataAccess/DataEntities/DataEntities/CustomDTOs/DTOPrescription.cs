using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DTOPrescription : NotifyChangedBase
    {
        public DTOPrescription()
            : base()
        {

        }
        #region Factory Method
     
        /// Create a new vPrescription object.
     
        /// <param name="patientRecID">Initial value of the PatientRecID property.</param>
        public static DTOPrescription CreateDTOPrescription(long patientRecID)
        {
            DTOPrescription DTOPrescription = new DTOPrescription();
            DTOPrescription.PatientRecID = patientRecID;
            return DTOPrescription;
        }
        #endregion

        #region Primitive Properties
     
        
     
        [DataMemberAttribute()]
        public long PatientRecID
        {
            get
            {
                return _PatientRecID;
            }
            set
            {
                if (_PatientRecID != value)
                {
                    OnPatientRecIDChanging(value);
                    _PatientRecID = value;
                    RaisePropertyChanged("PatientRecID");
                    OnPatientRecIDChanged();
                }
            }
        }
        private long _PatientRecID;
        partial void OnPatientRecIDChanging(long value);
        partial void OnPatientRecIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<long> PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                OnPatientIDChanging(value);
                _PatientID = value;
                RaisePropertyChanged("PatientID");
                OnPatientIDChanged();
            }
        }
        private Nullable<long> _PatientID;
        partial void OnPatientIDChanging(Nullable<long> value);
        partial void OnPatientIDChanged();

     
        
     
        [DataMemberAttribute()]
        public String NationalMedicalCode
        {
            get
            {
                return _NationalMedicalCode;
            }
            set
            {
                OnNationalMedicalCodeChanging(value);
                _NationalMedicalCode = value;
                RaisePropertyChanged("NationalMedicalCode");
                OnNationalMedicalCodeChanged();
            }
        }
        private String _NationalMedicalCode;
        partial void OnNationalMedicalCodeChanging(String value);
        partial void OnNationalMedicalCodeChanged();

     
        
     
        [DataMemberAttribute()]
        public String PatientRecBarCode
        {
            get
            {
                return _PatientRecBarCode;
            }
            set
            {
                OnPatientRecBarCodeChanging(value);
                _PatientRecBarCode = value;
                RaisePropertyChanged("PatientRecBarCode");
                OnPatientRecBarCodeChanged();
            }
        }
        private String _PatientRecBarCode;
        partial void OnPatientRecBarCodeChanging(String value);
        partial void OnPatientRecBarCodeChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<DateTime> CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                OnCreatedDateChanging(value);
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
                OnCreatedDateChanged();
            }
        }
        private Nullable<DateTime> _CreatedDate;
        partial void OnCreatedDateChanging(Nullable<DateTime> value);
        partial void OnCreatedDateChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<DateTime> FinishedDate
        {
            get
            {
                return _FinishedDate;
            }
            set
            {
                OnFinishedDateChanging(value);
                _FinishedDate = value;
                RaisePropertyChanged("FinishedDate");
                OnFinishedDateChanged();
            }
        }
        private Nullable<DateTime> _FinishedDate;
        partial void OnFinishedDateChanging(Nullable<DateTime> value);
        partial void OnFinishedDateChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Boolean> IsExpiredDate
        {
            get
            {
                return _IsExpiredDate;
            }
            set
            {
                OnIsExpiredDateChanging(value);
                _IsExpiredDate = value;
                RaisePropertyChanged("IsExpiredDate");
                OnIsExpiredDateChanged();
            }
        }
        private Nullable<Boolean> _IsExpiredDate;
        partial void OnIsExpiredDateChanging(Nullable<Boolean> value);
        partial void OnIsExpiredDateChanged();

     
        
     
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
                OnPtRegDetailIDChanged();
            }
        }
        private Nullable<Int64> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<Int64> value);
        partial void OnPtRegDetailIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> DoctorID
        {
            get
            {
                return _DoctorID;
            }
            set
            {
                OnDoctorIDChanging(value);
                _DoctorID = value;
                RaisePropertyChanged("DoctorID");
                OnDoctorIDChanged();
            }
        }
        private Nullable<Int64> _DoctorID;
        partial void OnDoctorIDChanging(Nullable<Int64> value);
        partial void OnDoctorIDChanged();

     
        
     
        [DataMemberAttribute()]
        public String DoctorName
        {
            get
            {
                return _DoctorName;
            }
            set
            {
                OnDoctorNameChanging(value);
                _DoctorName = value;
                RaisePropertyChanged("DoctorName");
                OnDoctorNameChanged();
            }
        }
        private String _DoctorName;
        partial void OnDoctorNameChanging(String value);
        partial void OnDoctorNameChanged();

        [DataMemberAttribute()]
        public String ConsultantDoctorFullName
        {
            get
            {
                return _ConsultantDoctorFullName;
            }
            set
            {
                OnConsultantDoctorFullNameChanging(value);
                _ConsultantDoctorFullName = value;
                RaisePropertyChanged("ConsultantDoctorFullName");
                OnConsultantDoctorFullNameChanged();
            }
        }
        private String _ConsultantDoctorFullName;
        partial void OnConsultantDoctorFullNameChanging(String value);
        partial void OnConsultantDoctorFullNameChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<DateTime> ExamDate
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
        private Nullable<DateTime> _ExamDate;
        partial void OnExamDateChanging(Nullable<DateTime> value);
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
        public String ProcessingType
        {
            get
            {
                return _ProcessingType;
            }
            set
            {
                OnProcessingTypeChanging(value);
                _ProcessingType = value;
                RaisePropertyChanged("ProcessingType");
                OnProcessingTypeChanged();
            }
        }
        private String _ProcessingType;
        partial void OnProcessingTypeChanging(String value);
        partial void OnProcessingTypeChanged();

     
        
     
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
        public String Behaving
        {
            get
            {
                return _Behaving;
            }
            set
            {
                OnBehavingChanging(value);
                _Behaving = value;
                RaisePropertyChanged("Behaving");
                OnBehavingChanged();
            }
        }
        private String _Behaving;
        partial void OnBehavingChanging(String value);
        partial void OnBehavingChanged();

     
        
     
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
        public Nullable<Int32> ApointmentID
        {
            get
            {
                return _ApointmentID;
            }
            set
            {
                OnApointmentIDChanging(value);
                _ApointmentID = value;
                RaisePropertyChanged("ApointmentID");
                OnApointmentIDChanged();
            }
        }
        private Nullable<Int32> _ApointmentID;
        partial void OnApointmentIDChanging(Nullable<Int32> value);
        partial void OnApointmentIDChanged();

     
        
     
        [DataMemberAttribute()]
        public String Dianogsis
        {
            get
            {
                return _Dianogsis;
            }
            set
            {
                OnDianogsisChanging(value);
                _Dianogsis = value;
                RaisePropertyChanged("Dianogsis");
                OnDianogsisChanged();
            }
        }
        private String _Dianogsis;
        partial void OnDianogsisChanging(String value);
        partial void OnDianogsisChanged();

     
        
     
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
                _DoctorAdvice = value;
                RaisePropertyChanged("DoctorAdvice");
                OnDoctorAdviceChanged();
            }
        }
        private String _DoctorAdvice;
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
        public Nullable<Boolean> NeedHoldConsultation
        {
            get
            {
                return _NeedHoldConsultation;
            }
            set
            {
                OnNeedHoldConsultationChanging(value);
                _NeedHoldConsultation = value;
                RaisePropertyChanged("NeedHoldConsultation");
                OnNeedHoldConsultationChanged();
            }
        }
        private Nullable<Boolean> _NeedHoldConsultation;
        partial void OnNeedHoldConsultationChanging(Nullable<Boolean> value);
        partial void OnNeedHoldConsultationChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> V_PrescriptionNotes
        {
            get
            {
                return _V_PrescriptionNotes;
            }
            set
            {
                OnV_PrescriptionNotesChanging(value);
                _V_PrescriptionNotes = value;
                RaisePropertyChanged("V_PrescriptionNotes");
                OnV_PrescriptionNotesChanged();
            }
        }
        private Nullable<Int64> _V_PrescriptionNotes;
        partial void OnV_PrescriptionNotesChanging(Nullable<Int64> value);
        partial void OnV_PrescriptionNotesChanged();

     
        
     
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
                _V_PrescriptionType = value;
                RaisePropertyChanged("V_PrescriptionType");
                OnV_PrescriptionTypeChanged();
            }
        }
        private Nullable<Int64> _V_PrescriptionType;
        partial void OnV_PrescriptionTypeChanging(Nullable<Int64> value);
        partial void OnV_PrescriptionTypeChanged();

     
        
     
        [DataMemberAttribute()]
        public String PrescriptionNotes
        {
            get
            {
                return _PrescriptionNotes;
            }
            set
            {
                OnPrescriptionNotesChanging(value);
                _PrescriptionNotes = value;
                RaisePropertyChanged("PrescriptionNotes");
                OnPrescriptionNotesChanged();
            }
        }
        private String _PrescriptionNotes;
        partial void OnPrescriptionNotesChanging(String value);
        partial void OnPrescriptionNotesChanged();

     
        
     
        [DataMemberAttribute()]
        public String PrescriptionType
        {
            get
            {
                return _PrescriptionType;
            }
            set
            {
                OnPrescriptionTypeChanging(value);
                _PrescriptionType = value;
                RaisePropertyChanged("PrescriptionType");
                OnPrescriptionTypeChanged();
            }
        }
        private String _PrescriptionType;
        partial void OnPrescriptionTypeChanging(String value);
        partial void OnPrescriptionTypeChanged();
        

        [DataMemberAttribute()]
        public Int64 IssuerStaffID
        {
            get
            {
                return _IssuerStaffID;
            }
            set
            {
                OnIssuerStaffIDChanging(value);
                _IssuerStaffID = value;
                RaisePropertyChanged("IssuerStaffID");
                OnIssuerStaffIDChanged();
            }
        }
        private Int64 _IssuerStaffID;
        partial void OnIssuerStaffIDChanging(Int64 value);
        partial void OnIssuerStaffIDChanged();
     

        [DataMemberAttribute()]
        public Nullable<long> ReIssuerStaffID
        {
            get
            {
                return _ReIssuerStaffID;
            }
            set
            {
                OnReIssuerStaffIDChanging(value);
                _ReIssuerStaffID = value;
                RaisePropertyChanged("ReIssuerStaffID");
                OnReIssuerStaffIDChanged();
            }
        }
        private Nullable<long> _ReIssuerStaffID;
        partial void OnReIssuerStaffIDChanging(Nullable<long> value);
        partial void OnReIssuerStaffIDChanged();
        
     
        [DataMemberAttribute()]
        public Nullable<DateTime> IssuedDateTime
        {
            get
            {
                return _IssuedDateTime;
            }
            set
            {
                OnIssuedDateTimeChanging(value);
                _IssuedDateTime = value;
                RaisePropertyChanged("IssuedDateTime");
                OnIssuedDateTimeChanged();
            }
        }
        private Nullable<DateTime> _IssuedDateTime;
        partial void OnIssuedDateTimeChanging(Nullable<DateTime> value);
        partial void OnIssuedDateTimeChanged();
        
     
        [DataMemberAttribute()]
        public String IssuerStaffIDName
        {
            get
            {
                return _IssuerStaffIDName;
            }
            set
            {
                OnIssuerStaffIDNameChanging(value);
                _IssuerStaffIDName = value;
                RaisePropertyChanged("IssuerStaffIDName");
                OnIssuerStaffIDNameChanged();
            }
        }
        private String _IssuerStaffIDName;
        partial void OnIssuerStaffIDNameChanging(String value);
        partial void OnIssuerStaffIDNameChanged();


        [DataMemberAttribute()]
        public String ReIssuerStaffIDName
        {
            get
            {
                return _ReIssuerStaffIDName;
            }
            set
            {
                OnReIssuerStaffIDNameChanging(value);
                _ReIssuerStaffIDName = value;
                RaisePropertyChanged("ReIssuerStaffIDName");
                OnReIssuerStaffIDNameChanged();
            }
        }
        private String _ReIssuerStaffIDName;
        partial void OnReIssuerStaffIDNameChanging(String value);
        partial void OnReIssuerStaffIDNameChanged();
     
        
     
        [DataMemberAttribute()]
        public Nullable<Byte> TimesNumberIsPrinted
        {
            get
            {
                return _TimesNumberIsPrinted;
            }
            set
            {
                OnTimesNumberIsPrintedChanging(value);
                _TimesNumberIsPrinted = value;
                RaisePropertyChanged("TimesNumberIsPrinted");
                OnTimesNumberIsPrintedChanged();
            }
        }
        private Nullable<Byte> _TimesNumberIsPrinted;
        partial void OnTimesNumberIsPrintedChanging(Nullable<Byte> value);
        partial void OnTimesNumberIsPrintedChanged();

     
     
        [DataMemberAttribute()]
        public String ChangeLogs
        {
            get
            {
                return _ChangeLogs;
            }
            set
            {
                OnChangeLogsChanging(value);
                _ChangeLogs = value;
                RaisePropertyChanged("ChangeLogs");
                OnChangeLogsChanged();
            }
        }
        private String _ChangeLogs;
        partial void OnChangeLogsChanging(String value);
        partial void OnChangeLogsChanged();

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public Patient SelectedPatient
        {
            get
            {
                return _SelectedPatient;
            }
            set
            {
                if (_SelectedPatient != value)
                {
                    OnSelectedPatientChanging(value);
                    _SelectedPatient = value;
                    RaisePropertyChanged("SelectedPatient");
                    OnSelectedPatientChanged();
                }
            }
        }
        private Patient _SelectedPatient;
        partial void OnSelectedPatientChanging(Patient value);
        partial void OnSelectedPatientChanged();
        #endregion

    }
}
