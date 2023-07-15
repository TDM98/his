using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;

using ErrorLibrary;
using System.Runtime.Serialization;

using AxLogging;

using ErrorLibrary.Resources;
using eHCMSLanguage;
using DataEntities.MedicalInstruction;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-11-25
 * Contents: Consultation Services Iterfaces - Common Utils
/*******************************************************************/
#endregion
/*
 * 20171002 #001 CMN: Added GetAllSugeriesByPtRegistrationID
 * 20210428 #002 BLQ: Added GetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID
 * 20220625 #003 DatTB: Thêm function lấy loại điều trị ngoại trú
 * 20220721 #004 DatTB: Validate dữ liệu trước khi gửi lên server
 * 20230411 #005 BLQ: Thêm lấy TT HSBA cho điều trị ngoại trú
 * 20230415 #006 QTD: Thêm giấy ra viện
 * 20230503 #007 DatTB: 
 * + Viết service get/insert/update tuổi động mạch
 * + Viết stored get/insert/update tuổi động mạch
*/
namespace ConsultationsService.Common
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class CommonUtilServices : eHCMS.WCFServiceCustomHeader, ICommonUtils
    {
        public CommonUtilServices()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        #region 1.Drugs
        public IList<RefGenericDrugDetail> SearchRefDrugNames(string brandName, long pageIndex, long pageSize, byte type)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchRefDrugNames(brandName, pageIndex, pageSize, type);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.SearchRefDrugNames(brandName, pageIndex, pageSize, type);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchRefDrugNames(brandName, pageIndex, pageSize, type);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchRefDrugNames. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DRUG_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 2.DiseaseReferences

        public IList<DiseasesReference> SearchRefDiseases(string searchKey, long pageIndex, long pageSize
            , byte type, long PatientID, DateTime curDatetime, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchRefDiseases(searchKey, pageIndex, pageSize, type, out Total);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.SearchRefDiseases(searchKey, pageIndex, pageSize, type, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchRefDiseases(searchKey, pageIndex, pageSize, type, PatientID, curDatetime, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchRefDiseases. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DISEASES_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region RefICD9

        public IList<RefICD9> SearchRefICD9(string searchKey, long pageIndex, long pageSize, byte ICD9SearchType, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchRefICD9(searchKey, pageIndex, pageSize, ICD9SearchType, out Total);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.SearchRefICD9(searchKey, pageIndex, pageSize, ICD9SearchType, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchRefICD9(searchKey, pageIndex, pageSize, ICD9SearchType, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchRefICD9. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_ICD9_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 3.Staff
        public IList<Staff> SearchStaffFullName(string searchKey, long pageIndex, long pageSize)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchStaffFullName(searchKey, pageIndex, pageSize);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.SearchStaffFullName(searchKey, pageIndex, pageSize);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchStaffFullName(searchKey, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchStaffFullName. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Staff> SearchStaffCat(Staff Staff, long pageIndex, long pageSize)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchStaffCat(Staff, pageIndex, pageSize);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.SearchStaffCat(Staff, pageIndex, pageSize);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SearchStaffCat(Staff, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchStaffFullName. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Staff> GetAllStaffs()
        {
            try
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllStaffs.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetAllStaffs();
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetAllStaffs();
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetAllStaffs();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllStaffs. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Staff> GetAllStaffs_FromStaffID(long nFromStaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllStaffs.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetAllStaffs_FromStaffID(nFromStaffID);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetAllStaffs_FromStaffID(nFromStaffID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetAllStaffs_FromStaffID(nFromStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllStaffs. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 4.Department
        public RefDepartment GetRefDepartmentByLID(long locationID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetRefDepartmentByLID(locationID);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetRefDepartmentByLID(locationID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetRefDepartmentByLID(locationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefDepartmentByLID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DEPARTMENT_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        public List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh(PatientServiceRecord entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.PatientServiceRecordsGetForKhamBenh(entity);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.PatientServiceRecordsGetForKhamBenh(entity);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.PatientServiceRecordsGetForKhamBenh(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientServiceRecordsGetForKhamBenh. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh_InPt(PatientServiceRecord entity)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving PatientServiceRecordsGetForKhamBenh_InPt.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.PatientServiceRecordsGetForKhamBenh_InPt(entity);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.PatientServiceRecordsGetForKhamBenh_InPt(entity);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.PatientServiceRecordsGetForKhamBenh_InPt(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientServiceRecordsGetForKhamBenh_InPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PrescriptionIssueHistory> GetPrescriptionIssueHistoriesInPtBySerRecID(long ServiceRecID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetPrescriptionIssueHistoriesInPtBySerRecID(ServiceRecID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPrescriptionIssueHistoriesInPtBySerRecID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #region ConsultingDiagnosys
        public bool UpdateConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys, out long ConsultingDiagnosysID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving UpdateConsultingDiagnosys.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.UpdateConsultingDiagnosys(aConsultingDiagnosys, out ConsultingDiagnosysID);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.UpdateConsultingDiagnosys(aConsultingDiagnosys, out ConsultingDiagnosysID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.UpdateConsultingDiagnosys(aConsultingDiagnosys, out ConsultingDiagnosysID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateConsultingDiagnosys. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public ConsultingDiagnosys GetConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetConsultingDiagnosys.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetConsultingDiagnosys(aConsultingDiagnosys);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetConsultingDiagnosys(aConsultingDiagnosys);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetConsultingDiagnosys(aConsultingDiagnosys);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetConsultingDiagnosys. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<ConsultingDiagnosys> GetReportConsultingDiagnosys(ConsultingDiagnosysSearchCriteria aSearchCriteria, out int TotalItemCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetReportConsultingDiagnosys.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetReportConsultingDiagnosys(aSearchCriteria, out TotalItemCount);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetReportConsultingDiagnosys(aSearchCriteria, out TotalItemCount);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetReportConsultingDiagnosys(aSearchCriteria, out TotalItemCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetReportConsultingDiagnosys. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<SurgerySchedule> GetSurgerySchedules()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetSurgerySchedules.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetSurgerySchedules();
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetSurgerySchedules();
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetSurgerySchedules();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSurgerySchedules. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<SurgeryScheduleDetail> GetSurgeryScheduleDetails(long ConsultingDiagnosysID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetSurgeryScheduleDetails.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetSurgeryScheduleDetails(ConsultingDiagnosysID);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetSurgeryScheduleDetails(ConsultingDiagnosysID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetSurgeryScheduleDetails(ConsultingDiagnosysID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSurgeryScheduleDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<SurgeryScheduleDetail_TeamMember> GetSurgeryScheduleDetail_TeamMembers(long ConsultingDiagnosysID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetSurgeryScheduleDetail_TeamMembers.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetSurgeryScheduleDetail_TeamMembers(ConsultingDiagnosysID);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetSurgeryScheduleDetail_TeamMembers(ConsultingDiagnosysID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetSurgeryScheduleDetail_TeamMembers(ConsultingDiagnosysID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSurgeryScheduleDetail_TeamMembers. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool EditSurgerySchedule(SurgerySchedule aEditSurgerySchedule, out long SurgeryScheduleID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving EditSurgerySchedule.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.EditSurgerySchedule(aEditSurgerySchedule, out SurgeryScheduleID);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.EditSurgerySchedule(aEditSurgerySchedule, out SurgeryScheduleID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.EditSurgerySchedule(aEditSurgerySchedule, out SurgeryScheduleID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving EditSurgerySchedule. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▼====: #001*/
        public List<RefMedicalServiceItem> GetAllSugeriesByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetAllSugeriesByPtRegistrationID.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetAllSugeriesByPtRegistrationID(PtRegistrationID);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetAllSugeriesByPtRegistrationID(PtRegistrationID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetAllSugeriesByPtRegistrationID(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllSugeriesByPtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool SaveCatastropheByPtRegDetailID(long PtRegDetailID, long V_CatastropheType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving SaveCatastropheByPtRegDetailID.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SaveCatastropheByPtRegDetailID(PtRegDetailID, V_CatastropheType);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.SaveCatastropheByPtRegDetailID(PtRegDetailID, V_CatastropheType);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.SaveCatastropheByPtRegDetailID(PtRegDetailID, V_CatastropheType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveCatastropheByPtRegDetailID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▲====: #001*/
        public DateTime? GetFirstExamDate(long PatientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetFirstExamDate.", CurrentUser);
                DateTime? MinExamDate;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetFirstExamDate(PatientID, out MinExamDate);
                //}
                //else
                //{
                //    CommonUtilsProvider.Instance.GetFirstExamDate(PatientID, out MinExamDate);
                //}
                aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetFirstExamDate(PatientID, out MinExamDate);
                return MinExamDate;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetFirstExamDate. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public DateTime? GetNextAppointment(long PatientID, long MedServiceID, DateTime CurentDate)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving GetNextAppointment.", CurrentUser);
                DateTime? ApptDate;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetNextAppointment(PatientID, MedServiceID, CurentDate, out ApptDate);
                //}
                //else
                //{
                //    CommonUtilsProvider.Instance.GetNextAppointment(PatientID, MedServiceID, CurentDate, out ApptDate);
                //}
                aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetNextAppointment(PatientID, MedServiceID, CurentDate, out ApptDate);
                return ApptDate;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetNextAppointment. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion
        public bool EditSmallProcedure(SmallProcedure aSmallProcedure, long StaffID, out long SmallProcedureID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.EditSmallProcedure(aSmallProcedure, StaffID, out SmallProcedureID);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.EditSmallProcedure(aSmallProcedure, StaffID, out SmallProcedureID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.EditSmallProcedure(aSmallProcedure, StaffID, out SmallProcedureID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving EditSmallProcedure. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public SmallProcedure GetSmallProcedure(long PtRegDetailID, long? SmallProcedureID, long? V_RegistrationType = null)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetSmallProcedure(PtRegDetailID, V_RegistrationType);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.GetSmallProcedure(PtRegDetailID, V_RegistrationType);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetSmallProcedure(PtRegDetailID, SmallProcedureID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving EditSmallProcedure. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public SmallProcedure GetLatesSmallProcedure(long PatientID, long MedServiceID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetLatesSmallProcedure(PatientID, MedServiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLatesSmallProcedure. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public int GetSmallProcedureTime(long MedServiceID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.GetSmallProcedureTime(MedServiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSmallProcedureTime. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool ChangeStatus(long PtRegDetailID, long StaffChangeStatus, string ReasonChangeStatus)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.ChangeStatus(PtRegDetailID, StaffChangeStatus, ReasonChangeStatus);
                //}
                //else
                //{
                //    return CommonUtilsProvider.Instance.ChangeStatus(PtRegDetailID, StaffChangeStatus, ReasonChangeStatus);
                //}
                return aEMR.DataAccessLayer.Providers.CommonUtilsProvider.Instance.ChangeStatus(PtRegDetailID, StaffChangeStatus, ReasonChangeStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving ChangeStatus. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public HistoryAndPhysicalExaminationInfo GetHistoryAndPhysicalExaminationInfoByPtRegDetailID(long PtRegDetailID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetHistoryAndPhysicalExaminationInfoByPtRegDetailID(PtRegDetailID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetHistoryAndPhysicalExaminationInfoByPtRegDetailID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼====: #002
        public HistoryAndPhysicalExaminationInfo GetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID(long OutPtTreatmentProgramID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID(OutPtTreatmentProgramID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #002

        public void EditHistoryAndPhysicalExaminationInfo(HistoryAndPhysicalExaminationInfo aItem, long StaffID, bool MaxillofacialTab, bool IsSaveSummary)
        {
            try
            {
                if (MaxillofacialTab)
                {
                    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.EditHistoryAndPhysicalExaminationInfo(aItem, StaffID);
                }
                else
                {
                    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.EditHistoryAndPhysicalExaminationInfo_V2(aItem, StaffID, IsSaveSummary);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving EditHistoryAndPhysicalExaminationInfo. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void SaveOutPtTreatmentProgram(OutPtTreatmentProgram Item)
        {
            try
            {
                //▼==== #004
                if (Item != null)
                {
                    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.SaveOutPtTreatmentProgram(Item);
                }
                //▲==== #004
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveOutPtTreatmentProgram. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<OutPtTreatmentProgram> GetOutPtTreatmentProgramCollectionByPatientID(long PatientID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetOutPtTreatmentProgramCollectionByPatientID(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetOutPtTreatmentProgramCollectionByPatientID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void UpdateTreatmentProgramIntoRegistration(long PtRegistrationID, long PtRegDetailID, long? OutPtTreatmentProgramID, out int OutPrescriptionsAmount)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateTreatmentProgramIntoRegistration(PtRegistrationID, PtRegDetailID, OutPtTreatmentProgramID, out OutPrescriptionsAmount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateTreatmentProgramIntoRegistration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<TicketCare> GetTicketCareByOutPtTreatmentProgramID(long OutPtTreatmentProgramID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetTicketCareByOutPtTreatmentProgramID(OutPtTreatmentProgramID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRegistrationByOutPtTreatmentProgramID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼==== #003
        public List<OutpatientTreatmentType> GetAllOutpatientTreatmentType()
        {
            try
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllOutpatientTreatmentType.", CurrentUser);
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetAllOutpatientTreatmentType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllOutpatientTreatmentType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #003
        public List<PatientRegistration> GetRegistrationByOutPtTreatmentProgramID(long OutPtTreatmentProgramID, bool IsDischargePapers)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRegistrationByOutPtTreatmentProgramID(OutPtTreatmentProgramID, IsDischargePapers);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRegistrationByOutPtTreatmentProgramID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool SaveOutPtTreatmentProgramItem(PatientRegistration Registration)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.SaveOutPtTreatmentProgramItem(Registration);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRegistrationByOutPtTreatmentProgramID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool OutPtTreatmentProgramMarkDeleted(OutPtTreatmentProgram Item)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.OutPtTreatmentProgramMarkDeleted(Item);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRegistrationByOutPtTreatmentProgramID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<Staff> GetNarcoticDoctorOfficial(string SearchName, long NarcoticDoctorStaffID, DateTime ProcedureDateTime, bool IsInPt)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetNarcoticDoctorOfficial(SearchName, NarcoticDoctorStaffID, ProcedureDateTime, IsInPt);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetNarcoticDoctorOfficial. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼==== #005
        public List<SummaryMedicalRecords> GetSummaryMedicalRecordByOutPtTreatmentProgramID(long OutPtTreatmentProgramID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetSummaryMedicalRecordByOutPtTreatmentProgramID(OutPtTreatmentProgramID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSummaryMedicalRecordByOutPtTreatmentProgramID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲==== #005  

        //▼==== #006
        public bool SaveDisChargePapersInfo(DischargePapersInfo DischargePapersInfo)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.SaveDisChargePapersInfo(DischargePapersInfo);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveDisChargePapersInfo. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public DischargePapersInfo GetDischargePapersInfo(long PtRegistrationID, long V_RegistrationType, out string DoctorAdvice)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetDischargePapersInfo(PtRegistrationID, V_RegistrationType, out DoctorAdvice);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDischargePapersInfo. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲==== #006

        //▼==== #007
        public bool SaveAgeOfTheArtery(AgeOfTheArtery obj, out long AgeOfTheArteryID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.SaveAgeOfTheArtery(obj, out AgeOfTheArteryID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveAgeOfTheArtery. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public bool UpdateAgeOfTheArtery(AgeOfTheArtery obj)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateAgeOfTheArtery(obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateAgeOfTheArtery. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public AgeOfTheArtery GetAgeOfTheArtery_ByPatient(long PtRegistrationID, long V_RegistrationType, long PatientClassID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetAgeOfTheArtery_ByPatient(PtRegistrationID, V_RegistrationType, PatientClassID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAgeOfTheArtery_ByPatient. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PSR_CANNOT_GET);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲==== #007
    }
}