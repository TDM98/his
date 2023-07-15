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
    public interface ICMDrug_AddEdit
    {

        void InitializeNewItem(Int64 pV_MedProductType);
        DataEntities.RefGenMedProductDetails ObjRefGenMedProductDetails_Current { get; set; }
        string TitleForm { get; set; }
        void SupplierGenMedProduct_LoadDrugIDNotPaging(long GenMedProductID);
        void GetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID);
    }
}
