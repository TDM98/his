using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using System.Collections.ObjectModel;

namespace CommonRecordsService
{
    [ServiceContract]
    public interface ICommonRecordsService
    {
        #region Common

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupPrescriptionType(AsyncCallback callback, object state);
        List<Lookup> EndGetLookupPrescriptionType(IAsyncResult asyncResult);

        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionByServiceRecID(long patientID, long? ServiecRecID, long? PtRegistrationID, DateTime? ExamDate, AsyncCallback callback, object state);
        IList<Prescription> EndGetPrescriptionByServiceRecID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllPrescriptions(AsyncCallback callback, object state);
        IList<Prescription> EndGetAllPrescriptions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionByPtID(long patientID, AsyncCallback callback, object state);
        IList<Prescription> EndGetPrescriptionByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchPrescription(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<Prescription> EndSearchPrescription(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLatestPrescriptionByPtID(long patientID, AsyncCallback callback, object state);
        Prescription EndGetLatestPrescriptionByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetNewPrescriptionByPtID(long patientID, long doctorID, AsyncCallback callback, object state);
        Prescription EndGetNewPrescriptionByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionID(long PrescriptID, AsyncCallback callback, object state);
        Prescription EndGetPrescriptionID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionByIssueID(long IssueID, AsyncCallback callback, object state);
        Prescription EndGetPrescriptionByIssueID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePrescription(Prescription entity, AsyncCallback callback, object state);
        bool EndDeletePrescription(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginAddPrescription(Prescription entity, long patientID, long? PtRegistrationID, AsyncCallback callback, object state);
        //bool EndAddPrescription(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_Update(Prescription entity, AsyncCallback callback, object state);
        bool EndPrescriptions_Update(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginAddFullPrescription(Prescription entity, long? PtRegistrationID, string xmlPrescriptDetails,  AsyncCallback callback, object state);
        //bool EndAddFullPrescription(out long newPrescriptID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginAddFullPrescriptionByServiceRecID(Prescription entity, string xmlPrescriptDetails, AsyncCallback callback, object state);
        //bool EndAddFullPrescriptionByServiceRecID(out long newPrescriptID, IAsyncResult asyncResult);

        #region 2.GetDrugForPrescription


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex, int IsMedDept, string xmlPrescriptDetails, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForPrescription_Auto(IAsyncResult asyncResult);

        #endregion

        #region 3.Prescription Details

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionDetailsByPrescriptID(long PrescriptID, AsyncCallback callback, object state);
        IList<PrescriptionDetail> EndGetPrescriptionDetailsByPrescriptID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPrescriptDetailsByID(long PrescriptID, AsyncCallback callback, object state);
        //IList<PrescriptionDetail> EndGetPrescriptDetailsByID(IAsyncResult asyncResult);

        #endregion

        #region 4.PrescriptionIssueHistory

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddPrescriptIssueHistory(Prescription entity, AsyncCallback callback, object state);
        bool EndAddPrescriptIssueHistory(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginUpdatePrescriptIssueHistory(Prescription entity, AsyncCallback callback, object state);
        //bool EndUpdatePrescriptIssueHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionIssueHistory(long PrescriptID, AsyncCallback callback, object state);
        IList<PrescriptionIssueHistory> EndGetPrescriptionIssueHistory(IAsyncResult asyncResult);

        #endregion

        #region 5.patientPaymentOld


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetValuePatientPaymentOld(long PtRegistrationID, AsyncCallback callback, object state);
        FeeDrug EndGetValuePatientPaymentOld(IAsyncResult asyncResult);

        #endregion

        #region choose dose member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInitChooseDoses(AsyncCallback callback, object state);
        IList<ChooseDose> EndInitChooseDoses(IAsyncResult asyncResult);
        #endregion

    }
}
