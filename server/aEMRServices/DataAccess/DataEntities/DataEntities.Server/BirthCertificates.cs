using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public class BirthCertificates : EntityBase
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
        private long _BirthCertificateID;
        [DataMemberAttribute()]
        public long BirthCertificateID
        {
            get
            {
                return _BirthCertificateID;
            }
            set
            {
                _BirthCertificateID = value;
                RaisePropertyChanged("BirthCertificateID");
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
        private long _PtRegistrationID_Child;
        [DataMemberAttribute()]
        public long PtRegistrationID_Child
        {
            get
            {
                return _PtRegistrationID_Child;
            }
            set
            {
                _PtRegistrationID_Child = value;
                RaisePropertyChanged("PtRegistrationID_Child");
            }
        }
        private DateTime _BirthDate;
        [DataMemberAttribute()]
        public DateTime BirthDate
        {
            get
            {
                return _BirthDate;
            }
            set
            {
                _BirthDate = value;
                RaisePropertyChanged("BirthDate");
            }
        }
        private int _BirthCertificateCode;
        [DataMemberAttribute()]
        public int BirthCertificateCode
        {
            get
            {
                return _BirthCertificateCode;
            }
            set
            {
                _BirthCertificateCode = value;
                RaisePropertyChanged("BirthCertificateCode");
            }
        }
        private int _NumOfChild;
        [DataMemberAttribute()]
        public int NumOfChild
        {
            get
            {
                return _NumOfChild;
            }
            set
            {
                _NumOfChild = value;
                RaisePropertyChanged("NumOfChild");
            }
        }
        private int _WeightOfChild;
        [DataMemberAttribute()]
        public int WeightOfChild
        {
            get
            {
                return _WeightOfChild;
            }
            set
            {
                _WeightOfChild = value;
                RaisePropertyChanged("WeightOfChild");
            }
        }
        private string _GenderOfChild;
        [DataMemberAttribute()]
        public string GenderOfChild
        {
            get
            {
                return _GenderOfChild;
            }
            set
            {
                _GenderOfChild = value;
                RaisePropertyChanged("GenderOfChild");
            }
        }
        private string _PlanningName;
        [DataMemberAttribute()]
        public string PlanningName
        {
            get
            {
                return _PlanningName;
            }
            set
            {
                _PlanningName = value;
                RaisePropertyChanged("PlanningName");
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
        private long _V_SurgicalBirth;
        [DataMemberAttribute()]
        public long V_SurgicalBirth
        {
            get
            {
                return _V_SurgicalBirth;
            }
            set
            {
                _V_SurgicalBirth = value;
                RaisePropertyChanged("V_SurgicalBirth");
            }
        }
        private long _V_BirthUnder32;
        [DataMemberAttribute()]
        public long V_BirthUnder32
        {
            get
            {
                return _V_BirthUnder32;
            }
            set
            {
                _V_BirthUnder32 = value;
                RaisePropertyChanged("V_BirthUnder32");
            }
        }
        private string _ChildStatus;
        [DataMemberAttribute()]
        public string ChildStatus
        {
            get
            {
                return _ChildStatus;
            }
            set
            {
                _ChildStatus = value;
                RaisePropertyChanged("ChildStatus");
            }
        }
        private byte _ChildCount;
        [DataMemberAttribute()]
        public byte ChildCount
        {
            get
            {
                return _ChildCount;
            }
            set
            {
                _ChildCount = value;
                RaisePropertyChanged("ChildCount");
            }
        }
        private byte _BirthCount;
        [DataMemberAttribute()]
        public byte BirthCount
        {
            get
            {
                return _BirthCount;
            }
            set
            {
                _BirthCount = value;
                RaisePropertyChanged("BirthCount");
            }
        }

    }
}
