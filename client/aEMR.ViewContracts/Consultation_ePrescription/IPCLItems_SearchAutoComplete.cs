using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface IPCLItems_SearchAutoComplete
    {
        string KeyAction { get; set; }
        PCLExamTypeSearchCriteria SearchCriteria { get; set; }
        long V_PCLMainCategory { get; set; }
        bool IsRegimenChecked { get; set; }
        ICS_DataStorage CS_DS { get; set; }
        void SetDataForAutoComplete(IList<PCLExamType> ListPclExamTypesAllPCLForms
            , IList<PCLExamTypeComboItem> ListPclExamTypesAllCombos
            , IList<PCLExamType> ListPclExamTypesAllPCLFormImages);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        bool IsAppointment { get; set; }
        bool IsPCLBookingView { get; set; }
    }
}