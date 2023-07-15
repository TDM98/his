using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefTypeAbsent : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefTypeAbsent object.

        /// <param name="tAID">Initial value of the TAID property.</param>
        /// <param name="tAName">Initial value of the TAName property.</param>
        /// <param name="tAActive">Initial value of the TAActive property.</param>
        public static RefTypeAbsent CreateRefTypeAbsent(long tAID, String tAName, Boolean tAActive)
        {
            RefTypeAbsent refTypeAbsent = new RefTypeAbsent();
            refTypeAbsent.TAID = tAID;
            refTypeAbsent.TAName = tAName;
            refTypeAbsent.TAActive = tAActive;
            return refTypeAbsent;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long TAID
        {
            get
            {
                return _TAID;
            }
            set
            {
                if (_TAID != value)
                {
                    OnTAIDChanging(value);
                    ////ReportPropertyChanging("TAID");
                    _TAID = value;
                    RaisePropertyChanged("TAID");
                    OnTAIDChanged();
                }
            }
        }
        private long _TAID;
        partial void OnTAIDChanging(long value);
        partial void OnTAIDChanged();





        [DataMemberAttribute()]
        public String TAName
        {
            get
            {
                return _TAName;
            }
            set
            {
                OnTANameChanging(value);
                ////ReportPropertyChanging("TAName");
                _TAName = value;
                RaisePropertyChanged("TAName");
                OnTANameChanged();
            }
        }
        private String _TAName;
        partial void OnTANameChanging(String value);
        partial void OnTANameChanged();





        [DataMemberAttribute()]
        public Boolean TAActive
        {
            get
            {
                return _TAActive;
            }
            set
            {
                OnTAActiveChanging(value);
                ////ReportPropertyChanging("TAActive");
                _TAActive = value;
                RaisePropertyChanged("TAActive");
                OnTAActiveChanged();
            }
        }
        private Boolean _TAActive;
        partial void OnTAActiveChanging(Boolean value);
        partial void OnTAActiveChanged();

        #endregion

    }
}
