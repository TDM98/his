using System;
//20181007 TNHX: [BM0000104] Allow selected Effectiveday when edit and refactor code
namespace aEMR.ViewContracts.Configuration
{
    public interface IPCLExamTypePriceList_AddEdit
    {
        string TitleForm { get; set; }

        void InitializeNewItem();
        DataEntities.PCLExamTypePriceList ObjPCLExamTypePriceList_Current { get; set; }
        
        bool btChooseItemFromDelete_IsEnabled { get; set; }
        
        bool btSave_IsEnabled { get; set; }
        DateTime EndDate { get; set; }
        DateTime BeginDate { get; set; }
        bool dpEffectiveDate_IsEnabled { get; set; }
    }
}
