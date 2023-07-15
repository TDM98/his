using System;
using System.Collections.Generic;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2011-01-06
 * Contents: Consultation Services
/*******************************************************************/
#endregion
/*
 * 20161215 #001 CMN: Added control for choose doctor and date
 * 20170608 #002 CMN: Added PatientType to Update PCL
 * 20180613 #003 TBLD: Lay HIRepResourceCode theo PCLResultParamImpID
 * 20180521 #001 TBLD: Added PatientFindBy
*/
namespace eHCMS.DAL
{
    public class SqlPCLsProvider : PCLsProvider
    {
        public SqlPCLsProvider()
            : base()
        {

        }

        #region 0. ExamTypes Member
        //public override List<PCLExamType> PCLExamTypes_ByPCLFormID(Int64 PCLFormID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamTypes_ByPCLFormID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, PCLFormID);
        //        cmd.AddParameter("@classpatient", SqlDbType.TinyInt, ConvertNullObjectToDBNull(ClassPatient));
        //        cn.Open();
        //        List<PCLExamType> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPCLExamTypeColectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        //public override IList<PCLExamType> GetPCLExamTypes_ByPCLFormID_Edit(long PCLFormID, long PatientPCLReqID,int? ClassPatient)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamTypes_ByPCLFormID_Edit", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, PCLFormID);
        //        cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, PatientPCLReqID);
        //        cmd.AddParameter("@classpatient", SqlDbType.TinyInt, ConvertNullObjectToDBNull(ClassPatient));
        //        cn.Open();
        //        List<PCLExamType> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetExamTypesColectionsFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}
        #endregion

        #region 1. PCLForm
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


