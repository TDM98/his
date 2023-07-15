using System;

namespace aEMR.ViewContracts.Configuration
{
    public interface ILookup_AddEdit
    {
        string TitleForm { get; set; }
        //ObservableCollection<DataEntities.Job> ObjJob_GetAll{ get; set; }
        DataEntities.Lookup ObjLookup_Current { get; set; }
        void InitializeNewItem();
    }
}
