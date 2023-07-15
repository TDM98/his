using System.Collections.Generic;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientSelectPclLAB
    {
        ObservableCollection<PCLForm> PCLForms { get; set; }
        PCLForm SelectedPCLForm { get; set; }
        ObservableCollection<PCLItem> PCLItemList { get; set; }
        /// <summary>
        /// Chi ra dich vu nay da duoc su dung hay chua.
        /// </summary>
        bool Used { get; set; }
        /// <summary>
        /// Co hien thi cot Used hay khong.
        /// </summary>
        bool ShowUsedField { get; set; }
        bool ShowLocationSelection { get; set; }
        long? DeptID { get; set; }
        void LoadPCLForms(long? pclCategory);
        ObservableCollection<Lookup> PCLCategories { get; }
        Lookup SelectedPCLCategory { get; set; }
        bool IsLoadingPclForm { get; set; }
        PCLExamType SelectedPCLExamType { get; set; }
        ObservableCollection<PCLExamType> PclExamTypes { get; }
        bool IsLoadingPclExamTypes { get; set; }
        PCLExamTypeLocation SelectedPclExamTypeLocation { get; set; }
        InPatientAdmDisDetails CurrentInPatientAdmDisDetail { get; set; }
        bool IsRegimenChecked { get; set; }
        List<RefTreatmentRegimen> ListRegiment { get; set; }
    }
}