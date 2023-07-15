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

namespace aEMR.ViewContracts
{
    public interface ISearchPCLRequest
    {
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

        PatientPCLRequestSearchCriteria SearchCriteria { get; set; }

        PagedSortableCollectionView<PatientPCLRequest> ObjPatientPCLRequest_SearchPaging { get; set; }

        void LoadData();
    }
}
