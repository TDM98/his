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
    public interface IDrugClasses_SearchPaging_Drug
    {
        DrugClass ObjDrugClasses_Selected { get; set; }
        DrugClassSearchCriteria criteria { get; set; }
        Int64 V_MedProductType { get; set; }
    }
}
