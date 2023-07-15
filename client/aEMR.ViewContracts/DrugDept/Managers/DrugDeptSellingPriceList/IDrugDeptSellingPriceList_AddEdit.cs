using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IDrugDeptSellingPriceList_AddEdit
    {
        long V_MedProductType { get; set; }
        string TitleForm { get; set; }

        void InitializeNewItem();
        DrugDeptSellingPriceList ObjDrugDeptSellingPriceList_Current { get; set; }

        DateTime EndDate { get; set; }
        DateTime BeginDate { get; set; }
        bool dpEffectiveDate_IsEnabled { get; set; }

    }
}
