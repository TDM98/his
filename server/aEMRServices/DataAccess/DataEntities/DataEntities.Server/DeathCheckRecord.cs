using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
  
    public partial class DeathCheckRecord : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long DeathCheckRecordID
        {
            get
            {
                return _DeathCheckRecordID;
            }
            set
            {
                if (_DeathCheckRecordID == value)
                {
                    return;
                }
                _DeathCheckRecordID = value;
                RaisePropertyChanged("DeathCheckRecordID");
            }
        }
        private long _DeathCheckRecordID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                {
                    return;
                }
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID == value)
                {
                    return;
                }
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private long _PatientID;

        [DataMemberAttribute]
        public string MedicalCode
        {
            get
            {
                return _MedicalCode;
            }
            set
            {
                if (_MedicalCode == value)
                {
                    return;
                }
                _MedicalCode = value;
                RaisePropertyChanged("MedicalCode");
            }
        }
        private string _MedicalCode;

        [DataMemberAttribute]
        public DateTime CheckRecordDate
        {
            get
            {
                return _CheckRecordDate;
            }
            set
            {
                if (_CheckRecordDate == value)
                {
                    return;
                }
                _CheckRecordDate = value;
                RaisePropertyChanged("CheckRecordDate");
            }
        }
        private DateTime _CheckRecordDate;

        [DataMemberAttribute]
        public long PresideStaffID
        {
            get
            {
                return _PresideStaffID;
            }
            set
            {
                if (_PresideStaffID == value)
                {
                    return;
                }
                _PresideStaffID = value;
                RaisePropertyChanged("PresideStaffID");
            }
        }
        private long _PresideStaffID;

        [DataMemberAttribute]
        public long SecretaryStaffID
        {
            get
            {
                return _SecretaryStaffID;
            }
            set
            {
                if (_SecretaryStaffID == value)
                {
                    return;
                }
                _SecretaryStaffID = value;
                RaisePropertyChanged("SecretaryStaffID");
            }
        }
        private long _SecretaryStaffID;

        [DataMemberAttribute]
        public string MemberStaff
        {
            get
            {
                return _MemberStaff;
            }
            set
            {
                if (_MemberStaff == value)
                {
                    return;
                }
                _MemberStaff = value;
                RaisePropertyChanged("MemberStaff");
            }
        }
        private string _MemberStaff;

        [DataMemberAttribute]
        public string TreatmentProcess
        {
            get
            {
                return _TreatmentProcess;
            }
            set
            {
                if (_TreatmentProcess == value)
                {
                    return;
                }
                _TreatmentProcess = value;
                RaisePropertyChanged("TreatmentProcess");
            }
        }
        private string _TreatmentProcess;

        [DataMemberAttribute]
        public string Conclude
        {
            get
            {
                return _Conclude;
            }
            set
            {
                if (_Conclude == value)
                {
                    return;
                }
                _Conclude = value;
                RaisePropertyChanged("Conclude");
            }
        }
        private string _Conclude;

        [DataMemberAttribute]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted == value)
                {
                    return;
                }
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private bool _IsDeleted;

        [DataMemberAttribute]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID == value)
                {
                    return;
                }
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private long _CreatedStaffID;

        [DataMemberAttribute]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID == value)
                {
                    return;
                }
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        private long _StaffID;

        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _CreatedDate;

        [DataMemberAttribute]
        public DateTime DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                if (_DateModified == value)
                {
                    return;
                }
                _DateModified = value;
                RaisePropertyChanged("DateModified");
            }
        }
        private DateTime _DateModified;

        [DataMemberAttribute]
        public string ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                if (_ModifiedLog == value)
                {
                    return;
                }
                _ModifiedLog = value;
                RaisePropertyChanged("ModifiedLog");
            }
        }
        private string _ModifiedLog;

        #endregion
    }
}
