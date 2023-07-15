using DataEntities;

namespace aEMR.ViewContracts
{
    public interface INutritionalRating_AddEdit
    {
        //void InitPatientInfo();
        //bool IsShowSummaryContent { get; set; }
        //IRegistration_DataStorage Registration_DataStorage { get; set; }
        //long V_RegistrationType { get; set; }
        NutritionalRating CurrentNutritionalRating { get; set; }
        long PtRegistrationID { get; set; }
        void InitializeNewItem();

        bool IsBMIBelow205 { get; set; }
    }
}