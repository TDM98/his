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
    public interface IRefGenMedProductSellingPrices_AddEdit
    {
        RefGenMedProductSellingPrices ObjRefGenMedProductDetails_Info { get; set; }
        RefGenMedProductSellingPrices ObjRefGenMedProductSellingPrices_Current { get; set; }
        string TitleForm { get; set; }
        void InitializeNewItem(Int64 GenMedProductID);
    }
}
