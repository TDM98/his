/*
 * 20170414 #001 CMN: Added more ImagingResult Attribute
 * 20170512 #002 CMN: Added DeptLocation to ImagingResult
 * 20170608 #003 CMN: Added PatientType to Update PCL
 * 20180613 #004 TBLD: Added HIRepResourceCode to PatientPCLImagingResult
 * 20181122 #005 TBL: Added NumberOfFilmsReceived
 * 20181207 #006 TTM: BM 0005339: Lưu trường PerformStaffID và PerformedDate
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using eHCMS.DAL;
using System.IO;
namespace eHCMS.DAL
{
    public class SqlPCLsImportProvider : PCLsImportProvider
    {
        public SqlPCLsImportProvider()
            : base()
        {

        }

        #region 0. examGroup Member
        public override IList<PCLExamGroup> GetExamGroup_All()
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
                return objLst;
            }
        }
        //public override IList<PCLExamGroup> GetExamGroup_ByPatientReqID(long PatientPCLReqID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamGroup_ByPatientReqID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);

        //        cn.Open();
        //        List<PCLExamGroup> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetExamGroupCollectionsFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}
        public override PCLExamGroup GetExamGroup_ByID(long PCLExamGroupID)
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
                return objItem;
            }
        }
        //public override PCLExamGroup GetExamGroup_ByType(long PCLExamTypeID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamGroup_ByType", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLExamTypeID);
        //        cn.Open();
        //        PCLExamGroup objItem = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader.Read())
        //        {
        //            objItem = GetExamGroupFromReader(reader);
        //        }
        //        reader.Close();
        //        return objItem;
        //    }
        //}
        #endregion

        #region 1. ExamTypes Member
        //public override IList<PCLExamType> GetExamTypes_ByGroup(long PCLExamGroupID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamTypes_ByGroup", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PCLExamGroupID", SqlDbType.BigInt, PCLExamGroupID);
        //        cn.Open();
        //        List<PCLExamType> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPCLExamTypeColectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}
        //public override IList<PCLExamType> GetExamTypes_ByPatientReqID(long PatientPCLReqID, long PCLExamGroupID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamTypes_ByPatientReqID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);
        //        cmd.AddParameter("@PCLExamGroupID", SqlDbType.BigInt, PCLExamGroupID);
        //        cn.Open();
        //        List<PCLExamType> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPCLExamTypeColectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}
        //public override IList<PCLExamType> GetExamTypeGroup_TreeView(long PatientPCLReqID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamGroup_TypeTreeView", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);
        //        cn.Open();
        //        List<PCLExamType> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPCLExamTypeColectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        //public override PCLExamType GetExamTypes_ByID(long PCLExamTypeID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamGroup_ByID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLExamTypeID);
        //        cn.Open();
        //        PCLExamType objItem = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader.Read())
        //        {
        //            objItem = GetExamTypesFromReader(reader);
        //            reader.Close();

        //            cmd.CommandText = "spPCLExamGroup_ByType";
        //            reader = ExecuteReader(cmd);
        //            if (reader != null)
        //            {
        //                if (reader.Read())
        //                {
        //                    objItem.PCLExamGroup = GetExamGroupFromReader(reader);
        //                }
        //                reader.Close();
        //            }
        //        }
        //        return objItem;
        //    }
        //}

        //public override IList<PCLExamType> GetPCLExamTypes_ByPCLFormID(long PCLFormID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamTypes_ByPatientReqID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PCLFormID);
        //        cn.Open();
        //        List<PCLExamType> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetExamTypesColectionsFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        public override IList<PCLExamType> PCLExamTypes_ByPatientPCLReqID(long PatientPCLReqID)
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
                return objLst;
            }
        }

        #endregion

        #region 2. PatientPCLImagingResults Member
        public override PatientPCLImagingResult GetPatientPCLImagingResults_ByPtID(long PatientID)
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
                return objItem;
            }
        }
        /*▼====: #003*/
        //public override PatientPCLImagingResult GetPatientPCLImagingResults_ByID(long PatientPCLReqID, long PCLExamTypeID)
        public override PatientPCLImagingResult GetPatientPCLImagingResults_ByID(long PatientPCLReqID, long PCLExamTypeID, long V_PCLRequestType = (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU)
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
                return objItem;
            }
        }

        public override PatientPCLImagingResult GetPatientPCLImagingResults_ByIDExt( PCLResultFileStorageDetailSearchCriteria p)
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
                return objItem;
            }
        }

        public override IList<PatientPCLImagingResult> GetPatientPCLImagingResults_ByPatientPCLReqID(long PatientPCLReqID)
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
                return objList;
            }
        }
        #endregion

        #region 3. PatientMedicalRecords
        public override PatientMedicalRecord spMedicalRecord_BlankByPtID(long PatientID)
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
                return objItem;
            }
        }
        #endregion

        #region 4. PatientPCLRequest member
        //public override IList<PatientPCLRequest> GetPatientPCLRequest_ByPtID(long PatientID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ByPtID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
        //        cn.Open();
        //        List<PatientPCLRequest> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPatientPCLRequestCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        //public override IList<PatientPCLRequest> GetPatientPCLRequest_ByRegistrationID(long regID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ByRegistrationID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, regID);
        //        cn.Open();
        //        List<PatientPCLRequest> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPatientPCLRequestCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}


        //public override IList<PatientPCLRequest> PatientPCLRequest_ByPtRegIDV_PCLCategory(long PtRegistrationID, long V_PCLCategory)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ByPtRegIDV_PCLCategory", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
        //        cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLCategory));
        //        cn.Open();
        //        List<PatientPCLRequest> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPatientPCLRequestCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}


        //public override IList<PatientPCLRequest> PatientPCLRequest_OldByPatientIDPtRegistrationID(Int64 PatientID, Int64 PtRegistrationID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientPCLRequest_OldByPatientIDPtRegistrationID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
        //        cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt,ConvertNullObjectToDBNull(PtRegistrationID));
        //        cn.Open();
        //        List<PatientPCLRequest> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPatientPCLRequestCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        //public override IList<PatientPCLRequest> GetPatientPCLRequest_ByServiceRecID(long PatientID, long? ServiceRecID, long? PtRegistrationID, DateTime? ExamDate,bool? IsImport)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ByServiceRecID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
        //        cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt,ConvertNullObjectToDBNull(ServiceRecID));
        //        cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
        //        cmd.AddParameter("@ExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ExamDate));
        //        cmd.AddParameter("@IsImport", SqlDbType.Bit, ConvertNullObjectToDBNull(IsImport));
        //        cn.Open();
        //        List<PatientPCLRequest> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPatientPCLRequestCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

       

        //public override void PatientPCLRequestDetails_CheckHasResult(
        //   Int64 PatientPCLReqID,
        //   Int64 PCLExamTypeID,
        //   out bool HasResult)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        HasResult = false;

        //        SqlCommand cmd = new SqlCommand("spPatientPCLRequestDetails_CheckHasResult", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);
        //        cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLExamTypeID);

        //        cmd.AddParameter("@HasResult", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);

        //        cn.Open();

        //        ExecuteNonQuery(cmd);

        //        if (cmd.Parameters["@HasResult"].Value != null)
        //            HasResult = Convert.ToBoolean(cmd.Parameters["@HasResult"].Value);
        //    }
        //}


        //public override IList<PatientPCLRequest> GetPatientPCLRequest_ByPtIDAll(long PatientID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ByPtIDAll", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
        //        cn.Open();
        //        List<PatientPCLRequest> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPatientPCLRequestCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}


        public override void PatientPCLRequest_UpdateV_PCLRequestStatus(Int64 PatientPCLReqID,Int64 V_PCLRequestStatus,Int64 PCLReqItemID, Int64 V_ExamRegStatus,  out string Result)
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
            }
        }


        #endregion

        #region 5. PCLForm
        //public override IList<PCLForm> GetForms_ByDeptID(long? deptID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLForms_ByDeptID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@DeptID", SqlDbType.BigInt, deptID);
        //        cn.Open();
        //        List<PCLForm> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPCLFormsColectionsFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        #endregion

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
        //private void DeleteFile(List<PCLResultFileStorageDetail> FileForDelete)
        //{
        //    foreach (PCLResultFileStorageDetail DelFile in FileForDelete)
        //    {
        //        string FilePath = Path.Combine(DelFile.PCLResultLocation, DelFile.PCLResultFileName);
        //        if (File.Exists(FilePath))
        //            try
        //            {
        //                File.Delete(FilePath);
        //            }
        //            catch(Exception ex)
        //            {
        //                throw (ex);
        //            }
        //    }
        //}
        private string CreateNameAndPathForImageFile(string strPatientCode, out string strFolderPath, string PCLStorePool, string SubFolderName = "Images")
        {
            strFolderPath = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, PCLStorePool, SubFolderName, DateTime.Now.ToString("yyMMdd")));
            if (!System.IO.Directory.Exists(strFolderPath))
                System.IO.Directory.CreateDirectory(strFolderPath);
            string strFileName = String.Join("-", new string[] { strPatientCode, DateTime.Now.ToString("yyMMddHHmmssfff"), Guid.NewGuid().ToString() });
            return strFileName;
        }
        public override bool AddPCLResultFileStorageDetails(PatientPCLImagingResult ExamResult, List<PCLResultFileStorageDetail> ResultFile, List<PCLResultFileStorageDetail> FileForDelete, List<PCLResultFileStorageDetail> FileForUpdate, string PCLStorePool)
        {
            string strFolderPath = "";
            try
            {
                foreach (var item in ResultFile)
                {
                    strFolderPath = "";
                    string strFileName = CreateNameAndPathForImageFile(ExamResult.PatientPCLRequest.PatientCode, out strFolderPath, PCLStorePool);
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
                foreach(var item in ResultFile)
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
                if (!string.IsNullOrEmpty(ExamResult.TemplateResultString))
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
                cmd.AddParameter("@PerformedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ExamResult.PerformedDate));
                //▲====== #006
                cmd.AddParameter("@Suggest", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ExamResult.Suggest));
                cmd.AddParameter("@ResultStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ExamResult.ResultStaffID));
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
                        return true;
                    }
                    else
                    {
                        foreach (var item in ResultFile)
                        {
                            ListFilePathForDeletion.Add(Path.Combine(item.PCLResultLocation, item.PCLResultFileName));
                        }
                        DeleteStoredImageFile(ListFilePathForDeletion);
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

        public override List<PCLResultFileStorageDetail> GetPCLResultFileStorageDetails_ByID(long PatientPCLReqID, long PCLExamTypeID)
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

        public override bool AddScanFileStorageDetails(long StaffID, long PatientID, long PtRegistrationID, string strPatientCode, List<ScanImageFileStorageDetail> NewFileToSave, List<ScanImageFileStorageDetail> SavedFileToDelete, string PCLStorePool)
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
                            foreach(var filePath in SavedFileToDelete)
                            {
                                ListFilePathForDeletion.Add(Path.Combine(filePath.ScanFileStorageLocation, filePath.ScanImageFileName));
                            }
                            DeleteStoredImageFile(ListFilePathForDeletion);                            
                        }
                        return true;
                    }
                    else
                    {
                        foreach (var item in NewFileToSave)
                        {
                            ListFilePathForDeletion.Add(Path.Combine(item.ScanFileStorageLocation, item.ScanImageFileName));
                        }

                        DeleteStoredImageFile(ListFilePathForDeletion);
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
        public override IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails(long? PtRegistrationID, long? PatientID = null)
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
            }
            return objLst;
        }
        public override IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPtRegID(long PtRegistrationID)
        {
            return GetSavedScanFileStorageDetails(PtRegistrationID);
        }

        public override IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPatientID(long PatientID)
        {
            return GetSavedScanFileStorageDetails(null, PatientID);
        }

        #endregion


        #region 7. TestingAgency member
        public override List<TestingAgency> GetTestingAgency_All()
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
                return objLst;
            }
        }
        public override bool TestingAgency_Insert(TestingAgency Agency)
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
            }
            return results;
        }
        public override bool TestingAgency_Update(TestingAgency Agency)
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
            }
            return results;
        }

        public override bool TestingAgency_Delete(TestingAgency Agency)
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
            }
            return results;
        }

        public override bool TestingAgency_InsertXML(List<TestingAgency> Agency)
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
            }
            return results;
        }

        public override bool TestingAgency_DeleteXML(List<TestingAgency> Agency)
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
        public override List<Hospital> GetHospital_Auto(string HosName, int PageIndex, int PageSize)
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
                return objLst;
            }
        }
        #endregion

        #region explorer file
        public override List<FolderTree> FillDirectoryAll(string dir, int type)
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
        public override List<String> GetFolderList(string dir)
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

        public override bool CopyAll(IList<PCLResultFileStorageDetail> ListDetails, string ImageStore)
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

        public override bool DeleteFile(string path, List<String> ListPath)
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

        public override bool SaveImageClipBoard(List<byte[]> buffers, string ImageStore)
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

    }
}
