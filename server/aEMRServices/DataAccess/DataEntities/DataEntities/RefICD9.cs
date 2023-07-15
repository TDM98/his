using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;


namespace DataEntities
{
    public class RefICD9 : NotifyChangedBase
    {
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ICD9ID
        {
            get
            {
                return _ICD9ID;
            }
            set
            {
                if (_ICD9ID != value)
                {
                    _ICD9ID = value;
                    RaisePropertyChanged("ICD9ID");
                }
            }
        }
        private Int64 _ICD9ID;


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
        public string ProcedureName
        {
            get
            {
                return _ProcedureName;
            }
            set
            {
                _ProcedureName = value;
                RaisePropertyChanged("ProcedureName");
            }
        }
        private string _ProcedureName;

        #endregion


    }
}
