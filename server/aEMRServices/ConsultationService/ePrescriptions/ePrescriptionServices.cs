using System;
using System.Collections.Generic;

using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;

using ErrorLibrary;
using System.Runtime.Serialization;

using AxLogging;

using ErrorLibrary.Resources;
using System.Net;
using System.IO;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
using eHCMSLanguage;
using System.Data.SqlClient;


#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-11-22
 * Contents: Consultation Services - ePrescription
/*******************************************************************/
#endregion

/*
* 20180922 #001 TBL: BM 0000073. Them parameter List<string> listICD10Codes cho GetDrugsInTreatmentRegimen
* 20181004 #002 TTM: BM 0000138: Thêm hàm chỉ lấy chi tiết toa thuốc (không đầy đủ, trả về là string).
* 20181012 #003 TTM: Chuyển từ GetPrescriptionDetailsByPrescriptID => GetPrescriptionDetailsByPrescriptID_V2
* 20220823 #004 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
* 20220929 #005 DatTB:
* + Thêm textbox tìm bệnh nhân theo tên/mã/stt
* + Thêm đối tượng ưu tiên
* 20230329 #006 DatTB: Thay đổi list soạn thuốc trước thành list phân trang
 */

namespace ConsultationsService.ePrescriptions
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class ePrescriptionServices : eHCMS.WCFServiceCustomHeader, IePrescriptions
    {
        public ePrescriptionServices()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        #region Common
        public List<Lookup> GetLookupPrescriptionType()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetLookupPresriptionType();
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetLookupPresriptionType();
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetLookupPresriptionType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLookupPrescriptionType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTIONTYPE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 1.Prescription

        public IList<Prescription> GetPrescriptionByServiceRecID(long patientID, long? ServiecRecID, long? PtRegistrationID, DateTime? ExamDate)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByServiceRecID(patientID, ServiecRecID, PtRegistrationID, ExamDate);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionByServiceRecID(patientID, ServiecRecID, PtRegistrationID, ExamDate);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByServiceRecID(patientID, ServiecRecID, PtRegistrationID, ExamDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionByServiceRecID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Prescription> GetAllPrescriptions()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetAllPrescriptions();
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetAllPrescriptions();
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetAllPrescriptions();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllPrescriptions. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Prescription> GetPrescriptionByPtID(long patientID)
        {
            try
            {
                bool latest = false;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByPtID(patientID, latest);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionByPtID(patientID, latest);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByPtID(patientID, latest);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Prescription> GetPrescriptionByPtID_Paging(long patientID, long? V_PrescriptionType, bool isInPatient, int PageIndex, int PageSize, out int TotalCount)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByPtID_Paging(patientID, V_PrescriptionType, isInPatient, PageIndex, PageSize, out TotalCount);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionByPtID_Paging(patientID, V_PrescriptionType, isInPatient, PageIndex, PageSize, out TotalCount);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByPtID_Paging(patientID, V_PrescriptionType, isInPatient, PageIndex, PageSize, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionByPtID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Prescription> GetPrescriptionByID(long PrescriptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByID(PrescriptID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionByID(PrescriptID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByID(PrescriptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Prescription> SearchPrescription(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.SearchPrescription(Criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.SearchPrescription(Criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.SearchPrescription(Criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchPrescription. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Prescription> GetChoNhanThuocList( DateTime fromDate, DateTime toDate,int IsWaiting)
        {
            try
            { 
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetChoNhanThuocList(fromDate, toDate, IsWaiting);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetChoNhanThuocList. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void UpdateChoNhanThuoc(long outiID, bool IsWaiting, int CountPrint, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.UpdateChoNhanThuoc(outiID, IsWaiting, CountPrint,out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateChoNhanThuoc. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Prescription> Prescription_Seach_WithIsSoldIssueID(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescription_Seach_WithIsSoldIssueID(Criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.Prescription_Seach_WithIsSoldIssueID(Criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescription_Seach_WithIsSoldIssueID(Criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescription_Seach_WithIsSoldIssueID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public Prescription GetLatestPrescriptionByPtID(long patientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetLatestPrescriptionByPtID(patientID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetLatestPrescriptionByPtID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetLatestPrescriptionByPtID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLatestPrescriptionByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Prescription GetLatestPrescriptionByPtID_InPt(long patientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetLatestPrescriptionByPtID_InPt.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetLatestPrescriptionByPtID_InPt(patientID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetLatestPrescriptionByPtID_InPt(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetLatestPrescriptionByPtID_InPt(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLatestPrescriptionByPtID_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public Prescription GetNewPrescriptionByPtID(long patientID, long doctorID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetNewPrescriptionByPtID(patientID, doctorID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetNewPrescriptionByPtID(patientID, doctorID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetNewPrescriptionByPtID(patientID, doctorID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetNewPrescriptionByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.ToString()));
            }
        }

        public Prescription GetPrescriptionID(long PrescriptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionID(PrescriptID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionID(PrescriptID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionID(PrescriptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▼====== #002
        public string GetPrescriptDetailsStr_FromPrescriptID(long PrescriptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptDetailsStr_FromPrescriptID(PrescriptID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptDetailsStr_FromPrescriptID(PrescriptID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptDetailsStr_FromPrescriptID(PrescriptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptDetailsStr_FromPrescriptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #002
        public Prescription GetPrescriptionByIssueID(long IssueID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByIssueID(IssueID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionByIssueID(IssueID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionByIssueID(IssueID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionByIssueID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeletePrescription(Prescription entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.DeletePrescription(entity);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.DeletePrescription(entity);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.DeletePrescription(entity);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeletePrescription. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        

        public void Prescriptions_UpdateDoctorAdvice(Prescription entity, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_UpdateDoctorAdvice(entity, out Result);
                //}
                //else
                //{
                //    ePrescriptionsProvider.Instance.Prescriptions_UpdateDoctorAdvice(entity, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_UpdateDoctorAdvice(entity, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_UpdateDoctorAdvice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public bool AddFullPrescriptionByServiceRecID(Prescription entity, out long newPrescriptID)
        //{
        //    try
        //    {
        //        long IssueID = 0;
        //        bool BOK = false;
        //        BOK= ePrescriptionsProvider.Instance.AddFullPrescriptionByServiceRecID(entity, out newPrescriptID,out IssueID);

        //        if (entity.PtRegistrationID.HasValue)
        //        {
        //            PatientQueue queue = new PatientQueue();
        //            queue.V_QueueType = (long)(int)AllLookupValues.QueueType.MUA_THUOC;
        //            queue.PatientID = entity.PatientID;
        //            queue.RegistrationID = entity.PtRegistrationID.Value;
        //            queue.PrescriptionIssueID = IssueID;
        //            queue.FullName = entity.PatientFullName;

        //            PatientProvider.Instance.PatientQueue_Insert(queue); 
        //        }
        //        return BOK;

        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving AddFullPrescriptionByServiceRecID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_ADDFULL);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}


        public bool Prescriptions_Add(Int16 NumberTypePrescriptions_Rule, Prescription entity, out long newPrescriptID, out long IssueID, out string OutError
            , out IList<PrescriptionIssueHistory> allPrescriptionIssueHistory)
        {
            try
            {
                newPrescriptID = 0;
                IssueID = 0;
                bool BOK = false;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    BOK =  aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_Add(NumberTypePrescriptions_Rule, entity, out newPrescriptID, out IssueID, out OutError);
                //}
                //else
                //{
                //    BOK = ePrescriptionsProvider.Instance.Prescriptions_Add(NumberTypePrescriptions_Rule, entity, out newPrescriptID, out IssueID, out OutError);
                //}
                BOK = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_Add(NumberTypePrescriptions_Rule, entity, out newPrescriptID, out IssueID, out OutError);
                //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
                //if (entity.PtRegistrationID.HasValue)
                //{
                //    PatientQueue queue = new PatientQueue();
                //    queue.V_QueueType = (long)(int)AllLookupValues.QueueType.MUA_THUOC;
                //    queue.PatientID = entity.PatientID;
                //    queue.RegistrationID = entity.PtRegistrationID.Value;
                //    queue.PrescriptionIssueID = IssueID;
                //    queue.FullName = entity.PatientFullName;

                //    //PatientProvider.Instance.PatientQueue_Insert(queue);
                //}
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allPrescriptionIssueHistory = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(entity.PtRegistrationID.Value, true);
                //}
                //else
                //{
                //    allPrescriptionIssueHistory = ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(entity.PtRegistrationID.Value, true);
                //}
                allPrescriptionIssueHistory = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(entity.PtRegistrationID.Value, true);
                return BOK;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_Add. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_ADDFULL);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public void Prescriptions_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, out string Result, out long NewPrescriptID
            , out long IssueID, out IList<PrescriptionIssueHistory> allPrescriptionIssueHistory)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_Update(entity, entity_OLD, AllowUpdateThoughReturnDrugNotEnough, out Result, out NewPrescriptID, out IssueID);
                //}
                //else
                //{
                //    ePrescriptionsProvider.Instance.Prescriptions_Update(entity, entity_OLD, AllowUpdateThoughReturnDrugNotEnough, out Result, out NewPrescriptID, out IssueID);
                //}
                aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_Update(entity, entity_OLD, AllowUpdateThoughReturnDrugNotEnough, out Result, out NewPrescriptID, out IssueID);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allPrescriptionIssueHistory = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(entity.PtRegistrationID.Value, true);
                //}
                //else
                //{
                //    allPrescriptionIssueHistory = ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(entity.PtRegistrationID.Value, true);
                //}
                allPrescriptionIssueHistory = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(entity.PtRegistrationID.Value, true);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool Prescriptions_InPt_Add(Prescription entity, out long newPrescriptID, out long IssueID, out string OutError)
        {
            try
            {
                newPrescriptID = 0;
                IssueID = 0;
                bool BOK = false;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    BOK = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_InPt_Add(entity, out newPrescriptID, out IssueID, out OutError);
                //}
                //else
                //{
                //    BOK = ePrescriptionsProvider.Instance.Prescriptions_InPt_Add(entity, out newPrescriptID, out IssueID, out OutError);
                //}
                BOK = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_InPt_Add(entity, out newPrescriptID, out IssueID, out OutError);
                return BOK;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_InPt_Add. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_ADDFULL);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public void Prescriptions_InPt_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, out string Result, out long NewPrescriptID, out long IssueID, out string ServerError)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_InPt_Update(entity, entity_OLD, AllowUpdateThoughReturnDrugNotEnough, out Result, out NewPrescriptID, out IssueID);
                //}
                //else
                //{
                //    ePrescriptionsProvider.Instance.Prescriptions_InPt_Update(entity, entity_OLD, AllowUpdateThoughReturnDrugNotEnough, out Result, out NewPrescriptID, out IssueID);
                //}
                aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_InPt_Update(entity, entity_OLD, AllowUpdateThoughReturnDrugNotEnough, out Result, out NewPrescriptID, out IssueID, out ServerError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_InPt_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool Prescriptions_DuocSiEdit(Prescription entity, Prescription entity_BacSi, out long newPrescriptID, out long IssueID, out string OutError)
        {
            try
            {
                IssueID = 0;
                bool BOK = false;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    BOK = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_DuocSiEdit(entity, entity_BacSi, out newPrescriptID, out IssueID, out OutError);
                //}
                //else
                //{
                //    BOK = ePrescriptionsProvider.Instance.Prescriptions_DuocSiEdit(entity, entity_BacSi, out newPrescriptID, out IssueID, out OutError);
                //}
                BOK = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_DuocSiEdit(entity, entity_BacSi, out newPrescriptID, out IssueID, out OutError);
                //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
                //if (entity.PtRegistrationID.HasValue)
                //{
                //    PatientQueue queue = new PatientQueue();
                //    queue.V_QueueType = (long)(int)AllLookupValues.QueueType.MUA_THUOC;
                //    queue.PatientID = entity.PatientID;
                //    queue.RegistrationID = entity.PtRegistrationID.Value;
                //    queue.PrescriptionIssueID = IssueID;
                //    queue.FullName = entity.PatientFullName;

                //    //PatientProvider.Instance.PatientQueue_Insert(queue);
                //}
                return BOK;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_DuocSiEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_ADDFULL);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool Prescriptions_DuocSiEditDuocSi(Prescription entity, Prescription entity_DuocSi, out long newPrescriptID, out long IssueID, out string OutError)
        {
            try
            {
                IssueID = 0;
                bool BOK = false;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    BOK = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_DuocSiEditDuocSi(entity, entity_DuocSi, out newPrescriptID, out IssueID, out OutError);
                //}
                //else
                //{
                //    BOK = ePrescriptionsProvider.Instance.Prescriptions_DuocSiEditDuocSi(entity, entity_DuocSi, out newPrescriptID, out IssueID, out OutError);
                //}
                BOK = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_DuocSiEditDuocSi(entity, entity_DuocSi, out newPrescriptID, out IssueID, out OutError);
                //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
                //if (entity.PtRegistrationID.HasValue)
                //{
                //    PatientQueue queue = new PatientQueue();
                //    queue.V_QueueType = (long)(int)AllLookupValues.QueueType.MUA_THUOC;
                //    queue.PatientID = entity.PatientID;
                //    queue.RegistrationID = entity.PtRegistrationID.Value;
                //    queue.PrescriptionIssueID = IssueID;
                //    queue.FullName = entity.PatientFullName;

                //    //PatientProvider.Instance.PatientQueue_Insert(queue);
                //}
                return BOK;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_DuocSiEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_ADDFULL);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<Prescription> Prescriptions_TrongNgay_ByPatientID(Int64 PatientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_TrongNgay_ByPatientID(PatientID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.Prescriptions_TrongNgay_ByPatientID(PatientID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_TrongNgay_ByPatientID(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_TrongNgay_ByPatientID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Prescription Prescription_ByPrescriptIDIssueID(Int64 PrescriptID, Int64 IssueID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescription_ByPrescriptIDIssueID(PrescriptID, IssueID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.Prescription_ByPrescriptIDIssueID(PrescriptID, IssueID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescription_ByPrescriptIDIssueID(PrescriptID, IssueID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescription_ByPrescriptIDIssueID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTISSUEHISTORY_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DiagnosisIcd10Items> GetAllDiagnosisIcd10Items_InDay(long PatientID, long ServiceRecID, long PtRegDetailID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetAllDiagnosisIcd10Items_InDay(PatientID, ServiceRecID, PtRegDetailID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllDiagnosisIcd10Items_InDay. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Prescription> Prescriptions_ListRootByPatientID_Paging(
          PrescriptionSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_ListRootByPatientID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.Prescriptions_ListRootByPatientID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_ListRootByPatientID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_ListRootByPatientID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //==== 20161004 CMN Begin: Call Rest service check Drug Interactions
        public static string ReadRestService(string aUrl)
        {
            string mContent;
            HttpWebRequest request = WebRequest.Create(aUrl) as HttpWebRequest;
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                mContent = reader.ReadToEnd();
            }
            return mContent;
        }
        public static string GetTermTypes(string tty)
        {
            switch (tty)
            {
                case "IN":
                    tty = "Ingredient";
                    //A compound or moiety that gives the drug its distinctive clinical properties. Ingredients generally use the United States Adopted Name (USAN).
                    break;
                case "PIN":
                    tty = "Precise Ingredient";
                    //A specified form of the ingredient that may or may not be clinically active. Most precise ingredients are salt or isomer forms.
                    break;
                case "MIN":
                    tty = "Multiple Ingredients";
                    //Two or more ingredients appearing together in a single drug preparation, created from SCDF. In rare cases when IN/PIN or PIN/PIN combinations of the same base ingredient exist, created from SCD.
                    break;
                case "DF":
                    tty = "Dose Form";
                    //See Appendix 2 for a full list of Dose Forms.
                    break;
                case "DFG":
                    tty = "Dose Form Group";
                    //See Appendix 3 for a full list of Dose Form Groups.
                    break;
                case "SCDC":
                    tty = "Semantic Clinical Drug Component";
                    //Ingredient + Strength
                    break;
                case "SCDF":
                    tty = "Semantic Clinical Drug Form";
                    //Ingredient + Dose Form
                    break;
                case "SCDG":
                    tty = "Semantic Clinical Dose Form Group";
                    //Ingredient + Dose Form Group
                    break;
                case "SCD":
                    tty = "Semantic Clinical Drug";
                    //Ingredient + Strength + Dose Form
                    break;
                case "BN":
                    tty = "Brand Name";
                    //A proprietary name for a family of products containing a specific active ingredient.
                    break;
                case "SBDC":
                    tty = "Semantic Branded Drug Component";
                    //Ingredient + Strength + Brand Name
                    break;
                case "SBDF":
                    tty = "Semantic Branded Drug Form";
                    //Ingredient + Dose Form + Brand Name
                    break;
                case "SBDG":
                    tty = "Semantic Branded Dose Form Group";
                    //Brand Name + Dose Form Group
                    break;
                case "SBD":
                    tty = "Semantic Branded Drug";
                    //Ingredient + Strength + Dose Form + Brand Name
                    break;
                case "PSN":
                    tty = "Prescribable Name";
                    //Synonym of another TTY, given for clarity and for display purposes in electronic prescribing applications. Only one PSN per concept.
                    break;
                case "SY":
                    tty = "Synonym";
                    //Synonym of another TTY, given for clarity.
                    break;
                case "TMSY":
                    tty = "Tall Man Lettering Synonym";
                    //Tall Man Lettering synonym of another TTY, given to distinguish between commonly confused drugs.
                    break;
                case "BPCK":
                    tty = "Brand Name Pack";
                    //{# (Ingredient Strength Dose Form) / # (Ingredient Strength Dose Form)} Pack [Brand Name]
                    break;
                case "GPCK":
                    tty = "Generic Pack";
                    //{# (Ingredient + Strength + Dose Form) / # (Ingredient + Strength + Dose Form)} Pack
                    break;
            }
            return tty;
        }
        //Class to Serializer xml result from rest service
        public class InteractionDrug
        {
            public string rxcui { get; set; }
            public string name { get; set; }
            public string tty { get; set; }
            public string description { get; set; }
            public InteractionDrug(string rxcui, string name, string tty, string description)
            {
                this.rxcui = rxcui;
                this.name = name;
                this.tty = GetTermTypes(tty);
                this.description = description;
            }
        }
        public class Drug
        {
            public string rxcui { get; set; }
            public string name { get; set; }
            public string tty { get; set; }
            public string language { get; set; }
            public Drug(string rxcui, string name)
            {
                this.rxcui = rxcui;
                this.name = name;
            }
        }
        public partial class interactiondata
        {
            public List<InteractionDrug> GetAllInteractionDrug()
            {
                string DrugID = userInput[0].rxcui;
                List<InteractionDrug> mInteractionDrug = new List<InteractionDrug>();
                if (this.interactionTypeGroup != null)
                {
                    InteractionType[] mInteractionType = this.interactionTypeGroup[0].interactionType;
                    foreach (InteractionType aInteractionType in mInteractionType)
                    {
                        InteractionPair[] mInteractionPair = aInteractionType.interactionPair;
                        foreach (InteractionPair aInteractionPair in mInteractionPair)
                        {
                            string mDescription = aInteractionPair.description;
                            InteractionConcept[] mInteractionConcept = aInteractionPair.interactionConcept;
                            foreach (InteractionConcept aInteractionConcept in mInteractionConcept)
                            {
                                if (aInteractionConcept.minConceptItem[0].rxcui != DrugID && !mInteractionDrug.Any(x => x.rxcui == aInteractionConcept.minConceptItem[0].rxcui))
                                {
                                    InteractionDrug aInteractionDrug = new InteractionDrug(aInteractionConcept.minConceptItem[0].rxcui, aInteractionConcept.minConceptItem[0].name, aInteractionConcept.minConceptItem[0].tty, mDescription);
                                    mInteractionDrug.Add(aInteractionDrug);
                                }
                            }
                        }
                    }
                }
                return mInteractionDrug;
            }
            public string nlmDisclaimer { get; set; }
            [System.Xml.Serialization.XmlElementAttribute("userInput", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public UserInput[] userInput { get; set; }
            public class UserInput
            {
                [System.Xml.Serialization.XmlElementAttribute("rxcui", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public string rxcui { get; set; }
            }
            [System.Xml.Serialization.XmlElementAttribute("interactionTypeGroup", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public InteractionTypeGroup[] interactionTypeGroup { get; set; }
            public class InteractionTypeGroup
            {
                public string sourceDisclaimer { get; set; }
                public string sourceName { get; set; }
                [System.Xml.Serialization.XmlElementAttribute("interactionType", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public InteractionType[] interactionType { get; set; }
            }
            public class InteractionType
            {
                public string comment { get; set; }
                [System.Xml.Serialization.XmlElementAttribute("minConceptItem", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public MinConceptItem[] minConceptItem { get; set; }
                [System.Xml.Serialization.XmlElementAttribute("interactionPair", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public InteractionPair[] interactionPair { get; set; }
            }
            public class MinConceptItem
            {
                public string rxcui { get; set; }
                public string name { get; set; }
                public string tty { get; set; }
            }
            public class InteractionPair
            {
                public string severity { get; set; }
                public string description { get; set; }
                [System.Xml.Serialization.XmlElementAttribute("interactionConcept", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public InteractionConcept[] interactionConcept { get; set; }
            }
            public class InteractionConcept
            {
                [System.Xml.Serialization.XmlElementAttribute("minConceptItem", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public MinConceptItem[] minConceptItem { get; set; }
                [System.Xml.Serialization.XmlElementAttribute("sourceConceptItem", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public SourceConceptItem[] sourceConceptItem { get; set; }
                public class SourceConceptItem
                {
                    public string id { get; set; }
                    public string name { get; set; }
                    public string url { get; set; }
                }
            }
        }
        public partial class interactiondata
        {
            public List<string[]> GetAllIntraction()
            {
                List<string[]> mDrugInteraction = new List<string[]>();
                FullInteractionType[] mFullInteractionType = this.fullInteractionTypeGroup[0].fullInteractionType;
                foreach (FullInteractionType aFullInteractionType in mFullInteractionType)
                {
                    InteractionPair[] mInteractionPair = aFullInteractionType.interactionPair;
                    foreach (InteractionPair aInteractionPair in mInteractionPair)
                    {
                        string[] aDrugInteraction = new string[3] { aInteractionPair.interactionConcept[0].minConceptItem[0].rxcui, aInteractionPair.interactionConcept[1].minConceptItem[0].rxcui, aInteractionPair.description };
                        mDrugInteraction.Add(aDrugInteraction);
                    }
                }
                return mDrugInteraction;
            }
            [System.Xml.Serialization.XmlElementAttribute("fullInteractionTypeGroup", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public FullInteractionTypeGroup[] fullInteractionTypeGroup { get; set; }
            public class FullInteractionTypeGroup
            {
                [System.Xml.Serialization.XmlElementAttribute("fullInteractionType", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public FullInteractionType[] fullInteractionType { get; set; }
            }
            public class FullInteractionType
            {
                [System.Xml.Serialization.XmlElementAttribute("interactionPair", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public InteractionPair[] interactionPair { get; set; }
            }
        }
        public partial class rxnormdata
        {
            public List<Drug> GetAllDrug()
            {
                List<Drug> mDrug = new List<Drug>();
                foreach (IdGroup aIdGroup in this.idGroup)
                {
                    mDrug.Add(new Drug(aIdGroup.rxnormId, aIdGroup.name));
                }
                return mDrug;
            }
            [System.Xml.Serialization.XmlElementAttribute("idGroup", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public IdGroup[] idGroup { get; set; }
            public class IdGroup
            {
                public string name { get; set; }
                public string rxnormId { get; set; }
            }
        }
        public partial class rxnormdata
        {
            [System.Xml.Serialization.XmlElementAttribute("displayTermsList", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
            public DisplayTermsList[] displayTermsList { get; set; }
            public class DisplayTermsList
            {
                [System.Xml.Serialization.XmlElementAttribute("term", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
                public string[] term { get; set; }
            }
        }
        public string CheckDrugInteraction(string[] aGenericName, string[] aBrandName)
        {
            if (aGenericName.Length == 0)
                return "";
            List<string> aDrugID = new List<string>();
            string DrugIntractions = "";
            try
            {
                for (int i = 0; i < aGenericName.Length; i++)
                {
                    string aName = aGenericName[i];
                    string RequestContent = ReadRestService("https://rxnav.nlm.nih.gov/REST/rxcui?name=" + aName);
                    XmlSerializer XMLSerializer = new XmlSerializer(typeof(rxnormdata));
                    StringReader RequestReader = new StringReader(RequestContent);
                    rxnormdata RequestResult = (rxnormdata)XMLSerializer.Deserialize(RequestReader);
                    List<Drug> mDrug = RequestResult.GetAllDrug();
                    if (mDrug.Count == 1)
                        aDrugID.Add(mDrug.First().rxcui);
                }
                {
                    string mListDrugID = String.Join("+", aDrugID.Distinct().ToArray());
                    string RequestContent = ReadRestService("https://rxnav.nlm.nih.gov/REST/interaction/list.xml?rxcuis=" + mListDrugID);
                    XmlSerializer XMLSerializer = new XmlSerializer(typeof(interactiondata));
                    StringReader RequestReader = new StringReader(RequestContent);
                    interactiondata RequestResult = (interactiondata)XMLSerializer.Deserialize(RequestReader);
                    List<string[]> mFoundIntraction = RequestResult.GetAllIntraction();
                    List<string> mDrugName = aBrandName.ToList();
                    int Pos = 1;
                    foreach (string[] IntractionPair in mFoundIntraction)
                    {
                        int[] mSourIndex = aDrugID.Where(x => x == IntractionPair[0]).Select(x => aDrugID.IndexOf(x)).ToArray();
                        int[] mDesIndex = aDrugID.Where(x => x == IntractionPair[1]).Select(x => aDrugID.IndexOf(x)).ToArray();
                        String SourName = String.Join(",", mDrugName.Where(x => mSourIndex.Contains(mDrugName.IndexOf(x))).Distinct().ToArray());
                        String DesName = String.Join(",", mDrugName.Where(x => mDesIndex.Contains(mDrugName.IndexOf(x))).Distinct().ToArray());
                        if (!String.IsNullOrEmpty(SourName) && !String.IsNullOrEmpty(DesName))
                        {
                            DrugIntractions += Pos.ToString() + ". " + SourName + " tương tác với " + DesName + " : " + IntractionPair[2] + "\n";
                            Pos++;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
            return DrugIntractions;
        }
        //==== 20161004 CMN End.
        #endregion

        #region 2.GetDrugForPrescription
        public IList<GetDrugForSellVisitor> GetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex, int IsMedDept, long? StoreID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetDrugForPrescription_Auto(BrandName, IsInsurance, PageSize, PageIndex, IsMedDept, StoreID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetDrugForPrescription_Auto(BrandName, IsInsurance, PageSize, PageIndex, IsMedDept, StoreID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetDrugForPrescription_Auto(BrandName, IsInsurance, PageSize, PageIndex, IsMedDept, StoreID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForPrescription_Auto. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_DRUG_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<GetDrugForSellVisitor> SearchDrugForPrescription_Paging(String BrandName, bool IsSearchByGenericName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, out int Total)
        {
            Total = 0;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.SearchDrugForPrescription_Paging(BrandName, IsSearchByGenericName, IsInsurance, StoreID, PageIndex, PageSize, CountTotal, out Total);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.SearchDrugForPrescription_Paging(BrandName, IsSearchByGenericName, IsInsurance, StoreID, PageIndex, PageSize, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.SearchDrugForPrescription_Paging(BrandName, IsSearchByGenericName
                    , IsInsurance, StoreID, PageIndex, PageSize, CountTotal, out Total);
            }
            catch (SqlException ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForPrescription_Auto. Status: Failed.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchDrugForPrescription_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load SearchDrugForPrescription_Paging List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return null;
        }
        /*▼====: #001*/
        //public IList<GetDrugForSellVisitor> GetDrugsInTreatmentRegimen(long PtRegDetailID)
        //{
        //    try
        //    {
        //        return ePrescriptionsProvider.Instance.GetDrugsInTreatmentRegimen(PtRegDetailID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetDrugsInTreatmentRegimen. Status: Failed.", CurrentUser);
        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load GetDrugsInTreatmentRegimen");
        //        throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
        //    }
        //}
        public IList<GetDrugForSellVisitor> GetDrugsInTreatmentRegimen(long PtRegDetailID, List<string> listICD10Codes)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetDrugsInTreatmentRegimen(PtRegDetailID, listICD10Codes);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetDrugsInTreatmentRegimen(PtRegDetailID, listICD10Codes);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetDrugsInTreatmentRegimen(PtRegDetailID, listICD10Codes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugsInTreatmentRegimen. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load GetDrugsInTreatmentRegimen");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▲====: #001*/
        public IList<RefTreatmentRegimen> GetRefTreatmentRegimensAndDetail(long? PtRegDetailID = null, List<string> listICD10Codes = null)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetRefTreatmentRegimensAndDetail(null, PtRegDetailID, listICD10Codes);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetRefTreatmentRegimensAndDetail(null, PtRegDetailID, listICD10Codes);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetRefTreatmentRegimensAndDetail(null, PtRegDetailID, listICD10Codes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefTreatmentRegimensAndDetail. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load GetRefTreatmentRegimensAndDetail");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<RefTreatmentRegimen> GetRefTreatmentRegimensAndDetailByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetRefTreatmentRegimensAndDetail(null, null, null, PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefTreatmentRegimensAndDetailByPtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load GetRefTreatmentRegimensAndDetailByPtRegistrationID");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool EditRefTreatmentRegimen(RefTreatmentRegimen aRefTreatmentRegimen, out RefTreatmentRegimen aOutRefTreatmentRegimen)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.EditRefTreatmentRegimen(aRefTreatmentRegimen, out aOutRefTreatmentRegimen);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.EditRefTreatmentRegimen(aRefTreatmentRegimen, out aOutRefTreatmentRegimen);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.EditRefTreatmentRegimen(aRefTreatmentRegimen, out aOutRefTreatmentRegimen);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving EditRefTreatmentRegimen. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load EditRefTreatmentRegimen");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<GetDrugForSellVisitor> SearchGenMedProductForPrescription_Paging(String BrandName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, bool IsSearchByGenericName, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.SearchGenMedProductForPrescription_Paging(BrandName, IsInsurance, StoreID, PageIndex, PageSize, CountTotal, out Total);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.SearchGenMedProductForPrescription_Paging(BrandName, IsInsurance, StoreID, PageIndex, PageSize, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.SearchGenMedProductForPrescription_Paging(BrandName, IsInsurance, StoreID, PageIndex, PageSize, CountTotal, IsSearchByGenericName, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchGenMedProductForPrescription_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load SearchGenMedProductForPrescription_Paging List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<GetDrugForSellVisitor> GetDrugForPrescription_Remaining(long? StoreID, string xml)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetDrugForPrescription_Remaining(StoreID, xml);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetDrugForPrescription_Remaining(StoreID, xml);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetDrugForPrescription_Remaining(StoreID, xml);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugForPrescription_Remaining. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load GetDrugForPrescription_Remaining List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<GetDrugForSellVisitor> GetListDrugPatientUsed(long PatientID, int PageIndex, int PageSize, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetListDrugPatientUsed(PatientID, PageIndex, PageSize, out Total);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetListDrugPatientUsed(PatientID, PageIndex, PageSize, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetListDrugPatientUsed(PatientID, PageIndex, PageSize, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetListDrugPatientUsed. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load GetListDrugPatientUsed");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 3.Prescription Details

        public IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID(long PrescriptID, bool GetRemaining = false)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID(PrescriptID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID(PrescriptID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID(PrescriptID, GetRemaining);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionDetailsByPrescriptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTDETAIL_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_WithNDay(long PrescriptID, out int NDay, bool GetRemaining)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_WithNDay(PrescriptID, out NDay);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_WithNDay(PrescriptID, out NDay);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_WithNDay(PrescriptID, out NDay, GetRemaining);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionDetailsByPrescriptID_WithNDay. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTDETAIL_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▼====== #003
        public IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_V2(long PrescriptID, long? IssueID, long? AppointmentID, out bool CanEdit, out string ReasonCanEdit, out bool IsEdit, bool GetRemaining)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_V2(PrescriptID, IssueID, AppointmentID, out CanEdit, out ReasonCanEdit, out IsEdit);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_V2(PrescriptID, IssueID, AppointmentID, out CanEdit, out ReasonCanEdit, out IsEdit);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_V2(PrescriptID, IssueID, AppointmentID, out CanEdit, out ReasonCanEdit, out IsEdit, GetRemaining);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionDetailsByPrescriptID_V2. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTDETAIL_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #003
        public IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_InPt(long PrescriptID, bool GetRemaining, bool OutPatient)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_InPt(PrescriptID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_InPt(PrescriptID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_InPt(PrescriptID, null, GetRemaining, OutPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionDetailsByPrescriptID_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTDETAIL_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_InPt_V2(long PrescriptID, bool GetRemaining, bool OutPatient, long[] V_CatDrugType = null)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_InPt(PrescriptID, V_CatDrugType);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_InPt(PrescriptID, V_CatDrugType);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionDetailsByPrescriptID_InPt(PrescriptID, V_CatDrugType, GetRemaining, OutPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionDetailsByPrescriptID_InPt_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTDETAIL_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void UpdateDrugNotDisplayInList(long PatientID, long DrugID, bool? NotDisplayInList)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.UpdateDrugNotDisplayInList(PatientID, DrugID, NotDisplayInList);
                //}
                //else
                //{
                //    ePrescriptionsProvider.Instance.UpdateDrugNotDisplayInList(PatientID, DrugID, NotDisplayInList);
                //}
                aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.UpdateDrugNotDisplayInList(PatientID, DrugID, NotDisplayInList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateDrugNotDisplayInList. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTDETAIL_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 4.PrescriptionIssueHistory

        //KMx: Sau khi kiểm tra toàn bộ chương trình, thấy hàm này không được gọi (không được sử dụng) nữa (22/02/2014 16:38).
        public bool AddPrescriptIssueHistory(Int16 NumberTypePrescriptions_Rule, Prescription entity, out string OutError)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.AddPrescriptIssueHistory(NumberTypePrescriptions_Rule, entity, out OutError);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.AddPrescriptIssueHistory(NumberTypePrescriptions_Rule, entity, out OutError);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.AddPrescriptIssueHistory(NumberTypePrescriptions_Rule, entity, out OutError);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddPrescriptIssueHistory. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTISSUEHISTORY_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public bool UpdatePrescriptIssueHistory(Prescription entity)
        //{
        //    try
        //    {
        //        long prescriptIssuedCase = (long)entity.PrescriptionIssueHistory.V_PrescriptionIssuedCase;
        //        return ePrescriptionsProvider.Instance.UpdatePrescriptIssueHistory(entity, prescriptIssuedCase);
        //        //CRUDOperationResponse response = new CRUDOperationResponse();
        //        //return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving UpdatePrescriptIssueHistory. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTISSUEHISTORY_CANNOT_UPDATE);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public IList<PrescriptionIssueHistory> GetPrescriptionIssueHistory(long prescriptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionIssueHistory(prescriptID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetPrescriptionIssueHistory(prescriptID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionIssueHistory(prescriptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionIssueHistory. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTISSUEHISTORY_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 5.patientPaymentOld

        public FeeDrug GetValuePatientPaymentOld(long PtRegistrationID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetValuePatientPaymentOld(PtRegistrationID);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.GetValuePatientPaymentOld(PtRegistrationID);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetValuePatientPaymentOld(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetValuePatientPaymentOld. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FEEDRUG_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion
        #region choose dose member
        public IList<ChooseDose> InitChooseDoses()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.InitChooseDoses();
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.InitChooseDoses();
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.InitChooseDoses();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetValuePatientPaymentOld. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_CHOOSEDOSE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region PrescriptionDetailSchedules
        public List<PrescriptionDetailSchedules> PrescriptionDetailSchedules_ByPrescriptDetailID(Int64 PrescriptDetailID, bool IsNotIncat)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionDetailSchedules_ByPrescriptDetailID(PrescriptDetailID, IsNotIncat);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.PrescriptionDetailSchedules_ByPrescriptDetailID(PrescriptDetailID, IsNotIncat);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionDetailSchedules_ByPrescriptDetailID(PrescriptDetailID, IsNotIncat);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PrescriptionDetailSchedules_ByPrescriptDetailID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTISSUEHISTORY_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region PrescriptionDetailSchedulesLieuDung
        public List<PrescriptionDetailSchedulesLieuDung> InitChoosePrescriptionDetailSchedulesLieuDung()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.InitChoosePrescriptionDetailSchedulesLieuDung();
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.InitChoosePrescriptionDetailSchedulesLieuDung();
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.InitChoosePrescriptionDetailSchedulesLieuDung();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InitChoosePrescriptionDetailSchedulesLieuDung. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_CHOOSEDOSE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        public void Prescriptions_CheckCanEdit(Int64 PrescriptID, Int64 IssueID, out bool CanEdit, out string ReasonCanEdit)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_CheckCanEdit(PrescriptID, IssueID, out CanEdit, out ReasonCanEdit);
                //}
                //else
                //{
                //    ePrescriptionsProvider.Instance.Prescriptions_CheckCanEdit(PrescriptID, IssueID, out CanEdit, out ReasonCanEdit);
                //}
                aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_CheckCanEdit(PrescriptID, IssueID, out CanEdit, out ReasonCanEdit);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_CheckCanEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTISSUEHISTORY_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #region "PrescriptionNoteTemplates"
        public IList<PrescriptionNoteTemplates> PrescriptionNoteTemplates_GetAll()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionNoteTemplates_GetAll();
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.PrescriptionNoteTemplates_GetAll();
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionNoteTemplates_GetAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PrescriptionNoteTemplates_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PrescriptionNoteTemplates_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PrescriptionNoteTemplates> PrescriptionNoteTemplates_GetAllIsActive(PrescriptionNoteTemplates Obj)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionNoteTemplates_GetAllIsActive(Obj);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.PrescriptionNoteTemplates_GetAllIsActive(Obj);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionNoteTemplates_GetAllIsActive(Obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PrescriptionNoteTemplates_GetAllIsActive. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PrescriptionNoteTemplates_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PrescriptionNoteTemplates_Save(PrescriptionNoteTemplates Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionNoteTemplates_Save(Obj, out Result);
                //}
                //else
                //{
                //    ePrescriptionsProvider.Instance.PrescriptionNoteTemplates_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionNoteTemplates_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PrescriptionNoteTemplates_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PrescriptionNoteTemplates_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        public bool PrescriptionsTemplateInsert(PrescriptionTemplate Obj)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionsTemplateInsert(Obj);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.PrescriptionsTemplateInsert(Obj);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionsTemplateInsert(Obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_CheckCanEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTISSUEHISTORY_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PrescriptionsTemplateDelete(PrescriptionTemplate Obj)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionsTemplateDelete(Obj);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.PrescriptionsTemplateDelete(Obj);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionsTemplateDelete(Obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_CheckCanEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTISSUEHISTORY_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PrescriptionTemplate> PrescriptionsTemplateGetAll(PrescriptionTemplate Obj)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionsTemplateGetAll(Obj);
                //}
                //else
                //{
                //    return ePrescriptionsProvider.Instance.PrescriptionsTemplateGetAll(Obj);
                //}
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescriptionsTemplateGetAll(Obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_CheckCanEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPTISSUEHISTORY_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void GetAppointmentID(long issueID, bool isInPatient, out long? appointmentID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetAppointmentID(issueID, isInPatient, out appointmentID);
                //}
                //else
                //{
                //    ePrescriptionsProvider.Instance.GetAppointmentID(issueID, isInPatient, out appointmentID);
                //}
                aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetAppointmentID(issueID, isInPatient, out appointmentID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAppointmentID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Prescription> SearchOutPtTreatmentPrescription(PrescriptionSearchCriteria Criteria, DateTime SearchTime, int pageIndex, int pageSize, out int totalCount)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.SearchOutPtTreatmentPrescription(Criteria, SearchTime, pageIndex, pageSize, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchPrescription. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public decimal GetTotalHIPaymentForRegistration(long PtRegistrationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetTotalHIPaymentForRegistration(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetTotalHIPaymentForRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_GET_APPOINTMENTID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool CheckStatusPCLRequestBeforeCreateNew(long PtRegistrationID, bool IsGCT)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.CheckStatusPCLRequestBeforeCreateNew(PtRegistrationID, IsGCT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving CheckStatusPCLRequestBeforeCreateNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error Check Status PCL Request Before CreateNew");

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<Prescription> Prescriptions_BaoHiemConThuoc_ByPatientID(Int64 PatientID, long PtRegDetailID, bool CungChuyenKhoa)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_BaoHiemConThuoc_ByPatientID(PatientID, PtRegDetailID, CungChuyenKhoa);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Prescriptions_BaoHiemConThuoc_ByPatientID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public int? GetRemainingMedicineDays(long OutPtTreatmentProgramID, long PtRegDetailID, out int MinNumOfDayMedicine, out int MaxNumOfDayMedicine)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetRemainingMedicineDays(OutPtTreatmentProgramID, PtRegDetailID, out MinNumOfDayMedicine, out MaxNumOfDayMedicine);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRemainingMedicineDays. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼==== #004
        public IList<Prescription> GetChoSoanThuocList(DateTime fromDate, DateTime toDate, int IsWaiting, string sPatientCode, string sPatientName, int sStoreServiceSeqNum, int pageIndex, int pageSize, out int totalCount) //==== #005, #006
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetChoSoanThuocList(fromDate, toDate, IsWaiting, sPatientCode, sPatientName, sStoreServiceSeqNum, pageIndex, pageSize, out totalCount); //==== #005, #006
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetChoSoanThuocList. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void UpdateChoSoanThuoc(long PtRegistrationID, bool IsWaiting, int CountPrint, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.UpdateChoSoanThuoc(PtRegistrationID, IsWaiting, CountPrint, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateChoSoanThuoc. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PRESCRIPT_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #004
    }
}
