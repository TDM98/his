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

namespace aEMR.ViewContracts.Configuration
{
    public interface IPCLExamTypePrices
    {
        DataEntities.PCLExamTypePriceSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<DataEntities.PCLExamTypePrice> ObjPCLExamTypePrices_ByPCLExamTypeID_Paging { get;}
        DataEntities.PCLExamType ObjPCLExamType_Current { get; set; }

        bool IStatusFromDate { get; set; }
        bool IStatusToDate { get; set; }
        bool IStatusCheckFindDate { get; set; }
    }
}
