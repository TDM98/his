/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
 * 20210527 #003 TNHX: 317 Lấy danh sách mã máy cho màn hình nhập kết quả xét nghiệm
 * 20210705 #004 BLQ: 330 + 381
 * 20220825 #005 BLQ: Thêm giao dịch chữ ký số
 * 20230403 #006 QTD: Thêm mã máy
 * 20230626 #007 QTD: Lấy danh sách hẹn SMS XN
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using aEMR.DataContracts;
using DataEntities;

namespace PCLsProxy
{
    [ServiceContract]
    public interface IPCLs
    {

        #region 0. PCLExamTypes Member
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPCLExamTypes_ByPCLFormID(long PCLFormID, int? ClassPatient, AsyncCallback callback, object state);
        //IList<PCLExamType> EndGetPCLExamTypes_ByPCLFormID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPCLExamTypes_ByPCLFormID_Edit(long PCLFormID, long PatientPCLReqID, int? ClassPatient, AsyncCallback callback, object state);
        //IList<PCLExamType> EndGetPCLExamTypes_ByPCLFormID_Edit(IAsyncResult asyncResult);
        #endregion

        #region 1. PCLForm Member
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetForms_ByDeptID(long? deptID, AsyncCallback callback, object state);
        //IList<PCLForm> EndGetForms_ByDeptID(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginPCLForms_ByDeptIDV_PCLCategory(Int64 DeptID, Int64 V_PCLCategory, AsyncCallback callback, object state);
        //IList<PCLForm> EndPCLForms_ByDeptIDV_PCLCategory(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllPCLGroups(long? pclCategoryID, AsyncCallback callback, object state);
        IList<PCLGroup> EndGetAllPCLGroups(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllActiveExamTypesPaging(PCLExamTypeSearchCriteria searchCriteria, int pageIndex, int pageSize, bool bCountTotal, string orderBy, AsyncCallback callback, object state);
        IList<PCLExamType> EndGetAllActiveExamTypesPaging(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllActiveExamTypes(PCLExamTypeSearchCriteria searchCriteria, AsyncCallback callback, object state);
        IList<PCLExamType> EndGetAllActiveExamTypes(IAsyncResult asyncResult);

        #endregion

        #region 2. PatientPCLRequest
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddFullPtPCLReq(PatientPCLRequest entity, long PatientID, long? ptRegDetailID, out long newPatientPCLReqID, AsyncCallback callback, object state);
        bool EndAddFullPtPCLReq(out long newPatientPCLReqID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginAddFullPtPCLReqServiceRecID(PatientPCLRequest entity, out long newPatientPCLReqID, AsyncCallback callback, object state);
        //bool EndAddFullPtPCLReqServiceRecID(out long newPatientPCLReqID, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginUpdateFullPtPCLReqServiceRecID(PatientPCLRequest entity, AsyncCallback callback, object state);
        //bool EndUpdateFullPtPCLReqServiceRecID(IAsyncResult asyncResult);

        //Hàm này viết lấy sai bét
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPMRByPtPCLFormRequest(long LoggedDoctorID, long PatientID, long? ptRegDetailID, long? PatientRecID, AsyncCallback callback, object state);
        //PatientMedicalRecord EndGetPMRByPtPCLFormRequest(IAsyncResult asyncResult);
        //Hàm này viết lấy sai bét

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequest_ByPatientID_Paging(PatientPCLRequestSearchCriteria SearchCriteria, long V_RegistrationType,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndPatientPCLRequest_ByPatientID_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientPCLRequestList_ByRegistrationID(long RegistrationID, long V_RegistrationType, long V_PCLMainCategory, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndGetPatientPCLRequestList_ByRegistrationID(IAsyncResult asyncResult);

        /// <summary>
        /// Author: VuTTM
        /// Description: Getting the deleted PCL request list.
        /// </summary>
        /// <param name="RegistrationID"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientDeletedPCLRequestListByRegistrationID(long RegistrationID, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndGetPatientDeletedPCLRequestListByRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequestDetail_ByPatientPCLReqID(PatientPCLRequestDetailSearchCriteria SearchCriteria, long V_RegistrationType, AsyncCallback callback, object state);
        IList<PatientPCLRequestDetail> EndPatientPCLRequestDetail_ByPatientPCLReqID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequestDetail_ByPtRegistrationID(long PtRegistrationID, long V_RegistrationType, long V_PCLMainCategory, AsyncCallback callback, object state);
        IList<PatientPCLRequestDetail> EndPatientPCLRequestDetail_ByPtRegistrationID(IAsyncResult asyncResult);

        //Phiếu cuối cùng
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequest_RequestLastest(Int64 PatientID, long V_RegistrationType, long? V_PCLMainCategory, AsyncCallback callback, object state);
        PatientPCLRequest EndPatientPCLRequest_RequestLastest(IAsyncResult asyncResult);
        //Phiếu cuối cùng

        //Danh sách phiếu yêu cầu CLS
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequest_SearchPaging(PatientPCLRequestSearchCriteria SearchCriteria,

            int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndPatientPCLRequest_SearchPaging(out int Total, IAsyncResult asyncResult);
        //Danh sách phiếu yêu cầu CLS

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientPCLRequestResultsByReqID(PatientPCLRequestSearchCriteria SearchCriteria, AsyncCallback callback, object state);
        PatientPCLRequest EndGetPatientPCLRequestResultsByReqID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCheckTemplatePCLResultByReqID(long PatientPCLReqID, bool InPt, AsyncCallback callback, object state);
        bool EndCheckTemplatePCLResultByReqID(out bool IsNewTemplate,out long V_ReportForm,out long PCLImgResultID,out long V_PCLRequestType,out string TemplateResultString, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLResult_Criterion(long ServiceRecID, AsyncCallback callback, object state);
        bool EndGetPCLResult_Criterion(out long V_ResultType, out string TemplateResultString, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequest_ViewResult_SearchPaging(PatientPCLRequestSearchCriteria SearchCriteria,
            int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndPatientPCLRequest_ViewResult_SearchPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequest_ViewResult_SearchPagingNew(PatientPCLRequestSearchCriteria SearchCriteria,
            int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndPatientPCLRequest_ViewResult_SearchPagingNew(out int Total, IAsyncResult asyncResult);

        //Danh sách phiếu các lần trước
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequest_ByPatientIDV_Param_Paging(PatientPCLRequestSearchCriteria SearchCriteria,

            int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndPatientPCLRequest_ByPatientIDV_Param_Paging(out int Total, IAsyncResult asyncResult);
        //Danh sách phiếu các lần trước

        #endregion

        #region 3.PCL results
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPCLResult_PtPCLRequest(long? registrationID, bool isImported, long V_PCLCategory, AsyncCallback callback, object state);
        //IList<PatientPCLRequest> EndGetPCLResult_PtPCLRequest(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLResult_PCLExamGroup(long? ptID, long? ptPCLReqID, bool isImported, AsyncCallback callback, object state);
        IList<PCLExamGroup> EndGetPCLResult_PCLExamGroup(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPCLResult_PCLExamType(long? ptID, long? PCLExamGroupID, bool isImported, AsyncCallback callback, object state);
        //IList<PCLExamType> EndGetPCLResult_PCLExamType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID, AsyncCallback callback, object state);
        IList<PCLResultFileStorageDetail> EndGetPCLResultFileStoreDetails(IAsyncResult asyncResult);

        /*▼====: #001*/
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLResultFileStoreDetails_V2(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID, long V_PCLRequestType, AsyncCallback callback, object state);
        IList<PCLResultFileStorageDetail> EndGetPCLResultFileStoreDetails_V2(IAsyncResult asyncResult);
        /*▲====: #001*/

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLResultFileStoreDetailsExt(PCLResultFileStorageDetailSearchCriteria p, AsyncCallback callback, object state);
        IList<PCLResultFileStorageDetail> EndGetPCLResultFileStoreDetailsExt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLResultFileStoreDetailsByPCLImgResultID(long PCLImgResultID, AsyncCallback callback, object state);
        IList<PCLResultFileStorageDetail> EndGetPCLResultFileStoreDetailsByPCLImgResultID(IAsyncResult asyncResult);
        #endregion

        #region 4. PCL Laboratory
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllMedSpecCatg(AsyncCallback callback, object state);
        IList<MedicalSpecimensCategory> EndGetAllMedSpecCatg(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMedSpecsByCatgID(short? medSpecCatgID, AsyncCallback callback, object state);
        IList<MedicalSpecimen> EndGetMedSpecsByCatgID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPtPCLLabExamTypes_RefByPtPCLReqID(long? PtPCLReqID, long? PCLExamGroupID, AsyncCallback callback, object state);
        //IList<PCLExamType> EndGetPtPCLLabExamTypes_RefByPtPCLReqID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPtPCLLabExamTypes_ByPtPCLReqID(long PatientID, long? PtPCLReqID, long? PCLExamGroupID, long? PCLExamTypeID, bool isImported, AsyncCallback callback, object state);
        IList<PatientPCLLaboratoryResultDetail> EndGetPtPCLLabExamTypes_ByPtPCLReqID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMedSpecInfo_ByPtPCLReqID(long? PtPCLReqID, AsyncCallback callback, object state);
        IList<MedicalSpecimenInfo> EndGetMedSpecInfo_ByPtPCLReqID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTestItems_ByPatientID(long PatientID, PatientPCLRequestSearchCriteria SearchCriteria, AsyncCallback callback, object state);
        string EndPCLExamTestItems_ByPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLLaboratoryResults_ByExamTest_Paging(long PatientID, long PCLExamTestItemID, long PCLExamTypeID, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, long PatientFindBy, AsyncCallback callback, object state);
        IList<PCLExamTestItems> EndPatientPCLLaboratoryResults_ByExamTest_Paging(out int TotalRow, out int MaxRow, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLLaboratoryResults_ByExamTest_Crosstab(long PatientID, string strXml, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, AsyncCallback callback, object state);
        string EndPatientPCLLaboratoryResults_ByExamTest_Crosstab(out int TotalCol, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTestItems_SearchPaging(GeneralSearchCriteria SearchCriteria, int PageIndex, int PageSize, bool bCount, AsyncCallback callback, object state);
        IList<PCLExamTestItems> EndPCLExamTestItems_SearchPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTestItems_Save(PCLExamTestItems Item, AsyncCallback callback, object state);
        bool EndPCLExamTestItems_Save(out long PCLExamTestItemID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDepartmentReqCashAdv_Save(RefDepartmentReqCashAdv Target, AsyncCallback callback, object state);
        bool EndRefDepartmentReqCashAdv_Save(out long ID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDepartmentReqCashAdv_Delete(long RefDepartmentReqCashAdvID, AsyncCallback callback, object state);
        bool EndRefDepartmentReqCashAdv_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDepartmentReqCashAdv_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, AsyncCallback callback, object state);
        List<RefDepartmentReqCashAdv> EndRefDepartmentReqCashAdv_GetAll(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeServiceTarget_Save(PCLExamTypeServiceTarget Target, AsyncCallback callback, object state);
        bool EndPCLExamTypeServiceTarget_Save(out long ID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeServiceTarget_Delete(long PCLExamTypeServiceTargetID, AsyncCallback callback, object state);
        bool EndPCLExamTypeServiceTarget_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeServiceTarget_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, AsyncCallback callback, object state);
        List<PCLExamTypeServiceTarget> EndPCLExamTypeServiceTarget_GetAll(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeServiceTarget_Checked(long PCLExamTypeID, DateTime Date, AsyncCallback callback, object state);
        bool EndPCLExamTypeServiceTarget_Checked(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeServiceTarget_Checked_Appointment(long PCLExamTypeID, DateTime Date, AsyncCallback callback, object state);
        bool EndPCLExamTypeServiceTarget_Checked_Appointment(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLLaboratoryResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType, AsyncCallback callback, object state);
        IList<PatientPCLLaboratoryResultDetail> EndPCLLaboratoryResults_With_ResultOld(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLExamTypeName(long PatientID, long PatientPCLReqID, long V_PCLRequestType, AsyncCallback callback, object state);
        string EndGetPCLExamTypeName(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLLaboratoryResults_No_ResultOld(long PatientID, long PatientPCLReqID, AsyncCallback callback, object state);
        IList<PatientPCLLaboratoryResultDetail> EndPCLLaboratoryResults_No_ResultOld(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginAddFullPtPCLLabResult(PatientPCLLaboratoryResult entity, out long newLabResultID, AsyncCallback callback, object state);
        //bool EndAddFullPtPCLLabResult(out long newLabResultID, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddPatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity, AsyncCallback callback, object state);
        bool EndAddPatientPCLLaboratoryResultDetail(IAsyncResult asyncResult);

        // 20181216 TNHX: [BM0005430] Add PatientID for report PCLDEPARTMENT_LABORATORY_RESULT
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePatientPCLLaboratoryResultDetailXml(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity
            , PatientPCLLaboratoryResult entity, long PCLRequestTypeID, long PatientID,bool? IsWaitResult,bool? IsDone, AsyncCallback callback, object state);
        bool EndUpdatePatientPCLLaboratoryResultDetailXml(out string errorOutput, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePatientPCLLaboratoryResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID, long PCLExamTypeID, AsyncCallback callback, object state);
        bool EndDeletePatientPCLLaboratoryResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePatientPCLImagingResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID, AsyncCallback callback, object state);
        bool EndDeletePatientPCLImagingResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity, AsyncCallback callback, object state);
        bool EndDeletePatientPCLLaboratoryResultDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginListPatientPCLRequest_LAB_Paging(long PatientID, long? DeptLocID, long V_PCLRequestType,

            int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndListPatientPCLRequest_LAB_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginLIS_Order(string SoPhieuChiDinh, bool IsAll, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndLIS_Order(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginLIS_Result(PatientPCLRequest_LABCom ParamLabCom, AsyncCallback callback, object state);
        bool EndLIS_Result(IAsyncResult asyncResult);


        #endregion

        #region 5. Tuyen
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPCLItemsByPCLFormID(long? PCLFormID, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<PCLItem> EndGetPCLItemsByPCLFormID(out int totalCount, IAsyncResult asyncResult);
        #endregion


        #region 6.Dinh

        #region PCLExamParamResult

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetPCLExamParamResultList(long PCLExamResultTemplateID, long ParamEnum, AsyncCallback callback, object state);
        List<PCLExamParamResult> EndGetPCLExamParamResultList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetPCLExamParamResult(long PCLExamResultTemplateID, long ParamEnum, AsyncCallback callback, object state);
        PCLExamParamResult EndGetPCLExamParamResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddPCLExamParamResult(PCLExamParamResult entity, AsyncCallback callback, object state);
        bool EndAddPCLExamParamResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdatePCLExamParamResult(PCLExamParamResult entity, AsyncCallback callback, object state);
        bool EndUpdatePCLExamParamResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeletePCLExamParamResult(PCLExamParamResult entity, AsyncCallback callback, object state);
        bool EndDeletePCLExamParamResult(IAsyncResult asyncResult);

        #endregion

        #region PCLExamResultTemplate

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetPCLExamResultTemplateList(long PCLExamGroupTemplateResultID, AsyncCallback callback, object state);
        List<PCLExamResultTemplate> EndGetPCLExamResultTemplateList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetPCLExamResultTemplateListByTypeID(long PCLExamGroupTemplateResultID, int ParamEnum, AsyncCallback callback, object state);
        List<PCLExamResultTemplate> EndGetPCLExamResultTemplateListByTypeID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetPCLExamResultTemplate(long PCLExamResultTemplateID, AsyncCallback callback, object state);
        PCLExamResultTemplate EndGetPCLExamResultTemplate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddPCLExamResultTemplate(PCLExamResultTemplate entity, AsyncCallback callback, object state);
        bool EndAddPCLExamResultTemplate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdatePCLExamResultTemplate(PCLExamResultTemplate entity, AsyncCallback callback, object state);
        bool EndUpdatePCLExamResultTemplate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeletePCLExamResultTemplate(PCLExamResultTemplate entity, AsyncCallback callback, object state);
        bool EndDeletePCLExamResultTemplate(IAsyncResult asyncResult);

        #endregion

        /*▼====: #002*/
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetResourcesForMedicalServicesListByTypeID(long PCLResultParamImpID, AsyncCallback callback, object state);
        List<Resources> EndGetResourcesForMedicalServicesListByTypeID(IAsyncResult asyncResult);
        /*▲====: #002*/
        //▼====: #003
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetResourcesForLaboratory(AsyncCallback callback, object state);
        List<Resources> EndGetResourcesForLaboratory(IAsyncResult asyncResult);
        //▲====: #003

        #region UltraResParams_EchoCardiography

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_EchoCardiography(long UltraResParams_EchoCardiographyID, long PCLImgResultID, AsyncCallback callback, object state);
        UltraResParams_EchoCardiography EndGetUltraResParams_EchoCardiography(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_EchoCardiographyResult(long UltraResParams_EchoCardiographyID, long PCLImgResultID, AsyncCallback callback, object state);
        string EndGetUltraResParams_EchoCardiographyResult(IAsyncResult asyncResult);

        //==== 20161013 CMN Begin: Add PCL Image Method
        //[OperationContract(AsyncPattern = true)]
        //[FaultContract(typeof(AxException))]
        //IAsyncResult BeginAddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity, AsyncCallback callback, object state);
        //bool EndAddUltraResParams_EchoCardiography(IAsyncResult asyncResult);
        //[OperationContract(AsyncPattern = true)]
        //[FaultContract(typeof(AxException))]
        //IAsyncResult BeginUpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity, AsyncCallback callback, object state);
        //bool EndUpdateUltraResParams_EchoCardiography(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete, AsyncCallback callback, object state);
        bool EndAddUltraResParams_EchoCardiography(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete, AsyncCallback callback, object state);
        bool EndUpdateUltraResParams_EchoCardiography(IAsyncResult asyncResult);
        //==== 20161013 CMN End.

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity, AsyncCallback callback, object state);
        bool EndDeleteUltraResParams_EchoCardiography(IAsyncResult asyncResult);

        #endregion
        //==== 20161214 CMN Begin: Add General information
        #region UltraResParams_FetalEchocardiography
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_FetalEchocardiography(long PatientPCLReqID, AsyncCallback callback, object state);
        UltraResParams_FetalEchocardiography EndGetUltraResParams_FetalEchocardiography(IAsyncResult asyncResult);
        #endregion
        //==== 20161214 CMN End.
        #region UltraResParams_FetalEchocardiography2D
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_FetalEchocardiography2D(long UltraResParams_FetalEchocardiography2DID
            , long PCLImgResultID
            , AsyncCallback callback, object state);
        UltraResParams_FetalEchocardiography2D EndGetUltraResParams_FetalEchocardiography2D(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity
            , AsyncCallback callback, object state);
        bool EndAddUltraResParams_FetalEchocardiography2D(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity
            , AsyncCallback callback, object state);
        bool EndUpdateUltraResParams_FetalEchocardiography2D(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity
            , AsyncCallback callback, object state);
        bool EndDeleteUltraResParams_FetalEchocardiography2D(IAsyncResult asyncResult);

        #endregion

        #region UltraResParams_FetalEchocardiography Doppler
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_FetalEchocardiographyDoppler(long UltraResParams_FetalEchocardiographyDopplerID
            , AsyncCallback callback, object state);
        IList<UltraResParams_FetalEchocardiographyDoppler> EndGetUltraResParams_FetalEchocardiographyDoppler(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_FetalEchocardiographyDopplerByID(long UltraResParams_FetalEchocardiographyDopplerID
            , long PCLImgResultID, AsyncCallback callback, object state);
        UltraResParams_FetalEchocardiographyDoppler EndGetUltraResParams_FetalEchocardiographyDopplerByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity
            , AsyncCallback callback, object state);
        bool EndAddUltraResParams_FetalEchocardiographyDoppler(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity
            , AsyncCallback callback, object state);
        bool EndUpdateUltraResParams_FetalEchocardiographyDoppler(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity
            , AsyncCallback callback, object state);
        bool EndDeleteUltraResParams_FetalEchocardiographyDoppler(IAsyncResult asyncResult);

        #endregion

        #region UltraResParams_FetalEchocardiographyPostpartum

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_FetalEchocardiographyPostpartum(long UltraResParams_FetalEchocardiographyPostpartumID
            , AsyncCallback callback, object state);
        IList<UltraResParams_FetalEchocardiographyPostpartum>
            EndGetUltraResParams_FetalEchocardiographyPostpartum(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_FetalEchocardiographyPostpartumByID(long UltraResParams_FetalEchocardiographyPostpartumID
            , long PCLImgResultID, AsyncCallback callback, object state);
        UltraResParams_FetalEchocardiographyPostpartum EndGetUltraResParams_FetalEchocardiographyPostpartumByID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity
            , AsyncCallback callback, object state);
        bool EndAddUltraResParams_FetalEchocardiographyPostpartum(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity
            , AsyncCallback callback, object state);
        bool EndUpdateUltraResParams_FetalEchocardiographyPostpartum(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity
            , AsyncCallback callback, object state);
        bool EndDeleteUltraResParams_FetalEchocardiographyPostpartum(IAsyncResult asyncResult);

        #endregion

        #region UltraResParams_FetalEchocardiographyResult

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_FetalEchocardiographyResult(
            long UltraResParams_FetalEchocardiographyResultID, AsyncCallback callback, object state);
        IList<UltraResParams_FetalEchocardiographyResult> EndGetUltraResParams_FetalEchocardiographyResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetUltraResParams_FetalEchocardiographyResultByID(
            long UltraResParams_FetalEchocardiographyResultID, long PCLImgResultID, AsyncCallback callback, object state);
        UltraResParams_FetalEchocardiographyResult EndGetUltraResParams_FetalEchocardiographyResultByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity
            , AsyncCallback callback, object state);
        bool EndAddUltraResParams_FetalEchocardiographyResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity
            , AsyncCallback callback, object state);
        bool EndUpdateUltraResParams_FetalEchocardiographyResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity
            , AsyncCallback callback, object state);
        bool EndDeleteUltraResParams_FetalEchocardiographyResult(IAsyncResult asyncResult);
        #endregion

        #region Abdominal Ultrasound

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginInsertAbdominalUltrasoundResult(AbdominalUltrasound entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete, AsyncCallback callback, object state);
        bool EndInsertAbdominalUltrasoundResult(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateAbdominalUltrasoundResult(AbdominalUltrasound entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete, AsyncCallback callback, object state);
        bool EndUpdateAbdominalUltrasoundResult(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetAbdominalUltrasoundResult(long PatientPCLReqID, AsyncCallback callback, object state);
        AbdominalUltrasound EndGetAbdominalUltrasoundResult(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_Exam

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_Exam(long URP_FE_ExamID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_Exam EndGetURP_FE_Exam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_Exam(URP_FE_Exam entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_Exam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_Exam(URP_FE_Exam entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_Exam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_Exam(URP_FE_Exam entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_Exam(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_Oesophagienne

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_Oesophagienne(long URP_FE_OesophagienneID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_Oesophagienne EndGetURP_FE_Oesophagienne(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_Oesophagienne(URP_FE_Oesophagienne entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_Oesophagienne(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_Oesophagienne(URP_FE_Oesophagienne entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_Oesophagienne(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_Oesophagienne(URP_FE_Oesophagienne entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_Oesophagienne(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_OesophagienneCheck

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_OesophagienneCheck(long URP_FE_OesophagienneCheckID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_OesophagienneCheck EndGetURP_FE_OesophagienneCheck(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_OesophagienneCheck(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_OesophagienneCheck(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_OesophagienneCheck(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_OesophagienneDiagnosis

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_OesophagienneDiagnosis(long URP_FE_OesophagienneDiagnosisID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_OesophagienneDiagnosis EndGetURP_FE_OesophagienneDiagnosis(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_OesophagienneDiagnosis(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_OesophagienneDiagnosis(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_OesophagienneDiagnosis(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_StressDipyridamole

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDipyridamole(long URP_FE_StressDipyridamoleID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDipyridamole EndGetURP_FE_StressDipyridamole(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDipyridamole(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDipyridamole(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDipyridamole(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_StressDipyridamoleElectrocardiogram

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDipyridamoleElectrocardiogram(long URP_FE_StressDipyridamoleElectrocardiogramID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDipyridamoleElectrocardiogram EndGetURP_FE_StressDipyridamoleElectrocardiogram(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDipyridamoleElectrocardiogram(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDipyridamoleElectrocardiogram(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDipyridamoleElectrocardiogram(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_StressDipyridamoleExam

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDipyridamoleExam(long URP_FE_StressDipyridamoleExamID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDipyridamoleExam EndGetURP_FE_StressDipyridamoleExam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDipyridamoleExam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDipyridamoleExam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDipyridamoleExam(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_StressDipyridamoleImage

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDipyridamoleImage(long URP_FE_StressDipyridamoleImageID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDipyridamoleImage EndGetURP_FE_StressDipyridamoleImage(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDipyridamoleImage(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDipyridamoleImage(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDipyridamoleImage(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_StressDipyridamoleResult

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDipyridamoleResult(long URP_FE_StressDipyridamoleResultID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDipyridamoleResult EndGetURP_FE_StressDipyridamoleResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDipyridamoleResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDipyridamoleResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDipyridamoleResult(IAsyncResult asyncResult);

        #endregion


        #region URP_FE_StressDobutamine

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDobutamine(long URP_FE_StressDobutamineID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDobutamine EndGetURP_FE_StressDobutamine(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDobutamine(URP_FE_StressDobutamine entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDobutamine(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDobutamine(URP_FE_StressDobutamine entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDobutamine(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDobutamine(URP_FE_StressDobutamine entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDobutamine(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_StressDobutamineElectrocardiogram

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDobutamineElectrocardiogram(long URP_FE_StressDobutamineElectrocardiogramID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDobutamineElectrocardiogram EndGetURP_FE_StressDobutamineElectrocardiogram(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDobutamineElectrocardiogram(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDobutamineElectrocardiogram(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDobutamineElectrocardiogram(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_StressDobutamineExam

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDobutamineExam(long URP_FE_StressDobutamineExamID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDobutamineExam EndGetURP_FE_StressDobutamineExam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDobutamineExam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDobutamineExam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDobutamineExam(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_StressDobutamineImages

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDobutamineImages(long URP_FE_StressDobutamineImagesID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDobutamineImages EndGetURP_FE_StressDobutamineImages(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDobutamineImages(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDobutamineImages(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDobutamineImages(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_StressDobutamineResult

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_StressDobutamineResult(long URP_FE_StressDobutamineResultID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_StressDobutamineResult EndGetURP_FE_StressDobutamineResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_StressDobutamineResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_StressDobutamineResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_StressDobutamineResult(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_VasculaireAnother

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_VasculaireAnother(long URP_FE_VasculaireAnotherID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_VasculaireAnother EndGetURP_FE_VasculaireAnother(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_VasculaireAnother(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_VasculaireAnother(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_VasculaireAnother(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_VasculaireAorta

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_VasculaireAorta(long URP_FE_VasculaireAortaID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_VasculaireAorta EndGetURP_FE_VasculaireAorta(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_VasculaireAorta(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_VasculaireAorta(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_VasculaireAorta(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_VasculaireCarotid

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_VasculaireCarotid(long URP_FE_VasculaireCarotidID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_VasculaireCarotid EndGetURP_FE_VasculaireCarotid(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_VasculaireCarotid(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_VasculaireCarotid(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_VasculaireCarotid(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_VasculaireExam

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_VasculaireExam(long URP_FE_VasculaireExamID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_VasculaireExam EndGetURP_FE_VasculaireExam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_VasculaireExam(URP_FE_VasculaireExam entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_VasculaireExam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_VasculaireExam(URP_FE_VasculaireExam entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_VasculaireExam(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_VasculaireExam(URP_FE_VasculaireExam entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_VasculaireExam(IAsyncResult asyncResult);

        #endregion

        #region URP_FE_VasculaireLeg

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetURP_FE_VasculaireLeg(long URP_FE_VasculaireLegID, long PCLImgResultID, AsyncCallback callback, object state);
        URP_FE_VasculaireLeg EndGetURP_FE_VasculaireLeg(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity, AsyncCallback callback, object state);
        bool EndAddURP_FE_VasculaireLeg(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity, AsyncCallback callback, object state);
        bool EndUpdateURP_FE_VasculaireLeg(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity, AsyncCallback callback, object state);
        bool EndDeleteURP_FE_VasculaireLeg(IAsyncResult asyncResult);

        #endregion

        //==== 20161129 CMN Bein: Add button save for all FE pages
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddAndUpdateFE_Vasculaire(URP_FE_Vasculaire entity, AsyncCallback callback, object state);
        bool EndAddAndUpdateFE_Vasculaire(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddAndUpdateUltraResParams_FetalEchocardiography(UltraResParams_FetalEchocardiography entity, AsyncCallback callback, object state);
        bool EndAddAndUpdateUltraResParams_FetalEchocardiography(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddAndUpdateURP_FE_Oesophagienne(URP_FE_OesophagienneUltra entity, AsyncCallback callback, object state);
        bool EndAddAndUpdateURP_FE_Oesophagienne(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddAndUpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamoleUltra entity, AsyncCallback callback, object state);
        bool EndAddAndUpdateURP_FE_StressDipyridamole(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginAddAndUpdateURP_FE_StressDobutamine(URP_FE_StressDobutamineUltra entity, AsyncCallback callback, object state);
        bool EndAddAndUpdateURP_FE_StressDobutamine(IAsyncResult asyncResult);
        //==== 20161129 CMN End: Add button save for all FE pages
        #endregion

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetPCLExamTypeServiceApmtTarget(long PCLExamTypeID, DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        IList<PatientPCLRequestDetail> EndGetPCLExamTypeServiceApmtTarget(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequest_SearchPaging_ForMedicalExamination(PatientPCLRequestSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndPatientPCLRequest_SearchPaging_ForMedicalExamination(out int Total, IAsyncResult asyncResult);

        //▼====: #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateReceptionTime(long PatientPCLReqID,DateTime ReceptionTime, long V_PCLRequestType, AsyncCallback callback, object state);
        void EndUpdateReceptionTime(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDateStarted2(long PtRegDetailID, DateTime DateStarted2, AsyncCallback callback, object state);
        void EndUpdateDateStarted2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePrescriptionsDate(long PtRegistrationID, DateTime PrescriptionsDate, AsyncCallback callback, object state);
        void EndUpdatePrescriptionsDate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetHistoryPCLByPCLExamType(long PatientID, long PCLExamTypeID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequestDetail> EndGetHistoryPCLByPCLExamType(out int Total, IAsyncResult asyncResult);
        //▲====: #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLImagingResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType, AsyncCallback callback, object state);
        IList<PatientPCLImagingResultDetail> EndPCLImagingResults_With_ResultOld(IAsyncResult asyncResult);

        //▼====: #005
        #region Digital Signature Transaction
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientPCLLaboratoryResultForSendTransaction(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction, int PageIndex, int PageSize, 
            string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndGetPatientPCLLaboratoryResultForSendTransaction(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetListHistoryTransaction_Paging(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction, int PageIndex, int PageSize, 
            string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndGetListHistoryTransaction_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdatePCLRequestStatusGeneratingResult(string ListPCLResults, long V_PCLRequestStatus, AsyncCallback callback, object state);
        bool EndUpdatePCLRequestStatusGeneratingResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeletePCLRequestFromList(PatientPCLRequest request, int PatientFindBy, AsyncCallback callback, object state);
        bool EndDeletePCLRequestFromList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteTransactionHistory(PatientPCLRequest request, int PatientFindBy, AsyncCallback callback, object state);
        bool EndDeleteTransactionHistory(IAsyncResult asyncResult);
        #endregion
        //▲====: #005

        //▼====: #006
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetResourcesForMedicalServicesListByDeptIDAndTypeID(long DeptID, long PtRegDetailID, AsyncCallback callback, object state);
        List<Resources> EndGetResourcesForMedicalServicesListByDeptIDAndTypeID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetResourcesForMedicalServicesListBySmallProcedureID(long SmallProcedureID, AsyncCallback callback, object state);
        List<Resources> EndGetResourcesForMedicalServicesListBySmallProcedureID(IAsyncResult asyncResult);
        //▲====: #006

        //▼====: #007
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetListAppointmentsLab_Paging(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction, int PageIndex, int PageSize,
            string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        IList<PatientPCLRequest> EndGetListAppointmentsLab_Paging(out int Total, IAsyncResult asyncResult);
        //▲====: #007
    }
}