        //public override IList<PCLForm> PCLForms_ByDeptIDV_PCLCategory(Int64 DeptID, Int64 V_PCLCategory)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLForms_ByDeptIDV_PCLCategory", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@DeptID", SqlDbType.BigInt,ConvertNullObjectToDBNull(DeptID));
        //        cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLCategory));
        //        cn.Open();
        //        List<PCLForm> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPCLFormsColectionsFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        public override IList<PCLGroup> GetAllPCLGroups(long? pclCategoryID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLGroups_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(pclCategoryID));
                cn.Open();
                List<PCLGroup> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLGroupColectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        public override IList<PCLExamType> GetAllActiveExamTypes(PCLExamTypeSearchCriteria searchCriteria, int pageIndex, int pageSize, bool bCountTotal, string orderBy, out int totalCount)
        {
            totalCount = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypes_GetAllActiveItemsByGroupID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchCriteria.PCLGroupID));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(orderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<PCLExamType> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    retVal = GetPCLExamTypeColectionFromReader(reader);
                    reader.Close();

                    if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                    {
                        totalCount = (int)cmd.Parameters["@Total"].Value;
                    }
                    else
                        totalCount = -1;
                }
                return retVal;
            }
        }
        public override IList<PCLExamType> GetAllActiveExamTypes(PCLExamTypeSearchCriteria searchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypes_GetAllActiveItemsByGroupID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchCriteria.PCLGroupID));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(searchCriteria.OrderBy));
                cn.Open();
                List<PCLExamType> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    retVal = GetPCLExamTypeColectionFromReader(reader);
                    reader.Close();
                }
                return retVal;
            }
        }
        #endregion

        #region 2. PatientPCLRequest

        public override bool AddFullPtPCLReq(PatientPCLRequest entity, long PatientID, long? PtRegistrationID, out long newPatientPCLReqID)
        {
            newPatientPCLReqID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_InsertFully", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                SqlParameter par2 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                SqlParameter par3 = cmd.Parameters.Add("@DoctorID", SqlDbType.BigInt);

                SqlParameter par11 = cmd.Parameters.Add("@ReqFromDeptLocID", SqlDbType.BigInt);
                SqlParameter par12 = cmd.Parameters.Add("@PCLDeptLocID", SqlDbType.BigInt);

                SqlParameter par5 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
                SqlParameter par6 = cmd.Parameters.Add("@DoctorComments", SqlDbType.NVarChar);
                SqlParameter par7 = cmd.Parameters.Add("@IsExternalExam", SqlDbType.Bit);
                SqlParameter par8 = cmd.Parameters.Add("@IsCaseOfEmergency", SqlDbType.Bit);
                SqlParameter par9 = cmd.Parameters.Add("@XMLPtPCLRequestDetails", SqlDbType.Xml);
                SqlParameter par10 = cmd.Parameters.Add("@NewPatientPCLReqID", SqlDbType.BigInt);
                par10.Direction = ParameterDirection.Output;

                par1.Value = PatientID;
                par2.Value = PtRegistrationID;
                par3.Value = entity.PatientServiceRecord.StaffID;

                //////par11.Value = entity.ReqFromDeptLocID;
                //////par12.Value = entity.PCLDeptLocID;

                par5.Value = entity.Diagnosis;
                par6.Value = entity.DoctorComments;
                par7.Value = entity.IsExternalExam;
                par8.Value = entity.IsCaseOfEmergency;
                par9.Value = entity.PatientPCLRequestDetailsXML;
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                newPatientPCLReqID = (long)cmd.Parameters["@NewPatientPCLReqID"].Value;
                return retVal > 0;
            }
        }

        //public override bool AddFullPtPCLReqServiceRecID(PatientPCLRequest entity, out long newPatientPCLReqID)
        //{
        //    newPatientPCLReqID = 0;
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientPCLRequest_InsertFullyServiceRecID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        SqlParameter par1 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);
        //        SqlParameter par3 = cmd.Parameters.Add("@DoctorID", SqlDbType.BigInt);
        //        SqlParameter par5 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
        //        SqlParameter par6 = cmd.Parameters.Add("@DoctorComments", SqlDbType.NVarChar);
        //        SqlParameter par7 = cmd.Parameters.Add("@IsExternalExam", SqlDbType.Bit);
        //        SqlParameter par8 = cmd.Parameters.Add("@IsCaseOfEmergency", SqlDbType.Bit);
        //        SqlParameter par9 = cmd.Parameters.Add("@XMLPtPCLRequestDetails", SqlDbType.Xml);
        //        SqlParameter par10 = cmd.Parameters.Add("@NewPatientPCLReqID", SqlDbType.BigInt);
        //        par10.Direction = ParameterDirection.Output;

        //        par1.Value = entity.ServiceRecID;
        //        par3.Value = entity.PatientServiceRecord.StaffID;
        //        par5.Value = entity.Diagnosis;
        //        par6.Value = entity.DoctorComments;
        //        par7.Value = entity.IsExternalExam;
        //        par8.Value = entity.IsCaseOfEmergency;
        //        par9.Value = entity.PatientPCLRequestDetailsXML;
        //        cn.Open();
        //        int retVal = ExecuteNonQuery(cmd);
        //        newPatientPCLReqID = (long)cmd.Parameters["@NewPatientPCLReqID"].Value;
        //        return retVal > 0;
        //    }
        //}
        //public override bool UpdateFullPtPCLReqServiceRecID(PatientPCLRequest entity)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientPCLRequest_UpdateFullyServiceRecID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        SqlParameter par1 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);
        //        SqlParameter par3 = cmd.Parameters.Add("@DoctorID", SqlDbType.BigInt);
        //        SqlParameter par4 = cmd.Parameters.Add("@PCLFormID", SqlDbType.BigInt);
        //        SqlParameter par5 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
        //        SqlParameter par6 = cmd.Parameters.Add("@DoctorComments", SqlDbType.NVarChar);
        //        SqlParameter par7 = cmd.Parameters.Add("@IsExternalExam", SqlDbType.Bit);
        //        SqlParameter par8 = cmd.Parameters.Add("@IsCaseOfEmergency", SqlDbType.Bit);
        //        SqlParameter par9 = cmd.Parameters.Add("@XMLPtPCLRequestDetails", SqlDbType.Xml);
        //        SqlParameter par10 = cmd.Parameters.Add("@PatientPCLReqID", SqlDbType.BigInt);

        //        par1.Value = entity.ServiceRecID;
        //        par3.Value = entity.PatientServiceRecord.StaffID;
        //        par5.Value = entity.Diagnosis;
        //        par6.Value = entity.DoctorComments;
        //        par7.Value = entity.IsExternalExam;
        //        par8.Value = entity.IsCaseOfEmergency;
        //        par9.Value = entity.PatientPCLRequestDetailsXML;
        //        par10.Value = entity.PatientPCLReqID;
        //        cn.Open();
        //        int retVal = ExecuteNonQuery(cmd);
        //        if (retVal > 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}
        public override PatientMedicalRecord GetPMRByPtPCLFormRequest(long LoggedDoctorID, long PatientID, long? PtRegistrationID, long? PatientRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalRecords_ByPClFormRequest", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(LoggedDoctorID));
                cmd.AddParameter("@PatientRecIDPara", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientRecID));

                cn.Open();

                PatientMedicalRecord obj = new PatientMedicalRecord();
                IDataReader reader = ExecuteReader(cmd);
                obj = GetPMRItemFromReader(reader);
                reader.Close();
                return obj;

            }
        }

        public override List<PatientPCLRequest> PatientPCLRequest_ByPatientID_Paging(
            PatientPCLRequestSearchCriteria SearchCriteria, long V_RegistrationType,
          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
      )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ByPatientID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.PtRegistrationID));
                cmd.AddParameter("@TypeList", SqlDbType.Int, (int)(SearchCriteria.LoaiDanhSach));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));

                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return lst;
            }
        }

        public override List<PatientPCLRequest> GetPatientPCLRequestList_ByRegistrationID(long RegistrationID, long V_RegistrationType, long V_PCLMainCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPCLRequestByRegistrationIDForConsultation", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLMainCategory));

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                return lst;
            }
        }

        public override IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPatientPCLReqID(PatientPCLRequestDetailSearchCriteria SearchCriteria, long V_RegistrationType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequestDetails_ByPatientPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientPCLReqID));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, SearchCriteria.V_ExamRegStatus);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cn.Open();
                IList<PatientPCLRequestDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestDetailsCollectionFromReader(reader);

                //foreach (var patientPclRequestDetail in objLst)
                //{
                //    var ListDeptLoc=ConfigurationManagerProviders.Instance.ListDeptLocation_ByPCLExamTypeID(patientPclRequestDetail.PCLExamType.PCLExamTypeID);
                //    patientPclRequestDetail.PCLExamType.ObjDeptLocationList = new ObservableCollection<DeptLocation>(ListDeptLoc); 
                //}

                reader.Close();
                return objLst;
            }
        }

        public override IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPtRegistrationID(long PtRegistrationID, long V_RegistrationType, long V_PCLMainCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequestDetails_ByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLMainCategory));
                cn.Open();
                IList<PatientPCLRequestDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestDetailsCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }


        //Phiếu cuối cùng
        public override PatientPCLRequest PatientPCLRequest_RequestLastest(Int64 PatientID, long V_RegistrationType, long? V_PCLMainCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_RequestLastest", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLMainCategory));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cn.Open();

                PatientPCLRequest obj = new PatientPCLRequest();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    obj = GetPatientPCLRequestFromReader(reader);
                }
                reader.Close();
                return obj;
            }
        }
        //Phiếu cuối cùng


        //ds Phieu cuoi cung trong ngay chua tra tien
        public override IList<PatientPCLRequest> PatientPCLReq_RequestLastestInDayNotPaid(long PatientID, long V_PCLRequestType, long ReqFromDeptLocID, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLReq_RequestLastestInDayNotPaid", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                cmd.AddParameter("@ReqFromDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ReqFromDeptLocID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();

                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                foreach (var item in lst)
                {
                    //Doc detail ra
                    item.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>(PatientPCLRequestDetails_ByPatientPCLReqIDSimple(item.PatientPCLReqID));
                }

                return lst;
            }
        }
        //ds Phieu cuoi cung trong ngay chua tra tien


        //Chi tiet 1 phieu simple
        private IList<PatientPCLRequestDetail> PatientPCLRequestDetails_ByPatientPCLReqIDSimple(long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequestDetails_ByPatientPCLReqIDSimple", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                IList<PatientPCLRequestDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestDetailsCollectionFromReader(reader);

                reader.Close();

                return objLst;
            }
        }
        //Chi tiet 1 phieu simple



        //Danh sách phiếu yêu cầu CLS
        public override IList<PatientPCLRequest> PatientPCLRequest_SearchPaging(
         PatientPCLRequestSearchCriteria SearchCriteria,

       int PageIndex,
       int PageSize,
       string OrderBy,
       bool CountTotal,
       out int Total
   )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_SearchPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeLocationsDeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeLocationsDeptLocationID));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLResultParamImpID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_ExamRegStatus));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(SearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FullName));
                cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLRequestNumID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = int.MaxValue;

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return lst;
            }
        }
        //Danh sách phiếu yêu cầu CLS

        public override PatientPCLRequest GetPatientPCLRequestResultsByReqID(PatientPCLRequestSearchCriteria SearchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPatientPCLRequestResultsByReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientPCLReqID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));
                cn.Open();
                List<PatientPCLRequest> mPCLRequestCollection = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    mPCLRequestCollection = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();
                return mPCLRequestCollection.FirstOrDefault();
            }
        }

        public override IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPaging(
                           PatientPCLRequestSearchCriteria SearchCriteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
           out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ViewResult_SearchPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLResultParamImpID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_ExamRegStatus));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(SearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FullName));
                cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLRequestNumID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = int.MaxValue;

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return lst;
            }
        }

        /// <summary>
        /// union thêm ngoại viện 
        /// </summary>

        public override IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPagingNew(
                           PatientPCLRequestSearchCriteria SearchCriteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
           out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ViewResult_SearchPagingNew", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLResultParamImpID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_ExamRegStatus));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(SearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FullName));
                cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLRequestNumID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = int.MaxValue;

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return lst;
            }
        }


        //Danh sách phiếu các lần trước
        public override IList<PatientPCLRequest> PatientPCLRequest_ByPatientIDV_Param_Paging(
         PatientPCLRequestSearchCriteria SearchCriteria,

       int PageIndex,
       int PageSize,
       string OrderBy,
       bool CountTotal,
       out int Total
   )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ByPatientIDV_Param_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));
                cmd.AddParameter("@V_Param", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_Param));
                /*▼====: #001*/
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));
                /*▲====: #001*/
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return lst;
            }
        }
        //Danh sách phiếu các lần trước


        #endregion

        #region 3. PCL result
        //public override IList<PatientPCLRequest> GetPCLResult_PtPCLRequest(long? registrationID, bool isImported, long V_PCLCategory)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLResult_PatientPCLRequest_ByRegistrationID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(registrationID));
        //        if (isImported)
        //        {
        //            cmd.AddParameter("@IsImported", SqlDbType.Bit, 1);
        //        }
        //        else
        //        {
        //            cmd.AddParameter("@IsImported", SqlDbType.Bit, 0);
        //        }
        //        cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, V_PCLCategory);
        //        cn.Open();

        //        List<PatientPCLRequest> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPatientPCLRequestCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        public override IList<PCLExamGroup> GetPCLResult_PCLExamGroup(long? ptID, long? ptPCLReqID, bool isImported)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResult_PCLExamGroup_ByPtPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptPCLReqID));
                if (isImported)
                {
                    cmd.AddParameter("@IsImported", SqlDbType.Bit, 1);
                }
                else
                {
                    cmd.AddParameter("@IsImported", SqlDbType.Bit, 0);
                }
                cn.Open();

                List<PCLExamGroup> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetExamGroupCollectionsFromReader(reader);
                reader.Close();
                return objLst;

            }
        }

        //public override IList<PCLExamType> GetPCLResult_PCLExamType(long? ptID, long? PCLExamGroupID, bool isImported)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLResult_PCLExamType_ByExamGroupID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptID));
        //        cmd.AddParameter("@PCLExamGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamGroupID));
        //        if (isImported)
        //        {
        //            cmd.AddParameter("@IsImported", SqlDbType.Bit, 1);
        //        }
        //        else
        //        {
        //            cmd.AddParameter("@IsImported", SqlDbType.Bit, 0);
        //        }

        //        cn.Open();

        //        List<PCLExamType> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetPCLExamTypeColectionFromReader(reader);
        //        reader.Close();
        //        return objLst;

        //    }
        //}
        public override PCLResultFileStorageDetail GetPCLResultFileStorageDetailFromReader(IDataReader reader)
        {
            PCLResultFileStorageDetail p = base.GetPCLResultFileStorageDetailFromReader(reader);
            if (reader.HasColumn("ServiceRecID") && reader["ServiceRecID"] != DBNull.Value)
            {
                p.ServiceRecID = reader["ServiceRecID"] as long?;
            }
            if (reader.HasColumn("PCLRequestDate") && reader["PCLRequestDate"] != DBNull.Value)
            {
                p.PCLRequestDate = (DateTime)reader["PCLRequestDate"];
            }
            if (reader.HasColumn("AgencyID") && reader["AgencyID"] != DBNull.Value)
            {
                p.AgencyID = reader["AgencyID"] as long?;
            }
            if (reader.HasColumn("AgencyNameAdd") && reader["AgencyNameAdd"] != DBNull.Value)
            {
                p.AgencyNameAddress = reader["AgencyNameAdd"].ToString();
            }
            if (reader.HasColumn("PCLExamDate") && reader["PCLExamDate"] != DBNull.Value)
            {
                p.PCLExamDate = (DateTime)reader["PCLExamDate"];
            }
            if (reader.HasColumn("DiagnoseOnPCLExam") && reader["DiagnoseOnPCLExam"] != DBNull.Value)
            {
                p.DiagnoseOnPCLExam = reader["DiagnoseOnPCLExam"].ToString();
            }

            if (reader.HasColumn("ExamDoctorID") && reader["ExamDoctorID"] != DBNull.Value)
            {
                p.ExamDoctorID = reader["ExamDoctorID"] as long?;
            }
            if (reader.HasColumn("ExamDoctorFullName") && reader["ExamDoctorFullName"] != DBNull.Value)
            {
                p.ExamDoctorFullName = reader["ExamDoctorFullName"].ToString();
            }
            if (reader.HasColumn("PCLExamForOutPatient") && reader["PCLExamForOutPatient"] != DBNull.Value)
            {
                p.PCLExamForOutPatient = reader["PCLExamForOutPatient"] as bool?;
            }
            try
            {
                if (reader.HasColumn("IsExternalExam") && reader["IsExternalExam"] != DBNull.Value)
                {
                    p.IsExternalExam = reader["IsExternalExam"] as bool?;
                }
            }
            catch
            { }

            //try
            //{
            //    p.PCLExamGroupID = reader["PCLExamGroupID"] as long?;
            //    p.PCLExamTypeID = reader["PCLExamTypeID"] as long?;
            //}
            //catch
            //{ }
            if (reader.HasColumn("RequestDoctor") && reader["RequestDoctor"] != DBNull.Value)
            {
                p.RequestDoctor = reader["RequestDoctor"].ToString();
            }
            if (reader.HasColumn("ResultType") && reader["ResultType"] != DBNull.Value)
            {
                p.ResultType = reader["ResultType"].ToString();
            }
            if (reader.HasColumn("PCLExamTypeCode") && reader["PCLExamTypeCode"] != DBNull.Value)
            {
                p.PCLExamTypeCode = reader["PCLExamTypeCode"].ToString();
            }
            if (reader.HasColumn("PathNameOfResource") && reader["PathNameOfResource"] != DBNull.Value)
            {
                p.PathNameOfResource = reader["PathNameOfResource"].ToString();
            }
            if (reader.HasColumn("IsImage") && reader["IsImage"] != DBNull.Value)
            {
                p.IsImage = (bool)reader["IsImage"];
            }
            if (reader.HasColumn("IsVideo") && reader["IsVideo"] != DBNull.Value)
            {
                p.IsVideo = (bool)reader["IsVideo"];
            }
            if (reader.HasColumn("IsDocument") && reader["IsDocument"] != DBNull.Value)
            {
                p.IsDocument = (bool)reader["IsDocument"];
            }
            if (reader.HasColumn("IsOthers") && reader["IsOthers"] != DBNull.Value)
            {
                p.IsOthers = (bool)reader["IsOthers"];
            }
            return p;
        }
        /*▼====: #002*/
        //public override IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID)
        public override IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID, long V_PCLRequestType = (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU)
        /*▲====: #002*/
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResultFileStorageDetails_ByPCLExTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                /*▼====: #002*/
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                /*▲====: #002*/
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptPCLReqID));
                cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLGroupID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cn.Open();

                List<PCLResultFileStorageDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLResultFileStorageDetailCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsExt(PCLResultFileStorageDetailSearchCriteria p)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResultFileStorageDetails_ByPCLExTypeIDExt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PatientPCLReqID));
                cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PCLGroupID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PCLExamTypeID));
                cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(p.IsExternalExam));
                cn.Open();

                List<PCLResultFileStorageDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLResultFileStorageDetailCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsByPCLImgResultID(long PCLImgResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResultFileStorageDetails_GetByPCLImgResultID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLImgResultID));

                cn.Open();

                List<PCLResultFileStorageDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLResultFileStorageDetailCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        #endregion

        #region 4. PCL Laboratory

        public override IList<MedicalSpecimensCategory> GetAllMedSpecCatg()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedicalSpecimensCategory_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                List<MedicalSpecimensCategory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetAllMedSpecCatgColectionsFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override IList<MedicalSpecimen> GetMedSpecsByCatgID(short? medSpecCatgID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedicalSpecimens_ByCatgID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@MedSpecCatID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(medSpecCatgID));
                cn.Open();

                List<MedicalSpecimen> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetMedSpecsByCatgIDColectionsFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override string PCLExamTestItems_ByPatientID(long PatientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTestItems_ByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@Results", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                cmd.ExecuteNonQuery();
                if (cmd.Parameters["@Results"].Value != DBNull.Value && !string.IsNullOrEmpty(cmd.Parameters["@Results"].Value.ToString()))
                {
                    return cmd.Parameters["@Results"].Value.ToString();
                }
                return "";
            }
        }

        public override IList<PCLExamTestItems> PCLExamTestItems_SearchPaging(GeneralSearchCriteria SearchCriteria, int PageIndex, int PageSize, bool bCount, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTestItems_SearchPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTestItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FindName));
                cmd.AddParameter("@PCLExamTestItemCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FindCode));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.FindID));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@bCount", SqlDbType.Bit, ConvertNullObjectToDBNull(bCount));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();

                List<PCLExamTestItems> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLExamTestItemsColectionFromReader(reader);
                reader.Close();
                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = Convert.ToInt32(cmd.Parameters["@Total"].Value);
                }

                return objLst;
            }
        }

        public override bool PCLExamTestItems_Save(PCLExamTestItems Item, out long PCLExamTestItemID)
        {
            PCLExamTestItemID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTestItems_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTestItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.PCLExamTestItemID));
                cmd.AddParameter("@PCLExamTestItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemName));
                cmd.AddParameter("@PCLExamTestItemDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemDescription));
                cmd.AddParameter("@PCLExamTestItemCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemCode));
                cmd.AddParameter("@PCLExamTestItemUnit", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemUnit));
                cmd.AddParameter("@PCLExamTestItemRefScale", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemRefScale));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Item.IsActive));
                cmd.AddParameter("@IsBold", SqlDbType.Bit, ConvertNullObjectToDBNull(Item.IsBold));
                cmd.AddParameter("@TestItemIsExamType", SqlDbType.Bit, ConvertNullObjectToDBNull(Item.TestItemIsExamType));
                cmd.AddParameter("@IsNoNeedResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.IsNoNeedResult));
                cmd.AddParameter("@OutputID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@PCLExamTestItemHICode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemHICode));
                cmd.AddParameter("@PCLExamTestItemHIName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemHIName));
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                if (cmd.Parameters["@OutputID"].Value != DBNull.Value)
                {
                    PCLExamTestItemID = (long)cmd.Parameters["@OutputID"].Value;
                }

                return count > 0;
            }
        }

        public override bool RefDepartmentReqCashAdv_Save(RefDepartmentReqCashAdv Target, out long ID)
        {
            ID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartmentReqCashAdv_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RefDepartmentReqCashAdvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Target.RefDepartmentReqCashAdvID));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Target.DeptID));
                cmd.AddParameter("@CashAdvAmtReq", SqlDbType.Money, ConvertNullObjectToDBNull(Target.CashAdvAmtReq));
                cmd.AddParameter("@ID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                if (cmd.Parameters["@ID"].Value != DBNull.Value)
                {
                    ID = (long)cmd.Parameters["@ID"].Value;
                }

                return count > 0;
            }
        }

        public override bool RefDepartmentReqCashAdv_Delete(long RefDepartmentReqCashAdvID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartmentReqCashAdv_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RefDepartmentReqCashAdvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefDepartmentReqCashAdvID));
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public override List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartmentReqCashAdv_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SearchText", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchText));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@bCount", SqlDbType.Bit, ConvertNullObjectToDBNull(bCount));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<RefDepartmentReqCashAdv> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefDepartmentReqCashAdvCollectionFromReader(reader);
                reader.Close();
                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                return objLst;
            }
        }

        public override bool PCLExamTypeServiceTarget_Save(PCLExamTypeServiceTarget Target, out long ID)
        {
            ID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeServiceTargetID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Target.PCLExamTypeServiceTargetID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Target.PCLExamTypeID));
                cmd.AddParameter("@MondayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.MondayTargetNumberOfCases));
                cmd.AddParameter("@TuesdayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.TuesdayTargetNumberOfCases));
                cmd.AddParameter("@WednesdayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.WednesdayTargetNumberOfCases));
                cmd.AddParameter("@ThursdayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.ThursdayTargetNumberOfCases));
                cmd.AddParameter("@FridayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.FridayTargetNumberOfCases));
                cmd.AddParameter("@SaturdayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.SaturdayTargetNumberOfCases));
                cmd.AddParameter("@SundayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.SundayTargetNumberOfCases));
                cmd.AddParameter("@ID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                if (cmd.Parameters["@ID"].Value != DBNull.Value)
                {
                    ID = (long)cmd.Parameters["@ID"].Value;
                }

                return count > 0;
            }
        }

        public override bool PCLExamTypeServiceTarget_Delete(long PCLExamTypeServiceTargetID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeServiceTargetID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeServiceTargetID));
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public override List<PCLExamTypeServiceTarget> PCLExamTypeServiceTarget_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SearchText", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchText));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@bCount", SqlDbType.Bit, ConvertNullObjectToDBNull(bCount));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<PCLExamTypeServiceTarget> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLExamTypeServiceTargetColectionFromReader(reader);
                reader.Close();
                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                return objLst;
            }
        }

        public override bool PCLExamTypeServiceTarget_Checked(long PCLExamTypeID, DateTime Date)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_Checked", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(Date));
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public override bool PCLExamTypeServiceTarget_Checked_Appointment(long PCLExamTypeID, DateTime Date)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_Checked_Appointment", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(Date));
                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        public override IList<PCLExamTestItems> PatientPCLLaboratoryResults_ByExamTest_Paging(long PatientID, long PCLExamTestItemID, long PCLExamTypeID, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRow, out int MaxRow)
        {
            TotalRow = 0;
            MaxRow = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResults_ByExamTest_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PCLExamTestItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTestItemID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@TotalRow", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@MaxRow", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();

                List<PCLExamTestItems> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLExamTestItemsColectionFromReader(reader);
                reader.Close();
                if (cmd.Parameters["@TotalRow"].Value != DBNull.Value)
                {
                    TotalRow = (int)cmd.Parameters["@TotalRow"].Value;
                }
                if (cmd.Parameters["@MaxRow"].Value != DBNull.Value)
                {
                    MaxRow = (int)cmd.Parameters["@MaxRow"].Value;
                }
                return objLst;
            }
        }

        public override string PatientPCLLaboratoryResults_ByExamTest_Crosstab(long PatientID, string strXml, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalCol)
        {
            TotalCol = 0;
            DataTable nYTEST = PatientPCLLaboratoryResults_ByExamTest_Crosstab_New(PatientID, strXml, FromDate, ToDate, PageIndex, PageSize, out TotalCol);
            DataTable dtResult = Pivot(nYTEST, "PCLExamTestItemName", "SamplingDate", "Value");

            var sb = new StringBuilder();
            sb.Append("<data>");
            sb.Append("<columns>");
            //sb.Append("<column name=\"Name\"></column>");
            //sb.Append("<column name=\"Age\"></column>");
            sb.Append("[ValueCols]");
            sb.Append("</columns>");

            sb.Append("<rows>");
            //sb.Append("<row>");
            //sb.Append("<cell>Bob</cell>");
            //sb.Append("<cell>30</cell>");
            sb.Append("[ValueCells]");
            //sb.Append("</row>");
            sb.Append("</rows>");

            sb.Append("</data>");


            //DataTable Column
            var sbValueCols = new StringBuilder();

            //=================================================
            //Tính toán phun về Client nhận chuỗi
            foreach (DataColumn dc in dtResult.Columns)
            {
                sbValueCols.Append("<column name=\"" + dc.ColumnName.Trim() + "\"></column>");
            }

            //DataTable Row
            var sbValueCells = new StringBuilder();

            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                sbValueCells.Append("<row>");
                for (int j = 0; j < dtResult.Columns.Count; j++)
                {
                    sbValueCells.Append("<cell>" + dtResult.Rows[i][j].ToString().Replace("<", "&lt;").Replace(">", "&gt;") + "</cell>");
                }
                sbValueCells.Append("</row>");
            }

            sb = sb.Replace("[ValueCols]", sbValueCols.ToString());
            sb = sb.Replace("[ValueCells]", sbValueCells.ToString());

            return sb.ToString();

        }

        public DataTable PatientPCLLaboratoryResults_ByExamTest_Crosstab_New(long PatientID, string strXml, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalCol)
        {
            TotalCol = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResults_ByExamTest_Crosstab_new", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                //cmd.AddParameter("@PCLExamTestItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTestItemID));
                //cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@ListID", SqlDbType.Xml, ConvertNullObjectToDBNull(strXml));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@TotalCol", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                // cmd.ExecuteNonQuery();
                IDataReader reader = cmd.ExecuteReader();
                DataTable tbl = null;
                if (reader != null)
                {
                    tbl = new DataTable();
                    tbl.Load(reader);
                }
                reader.Close();
                if (cmd.Parameters["@TotalCol"].Value != DBNull.Value)
                {
                    TotalCol = (int)cmd.Parameters["@TotalCol"].Value;
                }


                return tbl;
                // return "";
            }
        }

        public DataTable Pivot(DataTable src, string VerticalColumnName, string HorizontalColumnName, string ValueColumnName)
        {
            DataTable dst = new DataTable();
            if (src == null || src.Rows.Count == 0)
                return dst;

            // find all distinct names for column and row
            ArrayList ColumnValues = new ArrayList();
            ArrayList RowValues = new ArrayList();
            foreach (DataRow dr in src.Rows)
            {
                // find all column values
                object column = dr[VerticalColumnName];
                if (!ColumnValues.Contains(column))
                    ColumnValues.Add(column);

                //find all row values
                object row = dr[HorizontalColumnName];
                if (!RowValues.Contains(row))
                    RowValues.Add(row);
            }

            //ColumnValues.Sort();
            //RowValues.Sort();

            //create columns
            dst = new DataTable();
            dst.Columns.Add("Tên", src.Columns[VerticalColumnName].DataType);
            // dst.Columns.Add(VerticalColumnName, src.Columns[VerticalColumnName].DataType);
            dst.Columns.Add("ĐVT", src.Columns["PCLExamTestItemUnit"].DataType);
            dst.Columns.Add("Thang Tham Chiếu", src.Columns["PCLExamTestItemRefScale"].DataType);

            Type t = src.Columns[ValueColumnName].DataType;
            foreach (object ColumnNameInRow in RowValues)
            {
                dst.Columns.Add(ColumnNameInRow.ToString(), t);
            }

            //create destination rows
            foreach (object RowName in ColumnValues)
            {
                DataRow NewRow = dst.NewRow();
                NewRow["Tên"] = RowName.ToString();//VerticalColumnName
                dst.Rows.Add(NewRow);
            }

            //fill out pivot table
            foreach (DataRow drSource in src.Rows)
            {
                object key = drSource[VerticalColumnName];
                string ColumnNameInRow = Convert.ToString(drSource[HorizontalColumnName]);
                int index = ColumnValues.IndexOf(key);
                dst.Rows[index][ColumnNameInRow] = drSource[ValueColumnName];// sum(dst.Rows[index][ColumnNameInRow], drSource[ValueColumnName]);

                dst.Rows[index][1] = drSource["PCLExamTestItemUnit"];
                dst.Rows[index][2] = drSource["PCLExamTestItemRefScale"];
            }

            return dst;
        }

        dynamic sum(dynamic a, dynamic b)
        {
            if (a is DBNull && b is DBNull)
                return DBNull.Value;
            else if (a is DBNull && !(b is DBNull))
                return b;
            else if (!(a is DBNull) && b is DBNull)
                return a;
            else
                return a + b;
        }

        public override IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLLaboratoryResults_With_ResultOld", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                cn.Open();

                List<PatientPCLLaboratoryResultDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLLaboratoryResultDetailCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_No_ResultOld(long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResults_ByPatientPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cn.Open();

                List<PatientPCLLaboratoryResultDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLLaboratoryResultDetailCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override IList<PatientPCLLaboratoryResultDetail> GetPtPCLLabExamTypes_ByPtPCLReqID(long PatientID, long? PtPCLReqID, long? PCLExamGroupID, long? PCLExamTypeID, bool isImported)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPtPCLLabExamTypes_ByPtPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtPCLReqID));
                cmd.AddParameter("@PCLExamGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamGroupID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                if (isImported)
                {
                    cmd.AddParameter("@IsImported", SqlDbType.Bit, 1);
                }
                else
                {
                    cmd.AddParameter("@IsImported", SqlDbType.Bit, 0);
                }

                cn.Open();

                List<PatientPCLLaboratoryResultDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPCLLabExamTypesByReqIDColectionsFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override IList<MedicalSpecimenInfo> GetMedSpecInfo_ByPtPCLReqID(long? PtPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedicalSpecimenInfo_ByPtPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtPCLReqID));

                cn.Open();

                List<MedicalSpecimenInfo> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetMedSpecByReqIDColectionsFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        //public override bool AddFullPtPCLLabResult(PatientPCLLaboratoryResult entity, out long newLabResultID)
        //{
        //    newLabResultID = 0;
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResults_InsertFully", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        SqlParameter par1 = cmd.Parameters.Add("@PCLExtRefID", SqlDbType.BigInt);
        //        SqlParameter par2 = cmd.Parameters.Add("@PatientPCLReqID", SqlDbType.BigInt);
        //        SqlParameter par3 = cmd.Parameters.Add("@StaffID", SqlDbType.BigInt);
        //        SqlParameter par4 = cmd.Parameters.Add("@AgencyID", SqlDbType.BigInt);
        //        SqlParameter par5 = cmd.Parameters.Add("@SamplingDate", SqlDbType.DateTime);
        //        SqlParameter par6 = cmd.Parameters.Add("@SampleCode", SqlDbType.NVarChar);
        //        SqlParameter par7 = cmd.Parameters.Add("@DiagnosisOnExam", SqlDbType.NVarChar);
        //        SqlParameter par8 = cmd.Parameters.Add("@PCLForOutPatient", SqlDbType.Bit);
        //        SqlParameter par9 = cmd.Parameters.Add("@IsExternalExam", SqlDbType.Bit);
        //        SqlParameter par10 = cmd.Parameters.Add("@MedSpecID", SqlDbType.BigInt);
        //        SqlParameter par11 = cmd.Parameters.Add("@XMLPtPCLLabResultDetails", SqlDbType.Xml);
        //        SqlParameter par12 = cmd.Parameters.Add("@NewLabResultID", SqlDbType.BigInt);
        //        par12.Direction = ParameterDirection.Output;


        //        par1.Value = ConvertNullObjectToDBNull(entity.PCLExtRefID);
        //        par2.Value = ConvertNullObjectToDBNull(entity.PatientPCLReqID);
        //        par3.Value = ConvertNullObjectToDBNull(entity.StaffID);
        //        par4.Value = ConvertNullObjectToDBNull(entity.AgencyID);
        //        par5.Value = ConvertNullObjectToDBNull(entity.SamplingDate);
        //        par6.Value = ConvertNullObjectToDBNull(entity.SampleCode);
        //        par7.Value = ConvertNullObjectToDBNull(entity.DiagnosisOnExam);
        //        par8.Value = ConvertNullObjectToDBNull(entity.PCLForOutPatient);
        //        par9.Value = ConvertNullObjectToDBNull(entity.IsExternalExam);
        //        par10.Value = ConvertNullObjectToDBNull(entity.MedicalSpecimen.MedSpecID);
        //        par11.Value = ConvertNullObjectToDBNull(entity.PatientPCLLabResultDetailsXML);
        //        cn.Open();
        //        int retVal = ExecuteNonQuery(cmd);
        //        newLabResultID = (long)cmd.Parameters["@NewLabResultID"].Value;
        //        return retVal > 0;
        //    }
        //}


        public override bool AddPatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResultDetailsInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LabResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.LabResultID));
                cmd.AddParameter("@PCLExamTypeTestItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamTypeTestItemID));
                cmd.AddParameter("@Value", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Value));
                cmd.AddParameter("@IsAbnormal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsAbnormal));
                cmd.AddParameter("@Comments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Comments));
                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        // 20181216 TNHX: [BM0005430] Add PatientID for report PCLDEPARTMENT_LABORATORY_RESULT
        public override bool UpdatePatientPCLLaboratoryResultDetail(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity, PatientPCLLaboratoryResult entity, long PCLRequestTypeID, long PatientID
            , out string errorOutput)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                List<SqlCommand> lstCmd = new List<SqlCommand>();

                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResultDetailsUpdateXml", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@PCLExtRefID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExtRefID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientPCLReqID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.StaffID));
                //cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.AgencyID));
                //cmd.AddParameter("@SampleCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SampleCode));
                //cmd.AddParameter("@DiagnosisOnExam", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisOnExam));
                //cmd.AddParameter("@PCLForOutPatient", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PCLForOutPatient));
                //cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsExternalExam));
                //if (entity.MedSpecID>0)
                //    cmd.AddParameter("@MedSpecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MedSpecID));
                //else
                //    cmd.AddParameter("@MedSpecID", SqlDbType.BigInt, null);

                cmd.AddParameter("@LabResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.LabResultID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLRequestTypeID));
                errorOutput = "";
                cmd.AddParameter("@DataXML", SqlDbType.Xml, PatientPCLLabResDetails_ConvertListToXml(allPatientPCLLaboratoryResultDetailentity));
                cmd.AddParameter("@errorOutput", SqlDbType.NVarChar, ConvertNullObjectToDBNull(errorOutput), ParameterDirection.Output);

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (cmd.Parameters["@errorOutput"].Value != null)
                {
                    errorOutput = cmd.Parameters["@errorOutput"].Value.ToString();
                }
                else
                    errorOutput = "";

                if (retVal > 0)
                    return true;
                else return false;

            }
        }

        public override bool DeletePatientPCLLaboratoryResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID, long PCLExamTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLRequestTypeID));
                cmd.AddParameter("@CancelStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CancelStaffID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;

            }
        }

        public override bool DeletePatientPCLImagingResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLImagingResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLRequestTypeID));
                cmd.AddParameter("@CancelStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CancelStaffID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;

            }
        }

        //public override bool UpdatePatientPCLLaboratoryResultDetail(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        List<SqlCommand> lstCmd = new List<SqlCommand>();
        //        SqlTransaction transaction;
        //        cn.Open();
        //        transaction = cn.BeginTransaction();

        //        foreach (var entity in allPatientPCLLaboratoryResultDetailentity)
        //        {
        //            SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResultDetailsUpdate", cn);
        //            cmd.CommandType = CommandType.StoredProcedure;


        //            cmd.AddParameter("@LabResultDetailID", SqlDbType.BigInt,
        //                             ConvertNullObjectToDBNull(entity.LabResultDetailID));
        //            cmd.AddParameter("@LabResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.LabResultID));
        //            cmd.AddParameter("@PCLExamTypeTestItemID", SqlDbType.BigInt,
        //                             ConvertNullObjectToDBNull(entity.PCLExamTypeTestItemID));
        //            cmd.AddParameter("@Value", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Value));
        //            cmd.AddParameter("@IsAbnormal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsAbnormal));
        //            cmd.AddParameter("@Comments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Comments));

        //            cmd.Transaction = transaction;
        //            lstCmd.Add(cmd);
        //        }
        //        //status o day

        //        try
        //        {
        //            foreach (SqlCommand sQLCmd in lstCmd)
        //            {
        //                sQLCmd.ExecuteNonQuery();
        //            }
        //            transaction.Commit();
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            return false;
        //        }

        //        return true;
        //    }




        //}

        public override bool DeletePatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResultDetailsDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LabResultDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.LabResultDetailID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        private string PatientPCLLabResDetails_ConvertListToXml(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetail)
        {
            if (allPatientPCLLaboratoryResultDetail != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PatientPCLLaboratoryResultDetail item in allPatientPCLLaboratoryResultDetail)
                {
                    if (item.PCLExamTypeTestItemID > 0)
                    {
                        sb.Append("<PatientPCLLabResDetails>");
                        sb.AppendFormat("<LabResultDetailID>{0}</LabResultDetailID>", item.LabResultDetailID);

                        sb.AppendFormat("<LabResultID>{0}</LabResultID>", item.LabResultID);
                        sb.AppendFormat("<PCLExamTypeTestItemID>{0}</PCLExamTypeTestItemID>", item.PCLExamTypeTestItemID);
                        if (item.Value != null)
                        {
                            sb.AppendFormat("<Value>{0}</Value>", item.Value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;"));
                        }
                        sb.AppendFormat("<IsAbnormal>{0}</IsAbnormal>", item.IsAbnormal);
                        sb.AppendFormat("<Comments>{0}</Comments>", item.Comments);
                        sb.AppendFormat("<Value_Old>{0}</Value_Old>", item.Value_Old);
                        sb.Append("</PatientPCLLabResDetails>");
                    }

                }
                sb.Append("</DS>");



                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public override IList<PatientPCLRequest> ListPatientPCLRequest_LAB_Paging(long PatientID, long? DeptLocID, long V_PCLRequestType,
          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spListPatientPCLRequest_LAB_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = 120;

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return lst;
            }
        }

        public override IList<PatientPCLRequest> LIS_Order(string SoPhieuChiDinh, bool IsAll)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_LIS_Order", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SoPhieuChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SoPhieuChiDinh));
                cmd.AddParameter("@IsAll", SqlDbType.Bit, ConvertNullObjectToDBNull(IsAll));
                cn.Open();

                List<PatientPCLRequest> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }

        }

        public override bool LIS_Result(PatientPCLRequest_LABCom ParamLabCom)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {

                SqlCommand cmd = new SqlCommand("sp_LIS_Result", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SoPhieuChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ParamLabCom.SoPhieuChiDinh));
                cmd.AddParameter("@MaDichVu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ParamLabCom.MaDichVu));
                cmd.AddParameter("@KetQua", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ParamLabCom.KetQua));
                cmd.AddParameter("@ChiSoBinhThuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(""));
                cmd.AddParameter("@DonViTinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(""));
                cmd.AddParameter("@BatThuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull("0"));
                cmd.AddParameter("@SoBenhPham", SqlDbType.NVarChar, ConvertNullObjectToDBNull("1"));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                return false;
            }
        }

        #endregion

        #region 5. Tuyen
        public override IList<PCLItem> GetPCLItemsByPCLFormID(long? PCLFormID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_PCL_GetPCLItems_ByFormID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLFormID));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(""));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLItem> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPCLItemCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                return retVal;
            }
        }
        #endregion


        #region dinh

        #region PCLExamParamResult
        public override List<PCLExamParamResult> GetPCLExamParamResultList(long PCLExamResultID, long ParamEnum)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamResultID));
                cmd.AddParameter("@ParamEnum", SqlDbType.BigInt, ConvertNullObjectToDBNull(ParamEnum));

                cn.Open();
                List<PCLExamParamResult> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPCLExamParamResultCollectionFromReader(reader);
                reader.Close();


                return retVal;
            }
        }

        public override PCLExamParamResult GetPCLExamParamResult(long PCLExamResultID, long ParamEnum)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamResultID));
                cmd.AddParameter("@ParamEnum", SqlDbType.BigInt, ConvertNullObjectToDBNull(ParamEnum));

                cn.Open();
                PCLExamParamResult retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetPCLExamParamResultFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddPCLExamParamResult(PCLExamParamResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ParamEnum", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ParamEnum));
                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PCLExamGroupTemplateResultID));
                cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.GroupName));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdatePCLExamParamResult(PCLExamParamResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamResultID));
                cmd.AddParameter("@ParamEnum", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ParamEnum));
                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PCLExamGroupTemplateResultID));
                cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.GroupName));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeletePCLExamParamResult(PCLExamParamResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamResultID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region PCLExamResultTemplate
        public override List<PCLExamResultTemplate> GetPCLExamResultTemplateList(long PCLExamGroupTemplateResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamGroupTemplateResultID));

                cn.Open();
                List<PCLExamResultTemplate> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPCLExamResultTemplateCollectionFromReader(reader);
                reader.Close();


                return retVal;
            }
        }

        public override List<PCLExamResultTemplate> GetPCLExamResultTemplateListByTypeID(long PCLExamGroupTemplateResultID, int ParamEnum)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateGetByTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamGroupTemplateResultID));
                cmd.AddParameter("@ParamEnum", SqlDbType.Int, ConvertNullObjectToDBNull(ParamEnum));

                cn.Open();
                List<PCLExamResultTemplate> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPCLExamResultTemplateCollectionFromReader(reader);
                reader.Close();


                return retVal;
            }
        }

        /*▼====: #003*/
        public override List<Resources> GetResourcesForMedicalServicesListByTypeID(long PCLResultParamImpID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcesForMedicalServices_LoadForPCL", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.VarChar, ConvertNullObjectToDBNull(PCLResultParamImpID));

                cn.Open();
                List<Resources> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetResourcesForMedicalServicesListByTypeIDCollectionFromReader(reader);
                reader.Close();
                return retVal;
            }
        }
        /*▲====: #003*/

        public override PCLExamResultTemplate GetPCLExamResultTemplate(long PCLExamResultTemplateID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateGetByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamResultTemplateID));

                cn.Open();
                PCLExamResultTemplate retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetPCLExamResultTemplateFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddPCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTemplateName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PCLExamTemplateName));
                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PCLExamGroupTemplateResultID));
                cmd.AddParameter("@ResultContent", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ResultContent));
                cmd.AddParameter("@Descriptions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Descriptions));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdatePCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamResultTemplateID));
                cmd.AddParameter("@PCLExamTemplateName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PCLExamTemplateName));
                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PCLExamGroupTemplateResultID));
                cmd.AddParameter("@ResultContent", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ResultContent));
                cmd.AddParameter("@Descriptions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Descriptions));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeletePCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamResultTemplateID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion






        #region UltraResParams_EchoCardiography
        public override UltraResParams_EchoCardiography GetUltraResParams_EchoCardiography(long UltraResParams_EchoCardiographyID, long PCLImgResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_EchoCardiographyID));
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLImgResultID));

                cn.Open();
                UltraResParams_EchoCardiography retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_EchoCardiographyFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override string GetUltraResParams_EchoCardiographyResult(long UltraResParams_EchoCardiographyID, long PCLImgResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyGetResult", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_EchoCardiographyID));
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLImgResultID));

                cn.Open();
                string resVal = "";

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    if (reader.HasColumn("Conclusion") && reader["Conclusion"] != DBNull.Value)
                    {
                        resVal = reader["Conclusion"].ToString();
                    }

                }
                reader.Close();

                return resVal;
            }
        }

        public override bool AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                //cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_EchoCardiographyID));
                cmd.AddParameter("@TM_Vlt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Vlt_Ttr));
                cmd.AddParameter("@TM_Dktt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Dktt_Ttr));
                cmd.AddParameter("@TM_Tstt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Tstt_Ttr));
                cmd.AddParameter("@TM_Vlt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Vlt_Tt));
                cmd.AddParameter("@TM_Dktt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Dktt_Tt));
                cmd.AddParameter("@TM_Tstt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Tstt_Tt));
                cmd.AddParameter("@TM_Pxcr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Pxcr));
                cmd.AddParameter("@TM_Pxtm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Pxtm));
                cmd.AddParameter("@TM_RV", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_RV));
                cmd.AddParameter("@TM_Ao", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Ao));
                cmd.AddParameter("@TM_La", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_La));
                cmd.AddParameter("@TM_Ssa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Ssa));
                cmd.AddParameter("@V_2D_Situs", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_2D_Situs));
                cmd.AddParameter("@TwoD_Veins", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Veins));
                cmd.AddParameter("@TwoD_Ivc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Ivc));
                cmd.AddParameter("@TwoD_Svc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Svc));
                cmd.AddParameter("@TwoD_Tvi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Tvi));
                cmd.AddParameter("@V_2D_Lsvc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_2D_Lsvc));
                cmd.AddParameter("@V_2D_Azygos", SqlDbType.SmallInt, ConvertNullObjectToDBNull(entity.V_2D_Azygos));
                cmd.AddParameter("@TwoD_Pv", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pv));
                cmd.AddParameter("@TwoD_Azygos", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Azygos));
                cmd.AddParameter("@TwoD_Atria", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Atria));
                cmd.AddParameter("@TwoD_Valves", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Valves));
                cmd.AddParameter("@TwoD_Cd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Cd));
                cmd.AddParameter("@TwoD_Ma", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ma));
                cmd.AddParameter("@TwoD_MitralArea", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_MitralArea));
                cmd.AddParameter("@TwoD_Ta", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ta));
                cmd.AddParameter("@TwoD_LSVC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_LSVC));
                cmd.AddParameter("@TwoD_Ventricles", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Ventricles));
                cmd.AddParameter("@TwoD_Aorte", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Aorte));
                cmd.AddParameter("@TwoD_Asc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Asc));
                cmd.AddParameter("@TwoD_Cr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Cr));
                cmd.AddParameter("@TwoD_Is", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Is));
                cmd.AddParameter("@TwoD_Abd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Abd));
                cmd.AddParameter("@TwoD_D2", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_D2));
                cmd.AddParameter("@TwoD_Ann", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ann));
                cmd.AddParameter("@TwoD_Tap", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Tap));
                cmd.AddParameter("@TwoD_Rpa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Rpa));
                cmd.AddParameter("@TwoD_Lpa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Lpa));
                cmd.AddParameter("@TwoD_Pericarde", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pericarde));
                cmd.AddParameter("@TwoD_Pa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pa));
                cmd.AddParameter("@TwoD_Others", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Others));
                cmd.AddParameter("@DOPPLER_Mitral_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_VelMax));
                cmd.AddParameter("@DOPPLER_Mitral_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_GdMax));
                cmd.AddParameter("@DOPPLER_Mitral_Ms", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Ms));
                cmd.AddParameter("@V_DOPPLER_Mitral_Mr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Mr));
                cmd.AddParameter("@V_DOPPLER_Mitral_Ea", SqlDbType.Int, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Ea));
                cmd.AddParameter("@DOPPLER_Mitral_Ea", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Ea));
                cmd.AddParameter("@DOPPLER_Mitral_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Moy));
                cmd.AddParameter("@DOPPLER_Mitral_Sm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Sm));
                //cmd.AddParameter("@V_DOPPLER_Mitral_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Grade));
                cmd.AddParameter("@V_DOPPLER_Mitral_Grade", SqlDbType.BigInt, entity.LDOPPLER_Mitral_Grade != null ? entity.LDOPPLER_Mitral_Grade.LookupID : 0);
                cmd.AddParameter("@DOPPLER_Aortic_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_VelMax));
                cmd.AddParameter("@DOPPLER_Aortic_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_GdMax));
                cmd.AddParameter("@DOPPLER_Aortic_As", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_As));
                cmd.AddParameter("@V_DOPPLER_Aortic_Ar", SqlDbType.Float, ConvertNullObjectToDBNull(entity.V_DOPPLER_Aortic_Ar));
                cmd.AddParameter("@DOPPLER_Aortic_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Moy));
                cmd.AddParameter("@DOPPLER_Aortic_SAo", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_SAo));
                //cmd.AddParameter("@V_DOPPLER_Aortic_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Aortic_Grade));
                cmd.AddParameter("@V_DOPPLER_Aortic_Grade", SqlDbType.BigInt, entity.LDOPPLER_Aortic_Grade != null ? entity.LDOPPLER_Aortic_Grade.LookupID : 0);
                cmd.AddParameter("@DOPPLER_Aortic_PHT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_PHT));
                cmd.AddParameter("@DOPPLER_Aortic_Dfo", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Dfo));
                cmd.AddParameter("@DOPPLER_Aortic_Edtd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Edtd));
                cmd.AddParameter("@DOPPLER_Aortic_ExtSpat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_ExtSpat));
                cmd.AddParameter("@DOPPLER_Tricuspid_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_VelMax));
                cmd.AddParameter("@V_DOPPLER_Tricuspid_Tr", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Tricuspid_Tr));
                cmd.AddParameter("@DOPPLER_Tricuspid_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_GdMax));
                cmd.AddParameter("@DOPPLER_Tricuspid_Paps", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_Paps));
                cmd.AddParameter("@DOPPLER_Tricuspid_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_Moy));
                cmd.AddParameter("@DOPPLER_Pulmonary_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_VelMax));
                cmd.AddParameter("@DOPPLER_Pulmonary_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_GdMax));
                cmd.AddParameter("@V_DOPPLER_Pulmonary_Pr", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Pulmonary_Pr));
                cmd.AddParameter("@DOPPLER_Pulmonary_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Moy));
                cmd.AddParameter("@DOPPLER_Pulmonary_Papm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Papm));
                cmd.AddParameter("@DOPPLER_Pulmonary_Papd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Papd));
                cmd.AddParameter("@DOPPLER_Pulmonary_Orthers", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Orthers));

                //cmd.AddParameter("@V_DOPPLER_Tricuspid_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Tricuspid_Grade));
                cmd.AddParameter("@V_DOPPLER_Tricuspid_Grade", SqlDbType.BigInt, entity.LDOPPLER_Tricuspid_Grade != null ? entity.LDOPPLER_Tricuspid_Grade.LookupID : 0);

                //cmd.AddParameter("@V_DOPPLER_Pulmonary_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Pulmonary_Grade));
                cmd.AddParameter("@V_DOPPLER_Pulmonary_Grade", SqlDbType.BigInt, entity.LDOPPLER_Pulmonary_Grade != null ? entity.LDOPPLER_Pulmonary_Grade.LookupID : 0);

                cmd.AddParameter("@Conclusion", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Conclusion));
                cmd.AddParameter("@TabIndex", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TabIndex));
                cmd.AddParameter("@Tab1_TM_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab1_TM_Update_Required));
                cmd.AddParameter("@Tab2_2D_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab2_2D_Update_Required));
                cmd.AddParameter("@Tab3_Doppler_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab3_Doppler_Update_Required));
                cmd.AddParameter("@Tab4_Conclusion_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab4_Conclusion_Update_Required));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_EchoCardiographyID));
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@TM_Vlt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Vlt_Ttr));
                cmd.AddParameter("@TM_Dktt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Dktt_Ttr));
                cmd.AddParameter("@TM_Tstt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Tstt_Ttr));
                cmd.AddParameter("@TM_Vlt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Vlt_Tt));
                cmd.AddParameter("@TM_Dktt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Dktt_Tt));
                cmd.AddParameter("@TM_Tstt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Tstt_Tt));
                cmd.AddParameter("@TM_Pxcr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Pxcr));
                cmd.AddParameter("@TM_Pxtm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Pxtm));
                cmd.AddParameter("@TM_RV", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_RV));
                cmd.AddParameter("@TM_Ao", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Ao));
                cmd.AddParameter("@TM_La", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_La));
                cmd.AddParameter("@TM_Ssa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Ssa));
                cmd.AddParameter("@V_2D_Situs", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_2D_Situs));
                cmd.AddParameter("@TwoD_Veins", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Veins));
                cmd.AddParameter("@TwoD_Ivc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Ivc));
                cmd.AddParameter("@TwoD_Svc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Svc));
                cmd.AddParameter("@TwoD_Tvi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Tvi));
                cmd.AddParameter("@V_2D_Lsvc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_2D_Lsvc));
                cmd.AddParameter("@V_2D_Azygos", SqlDbType.SmallInt, ConvertNullObjectToDBNull(entity.V_2D_Azygos));
                cmd.AddParameter("@TwoD_Pv", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pv));
                cmd.AddParameter("@TwoD_Azygos", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Azygos));
                cmd.AddParameter("@TwoD_Atria", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Atria));
                cmd.AddParameter("@TwoD_Valves", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Valves));
                cmd.AddParameter("@TwoD_Cd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Cd));
                cmd.AddParameter("@TwoD_Ma", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ma));
                cmd.AddParameter("@TwoD_MitralArea", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_MitralArea));
                cmd.AddParameter("@TwoD_Ta", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ta));
                cmd.AddParameter("@TwoD_LSVC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_LSVC));
                cmd.AddParameter("@TwoD_Ventricles", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Ventricles));
                cmd.AddParameter("@TwoD_Aorte", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Aorte));
                cmd.AddParameter("@TwoD_Asc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Asc));
                cmd.AddParameter("@TwoD_Cr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Cr));
                cmd.AddParameter("@TwoD_Is", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Is));
                cmd.AddParameter("@TwoD_Abd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Abd));
                cmd.AddParameter("@TwoD_D2", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_D2));
                cmd.AddParameter("@TwoD_Ann", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ann));
                cmd.AddParameter("@TwoD_Tap", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Tap));
                cmd.AddParameter("@TwoD_Rpa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Rpa));
                cmd.AddParameter("@TwoD_Lpa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Lpa));
                cmd.AddParameter("@TwoD_Pericarde", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pericarde));
                cmd.AddParameter("@TwoD_Pa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pa));
                cmd.AddParameter("@TwoD_Others", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Others));
                cmd.AddParameter("@DOPPLER_Mitral_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_VelMax));
                cmd.AddParameter("@DOPPLER_Mitral_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_GdMax));
                cmd.AddParameter("@DOPPLER_Mitral_Ms", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Ms));
                cmd.AddParameter("@V_DOPPLER_Mitral_Mr", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Mr));
                cmd.AddParameter("@V_DOPPLER_Mitral_Ea", SqlDbType.Int, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Ea));
                cmd.AddParameter("@DOPPLER_Mitral_Ea", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Ea));
                cmd.AddParameter("@DOPPLER_Mitral_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Moy));
                cmd.AddParameter("@DOPPLER_Mitral_Sm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Sm));
                //cmd.AddParameter("@V_DOPPLER_Mitral_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Grade));
                cmd.AddParameter("@V_DOPPLER_Mitral_Grade", SqlDbType.BigInt, entity.LDOPPLER_Mitral_Grade != null ? entity.LDOPPLER_Mitral_Grade.LookupID : 0);
                cmd.AddParameter("@DOPPLER_Aortic_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_VelMax));
                cmd.AddParameter("@DOPPLER_Aortic_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_GdMax));
                cmd.AddParameter("@DOPPLER_Aortic_As", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_As));
                cmd.AddParameter("@V_DOPPLER_Aortic_Ar", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.V_DOPPLER_Aortic_Ar));
                cmd.AddParameter("@DOPPLER_Aortic_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Moy));
                cmd.AddParameter("@DOPPLER_Aortic_SAo", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_SAo));
                //cmd.AddParameter("@V_DOPPLER_Aortic_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Aortic_Grade));
                cmd.AddParameter("@V_DOPPLER_Aortic_Grade", SqlDbType.BigInt, entity.LDOPPLER_Aortic_Grade != null ? entity.LDOPPLER_Aortic_Grade.LookupID : 0);
                cmd.AddParameter("@DOPPLER_Aortic_PHT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_PHT));
                cmd.AddParameter("@DOPPLER_Aortic_Dfo", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Dfo));
                cmd.AddParameter("@DOPPLER_Aortic_Edtd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Edtd));
                cmd.AddParameter("@DOPPLER_Aortic_ExtSpat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_ExtSpat));
                cmd.AddParameter("@DOPPLER_Tricuspid_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_VelMax));
                cmd.AddParameter("@V_DOPPLER_Tricuspid_Tr", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.V_DOPPLER_Tricuspid_Tr));
                cmd.AddParameter("@DOPPLER_Tricuspid_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_GdMax));
                cmd.AddParameter("@DOPPLER_Tricuspid_Paps", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_Paps));
                cmd.AddParameter("@DOPPLER_Tricuspid_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_Moy));
                cmd.AddParameter("@DOPPLER_Pulmonary_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_VelMax));
                cmd.AddParameter("@DOPPLER_Pulmonary_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_GdMax));
                cmd.AddParameter("@V_DOPPLER_Pulmonary_Pr", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.V_DOPPLER_Pulmonary_Pr));
                cmd.AddParameter("@DOPPLER_Pulmonary_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Moy));
                cmd.AddParameter("@DOPPLER_Pulmonary_Papm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Papm));
                cmd.AddParameter("@DOPPLER_Pulmonary_Papd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Papd));
                cmd.AddParameter("@DOPPLER_Pulmonary_Orthers", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Orthers));

                //cmd.AddParameter("@V_DOPPLER_Tricuspid_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Tricuspid_Grade));
                cmd.AddParameter("@V_DOPPLER_Tricuspid_Grade", SqlDbType.BigInt, entity.LDOPPLER_Tricuspid_Grade != null ? entity.LDOPPLER_Tricuspid_Grade.LookupID : 0);
                //cmd.AddParameter("@V_DOPPLER_Pulmonary_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Pulmonary_Grade));
                cmd.AddParameter("@V_DOPPLER_Pulmonary_Grade", SqlDbType.BigInt, entity.LDOPPLER_Pulmonary_Grade != null ? entity.LDOPPLER_Pulmonary_Grade.LookupID : 0);

                cmd.AddParameter("@Conclusion", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Conclusion));
                cmd.AddParameter("@TabIndex", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TabIndex));

                cmd.AddParameter("@Tab1_TM_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab1_TM_Update_Required));
                cmd.AddParameter("@Tab2_2D_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab2_2D_Update_Required));
                cmd.AddParameter("@Tab3_Doppler_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab3_Doppler_Update_Required));
                cmd.AddParameter("@Tab4_Conclusion_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab4_Conclusion_Update_Required));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else
                    return false;
            }
        }

        public override bool DeleteUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_EchoCardiographyID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        //20161214 CMN Begin: Add general inf
        #region UltraResParams_FetalEchocardiography
        public override UltraResParams_FetalEchocardiography GetUltraResParams_FetalEchocardiography(long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cn.Open();
                UltraResParams_FetalEchocardiography retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiographyFromReader(reader);
                }
                reader.Close();
                return retVal;
            }
        }
        #endregion
        //20161214 CMN End.
        #region UltraResParams_FetalEchocardiography2D
        //public override IList<UltraResParams_FetalEchocardiography2D> GetUltraResParams_FetalEchocardiography2D(long UltraResParams_FetalEchocardiography2DID
        //                                                        ,long PCLImgResultID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiography2DGetAll", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@UltraResParams_FetalEchocardiography2DID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiography2DID));
        //        cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLImgResultID));

        //        cn.Open();
        //        List<UltraResParams_FetalEchocardiography2D> retVal = null;

        //        IDataReader reader = ExecuteReader(cmd);

        //        retVal = GetUltraResParams_FetalEchocardiography2DCollectionFromReader(reader);
        //        reader.Close();


        //        return retVal;
        //    }
        //}

        public override UltraResParams_FetalEchocardiography2D GetUltraResParams_FetalEchocardiography2D(long UltraResParams_FetalEchocardiography2DID
                                                                , long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiography2DGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiography2DID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiography2DID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                UltraResParams_FetalEchocardiography2D retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiography2DFromReader(reader);
                }
                reader.Close();


                return retVal;
            }
        }

        public override bool AddUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiography2DInsert", cn);

                cmd.AddParameter("@NTSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NTSize));

                cmd.AddParameter("@NPSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NPSize));

                cmd.AddParameter("@VanVieussensLeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.VanVieussensLeftAtrium));

                cmd.AddParameter("@AtrialSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AtrialSeptalDefect));

                cmd.AddParameter("@MitralValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValveSize));

                cmd.AddParameter("@TriscupidValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValveSize));

                cmd.AddParameter("@DifferenceMitralTricuspid", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DifferenceMitralTricuspid));

                cmd.AddParameter("@TPTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TPTTr));

                cmd.AddParameter("@VLTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.VLTTTr));

                cmd.AddParameter("@TTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TTTTr));

                cmd.AddParameter("@DKTTTTr_VGd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTTTTr_VGd));

                cmd.AddParameter("@DKTTTT_VGs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTTTT_VGs));

                cmd.AddParameter("@DKTPTTr_VDd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTPTTr_VDd));

                cmd.AddParameter("@DKTPTT_VDs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTPTT_VDs));

                cmd.AddParameter("@Systolic", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Systolic));

                cmd.AddParameter("@VentricularSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.VentricularSeptalDefect));

                cmd.AddParameter("@AortaCompatible", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AortaCompatible));

                cmd.AddParameter("@AortaSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AortaSize));

                cmd.AddParameter("@PulmonaryArterySize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryArterySize));

                cmd.AddParameter("@AorticArch", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AorticArch));

                cmd.AddParameter("@AorticCoarctation", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticCoarctation));

                cmd.AddParameter("@HeartRateNomal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.HeartRateNomal));

                cmd.AddParameter("@RequencyHeartRateNomal", SqlDbType.Float, ConvertNullObjectToDBNull(entity.RequencyHeartRateNomal));

                cmd.AddParameter("@PericardialEffusion", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PericardialEffusion));

                cmd.AddParameter("@FetalCardialAxis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FetalCardialAxis));

                cmd.AddParameter("@CardialRateS", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CardialRateS));

                cmd.AddParameter("@LN", SqlDbType.Float, ConvertNullObjectToDBNull(entity.LN));

                cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrderRecord));

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public override bool UpdateUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiography2DUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiography2DID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiography2DID));

                cmd.AddParameter("@NTSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NTSize));

                cmd.AddParameter("@NPSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NPSize));

                cmd.AddParameter("@VanVieussensLeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.VanVieussensLeftAtrium));

                cmd.AddParameter("@AtrialSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AtrialSeptalDefect));

                cmd.AddParameter("@MitralValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValveSize));

                cmd.AddParameter("@TriscupidValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValveSize));

                cmd.AddParameter("@DifferenceMitralTricuspid", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DifferenceMitralTricuspid));

                cmd.AddParameter("@TPTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TPTTr));

                cmd.AddParameter("@VLTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.VLTTTr));

                cmd.AddParameter("@TTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TTTTr));

                cmd.AddParameter("@DKTTTTr_VGd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTTTTr_VGd));

                cmd.AddParameter("@DKTTTT_VGs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTTTT_VGs));

                cmd.AddParameter("@DKTPTTr_VDd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTPTTr_VDd));

                cmd.AddParameter("@DKTPTT_VDs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTPTT_VDs));

                cmd.AddParameter("@Systolic", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Systolic));

                cmd.AddParameter("@VentricularSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.VentricularSeptalDefect));

                cmd.AddParameter("@AortaCompatible", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AortaCompatible));

                cmd.AddParameter("@AortaSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AortaSize));

                cmd.AddParameter("@PulmonaryArterySize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryArterySize));

                cmd.AddParameter("@AorticArch", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AorticArch));

                cmd.AddParameter("@AorticCoarctation", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticCoarctation));

                cmd.AddParameter("@HeartRateNomal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.HeartRateNomal));

                cmd.AddParameter("@RequencyHeartRateNomal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RequencyHeartRateNomal));

                cmd.AddParameter("@PericardialEffusion", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PericardialEffusion));

                cmd.AddParameter("@FetalCardialAxis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FetalCardialAxis));

                cmd.AddParameter("@CardialRateS", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CardialRateS));

                cmd.AddParameter("@LN", SqlDbType.Float, ConvertNullObjectToDBNull(entity.LN));

                cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrderRecord));

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public override bool DeleteUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiography2DDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@UltraResParams_FetalEchocardiography2DID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiography2DID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region UltraResParams_FetalEchocardiography Doppler
        public override IList<UltraResParams_FetalEchocardiographyDoppler> GetUltraResParams_FetalEchocardiographyDoppler(long UltraResParams_FetalEchocardiographyDopplerID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyDopplerID));


                cn.Open();
                List<UltraResParams_FetalEchocardiographyDoppler> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUltraResParams_FetalEchocardiographyDopplerCollectionFromReader(reader);
                reader.Close();


                return retVal;
            }
        }
        public override UltraResParams_FetalEchocardiographyDoppler GetUltraResParams_FetalEchocardiographyDopplerByID(
                                    long UltraResParams_FetalEchocardiographyDopplerID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyDopplerID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));


                cn.Open();
                UltraResParams_FetalEchocardiographyDoppler retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiographyDopplerFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@MitralValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValve_Vmax));


                cmd.AddParameter("@MitralValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValve_Gdmax));

                cmd.AddParameter("@MitralValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MitralValve_Open));

                cmd.AddParameter("@MitralValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.MitralValve_Stenosis));

                cmd.AddParameter("@TriscupidValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValve_Vmax));

                cmd.AddParameter("@TriscupidValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValve_Gdmax));

                cmd.AddParameter("@TriscupidValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TriscupidValve_Open));

                cmd.AddParameter("@TriscupidValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TriscupidValve_Stenosis));

                cmd.AddParameter("@AorticValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Vmax));

                cmd.AddParameter("@AorticValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Gdmax));

                cmd.AddParameter("@AorticValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.AorticValve_Open));

                cmd.AddParameter("@AorticValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Stenosis));

                cmd.AddParameter("@PulmonaryValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Vmax));

                cmd.AddParameter("@PulmonaryValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Gdmax));

                cmd.AddParameter("@PulmonaryValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PulmonaryValve_Open));

                cmd.AddParameter("@PulmonaryValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Stenosis));

                cmd.AddParameter("@AorticCoarctationBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticCoarctationBloodTraffic));

                cmd.AddParameter("@VanViewessensBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.VanViewessensBloodTraffic));

                cmd.AddParameter("@DuctusAteriosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DuctusAteriosusBloodTraffic));

                cmd.AddParameter("@DuctusVenosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DuctusVenosusBloodTraffic));

                cmd.AddParameter("@PulmonaryVeins_LeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PulmonaryVeins_LeftAtrium));

                cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrderRecord));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public override bool UpdateUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyDopplerID));

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@MitralValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValve_Vmax));


                cmd.AddParameter("@MitralValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValve_Gdmax));

                cmd.AddParameter("@MitralValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MitralValve_Open));

                cmd.AddParameter("@MitralValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.MitralValve_Stenosis));

                cmd.AddParameter("@TriscupidValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValve_Vmax));

                cmd.AddParameter("@TriscupidValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValve_Gdmax));

                cmd.AddParameter("@TriscupidValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TriscupidValve_Open));

                cmd.AddParameter("@TriscupidValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TriscupidValve_Stenosis));

                cmd.AddParameter("@AorticValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Vmax));

                cmd.AddParameter("@AorticValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Gdmax));

                cmd.AddParameter("@AorticValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.AorticValve_Open));

                cmd.AddParameter("@AorticValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Stenosis));

                cmd.AddParameter("@PulmonaryValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Vmax));

                cmd.AddParameter("@PulmonaryValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Gdmax));

                cmd.AddParameter("@PulmonaryValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PulmonaryValve_Open));

                cmd.AddParameter("@PulmonaryValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Stenosis));

                cmd.AddParameter("@AorticCoarctationBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticCoarctationBloodTraffic));

                cmd.AddParameter("@VanViewessensBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.VanViewessensBloodTraffic));

                cmd.AddParameter("@DuctusAteriosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DuctusAteriosusBloodTraffic));

                cmd.AddParameter("@DuctusVenosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DuctusVenosusBloodTraffic));

                cmd.AddParameter("@PulmonaryVeins_LeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PulmonaryVeins_LeftAtrium));

                cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrderRecord));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public override bool DeleteUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyDopplerID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region UltraResParams_FetalEchocardiographyPostpartum
        public override IList<UltraResParams_FetalEchocardiographyPostpartum> GetUltraResParams_FetalEchocardiographyPostpartum(long UltraResParams_FetalEchocardiographyPostpartumID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyPostpartumID));


                cn.Open();
                List<UltraResParams_FetalEchocardiographyPostpartum> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUltraResParams_FetalEchocardiographyPostpartumCollectionFromReader(reader);
                reader.Close();


                return retVal;
            }
        }

        public override UltraResParams_FetalEchocardiographyPostpartum GetUltraResParams_FetalEchocardiographyPostpartumByID(
            long UltraResParams_FetalEchocardiographyPostpartumID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyPostpartumID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                UltraResParams_FetalEchocardiographyPostpartum retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiographyPostpartumFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@BabyBirthday", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.BabyBirthday));

                cmd.AddParameter("@BabyWeight", SqlDbType.Float, ConvertNullObjectToDBNull(entity.BabyWeight));

                cmd.AddParameter("@BabySex", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BabySex));

                cmd.AddParameter("@URP_Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.URP_Date));

                cmd.AddParameter("@PFO", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PFO));

                cmd.AddParameter("@PCA", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PCA));

                cmd.AddParameter("@AnotherDiagnosic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AnotherDiagnosic));

                cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Notes));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));



                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public override bool UpdateUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyPostpartumID));

                cmd.AddParameter("@BabyBirthday", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.BabyBirthday));

                cmd.AddParameter("@BabyWeight", SqlDbType.Float, ConvertNullObjectToDBNull(entity.BabyWeight));

                cmd.AddParameter("@BabySex", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BabySex));

                cmd.AddParameter("@URP_Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.URP_Date));

                cmd.AddParameter("@PFO", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PFO));

                cmd.AddParameter("@PCA", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PCA));

                cmd.AddParameter("@AnotherDiagnosic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AnotherDiagnosic));

                cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Notes));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public override bool DeleteUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyPostpartumID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region UltraResParams_FetalEchocardiographyResult
        public override IList<UltraResParams_FetalEchocardiographyResult> GetUltraResParams_FetalEchocardiographyResult(long UltraResParams_FetalEchocardiographyResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyResultID));


                cn.Open();
                List<UltraResParams_FetalEchocardiographyResult> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUltraResParams_FetalEchocardiographyResultCollectionFromReader(reader);
                reader.Close();


                return retVal;
            }
        }

        public override UltraResParams_FetalEchocardiographyResult GetUltraResParams_FetalEchocardiographyResultByID(
                                    long UltraResParams_FetalEchocardiographyResultID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyResultID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                UltraResParams_FetalEchocardiographyResult retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiographyResultFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@CardialAbnormal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CardialAbnormal));

                cmd.AddParameter("@CardialAbnormalDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.CardialAbnormalDetail));

                cmd.AddParameter("@Susgest", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Susgest));

                cmd.AddParameter("@UltraResParamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.UltraResParamDate));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public override bool UpdateUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyResultID));

                cmd.AddParameter("@CardialAbnormal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CardialAbnormal));

                cmd.AddParameter("@CardialAbnormalDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.CardialAbnormalDetail));

                cmd.AddParameter("@Susgest", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Susgest));

                cmd.AddParameter("@UltraResParamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.UltraResParamDate));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public override bool DeleteUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyResultID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }


        #endregion

        #region Abdominal Ultrasound

        public override bool InsertAbdominalUltrasoundResult(AbdominalUltrasound entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spInsertAbdominalUltrasoundResult", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientPCLReqID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                    cmd.AddParameter("@Liver", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Liver));
                    cmd.AddParameter("@Gallbladder", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Gallbladder));
                    cmd.AddParameter("@Pancreas", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Pancreas));
                    cmd.AddParameter("@Spleen", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Spleen));
                    cmd.AddParameter("@RightKidney", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RightKidney));
                    cmd.AddParameter("@LeftKidney", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeftKidney));
                    cmd.AddParameter("@Bladder", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Bladder));
                    cmd.AddParameter("@Prostate", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Prostate));
                    cmd.AddParameter("@Uterus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Uterus));
                    cmd.AddParameter("@RightOvary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RightOvary));
                    cmd.AddParameter("@LeftOvary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeftOvary));
                    cmd.AddParameter("@PeritonealFluid", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PeritonealFluid));
                    cmd.AddParameter("@PleuralFluid", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PleuralFluid));
                    cmd.AddParameter("@AbdominalAortic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AbdominalAortic));
                    cmd.AddParameter("@Conclusion", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Conclusion));

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);

                    if (retVal > 0)
                    {
                        return true;
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override bool UpdateAbdominalUltrasoundResult(AbdominalUltrasound entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateAbdominalUltrasoundResult", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@AbdominalUltrasoundID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.AbdominalUltrasoundID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                    cmd.AddParameter("@Liver", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Liver));
                    cmd.AddParameter("@Gallbladder", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Gallbladder));
                    cmd.AddParameter("@Pancreas", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Pancreas));
                    cmd.AddParameter("@Spleen", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Spleen));
                    cmd.AddParameter("@RightKidney", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RightKidney));
                    cmd.AddParameter("@LeftKidney", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeftKidney));
                    cmd.AddParameter("@Bladder", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Bladder));
                    cmd.AddParameter("@Prostate", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Prostate));
                    cmd.AddParameter("@Uterus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Uterus));
                    cmd.AddParameter("@RightOvary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RightOvary));
                    cmd.AddParameter("@LeftOvary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeftOvary));
                    cmd.AddParameter("@PeritonealFluid", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PeritonealFluid));
                    cmd.AddParameter("@PleuralFluid", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PleuralFluid));
                    cmd.AddParameter("@AbdominalAortic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AbdominalAortic));
                    cmd.AddParameter("@Conclusion", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Conclusion));

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);

                    if (retVal > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override AbdominalUltrasound GetAbdominalUltrasoundResult(long PatientPCLReqID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetAbdominalUltrasoundResult", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                    cn.Open();
                    AbdominalUltrasound retVal = null;

                    IDataReader reader = ExecuteReader(cmd);
                    while (reader.Read())
                    {
                        retVal = GetAbdominalUltrasoundResultFromReader(reader);
                    }

                    reader.Close();

                    return retVal;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion


        #region URP_FE_Exam

        public override URP_FE_Exam GetURP_FE_Exam(long URP_FE_ExamID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_ExamGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_ExamID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_Exam retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_ExamFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_Exam(URP_FE_Exam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_ExamInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));



                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CaoHuyetAp", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CaoHuyetAp));
                cmd.AddParameter("@CaoHuyetApDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.CaoHuyetApDetail));
                cmd.AddParameter("@Cholesterol", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Cholesterol));
                cmd.AddParameter("@Triglyceride", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Triglyceride));
                cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HDL));
                cmd.AddParameter("@LDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.LDL));
                cmd.AddParameter("@TieuDuong", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TieuDuong));
                cmd.AddParameter("@TieuDuongDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TieuDuongDetail));
                cmd.AddParameter("@ThuocLa", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThuocLa));
                cmd.AddParameter("@Detail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Detail));
                cmd.AddParameter("@ThuocNguaThai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThuocNguaThai));
                cmd.AddParameter("@ThuocNguaThaiDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThuocNguaThaiDetail));

                cmd.AddParameter("NhanApMP", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.NhanApMP));
                cmd.AddParameter("NhanApMT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.NhanApMT));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_Exam(URP_FE_Exam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_ExamUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_ExamID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CaoHuyetAp", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CaoHuyetAp));
                cmd.AddParameter("@CaoHuyetApDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.CaoHuyetApDetail));
                cmd.AddParameter("@Cholesterol", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Cholesterol));
                cmd.AddParameter("@Triglyceride", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Triglyceride));
                cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HDL));
                cmd.AddParameter("@LDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.LDL));
                cmd.AddParameter("@TieuDuong", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TieuDuong));
                cmd.AddParameter("@TieuDuongDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TieuDuongDetail));
                cmd.AddParameter("@ThuocLa", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThuocLa));
                cmd.AddParameter("@Detail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Detail));
                cmd.AddParameter("@ThuocNguaThai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThuocNguaThai));
                cmd.AddParameter("@ThuocNguaThaiDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThuocNguaThaiDetail));

                cmd.AddParameter("NhanApMP", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.NhanApMP));
                cmd.AddParameter("NhanApMT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.NhanApMT));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_Exam(URP_FE_Exam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_ExamDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_ExamID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_Oesophagienne
        public override URP_FE_Oesophagienne GetURP_FE_Oesophagienne(long URP_FE_OesophagienneID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_OesophagienneID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_Oesophagienne retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_OesophagienneFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@ChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinh));
                //cmd.AddParameter("@ChanDoanThanhNguc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChanDoanThanhNguc));
                //cmd.AddParameter("@V_ChanDoanThanhNgucID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ChanDoanThanhNgucID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_OesophagienneID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneID));
                cmd.AddParameter("@ChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinh));
                //cmd.AddParameter("@ChanDoanThanhNguc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChanDoanThanhNguc));
                //cmd.AddParameter("@V_ChanDoanThanhNgucID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ChanDoanThanhNgucID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_OesophagienneCheck
        public override URP_FE_OesophagienneCheck GetURP_FE_OesophagienneCheck(long URP_FE_OesophagienneCheckID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneCheckGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneCheckID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_OesophagienneCheckID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_OesophagienneCheck retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_OesophagienneCheckFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneCheckInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CatNghia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CatNghia));
                cmd.AddParameter("@NuotNghen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NuotNghen));
                cmd.AddParameter("@NuotDau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NuotDau));
                cmd.AddParameter("@OiMau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.OiMau));
                cmd.AddParameter("@XaTriTrungThat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.XaTriTrungThat));
                cmd.AddParameter("@CotSongCo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CotSongCo));
                cmd.AddParameter("@ChanThuongLongNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChanThuongLongNguc));
                cmd.AddParameter("@LanKhamNoiSoiGanDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LanKhamNoiSoiGanDay));
                cmd.AddParameter("@DiUngThuoc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DiUngThuoc));
                cmd.AddParameter("@NghienRuou", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NghienRuou));
                cmd.AddParameter("@BiTieu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BiTieu));
                cmd.AddParameter("@TangNhanApGocHep", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TangNhanApGocHep));
                cmd.AddParameter("@Suyen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Suyen));
                cmd.AddParameter("@LanAnSauCung", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LanAnSauCung));
                cmd.AddParameter("@RangGiaHamGia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RangGiaHamGia));
                cmd.AddParameter("@HuyetApTT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApTT));
                cmd.AddParameter("@HuyetApTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApTTr));
                cmd.AddParameter("@Mach", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Mach));
                cmd.AddParameter("@DoBaoHoaOxy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DoBaoHoaOxy));
                cmd.AddParameter("@ThucHienDuongTruyenTinhMach", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThucHienDuongTruyenTinhMach));
                cmd.AddParameter("@KiemTraDauDoSieuAm", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.KiemTraDauDoSieuAm));
                cmd.AddParameter("@ChinhDauDoTrungTinh", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChinhDauDoTrungTinh));
                cmd.AddParameter("@TeMeBenhNhan", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TeMeBenhNhan));
                cmd.AddParameter("@DatBenhNhanNghiengTrai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DatBenhNhanNghiengTrai));
                cmd.AddParameter("@CotDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CotDay));
                cmd.AddParameter("@BenhNhanThoaiMai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BenhNhanThoaiMai));
                cmd.AddParameter("@BoiTronDauDo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BoiTronDauDo));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneCheckUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_OesophagienneCheckID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneCheckID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CatNghia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CatNghia));
                cmd.AddParameter("@NuotNghen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NuotNghen));
                cmd.AddParameter("@NuotDau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NuotDau));
                cmd.AddParameter("@OiMau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.OiMau));
                cmd.AddParameter("@XaTriTrungThat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.XaTriTrungThat));
                cmd.AddParameter("@CotSongCo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CotSongCo));
                cmd.AddParameter("@ChanThuongLongNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChanThuongLongNguc));
                cmd.AddParameter("@LanKhamNoiSoiGanDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LanKhamNoiSoiGanDay));
                cmd.AddParameter("@DiUngThuoc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DiUngThuoc));
                cmd.AddParameter("@NghienRuou", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NghienRuou));
                cmd.AddParameter("@BiTieu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BiTieu));
                cmd.AddParameter("@TangNhanApGocHep", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TangNhanApGocHep));
                cmd.AddParameter("@Suyen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Suyen));
                cmd.AddParameter("@LanAnSauCung", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LanAnSauCung));
                cmd.AddParameter("@RangGiaHamGia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RangGiaHamGia));
                cmd.AddParameter("@HuyetApTT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApTT));
                cmd.AddParameter("@HuyetApTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApTTr));
                cmd.AddParameter("@Mach", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Mach));
                cmd.AddParameter("@DoBaoHoaOxy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DoBaoHoaOxy));
                cmd.AddParameter("@ThucHienDuongTruyenTinhMach", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThucHienDuongTruyenTinhMach));
                cmd.AddParameter("@KiemTraDauDoSieuAm", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.KiemTraDauDoSieuAm));
                cmd.AddParameter("@ChinhDauDoTrungTinh", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChinhDauDoTrungTinh));
                cmd.AddParameter("@TeMeBenhNhan", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TeMeBenhNhan));
                cmd.AddParameter("@DatBenhNhanNghiengTrai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DatBenhNhanNghiengTrai));
                cmd.AddParameter("@CotDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CotDay));
                cmd.AddParameter("@BenhNhanThoaiMai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BenhNhanThoaiMai));
                cmd.AddParameter("@BoiTronDauDo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BoiTronDauDo));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneCheckDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneCheckID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneCheckID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_OesophagienneDiagnosis

        public override URP_FE_OesophagienneDiagnosis GetURP_FE_OesophagienneDiagnosis(long URP_FE_OesophagienneDiagnosisID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDiagnosisGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneDiagnosisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_OesophagienneDiagnosisID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_OesophagienneDiagnosis retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_OesophagienneDiagnosisFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDiagnosisInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@ChanDoanQuaThucQuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChanDoanQuaThucQuan));
                //cmd.AddParameter("@V_ChanDoanQuaThucQuanID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ChanDoanQuaThucQuanID));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDiagnosisUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_OesophagienneDiagnosisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneDiagnosisID));
                cmd.AddParameter("@ChanDoanQuaThucQuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChanDoanQuaThucQuan));
                //cmd.AddParameter("@V_ChanDoanQuaThucQuanID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ChanDoanQuaThucQuanID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDiagnosisDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneDiagnosisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneDiagnosisID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamole

        public override URP_FE_StressDipyridamole GetURP_FE_StressDipyridamole(long URP_FE_StressDipyridamoleID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamole retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTimCanDat));
                cmd.AddParameter("@TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_HuyetAp_TT));
                cmd.AddParameter("@TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_HuyetAp_TTr));
                cmd.AddParameter("@TNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_TanSoTim));
                cmd.AddParameter("@TNP_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TNP_TacDungPhu));
                cmd.AddParameter("@TruyenDipyridamole056_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipyridamole056_DungLuong));
                cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy056_P2_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_TanSoTim));
                cmd.AddParameter("@TruyenDipy056_P2_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_TacDungPhu));
                cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy056_P4_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_TanSoTim));
                cmd.AddParameter("@TruyenDipy056_P4_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_TacDungPhu));
                cmd.AddParameter("@SauLieuDauP6_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_HuyetAp_TT));
                cmd.AddParameter("@SauLieuDauP6_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_HuyetAp_TTr));
                cmd.AddParameter("@SauLieuDauP6_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_TanSoTim));
                cmd.AddParameter("@SauLieuDauP6_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SauLieuDauP6_TacDungPhu));
                cmd.AddParameter("@TruyenDipyridamole028_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipyridamole028_DungLuong));
                cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy028_P8_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_TanSoTim));
                cmd.AddParameter("@TruyenDipy028_P8_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_TacDungPhu));
                cmd.AddParameter("@SauLieu2P10_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_HuyetAp_TT));
                cmd.AddParameter("@SauLieu2P10_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_HuyetAp_TTr));
                cmd.AddParameter("@SauLieu2P10_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_TanSoTim));
                cmd.AddParameter("@SauLieu2P10_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SauLieu2P10_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP12_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP12_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP12_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_TanSoTim));
                cmd.AddParameter("@ThemAtropineP12_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP12_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP13_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP13_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP13_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_TanSoTim));
                cmd.AddParameter("@ThemAtropineP13_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP13_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP14_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP14_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP14_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_TanSoTim));
                cmd.AddParameter("@ThemAtropineP14_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP14_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP15_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP15_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP15_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_TanSoTim));
                cmd.AddParameter("@ThemAtropineP15_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP15_TacDungPhu));
                cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_HuyetAp_TT));
                cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_HuyetAp_TTr));
                cmd.AddParameter("@TheoDoiAtropineP16_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_TanSoTim));
                cmd.AddParameter("@TheoDoiAtropineP16_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_TacDungPhu));
                cmd.AddParameter("@ThemAminophyline_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_DungLuong));
                cmd.AddParameter("@ThemAminophyline_Phut", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_Phut));
                cmd.AddParameter("@ThemAminophyline_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_HuyetAp_TT));
                cmd.AddParameter("@ThemAminophyline_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_HuyetAp_TTr));
                cmd.AddParameter("@ThemAminophyline_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_TanSoTim));
                cmd.AddParameter("@ThemAminophyline_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAminophyline_TacDungPhu));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDipyridamoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTimCanDat));
                cmd.AddParameter("@TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_HuyetAp_TT));
                cmd.AddParameter("@TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_HuyetAp_TTr));
                cmd.AddParameter("@TNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_TanSoTim));
                cmd.AddParameter("@TNP_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TNP_TacDungPhu));
                cmd.AddParameter("@TruyenDipyridamole056_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipyridamole056_DungLuong));
                cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy056_P2_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_TanSoTim));
                cmd.AddParameter("@TruyenDipy056_P2_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_TacDungPhu));
                cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy056_P4_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_TanSoTim));
                cmd.AddParameter("@TruyenDipy056_P4_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_TacDungPhu));
                cmd.AddParameter("@SauLieuDauP6_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_HuyetAp_TT));
                cmd.AddParameter("@SauLieuDauP6_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_HuyetAp_TTr));
                cmd.AddParameter("@SauLieuDauP6_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_TanSoTim));
                cmd.AddParameter("@SauLieuDauP6_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SauLieuDauP6_TacDungPhu));
                cmd.AddParameter("@TruyenDipyridamole028_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipyridamole028_DungLuong));
                cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy028_P8_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_TanSoTim));
                cmd.AddParameter("@TruyenDipy028_P8_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_TacDungPhu));
                cmd.AddParameter("@SauLieu2P10_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_HuyetAp_TT));
                cmd.AddParameter("@SauLieu2P10_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_HuyetAp_TTr));
                cmd.AddParameter("@SauLieu2P10_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_TanSoTim));
                cmd.AddParameter("@SauLieu2P10_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SauLieu2P10_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP12_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP12_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP12_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_TanSoTim));
                cmd.AddParameter("@ThemAtropineP12_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP12_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP13_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP13_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP13_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_TanSoTim));
                cmd.AddParameter("@ThemAtropineP13_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP13_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP14_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP14_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP14_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_TanSoTim));
                cmd.AddParameter("@ThemAtropineP14_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP14_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP15_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP15_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP15_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_TanSoTim));
                cmd.AddParameter("@ThemAtropineP15_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP15_TacDungPhu));
                cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_HuyetAp_TT));
                cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_HuyetAp_TTr));
                cmd.AddParameter("@TheoDoiAtropineP16_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_TanSoTim));
                cmd.AddParameter("@TheoDoiAtropineP16_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_TacDungPhu));
                cmd.AddParameter("@ThemAminophyline_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_DungLuong));
                cmd.AddParameter("@ThemAminophyline_Phut", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_Phut));
                cmd.AddParameter("@ThemAminophyline_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_HuyetAp_TT));
                cmd.AddParameter("@ThemAminophyline_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_HuyetAp_TTr));
                cmd.AddParameter("@ThemAminophyline_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_TanSoTim));
                cmd.AddParameter("@ThemAminophyline_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAminophyline_TacDungPhu));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamoleElectrocardiogram

        public override URP_FE_StressDipyridamoleElectrocardiogram GetURP_FE_StressDipyridamoleElectrocardiogram(long URP_FE_StressDipyridamoleElectrocardiogramID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleElectrocardiogramGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleElectrocardiogramID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamoleElectrocardiogram retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleElectrocardiogramFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleElectrocardiogramInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriConDauThatNguc));
                cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriDIGITALIS));
                cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LyDoKhongThucHienDuoc));
                cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MucGangSuc));
                cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThoiGianGangSuc));
                cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTim));
                cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApToiDa));
                cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ConDauThatNguc));
                cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.STChenhXuong));
                cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhipTim));
                cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipTimChiTiet));
                cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.XetNghiemKhac));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleElectrocardiogramUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@URP_FE_StressDipyridamoleElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleElectrocardiogramID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriConDauThatNguc));
                cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriDIGITALIS));
                cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LyDoKhongThucHienDuoc));
                cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MucGangSuc));
                cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThoiGianGangSuc));
                cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTim));
                cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApToiDa));
                cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ConDauThatNguc));
                cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.STChenhXuong));
                cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhipTim));
                cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipTimChiTiet));
                cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.XetNghiemKhac));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleElectrocardiogramDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleElectrocardiogramID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamoleExam
        public override URP_FE_StressDipyridamoleExam GetURP_FE_StressDipyridamoleExam(long URP_FE_StressDipyridamoleExamID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleExamGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleExamID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamoleExam retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleExamFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleExamInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungHienTai));
                cmd.AddParameter("@ChiDinhSATGSDipy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChiDinhSATGSDipy));
                cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinhDetail));
                cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTruocNgayKham));
                cmd.AddParameter("@TDDTrongNgaySATGSDipy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTrongNgaySATGSDipy));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleExamUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDipyridamoleExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleExamID));

                cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungHienTai));
                cmd.AddParameter("@ChiDinhSATGSDipy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChiDinhSATGSDipy));
                cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinhDetail));
                cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTruocNgayKham));
                cmd.AddParameter("@TDDTrongNgaySATGSDipy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTrongNgaySATGSDipy));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleExamDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleExamID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamoleImage
        public override URP_FE_StressDipyridamoleImage GetURP_FE_StressDipyridamoleImage(long URP_FE_StressDipyridamoleImageID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleImageGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleImageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleImageID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamoleImage retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleImageFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleImageInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleImageUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDipyridamoleImageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleImageID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleImageDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleImageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleImageID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamoleResult
        public override URP_FE_StressDipyridamoleResult GetURP_FE_StressDipyridamoleResult(long URP_FE_StressDipyridamoleResultID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleResultID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamoleResult retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleResultFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleResultInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThayDoiDTD));
                cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThayDoiDTDChiTiet));
                cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhip));
                cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipChiTiet));
                cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TDPHayBienChung));
                cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungKhac));
                cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.BienPhapDieuTri));
                cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSieuAmTim));
                cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSieuAmTim));
                cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_TNP));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_TNP));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_TNP));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_KetLuan));
                cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_TNP));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_KetLuan));
                cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_TNP));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_KetLuan));
                cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_TNP));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_TNP));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_TNP));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_TNP));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_KetLuan));
                cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_TNP));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_KetLuan));
                cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_TNP));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_KetLuan));
                cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_TNP));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_KetLuan));
                cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_TNP));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_KetLuan));
                cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_TNP));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_KetLuan));
                cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_TNP));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_KetLuan));
                cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_TNP));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_KetLuan));
                cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_TNP));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_KetLuan));
                cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_TNP));
                cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleResultUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDipyridamoleResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThayDoiDTD));
                cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThayDoiDTDChiTiet));
                cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhip));
                cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipChiTiet));
                cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TDPHayBienChung));
                cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungKhac));
                cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.BienPhapDieuTri));
                cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSieuAmTim));
                cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSieuAmTim));
                cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_TNP));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_TNP));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_TNP));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_KetLuan));
                cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_TNP));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_KetLuan));
                cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_TNP));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_KetLuan));
                cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_TNP));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_TNP));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_TNP));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_TNP));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_KetLuan));
                cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_TNP));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_KetLuan));
                cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_TNP));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_KetLuan));
                cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_TNP));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_KetLuan));
                cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_TNP));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_KetLuan));
                cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_TNP));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_KetLuan));
                cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_TNP));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_KetLuan));
                cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_TNP));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_KetLuan));
                cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_TNP));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_KetLuan));
                cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_TNP));
                cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_KetLuan));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleResultID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDobutamine
        public override URP_FE_StressDobutamine GetURP_FE_StressDobutamine(long URP_FE_StressDobutamineID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamine retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TruyenTinhMach", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TruyenTinhMach));
                cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTimCanDat));
                cmd.AddParameter("@TD_TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TT));
                cmd.AddParameter("@TD_TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TTr));
                cmd.AddParameter("@TD_TNP_HuyetAp_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TanSoTim));
                cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_DoChenhApMin));
                cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_DoChenhApMax));
                cmd.AddParameter("@FiveMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DungLuong));
                cmd.AddParameter("@FiveMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_HuyetAp_TT));
                cmd.AddParameter("@FiveMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_HuyetAp_TTr));
                cmd.AddParameter("@FiveMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_TanSoTim));
                cmd.AddParameter("@FiveMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DoChenhApMin));
                cmd.AddParameter("@FiveMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DoChenhApMax));
                cmd.AddParameter("@TenMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DungLuong));
                cmd.AddParameter("@TenMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_HuyetAp_TT));
                cmd.AddParameter("@TenMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_HuyetAp_TTr));
                cmd.AddParameter("@TenMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_TanSoTim));
                cmd.AddParameter("@TenMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DoChenhApMin));
                cmd.AddParameter("@TenMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DoChenhApMax));
                cmd.AddParameter("@TwentyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DungLuong));
                cmd.AddParameter("@TwentyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_HuyetAp_TT));
                cmd.AddParameter("@TwentyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_HuyetAp_TTr));
                cmd.AddParameter("@TwentyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_TanSoTim));
                cmd.AddParameter("@TwentyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DoChenhApMin));
                cmd.AddParameter("@TwentyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DoChenhApMax));
                cmd.AddParameter("@ThirtyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DungLuong));
                cmd.AddParameter("@ThirtyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_HuyetAp_TT));
                cmd.AddParameter("@ThirtyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_HuyetAp_TTr));
                cmd.AddParameter("@ThirtyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_TanSoTim));
                cmd.AddParameter("@ThirtyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DoChenhApMin));
                cmd.AddParameter("@ThirtyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DoChenhApMax));
                cmd.AddParameter("@FortyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DungLuong));
                cmd.AddParameter("@FortyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_HuyetAp_TT));
                cmd.AddParameter("@FortyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_HuyetAp_TTr));
                cmd.AddParameter("@FortyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_TanSoTim));
                cmd.AddParameter("@FortyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DoChenhApMin));
                cmd.AddParameter("@FortyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DoChenhApMax));
                cmd.AddParameter("@Atropine_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DungLuong));
                cmd.AddParameter("@Atropine_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_HuyetAp_TT));
                cmd.AddParameter("@Atropine_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_HuyetAp_TTr));
                cmd.AddParameter("@Atropine_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_TanSoTim));
                cmd.AddParameter("@Atropine_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DoChenhApMin));
                cmd.AddParameter("@Atropine_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DoChenhApMax));
                cmd.AddParameter("@NgungNP_ThoiGian", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_ThoiGian));
                cmd.AddParameter("@NgungNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_HuyetAp_TT));
                cmd.AddParameter("@NgungNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_HuyetAp_TTr));
                cmd.AddParameter("@NgungNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_TanSoTim));
                cmd.AddParameter("@NgungNP_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_DoChenhApMin));
                cmd.AddParameter("@NgungNP_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_DoChenhApMax));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_StressDobutamineID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TruyenTinhMach", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TruyenTinhMach));
                cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTimCanDat));
                cmd.AddParameter("@TD_TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TT));
                cmd.AddParameter("@TD_TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TTr));
                cmd.AddParameter("@TD_TNP_HuyetAp_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TanSoTim));
                cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_DoChenhApMin));
                cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_DoChenhApMax));
                cmd.AddParameter("@FiveMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DungLuong));
                cmd.AddParameter("@FiveMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_HuyetAp_TT));
                cmd.AddParameter("@FiveMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_HuyetAp_TTr));
                cmd.AddParameter("@FiveMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_TanSoTim));
                cmd.AddParameter("@FiveMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DoChenhApMin));
                cmd.AddParameter("@FiveMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DoChenhApMax));
                cmd.AddParameter("@TenMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DungLuong));
                cmd.AddParameter("@TenMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_HuyetAp_TT));
                cmd.AddParameter("@TenMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_HuyetAp_TTr));
                cmd.AddParameter("@TenMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_TanSoTim));
                cmd.AddParameter("@TenMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DoChenhApMin));
                cmd.AddParameter("@TenMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DoChenhApMax));
                cmd.AddParameter("@TwentyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DungLuong));
                cmd.AddParameter("@TwentyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_HuyetAp_TT));
                cmd.AddParameter("@TwentyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_HuyetAp_TTr));
                cmd.AddParameter("@TwentyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_TanSoTim));
                cmd.AddParameter("@TwentyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DoChenhApMin));
                cmd.AddParameter("@TwentyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DoChenhApMax));
                cmd.AddParameter("@ThirtyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DungLuong));
                cmd.AddParameter("@ThirtyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_HuyetAp_TT));
                cmd.AddParameter("@ThirtyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_HuyetAp_TTr));
                cmd.AddParameter("@ThirtyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_TanSoTim));
                cmd.AddParameter("@ThirtyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DoChenhApMin));
                cmd.AddParameter("@ThirtyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DoChenhApMax));
                cmd.AddParameter("@FortyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DungLuong));
                cmd.AddParameter("@FortyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_HuyetAp_TT));
                cmd.AddParameter("@FortyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_HuyetAp_TTr));
                cmd.AddParameter("@FortyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_TanSoTim));
                cmd.AddParameter("@FortyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DoChenhApMin));
                cmd.AddParameter("@FortyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DoChenhApMax));
                cmd.AddParameter("@Atropine_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DungLuong));
                cmd.AddParameter("@Atropine_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_HuyetAp_TT));
                cmd.AddParameter("@Atropine_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_HuyetAp_TTr));
                cmd.AddParameter("@Atropine_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_TanSoTim));
                cmd.AddParameter("@Atropine_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DoChenhApMin));
                cmd.AddParameter("@Atropine_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DoChenhApMax));
                cmd.AddParameter("@NgungNP_ThoiGian", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_ThoiGian));
                cmd.AddParameter("@NgungNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_HuyetAp_TT));
                cmd.AddParameter("@NgungNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_HuyetAp_TTr));
                cmd.AddParameter("@NgungNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_TanSoTim));
                cmd.AddParameter("@NgungNP_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_DoChenhApMin));
                cmd.AddParameter("@NgungNP_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_DoChenhApMax));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDobutamineElectrocardiogram
        public override URP_FE_StressDobutamineElectrocardiogram GetURP_FE_StressDobutamineElectrocardiogram(long URP_FE_StressDobutamineElectrocardiogramID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineElectrocardiogramGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineElectrocardiogramID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamineElectrocardiogram retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineElectrocardiogramFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineElectrocardiogramInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriConDauThatNguc));
                cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriDIGITALIS));
                cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LyDoKhongThucHienDuoc));
                cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MucGangSuc));
                cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThoiGianGangSuc));
                cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTim));
                cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApToiDa));
                cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ConDauThatNguc));
                cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.STChenhXuong));
                cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhipTim));
                cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipTimChiTiet));
                cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.XetNghiemKhac));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineElectrocardiogramUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@URP_FE_StressDobutamineElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineElectrocardiogramID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriConDauThatNguc));
                cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriDIGITALIS));
                cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LyDoKhongThucHienDuoc));
                cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MucGangSuc));
                cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThoiGianGangSuc));
                cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTim));
                cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApToiDa));
                cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ConDauThatNguc));
                cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.STChenhXuong));
                cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhipTim));
                cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipTimChiTiet));
                cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.XetNghiemKhac));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineElectrocardiogramDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineElectrocardiogramID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDobutamineExam
        public override URP_FE_StressDobutamineExam GetURP_FE_StressDobutamineExam(long URP_FE_StressDobutamineExamID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineExamGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineExamID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamineExam retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineExamFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineExamInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungHienTai));
                cmd.AddParameter("@ChiDinhSATGSDobu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChiDinhSATGSDobu));
                cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinhDetail));
                cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTruocNgayKham));
                cmd.AddParameter("@TDDTrongNgaySATGSDobu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTrongNgaySATGSDobu));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineExamUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDobutamineExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineExamID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungHienTai));
                cmd.AddParameter("@ChiDinhSATGSDobu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChiDinhSATGSDobu));
                cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinhDetail));
                cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTruocNgayKham));
                cmd.AddParameter("@TDDTrongNgaySATGSDobu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTrongNgaySATGSDobu));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineExamDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineExamID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDobutamineImages

        public override URP_FE_StressDobutamineImages GetURP_FE_StressDobutamineImages(long URP_FE_StressDobutamineImagesID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineImagesGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineImagesID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamineImages retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineImagesFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineImagesInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineImagesUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineImagesID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineImagesDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineImagesID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_StressDobutamineResult
        public override URP_FE_StressDobutamineResult GetURP_FE_StressDobutamineResult(long URP_FE_StressDobutamineResultID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineResultID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamineResult retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineResultFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineResultInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThayDoiDTD));
                cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThayDoiDTDChiTiet));
                cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhip));
                cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipChiTiet));
                cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TDPHayBienChung));
                cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungKhac));
                cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.BienPhapDieuTri));
                cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSieuAmTim));
                cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSieuAmTim));
                cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_TNP));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_TNP));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_TNP));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_KetLuan));
                cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_TNP));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_KetLuan));
                cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_TNP));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_KetLuan));
                cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_TNP));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_TNP));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_TNP));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_TNP));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_KetLuan));
                cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_TNP));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_KetLuan));
                cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_TNP));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_KetLuan));
                cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_TNP));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_KetLuan));
                cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_TNP));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_KetLuan));
                cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_TNP));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_KetLuan));
                cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_TNP));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_KetLuan));
                cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_TNP));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_KetLuan));
                cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_TNP));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_KetLuan));
                cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_TNP));
                cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineResultUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDobutamineResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThayDoiDTD));
                cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThayDoiDTDChiTiet));
                cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhip));
                cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipChiTiet));
                cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TDPHayBienChung));
                cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungKhac));
                cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.BienPhapDieuTri));
                cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSieuAmTim));
                cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSieuAmTim));
                cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_TNP));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_TNP));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_TNP));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_KetLuan));
                cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_TNP));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_KetLuan));
                cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_TNP));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_KetLuan));
                cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_TNP));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_TNP));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_TNP));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_TNP));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_KetLuan));
                cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_TNP));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_KetLuan));
                cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_TNP));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_KetLuan));
                cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_TNP));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_KetLuan));
                cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_TNP));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_KetLuan));
                cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_TNP));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_KetLuan));
                cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_TNP));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_KetLuan));
                cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_TNP));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_KetLuan));
                cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_TNP));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_KetLuan));
                cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_TNP));
                cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_KetLuan));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineResultID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_VasculaireAnother
        public override URP_FE_VasculaireAnother GetURP_FE_VasculaireAnother(long URP_FE_VasculaireAnotherID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAnotherGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireAnotherID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireAnotherID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_VasculaireAnother retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireAnotherFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAnotherInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@MoTa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.MoTa));
                cmd.AddParameter("@KetLuanEx", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuanEx));
                cmd.AddParameter("@V_MotaEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_MotaEx));
                cmd.AddParameter("@V_KetLuanEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuanEx));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAnotherUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_VasculaireAnotherID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireAnotherID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@MoTa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.MoTa));
                cmd.AddParameter("@KetLuanEx", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuanEx));
                cmd.AddParameter("@V_MotaEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_MotaEx));
                cmd.AddParameter("@V_KetLuanEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuanEx));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAnotherDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireAnotherID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireAnotherID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_VasculaireAorta
        public override URP_FE_VasculaireAorta GetURP_FE_VasculaireAorta(long URP_FE_VasculaireAortaID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAortaGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireAortaID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireAortaID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_VasculaireAorta retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireAortaFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAortaInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DMCNgang", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCNgang));
                cmd.AddParameter("@DMCLen", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCLen));
                cmd.AddParameter("@EoDMC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.EoDMC));
                cmd.AddParameter("@DMCXuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCXuong));
                cmd.AddParameter("@DMThanP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanP_v));
                cmd.AddParameter("@DMThanP_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanP_RI));
                cmd.AddParameter("@DMThanT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanT_v));
                cmd.AddParameter("@DMThanT_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanT_RI));
                cmd.AddParameter("@DMChauP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMChauP_v));
                cmd.AddParameter("@DMChauT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMChauT_v));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAortaUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_VasculaireAortaID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireAortaID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DMCNgang", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCNgang));
                cmd.AddParameter("@DMCLen", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCLen));
                cmd.AddParameter("@EoDMC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.EoDMC));
                cmd.AddParameter("@DMCXuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCXuong));
                cmd.AddParameter("@DMThanP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanP_v));
                cmd.AddParameter("@DMThanP_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanP_RI));
                cmd.AddParameter("@DMThanT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanT_v));
                cmd.AddParameter("@DMThanT_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanT_RI));
                cmd.AddParameter("@DMChauP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMChauP_v));
                cmd.AddParameter("@DMChauT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMChauT_v));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAortaDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireAortaID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireAortaID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_VasculaireCarotid
        public override URP_FE_VasculaireCarotid GetURP_FE_VasculaireCarotid(long URP_FE_VasculaireCarotidID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireCarotidGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireCarotidID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireCarotidID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_VasculaireCarotid retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireCarotidFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireCarotidInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@V_KetQuaSAMMTruoc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSAMMTruoc));
                cmd.AddParameter("@KetQuaSAMMTruoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSAMMTruoc));
                cmd.AddParameter("@DMCP_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ECA));
                cmd.AddParameter("@DMCP_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ICA));
                cmd.AddParameter("@DMCP_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ICA_SR));
                cmd.AddParameter("@DMCP_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_CCA_TCC));
                cmd.AddParameter("@DMCP_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_CCA));
                cmd.AddParameter("@DMCT_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ECA));
                cmd.AddParameter("@DMCT_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ICA));
                cmd.AddParameter("@DMCT_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ICA_SR));
                cmd.AddParameter("@DMCT_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_CCA_TCC));
                cmd.AddParameter("@DMCT_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_CCA));
                cmd.AddParameter("@DMCotSongP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongP_d));
                cmd.AddParameter("@DMCotSongP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongP_r));
                cmd.AddParameter("@DMCotSongT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongT_d));
                cmd.AddParameter("@DMCotSongT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongT_r));
                cmd.AddParameter("@DMDuoiDonP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonP_d));
                cmd.AddParameter("@DMDuoiDonP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonP_r));
                cmd.AddParameter("@DMDuoiDonT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonT_d));
                cmd.AddParameter("@DMDuoiDonT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonT_r));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireCarotidUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_VasculaireCarotidID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireCarotidID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@V_KetQuaSAMMTruoc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSAMMTruoc));
                cmd.AddParameter("@KetQuaSAMMTruoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSAMMTruoc));
                cmd.AddParameter("@DMCP_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ECA));
                cmd.AddParameter("@DMCP_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ICA));
                cmd.AddParameter("@DMCP_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ICA_SR));
                cmd.AddParameter("@DMCP_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_CCA_TCC));
                cmd.AddParameter("@DMCP_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_CCA));
                cmd.AddParameter("@DMCT_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ECA));
                cmd.AddParameter("@DMCT_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ICA));
                cmd.AddParameter("@DMCT_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ICA_SR));
                cmd.AddParameter("@DMCT_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_CCA_TCC));
                cmd.AddParameter("@DMCT_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_CCA));
                cmd.AddParameter("@DMCotSongP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongP_d));
                cmd.AddParameter("@DMCotSongP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongP_r));
                cmd.AddParameter("@DMCotSongT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongT_d));
                cmd.AddParameter("@DMCotSongT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongT_r));
                cmd.AddParameter("@DMDuoiDonP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonP_d));
                cmd.AddParameter("@DMDuoiDonP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonP_r));
                cmd.AddParameter("@DMDuoiDonT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonT_d));
                cmd.AddParameter("@DMDuoiDonT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonT_r));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireCarotidDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireCarotidID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireCarotidID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_VasculaireExam
        public override URP_FE_VasculaireExam GetURP_FE_VasculaireExam(long URP_FE_VasculaireExamID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireExamGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireExamID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_VasculaireExam retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireExamFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireExamInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@NoiLap", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NoiLap));
                cmd.AddParameter("@ChongMat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChongMat));
                cmd.AddParameter("@DotQuy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DotQuy));
                cmd.AddParameter("@GiamTriNho", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.GiamTriNho));
                cmd.AddParameter("@ThoangMu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThoangMu));
                cmd.AddParameter("@NhinMo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NhinMo));
                cmd.AddParameter("@LietNuaNguoi", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LietNuaNguoi));
                cmd.AddParameter("@TeYeuChanTay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TeYeuChanTay));
                cmd.AddParameter("@DaPThuatDMC", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DaPThuatDMC));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireExamUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_VasculaireExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireExamID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@NoiLap", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NoiLap));
                cmd.AddParameter("@ChongMat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChongMat));
                cmd.AddParameter("@DotQuy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DotQuy));
                cmd.AddParameter("@GiamTriNho", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.GiamTriNho));
                cmd.AddParameter("@ThoangMu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThoangMu));
                cmd.AddParameter("@NhinMo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NhinMo));
                cmd.AddParameter("@LietNuaNguoi", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LietNuaNguoi));
                cmd.AddParameter("@TeYeuChanTay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TeYeuChanTay));
                cmd.AddParameter("@DaPThuatDMC", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DaPThuatDMC));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireExamDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireExamID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_VasculaireLeg
        public override URP_FE_VasculaireLeg GetURP_FE_VasculaireLeg(long URP_FE_VasculaireLegID, long PCLImgResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireLegGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireLegID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireLegID));
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLImgResultID));

                cn.Open();
                URP_FE_VasculaireLeg retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireLegFromReader(reader);
                }

                reader.Close();


                return retVal;
            }
        }

        public override bool AddURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireLegInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CT_EI_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_EI_P));
                cmd.AddParameter("@CT_EI_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_EI_T));
                cmd.AddParameter("@CT_CF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_CF_P));
                cmd.AddParameter("@CT_CF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_CF_T));
                cmd.AddParameter("@CT_SF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_SF_P));
                cmd.AddParameter("@CT_SF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_SF_T));
                cmd.AddParameter("@CT_POP_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_POP_P));
                cmd.AddParameter("@CT_POP_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_POP_T));
                cmd.AddParameter("@CT_AT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_AT_P));
                cmd.AddParameter("@CT_AT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_AT_T));
                cmd.AddParameter("@CT_PER_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PER_P));
                cmd.AddParameter("@CT_PER_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PER_T));
                cmd.AddParameter("@CT_GrSaph_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_GrSaph_P));
                cmd.AddParameter("@CT_GrSaph_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_GrSaph_T));
                cmd.AddParameter("@CT_PT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PT_P));
                cmd.AddParameter("@CT_PT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PT_T));
                cmd.AddParameter("@CD_EI_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_EI_P));
                cmd.AddParameter("@CD_EI_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_EI_T));
                cmd.AddParameter("@CD_CF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_CF_P));
                cmd.AddParameter("@CD_CF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_CF_T));
                cmd.AddParameter("@CD_SF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_SF_P));
                cmd.AddParameter("@CD_SF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_SF_T));
                cmd.AddParameter("@CD_POP_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_POP_P));
                cmd.AddParameter("@CD_POP_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_POP_T));
                cmd.AddParameter("@CD_AT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_AT_P));
                cmd.AddParameter("@CD_AT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_AT_T));
                cmd.AddParameter("@CD_PER_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PER_P));
                cmd.AddParameter("@CD_PER_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PER_T));
                cmd.AddParameter("@CD_GrSaph_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_GrSaph_P));
                cmd.AddParameter("@CD_GrSaph_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_GrSaph_T));
                cmd.AddParameter("@CD_PT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PT_P));
                cmd.AddParameter("@CD_PT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PT_T));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool UpdateURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireLegUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_VasculaireLegID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireLegID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CT_EI_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_EI_P));
                cmd.AddParameter("@CT_EI_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_EI_T));
                cmd.AddParameter("@CT_CF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_CF_P));
                cmd.AddParameter("@CT_CF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_CF_T));
                cmd.AddParameter("@CT_SF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_SF_P));
                cmd.AddParameter("@CT_SF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_SF_T));
                cmd.AddParameter("@CT_POP_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_POP_P));
                cmd.AddParameter("@CT_POP_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_POP_T));
                cmd.AddParameter("@CT_AT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_AT_P));
                cmd.AddParameter("@CT_AT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_AT_T));
                cmd.AddParameter("@CT_PER_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PER_P));
                cmd.AddParameter("@CT_PER_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PER_T));
                cmd.AddParameter("@CT_GrSaph_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_GrSaph_P));
                cmd.AddParameter("@CT_GrSaph_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_GrSaph_T));
                cmd.AddParameter("@CT_PT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PT_P));
                cmd.AddParameter("@CT_PT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PT_T));
                cmd.AddParameter("@CD_EI_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_EI_P));
                cmd.AddParameter("@CD_EI_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_EI_T));
                cmd.AddParameter("@CD_CF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_CF_P));
                cmd.AddParameter("@CD_CF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_CF_T));
                cmd.AddParameter("@CD_SF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_SF_P));
                cmd.AddParameter("@CD_SF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_SF_T));
                cmd.AddParameter("@CD_POP_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_POP_P));
                cmd.AddParameter("@CD_POP_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_POP_T));
                cmd.AddParameter("@CD_AT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_AT_P));
                cmd.AddParameter("@CD_AT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_AT_T));
                cmd.AddParameter("@CD_PER_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PER_P));
                cmd.AddParameter("@CD_PER_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PER_T));
                cmd.AddParameter("@CD_GrSaph_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_GrSaph_P));
                cmd.AddParameter("@CD_GrSaph_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_GrSaph_T));
                cmd.AddParameter("@CD_PT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PT_P));
                cmd.AddParameter("@CD_PT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PT_T));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public override bool DeleteURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireLegDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireLegID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireLegID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        //==== 20161129 CMN Bein: Add button save for all FE pages
        public override bool AddAndUpdateFE_Vasculaire(URP_FE_Vasculaire entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateFE_Vasculaire", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.DoctorStaffID));
                    cmd.AddParameter("@NoiLap", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.NoiLap));
                    cmd.AddParameter("@ChongMat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.ChongMat));
                    cmd.AddParameter("@DotQuy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.DotQuy));
                    cmd.AddParameter("@GiamTriNho", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.GiamTriNho));
                    cmd.AddParameter("@ThoangMu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.ThoangMu));
                    cmd.AddParameter("@NhinMo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.NhinMo));
                    cmd.AddParameter("@LietNuaNguoi", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.LietNuaNguoi));
                    cmd.AddParameter("@TeYeuChanTay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.TeYeuChanTay));
                    cmd.AddParameter("@DaPThuatDMC", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.DaPThuatDMC));
                    cmd.AddParameter("@URP_FE_VasculaireExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.URP_FE_VasculaireExamID));
                    cmd.AddParameter("@Tab1_EX_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.Tab_Update_Required));

                    cmd.AddParameter("@V_KetQuaSAMMTruoc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.V_KetQuaSAMMTruoc));
                    cmd.AddParameter("@KetQuaSAMMTruoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.KetQuaSAMMTruoc));
                    cmd.AddParameter("@DMCP_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_ECA));
                    cmd.AddParameter("@DMCP_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_ICA));
                    cmd.AddParameter("@DMCP_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_ICA_SR));
                    cmd.AddParameter("@DMCP_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_CCA_TCC));
                    cmd.AddParameter("@DMCP_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_CCA));
                    cmd.AddParameter("@DMCT_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_ECA));
                    cmd.AddParameter("@DMCT_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_ICA));
                    cmd.AddParameter("@DMCT_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_ICA_SR));
                    cmd.AddParameter("@DMCT_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_CCA_TCC));
                    cmd.AddParameter("@DMCT_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_CCA));
                    cmd.AddParameter("@DMCotSongP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCotSongP_d));
                    cmd.AddParameter("@DMCotSongP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCotSongP_r));
                    cmd.AddParameter("@DMCotSongT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCotSongT_d));
                    cmd.AddParameter("@DMCotSongT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCotSongT_r));
                    cmd.AddParameter("@DMDuoiDonP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMDuoiDonP_d));
                    cmd.AddParameter("@DMDuoiDonP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMDuoiDonP_r));
                    cmd.AddParameter("@DMDuoiDonT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMDuoiDonT_d));
                    cmd.AddParameter("@DMDuoiDonT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMDuoiDonT_r));
                    cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.KetLuan));
                    cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.V_KetLuan));
                    cmd.AddParameter("@URP_FE_VasculaireCarotidID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.URP_FE_VasculaireCarotidID));
                    cmd.AddParameter("@Tab1_CA_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.Tab_Update_Required));

                    cmd.AddParameter("@URP_FE_VasculaireAortaID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.URP_FE_VasculaireAortaID));
                    cmd.AddParameter("@DMCNgang", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMCNgang));
                    cmd.AddParameter("@DMCLen", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMCLen));
                    cmd.AddParameter("@EoDMC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.EoDMC));
                    cmd.AddParameter("@DMCXuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMCXuong));
                    cmd.AddParameter("@DMThanP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMThanP_v));
                    cmd.AddParameter("@DMThanP_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMThanP_RI));
                    cmd.AddParameter("@DMThanT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMThanT_v));
                    cmd.AddParameter("@DMThanT_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMThanT_RI));
                    cmd.AddParameter("@DMChauP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMChauP_v));
                    cmd.AddParameter("@DMChauT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMChauT_v));
                    cmd.AddParameter("@KetLuan2", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.KetLuan));
                    cmd.AddParameter("@V_KetLuan2", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.V_KetLuan));
                    cmd.AddParameter("@Tab2_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.Tab_Update_Required));

                    cmd.AddParameter("@KetLuan3", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_StressDobutamineImages.KetLuan));
                    cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_StressDobutamineImages.URP_FE_StressDobutamineImagesID));
                    cmd.AddParameter("@Tab3_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_StressDobutamineImages.Tab_Update_Required));

                    cmd.AddParameter("@MoTa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.MoTa));
                    cmd.AddParameter("@KetLuanEx", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.KetLuanEx));
                    cmd.AddParameter("@V_MotaEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.V_MotaEx));
                    cmd.AddParameter("@V_KetLuanEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.V_KetLuanEx));
                    cmd.AddParameter("@URP_FE_VasculaireAnotherID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.URP_FE_VasculaireAnotherID));
                    cmd.AddParameter("@Tab4_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.Tab_Update_Required));
                    //==== #001
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.CreateDate));
                    //==== #001

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override bool AddAndUpdateUltraResParams_FetalEchocardiography(UltraResParams_FetalEchocardiography entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateUltraResParams_FetalEchocardiography", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@NTSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.NTSize));
                    cmd.AddParameter("@NPSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.NPSize));
                    cmd.AddParameter("@VanVieussensLeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.VanVieussensLeftAtrium));
                    cmd.AddParameter("@AtrialSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AtrialSeptalDefect));
                    cmd.AddParameter("@MitralValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.MitralValveSize));
                    cmd.AddParameter("@TriscupidValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.TriscupidValveSize));
                    cmd.AddParameter("@DifferenceMitralTricuspid", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DifferenceMitralTricuspid));
                    cmd.AddParameter("@TPTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.TPTTr));
                    cmd.AddParameter("@VLTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.VLTTTr));
                    cmd.AddParameter("@TTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.TTTTr));
                    cmd.AddParameter("@DKTTTTr_VGd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DKTTTTr_VGd));
                    cmd.AddParameter("@DKTTTT_VGs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DKTTTT_VGs));
                    cmd.AddParameter("@DKTPTTr_VDd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DKTPTTr_VDd));
                    cmd.AddParameter("@DKTPTT_VDs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DKTPTT_VDs));
                    cmd.AddParameter("@Systolic", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.Systolic));
                    cmd.AddParameter("@VentricularSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.VentricularSeptalDefect));
                    cmd.AddParameter("@AortaCompatible", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AortaCompatible));
                    cmd.AddParameter("@AortaSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AortaSize));
                    cmd.AddParameter("@PulmonaryArterySize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.PulmonaryArterySize));
                    cmd.AddParameter("@AorticArch", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AorticArch));
                    cmd.AddParameter("@AorticCoarctation", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AorticCoarctation));
                    cmd.AddParameter("@HeartRateNomal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.HeartRateNomal));
                    cmd.AddParameter("@RequencyHeartRateNomal", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.RequencyHeartRateNomal));
                    cmd.AddParameter("@PericardialEffusion", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.PericardialEffusion));
                    cmd.AddParameter("@FetalCardialAxis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.FetalCardialAxis));
                    cmd.AddParameter("@CardialRateS", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.CardialRateS));
                    cmd.AddParameter("@LN", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.LN));
                    cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.OrderRecord));
                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.DoctorStaffID));
                    cmd.AddParameter("@UltraResParams_FetalEchocardiography2DID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.UltraResParams_FetalEchocardiography2DID));
                    cmd.AddParameter("@Tab1_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.Tab_Update_Required));
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.CreateDate));
                    //==== 20161213 CMN Begin: Add lookup for FetalEchocardiography
                    cmd.AddParameter("@UltraResParams_FetalEchocardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyID));
                    cmd.AddParameter("@FetalAge", SqlDbType.TinyInt, ConvertNullObjectToDBNull(entity.FetalAge));
                    cmd.AddParameter("@NuchalTranslucency", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NuchalTranslucency));
                    if (entity.V_EchographyPosture != null)
                        cmd.AddParameter("@V_EchographyPosture", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_EchographyPosture.LookupID));
                    if (entity.V_MomMedHis != null)
                        cmd.AddParameter("@V_MomMedHis", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_MomMedHis.LookupID));
                    cmd.AddParameter("@Notice", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Notice));
                    //==== 20161213 CMN End.

                    cmd.AddParameter("@MitralValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.MitralValve_Vmax));
                    cmd.AddParameter("@MitralValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.MitralValve_Gdmax));
                    cmd.AddParameter("@MitralValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.MitralValve_Open));
                    cmd.AddParameter("@MitralValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.MitralValve_Stenosis));
                    cmd.AddParameter("@TriscupidValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.TriscupidValve_Vmax));
                    cmd.AddParameter("@TriscupidValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.TriscupidValve_Gdmax));
                    cmd.AddParameter("@TriscupidValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.TriscupidValve_Open));
                    cmd.AddParameter("@TriscupidValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.TriscupidValve_Stenosis));
                    cmd.AddParameter("@AorticValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticValve_Vmax));
                    cmd.AddParameter("@AorticValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticValve_Gdmax));
                    cmd.AddParameter("@AorticValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticValve_Open));
                    cmd.AddParameter("@AorticValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticValve_Stenosis));
                    cmd.AddParameter("@PulmonaryValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryValve_Vmax));
                    cmd.AddParameter("@PulmonaryValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryValve_Gdmax));
                    cmd.AddParameter("@PulmonaryValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryValve_Open));
                    cmd.AddParameter("@PulmonaryValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryValve_Stenosis));
                    cmd.AddParameter("@AorticCoarctationBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticCoarctationBloodTraffic));
                    cmd.AddParameter("@VanViewessensBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.VanViewessensBloodTraffic));
                    cmd.AddParameter("@DuctusAteriosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.DuctusAteriosusBloodTraffic));
                    cmd.AddParameter("@DuctusVenosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.DuctusVenosusBloodTraffic));
                    cmd.AddParameter("@PulmonaryVeins_LeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryVeins_LeftAtrium));
                    cmd.AddParameter("@OrderRecord2", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.OrderRecord));
                    cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.UltraResParams_FetalEchocardiographyDopplerID));
                    cmd.AddParameter("@Tab2_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.Tab_Update_Required));

                    cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.UltraResParams_FetalEchocardiographyResultID));
                    cmd.AddParameter("@CardialAbnormal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.CardialAbnormal));
                    cmd.AddParameter("@CardialAbnormalDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.CardialAbnormalDetail));
                    cmd.AddParameter("@Susgest", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.Susgest));
                    cmd.AddParameter("@UltraResParamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.UltraResParamDate));
                    cmd.AddParameter("@Tab3_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.Tab_Update_Required));

                    cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.UltraResParams_FetalEchocardiographyPostpartumID));
                    cmd.AddParameter("@BabyBirthday", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.BabyBirthday));
                    cmd.AddParameter("@BabyWeight", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.BabyWeight));
                    cmd.AddParameter("@BabySex", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.BabySex));
                    cmd.AddParameter("@URP_Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.URP_Date));
                    cmd.AddParameter("@PFO", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.PFO));
                    cmd.AddParameter("@PCA", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.PCA));
                    cmd.AddParameter("@AnotherDiagnosic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.AnotherDiagnosic));
                    cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.Notes));
                    cmd.AddParameter("@Tab4_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.Tab_Update_Required));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override bool AddAndUpdateURP_FE_Oesophagienne(URP_FE_OesophagienneUltra entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateURP_FE_Oesophagienne", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@URP_FE_OesophagienneID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.URP_FE_OesophagienneID));
                    cmd.AddParameter("@ChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.ChiDinh));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.DoctorStaffID));
                    //==== #001
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.CreateDate));
                    //==== #001
                    cmd.AddParameter("@Tab1_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.Tab_Update_Required));

                    cmd.AddParameter("@URP_FE_OesophagienneDiagnosisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneDiagnosis.URP_FE_OesophagienneDiagnosisID));
                    cmd.AddParameter("@ChanDoanQuaThucQuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneDiagnosis.ChanDoanQuaThucQuan));
                    cmd.AddParameter("@Tab2_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.Tab_Update_Required));

                    cmd.AddParameter("@URP_FE_OesophagienneCheckID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.URP_FE_OesophagienneCheckID));
                    cmd.AddParameter("@CatNghia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.CatNghia));
                    cmd.AddParameter("@NuotNghen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.NuotNghen));
                    cmd.AddParameter("@NuotDau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.NuotDau));
                    cmd.AddParameter("@OiMau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.OiMau));
                    cmd.AddParameter("@XaTriTrungThat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.XaTriTrungThat));
                    cmd.AddParameter("@CotSongCo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.CotSongCo));
                    cmd.AddParameter("@ChanThuongLongNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.ChanThuongLongNguc));
                    cmd.AddParameter("@LanKhamNoiSoiGanDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.LanKhamNoiSoiGanDay));
                    cmd.AddParameter("@DiUngThuoc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.DiUngThuoc));
                    cmd.AddParameter("@NghienRuou", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.NghienRuou));
                    cmd.AddParameter("@BiTieu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.BiTieu));
                    cmd.AddParameter("@TangNhanApGocHep", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.TangNhanApGocHep));
                    cmd.AddParameter("@Suyen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.Suyen));
                    cmd.AddParameter("@LanAnSauCung", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.LanAnSauCung));
                    cmd.AddParameter("@RangGiaHamGia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.RangGiaHamGia));
                    cmd.AddParameter("@HuyetApTT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.HuyetApTT));
                    cmd.AddParameter("@HuyetApTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.HuyetApTTr));
                    cmd.AddParameter("@Mach", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.Mach));
                    cmd.AddParameter("@DoBaoHoaOxy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.DoBaoHoaOxy));
                    cmd.AddParameter("@ThucHienDuongTruyenTinhMach", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.ThucHienDuongTruyenTinhMach));
                    cmd.AddParameter("@KiemTraDauDoSieuAm", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.KiemTraDauDoSieuAm));
                    cmd.AddParameter("@ChinhDauDoTrungTinh", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.ChinhDauDoTrungTinh));
                    cmd.AddParameter("@TeMeBenhNhan", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.TeMeBenhNhan));
                    cmd.AddParameter("@DatBenhNhanNghiengTrai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.DatBenhNhanNghiengTrai));
                    cmd.AddParameter("@CotDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.CotDay));
                    cmd.AddParameter("@BenhNhanThoaiMai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.BenhNhanThoaiMai));
                    cmd.AddParameter("@BoiTronDauDo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.BoiTronDauDo));
                    cmd.AddParameter("@Tab3_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.Tab_Update_Required));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override bool AddAndUpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamoleUltra entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateURP_FE_StressDipyridamole", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleImage.DoctorStaffID));
                    cmd.AddParameter("@URP_FE_StressDipyridamoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.URP_FE_StressDipyridamoleID));
                    cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TanSoTimCanDat));
                    cmd.AddParameter("@TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TNP_HuyetAp_TT));
                    cmd.AddParameter("@TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TNP_HuyetAp_TTr));
                    cmd.AddParameter("@TNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TNP_TanSoTim));
                    cmd.AddParameter("@TNP_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TNP_TacDungPhu));
                    cmd.AddParameter("@TruyenDipyridamole056_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipyridamole056_DungLuong));
                    cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P2_HuyetAp_TT));
                    cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P2_HuyetAp_TTr));
                    cmd.AddParameter("@TruyenDipy056_P2_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P2_TanSoTim));
                    cmd.AddParameter("@TruyenDipy056_P2_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P2_TacDungPhu));
                    cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P4_HuyetAp_TT));
                    cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P4_HuyetAp_TTr));
                    cmd.AddParameter("@TruyenDipy056_P4_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P4_TanSoTim));
                    cmd.AddParameter("@TruyenDipy056_P4_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P4_TacDungPhu));
                    cmd.AddParameter("@SauLieuDauP6_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieuDauP6_HuyetAp_TT));
                    cmd.AddParameter("@SauLieuDauP6_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieuDauP6_HuyetAp_TTr));
                    cmd.AddParameter("@SauLieuDauP6_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieuDauP6_TanSoTim));
                    cmd.AddParameter("@SauLieuDauP6_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieuDauP6_TacDungPhu));
                    cmd.AddParameter("@TruyenDipyridamole028_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipyridamole028_DungLuong));
                    cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy028_P8_HuyetAp_TT));
                    cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy028_P8_HuyetAp_TTr));
                    cmd.AddParameter("@TruyenDipy028_P8_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy028_P8_TanSoTim));
                    cmd.AddParameter("@TruyenDipy028_P8_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy028_P8_TacDungPhu));
                    cmd.AddParameter("@SauLieu2P10_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieu2P10_HuyetAp_TT));
                    cmd.AddParameter("@SauLieu2P10_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieu2P10_HuyetAp_TTr));
                    cmd.AddParameter("@SauLieu2P10_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieu2P10_TanSoTim));
                    cmd.AddParameter("@SauLieu2P10_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieu2P10_TacDungPhu));
                    cmd.AddParameter("@ThemAtropineP12_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP12_HuyetAp_TT));
                    cmd.AddParameter("@ThemAtropineP12_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP12_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAtropineP12_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP12_TanSoTim));
                    cmd.AddParameter("@ThemAtropineP12_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP12_TacDungPhu));
                    cmd.AddParameter("@ThemAtropineP13_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP13_HuyetAp_TT));
                    cmd.AddParameter("@ThemAtropineP13_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP13_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAtropineP13_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP13_TanSoTim));
                    cmd.AddParameter("@ThemAtropineP13_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP13_TacDungPhu));
                    cmd.AddParameter("@ThemAtropineP14_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP14_HuyetAp_TT));
                    cmd.AddParameter("@ThemAtropineP14_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP14_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAtropineP14_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP14_TanSoTim));
                    cmd.AddParameter("@ThemAtropineP14_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP14_TacDungPhu));
                    cmd.AddParameter("@ThemAtropineP15_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP15_HuyetAp_TT));
                    cmd.AddParameter("@ThemAtropineP15_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP15_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAtropineP15_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP15_TanSoTim));
                    cmd.AddParameter("@ThemAtropineP15_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP15_TacDungPhu));
                    cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TheoDoiAtropineP16_HuyetAp_TT));
                    cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TheoDoiAtropineP16_HuyetAp_TTr));
                    cmd.AddParameter("@TheoDoiAtropineP16_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TheoDoiAtropineP16_TanSoTim));
                    cmd.AddParameter("@TheoDoiAtropineP16_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TheoDoiAtropineP16_TacDungPhu));
                    cmd.AddParameter("@ThemAminophyline_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_DungLuong));
                    cmd.AddParameter("@ThemAminophyline_Phut", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_Phut));
                    cmd.AddParameter("@ThemAminophyline_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_HuyetAp_TT));
                    cmd.AddParameter("@ThemAminophyline_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAminophyline_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_TanSoTim));
                    cmd.AddParameter("@ThemAminophyline_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_TacDungPhu));

                    cmd.AddParameter("@URP_FE_StressDipyridamoleElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.URP_FE_StressDipyridamoleElectrocardiogramID));
                    cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.DieuTriConDauThatNguc));
                    cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.DieuTriDIGITALIS));
                    cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.LyDoKhongThucHienDuoc));
                    cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.MucGangSuc));
                    cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.ThoiGianGangSuc));
                    cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.TanSoTim));
                    cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.HuyetApToiDa));
                    cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.ConDauThatNguc));
                    cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.STChenhXuong));
                    cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.RoiLoanNhipTim));
                    cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.RoiLoanNhipTimChiTiet));
                    cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.XetNghiemKhac));

                    cmd.AddParameter("@URP_FE_StressDipyridamoleExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.URP_FE_StressDipyridamoleExamID));
                    cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.TrieuChungHienTai));
                    cmd.AddParameter("@ChiDinhSATGSDipy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.ChiDinhSATGSDipy));
                    cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.ChiDinhDetail));
                    cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.TDDTruocNgayKham));
                    cmd.AddParameter("@TDDTrongNgaySATGSDipy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.TDDTrongNgaySATGSDipy));

                    cmd.AddParameter("@URP_FE_StressDipyridamoleImageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleImage.URP_FE_StressDipyridamoleImageID));
                    cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleImage.KetLuan));
                    //==== #001
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleImage.CreateDate));
                    //==== #001

                    cmd.AddParameter("@URP_FE_StressDipyridamoleResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.URP_FE_StressDipyridamoleResultID));
                    cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThayDoiDTD));
                    cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThayDoiDTDChiTiet));
                    cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.RoiLoanNhip));
                    cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.RoiLoanNhipChiTiet));
                    cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TDPHayBienChung));
                    cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TrieuChungKhac));
                    cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.BienPhapDieuTri));
                    cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.V_KetQuaSieuAmTim));
                    cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.KetQuaSieuAmTim));
                    cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Mom_TNP));
                    cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Mom_KetLuan));
                    cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Giua_TNP));
                    cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Giua_KetLuan));
                    cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Day_TNP));
                    cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Day_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Mom_TNP));
                    cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Mom_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Mom_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Mom_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Giua_TNP));
                    cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Giua_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Giua_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Giua_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Day_TNP));
                    cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Day_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Day_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Day_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Mom_TNP));
                    cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Mom_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Giua_TNP));
                    cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Giua_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Day_TNP));
                    cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Day_KetLuan));
                    cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Mom_TNP));
                    cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Mom_KetLuan));
                    cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Giua_TNP));
                    cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Giua_KetLuan));
                    cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Day_TNP));
                    cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Day_KetLuan));
                    cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Mom_TNP));
                    cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Mom_KetLuan));
                    cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Giua_TNP));
                    cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Giua_KetLuan));
                    cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Day_TNP));
                    cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Day_KetLuan));
                    cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Mom_TNP));
                    cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Mom_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Mom_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Mom_KetLuan));
                    cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Giua_TNP));
                    cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Giua_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Giua_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Giua_KetLuan));
                    cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Day_TNP));
                    cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Day_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Day_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Day_KetLuan));

                    cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.URP_FE_ExamID));
                    cmd.AddParameter("@CaoHuyetAp", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.CaoHuyetAp));
                    cmd.AddParameter("@CaoHuyetApDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.CaoHuyetApDetail));
                    cmd.AddParameter("@Cholesterol", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Cholesterol));
                    cmd.AddParameter("@Triglyceride", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Triglyceride));
                    cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.HDL));
                    cmd.AddParameter("@LDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.LDL));
                    cmd.AddParameter("@TieuDuong", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.TieuDuong));
                    cmd.AddParameter("@TieuDuongDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.TieuDuongDetail));
                    cmd.AddParameter("@ThuocLa", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocLa));
                    cmd.AddParameter("@Detail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Detail));
                    cmd.AddParameter("@ThuocNguaThai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocNguaThai));
                    cmd.AddParameter("@ThuocNguaThaiDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocNguaThaiDetail));
                    cmd.AddParameter("NhanApMP", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.NhanApMP));
                    cmd.AddParameter("NhanApMT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.NhanApMT));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override bool AddAndUpdateURP_FE_StressDobutamine(URP_FE_StressDobutamineUltra entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateURP_FE_StressDobutamine", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineImages.DoctorStaffID));
                    cmd.AddParameter("@URP_FE_StressDobutamineID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.URP_FE_StressDobutamineID));
                    cmd.AddParameter("@TruyenTinhMach", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TruyenTinhMach));
                    cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TanSoTimCanDat));
                    cmd.AddParameter("@TD_TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_TT));
                    cmd.AddParameter("@TD_TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_TTr));
                    cmd.AddParameter("@TD_TNP_HuyetAp_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_TanSoTim));
                    cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_DoChenhApMin));
                    cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_DoChenhApMax));
                    cmd.AddParameter("@FiveMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_DungLuong));
                    cmd.AddParameter("@FiveMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_HuyetAp_TT));
                    cmd.AddParameter("@FiveMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_HuyetAp_TTr));
                    cmd.AddParameter("@FiveMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_TanSoTim));
                    cmd.AddParameter("@FiveMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_DoChenhApMin));
                    cmd.AddParameter("@FiveMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_DoChenhApMax));
                    cmd.AddParameter("@TenMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_DungLuong));
                    cmd.AddParameter("@TenMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_HuyetAp_TT));
                    cmd.AddParameter("@TenMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_HuyetAp_TTr));
                    cmd.AddParameter("@TenMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_TanSoTim));
                    cmd.AddParameter("@TenMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_DoChenhApMin));
                    cmd.AddParameter("@TenMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_DoChenhApMax));
                    cmd.AddParameter("@TwentyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_DungLuong));
                    cmd.AddParameter("@TwentyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_HuyetAp_TT));
                    cmd.AddParameter("@TwentyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_HuyetAp_TTr));
                    cmd.AddParameter("@TwentyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_TanSoTim));
                    cmd.AddParameter("@TwentyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_DoChenhApMin));
                    cmd.AddParameter("@TwentyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_DoChenhApMax));
                    cmd.AddParameter("@ThirtyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_DungLuong));
                    cmd.AddParameter("@ThirtyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_HuyetAp_TT));
                    cmd.AddParameter("@ThirtyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_HuyetAp_TTr));
                    cmd.AddParameter("@ThirtyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_TanSoTim));
                    cmd.AddParameter("@ThirtyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_DoChenhApMin));
                    cmd.AddParameter("@ThirtyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_DoChenhApMax));
                    cmd.AddParameter("@FortyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_DungLuong));
                    cmd.AddParameter("@FortyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_HuyetAp_TT));
                    cmd.AddParameter("@FortyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_HuyetAp_TTr));
                    cmd.AddParameter("@FortyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_TanSoTim));
                    cmd.AddParameter("@FortyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_DoChenhApMin));
                    cmd.AddParameter("@FortyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_DoChenhApMax));
                    cmd.AddParameter("@Atropine_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_DungLuong));
                    cmd.AddParameter("@Atropine_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_HuyetAp_TT));
                    cmd.AddParameter("@Atropine_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_HuyetAp_TTr));
                    cmd.AddParameter("@Atropine_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_TanSoTim));
                    cmd.AddParameter("@Atropine_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_DoChenhApMin));
                    cmd.AddParameter("@Atropine_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_DoChenhApMax));
                    cmd.AddParameter("@NgungNP_ThoiGian", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_ThoiGian));
                    cmd.AddParameter("@NgungNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_HuyetAp_TT));
                    cmd.AddParameter("@NgungNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_HuyetAp_TTr));
                    cmd.AddParameter("@NgungNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_TanSoTim));
                    cmd.AddParameter("@NgungNP_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_DoChenhApMin));
                    cmd.AddParameter("@NgungNP_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_DoChenhApMax));

                    cmd.AddParameter("@URP_FE_StressDobutamineElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.URP_FE_StressDobutamineElectrocardiogramID));
                    cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.DieuTriConDauThatNguc));
                    cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.DieuTriDIGITALIS));
                    cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.LyDoKhongThucHienDuoc));
                    cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.MucGangSuc));
                    cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.ThoiGianGangSuc));
                    cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.TanSoTim));
                    cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.HuyetApToiDa));
                    cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.ConDauThatNguc));
                    cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.STChenhXuong));
                    cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.RoiLoanNhipTim));
                    cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.RoiLoanNhipTimChiTiet));
                    cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.XetNghiemKhac));

                    cmd.AddParameter("@URP_FE_StressDobutamineExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.URP_FE_StressDobutamineExamID));
                    cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.TrieuChungHienTai));
                    cmd.AddParameter("@ChiDinhSATGSDobu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.ChiDinhSATGSDobu));
                    cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.ChiDinhDetail));
                    cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.TDDTruocNgayKham));
                    cmd.AddParameter("@TDDTrongNgaySATGSDobu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.TDDTrongNgaySATGSDobu));

                    cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineImages.URP_FE_StressDobutamineImagesID));
                    cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineImages.KetLuan));
                    //==== #001
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineImages.CreateDate));
                    //==== #001

                    cmd.AddParameter("@URP_FE_StressDobutamineResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.URP_FE_StressDobutamineResultID));
                    cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThayDoiDTD));
                    cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThayDoiDTDChiTiet));
                    cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.RoiLoanNhip));
                    cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.RoiLoanNhipChiTiet));
                    cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TDPHayBienChung));
                    cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TrieuChungKhac));
                    cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.BienPhapDieuTri));
                    cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.V_KetQuaSieuAmTim));
                    cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.KetQuaSieuAmTim));
                    cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Mom_TNP));
                    cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Mom_KetLuan));
                    cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Giua_TNP));
                    cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Giua_KetLuan));
                    cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Day_TNP));
                    cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Day_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Mom_TNP));
                    cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Mom_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Mom_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Mom_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Giua_TNP));
                    cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Giua_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Giua_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Giua_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Day_TNP));
                    cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Day_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Day_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Day_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Mom_TNP));
                    cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Mom_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Giua_TNP));
                    cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Giua_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Day_TNP));
                    cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Day_KetLuan));
                    cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Mom_TNP));
                    cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Mom_KetLuan));
                    cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Giua_TNP));
                    cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Giua_KetLuan));
                    cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Day_TNP));
                    cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Day_KetLuan));
                    cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Mom_TNP));
                    cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Mom_KetLuan));
                    cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Giua_TNP));
                    cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Giua_KetLuan));
                    cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Day_TNP));
                    cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Day_KetLuan));
                    cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Mom_TNP));
                    cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Mom_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Mom_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Mom_KetLuan));
                    cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Giua_TNP));
                    cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Giua_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Giua_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Giua_KetLuan));
                    cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Day_TNP));
                    cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Day_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Day_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Day_KetLuan));

                    cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.URP_FE_ExamID));
                    cmd.AddParameter("@CaoHuyetAp", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.CaoHuyetAp));
                    cmd.AddParameter("@CaoHuyetApDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.CaoHuyetApDetail));
                    cmd.AddParameter("@Cholesterol", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Cholesterol));
                    cmd.AddParameter("@Triglyceride", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Triglyceride));
                    cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.HDL));
                    cmd.AddParameter("@LDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.LDL));
                    cmd.AddParameter("@TieuDuong", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.TieuDuong));
                    cmd.AddParameter("@TieuDuongDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.TieuDuongDetail));
                    cmd.AddParameter("@ThuocLa", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocLa));
                    cmd.AddParameter("@Detail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Detail));
                    cmd.AddParameter("@ThuocNguaThai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocNguaThai));
                    cmd.AddParameter("@ThuocNguaThaiDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocNguaThaiDetail));
                    cmd.AddParameter("NhanApMP", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.NhanApMP));
                    cmd.AddParameter("NhanApMT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.NhanApMT));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==== 20161129 CMN End: Add button save for all FE pages
        #endregion
    }
}