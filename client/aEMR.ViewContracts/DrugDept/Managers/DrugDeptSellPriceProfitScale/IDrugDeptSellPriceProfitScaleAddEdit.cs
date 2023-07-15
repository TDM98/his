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
    public interface IDrugDeptSellPriceProfitScaleAddEdit
    {
        long V_MedProductType { get; set; }
        string TitleForm { get; set; }
        DrugDeptSellPriceProfitScale ObjDrugDeptSellPriceProfitScale_Current { get; set; }
        void InitializeNewItem();
        void FormLoad();
    }
}
