using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class UserLoginHistory : NotifyChangedBase
    {
        #region Factory Method
        
        /// Create a new UserLoginHistory object.

        /// <param name="LoggedHistoryID">Initial value of the LoggedHistoryID property.</param>
        /// <param name="accountID">Initial value of the AccountID property.</param>
        /// <param name="logDateTime">Initial value of the LogDateTime property.</param>
        public static UserLoginHistory CreateUserLoginHistory(long LoggedHistoryID, long accountID, DateTime logDateTime)
        {
            UserLoginHistory UserLoginHistory = new UserLoginHistory();
            UserLoginHistory.LoggedHistoryID = LoggedHistoryID;
            UserLoginHistory.AccountID = accountID;
            UserLoginHistory.LogDateTime = logDateTime;
            return UserLoginHistory;
        }

        #endregion

        #region Primitive Properties


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
                    OnLoggedHistoryIDChanging(value);
                    ////ReportPropertyChanging("LoggedHistoryID");
                    _LoggedHistoryID = value;
                    RaisePropertyChanged("LoggedHistoryID");
                    OnLoggedHistoryIDChanged();
                }
            }
        }
        private long _LoggedHistoryID;
        partial void OnLoggedHistoryIDChanging(long value);
        partial void OnLoggedHistoryIDChanged();

        [DataMemberAttribute()]
        public long AccountID
        {
            get
            {
                return _AccountID;
            }
            set
            {
                OnAccountIDChanging(value);
                ////ReportPropertyChanging("AccountID");
                _AccountID = value;
                RaisePropertyChanged("AccountID");
                OnAccountIDChanged();
            }
        }
        private long _AccountID;
        partial void OnAccountIDChanging(long value);
        partial void OnAccountIDChanged();


        [DataMemberAttribute()]
        public Nullable<DateTime> LogDateTime
        {
            get
            {
                return _LogDateTime;
            }
            set
            {
                OnLogDateTimeChanging(value);
                ////ReportPropertyChanging("LogDateTime");
                _LogDateTime = value;
                RaisePropertyChanged("LogDateTime");
                OnLogDateTimeChanged();
            }
        }
        private Nullable<DateTime> _LogDateTime;
        partial void OnLogDateTimeChanging(Nullable<DateTime> value);
        partial void OnLogDateTimeChanged();

        [DataMemberAttribute()]
        public string HostName
        {
            get
            {
                return _HostName;
            }
            set
            {
                OnHostNameChanging(value);
                ////ReportPropertyChanging("HostName");
                _HostName = value;
                RaisePropertyChanged("HostName");
                OnHostNameChanged();
            }
        }
        private string _HostName;
        partial void OnHostNameChanging(string value);
        partial void OnHostNameChanged();

        [DataMemberAttribute()]
        public string tempHostName
        {
            get
            {
                return _tempHostName;
            }
            set
            {
                OntempHostNameChanging(value);
                ////ReportPropertyChanging("tempHostName");
                _tempHostName = value;
                RaisePropertyChanged("tempHostName");
                OntempHostNameChanged();
            }
        }
        private string _tempHostName;
        partial void OntempHostNameChanging(string value);
        partial void OntempHostNameChanged();

        [DataMemberAttribute()]
        public string HostIPV4
        {
            get
            {
                return _HostIPV4;
            }
            set
            {
                OnHostIPV4Changing(value);
                ////ReportPropertyChanging("HostIPV4");
                _HostIPV4 = value;
                RaisePropertyChanged("HostIPV4");
                OnHostIPV4Changed();
            }
        }
        private string _HostIPV4;
        partial void OnHostIPV4Changing(string value);
        partial void OnHostIPV4Changed();
        #endregion

        #region Navigation Properties
        
        [DataMemberAttribute()]
        public UserAccount UserAccount
        {
            get
            {
                return _UserAccount;
            }
            set
            {
                OnUserAccountChanging(value);
                _UserAccount = value;
                RaisePropertyChanged("UserAccount");
                OnUserAccountChanged();
            }
        }
        private UserAccount _UserAccount;
        partial void OnUserAccountChanging(UserAccount value);
        partial void OnUserAccountChanged();
        #endregion

        public override bool Equals(object obj)
        {
            UserLoginHistory info = obj as UserLoginHistory;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.LoggedHistoryID > 0 && this.LoggedHistoryID == info.LoggedHistoryID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
