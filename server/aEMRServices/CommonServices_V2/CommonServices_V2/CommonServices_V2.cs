using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using AxLogging;
using DataEntities;

using eHCMS.Caching;

using eHCMS.Services.Core;
using ErrorLibrary;
using ErrorLibrary.Resources;
using eHCMSLanguage;
using eHCMS.Configurations;

using aEMR.DataAccessLayer.Providers;
using DataEntities.MedicalInstruction;

/*
 * 20180606 #001 CMN: Added enum for LabSoft API
 * 20181113 #002 TTM: BM 0005228: Thêm hàm lấy danh sách phường xã
 * 20210118 #003 TNHX: Thêm service cho PAC
 * 20210217 #004 TNHX: Thêm mã SID - SampleCode cho LIS
 * 20210923 #005 TNHX: 571 Quản lý điều dưỡng thực hiện y lệnh
 * 20230316 #006 QTD:  Dữ liệu 130
 * 20230601 #007 QTD:  Lấy lại dữ liệu cho quản lý danh mục Lookup
*/
namespace CommonService_V2
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceKnownTypeAttribute(typeof(IInvoiceItem))]
    public class CommonService_V2 : eHCMS.WCFServiceCustomHeader, ICommonService_V2
    {
        private string _ModuleName = "Common Module";

        public CommonService_V2()
        {
        }

        public List<PatientTransactionPayment> GetAllPayments_New(long transactionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all payments.", CurrentUser);
                List<PatientTransactionPayment> allPayments = new List<PatientTransactionPayment>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allPayments = aEMR.DataAccessLayer.Providers.CommonProvider.Payments.GetAllPayments_New(transactionID);
                //}
                //else
                //{
                //    allPayments = CommonProvider.Payments.GetAllPayments_New(transactionID);
                //}
                allPayments = aEMR.DataAccessLayer.Providers.CommonProvider.Payments.GetAllPayments_New(transactionID);
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Success.", CurrentUser);
                return allPayments;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientTransactionPayment> GetPatientPaymentByDay_New(PatientPaymentSearchCriteria PatientPaymentSearch, int FindPatient)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all payments.", CurrentUser);
                List<PatientTransactionPayment> allPayments = new List<PatientTransactionPayment>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allPayments = aEMR.DataAccessLayer.Providers.CommonProvider.Payments.GetPatientPaymentByDay_New(PatientPaymentSearch, FindPatient);
                //}
                //else
                //{
                //    allPayments = CommonProvider.Payments.GetPatientPaymentByDay_New(PatientPaymentSearch, FindPatient);
                //}
                allPayments = aEMR.DataAccessLayer.Providers.CommonProvider.Payments.GetPatientPaymentByDay_New(PatientPaymentSearch, FindPatient);
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Success.", CurrentUser);
                return allPayments;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff, List<PatientTransactionPayment> allPayment, out long RepPaymentRecvID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonProvider.Payments.AddReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff, allPayment, out RepPaymentRecvID);
                //}
                //else
                //{
                //    return CommonProvider.Payments.AddReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff, allPayment, out RepPaymentRecvID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonProvider.Payments.AddReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff, allPayment, out RepPaymentRecvID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<ReportPaymentReceiptByStaff> GetReportPaymentReceiptByStaff(DateTime FromDate, DateTime ToDate, bool bFilterByStaffID, long loggedStaffID, Int64 nStaffID)
        {
            try
            {
                List<ReportPaymentReceiptByStaff> allReportPaymentReceiptByStaff = new List<ReportPaymentReceiptByStaff>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allReportPaymentReceiptByStaff = aEMR.DataAccessLayer.Providers.CommonProvider.Payments.GetReportPaymentReceiptByStaff(FromDate, ToDate, bFilterByStaffID, nStaffID, loggedStaffID);
                //}
                //else
                //{
                //    allReportPaymentReceiptByStaff = CommonProvider.Payments.GetReportPaymentReceiptByStaff(FromDate, ToDate, bFilterByStaffID, nStaffID, loggedStaffID);
                //}
                allReportPaymentReceiptByStaff = aEMR.DataAccessLayer.Providers.CommonProvider.Payments.GetReportPaymentReceiptByStaff(FromDate, ToDate, bFilterByStaffID, nStaffID, loggedStaffID);
                return allReportPaymentReceiptByStaff;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public ReportPaymentReceiptByStaffDetails GetReportPaymentReceiptByStaffDetails(long RepPaymentRecvID)
        {
            try
            {
                ReportPaymentReceiptByStaffDetails allReportPaymentReceiptByStaffDetails = new ReportPaymentReceiptByStaffDetails();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allReportPaymentReceiptByStaffDetails = aEMR.DataAccessLayer.Providers.CommonProvider.Payments.GetReportPaymentReceiptByStaffDetails(RepPaymentRecvID);
                //}
                //else
                //{
                //    allReportPaymentReceiptByStaffDetails = CommonProvider.Payments.GetReportPaymentReceiptByStaffDetails(RepPaymentRecvID);
                //}
                allReportPaymentReceiptByStaffDetails = aEMR.DataAccessLayer.Providers.CommonProvider.Payments.GetReportPaymentReceiptByStaffDetails(RepPaymentRecvID);
                return allReportPaymentReceiptByStaffDetails;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateReportPaymentReceiptByStaff(ReportPaymentReceiptByStaff curReportPaymentReceiptByStaff)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonProvider.Payments.UpdateReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff);
                //}
                //else
                //{
                //    return CommonProvider.Payments.UpdateReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff);
                //}
                return aEMR.DataAccessLayer.Providers.CommonProvider.Payments.UpdateReportPaymentReceiptByStaff(curReportPaymentReceiptByStaff);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all payments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public DateTime GetMaxExamDate()
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.PatientRg.GetMaxExamDate();
            //}
            //else
            //{
            //    return CommonProvider.PatientRg.GetMaxExamDate();
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.PatientRg.GetMaxExamDate();
        }

        private PatientPCLRequestDetail CreateRequestDetailsForPCLExamType(PCLExamType item)
        {
            var details = new PatientPCLRequestDetail();
            if (item != null)
            {
                details.PCLExamType = item;
                details.PCLExamTypeID = item.PCLExamTypeID;
                details.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                details.V_ExamRegStatus = (long)details.ExamRegStatus;
                if (item.PCLExamTypeLocations != null && item.PCLExamTypeLocations.Count > 0)
                {
                    details.DeptLocation = item.PCLExamTypeLocations[0].DeptLocation;
                }
            }
            return details;
        }


        public TransferForm CreateBlankTransferFormByRegID(long PtRegistrationID, int PatientFindBy, long V_TransferFormType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start creating BlankTransferFormByRegID .", CurrentUser);
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.CreateBlankTransferFormByRegID(PtRegistrationID, PatientFindBy, V_TransferFormType);
                }
                else
                {
                    return PatientProvider.Instance.CreateBlankTransferFormByRegID(PtRegistrationID, PatientFindBy, V_TransferFormType);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CreateBlankTransferFormByRegID . Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_RegistrationSimple, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public DateTime GetDate()
        {
            return DateTime.Now;
        }

        public List<PatientRegistrationDetail> CheckServiceExist(List<PatientRegistrationDetail> regDetails, List<PatientRegistrationDetail> regDetailList)
        {
            //loc danh sach regDetailList ngoai tru nhung phan tu nam trong regDetails
            if (regDetailList == null || regDetailList.Count < 1)
            {
                return null;
            }
            if (regDetails == null || regDetails.Count < 1)
            {
                return regDetailList;
            }
            return regDetailList.Where(
                u => regDetailList.Select(i => i.PtRegDetailID).Except(regDetails.
                    Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
        }

        public List<PatientRegistrationDetail> CheckServiceHasPaidExist(List<PatientRegistrationDetail> regDetailList, List<PatientRegistrationDetail> delRegDetailList, bool isDoing = false)
        {
            if (regDetailList == null || delRegDetailList == null)
            {
                return null;
            }
            if (isDoing)
            {
                return delRegDetailList.Where(
                u => delRegDetailList.Select(i => i.PtRegDetailID).Intersect(regDetailList.Where(s =>
                    (s.PaidTime != null && s.ExamRegStatus != AllLookupValues.ExamRegStatus.DANG_KY_KHAM
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI))
                    .Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
            }
            else
            {
                return delRegDetailList.Where(
                u => delRegDetailList.Select(i => i.PtRegDetailID).Intersect(regDetailList.Where(s => (s.PaidTime != null))
                    .Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
            }

        }

        public List<PatientPCLRequestDetail> CheckPCLExist(List<PatientPCLRequest> regPCLInfo, List<PatientPCLRequestDetail> regPCLDetails, bool isDoing = false)
        {
            if (regPCLDetails == null || regPCLDetails.Count < 1)
            {
                return null;
            }
            if (regPCLInfo == null || regPCLInfo.Count < 1)
            {
                return regPCLDetails;
            }
            List<PatientPCLRequestDetail> temp = new List<PatientPCLRequestDetail>();
            foreach (var item in regPCLInfo)
            {
                temp.AddRange(regPCLDetails.Where(
                u => regPCLDetails.Select(i => i.PCLReqItemID).
                    Except(item.PatientPCLRequestIndicators.Select(k => k.PCLReqItemID)
                    ).Contains(u.PCLReqItemID)).ToList());
            }
            return temp;
        }

        public List<PatientPCLRequestDetail> CheckPCLPaidExist(List<PatientPCLRequest> regPCLInfo, List<PatientPCLRequestDetail> regPCLDetails, bool isDoing = false)
        {
            List<PatientPCLRequestDetail> temp = new List<PatientPCLRequestDetail>();
            foreach (var item in regPCLInfo)
            {
                if (isDoing)
                {
                    temp.AddRange(regPCLDetails.Where(
                    u => regPCLDetails.Select(i => i.PCLReqItemID).
                        Intersect(item.PatientPCLRequestIndicators.Where(s => (s.PaidTime != null && s.ExamRegStatus != AllLookupValues.ExamRegStatus.DANG_KY_KHAM
                            && s.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI))
                        .Select(k => k.PCLReqItemID)
                        ).Contains(u.PCLReqItemID)).ToList());
                }
                else
                {
                    temp.AddRange(regPCLDetails.Where(
                    u => regPCLDetails.Select(i => i.PCLReqItemID).
                        Intersect(item.PatientPCLRequestIndicators.Where(s => (s.PaidTime != null))
                        .Select(k => k.PCLReqItemID)
                        ).Contains(u.PCLReqItemID)).ToList());
                }
            }
            return temp;
        }

        public List<PatientPCLRequest> CheckPCLPaidExist(List<PatientPCLRequest> regPCLInfo, List<PatientPCLRequest> regPCLDetails, bool isDoing = false)
        {
            List<PatientPCLRequest> temps = new List<PatientPCLRequest>();
            foreach (var item in regPCLDetails)
            {
                var pclDetails = CheckPCLPaidExist(regPCLInfo, item.PatientPCLRequestIndicators.ToList(), isDoing);
                if (pclDetails != null && pclDetails.Count > 0)
                {
                    var temp = item;
                    temp.PatientPCLRequestIndicators = new List<PatientPCLRequestDetail>().ToObservableCollection();
                    temp.PatientPCLRequestIndicators.AddRange(pclDetails);
                    temps.Add(temp);
                }
            }
            return temps;
        }

        private PatientRegistration GetRegistrationAndOtherDetails(long nPtRegistrationID, int nFindPatient, bool bGetRegDetails, bool bGetPCLReqDetails)
        {
            PatientRegistration regInfo = new PatientRegistration();
            DbConnection connection;
            if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            {
                regInfo = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetRegistration(nPtRegistrationID, nFindPatient);
                connection = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.CreateConnection();
            }
            else
            {
                regInfo = PatientProvider.Instance.GetRegistration(nPtRegistrationID, nFindPatient);
                connection = PatientProvider.Instance.CreateConnection();
            }
            List<PatientPCLRequest> pclRequestList = null;
            List<PatientRegistrationDetail> regDetails = null;

            using (connection)
            {
                if (bGetRegDetails)
                {
                    if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    {
                        regDetails = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetAllRegistrationDetails(nPtRegistrationID, nFindPatient, connection, null);
                    }
                    else
                    {
                        regDetails = PatientProvider.Instance.GetAllRegistrationDetails(nPtRegistrationID, nFindPatient, connection, null);
                    }

                    if (regDetails != null)
                    {
                        regInfo.PatientRegistrationDetails = new List<PatientRegistrationDetail>().ToObservableCollection();
                        foreach (var item in regDetails)
                        {
                            regInfo.PatientRegistrationDetails.Add(item);
                        }
                    }
                }

                if (bGetPCLReqDetails)
                {
                    if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    {
                        pclRequestList = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetPCLRequestListByRegistrationID(nPtRegistrationID, connection, null);
                    }
                    else
                    {
                        pclRequestList = PatientProvider.Instance.GetPCLRequestListByRegistrationID(nPtRegistrationID, connection, null);
                    }
                    if (pclRequestList != null)
                    {
                        regInfo.PCLRequests = pclRequestList.ToObservableCollection();
                    }
                }
            }

            return regInfo;
        }

        public PatientPCLRequest UpdateDrAndDiagTrmtForPCLReq(long ServiceRecID, long PCLReqID, long DoctorStaffID)
        {
            try
            {
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.UpdateDrAndDiagTrmtForPCLReq(ServiceRecID, PCLReqID, DoctorStaffID);
                }
                else
                {
                    return PatientProvider.Instance.UpdateDrAndDiagTrmtForPCLReq(ServiceRecID, PCLReqID, DoctorStaffID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateDrAndDiagTrmtForPCLReq. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddServicesAndPCLRequests, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void DeletePCLRequestWithDetails(Int64 PatientPCLReqID, out string Result)
        {
            try
            {
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    aEMR.DataAccessLayer.Providers.PatientProvider.Instance.DeletePCLRequestWithDetails(PatientPCLReqID, out Result);
                }
                else
                {
                    PatientProvider.Instance.DeletePCLRequestWithDetails(PatientPCLReqID, out Result);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeletePCLRequestWithDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_DeletePCLRequestWithDetails);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void DeleteInPtPCLRequestWithDetails(Int64 PatientPCLReqID)
        {
            try
            {
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    aEMR.DataAccessLayer.Providers.PatientProvider.Instance.DeleteInPtPCLRequestWithDetails(PatientPCLReqID);
                }
                else
                {
                    PatientProvider.Instance.DeleteInPtPCLRequestWithDetails(PatientPCLReqID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteInPtPCLRequestWithDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_DeletePCLRequestWithDetails);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }            
        }

        public Hospital SearchHospitalByHICode(string HiCode)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching hospitals by HICode.", CurrentUser);
                Hospital hospital = new Hospital();
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    hospital = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.SearchHospitalByHICode(HiCode);
                }
                else
                {
                    hospital = PatientProvider.Instance.SearchHospitalByHICode(HiCode);
                }
                AxLogger.Instance.LogInfo("End of searching hospitals by HICode. Status: Success.", CurrentUser);
                return hospital;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching hospitals by HICode. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchHospitalByHICode, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Hospital> LoadCrossRegionHospitals()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start getting list of hospitals which are allowed to cross region.", CurrentUser);
                List<Hospital> hospital = new List<Hospital>();
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    hospital = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.LoadCrossRegionHospitals();
                }
                else
                {
                    hospital = PatientProvider.Instance.LoadCrossRegionHospitals();
                }
                AxLogger.Instance.LogInfo("End getting list of hospitals which are allowed to cross region.", CurrentUser);
                return hospital;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End getting list of hospitals which are allowed to cross region. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchHospitalByHICode, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Hospital> SearchHospitals(string HospitalName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching hospitals.", CurrentUser);
                List<Hospital> hospitals = new List<Hospital>();
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    hospitals = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.SearchHospitals(HospitalName, pageIndex, pageSize, bCountTotal, out totalCount);
                }
                else
                {
                    hospitals = PatientProvider.Instance.SearchHospitals(HospitalName, pageIndex, pageSize, bCountTotal, out totalCount);
                }
                AxLogger.Instance.LogInfo("End of searching hospitals. Status: Success.", CurrentUser);
                return hospitals;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching hospitals. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchHospitals, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Hospital> SearchHospitalsNew(HospitalSearchCriteria entity, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching hospitals.", CurrentUser);
                List<Hospital> hospitals = new List<Hospital>();
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    hospitals = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.SearchHospitalsNew(entity, pageIndex, pageSize, bCountTotal, out totalCount);
                }
                else
                {
                    hospitals = PatientProvider.Instance.SearchHospitalsNew(entity, pageIndex, pageSize, bCountTotal, out totalCount);
                }
                AxLogger.Instance.LogInfo("End of searching hospitals. Status: Success.", CurrentUser);
                return hospitals;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching hospitals. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchHospitals, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void CreateForm02(InPatientRptForm02 CurrentRptForm02, List<InPatientBillingInvoice> billingInvoices)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start processing create form 02.", CurrentUser);
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    aEMR.DataAccessLayer.Providers.PatientProvider.Instance.CreateForm02(CurrentRptForm02, billingInvoices);
                }
                else
                {
                    PatientProvider.Instance.CreateForm02(CurrentRptForm02, billingInvoices);
                }
                
                AxLogger.Instance.LogInfo("End of processing create form 02. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing create form 02. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateForm02, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<InPatientRptForm02> GetForm02(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start processing get form 02.", CurrentUser);
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetForm02(PtRegistrationID);
                }
                else
                {
                    return PatientProvider.Instance.GetForm02(PtRegistrationID);
                }

                //AxLogger.Instance.LogInfo("Start processing get form 02. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing get form 02. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetForm02, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void InPatientSettlement(long ptRegistrationID, long staffID)
        {
            try
            {
                if (ptRegistrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start InPatientSettlement.", CurrentUser);
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    aEMR.DataAccessLayer.Providers.PatientProvider.Instance.InPatientSettlement(ptRegistrationID, staffID);
                }
                else
                {
                    PatientProvider.Instance.InPatientSettlement(ptRegistrationID, staffID);
                }
                AxLogger.Instance.LogInfo("End InPatientSettlement. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("InPatientSettlement. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_InPatientSettlement, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }

        public List<PatientPaymentAccount> GetAllPatientPaymentAccounts()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllPatientPaymentAccounts.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllPatientPaymentAccounts();
                //}
                //else
                //{
                //    return CommonProvider.Lookups.GetAllPatientPaymentAccounts();
                //}
                return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllPatientPaymentAccounts();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllPatientPaymentAccounts. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientRegistration GetRegistrationNoiTru(long registrationID)
        {
            if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            {
                return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetRegistration(registrationID, (int)AllLookupValues.V_FindPatientType.NOI_TRU);
            }
            else
            {
                return PatientProvider.Instance.GetRegistration(registrationID, (int)AllLookupValues.V_FindPatientType.NOI_TRU);
            }
        }

        public PatientRegistration GetRegistrationNgoaiTru(long registrationID)
        {
            if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            {
                return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetRegistration(registrationID, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
            }
            else
            {
                return PatientProvider.Instance.GetRegistration(registrationID, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
            }
        }

        public List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_DeptID(long DeptID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RefDepartmentReqCashAdv_DeptID.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.RefDepartmentReqCashAdv_DeptID(DeptID);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.RefDepartmentReqCashAdv_DeptID(DeptID);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.RefDepartmentReqCashAdv_DeptID(DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RefDepartmentReqCashAdv_DeptID. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientCashAdvance> PatientCashAdvance_GetAll(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientCashAdvance_GetAll.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.PatientCashAdvance_GetAll(PtRegistrationID, V_RegistrationType);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.PatientCashAdvance_GetAll(PtRegistrationID, V_RegistrationType);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.PatientCashAdvance_GetAll(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientCashAdvance_GetAll. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool GenericPayment_FullOperation(GenericPayment GenPayment, out GenericPayment OutGenericPayment)
        {
            try
            {
                OutGenericPayment = new GenericPayment();
                long OutGenericPaymentID = 0;
                AxLogger.Instance.LogInfo("Start loading GenericPayment_FullOperation.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GenericPayment_FullOperation(GenPayment, out OutGenericPaymentID);
                //}
                //else
                //{
                //    PaymentProvider.Instance.GenericPayment_FullOperation(GenPayment, out OutGenericPaymentID);
                //}
                aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GenericPayment_FullOperation(GenPayment, out OutGenericPaymentID);
                bool BOK;
                BOK = (OutGenericPaymentID > 0);
                if (BOK)
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    OutGenericPayment = aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetGenericPaymentByID(OutGenericPaymentID);
                    //}
                    //else
                    //{
                    //    OutGenericPayment = PaymentProvider.Instance.GetGenericPaymentByID(OutGenericPaymentID);
                    //}
                    OutGenericPayment = aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetGenericPaymentByID(OutGenericPaymentID);
                }
                return BOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GenericPayment_FullOperation. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<GenericPayment> GenericPayment_GetAll(DateTime? FromDate, DateTime? ToDate, long? V_GenericPaymentType, long? StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GenericPayment_GetAll.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GenericPayment_GetAll(FromDate, ToDate, V_GenericPaymentType, StaffID);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.GenericPayment_GetAll(FromDate, ToDate, V_GenericPaymentType, StaffID);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GenericPayment_GetAll(FromDate, ToDate, V_GenericPaymentType, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GenericPayment_GetAll. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public GenericPayment GenericPayment_SearchByCode(string GenPaymtCode, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GenericPayment_GetAll.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GenericPayment_SearchByCode(GenPaymtCode, StaffID);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.GenericPayment_SearchByCode(GenPaymtCode, StaffID);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GenericPayment_SearchByCode(GenPaymtCode, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GenericPayment by Code. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<TransactionFinalization> GetPatientSettlement(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetPatientSettlement.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetPatientSettlement(PtRegistrationID, V_RegistrationType);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.GetPatientSettlement(PtRegistrationID, V_RegistrationType);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetPatientSettlement(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetPatientSettlement. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetPatientSettlement, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public PatientTransaction GetAllPaymentByRegistrationID_InPt(long registrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllPaymentByRegistrationID_InPt.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetAllPaymentByRegistrationID_InPt(registrationID);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.GetAllPaymentByRegistrationID_InPt(registrationID);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetAllPaymentByRegistrationID_InPt(registrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllPaymentByRegistrationID_InPt. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RptPatientCashAdvReminder> RptPatientCashAdvReminder_GetAll(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_GetAll.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.RptPatientCashAdvReminder_GetAll(PtRegistrationID);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.RptPatientCashAdvReminder_GetAll(PtRegistrationID);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.RptPatientCashAdvReminder_GetAll(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_GetAll. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt(SearchOutwardReport Searchcriate)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_GetAll.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetReportOutPatientCashReceipt(Searchcriate);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.GetReportOutPatientCashReceipt(Searchcriate);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetReportOutPatientCashReceipt(Searchcriate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_GetAll. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PatientTransactionPayment_UpdateNote(List<PatientTransactionPayment> allPayment)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientTransactionPayment_UpdateNote.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.PatientTransactionPayment_UpdateNote(allPayment);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.PatientTransactionPayment_UpdateNote(allPayment);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.PatientTransactionPayment_UpdateNote(allPayment);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientTransactionPayment_UpdateNote. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PatientTransactionPayment_UpdateID(PatientTransactionPayment Payment)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientTransactionPayment_UpdateNote.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.PatientTransactionPayment_UpdateID(Payment);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.PatientTransactionPayment_UpdateID(Payment);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.PatientTransactionPayment_UpdateID(Payment);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientTransactionPayment_UpdateNote. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientTransactionPayment> PatientTransactionPayment_Load(long TransactionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading PatientTransactionPayment_Load.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.PatientTransactionPayment_Load(TransactionID);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.PatientTransactionPayment_Load(TransactionID);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.PatientTransactionPayment_Load(TransactionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientTransactionPayment_Load. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<ReportOutPatientCashReceipt_Payments> GetReportOutPatientCashReceipt_TongHop(SearchOutwardReport Searchcriate, bool? IsTongHop, long loggedStaffID, out List<PatientTransactionPayment> OutPaymentLst)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetReportOutPatientCashReceipt_TongHop.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetReportOutPatientCashReceipt_TongHop(Searchcriate, IsTongHop, loggedStaffID, out OutPaymentLst);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.GetReportOutPatientCashReceipt_TongHop(Searchcriate, IsTongHop, loggedStaffID, out OutPaymentLst);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetReportOutPatientCashReceipt_TongHop(Searchcriate, IsTongHop, loggedStaffID, out OutPaymentLst);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetReportOutPatientCashReceipt_TongHop. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #region Export excel from database
        public List<List<string>> ExportToExcellAll_ListRefGenericDrug(DrugSearchCriteria criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ExportToExcellAll_ListRefGenericDrug.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenericDrug(criteria);
                //}
                //else
                //{
                //    return AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenericDrug(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenericDrug(criteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_ListRefGenericDrug. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼===== 25072018 TTM
        public List<List<string>> ExportToExcellAll_ListRefGenericDrug_New(DrugSearchCriteria criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ExportToExcellAll_ListRefGenericDrug_New.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenericDrug_New(criteria);
                //}
                //else
                //{
                //    return AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenericDrug_New(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenericDrug_New(criteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_ListRefGenericDrug_New. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== 25072018 TTM
        public List<List<string>> ExportToExcellAll_ListRefGenMedProductDetail(RefGenMedProductDetailsSearchCriteria criteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ExportToExcellAll_ListRefGenericDrug.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenMedProductDetail(criteria);
                //}
                //else
                //{
                //    return AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenMedProductDetail(criteria);
                //}
                return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.ExportToExcellAll_ListRefGenMedProductDetail(criteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportToExcellAll_ListRefGenMedProductDetail. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region InPatientInstruction
        public void AddInPatientInstruction(PatientRegistration ptRegistration, bool IsUpdateDiagConfirmInPT, long WebIntPtDiagDrInstructionID, out long IntPtDiagDrInstructionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start saving inpatient Registration.", CurrentUser);
                if (ptRegistration.InPatientInstruction.PclRequests != null)
                {
                    foreach (PatientPCLRequest row in ptRegistration.InPatientInstruction.PclRequests)
                    {
                        if (row.PatientPCLRequestIndicators != null)
                        {
                            foreach (PatientPCLRequestDetail itemrow in row.PatientPCLRequestIndicators)
                            {
                                itemrow.PatientPCLRequest = row;
                            }
                        }
                    }
                    List<PatientPCLRequestDetail> newLstRequestDetails = ptRegistration.InPatientInstruction.PclRequests.Where(x => x.PatientPCLReqID == 0).SelectMany(request => request.PatientPCLRequestIndicators).ToList();
                    var newLstRequests = eHCMSBillPaymt.RegAndPaymentProcessorBase.SplitVote(newLstRequestDetails);
                    foreach (var item in newLstRequests)
                    {
                        item.MedicalInstructionDate = ptRegistration.InPatientInstruction.InstructionDate;
                        item.DoctorStaff = ptRegistration.InPatientInstruction.DoctorStaff;
                        item.DoctorStaffID = ptRegistration.InPatientInstruction.DoctorStaff == null ? 0 : ptRegistration.InPatientInstruction.DoctorStaff.StaffID;
                    }
                    if (newLstRequests == null) newLstRequests = new List<PatientPCLRequest>();
                    foreach (var item in ptRegistration.InPatientInstruction.PclRequests.Where(x => x.PatientPCLReqID > 0))
                    {
                        newLstRequests.Add(item);
                    }
                    ptRegistration.InPatientInstruction.PclRequests = newLstRequests.ToObservableCollection();
                }
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    aEMR.DataAccessLayer.Providers.PatientProvider.Instance.AddInPatientInstruction(ptRegistration, IsUpdateDiagConfirmInPT, WebIntPtDiagDrInstructionID, out IntPtDiagDrInstructionID);
                }
                else
                {
                    PatientProvider.Instance.AddInPatientInstruction(ptRegistration, IsUpdateDiagConfirmInPT, WebIntPtDiagDrInstructionID, out IntPtDiagDrInstructionID);
                }
                AxLogger.Instance.LogInfo("End of saving inpatient Registration.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving inpatient Registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public InPatientInstruction GetInPatientInstruction(PatientRegistration ptRegistration)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInPatientInstruction.", CurrentUser);
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetInPatientInstruction(ptRegistration);
                }
                else
                {
                    return PatientProvider.Instance.GetInPatientInstruction(ptRegistration);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInPatientInstruction. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<InPatientInstruction> GetInPatientInstructionCollection(PatientRegistration aRegistration)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInPatientInstructionCollection.", CurrentUser);
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetInPatientInstructionCollection(aRegistration);
                }
                else
                {
                    return PatientProvider.Instance.GetInPatientInstructionCollection(aRegistration);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInPatientInstructionCollection. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<InPatientInstruction> GetInPatientInstructionCollectionForCreateOutward(long PtRegistrationID, bool? IsCreatedOutward, long V_MedProductType, long StoreID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInPatientInstructionCollectionForCreateOutward.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetInPatientInstructionCollection(new PatientRegistration { PtRegistrationID = PtRegistrationID }, IsCreatedOutward, V_MedProductType, StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInPatientInstructionCollectionForCreateOutward. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<InPatientInstruction> GetInPatientInstructionCollectionForTransmissionMonitor(long PtRegistrationID)//, bool? IsCreatedOutward, long V_MedProductType, long StoreID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInPatientInstructionCollectionForTransmissionMonitor.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetInPatientInstructionCollectionForTransmissionMonitor(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInPatientInstructionCollectionForTransmissionMonitor. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<TransmissionMonitor> GetTransmissionMonitorByInPatientInstructionID(long InPatientInstructionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetTransmissionMonitorByInPatientInstructionID.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetTransmissionMonitorByInPatientInstructionID(InPatientInstructionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetTransmissionMonitorByInPatientInstructionID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool SaveTransmissionMonitor(TransmissionMonitor CurTransmissionMonitor, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveTransmissionMonitor.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.SaveTransmissionMonitor(CurTransmissionMonitor, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveTransmissionMonitor. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public InPatientInstruction GetInPatientInstructionByInstructionID(long aIntPtDiagDrInstructionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInPatientInstructionByInstructionID.", CurrentUser);
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetInPatientInstructionByInstructionID(aIntPtDiagDrInstructionID);
                }
                else
                {
                    return PatientProvider.Instance.GetInPatientInstructionByInstructionID(aIntPtDiagDrInstructionID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInPatientInstructionByInstructionID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<Intravenous> GetIntravenousPlan_InPt(long IntPtDiagDrInstructionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetIntravenousPlan.", CurrentUser);
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetIntravenousPlan_InPt(IntPtDiagDrInstructionID);
                }
                else
                {
                    return PatientProvider.Instance.GetIntravenousPlan_InPt(IntPtDiagDrInstructionID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetIntravenousPlan. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<ReqOutwardDrugClinicDeptPatient> GetAntibioticTreatmentUsageHistory(long PtRegistrationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetAntibioticTreatmentUsageHistory(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAntibioticTreatmentUsageHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<ReqOutwardDrugClinicDeptPatient> GetAllDrugFromDoctorInstruction(long PtRegistrationID, DateTime CurrentDate)
        {
            try
            {
                return PatientProvider.Instance.GetAllDrugFromDoctorInstruction(PtRegistrationID, CurrentDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllDrugFromDoctorInstruction. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void GetAllItemsByInstructionID(long IntPtDiagDrInstructionID, out List<PatientRegistrationDetail> AllRegistrationItems, out List<PatientPCLRequestDetail> AllPCLRequestItems)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetAllRegistrationItemsByInstructionID.", CurrentUser);
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetAllItemsByInstructionID(IntPtDiagDrInstructionID, out AllRegistrationItems, out AllPCLRequestItems);
                }
                else
                {
                    PatientProvider.Instance.GetAllItemsByInstructionID(IntPtDiagDrInstructionID, out AllRegistrationItems, out AllPCLRequestItems);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRegistrationItemsByInstructionID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool SaveInstructionMonitoring(InPatientInstruction InPatientInstruction, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveInstructionMonitoring.", CurrentUser);
                return PatientProvider.Instance.SaveInstructionMonitoring(InPatientInstruction, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveInstructionMonitoring. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //private bool SplitVoteCondition(PatientPCLRequestDetail reqDetails, PatientPCLRequest item)
        //{
        //    bool exists = false;
        //    if (item.PatientPCLRequestIndicators != null)
        //    {
        //        foreach (var row in item.PatientPCLRequestIndicators)
        //        {
        //            //neu = nhau thi tach phieu
        //            if (row.PCLExamType.PCLExamTypeID == reqDetails.PCLExamType.PCLExamTypeID)
        //            {
        //                exists = true;
        //                break;
        //            }
        //        }
        //        if (exists)
        //        {
        //            exists = false;
        //        }
        //        else
        //        {
        //            //o day chua kiem tra de tach theo phong
        //            if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.V_PCLMainCategory == reqDetails.PCLExamType.V_PCLMainCategory
        //                && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HITTypeID == reqDetails.PCLExamType.HITTypeID
        //                && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == reqDetails.PCLExamType.IsExternalExam)
        //            {
        //                if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == true)
        //                {
        //                    if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HosIDofExternalExam == reqDetails.PCLExamType.HosIDofExternalExam)
        //                    {
        //                        item.PatientPCLRequestIndicators.Add(reqDetails);
        //                        exists = true;
        //                    }
        //                }
        //                else
        //                {
        //                    item.PatientPCLRequestIndicators.Add(reqDetails);
        //                    exists = true;
        //                }
        //            }
        //        }
        //    }
        //    return exists;
        //}

        //private List<PatientPCLRequest> SplitVote(List<PatientPCLRequestDetail> lstDetail_CreateNew)
        //{
        //    var requests = new List<PatientPCLRequest>(); //new Dictionary<long ,PatientPCLRequest>();
        //    Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc = null;

        //    if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
        //    {
        //        MAPPCLExamTypeDeptLoc = aEMR.DataAccessLayer.Providers.DataProviderBase.MAPPCLExamTypeDeptLoc;
        //    }
        //    else
        //    {
        //        MAPPCLExamTypeDeptLoc = DataProviderBase.MAPPCLExamTypeDeptLoc;
        //    }

        //    foreach (var reqDetails in lstDetail_CreateNew)
        //    {
        //        reqDetails.ObjDeptLocIDList = MAPPCLExamTypeDeptLoc[reqDetails.PCLExamType.PCLExamTypeID].ObjDeptLocationList;

        //        if (reqDetails.DeptLocation == null)
        //        {
        //            reqDetails.DeptLocation = new DeptLocation();
        //        }

        //        bool exists = false;
        //        foreach (PatientPCLRequest item in requests)
        //        {
        //            exists = false;
        //            if (item.PCLDeptLocID == reqDetails.DeptLocation.DeptLocationID && item.PCLDeptLocID.GetValueOrDefault(0) > 0)
        //            {
        //                //neu duoc chon phong ngay tu dau
        //                exists = SplitVoteCondition(reqDetails, item);
        //            }
        //            else
        //            {
        //                PatientPCLRequestDetail OldItem = item.PatientPCLRequestIndicators.FirstOrDefault();
        //                if (OldItem.ObjDeptLocIDList != null && OldItem.ObjDeptLocIDList.Count > 0 && reqDetails.ObjDeptLocIDList != null && reqDetails.ObjDeptLocIDList.Count > 0)
        //                {
        //                    foreach (var item1 in OldItem.ObjDeptLocIDList)
        //                    {
        //                        foreach (var detail1 in reqDetails.ObjDeptLocIDList)
        //                        {
        //                            if (detail1.DeptLocationID == item1.DeptLocationID)
        //                            {
        //                                exists = true;
        //                                break;
        //                            }
        //                        }
        //                        if (exists)
        //                        {
        //                            //da ton tai,thi dua vao nhung dk khac de nhu V_Maincatelogy and HITypeID de tach phieu
        //                            exists = SplitVoteCondition(reqDetails, item);
        //                            break;
        //                        }
        //                    }
        //                }
        //            }

        //            if (exists)
        //            {
        //                break;
        //            }
        //        }
        //        if (!exists)
        //        {
        //            var newRequest = new PatientPCLRequest();
        //            //newRequest.PCLRequestNumID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.PCLRequestNumID : "";
        //            newRequest.PatientPCLReqID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.PatientPCLReqID : 0;
        //            newRequest.StaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.StaffID : 0;
        //            newRequest.DoctorStaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorStaffID : 0;
        //            newRequest.Diagnosis = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.Diagnosis : "";

        //            newRequest.DoctorComments = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorComments : "";
        //            newRequest.DeptID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DeptID : 0;
        //            newRequest.PCLDeptLocID = reqDetails.DeptLocation != null ? reqDetails.DeptLocation.DeptLocationID : 0;
        //            newRequest.ReqFromDeptID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ReqFromDeptID : 0;
        //            newRequest.ReqFromDeptLocID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ReqFromDeptLocID : 0;
        //            newRequest.RequestedDoctorName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.RequestedDoctorName : "";
        //            newRequest.ServiceRecID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ServiceRecID : 0;

        //            newRequest.V_PCLRequestStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatus : 0;
        //            newRequest.V_PCLRequestType = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestType : 0;
        //            newRequest.V_PCLRequestStatusName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatusName : "";
        //            newRequest.ExamRegStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamRegStatus : 0;
        //            newRequest.RecordState = RecordState.DETACHED;
        //            newRequest.EntityState = EntityState.NEW;
        //            newRequest.CreatedDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.CreatedDate : DateTime.Now;
        //            newRequest.ExamDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamDate : DateTime.Now;

        //            newRequest.PatientPCLRequestIndicators = new List<PatientPCLRequestDetail>().ToObservableCollection();
        //            newRequest.PatientPCLRequestIndicators.Add(reqDetails);
        //            newRequest.V_PCLMainCategory = reqDetails.PCLExamType.V_PCLMainCategory;
        //            newRequest.IsExternalExam = reqDetails.PCLExamType.IsExternalExam;
        //            newRequest.AgencyID = reqDetails.PCLExamType.HosIDofExternalExam;
        //            newRequest.PCLRequestNumID = new ServiceSequenceNumberProvider().GetCode_PCLRequest_InPt(newRequest.V_PCLMainCategory);

        //            requests.Add(newRequest);
        //        }
        //    }
        //    return requests;
        //}
        #endregion

        private IList<Lookup> GetLookupByType(LookupValues lookUpType)
        {
            string mainCacheKey = "LookupValues_" + ((int)lookUpType).ToString();
            List<Lookup> lookups;
            //Kiểm tra nếu có trong cache thì lấy từ trong cache.
            if (ServerAppConfig.CachingEnabled)
            {
                lookups = (List<Lookup>)AxCache.Current[mainCacheKey];
                if (lookups != null)
                {
                    return lookups;
                }
            }
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    lookups = aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllLookupsByType(lookUpType);
            //}
            //else
            //{
            //    lookups = CommonProvider.Lookups.GetAllLookupsByType(lookUpType);
            //}
            lookups = aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllLookupsByType(lookUpType);
            //Lưu vào cache để lần sau sử dụng.
            if (ServerAppConfig.CachingEnabled)
            {
                if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                {
                    AxCache.Current[mainCacheKey] = lookups;
                }
                else
                {
                    AxCache.Current.Insert(mainCacheKey, lookups, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                }
            }
            return lookups;
        }
        public IList<Lookup> GetAllLookupValuesByType(LookupValues lookUpType)
        {
            IList<Lookup> allLookups = null;
            try
            {
                allLookups = GetLookupByType(lookUpType);
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "CM.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
            return allLookups;
        }
        public IList<RefGenericDrugCategory_2> RefGenericDrugCategory_2_Get(long V_MedProductType)
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.RefGenericDrugCategory_2_Get(V_MedProductType);
            //}
            //else
            //{
            //    return CommonProvider.Countries.RefGenericDrugCategory_2_Get(V_MedProductType);
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.RefGenericDrugCategory_2_Get(V_MedProductType);
        }
        public IList<Gender> GetAllGenders()
        {
            List<Gender> genders = new List<Gender>();
            genders.Add(new Gender("M", "Nam"));
            genders.Add(new Gender("F", "Nữ"));
            genders.Add(new Gender("U", "Chưa xác định"));
            return genders;
        }
        public IList<RefCountry> GetAllCountries()
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.GetAllCountries();
            //}
            //else
            //{
            //    return CommonProvider.Countries.GetAllCountries();
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.GetAllCountries();
        }
        public IList<CitiesProvince> GetAllProvinces()
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.GetAllProvinces();
            //}
            //else
            //{
            //    return CommonProvider.Countries.GetAllProvinces();
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.GetAllProvinces();
        }
        public Stream GetVideoAndImage(string path)
        {
            return GetVideoAndImage_V2(path);
        }
        public Stream GetVideoAndImage_V2(string aPath, bool aMapPath = false)
        {
            try
            {
                if (aMapPath)
                {
                    string mServerPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
                    aPath = Path.Combine(mServerPath, aPath);
                }
                FileInfo mFileInfo = new FileInfo(aPath);
                Stream mStream = mFileInfo.OpenRead();
                return mStream;
            }
            catch
            {
                return new MemoryStream();
            }
        }
        public IList<RefDepartment> GetAllDepartments(bool bIncludeDeleted)
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllDepartments(bIncludeDeleted);
            //}
            //else
            //{
            //    return CommonProvider.Lookups.GetAllDepartments(bIncludeDeleted);
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllDepartments(bIncludeDeleted);
        }
        public AxServerConfigSection GetServerConfiguration()
        {
            return Globals.AxServerSettings;
        }
        public List<SuburbNames> GetAllSuburbNames()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.GetAllSuburbNames();
                //}
                //else
                //{
                //    return AppConfigsProvider.Instance.GetAllSuburbNames();
                //}
                return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.GetAllSuburbNames();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating UpdateRefApplicationConfigs. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateRefApplicationConfigs, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▼====== #002
        public List<WardNames> GetAllWardNames()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.GetAllWardNames();
                //}
                //else
                //{
                //    return AppConfigsProvider.Instance.GetAllWardNames();
                //}
                return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.GetAllWardNames();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Get All WardNames. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateRefApplicationConfigs, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #002
        public List<StaffPosition> GetAllStaffPosition()
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Staffs.GetAllStaffPosition();
            //}
            //else
            //{
            //    return CommonProvider.Staffs.GetAllStaffPosition();
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Staffs.GetAllStaffPosition();
        }
        public IList<RefDepartment> GetDepartments(long locationID)
        {
            try
            {
                string mainCacheKey = locationID.ToString() + "_Dept";
                List<RefDepartment> allDepartments;
                //Kiểm tra nếu có trong cache thì lấy từ trong cache.
                if (ServerAppConfig.CachingEnabled)
                {
                    allDepartments = (List<RefDepartment>)AxCache.Current[mainCacheKey];
                    if (allDepartments != null)
                    {
                        return allDepartments;
                    }
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allDepartments = aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetDepartments(locationID);
                //}
                //else
                //{
                //    allDepartments = CommonProvider.Lookups.GetDepartments(locationID);
                //}
                allDepartments = aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetDepartments(locationID);
                //Lưu vào cache để lần sau sử dụng.
                if (ServerAppConfig.CachingEnabled)
                {
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allDepartments;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allDepartments, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                    }
                }
                return allDepartments;
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }
        public IList<RefDepartment> GetAllDepartmentsByV_DeptTypeOperation(long V_DeptTypeOperation)
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllDepartmentsByV_DeptTypeOperation(V_DeptTypeOperation);
            //}
            //else
            //{
            //    return CommonProvider.Lookups.GetAllDepartmentsByV_DeptTypeOperation(V_DeptTypeOperation);
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllDepartmentsByV_DeptTypeOperation(V_DeptTypeOperation);
        }
        public IList<Lookup> GetAllDocumentTypeOnHold()
        {
            try
            {
                string mainCacheKey = "AllDocsOnHold";
                IList<Lookup> allDocs;
                //Kiểm tra nếu có trong cache thì lấy từ trong cache.
                if (ServerAppConfig.CachingEnabled)
                {
                    allDocs = (List<Lookup>)AxCache.Current[mainCacheKey];
                    if (allDocs != null)
                    {
                        return allDocs;
                    }
                }

                allDocs = GetLookupByType(LookupValues.DOCUMENT_TYPE_ON_HOLD);

                //Lưu vào cache để lần sau sử dụng.
                if (ServerAppConfig.CachingEnabled)
                {
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allDocs;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allDocs, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                    }
                }
                return allDocs;
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }
        public IList<Hospital> Hospital_ByCityProvinceID(long? CityProvinceID)
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.Hospital_ByCityProvinceID(CityProvinceID);
            //}
            //else
            //{
            //    return CommonProvider.Countries.Hospital_ByCityProvinceID(CityProvinceID);
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.Hospital_ByCityProvinceID(CityProvinceID);
        }
        public IList<RefGenericDrugCategory_1> RefGenericDrugCategory_1_Get(long V_MedProductType)
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.RefGenericDrugCategory_1_Get(V_MedProductType);
            //}
            //else
            //{
            //    return CommonProvider.Countries.RefGenericDrugCategory_1_Get(V_MedProductType);
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.RefGenericDrugCategory_1_Get(V_MedProductType);
        }
        public IList<DeptTransferDocReq> GetAllDocTypeRequire()
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllDocTypeRequire();
            //}
            //else
            //{
            //    return CommonProvider.Lookups.GetAllDocTypeRequire();
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllDocTypeRequire();
        }
        public List<PatientTransactionDetail> GetTransactionSum(long TransactionID)
        {
            try
            {
                List<PatientTransactionDetail> allTranDetails = new List<PatientTransactionDetail>();
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    allTranDetails = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetAlltransactionDetailsSum(TransactionID);
                }
                else
                {
                    allTranDetails = PatientProvider.Instance.GetAlltransactionDetailsSum(TransactionID);
                }
                foreach (var patientTransactionDetail in allTranDetails)
                {
                    if (patientTransactionDetail.outiID != null)
                    {
                        patientTransactionDetail.TransactionType = "Phiếu Xuất";
                    }
                    if (patientTransactionDetail.PCLRequestID != null)
                    {
                        patientTransactionDetail.TransactionType = "PCLRequest";
                    }
                    if (patientTransactionDetail.PtRegDetailID != null)
                    {
                        patientTransactionDetail.TransactionType = "Service";
                    }
                    if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.BILL_NOI_TRU)
                    {
                        patientTransactionDetail.TransactionType = "BILL NOI TRU";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NGOAITRU)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NGOAI TRU";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHOCUAKHOA)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NOI TRU KHO CUA KHOA";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHODUOC)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NOI TRU KHO DUOC";
                    }


                }
                return allTranDetails;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating GetTransactionSum. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetTransactionSum, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<PatientTransactionDetail> GetTransactionSum_InPt(long TransactionID)
        {
            try
            {
                List<PatientTransactionDetail> allTranDetails = new List<PatientTransactionDetail>();
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    allTranDetails = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetAlltransactionDetailsSum_InPt(TransactionID);
                }
                else
                {
                    allTranDetails = PatientProvider.Instance.GetAlltransactionDetailsSum_InPt(TransactionID);
                }
                foreach (var patientTransactionDetail in allTranDetails)
                {
                    if (patientTransactionDetail.outiID != null)
                    {
                        patientTransactionDetail.TransactionType = "Phiếu Xuất";
                    }
                    if (patientTransactionDetail.PCLRequestID != null)
                    {
                        patientTransactionDetail.TransactionType = "PCLRequest";
                    }
                    if (patientTransactionDetail.PtRegDetailID != null)
                    {
                        patientTransactionDetail.TransactionType = "Service";
                    }
                    if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.BILL_NOI_TRU)
                    {
                        patientTransactionDetail.TransactionType = "BILL NOI TRU";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NGOAITRU)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NGOAI TRU";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHOCUAKHOA)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NOI TRU KHO CUA KHOA";
                    }
                    else if (patientTransactionDetail.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHODUOC)
                    {
                        patientTransactionDetail.TransactionType = "DRUG NOI TRU KHO DUOC";
                    }


                }
                return allTranDetails;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating GetTransactionSum. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetTransactionSum, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<ReportOutPatientCashReceipt_Payments> GetMoreReportOutPatientCashReceipt_TongHop_Async(int RefAsyncKey, out List<PatientTransactionPayment> OutPaymentLst, out int AsyncKey)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetReportOutPatientCashReceipt_TongHop.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetMoreReportOutPatientCashReceipt_TongHop_Async(RefAsyncKey, out OutPaymentLst, out AsyncKey);
                //}
                //else
                //{
                //    return PaymentProvider.Instance.GetMoreReportOutPatientCashReceipt_TongHop_Async(RefAsyncKey, out OutPaymentLst, out AsyncKey);
                //}
                return aEMR.DataAccessLayer.Providers.PaymentProvider.Instance.GetMoreReportOutPatientCashReceipt_TongHop_Async(RefAsyncKey, out OutPaymentLst, out AsyncKey);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetReportOutPatientCashReceipt_TongHop. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool DoUpload(string filename, byte[] content, bool append, string dir)
        {
            string folder = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, dir));
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            FileMode m = FileMode.Create;
            if (append)
                m = FileMode.Append;

            using (FileStream fs = new FileStream(folder + @"\" + filename, m, FileAccess.Write))
            {
                fs.Write(content, 0, content.Length);
            }
            return true;
        }
        public bool DoListUpload(List<PCLResultFileStorageDetail> lst)
        {
            try
            {
                foreach (var item in lst)
                {
                    DoUpload(item.PCLResultFileName, item.File, false, item.FullPath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public IList<Currency> GetAllCurrency(bool? IsActive)
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.GetAllCurrency(IsActive);
            //}
            //else
            //{
            //    return CommonProvider.Countries.GetAllCurrency(IsActive);
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.GetAllCurrency(IsActive);
        }
        public IList<Lookup> GetAllEthnics()
        {
            return GetLookupByType(LookupValues.ETHNIC);
        }
        public IList<Lookup> GetAllReligion()
        {
            return GetLookupByType(LookupValues.RELIGION);
        }
        public IList<Lookup> GetAllMaritalStatus()
        {
            IList<Lookup> allLookups = null;
            try
            {
                allLookups = GetLookupByType(LookupValues.MARITAL_STATUS);
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "CM.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
            return allLookups;
        }
        public IList<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory()
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.LoadRefPharmacyDrugCategory();
            //}
            //else
            //{
            //    return CommonProvider.Countries.LoadRefPharmacyDrugCategory();
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.LoadRefPharmacyDrugCategory();
        }

        //▼===== 25072018 TTM
        public IList<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory_New()
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.LoadRefPharmacyDrugCategory_New();
            //}
            //else
            //{
            //    return CommonProvider.Countries.LoadRefPharmacyDrugCategory_New();
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Countries.LoadRefPharmacyDrugCategory_New();
        }
        //▼===== 25072018 TTM
        public IList<Staff> GetAllStaffContain()
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Staffs.GetAllStaffContain();
            //}
            //else
            //{
            //    return CommonProvider.Staffs.GetAllStaffContain();
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Staffs.GetAllStaffContain();
        }
        public IList<Lookup> GetAllBankName()
        {
            return GetLookupByType(LookupValues.BANK_NAME);
        }
        public bool SaveImageCapture(byte[] byteArray)
        {
            try
            {

                string imageFileName = DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-") + ".jpeg";
                FileStream f = new FileStream(@"\\AXSERVER01\ImageCapture\" + imageFileName, FileMode.Create);

                f.Write(byteArray, 0, byteArray.Length);
                f.Flush();
                f.Close();

            }
            catch
            {
                return false;
            }

            return true;
        }
        public bool SaveImageCapture1(MemoryStream byteArray)
        {
            try
            {

                string imageFileName = DateTime.Now.ToShortDateString().Replace(@"/", @"-").Replace(@"\", @"-") + ".jpeg";
                FileStream f = new FileStream(@"\\\\AXSERVER01\\ImageCapture\\" + imageFileName, FileMode.Create);

                f.Write(byteArray.ToArray(), 0, byteArray.ToArray().Length);
                f.Flush();
                f.Close();


            }
            catch
            {
                return false;
            }

            return true;
        }
        public IList<Lookup> GetAllLookupValuesForTransferForm(LookupValues lookUpType)
        {
            IList<Lookup> allLookups = null;
            try
            {
                allLookups = GetLookupByType(lookUpType);
            }
            catch
            {
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "CM.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
            return allLookups;
        }
        public IList<DrugDeptProductGroupReportType> GetDrugDeptProductGroupReportTypes()
        {
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetDrugDeptProductGroupReportTypes();
            //}
            //else
            //{
            //    return CommonProvider.Lookups.GetDrugDeptProductGroupReportTypes();
            //}
            return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetDrugDeptProductGroupReportTypes();
        }
        public long EditRefMedicalServiceGroup(RefMedicalServiceGroups aRefMedicalServiceGroup)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.EditRefMedicalServiceGroup(aRefMedicalServiceGroup);
                //}
                //else
                //{
                //    return CommonProvider_V2.Instance.EditRefMedicalServiceGroup(aRefMedicalServiceGroup);
                //}
                return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.EditRefMedicalServiceGroup(aRefMedicalServiceGroup);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all EditRefMedicalServiceGroups. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<RefMedicalServiceGroups> GetRefMedicalServiceGroups(string MedicalServiceGroupCode)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.GetRefMedicalServiceGroups(MedicalServiceGroupCode);
                //}
                //else
                //{
                //    return CommonProvider_V2.Instance.GetRefMedicalServiceGroups(MedicalServiceGroupCode);
                //}
                return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.GetRefMedicalServiceGroups(MedicalServiceGroupCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all EditRefMedicalServiceGroups. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<RefMedicalServiceGroupItem> GetRefMedicalServiceGroupItemsByID(long MedicalServiceGroupID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.GetRefMedicalServiceGroupItemsByID(MedicalServiceGroupID);
                //}
                //else
                //{
                //    return CommonProvider_V2.Instance.GetRefMedicalServiceGroupItemsByID(MedicalServiceGroupID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.GetRefMedicalServiceGroupItemsByID(MedicalServiceGroupID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all EditRefMedicalServiceGroups. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<ShortHandDictionary> GetShortHandDictionariesByStaffID(long StaffID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.GetShortHandDictionariesByStaffID(StaffID);
                //}
                //else
                //{
                //    return CommonProvider_V2.Instance.GetShortHandDictionariesByStaffID(StaffID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.GetShortHandDictionariesByStaffID(StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetShortHandDictionariesByStaffID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool UpdateOrderNumberRegistration(List<string> orderNumLst)
        {
            if (null == orderNumLst || orderNumLst.Count < 1)
            {
                return true;
            }
            try
            {
                long PtRegDetailID;
                long PCLReqItemID;
                long PrescriptID;
                long OrderNumber;
                long RoomId;
                foreach (string orderNumInfo in orderNumLst)
                {
                    if (string.IsNullOrEmpty(orderNumInfo))
                    {
                        continue;
                    }
                    string[] info = orderNumInfo.Split("-".ToCharArray());
                    if (string.IsNullOrEmpty(info[1]) || string.IsNullOrEmpty(info[2]))
                    {
                        continue;
                    }

                    PtRegDetailID = 0;
                    PCLReqItemID = 0;
                    PrescriptID = 0;
                    OrderNumber = 0;
                    RoomId = 0;
                    switch (info[0])
                    {
                        case "PCL":
                            PCLReqItemID = long.Parse(info[1]);
                            break;
                        case "REG":
                            PtRegDetailID = long.Parse(info[1]);
                            break;
                        case "PRE":
                            PrescriptID = long.Parse(info[1]);
                            break;
                        default:
                            break;
                    }

                    OrderNumber = long.Parse(info[2]);
                    if (!string.IsNullOrEmpty(info[3]))
                    {
                        RoomId = long.Parse(info[3]);
                    }

                    if ((0 == PtRegDetailID && 0 == PCLReqItemID && 0 == PrescriptID)
                        || 0 == OrderNumber)
                    {
                        continue;
                    }

                    aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance
                        .UpdateOrderNumberRegistration(PtRegDetailID, PCLReqItemID, PrescriptID, OrderNumber, RoomId);
                }

                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all UpdateOrderNumberRegistration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #region Quản lý điều dưỡng thực hiện y lệnh
        //▼====: #005
        public TicketCare SaveTicketCare(TicketCare gTicketCare, long CreatedStaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveTicketCare.", CurrentUser);
                return PatientProvider.Instance.SaveTicketCare(gTicketCare, CreatedStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveTicketCare. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public TicketCare GetTicketCare(long IntPtDiagDrInstructionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetTicketCare.", CurrentUser);
                return PatientProvider.Instance.GetTicketCare(IntPtDiagDrInstructionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetTicketCare. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<TicketCare> GetTicketCareListForRegistration(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetTicketCareForRegistration.", CurrentUser);
                return PatientProvider.Instance.GetTicketCareListForRegistration(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetTicketCareForRegistration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<ExecuteDrug> GetExecuteDrugListForRegistration(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetExecuteDrugForRegistration.", CurrentUser);
                return PatientProvider.Instance.GetExecuteDrugListForRegistration(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetExecuteDrugForRegistration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool SaveExecuteDrug(long ExecuteDrugID, long ExecuteDrugDetailID, long StaffID, long CreatedStaffID, DateTime DateExecute)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveExecuteDrug.", CurrentUser);
                return PatientProvider.Instance.SaveExecuteDrug(ExecuteDrugID, ExecuteDrugDetailID, StaffID, CreatedStaffID, DateExecute);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveExecuteDrug. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteExecuteDrug(long ExecuteDrugDetailID, long CreatedStaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start DeleteExecuteDrug.", CurrentUser);
                return PatientProvider.Instance.DeleteExecuteDrug(ExecuteDrugDetailID, CreatedStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteExecuteDrug. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #005
        #endregion

        public IList<DiseaseProgression> GetAllDiseaseProgression(bool UseUseInConfig)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.GetAllDiseaseProgression(UseUseInConfig);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all Get All Disease Progression. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<DiseaseProgressionDetails> GetAllDiseaseProgressionDetails()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.GetAllDiseaseProgressionDetails();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all Get All Disease Progression details. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool UpdateIsBlockBill(bool IsBlock, long PtRegistrationID, long DeptID)
        {
            try
            {
                return CommonProvider_V2.Instance.UpdateIsBlockBill(IsBlock, PtRegistrationID, DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateIsBlockBill. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼==== #006
        public IList<RefNationality> GetAllNationalities()
        {            
            return CommonProvider.Countries.GetAllNationalities();
        }

        public List<RefJob> GetAllJobs()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.AppConfigsProvider.Instance.GetAllJobs();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Get All Job. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateRefApplicationConfigs, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #006

        //▼==== #007
        public IList<Lookup> GetAllLookupValuesByTypeForMngt(LookupValues lookUpType, string SearchCriteria)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllLookupsByType_V2(lookUpType, SearchCriteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Get All Lookup. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateRefApplicationConfigs, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }            
        }

        public IList<LookupTree> GetTreeView_LookupForMngt()
        {
            try
            {
                IList<Lookup> familyLookups = aEMR.DataAccessLayer.Providers.CommonProvider.Lookups.GetAllLookupsByType_V2(0, null);
                List<LookupTree> results = new List<LookupTree>();

                var listParent = familyLookups.Where(x => x.ObjectNotes != "" && x.ObjectNotes != null)
                    .GroupBy(x => x.ObjectTypeID)
                    .Select(g => new Lookup
                    {
                        ObjectTypeID = g.Key,
                        ObjectValue = g.FirstOrDefault().ObjectNotes
                    }).ToObservableCollection();

                foreach(Lookup itemParent in listParent)
                {
                    LookupTree genericItem = new LookupTree(
                        itemParent.ObjectValue, itemParent.ObjectTypeID, 0, null, null,
                        familyLookups.FirstOrDefault(x => x.ObjectTypeID == itemParent.ObjectTypeID && x.ObjectNotes != null && x.ObjectNotes != "")
                    );
                    results.Add(genericItem);
                }

                //foreach (LookupTree node in list)
                //{
                //    foreach(Lookup children in familyLookups.Where(x => x.ObjectTypeID == node.NodeID))
                //    {
                //        node.Children.Add(children);
                //    }
                //    results.Add(node);
                //}
                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyTherapies. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_FOUND);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //private void FindChildrenLookup(LookupTree item)
        //{
        //    try
        //    {
        //        var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);
        //        foreach (LookupTree child in children)
        //        {
        //            item.Children.Add(child);
        //            FindChildrenLookup(child);
        //        }
        //    }
        //    catch
        //    { }
        //}
        //▲==== #007
    }
}
//▼====: #001
namespace LabSoftService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LabSoftService : eHCMS.WCFServiceCustomHeader, ILabSoftService
    {
        //private string _ModuleName = "LabSoft Module";
        public LabSoftService() {}

        public List<LIS_Staff> GetBacSi()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all GetBacSi.", CurrentUser);
                List<LIS_Staff> AllCategories = new List<LIS_Staff>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Doctors).Select(x => x as LIS_Staff).ToList();
                //}
                //else
                //{
                //    AllCategories = CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Doctors).Select(x => x as LIS_Staff).ToList();
                //}
                AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Doctors).Select(x => x as LIS_Staff).ToList();
                AxLogger.Instance.LogInfo("End of loading all GetBacSi. Status: Success.", CurrentUser);
                return AllCategories;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetBacSi. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<LIS_Department> GetKhoaPhong()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all GetKhoaPhong.", CurrentUser);
                List<LIS_Department> AllCategories = new List<LIS_Department>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Departments).Select(x => x as LIS_Department).ToList();
                //}
                //else
                //{
                //    AllCategories = CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Departments).Select(x => x as LIS_Department).ToList();
                //}
                AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Departments).Select(x => x as LIS_Department).ToList();
                AxLogger.Instance.LogInfo("End of loading all GetKhoaPhong. Status: Success.", CurrentUser);
                return AllCategories;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetKhoaPhong. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<LIS_Object> GetDoiTuong()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all GetDoiTuong.", CurrentUser);
                List<LIS_Object> AllCategories = new List<LIS_Object>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Objects).Select(x => x as LIS_Object).ToList();
                //}
                //else
                //{
                //    AllCategories = CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Objects).Select(x => x as LIS_Object).ToList();
                //}
                AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Objects).Select(x => x as LIS_Object).ToList();
                AxLogger.Instance.LogInfo("End of loading all GetDoiTuong. Status: Success.", CurrentUser);
                return AllCategories;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetDoiTuong. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<LIS_PCLItem> GetXetNghiem()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all GetXetNghiem.", CurrentUser);
                List<LIS_PCLItem> AllCategories = new List<LIS_PCLItem>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.PCLItems).Select(x => x as LIS_PCLItem).ToList();
                //}
                //else
                //{
                //    AllCategories = CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.PCLItems).Select(x => x as LIS_PCLItem).ToList();
                //}
                AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.PCLItems).Select(x => x as LIS_PCLItem).ToList();
                AxLogger.Instance.LogInfo("End of loading all GetXetNghiem. Status: Success.", CurrentUser);
                return AllCategories;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetXetNghiem. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<LIS_Device> GetThietBi()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all GetThietBi.", CurrentUser);
                List<LIS_Device> AllCategories = new List<LIS_Device>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Devices).Select(x => x as LIS_Device).ToList();
                //}
                //else
                //{
                //    AllCategories = CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Devices).Select(x => x as LIS_Device).ToList();
                //}
                AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Devices).Select(x => x as LIS_Device).ToList();
                AxLogger.Instance.LogInfo("End of loading all GetThietBi. Status: Success.", CurrentUser);
                return AllCategories;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetThietBi. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<LIS_User> GetNguoiDung()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all GetNguoiDung.", CurrentUser);
                List<LIS_User> AllCategories = new List<LIS_User>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Users).Select(x => x as LIS_User).ToList();
                //}
                //else
                //{
                //    AllCategories = CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Users).Select(x => x as LIS_User).ToList();
                //}
                AllCategories = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetCategories(DataEntities.eLabSoftCategory.Users).Select(x => x as LIS_User).ToList();
                AxLogger.Instance.LogInfo("End of loading all GetNguoiDung. Status: Success.", CurrentUser);
                return AllCategories;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetNguoiDung. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<LIS_PCLRequest> GetDanhSachChiDinh(string TuNgay, string DenNgay, int TrangThai)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all GetDanhSachChiDinh.", CurrentUser);
                List<LIS_PCLRequest> AllPCLRequests = new List<LIS_PCLRequest>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    AllPCLRequests = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_PatientList(TuNgay, DenNgay, TrangThai);
                //}
                //else
                //{
                //    AllPCLRequests = CommonProvider_V2.Instance.LIS_PatientList(TuNgay, DenNgay, TrangThai);
                //}
                AllPCLRequests = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_PatientList(TuNgay, DenNgay, TrangThai);
                AxLogger.Instance.LogInfo("End of loading all GetDanhSachChiDinh. Status: Success.", CurrentUser);
                return AllPCLRequests;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetDanhSachChiDinh. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<LIS_PCLRequest> GetChiDinh(string SoPhieuChiDinh, int TrangThai)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all GetChiDinh.", CurrentUser);
                List<LIS_PCLRequest> AllPCLRequests = new List<LIS_PCLRequest>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    AllPCLRequests = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_Order(SoPhieuChiDinh, TrangThai);
                //}
                //else
                //{
                //    AllPCLRequests = CommonProvider_V2.Instance.LIS_Order(SoPhieuChiDinh, TrangThai);
                //}
                AllPCLRequests = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_Order(SoPhieuChiDinh, TrangThai);
                AxLogger.Instance.LogInfo("End of loading all GetChiDinh. Status: Success.", CurrentUser);
                return AllPCLRequests;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetChiDinh. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool CapNhatTrangThaiNhanMau(string SoPhieuChiDinh, string MaDichVu, int TrangThai, string NgayTiepNhan, out string TrangThaiCapNhat)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all CapNhatTrangThaiNhanMau.", CurrentUser);
                bool UpdateStatus;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    UpdateStatus = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_UpdateOrderStatus(SoPhieuChiDinh, MaDichVu, TrangThai, NgayTiepNhan, out TrangThaiCapNhat);
                //}
                //else
                //{
                //    UpdateStatus = CommonProvider_V2.Instance.LIS_UpdateOrderStatus(SoPhieuChiDinh, MaDichVu, TrangThai, NgayTiepNhan, out TrangThaiCapNhat);
                //}
                UpdateStatus = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_UpdateOrderStatus(SoPhieuChiDinh, MaDichVu, TrangThai, NgayTiepNhan, out TrangThaiCapNhat);
                AxLogger.Instance.LogInfo("End of loading all CapNhatTrangThaiNhanMau. Status: Success.", CurrentUser);
                return UpdateStatus;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all CapNhatTrangThaiNhanMau. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼====: #004
        public bool NhanKetQuaXetNghiem(string SoPhieuChiDinh, string MaDichVu, string KetQua, string CSBT
            , string DonViTinh, bool BatThuong, int TrangThai, string MaNV_XacNhan
            , string ThoiGianXacNhan, string MaThietBi, string SampleCode)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all NhanKetQuaXetNghiem.", CurrentUser);
                bool UpdateStatus;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    UpdateStatus = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_Result(SoPhieuChiDinh, MaDichVu, KetQua, CSBT, DonViTinh, BatThuong, TrangThai, MaNV_XacNhan, ThoiGianXacNhan, MaThietBi);
                //}
                //else
                //{
                //    UpdateStatus = CommonProvider_V2.Instance.LIS_Result(SoPhieuChiDinh, MaDichVu, KetQua, CSBT, DonViTinh, BatThuong, TrangThai, MaNV_XacNhan, ThoiGianXacNhan, MaThietBi);
                //}
                UpdateStatus = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_Result(SoPhieuChiDinh, MaDichVu, KetQua, CSBT, DonViTinh, BatThuong, TrangThai, MaNV_XacNhan, ThoiGianXacNhan, MaThietBi, SampleCode);
                AxLogger.Instance.LogInfo("End of loading all NhanKetQuaXetNghiem. Status: Success.", CurrentUser);
                return UpdateStatus;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all NhanKetQuaXetNghiem. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #004

        public List<LIS_PCLRequest> KiemKetQuaXetNghiem(string SoPhieuChiDinh)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all KiemKetQuaXetNghiem.", CurrentUser);
                List<LIS_PCLRequest> AllPCLRequests = new List<LIS_PCLRequest>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    AllPCLRequests = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetPCLResult(SoPhieuChiDinh);
                //}
                //else
                //{
                //    AllPCLRequests = CommonProvider_V2.Instance.LIS_GetPCLResult(SoPhieuChiDinh);
                //}
                AllPCLRequests = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.LIS_GetPCLResult(SoPhieuChiDinh);
                AxLogger.Instance.LogInfo("End of loading all KiemKetQuaXetNghiem. Status: Success.", CurrentUser);
                return AllPCLRequests;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all KiemKetQuaXetNghiem. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
    }
}
//▲====: #001
namespace RISService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RISService : eHCMS.WCFServiceCustomHeader, IRISService
    {
        CommonService_V2.CommonService_V2 gCommonService_V2 = new CommonService_V2.CommonService_V2();
        private Dictionary<long, List<Resources>> ResourceCollectionCache = new Dictionary<long, List<Resources>>();
        public APIPatientPCLRequest GetPCLRequestDetail(string PCLRequestNumID)
        {
            try
            {
                int mTotalRecord = 0;
                var mPatientPCLRequest = PCLsProvider.Instance.PatientPCLRequest_SearchPaging(new PatientPCLRequestSearchCriteria
                {
                    PCLRequestNumID = PCLRequestNumID,
                    V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging,
                    FromDate = DateTime.Now,
                    ToDate = DateTime.Now
                }, 0, 1, null, false, out mTotalRecord);
                if (mPatientPCLRequest == null || mPatientPCLRequest.Count == 0)
                {
                    return null;
                }
                string ResourceCodeArray = null;
                if (mPatientPCLRequest[0].PCLResultParamImpID.HasValue && mPatientPCLRequest[0].PCLResultParamImpID > 0)
                {
                    List<Resources> mResourceCollection = new List<Resources>();
                    if (ResourceCollectionCache.ContainsKey(mPatientPCLRequest[0].PCLResultParamImpID.Value))
                    {
                        mResourceCollection = ResourceCollectionCache[mPatientPCLRequest[0].PCLResultParamImpID.Value];
                    }
                    else
                    {
                        mResourceCollection = PCLsProvider.Instance.GetResourcesForMedicalServicesListByTypeID(mPatientPCLRequest[0].PCLResultParamImpID.Value);
                        ResourceCollectionCache.Add(mPatientPCLRequest[0].PCLResultParamImpID.Value, mResourceCollection);
                    }
                    if (mResourceCollection != null && mResourceCollection.Count > 0)
                    {
                        ResourceCodeArray = string.Join(";", mResourceCollection.Select(x => x.HIRepResourceCode));
                    }
                }
                byte[] mDefaultTemplateFile = null;
                var ImageResult = PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByID(mPatientPCLRequest[0].PatientPCLReqID, !mPatientPCLRequest[0].PCLExamTypeID.HasValue ? 0 : mPatientPCLRequest[0].PCLExamTypeID.Value, (long)mPatientPCLRequest[0].V_PCLRequestType);
                var ContentFilePath = ImageResult == null ? null : ImageResult.TemplateResultString;
                if (string.IsNullOrEmpty(ContentFilePath) && mPatientPCLRequest[0].TemplateFileName.Split(';').Length == 1)
                {
                    ContentFilePath = Path.Combine(Globals.AxServerSettings.CommonItems.ReportTemplatesLocation, mPatientPCLRequest[0].TemplateFileName);
                }
                mPatientPCLRequest[0].TemplateFileName = ImageResult == null ? null : ImageResult.TemplateResultString;
                if (!string.IsNullOrEmpty(ContentFilePath))
                {
                    var mStream = gCommonService_V2.GetVideoAndImage(ContentFilePath);
                    if (mStream != null && mStream.Length > 0)
                    {
                        using (var aMemoryStream = new MemoryStream())
                        {
                            mStream.CopyTo(aMemoryStream);
                            mDefaultTemplateFile = aMemoryStream.ToArray();
                            aMemoryStream.Close();
                        }
                    }
                    mStream.Close();
                }
                var mAPIPatientPCLRequest = new APIPatientPCLRequest
                {
                    Diagnosis = mPatientPCLRequest[0].Diagnosis,
                    ChargeableItemName = mPatientPCLRequest[0].ChargeableItemName,
                    V_ExamRegStatusName = mPatientPCLRequest[0].V_ExamRegStatusName,
                    V_PCLRequestStatusName = mPatientPCLRequest[0].V_PCLRequestStatusName,
                    TemplateFileName = mPatientPCLRequest[0].TemplateFileName,
                    ResourceCodeArray = ResourceCodeArray,
                    DefaultTemplateFile = mDefaultTemplateFile,
                    ServerPublicAddress = Globals.AxServerSettings.CommonItems.ServerPublicAddress,
                    V_PCLRequestType = (long)mPatientPCLRequest[0].V_PCLRequestType,
                    PatientPCLReqID = mPatientPCLRequest[0].PatientPCLReqID,
                    PCLExamTypeID = !mPatientPCLRequest[0].PCLExamTypeID.HasValue || mPatientPCLRequest[0].PCLExamTypeID.Value == 0 ? 0 : mPatientPCLRequest[0].PCLExamTypeID.Value,
                    RequestedDoctorName = mPatientPCLRequest[0].RequestedDoctorName,
                    ReqFromDeptLocIDName = mPatientPCLRequest[0].ReqFromDeptLocIDName,
                    MedicalInstructionDate = mPatientPCLRequest[0].MedicalInstructionDate,
                    ResultDate = mPatientPCLRequest[0].ResultDate,
                    PCLRequestNumID = mPatientPCLRequest[0].PCLRequestNumID,
                    PCLExamTypeName = mPatientPCLRequest[0].PCLExamTypeName
                };
                if (mPatientPCLRequest[0].PatientID > 0)
                {
                    mPatientPCLRequest[0].Patient = PatientProvider.Instance.GetPatientByID(mPatientPCLRequest[0].PatientID);
                }
                if (mPatientPCLRequest[0].Patient != null)
                {
                    mAPIPatientPCLRequest.Patient = new APIPatient
                    {
                        PatientCode = mPatientPCLRequest[0].Patient.PatientCode,
                        FullName = mPatientPCLRequest[0].Patient.FullName,
                        AgeString = mPatientPCLRequest[0].Patient.AgeString,
                        MonthsOld = mPatientPCLRequest[0].Patient.MonthsOld,
                        GenderString = mPatientPCLRequest[0].Patient.GenderString,
                        PatientFullStreetAddress = mPatientPCLRequest[0].Patient.PatientFullStreetAddress
                    };
                }
                if (ImageResult != null)
                {
                    mAPIPatientPCLRequest.PCLRequestResult = new APIPatientPCLRequestResult
                    {
                        Suggest = ImageResult.Suggest,
                        PtRegistrationCode = ImageResult.PtRegistrationCode,
                        PerformStaffFullName = ImageResult.PerformStaffFullName
                    };
                }
                return mAPIPatientPCLRequest;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetPCLRequestDetail. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<APIStaff> GetAllStaffs()
        {
            try
            {
                var AllStaffs = CommonUtilsProvider.Instance.GetAllStaffs();
                if (AllStaffs == null || AllStaffs.Count == 0)
                {

                }
                return AllStaffs.Select(x => new APIStaff { StaffID = x.StaffID, FullName = x.FullName }).ToList();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all GetAllStaffs. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddPCLResultFileStorageDetails(APIPatientPCLRequest aAPIPatientPCLRequest, long StaffID, string DiagnoseOnPCLExam
            , string HIRepResourceCode
            , string TemplateResultString
            , string TemplateResultDescription
            , string TemplateResult
            , long PerformStaffID
            , string Suggest)
        {
            try
            {
                bool PCLExamForOutPatient = true;
                if (aAPIPatientPCLRequest.V_PCLRequestType == (long)AllLookupValues.V_PCLRequestType.NOI_TRU)
                {
                    PCLExamForOutPatient = false;
                }
                PatientPCLImagingResult ImagingResult = new PatientPCLImagingResult
                {
                    PCLExamDate = DateTime.Now,
                    PerformedDate = DateTime.Now,
                    StaffID = StaffID,
                    PatientPCLReqID = aAPIPatientPCLRequest.PatientPCLReqID,
                    PCLExamTypeID = aAPIPatientPCLRequest.PCLExamTypeID,
                    DiagnoseOnPCLExam = DiagnoseOnPCLExam,
                    HIRepResourceCode = HIRepResourceCode,
                    TemplateResultFileName = aAPIPatientPCLRequest.TemplateFileName,
                    TemplateResultString = TemplateResultString,
                    TemplateResultDescription = TemplateResultDescription,
                    TemplateResult = TemplateResult,
                    PerformStaffID = PerformStaffID,
                    Suggest = Suggest,
                    PCLExamForOutPatient = PCLExamForOutPatient,
                    PatientPCLRequest = new PatientPCLRequest
                    {
                        V_PCLRequestType = aAPIPatientPCLRequest.V_PCLRequestType == (long)AllLookupValues.V_PCLRequestType.NOI_TRU ? AllLookupValues.V_PCLRequestType.NOI_TRU : AllLookupValues.V_PCLRequestType.NGOAI_TRU
                    }
                };
                PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, new List<PCLResultFileStorageDetail>(), new List<PCLResultFileStorageDetail>(), new List<PCLResultFileStorageDetail>(), Globals.AxServerSettings.Hospitals.PCLStorePool);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all AddPCLResultFileStorageDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllPayments, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public DateTime GetDate()
        {
            return gCommonService_V2.GetDate();
        }
    }
}

//▼====: #003
namespace PACService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PACService : eHCMS.WCFServiceCustomHeader, IPACService
    {
        CommonService_V2.CommonService_V2 gCommonService_V2 = new CommonService_V2.CommonService_V2();
        private Dictionary<long, List<Resources>> ResourceCollectionCache = new Dictionary<long, List<Resources>>();
        public APIPatientPCLRequest GetPCLRequestDetail(string PCLRequestNumID)
        {
            try
            {
                int mTotalRecord = 0;
                var mPatientPCLRequest = PCLsProvider.Instance.PatientPCLRequest_SearchPaging(new PatientPCLRequestSearchCriteria
                {
                    PCLRequestNumID = PCLRequestNumID,
                    V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging,
                    FromDate = DateTime.Now.AddDays(-1),
                    ToDate = DateTime.Now
                }, 0, 1, null, false, out mTotalRecord);
                if (mPatientPCLRequest == null || mPatientPCLRequest.Count == 0)
                {
                    return null;
                }
                string ResourceCodeArray = null;
                if (mPatientPCLRequest[0].PCLResultParamImpID.HasValue && mPatientPCLRequest[0].PCLResultParamImpID > 0)
                {
                    List<Resources> mResourceCollection = new List<Resources>();
                    if (ResourceCollectionCache.ContainsKey(mPatientPCLRequest[0].PCLResultParamImpID.Value))
                    {
                        mResourceCollection = ResourceCollectionCache[mPatientPCLRequest[0].PCLResultParamImpID.Value];
                    }
                    else
                    {
                        mResourceCollection = PCLsProvider.Instance.GetResourcesForMedicalServicesListByTypeID(mPatientPCLRequest[0].PCLResultParamImpID.Value);
                        ResourceCollectionCache.Add(mPatientPCLRequest[0].PCLResultParamImpID.Value, mResourceCollection);
                    }
                    if (mResourceCollection != null && mResourceCollection.Count > 0)
                    {
                        ResourceCodeArray = string.Join(";", mResourceCollection.Select(x => x.HIRepResourceCode));
                    }
                }
                byte[] mDefaultTemplateFile = null;
                var ImageResult = PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByID(mPatientPCLRequest[0].PatientPCLReqID, !mPatientPCLRequest[0].PCLExamTypeID.HasValue ? 0 : mPatientPCLRequest[0].PCLExamTypeID.Value, (long)mPatientPCLRequest[0].V_PCLRequestType);
                var ContentFilePath = ImageResult == null ? null : ImageResult.TemplateResultString;
                //if (mPatientPCLRequest[0].TemplateFileName == null)
                //{
                //    throw new Exception("CLS chưa cấu hình mẫu trả kết quả!. Vui long liên hệ IT để cập nhật!");
                //}
                if (string.IsNullOrEmpty(ContentFilePath) && mPatientPCLRequest[0].TemplateFileName != null  && mPatientPCLRequest[0].TemplateFileName.Split(';').Length == 1)
                {
                    ContentFilePath = Path.Combine(Globals.AxServerSettings.CommonItems.ReportTemplatesLocation, mPatientPCLRequest[0].TemplateFileName);
                }
                mPatientPCLRequest[0].TemplateFileName = ImageResult == null ? null : ImageResult.TemplateResultString;
                if (!string.IsNullOrEmpty(ContentFilePath))
                {
                    var mStream = gCommonService_V2.GetVideoAndImage(ContentFilePath);
                    if (mStream != null && mStream.Length > 0)
                    {
                        using (var aMemoryStream = new MemoryStream())
                        {
                            mStream.CopyTo(aMemoryStream);
                            mDefaultTemplateFile = aMemoryStream.ToArray();
                            aMemoryStream.Close();
                        }
                    }
                    mStream.Close();
                }
                var mAPIPatientPCLRequest = new APIPatientPCLRequest
                {
                    Diagnosis = mPatientPCLRequest[0].Diagnosis,
                    ChargeableItemName = mPatientPCLRequest[0].ChargeableItemName,
                    V_ExamRegStatusName = mPatientPCLRequest[0].V_ExamRegStatusName,
                    V_PCLRequestStatusName = mPatientPCLRequest[0].V_PCLRequestStatusName,
                    TemplateFileName = mPatientPCLRequest[0].TemplateFileName,
                    ResourceCodeArray = ResourceCodeArray,
                    DefaultTemplateFile = mDefaultTemplateFile,
                    ServerPublicAddress = Globals.AxServerSettings.CommonItems.ServerPublicAddress,
                    V_PCLRequestType = (long)mPatientPCLRequest[0].V_PCLRequestType,
                    PatientPCLReqID = mPatientPCLRequest[0].PatientPCLReqID,
                    PCLExamTypeID = !mPatientPCLRequest[0].PCLExamTypeID.HasValue || mPatientPCLRequest[0].PCLExamTypeID.Value == 0 ? 0 : mPatientPCLRequest[0].PCLExamTypeID.Value,
                    RequestedDoctorName = mPatientPCLRequest[0].RequestedDoctorName,
                    ReqFromDeptLocIDName = mPatientPCLRequest[0].ReqFromDeptLocIDName,
                    MedicalInstructionDate = mPatientPCLRequest[0].MedicalInstructionDate,
                    ResultDate = mPatientPCLRequest[0].ResultDate,
                    PCLRequestNumID = mPatientPCLRequest[0].PCLRequestNumID,
                    PCLExamTypeName = mPatientPCLRequest[0].PCLExamTypeName
                };
                if (mPatientPCLRequest[0].PatientID > 0)
                {
                    mPatientPCLRequest[0].Patient = PatientProvider.Instance.GetPatientByID(mPatientPCLRequest[0].PatientID);
                }
                if (mPatientPCLRequest[0].Patient != null)
                {
                    mAPIPatientPCLRequest.Patient = new APIPatient
                    {
                        PatientCode = mPatientPCLRequest[0].Patient.PatientCode,
                        FullName = mPatientPCLRequest[0].Patient.FullName,
                        AgeString = mPatientPCLRequest[0].Patient.AgeString,
                        MonthsOld = mPatientPCLRequest[0].Patient.MonthsOld,
                        GenderString = mPatientPCLRequest[0].Patient.GenderString,
                        PatientFullStreetAddress = mPatientPCLRequest[0].Patient.PatientFullStreetAddress
                    };
                }
                if (ImageResult != null)
                {
                    mAPIPatientPCLRequest.PCLRequestResult = new APIPatientPCLRequestResult
                    {
                        Suggest = ImageResult.Suggest,
                        PtRegistrationCode = ImageResult.PtRegistrationCode,
                        PerformStaffFullName = ImageResult.PerformStaffFullName
                    };
                }
                return mAPIPatientPCLRequest;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all API_PAC_GetPCLRequestDetail. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_API_PAC_GetPCLRequestDetail, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<APIStaff> GetAllStaffs()
        {
            try
            {
                var AllStaffs = CommonProvider_V2.Instance.PAC_GetListDoctors();
                if (AllStaffs == null || AllStaffs.Count == 0)
                {

                }
                return AllStaffs.Select(x => new APIStaff { StaffID = x.StaffID, FullName = x.FullName, UserDomain = x.UserDomain }).ToList();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all API_PAC_GetAllStaffs. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_API_PAC_GetAllStaffs, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool AddPCLResultFileStorageDetails(long PatientPCLReqID, long PCLExamTypeID, long V_PCLRequestType
            , long StaffID, string DiagnoseOnPCLExam
            , string HIRepResourceCode
            , string TemplateResultString
            , string TemplateResultDescription
            , string TemplateResult
            , long PerformStaffID
            , string Suggest)
        {
            try
            {
                bool PCLExamForOutPatient = true;
                if (V_PCLRequestType == (long)AllLookupValues.V_PCLRequestType.NOI_TRU)
                {
                    PCLExamForOutPatient = false;
                }
                PatientPCLImagingResult ImagingResult = new PatientPCLImagingResult
                {
                    PCLExamDate = DateTime.Now,
                    PerformedDate = DateTime.Now,
                    StaffID = StaffID,
                    PatientPCLReqID = PatientPCLReqID,
                    PCLExamTypeID = PCLExamTypeID,
                    DiagnoseOnPCLExam = DiagnoseOnPCLExam,
                    HIRepResourceCode = HIRepResourceCode,
                    //TemplateResultFileName = aAPIPatientPCLRequest.TemplateFileName,
                    TemplateResultString = TemplateResultString,
                    TemplateResultDescription = TemplateResultDescription,
                    TemplateResult = TemplateResult,
                    PerformStaffID = PerformStaffID,
                    Suggest = Suggest,
                    PCLExamForOutPatient = PCLExamForOutPatient,
                    PatientPCLRequest = new PatientPCLRequest
                    {
                        V_PCLRequestType = V_PCLRequestType == (long)AllLookupValues.V_PCLRequestType.NOI_TRU ? AllLookupValues.V_PCLRequestType.NOI_TRU : AllLookupValues.V_PCLRequestType.NGOAI_TRU
                    }
                };
                PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, new List<PCLResultFileStorageDetail>(), new List<PCLResultFileStorageDetail>(), new List<PCLResultFileStorageDetail>(), Globals.AxServerSettings.Hospitals.PCLStorePool, true);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all API_PAC_AddPCLResultFileStorageDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_API_PAC_AddPCLResultFileStorageDetails, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public DateTime GetDate()
        {
            return gCommonService_V2.GetDate();
        }     
    }
}
//▲====: #003
