using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;

namespace UnitProxy
{
    [ServiceContract]
    public interface IUnit
    {
        #region Unit member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetUnits(AsyncCallback callback, object state);
        IList<RefUnit> EndGetUnits(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPagingUnits(int pageIndex, int pageSize, bool bCountTotal,  AsyncCallback callback, object state);
        IList<RefUnit> EndGetPagingUnits(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchUnit(string UnitName, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefUnit> EndSearchUnit(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteUnitByID(long unitID, AsyncCallback callback, object state);
        bool EndDeleteUnitByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateUnit(RefUnit unit, AsyncCallback callback, object state);
        bool EndUpdateUnit(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewUnit(RefUnit newunit, AsyncCallback callback, object state);
        bool EndAddNewUnit(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetUnitByID(long UnitID, AsyncCallback callback, object state);
        RefUnit EndGetUnitByID(IAsyncResult asyncResult);

        #endregion

        #region DrugDeptUnit member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugDeptUnits(AsyncCallback callback, object state);
        IList<RefUnit> EndGetDrugDeptUnits(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPagingDrugDeptUnits(int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefUnit> EndGetPagingDrugDeptUnits(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchDrugDeptUnit(string UnitName, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefUnit> EndSearchDrugDeptUnit(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteDrugDeptUnitByID(long unitID, long? StaffID, AsyncCallback callback, object state);
        bool EndDeleteDrugDeptUnitByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDrugDeptUnit(RefUnit unit, AsyncCallback callback, object state);
        bool EndUpdateDrugDeptUnit(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewDrugDeptUnit(RefUnit newunit, AsyncCallback callback, object state);
        bool EndAddNewDrugDeptUnit(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugDeptUnitByID(long UnitID, AsyncCallback callback, object state);
        RefUnit EndGetDrugDeptUnitByID(IAsyncResult asyncResult);

        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSellingPriceFormularByID(long ItemID, AsyncCallback callback, object state);
        SellingPriceFormular EndGetSellingPriceFormularByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSellingPriceFormularIsActive(AsyncCallback callback, object state);
        SellingPriceFormular EndGetSellingPriceFormularIsActive(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSellingPriceFormularAll(DateTime? FromDate,DateTime? Todate,int PageIndex,int PageSize,AsyncCallback callback, object state);
        IList<SellingPriceFormular> EndGetSellingPriceFormularAll(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewSellingPriceFormular(SellingPriceFormular p,AsyncCallback callback, object state);
        bool EndAddNewSellingPriceFormular(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateSellingPriceFormular(SellingPriceFormular p, AsyncCallback callback, object state);
        bool EndUpdateSellingPriceFormular(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteSellingPriceFormular(long ItemID, AsyncCallback callback, object state);
        bool EndDeleteSellingPriceFormular(IAsyncResult asyncResult);

        // -------------- DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginLoadRefGeneralUnits( AsyncCallback callback, object state);
        //IList<RefGeneralUnits> EndLoadRefGeneralUnits(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTrainningTypeList(AsyncCallback callback, object state);
        IList<Lookup> EndGetTrainningTypeList(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTrainingForSubOrgList(TrainingForSubOrg Training, AsyncCallback callback, object state);
        IList<TrainingForSubOrg> EndGetTrainingForSubOrgList(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetScientificResearchActivityList(ScientificResearchActivities Activity, AsyncCallback callback, object state);
        IList<ScientificResearchActivities> EndGetScientificResearchActivityList(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertUpdateTrainingForSubOrg(bool ISAdd, TrainingForSubOrg objTraining, AsyncCallback callback, object state);
        bool EndInsertUpdateTrainingForSubOrg(out int Result, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]     
        IAsyncResult BeginInsertUpdateScientificResearchActivity(bool ISAdd, ScientificResearchActivities objActivity, AsyncCallback callback, object state);
        bool EndInsertUpdateScientificResearchActivity(out int Result, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteScientificResearchActivity(long ActivityID, AsyncCallback callback, object state);
        bool EndDeleteScientificResearchActivity(out int Result, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteTrainingForSubOrg(long TrainingID, AsyncCallback callback, object state);
        bool EndDeleteTrainingForSubOrg(out int Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginActivityClassListAll(long ClassID, AsyncCallback callback, object state);
        IList<ActivityClasses> EndActivityClassListAll(IAsyncResult asyncResult);
		// -------------- DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học
    }
}
