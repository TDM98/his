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
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface ICMDrugDeptSellingItemPrices
    {
        long V_MedProductType { get; set; }
        string TitleForm { get; set; }

        DataEntities.DrugDeptSellingItemPricesSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<DataEntities.DrugDeptSellingItemPrices> ObjDrugDeptSellingItemPrices_ByDrugID_Paging { get; }
        DataEntities.RefGenMedProductDetails ObjDrug_Current { get; set; }

        bool IStatusFromDate { get; set; }
        bool IStatusToDate { get; set; }
        bool IStatusCheckFindDate { get; set; }
        bool IsStore { get; set; }
        bool mTaoGiaMoi { get; set; }
        bool mChinhSuaGia { get; set; }
    }
}
