using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;

namespace aEMR.ViewContracts
{
    public interface ITransferFormList
    {
        ObservableCollection<TransferForm> TransferFormList { get; set; }
        IPaperReferalFull ParentReferralPaper { get; set; }
        int V_TransferFormType { get; set; }
    }
}
