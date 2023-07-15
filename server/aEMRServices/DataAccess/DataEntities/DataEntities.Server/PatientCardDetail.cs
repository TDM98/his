using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class PatientCardDetail : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public long PatientCardDetailID
        {
            get
            {
                return _PatientCardDetailID;
            }
            set
            {
                _PatientCardDetailID = value;
                RaisePropertyChanged("PatientCardDetailID");
            }
        }
        private long _PatientCardDetailID;

        [DataMemberAttribute()]
        public long CardID
        {
            get
            {
                return _CardID;
            }
            set
            {
                _CardID = value;
                RaisePropertyChanged("CardID");
            }
        }
        private long _CardID;

        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private long _PatientID;

        [DataMemberAttribute()]
        public string AccountNumber
        {
            get
            {
                return _AccountNumber;
            }
            set
            {
                _AccountNumber = value;
                RaisePropertyChanged("AccountNumber");
            }
        }
        private string _AccountNumber;

        [DataMemberAttribute()]
        public long V_PatientClass
        {
            get
            {
                return _V_PatientClass;
            }
            set
            {
                _V_PatientClass = value;
                RaisePropertyChanged("V_PatientClass");
            }
        }
        private long _V_PatientClass;

        [DataMemberAttribute()]
        public DateTime? OpenCardDate
        {
            get
            {
                return _OpenCardDate;
            }
            set
            {
                _OpenCardDate = value;
                RaisePropertyChanged("OpenCardDate");
            }
        }
        private DateTime? _OpenCardDate;

        [DataMemberAttribute()]
        public DateTime? ExpireCardDate
        {
            get
            {
                return _ExpireCardDate;
            }
            set
            {
                _ExpireCardDate = value;
                RaisePropertyChanged("ExpireCardDate");
            }
        }
        private DateTime? _ExpireCardDate;

        [DataMemberAttribute()]
        public long CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                _CreatedStaff = value;
                RaisePropertyChanged("CreatedStaff");
            }
        }
        private long _CreatedStaff;
        [DataMemberAttribute()]
        public string Logmodified
        {
            get
            {
                return _Logmodified;
            }
            set
            {
                _Logmodified = value;
                RaisePropertyChanged("Logmodified");
            }
        }
        private string _Logmodified;

    }
}
