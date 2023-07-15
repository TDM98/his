using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class UserLogin : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new UserLogin object.

        /// <param name="loggedItemID">Initial value of the LoggedItemID property.</param>
        /// <param name="accountID">Initial value of the AccountID property.</param>
        /// <param name="logDateTime">Initial value of the LogDateTime property.</param>
        public static UserLogin CreateUserLogin(long loggedItemID, long accountID, DateTime logDateTime)
        {
            UserLogin userLogin = new UserLogin();
            userLogin.LoggedItemID = loggedItemID;
            userLogin.AccountID = accountID;
            userLogin.LogDateTime = logDateTime;
            return userLogin;
        }

        #endregion
        #region Primitive Properties


        [DataMemberAttribute()]
        public long LoggedItemID
        {
            get
            {
                return _LoggedItemID;
            }
            set
            {
                if (_LoggedItemID != value)
                {
                    OnLoggedItemIDChanging(value);
                    ////ReportPropertyChanging("LoggedItemID");
                    _LoggedItemID = value;
                    RaisePropertyChanged("LoggedItemID");
                    OnLoggedItemIDChanged();
                }
            }
        }
        private long _LoggedItemID;
        partial void OnLoggedItemIDChanging(long value);
        partial void OnLoggedItemIDChanged();





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
        public DateTime LogDateTime
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
        private DateTime _LogDateTime;
        partial void OnLogDateTimeChanging(DateTime value);
        partial void OnLogDateTimeChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_USERLOGI_REL_UMGMT_USERACCO", "UserAccounts")]
        public UserAccount UserAccount
        {
            get;
            set;
        }

        #endregion
    }
}
