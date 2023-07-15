using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPrescriptionNoteTemplates_AddEdit
    {
        string TitleForm { get; set; }
        PrescriptionNoteTemplates ObjPrescriptionNoteTemplates_Current { get; set; }
        void InitializeNewItem();
    }
}
