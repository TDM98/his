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
    public interface IPharmacySellingItemPrices
    {
        DataEntities.PharmacySellingItemPricesSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<DataEntities.PharmacySellingItemPrices> ObjPharmacySellingItemPrices_ByDrugID_Paging { get; }
        DataEntities.RefGenericDrugDetail ObjDrug_Current { get; set; }

        bool IStatusFromDate { get; set; }
        bool IStatusToDate { get; set; }
        bool IStatusCheckFindDate { get; set; }
    }
}
