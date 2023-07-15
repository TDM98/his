using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class HosClientContractPatientGroup : NotifyChangedBase
    {
        private long _HosClientContractPatientGroupID;
        private long _HosClientContractID;
        private string _HosClientContractPatientGroupName;
        [DataMemberAttribute]
        public long HosClientContractPatientGroupID
        {
            get
            {
                return _HosClientContractPatientGroupID;
            }
            set
            {
                if (_HosClientContractPatientGroupID == value)
                {
                    return;
                }
                _HosClientContractPatientGroupID = value;
                RaisePropertyChanged("HosClientContractPatientGroupID");
            }
        }
        [DataMemberAttribute]
        public long HosClientContractID
        {
            get
            {
                return _HosClientContractID;
            }
            set
            {
                if (_HosClientContractID == value)
                {
                    return;
                }
                _HosClientContractID = value;
                RaisePropertyChanged("HosClientContractID");
            }
        }
        [DataMemberAttribute]
        public string HosClientContractPatientGroupName
        {
            get
            {
                return _HosClientContractPatientGroupName;
            }
            set
            {
                if (_HosClientContractPatientGroupName == value)
                {
                    return;
                }
                _HosClientContractPatientGroupName = value;
                RaisePropertyChanged("HosClientContractPatientGroupName");
            }
        }
    }
}