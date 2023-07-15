using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RegistrationType:NotifyChangedBase
    {
        public RegistrationType()
            : base()
        {

        }

        private byte _RegTypeID;
        public byte RegTypeID
        {
            get
            {
                return _RegTypeID;
            }
            set
            {
                ValidateProperty("RegTypeID", value);
                _RegTypeID = value;
                RaisePropertyChanged("RegTypeID");
            }
        }
        private string _RegTypeName;
        public string RegTypeName
        {
            get
            {
                return _RegTypeName;
            }
            set
            {
                ValidateProperty("RegTypeName", value);
                _RegTypeName = value;
                RaisePropertyChanged("RegTypeName");
            }
        }
    }
}
