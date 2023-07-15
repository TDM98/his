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
    public interface IPharmacySellingItemPrices_AddEdit
    {
        string TitleForm { get; set; }
        DataEntities.RefGenericDrugDetail ObjDrug_Current { get; set; }

        void InitializeNewItem();
        DataEntities.PharmacySellingItemPrices ObjPharmacySellingItemPrices_Current { get; set; }
    }
}
