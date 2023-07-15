using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public class InjuryCertificates : EntityBase
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
        private long _InjuryCertificateID;
        [DataMemberAttribute()]
        public long InjuryCertificateID
        {
            get
            {
                return _InjuryCertificateID;
            }
            set
            {
                _InjuryCertificateID = value;
                RaisePropertyChanged("InjuryCertificateID");
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
        private string _ReasonAdmission;
        [DataMemberAttribute()]
        public string ReasonAdmission
        {
            get
            {
                return _ReasonAdmission;
            }
            set
            {
                _ReasonAdmission = value;
                RaisePropertyChanged("ReasonAdmission");
            }
        }
        private int _InjuryCertificateCode;
        [DataMemberAttribute()]
        public int InjuryCertificateCode
        {
            get
            {
                return _InjuryCertificateCode;
            }
            set
            {
                _InjuryCertificateCode = value;
                RaisePropertyChanged("InjuryCertificateCode");
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
        private string _ClinicalSigns;
        [DataMemberAttribute()]
        public string ClinicalSigns
        {
            get
            {
                return _ClinicalSigns;
            }
            set
            {
                _ClinicalSigns = value;
                RaisePropertyChanged("ClinicalSigns");
            }
        }
        private string _AdmissionStatus;
        [DataMemberAttribute()]
        public string AdmissionStatus
        {
            get
            {
                return _AdmissionStatus;
            }
            set
            {
                _AdmissionStatus = value;
                RaisePropertyChanged("AdmissionStatus");
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
