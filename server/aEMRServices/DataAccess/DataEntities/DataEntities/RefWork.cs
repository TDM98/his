using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class RefWork : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefWork object.

        /// <param name="wID">Initial value of the WID property.</param>
        /// <param name="wName">Initial value of the WName property.</param>
        /// <param name="wActive">Initial value of the WActive property.</param>
        public static RefWork CreateRefWork(long wID, String wName, Boolean wActive)
        {
            RefWork refWork = new RefWork();
            refWork.WID = wID;
            refWork.WName = wName;
            refWork.WActive = wActive;
            return refWork;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long WID
        {
            get
            {
                return _WID;
            }
            set
            {
                if (_WID != value)
                {
                    OnWIDChanging(value);
                    ////ReportPropertyChanging("WID");
                    _WID = value;
                    RaisePropertyChanged("WID");
                    OnWIDChanged();
                }
            }
        }
        private long _WID;
        partial void OnWIDChanging(long value);
        partial void OnWIDChanged();





        [DataMemberAttribute()]
        public String WName
        {
            get
            {
                return _WName;
            }
            set
            {
                OnWNameChanging(value);
                ////ReportPropertyChanging("WName");
                _WName = value;
                RaisePropertyChanged("WName");
                OnWNameChanged();
            }
        }
        private String _WName;
        partial void OnWNameChanging(String value);
        partial void OnWNameChanged();





        [DataMemberAttribute()]
        public Boolean WActive
        {
            get
            {
                return _WActive;
            }
            set
            {
                OnWActiveChanging(value);
                ////ReportPropertyChanging("WActive");
                _WActive = value;
                RaisePropertyChanged("WActive");
                OnWActiveChanged();
            }
        }
        private Boolean _WActive;
        partial void OnWActiveChanging(Boolean value);
        partial void OnWActiveChanged();

        #endregion

    }
}
