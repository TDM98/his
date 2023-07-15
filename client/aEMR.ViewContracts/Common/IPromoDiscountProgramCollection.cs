using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IPromoDiscountProgramCollection
    {
        long PtRegistrationID { get; set; }
        bool IsUpdated { get; set; }
        bool IsChoosed { get; set; }
        ObservableCollection<PromoDiscountProgram> DiscountProgramCollection { get; set; }
        PromoDiscountProgram SelectedPromoDiscountProgram { get; set; }
        long V_RegistrationType { get; set; }
        void GetAllExamptions();
    }
}