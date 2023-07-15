using System;
using System.Collections.Generic;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using eHCMS.Configurations;
using eHCMS.DAL;
using AxLogging;
using System.Linq;
using DataEntities.MedicalInstruction;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-13
 * Contents: Consultation Services
/*******************************************************************/
#endregion
/*
 * 20182510 #001 TTM: BM 0002173: Thay đổi cách lưu/ cập nhật/ lấy dữ liệu của tình trạng thể chất từ lấy theo TTTC cuối cùng => theo đăng ký
 * 20210428 #002 BLQ: Added GetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID
 * 20220125 #003 TNHX: Lưu thông tin nước tiểu
 * 20220509 #004 DatTB: Chỉnh sửa HSBA RHM: Bệnh chuyên khoa long -> string
 * 20220624 #005 DatTB: Thêm các thông tin: Mã số HSBA, Ngày đẩy cổng, Ngày tổng kết
 * 20220625 #006 DatTB: Thêm function lấy loại điều trị ngoại trú.
 * 20220708 #007 BLQ: Thêm trường đánh giá ý thức và mức độ đau
 * 20220721 #008 DatTB: Validate dữ liệu trước khi gửi lên server
 * 20220811 #009 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
 * + Thêm trường Ngày dự kiến tổng kết
 * 20220830 #010 QTD Lưu thông tin bệnh án theo cách mới
 * 20230201 #011 DatTB: Thêm trường dữ liệu về KSNK trong phần Thông tin chung NB nội trú
 * 20230411 #012 BLQ: Thêm lấy TT HSBA cho điều trị ngoại trú
 * 20230415 #013 QTD: Thêm Giấy ra viện
 * 20230503 #014 DatTB: 
 * + Viết service get/insert/update tuổi động mạch
 * + Viết stored get/insert/update tuổi động mạch
 * 20230626 #015 DatTB: Thêm thông tin khoa/phòng lưu sinh hiệu
 */
