using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class DTODiagnosisAndTreatment : NotifyChangedBase
    {
        public DTODiagnosisAndTreatment()
            : base()
        {

        }

        #region Factory Method

     
        /// Create a new vDiagnosisAndTreatment object.
     
        /// <param name="patientRecID">Initial value of the PatientRecID property.</param>
        public static DTODiagnosisAndTreatment CreateDTODiagnosisAndTreatment(long patientRecID)
        {
            DTODiagnosisAndTreatment DTODiagnosisAndTreatment = new DTODiagnosisAndTreatment();
            DTODiagnosisAndTreatment.PatientRecID = patientRecID;
            return DTODiagnosisAndTreatment;
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
        public Nullable<long> DTItemID
        {
            get
            {
                return _DTItemID;
            }
            set
            {
                OnDTItemIDChanging(value);
                _DTItemID = value;
                RaisePropertyChanged("DTItemID");
                OnDTItemIDChanged();
            }
        }
        private Nullable<long> _DTItemID;
        partial void OnDTItemIDChanging(Nullable<long> value);
        partial void OnDTItemIDChanged();

     
        
     
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
        private String _Diagnosis;
        partial void OnDiagnosisChanging(String value);
        partial void OnDiagnosisChanged();

     
        
     
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
                _OrientedTreatment = value;
                RaisePropertyChanged("OrientedTreatment");
                OnOrientedTreatmentChanged();
            }
        }
        private String _OrientedTreatment;
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
                _DoctorComments = value;
                RaisePropertyChanged("DoctorComments");
                OnDoctorCommentsChanged();
            }
        }
        private String _DoctorComments;
        partial void OnDoctorCommentsChanging(String value);
        partial void OnDoctorCommentsChanged();

     
        
     
        [DataMemberAttribute()]
        public String ICD10Code
        {
            get
            {
                return _ICD10Code;
            }
            set
            {
                OnICD10CodeChanging(value);
                _ICD10Code = value;
                RaisePropertyChanged("ICD10Code");
                OnICD10CodeChanged();
            }
        }
        private String _ICD10Code;
        partial void OnICD10CodeChanging(String value);
        partial void OnICD10CodeChanged();

     
        
     
        [DataMemberAttribute()]
        public String DiseaseName
        {
            get
            {
                return _DiseaseName;
            }
            set
            {
                OnDiseaseNameChanging(value);
                _DiseaseName = value;
                RaisePropertyChanged("DiseaseName");
                OnDiseaseNameChanged();
            }
        }
        private String _DiseaseName;
        partial void OnDiseaseNameChanging(String value);
        partial void OnDiseaseNameChanged();

     
        
     
        [DataMemberAttribute()]
        public String DiseaseNameVN
        {
            get
            {
                return _DiseaseNameVN;
            }
            set
            {
                OnDiseaseNameVNChanging(value);
                _DiseaseNameVN = value;
                RaisePropertyChanged("DiseaseNameVN");
                OnDiseaseNameVNChanged();
            }
        }
        private String _DiseaseNameVN;
        partial void OnDiseaseNameVNChanging(String value);
        partial void OnDiseaseNameVNChanged();

        #endregion

    }
}
