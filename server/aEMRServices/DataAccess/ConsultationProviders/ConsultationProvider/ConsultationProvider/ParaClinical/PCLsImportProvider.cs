/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
using System.Xml.Linq;
using System.IO;
#region
/*******************************************************************
 * Author: NyNguyen
 * Modified date: 2011-1-5
/*******************************************************************/
#endregion

namespace eHCMS.DAL
{
    public abstract class PCLsImportProvider : DataProviderBase
    {
        static private PCLsImportProvider _instance = null;
        static public PCLsImportProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Consultations.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.Consultations.PCLsImport.ProviderType);
                    _instance = (PCLsImportProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }
        public PCLsImportProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }

        #region 0. examGroup Member
        public abstract IList<PCLExamGroup> GetExamGroup_All();
        //public abstract IList<PCLExamGroup> GetExamGroup_ByPatientReqID(long PatientPCLReqID);
        public abstract PCLExamGroup GetExamGroup_ByID(long PCLExamGroupID);
        //public abstract PCLExamGroup GetExamGroup_ByType(long PCLExamTypeID);
        #endregion

        #region 1. ExamTypes Member
        //public abstract IList<PCLExamType> GetExamTypes_ByGroup(long PCLExamGroupID);
        //public abstract IList<PCLExamType> GetExamTypes_ByPatientReqID(long PatientPCLReqID, long PCLExamGroupID);
        //public abstract IList<PCLExamType> GetExamTypeGroup_TreeView(long PatientPCLReqID);
        
        public abstract IList<PCLExamType> PCLExamTypes_ByPatientPCLReqID(long PatientPCLReqID);

        #endregion

        #region 2. PatientPCLImagingResults Member
        public abstract PatientPCLImagingResult GetPatientPCLImagingResults_ByPtID(long PatientID);
        /*▼====: #001*/
        public abstract PatientPCLImagingResult GetPatientPCLImagingResults_ByID(long PatientPCLReqID, long PCLExamTypeID, long V_PCLRequestType = (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU);
        /*▲====: #001*/
        public abstract PatientPCLImagingResult GetPatientPCLImagingResults_ByIDExt(PCLResultFileStorageDetailSearchCriteria p);        

        public abstract IList<PatientPCLImagingResult> GetPatientPCLImagingResults_ByPatientPCLReqID(long PatientPCLReqID);
   
        #endregion

        #region 3. PatientMedicalRecords
        public abstract PatientMedicalRecord spMedicalRecord_BlankByPtID(long PatientID);
        #endregion

        #region 4. PatientPCLRequest member
        //public abstract IList<PatientPCLRequest> GetPatientPCLRequest_ByPtID(long PatientID);
        //public abstract IList<PatientPCLRequest> GetPatientPCLRequest_ByRegistrationID(long regID);
        //public abstract IList<PatientPCLRequest> PatientPCLRequest_ByPtRegIDV_PCLCategory(long PtRegistrationID, long V_PCLCategory);
        //public abstract IList<PatientPCLRequest> GetPatientPCLRequest_ByPtIDAll(long PatientID);
        //public abstract IList<PatientPCLRequest> GetPatientPCLRequest_ByServiceRecID(long PatientID,long? ServiceRecID,long? PtRegistrationID,DateTime? ExamDate,bool? IsImport);

        //protected override PatientPCLRequestDetail GetPatientPCLRequestDetailsFromReader(IDataReader reader, long? staffID = null)
        //{
        //    PatientPCLRequestDetail p = base.GetPatientPCLRequestDetailsFromReader(reader, staffID);

        //    //Xét Có Kết Quả Chưa
        //    bool HasResutl = false;

        //    Int64 PatientPCLReqID = 0;
        //    Int64 PCLExamTypeID = 0;

        //    Int64.TryParse(reader["PatientPCLReqID"].ToString(), out PatientPCLReqID);
        //    Int64.TryParse(reader["PCLExamTypeID"].ToString(), out PCLExamTypeID);
        //    if (PatientPCLReqID > 0 && PCLExamTypeID > 0)
        //    {
        //        PatientPCLRequestDetails_CheckHasResult(p.PatientPCLReqID, p.PCLExamTypeID, out HasResutl);
        //    }
        //    p.HasResult = HasResutl;
        //    //Xét Có Kết Quả Chưa
        //    return p;
        //}
        //public abstract void PatientPCLRequestDetails_CheckHasResult(
        //   Int64 PatientPCLReqID,
        //   Int64 PCLExamTypeID,
        //   out bool HasResult);


        //public abstract IList<PatientPCLRequest> PatientPCLRequest_OldByPatientIDPtRegistrationID(Int64 PatientID, Int64 PtRegistrationID);


        public abstract void PatientPCLRequest_UpdateV_PCLRequestStatus(Int64 PatientPCLReqID, Int64 V_PCLRequestStatus, Int64 PCLReqItemID, Int64 V_ExamRegStatus, out string Result);

        #endregion

      

        #region 6. PCLResultFileStorageDetails member
        //==== 20161007 CMN Begin: Add List for delete file
        //public abstract bool AddPCLResultFileStorageDetails(PatientPCLImagingResult ExamResult, IList<PCLResultFileStorageDetail> ResultFile);
        public abstract bool AddPCLResultFileStorageDetails(PatientPCLImagingResult ExamResult, List<PCLResultFileStorageDetail> ResultFile, List<PCLResultFileStorageDetail> FileForDelete, List<PCLResultFileStorageDetail> FileForUpdate, string PCLStorePool);
        //==== 20161007 CMN End.
        public abstract List<PCLResultFileStorageDetail> GetPCLResultFileStorageDetails_ByID(long PatientPCLReqID, long PCLExamTypeID);

        #endregion

        #region Save Scan Image to File and Details to DB

        public abstract bool AddScanFileStorageDetails(long StaffID, long PatientID, long PtRegistrationID, string strPatientCode, List<ScanImageFileStorageDetail> NewFileToSave, List<ScanImageFileStorageDetail> SavedFileToDelete, string PCLStorePool);

        public abstract IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails(long? PtRegistrationID, long? PatientID);
        public abstract IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPtRegID(long PtRegistrationID);
        public abstract IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPatientID(long PatientID);

        #endregion


        #region 7. TestingAgency member
        public abstract List<TestingAgency> GetTestingAgency_All();
        public abstract bool TestingAgency_Insert(TestingAgency Agency);

        public abstract bool TestingAgency_Update(TestingAgency Agency);
        public abstract bool TestingAgency_Delete(TestingAgency Agency);

        public abstract bool TestingAgency_InsertXML(List<TestingAgency> Agency);
        public abstract bool TestingAgency_DeleteXML(List<TestingAgency> Agency);
    
        #endregion

        #region 8. Hopitals member
        public abstract List<Hospital> GetHospital_Auto(string HosName, int PageIndex, int PageSize);
    
        #endregion

        #region explorer file
        public abstract List<FolderTree> FillDirectoryAll(string dir,int type);
        public abstract List<String> GetFolderList(string dir);
        public abstract bool CopyAll(IList<PCLResultFileStorageDetail> ListDetails, string ImageStore);
        public abstract bool DeleteFile(string path,List<String> ListPath);
        public abstract bool SaveImageClipBoard(List<byte[]> buffer,string ImageStore);
        #endregion
    }
}

