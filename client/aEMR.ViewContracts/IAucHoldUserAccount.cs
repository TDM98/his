using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface IAucHoldUserAccount
    {
        long AccountID { get; set; }
        void setDefault();
        bool IsProcessing { get; set; }
    }
}
