using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
using eHCMS.DAL;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-13
 * Contents: Consultation Services
/*******************************************************************/
#endregion
/*
 * 20182510 #001 TTM: BM 0002173: Thay đổi cách lưu/ cập nhật/ lấy dữ liệu của tình trạng thể chất từ lấy theo TTTC cuối cùng => theo đăng ký
 */
namespace aEMR.DataAccessLayer.Providers
{
    public class SummaryProvider: DataProviderBase
    {
        static private SummaryProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public SummaryProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SummaryProvider();
                }
                return _instance;
            }
        }

        public SummaryProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }

        public  List<MDAllergy> MDAllergies_ByPatientID(Int64 PatientID, int flag)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = (SqlCommand)conn.CreateCommand();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spMDAllergies_ByPatientID";

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(@PatientID));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(@flag));

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                IDataReader reader = ExecuteReader(cmd);
                List<MDAllergy> retVal = null;

                try
                {
                    retVal = GetMDAllergyCollectionFromReader(reader);
                }
                finally
                {
                    reader.Close();
                }
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }

        public  void MDAllergies_Save(MDAllergy Obj, out string Result)
        {
            Result = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spMDAllergies_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AItemID", SqlDbType.BigInt, Obj.AItemID);
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PatientID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.StaffID));
                cmd.AddParameter("@AllergiesItems", SqlDbType.NVarChar, Obj.AllergiesItems);
                cmd.AddParameter("@Reactions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Reactions));
                cmd.AddParameter("@V_AItemType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_AItemType));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);

            }
        }

        public  void MDAllergies_IsDeleted(MDAllergy Obj, out string Result)
        {
            Result = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spMDAllergies_IsDeleted", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AItemID", SqlDbType.BigInt, Obj.AItemID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.StaffID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        #region 2.Warnning
        public  List<MDWarning> MDWarnings_ByPatientID(Int64 PatientID, int flag)
        {
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMDWarnings_ByPatientID", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@flag", SqlDbType.Int, ConvertNullObjectToDBNull(flag));

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                IDataReader reader = ExecuteReader(cmd);
                List<MDWarning> retVal = null;

                try
                {
                    retVal = GetMDWarningCollectionFromReader(reader);
                }
                finally
                {
                    reader.Close();
                }
                CleanUpConnectionAndCommand(conn, cmd);
                return retVal;
            }
        }

        public  void MDWarnings_Save(MDWarning Obj, out long Result)
        {
            Result = -1;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMDWarnings_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@WItemID", SqlDbType.BigInt, Obj.WItemID);
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PatientID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.StaffID));
                cmd.AddParameter("@WarningItems", SqlDbType.NVarChar, Obj.WarningItems);

                cmd.AddParameter("@Result", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = (long)cmd.Parameters["@Result"].Value;
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public  void MDWarnings_IsDeleted(MDWarning Obj, out string Result)
        {
            Result = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spMDWarnings_IsDeleted", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@WItemID", SqlDbType.BigInt, Obj.WItemID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.StaffID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        #endregion

        #region 3.PhysicalExamination

        public  List<PhysicalExamination> GetPhyExamByPtID(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                //SqlCommand cmd = new SqlCommand("spPhysicalExamination_ByPtID", cn);
                //Dinh sua
                SqlCommand cmd = new SqlCommand("spPhysicalExamination_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                SqlParameter par2 = cmd.Parameters.Add("@IsLastest", SqlDbType.Bit);

                par1.Value = patientID;
                par2.Value = 0;

                cn.Open();
                List<PhysicalExamination> phyExamList = null;
                IDataReader reader = ExecuteReader(cmd);
                phyExamList = GetPhysicalExaminationCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return phyExamList;
            }
        }
        
        public List<PhysicalExamination> GetPhyExamByPtID_InPT(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPhysicalExamination_ByPtID_InPT", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                List<PhysicalExamination> phyExamList = null;
                IDataReader reader = ExecuteReader(cmd);
                phyExamList = GetPhysicalExaminationCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return phyExamList;
            }
        }
        public  PhysicalExamination GetLastPhyExamByPtID(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                //Dinh sua
                SqlCommand cmd = new SqlCommand("spPhysicalExamination_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                SqlParameter par2 = cmd.Parameters.Add("@IsLastest", SqlDbType.Bit);

                par1.Value = patientID;
                par2.Value = 1;

                cn.Open();
                PhysicalExamination phyExam = null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    phyExam = GetPhysicalExaminationFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return phyExam;
            }
        }

        //▼====== #001
        public  PhysicalExamination GetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPhysicalExamination_ByPtRegID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cn.Open();
                PhysicalExamination phyExam = null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    phyExam = GetPhysicalExaminationFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return phyExam;
            }
        }
        //▲====== #001
        #endregion

        #region 4.Consultation

        public  List<PatientServiceRecord> GetConsultationByPtID(long patientID, long processTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultation_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                SqlParameter par2 = cmd.Parameters.Add("@ProcessingTypeID", SqlDbType.BigInt);

                par1.Value = patientID;
                par2.Value = processTypeID;

                cn.Open();
                List<PatientServiceRecord> consultationList = null;
                IDataReader reader = ExecuteReader(cmd);
                consultationList = GetConsultationCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return consultationList;
            }
        }

        public  List<PatientServiceRecord> GetSumConsulByPtID(long patientID, long processTypeID, int PageIndex, int PageSize, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSumConsultation_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, patientID);
                cmd.AddParameter("@ProcessingTypeID", SqlDbType.BigInt, processTypeID);
                cmd.AddParameter("@PageIndex", SqlDbType.BigInt, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.BigInt, PageSize);
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                List<PatientServiceRecord> sumConsulList = null;
                IDataReader reader = ExecuteReader(cmd);
                sumConsulList = GetConsultationCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return sumConsulList;
            }
        }
    }
    #endregion
}
