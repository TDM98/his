/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
 * 20180613 #002 TBLD: Tao contract cho GetResourcesForMedicalServicesListByTypeID
 * 20210527 #003 TNHX: 317 Lấy danh sách mã máy cho màn hình nhập kết quả xét nghiệm
 * 20210705 #004 BLQ: 330 + 381
 * 20230401 #005 QTD: Lấy mã máy theo khoa và nhóm DVKT
 * 20230626 #006 QTD: Lấy danh sách hẹn SMS XN
*/
using System;
using System.Collections.Generic;

using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;

using eHCMS.Configurations;

using ErrorLibrary;
using System.Runtime.Serialization;

using AxLogging;

using ErrorLibrary.Resources;
using System.Data.SqlClient;
using eHCMSLanguage;


namespace ConsultationsService.ParaClinical
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class PCLsService : eHCMS.WCFServiceCustomHeader, IPCLs
    {
        public PCLsService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        #region 0. examType Member
        //public IList<PCLExamType> GetPCLExamTypes_ByPCLFormID(long PCLFormID,int? ClassPatient)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.GetPCLExamTypes_ByPCLFormID(PCLFormID,ClassPatient);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetPCLExamTypes_ByPCLFormID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPCLExamTypes_ByPCLFormID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        //public IList<PCLExamType> GetPCLExamTypes_ByPCLFormID_Edit(long PCLFormID, long PatientPCLReqID, int? ClassPatient)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.GetPCLExamTypes_ByPCLFormID_Edit(PCLFormID,PatientPCLReqID,ClassPatient);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetPCLExamTypes_ByPCLFormID_Edit. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPCLExamTypes_ByPCLFormID_Edit);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        #endregion

        #region 1. PCLForm Member

        //public IList<PCLForm> GetForms_ByDeptID(long? deptID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.GetForms_ByDeptID(deptID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetForms_ByDeptID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetForms_ByDeptID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        //public IList<PCLForm> PCLForms_ByDeptIDV_PCLCategory(Int64 DeptID, Int64 V_PCLCategory)
        //{
        //    try
        //    {
        //        return PCLsProvider.Instance.PCLForms_ByDeptIDV_PCLCategory(DeptID, V_PCLCategory);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of PCLForms_ByDeptIDV_PCLCategory. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PCLForms_ByDeptIDV_PCLCategory);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public IList<PCLGroup> GetAllPCLGroups(long? pclCategoryID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAllPCLGroups(pclCategoryID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetAllPCLGroups(pclCategoryID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAllPCLGroups(pclCategoryID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllPCLGroups. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetAllPCLGroups);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PCLExamType> GetAllActiveExamTypesPaging(PCLExamTypeSearchCriteria searchCriteria, int pageIndex, int pageSize, bool bCountTotal, string orderBy, out int totalCount)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAllActiveExamTypes(searchCriteria, pageIndex, pageSize, bCountTotal, orderBy, out totalCount);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetAllActiveExamTypes(searchCriteria, pageIndex, pageSize, bCountTotal, orderBy, out totalCount);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAllActiveExamTypes(searchCriteria, pageIndex, pageSize, bCountTotal, orderBy, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllActiveExamTypes. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetAllActiveExamTypes);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PCLExamType> GetAllActiveExamTypes(PCLExamTypeSearchCriteria searchCriteria)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAllActiveExamTypes(searchCriteria);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetAllActiveExamTypes(searchCriteria);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAllActiveExamTypes(searchCriteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllActiveExamTypes. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetAllActiveExamTypes);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 2. PatientPCLRequest

        public bool AddFullPtPCLReq(PatientPCLRequest entity, long PatientID, long? ptRegDetailID, out long newPatientPCLReqID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddFullPtPCLReq(entity, PatientID, ptRegDetailID, out newPatientPCLReqID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddFullPtPCLReq(entity, PatientID, ptRegDetailID, out newPatientPCLReqID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddFullPtPCLReq(entity, PatientID, ptRegDetailID, out newPatientPCLReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddFullPtPCLReq. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_AddFullPtPCLReq);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public bool AddFullPtPCLReqServiceRecID(PatientPCLRequest entity, out long newPatientPCLReqID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.AddFullPtPCLReqServiceRecID(entity, out newPatientPCLReqID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of AddFullPtPCLReqServiceRecID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_AddFullPtPCLReqServiceRecID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        //public bool UpdateFullPtPCLReqServiceRecID(PatientPCLRequest entity)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.UpdateFullPtPCLReqServiceRecID(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of UpdateFullPtPCLReqServiceRecID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_UpdateFullPtPCLReqServiceRecID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public PatientMedicalRecord GetPMRByPtPCLFormRequest(long LoggedDoctorID, long PatientID, long? ptRegDetailID, long? PatientRecID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPMRByPtPCLFormRequest(LoggedDoctorID, PatientID, ptRegDetailID, PatientRecID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPMRByPtPCLFormRequest(LoggedDoctorID, PatientID, ptRegDetailID, PatientRecID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPMRByPtPCLFormRequest(LoggedDoctorID, PatientID, ptRegDetailID, PatientRecID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPMRByPtPCLFormRequest. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPMRByPtPCLFormRequest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientPCLRequest> PatientPCLRequest_ByPatientID_Paging(
            PatientPCLRequestSearchCriteria SearchCriteria, long V_RegistrationType,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total)
        {
            try
            {
                List<PatientPCLRequest> ListResult = new List<PatientPCLRequest>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_ByPatientID_Paging(SearchCriteria, V_RegistrationType, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    ListResult = PCLsProvider.Instance.PatientPCLRequest_ByPatientID_Paging(SearchCriteria, V_RegistrationType, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_ByPatientID_Paging(SearchCriteria, V_RegistrationType, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                return ListResult;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLRequest_ByPatientID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_ByPatientID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientPCLRequest> GetPatientPCLRequestList_ByRegistrationID(long RegistrationID, long V_RegistrationType, long V_PCLMainCategory)
        {
            try
            {
                List<PatientPCLRequest> ListResult = new List<PatientPCLRequest>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPatientPCLRequestList_ByRegistrationID(RegistrationID, V_RegistrationType, V_PCLMainCategory);
                //}
                //else
                //{
                //    ListResult = PCLsProvider.Instance.GetPatientPCLRequestList_ByRegistrationID(RegistrationID, V_RegistrationType, V_PCLMainCategory);
                //}
                ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPatientPCLRequestList_ByRegistrationID(RegistrationID, V_RegistrationType, V_PCLMainCategory);
                return ListResult;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPatientPCLRequestList_ByRegistrationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLRequestList_ByRegistrationID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        /// <summary>
        /// Author: VuTTM
        /// Description: Getting the deleted PCL request list.
        /// </summary>
        /// <param name="registrationID"></param>
        /// <returns></returns>
        public List<PatientPCLRequest> GetPatientDeletedPCLRequestListByRegistrationID(long registrationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetDeletedPCLRequestListByRegistrationID(registrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPatientDeletedPCLRequestListByRegistrationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLRequestList_ByRegistrationID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Lấy PCLExamType in PatientPCLRequestDetails
        public IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPatientPCLReqID(PatientPCLRequestDetailSearchCriteria SearchCriteria, long V_RegistrationType)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequestDetail_ByPatientPCLReqID(SearchCriteria, V_RegistrationType);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PatientPCLRequestDetail_ByPatientPCLReqID(SearchCriteria, V_RegistrationType);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequestDetail_ByPatientPCLReqID(SearchCriteria, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLRequestDetail_ByPatientPCLReqID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequestDetail_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPtRegistrationID(long PtRegistrationID, long V_RegistrationType, long V_PCLMainCategory)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequestDetail_ByPtRegistrationID(PtRegistrationID, V_RegistrationType, V_PCLMainCategory);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PatientPCLRequestDetail_ByPtRegistrationID(PtRegistrationID, V_RegistrationType, V_PCLMainCategory);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequestDetail_ByPtRegistrationID(PtRegistrationID, V_RegistrationType, V_PCLMainCategory);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLRequestDetail_ByPtRegistrationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequestDetail_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        //Phiếu cuối cùng
        public PatientPCLRequest PatientPCLRequest_RequestLastest(Int64 PatientID, long V_RegistrationType, long? V_PCLMainCategory)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_RequestLastest(PatientID, V_RegistrationType, V_PCLMainCategory);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PatientPCLRequest_RequestLastest(PatientID, V_RegistrationType, V_PCLMainCategory);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_RequestLastest(PatientID, V_RegistrationType, V_PCLMainCategory);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLRequest_RequestLastest. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_RequestLastest);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //Phiếu cuối cùng


        //Ds phiếu yêu cầu CLS
        public IList<PatientPCLRequest> PatientPCLRequest_SearchPaging(
        PatientPCLRequestSearchCriteria SearchCriteria,

         int PageIndex,
         int PageSize,
         string OrderBy,
         bool CountTotal,
         out int Total)
        {
            try
            {
                IList<PatientPCLRequest> ListResult;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    ListResult =  aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_SearchPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    ListResult = PCLsProvider.Instance.PatientPCLRequest_SearchPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_SearchPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                return ListResult;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLRequest_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //Ds phiếu yêu cầu CLS

        public PatientPCLRequest GetPatientPCLRequestResultsByReqID(PatientPCLRequestSearchCriteria SearchCriteria)
        {
            try
            {
                PatientPCLRequest mPCLRequest = new PatientPCLRequest();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    mPCLRequest = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPatientPCLRequestResultsByReqID(SearchCriteria);
                //}
                //else
                //{
                //    mPCLRequest = PCLsProvider.Instance.GetPatientPCLRequestResultsByReqID(SearchCriteria);
                //}
                mPCLRequest = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPatientPCLRequestResultsByReqID(SearchCriteria);
                return mPCLRequest;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPatientPCLRequestResultsByReqID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_SearchPaging);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void CheckTemplatePCLResultByReqID(long PatientPCLReqID, bool InPt, out bool IsNewTemplate, out long V_ReportForm, out long PCLImgResultID, out long V_PCLRequestType, out string TemplateResultString)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.CheckTemplatePCLResultByReqID(PatientPCLReqID, InPt,out IsNewTemplate, out V_ReportForm, out PCLImgResultID, out V_PCLRequestType, out TemplateResultString);
            }
            catch (SqlException ex)
            {
                //if (ex.Class == 16)//message tu database show len
                //{
                //    return false;
                //}
                AxLogger.Instance.LogInfo("End of CheckTemplatePCLResultByReqID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_SearchPaging);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void GetPCLResult_Criterion(long ServiceRecID, out long V_ResultType, out string TemplateResultString)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLResult_Criterion(ServiceRecID, out V_ResultType, out TemplateResultString);
            }
            catch (SqlException ex)
            {
                //if (ex.Class == 16)//message tu database show len
                //{
                //    return false;
                //}
                AxLogger.Instance.LogInfo("End of GetPCLResult_Criterion. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_SearchPaging);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPaging(
       PatientPCLRequestSearchCriteria SearchCriteria,

        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        out int Total)
        {
            try
            {
                IList<PatientPCLRequest> ListResult;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_ViewResult_SearchPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    ListResult = PCLsProvider.Instance.PatientPCLRequest_ViewResult_SearchPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_ViewResult_SearchPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                return ListResult;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLRequest_ViewResult_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPagingNew(
                                          PatientPCLRequestSearchCriteria SearchCriteria,
                                           int PageIndex,
                                           int PageSize,
                                           string OrderBy,
                                           bool CountTotal,
                                           out int Total)
        {
            try
            {
                IList<PatientPCLRequest> ListResult;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_ViewResult_SearchPagingNew(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    ListResult = PCLsProvider.Instance.PatientPCLRequest_ViewResult_SearchPagingNew(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_ViewResult_SearchPagingNew(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                return ListResult;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLRequest_ViewResult_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Danh sách phiếu các lần trước
        public IList<PatientPCLRequest> PatientPCLRequest_ByPatientIDV_Param_Paging(
        PatientPCLRequestSearchCriteria SearchCriteria,

         int PageIndex,
         int PageSize,
         string OrderBy,
         bool CountTotal,
         out int Total)
        {
            try
            {
                IList<PatientPCLRequest> ListResult;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_ByPatientIDV_Param_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    ListResult = PCLsProvider.Instance.PatientPCLRequest_ByPatientIDV_Param_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_ByPatientIDV_Param_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                return ListResult;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLRequest_ByPatientIDV_Param_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_ByPatientIDV_Param_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //Danh sách phiếu các lần trước

        #endregion

        #region 3.PCL results
        //public IList<PatientPCLRequest> GetPCLResult_PtPCLRequest(long? registrationID, bool isImported, long V_PCLCategory)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.GetPCLResult_PtPCLRequest(registrationID, isImported, V_PCLCategory);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetPCLResult_PtPCLRequest. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPCLResult_PtPCLRequest);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public IList<PCLExamGroup> GetPCLResult_PCLExamGroup(long? ptID, long? ptPCLReqID, bool isImported)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLResult_PCLExamGroup(ptID, ptPCLReqID, isImported);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLResult_PCLExamGroup(ptID, ptPCLReqID, isImported);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLResult_PCLExamGroup(ptID, ptPCLReqID, isImported);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLResult_PCLExamGroup. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPCLResult_PCLExamGroup);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public IList<PCLExamType> GetPCLResult_PCLExamType(long? ptID, long? PCLExamGroupID, bool isImported)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.GetPCLResult_PCLExamType(ptID, PCLExamGroupID, isImported);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetPCLResult_PCLExamType. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPCLResult_PCLExamType);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        /*▼====: #001*/
        public IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID)
        {
            return GetPCLResultFileStoreDetails_V2(ptID, ptPCLReqID, PCLGroupID, PCLExamTypeID, (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU);
        }
        public IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails_V2(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID, long V_PCLRequestType)
        /*▲====: #001*/
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLResultFileStoreDetails(ptID, ptPCLReqID, PCLGroupID, PCLExamTypeID, V_PCLRequestType);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLResultFileStoreDetails(ptID, ptPCLReqID, PCLGroupID, PCLExamTypeID, V_PCLRequestType);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLResultFileStoreDetails(ptID, ptPCLReqID, PCLGroupID, PCLExamTypeID, V_PCLRequestType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLResultFileStoreDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPCLResultFileStoreDetails);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsExt(PCLResultFileStorageDetailSearchCriteria p)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLResultFileStoreDetailsExt(p);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLResultFileStoreDetailsExt(p);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLResultFileStoreDetailsExt(p);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLResultFileStoreDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPCLResultFileStoreDetails);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsByPCLImgResultID(long PCLImgResultID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLResultFileStoreDetailsByPCLImgResultID(PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLResultFileStoreDetailsByPCLImgResultID(PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLResultFileStoreDetailsByPCLImgResultID(PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLResultFileStoreDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPCLResultFileStoreDetails);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 4. PCL Laboratory
        public IList<MedicalSpecimensCategory> GetAllMedSpecCatg()
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAllMedSpecCatg();
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetAllMedSpecCatg();
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAllMedSpecCatg();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllMedSpecCatg. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetAllMedSpecCatg);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<MedicalSpecimen> GetMedSpecsByCatgID(short? medSpecCatgID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetMedSpecsByCatgID(medSpecCatgID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetMedSpecsByCatgID(medSpecCatgID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetMedSpecsByCatgID(medSpecCatgID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedSpecsByCatgID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetMedSpecsByCatgID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //public IList<PCLExamType> GetPtPCLLabExamTypes_RefByPtPCLReqID(long? PtPCLReqID, long? PCLExamGroupID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.GetPtPCLLabExamTypes_RefByPtPCLReqID(PtPCLReqID, PCLExamGroupID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetPtPCLLabExamTypes_RefByPtPCLReqID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPtPCLLabExamTypes_RefByPtPCLReqID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        public IList<PatientPCLLaboratoryResultDetail> GetPtPCLLabExamTypes_ByPtPCLReqID(long PatientID, long? PtPCLReqID, long? PCLExamGroupID, long? PCLExamTypeID, bool isImported)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPtPCLLabExamTypes_ByPtPCLReqID(PatientID, PtPCLReqID, PCLExamGroupID, PCLExamTypeID, isImported);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPtPCLLabExamTypes_ByPtPCLReqID(PatientID, PtPCLReqID, PCLExamGroupID, PCLExamTypeID, isImported);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPtPCLLabExamTypes_ByPtPCLReqID(PatientID, PtPCLReqID, PCLExamGroupID, PCLExamTypeID, isImported);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPtPCLLabExamTypes_ByPtPCLReqID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPtPCLLabExamTypes_ByPtPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<MedicalSpecimenInfo> GetMedSpecInfo_ByPtPCLReqID(long? PtPCLReqID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetMedSpecInfo_ByPtPCLReqID(PtPCLReqID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetMedSpecInfo_ByPtPCLReqID(PtPCLReqID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetMedSpecInfo_ByPtPCLReqID(PtPCLReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedSpecInfo_ByPtPCLReqID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetMedSpecInfo_ByPtPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public string PCLExamTestItems_ByPatientID(long PatientID, PatientPCLRequestSearchCriteria SearchCriteria)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTestItems_ByPatientID(PatientID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLExamTestItems_ByPatientID(PatientID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTestItems_ByPatientID(PatientID, SearchCriteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTestItems_ByPatientID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<PCLExamTestItems> PatientPCLLaboratoryResults_ByExamTest_Paging(long PatientID, long PCLExamTestItemID, long PCLExamTypeID, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, long PatientFindBy, out int TotalRow, out int MaxRow)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLLaboratoryResults_ByExamTest_Paging(PatientID, PCLExamTestItemID, PCLExamTypeID, FromDate, ToDate, PageIndex, PageSize, out TotalRow, out MaxRow);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PatientPCLLaboratoryResults_ByExamTest_Paging(PatientID, PCLExamTestItemID, PCLExamTypeID, FromDate, ToDate, PageIndex, PageSize, out TotalRow, out MaxRow);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLLaboratoryResults_ByExamTest_Paging(PatientID, PCLExamTestItemID, PCLExamTypeID, FromDate, ToDate, PageIndex, PageSize, PatientFindBy, out TotalRow, out MaxRow);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLLaboratoryResults_ByExamTest_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public string PatientPCLLaboratoryResults_ByExamTest_Crosstab(long PatientID, string strXml, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalCol)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLLaboratoryResults_ByExamTest_Crosstab(PatientID, strXml, FromDate, ToDate, PageIndex, PageSize, out TotalCol);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PatientPCLLaboratoryResults_ByExamTest_Crosstab(PatientID, strXml, FromDate, ToDate, PageIndex, PageSize, out TotalCol);
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLLaboratoryResults_ByExamTest_Crosstab(PatientID, strXml, FromDate, ToDate, PageIndex, PageSize, out TotalCol);
                //}
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLLaboratoryResults_ByExamTest_Crosstab. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PCLExamTestItems> PCLExamTestItems_SearchPaging(GeneralSearchCriteria SearchCriteria, int PageIndex, int PageSize, bool bCount, out int Total)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTestItems_SearchPaging(SearchCriteria, PageIndex, PageSize, bCount, out Total);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLExamTestItems_SearchPaging(SearchCriteria, PageIndex, PageSize, bCount, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTestItems_SearchPaging(SearchCriteria, PageIndex, PageSize, bCount, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTestItems_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PCLExamTestItems_Save(PCLExamTestItems Item, out long PCLExamTestItemID)
        {
            PCLExamTestItemID = 0;
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTestItems_Save(Item, out PCLExamTestItemID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLExamTestItems_Save(Item, out PCLExamTestItemID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTestItems_Save(Item, out PCLExamTestItemID);
            }
            catch (SqlException ex)
            {
                if (ex.Class == 16)//message tu database show len
                {
                    return false;
                }
                else
                {
                    AxLogger.Instance.LogInfo("End of PCLExamTestItems_Save. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }

        public bool RefDepartmentReqCashAdv_Save(RefDepartmentReqCashAdv Target, out long ID)
        {
            ID = 0;
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.RefDepartmentReqCashAdv_Save(Target, out ID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.RefDepartmentReqCashAdv_Save(Target, out ID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.RefDepartmentReqCashAdv_Save(Target, out ID);
            }
            catch (SqlException ex)
            {
                if (ex.Class == 16)//message tu database show len
                {
                    return false;
                }
                else
                {
                    AxLogger.Instance.LogInfo("End of RefDepartmentReqCashAdv_Save. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }

        public bool RefDepartmentReqCashAdv_Delete(long RefDepartmentReqCashAdvID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.RefDepartmentReqCashAdv_Delete(RefDepartmentReqCashAdvID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.RefDepartmentReqCashAdv_Delete(RefDepartmentReqCashAdvID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.RefDepartmentReqCashAdv_Delete(RefDepartmentReqCashAdvID);
            }
            catch (SqlException ex)
            {

                AxLogger.Instance.LogInfo("End of RefDepartmentReqCashAdv_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, out int Total)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.RefDepartmentReqCashAdv_GetAll(SearchText, OrderBy, PageIndex, PageSize, bCount, out Total);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.RefDepartmentReqCashAdv_GetAll(SearchText, OrderBy, PageIndex, PageSize, bCount, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.RefDepartmentReqCashAdv_GetAll(SearchText, OrderBy, PageIndex, PageSize, bCount, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDepartmentReqCashAdv_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PCLExamTypeServiceTarget_Save(PCLExamTypeServiceTarget Target, out long ID)
        {
            ID = 0;
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_Save(Target, out ID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLExamTypeServiceTarget_Save(Target, out ID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_Save(Target, out ID);
            }
            catch (SqlException ex)
            {
                if (ex.Class == 16)//message tu database show len
                {
                    return false;
                }
                else
                {
                    AxLogger.Instance.LogInfo("End of PCLExamTypeServiceTarget_Save. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }

        public bool PCLExamTypeServiceTarget_Delete(long PCLExamTypeServiceTargetID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_Delete(PCLExamTypeServiceTargetID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLExamTypeServiceTarget_Delete(PCLExamTypeServiceTargetID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_Delete(PCLExamTypeServiceTargetID);
            }
            catch (SqlException ex)
            {

                AxLogger.Instance.LogInfo("End of PCLExamTypeServiceTarget_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamTypeServiceTarget> PCLExamTypeServiceTarget_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, out int Total)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_GetAll(SearchText, OrderBy, PageIndex, PageSize, bCount, out Total);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLExamTypeServiceTarget_GetAll(SearchText, OrderBy, PageIndex, PageSize, bCount, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_GetAll(SearchText, OrderBy, PageIndex, PageSize, bCount, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeServiceTarget_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PCLExamTypeServiceTarget_Checked(long PCLExamTypeID, DateTime Date)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_Checked(PCLExamTypeID, Date);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLExamTypeServiceTarget_Checked(PCLExamTypeID, Date);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_Checked(PCLExamTypeID, Date);
            }
            catch (SqlException ex)
            {
                if (ex.Class == 16)//message tu database show len
                {
                    return false;
                }
                else
                {
                    AxLogger.Instance.LogInfo("End of PCLExamTypeServiceTarget_Checked. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }

        public bool PCLExamTypeServiceTarget_Checked_Appointment(long PCLExamTypeID, DateTime Date)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_Checked_Appointment(PCLExamTypeID, Date);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLExamTypeServiceTarget_Checked_Appointment(PCLExamTypeID, Date);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLExamTypeServiceTarget_Checked_Appointment(PCLExamTypeID, Date);
            }
            catch (SqlException ex)
            {
                if (ex.Class == 16)//message tu database show len
                {
                    return false;
                }
                else
                {
                    AxLogger.Instance.LogInfo("End of PCLExamTypeServiceTarget_Checked. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }

        public IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID, V_PCLRequestType);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID, V_PCLRequestType);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID, V_PCLRequestType);
            }
            catch (SqlException ex)
            {
                if (ex.Class == 16)//message tu database show len
                {
                    return null;
                }
                else
                {
                    AxLogger.Instance.LogInfo("End of GetPatientPCLLaboratoryResults_ByPatientPCLReqID. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }

        public string GetPCLExamTypeName(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamTypeName(PatientID, PatientPCLReqID, V_PCLRequestType);
            }
            catch (SqlException ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamTypeName. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "GetPCLExamTypeName");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_No_ResultOld(long PatientPCLReqID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLLaboratoryResults_No_ResultOld(PatientPCLReqID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLLaboratoryResults_No_ResultOld(PatientPCLReqID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLLaboratoryResults_No_ResultOld(PatientPCLReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPatientPCLLaboratoryResults_ByPatientPCLReqID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public bool AddFullPtPCLLabResult(PatientPCLLaboratoryResult entity, out long newLabResultID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.AddFullPtPCLLabResult(entity, out newLabResultID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of AddFullPtPCLLabResult. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_AddFullPtPCLLabResult);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public bool AddPatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddPatientPCLLaboratoryResultDetail(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddPatientPCLLaboratoryResultDetail(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddPatientPCLLaboratoryResultDetail(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddPatientPCLLaboratoryResultDetail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_AddPatientPCLLaboratoryResultDetail);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public bool UpdatePatientPCLLaboratoryResultDetail(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsProvider.Instance.UpdatePatientPCLLaboratoryResultDetail(allPatientPCLLaboratoryResultDetailentity);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of UpdatePatientPCLLaboratoryResultDetail. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_UpdatePatientPCLLaboratoryResultDetail);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        // 20181216 TNHX: [BM0005430] Add PatientID for report PCLDEPARTMENT_LABORATORY_RESULT
        private static readonly object olockLAB = new object();
        public bool UpdatePatientPCLLaboratoryResultDetailXml(
            IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity
            , PatientPCLLaboratoryResult entity, long PCLRequestTypeID, long PatientID,bool? IsWaitResult,bool? IsDone, out string errorOutput)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start UpdatePatientPCLLaboratoryResultDetailXml.", CurrentUser);
                errorOutput = "";
                lock (olockLAB)
                {
                    bool b;
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    b = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdatePatientPCLLaboratoryResultDetail(allPatientPCLLaboratoryResultDetailentity, entity, PCLRequestTypeID, PatientID, out errorOutput);
                    //}
                    //else
                    //{
                    //    b = PCLsProvider.Instance.UpdatePatientPCLLaboratoryResultDetail(allPatientPCLLaboratoryResultDetailentity, entity, PCLRequestTypeID, PatientID, out errorOutput);
                    //}
                    b = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdatePatientPCLLaboratoryResultDetail(allPatientPCLLaboratoryResultDetailentity, entity, PCLRequestTypeID, PatientID, IsWaitResult, IsDone, out errorOutput);
                    AxLogger.Instance.LogInfo("End of UpdatePatientPCLLaboratoryResultDetail. Status: Successful.", CurrentUser);
                    return b;
                }
            }
            catch (Exception ex)
            {
                errorOutput = ex.Message;
                AxLogger.Instance.LogInfo("End of UpdatePatientPCLLaboratoryResultDetail. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_UpdatePatientPCLLaboratoryResultDetail);
                return false;
            }
        }

        public bool DeletePatientPCLLaboratoryResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID, long PCLExamTypeID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start DeletePatientPCLLaboratoryResult.", CurrentUser);
                lock (olockLAB)
                {
                    bool b;
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    b = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePatientPCLLaboratoryResult(PatientPCLReqID, PCLRequestTypeID, CancelStaffID, PCLExamTypeID);
                    //}
                    //else
                    //{
                    //    b = PCLsProvider.Instance.DeletePatientPCLLaboratoryResult(PatientPCLReqID, PCLRequestTypeID, CancelStaffID, PCLExamTypeID);
                    //}
                    b = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePatientPCLLaboratoryResult(PatientPCLReqID, PCLRequestTypeID, CancelStaffID, PCLExamTypeID);
                    AxLogger.Instance.LogInfo("End of DeletePatientPCLLaboratoryResultDetailXml. Status: Successful.", CurrentUser);
                    return b;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeletePatientPCLLaboratoryResultDetailXml. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ex.Message);
                throw new FaultException<AxException>(axErr, ex.Message);
            }
        }

        public bool DeletePatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePatientPCLLaboratoryResultDetail(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeletePatientPCLLaboratoryResultDetail(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePatientPCLLaboratoryResultDetail(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeletePatientPCLLaboratoryResultDetail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_DeletePatientPCLLaboratoryResultDetail);

                throw new FaultException<AxException>(axErr, ex.Message);
            }
        }

        public bool DeletePatientPCLImagingResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start DeletePatientPCLImagingResult.", CurrentUser);
                lock (olockLAB)
                {
                    bool b;
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    b = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePatientPCLImagingResult(PatientPCLReqID, PCLRequestTypeID, CancelStaffID);
                    //}
                    //else
                    //{
                    //    b = PCLsProvider.Instance.DeletePatientPCLImagingResult(PatientPCLReqID, PCLRequestTypeID, CancelStaffID);
                    //}
                    b = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePatientPCLImagingResult(PatientPCLReqID, PCLRequestTypeID, CancelStaffID);
                    AxLogger.Instance.LogInfo("End of DeletePatientPCLImagingResult. Status: Successful.", CurrentUser);
                    return b;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeletePatientPCLImagingResult. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Delete Fail");
                return false;
            }
        }
        public IList<PatientPCLRequest> ListPatientPCLRequest_LAB_Paging(long PatientID, long? DeptLocID, long V_PCLRequestType,
        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        out int Total)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.ListPatientPCLRequest_LAB_Paging(PatientID, DeptLocID, V_PCLRequestType, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.ListPatientPCLRequest_LAB_Paging(PatientID, DeptLocID, V_PCLRequestType, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.ListPatientPCLRequest_LAB_Paging(PatientID, DeptLocID, V_PCLRequestType, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ListPatientPCLRequest_LAB_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.ListPatientPCLRequest_LAB_Paging);

                throw new FaultException<AxException>(axErr, ex.Message);
            }
        }
        #endregion

        public IList<PCLItem> GetPCLItemsByPCLFormID(long? PCLFormID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLItemsByPCLFormID(PCLFormID, pageIndex, pageSize, bCountTotal, out totalCount);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLItemsByPCLFormID(PCLFormID, pageIndex, pageSize, bCountTotal, out totalCount);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLItemsByPCLFormID(PCLFormID, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLItemsByPCLFormID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<PatientPCLRequest> LIS_Order(string SoPhieuChiDinh, bool IsAll)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.LIS_Order(SoPhieuChiDinh, IsAll);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.LIS_Order(SoPhieuChiDinh, IsAll);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.LIS_Order(SoPhieuChiDinh, IsAll);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of LIS_Order. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_LIS_Order);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool LIS_Result(PatientPCLRequest_LABCom ParamLabCom)
        {
            try
            {
                lock (olockLAB)
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.LIS_Result(ParamLabCom);
                    //}
                    //else
                    //{
                    //    return PCLsProvider.Instance.LIS_Result(ParamLabCom);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.LIS_Result(ParamLabCom);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of LIS_Result. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_LIS_Result);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #region 6. Dinh
        #region PCLExamParamResult
        public List<PCLExamParamResult> GetPCLExamParamResultList(long PCLExamResultTemplateID, long ParamEnum)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamParamResultList(PCLExamResultTemplateID, ParamEnum);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLExamParamResultList(PCLExamResultTemplateID, ParamEnum);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamParamResultList(PCLExamResultTemplateID, ParamEnum);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamParamResultList. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PCLExamParamResult GetPCLExamParamResult(long PCLExamResultTemplateID, long ParamEnum)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamParamResult(PCLExamResultTemplateID, ParamEnum);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLExamParamResult(PCLExamResultTemplateID, ParamEnum);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamParamResult(PCLExamResultTemplateID, ParamEnum);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamParamResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddPCLExamParamResult(PCLExamParamResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddPCLExamParamResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddPCLExamParamResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddPCLExamParamResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddPCLExamParamResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdatePCLExamParamResult(PCLExamParamResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdatePCLExamParamResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdatePCLExamParamResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdatePCLExamParamResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdatePCLExamParamResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeletePCLExamParamResult(PCLExamParamResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePCLExamParamResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeletePCLExamParamResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePCLExamParamResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeletePCLExamParamResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion


        #region PCLExamResultTemplate
        public List<PCLExamResultTemplate> GetPCLExamResultTemplateList(long PCLExamGroupTemplateResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamResultTemplateList(PCLExamGroupTemplateResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLExamResultTemplateList(PCLExamGroupTemplateResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamResultTemplateList(PCLExamGroupTemplateResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamResultTemplateList. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamResultTemplate> GetPCLExamResultTemplateListByTypeID(long PCLExamGroupTemplateResultID, int ParamEnum)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamResultTemplateListByTypeID(PCLExamGroupTemplateResultID, ParamEnum);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLExamResultTemplateListByTypeID(PCLExamGroupTemplateResultID, ParamEnum);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamResultTemplateListByTypeID(PCLExamGroupTemplateResultID, ParamEnum);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamResultTemplateListByTypeID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        /*▼====: #002*/
        public List<Resources> GetResourcesForMedicalServicesListByTypeID(long PCLResultParamImpID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetResourcesForMedicalServicesListByTypeID(PCLResultParamImpID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetResourcesForMedicalServicesListByTypeID(PCLResultParamImpID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetResourcesForMedicalServicesListByTypeID(PCLResultParamImpID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcesForMedicalServicesListByTypeID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        /*▲====: #002*/

        public PCLExamResultTemplate GetPCLExamResultTemplate(long PCLExamResultTemplateID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamResultTemplate(PCLExamResultTemplateID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetPCLExamResultTemplate(PCLExamResultTemplateID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamResultTemplate(PCLExamResultTemplateID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamResultTemplate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddPCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddPCLExamResultTemplate(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddPCLExamResultTemplate(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddPCLExamResultTemplate(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddPCLExamResultTemplate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdatePCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdatePCLExamResultTemplate(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdatePCLExamResultTemplate(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdatePCLExamResultTemplate(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdatePCLExamResultTemplate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeletePCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePCLExamResultTemplate(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeletePCLExamResultTemplate(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePCLExamResultTemplate(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeletePCLExamResultTemplate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #003
        public List<Resources> GetResourcesForLaboratory()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetResourcesForLaboratory();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcesForLaboratory. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot GetResourcesForLaboratory");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #003
        #endregion


        #region UltraResParams_EchoCardiography

        public UltraResParams_EchoCardiography GetUltraResParams_EchoCardiography(
                                                                long UltraResParams_EchoCardiographyID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_EchoCardiography(UltraResParams_EchoCardiographyID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_EchoCardiography(UltraResParams_EchoCardiographyID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_EchoCardiography(UltraResParams_EchoCardiographyID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_EchoCardiography. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public string GetUltraResParams_EchoCardiographyResult(long UltraResParams_EchoCardiographyID,
                                                                        long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_EchoCardiographyResult(UltraResParams_EchoCardiographyID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_EchoCardiographyResult(UltraResParams_EchoCardiographyID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_EchoCardiographyResult(UltraResParams_EchoCardiographyID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_EchoCardiographyResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //==== 20161013 CMN Begin: Add PCL Image Method
        //public bool AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        //{
        //    try
        //    {
        //        return PCLsProvider.Instance.AddUltraResParams_EchoCardiography(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of AddUltraResParams_EchoCardiography. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        public bool AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete)
        {
            bool PCLResult = false;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_EchoCardiography(entity);
                //}
                //else
                //{
                //    PCLResult = PCLsProvider.Instance.AddUltraResParams_EchoCardiography(entity);
                //}
                PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_EchoCardiography(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddUltraResParams_EchoCardiography. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            if (!PCLResult)
                return false;
            if (ImagingResult != null)
            {
                try
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    //else
                    //{
                    //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of AddUltraResParams_EchoCardiography. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1846_G1_LoiLuuFileHA));
                }
            }

            return true;
        }
        //public bool UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        //{
        //    try
        //    {
        //        return PCLsProvider.Instance.UpdateUltraResParams_EchoCardiography(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of UpdateUltraResParams_EchoCardiography. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        public bool UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete)
        {
            bool PCLResult = false;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_EchoCardiography(entity);
                //}
                //else
                //{
                //    PCLResult = PCLsProvider.Instance.UpdateUltraResParams_EchoCardiography(entity);
                //}
                PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_EchoCardiography(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateUltraResParams_EchoCardiography. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            if (!PCLResult)
                return false;
            if (ImagingResult != null)
            {
                try
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    //else
                    //{
                    //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of AddUltraResParams_EchoCardiography. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
            }

            return true;
        }
        //==== 20161013 CMN End.
        public bool DeleteUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_EchoCardiography(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteUltraResParams_EchoCardiography(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_EchoCardiography(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteUltraResParams_EchoCardiography. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        //==== 20161214 CMN Begin: Add General information
        #region UltraResParams_FetalEchocardiography
        public UltraResParams_FetalEchocardiography GetUltraResParams_FetalEchocardiography(long PatientPCLReqID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiography(PatientPCLReqID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_FetalEchocardiography(PatientPCLReqID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiography(PatientPCLReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_FetalEchocardiography. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "UltraResParams_FetalEchocardiography");
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        //==== 20161214 CMN End.
        #region UltraResParams_FetalEchocardiography2D

        public UltraResParams_FetalEchocardiography2D GetUltraResParams_FetalEchocardiography2D(
                                                                long UltraResParams_FetalEchocardiography2DID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2DID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2DID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2DID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_FetalEchocardiography2D. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_FetalEchocardiography2D(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddUltraResParams_FetalEchocardiography2D(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_FetalEchocardiography2D(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddUltraResParams_FetalEchocardiography2D. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiography2D(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiography2D(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiography2D(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateUltraResParams_FetalEchocardiography2D. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiography2D(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiography2D(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiography2D(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteUltraResParams_FetalEchocardiography2D. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region UltraResParams_FetalEchocardiography Doppler

        public IList<UltraResParams_FetalEchocardiographyDoppler>
            GetUltraResParams_FetalEchocardiographyDoppler(long UltraResParams_FetalEchocardiographyDopplerID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDopplerID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDopplerID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDopplerID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_FetalEchocardiographyDoppler. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public UltraResParams_FetalEchocardiographyDoppler GetUltraResParams_FetalEchocardiographyDopplerByID(
            long UltraResParams_FetalEchocardiographyDopplerID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyDopplerByID(UltraResParams_FetalEchocardiographyDopplerID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyDopplerByID(UltraResParams_FetalEchocardiographyDopplerID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyDopplerByID(UltraResParams_FetalEchocardiographyDopplerID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_FetalEchocardiographyDopplerByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_FetalEchocardiographyDoppler(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddUltraResParams_FetalEchocardiographyDoppler(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_FetalEchocardiographyDoppler(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddUltraResParams_FetalEchocardiographyDoppler. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiographyDoppler(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiographyDoppler(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiographyDoppler(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateUltraResParams_FetalEchocardiographyDoppler. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiographyDoppler(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiographyDoppler(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiographyDoppler(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteUltraResParams_FetalEchocardiographyDoppler. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region UltraResParams_FetalEchocardiographyPostpartum

        public IList<UltraResParams_FetalEchocardiographyPostpartum>
            GetUltraResParams_FetalEchocardiographyPostpartum(long UltraResParams_FetalEchocardiographyPostpartumID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartumID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartumID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartumID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_FetalEchocardiographyPostpartum. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public UltraResParams_FetalEchocardiographyPostpartum GetUltraResParams_FetalEchocardiographyPostpartumByID(
            long UltraResParams_FetalEchocardiographyPostpartumID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyPostpartumByID(UltraResParams_FetalEchocardiographyPostpartumID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyPostpartumByID(UltraResParams_FetalEchocardiographyPostpartumID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyPostpartumByID(UltraResParams_FetalEchocardiographyPostpartumID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_FetalEchocardiographyPostpartumByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_FetalEchocardiographyPostpartum(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddUltraResParams_FetalEchocardiographyPostpartum(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_FetalEchocardiographyPostpartum(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddUltraResParams_FetalEchocardiographyPostpartum. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiographyPostpartum(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiographyPostpartum(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiographyPostpartum(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateUltraResParams_FetalEchocardiographyPostpartum. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiographyPostpartum(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiographyPostpartum(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiographyPostpartum(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteUltraResParams_FetalEchocardiographyPostpartum. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region UltraResParams_FetalEchocardiographyResult

        public IList<UltraResParams_FetalEchocardiographyResult> GetUltraResParams_FetalEchocardiographyResult(
                                                            long UltraResParams_FetalEchocardiographyResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_FetalEchocardiographyResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public UltraResParams_FetalEchocardiographyResult GetUltraResParams_FetalEchocardiographyResultByID(
            long UltraResParams_FetalEchocardiographyResultID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyResultByID(UltraResParams_FetalEchocardiographyResultID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyResultByID(UltraResParams_FetalEchocardiographyResultID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetUltraResParams_FetalEchocardiographyResultByID(UltraResParams_FetalEchocardiographyResultID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetUltraResParams_FetalEchocardiographyResultByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_FetalEchocardiographyResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddUltraResParams_FetalEchocardiographyResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddUltraResParams_FetalEchocardiographyResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddUltraResParams_FetalEchocardiographyResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiographyResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiographyResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateUltraResParams_FetalEchocardiographyResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateUltraResParams_FetalEchocardiographyResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiographyResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiographyResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteUltraResParams_FetalEchocardiographyResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteUltraResParams_FetalEchocardiographyResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Abdominal Ultrasound

        public bool InsertAbdominalUltrasoundResult(AbdominalUltrasound entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete)
        {
            bool PCLResult = false;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.InsertAbdominalUltrasoundResult(entity);
                //}
                //else
                //{
                //    PCLResult = PCLsProvider.Instance.InsertAbdominalUltrasoundResult(entity);
                //}
                PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.InsertAbdominalUltrasoundResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertAbdominalUltrasoundResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_InsertAbdominalUltrasoundResult);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

            if (!PCLResult)
            {
                return false;
            }

            if (ImagingResult != null)
            {
                try
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    //else
                    //{
                    //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of AddPCLResultFileStorageDetails. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1846_G1_LoiLuuFileHA));
                }
            }

            return true;
        }

        public bool UpdateAbdominalUltrasoundResult(AbdominalUltrasound entity, PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete)
        {
            bool PCLResult = false;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    PCLResult =  aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateAbdominalUltrasoundResult(entity);
                //}
                //else
                //{
                //    PCLResult = PCLsProvider.Instance.UpdateAbdominalUltrasoundResult(entity);
                //}
                PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateAbdominalUltrasoundResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateUltraResParams_EchoCardiography. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_UpdateAbdominalUltrasoundResult);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            if (!PCLResult)
                return false;
            if (ImagingResult != null)
            {
                try
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    //else
                    //{
                    //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of UpdateAbdominalUltrasoundResult. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
            }

            return true;
        }

        public AbdominalUltrasound GetAbdominalUltrasoundResult(long PatientPCLReqID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAbdominalUltrasoundResult(PatientPCLReqID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetAbdominalUltrasoundResult(PatientPCLReqID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetAbdominalUltrasoundResult(PatientPCLReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAbdominalUltrasoundResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetAbdominalUltrasoundResult);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion



        #region URP_FE_Exam

        public URP_FE_Exam GetURP_FE_Exam(
            long URP_FE_ExamID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_Exam(URP_FE_ExamID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_Exam(URP_FE_ExamID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_Exam(URP_FE_ExamID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_Exam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_Exam(URP_FE_Exam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_Exam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_Exam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_Exam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_Exam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_Exam(URP_FE_Exam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_Exam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_Exam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_Exam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_Exam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_Exam(URP_FE_Exam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_Exam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_Exam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_Exam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_Exam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_Oesophagienne
        public URP_FE_Oesophagienne GetURP_FE_Oesophagienne(
            long URP_FE_OesophagienneID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_Oesophagienne(URP_FE_OesophagienneID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_Oesophagienne(URP_FE_OesophagienneID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_Oesophagienne(URP_FE_OesophagienneID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_Oesophagienne. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_Oesophagienne(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_Oesophagienne(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_Oesophagienne(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_Oesophagienne. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_Oesophagienne(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_Oesophagienne(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_Oesophagienne(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_Oesophagienne. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_Oesophagienne(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_Oesophagienne(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_Oesophagienne(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_Oesophagienne. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_OesophagienneCheck
        public URP_FE_OesophagienneCheck GetURP_FE_OesophagienneCheck(
            long URP_FE_OesophagienneCheckID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheckID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheckID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheckID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_OesophagienneCheck. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_OesophagienneCheck(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_OesophagienneCheck(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_OesophagienneCheck(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_OesophagienneCheck. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_OesophagienneCheck(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_OesophagienneCheck(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_OesophagienneCheck(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_OesophagienneCheck. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_OesophagienneCheck(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_OesophagienneCheck(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_OesophagienneCheck(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_OesophagienneCheck. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_OesophagienneDiagnosis
        public URP_FE_OesophagienneDiagnosis GetURP_FE_OesophagienneDiagnosis(
            long URP_FE_OesophagienneDiagnosisID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosisID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosisID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosisID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_OesophagienneDiagnosis. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_OesophagienneDiagnosis(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_OesophagienneDiagnosis(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_OesophagienneDiagnosis(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_OesophagienneDiagnosis. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_OesophagienneDiagnosis(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_OesophagienneDiagnosis(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_OesophagienneDiagnosis(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_OesophagienneDiagnosis. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_OesophagienneDiagnosis(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_OesophagienneDiagnosis(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_OesophagienneDiagnosis(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_OesophagienneDiagnosis. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_StressDipyridamole
        public URP_FE_StressDipyridamole GetURP_FE_StressDipyridamole(
            long URP_FE_StressDipyridamoleID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamole(URP_FE_StressDipyridamoleID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDipyridamole(URP_FE_StressDipyridamoleID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamole(URP_FE_StressDipyridamoleID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDipyridamole. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamole(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDipyridamole(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamole(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDipyridamole. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamole(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDipyridamole(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamole(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDipyridamole. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamole(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDipyridamole(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamole(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDipyridamole. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region URP_FE_StressDipyridamoleElectrocardiogram
        public URP_FE_StressDipyridamoleElectrocardiogram GetURP_FE_StressDipyridamoleElectrocardiogram(
            long URP_FE_StressDipyridamoleElectrocardiogramID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogramID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogramID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogramID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDipyridamoleElectrocardiogram. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamoleElectrocardiogram(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDipyridamoleElectrocardiogram(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamoleElectrocardiogram(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDipyridamoleElectrocardiogram. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleElectrocardiogram(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleElectrocardiogram(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleElectrocardiogram(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDipyridamoleElectrocardiogram. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleElectrocardiogram(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleElectrocardiogram(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleElectrocardiogram(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDipyridamoleElectrocardiogram. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region URP_FE_StressDipyridamoleExam
        public URP_FE_StressDipyridamoleExam GetURP_FE_StressDipyridamoleExam(
            long URP_FE_StressDipyridamoleExamID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExamID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExamID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExamID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDipyridamoleExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamoleExam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDipyridamoleExam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamoleExam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDipyridamoleExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleExam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleExam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleExam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDipyridamoleExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleExam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleExam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleExam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDipyridamoleExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion


        #region URP_FE_StressDipyridamoleImage
        public URP_FE_StressDipyridamoleImage GetURP_FE_StressDipyridamoleImage(
            long URP_FE_StressDipyridamoleImageID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImageID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImageID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImageID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDipyridamoleImage. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamoleImage(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDipyridamoleImage(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamoleImage(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDipyridamoleImage. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleImage(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleImage(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleImage(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDipyridamoleImage. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleImage(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleImage(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleImage(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDipyridamoleImage. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_StressDipyridamoleResult
        public URP_FE_StressDipyridamoleResult GetURP_FE_StressDipyridamoleResult(
            long URP_FE_StressDipyridamoleResultID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResultID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResultID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResultID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDipyridamoleResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamoleResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDipyridamoleResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDipyridamoleResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDipyridamoleResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDipyridamoleResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDipyridamoleResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDipyridamoleResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDipyridamoleResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion



        #region URP_FE_StressDobutamine
        public URP_FE_StressDobutamine GetURP_FE_StressDobutamine(
            long URP_FE_StressDobutamineID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamine(URP_FE_StressDobutamineID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDobutamine(URP_FE_StressDobutamineID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamine(URP_FE_StressDobutamineID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDobutamine. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamine(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDobutamine(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamine(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDobutamine. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamine(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDobutamine(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamine(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDobutamine. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamine(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDobutamine(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamine(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDobutamine. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region URP_FE_StressDobutamineElectrocardiogram
        public URP_FE_StressDobutamineElectrocardiogram GetURP_FE_StressDobutamineElectrocardiogram(
            long URP_FE_StressDobutamineElectrocardiogramID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogramID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogramID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogramID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDobutamineElectrocardiogram. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamineElectrocardiogram(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDobutamineElectrocardiogram(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamineElectrocardiogram(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDobutamineElectrocardiogram. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamineElectrocardiogram(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDobutamineElectrocardiogram(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamineElectrocardiogram(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDobutamineElectrocardiogram. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamineElectrocardiogram(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDobutamineElectrocardiogram(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamineElectrocardiogram(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDobutamineElectrocardiogram. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region URP_FE_StressDobutamineExam
        public URP_FE_StressDobutamineExam GetURP_FE_StressDobutamineExam(
            long URP_FE_StressDobutamineExamID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExamID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExamID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExamID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDobutamineExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamineExam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDobutamineExam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamineExam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDobutamineExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamineExam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDobutamineExam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamineExam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDobutamineExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamineExam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDobutamineExam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamineExam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDobutamineExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region URP_FE_StressDobutamineImages
        public URP_FE_StressDobutamineImages GetURP_FE_StressDobutamineImages(
            long URP_FE_StressDobutamineImagesID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImagesID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImagesID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImagesID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDobutamineImages. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamineImages(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDobutamineImages(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamineImages(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDobutamineImages. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamineImages(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDobutamineImages(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamineImages(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDobutamineImages. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamineImages(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDobutamineImages(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamineImages(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDobutamineImages. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_StressDobutamineResult
        public URP_FE_StressDobutamineResult GetURP_FE_StressDobutamineResult(
            long URP_FE_StressDobutamineResultID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResultID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResultID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResultID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_StressDobutamineResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamineResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_StressDobutamineResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_StressDobutamineResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_StressDobutamineResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamineResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_StressDobutamineResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_StressDobutamineResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_StressDobutamineResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamineResult(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_StressDobutamineResult(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_StressDobutamineResult(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_StressDobutamineResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_VasculaireAnother
        public URP_FE_VasculaireAnother GetURP_FE_VasculaireAnother(
            long URP_FE_VasculaireAnotherID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireAnother(URP_FE_VasculaireAnotherID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_VasculaireAnother(URP_FE_VasculaireAnotherID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireAnother(URP_FE_VasculaireAnotherID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_VasculaireAnother. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireAnother(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_VasculaireAnother(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireAnother(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_VasculaireAnother. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireAnother(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_VasculaireAnother(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireAnother(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_VasculaireAnother. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireAnother(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_VasculaireAnother(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireAnother(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_VasculaireAnother. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region URP_FE_VasculaireAorta
        public URP_FE_VasculaireAorta GetURP_FE_VasculaireAorta(
            long URP_FE_VasculaireAortaID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireAorta(URP_FE_VasculaireAortaID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_VasculaireAorta(URP_FE_VasculaireAortaID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireAorta(URP_FE_VasculaireAortaID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_VasculaireAorta. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireAorta(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_VasculaireAorta(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireAorta(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_VasculaireAorta. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireAorta(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_VasculaireAorta(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireAorta(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_VasculaireAorta. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireAorta(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_VasculaireAorta(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireAorta(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_VasculaireAorta. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_VasculaireCarotid
        public URP_FE_VasculaireCarotid GetURP_FE_VasculaireCarotid(
            long URP_FE_VasculaireCarotidID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotidID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotidID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotidID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_VasculaireCarotid. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireCarotid(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_VasculaireCarotid(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireCarotid(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_VasculaireCarotid. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireCarotid(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_VasculaireCarotid(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireCarotid(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_VasculaireCarotid. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireCarotid(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_VasculaireCarotid(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireCarotid(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_VasculaireCarotid. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_VasculaireExam
        public URP_FE_VasculaireExam GetURP_FE_VasculaireExam(
            long URP_FE_VasculaireExamID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireExam(URP_FE_VasculaireExamID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_VasculaireExam(URP_FE_VasculaireExamID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireExam(URP_FE_VasculaireExamID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_VasculaireExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireExam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_VasculaireExam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireExam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_VasculaireExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireExam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_VasculaireExam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireExam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_VasculaireExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireExam(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_VasculaireExam(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireExam(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_VasculaireExam. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region URP_FE_VasculaireLeg
        public URP_FE_VasculaireLeg GetURP_FE_VasculaireLeg(
            long URP_FE_VasculaireLegID, long PCLImgResultID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireLeg(URP_FE_VasculaireLegID, PCLImgResultID);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.GetURP_FE_VasculaireLeg(URP_FE_VasculaireLegID, PCLImgResultID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetURP_FE_VasculaireLeg(URP_FE_VasculaireLegID, PCLImgResultID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetURP_FE_VasculaireLeg. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireLeg(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.AddURP_FE_VasculaireLeg(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddURP_FE_VasculaireLeg(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddURP_FE_VasculaireLeg. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireLeg(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.UpdateURP_FE_VasculaireLeg(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateURP_FE_VasculaireLeg(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateURP_FE_VasculaireLeg. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireLeg(entity);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.DeleteURP_FE_VasculaireLeg(entity);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteURP_FE_VasculaireLeg(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteURP_FE_VasculaireLeg. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        //==== 20161129 CMN Bein: Add button save for all FE pages
        public bool AddAndUpdateFE_Vasculaire(URP_FE_Vasculaire entity)
        {
            bool PCLResult = false;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateFE_Vasculaire(entity);
                //}
                //else
                //{
                //    PCLResult = PCLsProvider.Instance.AddAndUpdateFE_Vasculaire(entity);
                //}
                PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateFE_Vasculaire(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddAndUpdateFE_Vasculaire. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load update FE Vasculaire");

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
            if (!PCLResult)
                return false;
            if (entity.ObjPatientPCLImagingResult != null)
            {
                try
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    //else
                    //{
                    //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of UpdateAbdominalUltrasoundResult. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
            }

            return true;
        }
        public bool AddAndUpdateUltraResParams_FetalEchocardiography(UltraResParams_FetalEchocardiography entity)
        {
            bool PCLResult = false;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateUltraResParams_FetalEchocardiography(entity);
                //}
                //else
                //{
                //    PCLResult = PCLsProvider.Instance.AddAndUpdateUltraResParams_FetalEchocardiography(entity);
                //}
                PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateUltraResParams_FetalEchocardiography(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddAndUpdateUltraResParams_FetalEchocardiography. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load update Fetal Echocardiography");

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
            if (!PCLResult)
                return false;
            if (entity.ObjPatientPCLImagingResult != null)
            {
                try
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    //else
                    //{
                    //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of UpdateAbdominalUltrasoundResult. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
            }

            return true;
        }
        public bool AddAndUpdateURP_FE_Oesophagienne(URP_FE_OesophagienneUltra entity)
        {
            bool PCLResult = false;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateURP_FE_Oesophagienne(entity);
                //}
                //else
                //{
                //    PCLResult = PCLsProvider.Instance.AddAndUpdateURP_FE_Oesophagienne(entity);
                //}
                PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateURP_FE_Oesophagienne(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddAndUpdateURP_FE_Oesophagienne. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load update FE Oesophagienne");

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
            if (!PCLResult)
                return false;
            if (entity.ObjPatientPCLImagingResult != null)
            {
                try
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    //else
                    //{
                    //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of UpdateAbdominalUltrasoundResult. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
            }

            return true;
        }
        public bool AddAndUpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamoleUltra entity)
        {
            bool PCLResult = false;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateURP_FE_StressDipyridamole(entity);
                //}
                //else
                //{
                //    PCLResult = PCLsProvider.Instance.AddAndUpdateURP_FE_StressDipyridamole(entity);
                //}
                PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateURP_FE_StressDipyridamole(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddAndUpdateURP_FE_StressDipyridamole. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load update FE Oesophagienne");

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
            if (!PCLResult)
                return false;
            if (entity.ObjPatientPCLImagingResult != null)
            {
                try
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    //else
                    //{
                    //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of UpdateAbdominalUltrasoundResult. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
            }

            return true;
        }
        public bool AddAndUpdateURP_FE_StressDobutamine(URP_FE_StressDobutamineUltra entity)
        {
            bool PCLResult = false;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateURP_FE_StressDobutamine(entity);
                //}
                //else
                //{
                //    PCLResult = PCLsProvider.Instance.AddAndUpdateURP_FE_StressDobutamine(entity);
                //}
                PCLResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.AddAndUpdateURP_FE_StressDobutamine(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddAndUpdateURP_FE_StressDobutamine. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load update FE StressDobutamine");

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
            if (!PCLResult)
                return false;
            if (entity.ObjPatientPCLImagingResult != null)
            {
                try
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    //else
                    //{
                    //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                    //}
                    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(entity.ObjPatientPCLImagingResult, entity.FileForStore, entity.FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of UpdateAbdominalUltrasoundResult. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
            }

            return true;
        }
        //==== 20161129 CMN End: Add button save for all FE pages
        #endregion
        public IList<PatientPCLRequestDetail> GetPCLExamTypeServiceApmtTarget(long PCLExamTypeID, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPCLExamTypeServiceApmtTarget(PCLExamTypeID, FromDate, ToDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamTypeServiceApmtTarget. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load GetPCLExamTypeServiceApmtTarget");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PatientPCLRequest> PatientPCLRequest_SearchPaging_ForMedicalExamination(
            PatientPCLRequestSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                IList<PatientPCLRequest> ListResult;
                ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PatientPCLRequest_SearchPaging_ForMedicalExamination(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                return ListResult;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientPCLRequest_SearchPaging_ForMedicalExamination. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PatientPCLRequest_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #004
        public void UpdateReceptionTime(long PatientPCLReqID, DateTime ReceptionTime, long V_PCLRequestType)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateReceptionTime(PatientPCLReqID, ReceptionTime, V_PCLRequestType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateReceptionTime. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error UpdateReceptionTime");
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientPCLRequestDetail> GetHistoryPCLByPCLExamType(long PatientID, long PCLExamTypeID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                IList<PatientPCLRequestDetail> ListResult;
                ListResult = aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetHistoryPCLByPCLExamType(PatientID, PCLExamTypeID, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                return ListResult;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHistoryPCLByPCLExamType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetHistoryPCLByPCLExamType");
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void UpdateDateStarted2(long PtRegDetailID, DateTime DateStarted2)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdateDateStarted2(PtRegDetailID, DateStarted2);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateDateStarted2. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error UpdateDateStarted2");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void UpdatePrescriptionsDate(long PtRegistrationID, DateTime PrescriptionsDate)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdatePrescriptionsDate(PtRegistrationID, PrescriptionsDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdatePrescriptionsDate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error UpdatePrescriptionsDate");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #004
        public IList<PatientPCLImagingResultDetail> PCLImagingResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID, V_PCLRequestType);
                //}
                //else
                //{
                //    return PCLsProvider.Instance.PCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID, V_PCLRequestType);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.PCLImagingResults_With_ResultOld(PatientID, PatientPCLReqID, V_PCLRequestType);
            }
            catch (SqlException ex)
            {
                if (ex.Class == 16)//message tu database show len
                {
                    return null;
                }
                else
                {
                    AxLogger.Instance.LogInfo("End of GetPatientPCLImagingResults_ByPatientPCLReqID. Status: Failed.", CurrentUser);

                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_GetPatientPCLLaboratoryResults_ByPatientPCLReqID);

                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }
        public IList<PatientPCLRequest> GetPatientPCLLaboratoryResultForSendTransaction(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetPatientPCLLaboratoryResultForSendTransaction(SearchCriteriaSendTransaction
                    , PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPatientPCLLaboratoryResultForSendTransaction. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetPatientPCLLaboratoryResultForSendTransaction");
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<PatientPCLRequest> GetListHistoryTransaction_Paging(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetListHistoryTransaction_Paging(SearchCriteriaSendTransaction
                    , PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetListHistoryTransaction_Paging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetListHistoryTransaction_Paging");
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdatePCLRequestStatusGeneratingResult(string ListPCLResults, long V_PCLRequestStatus)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.UpdatePCLRequestStatusGeneratingResult(ListPCLResults, V_PCLRequestStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdatePCLRequestStatusGeneratingResult. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error UpdatePCLRequestStatusGeneratingResult");
                throw new FaultException<AxException>(axErr, ex.Message);
            }
        }


        // step 2: gửi danh sách file PDF qua cho service gửi SmartCA ký 
        // Step 3: sau khi ký thì service đánh dấu đã ký
        public bool DeletePCLRequestFromList(PatientPCLRequest request, int PatientFindBy)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeletePCLRequestFromList(request, PatientFindBy);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeletePCLRequestFromList. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error DeletePCLRequestFromList");
                throw new FaultException<AxException>(axErr, ex.Message);
            }
        }
        public bool DeleteTransactionHistory(PatientPCLRequest request, int PatientFindBy)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.DeleteTransactionHistory(request, PatientFindBy);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteTransactionHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error DeleteTransactionHistory");
                throw new FaultException<AxException>(axErr, ex.Message);
            }
        }

        //▼====: #005
        public List<Resources> GetResourcesForMedicalServicesListByDeptIDAndTypeID(long DeptID, long PtRegDetailID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetResourcesForMedicalServicesListByDeptIDAndTypeID(DeptID, PtRegDetailID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcesForMedicalServicesListByDeptIDAndTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Resources> GetResourcesForMedicalServicesListBySmallProcedureID(long SmallProcedureID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetResourcesForMedicalServicesListBySmallProcedureID(SmallProcedureID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcesForMedicalServicesListBySmallProcedureID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #005

        //▼====: #006
        public IList<PatientPCLRequest> GetListAppointmentsLab_Paging(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsProvider.Instance.GetListAppointmentsLab_Paging(SearchCriteriaSendTransaction, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetListHistoryTransaction_Paging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetListHistoryTransaction_Paging");
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #006
    }
}