using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public class SummaryMedicalRecords : EntityBase
    {
        private HealthInsurance _UsingHealthInsurance;
        [DataMemberAttribute()]
        public HealthInsurance UsingHealthInsurance
        {
            get
            {
                return _UsingHealthInsurance;
            }
            set
            {
                _UsingHealthInsurance = value;
                RaisePropertyChanged("UsingHealthInsurance");
            }
        }
        private PatientRegistration _CurPatientRegistration;
        [DataMemberAttribute()]
        public PatientRegistration CurPatientRegistration
        {
            get
            {
                return _CurPatientRegistration;
            }
            set
            {
                _CurPatientRegistration = value;
                RaisePropertyChanged("CurPatientRegistration");
            }
        }
        private long _SummaryMedicalRecordID;
        [DataMemberAttribute()]
        public long SummaryMedicalRecordID
        {
            get
            {
                return _SummaryMedicalRecordID;
            }
            set
            {
                _SummaryMedicalRecordID = value;
                RaisePropertyChanged("SummaryMedicalRecordID");
            }
        }
        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        private string _AdmissionDiagnosis;
        [DataMemberAttribute()]
        public string AdmissionDiagnosis
        {
            get
            {
                return _AdmissionDiagnosis;
            }
            set
            {
                _AdmissionDiagnosis = value;
                RaisePropertyChanged("AdmissionDiagnosis");
            }
        }
        private string _DischargeDiagnosis;
        [DataMemberAttribute()]
        public string DischargeDiagnosis
        {
            get
            {
                return _DischargeDiagnosis;
            }
            set
            {
                _DischargeDiagnosis = value;
                RaisePropertyChanged("DischargeDiagnosis");
            }
        }
        private string _PathologicalProcess;
        [DataMemberAttribute()]
        public string PathologicalProcess
        {
            get
            {
                return _PathologicalProcess;
            }
            set
            {
                _PathologicalProcess = value;
                RaisePropertyChanged("PathologicalProcess");
            }
        }
        private string _SummaryResultPCL;
        [DataMemberAttribute()]
        public string SummaryResultPCL
        {
            get
            {
                return _SummaryResultPCL;
            }
            set
            {
                _SummaryResultPCL = value;
                RaisePropertyChanged("SummaryResultPCL");
            }
        }
        private string _Treatment;
        [DataMemberAttribute()]
        public string Treatment
        {
            get
            {
                return _Treatment;
            }
            set
            {
                _Treatment = value;
                RaisePropertyChanged("Treatment");
            }
        }
        private string _DischargeStatus;
        [DataMemberAttribute()]
        public string DischargeStatus
        {
            get
            {
                return _DischargeStatus;
            }
            set
            {
                _DischargeStatus = value;
                RaisePropertyChanged("DischargeStatus");
            }
        }
        private string _Note;
        [DataMemberAttribute()]
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
                RaisePropertyChanged("Note");
            }
        }
        private long _V_RegistrationType;
        [DataMemberAttribute()]
        public long V_RegistrationType
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
        private DateTime _CreatedDate;
        [DataMemberAttribute()]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private bool _IsDelete;
        [DataMemberAttribute()]
        public bool IsDelete
        {
            get
            {
                return _IsDelete;
            }
            set
            {
                _IsDelete = value;
                RaisePropertyChanged("IsDelete");
            }
        }
        private long _StaffID;
        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        private string _DoctorStaffName;
        [DataMemberAttribute()]
        public string DoctorStaffName
        {
            get
            {
                return _DoctorStaffName;
            }
            set
            {
                _DoctorStaffName = value;
                RaisePropertyChanged("DoctorStaffName");
            }
        }
        private long _V_OutDischargeCondition;
        [DataMemberAttribute()]
        public long V_OutDischargeCondition
        {
            get
            {
                return _V_OutDischargeCondition;
            }
            set
            {
                _V_OutDischargeCondition = value;
                RaisePropertyChanged("V_OutDischargeCondition");
            }
        }
        private long _ChiefDoctorStaffID;
        [DataMemberAttribute()]
        public long ChiefDoctorStaffID
        {
            get
            {
                return _ChiefDoctorStaffID;
            }
            set
            {
                _ChiefDoctorStaffID = value;
                RaisePropertyChanged("ChiefDoctorStaffID");
            }
        }
        private long _ConfirmHIStaffID;
        [DataMemberAttribute()]
        public long ConfirmHIStaffID
        {
            get
            {
                return _ConfirmHIStaffID;
            }
            set
            {
                _ConfirmHIStaffID = value;
                RaisePropertyChanged("ConfirmHIStaffID");
            }
        }
        
        
    }
}
