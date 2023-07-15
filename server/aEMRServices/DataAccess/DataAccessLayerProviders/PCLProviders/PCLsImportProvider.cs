/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
 * 20210701 #008 TNHX: 260 Lưu thêm user bsi mượn
 * 20211013 #009 TNHX: Thêm tích chờ kết quả cho XN
 * 20230607 #010 DatTB: Thêm các trường lưu bệnh phẩm xét nghiệm
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
using eHCMS.DAL;
using System.Net;
#region
/*******************************************************************
 * Author: NyNguyen
 * Modified date: 2011-1-5
/*******************************************************************/
#endregion

namespace aEMR.DataAccessLayer.Providers
{
    public class PCLsImportProvider : DataProviderBase
    {
        static private PCLsImportProvider _instance = null;
        static public PCLsImportProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PCLsImportProvider();
                }
                return _instance;
            }
        }
        public PCLsImportProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }
        public  IList<PCLExamGroup> GetExamGroup_All()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamGroup_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<PCLExamGroup> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetExamGroupCollectionsFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public  PCLExamGroup GetExamGroup_ByID(long PCLExamGroupID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamGroup_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamGroupID", SqlDbType.BigInt, PCLExamGroupID);
                cn.Open();
                PCLExamGroup objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    objItem = GetExamGroupFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }

        public  IList<PCLExamType> PCLExamTypes_ByPatientPCLReqID(long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypes_ByPatientPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cn.Open();
                List<PCLExamType> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLExamTypeColectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        #region 2. PatientPCLImagingResults Member
        public  PatientPCLImagingResult GetPatientPCLImagingResults_ByPtID(long PatientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLImagingResults_BlankByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
                cn.Open();
                PatientPCLImagingResult objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    objItem = GetPatientPCLImagingResultFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }
        /*▼====: #003*/
        //public  PatientPCLImagingResult GetPatientPCLImagingResults_ByID(long PatientPCLReqID, long PCLExamTypeID)
        public  PatientPCLImagingResult GetPatientPCLImagingResults_ByID(long PatientPCLReqID, long PCLExamTypeID, long V_PCLRequestType = (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU)
        /*▲====: #003*/
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLImagingResults_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                /*▼====: #003*/
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, V_PCLRequestType);
                /*▲====: #003*/
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLExamTypeID);
                cn.Open();
                PatientPCLImagingResult objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    objItem = GetPatientPCLImagingResultFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }

        public  PatientPCLImagingResult GetPatientPCLImagingResults_ByIDExt(PCLResultFileStorageDetailSearchCriteria p)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLImagingResults_ByIDExt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, p.PatientPCLReqID);
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, p.PCLExamTypeID);
                cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, p.IsExternalExam);
                cn.Open();
                PatientPCLImagingResult objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    objItem = GetPatientPCLImagingResultFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }

        public  IList<PatientPCLImagingResult> GetPatientPCLImagingResults_ByPatientPCLReqID(long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLImagingResults_GetByPatientPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);

                cn.Open();
                IList<PatientPCLImagingResult> objList = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    objList = GetPatientPCLImagingResultColectionsFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        #endregion

        #region 3. PatientMedicalRecords
        public  PatientMedicalRecord spMedicalRecord_BlankByPtID(long PatientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedicalRecord_BlankByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
                cn.Open();
                PatientMedicalRecord objItem = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    objItem = GetPMRsFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objItem;
            }
        }
        #endregion

        public  void PatientPCLRequest_UpdateV_PCLRequestStatus(Int64 PatientPCLReqID, Int64 V_PCLRequestStatus, Int64 PCLReqItemID, Int64 V_ExamRegStatus, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_UpdateV_PCLRequestStatus", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestStatus));

                cmd.AddParameter("@PCLReqItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLReqItemID));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ExamRegStatus));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        private void DeleteStoredImageFile(List<string> ListFilePathForDeletion)
        {
            foreach (string FilePath in ListFilePathForDeletion)
            {
                if (File.Exists(FilePath))
                {
                    try
                    {
                        File.Delete(FilePath);
                    }
                    catch (Exception ex)
                    {
                        throw (ex);
                    }
                }
            }
        }

        #region PCLResultFileStorageDetails member
        
        private string CreateNameAndPathForImageFile(string strPatientCode, out string strFolderPath, string PCLStorePool, string SubFolderName = "Images")
        {
            strFolderPath = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, PCLStorePool, SubFolderName, DateTime.Now.ToString("yyMMdd")));
            if (!System.IO.Directory.Exists(strFolderPath))
                System.IO.Directory.CreateDirectory(strFolderPath);
            string strFileName = String.Join("-", new string[] { strPatientCode, DateTime.Now.ToString("yyMMddHHmmssfff"), Guid.NewGuid().ToString() });
            //TNV: add extention for images file
            if (SubFolderName == "Images")
                strFileName += ".jpg";
            return strFileName;
        }
        public  bool AddPCLResultFileStorageDetails(PatientPCLImagingResult ExamResult, List<PCLResultFileStorageDetail> ResultFile
            , List<PCLResultFileStorageDetail> FileForDelete, List<PCLResultFileStorageDetail> FileForUpdate, string PCLStorePool, bool IsImportFromPAC = false )
        {
            var strFolderPath = "";
            try
            {
                foreach (var item in ResultFile)
                {

                    strFolderPath = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, PCLStorePool, Globals.AxServerSettings.Pcls.PCLImageFolder, DateTime.Now.ToString("yyMMdd")));
                    if (!System.IO.Directory.Exists(strFolderPath))
                    {
                        System.IO.Directory.CreateDirectory(strFolderPath);
                    }
                    string strFileName = item.PCLResultFileName;
                    string strFolderPathAndFileName = Path.Combine(strFolderPath, strFileName);
                    if (!File.Exists(strFolderPathAndFileName) && item.File != null)
                    {
                        using (FileStream fs = new FileStream(strFolderPathAndFileName, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(item.File, 0, item.File.Length);
                            fs.Close();
                        }
                        item.PCLResultFileName = strFileName;
                        item.PCLResultLocation = strFolderPath;
                    }
                }
            }
            catch (Exception ex)
            {
                List<string> ListFilePathForDeletion = new List<string>();
                foreach (var item in ResultFile)
                {
                    ListFilePathForDeletion.Add(Path.Combine(item.PCLResultLocation, item.PCLResultFileName));
                }
                DeleteStoredImageFile(ListFilePathForDeletion);
                //DeleteFile(ResultFile);
                throw (ex);
            }
            // bool results = false;
            XDocument xml = GenerateListToXMLWithNameSpace_ExamResult(ResultFile);
            //==== 20161007 CMN Begin: Add List for delete file
            XDocument XmlForDelete = GenerateListToXMLWithNameSpace_ExamResult(FileForDelete);
            //==== 20161007 CMN End.

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLImagingResults_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                /*▼====: #003*/
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)ExamResult.PatientPCLRequest.V_PCLRequestType));
                /*▲====: #003*/
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.StaffID));
                //cmd.AddParameter("@PCLRequestFromID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.PCLExtRefID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.PatientPCLReqID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.PCLExamTypeID));
                if (ExamResult.TestingAgency != null)
                {
                    cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertZeroObjectToDBNull(ExamResult.TestingAgency.AgencyID));
                }
                else
                {
                    cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.AgencyID));
                }
                // cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult));
                cmd.AddParameter("@PCLExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ExamResult.PCLExamDate));
                cmd.AddParameter("@DiagnoseOnPCLExam", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ExamResult.DiagnoseOnPCLExam));
                cmd.AddParameter("@PCLExamForOutPatient", SqlDbType.Bit, ConvertNullObjectToDBNull(ExamResult.PCLExamForOutPatient));
                cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(ExamResult.IsExternalExam));
                cmd.AddParameter("@ResultExplanation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ExamResult.ResultExplanation));
                cmd.AddParameter("@XML", SqlDbType.Xml, xml.ToString());
                cmd.AddParameter("@XMLDetail", SqlDbType.Xml, ExamResult.PatientPCLImagingResultDetail == null ? null: GenerateListToXMLWithNameSpace_ExamResultDetail(ExamResult.PatientPCLImagingResultDetail).ToString());
                //==== 20161007 CMN Begin: Add List for delete file
                cmd.AddParameter("@XMLForDeltete", SqlDbType.Xml, XmlForDelete.ToString());
                //==== 20161007 CMN End.
                /*==== #001 ====*/
                cmd.AddParameter("@Height", SqlDbType.Float, ConvertNullObjectToDBNull(ExamResult.Height));
                cmd.AddParameter("@Weight", SqlDbType.Float, ConvertNullObjectToDBNull(ExamResult.Weight));
                cmd.AddParameter("@BSA", SqlDbType.Float, ConvertNullObjectToDBNull(ExamResult.BSA));
                cmd.AddParameter("@ICD10List", SqlDbType.VarChar, ConvertNullObjectToDBNull(ExamResult.ICD10List));
                /*==== #001 ====*/
                /*==== #002 ====*/
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.DeptLocationID));
                /*==== #002 ====*/
                /*▼====: #004*/
                cmd.AddParameter("@HIRepResourceCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(ExamResult.HIRepResourceCode));
                if (FileForUpdate != null && FileForUpdate.Count > 0)
                {
                    cmd.AddParameter("@FileForUpdate", SqlDbType.Xml, GenerateListToXMLWithNameSpace_ExamResult(FileForUpdate).ToString());
                }
                if (!string.IsNullOrEmpty(ExamResult.TemplateResultString) && !IsImportFromPAC)
                {
                    string strFolderPathAndFileName = "";
                    if (!string.IsNullOrEmpty(ExamResult.TemplateResultFileName))
                    {
                        strFolderPathAndFileName = ExamResult.TemplateResultFileName;
                    }
                    else
                    {
                        string strFileName = CreateNameAndPathForImageFile(ExamResult.PatientPCLRequest.PatientCode, out strFolderPath, PCLStorePool, "ReportTemplates");
                        strFolderPathAndFileName = Path.Combine(strFolderPath, strFileName);
                    }
                    if (!string.IsNullOrEmpty(ExamResult.TemplateResultString))
                    {
                        StreamWriter str = new StreamWriter(strFolderPathAndFileName, false);
                        str.Write(ExamResult.TemplateResultString);
                        str.Close();
                        ExamResult.TemplateResultString = strFolderPathAndFileName;
                    }
                }
                cmd.AddParameter("@TemplateResultString", SqlDbType.VarChar, ConvertNullObjectToDBNull(ExamResult.TemplateResultString));
                cmd.AddParameter("@TemplateResultDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ExamResult.TemplateResultDescription));
                cmd.AddParameter("@TemplateResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ExamResult.TemplateResult));
                /*▲====: #004*/
                //==== #005 =====
                cmd.AddParameter("@NumberOfFilmsReceived", SqlDbType.SmallInt, ConvertNullObjectToDBNull(ExamResult.NumberOfFilmsReceived));
                //==== #005 =====
                //▼====== #006
                cmd.AddParameter("@PerformStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.PerformStaffID));
                if(ExamResult.PerformedDate != DateTime.MinValue)
                {
                    cmd.AddParameter("@PerformedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ExamResult.PerformedDate));
                }
                //▲====== #006
                cmd.AddParameter("@Suggest", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ExamResult.Suggest));
                cmd.AddParameter("@ResultStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.ResultStaffID));
                //▼====== #008
                cmd.AddParameter("@UserOfficialAccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.UserOfficialAccountID));
                //▲====== #008
                //▼====== #009
                cmd.AddParameter("@IsWaitResult", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.PatientPCLRequest.IsWaitResult));
                cmd.AddParameter("@IsDone", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.PatientPCLRequest.IsDone));
                //▲====== #009
                //▼==== #010
                cmd.AddParameter("@SpecimenID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.SpecimenID));
                cmd.AddParameter("@SampleQuality", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ExamResult.SampleQuality));
                //▲==== #010
                cn.Open();

                List<string> ListFilePathForDeletion = new List<string>();
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (FileForDelete.Count > 0)
                        {
                            foreach (var item in FileForDelete)
                            {
                                ListFilePathForDeletion.Add(Path.Combine(item.PCLResultLocation, item.PCLResultFileName));
                            }
                            DeleteStoredImageFile(ListFilePathForDeletion);
                        }
                        CleanUpConnectionAndCommand(cn, cmd);
                        return true;
                    }
                    else
                    {
                        foreach (var item in ResultFile)
                        {
                            ListFilePathForDeletion.Add(Path.Combine(item.PCLResultLocation, item.PCLResultFileName));
                        }
                        DeleteStoredImageFile(ListFilePathForDeletion);
                        CleanUpConnectionAndCommand(cn, cmd);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    foreach (var item in ResultFile)
                    {
                        ListFilePathForDeletion.Add(Path.Combine(item.PCLResultLocation, item.PCLResultFileName));
                    }
                    DeleteStoredImageFile(ListFilePathForDeletion);
                    throw (ex);
                }

            }

            //  return results;
        }

        public  List<PCLResultFileStorageDetail> GetPCLResultFileStorageDetails_ByID(long PatientPCLReqID, long PCLExamTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResultFileStorageDetails_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLExamTypeID);
                cn.Open();
                List<PCLResultFileStorageDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLResultFileStorageDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        #endregion

        #region Save Scan Image to File and Details to DB

        private string CreateNameAndPathForScanImageFile(string strPatientCode, out string strFolderPath, string PCLStorePool)
        {
            strFolderPath = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, PCLStorePool, "ScanImages", DateTime.Now.ToString("yyMMdd")));
            if (!System.IO.Directory.Exists(strFolderPath))
                System.IO.Directory.CreateDirectory(strFolderPath);
            string strFileName = String.Join("-", new string[] { strPatientCode, DateTime.Now.ToString("yyMMddHHmmssfff"), Guid.NewGuid().ToString() });
            return strFileName;
        }

        public  bool AddScanFileStorageDetails(long StaffID, long PatientID, long PtRegistrationID, string strPatientCode, List<ScanImageFileStorageDetail> NewFileToSave, List<ScanImageFileStorageDetail> SavedFileToDelete, string PCLStorePool)
        {

            try
            {
                foreach (var item in NewFileToSave)
                {
                    string strFolderPath = "";
                    string strFileName = CreateNameAndPathForScanImageFile(strPatientCode, out strFolderPath, PCLStorePool);
                    string strFolderPathAndFileName = Path.Combine(strFolderPath, strFileName);
                    if (!File.Exists(strFolderPathAndFileName) && item.File != null)
                    {
                        using (FileStream fs = new FileStream(strFolderPathAndFileName, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(item.File, 0, item.File.Length);
                            fs.Close();
                        }
                        item.ScanImageFileName = strFileName;
                        item.ScanFileStorageLocation = strFolderPath;
                    }
                }
            }
            catch (Exception ex)
            {
                List<string> ListFilePathForDeletion = new List<string>();
                foreach (var item in NewFileToSave)
                {
                    ListFilePathForDeletion.Add(Path.Combine(item.ScanFileStorageLocation, item.ScanImageFileName));
                }
                DeleteStoredImageFile(ListFilePathForDeletion);

                throw (ex);
            }

            XDocument xml = GenerateListToXMLWithNameSpace_FromListScanImageFile(NewFileToSave);

            XDocument XmlForDelete = GenerateListToXMLWithNameSpace_FromListScanImageFile(SavedFileToDelete);
            
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_AddUpdateImageFileStorageDetails", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@XML", SqlDbType.Xml, xml.ToString());
                cmd.AddParameter("@XMLForDeltete", SqlDbType.Xml, XmlForDelete.ToString());
                cn.Open();
                List<string> ListFilePathForDeletion = new List<string>();
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (SavedFileToDelete.Count > 0)
                        {
                            foreach (var filePath in SavedFileToDelete)
                            {
                                ListFilePathForDeletion.Add(Path.Combine(filePath.ScanFileStorageLocation, filePath.ScanImageFileName));
                            }
                            DeleteStoredImageFile(ListFilePathForDeletion);
                        }
                        CleanUpConnectionAndCommand(cn, cmd);
                        return true;
                    }
                    else
                    {
                        foreach (var item in NewFileToSave)
                        {
                            ListFilePathForDeletion.Add(Path.Combine(item.ScanFileStorageLocation, item.ScanImageFileName));
                        }

                        DeleteStoredImageFile(ListFilePathForDeletion);
                        CleanUpConnectionAndCommand(cn, cmd);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    foreach (var item in NewFileToSave)
                    {
                        ListFilePathForDeletion.Add(Path.Combine(item.ScanFileStorageLocation, item.ScanImageFileName));
                    }
                    DeleteStoredImageFile(ListFilePathForDeletion);
                    throw (ex);
                }

            }

        }
        public  IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails(long? PtRegistrationID, long? PatientID = null)
        {
            List<ScanImageFileStorageDetail> objLst = null;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetPtImageFileStorageDetails", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetListImageFileStorageDetailFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return objLst;
        }
        public  IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPtRegID(long PtRegistrationID)
        {
            return GetSavedScanFileStorageDetails(PtRegistrationID);
        }

        public  IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPatientID(long PatientID)
        {
            return GetSavedScanFileStorageDetails(null, PatientID);
        }

        #endregion


        #region 7. TestingAgency member
        public  List<TestingAgency> GetTestingAgency_All()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spTestingAgency_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<TestingAgency> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetTestingAgencyCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public  bool TestingAgency_Insert(TestingAgency Agency)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spTestingAgency_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@HosID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Agency.HosID));
                cmd.AddParameter("@AgencyName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Agency.AgencyName));
                cmd.AddParameter("@AgencyAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Agency.AgencyAddress));
                cmd.AddParameter("@AgencyNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Agency.AgencyNotes));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    results = true;
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }
        public  bool TestingAgency_Update(TestingAgency Agency)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spTestingAgency_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Agency.AgencyID));
                cmd.AddParameter("@HosID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Agency.HosID));
                cmd.AddParameter("@AgencyName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Agency.AgencyName));
                cmd.AddParameter("@AgencyAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Agency.AgencyAddress));
                cmd.AddParameter("@AgencyNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Agency.AgencyNotes));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    results = true;
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public  bool TestingAgency_Delete(TestingAgency Agency)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spTestingAgency_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Agency.AgencyID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    results = true;
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public  bool TestingAgency_InsertXML(List<TestingAgency> Agency)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spTestingAgencyInsertXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, TestingAgencys_ConvertListToXml(Agency));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    results = true;
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public  bool TestingAgency_DeleteXML(List<TestingAgency> Agency)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spTestingAgencyDeleteXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, TestingAgencys_ConvertListToXml(Agency));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    results = true;
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        private string TestingAgencys_ConvertListToXml(IList<TestingAgency> lstTestingAgency)
        {
            if (lstTestingAgency != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (TestingAgency item in lstTestingAgency)
                {
                    sb.Append("<TestingAgencys>");
                    if (item.AgencyID > 0)
                        sb.AppendFormat("<AgencyID>{0}</AgencyID>", item.AgencyID);
                    if (item.HosID > 0)
                        sb.AppendFormat("<HosID>{0}</HosID>", item.HosID);
                    sb.AppendFormat("<AgencyNotes>{0}</AgencyNotes>", item.AgencyNotes);
                    sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", item.IsDeleted);

                    sb.Append("</TestingAgencys>");

                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 8. Hopitals member
        public  List<Hospital> GetHospital_Auto(string HosName, int PageIndex, int PageSize)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHopital_Auto", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@HopName", SqlDbType.NVarChar, HosName);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cn.Open();
                List<Hospital> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetHospitalCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        #endregion

        #region explorer file
        public  List<FolderTree> FillDirectoryAll(string dir, int type)
        {
            try
            {
                DirectoryInfo sourcedinfo = new DirectoryInfo(dir);
                List<FolderTree> p = new List<FolderTree>();
                FillDirectory(sourcedinfo, p, type);
                return p;
            }
            catch
            {
                return null;
            }
        }
        public void FillDirectory(DirectoryInfo dir, List<FolderTree> p, int type)
        {
            AddFillDirectory(dir, p, type);
            foreach (DirectoryInfo diSourceDir in dir.GetDirectories())
            {
                FolderTree tr = new FolderTree();
                tr.FolderName = diSourceDir.Name;
                //tr.FolderChildren = null;
                tr.CreateDate = diSourceDir.CreationTime;
                tr.Extention = diSourceDir.Extension;
                //tr.ParentFolderName = diSourceDir.FullName;
                this.FindChildren(diSourceDir, tr, type);
                p.Add(tr);
            }
        }
        public void AddFillDirectory(DirectoryInfo dir, List<FolderTree> p, int type)
        {
            foreach (FileInfo fi in dir.GetFiles())
            {
                if (type == 1)
                {
                    if (fi.Extension == ".bmp" || fi.Extension == ".jpeg" || fi.Extension == ".jpg" || fi.Extension == ".jpe" || fi.Extension == ".jfif" || fi.Extension == ".gif" || fi.Extension == ".png")
                    {
                        FolderTree tr = new FolderTree();
                        tr.FolderName = fi.Name;
                        //tr.FolderChildren = null;
                        tr.CreateDate = fi.CreationTime;
                        tr.Extention = fi.Extension;
                        tr.ParentFolderName = fi.FullName;
                        p.Add(tr);
                    }
                }
                else if (type == 2)
                {
                    if (fi.Extension == ".mp4" || fi.Extension == ".mov" || fi.Extension == ".wmv" || fi.Extension == ".flv" || fi.Extension == ".wma")
                    {
                        FolderTree tr = new FolderTree();
                        tr.FolderName = fi.Name;
                        //tr.FolderChildren = null;
                        tr.CreateDate = fi.CreationTime;
                        tr.Extention = fi.Extension;
                        tr.ParentFolderName = fi.FullName;
                        p.Add(tr);
                    }
                }
                else
                {
                    if (fi.Extension == ".txt" || fi.Extension == ".pdf" || fi.Extension == ".csv" || fi.Extension == ".doc" || fi.Extension == ".docx")
                    {
                        FolderTree tr = new FolderTree();
                        tr.FolderName = fi.Name;
                        //tr.FolderChildren = null;
                        tr.CreateDate = fi.CreationTime;
                        tr.Extention = fi.Extension;
                        tr.ParentFolderName = fi.FullName;
                        p.Add(tr);
                    }
                }

            }
        }
        private void FindChildren(DirectoryInfo dir, FolderTree p, int type)
        {
            p.FolderChildren = new List<FolderTree>();
            AddFillDirectory(dir, p.FolderChildren, type);
            foreach (DirectoryInfo diSourceDir in dir.GetDirectories())
            {
                FolderTree tr = new FolderTree();
                tr.FolderName = diSourceDir.Name;
                tr.FolderChildren = new List<FolderTree>();
                tr.CreateDate = diSourceDir.CreationTime;
                tr.Extention = diSourceDir.Extension;
                //  tr.ParentFolderName = diSourceDir.FullName;
                p.FolderChildren.Add(tr);
                AddFillDirectory(diSourceDir, p.FolderChildren, type);
                FindChildren(diSourceDir, tr, type);
            }
        }
        public  List<String> GetFolderList(string dir)
        {
            try
            {
                List<String> p = new List<String>();
                DirectoryInfo sourcedinfo = new DirectoryInfo(dir);
                foreach (DirectoryInfo diSourceDir in sourcedinfo.GetDirectories())
                {
                    p.Add(diSourceDir.Name);
                }
                return p;
            }
            catch
            {
                return null;
            }
        }

        public  bool CopyAll(IList<PCLResultFileStorageDetail> ListDetails, string ImageStore)
        {
            try
            {
                foreach (PCLResultFileStorageDetail p in ListDetails)
                {
                    if (p.Flag.HasValue && p.Flag == 1)
                    {
                        CopyFile(ImageStore, p);
                        p.Flag = 0;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void CopyFile(string ImageStore, PCLResultFileStorageDetail p)
        {
            DirectoryInfo target = new DirectoryInfo(ImageStore + @"\" + p.PCLResultLocation);
            //check if the target directory exists
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }
            FileInfo fi = new FileInfo(p.FullPath);
            fi.CopyTo(Path.Combine(target.ToString(), p.PCLResultFileName), true);
        }

        public  bool DeleteFile(string path, List<String> ListPath)
        {
            try
            {
                foreach (String item in ListPath)
                {
                    FileInfo fi = new FileInfo(path + @"\" + item);
                    fi.Delete();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public  bool SaveImageClipBoard(List<byte[]> buffers, string ImageStore)
        {
            System.Drawing.Image newImage;
            foreach (byte[] buffer in buffers)
            {
                if (buffer.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length))
                    {
                        ms.Write(buffer, 0, buffer.Length);
                        newImage = System.Drawing.Image.FromStream(ms, true);
                        newImage.Save(ImageStore + @"\" + System.Guid.NewGuid().ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }

                }
            }
            return true;

        }
        #endregion

        public bool SavePCLImagingResultPDF(byte[] FileExport, string FileName)
        {
            var strFolderPath = "";
            try
            {
                strFolderPath = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Globals.AxServerSettings.Hospitals.PDFStorePool, DateTime.Now.ToString("yyMMdd")));
                if (!Directory.Exists(strFolderPath))
                {
                    Directory.CreateDirectory(strFolderPath);
                }
                string strFolderPathAndFileName = Path.Combine(strFolderPath, FileName);
                if (!File.Exists(strFolderPathAndFileName) && FileExport != null)
                {
                    using (FileStream fs = new FileStream(strFolderPathAndFileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(FileExport, 0, FileExport.Length);
                        fs.Close();
                    }
                }
                string ftpUsername = Globals.AxServerSettings.Hospitals.FTPAdminUserName;
                string ftpPassword = Globals.AxServerSettings.Hospitals.FTPAdminPassword;
                if (DirectoryExists(Globals.AxServerSettings.Hospitals.FTPLinkKQXN + DateTime.Now.ToString("yyyyMMdd"), ftpUsername, ftpPassword))
                {

                    using (WebClient client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                        client.UploadFile(Globals.AxServerSettings.Hospitals.FTPLinkKQXN + DateTime.Now.ToString("yyyyMMdd") + "/ChuaKySo/" + FileName, "STOR", strFolderPathAndFileName);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //List<string> ListFilePathForDeletion = new List<string>();

                //ListFilePathForDeletion.Add(Path.Combine(item.PCLResultLocation, item.PCLResultFileName));

                //DeleteStoredImageFile(ListFilePathForDeletion);
                //DeleteFile(ResultFile);
                throw (ex);
            }

        }
        public bool DirectoryExists(string directory, string ftpUName, string ftpPWord)
        {
            bool directoryExists;

            var request = (FtpWebRequest)WebRequest.Create(directory);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(ftpUName, ftpPWord);

            try
            {
                using (request.GetResponse())
                {
                    directoryExists = true;
                }
            }
            catch (WebException)
            {
                directoryExists = false;
                WebRequest ftpRequest = WebRequest.Create(directory);
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpRequest.Credentials = new NetworkCredential(ftpUName, ftpPWord);
                try
                {
                    using (ftpRequest.GetResponse())
                    {
                        WebRequest ftpRequestCKS = WebRequest.Create(directory + "/ChuaKySo/");
                        ftpRequestCKS.Method = WebRequestMethods.Ftp.MakeDirectory;
                        ftpRequestCKS.Credentials = new NetworkCredential(ftpUName, ftpPWord);
                        try
                        {
                            using (ftpRequestCKS.GetResponse())
                            {
                                directoryExists = true;
                            }
                        }
                        catch (WebException webEx)
                        {
                            throw new Exception(webEx.Message);
                        }
                    }
                }
                catch (WebException webEx)
                {
                    throw new Exception(webEx.Message);
                }
            }

            return directoryExists;
        }

    }
}

