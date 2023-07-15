using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataEntities
{
    //public delegate void CheckAccountNameExistsCallbackDelegate(bool isAccountNameExisted);
 
    /// 
 
    /// <param name="isAccountNameExisted"></param>
    /// <param name="atTime">Time when start checking username</param>
    /// 
    public delegate void CheckAccountNameExistsCallbackDelegate(bool isAccountNameExisted, long atTime);
    public partial class UserAccount
    {
        private const string USERNAME_EXISTS = "AccountName already existed.";

        #region Delegate for external access

        
//         [IgnoreDataMember]
//         public Action<string, CheckAccountNameExistsCallbackDelegate> CheckAccountNameExistsDelegate { get; set; }

     
        /// The second variable here is the timestamp.
     
        [IgnoreDataMember]
        public Action<string,long, CheckAccountNameExistsCallbackDelegate> CheckAccountNameExistsDelegate { get; set; }
        #endregion


        partial void OnAccountNameChanging(String value)
        {
            if (CheckAccountNameExistsDelegate != null)
            {
                RemoveError("AccountName", USERNAME_EXISTS);
                _CheckAccountNameBeginTime = DateTime.Now.Ticks;
                CheckAccountNameExistsDelegate(value, _CheckAccountNameBeginTime,CheckAccountNameExistsCallback);
            }
        }

        private long _CheckAccountNameBeginTime;

        public void CheckAccountNameExistsCallback(bool isAccountNameExisted, long timeStartChecking)
        {
            if (timeStartChecking == _CheckAccountNameBeginTime)
            {
                if (isAccountNameExisted)
                {
                    AddError("AccountName", USERNAME_EXISTS, false);
                }
                else
                    RemoveError("AccountName", USERNAME_EXISTS); 
            }
        }
        public override string ToString()
        {
            return this.AccountName;
        }
    }
}
