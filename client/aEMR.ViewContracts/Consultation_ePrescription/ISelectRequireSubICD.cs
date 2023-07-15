using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface ISelectRequireSubICD
    {
        string TitleForm { get; set; }
        ObservableCollection<RequiredSubDiseasesReferences> ListRequiredSubDiseasesReferences { get; set; }
        int MainICDIndex { get; set; }
        //void InitializeNewItem();
    }
}
