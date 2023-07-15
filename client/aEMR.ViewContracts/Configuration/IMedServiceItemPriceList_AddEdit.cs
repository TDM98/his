using System;
using System.Windows;
//20181007 TNHX: [BM0000104] Allow selected Effectiveday when edit and refactor code
namespace aEMR.ViewContracts.Configuration
{
    public interface IMedServiceItemPriceList_AddEdit
    {
        string TitleForm { get; set; }

        void InitializeNewItem(Int64 pMedicalServiceTypeID);
        DataEntities.MedServiceItemPriceList ObjMedServiceItemPriceList_Current { get; set; }
        Visibility dgCellTemplate0_Visible { get; set; }
        
        bool btChooseItemFromDelete_IsEnabled { get; set; }
        bool dpEffectiveDate_IsEnabled { get; set; }
        bool btSave_IsEnabled { get; set; }
        DateTime EndDate { get; set; }
        DateTime BeginDate { get; set; }
        string MedServiceItemTypeName { get; set; }
    }
}
