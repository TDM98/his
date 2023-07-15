using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;

using eHCMS.Configurations;

using ErrorLibrary;
using System.Runtime.Serialization;

using AxLogging;
using System.Linq;
using ErrorLibrary.Resources;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Service.Core.Common;
using eHCMSLanguage;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-11-22
 * Contents: Consultation Services - Patient Medical Records
/*******************************************************************/
/*
 * 20180917 #001 TTM: 
 * 20180923 #002 TBL:    BM 0000066. Added out long DTItemID
 * 20181023 #003 TBL:    BM 0003206. Khi luu can phai biet la them moi hay la cap nhat
 * 20191010 #004 TTM:    BM 0017443: [Kiểm soát nhiễm khuẩn]: Bổ sung màn hình hội chẩn.
 */
#endregion

/*
* 20210428 #001 TNHX:  Lấy danh sách ICD sao kèm theo
* 20210611 #002 TNHX:  Lấy danh sách quy tắc ICD
* 20220602 #003 DatTB: IssueID: 1619 | [CSKH] Thêm màn hình nhập kết quả cho KSK
* 20230102 #004 BLQ: Thêm thông tin tờ tự khai
* 20230307 #005 BLQ: Thêm định mức bàn khám 
* 20230330 #006 QTD: Thêm ICD9 cho ngoại trú
* 20230403 #007 QTD: Thêm mã máy
* 20230503 #008 QTD: Thêm dữ liệu sơ kết
* 20230517 #009 DatTB: Chỉnh sửa service xóa giấy chuyển tuyến
* 20230518 #010 DatTB: Chỉnh service lưu thêm người thêm,sửa giấy chuyển tuyến
* 20230527 #011 DatTB: Thêm service xóa kết quả KSK
*/
namespace ConsultationsService.ePMRs
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class ePMRsServices : eHCMS.WCFServiceCustomHeader, IePMRs
    {
        public ePMRsServices()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        #region Common
        public List<Lookup> GetLookupBehaving()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLookupBehaving();
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetLookupBehaving();
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLookupBehaving();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLookupBehaving. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Lookup> GetLookupProcessingType()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLookupProcessingType();
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetLookupProcessingType();
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLookupProcessingType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLookupProcessingType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_LKBEHAVING_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 1.MedcialRecordTemplate

        public IList<MedicalRecordTemplate> GetAllMedRecTemplates()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllMedRecTemplates();
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetAllMedRecTemplates();
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllMedRecTemplates();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllMedRecTemplates. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_MEDICALRECORDTEMPLATE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion


        #region 1.DiagnosisTreatment


        public IList<DiagnosisTreatment> GetAllDiagnosisTreatments()
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllDiagnosisTreatments();
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetAllDiagnosisTreatments();
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllDiagnosisTreatments();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllDiagnosisTreatments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DiagnosisTreatment> GetDiagnosisTreatmentsByServiceRecID(long PatientID, long? ServiceRecID, long? PtRegistrationID, DateTime? ExamDate)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsByServiceRecID(PatientID, ServiceRecID, PtRegistrationID, ExamDate);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetDiagnosisTreatmentsByServiceRecID(PatientID, ServiceRecID, PtRegistrationID, ExamDate);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsByServiceRecID(PatientID, ServiceRecID, PtRegistrationID, ExamDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisTreatmentsByServiceRecID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DiagnosisTreatment> GetDiagnosisTreatment_InPt_ByPtRegID(long PtRegistrationID, long? DTItemID, long? DeptID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatment_InPt_ByPtRegID(PtRegistrationID, DTItemID, DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisTreatment_InPt_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOADID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<DiagnosisTreatment> GetDiagnosisTreatmentByPtID(long? patientID, long? ptRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentByPtID(patientID, ptRegistrationID, nationalMedCode, opt, latest, V_RegistrationType, ServiceRecID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetDiagnosisTreatmentByPtID(patientID, ptRegistrationID, nationalMedCode, opt, latest, V_RegistrationType, ServiceRecID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentByPtID(patientID, ptRegistrationID, nationalMedCode, opt, latest, V_RegistrationType, ServiceRecID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisTreatmentByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DiagnosisTreatment> GetDiagnosisTreatmentByPtID_V2(long? patientID, long? ptRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID, long? PtRegDetailID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentByPtID_V2(patientID, ptRegistrationID, nationalMedCode, opt, latest, V_RegistrationType, ServiceRecID, PtRegDetailID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisTreatmentByPtID_V2. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, long? V_Behaving, int PageIndex, int PageSize, out int TotalCount)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentListByPtID(patientID, PtRegistrationID, nationalMedCode, opt, V_Behaving, PageIndex, PageSize, out TotalCount);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetDiagnosisTreatmentListByPtID(patientID, PtRegistrationID, nationalMedCode, opt, V_Behaving, PageIndex, PageSize, out TotalCount);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentListByPtID(patientID, PtRegistrationID, nationalMedCode, opt, V_Behaving, PageIndex, PageSize, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisTreatmentByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID_InPt(long patientID, long? V_Behaving, int PageIndex, int PageSize, out int TotalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetDiagnosisTreatmentListByPtID_InPt.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentListByPtID_InPt(patientID, V_Behaving, PageIndex, PageSize, out TotalCount);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetDiagnosisTreatmentListByPtID_InPt(patientID, V_Behaving, PageIndex, PageSize, out TotalCount);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentListByPtID_InPt(patientID, V_Behaving, PageIndex, PageSize, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisTreatmentListByPtID_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOAD_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public DiagnosisTreatment GetDiagnosisTreatmentByDTItemID(long DTItemID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentByDTItemID(DTItemID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetDiagnosisTreatmentByDTItemID(DTItemID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentByDTItemID(DTItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisTreatmentByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID(long? patientID, long? ptRegistrationID, string nationalMedCode, int opt, bool latest)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLatestDiagnosisTreatmentByPtID(patientID, ptRegistrationID, nationalMedCode, opt, latest);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetLatestDiagnosisTreatmentByPtID(patientID, ptRegistrationID, nationalMedCode, opt, latest);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLatestDiagnosisTreatmentByPtID(patientID, ptRegistrationID, nationalMedCode, opt, latest);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisTreatmentByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID_InPt(long patientID, long? V_DiagnosisType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetLatestDiagnosisTreatmentByPtID_InPt.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLatestDiagnosisTreatmentByPtID_InPt(patientID, V_DiagnosisType);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetLatestDiagnosisTreatmentByPtID_InPt(patientID, V_DiagnosisType);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLatestDiagnosisTreatmentByPtID_InPt(patientID, V_DiagnosisType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLatestDiagnosisTreatmentByPtID_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //==== #001
        public DiagnosisTreatment GetLatestDiagnosisTreatment_InPtByInstructionID(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetLatestDiagnosisTreatment_InPtByInstructionID.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLatestDiagnosisTreatment_InPtByInstructionID(patientID, V_DiagnosisType, IntPtDiagDrInstructionID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetLatestDiagnosisTreatment_InPtByInstructionID(patientID, V_DiagnosisType, IntPtDiagDrInstructionID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLatestDiagnosisTreatment_InPtByInstructionID(patientID, V_DiagnosisType, IntPtDiagDrInstructionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLatestDiagnosisTreatment_InPtByInstructionID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //==== #001
        public DiagnosisTreatment GetLatestDiagnosisTreatment_InPtByInstructionID_V2(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID, out List<DiagnosisIcd10Items> ICD10List)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetLatestDiagnosisTreatment_InPtByInstructionID_V2.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLatestDiagnosisTreatment_InPtByInstructionID_V2(patientID, V_DiagnosisType, IntPtDiagDrInstructionID, out ICD10List);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLatestDiagnosisTreatment_InPtByInstructionID_V2. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public long? spGetPtRegistrationIDInDiagnosisTreatment_Latest(long patientID, long PtRegistrationID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.spGetPtRegistrationIDInDiagnosisTreatment_Latest(patientID, PtRegistrationID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.spGetPtRegistrationIDInDiagnosisTreatment_Latest(patientID, PtRegistrationID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.spGetPtRegistrationIDInDiagnosisTreatment_Latest(patientID, PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spGetPtRegistrationIDInDiagnosisTreatment_Latest. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool CheckDiagnosisTreatmentExists_PtRegDetailID(long patientID, long PtRegDetailID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.CheckDiagnosisTreatmentExists_PtRegDetailID(patientID, PtRegDetailID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.CheckDiagnosisTreatmentExists_PtRegDetailID(patientID, PtRegDetailID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.CheckDiagnosisTreatmentExists_PtRegDetailID(patientID, PtRegDetailID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving CheckDiagnosisTreatmentExists_PtRegDetailID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool CheckStatusInPtRegistration_InPtRegistrationID(long InPtRegistrationID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.CheckDiagnosisTreatmentExists_PtRegDetailID(patientID, PtRegDetailID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.CheckDiagnosisTreatmentExists_PtRegDetailID(patientID, PtRegDetailID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.CheckStatusInPtRegistration_InPtRegistrationID(InPtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving CheckStatusInPtRegistration_InPtRegistrationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void UpdateInPtRegistrationID_PtRegistration(long PtRegistrationID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.CheckDiagnosisTreatmentExists_PtRegDetailID(patientID, PtRegDetailID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.CheckDiagnosisTreatmentExists_PtRegDetailID(patientID, PtRegDetailID);
                //}
                aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateInPtRegistrationID_PtRegistration(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateInPtRegistrationID_PtRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public DiagnosisTreatment DiagnosisTreatment_GetLast(long PatientID, long PtRegistrationID, long ServiceRecID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DiagnosisTreatment_GetLast(PatientID, PtRegistrationID, ServiceRecID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.DiagnosisTreatment_GetLast(PatientID, PtRegistrationID, ServiceRecID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DiagnosisTreatment_GetLast(PatientID, PtRegistrationID, ServiceRecID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DiagnosisTreatment_GetLast. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public DiagnosisTreatment GetDiagnosisTreatment_InPt(long ServiceRecID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Begin of retrieving GetDiagnosisTreatment_InPt.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatment_InPt(ServiceRecID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetDiagnosisTreatment_InPt(ServiceRecID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatment_InPt(ServiceRecID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisTreatment_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET_BY_SERVICERECID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public DiagnosisTreatment GetBlankDiagnosisTreatmentByPtID(long? patientID, long? ptRegistrationID, string nationalMedCode)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetBlankDiagnosisTreatmentByPtID(patientID, ptRegistrationID, nationalMedCode);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetBlankDiagnosisTreatmentByPtID(patientID, ptRegistrationID, nationalMedCode);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetBlankDiagnosisTreatmentByPtID(patientID, ptRegistrationID, nationalMedCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetBlankDiagnosisTreatmentByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOADID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteDiagnosisTreatment(DiagnosisTreatment entity)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DeleteDiagnosisTreatment(entity);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.DeleteDiagnosisTreatment(entity);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DeleteDiagnosisTreatment(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteDiagnosisTreatment. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        /*▼====: #002*/
        //public bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServiceRecID)
        public bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServiceRecID, out long DTItemID)
        /*▲====: #002*/
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                /*▼====: #002*/
                //return ePMRsProvider.Instance.AddDiagnosisTreatment(entity, DiagnosisIcd10ListID, XML, out ServiceRecID);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddDiagnosisTreatment(entity, DiagnosisIcd10ListID, XML, out ServiceRecID, out DTItemID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.AddDiagnosisTreatment(entity, DiagnosisIcd10ListID, XML, out ServiceRecID, out DTItemID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddDiagnosisTreatment(entity, DiagnosisIcd10ListID, XML, out ServiceRecID, out DTItemID);
                /*▲====: #002*/
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddDiagnosisTreatment. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_ADD);

                //throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError)); 
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int AddDiagnosisTreatmentAndPrescription(bool IsUpdateWithoutChangeDoctorIDAndDatetime, DiagnosisTreatment aDiagnosisTreatment, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> aDiagnosisIcd10Items, out long ServiceRecID
            /*▼====: #002*/
            , long DiagnosisIcd9ListID, IList<DiagnosisICD9Items> aDiagnosisIcd9Items
            /*▲====: #002*/
            , Int16 NumberTypePrescriptions_Rule, Prescription aPrescription, out long OutPrescriptID, out long OutIssueID, out string OutError
            , Prescription aPrescription_Old, bool AllowUpdateThoughReturnDrugNotEnough
            /*▼====: #002*/
            //, out IList<PrescriptionIssueHistory> AllPrescriptionIssueHistory)
            , out IList<PrescriptionIssueHistory> AllPrescriptionIssueHistory, out long DTItemID
            , SmallProcedure UpdatedSmallProcedure, long StaffID, IList<Resources> ResourceList, out long SmallProcedureID, out int VersionNumber)
        /*▲====: #002*/
        {
            try
            {
                /*▼====: #003*/
                Int16 result = 0;
                Int16 newDiagnosisTreatment = 0b1000;
                Int16 updateDiagnosisTreatment = 0b0100;
                Int16 newPrescription = 0b0010;
                Int16 updatePrescription = 0b0001;
                /*▲====: #003*/
                ServiceRecID = 0;
                OutPrescriptID = 0;
                OutIssueID = 0;
                OutError = null;
                SmallProcedureID = 0;
                bool mDiagnosisTreatmentRetVal = false;
                bool PrescriptionRetVal = false;
                AllPrescriptionIssueHistory = null;
                /*▼====: #002*/
                DTItemID = 0;
                /*▲====: #002*/
                VersionNumber = 0;
                if (aDiagnosisTreatment != null)
                {
                    if (aDiagnosisTreatment.DTItemID == 0)
                    {
                        /*▼====: #002*/
                        //mDiagnosisTreatmentRetVal = ePMRsProvider.Instance.AddDiagnosisTreatment(aDiagnosisTreatment, DiagnosisIcd10ListID, aDiagnosisIcd10Items, out ServiceRecID);
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    mDiagnosisTreatmentRetVal = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddDiagnosisTreatment(aDiagnosisTreatment, DiagnosisIcd10ListID, aDiagnosisIcd10Items, out ServiceRecID, out DTItemID);
                        //}
                        //else
                        //{
                        //    mDiagnosisTreatmentRetVal = ePMRsProvider.Instance.AddDiagnosisTreatment(aDiagnosisTreatment, DiagnosisIcd10ListID, aDiagnosisIcd10Items, out ServiceRecID, out DTItemID);
                        //}
                        mDiagnosisTreatmentRetVal = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddDiagnosisTreatment(aDiagnosisTreatment, DiagnosisIcd10ListID, aDiagnosisIcd10Items, out ServiceRecID, out DTItemID);
                        /*▼====: #003*/
                        if (mDiagnosisTreatmentRetVal)
                        {
                            result += newDiagnosisTreatment;
                        }
                        //TBL: Them moi chan doan that bai thi return khong di luu toa thuoc nua
                        else
                        {
                            return result;
                        }
                        /*▲====: #003*/
                        /*▲====: #002*/
                        if (aPrescription != null)
                        {
                            aPrescription.ServiceRecID = ServiceRecID;
                        }
                        if (UpdatedSmallProcedure != null)
                        {
                            UpdatedSmallProcedure.ServiceRecID = ServiceRecID;
                        }

                    }
                    else
                    {
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    mDiagnosisTreatmentRetVal = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateDiagnosisTreatment(aDiagnosisTreatment, DiagnosisIcd10ListID, aDiagnosisIcd10Items);
                        //}
                        //else
                        //{
                        //    mDiagnosisTreatmentRetVal = ePMRsProvider.Instance.UpdateDiagnosisTreatment(aDiagnosisTreatment, DiagnosisIcd10ListID, aDiagnosisIcd10Items);
                        //}
                        mDiagnosisTreatmentRetVal = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateDiagnosisTreatment(aDiagnosisTreatment, DiagnosisIcd10ListID, aDiagnosisIcd10Items, out VersionNumber, IsUpdateWithoutChangeDoctorIDAndDatetime);
                        /*▼====: #003*/
                        if (mDiagnosisTreatmentRetVal)
                        {
                            result += updateDiagnosisTreatment;
                        }
                        else
                        {
                            return result;
                        }
                        /*▲====: #003*/
                    }
                }
                else
                {
                    mDiagnosisTreatmentRetVal = true;
                }

                if (aPrescription != null)
                {
                    if (aDiagnosisTreatment == null && aPrescription.ServiceRecID.GetValueOrDefault(0) == 0)
                    {
                        throw new Exception(eHCMSResources.A0403_G1_Msg_InfoChuaCoCDChoDK);
                    }

                    if (//▼====== #001  Thêm điều kiện cho lưu toa rỗng.
                        //              Ban đầu điều kiện ngăn chặn việc lưu dòng rỗng trong toa thuốc, nhưng toa rỗng thì chắn chắc chỉ lưu dòng rỗng.
                        //              Thêm điều kiện kiểm tra toa rỗng hay không, nếu toa rỗng thì đc phép lưu dòng rỗng trong toa và ngược lại
                        aPrescription.PreNoDrug != true
                        //▲====== #001
                        && (!aPrescription.PrescriptionDetails.Any(x => x.DrugID.GetValueOrDefault(0) > 0)))
                    {
                        PrescriptionRetVal = true;
                        /*▼====: #003*/
                        result += newPrescription;
                        //return mDiagnosisTreatmentRetVal;
                        /*▲====: #003*/
                    }
                    if (aPrescription.PrescriptID == 0)
                    {
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    PrescriptionRetVal = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_Add(NumberTypePrescriptions_Rule, aPrescription, out OutPrescriptID, out OutIssueID, out OutError);
                        //}
                        //else
                        //{
                        //    PrescriptionRetVal = ePrescriptionsProvider.Instance.Prescriptions_Add(NumberTypePrescriptions_Rule, aPrescription, out OutPrescriptID, out OutIssueID, out OutError);
                        //}
                        PrescriptionRetVal = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_Add(NumberTypePrescriptions_Rule, aPrescription, out OutPrescriptID, out OutIssueID, out OutError);
                        /*▼====: #003*/
                        if (PrescriptionRetVal)
                        {
                            result += newPrescription;
                        }
                        /*▲====: #003*/
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    AllPrescriptionIssueHistory = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(aPrescription.PtRegistrationID.Value, true);
                        //}
                        //else
                        //{
                        //    AllPrescriptionIssueHistory = ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(aPrescription.PtRegistrationID.Value, true);
                        //}
                        AllPrescriptionIssueHistory = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(aPrescription.PtRegistrationID.Value, true);
                    }
                    else
                    {
                        /*▼====: #003*/
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    PrescriptionRetVal = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_Update(aPrescription, aPrescription_Old, AllowUpdateThoughReturnDrugNotEnough, out OutError, out OutPrescriptID, out OutIssueID);
                        //}
                        //else
                        //{
                        //    PrescriptionRetVal = ePrescriptionsProvider.Instance.Prescriptions_Update(aPrescription, aPrescription_Old, AllowUpdateThoughReturnDrugNotEnough, out OutError, out OutPrescriptID, out OutIssueID);
                        //}
                        PrescriptionRetVal = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.Prescriptions_Update(aPrescription, aPrescription_Old, AllowUpdateThoughReturnDrugNotEnough, out OutError, out OutPrescriptID, out OutIssueID, IsUpdateWithoutChangeDoctorIDAndDatetime);
                        //PrescriptionRetVal = true;
                        if (PrescriptionRetVal)
                        {
                            result += updatePrescription;
                        }
                        /*▲====: #003*/
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    AllPrescriptionIssueHistory = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(aPrescription.PtRegistrationID.Value, true);
                        //}
                        //else
                        //{
                        //    AllPrescriptionIssueHistory = ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(aPrescription.PtRegistrationID.Value, true);
                        //}
                        AllPrescriptionIssueHistory = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.PrescIssueHistoryByPtRegisID(aPrescription.PtRegistrationID.Value, true);
                    }
                }
                else
                {
                    PrescriptionRetVal = true;
                }
                /*▼====: #003*/
                //return mDiagnosisTreatmentRetVal && PrescriptionRetVal;
                if (UpdatedSmallProcedure != null)
                {
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.EditSmallProcedure(UpdatedSmallProcedure, StaffID, out SmallProcedureID);
                    //}
                    //else
                    //{
                    //    CommonUtilsProvider.Instance.EditSmallProcedure(UpdatedSmallProcedure, StaffID, out SmallProcedureID);
                    //}

                    aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.EditSmallProcedure_V2(UpdatedSmallProcedure, StaffID, DiagnosisIcd9ListID, aDiagnosisIcd9Items, ResourceList, out SmallProcedureID);
                    //aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.EditSmallProcedure(UpdatedSmallProcedure, StaffID, out SmallProcedureID);
                }
                return result;
                /*▲====: #003*/
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddDiagnosisTreatmentAndPrescription. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_ADD);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool AddDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, out List<InPatientDeptDetail> ReloadInPatientDeptDetails)
        {
            try
            {
                long SmallProcedureID;
                return AddDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, XML, DiagnosisICD9ListID, ICD9XML, new List<Resources>(), out ReloadInPatientDeptDetails, null, out SmallProcedureID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool AddDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML
            //▼====: #007
            , IList<Resources> ResourceList
            //▲====: #007
            , out List<InPatientDeptDetail> ReloadInPatientDeptDetails, SmallProcedure aSmallProcedure, out long SmallProcedureID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving AddDiagnosisTreatment_InPt.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, XML, DiagnosisICD9ListID, ICD9XML, out ReloadInPatientDeptDetails, aSmallProcedure, out SmallProcedureID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.AddDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, XML, DiagnosisICD9ListID, ICD9XML, out ReloadInPatientDeptDetails, aSmallProcedure, out SmallProcedureID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, XML, DiagnosisICD9ListID, ICD9XML, ResourceList, out ReloadInPatientDeptDetails, aSmallProcedure, out SmallProcedureID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddDiagnosisTreatment_InPt. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_ADD_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out int VersionNumberOut, bool IsUpdateWithoutChangeDoctorIDAndDatetime = false)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateDiagnosisTreatment(entity, DiagnosisIcd10ListID, XML);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.UpdateDiagnosisTreatment(entity, DiagnosisIcd10ListID, XML);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateDiagnosisTreatment(entity, DiagnosisIcd10ListID, XML, out VersionNumberOut, IsUpdateWithoutChangeDoctorIDAndDatetime);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateDiagnosisTreatment. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, bool IsUpdateDiagConfirmInPT, out int VersionNumberOut)
        {
            try
            {
                VersionNumberOut = 0;
                return UpdateDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, out ReloadInPatientDeptDetails, XML, DiagnosisICD9ListID, ICD9XML, new List<Resources>(), null, IsUpdateDiagConfirmInPT, out VersionNumberOut);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML
            //▼====: #007
            , IList<Resources> ResourceList
            //▲====: #007
            , SmallProcedure aSmallProcedure, bool IsUpdateDiagConfirmInPT, out int VersionNumberOut)
        {
            try
            {
                VersionNumberOut = 0;
                AxLogger.Instance.LogInfo("Start of retrieving UpdateDiagnosisTreatment_InPt", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, out ReloadInPatientDeptDetails, XML, DiagnosisICD9ListID, ICD9XML, aSmallProcedure);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.UpdateDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, out ReloadInPatientDeptDetails, XML, DiagnosisICD9ListID, ICD9XML, aSmallProcedure);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, out ReloadInPatientDeptDetails, XML, DiagnosisICD9ListID, ICD9XML, ResourceList, aSmallProcedure, IsUpdateDiagConfirmInPT, out VersionNumberOut);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateDiagnosisTreatment_InPt. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool CheckValidProcedure(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving CheckValidProcedure", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.CheckValidProcedure(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving CheckValidProcedure. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load(long? ServiceRecID, long? PatientID, bool Last)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisIcd10Items_Load(ServiceRecID, PatientID, Last);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetDiagnosisIcd10Items_Load(ServiceRecID, PatientID, Last);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisIcd10Items_Load(ServiceRecID, PatientID, Last);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisIcd10Items_Load. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load_InPt(long DTItemID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetDiagnosisIcd10Items_Load_InPt.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisIcd10Items_Load_InPt(DTItemID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetDiagnosisIcd10Items_Load_InPt(DTItemID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisIcd10Items_Load_InPt(DTItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisIcd10Items_Load_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_ICD10ITEMS_CANNOT_GET_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<DiagnosisICD9Items> GetDiagnosisICD9Items_Load_InPt(long DTItemID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetDiagnosisICD9Items_Load_InPt.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisICD9Items_Load_InPt(DTItemID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetDiagnosisICD9Items_Load_InPt(DTItemID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisICD9Items_Load_InPt(DTItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisICD9Items_Load_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_ICD9ITEMS_CANNOT_GET_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateDiseaseProgression(DiagnosisTreatment entity, bool IsUpdate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateDiseaseProgression(entity, IsUpdate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateDiseaseProgression. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        #region 3.PMRs
        public bool DeletePMR(PatientMedicalRecord entity)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DeletePMR(entity);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.DeletePMR(entity);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DeletePMR(entity);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateDiagnosisTreatment. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMP_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddPMR(long ptID, string ptRecBarCode)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddPMR(ptID, ptRecBarCode);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.AddPMR(ptID, ptRecBarCode);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddPMR(ptID, ptRecBarCode);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddPMR. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdatePMR(PatientMedicalRecord entity)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdatePMR(entity);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.UpdatePMR(entity);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdatePMR(entity);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdatePMR. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientMedicalRecord> GetAllPMRsByPtID(long? patientID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //IncludeExpiredPMR	0: Get all PMRs, 1: Only retrieve openning PMR 
                int IncludeExpiredPMR = 0;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRsByPtID(patientID, IncludeExpiredPMR);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetPMRsByPtID(patientID, IncludeExpiredPMR);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRsByPtID(patientID, IncludeExpiredPMR);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllPMRsByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientMedicalRecord> GetOpeningPMRsByPtID(long? patientID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //IncludeExpiredPMR	0: Get all PMRs, 1: Only retrieve openning PMR 
                int IncludeExpiredPMR = 1;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRsByPtID(patientID, IncludeExpiredPMR);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetPMRsByPtID(patientID, IncludeExpiredPMR);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRsByPtID(patientID, IncludeExpiredPMR);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOpeningPMRsByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientMedicalRecord> GetExpiredPMRsByPtID(long? patientID)
        {
            try
            {
                //IncludeExpiredPMR	0: Get all PMRs, 1: Only retrieve openning PMR 
                int IncludeExpiredPMR = 2;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRsByPtID(patientID, IncludeExpiredPMR);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetPMRsByPtID(patientID, IncludeExpiredPMR);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRsByPtID(patientID, IncludeExpiredPMR);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetExpiredPMRsByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }



        public IList<PatientMedicalRecord> GetPMRsByPtID(long? patientID, int? inclExpiredPMR)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRsByPtID(patientID, inclExpiredPMR);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetPMRsByPtID(patientID, inclExpiredPMR);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRsByPtID(patientID, inclExpiredPMR);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPMRsByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public PatientMedicalRecord GetPMRByPtID(long? patientID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRByPtID(patientID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetPMRByPtID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetPMRByPtID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPMRByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientMedicalRecord PatientMedicalRecords_ByPatientID(long patientID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalRecords_ByPatientID(patientID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.PatientMedicalRecords_ByPatientID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalRecords_ByPatientID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientMedicalRecords_ByPatientID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }



        public bool PatientMedicalRecords_Save(long PatientRecID, long PatientID, string NationalMedicalCode, out string Error)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalRecords_Save(PatientRecID, PatientID, NationalMedicalCode, out Error);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.PatientMedicalRecords_Save(PatientRecID, PatientID, NationalMedicalCode, out Error);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalRecords_Save(PatientRecID, PatientID, NationalMedicalCode, out Error);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientMedicalRecords_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_SAVE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }



        #endregion

        #region thao tac tren file

        public PatientMedicalFile PatientMedicalFiles_ByID(long ServiceRecID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_ByID(ServiceRecID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.PatientMedicalFiles_ByID(ServiceRecID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_ByID(ServiceRecID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientMedicalFiles_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientMedicalFile> PatientMedicalFiles_ByPatientRecID(long PatientRecID)
        {
            try
            {
                DateTime? tempDate = null;
                IList<PatientMedicalFile> lstPMFile;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    lstPMFile = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_ByPatientRecID(PatientRecID);
                //}
                //else
                //{
                //    lstPMFile = ePMRsProvider.Instance.PatientMedicalFiles_ByPatientRecID(PatientRecID);
                //}
                lstPMFile = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_ByPatientRecID(PatientRecID);
                foreach (var item in lstPMFile)
                {
                    item.FinishedDate = tempDate;
                    tempDate = item.RecCreatedDate;
                }
                return lstPMFile;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientMedicalFiles_ByPatientRecID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool CheckExists_PatientMedicalFiles(long ServiceRecID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.CheckExists_PatientMedicalFiles(ServiceRecID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.CheckExists_PatientMedicalFiles(ServiceRecID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.CheckExists_PatientMedicalFiles(ServiceRecID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving CheckExists_PatientMedicalFiles. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool Insert_PatientMedicalFiles(PatientMedicalFile entity, out long PatientRecID)
        {
            PatientRecID = 0;
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.Insert_PatientMedicalFiles(entity, out PatientRecID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.Insert_PatientMedicalFiles(entity, out PatientRecID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.Insert_PatientMedicalFiles(entity, out PatientRecID);
            }
            catch (SqlException ex)
            {
                //if (ex.Message.StartsWith("Cannot insert duplicate key"))
                //{
                //    //code
                //    return false;
                //}
                //else
                //{
                AxLogger.Instance.LogInfo("End of retrieving Insert_PatientMedicalFiles. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                //}
            }
        }

        public bool PatientMedicalFiles_Update(PatientMedicalFile entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_Update(entity);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.PatientMedicalFiles_Update(entity);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_Update(entity);
            }
            catch (SqlException ex)
            {
                //if (ex.Message.StartsWith("Cannot insert duplicate key"))
                //{
                //    //code
                //    return false;
                //}
                //else
                //{
                AxLogger.Instance.LogInfo("End of retrieving PatientMedicalFiles_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                //}
            }
        }

        public bool PatientMedicalFiles_Delete(PatientMedicalFile entity, long staffID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_Delete(entity, staffID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.PatientMedicalFiles_Delete(entity, staffID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_Delete(entity, staffID);
            }
            catch (SqlException ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientMedicalFiles_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool PatientMedicalFiles_Active(PatientMedicalFile entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_Active(entity);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.PatientMedicalFiles_Active(entity);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.PatientMedicalFiles_Active(entity);
            }
            catch (SqlException ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientMedicalFiles_Active. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_ACTIVE);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool WriteFileXML(string path, string FileName, List<StringXml> Contents)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.WriteFileXML(path, FileName, Contents);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.WriteFileXML(path, FileName, Contents);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.WriteFileXML(path, FileName, Contents);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving WriteFileXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<StringXml> ReadFileXML(string FullPath)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.ReadFileXML(FullPath);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.ReadFileXML(FullPath);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.ReadFileXML(FullPath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving WriteFileXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion


        //-----Dinh them
        #region patient service

        public List<PatientRegistrationDetailEx> GetPatientRegistrationDetailsByRoom(out int totalCount, SeachPtRegistrationCriteria SeachRegCriteria)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetPatientRegistrationDetailsByRoom(out totalCount, SeachRegCriteria);
                //}
                //else
                //{
                //    return PatientProvider.Instance.GetPatientRegistrationDetailsByRoom(out totalCount, SeachRegCriteria);
                //}
                return aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetPatientRegistrationDetailsByRoom(out totalCount, SeachRegCriteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving insert GetPatientRegistrationDetailsByRoom. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        private static string FormatField(string data, string format)
        {
            switch (format)
            {
                case "xls":
                    return String.Format("<Cell><Data ss:Type=\"String" +
                       "\">{0}</Data></Cell>", data);
                case "CSV":
                    return String.Format("\"{0}\"",
                      data.Replace("\"", "\"\"\"").Replace("\n",
                      "").Replace("\r", ""));
            }
            return data;
        }

        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields, string strFormat)
        {
            switch (strFormat)
            {
                case "xls":
                    strBuilder.AppendLine("<Row>");
                    strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
                    strBuilder.AppendLine("</Row>");
                    break;
                case "CSV":
                    strBuilder.AppendLine(String.Join(",", lstFields.ToArray()));
                    break;
            }
        }

        private string ExportAll(List<List<string>> listData, string sheetName, string fileName, string fileExt)
        {
            try
            {
                if (listData != null)
                {
                    string strFormat = fileExt.Substring(fileExt.IndexOf('.') + 1).ToLower();
                    StringBuilder strBuilder = new StringBuilder();
                    List<string> lstFields = new List<string>();
                    //BuildStringOfRow(strBuilder, columnNames);
                    foreach (List<string> data in listData)
                    {
                        lstFields.Clear();
                        foreach (var col in data)
                        {
                            lstFields.Add(FormatField(col, strFormat));
                        }
                        BuildStringOfRow(strBuilder, lstFields, strFormat);
                    }
                    fileName += fileExt;
                    StreamWriter sw = new StreamWriter(fileName);
                    if (strFormat == "xls")
                    {
                        //Let us write the headers for the Excel XML
                        sw.WriteLine("<?xml version=\"1.0\" " +
                                        "encoding=\"utf-8\"?>");
                        sw.WriteLine("<?mso-application progid" +
                                        "=\"Excel.Sheet\"?>");
                        sw.WriteLine("<Workbook xmlns=\"urn:" +
                                        "schemas-microsoft-com:office:spreadsheet\">");
                        sw.WriteLine("<DocumentProperties " +
                                        "xmlns=\"urn:schemas-microsoft-com:" +
                                        "office:office\">");
                        //sw.WriteLine("<Author>" + Globals.LoggedUserAccount.Staff.FullName.Trim() + "</Author>");
                        sw.WriteLine("<Created>" +
                                        DateTime.Now.ToLocalTime().ToLongDateString() +
                                        "</Created>");
                        sw.WriteLine("<LastSaved>" +
                                        DateTime.Now.ToLocalTime().ToLongDateString() +
                                        "</LastSaved>");
                        sw.WriteLine("<Company>Viện Tim</Company>");
                        sw.WriteLine("<Version>12.00</Version>");
                        sw.WriteLine("</DocumentProperties>");
                        sw.WriteLine("<Worksheet ss:Name=\"" + sheetName + "\" " +
                            "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                        sw.WriteLine("<Table>");
                    }
                    sw.Write(strBuilder.ToString());
                    if (strFormat == "xls")
                    {
                        sw.WriteLine("</Table>");
                        sw.WriteLine("</Worksheet>");
                        sw.WriteLine("</Workbook>");
                    }
                    sw.Close();
                }
                return fileName;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return "";
            }
        }

        public Stream ExportToExcelBangKeChiTietKhamBenh(SeachPtRegistrationCriteria SeachRegCriteria, string StoreName, string ShowTitle)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Export To Excel Bang Ke Chi Tiet Kham Benh List.", CurrentUser);
                List<List<string>> res;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    res = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.ExportToExcelBangKeChiTietKhamBenh(SeachRegCriteria);
                //}
                //else
                //{
                //    res = PatientProvider.Instance.ExportToExcelBangKeChiTietKhamBenh(SeachRegCriteria);
                //}
                res = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.ExportToExcelBangKeChiTietKhamBenh(SeachRegCriteria);
                string folderPath = Globals.AxServerSettings.Servers.ExcelStorePool + "\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                if (!Directory.Exists(folderPath))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(folderPath);
                }

                var filePath = ExportAll(res, "Bang_Ke_Chi_Tiet_Kham_Benh", folderPath + "\\" + StoreName, ShowTitle);
                return CommonFunction.GetVideoAndImage(filePath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Export To Excel Bang Ke Chi Tiet Kham Benh. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONSULTATION_CANNOT_EXPORT_EXCEL_BANGKECHITIETKB, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientServicesTree> GetPatientServicesTreeView(long patientID, bool IsCriterion_PCLResult, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                List<PatientServicesTree> listDay = new List<PatientServicesTree>();
                //lay danh sach ServiceRecID
                //IList<PatientServiceRecord> lstPSR;
                IList<TreatmentHistory> lstTH;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    lstPSR = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientServiceRecord(patientID);
                //}
                //else
                //{
                //    lstPSR = ePMRsProvider.Instance.GetAllPatientServiceRecord(patientID);
                //}
                //lstPSR = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientServiceRecord(patientID, FromDate,  ToDate);
                lstTH = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTreatmentHistoriesByPatientID(patientID, IsCriterion_PCLResult, FromDate,  ToDate);
                //Lay danh sach noi tru
                //List<PatientRegistration> lstPR = new List<PatientRegistration>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    lstPR = aEMR.DataAccessLayer.Providers.CommonProvider.PatientRg.GetAllPatientRegistration_ByRegType(patientID, (long)AllLookupValues.RegistrationType.NOI_TRU);
                //}
                //else
                //{
                //    lstPR = CommonProvider.PatientRg.GetAllPatientRegistration_ByRegType(patientID, (long)AllLookupValues.RegistrationType.NOI_TRU);
                //}
                //lstPR = aEMR.DataAccessLayer.Providers.CommonProvider.PatientRg.GetAllPatientRegistration_ByRegType(patientID, (long)AllLookupValues.RegistrationType.NOI_TRU);
                //add ngay dau tien vao
                //if (lstPSR != null && lstPSR.Count > 0)
                //{
                //    //PatientServicesTree genericItem = new PatientServicesTree();
                //    //genericItem = new PatientServicesTree(lstPSR.FirstOrDefault().ExamDate);
                //    //listDay.Add(genericItem);
                //    //them cac ngay kham khac vao
                //    foreach (var item in lstPSR)
                //    {
                //        if (listDay.Count < 1 ||
                //            (item.ExamDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))//neu Khac ngay
                //        {
                //            PatientServicesTree dayItem = new PatientServicesTree();
                //            dayItem = new PatientServicesTree(item.ExamDate);
                //            listDay.Add(dayItem);
                //        }
                //        if (listDay.LastOrDefault().Children == null)
                //        {
                //            listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                //        }
                //        listDay.LastOrDefault().Children.Add(FindChildren(item, listDay.LastOrDefault()));
                //    }
                //}
                // lay xong tat ca cac ngay di kham
                //kiem tra cac dich vu trong ngay
                //List<PatientServicesTree> listPatientServicesTree = new List<PatientServicesTree>();
                //listPatientServicesTree.Clear();
                //List<PatientServicesTree> results = new List<PatientServicesTree>();
                //foreach (PatientRegistration item in lstPR)
                //{
                //    PatientServicesTree genericItem = new PatientServicesTree();
                //    genericItem = new PatientServicesTree(item.PtRegistrationID, item.ExamDate.ToShortDateString()
                //        , ""
                //        , 1
                //        , null, null, "PatientRegistration");
                //    listPatientServicesTree.Add(genericItem);
                //}

                //listPatientServicesTree.Sort(IComparable<PatientServicesTree>);

                // Get all the first level nodes. In our case it is only one - House M.D.
                //var rootNodes = listPatientServicesTree.Where(x => x.ParentID == null);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works
                //foreach (PatientServicesTree node in rootNodes)
                //{
                //    if (FindChildren(node))
                //        results.Add(node);
                //}
                if (lstTH != null && lstTH.Count > 0)
                {
                    foreach (var item in lstTH)
                    {
                        if (listDay.Count < 1 ||
                            (item.AdmissionDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))
                        {
                            PatientServicesTree dayItem = new PatientServicesTree();
                            dayItem = new PatientServicesTree(item.AdmissionDate,item.InPt);
                            listDay.Add(dayItem);
                            if (listDay.LastOrDefault().Children == null)
                            {
                                listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                            }
                            if (!IsCriterion_PCLResult)
                            {
                                FindChildren_New(lstTH.Where(x => x.AdmissionDate == item.AdmissionDate).ToList(), listDay);
                            }
                            else
                            {
                                FindChildren_Criterion_PCLResult(lstTH.Where(x => x.AdmissionDate == item.AdmissionDate).ToList(), listDay);
                            }   
                        }

                    }
                }
               
                return listDay;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllModules. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_MODULES);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        private void FindChildren_Criterion_PCLResult(List<TreatmentHistory> lstHistory, List<PatientServicesTree> listDay)
        {
            PatientServicesTree pstHA = new PatientServicesTree();
            pstHA.Children = new List<PatientServicesTree>();
            pstHA.InPt = listDay.LastOrDefault().InPt;
            foreach (var item in lstHistory)
            {
                if (item.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                {
                    PatientServicesTree subItem = new PatientServicesTree();
                    pstHA.NodeText = "CLS Hình Ảnh";
                    subItem = new PatientServicesTree(0, item.MedServiceName
                                                        , ""
                                                        , 2
                                                        , (int)AllLookupValues.PatientSummary.CanLamSang_TieuChiNV
                                                        , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                    pstHA.Children.Add(subItem);
                }
            }
            if (listDay.LastOrDefault().Children == null)
            {
                listDay.LastOrDefault().Children = new List<PatientServicesTree>();
            }
            if (pstHA.Children.Count > 0)
            {
                listDay.LastOrDefault().Children.Add(pstHA);
            }

            PatientServicesTree pstXN = new PatientServicesTree();
            pstXN.Children = new List<PatientServicesTree>();
            pstXN.InPt = listDay.LastOrDefault().InPt;
            foreach (var item in lstHistory)
            {
                if (item.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    PatientServicesTree subItem = new PatientServicesTree();
                    pstXN.NodeText = "CLS Xét Nghiệm";
                    subItem = new PatientServicesTree(0, item.MedServiceName
                                                        , ""
                                                        , 2
                                                        , (int)AllLookupValues.PatientSummary.CanLamSang_TieuChiNV
                                                        , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                    subItem.IsWarning = item.IsWarning;
                    pstXN.Children.Add(subItem);
                }
            }
            if (listDay.LastOrDefault().Children == null)
            {
                listDay.LastOrDefault().Children = new List<PatientServicesTree>();
            }
            if (pstXN.Children.Count > 0)
            {
                pstXN.Children = pstXN.InPt ? pstXN.Children.OrderByDescending(x => x.NodeText).ToList() : pstXN.Children.ToList();
                listDay.LastOrDefault().Children.Add(pstXN);
            }

        }
        private void FindChildren_New(List<TreatmentHistory> lstHistory,List<PatientServicesTree> listDay)
        {
            PatientServicesTree pstHA = new PatientServicesTree();
            pstHA.Children = new List<PatientServicesTree>();
            pstHA.InPt = listDay.LastOrDefault().InPt;
            foreach (var item in lstHistory)
            {
                if (item.PatientPCLReqID > 0 && item.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                {
                    PatientServicesTree subItem = new PatientServicesTree();
                    pstHA.NodeText = "CLS Hình Ảnh";
                    subItem = new PatientServicesTree(item.PatientPCLReqID, item.MedServiceName
                                                        , ""
                                                        , 2
                                                        , (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh
                                                        , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                    pstHA.Children.Add(subItem);
                }
            }
            if (listDay.LastOrDefault().Children == null)
            {
                listDay.LastOrDefault().Children = new List<PatientServicesTree>();
            }
            if (pstHA.Children.Count > 0)
            {
                listDay.LastOrDefault().Children.Add(pstHA);
            }

            PatientServicesTree pstXN = new PatientServicesTree();
            pstXN.Children = new List<PatientServicesTree>();
            pstXN.InPt = listDay.LastOrDefault().InPt;
            foreach (var item in lstHistory)
            {
                if (item.PatientPCLReqID > 0 && item.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    PatientServicesTree subItem = new PatientServicesTree();
                    pstXN.NodeText = "CLS Xét Nghiệm";
                    subItem = new PatientServicesTree(item.PatientPCLReqID, item.MedServiceName
                                                        , ""
                                                        , 2
                                                        , (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem
                                                        , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                    subItem.IsWarning = item.IsWarning;
                    pstXN.Children.Add(subItem);
                }
            }
            if (listDay.LastOrDefault().Children == null)
            {
                listDay.LastOrDefault().Children = new List<PatientServicesTree>();
            }
            if (pstXN.Children.Count > 0)
            {
                pstXN.Children = pstXN.InPt ? pstXN.Children.OrderByDescending(x => x.NodeText).ToList() : pstXN.Children.ToList();
                listDay.LastOrDefault().Children.Add(pstXN);
            }
         


            PatientServicesTree pstKB = new PatientServicesTree();
            pstKB.Children = new List<PatientServicesTree>();
            pstKB.InPt = listDay.LastOrDefault().InPt;
            foreach (var item in lstHistory)
            {
                if (item.DTItemID > 0)
                {
                    PatientServicesTree subItem = new PatientServicesTree(item.DTItemID, item.MedServiceName
                        , item.PtRegistrationID.ToString()
                        , 2
                        , (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan
                        , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                    pstKB.NodeText = "Khám Bệnh";
                    pstKB.Children.Add(subItem);
                }
                if (item.PrescriptID > 0)
                {
                    PatientServicesTree subItem = new PatientServicesTree((long)item.PrescriptID, item.MedServiceName
                        , ""
                        , 2
                        , (int)AllLookupValues.PatientSummary.ToaThuoc
                        , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                    pstKB.NodeText = "Khám Bệnh";
                    pstKB.Children.Add(subItem);
                }
                if (item.SmallProcedureID > 0)
                {
                    PatientServicesTree subItem = new PatientServicesTree((long)item.SmallProcedureID, item.MedServiceName
                        , ""
                        , 2
                        , (int)AllLookupValues.PatientSummary.ThuThuat
                        , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                    pstKB.NodeText = "Khám Bệnh";
                    pstKB.Children.Add(subItem);
                }
            }
            if (listDay.LastOrDefault().Children == null)
            {
                listDay.LastOrDefault().Children = new List<PatientServicesTree>();
            }
            if (pstKB.Children.Count > 0)
            {
                listDay.LastOrDefault().Children.Add(pstKB);
            }

            PatientServicesTree pstHC = new PatientServicesTree();
            pstHC.Children = new List<PatientServicesTree>();
            pstHC.InPt = listDay.LastOrDefault().InPt;
            foreach (var item in lstHistory)
            {
                if (item.DiagConsultationSummaryID > 0)
                {
                    PatientServicesTree subItem = new PatientServicesTree((long)item.DiagConsultationSummaryID, item.MedServiceName
                        , item.PtRegistrationID.ToString()
                        , 2
                        , (int)AllLookupValues.PatientSummary.HoiChan
                        , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                    pstHC.NodeText = "Hội chẩn";
                    pstHC.Children.Add(subItem);
                }

            }
            if (listDay.LastOrDefault().Children == null)
            {
                listDay.LastOrDefault().Children = new List<PatientServicesTree>();
            }
            if (pstHC.Children.Count > 0)
            {
                listDay.LastOrDefault().Children.Add(pstHC);
            }
        }
        private bool FindChildren(PatientServicesTree item)
        {
            try
            {
                //foreach (PatientServicesTree par in listPatientServicesTree)
                {
                    long serRecID = item.NodeID;
                    if (item.Code == "PatientServiceRecord")
                    {
                        item.Children = new List<PatientServicesTree>();
                        PatientServicesTree listTempPatientServicesTree = new PatientServicesTree();
                        //----Kiem tra thang PCL Request
                        IList<PatientPCLRequest> familyPatientPCLRequest;
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, 0);
                        //}
                        //else
                        //{
                        //    familyPatientPCLRequest = ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, 0);
                        //}
                        familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, 0);
                        foreach (PatientPCLRequest itemF in familyPatientPCLRequest)
                        {
                            PatientServicesTree subItem = new PatientServicesTree();
                            //child.Children.Add(subItem);
                            PatientServicesTree genericItem = new PatientServicesTree();
                            if (itemF.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                            {
                                subItem = new PatientServicesTree(itemF.PatientPCLReqID, "Cận Lâm Sàng Hình Ảnh"//itemF.PCLFormName
                                , itemF.ObjPCLResultParamImpID.ParamName
                                , 2
                                , (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh
                                , item, itemF.ServiceRecID, item.Code);
                            }
                            else
                            {
                                subItem = new PatientServicesTree(itemF.PatientPCLReqID, "Cận Lâm Sàng Xét Nghiệm"//itemF.PCLFormName
                                , ""
                                , 2
                                , (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem
                                , item, itemF.ServiceRecID, item.Code);
                            }
                            item.Children.Add(subItem);
                        }

                        //----Kiem tra thang Chan doan va Kham benh
                        DiagnosisTreatment curDiagnosisTreatment = new DiagnosisTreatment();
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    curDiagnosisTreatment = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                        //}
                        //else
                        //{
                        //    curDiagnosisTreatment = ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                        //}
                        curDiagnosisTreatment = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                        PatientServicesTree subIt = new PatientServicesTree();
                        //child.Children.Add(subItem);
                        if (curDiagnosisTreatment.DTItemID > 0)
                        {
                            subIt = new PatientServicesTree(curDiagnosisTreatment.DTItemID, "Khám Bệnh"//curDiagnosisTreatment.Diagnosis
                                , curDiagnosisTreatment.DiagnosisFinal
                                , 2
                                , (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan
                                , item, curDiagnosisTreatment.ServiceRecID, item.Code);
                            item.Children.Add(subIt);
                        }
                        //----Kiem tra thang Ra Toa

                        IList<PrescriptionIssueHistory> pIH = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                        foreach (PrescriptionIssueHistory itemF in pIH)
                        {
                            PatientServicesTree subItem = new PatientServicesTree();
                            //child.Children.Add(subItem);
                            subItem = new PatientServicesTree((long)itemF.PrescriptID, "Toa Thuốc"
                                , ""
                                , 2
                                , (int)AllLookupValues.PatientSummary.ToaThuoc
                                , item, itemF.ServiceRecID, item.Code);
                            item.Children.Add(subItem);
                        }
                    }
                    else
                    {
                        //----Kiem tra thang Noi Tru

                        InPatientAdmDisDetails pIH = new InPatientAdmDisDetails();
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    pIH = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetInPatientAdmDisDetails(serRecID);
                        //}
                        //else
                        //{
                        //    pIH = ePMRsProvider.Instance.GetInPatientAdmDisDetails(serRecID);
                        //}
                        pIH = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetInPatientAdmDisDetails(serRecID);
                        if (pIH != null && pIH.PtRegistrationID > 0)
                        {
                            PatientServicesTree subItem = new PatientServicesTree();
                            //child.Children.Add(subItem);
                            subItem = new PatientServicesTree((long)pIH.InPatientAdmDisDetailID, "Nội Trú"
                                , ""
                                , 2
                                , (int)AllLookupValues.PatientSummary.NoiTru
                                , item, serRecID, item.Code);
                            item.Children.Add(subItem);
                        }
                    }

                    if (item.Children.Count < 1)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }
            return true;
        }

        private PatientServicesTree FindChildren_Old(PatientServiceRecord item, PatientServicesTree dayItem)
        {
            PatientServicesTree pst = new PatientServicesTree();
            try
            {
                long serRecID = item.ServiceRecID;
                {
                    pst.Children = new List<PatientServicesTree>();
                    PatientServicesTree listTempPatientServicesTree = new PatientServicesTree();
                    //----Kiem tra thang PCL Request
                    IList<PatientPCLRequest> familyPatientPCLRequest;
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, 0);
                    //}
                    //else
                    //{
                    //    familyPatientPCLRequest = ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, 0);
                    //}
                    familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, 0);
                    long pclRequestID = 0;
                    foreach (PatientPCLRequest itemF in familyPatientPCLRequest)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();

                        //child.Children.Add(subItem);
                        PatientServicesTree genericItem = new PatientServicesTree();
                        if (itemF.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                        {
                            pst.NodeText = "CLS Hình Ảnh";
                            subItem = new PatientServicesTree(itemF.PatientPCLReqID, itemF.PCLExamTypeName //"CLS Hình Ảnh"//itemF.PCLFormName
                            , itemF.ObjPCLResultParamImpID.ParamName
                            , 2
                            , (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh
                            , dayItem, itemF.ServiceRecID, "PatientServiceRecord");
                        }
                        else
                        {
                            if (pclRequestID != itemF.PatientPCLReqID)
                            {
                                pst.NodeText = "CLS Xét Nghiệm";
                                subItem = new PatientServicesTree(itemF.PatientPCLReqID, itemF.PCLRequestNumID//itemF.PCLExamTypeName//itemF.PCLFormName
                                , itemF.PCLExamTypeName
                                , 2
                                , (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem
                                , dayItem, itemF.ServiceRecID, "PatientServiceRecord");
                                pclRequestID = itemF.PatientPCLReqID;
                            }
                        }
                        pst.Children.Add(subItem);
                    }

                    //----Kiem tra thang Chan doan va Kham benh

                    DiagnosisTreatment curDiagnosisTreatment = new DiagnosisTreatment();
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    curDiagnosisTreatment = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                    //}
                    //else
                    //{
                    //    curDiagnosisTreatment = ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                    //}
                    curDiagnosisTreatment = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                    PatientServicesTree subIt = new PatientServicesTree();
                    //child.Children.Add(subItem);
                    if (curDiagnosisTreatment.DTItemID > 0)
                    {
                        subIt = new PatientServicesTree(curDiagnosisTreatment.DTItemID, "Chẩn Đoán"//curDiagnosisTreatment.Diagnosis
                            , curDiagnosisTreatment.DiagnosisFinal
                            , 2
                            , (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan
                            , dayItem, curDiagnosisTreatment.ServiceRecID, "PatientServiceRecord");
                        pst.NodeText = "Khám Bệnh";
                        pst.Children.Add(subIt);
                    }
                    //----Kiem tra thang Ra Toa

                    IList<PrescriptionIssueHistory> pIH;
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    pIH = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                    //}
                    //else
                    //{
                    //    pIH = ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                    //}
                    pIH = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                    foreach (PrescriptionIssueHistory itemF in pIH)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();
                        //child.Children.Add(subItem);
                        subItem = new PatientServicesTree((long)itemF.PrescriptID, "Toa Thuốc"
                            , ""
                            , 2
                            , (int)AllLookupValues.PatientSummary.ToaThuoc
                            , dayItem, itemF.ServiceRecID, "PatientServiceRecord");
                        pst.NodeText = "Khám Bệnh";
                        pst.Children.Add(subItem);
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }
            return pst;
        }

        private PatientServicesTree FindChildren(PatientServiceRecord item, PatientServicesTree dayItem)
        {
            PatientServicesTree pst = new PatientServicesTree();
            PatientServicesTree pstPCLExt = new PatientServicesTree();
            try
            {
                long serRecID = item.ServiceRecID;
                long ptRegistrationID = item.PtRegistrationID.Value;
                {
                    pst.Children = new List<PatientServicesTree>();
                    PatientServicesTree listTempPatientServicesTree = new PatientServicesTree();

                    //----Kiem tra thang Chan doan va Kham benh

                    DiagnosisTreatment curDiagnosisTreatment = new DiagnosisTreatment();
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    curDiagnosisTreatment = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                    //}
                    //else
                    //{
                    //    curDiagnosisTreatment = ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                    //}
                    curDiagnosisTreatment = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                    PatientServicesTree subIt = new PatientServicesTree();
                    //child.Children.Add(subItem);
                    if (curDiagnosisTreatment.DTItemID > 0)
                    {
                        subIt = new PatientServicesTree(curDiagnosisTreatment.DTItemID, "Chẩn Đoán"//curDiagnosisTreatment.Diagnosis
                            , curDiagnosisTreatment.DiagnosisFinal
                            , 2
                            , (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan
                            , dayItem, curDiagnosisTreatment.ServiceRecID, "PatientServiceRecord");
                        pst.NodeText = "Khám Bệnh";
                        pst.Children.Add(subIt);
                    }
                    //----Kiem tra thang Ra Toa

                    IList<PrescriptionIssueHistory> pIH;
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    pIH = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                    //}
                    //else
                    //{
                    //    pIH = ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                    //}
                    pIH = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                    foreach (PrescriptionIssueHistory itemF in pIH)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();
                        //child.Children.Add(subItem);
                        subItem = new PatientServicesTree((long)itemF.PrescriptID, "Toa Thuốc"
                            , ""
                            , 2
                            , (int)AllLookupValues.PatientSummary.ToaThuoc
                            , dayItem, itemF.ServiceRecID, "PatientServiceRecord");
                        pst.NodeText = "Khám Bệnh";
                        pst.Children.Add(subItem);
                    }

                    //----Kiem tra thang cls ngoai vien

                    IList<PatientPCLRequest_Ext> pPCLExt;
                    //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    //{
                    //    pPCLExt = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetPCLRequestListExtByRegistrationID(ptRegistrationID);
                    //}
                    //else
                    //{
                    //    pPCLExt = PatientProvider.Instance.GetPCLRequestListExtByRegistrationID(ptRegistrationID);
                    //}
                    pPCLExt = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetPCLRequestListExtByRegistrationID(ptRegistrationID);
                    pstPCLExt.Children = new List<PatientServicesTree>();
                    foreach (PatientPCLRequest_Ext itemF in pPCLExt)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();

                        //child.Children.Add(subItem);
                        subItem = new PatientServicesTree((long)itemF.PatientPCLReqExtID, itemF.PCLRequestNumID
                            , ""
                            , 2
                            , (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh_NgoaiVien
                            , dayItem, itemF.PtRegistrationID, "PatientServiceRecord");
                        pstPCLExt.NodeText = "CLS hình ảnh Ngoại Viện";
                        pstPCLExt.Children.Add(subItem);
                    }
                }

                //----Kiem tra thang PCL Request
                PatientServicesTree pst1 = new PatientServicesTree();
                pst1.Children = new List<PatientServicesTree>();

                PatientServicesTree pst2 = new PatientServicesTree();
                pst2.Children = new List<PatientServicesTree>();

                IList<PatientPCLRequest> familyPatientPCLRequest;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, 0);
                //}
                //else
                //{
                //    familyPatientPCLRequest = ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, 0);
                //}
                familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, 0);
                long pclRequestID = 0;
                foreach (PatientPCLRequest itemF in familyPatientPCLRequest)
                {
                    PatientServicesTree subItem = new PatientServicesTree();

                    //child.Children.Add(subItem);
                    PatientServicesTree genericItem = new PatientServicesTree();
                    if (itemF.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                    {
                        pst1.NodeText = "CLS Hình Ảnh";
                        subItem = new PatientServicesTree(itemF.PatientPCLReqID, itemF.PCLExamTypeName //"CLS Hình Ảnh"//itemF.PCLFormName
                        , itemF.ObjPCLResultParamImpID.ParamName
                        , 2
                        , (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh
                        , dayItem, itemF.ServiceRecID, "PatientServiceRecord");

                        pst1.Children.Add(subItem);
                    }
                    else
                    {
                        if (pclRequestID != itemF.PatientPCLReqID)
                        {
                            pst2.NodeText = "CLS Xét Nghiệm";
                            subItem = new PatientServicesTree(itemF.PatientPCLReqID, itemF.PCLRequestNumID//itemF.PCLExamTypeName//itemF.PCLFormName
                            , itemF.PCLExamTypeName
                            , 2
                            , (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem
                            , dayItem, itemF.ServiceRecID, "PatientServiceRecord");
                            pclRequestID = itemF.PatientPCLReqID;

                            pst2.Children.Add(subItem);
                        }
                    }
                }
                if (pst.Children.Count > 0)
                {
                    if (pst1.Children.Count > 0)
                    {
                        dayItem.Children.Add(pst1);
                    }
                    if (pst2.Children.Count > 0)
                    {
                        dayItem.Children.Add(pst2);
                    }
                    if (pstPCLExt.Children.Count > 0)
                    {
                        dayItem.Children.Add(pstPCLExt);
                    }
                }
                else
                {
                    if (pst1.Children.Count > 0)
                    {
                        pst = pst1;
                        if (pst2.Children.Count > 0)
                        {
                            dayItem.Children.Add(pst2);
                        }
                        if (pstPCLExt.Children.Count > 0)
                        {
                            dayItem.Children.Add(pstPCLExt);
                        }
                    }
                    else if (pst2.Children.Count > 0)
                    {
                        pst = pst2;
                        if (pstPCLExt.Children.Count > 0)
                        {
                            dayItem.Children.Add(pstPCLExt);
                        }
                    }
                    else if (pstPCLExt.Children.Count > 0)
                    {
                        pst = pstPCLExt;
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }
            return pst;
        }

        public List<PatientServicesTree> GetPatientServicesTreeViewEnum(long patientID, int PatientSummaryEnum, bool IsCriterion_PCLResult, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                List<PatientServicesTree> listDay = new List<PatientServicesTree>();
                //lay danh sach ServiceRecID
                //IList<PatientServiceRecord> lstPSR;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    lstPSR = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientServiceRecord(patientID);
                //}
                //else
                //{
                //    lstPSR = ePMRsProvider.Instance.GetAllPatientServiceRecord(patientID);
                //}
                //lstPSR = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientServiceRecord(patientID, FromDate,  ToDate);
                //Lay danh sach noi tru
                //List<PatientRegistration> lstPR = aEMR.DataAccessLayer.Providers.CommonProvider.PatientRg.GetAllPatientRegistration_ByRegType(patientID, (long)AllLookupValues.RegistrationType.NOI_TRU);
                ////add ngay dau tien vao
                //if (lstPSR != null && lstPSR.Count > 0)
                //{
                //    PatientServicesTree genericItem = new PatientServicesTree();
                //    genericItem = new PatientServicesTree(lstPSR.FirstOrDefault().ExamDate);
                //    listDay.Add(genericItem);
                //    //them cac ngay kham khac vao
                //    foreach (var item in lstPSR)
                //    {
                //        PatientServicesTree temp = FindChildren(item, listDay.LastOrDefault(), PatientSummaryEnum);
                //        if (temp != null && temp.NodeText != "")
                //        {
                //            if (listDay.Count < 1 ||
                //                (item.ExamDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))//neu Khac ngay
                //            {
                //                PatientServicesTree dayItem = new PatientServicesTree();
                //                dayItem = new PatientServicesTree(item.ExamDate);
                //                listDay.Add(dayItem);
                //            }
                //            if (listDay.LastOrDefault().Children == null)
                //            {
                //                listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                //            }
                //            listDay.LastOrDefault().Children.Add(temp);
                //        }
                //    }
                //}


                //foreach (PatientRegistration item in lstPR)
                //{
                //    PatientServicesTree genericItem = new PatientServicesTree();
                //    genericItem = new PatientServicesTree(item.PtRegistrationID, item.ExamDate.ToShortDateString()
                //        , ""
                //        , 1
                //        , null, null, "PatientRegistration");
                //    listPatientServicesTree.Add(genericItem);
                //}
                //// Get all the first level nodes. In our case it is only one - House M.D.
                //var rootNodes = listPatientServicesTree.Where(x => x.ParentID == null);

                //// Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                //// see bellow how the FindChildren method works
                //foreach (PatientServicesTree node in rootNodes)
                //{
                //    if (FindChildren(node, PatientSummaryEnum))
                //        results.Add(node);
                //}
                IList<TreatmentHistory> lstTH;
                lstTH = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTreatmentHistoriesByPatientID(patientID, IsCriterion_PCLResult, FromDate, ToDate);
                switch (PatientSummaryEnum)
                {
                    case (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh:
                        if (lstTH != null && lstTH.Count > 0)
                        {
                            foreach (var item in lstTH.Where(x=>x.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging))
                            {
                                if (listDay.Count < 1 ||
                                    (item.AdmissionDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))
                                {
                                    PatientServicesTree dayItem = new PatientServicesTree();
                                    dayItem = new PatientServicesTree(item.AdmissionDate,item.InPt);
                                    listDay.Add(dayItem);
                                    FindChildren_New(lstTH.Where(x => x.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging && x.AdmissionDate == item.AdmissionDate).ToList(), 
                                        listDay, (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh);
                                }
                            }
                        }
                        break;
                    case (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem:
                        if (lstTH != null && lstTH.Count > 0)
                        {
                            foreach (var item in lstTH.Where(x => x.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory))
                            {
                                if (listDay.Count < 1 ||
                                    (item.AdmissionDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))
                                {
                                    PatientServicesTree dayItem = new PatientServicesTree();
                                    dayItem = new PatientServicesTree(item.AdmissionDate, item.InPt);
                                    listDay.Add(dayItem);
                                    FindChildren_New(lstTH.Where(x => x.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory && x.AdmissionDate == item.AdmissionDate).ToList(),
                                      listDay, (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem);
                                }
                            }
                        }
                        break;
                    case (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan:
                        if (lstTH != null && lstTH.Count > 0)
                        {
                            foreach (var item in lstTH.Where(x => x.DTItemID > 0))
                            {
                                if (listDay.Count < 1 ||
                                    (item.AdmissionDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))
                                {
                                    PatientServicesTree dayItem = new PatientServicesTree();
                                    dayItem = new PatientServicesTree(item.AdmissionDate, item.InPt);
                                    listDay.Add(dayItem);
                                    FindChildren_New(lstTH.Where(x => x.DTItemID > 0 && x.AdmissionDate == item.AdmissionDate).ToList(),
                                      listDay, (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan);
                                }
                            }
                        }
                        break;
                    case (int)AllLookupValues.PatientSummary.ToaThuoc:
                        if (lstTH != null && lstTH.Count > 0)
                        {
                            foreach (var item in lstTH.Where(x => x.PrescriptID > 0))
                            {
                                if (listDay.Count < 1 ||
                                    (item.AdmissionDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))
                                {
                                    PatientServicesTree dayItem = new PatientServicesTree();
                                    dayItem = new PatientServicesTree(item.AdmissionDate, item.InPt);
                                    listDay.Add(dayItem);
                                    FindChildren_New(lstTH.Where(x => x.PrescriptID > 0 && x.AdmissionDate == item.AdmissionDate).ToList(),
                                     listDay, (int)AllLookupValues.PatientSummary.ToaThuoc);
                                }
                            }
                        }
                        break;
                    case (int)AllLookupValues.PatientSummary.NoiTru:
                        if (lstTH != null && lstTH.Count > 0)
                        {
                            foreach (var item in lstTH.Where(x => x.InPt))
                            {
                                if (listDay.Count < 1 ||
                                    (item.AdmissionDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))
                                {
                                    PatientServicesTree dayItem = new PatientServicesTree();
                                    dayItem = new PatientServicesTree(item.AdmissionDate, item.InPt);
                                    listDay.Add(dayItem);
                                    FindChildren_New(lstTH.Where(x => x.InPt && x.AdmissionDate == item.AdmissionDate).ToList(),
                                     listDay, (int)AllLookupValues.PatientSummary.NoiTru);
                                }
                            }
                        }
                        break;
                     case (int)AllLookupValues.PatientSummary.ThuThuat:
                        if (lstTH != null && lstTH.Count > 0)
                        {
                            foreach (var item in lstTH.Where(x => x.SmallProcedureID>0))
                            {
                                if (listDay.Count < 1 ||
                                    (item.AdmissionDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))
                                {
                                    PatientServicesTree dayItem = new PatientServicesTree();
                                    dayItem = new PatientServicesTree(item.AdmissionDate, item.InPt);
                                    listDay.Add(dayItem);
                                    FindChildren_New(lstTH.Where(x => x.SmallProcedureID >0 && x.AdmissionDate == item.AdmissionDate).ToList(),
                                     listDay, (int)AllLookupValues.PatientSummary.ThuThuat);
                                }
                            }
                        }
                        break;
                    case (int)AllLookupValues.PatientSummary.HoiChan:
                        if (lstTH != null && lstTH.Count > 0)
                        {
                            foreach (var item in lstTH.Where(x => x.DiagConsultationSummaryID>0))
                            {
                                if (listDay.Count < 1 ||
                                    (item.AdmissionDate.ToShortDateString() != listDay.LastOrDefault().ExamDate.ToShortDateString()))
                                {
                                    PatientServicesTree dayItem = new PatientServicesTree();
                                    dayItem = new PatientServicesTree(item.AdmissionDate, item.InPt);
                                    listDay.Add(dayItem);
                                    FindChildren_New(lstTH.Where(x => x.DiagConsultationSummaryID > 0 && x.AdmissionDate == item.AdmissionDate).ToList(),
                                     listDay, (int)AllLookupValues.PatientSummary.HoiChan);
                                }
                            }
                        }
                        break;
                   
                    default:
                        break;
                }
                return listDay;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllModules. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.USER_ACCOUNT_GET_ALL_MODULES);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        private void FindChildren_New(List<TreatmentHistory> lstHistory, List<PatientServicesTree> listDay, int PatientSummaryEnum)
        {
            PatientServicesTree pst = new PatientServicesTree();
            pst.Children = new List<PatientServicesTree>();
            pst.InPt = listDay.LastOrDefault().InPt;
            switch (PatientSummaryEnum)
            {
                case (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh:
                    foreach (var item in lstHistory)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();
                        pst.NodeText = "CLS Hình Ảnh";
                        subItem = new PatientServicesTree(item.PatientPCLReqID, item.MedServiceName
                                                                , ""
                                                                , 2
                                                                , (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh
                                                                , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                        pst.Children.Add(subItem);
                    }
                    if (listDay.LastOrDefault().Children == null)
                    {
                        listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                    }
                    if (pst.Children.Count > 0)
                    {
                        listDay.LastOrDefault().Children.Add(pst);
                    }
                    break;
                case (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem:
                    foreach (var item in lstHistory)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();
                        pst.NodeText = "CLS Xét Nghiệm";
                        subItem = new PatientServicesTree(item.PatientPCLReqID, item.MedServiceName
                                                                , ""
                                                                , 2
                                                                , (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem
                                                                , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                        pst.IsWarning = item.IsWarning;
                        pst.Children.Add(subItem);
                    }
                    if (listDay.LastOrDefault().Children == null)
                    {
                        listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                    }
                    if (pst.Children.Count > 0)
                    {
                        pst.Children = pst.InPt ? pst.Children.OrderByDescending(x=>x.NodeText).ToList() : pst.Children.ToList();
                        listDay.LastOrDefault().Children.Add(pst);
                    }
                    break;
                case (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan:
                    foreach (var item in lstHistory)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();
                        pst.NodeText = "Khám bệnh";
                        subItem = new PatientServicesTree(item.DTItemID, item.MedServiceName
                                                                , ""
                                                                , 2
                                                                , (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan
                                                                , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                        pst.Children.Add(subItem);
                    }
                    if (listDay.LastOrDefault().Children == null)
                    {
                        listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                    }
                    if (pst.Children.Count > 0)
                    {
                        listDay.LastOrDefault().Children.Add(pst);
                    }
                    break;
                case (int)AllLookupValues.PatientSummary.ToaThuoc:
                    foreach (var item in lstHistory)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();
                        pst.NodeText = "Khám bệnh";
                        subItem = new PatientServicesTree(item.PrescriptID, item.MedServiceName
                                                                , ""
                                                                , 2
                                                                , (int)AllLookupValues.PatientSummary.ToaThuoc
                                                                , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                        pst.Children.Add(subItem);
                    }
                    if (listDay.LastOrDefault().Children == null)
                    {
                        listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                    }
                    if (pst.Children.Count > 0)
                    {
                        listDay.LastOrDefault().Children.Add(pst);
                    }
                    break;
                case (int)AllLookupValues.PatientSummary.ThuThuat:
                    foreach (var item in lstHistory)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();
                        pst.NodeText = "Khám bệnh";
                        subItem = new PatientServicesTree((long)item.SmallProcedureID, item.MedServiceName
                                                                , ""
                                                                , 2
                                                                , (int)AllLookupValues.PatientSummary.ThuThuat
                                                                , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                        pst.Children.Add(subItem);
                    }
                    if (listDay.LastOrDefault().Children == null)
                    {
                        listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                    }
                    if (pst.Children.Count > 0)
                    {
                        listDay.LastOrDefault().Children.Add(pst);
                    }
                    break;
                case (int)AllLookupValues.PatientSummary.HoiChan:
                    foreach (var item in lstHistory)
                    {
                        PatientServicesTree subItem = new PatientServicesTree();
                        pst.NodeText = "Hội chẩn";
                        subItem = new PatientServicesTree((long)item.DiagConsultationSummaryID, item.MedServiceName
                                                                , ""
                                                                , 2
                                                                , (int)AllLookupValues.PatientSummary.HoiChan
                                                                , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                        pst.Children.Add(subItem);
                    }
                    if (listDay.LastOrDefault().Children == null)
                    {
                        listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                    }
                    if (pst.Children.Count > 0)
                    {
                        listDay.LastOrDefault().Children.Add(pst);
                    }
                    break;
                case (int)AllLookupValues.PatientSummary.NoiTru:
                    //foreach (var item in lstHistory)
                    //{
                    //    PatientServicesTree subItem = new PatientServicesTree();
                    //    pst.NodeText = "Khám bệnh";
                    //    subItem = new PatientServicesTree(item.PatientPCLReqID, item.MedServiceName
                    //                                            , ""
                    //                                            , 2
                    //                                            , (int)AllLookupValues.PatientSummary.NoiTru
                    //                                            , listDay.LastOrDefault(), item.ServiceRecID, "PatientServiceRecord");
                    //    pst.Children.Add(subItem);
                    //}
                    //if (listDay.LastOrDefault().Children == null)
                    //{
                    //    listDay.LastOrDefault().Children = new List<PatientServicesTree>();
                    //}
                    //if (pst.Children.Count > 0)
                    //{
                    //    listDay.LastOrDefault().Children.Add(pst);
                    //}
                    FindChildren_New(lstHistory,listDay);
                    break;

                default:
                    break;
            }
        }
        private PatientServicesTree FindChildren(PatientServiceRecord item, PatientServicesTree dayItem, int PatientSummaryEnum)
        {
            PatientServicesTree pst = new PatientServicesTree();
            pst.Children = new List<PatientServicesTree>();
            try
            {
                long serRecID = item.ServiceRecID;

                {
                    if (PatientSummaryEnum == (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh)
                    {
                        pst.Children = new List<PatientServicesTree>();
                        PatientServicesTree listTempPatientServicesTree = new PatientServicesTree();
                        //----Kiem tra thang PCL Request
                        IList<PatientPCLRequest> familyPatientPCLRequest;
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, (int)AllLookupValues.V_PCLMainCategory.Imaging);
                        //}
                        //else
                        //{
                        //    familyPatientPCLRequest = ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, (int)AllLookupValues.V_PCLMainCategory.Imaging);
                        //}
                        familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, (int)AllLookupValues.V_PCLMainCategory.Imaging);
                        if (familyPatientPCLRequest == null || familyPatientPCLRequest.Count < 1)
                        {
                            return null;
                        }
                        pst.NodeText = "CLS Hình Ảnh";
                        foreach (PatientPCLRequest itemF in familyPatientPCLRequest)
                        {
                            PatientServicesTree subItem = new PatientServicesTree();
                            //child.Children.Add(subItem);
                            PatientServicesTree genericItem = new PatientServicesTree();
                            subItem = new PatientServicesTree(itemF.PatientPCLReqID, itemF.PCLExamTypeName //itemF.PCLFormName
                                                              , itemF.ObjPCLResultParamImpID.ParamName
                                                              , 2
                                                              , (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh
                                                              , dayItem, itemF.ServiceRecID, dayItem.Code);
                            pst.Children.Add(subItem);

                            //IList<PatientPCLImagingResult> allPatientPCLImagingResult =
                            //    PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByPatientPCLReqID(
                            //        itemF.PatientPCLReqID);
                            //if (allPatientPCLImagingResult.Count > 0)
                            //{
                            //    PatientServicesTree ssubItem = new PatientServicesTree();
                            //    ssubItem = new PatientServicesTree(itemF.PatientPCLReqID, "Kết Quả Hình Ảnh"
                            //        //itemF.PCLFormName
                            //                                       , ""
                            //                                       , 3
                            //                                       , (int)AllLookupValues.PatientSummary.CanLamSang
                            //                                       , item, itemF.ServiceRecID, item.Code);
                            //    item.Children[item.Children.Count - 1].Children = new List<PatientServicesTree>();

                            //    item.Children[item.Children.Count - 1].Children.Add(ssubItem);
                            //}
                        }

                    }
                    else
                        if (PatientSummaryEnum == (int)AllLookupValues.PatientSummary.CanLamSang_XetNghiem)
                    {
                        pst.Children = new List<PatientServicesTree>();
                        PatientServicesTree listTempPatientServicesTree = new PatientServicesTree();
                        //----Kiem tra thang PCL Request
                        IList<PatientPCLRequest> familyPatientPCLRequest;
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, (int)AllLookupValues.V_PCLMainCategory.Laboratory);
                        //}
                        //else
                        //{
                        //    familyPatientPCLRequest = ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, (int)AllLookupValues.V_PCLMainCategory.Laboratory);
                        //}
                        familyPatientPCLRequest = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllPatientPCLReqServiceRecID(serRecID, (int)AllLookupValues.V_PCLMainCategory.Laboratory);
                        if (familyPatientPCLRequest == null || familyPatientPCLRequest.Count < 1)
                        {
                            return null;
                        }
                        pst.NodeText = "CLS Xét Nghiệm";
                        foreach (PatientPCLRequest itemF in familyPatientPCLRequest)
                        {
                            PatientServicesTree subItem = new PatientServicesTree();
                            //child.Children.Add(subItem);
                            PatientServicesTree genericItem = new PatientServicesTree();
                            subItem = new PatientServicesTree(itemF.PatientPCLReqID, itemF.PCLRequestNumID
                                                                //itemF.PCLFormName
                                                                , itemF.PCLExamTypeName
                                                                , 2
                                                                ,
                                                                (int)
                                                                AllLookupValues.PatientSummary.CanLamSang_XetNghiem
                                                                , dayItem, itemF.ServiceRecID, dayItem.Code);
                            pst.Children.Add(subItem);
                        }
                    }
                    else
                            if (PatientSummaryEnum == (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh_NgoaiVien)
                    {
                        pst.Children = new List<PatientServicesTree>();
                        PatientServicesTree listTempPatientServicesTree = new PatientServicesTree();
                        //----Kiem tra thang PCL Request
                        List<PatientPCLRequest_Ext> pPCLExt = new List<PatientPCLRequest_Ext>();
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    pPCLExt = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetPCLRequestListExtByRegistrationID(item.PtRegistrationID.Value);
                        //}
                        //else
                        //{
                        //    pPCLExt = PatientProvider.Instance.GetPCLRequestListExtByRegistrationID(item.PtRegistrationID.Value);
                        //}
                        pPCLExt = aEMR.DataAccessLayer.Providers.PatientProvider.Instance.GetPCLRequestListExtByRegistrationID(item.PtRegistrationID.Value);
                        if (pPCLExt == null || pPCLExt.Count < 1)
                        {
                            return null;
                        }
                        pst.NodeText = "CLS hình ảnh Ngoại Viện";
                        foreach (PatientPCLRequest_Ext itemF in pPCLExt)
                        {
                            PatientServicesTree subItem = new PatientServicesTree();
                            PatientServicesTree genericItem = new PatientServicesTree();
                            subItem = new PatientServicesTree((long)itemF.PatientPCLReqExtID, itemF.PCLRequestNumID
                                , ""
                                , 2
                                , (int)AllLookupValues.PatientSummary.CanLamSang_HinhAnh_NgoaiVien
                                , dayItem, itemF.PtRegistrationID, dayItem.Code);
                            pst.Children.Add(subItem);
                        }

                        //----Kiem tra thang cls ngoai vien
                    }

                    //----Kiem tra thang Chan doan va Kham benh
                    if (PatientSummaryEnum == (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan)
                    {
                        DiagnosisTreatment curDiagnosisTreatment = new DiagnosisTreatment();
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    curDiagnosisTreatment = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                        //}
                        //else
                        //{
                        //    curDiagnosisTreatment = ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                        //}
                        curDiagnosisTreatment = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisTreatmentsBySerRecID(serRecID);
                        if (curDiagnosisTreatment == null || curDiagnosisTreatment.DTItemID < 1)
                        {
                            return null;
                        }
                        pst.NodeText = "Khám Bệnh";
                        PatientServicesTree subIt = new PatientServicesTree();
                        subIt = new PatientServicesTree(curDiagnosisTreatment.DTItemID, "Chẩn Đoán"
                                                            //curDiagnosisTreatment.Diagnosis
                                                            , curDiagnosisTreatment.DiagnosisFinal
                                                            , 2
                                                            , (int)AllLookupValues.PatientSummary.KhamBenh_ChanDoan
                                                            , dayItem, curDiagnosisTreatment.ServiceRecID, dayItem.Code);
                        pst.Children.Add(subIt);
                    }



                    //----Kiem tra thang Ra Toa
                    if (PatientSummaryEnum == (int)AllLookupValues.PatientSummary.ToaThuoc)
                    {
                        IList<PrescriptionIssueHistory> pIH;
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    pIH = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                        //}
                        //else
                        //{
                        //    pIH = ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                        //}
                        pIH = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.GetPrescriptionIssueHistoryBySerRecID(serRecID);
                        if (pIH == null || pIH.Count < 1)
                        {
                            return null;
                        }

                        pst.NodeText = "Khám Bệnh";
                        foreach (PrescriptionIssueHistory itemF in pIH)
                        {
                            PatientServicesTree subItem = new PatientServicesTree();
                            //child.Children.Add(subItem);
                            subItem = new PatientServicesTree((long)itemF.PrescriptID, "Toa Thuốc"
                                                              , ""
                                                              , 2
                                                              , (int)AllLookupValues.PatientSummary.ToaThuoc
                                                              , dayItem, itemF.ServiceRecID, dayItem.Code);
                            pst.Children.Add(subItem);
                        }
                    }
                }
                //else
                {
                    //----Kiem tra thang Noi Tru
                    if (PatientSummaryEnum == (int)AllLookupValues.PatientSummary.NoiTru)
                    {
                        InPatientAdmDisDetails pIH = new InPatientAdmDisDetails();
                        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                        //{
                        //    pIH = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetInPatientAdmDisDetails(serRecID);
                        //}
                        //else
                        //{
                        //    pIH = ePMRsProvider.Instance.GetInPatientAdmDisDetails(serRecID);
                        //}
                        pIH = aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetInPatientAdmDisDetails(serRecID);
                        if (pIH == null || pIH.PtRegistrationID < 1)
                        {
                            return null;
                        }
                        PatientServicesTree subItem = new PatientServicesTree();
                        //child.Children.Add(subItem);
                        subItem = new PatientServicesTree((long)pIH.InPatientAdmDisDetailID, "Nội Trú"
                                                              , ""
                                                              , 2
                                                              , (int)AllLookupValues.PatientSummary.NoiTru
                                                              , dayItem, serRecID, dayItem.Code);
                        pst.Children.Add(subItem);
                    }
                }

                //foreach (PatientServicesTree op in par.Children)
                //{
                //    op.Children = new List<PatientServicesTree>();
                //    PatientServicesTree listOPPatientServicesTree = new PatientServicesTree();
                //    IList<Operation> familyOperation = UserAccounts.Instance.GetAllOperationsByFuncID(op.NodeID);
                //    foreach (Operation itemO in familyOperation)
                //    {
                //        PatientServicesTree subItem = new PatientServicesTree();
                //        //child.Children.Add(subItem);
                //        PatientServicesTree genericItem = new PatientServicesTree();
                //        subItem = new PatientServicesTree(itemO.OperationID, itemO.OperationName, itemO.Description, 3, itemO.Enum, op, itemO.FunctionID, item.Code);
                //        op.Children.Add(subItem);
                //    }
                //}

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }
            return pst;
        }
   
        public bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.InsertInPatientAdmDisDetails(entity);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.InsertInPatientAdmDisDetails(entity);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.InsertInPatientAdmDisDetails(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving insert InPatientAdmDisDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.InPatientAdmDisDetails_insert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateInPatientAdmDisDetails(entity);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.UpdateInPatientAdmDisDetails(entity);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateInPatientAdmDisDetails(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving update InPatientAdmDisDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.InPatientAdmDisDetails_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public InPatientAdmDisDetails GetInPatientAdmDisDetails(long PtRegistrationID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetInPatientAdmDisDetails(PtRegistrationID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetInPatientAdmDisDetails(PtRegistrationID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetInPatientAdmDisDetails(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving get all InPatientAdmDisDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.InPatientAdmDisDetails_Get);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //private void FindChildrenOperation(PatientServicesTree item)
        //{
        //    try
        //    {
        //        foreach (PatientServicesTree par in listPatientServicesTree)
        //        {

        //            //par.Children = new List<PatientServicesTree>();
        //            //PatientServicesTree listTempPatientServicesTree = new PatientServicesTree();
        //            //IList<Function> familyFunction = UserAccounts.Instance.GetAllFunction(par.NodeID);

        //            //foreach (Function itemF in familyFunction)
        //            //{
        //            //    PatientServicesTree subItem = new PatientServicesTree();
        //            //    //child.Children.Add(subItem);
        //            //    PatientServicesTree genericItem = new PatientServicesTree();
        //            //    subItem = new PatientServicesTree(itemF.FunctionID, itemF.FunctionName, itemF.FunctionDescription, 2, itemF.eNum, par, itemF.ModuleID, item.Code);
        //            //    par.Children.Add(subItem);
        //            //}
        //            ////item.Children.Add(child);

        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}




        #endregion
        #region TreatmentProcess
        public TreatmentProcess SaveTreatmentProcess(TreatmentProcess entity)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving SaveTreatmentProcess", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.SaveTreatmentProcess(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveTreatmentProcess. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public TreatmentProcess GetTreatmentProcessByPtRegistrationID(long PtRegistrationID)/*TMA*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetTreatmentProcess", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTreatmentProcessByPtRegistrationID(PtRegistrationID);/*TMA*/
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetTreatmentProcessByPtRegistrationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteTreatmentProcess(long TreatmentProcessID, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving DeleteTreatmentProcess", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DeleteTransferForm(TransferFormID, StaffID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.DeleteTransferForm(TransferFormID, StaffID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DeleteTreatmentProcess(TreatmentProcessID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteTreatmentProcess. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        #region Giấy chuyển tuyến
        public TransferForm SaveTransferForm(TransferForm entity, long StaffID) //==== #010
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving SaveTransferForm", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.SaveTransferForm(entity);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.SaveTransferForm(entity);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.SaveTransferForm(entity, StaffID); //==== #010
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveTransferForm. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<TransferForm> GetTransferForm(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID)/*TMA*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetTransferForm", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTransferForm(TextCriterial, FindBy, V_TransferFormType, V_PatientFindBy, TransferFormID);/*TMA*/
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetTransferForm(TextCriterial, FindBy, V_TransferFormType, V_PatientFindBy, TransferFormID);/*TMA*/
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTransferForm(TextCriterial, FindBy, V_TransferFormType, V_PatientFindBy, TransferFormID);/*TMA*/
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetTransferForm. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<TransferForm> GetTransferForm_Date(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID, DateTime FromDate, DateTime ToDate)/*TMA*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetTransferForm", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTransferForm_Date(TextCriterial, FindBy, V_TransferFormType, V_PatientFindBy, TransferFormID, FromDate, ToDate);/*TMA*/
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetTransferForm_Date(TextCriterial, FindBy, V_TransferFormType, V_PatientFindBy, TransferFormID, FromDate, ToDate);/*TMA*/
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTransferForm_Date(TextCriterial, FindBy, V_TransferFormType, V_PatientFindBy, TransferFormID, FromDate, ToDate);/*TMA*/
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetTransferForm. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteTransferForm(long TransferFormID, long StaffID, string DeletedReason) //==== #009
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving DeleteTransferForm", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DeleteTransferForm(TransferFormID, StaffID);
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.DeleteTransferForm(TransferFormID, StaffID);
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DeleteTransferForm(TransferFormID, StaffID, DeletedReason); //==== #009
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteTransferForm. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public TransferForm GetTransferFormByPtRegistrationID(long PtRegistrationID)/*TMA*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetTransferForm", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTransferForm(TextCriterial, FindBy, V_TransferFormType, V_PatientFindBy, TransferFormID);/*TMA*/
                //}
                //else
                //{
                //    return ePMRsProvider.Instance.GetTransferForm(TextCriterial, FindBy, V_TransferFormType, V_PatientFindBy, TransferFormID);/*TMA*/
                //}
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTransferFormByPtRegistrationID(PtRegistrationID);/*TMA*/
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetTransferFormByPtRegistrationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion Giấy chuyển tuyến

        #region Other Func
        public DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(long? InPtRegistrationID, long? V_DiagnosisType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetLatestDiagnosisTreatmentByPtID_InPt_ForDiag.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(InPtRegistrationID, V_DiagnosisType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLatestDiagnosisTreatmentByPtID_InPt_ForDiag. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        public MedicalExaminationResult GetMedicalExaminationResultByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetMedicalExaminationResultByPtRegistrationID(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetMedicalExaminationResultByPtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public MedicalExaminationResult GetMedicalExaminationResultByPtRegDetailID(long PtRegDetailID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetMedicalExaminationResultByPtRegDetailID(PtRegDetailID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetMedicalExaminationResultByPtRegDetailID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void UpdateMedicalExaminationResult(MedicalExaminationResult aMedicalExaminationResult)
        {

            try
            {
                aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateMedicalExaminationResult(aMedicalExaminationResult);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetMedicalExaminationResultByPtRegDetailID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼==== #011
        public void DeleteMedicalExaminationResult(MedicalExaminationResult aMedicalExaminationResult)
        {

            try
            {
                aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.DeleteMedicalExaminationResult(aMedicalExaminationResult);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteMedicalExaminationResult. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲==== #011

        public IList<TreatmentHistory> GetTreatmentHistoriesByPatientID(long PatientID, bool IsCriterion_PCLResult, DateTime? ToDate, DateTime? FromDate)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTreatmentHistoriesByPatientID(PatientID, IsCriterion_PCLResult, ToDate, FromDate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetTreatmentHistoriesByPatientID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<TreatmentHistory> GetTreatmentHistoriesByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTreatmentHistoriesByPtRegistrationID(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetTreatmentHistoriesByPtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼===== #004
        public bool AddUpdateDiagnosysConsultation(DiagnosysConsultationSummary gDiagConsultation, List<Staff> SurgeryDoctorCollection, List<DiagnosisIcd10Items> refICD10List, bool isUpdate, out long DiagConsultationSummaryID)
        {
            try
            {
                DiagConsultationSummaryID = 0;
                if (!isUpdate)
                {
                    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddDiagnosysConsultation(gDiagConsultation, SurgeryDoctorCollection, refICD10List, out DiagConsultationSummaryID);
                }
                else
                {
                    return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateDiagnosysConsultation(gDiagConsultation, SurgeryDoctorCollection, refICD10List, out DiagConsultationSummaryID);
                }
            }
            catch (Exception ex)
            {
                if (!isUpdate)
                {
                    AxLogger.Instance.LogInfo("End of retrieving AddDiagnosysConsultation. Status: Failed.", CurrentUser);
                }
                else
                {
                    AxLogger.Instance.LogInfo("End of retrieving UpdateDiagnosysConsultation. Status: Failed.", CurrentUser);
                }
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public DiagnosysConsultationSummary LoadDiagnosysConsultationSummary(long DiagConsultationSummaryID, out List<Staff> StaffList, out List<DiagnosisIcd10Items> ICD10List)
        {
            try
            {
                ICD10List = new List<DiagnosisIcd10Items>();
                StaffList = new List<Staff>();
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.LoadDiagnosysConsultationSummary(DiagConsultationSummaryID, out StaffList, out ICD10List);
            }
            catch (Exception ex)
            {

                AxLogger.Instance.LogInfo("End of retrieving LoadDiagnosysConsultationSummary. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== #004
        public TransferForm GetTransferFormByID(long TransferFormID, int V_PatientFindBy)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetTransferFormByID.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetTransferFormByID(TransferFormID,V_PatientFindBy);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetTransferFormByID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_ADD_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼====: #001
        public IList<RequiredSubDiseasesReferences> GetListRequiredSubDiseasesReferences(string MainICD10)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetListRequiredSubDiseasesReferences.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetListRequiredSubDiseasesReferences(MainICD10);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetListRequiredSubDiseasesReferences. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_ADD_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #001
        //▼====: #002
        public IList<RuleDiseasesReferences> GetListRuleDiseasesReferences(string MainICD10)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetListRuleDiseasesReferences.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetListRuleDiseasesReferences(MainICD10);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetListRuleDiseasesReferences. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_ADD_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #002
        public IList<PCLExamAccordingICD> GetListPCLExamAccordingICD(long PatientID, long V_SpecialistType, long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetListPCLExamAccordingICD.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetListPCLExamAccordingICD(PatientID, V_SpecialistType, PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetListPCLExamAccordingICD. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_ADD_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public AdmissionExamination GetAdmissionExamination(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving Get AdmissionExamination.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAdmissionExamination(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Get AdmissionExamination. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_ADD_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool SaveAdmissionExamination(AdmissionExamination admissionExamination, out long AdmissionExaminationID_New)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving Save AdmissionExamination.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.SaveAdmissionExamination(admissionExamination, out AdmissionExaminationID_New);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Save AdmissionExamination. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_ADD_INPT);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<DiagnosisTreatment> GetDiagAndDoctorInstruction_InPt_ByPtRegID(long PtRegistrationID, long? DTItemID, long? DeptID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagAndDoctorInstruction_InPt_ByPtRegID(PtRegistrationID, DTItemID, DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagAndDoctorInstruction_InPt_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOADID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void GetDataFromWebForInstruction_ByIntPtDiagDrInstructionID(long IntPtDiagDrInstructionID, out string Disease_Progression, out string Diet, out long V_LevelCare,
             out string PCLExamTypeList, out IList<RefMedicalServiceGroupItem> MedServiceList, out string DrugList)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDataFromWebForInstruction_ByIntPtDiagDrInstructionID(IntPtDiagDrInstructionID, out Disease_Progression
                    , out Diet, out V_LevelCare, out PCLExamTypeList, out MedServiceList, out DrugList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDataFromWebForInstruction_ByIntPtDiagDrInstructionID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_LOADID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼==== #003
        public void UpdateConclusionMedicalExaminationResult(MedicalExaminationResult Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.UpdateConclusionMedicalExaminationResult(Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateConclusionMedicalExaminationResult. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_INPT_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲==== #003  
        //▼==== #004
        public SelfDeclaration GetSelfDeclarationByPatientID(long PatientID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetSelfDeclarationByPatientID(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSelfDeclarationByPatientID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetSelfDeclarationByPatientID");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public SelfDeclaration GetSelfDeclarationByPtRegistrationID(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetSelfDeclarationByPtRegistrationID(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSelfDeclarationByPtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error GetSelfDeclarationByPtRegistrationID");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool SaveSelfDeclaration(SelfDeclaration Obj, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.SaveSelfDeclaration(Obj, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveSelfDeclaration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error SaveSelfDeclaration");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲==== #004
        //▼==== #005
        public bool CheckBeforeUpdateDiagnosisTreatment(long PtRegDetailID, long DoctorStaffID, out string errorMessages, out string confirmMessages)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start CheckBeforeUpdateDiagnosisTreatment.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.CheckBeforeUpdateDiagnosisTreatment(PtRegDetailID, DoctorStaffID, out errorMessages, out confirmMessages);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End CheckBeforeUpdateDiagnosisTreatment. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, "Lõi khi kiểm tra định mức bàn khám", CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }
        //▲==== #005

        //▼==== #006
        public IList<DiagnosisICD9Items> GetDiagnosisICD9Items_Load(long DTItemID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetDiagnosisICD9Items_Load.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetDiagnosisICD9Items_Load(DTItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiagnosisICD9Items_Load. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_ICD9ITEMS_CANNOT_GET_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #006

        //▼==== #008
        public IList<TreatmentProcess> GetAllTreatmentProcessByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetAllTreatmentProcessByPtRegistrationID", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetAllTreatmentProcessByPtRegistrationID(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllTreatmentProcessByPtRegistrationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲==== #008
        public bool AddNewDiagDetal_KhoaSan(long PtRegistrationID, int? SoConChet, DateTime? NgayConChet)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving AddNewDiagDetal_KhoaSan", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.AddNewDiagDetal_KhoaSan(PtRegistrationID, SoConChet, NgayConChet);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddNewDiagDetal_KhoaSan. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool GetSoConChet_KhoaSan(long PtRegistrationID, out int? SoConChet, out DateTime? NgayConChet)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetSoConChet_KhoaSan", CurrentUser);
                return aEMR.DataAccessLayer.Providers.ePMRsProvider.Instance.GetSoConChet_KhoaSan(PtRegistrationID, out SoConChet, out NgayConChet);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSoConChet_KhoaSan. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DIAGNOSISTREATMENT_CANNOT_UPDATE_INPT);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
    }
}
