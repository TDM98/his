using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class ManagementUserOfficial : NotifyChangedBase
    {
        public static ManagementUserOfficial CreateManagementUserOfficial(long ManagementUserOfficialID
            , DateTime RecCreatedDate, DateTime FromDate, DateTime? ToDate
            , int PatientFindBy, long LoginUserID, long UserOfficialID, long StaffID, bool IsDeleted)
        {
            ManagementUserOfficial IBE = new ManagementUserOfficial
            {
                ManagementUserOfficialID = ManagementUserOfficialID,
                RecCreatedDate = RecCreatedDate,
                FromDate = FromDate,
                ToDate = ToDate,
                PatientFindBy = PatientFindBy,
                LoginUserID = LoginUserID,
                UserOfficialID = UserOfficialID,
                StaffID = StaffID,
                IsDeleted = IsDeleted,
            };
            return IBE;
        }
        private long _ManagementUserOfficialID;
        [DataMemberAttribute()]
        public long ManagementUserOfficialID
        {
            get
            {
                return _ManagementUserOfficialID;
            }
            set
            {
                if (_ManagementUserOfficialID != value)
                {
                    _ManagementUserOfficialID = value;
                    RaisePropertyChanged("ManagementUserOfficialID");
                }
            }
        }
        [DataMemberAttribute()]
        public DateTime RecCreatedDate { get; set; }
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
                if (_FromDate != value)
                {
                    _FromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }
        private DateTime? _ToDate;
        [DataMemberAttribute()]
        public DateTime? ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }
        private int _PatientFindBy;
        [DataMemberAttribute()]
        public int PatientFindBy
        {
            get
            {
                return _PatientFindBy;
            }
            set
            {
                if (_PatientFindBy != value)
                {
                    _PatientFindBy = value;
                    RaisePropertyChanged("PatientFindBy");
                }
            }
        }
        private string _PatientFindByStr;
        [DataMemberAttribute()]
        public string PatientFindByStr
        {
            get
            {
                return _PatientFindByStr;
            }
            set
            {
                if (_PatientFindByStr != value)
                {
                    _PatientFindByStr = value;
                    RaisePropertyChanged("PatientFindByStr");
                }
            }
        }
        private long _LoginUserID;
        [DataMemberAttribute()]
        public long LoginUserID
        {
            get
            {
                return _LoginUserID;
            }
            set
            {
                if (_LoginUserID != value)
                {
                    _LoginUserID = value;
                    RaisePropertyChanged("LoginUserID");
                }
            }
        }
        private long _UserOfficialID;
        [DataMemberAttribute()]
        public long UserOfficialID
        {
            get
            {
                return _UserOfficialID;
            }
            set
            {
                if (_UserOfficialID != value)
                {
                    _UserOfficialID = value;
                    RaisePropertyChanged("UserOfficialID");
                }
            }
        }
        private Staff _OfficialAccount;
        [DataMemberAttribute()]
        public Staff OfficialAccount
        {
            get
            {
                return _OfficialAccount;
            }
            set
            {
                if (_OfficialAccount != value)
                {
                    _OfficialAccount = value;
                    RaisePropertyChanged("OfficialAccount");
                }
            }
        }
        private Staff _CreatedStaff;
        [DataMemberAttribute()]
        public Staff CreatedStaff
        {
            get
            {
                return _CreatedStaff;
            }
            set
            {
                if (_CreatedStaff != value)
                {
                    _CreatedStaff = value;
                    RaisePropertyChanged("CreatedStaff");
                }
            }
        }
        private Staff _LoginUserStaff;
        [DataMemberAttribute()]
        public Staff LoginUserStaff
        {
            get
            {
                return _LoginUserStaff;
            }
            set
            {
                if (_LoginUserStaff != value)
                {
                    _LoginUserStaff = value;
                    RaisePropertyChanged("LoginUserStaff");
                }
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
                if (_StaffID != value)
                {
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }
        private bool _IsDeleted;
        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                if (_IsDeleted != value)
                {
                    _IsDeleted = value;
                    RaisePropertyChanged("IsDeleted");
                }
            }
        }

        private bool _CanDelete;
        [DataMemberAttribute()]
        public bool CanDelete
        {
            get
            {
                return _CanDelete;
            }
            set
            {
                if (_CanDelete != value)
                {
                    _CanDelete = value;
                    RaisePropertyChanged("CanDelete");
                }
            }
        }
    }
}
