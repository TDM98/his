using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IModifyBillingInvItem
    {        
        MedRegItemBase ModifyBillingInvItem { get; set; }
        int PopupType { get; set; }
        void Init();

        void SetParentInPtBillingInvDetailsLst(object parentVM);

    }
}

