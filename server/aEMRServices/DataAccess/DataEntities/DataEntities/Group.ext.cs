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
    public delegate void CheckGroupNameExistsCallbackDelegate(bool isGroupNameExisted, long atTime);
    public partial class Group
    {
        private const string GROUPNAME_EXISTS = "GroupName already existed.";

        #region Delegate for external access

        
//         [IgnoreDataMember]
//         public Action<string, CheckUserNameExistsCallbackDelegate> CheckUserNameExistsDelegate { get; set; }

     
        /// The second variable here is the timestamp.
     
        [IgnoreDataMember]
        public Action<string, long, CheckGroupNameExistsCallbackDelegate> CheckGroupNameExistsDelegate { get; set; }
        #endregion


        partial void OnGroupNameChanging(String value)
        {
            if (CheckGroupNameExistsDelegate != null && !string.IsNullOrWhiteSpace(value))
            {
                RemoveError("GroupName", GROUPNAME_EXISTS);
                _CheckGroupNameBeginTime = DateTime.Now.Ticks;
                CheckGroupNameExistsDelegate(value, _CheckGroupNameBeginTime, CheckGroupNameExistsCallback);
            }
        }

        private long _CheckGroupNameBeginTime;

        public void CheckGroupNameExistsCallback(bool isGroupNameExisted, long timeStartChecking)
        {
            if (timeStartChecking == _CheckGroupNameBeginTime)
            {
                if (isGroupNameExisted)
                {
                    AddError("GroupName", GROUPNAME_EXISTS, false);
                }
                else
                    RemoveError("GroupName", GROUPNAME_EXISTS); 
            }
        }
    }
}
