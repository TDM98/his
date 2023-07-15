using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
/*
 * 20221128 #001 TNHX: Lấy đường dẫn file pdf kết quả ký số
 * 20230518 #002 DatTB: Thêm các trường lưu bệnh phẩm xét nghiệm
 */
namespace DataEntities
{
    public partial class PatientPCLLaboratoryResult : NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new PatientPCLLaboratoryResult object.
     
        /// <param name="labResultID">Initial value of the LabResultID property.</param>
        /// <param name="samplingDate">Initial value of the SamplingDate property.</param>
        public static PatientPCLLaboratoryResult CreatePatientPCLLaboratoryResult(Int64 labResultID, DateTime samplingDate)
        {
            PatientPCLLaboratoryResult patientPCLLaboratoryResult = new PatientPCLLaboratoryResult();
            patientPCLLaboratoryResult.LabResultID = labResultID;
            patientPCLLaboratoryResult.SamplingDate = samplingDate;
            return patientPCLLaboratoryResult;
        }

        #endregion
        #region Primitive Properties
     
        [DataMemberAttribute()]
        public Int64 LabResultID
        {
            get
            {
                return _LabResultID;
            }
            set
            {
                if (_LabResultID != value)
                {
                    OnLabResultIDChanging(value);
                    _LabResultID = value;
                    RaisePropertyChanged("LabResultID");
                    OnLabResultIDChanged();
                }
            }
        }
        private Int64 _LabResultID;
        partial void OnLabResultIDChanging(Int64 value);
        partial void OnLabResultIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Int64 PCLExtRefID
        {
            get
            {
                return _PCLExtRefID;
            }
            set
            {
                OnPCLExtRefIDChanging(value);
                _PCLExtRefID = value;
                RaisePropertyChanged("PCLExtRefID");
                OnPCLExtRefIDChanged();
            }
        }
        private Int64 _PCLExtRefID;
        partial void OnPCLExtRefIDChanging(Int64 value);
        partial void OnPCLExtRefIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                OnPatientPCLReqIDChanging(value);
                _PatientPCLReqID = value;
                RaisePropertyChanged("PatientPCLReqID");
                OnPatientPCLReqIDChanged();
            }
        }
        private Nullable<Int64> _PatientPCLReqID;
        partial void OnPatientPCLReqIDChanging(Nullable<Int64> value);
        partial void OnPatientPCLReqIDChanged();

     
        
     
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
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> AgencyID
        {
            get
            {
                return _AgencyID;
            }
            set
            {
                OnAgencyIDChanging(value);
                _AgencyID = value;
                RaisePropertyChanged("AgencyID");
                OnAgencyIDChanged();
            }
        }
        private Nullable<Int64> _AgencyID;
        partial void OnAgencyIDChanging(Nullable<Int64> value);
        partial void OnAgencyIDChanged();

     
        
     
        [DataMemberAttribute()]
        public DateTime SamplingDate
        {
            get
            {
                return _SamplingDate;
            }
            set
            {
                OnSamplingDateChanging(value);
                _SamplingDate = value;
                RaisePropertyChanged("SamplingDate");
                OnSamplingDateChanged();
            }
        }
        private DateTime _SamplingDate;
        partial void OnSamplingDateChanging(DateTime value);
        partial void OnSamplingDateChanged();

     
        
     
        [DataMemberAttribute()]
        public String DiagnosisOnExam
        {
            get
            {
                return _DiagnosisOnExam;
            }
            set
            {
                OnDiagnosisOnExamChanging(value);
                _DiagnosisOnExam = value;
                RaisePropertyChanged("DiagnosisOnExam");
                OnDiagnosisOnExamChanged();
            }
        }
        private String _DiagnosisOnExam;
        partial void OnDiagnosisOnExamChanging(String value);
        partial void OnDiagnosisOnExamChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Boolean> PCLForOutPatient
        {
            get
            {
                return _PCLForOutPatient;
            }
            set
            {
                OnPCLForOutPatientChanging(value);
                _PCLForOutPatient = value;
                RaisePropertyChanged("PCLForOutPatient");
                OnPCLForOutPatientChanged();
            }
        }
        private Nullable<Boolean> _PCLForOutPatient;
        partial void OnPCLForOutPatientChanging(Nullable<Boolean> value);
        partial void OnPCLForOutPatientChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Boolean> IsExternalExam
        {
            get
            {
                return _IsExternalExam;
            }
            set
            {
                OnIsExternalExamChanging(value);
                _IsExternalExam = value;
                RaisePropertyChanged("IsExternalExam");
                OnIsExternalExamChanged();
            }
        }
        private Nullable<Boolean> _IsExternalExam;
        partial void OnIsExternalExamChanging(Nullable<Boolean> value);
        partial void OnIsExternalExamChanged();


     
        
     
        [DataMemberAttribute()]
        public String SampleCode
        {
            get
            {
                return _SampleCode;
            }
            set
            {
                OnSampleCodeChanging(value);
                _SampleCode = value;
                RaisePropertyChanged("SampleCode");
                OnSampleCodeChanged();
            }
        }
        private String _SampleCode;
        partial void OnSampleCodeChanging(String value);
        partial void OnSampleCodeChanged();


        
        [DataMemberAttribute()]
        public long MedSpecID
        {
            get
            {
                return _MedSpecID;
            }
            set
            {
                OnMedSpecIDChanging(value);
                _MedSpecID = value;
                RaisePropertyChanged("MedSpecID");
                OnMedSpecIDChanged();
            }
        }
        private long _MedSpecID;
        partial void OnMedSpecIDChanging(long value);
        partial void OnMedSpecIDChanged();
        #endregion

        #region Navigation Properties

     
        
     
        [DataMemberAttribute()]
        public PatientPCLRequest PatientPCLRequest
        {
            get;
            set;
        }

     
        
     
        [DataMemberAttribute()]
        public PCLForExternalRef PCLForExternalRef
        {
            get;
            set;
        }

     
        
     
        [DataMemberAttribute()]
        public Staff Staff
        {
            get;
            set;
        }

     
        
     
        [DataMemberAttribute()]
        public TestingAgency TestingAgency
        {
            get;
            set;
        }

     
        
     
        [DataMemberAttribute()]
        public MedicalSpecimen MedicalSpecimen
        {
            get;
            set;
        }

        #endregion

        private string _Suggest;
        [DataMemberAttribute]
        public string Suggest
        {
            get { return _Suggest; }
            set
            {
                _Suggest = value;
                RaisePropertyChanged("Suggest");
            }
        }

        //▼====: #001
        private string _DigitalSignatureResultPath;
        [DataMemberAttribute]
        public string DigitalSignatureResultPath
        {
            get { return _DigitalSignatureResultPath; }
            set
            {
                _DigitalSignatureResultPath = value;
                RaisePropertyChanged("DigitalSignatureResultPath");
            }
        }
        //▲====: #001

        //▼==== #002
        private long _SpecimenID;
        [DataMemberAttribute]
        public long SpecimenID
        {
            get { return _SpecimenID; }
            set
            {
                if (_SpecimenID != value)
                {
                    _SpecimenID = value;
                    RaisePropertyChanged("SpecimenID");
                }
            }
        }

        private string _SampleQuality;
        [DataMemberAttribute]
        public string SampleQuality
        {
            get { return _SampleQuality; }
            set
            {
                if (_SampleQuality != value)
                {
                    _SampleQuality = value;
                    RaisePropertyChanged("SampleQuality");
                }
            }
        }
        //▲==== #002
    }
}
