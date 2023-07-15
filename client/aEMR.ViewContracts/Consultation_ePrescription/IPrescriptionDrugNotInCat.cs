using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPrescriptionDrugNotInCat
    {
        PrescriptionDetail ObjPrescriptionDetail { get; set; }
        int IndexRow { get;set; }
        ObservableCollection<ChooseDose> ChooseDoses { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}