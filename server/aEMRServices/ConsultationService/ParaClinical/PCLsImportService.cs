/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
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

using eHCMSLanguage;

namespace ConsultationsService.ParaClinical
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]

    public class PCLsImportService : eHCMS.WCFServiceCustomHeader, IPCLsImport
    {
        public PCLsImportService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        #region 0. examGroup Member
        public IList<PCLExamGroup> GetExamGroup_All()
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetExamGroup_All();
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetExamGroup_All();
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetExamGroup_All();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetExamGroup_All. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_EXAMGROUP_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //public IList<PCLExamGroup> GetExamGroup_ByPatientReqID(long PatientPCLReqID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.GetExamGroup_ByPatientReqID(PatientPCLReqID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetExamGroup_ByPatientReqID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_EXAMGROUP_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        public PCLExamGroup GetExamGroup_ByID(long PCLExamGroupID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetExamGroup_ByID(PCLExamGroupID);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetExamGroup_ByID(PCLExamGroupID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetExamGroup_ByID(PCLExamGroupID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetExamGroup_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_EXAMGROUP_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //public PCLExamGroup GetExamGroup_ByType(long PCLExamTypeID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.GetExamGroup_ByType(PCLExamTypeID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetExamGroup_ByType. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_EXAMGROUP_CANNOT_GET);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        #endregion

        #region 1. ExamTypes Member
        //public IList<PCLExamType> GetExamTypes_ByGroup(long PCLExamGroupID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.GetExamTypes_ByGroup(PCLExamGroupID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetExamTypes_ByGroup. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_EXAMTYPE_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        //public IList<PCLExamType> GetExamTypes_ByPatientReqID(long PatientPCLReqID, long PCLExamGroupID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.GetExamTypes_ByPatientReqID(PatientPCLReqID, PCLExamGroupID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetExamTypes_ByPatientReqID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_EXAMTYPE_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}


        public IList<PCLExamType> PCLExamTypes_ByPatientPCLReqID(long PatientPCLReqID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.PCLExamTypes_ByPatientPCLReqID(PatientPCLReqID);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.PCLExamTypes_ByPatientPCLReqID(PatientPCLReqID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.PCLExamTypes_ByPatientPCLReqID(PatientPCLReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PCLExamTypes_ByPatientPCLReqID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_EXAMTYPE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        //public IList<PCLExamType> GetExamTypeGroup_TreeView(long PatientPCLReqID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.GetExamTypeGroup_TreeView(PatientPCLReqID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetExamTypes_ByPatientReqID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_EXAMTYPE_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
    
        //private PCLExamType GetExamTypeGroup_TreeByID(long? ExamTypeID, IList<PCLExamType> lst)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        var results = lst.Where(x => x.PCLExamTypeID == ExamTypeID).FirstOrDefault();
        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetExamTypeGroup_TreeByID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_EXAMTYPE_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        //public List<ExamGroupTypeTree> list = new List<ExamGroupTypeTree>();
        //public List<ExamGroupTypeTree> GetTreeView_ExamGroupType(long PatientPCLReqID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        list.Clear();
        //        IList<PCLExamType> EXamTypetrees = PCLsImportProvider.Instance.GetExamTypeGroup_TreeView(PatientPCLReqID); ;
        //        List<ExamGroupTypeTree> results = new List<ExamGroupTypeTree>();
        //        foreach (PCLExamType item in EXamTypetrees)
        //        {
        //            ExamGroupTypeTree genericItem = new ExamGroupTypeTree(item.PCLExamTypeName, item.PCLExamTypeID, item.PCLExamTypeSubCategoryID, item.PCLExamTypeDescription, item.PCLExamTypeCode, GetExamTypeGroup_TreeByID(item.PCLExamTypeSubCategoryID, EXamTypetrees));
        //            this.list.Add(genericItem);
        //        }

        //        // Get all the first level nodes. In our case it is only one - House M.D.
        //        var rootNodes = this.list.Where(x => x.PCLExamGoupID == x.PCLExamTypeID);

        //        // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
        //        // see bellow how the FindChildren method works
        //        foreach (ExamGroupTypeTree node in rootNodes)
        //        {
        //            this.FindChildren(node);
        //            results.Add(node);
        //        }

        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetTreeView. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }

        //}
        //private void FindChildren(ExamGroupTypeTree item)
        //{
        //    try
        //    {
        //        // find all the children of the item
        //        var children = list.Where(x => x.PCLExamGoupID == item.PCLExamTypeID && x.PCLExamTypeID != item.PCLExamTypeID);

        //        // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
        //        foreach (ExamGroupTypeTree child in children)
        //        {
        //            item.Children.Add(child);
        //            FindChildren(child);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}
       
        #endregion

        #region 2. PatientPCLImagingResults Member
        public PatientPCLImagingResult GetPatientPCLImagingResults_ByPtID(long PatientID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByPtID(PatientID);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByPtID(PatientID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByPtID(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPatientPCLImagingResults_ByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLImagingResult_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        /*▼====: #001*/
        public PatientPCLImagingResult GetPatientPCLImagingResults_ByID(long PatientPCLReqID, long PCLExamTypeID)
        {
            return GetPatientPCLImagingResults_ByID_V2(PatientPCLReqID, PCLExamTypeID, (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU);
        }
        /*▲====: #001*/
        public PatientPCLImagingResult GetPatientPCLImagingResults_ByID_V2(long PatientPCLReqID, long PCLExamTypeID, long V_PCLRequestType)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByID(PatientPCLReqID, PCLExamTypeID, V_PCLRequestType);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByID(PatientPCLReqID, PCLExamTypeID, V_PCLRequestType);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByID(PatientPCLReqID, PCLExamTypeID, V_PCLRequestType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPatientPCLImagingResults_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLImagingResult_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientPCLImagingResult GetPatientPCLImagingResults_ByIDExt(PCLResultFileStorageDetailSearchCriteria p)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByIDExt(p);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByIDExt(p);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByIDExt(p);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPatientPCLImagingResults_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLImagingResult_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientPCLImagingResult> GetPatientPCLImagingResults_ByPatientPCLReqID(long PatientPCLReqID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByPatientPCLReqID(PatientPCLReqID);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByPatientPCLReqID(PatientPCLReqID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPatientPCLImagingResults_ByPatientPCLReqID(PatientPCLReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPatientPCLImagingResults_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLImagingResult_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 3. PatientMedicalRecords

        public PatientMedicalRecord spMedicalRecord_BlankByPtID(long PatientID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.spMedicalRecord_BlankByPtID(PatientID);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.spMedicalRecord_BlankByPtID(PatientID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.spMedicalRecord_BlankByPtID(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving spMedicalRecord_BlankByPtID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PMR_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 4. PatientPCLRequest member

        //public IList<PatientPCLRequest> GetPatientPCLRequest_ByPtID(long PatientID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.GetPatientPCLRequest_ByPtID(PatientID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetPatientPCLRequest_ByPtID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLREQUEST_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        //public IList<PatientPCLRequest> GetPatientPCLRequest_ByRegistrationID(long regID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.GetPatientPCLRequest_ByRegistrationID(regID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetPatientPCLRequest_ByPtID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLREQUEST_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        //public IList<PatientPCLRequest> PatientPCLRequest_ByPtRegIDV_PCLCategory(long PtRegistrationID, long V_PCLCategory)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.PatientPCLRequest_ByPtRegIDV_PCLCategory(PtRegistrationID,V_PCLCategory);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving PatientPCLRequest_ByPtRegIDV_PCLCategory. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLREQUEST_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        //public IList<PatientPCLRequest> GetPatientPCLRequest_ByPtIDAll(long PatientID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.GetPatientPCLRequest_ByPtIDAll(PatientID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetPatientPCLRequest_ByPtIDAll. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLREQUEST_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        //public IList<PatientPCLRequest> GetPatientPCLRequest_ByServiceRecID(long PatientID, long? ServiceRecID, long? PtRegistrationID, DateTime? ExamDate,bool? IsImport)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.GetPatientPCLRequest_ByServiceRecID(PatientID,ServiceRecID,PtRegistrationID,ExamDate,IsImport);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving GetPatientPCLRequest_ByServiceRecID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLREQUEST_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        
      

        //public IList<PatientPCLRequest> PatientPCLRequest_OldByPatientIDPtRegistrationID(Int64 PatientID, Int64 PtRegistrationID)
        //{
        //    try
        //    {
        //        //System.Threading.Thread.Sleep(1000);
        //        return PCLsImportProvider.Instance.PatientPCLRequest_OldByPatientIDPtRegistrationID(PatientID, PtRegistrationID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving PatientPCLRequest_OldByPatientIDPtRegistrationID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_PCLREQUEST_CANNOT_LOAD);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public void PatientPCLRequest_UpdateV_PCLRequestStatus(Int64 PatientPCLReqID, Int64 V_PCLRequestStatus, Int64 PCLReqItemID, Int64 V_ExamRegStatus, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.PatientPCLRequest_UpdateV_PCLRequestStatus(PatientPCLReqID, V_PCLRequestStatus, PCLReqItemID, V_ExamRegStatus, out Result);
                //}
                //else
                //{
                //    PCLsImportProvider.Instance.PatientPCLRequest_UpdateV_PCLRequestStatus(PatientPCLReqID, V_PCLRequestStatus, PCLReqItemID, V_ExamRegStatus, out Result);
                //}
                aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.PatientPCLRequest_UpdateV_PCLRequestStatus(PatientPCLReqID, V_PCLRequestStatus, PCLReqItemID, V_ExamRegStatus, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientPCLRequest_UpdateV_PCLRequestStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #endregion

        #region PCLResultFileStorageDetails member

        //==== 20161005 CMN Begin: Combine Upload file
        public bool UploadImageToDatabase(PatientPCLImagingResult ExamResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ExamResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ExamResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ExamResult, FileForStorage, FileForDelete, null, Globals.AxServerSettings.Hospitals.PCLStorePool);
                //}
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UploadImageToDatabase. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //20161005:End-CongNM

        //==== 20161007 CMN Begin: Add List for delete file
        public bool AddPCLResultFileStorageDetails(PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> ResultFile, List<PCLResultFileStorageDetail> FileForDelete, List<PCLResultFileStorageDetail> FileForUpdate)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, ResultFile, FileForDelete, FileForUpdate, Globals.AxServerSettings.Hospitals.PCLStorePool);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, ResultFile, FileForDelete, FileForUpdate, Globals.AxServerSettings.Hospitals.PCLStorePool);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddPCLResultFileStorageDetails(ImagingResult, ResultFile, FileForDelete, FileForUpdate, Globals.AxServerSettings.Hospitals.PCLStorePool);
                //==== 20161007 CMN End.
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddPCLResultFileStorageDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PCLResultFileStorageDetail> GetPCLResultFileStorageDetails_ByID(long PatientPCLReqID, long PCLExamTypeID)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPCLResultFileStorageDetails_ByID(PatientPCLReqID, PCLExamTypeID);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetPCLResultFileStorageDetails_ByID(PatientPCLReqID, PCLExamTypeID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetPCLResultFileStorageDetails_ByID(PatientPCLReqID, PCLExamTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPCLResultFileStorageDetails_ByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Write Scan Image to File the Save Storage Detail Parameters to DB

        public bool AddScanFileStorageDetails(long StaffID, long PatientID, long PtRegistrationID, string strPatientCode, List<ScanImageFileStorageDetail> NewFileToSave, List<ScanImageFileStorageDetail> SavedFileToDelete)        
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddScanFileStorageDetails(StaffID, PatientID, PtRegistrationID, strPatientCode, NewFileToSave, SavedFileToDelete, Globals.AxServerSettings.Hospitals.PCLStorePool);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.AddScanFileStorageDetails(StaffID, PatientID, PtRegistrationID, strPatientCode, NewFileToSave, SavedFileToDelete, Globals.AxServerSettings.Hospitals.PCLStorePool);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.AddScanFileStorageDetails(StaffID, PatientID, PtRegistrationID, strPatientCode, NewFileToSave, SavedFileToDelete, Globals.AxServerSettings.Hospitals.PCLStorePool);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddPCLResultFileStorageDetails. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPtRegID(long PtRegistrationID)        
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetSavedScanFileStorageDetails_ByPtRegID(PtRegistrationID);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetSavedScanFileStorageDetails_ByPtRegID(PtRegistrationID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetSavedScanFileStorageDetails_ByPtRegID(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSavedScanFileStorageDetails_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPatientID(long PatientID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetSavedScanFileStorageDetails_ByPatientID(PatientID);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetSavedScanFileStorageDetails_ByPatientID(PatientID);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetSavedScanFileStorageDetails_ByPatientID(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPCLResultFileStorageDetails_ByID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        # endregion

        #region 7. TestingAgency member
        public IList<TestingAgency> GetTestingAgency_All()
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetTestingAgency_All();
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetTestingAgency_All();
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetTestingAgency_All();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetTestingAgency_All. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_TESTINGAGENCY_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool TestingAgency_Insert(TestingAgency Agency)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_Insert(Agency);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.TestingAgency_Insert(Agency);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_Insert(Agency);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving TestingAgency_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_TESTINGAGENCY_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool TestingAgency_Update(TestingAgency Agency)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_Update(Agency);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.TestingAgency_Update(Agency);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_Update(Agency);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving TestingAgency_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_TESTINGAGENCY_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool TestingAgency_Delete(TestingAgency Agency)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_Delete(Agency);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.TestingAgency_Delete(Agency);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_Delete(Agency);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving TestingAgency_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_TESTINGAGENCY_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool TestingAgency_InsertXML(List<TestingAgency> Agency)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_InsertXML(Agency);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.TestingAgency_InsertXML(Agency);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_InsertXML(Agency);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving TestingAgency_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_TESTINGAGENCY_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool TestingAgency_DeleteXML(List<TestingAgency> Agency)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_DeleteXML(Agency);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.TestingAgency_DeleteXML(Agency);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.TestingAgency_DeleteXML(Agency);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving TestingAgency_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_TESTINGAGENCY_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 8. Hopitals member
        public IList<Hospital> GetHospital_Auto(string HosName, int PageIndex, int PageSize)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetHospital_Auto(HosName, PageIndex, PageSize);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetHospital_Auto(HosName, PageIndex, PageSize);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetHospital_Auto(HosName, PageIndex, PageSize);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetHospital_Auto. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_HOSPITAL_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region explorer file
        public IList<FolderTree> FillDirectoryAll(string dir,int type)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.FillDirectoryAll(dir, type);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.FillDirectoryAll(dir, type);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.FillDirectoryAll(dir, type);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving FillDirectoryAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FOLDERTREE_CANNOT_LOAD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<String> GetFolderList(string dir)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetFolderList(dir);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.GetFolderList(dir);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.GetFolderList(dir);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetFolderList. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_LISTFOLDER_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool CopyAll(IList<PCLResultFileStorageDetail> ListDetails, string ImageStore)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.CopyAll(ListDetails, ImageStore);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.CopyAll(ListDetails, ImageStore);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.CopyAll(ListDetails, ImageStore);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving CopyAll. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_ADD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteFile(string path, List<String> ListPath)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.DeleteFile(path, ListPath);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.DeleteFile(path, ListPath);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.DeleteFile(path, ListPath);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteFile. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_FILE_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SaveImageClipBoard(List<byte[]> buffer, string ImageStore)
        {
            try
            {
                //System.Threading.Thread.Sleep(1000);
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.SaveImageClipBoard(buffer, ImageStore);
                //}
                //else
                //{
                //    return PCLsImportProvider.Instance.SaveImageClipBoard(buffer, ImageStore);
                //}
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.SaveImageClipBoard(buffer, ImageStore);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveImageClipBoard. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_IMAGE_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        public bool SavePCLImagingResultPDF(byte[] FileExport, string FileName)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.PCLsImportProvider.Instance.SavePCLImagingResultPDF(FileExport, FileName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SavePCLImagingResultPDF. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_RESULTFILE_CANNOT_ADD);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
    }
}


