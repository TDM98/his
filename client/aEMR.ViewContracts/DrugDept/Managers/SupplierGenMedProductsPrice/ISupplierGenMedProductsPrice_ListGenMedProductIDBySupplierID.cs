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
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface ISupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID
    {
        string TitleForm { get; set; }
        Supplier ObjSupplierCurrent { get; set; }
        ObservableCollection<Lookup> ObjV_MedProductType { get;set;}
        SupplierGenMedProductsPriceSearchCriteria Criteria { get; set; }
        bool mTaoGiaMoi { get; set; }
        bool mSuaGia { get; set; }
    }
}
