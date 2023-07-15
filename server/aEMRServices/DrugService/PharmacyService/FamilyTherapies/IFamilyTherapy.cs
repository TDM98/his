using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using EFData;

using eHCMS.Services.Core;

namespace PharmacyService
{
    [ServiceContract]
    public interface IFamilyTherapy
    {

        [OperationContract]
        RefFamilyTherapy GetFamilyTherapyByID(decimal faID);

        [OperationContract]
        CRUDOperationResponse DeleteFamilyTherapy(RefFamilyTherapy familytherapy);

        [OperationContract]
        CRUDOperationResponse DeleteFamilyTherapyByID(decimal faID);

        [OperationContract]
        IList<RefFamilyTherapy> GetAllFamilyTherapies();

        [OperationContract]
        IList<RefFamilyTherapy> GetFamilyTherapies(bool bCountTotal, out int totalCount);

        [OperationContract]
        IList<RefFamilyTherapy> SearchFamilyTherapies(FamilyTherapySearchCriteria criteria,bool bCountTotal, out int totalCount);

        [OperationContract]
        CRUDOperationResponse AddNewFamilyTherapy(RefFamilyTherapy newFamilyTherapy);

        [OperationContract]
        List<FamilyTherapyTreeview> GetTreeview(FamilyTherapySearchCriteria criteria, bool bCountTotal, out int totalCount);

        [OperationContract]
        List<FamilyTherapyTreeview> GetFamilyTherapiesTreeView(bool bCountTotal, out int totalCount);



    }
}
