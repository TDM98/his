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

namespace aEMR.ViewContracts
{
    public interface IDrugDeptSellingItemPrices_AddEdit
    {
        long V_MedProductType { get; set; }
        string TitleForm { get; set; }

        DataEntities.RefGenMedProductDetails ObjDrug_Current { get; set; }

        void InitializeNewItem();
        DataEntities.DrugDeptSellingItemPrices ObjDrugDeptSellingItemPrices_Current { get; set; }
    }
}
