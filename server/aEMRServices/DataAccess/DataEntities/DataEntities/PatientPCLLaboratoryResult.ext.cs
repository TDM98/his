using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
namespace DataEntities
{
    public partial class PatientPCLLaboratoryResult : NotifyChangedBase
    {
        #region Extended properties
        [DataMemberAttribute()]
        public string PatientPCLLabResultDetailsXML
        {
            get
            {
                return _PatientPCLLabResultDetailsXML;
            }
            set
            {
                if (_PatientPCLLabResultDetailsXML != value)
                {
                    OnPatientPCLLabResultDetailsXMLChanging(value);
                    _PatientPCLLabResultDetailsXML = value;
                    RaisePropertyChanged("PatientPCLLabResultDetailsXML");
                    OnPatientPCLLabResultDetailsXMLChanged();
                }
            }
        }
        private string _PatientPCLLabResultDetailsXML;
        partial void OnPatientPCLLabResultDetailsXMLChanging(string value);
        partial void OnPatientPCLLabResultDetailsXMLChanged();

        #endregion
    }
}
