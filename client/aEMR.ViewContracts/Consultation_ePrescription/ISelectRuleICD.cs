using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface ISelectRuleICD
    {
        string TitleForm { get; set; }
        ObservableCollection<RuleDiseasesReferences> ListRuleDiseasesReferences { get; set; }
        int MainICDIndex { get; set; }
    }
}
