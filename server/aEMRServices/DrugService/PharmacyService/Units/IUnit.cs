using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;

namespace PharmacyService
{
    [ServiceContract]
    public interface IUnit
    {
        #region Unit member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefUnit> GetUnits();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefUnit> GetPagingUnits(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefUnit> SearchUnit(string UnitName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteUnitByID(long unitID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateUnit(RefUnit unit);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewUnit(RefUnit newunit);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefUnit GetUnitByID(long UnitID);

        #endregion

        #region DrugDeptUnit member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefUnit> GetDrugDeptUnits();

        // -------------- DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ScientificResearchActivities>GetScientificResearchActivityList(ScientificResearchActivities Activity);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<TrainingForSubOrg>GetTrainingForSubOrgList(TrainingForSubOrg Training);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetTrainningTypeList();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertUpdateScientificResearchActivity( bool ISAdd, ScientificResearchActivities objActivity, out int Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertUpdateTrainingForSubOrg(bool ISAdd,TrainingForSubOrg objTraining, out int Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteScientificResearchActivity(long ActivityID, out int Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteTrainingForSubOrg(long TrainingID, out int Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ActivityClasses> ActivityClassListAll(long ClassID);


        // DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học--------------------------

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefUnit> GetPagingDrugDeptUnits(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefUnit> SearchDrugDeptUnit(string UnitName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteDrugDeptUnitByID(long unitID, long? StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDrugDeptUnit(RefUnit unit);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewDrugDeptUnit(RefUnit newunit);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefUnit GetDrugDeptUnitByID(long UnitID);

        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SellingPriceFormular GetSellingPriceFormularByID(long ItemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SellingPriceFormular GetSellingPriceFormularIsActive();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<SellingPriceFormular> GetSellingPriceFormularAll(DateTime? FromDate, DateTime? Todate, int PageIndex, int PageSize, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewSellingPriceFormular(SellingPriceFormular p);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateSellingPriceFormular(SellingPriceFormular p);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteSellingPriceFormular(long ItemID);

    }
}
