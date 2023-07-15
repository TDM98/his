using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public class PatientTreatmentCertificates : EntityBase
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
        private long _PatientTreatmentCertificateID;
        [DataMemberAttribute()]
        public long PatientTreatmentCertificateID
        {
            get
            {
                return _PatientTreatmentCertificateID;
            }
            set
            {
                _PatientTreatmentCertificateID = value;
                RaisePropertyChanged("PatientTreatmentCertificateID");
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
        private int _PatientTreatmentCertificateCode;
        [DataMemberAttribute()]
        public int PatientTreatmentCertificateCode
        {
            get
            {
                return _PatientTreatmentCertificateCode;
            }
            set
            {
                _PatientTreatmentCertificateCode = value;
                RaisePropertyChanged("PatientTreatmentCertificateCode");
            }
        }
        private string _Diagnosis;
        [DataMemberAttribute()]
        public string Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                _Diagnosis = value;
                RaisePropertyChanged("Diagnosis");
            }
        }
        private string _MedicalHistory;
        [DataMemberAttribute()]
        public string MedicalHistory
        {
            get
            {
                return _MedicalHistory;
            }
            set
            {
                _MedicalHistory = value;
                RaisePropertyChanged("MedicalHistory");
            }
        }
        private string _DoctorAdvice;
        [DataMemberAttribute()]
        public string DoctorAdvice
        {
            get
            {
                return _DoctorAdvice;
            }
            set
            {
                _DoctorAdvice = value;
                RaisePropertyChanged("DoctorAdvice");
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
    }
}
