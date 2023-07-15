/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
 * 20180613 #002 TBLD: Tao contract cho GetResourcesForMedicalServicesListByTypeID
 * 20210527 #003 TNHX: 317 Lấy danh sách mã máy cho màn hình nhập kết quả xét nghiệm
 * 20210705 #004 BLQ: 330 + 381
 * 20220825 #005 BLQ: Thêm giao dịch chữ ký số
 * 20230401 #006 QTD: Lấy mã máy theo khoa và nhóm DVKT
 * 20230626 #007 QTD: Lấy danh sách hẹn SMS XN
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;
namespace ConsultationsService.ParaClinical
{
    [ServiceContract]
    public interface IPCLs
    {
        #region 0. PCLExamTypes Member
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLExamType> GetPCLExamTypes_ByPCLFormID(long PCLFormID,int? ClassPatient);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLExamType> GetPCLExamTypes_ByPCLFormID_Edit(long PCLFormID, long PatientPCLReqID, int? ClassPatient);
        #endregion

        #region 1. PCLForm Member
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLForm> GetForms_ByDeptID(long? deptID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLForm> PCLForms_ByDeptIDV_PCLCategory(Int64 DeptID, Int64 V_PCLCategory);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLGroup> GetAllPCLGroups(long? pclCategoryID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamType> GetAllActiveExamTypesPaging(PCLExamTypeSearchCriteria searchCriteria, int pageIndex, int pageSize, bool bCountTotal, string orderBy, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamType> GetAllActiveExamTypes(PCLExamTypeSearchCriteria searchCriteria);
        #endregion

        #region 2. PatientPCLRequest

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddFullPtPCLReq(PatientPCLRequest entity, long PatientID, long? ptRegDetailID, out long newPatientPCLReqID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddFullPtPCLReqServiceRecID(PatientPCLRequest entity, out long newPatientPCLReqID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool UpdateFullPtPCLReqServiceRecID(PatientPCLRequest entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientMedicalRecord GetPMRByPtPCLFormRequest(long LoggedDoctorID, long PatientID, long? ptRegDetailID,long? PatientRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientPCLRequest> PatientPCLRequest_ByPatientID_Paging(
            PatientPCLRequestSearchCriteria SearchCriteria, long V_RegistrationType, 

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientPCLRequest> GetPatientPCLRequestList_ByRegistrationID(long RegistrationID, long V_RegistrationType, long V_PCLMainCategory);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientPCLRequest> GetPatientDeletedPCLRequestListByRegistrationID(long RegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPatientPCLReqID(PatientPCLRequestDetailSearchCriteria SearchCriteria, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPtRegistrationID(long PtRegistrationID, long V_RegistrationType, long V_PCLMainCategory);


        //Phiếu cuối cùng
        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLRequest PatientPCLRequest_RequestLastest(Int64 PatientID, long V_RegistrationType, long? V_PCLMainCategory);
        //Phiếu cuối cùng
        

        //Danh sách phiếu yêu cầu CLS
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> PatientPCLRequest_SearchPaging(
            PatientPCLRequestSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total);
        //Danh sách phiếu yêu cầu CLS

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLRequest GetPatientPCLRequestResultsByReqID(PatientPCLRequestSearchCriteria SearchCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CheckTemplatePCLResultByReqID(long PatientPCLReqID, bool InPt, out bool IsNewTemplate, out long V_ReportForm, out long PCLImgResultID, out long V_PCLRequestType, out string TemplateResultString);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GetPCLResult_Criterion(long ServiceRecID, out long V_ResultType, out string TemplateResultString);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPaging(
                     PatientPCLRequestSearchCriteria SearchCriteria,
          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
         out int Total);

        //union ngoai vien
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPagingNew(
                     PatientPCLRequestSearchCriteria SearchCriteria,
          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
         out int Total);


        //Danh sách phiếu các lần trước
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> PatientPCLRequest_ByPatientIDV_Param_Paging(
            PatientPCLRequestSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total);
        //Danh sách phiếu các lần trước



        #endregion

        #region 3.PCL results
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PatientPCLRequest> GetPCLResult_PtPCLRequest(long? registrationID, bool isImported, long V_PCLCategory);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamGroup> GetPCLResult_PCLExamGroup(long? ptID, long? ptPCLReqID, bool isImported);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLExamType> GetPCLResult_PCLExamType(long? ptID, long? PCLExamGroupID, bool isImported);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID);

        /*▼====: #001*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails_V2(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID, long V_PCLRequestType);
        /*▲====: #001*/

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsExt(PCLResultFileStorageDetailSearchCriteria p);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsByPCLImgResultID(long PCLImgResultID);
        
        #endregion

        #region 4. PCL Laboratory
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<MedicalSpecimensCategory> GetAllMedSpecCatg();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<MedicalSpecimen> GetMedSpecsByCatgID(short? medSpecCatgID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLExamType> GetPtPCLLabExamTypes_RefByPtPCLReqID(long? PtPCLReqID, long? PCLExamGroupID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLLaboratoryResultDetail> GetPtPCLLabExamTypes_ByPtPCLReqID(long PatientID, long? PtPCLReqID, long? PCLExamGroupID, long? PCLExamTypeID, bool isImported);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<MedicalSpecimenInfo> GetMedSpecInfo_ByPtPCLReqID(long? PtPCLReqID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        string PCLExamTestItems_ByPatientID(long PatientID, PatientPCLRequestSearchCriteria SearchCriteria);

       
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamTestItems> PatientPCLLaboratoryResults_ByExamTest_Paging(long PatientID, long PCLExamTestItemID, long PCLExamTypeID, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, long PatientFindBy, out int TotalRow, out int MaxRow);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        string PatientPCLLaboratoryResults_ByExamTest_Crosstab(long PatientID, string strXml, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalCol);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamTestItems> PCLExamTestItems_SearchPaging(GeneralSearchCriteria SearchCriteria, int PageIndex, int PageSize, bool bCount, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTestItems_Save(PCLExamTestItems Item, out long PCLExamTestItemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeServiceTarget_Save(PCLExamTypeServiceTarget Target, out long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RefDepartmentReqCashAdv_Save(RefDepartmentReqCashAdv Target, out long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RefDepartmentReqCashAdv_Delete(long RefDepartmentReqCashAdvID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeServiceTarget_Delete(long PCLExamTypeServiceTargetID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypeServiceTarget> PCLExamTypeServiceTarget_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount,out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeServiceTarget_Checked(long PCLExamTypeID, DateTime Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeServiceTarget_Checked_Appointment(long PCLExamTypeID, DateTime Date);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        string GetPCLExamTypeName(long PatientID, long PatientPCLReqID, long V_PCLRequestType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_No_ResultOld(long PatientPCLReqID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddFullPtPCLLabResult(PatientPCLLaboratoryResult entity, out long newLabResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool UpdatePatientPCLLaboratoryResultDetail(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity);
        // 20181216 TNHX: [BM0005430] Add PatientID for report PCLDEPARTMENT_LABORATORY_RESULT
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePatientPCLLaboratoryResultDetailXml(
            IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity
            , PatientPCLLaboratoryResult entity, long PCLRequestTypeID, long PatientID,bool? IsWaitResult,bool? IsDone, out string errorOutput);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePatientPCLLaboratoryResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID, long PCLExamTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePatientPCLImagingResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> ListPatientPCLRequest_LAB_Paging(long PatientID,long? DeptLocID, long V_PCLRequestType,
        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        out int Total);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> LIS_Order(string SoPhieuChiDinh, bool IsAll);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool LIS_Result(PatientPCLRequest_LABCom ParamLabCom);


        #endregion

        #region 5. Tuyen
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLItem> GetPCLItemsByPCLFormID(long? PCLFormID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
#endregion


#region 6.Dinh

        #region PCLExamParamResult

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamParamResult> GetPCLExamParamResultList(long PCLExamResultTemplateID, long ParamEnum);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PCLExamParamResult GetPCLExamParamResult(long PCLExamResultTemplateID,long ParamEnum);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPCLExamParamResult(PCLExamParamResult entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePCLExamParamResult(PCLExamParamResult entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePCLExamParamResult(PCLExamParamResult entity);

        #endregion

        #region PCLExamResultTemplate

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamResultTemplate> GetPCLExamResultTemplateList(long PCLExamGroupTemplateResultID);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        List<PCLExamResultTemplate> GetPCLExamResultTemplateListByTypeID(long PCLExamGroupTemplateResultID,
                                                                         int ParamEnum);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PCLExamResultTemplate GetPCLExamResultTemplate(long PCLExamResultTemplateID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPCLExamResultTemplate(PCLExamResultTemplate entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePCLExamResultTemplate(PCLExamResultTemplate entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePCLExamResultTemplate(PCLExamResultTemplate entity);

        /*▼====: #002*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetResourcesForMedicalServicesListByTypeID(long PCLResultParamImpID);
        /*▲====: #002*/
        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetResourcesForLaboratory();
        //▲====: #003
        #endregion



        #region UltraResParams_EchoCardiography

        [OperationContract]
        [FaultContract(typeof(AxException))]
        UltraResParams_EchoCardiography GetUltraResParams_EchoCardiography(
            long UltraResParams_EchoCardiographyID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        string GetUltraResParams_EchoCardiographyResult(
            long UltraResParams_EchoCardiographyID, long PCLImgResultID);

        //==== 20161013 CMN Begin: Add PCL Image Method
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity);
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete);
        //==== 20161013 CMN End.

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity);

        #endregion
        
        //==== 20161214 CMN Begin: Add General information
        #region UltraResParams_FetalEchocardiography
        [OperationContract]
        [FaultContract(typeof(AxException))]
        UltraResParams_FetalEchocardiography GetUltraResParams_FetalEchocardiography(long PatientPCLReqID);
        #endregion
        //==== 20161214 CMN End.

        #region UltraResParams_FetalEchocardiography2D

        [OperationContract]
        [FaultContract(typeof (AxException))]
        UltraResParams_FetalEchocardiography2D GetUltraResParams_FetalEchocardiography2D(
            long UltraResParams_FetalEchocardiography2DID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity);

        #endregion

        #region UltraResParams_FetalEchocardiography Doppler
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<UltraResParams_FetalEchocardiographyDoppler>
            GetUltraResParams_FetalEchocardiographyDoppler(long UltraResParams_FetalEchocardiographyDopplerID);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        UltraResParams_FetalEchocardiographyDoppler GetUltraResParams_FetalEchocardiographyDopplerByID(
            long UltraResParams_FetalEchocardiographyDopplerID, long PCLImgResultID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity);

        #endregion

        #region UltraResParams_FetalEchocardiographyPostpartum

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<UltraResParams_FetalEchocardiographyPostpartum>
            GetUltraResParams_FetalEchocardiographyPostpartum(long UltraResParams_FetalEchocardiographyPostpartumID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        UltraResParams_FetalEchocardiographyPostpartum GetUltraResParams_FetalEchocardiographyPostpartumByID(
            long UltraResParams_FetalEchocardiographyPostpartumID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity);

        #endregion

        #region UltraResParams_FetalEchocardiographyResult

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<UltraResParams_FetalEchocardiographyResult> GetUltraResParams_FetalEchocardiographyResult(
            long UltraResParams_FetalEchocardiographyResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        UltraResParams_FetalEchocardiographyResult GetUltraResParams_FetalEchocardiographyResultByID(
            long UltraResParams_FetalEchocardiographyResultID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity);

        #endregion

        #region Abdominal Ultrasound

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertAbdominalUltrasoundResult(AbdominalUltrasound entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateAbdominalUltrasoundResult(AbdominalUltrasound entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AbdominalUltrasound GetAbdominalUltrasoundResult(long PatientPCLReqID);

        #endregion
        

        #region URP_FE_Exam

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_Exam GetURP_FE_Exam(
            long URP_FE_ExamID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_Exam(URP_FE_Exam entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_Exam(URP_FE_Exam entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_Exam(URP_FE_Exam entity);

        #endregion

        #region URP_FE_Oesophagienne

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_Oesophagienne GetURP_FE_Oesophagienne(
            long URP_FE_OesophagienneID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_Oesophagienne(URP_FE_Oesophagienne entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_Oesophagienne(URP_FE_Oesophagienne entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_Oesophagienne(URP_FE_Oesophagienne entity);

        #endregion

        #region URP_FE_OesophagienneCheck

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_OesophagienneCheck GetURP_FE_OesophagienneCheck(
            long URP_FE_OesophagienneCheckID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity);

        #endregion

        #region URP_FE_OesophagienneDiagnosis

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_OesophagienneDiagnosis GetURP_FE_OesophagienneDiagnosis(
            long URP_FE_OesophagienneDiagnosisID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity);

        #endregion

        #region URP_FE_StressDipyridamole

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDipyridamole GetURP_FE_StressDipyridamole(
            long URP_FE_StressDipyridamoleID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity);

        #endregion

        #region URP_FE_StressDipyridamoleElectrocardiogram

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDipyridamoleElectrocardiogram GetURP_FE_StressDipyridamoleElectrocardiogram(
            long URP_FE_StressDipyridamoleElectrocardiogramID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity);

        #endregion

        #region URP_FE_StressDipyridamoleExam

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDipyridamoleExam GetURP_FE_StressDipyridamoleExam(
            long URP_FE_StressDipyridamoleExamID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity);

        #endregion

        #region URP_FE_StressDipyridamoleImage

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDipyridamoleImage GetURP_FE_StressDipyridamoleImage(
            long URP_FE_StressDipyridamoleImageID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity);

        #endregion

        #region URP_FE_StressDipyridamoleResult

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDipyridamoleResult GetURP_FE_StressDipyridamoleResult(
            long URP_FE_StressDipyridamoleResultID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity);

        #endregion

        #region URP_FE_StressDobutamine

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDobutamine GetURP_FE_StressDobutamine(
            long URP_FE_StressDobutamineID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDobutamine(URP_FE_StressDobutamine entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDobutamine(URP_FE_StressDobutamine entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDobutamine(URP_FE_StressDobutamine entity);

        #endregion

        #region URP_FE_StressDobutamineElectrocardiogram

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDobutamineElectrocardiogram GetURP_FE_StressDobutamineElectrocardiogram(
            long URP_FE_StressDobutamineElectrocardiogramID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity);

        #endregion

        #region URP_FE_StressDobutamineExam

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDobutamineExam GetURP_FE_StressDobutamineExam(
            long URP_FE_StressDobutamineExamID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity);

        #endregion

        #region URP_FE_StressDobutamineImages

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDobutamineImages GetURP_FE_StressDobutamineImages(
            long URP_FE_StressDobutamineImagesID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity);

        #endregion

        #region URP_FE_StressDobutamineResult

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_StressDobutamineResult GetURP_FE_StressDobutamineResult(
            long URP_FE_StressDobutamineResultID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity);

        #endregion

        #region URP_FE_VasculaireAnother

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_VasculaireAnother GetURP_FE_VasculaireAnother(
            long URP_FE_VasculaireAnotherID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity);

        #endregion

        #region URP_FE_VasculaireAorta

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_VasculaireAorta GetURP_FE_VasculaireAorta(
            long URP_FE_VasculaireAortaID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity);

        #endregion

        #region URP_FE_VasculaireCarotid

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_VasculaireCarotid GetURP_FE_VasculaireCarotid(
            long URP_FE_VasculaireCarotidID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity);

        #endregion

        #region URP_FE_VasculaireExam

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_VasculaireExam GetURP_FE_VasculaireExam(
            long URP_FE_VasculaireExamID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_VasculaireExam(URP_FE_VasculaireExam entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_VasculaireExam(URP_FE_VasculaireExam entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_VasculaireExam(URP_FE_VasculaireExam entity);

        #endregion

        #region URP_FE_VasculaireLeg

        [OperationContract]
        [FaultContract(typeof(AxException))]
        URP_FE_VasculaireLeg GetURP_FE_VasculaireLeg(
            long URP_FE_VasculaireLegID, long PCLImgResultID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity);

        #endregion

        //==== 20161129 CMN Bein: Add button save for all FE pages
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddAndUpdateFE_Vasculaire(URP_FE_Vasculaire entity);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddAndUpdateUltraResParams_FetalEchocardiography(UltraResParams_FetalEchocardiography entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddAndUpdateURP_FE_Oesophagienne(URP_FE_OesophagienneUltra entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddAndUpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamoleUltra entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddAndUpdateURP_FE_StressDobutamine(URP_FE_StressDobutamineUltra entity);
        //==== 20161129 CMN End: Add button save for all FE pages
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequestDetail> GetPCLExamTypeServiceApmtTarget(long PCLExamTypeID, DateTime FromDate, DateTime ToDate);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> PatientPCLRequest_SearchPaging_ForMedicalExamination(PatientPCLRequestSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        //▼====: #004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateReceptionTime(long PatientPCLReqID, DateTime ReceptionTime, long V_PCLRequestType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateDateStarted2(long PtRegDetailID, DateTime DateStarted2);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdatePrescriptionsDate(long PtRegistrationID, DateTime PrescriptionsDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequestDetail> GetHistoryPCLByPCLExamType(long PatientID, long PCLExamTypeID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);
        //▲====: #004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLImagingResultDetail> PCLImagingResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType);

        //▼====: #005
        #region Digital Signature Transaction
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> GetPatientPCLLaboratoryResultForSendTransaction(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> GetListHistoryTransaction_Paging(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePCLRequestStatusGeneratingResult(string ListPCLResults, long V_PCLRequestStatus);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePCLRequestFromList(PatientPCLRequest request, int PatientFindBy);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteTransactionHistory(PatientPCLRequest request, int PatientFindBy);
        #endregion
        //▲====: #005

        //▼====: #006 
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetResourcesForMedicalServicesListByDeptIDAndTypeID(long DeptID, long PtRegDetailID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetResourcesForMedicalServicesListBySmallProcedureID(long SmallProcedureID);
        //▲====: #006

        //▼====: #006
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientPCLRequest> GetListAppointmentsLab_Paging(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);
        //▲====: #006
    }
}