namespace aEMR.DataAccessLayer.Providers
{
    public class CommonRecordsProvider : DataProviderBase
    {
        static private CommonRecordsProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public CommonRecordsProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommonRecordsProvider();
                }
                return _instance;
            }
        }

        public CommonRecordsProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }
        public List<Lookup> GetLookupVitalSignDataType()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.VSIGN_DATA_TYPE);
            return objLst;
        }

        public List<Lookup> GetLookupVitalSignContext()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.VITAL_SIGN_CONTEXT);
            return objLst;
        }

        public List<Lookup> GetLookupPMHStatus()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.PAST_MED_HISTORY_STATUS);
            return objLst;
        }

        public List<Lookup> GetLookupByObjectTypeID(LookupValues objectTypeID)
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
            reader.Close();
            return p;
        }

        public List<DiseasesReference> GetDiseasessReferences()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public List<DiseasesReference> GetDiseasessRefByICD10Code(string icd10Code)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        #region 1.VitalSigns
        //Stored procedure: spVitalSigns_All
        public List<VitalSign> GetAllVitalSigns()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public bool DeleteVitalSigns(byte vitalSignID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spVitalSigns_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@VSignID", SqlDbType.TinyInt, vitalSignID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool AddVitalSigns(VitalSign entity)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
                catch (SqlException exp)
                {
                    return exp.ErrorCode > 0;
                }
            }
        }

        public bool UpdateVitalSigns(VitalSign entity)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public List<PatientVitalSign> GetVitalSignsByPtID(long patientID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        /// <summary>
        /// Delete patient vitalsign
        /// </summary>
        /// <param name="entity">Current patient vitalsign</param>
        /// <param name="staffID">Logged User</param>
        /// <returns></returns>
        public bool DeleteItemPtVitalSigns(PatientVitalSign entity, long? staffID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        /// <summary>
        /// Add new Patient VitalSign into Patient Common Record info
        /// </summary>
        /// <param name="entity">Patient Common Medical Details Info</param>
        /// <param name="staffID">Logged User</param>
        /// <returns></returns>
        public bool AddItemPtVitalSigns(PatientVitalSign entity, long? staffID)
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
                CleanUpConnectionAndCommand(cn, cmd);
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
        public bool UpdateItemPtVitalSigns(PatientVitalSign entity, long oldPtVSignID, long? staffID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }


        #endregion

        #region 2.Medical Conditions
        public List<RefMedContraIndicationTypes> GetRefMedCondType()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalConditionTypes_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@MCTypeID", SqlDbType.Int);
                par1.Value = 0;
                cn.Open();
                List<RefMedContraIndicationTypes> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefMedicalConditionTypeCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public List<RefMedContraIndicationICD> GetRefMedConditions()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalConditions_ByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@MCTypeID", SqlDbType.Int);
                par1.Value = 0;
                cn.Open();
                List<RefMedContraIndicationICD> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefMedicalConditionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }


        public List<RefMedContraIndicationICD> GetRefMedConditionsByType(int medCondTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalConditions_ByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@MCTypeID", SqlDbType.BigInt);

                par1.Value = medCondTypeID;
                cn.Open();
                List<RefMedContraIndicationICD> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefMedicalConditionCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }

        }
        //Dinh them vao

        public bool DeleteMedReCond(long? MCRecID, long? StaffID, long? CommonMedRecID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch { return false; }
            return true;
        }
        public bool AddNewMedReCond(long? PatientID, long? StaffID
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch { return false; }
            return true;
        }

        public bool UpdateMedReCond(long? MCRecID, long? StaffID, long? CommonMedRecID
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
                    CleanUpConnectionAndCommand(cn, cmd);
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
        public List<MedicalConditionRecord> GetMedConditionByPtID(long patientID, int mcTypeID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        //Manipulate on refMedicalConditionType
        public bool DeleteRefMedCondType(RefMedContraIndicationTypes entity)
        {
            throw new NotImplementedException();
        }

        public bool AddRefMedCondType(RefMedContraIndicationTypes entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRefMedCondType(RefMedContraIndicationTypes entity)
        {
            throw new NotImplementedException();
        }

        //Manipulate on refMedicalConditions
        public bool DeleteRefMedCond(RefMedContraIndicationICD entity)
        {
            throw new NotImplementedException();
        }

        public bool AddRefMedCond(RefMedContraIndicationICD entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRefMedCond(RefMedContraIndicationICD entity)
        {
            throw new NotImplementedException();
        }


        //Manipulate on MedicalConditionRecords
        public bool DeleteMedCondRecs(MedicalConditionRecord entity)
        {
            throw new NotImplementedException();
        }

        public bool AddMedCondRecs(MedicalConditionRecord entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateMedCondRecs(MedicalConditionRecord entity)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region 3.Medical History
        public List<RefMedicalHistory> GetRefMedHistory()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public List<PastMedicalConditionHistory> GetPastMedCondHisByPtID(long patientID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }

        }


        //Manipulate on RefMedicalHistory
        public bool DeleteRefMedicalHistory(RefMedicalHistory entity)
        {
            throw new NotImplementedException();
        }

        public bool AddRefMedicalHistory(RefMedicalHistory entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRefMedicalHistory(RefMedicalHistory entity)
        {
            throw new NotImplementedException();
        }
        //Dinh them

        public bool DeleteMedicalHistory(long? PMHID
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch { return false; }
            return true;
        }
        public bool AddNewMedicalHistory(long? PatientID
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch { return false; }
            return true;
        }
        public bool UpdateMedicalHistory(long? StaffID
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch { return false; }
            return true;
        }

        //Manipulate on PastMedicalConditionHistory
        public bool DeleteMedCondHis(PastMedicalConditionHistory entity)
        {
            throw new NotImplementedException();
        }

        public bool AddMedCondHis(PastMedicalConditionHistory entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateMedCondHis(PastMedicalConditionHistory entity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 4.Immunization

        public List<RefImmunization> GetRefImmunization(long MedServiceID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public List<ImmunizationHistory> GetImmunizationByPtID(long patientID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }


        //Manupulate on RefImmunization
        public bool DeleteImmunization(RefImmunization entity)
        {
            throw new NotImplementedException();
        }

        public bool AddImmunization(RefImmunization entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateImmunization(RefImmunization entity)
        {
            throw new NotImplementedException();
        }


        //Manupulate on ImmunizationHistory
        public bool DeleteImmuHis(ImmunizationHistory entity)
        {
            throw new NotImplementedException();
        }

        public bool AddImmuHis(ImmunizationHistory entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateImmuHis(ImmunizationHistory entity)
        {
            throw new NotImplementedException();
        }

        //Dinh them vao
        public bool DeleteImmunization(long? IHID
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool AddNewImmunization(long? PatientID
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch { return false; }
            return true;
        }
        public bool UpdateImmunization(long? IHID
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch { return false; }
            return true;
        }
        #endregion

        #region 5.Hospitalization
        public List<HospitalizationHistory> GetHospitalizationHistoryByPtID(long patientID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public override Hospital GetHospitalFromReader(IDataReader reader)
        {
            Hospital p = base.GetHospitalFromReader(reader);
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
        public List<Hospital> GetHospitalByKey(string SearchKey, long V_HospitalType, int PageSize, int PageIndex)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return null; }
            return objLst;
        }
        public void HospitalizationHistory_Delete(long? HHID, long? StaffID, long? CommonMedRecID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }

        }
        public void HospitalizationHistory_Update(
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void HospitalizationHistory_Insert(
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 6.Family History
        public List<FamilyHistory> GetFamilyHistoryByPtID(long patientID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        //Manupulate on FamilyHistory
        public bool DeleteFamilyHistory(RefImmunization entity)
        {
            throw new NotImplementedException();
        }

        public bool AddFamilyHistory(RefImmunization entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateFamilyHistory(RefImmunization entity)
        {
            throw new NotImplementedException();
        }
        public void FamilyHistory_Insert(long? PatientID, long? StaffID, long? CommonMedRecID, long? IDCode, string FHFullName, long? V_FamilyRelationship, string FHNotes, bool? Decease, string DiseaseNameVN)
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
                    cmd.AddParameter("@DiseaseNameVN", SqlDbType.NVarChar, ConvertNullObjectToDBNull(DiseaseNameVN));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void FamilyHistory_Update(long? FHCode, long? StaffID, long? CommonMedRecID, long? IDCode, string FHFullName, long? V_FamilyRelationship, string FHNotes, bool? Decease, DateTime? CMRModifiedDate, string DiseaseNameVN)
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
                    cmd.AddParameter("@DiseaseNameVN", SqlDbType.NVarChar, ConvertNullObjectToDBNull(DiseaseNameVN));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void FamilyHistory_Delete(long? StaffID
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 7.Physical Examination

        public void PhysicalExamination_Insert(long? PatientID
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
                CleanUpConnectionAndCommand(cn, cmd);
            }

        }
        //▼====== #001
        public void PhysicalExamination_Insert_V2(long? PatientID, long PtRegistrationID, long V_RegistrationType
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
                cmd.AddParameter("@BustSize", SqlDbType.TinyInt, ConvertNullObjectToDBNull(p.BustSize));
                cmd.AddParameter("@V_HealthyClassification", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.V_HealthyClassification));
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
            }

        }

        public void PhysicalExamination_Update_V2(long? StaffID, PhysicalExamination p)
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
                    cmd.AddParameter("@BustSize", SqlDbType.TinyInt, ConvertNullObjectToDBNull(p.BustSize));
                    cmd.AddParameter("@V_HealthyClassification", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.V_HealthyClassification));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }

        }
        public void PhysicalExamination_InPT_Update(long? StaffID, PhysicalExamination p)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPhysicalExamination_InPT_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PhyExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PhyExamID));
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
                    cmd.AddParameter("@BustSize", SqlDbType.TinyInt, ConvertNullObjectToDBNull(p.BustSize));
                    cmd.AddParameter("@V_HealthyClassification", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.V_HealthyClassification));
                    cmd.AddParameter("@Diet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.Diet));
                    //▼====: #003
                    cmd.AddParameter("@Urine", SqlDbType.Float, ConvertNullObjectToDBNull(p.Urine));
                    //▲====: #003
                    //▼====: #007
                    cmd.AddParameter("@OxygenBreathing", SqlDbType.Bit, ConvertNullObjectToDBNull(p.OxygenBreathing));
                    cmd.AddParameter("@V_ConsciousnessLevel", SqlDbType.Int, ConvertNullObjectToDBNull(p.V_ConsciousnessLevel));
                    cmd.AddParameter("@V_PainLevel", SqlDbType.Int, ConvertNullObjectToDBNull(p.V_PainLevel));
                    //▲====: #007
                    //▼==== #015
                    cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.DeptLocID));
                    //▲==== #015
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }

        }
        //▲====== #001
        public void PhysicalExamination_Insert_InPT(long? PatientID, long PtRegistrationID, long V_RegistrationType, long? StaffID, PhysicalExamination p)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPhysicalExamination_Insert_InPT", cn);
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
                cmd.AddParameter("@BustSize", SqlDbType.TinyInt, ConvertNullObjectToDBNull(p.BustSize));
                cmd.AddParameter("@V_HealthyClassification", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.V_HealthyClassification));
                cmd.AddParameter("@Diet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(p.Diet));
                //▼====: #003
                cmd.AddParameter("@Urine", SqlDbType.Float, ConvertNullObjectToDBNull(p.Urine));
                //▲====: #003
                //▼====: #007
                cmd.AddParameter("@OxygenBreathing", SqlDbType.Bit, ConvertNullObjectToDBNull(p.OxygenBreathing));
                cmd.AddParameter("@V_ConsciousnessLevel", SqlDbType.Int, ConvertNullObjectToDBNull(p.V_ConsciousnessLevel));
                cmd.AddParameter("@V_PainLevel", SqlDbType.Int, ConvertNullObjectToDBNull(p.V_PainLevel));
                //▲====: #007
                //▼==== #015
                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.DeptLocID));
                //▲==== #015
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
            }

        }
        public void PhysicalExamination_Delete(long? StaffID, long? PhyExamID, long? CommonMedRecID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void PhysicalExamination_InPT_Delete(long? PhyExamID, long? CommonMedRecID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPhysicalExamination_InPT_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PhyExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PhyExamID));
                    cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void PhysicalExamination_Update(long? StaffID, long? PhyExamID
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
                    CleanUpConnectionAndCommand(cn, cmd);
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
        public PhysicalExamination PhysicalExamination_GetData(long? CommonMedRecID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }
        public List<PhysicalExamination> PhysicalExamination_ListData(long? CommonMedRecID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return PhysicalObj;
        }
        #endregion

        #region 8. Risk Factors

        public bool RiskFactorInsert(RiskFactors p)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return val > 0;
            }

        }
        public bool RiskFactorDelete(long RiskFactorID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRiskFactorsDelete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RiskFactorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RiskFactorID));

                    cn.Open();
                    var val = cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return val > 0;
                }

            }
            catch
            {
                return false;
            }
        }

        public List<RiskFactors> RiskFactorGet(long? PatientID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }

        public HistoryAndPhysicalExaminationInfo GetHistoryAndPhysicalExaminationInfoByPtRegDetailID(long PtRegDetailID)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spGetHistoryAndPhysicalExaminationInfoByPtRegDetailID", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                    CurrentConnection.Open();
                    IDataReader reader = ExecuteReader(CurrentCommand);
                    var ItemCollection = GetHistoryAndPhysicalExaminationInfoCollectionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    if (ItemCollection == null || ItemCollection.Count != 1)
                    {
                        return null;
                    }
                    return ItemCollection.First();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //▼====: #002
        public HistoryAndPhysicalExaminationInfo GetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID(long OutPtTreatmentProgramID)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spGetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OutPtTreatmentProgramID));
                    CurrentConnection.Open();
                    IDataReader reader = ExecuteReader(CurrentCommand);
                    var ItemCollection = GetHistoryAndPhysicalExaminationInfoCollectionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    if (ItemCollection == null || ItemCollection.Count != 1)
                    {
                        return null;
                    }
                    return ItemCollection.First();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //▲====: #002

        public void EditHistoryAndPhysicalExaminationInfo(HistoryAndPhysicalExaminationInfo aItem, long StaffID)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spEditHistoryAndPhysicalExaminationInfo", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@HistoryAndPhysicalExaminationInfoID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.HistoryAndPhysicalExaminationInfoID));
                    CurrentCommand.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.PtRegDetailID));
                    CurrentCommand.AddParameter("@HistoryAndPhysicalExamination", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.HistoryAndPhysicalExamination));
                    CurrentCommand.AddParameter("@PastMedicalHistory", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PastMedicalHistory));
                    CurrentCommand.AddParameter("@PastMedicalHistoryOfFamily", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PastMedicalHistoryOfFamily));
                    CurrentCommand.AddParameter("@PhysicalExamination", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PhysicalExamination));
                    CurrentCommand.AddParameter("@PhysicalExaminationAllParts", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PhysicalExaminationAllParts));
                    CurrentCommand.AddParameter("@ParaclinicalNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.ParaclinicalNote));
                    CurrentCommand.AddParameter("@MedicalInProcessed", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.MedicalInProcessed));
                    CurrentCommand.AddParameter("@LaboratoryNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.LaboratoryNote));
                    CurrentCommand.AddParameter("@TreatmentMethod", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.TreatmentMethod));
                    CurrentCommand.AddParameter("@DischargeStatus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DischargeStatus));
                    CurrentCommand.AddParameter("@TreatmentSolution", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.TreatmentSolution));
                    CurrentCommand.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.OutPtTreatmentProgramID));
                    CurrentCommand.AddParameter("@ReasonAdmission", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.ReasonAdmission));
                    CurrentCommand.AddParameter("@FirstDiagnostic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.FirstDiagnostic));
                    CurrentCommand.AddParameter("@DischargeDiagnostic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DischargeDiagnostic));
                    CurrentCommand.AddParameter("@PathologicalProcessAndClinicalCourse", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PathologicalProcessAndClinicalCourse));
                    CurrentCommand.AddParameter("@PCLResultsHaveDiagnosticValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PCLResultsHaveDiagnosticValue));
                    CurrentCommand.AddParameter("@DischargeDiagnostic_MainDisease", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DischargeDiagnostic_MainDisease));
                    CurrentCommand.AddParameter("@DischargeDiagnostic_IncludingDisease", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DischargeDiagnostic_IncludingDisease));
                    CurrentCommand.AddParameter("@Treatments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.Treatments));
                    CurrentCommand.AddParameter("@ConditionDischarge", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.ConditionDischarge));
                    CurrentCommand.AddParameter("@DirectionOfTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DirectionOfTreatment));
                    //DatTB 20220307: Thêm chẩn đoán cho phiếu RHM
                    CurrentCommand.AddParameter("@MedicalRecordNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.MedicalRecordNote));
                    CurrentCommand.AddParameter("@DiagnosisOfOutpatientDept", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DiagnosisOfOutpatientDept));
                    CurrentCommand.AddParameter("@ProcessedByDownline", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.ProcessedByDownline));
                    CurrentCommand.AddParameter("@SpecialistDisease", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.SpecialistDisease)); //<====: #004
                    CurrentCommand.AddParameter("@ProgDateTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(aItem.ProgDateTo));
                    CurrentConnection.Open();
                    ExecuteNonQuery(CurrentCommand);
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveOutPtTreatmentProgram(OutPtTreatmentProgram Item)
        {
            try
            {
                //▼==== #008
                if (Item != null)
                {
                    using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                    {
                        SqlCommand CurrentCommand = new SqlCommand("spSaveOutPtTreatmentProgram", CurrentConnection);
                        CurrentCommand.CommandType = CommandType.StoredProcedure;
                        CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.OutPtTreatmentProgramID));
                        CurrentCommand.AddParameter("@TreatmentProgName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.TreatmentProgName));
                        CurrentCommand.AddParameter("@ProgDateFrom", SqlDbType.DateTime, ConvertNullObjectToDBNull(Item.ProgDateFrom));
                        CurrentCommand.AddParameter("@ProgDateTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(Item.ProgDateTo));
                        CurrentCommand.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.DoctorStaffID));
                        CurrentCommand.AddParameter("@CreatorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.CreatorStaffID));
                        CurrentCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.PatientID));
                        //▼==== #005
                        CurrentCommand.AddParameter("@ProgDatePush", SqlDbType.Int, ConvertNullObjectToDBNull(Item.ProgDatePush));
                        CurrentCommand.AddParameter("@ProgDateFinal", SqlDbType.Int, ConvertNullObjectToDBNull(Item.ProgDateFinal));
                        //▲==== #005
                        //▼==== #006
                        CurrentCommand.AddParameter("@OutpatientTreatmentTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.OutpatientTreatmentTypeID));
                        //▲==== #006
                        //▼==== #009
                        CurrentCommand.AddParameter("@ProgDateFinalExpect", SqlDbType.DateTime, ConvertNullObjectToDBNull(Item.ProgDateFinalExpect));
                        //▲==== #009
                        CurrentConnection.Open();
                        ExecuteNonQuery(CurrentCommand);
                        CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    }
                }
                //▲==== #008
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<OutPtTreatmentProgram> GetOutPtTreatmentProgramCollectionByPatientID(long PatientID)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spGetOutPtTreatmentProgramCollectionByPatientID", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    CurrentConnection.Open();
                    var CurrentReader = ExecuteReader(CurrentCommand);
                    List<OutPtTreatmentProgram> ItemCollection = new List<OutPtTreatmentProgram>();
                    while (CurrentReader.Read())
                    {
                        OutPtTreatmentProgram Item = new OutPtTreatmentProgram();
                        Item.FillData(CurrentReader);
                        ItemCollection.Add(Item);
                    }
                    CurrentReader.Close();
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    return ItemCollection;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateTreatmentProgramIntoRegistration(long PtRegistrationID, long PtRegDetailID, long? OutPtTreatmentProgramID, out int OutPrescriptionsAmount)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spUpdateTreatmentProgramIntoRegistration", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    CurrentCommand.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                    CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OutPtTreatmentProgramID));
                    CurrentCommand.AddParameter("@OutPrescriptionsAmount", SqlDbType.Int, 16, ParameterDirection.Output);
                    CurrentConnection.Open();
                    ExecuteNonQuery(CurrentCommand);
                    if (CurrentCommand.Parameters["@OutPrescriptionsAmount"].Value != DBNull.Value)
                    {
                        OutPrescriptionsAmount = (int)CurrentCommand.Parameters["@OutPrescriptionsAmount"].Value;
                    }
                    else
                    {
                        OutPrescriptionsAmount = 0;
                    }
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<TicketCare> GetTicketCareByOutPtTreatmentProgramID(long OutPtTreatmentProgramID)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spGetTicketCareByOutPtTreatmentProgramID", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OutPtTreatmentProgramID));
                    CurrentConnection.Open();
                    IDataReader reader = ExecuteReader(CurrentCommand);
                    var ItemCollection = GetTicketCareListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    if (ItemCollection == null)
                    {
                        return null;
                    }
                    return ItemCollection;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //▼==== #006
        public List<OutpatientTreatmentType> GetAllOutpatientTreatmentType()
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spGetAllOutpatientTreatmentType", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentConnection.Open();
                    var CurrentReader = ExecuteReader(CurrentCommand);
                    List<OutpatientTreatmentType> ItemCollection = new List<OutpatientTreatmentType>();
                    while (CurrentReader.Read())
                    {
                        OutpatientTreatmentType Item = new OutpatientTreatmentType();
                        Item.FillData(CurrentReader);
                        ItemCollection.Add(Item);
                    }
                    CurrentReader.Close();
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    return ItemCollection;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //▲==== #006
        public List<PatientRegistration> GetRegistrationByOutPtTreatmentProgramID(long OutPtTreatmentProgramID, bool IsDischargePapers)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spGetRegistrationByOutPtTreatmentProgramID", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OutPtTreatmentProgramID));
                    CurrentCommand.AddParameter("@IsDischargePapers", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDischargePapers));
                    CurrentConnection.Open();
                    IDataReader reader = ExecuteReader(CurrentCommand);
                    var ItemCollection = GetPatientRegistrationCollectionFromReader(reader);
                    reader.Close();
                    foreach (var item in ItemCollection)
                    {
                        HealthInsurance curHI = null;
                        if (item.HisID.HasValue && item.HisID.Value > 0)
                        {
                            CurrentCommand.CommandText = "spGetHealthInsuranceByHisID";
                            CurrentCommand.Parameters.Clear();
                            CurrentCommand.AddParameter("@HisID", SqlDbType.BigInt, item.HisID.Value);
                            reader = ExecuteReader(CurrentCommand);
                            if (reader != null)
                            {
                                if (reader.Read())
                                {
                                    curHI = GetHealthInsuranceFromReader(reader);
                                    InsuranceBenefit benefit = GetInsuranceBenefitFromReader(reader);
                                    curHI.InsuranceBenefit = benefit;
                                    curHI.HisID = item.HisID;
                                    item.HealthInsurance = curHI;
                                }
                                reader.Close();
                            }
                        }
                    }
                   
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    if (ItemCollection == null)
                    {
                        return null;
                    }
                    return ItemCollection;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SaveOutPtTreatmentProgramItem(PatientRegistration Registration)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spSaveOutPtTreatmentProgramItem", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Registration.PtRegistrationID));
                    CurrentCommand.AddParameter("@PrescriptionsAmount", SqlDbType.Int, ConvertNullObjectToDBNull(Registration.PrescriptionsAmount));
                    CurrentCommand.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Registration.DischargeDate));
                    CurrentCommand.AddParameter("@V_OutDischargeCondition", SqlDbType.BigInt, ConvertNullObjectToDBNull(Registration.V_OutDischargeCondition.LookupID));
                    CurrentCommand.AddParameter("@V_OutDischargeType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Registration.V_OutDischargeType.LookupID));
                    CurrentCommand.AddParameter("@OutPtTreatmentProgramStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Registration.OutPtTreatmentProgramStaffID));
                    CurrentConnection.Open();
                    int res = ExecuteNonQuery(CurrentCommand);
                    
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    return res > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool OutPtTreatmentProgramMarkDeleted(OutPtTreatmentProgram Item)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spOutPtTreatmentProgramMarkDeleted", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.OutPtTreatmentProgramID));
                    CurrentCommand.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(Item.IsDeleted));
                    CurrentCommand.AddParameter("@DeletedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.DeletedStaffID));
                    CurrentCommand.AddParameter("@DeletedReason", SqlDbType.NVarChar , ConvertNullObjectToDBNull(Item.DeletedReason));
                
                    CurrentConnection.Open();
                    int res = ExecuteNonQuery(CurrentCommand);
                    
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    return res > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Staff> GetNarcoticDoctorOfficial(string SearchName, long NarcoticDoctorStaffID, DateTime ProcedureDateTime, bool IsInPt)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spGetNarcoticDoctorOfficial", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@SearchName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchName));
                    CurrentCommand.AddParameter("@NarcoticDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(NarcoticDoctorStaffID));
                    CurrentCommand.AddParameter("@ProcedureDateTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(ProcedureDateTime));
                    CurrentCommand.AddParameter("@PatientFindBy", SqlDbType.Bit, ConvertNullObjectToDBNull(IsInPt));
                 
                    CurrentConnection.Open();
                    IDataReader reader = ExecuteReader(CurrentCommand);
                    var ItemCollection = GetStaffCollectionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    if (ItemCollection == null || ItemCollection.Count == 0)
                    {
                        return null;
                    }
                    return ItemCollection;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //▼===== #010
        public void EditHistoryAndPhysicalExaminationInfo_V2(HistoryAndPhysicalExaminationInfo aItem, long StaffID, bool IsSaveSummary)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand();
                    if (IsSaveSummary)
                    {
                        CurrentCommand = new SqlCommand("spEditHistoryAndPhysicalExaminationInfo_Summary", CurrentConnection);
                        CurrentCommand.CommandType = CommandType.StoredProcedure;
                        CurrentCommand.AddParameter("@HistoryAndPhysicalExaminationInfoID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.HistoryAndPhysicalExaminationInfoID));
                        CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.OutPtTreatmentProgramID));
                        CurrentCommand.AddParameter("@PathologicalProcessAndClinicalCourse", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PathologicalProcessAndClinicalCourse));
                        CurrentCommand.AddParameter("@PCLResultsHaveDiagnosticValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PCLResultsHaveDiagnosticValue));
                        CurrentCommand.AddParameter("@DischargeDiagnostic_MainDisease", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DischargeDiagnostic_MainDisease));
                        CurrentCommand.AddParameter("@DischargeDiagnostic_IncludingDisease", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DischargeDiagnostic_IncludingDisease));
                        CurrentCommand.AddParameter("@Treatments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.Treatments));
                        CurrentCommand.AddParameter("@ConditionDischarge", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.ConditionDischarge));
                        CurrentCommand.AddParameter("@DirectionOfTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DirectionOfTreatment));
                        CurrentCommand.AddParameter("@ProgDateTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(aItem.ProgDateTo));
                        CurrentCommand.AddParameter("@V_OutDischargeType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.V_OutDischargeType));
                        CurrentCommand.AddParameter("@V_OutDischargeCondition", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.V_OutDischargeCondition));
                    }
                    else
                    {
                        CurrentCommand = new SqlCommand("spEditHistoryAndPhysicalExaminationInfo_V2", CurrentConnection);
                        CurrentCommand.CommandType = CommandType.StoredProcedure;
                        CurrentCommand.AddParameter("@HistoryAndPhysicalExaminationInfoID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.HistoryAndPhysicalExaminationInfoID));
                        CurrentCommand.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.PtRegDetailID));
                        CurrentCommand.AddParameter("@HistoryAndPhysicalExamination", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.HistoryAndPhysicalExamination));
                        CurrentCommand.AddParameter("@PastMedicalHistory", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PastMedicalHistory));
                        CurrentCommand.AddParameter("@PastMedicalHistoryOfFamily", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PastMedicalHistoryOfFamily));
                        CurrentCommand.AddParameter("@PhysicalExamination", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PhysicalExamination));
                        CurrentCommand.AddParameter("@PhysicalExaminationAllParts", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.PhysicalExaminationAllParts));
                        CurrentCommand.AddParameter("@ParaclinicalNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.ParaclinicalNote));
                        CurrentCommand.AddParameter("@MedicalInProcessed", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.MedicalInProcessed));
                        CurrentCommand.AddParameter("@LaboratoryNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.LaboratoryNote));
                        CurrentCommand.AddParameter("@TreatmentMethod", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.TreatmentMethod));
                        CurrentCommand.AddParameter("@DischargeStatus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DischargeStatus));
                        CurrentCommand.AddParameter("@TreatmentSolution", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.TreatmentSolution));
                        CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aItem.OutPtTreatmentProgramID));
                        CurrentCommand.AddParameter("@ReasonAdmission", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.ReasonAdmission));
                        CurrentCommand.AddParameter("@FirstDiagnostic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.FirstDiagnostic));
                        CurrentCommand.AddParameter("@DischargeDiagnostic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DischargeDiagnostic));
                        //DatTB 20220307: Thêm chẩn đoán cho phiếu RHM
                        CurrentCommand.AddParameter("@MedicalRecordNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.MedicalRecordNote));
                        CurrentCommand.AddParameter("@DiagnosisOfOutpatientDept", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.DiagnosisOfOutpatientDept));
                        CurrentCommand.AddParameter("@ProcessedByDownline", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.ProcessedByDownline));
                        CurrentCommand.AddParameter("@SpecialistDisease", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aItem.SpecialistDisease)); //<====: #004
                    }
                    CurrentCommand.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    CurrentConnection.Open();
                    ExecuteNonQuery(CurrentCommand);
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //▲===== #010


        //▼==== #011

        public List<InfectionControl> GetInfectionControlByPatientID(long? PatientID, int BacteriaType, long? InPatientAdmDisDetailID)
        {
            List<InfectionControl> obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetInfectionControlByPatientID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    cmd.AddParameter("@BacteriaType", SqlDbType.Int, ConvertNullObjectToDBNull(BacteriaType));
                    cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPatientAdmDisDetailID));

                    cn.Open();


                    IDataReader reader = ExecuteReader(cmd);
                    obj = GetInfectionControlCollectionFromReader(reader);

                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }

        //public InfectionControl GetInfectionControlByPatientID(long? PatientID)
        //{
        //    try
        //    {
        //        using (SqlConnection mConnection = new SqlConnection(ConnectionString))
        //        {
        //            SqlCommand mCommand = new SqlCommand("spGetInfectionControlByPatientID", mConnection);
        //            mCommand.CommandType = CommandType.StoredProcedure;
        //            mCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));

        //            mConnection.Open();
        //            InfectionControl Item = new InfectionControl();
        //            IDataReader reader = ExecuteReader(mCommand);
        //            if (reader != null && reader.Read())
        //            {
        //                Item = GetInfectionControlFromReader(reader);
        //            }
        //            reader.Close();
        //            CleanUpConnectionAndCommand(mConnection, mCommand);
        //            return Item;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public bool SaveInfectionControl(InfectionControl Obj)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spSaveInfectionControl", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@InfectionControlID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.InfectionControlID));
                    mCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PatientID));
                    mCommand.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.InPatientAdmDisDetailID));
                    mCommand.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.StaffID));
                    mCommand.AddParameter("@BacteriaType", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.BacteriaType));
                    mCommand.AddParameter("@DefiniteDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DefiniteDate));
                    mCommand.AddParameter("@BacteriaName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.BacteriaName));
                    mCommand.AddParameter("@V_Bacteria_LOT", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_Bacteria_LOT));
                    mCommand.AddParameter("@BacteriaMeasure", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.BacteriaMeasure));

                    mConnection.Open();
                    int retVal = ExecuteNonQuery(mCommand);
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲==== #011
        //▼==== #012
        public List<SummaryMedicalRecords> GetSummaryMedicalRecordByOutPtTreatmentProgramID(long OutPtTreatmentProgramID)
        {
            try
            {
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spGetSummaryMedicalRecordByOutPtTreatmentProgramID", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@OutPtTreatmentProgramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OutPtTreatmentProgramID));
                    CurrentConnection.Open();
                    IDataReader reader = ExecuteReader(CurrentCommand);
                    var ItemCollection = GetSummaryMedicalRecordsCollectionFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    if (ItemCollection == null)
                    {
                        return null;
                    }
                    return ItemCollection;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //▲==== #012

        //▼==== #013
        public bool SaveDisChargePapersInfo(DischargePapersInfo Item)
        {
            try
            {               
                using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand CurrentCommand = new SqlCommand("spSaveDisChargePapersInfo", CurrentConnection);
                    CurrentCommand.CommandType = CommandType.StoredProcedure;
                    CurrentCommand.AddParameter("@PaperID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.PaperID));
                    CurrentCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.PtRegistrationID));
                    CurrentCommand.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.V_RegistrationType));
                    CurrentCommand.AddParameter("@HeadOfDepartmentDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.HeadOfDepartmentDoctorStaffID));
                    CurrentCommand.AddParameter("@UnitLeaderDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.UnitLeaderDoctorStaffID));
                    CurrentCommand.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.DoctorStaffID));
                    CurrentCommand.AddParameter("@IsPregnancyTermination", SqlDbType.Bit, ConvertNullObjectToDBNull(Item.IsPregnancyTermination));
                    CurrentCommand.AddParameter("@PregnancyTerminationDateTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(Item.PregnancyTerminationDateTime));
                    CurrentCommand.AddParameter("@ReasonOfPregnancyTermination", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.ReasonOfPregnancyTermination));
                    CurrentCommand.AddParameter("@FetalAge", SqlDbType.Int, ConvertNullObjectToDBNull(Item.FetalAge));
                    CurrentCommand.AddParameter("@NumberDayOfLeaveForTreatment", SqlDbType.Int, ConvertNullObjectToDBNull(Item.NumberDayOfLeaveForTreatment));
                    CurrentCommand.AddParameter("@FromDateLeaveForTreatment", SqlDbType.DateTime, ConvertNullObjectToDBNull(Item.FromDateLeaveForTreatment));
                    CurrentCommand.AddParameter("@ToDateLeaveForTreatment", SqlDbType.DateTime, ConvertNullObjectToDBNull(Item.ToDateLeaveForTreatment));
                    CurrentCommand.AddParameter("@DischargeDiagnostic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.DischargeDiagnostic));
                    CurrentCommand.AddParameter("@TreatmentMethod", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.TreatmentMethod));
                    CurrentCommand.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.Notes));
                    CurrentConnection.Open();
                    int res = ExecuteNonQuery(CurrentCommand);
                    CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                    return res > 0;
                }              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DischargePapersInfo GetDischargePapersInfo(long PtRegistrationID, long V_RegistrationType, out string DoctorAdvice)
        {
            DischargePapersInfo obj = null;
            DoctorAdvice = "";
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetGetDischargePapersInfoByPtRegID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                    cmd.AddParameter("@DoctorAdvice", SqlDbType.NVarChar, 1024, ParameterDirection.Output);
                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null && reader.Read())
                    {
                        obj = GetDischargePapersInfoFromReader(reader);
                    }
                    reader.Close();
                    if (cmd.Parameters["@DoctorAdvice"].Value != DBNull.Value)
                    {
                        DoctorAdvice = cmd.Parameters["@DoctorAdvice"].Value.ToString();
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }
        //▲==== #013

        //▼==== #014   
        public bool SaveAgeOfTheArtery(AgeOfTheArtery obj, out long AgeOfTheArteryID)
        {
            bool results = false;
            AgeOfTheArteryID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAgeOfTheArtery_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.Patient.PatientID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.V_RegistrationType));
                cmd.AddParameter("@PatientClassID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.PatientClassID));
                cmd.AddParameter("@Age", SqlDbType.Int, ConvertNullObjectToDBNull(obj.Patient.Age));
                cmd.AddParameter("@AgePoint", SqlDbType.Int, ConvertNullObjectToDBNull(obj.AgePoint));
                cmd.AddParameter("@BloodPressure", SqlDbType.Float, ConvertNullObjectToDBNull(obj.BloodPressure));
                cmd.AddParameter("@IsTreatmentBloodPressure", SqlDbType.Bit, ConvertNullObjectToDBNull(obj.IsTreatmentBloodPressure));
                cmd.AddParameter("@BloodPressureScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.BloodPressureScore));
                cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(obj.HDL));
                cmd.AddParameter("@HDLScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.HDLScore));
                cmd.AddParameter("@Cholesterol", SqlDbType.Float, ConvertNullObjectToDBNull(obj.Cholesterol));
                cmd.AddParameter("@CholesterolScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.CholesterolScore));
                cmd.AddParameter("@IsSmoke", SqlDbType.Bit, ConvertNullObjectToDBNull(obj.IsSmoke));
                cmd.AddParameter("@SmokeScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.SmokeScore));
                cmd.AddParameter("@Diabetes", SqlDbType.Bit, ConvertNullObjectToDBNull(obj.Diabetes));
                cmd.AddParameter("@DiabetesScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.DiabetesScore));
                cmd.AddParameter("@TotalScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.TotalScore));
                cmd.AddParameter("@AgePointOfTheArtery", SqlDbType.Int, ConvertNullObjectToDBNull(obj.AgePointOfTheArtery));
                cmd.AddParameter("@Height", SqlDbType.Float, ConvertNullObjectToDBNull(obj.Height));
                cmd.AddParameter("@Weight", SqlDbType.Float, ConvertNullObjectToDBNull(obj.Weight));
                cmd.AddParameter("@Waist", SqlDbType.Float, ConvertNullObjectToDBNull(obj.Waist));
                cmd.AddParameter("@BMI", SqlDbType.Decimal, ConvertNullObjectToDBNull(obj.BMI));
                cmd.AddParameter("@Diagnosic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(obj.Diagnosic));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.CreatedStaff.StaffID));
                cmd.AddParameter("@id", SqlDbType.BigInt, 64, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);
                AgeOfTheArteryID = (cmd.Parameters["@id"].Value != null ? Convert.ToInt64(cmd.Parameters["@id"].Value) : 0);
                if (AgeOfTheArteryID > 0)
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public bool UpdateAgeOfTheArtery(AgeOfTheArtery obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAgeOfTheArtery_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@AgeOfTheArteryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.AgeOfTheArteryID));
                cmd.AddParameter("@Age", SqlDbType.Int, ConvertNullObjectToDBNull(obj.Patient.Age));
                cmd.AddParameter("@AgePoint", SqlDbType.Int, ConvertNullObjectToDBNull(obj.AgePoint));
                cmd.AddParameter("@BloodPressure", SqlDbType.Float, ConvertNullObjectToDBNull(obj.BloodPressure));
                cmd.AddParameter("@IsTreatmentBloodPressure", SqlDbType.Bit, ConvertNullObjectToDBNull(obj.IsTreatmentBloodPressure));
                cmd.AddParameter("@BloodPressureScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.BloodPressureScore));
                cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(obj.HDL));
                cmd.AddParameter("@HDLScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.HDLScore));
                cmd.AddParameter("@Cholesterol", SqlDbType.Float, ConvertNullObjectToDBNull(obj.Cholesterol));
                cmd.AddParameter("@CholesterolScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.CholesterolScore));
                cmd.AddParameter("@IsSmoke", SqlDbType.Bit, ConvertNullObjectToDBNull(obj.IsSmoke));
                cmd.AddParameter("@SmokeScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.SmokeScore));
                cmd.AddParameter("@Diabetes", SqlDbType.Bit, ConvertNullObjectToDBNull(obj.Diabetes));
                cmd.AddParameter("@DiabetesScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.DiabetesScore));
                cmd.AddParameter("@TotalScore", SqlDbType.Int, ConvertNullObjectToDBNull(obj.TotalScore));
                cmd.AddParameter("@AgePointOfTheArtery", SqlDbType.Int, ConvertNullObjectToDBNull(obj.AgePointOfTheArtery));
                cmd.AddParameter("@Height", SqlDbType.Float, ConvertNullObjectToDBNull(obj.Height));
                cmd.AddParameter("@Weight", SqlDbType.Float, ConvertNullObjectToDBNull(obj.Weight));
                cmd.AddParameter("@Waist", SqlDbType.Float, ConvertNullObjectToDBNull(obj.Waist));
                cmd.AddParameter("@BMI", SqlDbType.Decimal, ConvertNullObjectToDBNull(obj.BMI));
                cmd.AddParameter("@Diagnosic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(obj.Diagnosic));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.LastUpdateStaff.StaffID));

                cn.Open();
                
                int res = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return res > 0;
            }
        }

        public AgeOfTheArtery GetAgeOfTheArtery_ByPatient(long PtRegistrationID, long V_RegistrationType, long PatientClassID)
        {
            AgeOfTheArtery obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAgeOfTheArtery_GetByPatient", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                    cmd.AddParameter("@PatientClassID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientClassID));
                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null && reader.Read())
                    {
                        obj = GetAgeOfTheArteryFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return obj;
        }
        //▲==== #014
    }
    #endregion
}