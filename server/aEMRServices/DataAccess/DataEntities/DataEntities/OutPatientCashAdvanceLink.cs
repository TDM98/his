using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class OutPatientCashAdvanceLink : NotifyChangedBase
    {
        private long _PtCashAdvanceLinkID;
        private long _OutPtCashAdvanceID;
        private long _TransItemID;

        [DataMemberAttribute]
        public long PtCashAdvanceLinkID
        {
            get => _PtCashAdvanceLinkID; set
            {
                _PtCashAdvanceLinkID = value;
                RaisePropertyChanged("PtCashAdvanceLinkID");
            }
        }
        [DataMemberAttribute]
        public long OutPtCashAdvanceID
        {
            get => _OutPtCashAdvanceID; set
            {
                _OutPtCashAdvanceID = value;
                RaisePropertyChanged("OutPtCashAdvanceID");
            }
        }
        [DataMemberAttribute]
        public long TransItemID
        {
            get => _TransItemID; set
            {
                _TransItemID = value;
                RaisePropertyChanged("TransItemID");
            }
        }
    }
}