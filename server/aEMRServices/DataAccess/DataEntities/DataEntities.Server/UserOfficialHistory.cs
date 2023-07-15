using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class UserOfficialHistory : NotifyChangedBase
    {
        public static UserOfficialHistory CreateUserOfficialHistory(long UOHistoryID, DateTime RecCreatedDate, long OfficialAccountID
            , long LoggedAccountID, long LoggedHistoryID, bool IsDeleted)
        {
            UserOfficialHistory IBE = new UserOfficialHistory
            {
                UOHistoryID = UOHistoryID,
                RecCreatedDate = RecCreatedDate,
                OfficialAccountID = OfficialAccountID,
                LoggedAccountID = LoggedAccountID,
                LoggedHistoryID = LoggedHistoryID,
                IsDeleted = IsDeleted,
            };
            return IBE;
        }
        private long _UOHistoryID;
        [DataMemberAttribute()]
        public long UOHistoryID
        {
            get
            {
                return _UOHistoryID;
            }
            set
            {
                if (_UOHistoryID != value)
                {
                    _UOHistoryID = value;
                    RaisePropertyChanged("UOHistoryID");
                }
            }
        }
        [DataMemberAttribute()]
        public DateTime RecCreatedDate { get; set; }
        private long _OfficialAccountID;
        [DataMemberAttribute()]
        public long OfficialAccountID
        {
            get
            {
                return _OfficialAccountID;
            }
            set
            {
                if (_OfficialAccountID != value)
                {
                    _OfficialAccountID = value;
                    RaisePropertyChanged("OfficialAccountID");
                }
            }
        }
        private long _LoggedAccountID;
        [DataMemberAttribute()]
        public long LoggedAccountID
        {
            get
            {
                return _LoggedAccountID;
            }
            set
            {
                if (_LoggedAccountID != value)
                {
                    _LoggedAccountID = value;
                    RaisePropertyChanged("LoggedAccountID");
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
        private long _LoggedHistoryID;
        [DataMemberAttribute()]
        public long LoggedHistoryID
        {
            get
            {
                return _LoggedHistoryID;
            }
            set
            {
                if (_LoggedHistoryID != value)
                {
                    _LoggedHistoryID = value;
                    RaisePropertyChanged("LoggedHistoryID");
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
    }
}
