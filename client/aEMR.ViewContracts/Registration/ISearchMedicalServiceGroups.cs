using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface ISearchMedicalServiceGroups
    {
        void ApplySearchContent(List<RefMedicalServiceGroups> aMedicalServiceGroupCollection, string aSearchMedicalServiceGroupCode, List<RefMedicalServiceGroups> aSearchMedicalServiceGroupCollection);
        RefMedicalServiceGroups SelectedRefMedicalServiceGroup { get; }
        long V_RegistrationType { get; set; }
        string SearchMedicalServiceGroupName { get; set; }
        long V_MedicalServiceGroupType { get; set; }
    }
}
