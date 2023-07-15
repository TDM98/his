using System;
using System.Collections.Generic;

using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
using ErrorLibrary;
using System.Runtime.Serialization;

using AxLogging;

using ErrorLibrary.Resources;
using eHCMSLanguage;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-06
 * Contents: Consultation Services
/*******************************************************************/
#endregion
/*
 * 20182510 #001 TTM: BM 0002173: Thay đổi cách lưu/ cập nhật/ lấy dữ liệu của tình trạng thể chất từ lấy theo TTTC cuối cùng => theo đăng ký
 */
namespace ConsultationsService.PtDashboard.Summary
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class SummaryServices : eHCMS.WCFServiceCustomHeader, ISummary
    {
        private const string _ModuleName = "Consultation Service: SummaryServices";
        private const string _PrefixErrCode = "CON.SUM.";

        public SummaryServices()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }


        #region 1.Alleries
        public List<MDAllergy> MDAllergies_ByPatientID(Int64 PatientID, int flag)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDAllergies_ByPatientID(PatientID, flag);
                //}
                //else
                //{
                //    return SummaryProvider.Instance.MDAllergies_ByPatientID(PatientID, flag);
                //}
                return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDAllergies_ByPatientID(PatientID, flag);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving MDAllergies_ByPatientID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load MDAllergies_ByPatientID");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void MDAllergies_Save(MDAllergy Obj,out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDAllergies_Save(Obj, out Result);
                //}
                //else
                //{
                //    SummaryProvider.Instance.MDAllergies_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDAllergies_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving MDAllergies_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load MDAllergies_Save");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void MDAllergies_IsDeleted(MDAllergy Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDAllergies_IsDeleted(Obj, out Result);
                //}
                //else
                //{
                //    SummaryProvider.Instance.MDAllergies_IsDeleted(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDAllergies_IsDeleted(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving MDAllergies_IsDeleted. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load MDAllergies_IsDeleted");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 2.Warning
        public List<MDWarning> MDWarnings_ByPatientID(Int64 PatientID, int flag)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDWarnings_ByPatientID(PatientID, flag);
                //}
                //else
                //{
                //    return SummaryProvider.Instance.MDWarnings_ByPatientID(PatientID, flag);
                //}
                return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDWarnings_ByPatientID(PatientID, flag);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving MDWarnings_ByPatientID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Staff list");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        
        public void MDWarnings_Save(MDWarning Obj, out long Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDWarnings_Save(Obj, out Result);
                //}
                //else
                //{
                //    SummaryProvider.Instance.MDWarnings_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDWarnings_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving MDWarnings_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load MDWarnings_Save");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void MDWarnings_IsDeleted(MDWarning Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDWarnings_IsDeleted(Obj, out Result);
                //}
                //else
                //{
                //    SummaryProvider.Instance.MDWarnings_IsDeleted(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.MDWarnings_IsDeleted(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving MDWarnings_IsDeleted. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load MDWarnings_IsDeleted");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 3.PhysicalExamination
        public IList<Lookup> GetLookupSmokeStatus()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start get Smoke status.", CurrentUser);
                IList<Lookup> lst;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    lst = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.SMOKE_STATUS);
                //}
                //else
                //{
                //    lst = CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.SMOKE_STATUS);
                //}
                lst = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.SMOKE_STATUS);
                AxLogger.Instance.LogInfo("Start get Smoke status.Status: successed.", CurrentUser);
                return lst;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupByObjectTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_GetLookupByObjectTypeID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Lookup> GetLookupAlcoholDrinkingStatus()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start get lookup alcohol drinking status.", CurrentUser);
                IList<Lookup> lst;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    lst = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.ALCOHOL_DRINKING_STATUS);
                //}
                //else
                //{
                //    lst = CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.ALCOHOL_DRINKING_STATUS);
                //}
                lst = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.ALCOHOL_DRINKING_STATUS);
                AxLogger.Instance.LogInfo("Start get lookup alcohol drinking status. Status: Successed", CurrentUser);
                return lst;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupByObjectTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_GetLookupByObjectTypeID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<PhysicalExamination> GetPhyExamByPtID(long patientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetPhyExamByPtID(patientID);
                //}
                //else
                //{
                //    return SummaryProvider.Instance.GetPhyExamByPtID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetPhyExamByPtID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPhyExamByPtID. Status: Failed.", CurrentUser);
              AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUMMARY_RECORD_GetPhyExamByPtID);
               
               

               // axErr.ErrorCode = _PrefixErrCode + "0012";
               

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<PhysicalExamination> GetPhyExamByPtID_InPT(long PtRegistrationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetPhyExamByPtID_InPT(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPhyExamByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUMMARY_RECORD_GetPhyExamByPtID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public PhysicalExamination GetLastPhyExamByPtID(long patientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetLastPhyExamByPtID(patientID);
                //}
                //else
                //{
                //    return SummaryProvider.Instance.GetLastPhyExamByPtID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetLastPhyExamByPtID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLastPhyExamByPtID. Status: Failed.", CurrentUser);
              AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUMMARY_RECORD_GetLastPhyExamByPtID);
               
               

               // axErr.ErrorCode = _PrefixErrCode + "0013";
               

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddNewPhysicalExamination(PhysicalExamination entity, long? staffID) 
        {
            try 
            {
                AxLogger.Instance.LogInfo("Start of adding Physical Examination.", CurrentUser);
                if (entity.RefSmoke==null)
                {
                    entity.RefSmoke = new Lookup();
                }
                if (entity.RefAlcohol== null)
                {
                    entity.RefAlcohol = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Insert(
                //                                            entity.CommonMedicalRecord.PatientID
                //                                            , staffID
                //                                            , entity);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.PhysicalExamination_Insert(
                //                                            entity.CommonMedicalRecord.PatientID
                //                                            , staffID
                //                                            , entity);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Insert(
                                                           entity.CommonMedicalRecord.PatientID
                                                           , staffID
                                                           , entity);
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Insert. Status: Successed.", CurrentUser);
            }catch(Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Insert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_PhysicalExamination_Insert);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        //▼====== #001
        public bool AddNewPhysicalExamination_V2(PhysicalExamination entity, long? staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of adding Physical Examination.", CurrentUser);
                if (entity.RefSmoke == null)
                {
                    entity.RefSmoke = new Lookup();
                }
                if (entity.RefAlcohol == null)
                {
                    entity.RefAlcohol = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Insert_V2(entity.CommonMedicalRecord.PatientID, entity.PtRegistrationID, entity.V_RegistrationType, staffID, entity);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.PhysicalExamination_Insert_V2(entity.CommonMedicalRecord.PatientID, entity.PtRegistrationID, entity.V_RegistrationType, staffID, entity);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Insert_V2(entity.CommonMedicalRecord.PatientID, entity.PtRegistrationID, entity.V_RegistrationType, staffID, entity);
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Insert. Status: Successed.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Insert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_PhysicalExamination_Insert);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
            return true;
        }
        public bool UpdatePhysicalExamination_V2(PhysicalExamination entity, long? StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of Update PhysicalExamination.", CurrentUser);
                if (entity.RefSmoke == null)
                {
                    entity.RefSmoke = new Lookup();
                }
                if (entity.RefAlcohol == null)
                {
                    entity.RefAlcohol = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Update_V2(StaffID, entity);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.PhysicalExamination_Update_V2(StaffID, entity);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Update_V2(StaffID, entity);
                AxLogger.Instance.LogInfo("End of UpdatePhysicalExamination_V2. Status: Successed.", CurrentUser);

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdatePhysicalExamination_V2. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_PhysicalExamination_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
            return true;
        }
        public bool UpdatePhysicalExamination_InPT(PhysicalExamination entity, long? StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of Update UpdatePhysicalExamination_InPT.", CurrentUser);
                if (entity.RefSmoke == null)
                {
                    entity.RefSmoke = new Lookup();
                }
                if (entity.RefAlcohol == null)
                {
                    entity.RefAlcohol = new Lookup();
                }
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_InPT_Update(StaffID, entity);
                AxLogger.Instance.LogInfo("End of UpdatePhysicalExamination_InPT. Status: Successed.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdatePhysicalExamination_InPT. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_PhysicalExamination_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return true;
        }
        public PhysicalExamination GetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetPhyExam_ByPtRegID(PtRegistrationID, V_RegistrationType);
                //}
                //else
                //{
                //    return SummaryProvider.Instance.GetPhyExam_ByPtRegID(PtRegistrationID, V_RegistrationType);
                //}
                return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetPhyExam_ByPtRegID(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPhyExam_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUMMARY_RECORD_GetLastPhyExamByPtID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #001
        public bool AddNewPhysicalExamination_InPT(PhysicalExamination entity, long? staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of adding Physical Examination.", CurrentUser);
                if (entity.RefSmoke == null)
                {
                    entity.RefSmoke = new Lookup();
                }
                if (entity.RefAlcohol == null)
                {
                    entity.RefAlcohol = new Lookup();
                }
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Insert_InPT(entity.CommonMedicalRecord.PatientID, entity.PtRegistrationID, entity.V_RegistrationType, staffID, entity);
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Insert. Status: Successed.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Insert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_PhysicalExamination_Insert);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
            return true;
        }
        public bool UpdatePhysicalExamination(PhysicalExamination entity, long? StaffID, long? PhyExamID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of updating Physical Examination.", CurrentUser);
                if (entity.RefSmoke == null)
                {
                    entity.RefSmoke = new Lookup();
                }
                if (entity.RefAlcohol == null)
                {
                    entity.RefAlcohol = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Update(
                //                                            StaffID
                //                                            , PhyExamID
                //                                            , entity);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.PhysicalExamination_Update(
                //                                            StaffID
                //                                            , PhyExamID
                //                                            , entity);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Update(
                                                            StaffID
                                                            , PhyExamID
                                                            , entity);
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Update. Status: Successed.", CurrentUser);

            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Update. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_PhysicalExamination_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        public bool DeletePhysicalExamination(long? StaffID, long? PhyExamID, long? CommonMedRecID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of deleting Physical Examination.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Delete(
                //                                            StaffID
                //                                            , PhyExamID
                //                                            , CommonMedRecID);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.PhysicalExamination_Delete(
                //                                            StaffID
                //                                            , PhyExamID
                //                                            , CommonMedRecID);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Delete(
                                                          StaffID
                                                          , PhyExamID
                                                          , CommonMedRecID);
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Delete. Status: Successed.", CurrentUser);
            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Delete. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_PhysicalExamination_Delete);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;           
        }
        public bool DeletePhysicalExamination_InPT(long? PhyExamID, long? CommonMedRecID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of deleting Physical Examination.", CurrentUser);
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_InPT_Delete(PhyExamID, CommonMedRecID);
                AxLogger.Instance.LogInfo("End of PhysicalExamination_InPT_Delete. Status: Successed.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PhysicalExamination_InPT_Delete. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_PhysicalExamination_Delete);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
            return true;
        }
        #endregion

        #region 4.Consultation

        public IList<PatientServiceRecord> GetConsultationByPtID(long patientID, long processTypeID)//processTypeID=914
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetConsultationByPtID(patientID, processTypeID);
                //}
                //else
                //{
                //    return SummaryProvider.Instance.GetConsultationByPtID(patientID, processTypeID);
                //}
                return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetConsultationByPtID(patientID, processTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetConsultationByPtID. Status: Failed.", CurrentUser);
              AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUMMARY_RECORD_GetConsultationByPtID);
               
               

               // axErr.ErrorCode = _PrefixErrCode + "0014";
               

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientServiceRecord> GetSumConsulByPtID(long patientID, long processTypeID,int PageIndex,int PageSize,out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetSumConsulByPtID(patientID, processTypeID, PageIndex, PageSize, out Total);
                //}
                //else
                //{
                //    return SummaryProvider.Instance.GetSumConsulByPtID(patientID, processTypeID, PageIndex, PageSize, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.SummaryProvider.Instance.GetSumConsulByPtID(patientID, processTypeID, PageIndex, PageSize, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetSumConsulByPtID. Status: Failed.", CurrentUser);
              AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUMMARY_RECORD_GetSumConsulByPtID);
               
               

               // axErr.ErrorCode = _PrefixErrCode + "0015";
               

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        
        #region 6.Family History
        public bool DeleteFamilyHistory(long? StaffID, long? FHCode, long? CommonMedRecID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of deleting Family History.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.FamilyHistory_Delete(
                //                                            StaffID
                //                                            , FHCode
                //                                            , CommonMedRecID);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.FamilyHistory_Delete(
                //                                            StaffID
                //                                            , FHCode
                //                                            , CommonMedRecID);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.FamilyHistory_Delete(
                                                            StaffID
                                                            , FHCode
                                                            , CommonMedRecID);
            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of FamilyHistory_Delete. Status: Successed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_FamilyHistory_Delete);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }


        public bool AddNewFamilyHistory(FamilyHistory entity, long? staffID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of adding Family History.", CurrentUser);
                if (entity.LookupFamilyRelationship == null)
                {
                    entity.LookupFamilyRelationship = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.FamilyHistory_Insert(
                //                                            entity.CommonMedicalRecord.PatientID
                //                                            , staffID
                //                                            , entity.CommonMedRecID
                //                                            , entity.IDCode
                //                                            , entity.FHFullName
                //                                            , entity.LookupFamilyRelationship.LookupID
                //                                            , entity.FHNotes
                //                                            , entity.Decease);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.FamilyHistory_Insert(
                //                                            entity.CommonMedicalRecord.PatientID
                //                                            , staffID
                //                                            , entity.CommonMedRecID
                //                                            , entity.IDCode
                //                                            , entity.FHFullName
                //                                            , entity.LookupFamilyRelationship.LookupID
                //                                            , entity.FHNotes
                //                                            , entity.Decease);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.FamilyHistory_Insert(
                                                            entity.CommonMedicalRecord.PatientID
                                                            , staffID
                                                            , entity.CommonMedRecID
                                                            , entity.IDCode
                                                            , entity.FHFullName
                                                            , entity.LookupFamilyRelationship.LookupID
                                                            , entity.FHNotes
                                                            , entity.Decease, entity.DiseaseNameVN);
                AxLogger.Instance.LogInfo("End of FamilyHistory_Insert. Status: Successed.", CurrentUser);
            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of FamilyHistory_Insert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_FamilyHistory_Insert);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }


        public bool UpdateFamilyHistory(FamilyHistory entity, long? StaffID, long? FHCode) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of updating Family History. Status: Failed.", CurrentUser);
                if (entity.LookupFamilyRelationship==null)
                {
                    entity.LookupFamilyRelationship = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.FamilyHistory_Update(FHCode
                //                                                         , StaffID
                //                                                        , entity.CommonMedRecID
                //                                                        , entity.IDCode
                //                                                       , entity.FHFullName
                //                                                       , entity.LookupFamilyRelationship.LookupID
                //                                                       , entity.FHNotes
                //                                                       , entity.Decease
                //                                                       , entity.CommonMedicalRecord.CMRModifiedDate);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.FamilyHistory_Update(FHCode
                //                                                         , StaffID
                //                                                        , entity.CommonMedRecID
                //                                                        , entity.IDCode
                //                                                       , entity.FHFullName
                //                                                       , entity.LookupFamilyRelationship.LookupID
                //                                                       , entity.FHNotes
                //                                                       , entity.Decease
                //                                                       , entity.CommonMedicalRecord.CMRModifiedDate);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.FamilyHistory_Update(FHCode
                                                                        , StaffID
                                                                       , entity.CommonMedRecID
                                                                       , entity.IDCode
                                                                      , entity.FHFullName
                                                                      , entity.LookupFamilyRelationship.LookupID
                                                                      , entity.FHNotes
                                                                      , entity.Decease
                                                                      , entity.CommonMedicalRecord.CMRModifiedDate, entity.DiseaseNameVN);
                AxLogger.Instance.LogInfo("End of FamilyHistory_Update. Status: Successed.", CurrentUser);
            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of FamilyHistory_Update. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.SUMMARY_RECORD_FamilyHistory_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        #endregion
    }
}
