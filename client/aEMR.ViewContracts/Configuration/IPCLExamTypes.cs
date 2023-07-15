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
using aEMR.Common.Collections;

namespace aEMR.ViewContracts.Configuration
{
    public interface IPCLExamTypes
    {
        PCLExamTypeSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<DataEntities.PCLExamType> ObjPCLExamTypesAndPriceIsActive_Paging { get; }
        
        Visibility hplAddNewVisible { get; set; }
        Visibility dgCellTemplate0_Visible { get; set; }
        
    }
}
