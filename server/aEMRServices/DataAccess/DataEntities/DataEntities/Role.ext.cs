using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataEntities
{
 
    /// <param name="isUserNameExisted"></param>
    /// <param name="atTime">Time when start checking username</param>
    /// 
    public delegate void CheckRoleNameExistsCallbackDelegate(bool isRoleNameExisted, long atTime);
    public partial class Role
    {
        private const string ROLENAME_EXISTS = "Role Name already existed.";

        #region Delegate for external access

        
//         [IgnoreDataMember]
//         public Action<string, CheckUserNameExistsCallbackDelegate> CheckUserNameExistsDelegate { get; set; }

     
        /// The second variable here is the timestamp.
     
        [IgnoreDataMember]
        public Action<string, long, CheckRoleNameExistsCallbackDelegate> CheckRoleNameExistsDelegate { get; set; }
        #endregion


        partial void OnRoleNameChanging(String value)
        {
            if (CheckRoleNameExistsDelegate != null && !string.IsNullOrWhiteSpace(value))
            {
                RemoveError("RoleName", ROLENAME_EXISTS);
                _CheckGroupNameBeginTime = DateTime.Now.Ticks;
                CheckRoleNameExistsDelegate(value, _CheckGroupNameBeginTime, CheckRoleNameExistsCallback);
            }
        }

        private long _CheckGroupNameBeginTime;

        public void CheckRoleNameExistsCallback(bool isRoleNameExisted, long timeStartChecking)
        {
            if (timeStartChecking == _CheckGroupNameBeginTime)
            {
                if (isRoleNameExisted)
                {
                    AddError("RoleName", ROLENAME_EXISTS, false);
                }
                else
                    RemoveError("RoleName", ROLENAME_EXISTS); 
            }
        }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }
        private bool _isSelected = false;
    }
}
