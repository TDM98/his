using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public class VacationInsuranceCertificates : EntityBase
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
        private long _VacationInsuranceCertificateID;
        [DataMemberAttribute()]
        public long VacationInsuranceCertificateID
        {
            get
            {
                return _VacationInsuranceCertificateID;
            }
            set
            {
                _VacationInsuranceCertificateID = value;
                RaisePropertyChanged("VacationInsuranceCertificateID");
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
        private DateTime _FromDate;
        [DataMemberAttribute()]
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                RaisePropertyChanged("FromDate");
            }
        }
        private DateTime _ToDate;
        [DataMemberAttribute()]
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                RaisePropertyChanged("ToDate");
            }
        }
        private int _SeriNumber;
        [DataMemberAttribute()]
        public int SeriNumber
        {
            get
            {
                return _SeriNumber;
            }
            set
            {
                _SeriNumber = value;
                RaisePropertyChanged("SeriNumber");
            }
        }
        private String _VacationInsuranceCertificateCode;
        [DataMemberAttribute()]
        public String VacationInsuranceCertificateCode
        {
            get
            {
                return _VacationInsuranceCertificateCode;
            }
            set
            {
                _VacationInsuranceCertificateCode = value;
                RaisePropertyChanged("VacationInsuranceCertificateCode");
            }
        }
        private String _Diagnosis;
        [DataMemberAttribute()]
        public String Diagnosis
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
        private string _FatherName;
        [DataMemberAttribute()]
        public string FatherName
        {
            get
            {
                return _FatherName;
            }
            set
            {
                _FatherName = value;
                RaisePropertyChanged("FatherName");
            }
        }
        private string _MotherName;
        [DataMemberAttribute()]
        public string MotherName
        {
            get
            {
                return _MotherName;
            }
            set
            {
                _MotherName = value;
                RaisePropertyChanged("MotherName");
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
        private bool _IsInsurance;
        [DataMemberAttribute()]
        public bool IsInsurance
        {
            get
            {
                return _IsInsurance;
            }
            set
            {
                _IsInsurance = value;
                RaisePropertyChanged("IsInsurance");
            }
        }
        private string _SeriNumberText;
        [DataMemberAttribute()]
        public string SeriNumberText
        {
            get
            {
                return _SeriNumberText;
            }
            set
            {
                _SeriNumberText = value;
                RaisePropertyChanged("SeriNumberText");
            }
        }
        private string _DocumentNumber;
        [DataMemberAttribute()]
        public string DocumentNumber
        {
            get
            {
                return _DocumentNumber;
            }
            set
            {
                _DocumentNumber = value;
                RaisePropertyChanged("DocumentNumber");
            }
        }
        private string _MedicalNumber;
        [DataMemberAttribute()]
        public string MedicalNumber
        {
            get
            {
                return _MedicalNumber;
            }
            set
            {
                _MedicalNumber = value;
                RaisePropertyChanged("MedicalNumber");
            }
        }
        private long _CheifDoctorStaffID;
        [DataMemberAttribute()]
        public long CheifDoctorStaffID
        {
            get
            {
                return _CheifDoctorStaffID;
            }
            set
            {
                _CheifDoctorStaffID = value;
                RaisePropertyChanged("CheifDoctorStaffID");
            }
        }
        private bool _IsSuspendedPregnant;
        [DataMemberAttribute()]
        public bool IsSuspendedPregnant
        {
            get
            {
                return _IsSuspendedPregnant;
            }
            set
            {
                _IsSuspendedPregnant = value;
                RaisePropertyChanged("IsSuspendedPregnant");
            }
        }
        private string _ReasonSuspendedPregnant;
        [DataMemberAttribute()]
        public string ReasonSuspendedPregnant
        {
            get
            {
                return _ReasonSuspendedPregnant;
            }
            set
            {
                _ReasonSuspendedPregnant = value;
                RaisePropertyChanged("ReasonSuspendedPregnant");
            }
        }
        private int _NumOfDayLeave;
        [DataMemberAttribute()]
        public int NumOfDayLeave
        {
            get
            {
                return _NumOfDayLeave;
            }
            set
            {
                _NumOfDayLeave = value;
                RaisePropertyChanged("NumOfDayLeave");
            }
        }
        private string _TreatmentMethod;
        [DataMemberAttribute()]
        public string TreatmentMethod
        {
            get
            {
                return _TreatmentMethod;
            }
            set
            {
                _TreatmentMethod = value;
                RaisePropertyChanged("TreatmentMethod");
            }
        }
        private bool _ChildUnder7;
        [DataMemberAttribute()]
        public bool ChildUnder7
        {
            get
            {
                return _ChildUnder7;
            }
            set
            {
                _ChildUnder7 = value;
                RaisePropertyChanged("ChildUnder7");
            }
        }
        private bool _IsTuberculosis;
        [DataMemberAttribute()]
        public bool IsTuberculosis
        {
            get
            {
                return _IsTuberculosis;
            }
            set
            {
                _IsTuberculosis = value;
                RaisePropertyChanged("IsTuberculosis");
            }
        }
        private string _PatientCode;
        [DataMemberAttribute()]
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }
        private string _PatientName;
        [DataMemberAttribute()]
        public string PatientName
        {
            get
            {
                return _PatientName;
            }
            set
            {
                _PatientName = value;
                RaisePropertyChanged("PatientName");
            }
        }
        private DateTime _DOB;
        [DataMemberAttribute()]
        public DateTime DOB
        {
            get
            {
                return _DOB;
            }
            set
            {
                _DOB = value;
                RaisePropertyChanged("DOB");
            }
        }
        private string _Age;
        [DataMemberAttribute()]
        public string Age
        {
            get
            {
                return _Age;
            }
            set
            {
                _Age = value;
                RaisePropertyChanged("Age");
            }
        }
        private string _Gender;
        [DataMemberAttribute()]
        public string Gender
        {
            get
            {
                return _Gender;
            }
            set
            {
                _Gender = value;
                RaisePropertyChanged("Gender");
            }
        }
        private string _PatientEmployer;
        [DataMemberAttribute()]
        public string PatientEmployer
        {
            get
            {
                return _PatientEmployer;
            }
            set
            {
                _PatientEmployer = value;
                RaisePropertyChanged("PatientEmployer");
            }
        }
        private string _HICardNo;
        [DataMemberAttribute()]
        public string HICardNo
        {
            get
            {
                return _HICardNo;
            }
            set
            {
                _HICardNo = value;
                RaisePropertyChanged("HICardNo");
            }
        }
        private string _SocialInsuranceNumber;
        [DataMemberAttribute()]
        public string SocialInsuranceNumber
        {
            get
            {
                return _SocialInsuranceNumber;
            }
            set
            {
                _SocialInsuranceNumber = value;
                RaisePropertyChanged("SocialInsuranceNumber");
            }
        }
        private string _PatientStreetAddress;
        [DataMemberAttribute()]
        public string PatientStreetAddress
        {
            get
            {
                return _PatientStreetAddress;
            }
            set
            {
                _PatientStreetAddress = value;
                RaisePropertyChanged("PatientStreetAddress");
            }
        }
        private DateTime _AdmissionDate;
        [DataMemberAttribute()]
        public DateTime AdmissionDate
        {
            get
            {
                return _AdmissionDate;
            }
            set
            {
                _AdmissionDate = value;
                RaisePropertyChanged("AdmissionDate");
            }
        }
        private DateTime _DischargeDate;
        [DataMemberAttribute()]
        public DateTime DischargeDate
        {
            get
            {
                return _DischargeDate;
            }
            set
            {
                _DischargeDate = value;
                RaisePropertyChanged("DischargeDate");
            }
        }
        private long _DoctorStaffID;
        [DataMemberAttribute()]
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }
        private int _GestationalAge;
        [DataMemberAttribute()]
        public int GestationalAge
        {
            get
            {
                return _GestationalAge;
            }
            set
            {
                _GestationalAge = value;
                RaisePropertyChanged("GestationalAge");
            }
        }
        private string _HICardNoTemp;
        [DataMemberAttribute()]
        public string HICardNoTemp
        {
            get
            {
                return _HICardNoTemp;
            }
            set
            {
                _HICardNoTemp = value;
                RaisePropertyChanged("HICardNoTemp");
            }
        }

    }
}
