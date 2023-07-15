using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface IAddEditReferral
    {
        PaperReferal CurrentPaperReferral { get; }
        Patient CurrentPatient { get; set; }
        void CreateNewPaperReferal();
        void EditPaperReferal(PaperReferal referal);
    }
}
