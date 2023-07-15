using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Su dung cho 1 view chi chua 1 AutoCompleteBox
    /// </summary>
    public interface IDeptTreeView
    {
        void SelectTreeItemByDataItem(RefDepartmentsTree node);
    }
}
