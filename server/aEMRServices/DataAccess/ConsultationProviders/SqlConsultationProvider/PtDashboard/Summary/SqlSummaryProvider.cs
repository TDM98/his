using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-13
 * Contents: Consultation Services
/*******************************************************************/
#endregion

using eHCMS.DAL;
/*
 * 20182510 #001 TTM: BM 0002173: Thay đổi cách lưu/ cập nhật/ lấy dữ liệu của tình trạng thể chất từ lấy theo TTTC cuối cùng => theo đăng ký
 */
namespace eHCMS.DAL
{
    public class SqlSummaryProvider : SummaryProvider
    {
        public SqlSummaryProvider()
            : base()
        {

        }

        #region Override methods

        #region 1.Allergies
        public override List<MDAllergy> MDAllergies_ByPatientID(Int64 PatientID, int flag)
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
                return retVal;
            }
        }

        public override void MDAllergies_Save(MDAllergy Obj,out string Result)
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
                cmd.AddParameter("@V_AItemType", SqlDbType.BigInt,ConvertNullObjectToDBNull(Obj.V_AItemType));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

            }         
        }

        public override void MDAllergies_IsDeleted(MDAllergy Obj, out string Result)
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
            }
        }


        //public override List<MDAllergy> GetAllMDAllergies()
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spMDAllergies_ByPtID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
        //        SqlParameter par2 = cmd.Parameters.Add("@V_ALItemType", SqlDbType.BigInt);

        //        par1.Value = -1;
        //        par2.Value = -1;

        //        cn.Open();
        //        List<MDAllergy> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetMDAllergyCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}
        
        //public override List<MDAllergy> GetMDAllergiesByPtID(long patientID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spMDAllergies_ByPtID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
        //        SqlParameter par2 = cmd.Parameters.Add("@V_ALItemType", SqlDbType.BigInt);

        //        par1.Value = patientID;
        //        par2.Value = -1;

        //        cn.Open();
        //        List<MDAllergy> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetMDAllergyCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }

        //}

        //public override bool DeleteMDAllergy(MDAllergy entity)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spMDAllergies_Delete", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        //cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedicalRecord.StaffID));
        //        //cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedicalRecord.PatientID));
        //        cmd.AddParameter("@AItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.AItemID));

        //        cn.Open();

        //        int retVal = ExecuteNonQuery(cmd);
        //        return retVal > 0;
        //    }
        //}

        //public override bool AddMDAllergy(MDAllergy entity)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spMDAllergies_Insert", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        //cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedicalRecord.PatientID));
        //        //cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedicalRecord.StaffID));
        //        cmd.AddParameter("@AllergiesItems", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AllergiesItems));
        //        cmd.AddParameter("@Reactions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Reactions));
        //        cmd.AddParameter("@V_AItemType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_AItemType));
        //        cn.Open();

        //        int retVal = ExecuteNonQuery(cmd);
        //        return retVal > 0;
        //    }
        //}

        //public override bool UpdateMDAllergy(MDAllergy entity)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        #region 2.Warnning
        public override List<MDWarning> MDWarnings_ByPatientID(Int64 PatientID, int flag)
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
                return retVal;
            }
        }

        public override void MDWarnings_Save(MDWarning Obj, out long Result)
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
            }
        }

        public override void MDWarnings_IsDeleted(MDWarning Obj, out string Result)
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
            }
        }


        //public override List<MDWarning> GetAllMDWarnings()
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spMDWarnings_ByPtID", cn);
        //        //cmd.AddParameters(
        //        string[] pars = new string[1];
        //        pars[0] = "@PatientID";

        //        SqlDbType[] parsType = new SqlDbType[1];
        //        parsType[0] = SqlDbType.BigInt;

        //        object[] vals = new object[1];
        //        vals[0] = -1;

        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameters(pars, parsType, vals);


        //        cn.Open();
        //        List<MDWarning> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetMDWarningCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        //public override List<MDWarning> GetMDWarningsByPtID(long patientID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spMDWarnings_ByPtID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);

        //        par1.Value = patientID;

        //        cn.Open();
        //        List<MDWarning> objLst = null;
        //        IDataReader reader = ExecuteReader(cmd);
        //        objLst = GetMDWarningCollectionFromReader(reader);
        //        reader.Close();
        //        return objLst;
        //    }
        //}

        //public override string GetStMDWarningsByPtID(long patientID)
        //{
        //    List<MDWarning> objLst = null;
        //    objLst = this.GetMDWarningsByPtID(patientID);
        //    string sResult = string.Empty;
        //    foreach (MDWarning warningInfo in objLst)
        //    {
        //        sResult += warningInfo.WarningItems + ", ";
        //    }
            
        //    if (sResult != null && sResult.Length >= 2)
        //    {
        //        sResult = sResult.Remove(sResult.Length - 2);
        //    }
        //    return sResult;
        //}

        //public override bool DeleteMDWarning(MDWarning entity)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spMDWarnings_Delete", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        //cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedicalRecord.StaffID));
        //        //cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedicalRecord.PatientID));

        //        cn.Open();

        //        int retVal = ExecuteNonQuery(cmd);
        //        return retVal > 0;
        //    }
        //}
        
        //public override bool AddMDWarning(MDWarning entity)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spMDWarnings_Insert", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        //cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedicalRecord.PatientID));
        //        //cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedicalRecord.StaffID));
        //        cmd.AddParameter("@WarningItems", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.WarningItems));
        //        cn.Open();

        //        int retVal = ExecuteNonQuery(cmd);
        //        return retVal > 0;
        //    }
        //}

        //public override bool UpdateMDWarning(MDWarning entity)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        #region 3.PhysicalExamination

        public override List<PhysicalExamination> GetPhyExamByPtID(long patientID)
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
                return phyExamList;
            }            
        }
        public override PhysicalExamination GetLastPhyExamByPtID(long patientID)
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
                return phyExam;
            }
        }

        //▼====== #001
        public override PhysicalExamination GetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
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
                return phyExam;
            }
        }
        //▲====== #001
        #endregion

        #region 4.Consultation

        public override List<PatientServiceRecord> GetConsultationByPtID(long patientID, long processTypeID)
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
                return consultationList;
            }
        }
        
        public override List<PatientServiceRecord> GetSumConsulByPtID(long patientID, long processTypeID,int PageIndex,int PageSize,out int Total)
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
                if ( paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
                return sumConsulList;
            }
        }

        #endregion

        #endregion
    }
}
