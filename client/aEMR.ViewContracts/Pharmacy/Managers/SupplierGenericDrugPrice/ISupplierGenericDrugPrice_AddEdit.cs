using System;
using System.Collections.ObjectModel;
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
    public interface ISupplierGenericDrugPrice_AddEdit
    {
        string TitleForm { get; set; }
        Supplier ObjSupplierCurrent { get; set; }
        SupplierGenericDrugPrice ObjDrugCurrent { get; set; }
        void InitializeNewItem();
        SupplierGenericDrugPrice ObjSupplierGenericDrugPrice_Current { get; set; }
    }
}
