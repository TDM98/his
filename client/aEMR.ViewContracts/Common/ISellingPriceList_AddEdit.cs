
using System;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ISellingPriceList_AddEdit
    {
        long V_MedProductType { get; set; }
        string TitleForm { get; set; }

        void InitializeNewItem();
        //T ObjDrugDeptSellingPriceList_Current { get; set; }

        DateTime EndDate { get; set; }
        DateTime BeginDate { get; set; }
        bool dpEffectiveDate_IsEnabled { get; set; }
    }
}
