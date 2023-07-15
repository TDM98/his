using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class DiagnosisICD9Items : NotifyChangedBase
    {
        #region Primitive Properties

        [DataMemberAttribute()]
        public long DiagICD9ItemID
        {
            get
            {
                return _DiagICD9ItemID;
            }
            set
            {
                if (_DiagICD9ItemID != value)
                {
                    _DiagICD9ItemID = value;
                    RaisePropertyChanged("DiagICD9ItemID");
                }
            }
        }
        private long _DiagICD9ItemID;

        [DataMemberAttribute()]
        public long DiagnosisICD9ListID
        {
            get
            {
                return _DiagnosisICD9ListID;
            }
            set
            {
                _DiagnosisICD9ListID = value;
                RaisePropertyChanged("DiagnosisICD9ListID");
            }
        }
        private long _DiagnosisICD9ListID;

        [DataMemberAttribute()]
        public string ICD9Code
        {
            get
            {
                return _ICD9Code;
            }
            set
            {
                _ICD9Code = value;
                RaisePropertyChanged("ICD9Code");
            }
        }
        private string _ICD9Code;


        [DataMemberAttribute()]
        public bool IsMain
        {
            get
            {
                return _IsMain;
            }
            set
            {
                _IsMain = value;
                RaisePropertyChanged("IsMain");
            }
        }
        private bool _IsMain;

        [DataMemberAttribute()]
        public bool IsCongenital
        {
            get
            {
                return _IsCongenital;
            }
            set
            {
                _IsCongenital = value;
                RaisePropertyChanged("IsCongenital");
            }
        }
        private bool _IsCongenital;

        #endregion

        #region Navigation Properties

        private RefICD9 _RefICD9;
        public RefICD9 RefICD9
        {
            get
            {
                return _RefICD9;
            }
            set
            {
                if (_RefICD9 != value)
                {
                    _RefICD9 = value;
                    if (_RefICD9 != null)
                    {
                        _ICD9Code = value.ICD9Code;
                    }
                    RaisePropertyChanged("ICD9Code");
                    RaisePropertyChanged("RefICD9");
                }
            }
        }

        //private Lookup _LookupStatus;
        //public Lookup LookupStatus
        //{
        //    get
        //    {
        //        return _LookupStatus;
        //    }
        //    set
        //    {
        //        if (_LookupStatus != value)
        //        {
        //            _LookupStatus = value;
        //            RaisePropertyChanged("LookupStatus");
        //        }
        //    }
        //}

        #endregion

    }
}
