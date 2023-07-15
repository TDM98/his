using System;
using System.Collections.Generic;

using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;

using ErrorLibrary;

using AxLogging;
using Service.Core.HelperClasses;
using System.Diagnostics;
using ErrorLibrary.Resources;
using eHCMSLanguage;
/*
* 20230201 #001 DatTB: Thêm trường dữ liệu về KSNK trong phần Thông tin chung NB nội trú
 */
namespace ConsultationsService.PtDashboard.CommonRecords
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceKnownType(typeof(AxException))]
    public class CommonRecordsServices : eHCMS.WCFServiceCustomHeader, ICommonRecords
    {
        public CommonRecordsServices()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        #region Common
        public IList<Lookup> GetLookupVitalSignDataType()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupVitalSignDataType();
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupVitalSignDataType();
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupVitalSignDataType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLookupVitalSignDataType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_LKVITALSIGN_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Lookup> GetLookupVitalSignContext()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupVitalSignContext();
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupVitalSignContext();
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupVitalSignContext();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLookupVitalSignContext. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_LKVITALSIGN_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Lookup> GetLookupPMHStatus()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupPMHStatus();
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupPMHStatus();
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupPMHStatus();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetLookupPMHStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMHSTATUS_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DiseasesReference> GetDiseasessReferences()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetDiseasessReferences();
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetDiseasessReferences();
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetDiseasessReferences();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiseasessReferences. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DISEASESREFERENCE_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DiseasesReference> GetDiseasessRefByICD10Code(string icd10Code)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetDiseasessRefByICD10Code(icd10Code);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetDiseasessRefByICD10Code(icd10Code);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetDiseasessRefByICD10Code(icd10Code);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDiseasessRefByICD10Code. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_DISEASESREFERENCE_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 1.VitalSigns
        public IList<VitalSign> GetAllVitalSigns()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetAllVitalSigns();
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetAllVitalSigns();
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetAllVitalSigns();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllVitalSigns. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_VITALSIGN_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteVitalSigns(byte vitalSignID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteVitalSigns(vitalSignID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.DeleteVitalSigns(vitalSignID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteVitalSigns(vitalSignID);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteVitalSigns. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_VITALSIGN_CANNOT_DELETE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddVitalSigns(VitalSign entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddVitalSigns(entity);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.AddVitalSigns(entity);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddVitalSigns(entity);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddVitalSigns. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_VITALSIGN_CANNOT_ADD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateVitalSigns(VitalSign entity)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateVitalSigns(entity);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.UpdateVitalSigns(entity);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateVitalSigns(entity);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateVitalSigns. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_VITALSIGN_CANNOT_UPDATE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientVitalSign> GetVitalSignsByPtID(long patientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetVitalSignsByPtID(patientID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetVitalSignsByPtID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetVitalSignsByPtID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetVitalSignsByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PATIENTVITALSIGN_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteItemPtVitalSigns(PatientVitalSign entity, long? staffID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteItemPtVitalSigns(entity, staffID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.DeleteItemPtVitalSigns(entity, staffID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteItemPtVitalSigns(entity, staffID);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteItemPtVitalSigns. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PATIENTVITALSIGN_CANNOT_DELETE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddItemPtVitalSigns(PatientVitalSign entity, long? staffID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddItemPtVitalSigns(entity, staffID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.AddItemPtVitalSigns(entity, staffID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddItemPtVitalSigns(entity, staffID);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddItemPtVitalSigns. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PATIENTVITALSIGN_CANNOT_ADD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateItemPtVitalSigns(PatientVitalSign entity, long oldPtVSignID, long? staffID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateItemPtVitalSigns(entity, oldPtVSignID, staffID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.UpdateItemPtVitalSigns(entity, oldPtVSignID, staffID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateItemPtVitalSigns(entity, oldPtVSignID, staffID);
                //CRUDOperationResponse response = new CRUDOperationResponse();
                //return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateItemPtVitalSigns. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PATIENTVITALSIGN_CANNOT_UPDATE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region 2.Medical Conditions
        public IList<RefMedContraIndicationTypes> GetRefMedCondType()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefMedCondType();
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetRefMedCondType();
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefMedCondType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefMedCondType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetRefMedCondType);



                // axErr.ErrorCode = _PrefixErrCode + "0014";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefMedContraIndicationICD> GetRefMedConditions()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefMedConditions();
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetRefMedConditions();
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefMedConditions();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefMedConditions. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetRefMedConditions);



                // axErr.ErrorCode = _PrefixErrCode + "0015";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefMedContraIndicationICD> GetRefMedConditionsByType(int medCondTypeID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefMedConditionsByType(medCondTypeID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetRefMedConditionsByType(medCondTypeID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefMedConditionsByType(medCondTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefMedConditionsByType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetRefMedConditionsByType);



                // axErr.ErrorCode = _PrefixErrCode + "0016";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<MedicalConditionRecord> GetMedConditionByPtID(long patientID, int mcTypeID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetMedConditionByPtID(patientID, mcTypeID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetMedConditionByPtID(patientID, mcTypeID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetMedConditionByPtID(patientID, mcTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0017";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Manipulate on refMedicalConditionType
        public bool DeleteRefMedCondType(RefMedContraIndicationTypes entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0018";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddRefMedCondType(RefMedContraIndicationTypes entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0019";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateRefMedCondType(RefMedContraIndicationTypes entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0020";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Manipulate on refMedicalConditions
        public bool DeleteRefMedCond(RefMedContraIndicationICD entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0021";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddRefMedCond(RefMedContraIndicationICD entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0022";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateRefMedCond(RefMedContraIndicationICD entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0023";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Manipulate on MedicalConditionRecords
        public bool DeleteMedCondRecs(MedicalConditionRecord entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0024";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddMedCondRecs(MedicalConditionRecord entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0025";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateMedCondRecs(MedicalConditionRecord entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedConditionByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetMedConditionByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0026";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //Dinh them

        public bool DeleteMedReCond(long? MCRecID, long? StaffID, long? CommonMedRecID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start deleting MedicalConditionRecord.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteMedReCond(MCRecID, StaffID, CommonMedRecID);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.DeleteMedReCond(MCRecID, StaffID, CommonMedRecID);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteMedReCond(MCRecID, StaffID, CommonMedRecID);
                AxLogger.Instance.LogInfo("End of DeleteMedReCond. Status: Successful.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteMedReCond. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_DeleteMedReCond);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }

        public bool AddNewMedReCond(MedicalConditionRecord entity, long? StaffID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start adding MedicalConditionRecord.", CurrentUser);
                if (entity.RefMedicalCondition==null)
                {
                    entity.RefMedicalCondition = new RefMedContraIndicationICD();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddNewMedReCond(entity.CommonMedicalRecord.PatientID
                //                                                , StaffID
                //                                                , entity.CommonMedRecID
                //                                                , entity.RefMedicalCondition.MCID
                //                                                , entity.MCYesNo
                //                                                , entity.MCTextValue
                //                                                , entity.MCExplainOrNotes
                //                                                );
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.AddNewMedReCond(entity.CommonMedicalRecord.PatientID
                //                                                , StaffID
                //                                                , entity.CommonMedRecID
                //                                                , entity.RefMedicalCondition.MCID
                //                                                , entity.MCYesNo
                //                                                , entity.MCTextValue
                //                                                , entity.MCExplainOrNotes
                //                                                );
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddNewMedReCond(entity.CommonMedicalRecord.PatientID
                                                               , StaffID
                                                               , entity.CommonMedRecID
                                                               , entity.RefMedicalCondition.MCID
                                                               , entity.MCYesNo
                                                               , entity.MCTextValue
                                                               , entity.MCExplainOrNotes
                                                               );
                AxLogger.Instance.LogInfo("End of AddNewMedReCond. Status: Successful.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewMedReCond. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_AddNewMedReCond);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        public bool UpdateMedReCond(MedicalConditionRecord entity, long? MCRecID, long? StaffID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Updating MedicalConditionRecord.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateMedReCond(MCRecID, StaffID, entity.CommonMedicalRecord.CommonMedRecID
                //                                , entity.RefMedicalCondition.MCID, entity.MCYesNo, entity.MCTextValue
                //                                , entity.MCExplainOrNotes);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.UpdateMedReCond(MCRecID, StaffID, entity.CommonMedicalRecord.CommonMedRecID
                //                                , entity.RefMedicalCondition.MCID, entity.MCYesNo, entity.MCTextValue
                //                                , entity.MCExplainOrNotes);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateMedReCond(MCRecID, StaffID, entity.CommonMedicalRecord.CommonMedRecID
                                               , entity.RefMedicalCondition.MCID, entity.MCYesNo, entity.MCTextValue
                                               , entity.MCExplainOrNotes);
                AxLogger.Instance.LogInfo("End of UpdateMedReCond. Status: Successful.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateMedReCond. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_UpdateMedReCond);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        #endregion

        #region 3.Medical History

        public IList<RefMedicalHistory> GetRefMedHistory()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefMedHistory();
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetRefMedHistory();
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefMedHistory();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefMedHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetRefMedHistory);



                // axErr.ErrorCode = _PrefixErrCode + "0027";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PastMedicalConditionHistory> GetPastMedCondHisByPtID(long patientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetPastMedCondHisByPtID(patientID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetPastMedCondHisByPtID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetPastMedCondHisByPtID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPastMedCondHisByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetPastMedCondHisByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0028";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Manipulate on RefMedicalHistory
        public bool DeleteRefMedicalHistory(RefMedicalHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPastMedCondHisByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetPastMedCondHisByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0029";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddRefMedicalHistory(RefMedicalHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPastMedCondHisByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetPastMedCondHisByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0030";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateRefMedicalHistory(RefMedicalHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPastMedCondHisByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetPastMedCondHisByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0031";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Manipulate on PastMedicalConditionHistory
        public bool DeletePastMedCondHis(PastMedicalConditionHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPastMedCondHisByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetPastMedCondHisByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0032";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddPastMedCondHis(PastMedicalConditionHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPastMedCondHisByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetPastMedCondHisByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0033";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdatePastMedCondHis(PastMedicalConditionHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPastMedCondHisByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetPastMedCondHisByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0034";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //Dinh them
        public bool DeleteMedicalHistory(long? PMHID, long? StaffID, long? CommonMedRecID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Deleting PastMedicalConditionHistory.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteMedicalHistory(PMHID, StaffID, CommonMedRecID);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.DeleteMedicalHistory(PMHID, StaffID, CommonMedRecID);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteMedicalHistory(PMHID, StaffID, CommonMedRecID);
                AxLogger.Instance.LogInfo("End of DeleteMedicalHistory. Status: Successful.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteMedicalHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_DeleteMedicalHistory);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        public bool AddNewMedicalHistory(PastMedicalConditionHistory entity, long? StaffID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Adding PastMedicalConditionHistory.", CurrentUser);
                if (entity.RefMedicalHistory==null)
                {
                    entity.RefMedicalHistory = new RefMedicalHistory();
                }
                if (entity.LookupPMHStatus == null)
                {
                    entity.LookupPMHStatus = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddNewMedicalHistory(entity.CommonMedicalRecord.PatientID
                //                                   , StaffID
                //                                   , entity.CommonMedicalRecord.CommonMedRecID
                //                                   , entity.RefMedicalHistory.MedHistCode
                //                                   , entity.PMHYesNo
                //                                   , entity.PMHExplainReason
                //                                   , entity.LookupPMHStatus.LookupID);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.AddNewMedicalHistory(entity.CommonMedicalRecord.PatientID
                //                                   , StaffID
                //                                   , entity.CommonMedicalRecord.CommonMedRecID
                //                                   , entity.RefMedicalHistory.MedHistCode
                //                                   , entity.PMHYesNo
                //                                   , entity.PMHExplainReason
                //                                   , entity.LookupPMHStatus.LookupID);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddNewMedicalHistory(entity.CommonMedicalRecord.PatientID
                                                  , StaffID
                                                  , entity.CommonMedicalRecord.CommonMedRecID
                                                  , entity.RefMedicalHistory.MedHistCode
                                                  , entity.PMHYesNo
                                                  , entity.PMHExplainReason
                                                  , entity.LookupPMHStatus.LookupID);
                AxLogger.Instance.LogInfo("End of AddNewMedicalHistory. Status: Successful.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewMedicalHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_AddNewMedicalHistory);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }

        public bool UpdateMedicalHistory(PastMedicalConditionHistory entity, long? StaffID, long? PMHID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Updating PastMedicalConditionHistory.", CurrentUser);
                if (entity.RefMedicalHistory == null)
                {
                    entity.RefMedicalHistory = new RefMedicalHistory();
                }
                if (entity.LookupPMHStatus == null)
                {
                    entity.LookupPMHStatus = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateMedicalHistory(StaffID
                //                                   , entity.PMHID
                //                                   , entity.CommonMedicalRecord.CommonMedRecID
                //                                   , entity.RefMedicalHistory.MedHistCode
                //                                   , entity.PMHYesNo
                //                                   , entity.PMHExplainReason
                //                                   , entity.LookupPMHStatus.LookupID);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.UpdateMedicalHistory(StaffID
                //                                   , entity.PMHID
                //                                   , entity.CommonMedicalRecord.CommonMedRecID
                //                                   , entity.RefMedicalHistory.MedHistCode
                //                                   , entity.PMHYesNo
                //                                   , entity.PMHExplainReason
                //                                   , entity.LookupPMHStatus.LookupID);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateMedicalHistory(StaffID
                                                  , entity.PMHID
                                                  , entity.CommonMedicalRecord.CommonMedRecID
                                                  , entity.RefMedicalHistory.MedHistCode
                                                  , entity.PMHYesNo
                                                  , entity.PMHExplainReason
                                                  , entity.LookupPMHStatus.LookupID);
                AxLogger.Instance.LogInfo("End of UpdateMedicalHistory. Status: Successful.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateMedicalHistory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_UpdateMedicalHistory);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        #endregion

        #region 4.Immunization

        public IList<RefImmunization> GetRefImmunization(long MedServiceID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefImmunization(MedServiceID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetRefImmunization(MedServiceID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetRefImmunization(MedServiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefImmunization. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetRefImmunization);



                // axErr.ErrorCode = _PrefixErrCode + "0035";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<ImmunizationHistory> GetImmunizationByPtID(long patientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetImmunizationByPtID(patientID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetImmunizationByPtID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetImmunizationByPtID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetImmunizationByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetImmunizationByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0036";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Manupulate on RefImmunization
        public bool DeleteImmunization(RefImmunization entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetImmunizationByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetImmunizationByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0037";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddImmunization(RefImmunization entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetImmunizationByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetImmunizationByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0038";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateImmunization(RefImmunization entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetImmunizationByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetImmunizationByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0039";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Manupulate on ImmunizationHistory
        public bool DeleteImmuHis(ImmunizationHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetImmunizationByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetImmunizationByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0040";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddImmuHis(ImmunizationHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetImmunizationByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetImmunizationByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0041";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateImmuHis(ImmunizationHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetImmunizationByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetImmunizationByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0042";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //Dinh them


        public bool DeleteImmunization(long? IHID, long? StaffID, long? CommonMedRecID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Deleting ImmunizationHistory.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteImmunization(IHID, StaffID, CommonMedRecID);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.DeleteImmunization(IHID, StaffID, CommonMedRecID);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.DeleteImmunization(IHID, StaffID, CommonMedRecID);
                AxLogger.Instance.LogInfo("End of DeleteImmunization. Status: Successful.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteImmunization. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_DeleteImmunization);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        public bool AddNewImmunization(ImmunizationHistory entity, long? StaffID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Adding ImmunizationHistory.", CurrentUser);
                if (entity.RefImmunization==null)
                {
                    entity.RefImmunization = new RefImmunization();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddNewImmunization(entity.CommonMedicalRecord.PatientID
                //                                    , StaffID
                //                                    , entity.RefImmunization.IHCode
                //                                   , entity.CommonMedicalRecord.CommonMedRecID
                //                                   , entity.IHYesNo
                //                                   , entity.IHDate);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.AddNewImmunization(entity.CommonMedicalRecord.PatientID
                //                                    , StaffID
                //                                    , entity.RefImmunization.IHCode
                //                                   , entity.CommonMedicalRecord.CommonMedRecID
                //                                   , entity.IHYesNo
                //                                   , entity.IHDate);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.AddNewImmunization(entity.CommonMedicalRecord.PatientID
                                                   , StaffID
                                                   , entity.RefImmunization.IHCode
                                                  , entity.CommonMedicalRecord.CommonMedRecID
                                                  , entity.IHYesNo
                                                  , entity.IHDate);
                AxLogger.Instance.LogInfo("End of AddNewImmunization. Status: Successful.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewImmunization. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_AddNewImmunization);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        public bool UpdateImmunization(ImmunizationHistory entity, long? IHID, long? StaffID) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start updating ImmunizationHistory.", CurrentUser);
                if (entity.RefImmunization == null)
                {
                    entity.RefImmunization = new RefImmunization();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateImmunization(IHID
                //                                    , StaffID
                //                                    , entity.RefImmunization.IHCode
                //                                   , entity.CommonMedicalRecord.CommonMedRecID
                //                                   , entity.IHYesNo
                //                                   , entity.IHDate);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.UpdateImmunization(IHID
                //                                    , StaffID
                //                                    , entity.RefImmunization.IHCode
                //                                   , entity.CommonMedicalRecord.CommonMedRecID
                //                                   , entity.IHYesNo
                //                                   , entity.IHDate);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.UpdateImmunization(IHID
                                                   , StaffID
                                                   , entity.RefImmunization.IHCode
                                                  , entity.CommonMedicalRecord.CommonMedRecID
                                                  , entity.IHYesNo
                                                  , entity.IHDate);
                AxLogger.Instance.LogInfo("End of UpdateImmunization. Status: Successful.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateImmunization. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_UpdateImmunization);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        #endregion

        #region 5.Hospitalization
        public IList<Lookup> GetLookupAdmissionType()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.ADMISSION_TYPE);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.ADMISSION_TYPE);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.ADMISSION_TYPE);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupByObjectTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetLookupByObjectTypeID);



                // axErr.ErrorCode = _PrefixErrCode + "0043";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Lookup> GetLookupAdmissionReason()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.ADMISSION_REASON);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.ADMISSION_REASON);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.ADMISSION_REASON);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupByObjectTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetLookupByObjectTypeID);



                // axErr.ErrorCode = _PrefixErrCode + "0044";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Lookup> GetLookupReferralType()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.REFERRAL_TYPE);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.REFERRAL_TYPE);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.REFERRAL_TYPE);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupByObjectTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetLookupByObjectTypeID);



                // axErr.ErrorCode = _PrefixErrCode + "0045";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        
        public IList<Lookup> GetLookupTreatmentResult()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.TREATMENT_RESULT);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.TREATMENT_RESULT);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.TREATMENT_RESULT);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupByObjectTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetLookupByObjectTypeID);



                // axErr.ErrorCode = _PrefixErrCode + "0046";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Lookup> GetLookupDischargeReason()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.DISCHARGE_TYPE);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.DISCHARGE_TYPE);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.DISCHARGE_TYPE);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupByObjectTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetLookupByObjectTypeID);



                // axErr.ErrorCode = _PrefixErrCode + "0047";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Lookup> GetLookupHospitalType()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.HOSPITAL_TYPE);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.HOSPITAL_TYPE);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.HOSPITAL_TYPE);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupByObjectTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetLookupByObjectTypeID);



                // axErr.ErrorCode = _PrefixErrCode + "0048";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<HospitalizationHistory> GetHospitalizationHistoryByPtID(long patientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetHospitalizationHistoryByPtID(patientID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetHospitalizationHistoryByPtID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetHospitalizationHistoryByPtID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHospitalizationHistoryByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetHospitalizationHistoryByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0049";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Manupulate on HospitalizationHistory
        public bool DeleteHospitalizationHistory(HospitalizationHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHospitalizationHistoryByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetHospitalizationHistoryByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0050";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddHospitalizationHistory(HospitalizationHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHospitalizationHistoryByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetHospitalizationHistoryByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0051";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateHospitalizationHistory(HospitalizationHistory entity)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHospitalizationHistoryByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetHospitalizationHistoryByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0052";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Hospital> GetHospitalByKey(string SearchKey, long V_HospitalType, int PageSize, int PageIndex)
        {
            try 
            {
                AxLogger.Instance.LogInfo("Start GetHospital by key.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetHospitalByKey(SearchKey, V_HospitalType, PageSize, PageIndex);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetHospitalByKey(SearchKey, V_HospitalType, PageSize, PageIndex);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetHospitalByKey(SearchKey, V_HospitalType, PageSize, PageIndex);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetHospitalByKey. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_GetHospitalByKey);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteHospitalization(long? HHID, long? StaffID, long? CommonMedRecID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start deleting HospitalizationHistory.", CurrentUser);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.HospitalizationHistory_Delete(
                //                                            HHID, StaffID
                //                                            , CommonMedRecID);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.HospitalizationHistory_Delete(
                //                                            HHID, StaffID
                //                                            , CommonMedRecID);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.HospitalizationHistory_Delete(
                                                           HHID, StaffID
                                                           , CommonMedRecID);
                AxLogger.Instance.LogInfo("End of HospitalizationHistory_Delete. Status: Successful.", CurrentUser);
            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of HospitalizationHistory_Delete. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_HospitalizationHistory_Delete);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                 
            }
            //CRUDOperationResponse response = new CRUDOperationResponse();
            //throw new NotImplementedException();
            return true;
        }
        public bool AddNewHospitalization(HospitalizationHistory entity, long? StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start adding HospitalizationHistory.", CurrentUser);
                if (entity.DiseasesReference==null)
                {
                    entity.DiseasesReference = new DiseasesReference();
                }
                if (entity.LookupAdmissionType == null)
                {
                    entity.LookupAdmissionType = new Lookup();
                }
                if (entity.LookupTreatmentResult == null)
                {
                    entity.LookupTreatmentResult = new Lookup();
                }
                if (entity.LookupDischargeReason == null)
                {
                    entity.LookupDischargeReason = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.HospitalizationHistory_Insert(entity.CommonMedicalRecord.PatientID
                //                                              , StaffID
                //                                              , entity.CommonMedRecID
                //                                              , entity.DiseasesReference.IDCode
                //                                              , entity.HDate
                //                                              , entity.FromDate
                //                                              , entity.FromHospital.HosID
                //                                              , entity.LookupAdmissionType.LookupID
                //                                              , entity.GeneralDiagnoses
                //                                              , entity.LookupTreatmentResult.LookupID
                //                                              , entity.LookupDischargeReason.LookupID
                //                                              , entity.HHNotes);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.HospitalizationHistory_Insert(entity.CommonMedicalRecord.PatientID
                //                                              , StaffID
                //                                              , entity.CommonMedRecID
                //                                              , entity.DiseasesReference.IDCode
                //                                              , entity.HDate
                //                                              , entity.FromDate
                //                                              , entity.FromHospital.HosID
                //                                              , entity.LookupAdmissionType.LookupID
                //                                              , entity.GeneralDiagnoses
                //                                              , entity.LookupTreatmentResult.LookupID
                //                                              , entity.LookupDischargeReason.LookupID
                //                                              , entity.HHNotes);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.HospitalizationHistory_Insert(entity.CommonMedicalRecord.PatientID
                                                            , StaffID
                                                            , entity.CommonMedRecID
                                                            , entity.DiseasesReference.IDCode
                                                            , entity.HDate
                                                            , entity.FromDate
                                                            , entity.FromHospital.HosID
                                                            , entity.LookupAdmissionType.LookupID
                                                            , entity.GeneralDiagnoses
                                                            , entity.LookupTreatmentResult.LookupID
                                                            , entity.LookupDischargeReason.LookupID
                                                            , entity.HHNotes);
                AxLogger.Instance.LogInfo("End of HospitalizationHistory_Insert. Status: Success.", CurrentUser);
                                                            
            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of HospitalizationHistory_Insert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex,ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_HospitalizationHistory_Insert);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                
            }
            return true;
        }
        public bool UpdateHospitalization(HospitalizationHistory entity, long? HHID, long? StaffID
                                                               ) 
        {
            try
            {
                AxLogger.Instance.LogInfo("Start updating HospitalizationHistory.", CurrentUser);
                if (entity.DiseasesReference==null)
                {
                    entity.DiseasesReference = new DiseasesReference();
                }
                if (entity.LookupAdmissionType == null)
                {
                    entity.LookupAdmissionType = new Lookup();
                }
                if (entity.LookupTreatmentResult == null)
                {
                    entity.LookupTreatmentResult = new Lookup();
                }
                if (entity.LookupDischargeReason == null)
                {
                    entity.LookupDischargeReason = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.HospitalizationHistory_Update(HHID
                //                                              , StaffID
                //                                              , entity.CommonMedicalRecord.CommonMedRecID
                //                                              , entity.DiseasesReference.IDCode
                //                                              , entity.HDate
                //                                              , entity.FromDate
                //                                              , entity.FromHospital.HosID
                //                                              , entity.LookupAdmissionType.LookupID
                //                                              //, entity.LookupAdmissionReason.LookupID
                //                                              //, entity.LookupReferralType.LookupID
                //                                              , entity.GeneralDiagnoses
                //                                              //, entity.ToDate
                //                                              //, entity.ToHospital.HosID
                //                                              , entity.LookupTreatmentResult.LookupID
                //                                              , entity.LookupDischargeReason.LookupID
                //                                              //, entity.LookupHospitalType.LookupID
                //                                              , entity.HHNotes);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.HospitalizationHistory_Update(HHID
                //                                              , StaffID
                //                                              , entity.CommonMedicalRecord.CommonMedRecID
                //                                              , entity.DiseasesReference.IDCode
                //                                              , entity.HDate
                //                                              , entity.FromDate
                //                                              , entity.FromHospital.HosID
                //                                              , entity.LookupAdmissionType.LookupID
                //                                              //, entity.LookupAdmissionReason.LookupID
                //                                              //, entity.LookupReferralType.LookupID
                //                                              , entity.GeneralDiagnoses
                //                                              //, entity.ToDate
                //                                              //, entity.ToHospital.HosID
                //                                              , entity.LookupTreatmentResult.LookupID
                //                                              , entity.LookupDischargeReason.LookupID
                //                                              //, entity.LookupHospitalType.LookupID
                //                                              , entity.HHNotes);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.HospitalizationHistory_Update(HHID
                                                             , StaffID
                                                             , entity.CommonMedicalRecord.CommonMedRecID
                                                             , entity.DiseasesReference.IDCode
                                                             , entity.HDate
                                                             , entity.FromDate
                                                             , entity.FromHospital.HosID
                                                             , entity.LookupAdmissionType.LookupID
                                                             //, entity.LookupAdmissionReason.LookupID
                                                             //, entity.LookupReferralType.LookupID
                                                             , entity.GeneralDiagnoses
                                                             //, entity.ToDate
                                                             //, entity.ToHospital.HosID
                                                             , entity.LookupTreatmentResult.LookupID
                                                             , entity.LookupDischargeReason.LookupID
                                                             //, entity.LookupHospitalType.LookupID
                                                             , entity.HHNotes);
            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of HospitalizationHistory_Update. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_HospitalizationHistory_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                 
            }
            //CRUDOperationResponse response = new CRUDOperationResponse();
            //throw new NotImplementedException();
            return true;
        }
        #endregion

        #region 6.Family History
        public IList<Lookup> GetLookupFamilyRelationship()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.FAMILY_RELATIONSHIP);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.FAMILY_RELATIONSHIP);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetLookupByObjectTypeID(LookupValues.FAMILY_RELATIONSHIP);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupByObjectTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetLookupByObjectTypeID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<FamilyHistory> GetFamilyHistoryByPtID(long patientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetFamilyHistoryByPtID(patientID);
                //}
                //else
                //{
                //    return CommonRecordsProvider.Instance.GetFamilyHistoryByPtID(patientID);
                //}
                return aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetFamilyHistoryByPtID(patientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFamilyHistoryByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RECORD_GetFamilyHistoryByPtID);



                // axErr.ErrorCode = _PrefixErrCode + "0054";


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Manupulate on FamilyHistory
        //public CRUDOperationResponse DeleteFamilyHistory(FamilyHistory entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public CRUDOperationResponse AddFamilyHistory(FamilyHistory entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public CRUDOperationResponse UpdateFamilyHistory(FamilyHistory entity)
        //{
        //    throw new NotImplementedException();
        //}
        public bool DeleteFamilyHistory(long? StaffID, long? FHCode, long? CommonMedRecID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start deleting family history.", CurrentUser);
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
                AxLogger.Instance.LogInfo("End of FamilyHistory_Delete. Status: Successful.", CurrentUser);
            }
            catch (Exception ex) 
            {
               
                AxLogger.Instance.LogInfo("End of FamilyHistory_Delete. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_FamilyHistory_Delete);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                 
            }
            //CRUDOperationResponse response = new CRUDOperationResponse();
            //throw new NotImplementedException();
            return true;
        }


        public bool AddNewFamilyHistory(FamilyHistory entity, long? staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start adding Family History.", CurrentUser);
                if (entity.DiseasesReference == null)
                {
                    entity.DiseasesReference = new DiseasesReference();
                }
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
                //                                            , entity.DiseasesReference.IDCode
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
                //                                            , entity.DiseasesReference.IDCode
                //                                            , entity.FHFullName
                //                                            , entity.LookupFamilyRelationship.LookupID
                //                                            , entity.FHNotes
                //                                            , entity.Decease);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.FamilyHistory_Insert(
                                                           entity.CommonMedicalRecord.PatientID
                                                           , staffID
                                                           , entity.CommonMedRecID
                                                           , entity.DiseasesReference.IDCode
                                                           , entity.FHFullName
                                                           , entity.LookupFamilyRelationship.LookupID
                                                           , entity.FHNotes
                                                           , entity.Decease, entity.DiseaseNameVN);
                AxLogger.Instance.LogInfo("End of FamilyHistory_Insert. Status: Successful.", CurrentUser);
            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of FamilyHistory_Insert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_FamilyHistory_Insert);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));    
                 
            }
            
            return true;
        }


        public bool UpdateFamilyHistory(FamilyHistory entity, long? StaffID, long? FHCode)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start updating Family History.", CurrentUser);
                if (entity.DiseasesReference == null)
                {
                    entity.DiseasesReference = new DiseasesReference();
                }
                if (entity.LookupFamilyRelationship == null)
                {
                    entity.LookupFamilyRelationship = new Lookup();
                }
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.FamilyHistory_Update(FHCode
                //                                                         , StaffID
                //                                                        , entity.CommonMedRecID
                //                                                        , entity.DiseasesReference.IDCode
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
                //                                                        , entity.DiseasesReference.IDCode
                //                                                       , entity.FHFullName
                //                                                       , entity.LookupFamilyRelationship.LookupID
                //                                                       , entity.FHNotes
                //                                                       , entity.Decease
                //                                                       , entity.CommonMedicalRecord.CMRModifiedDate);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.FamilyHistory_Update(FHCode
                                                                       , StaffID
                                                                      , entity.CommonMedRecID
                                                                      , entity.DiseasesReference.IDCode
                                                                     , entity.FHFullName
                                                                     , entity.LookupFamilyRelationship.LookupID
                                                                     , entity.FHNotes
                                                                     , entity.Decease
                                                                     , entity.CommonMedicalRecord.CMRModifiedDate, entity.DiseaseNameVN);
                AxLogger.Instance.LogInfo("End of FamilyHistory_Update. Status: Succsessful.", CurrentUser);
            }
            catch (Exception ex) 
            {

                AxLogger.Instance.LogInfo("End of FamilyHistory_Update. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_FamilyHistory_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                 
            }
            
            return true;
            //CRUDOperationResponse response = new CRUDOperationResponse();
            //throw new NotImplementedException();
        }
        #endregion

        #region 7.Physical Examination
        
        
        public bool UpdatePhysicalExamination(PhysicalExamination entity, long? StaffID, long? PhyExamID)
        {
            try 
            {
                AxLogger.Instance.LogInfo("Start updating Physical Examination.", CurrentUser);
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
                //                                             StaffID
                //                                            , PhyExamID
                //                                            , entity);
                //}
                //else
                //{
                //    CommonRecordsProvider.Instance.PhysicalExamination_Update(
                //                                             StaffID
                //                                            , PhyExamID
                //                                            , entity);
                //}
                aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.PhysicalExamination_Update(
                                                            StaffID
                                                           , PhyExamID
                                                           , entity);
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Update. Status: Successful.", CurrentUser);
            }
            catch (Exception ex) 
            {
                AxLogger.Instance.LogInfo("End of PhysicalExamination_Update. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_PhysicalExamination_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                 
            }
            
            
            //CRUDOperationResponse response = new CRUDOperationResponse();
            return true;
        }
        #endregion


        #region 8. Risk Factor

        public bool RiskFactorInsert(RiskFactors entity)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start updating RiskFactorInsert.", CurrentUser);
                bool b;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    b = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.RiskFactorInsert(entity);
                //}
                //else
                //{
                //    b = CommonRecordsProvider.Instance.RiskFactorInsert(entity);
                //}
                b = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.RiskFactorInsert(entity);
                AxLogger.Instance.LogInfo("End of RiskFactorInsert. Status: Successful.", CurrentUser);
                return b;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RiskFactorInsert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_PhysicalExamination_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RiskFactorDelete(long RiskFactorID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start updating RiskFactorDelete.", CurrentUser);
                bool b;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    b = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.RiskFactorDelete(RiskFactorID);
                //}
                //else
                //{
                //    b = CommonRecordsProvider.Instance.RiskFactorDelete(RiskFactorID);
                //}
                b = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.RiskFactorDelete(RiskFactorID);
                AxLogger.Instance.LogInfo("End of RiskFactorDelete. Status: Successful.", CurrentUser);
                return b;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RiskFactorDelete. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_PhysicalExamination_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RiskFactors> RiskFactorGet(long? PatientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start updating RiskFactorInsert.", CurrentUser);
                List<RiskFactors> lst = new List<RiskFactors>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    lst = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.RiskFactorGet(PatientID);
                //}
                //else
                //{
                //    lst = CommonRecordsProvider.Instance.RiskFactorGet(PatientID);
                //}
                lst = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.RiskFactorGet(PatientID);
                AxLogger.Instance.LogInfo("End of RiskFactorInsert. Status: Successful.", CurrentUser);
                return lst;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RiskFactorInsert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_PhysicalExamination_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        //▼==== #001
        #region 9. Risk Factor
            

        public bool SaveInfectionControl(InfectionControl entity)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start updating SaveInfectionControl.", CurrentUser);
                bool b;
                b = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.SaveInfectionControl(entity);
                AxLogger.Instance.LogInfo("End of RiskFactorInsert. Status: Successful.", CurrentUser);
                return b;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RiskFactorInsert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_PhysicalExamination_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<InfectionControl> GetInfectionControlByPatientID(long? PatientID, int BacteriaType, long? InPatientAdmDisDetailID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start updating GetInfectionControlByPatientID.", CurrentUser);
                List<InfectionControl> lst = new List<InfectionControl>();
                lst = aEMR.DataAccessLayer.Providers.CommonRecordsProvider.Instance.GetInfectionControlByPatientID(PatientID, BacteriaType, InPatientAdmDisDetailID);
                AxLogger.Instance.LogInfo("End of RiskFactorInsert. Status: Successful.", CurrentUser);
                return lst;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RiskFactorInsert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorLibrary.Resources.ErrorNames.COMMON_RECORD_PhysicalExamination_Update);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        //▲==== #001

    }
}
