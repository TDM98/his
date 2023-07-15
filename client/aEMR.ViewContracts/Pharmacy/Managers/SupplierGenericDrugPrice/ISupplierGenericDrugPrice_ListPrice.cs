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
    public interface ISupplierGenericDrugPrice_ListPrice
    {
        Supplier ObjSupplierCurrent { get; set; }
        SupplierGenericDrugPrice ObjDrugCurrent { get; set; }
        SupplierGenericDrugPriceSearchCriteria Criteria { get; set; }
    }
}
