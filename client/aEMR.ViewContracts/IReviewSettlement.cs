using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IReviewSettlement
    {

        void GetPatientSettlement(long PtRegistrationID, long V_RegistrationType);
        ObservableCollection<TransactionFinalization> PatientSettlementList { get; set; }
    }
}
