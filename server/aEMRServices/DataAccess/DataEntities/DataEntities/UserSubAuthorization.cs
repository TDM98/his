using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class UserSubAuthorization : NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new User object.
     
        /// <param name="userName">Initial value of the AccountName property.</param>
        /// <param name="password">Initial value of the AccountPassword property.</param>
        public static UserSubAuthorization CreateUser(long accountIDAuth, long accountIDSub, string authPwd)
        {
            UserSubAuthorization user = new UserSubAuthorization();
            user.AccountIDAuth = accountIDAuth;
            user.AccountIDSub = accountIDSub;
            user.AuthPwd = authPwd;
            return user;
        }

        #endregion
        #region Primitive Properties

     
        
     
        /// 
        [DataMemberAttribute()]
        public long SubUserAuthorizationID
        {
            get
            {
                return _SubUserAuthorizationID;
            }
            set
            {
                if (_SubUserAuthorizationID != value)
                {
                    OnSubUserAuthorizationIDChanging(value);
                    _SubUserAuthorizationID = value;
                    OnSubUserAuthorizationIDChanged(); 
                }
            }
        }
        private long _SubUserAuthorizationID;
        partial void OnSubUserAuthorizationIDChanging(long value);
        partial void OnSubUserAuthorizationIDChanged();

        [Required(ErrorMessage = "AccountName is required")]
        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {

                OnRecCreatedDateChanging(value);
                ValidateProperty("RecCreatedDate", value);
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();

       [Required(ErrorMessage = "AccountIDAuth is required")]
        [DataMemberAttribute()]
        public long AccountIDAuth
        {
            get
            {
                return _AccountIDAuth;
            }
            set
            {
                OnAccountIDAuthChanging(value);
                _AccountIDAuth = value;
                OnAccountIDAuthChanged();
            }
        }
       private long _AccountIDAuth;
       partial void OnAccountIDAuthChanging(long value);
       partial void OnAccountIDAuthChanged();

        
       [DataMemberAttribute()]
       public long AccountIDSub
        {
            get
            {
                return _AccountIDSub;
            }
            set
            {
                OnAccountIDSubChanging(value);
                _AccountIDSub = value;
                OnAccountIDSubChanged();
            }
        }
        private long _AccountIDSub;
        partial void OnAccountIDSubChanging(long value);
        partial void OnAccountIDSubChanged();
        
        [DataMemberAttribute()]
        public string AuthPwd
        {
            get
            {
                return _AuthPwd;
            }
            set
            {
                OnAuthPwdChanging(value);
                _AuthPwd = value;
                OnAuthPwdChanged();
            }
        }
        private string _AuthPwd;
        partial void OnAuthPwdChanging(string value);
        partial void OnAuthPwdChanged();

        [DataMemberAttribute()]
        public string ConfirmAuthPwd
        {
            get
            {
                return _ConfirmAuthPwd;
            }
            set
            {
                OnConfirmAuthPwdChanging(value);
                _ConfirmAuthPwd = value;
                OnConfirmAuthPwdChanged();
            }
        }
        private string _ConfirmAuthPwd;
        partial void OnConfirmAuthPwdChanging(string value);
        partial void OnConfirmAuthPwdChanged();

       
        private bool _IsActivated;
        public bool IsActivated
        {
            get
            {
                return _IsActivated;
            }
            set
            {
                ValidateProperty("IsActivated",value);
                _IsActivated = value;
                RaisePropertyChanged("IsActivated");
            }
        }
        #endregion

        #region Navigation Properties
        
        [DataMemberAttribute()]
        public UserAccount AccountAuth
        {
            get
            {
                return _AccountAuth;
            }
            set
            {
                if (_AccountAuth != value)
                {
                    OnAccountAuthChanging(value);
                    _AccountAuth = value;
                    OnAccountAuthChanged();
                }
            }
        }
        private UserAccount _AccountAuth;
        partial void OnAccountAuthChanging(UserAccount value);
        partial void OnAccountAuthChanged();


        [DataMemberAttribute()]
        public UserAccount AccountSub
        {
            get
            {
                return _AccountSub;
            }
            set
            {
                if (_AccountSub != value)
                {
                    OnAccountSubChanging(value);
                    _AccountSub = value;
                    OnAccountSubChanged();
                }
            }
        }
        private UserAccount _AccountSub;
        partial void OnAccountSubChanging(UserAccount value);
        partial void OnAccountSubChanged();

        [DataMemberAttribute()]
        public ObservableCollection<Group> UserGroups
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            UserSubAuthorization info = obj as UserSubAuthorization;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.SubUserAuthorizationID > 0 && this.SubUserAuthorizationID == info.SubUserAuthorizationID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
