using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using AxLogging;
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
namespace eHCMS.DAL
{
    public class SqlCommonRecordsProvider : CommonRecordsProvider
    {
        public SqlCommonRecordsProvider()
            : base()
        {

        }

        #region Override methods
        //Retrieving data
        #region Common
        public override List<Lookup> GetLookupVitalSignDataType()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.VSIGN_DATA_TYPE);
            return objLst;
        }

        public override List<Lookup> GetLookupVitalSignContext()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.VITAL_SIGN_CONTEXT);
            return objLst;
        }

        public override List<Lookup> GetLookupPMHStatus()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.PAST_MED_HISTORY_STATUS);
            return objLst;
        }

        public override List<Lookup> GetLookupByObjectTypeID(LookupValues objectTypeID)
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(objectTypeID);
            return objLst;
        }

        protected override DiseasesReference GetRefDiseasesFromReader(IDataReader reader)
        {
            DiseasesReference p = base.GetRefDiseasesFromReader(reader);
            p.ParIDCode = reader["ParIDCode"] as long?;
            p.DeptID = reader["DeptID"] as long?;
            p.StaffID = reader["StaffID"] as long?;
            p.DiseaseDescription = reader["DiseaseDescription"].ToString();
            p.DiseaseNotes = reader["DiseaseNotes"].ToString();
            p.IsUserDefined = reader["IsUserDefined"] as bool?;
            return p;
        }

        public override List<DiseasesReference> GetDiseasessReferences()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiseasesReferences_ByICD10Code", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@ICD10Code", SqlDbType.VarChar);
                par1.Value = "";
                cn.Open();
                List<DiseasesReference> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefDiseasesCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override List<DiseasesReference> GetDiseasessRefByICD10Code(string icd10Code)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiseasesReferences_ByICD10Code", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@ICD10Code", SqlDbType.VarChar);
                par1.Value = icd10Code;
                cn.Open();
                List<DiseasesReference> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefDiseasesCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        #endregion

        #region 1.VitalSigns
        //Stored procedure: spVitalSigns_All
        public override List<VitalSign> GetAllVitalSigns()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spVitalSigns_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<VitalSign> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetVitalSignCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override bool DeleteVitalSigns(byte vitalSignID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spVitalSigns_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@VSignID", SqlDbType.TinyInt, vitalSignID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool AddVitalSigns(VitalSign entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spVitalSigns_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@VSignName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.VSignName));
                cmd.AddParameter("@VSignDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.VSignDescription));
                cmd.AddParameter("@IsPrimaryVitalSigns", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsPrimaryVitalSigns));
                cmd.AddParameter("@V_VSignDataType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_VSignDataType));
                cmd.AddParameter("@MedUnit", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.MedUnit));

                cn.Open();
                try
                {
                    int retVal = ExecuteNonQuery(cmd);
                    return retVal > 0;
                }
                catch (SqlException exp)
                {
                    return exp.ErrorCode > 0;
                }
            }
        }

        public override bool UpdateVitalSigns(VitalSign entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spVitalSigns_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@VSignID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(entity.VSignID));
                cmd.AddParameter("@VSignName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.VSignName));
                cmd.AddParameter("@VSignDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.VSignDescription));
                cmd.AddParameter("@IsPrimaryVitalSigns", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsPrimaryVitalSigns));
                cmd.AddParameter("@V_VSignDataType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_VSignDataType));
                cmd.AddParameter("@MedUnit", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.MedUnit));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override List<PatientVitalSign> GetVitalSignsByPtID(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientVitalSigns_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);

                par1.Value = patientID;

                cn.Open();
                List<PatientVitalSign> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientVitalSignCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        /// <summary>
        /// Delete patient vitalsign
        /// </summary>
        /// <param name="entity">Current patient vitalsign</param>
        /// <param name="staffID">Logged User</param>
        /// <returns></returns>
        public override bool DeleteItemPtVitalSigns(PatientVitalSign entity, long? staffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("[spPatientsVitalSigns_Delete]", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, staffID);
                cmd.AddParameter("@PtVSignID", SqlDbType.BigInt, entity.PtVSignID);
                cmd.AddParameter("@VSignID", SqlDbType.TinyInt, entity.VSignID);
                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, entity.CommonMedRecID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        /// <summary>
        /// Add new Patient VitalSign into Patient Common Record info
        /// </summary>
        /// <param name="entity">Patient Common Medical Details Info</param>
        /// <param name="staffID">Logged User</param>
        /// <returns></returns>
        public override bool AddItemPtVitalSigns(PatientVitalSign entity, long? staffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientVitalSigns_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedicalRecord.PatientID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(staffID));
                cmd.AddParameter("@VSignID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(entity.VitalSign.VSignID));
                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedRecID));
                cmd.AddParameter("@VSignExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.VSignExamDate));
                cmd.AddParameter("@VSignValue1", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.VSignValue1));
                cmd.AddParameter("@VSignValue2", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.VSignValue2));
                cmd.AddParameter("@VSignNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.VSignNotes));
                cmd.AddParameter("@V_VSignContext", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.LookupVSignContext.LookupID));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        /// <summary>
        /// Update Patient VitalSign 
        /// </summary>
        /// <param name="entity">New Patient Common Medical Details - New Patient VitalSIgn</param>
        /// <param name="oldPtVSignID">Current Patient VitalSign</param>
        /// <param name="staffID">Logged user</param>
        /// <returns></returns>
        public override bool UpdateItemPtVitalSigns(PatientVitalSign entity, long oldPtVSignID, long? staffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientVitalSigns_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(staffID));
                cmd.AddParameter("@OldPtVSignID", SqlDbType.BigInt, ConvertNullObjectToDBNull(oldPtVSignID));
                cmd.AddParameter("@VSignID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(entity.VitalSign.VSignID));//Change from combo box in GUIs
                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CommonMedRecID));
                cmd.AddParameter("@VSignExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.VSignExamDate));
                cmd.AddParameter("@VSignValue1", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.VSignValue1));
                cmd.AddParameter("@VSignValue2", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.VSignValue2));
                cmd.AddParameter("@VSignNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.VSignNotes));
                cmd.AddParameter("@V_VSignContext", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.LookupVSignContext.LookupID));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }


        #endregion

        #region 2.Medical Conditions
        public override List<RefMedicalConditionType> GetRefMedCondType()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalConditionTypes_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@MCTypeID", SqlDbType.Int);
                par1.Value = 0;
                cn.Open();
                List<RefMedicalConditionType> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefMedicalConditionTypeCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override List<RefMedicalCondition> GetRefMedConditions()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalConditions_ByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@MCTypeID", SqlDbType.Int);
                par1.Value = 0;
                cn.Open();
                List<RefMedicalCondition> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefMedicalConditionCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }


        public override List<RefMedicalCondition> GetRefMedConditionsByType(int medCondTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalConditions_ByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@MCTypeID", SqlDbType.BigInt);

                par1.Value = medCondTypeID;
                cn.Open();
                List<RefMedicalCondition> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefMedicalConditionCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }

        }
        //Dinh them vao

        public override bool DeleteMedReCond(long? MCRecID, long? StaffID, long? CommonMedRecID)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spMedicalRecords_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@MCRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MCRecID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { return false; }
            return true;
        }
        public override bool AddNewMedReCond(long? PatientID, long? StaffID
                                                    , long? CommonMedRecID, long? MCID, bool? MCYesNo
                                                    , string MCTextValue, string MCExplainOrNotes)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spMedicalRecords_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));

                    cmd.AddParameter("@MCID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MCID));
                    cmd.AddParameter("@MCYesNo", SqlDbType.Bit, ConvertNullObjectToDBNull(MCYesNo));
                    cmd.AddParameter("@MCTextValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MCTextValue));
                    cmd.AddParameter("@MCExplainOrNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MCExplainOrNotes));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { return false; }
            return true;
        }

        public override bool UpdateMedReCond(long? MCRecID, long? StaffID, long? CommonMedRecID
                                                , long? MCID, bool? MCYesNo, string MCTextValue
                                                , string MCExplainOrNotes)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spMedicalRecords_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@MCRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MCRecID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));

                    cmd.AddParameter("@MCID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MCID));
                    cmd.AddParameter("@MCYesNo", SqlDbType.Bit, ConvertNullObjectToDBNull(MCYesNo));
                    cmd.AddParameter("@MCTextValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MCTextValue));
                    cmd.AddParameter("@MCExplainOrNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MCExplainOrNotes));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Stored procedure: spMedicalConditionRecords_ByPtID
        /// </summary>
        /// <param name="patientID"></param>
        /// <returns></returns>
        public override List<MedicalConditionRecord> GetMedConditionByPtID(long patientID, int mcTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedicalConditionRecords_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.NVarChar);
                SqlParameter par2 = cmd.Parameters.Add("@MCTypeID", SqlDbType.Int);

                par1.Value = patientID;
                par2.Value = mcTypeID;

                cn.Open();
                List<MedicalConditionRecord> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetMedicalConditionRecordCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        //Manipulate on refMedicalConditionType
        public override bool DeleteRefMedCondType(RefMedicalConditionType entity)
        {
            throw new NotImplementedException();
        }

        public override bool AddRefMedCondType(RefMedicalConditionType entity)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateRefMedCondType(RefMedicalConditionType entity)
        {
            throw new NotImplementedException();
        }

        //Manipulate on refMedicalConditions
        public override bool DeleteRefMedCond(RefMedicalCondition entity)
        {
            throw new NotImplementedException();
        }

        public override bool AddRefMedCond(RefMedicalCondition entity)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateRefMedCond(RefMedicalCondition entity)
        {
            throw new NotImplementedException();
        }


        //Manipulate on MedicalConditionRecords
        public override bool DeleteMedCondRecs(MedicalConditionRecord entity)
        {
            throw new NotImplementedException();
        }

        public override bool AddMedCondRecs(MedicalConditionRecord entity)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateMedCondRecs(MedicalConditionRecord entity)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region 3.Medical History
        public override List<RefMedicalHistory> GetRefMedHistory()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalHistory_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<RefMedicalHistory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefMedicalHistoryCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override List<PastMedicalConditionHistory> GetPastMedCondHisByPtID(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPastMedicalConditionHistory_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.NVarChar);

                par1.Value = patientID;

                cn.Open();
                List<PastMedicalConditionHistory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPastMedCondHistoryCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }

        }


        //Manipulate on RefMedicalHistory
        public override bool DeleteRefMedicalHistory(RefMedicalHistory entity)
        {
            throw new NotImplementedException();
        }

        public override bool AddRefMedicalHistory(RefMedicalHistory entity)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateRefMedicalHistory(RefMedicalHistory entity)
        {
            throw new NotImplementedException();
        }
        //Dinh them

        public override bool DeleteMedicalHistory(long? PMHID
                                                      , long? StaffID
                                                      , long? CommonMedRecID)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPastMedicalRecords_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PMHID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PMHID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { return false; }
            return true;
        }
        public override bool AddNewMedicalHistory(long? PatientID
                                                   , long? StaffID
                                                   , long? CommonMedRecID
                                                   , long? MedHistCode
                                                   , bool? PMHYesNo
                                                   , string PMHExplainReason
                                                   , long? V_PMHStatus)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPastMedicalRecords_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cmd.AddParameter("@MedHistCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedHistCode));
                    cmd.AddParameter("@PMHYesNo", SqlDbType.Bit, ConvertNullObjectToDBNull(PMHYesNo));
                    cmd.AddParameter("@PMHExplainReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PMHExplainReason));
                    cmd.AddParameter("@V_PMHStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PMHStatus));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { return false; }
            return true;
        }
        public override bool UpdateMedicalHistory(long? StaffID
                                                   , long? PMHID
                                                   , long? CommonMedRecID
                                                   , long? MedHistCode
                                                   , bool? PMHYesNo
                                                   , string PMHExplainReason
                                                   , long? V_PMHStatus)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPastMedicalRecords_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@PMHID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PMHID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cmd.AddParameter("@MedHistCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedHistCode));
                    cmd.AddParameter("@PMHYesNo", SqlDbType.Bit, ConvertNullObjectToDBNull(PMHYesNo));
                    cmd.AddParameter("@PMHExplainReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PMHExplainReason));
                    cmd.AddParameter("@V_PMHStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PMHStatus));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { return false; }
            return true;
        }

        //Manipulate on PastMedicalConditionHistory
        public override bool DeleteMedCondHis(PastMedicalConditionHistory entity)
        {
            throw new NotImplementedException();
        }

        public override bool AddMedCondHis(PastMedicalConditionHistory entity)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateMedCondHis(PastMedicalConditionHistory entity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 4.Immunization

        public override List<RefImmunization> GetRefImmunization(long MedServiceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefImmunizations_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));
                cn.Open();
                List<RefImmunization> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefImmunizationCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override List<ImmunizationHistory> GetImmunizationByPtID(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spImmunizationHis_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.NVarChar);

                par1.Value = patientID;

                cn.Open();
                List<ImmunizationHistory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetImmunizationHistoryCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }


        //Manupulate on RefImmunization
        public override bool DeleteImmunization(RefImmunization entity)
        {
            throw new NotImplementedException();
        }

        public override bool AddImmunization(RefImmunization entity)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateImmunization(RefImmunization entity)
        {
            throw new NotImplementedException();
        }


        //Manupulate on ImmunizationHistory
        public override bool DeleteImmuHis(ImmunizationHistory entity)
        {
            throw new NotImplementedException();
        }

        public override bool AddImmuHis(ImmunizationHistory entity)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateImmuHis(ImmunizationHistory entity)
        {
            throw new NotImplementedException();
        }

        //Dinh them vao
        public override bool DeleteImmunization(long? IHID
                                                    , long? StaffID
                                                    , long? CommonMedRecID)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spImmunizationHistory_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@IHID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IHID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }
        public override bool AddNewImmunization(long? PatientID
                                                    , long? StaffID
                                                    , long? IHCode
                                                   , long? CommonMedRecID
                                                   , bool? IHYesNo
                                                   , DateTime? IHDate)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spImmunizationHistory_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));

                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    cmd.AddParameter("@IHCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IHCode));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cmd.AddParameter("@IHYesNo", SqlDbType.Bit, ConvertNullObjectToDBNull(IHYesNo));
                    cmd.AddParameter("@IHDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(IHDate));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { return false; }
            return true;
        }
        public override bool UpdateImmunization(long? IHID
                                                    , long? StaffID
                                                    , long? IHCode
                                                   , long? CommonMedRecID
                                                   , bool? IHYesNo
                                                   , DateTime? IHDate)
        {
            try
            {

                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spImmunizationHistory_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));

                    cmd.AddParameter("@IHID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IHID));
                    cmd.AddParameter("@IHCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IHCode));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cmd.AddParameter("@IHYesNo", SqlDbType.Bit, ConvertNullObjectToDBNull(IHYesNo));
                    cmd.AddParameter("@IHDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(IHDate));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { return false; }
            return true;
        }
        #endregion

        #region 5.Hospitalization
        public override List<HospitalizationHistory> GetHospitalizationHistoryByPtID(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHospitalizationHistory_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.NVarChar);

                par1.Value = patientID;
                cn.Open();
                List<HospitalizationHistory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetHospitalizationHistoryCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        public override Hospital GetHospitalFromReader(IDataReader reader)
        {
            Hospital p = new Hospital();
            try
            {
                p.HosID = (long)reader["HosID"];
                p.HosName = reader["HosName"].ToString();

                p.CityProvinceID = (long)reader["CityProvinceID"];
                p.HospitalCode = reader["HospitalCode"].ToString();
                p.HosShortName = reader["HosShortName"].ToString();

                p.HosAddress = reader["HosAddress"].ToString();
                p.HosLogoImgPath = reader["HosLogoImgPath"].ToString();
                p.Slogan = reader["Slogan"].ToString();
                p.HosPhone = reader["HosPhone"].ToString();
                p.HosWebSite = reader["HosWebSite"].ToString();
                p.V_HospitalType = (long)reader["V_HospitalType"];
                p.HICode = reader["HICode"].ToString();
            }
            catch
            { return null; }
            return p;
        }
        public override List<Hospital> GetHospitalByKey(string SearchKey, long V_HospitalType, int PageSize, int PageIndex)
        {
            List<Hospital> objLst = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    //SqlCommand cmd = new SqlCommand("spHospitals_SearchAutoComplete", cn);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.AddParameter("@SearchKey", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchKey));
                    //cmd.AddParameter("@V_HospitalType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_HospitalType));
                    //cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                    //cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));

                    SqlCommand cmd = new SqlCommand("proc_HospitalsSearch", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchKey));

                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);
                    objLst = GetHospitalCollectionFromReader(reader);
                    reader.Close();
                }
            }
            catch { return null; }
            return objLst;
        }
        public override void HospitalizationHistory_Delete(long? HHID, long? StaffID, long? CommonMedRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHospitalizationHistory_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@HHID", SqlDbType.BigInt, ConvertNullObjectToDBNull(HHID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                cn.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cn.Close();
            }

        }
        public override void HospitalizationHistory_Update(
                                                            long? HHID
                                                            , long? StaffID
                                                                , long? CommonMedRecID
                                                              , long? IDCode
                                                                , string HDate
                                                              , DateTime? FromDate
                                                              , long? FromHosID
                                                              , long? V_AdmissionType
            //, long? V_AdmissionReason
            //, long V_ReferralType
                                                              , string GeneralDiagnoses
            //, DateTime? ToDate
            //, long? ToHosID
                                                              , long? V_TreatmentResult
                                                              , long? V_DischargeReason
            //, long? V_HospitalType
                                                              , string HHNotes)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spHospitalizationHistory_Update_New", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@HHID", SqlDbType.BigInt, ConvertNullObjectToDBNull(HHID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));

                    cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDCode));
                    cmd.AddParameter("@HDate", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HDate));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                    cmd.AddParameter("@FromHosID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FromHosID));
                    cmd.AddParameter("@V_AdmissionType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AdmissionType));
                    //cmd.AddParameter("@V_AdmissionReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AdmissionReason));
                    //cmd.AddParameter("@V_ReferralType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ReferralType));
                    cmd.AddParameter("@GeneralDiagnoses", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GeneralDiagnoses));

                    //cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                    //cmd.AddParameter("@ToHosID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ToHosID));
                    cmd.AddParameter("@V_TreatmentResult", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_TreatmentResult));
                    cmd.AddParameter("@V_DischargeReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_DischargeReason));

                    //cmd.AddParameter("@V_HospitalType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_HospitalType));
                    cmd.AddParameter("@HHNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HHNotes));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override void HospitalizationHistory_Insert(
                                                                long? PatientID
                                                              , long? StaffID
                                                              , long? CommonMedRecID
                                                              , long? IDCode
                                                                , string HDate
                                                              , DateTime? FromDate
                                                              , long? FromHosID
                                                              , long? V_AdmissionType
            //, long? V_AdmissionReason
            //, long V_ReferralType
                                                              , string GeneralDiagnoses
            //, DateTime? ToDate
            //, long? ToHosID
                                                              , long? V_TreatmentResult
                                                              , long? V_DischargeReason
            //, long? V_HospitalType
                                                              , string HHNotes)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spHospitalizationHistory_Insert_New", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));

                    cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDCode));
                    cmd.AddParameter("@HDate", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HDate));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                    cmd.AddParameter("@FromHosID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FromHosID));
                    cmd.AddParameter("@V_AdmissionType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AdmissionType));
                    //cmd.AddParameter("@V_AdmissionReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AdmissionReason));
                    //cmd.AddParameter("@V_ReferralType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ReferralType));
                    cmd.AddParameter("@GeneralDiagnoses", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GeneralDiagnoses));

                    //cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                    //cmd.AddParameter("@ToHosID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ToHosID));
                    cmd.AddParameter("@V_TreatmentResult", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_TreatmentResult));
                    cmd.AddParameter("@V_DischargeReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_DischargeReason));

                    //cmd.AddParameter("@V_HospitalType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_HospitalType));
                    cmd.AddParameter("@HHNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HHNotes));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 6.Family History
        public override List<FamilyHistory> GetFamilyHistoryByPtID(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spFamilyHistory_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.NVarChar);

                par1.Value = patientID;
                cn.Open();
                List<FamilyHistory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetFamilyHistoryCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        //Manupulate on FamilyHistory
        public override bool DeleteFamilyHistory(RefImmunization entity)
        {
            throw new NotImplementedException();
        }

        public override bool AddFamilyHistory(RefImmunization entity)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateFamilyHistory(RefImmunization entity)
        {
            throw new NotImplementedException();
        }
        public override void FamilyHistory_Insert(long? PatientID
                                                        , long? StaffID
                                                        , long? CommonMedRecID
                                                        , long? IDCode
                                                       , string FHFullName
                                                       , long? V_FamilyRelationship
                                                       , string FHNotes
                                                       , bool? Decease)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spFamilyHistory_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDCode));
                    cmd.AddParameter("@FHFullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(FHFullName));
                    cmd.AddParameter("@V_FamilyRelationship", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_FamilyRelationship));
                    cmd.AddParameter("@FHNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(FHNotes));
                    cmd.AddParameter("@Decease", SqlDbType.Bit, ConvertNullObjectToDBNull(Decease));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }
        public override void FamilyHistory_Update(long? FHCode
                                                        , long? StaffID
                                                        , long? CommonMedRecID
                                                        , long? IDCode
                                                       , string FHFullName
                                                       , long? V_FamilyRelationship
                                                       , string FHNotes
                                                       , bool? Decease
                                                        , DateTime? CMRModifiedDate)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spFamilyHistory_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@FHCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(FHCode));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDCode));
                    cmd.AddParameter("@FHFullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(FHFullName));
                    cmd.AddParameter("@V_FamilyRelationship", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_FamilyRelationship));
                    cmd.AddParameter("@FHNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(FHNotes));
                    cmd.AddParameter("@Decease", SqlDbType.Bit, ConvertNullObjectToDBNull(Decease));
                    cmd.AddParameter("@CMRModifiedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(CMRModifiedDate));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }

        }
        public override void FamilyHistory_Delete(long? StaffID
                                                    , long? FHCode
                                                    , long? CommonMedRecID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spFamilyHistory_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.AddParameter("@FHCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(FHCode));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { }
        }
        #endregion

        #region 7.Physical Examination

        public override void PhysicalExamination_Insert(long? PatientID
                                                           , long? StaffID
                                                           , PhysicalExamination p)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPhysicalExamination_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));

                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.CommonMedRecID));
                if (p.RecordDate == null)
                {
                    p.RecordDate = DateTime.Now as DateTime?;
                }
                //cmd.AddParameter("@RecordDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.RecordDate));

                cmd.AddParameter("@Height", SqlDbType.Float, ConvertNullObjectToDBNull(p.Height));
                cmd.AddParameter("@Weight", SqlDbType.Float, ConvertNullObjectToDBNull(p.Weight));
                cmd.AddParameter("@SystolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.SystolicPressure));
                cmd.AddParameter("@DiastolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.DiastolicPressure));
                cmd.AddParameter("@Pulse", SqlDbType.Float, ConvertNullObjectToDBNull(p.Pulse));
                cmd.AddParameter("@Cholesterol", SqlDbType.Float, ConvertNullObjectToDBNull(p.Cholesterol));

                //cmd.AddParameter("@Smoke_EveryDay", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Smoke_EveryDay));
                //cmd.AddParameter("@Smoke_OnOccasion", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Smoke_OnOccasion));
                //cmd.AddParameter("@Smoke_Never", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Smoke_Never));
                //cmd.AddParameter("@Alcohol_CurrentHeavy", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Alcohol_CurrentHeavy));
                //cmd.AddParameter("@Alcohol_HeavyInThePast", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Alcohol_HeavyInThePast));
                //cmd.AddParameter("@Alcohol_CurrentLight", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Alcohol_CurrentLight));
                //cmd.AddParameter("@Alcohol_Never", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Alcohol_Never));
                cmd.AddParameter("@V_SmokeStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.RefSmoke.LookupID));
                cmd.AddParameter("@V_AlcoholDrinkingStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.RefAlcohol.LookupID));
                cmd.AddParameter("@CVRisk", SqlDbType.Float, ConvertNullObjectToDBNull(p.CVRisk));

                cmd.AddParameter("@MonthHaveSmoked", SqlDbType.Float, ConvertNullObjectToDBNull(p.MonthHaveSmoked));
                cmd.AddParameter("@MonthQuitSmoking", SqlDbType.Float, ConvertNullObjectToDBNull(p.MonthQuitSmoking));
                cmd.AddParameter("@SmokeCigarettePerDay", SqlDbType.TinyInt, ConvertNullObjectToDBNull(p.SmokeCigarettePerDay));

                cmd.AddParameter("@RespiratoryRate", SqlDbType.Float, ConvertNullObjectToDBNull(p.RespiratoryRate));
                cmd.AddParameter("@SpO2", SqlDbType.Float, ConvertNullObjectToDBNull(p.SpO2));
                cmd.AddParameter("@Temperature", SqlDbType.Float, ConvertNullObjectToDBNull(p.Temperature));
                cn.Open();
                cmd.ExecuteNonQuery();
            }

        }
        //▼====== #001
        public override void PhysicalExamination_Insert_V2(long? PatientID, long PtRegistrationID, long V_RegistrationType
                                                           , long? StaffID
                                                           , PhysicalExamination p)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPhysicalExamination_Insert_V2", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.CommonMedRecID));
                if (p.RecordDate == null)
                {
                    p.RecordDate = DateTime.Now as DateTime?;
                }
                cmd.AddParameter("@Height", SqlDbType.Float, ConvertNullObjectToDBNull(p.Height));
                cmd.AddParameter("@Weight", SqlDbType.Float, ConvertNullObjectToDBNull(p.Weight));
                cmd.AddParameter("@BMI", SqlDbType.Float, ConvertNullObjectToDBNull(p.BMI));
                cmd.AddParameter("@SystolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.SystolicPressure));
                cmd.AddParameter("@DiastolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.DiastolicPressure));
                cmd.AddParameter("@Pulse", SqlDbType.Float, ConvertNullObjectToDBNull(p.Pulse));
                cmd.AddParameter("@Cholesterol", SqlDbType.Float, ConvertNullObjectToDBNull(p.Cholesterol));
                cmd.AddParameter("@V_SmokeStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.RefSmoke.LookupID));
                cmd.AddParameter("@V_AlcoholDrinkingStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.RefAlcohol.LookupID));
                cmd.AddParameter("@CVRisk", SqlDbType.Float, ConvertNullObjectToDBNull(p.CVRisk));
                cmd.AddParameter("@MonthHaveSmoked", SqlDbType.Float, ConvertNullObjectToDBNull(p.MonthHaveSmoked));
                cmd.AddParameter("@MonthQuitSmoking", SqlDbType.Float, ConvertNullObjectToDBNull(p.MonthQuitSmoking));
                cmd.AddParameter("@SmokeCigarettePerDay", SqlDbType.TinyInt, ConvertNullObjectToDBNull(p.SmokeCigarettePerDay));
                cmd.AddParameter("@RespiratoryRate", SqlDbType.Float, ConvertNullObjectToDBNull(p.RespiratoryRate));
                cmd.AddParameter("@SpO2", SqlDbType.Float, ConvertNullObjectToDBNull(p.SpO2));
                cmd.AddParameter("@Temperature", SqlDbType.Float, ConvertNullObjectToDBNull(p.Temperature));
                cn.Open();
                cmd.ExecuteNonQuery();
            }

        }

        public override void PhysicalExamination_Update_V2(long? StaffID , PhysicalExamination p)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPhysicalExamination_Update_V2", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PtRegistrationID));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.V_RegistrationType));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.CommonMedRecID));
                    cmd.AddParameter("@RecordDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.RecordDate));
                    cmd.AddParameter("@Height", SqlDbType.Float, ConvertNullObjectToDBNull(p.Height));
                    cmd.AddParameter("@Weight", SqlDbType.Float, ConvertNullObjectToDBNull(p.Weight));
                    cmd.AddParameter("@BMI", SqlDbType.Float, ConvertNullObjectToDBNull(p.BMI));
                    cmd.AddParameter("@SystolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.SystolicPressure));
                    cmd.AddParameter("@DiastolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.DiastolicPressure));
                    cmd.AddParameter("@Pulse", SqlDbType.Float, ConvertNullObjectToDBNull(p.Pulse));
                    cmd.AddParameter("@Cholesterol", SqlDbType.Float, ConvertNullObjectToDBNull(p.Cholesterol));
                    cmd.AddParameter("@V_SmokeStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.RefSmoke.LookupID));
                    cmd.AddParameter("@V_AlcoholDrinkingStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.RefAlcohol.LookupID));
                    cmd.AddParameter("@CVRisk", SqlDbType.Float, ConvertNullObjectToDBNull(p.CVRisk));
                    cmd.AddParameter("@MonthHaveSmoked", SqlDbType.Float, ConvertNullObjectToDBNull(p.MonthHaveSmoked));
                    cmd.AddParameter("@MonthQuitSmoking", SqlDbType.Float, ConvertNullObjectToDBNull(p.MonthQuitSmoking));
                    cmd.AddParameter("@SmokeCigarettePerDay", SqlDbType.TinyInt, ConvertNullObjectToDBNull(p.SmokeCigarettePerDay));
                    cmd.AddParameter("@RespiratoryRate", SqlDbType.Float, ConvertNullObjectToDBNull(p.RespiratoryRate));
                    cmd.AddParameter("@SpO2", SqlDbType.Float, ConvertNullObjectToDBNull(p.SpO2));
                    cmd.AddParameter("@Temperature", SqlDbType.Float, ConvertNullObjectToDBNull(p.Temperature));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }

        }
        //▲====== #001
        public override void PhysicalExamination_Delete(long? StaffID, long? PhyExamID, long? CommonMedRecID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPhysicalExamination_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@PhyExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PhyExamID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

            }
            catch { }
        }
        public override void PhysicalExamination_Update(long? StaffID, long? PhyExamID
                                                            , PhysicalExamination p)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPhysicalExamination_Update1", cn);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@PhyExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PhyExamID));

                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.CommonMedRecID));

                    cmd.AddParameter("@RecordDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.RecordDate));
                    cmd.AddParameter("@Height", SqlDbType.Float, ConvertNullObjectToDBNull(p.Height));
                    cmd.AddParameter("@Weight", SqlDbType.Float, ConvertNullObjectToDBNull(p.Weight));
                    cmd.AddParameter("@SystolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.SystolicPressure));
                    cmd.AddParameter("@DiastolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.DiastolicPressure));
                    cmd.AddParameter("@Pulse", SqlDbType.Float, ConvertNullObjectToDBNull(p.Pulse));
                    cmd.AddParameter("@Cholesterol", SqlDbType.Float, ConvertNullObjectToDBNull(p.Cholesterol));


                    cmd.AddParameter("@V_SmokeStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.RefSmoke.LookupID));
                    cmd.AddParameter("@V_AlcoholDrinkingStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.RefAlcohol.LookupID));
                    cmd.AddParameter("@CVRisk", SqlDbType.Float, ConvertNullObjectToDBNull(p.CVRisk));

                    cmd.AddParameter("@MonthHaveSmoked", SqlDbType.Float, ConvertNullObjectToDBNull(p.MonthHaveSmoked));
                    cmd.AddParameter("@MonthQuitSmoking", SqlDbType.Float, ConvertNullObjectToDBNull(p.MonthQuitSmoking));
                    cmd.AddParameter("@SmokeCigarettePerDay", SqlDbType.TinyInt, ConvertNullObjectToDBNull(p.SmokeCigarettePerDay));

                    cmd.AddParameter("@RespiratoryRate", SqlDbType.Float, ConvertNullObjectToDBNull(p.RespiratoryRate));
                    cmd.AddParameter("@SpO2", SqlDbType.Float, ConvertNullObjectToDBNull(p.SpO2));
                    cmd.AddParameter("@Temperature", SqlDbType.Float, ConvertNullObjectToDBNull(p.Temperature));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }

        }
        protected virtual PhysicalExamination GetPhysicalExamFromReader(IDataReader reader)
        {
            PhysicalExamination p = new PhysicalExamination();
            try
            {
                p.CommonMedRecID = (long)reader["CommonMedRecID"];
                p.RecordDate = (DateTime?)reader["RecordDate"];
                p.Height = (float)reader["Height"];
                p.Weight = (float)reader["Weight"];
                p.SystolicPressure = (float)reader["SystolicPressure"];
                p.DiastolicPressure = (float)reader["DiastolicPressure"];
                p.Pulse = (float)reader["Pulse"];
                p.Cholesterol = (float)reader["Cholesterol"];
                //p.Smoke_EveryDay = (bool)reader["Smoke_EveryDay"];
                //p.Smoke_OnOccasion = (bool)reader["Smoke_OnOccasion"];
                //p.Smoke_Never = (bool)reader["Smoke_Never"];
                //p.Alcohol_CurrentHeavy = (bool)reader["Alcohol_CurrentHeavy"];
                //p.Alcohol_HeavyInThePast = (bool)reader["Alcohol_HeavyInThePast"];
                //p.Alcohol_CurrentLight = (bool)reader["Alcohol_CurrentLight"];
                //p.Alcohol_Never = (bool)reader["Alcohol_Never"];
                p.CVRisk = (float)reader["CVRisk"];

                p.MonthHaveSmoked = (float)reader["MonthHaveSmoked"];
                p.MonthQuitSmoking = (float)reader["MonthQuitSmoking"];
                p.SmokeCigarettePerDay = Convert.ToInt16(reader["SmokeCigarettePerDay"]);
            }
            catch { }
            return p;
        }
        protected virtual List<PhysicalExamination> GetPhysicalExamCollectionFromReader(IDataReader reader)
        {
            List<PhysicalExamination> PhysicalExam = new List<PhysicalExamination>();
            try
            {
                while (reader.Read())
                {
                    PhysicalExam.Add(GetPhysicalExamFromReader(reader));
                }
            }
            catch { }

            return PhysicalExam;
        }
        public override PhysicalExamination PhysicalExamination_GetData(long? CommonMedRecID)
        {
            PhysicalExamination obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPhysicalExamination_ByID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));

                    cn.Open();


                    IDataReader reader = ExecuteReader(cmd);

                    while (reader.Read())
                    {

                        obj = GetPhysicalExamFromReader(reader);
                    }
                    reader.Close();
                }

            }
            catch
            { }
            return obj;
        }
        public override List<PhysicalExamination> PhysicalExamination_ListData(long? CommonMedRecID)
        {
            List<PhysicalExamination> PhysicalObj = new List<PhysicalExamination>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_ByLID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {

                    PhysicalObj = GetPhysicalExamCollectionFromReader(reader);
                }
                reader.Close();
            }
            return PhysicalObj;
        }
        #endregion

        #region 8. Risk Factors

        public override bool RiskFactorInsert(RiskFactors p)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRiskFactorsInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PatientID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.StaffID));

                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.CommonMedRecID));

                cmd.AddParameter("@Diabetics", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Diabetics));
                cmd.AddParameter("@DiabeticsDescr", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.DiabeticsDescr));

                cmd.AddParameter("@Drinking", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Drinking));
                cmd.AddParameter("@DrinkingDescr", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.DrinkingDescr));

                cmd.AddParameter("@Dyslipidemia", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Dyslipidemia));
                cmd.AddParameter("@DyslipidemiaDescr", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.DyslipidemiaDescr));

                cmd.AddParameter("@Hypertension", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Hypertension));
                cmd.AddParameter("@HypertensionDescr", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.HypertensionDescr));

                cmd.AddParameter("@Obesity", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Obesity));
                cmd.AddParameter("@ObesityDescr", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.ObesityDescr));

                cmd.AddParameter("@Smoking", SqlDbType.Bit, ConvertNullObjectToDBNull(p.Smoking));
                cmd.AddParameter("@SmokingDescr", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.SmokingDescr));

                cmd.AddParameter("@Other", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.Other));                
                
                cn.Open();
                var val = cmd.ExecuteNonQuery();
                return val > 0;
            }

        }
        public override bool RiskFactorDelete(long RiskFactorID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRiskFactorsDelete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RiskFactorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RiskFactorID));

                    cn.Open();
                    var val=cmd.ExecuteNonQuery();
                    return val > 0;
                }

            }
            catch
            {
                return false; 
            }
        }

        public override List<RiskFactors> RiskFactorGet(long? PatientID)
        {
            List<RiskFactors> obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRiskFactorsGet", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));

                    cn.Open();


                    IDataReader reader = ExecuteReader(cmd);
                    obj = GetRiskFactorCollectionFromReader(reader);                    

                    reader.Close();
                }

            }
            catch
            { }
            return obj;
        }
       
        #endregion
        #endregion
    }
}
