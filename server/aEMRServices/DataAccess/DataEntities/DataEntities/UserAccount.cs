using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DataEntities
{
    public partial class UserAccount:NotifyChangedBase
    {
        #region Factory Method

     
        /// Create a new User object.
     
        /// <param name="userName">Initial value of the AccountName property.</param>
        /// <param name="password">Initial value of the AccountPassword property.</param>
        public static UserAccount CreateUser(String userName, String password)
        {
            UserAccount user = new UserAccount();
            user.AccountName = userName;
            user.AccountPassword = password;
            return user;
        }

        #endregion
        #region Primitive Properties

     
        
     
        /// 
        [DataMemberAttribute()]
        public long AccountID
        {
            get
            {
                return _AccountID;
            }
            set
            {
                if (_AccountID != value)
                {
                    OnAccountIDChanging(value);
                    _AccountID = value;
                    OnAccountIDChanged(); 
                }
            }
        }
        private long _AccountID;
        partial void OnAccountIDChanging(long value);
        partial void OnAccountIDChanged();

        [Required(ErrorMessage = "AccountName is required")]
        [DataMemberAttribute()]
        public String AccountName
        {
            get
            {
                return _AccountName;
            }
            set
            {

                OnAccountNameChanging(value);
                ValidateProperty("AccountName",value);
                _AccountName = value;
                RaisePropertyChanged("AccountName");
                OnAccountNameChanged();
            }
        }
        private String _AccountName;
        partial void OnAccountNameChanging(String value);
        partial void OnAccountNameChanged();

       [Required(ErrorMessage = "AccountPassword is required")]
        [DataMemberAttribute()]
        public String AccountPassword
        {
            get
            {
                return _AccountPassword;
            }
            set
            {
                OnAccountPasswordChanging(value);
                _AccountPassword = value;
                OnAccountPasswordChanged();
            }
        }
        private String _AccountPassword;
        partial void OnAccountPasswordChanging(String value);
        partial void OnAccountPasswordChanged();

        [Required(ErrorMessage = "AccountPassword Confirm is required")]
        [DataMemberAttribute()]
        public String AccountPasswordConfirm
        {
            get
            {
                return _AccountPasswordConfirm;
            }
            set
            {
                OnAccountPasswordConfirmChanging(value);
                _AccountPasswordConfirm = value;
                OnAccountPasswordConfirmChanged();
            }
        }
        private String _AccountPasswordConfirm;
        partial void OnAccountPasswordConfirmChanging(String value);
        partial void OnAccountPasswordConfirmChanged();
        
        [DataMemberAttribute()]
        public Nullable<DateTime> LastLogin
        {
            get
            {
                return _LastLogin;
            }
            set
            {
                OnLastLoginChanging(value);
                _LastLogin = value;
                OnLastLoginChanged();
            }
        }
        private Nullable<DateTime> _LastLogin;
        partial void OnLastLoginChanging(Nullable<DateTime> value);
        partial void OnLastLoginChanged();

        [DataMemberAttribute()]
        public long? StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                OnStaffIDChanged();
            }
        }
        private long? _StaffID;
        partial void OnStaffIDChanging(long? value);
        partial void OnStaffIDChanged();

        public Staff Staff
        {
            get
            {
                return _Staff;
            }
            set
            {
                _Staff = value;
                if(_Staff == null)
                {
                    StaffID = null;
                }
                else
                {
                    StaffID = _Staff.StaffID;
                }
                RaisePropertyChanged("Staff");
            }
        }
        private Staff _Staff;

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

        [DataMemberAttribute()]
        public List<StaffDeptResponsibilities> AllStaffDeptResponsibilities 
        {
            get
            {
                return _AllStaffDeptResponsibilities;
            }
            set
            {
                OnAllStaffDeptResponsibilitiesChanging(value);
                _AllStaffDeptResponsibilities = value;
                OnAllStaffDeptResponsibilitiesChanged();
            }
        }
        private List<StaffDeptResponsibilities> _AllStaffDeptResponsibilities;
        partial void OnAllStaffDeptResponsibilitiesChanging(List<StaffDeptResponsibilities> value);
        partial void OnAllStaffDeptResponsibilitiesChanged();


        //KMx: Danh sách các DepartmentID mà nhân viên được cấu hình trách nhiệm (15/09/2014 10:12).
        [DataMemberAttribute()]
        public ObservableCollection<long> DeptIDResponsibilityList
        {
            get
            {
                return _deptIDResponsibilityList;
            }
            set
            {
                OnDeptIDResponsibilityListChanging(value);
                _deptIDResponsibilityList = value;
                OnDeptIDResponsibilityListChanged();
            }
        }
        private ObservableCollection<long> _deptIDResponsibilityList;
        partial void OnDeptIDResponsibilityListChanging(ObservableCollection<long> value);
        partial void OnDeptIDResponsibilityListChanged();

        private List<ConsultationTimeSegments> _ConsultationTimeSegmentsList;
        [DataMemberAttribute()]
        public List<ConsultationTimeSegments> ConsultationTimeSegmentsList
        {
            get
            {
                return _ConsultationTimeSegmentsList;
            }
            set
            {
                _ConsultationTimeSegmentsList = value;
                RaisePropertyChanged("ConsultationTimeSegmentsList");
            }
        }
        
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<Group> UserGroups
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public long LoggedHistoryID
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            UserAccount info = obj as UserAccount;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.AccountID > 0 && this.AccountID == info.AccountID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
