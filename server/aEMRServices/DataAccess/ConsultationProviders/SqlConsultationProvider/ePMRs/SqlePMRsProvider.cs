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
 * Modified date: 2010-11-24
 * Contents: Consultation Services
/*******************************************************************/
#endregion

using eHCMS.DAL;
using System.Xml;
using System.IO;
/*
 * 20171107 #001 CMN: Added Added IsConfirmEmergencyTreatment into AdmissionInfo
 * 20180923 #002 TBL: BM 0000066. Added out long DTItemID
 * 20181027 #003 TBL: BM 0000130. Added V_TreatmentType
*/
namespace eHCMS.DAL
{
    public class SqlePMRsProvider : ePMRsProvider
    {
        public SqlePMRsProvider()
            : base()
        {

        }

        #region 0.Common
        public override List<Lookup> GetLookupBehaving()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.BEHAVING);
            return objLst;
        }

        public override List<Lookup> GetLookupProcessingType()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.PROCESSING_TYPE);
            return objLst;
        }

        #endregion
        #region Override methods
        #region 1.MedcalRecordTemplate
        public override IList<MedicalRecordTemplate> GetAllMedRecTemplates()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedicalRecordTemplates_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<MedicalRecordTemplate> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetMedRecTemplateCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        #endregion

        #region 2.DiagnosisTreatment
        public override IList<DiagnosisTreatment> GetDiagnosisTreatmentsByServiceRecID(long PatientID, long? ServiceRecID, long? PtRegistrationID, DateTime? ExamDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_ByServiceRecID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@ExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ExamDate));
                cn.Open();
                List<DiagnosisTreatment> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagTrmtCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        public override DiagnosisTreatment GetDiagnosisTreatmentsBySerRecID(long ServiceRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatmentGetByServiceRecID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));

                cn.Open();

                DiagnosisTreatment obj = new DiagnosisTreatment();
                IDataReader reader = ExecuteReader(cmd);
                obj = GetDiagTrmtItemFromReader(reader);
                reader.Close();
                return obj;
            }
        }
        public override IList<DiagnosisTreatment> GetAllDiagnosisTreatments()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<DiagnosisTreatment> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagTrmtCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        public override IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, long? V_Behaving, int PageIndex, int PageSize, out int TotalCount)
        {
            TotalCount = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_ByPtIDPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@NationalMedicalCode", SqlDbType.Char, ConvertNullObjectToDBNull(nationalMedCode));
                cmd.AddParameter("@Opt", SqlDbType.Int, ConvertNullObjectToDBNull(opt));
                cmd.AddParameter("@V_Behaving", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_Behaving));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);
                cn.Open();
                List<DiagnosisTreatment> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagTrmtCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    TotalCount = (int)paramTotal.Value;
                }
                else
                    TotalCount = -1;
                return objLst;
            }
        }


        public override IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID_InPt(long patientID, long? V_Behaving, int PageIndex, int PageSize, out int TotalCount)
        {
            TotalCount = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_ByPtIDPaging_InPt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@V_Behaving", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_Behaving));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);
                cn.Open();
                List<DiagnosisTreatment> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagTrmtCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    TotalCount = (int)paramTotal.Value;
                }
                else
                    TotalCount = -1;
                return objLst;
            }
        }


        public override DiagnosisTreatment GetDiagnosisTreatmentByDTItemID(long DTItemID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_ByDTItemID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTItemID));

                cn.Open();
                DiagnosisTreatment obj = new DiagnosisTreatment();
                IDataReader reader = ExecuteReader(cmd);
                obj = GetDiagTrmtItemFromReader(reader);
                reader.Close();
                return obj;
            }
        }

        public override IList<DiagnosisTreatment> GetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@NationalMedicalCode", SqlDbType.Char, ConvertNullObjectToDBNull(nationalMedCode));
                cmd.AddParameter("@Opt", SqlDbType.Int, ConvertNullObjectToDBNull(opt));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                if (latest)
                    cmd.AddParameter("@LatestRec", SqlDbType.Bit, 1);
                else
                    cmd.AddParameter("@LatestRec", SqlDbType.Bit, 0);

                cn.Open();
                List<DiagnosisTreatment> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagTrmtCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                SqlParameter par2 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                SqlParameter par3 = cmd.Parameters.Add("@NationalMedicalCode", SqlDbType.Char);
                SqlParameter par4 = cmd.Parameters.Add("@Opt", SqlDbType.Int);
                SqlParameter par5 = cmd.Parameters.Add("@LatestRec", SqlDbType.Bit);

                par1.Value = patientID;
                par2.Value = 0;
                par3.Value = "";
                par4.Value = opt;
                if (latest)
                    par5.Value = 1;
                else
                    par5.Value = 0;
                cn.Open();
                DiagnosisTreatment obj = new DiagnosisTreatment();
                IDataReader reader = ExecuteReader(cmd);
                obj = GetDiagTrmtItemFromReader(reader);
                reader.Close();
                return obj;
            }
        }

        
        public override DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID_InPt(long patientID, long? V_DiagnosisType)
        {
            //==== #001
            //try
            //{
            //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            //    {
            //        SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_ByPtID_InPt", cn);
            //        cmd.CommandType = CommandType.StoredProcedure;
            //        cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
            //        cmd.AddParameter("@V_DiagnosisType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_DiagnosisType));

            //        cn.Open();
            //        DiagnosisTreatment obj = new DiagnosisTreatment();
            //        IDataReader reader = ExecuteReader(cmd);
            //        obj = GetDiagTrmtItemFromReader(reader);
            //        reader.Close();
            //        return obj;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            return GetAllDiagnosisTreatment_InPt(patientID, V_DiagnosisType);
            //==== #001
        }
        //==== #001
        private DiagnosisTreatment GetAllDiagnosisTreatment_InPt(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID = 0)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_ByPtID_InPt", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                    cmd.AddParameter("@V_DiagnosisType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_DiagnosisType));
                    cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IntPtDiagDrInstructionID));
                    cn.Open();
                    DiagnosisTreatment obj = new DiagnosisTreatment();
                    IDataReader reader = ExecuteReader(cmd);
                    obj = GetDiagTrmtItemFromReader(reader);
                    reader.Close();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override DiagnosisTreatment GetLatestDiagnosisTreatment_InPtByInstructionID(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID)
        {
            return GetAllDiagnosisTreatment_InPt(patientID, V_DiagnosisType, IntPtDiagDrInstructionID);
        }
        //==== #001

        public override long? spGetPtRegistrationIDInDiagnosisTreatment_Latest(long patientID, long PtRegistrationID)
        {
            long? result = null;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPtRegistrationIDInDiagnosisTreatment_Latest", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                SqlParameter par13 = cmd.Parameters.Add("@PtRegistrationIDLatest", SqlDbType.BigInt);
                par13.Direction = ParameterDirection.Output;
                cn.Open();
                ExecuteNonQuery(cmd);
                if (cmd.Parameters["@PtRegistrationIDLatest"].Value != DBNull.Value)
                {
                    result = cmd.Parameters["@PtRegistrationIDLatest"].Value as long?;
                }
                return result;
            }
        }

        public override bool CheckDiagnosisTreatmentExists_PtRegDetailID(long patientID, long PtRegDetailID)
        {
            int results = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatmentExists_PtRegDetailID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                SqlParameter par13 = cmd.Parameters.Add("@IsExists", SqlDbType.TinyInt);
                par13.Direction = ParameterDirection.Output;
                cn.Open();
                ExecuteNonQuery(cmd);
                if (cmd.Parameters["@IsExists"].Value != DBNull.Value)
                {
                    results = Convert.ToInt32(cmd.Parameters["@IsExists"].Value);
                }
                return results <= 0;
            }
        }

        public override DiagnosisTreatment DiagnosisTreatment_GetLast(long PatientID, long PtRegistrationID, long ServiceRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_GetLast", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                SqlParameter par2 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                SqlParameter par3 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);

                par1.Value = PatientID;
                par2.Value = PtRegistrationID;
                par3.Value = ServiceRecID;

                cn.Open();
                DiagnosisTreatment obj = new DiagnosisTreatment();
                IDataReader reader = ExecuteReader(cmd);
                obj = GetDiagTrmtItemFromReader(reader);
                reader.Close();
                return obj;
            }
        }

        public override DiagnosisTreatment GetDiagnosisTreatment_InPt(long ServiceRecID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetDiagnosisTreatment_InPt", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ServiceRecID);

                    cn.Open();
                    DiagnosisTreatment obj = new DiagnosisTreatment();
                    IDataReader reader = ExecuteReader(cmd);
                    obj = GetDiagTrmtItemFromReader(reader);
                    reader.Close();
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override DiagnosisTreatment GetBlankDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisTreatmentBlank_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                SqlParameter par2 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                SqlParameter par3 = cmd.Parameters.Add("@NationalMedicalCode", SqlDbType.Char);

                par1.Value = patientID;
                par2.Value = PtRegistrationID;
                par3.Value = nationalMedCode;

                cn.Open();
                DiagnosisTreatment obj = new DiagnosisTreatment();
                IDataReader reader = ExecuteReader(cmd);
                obj = GetDiagTrmtItemFromReader(reader);
                reader.Close();
                return obj;
            }
        }

        public override bool DeleteDiagnosisTreatment(DiagnosisTreatment entity)
        {
            throw new NotImplementedException();
        }

        /*▼====: #002*/
        //public override bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServiceRecID)
        public override bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServiceRecID, out long DTItemID)
        /*▲====: #002*/
        {
            entity.PatientServiceRecord.ExamDate = DateTime.Now;

            ServiceRecID = 0;
            /*▼====: #002*/
            DTItemID = 0;
            /*▲====: #002*/
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {

                    SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_Insert", cn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.PatientMedicalRecord.PatientID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.PtRegistrationID));
                    cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.StaffID));
                    cmd.AddParameter("@V_Behaving", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.V_Behaving));
                    cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegDetailID));
                    cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Diagnosis));
                    cmd.AddParameter("@ICD10List", SqlDbType.NChar, ConvertNullObjectToDBNull(entity.ICD10List));
                    cmd.AddParameter("@OrientedTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrientedTreatment));
                    cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DoctorComments));
                    cmd.AddParameter("@DiagnosisFinal", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisFinal));
                    cmd.AddParameter("@MDRptTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MDRptTemplateID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DeptLocationID));
                    if (entity.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                        cmd.AddParameter("@DiagnosisIcd10ListID", SqlDbType.BigInt, 0);
                    else
                        cmd.AddParameter("@DiagnosisIcd10ListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiagnosisIcd10ListID));
                    cmd.AddParameter("@Treatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Treatment));
                    cmd.AddParameter("@V_DiagnosisType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.V_DiagnosisType));
                    cmd.AddParameter("@XML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertDiagnosisIcd10ItemsToXML(XML)));
                    cmd.AddParameter("@ExamDatePSR", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.PatientServiceRecord.ExamDate));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)entity.PatientServiceRecord.V_RegistrationType));
                    cmd.AddParameter("@HeartFailureType", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.HeartFailureType));

                    cmd.AddParameter("@KLSTriGiac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSTriGiac));
                    cmd.AddParameter("@KLSNiemMac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSNiemMac));
                    cmd.AddParameter("@KLSKetMac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSKetMac));
                    cmd.AddParameter("@KLSTuyenGiap", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSTuyenGiap));
                    cmd.AddParameter("@KLSHachBachHuyet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSHachBachHuyet));
                    cmd.AddParameter("@KLSPhoi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSPhoi));
                    cmd.AddParameter("@KLSTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSTim));
                    cmd.AddParameter("@KLSBung", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSBung));
                    cmd.AddParameter("@KLSTMH", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSTMH));
                    /*▼====: #003*/
                    cmd.AddParameter("@V_TreatmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_TreatmentType));
                    /*▲====: #003*/
                    SqlParameter par13 = cmd.Parameters.Add("@ServiceRecID", SqlDbType.BigInt);
                    par13.Direction = ParameterDirection.Output;
                    /*▼====: #002*/
                    SqlParameter par14 = cmd.Parameters.Add("@DTItemID", SqlDbType.BigInt);
                    par14.Direction = ParameterDirection.Output;
                    /*▲====: #002*/
                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    ServiceRecID = (long)cmd.Parameters["@ServiceRecID"].Value;
                    /*▼====: #002*/
                    if (par14.Value != DBNull.Value)
                    {
                        DTItemID = (long)par14.Value;
                    }
                    /*▲====: #002*/
                }
            }
            catch (Exception ex)
            {
                AxLogging.AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }

            /*▼====: #002*/
            //return ServiceRecID > 0;
            return ServiceRecID > 0 && DTItemID > 0;
            /*▲====: #002*/
        }

        public override bool AddDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, out List<InPatientDeptDetail> ReloadInPatientDeptDetails)
        {
            long SmallProcedureID;
            return AddDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, XML, DiagnosisICD9ListID, ICD9XML, out ReloadInPatientDeptDetails, null, out SmallProcedureID);
        }
        public override bool AddDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, SmallProcedure aSmallProcedure, out long SmallProcedureID)
        {
            //entity.PatientServiceRecord.ExamDate = DateTime.Now;
            ReloadInPatientDeptDetails = null;
            bool result = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SmallProcedureID = 0;
                    if (aSmallProcedure != null)
                    {
                        SmallProcedureID = aSmallProcedure.SmallProcedureID;
                        CommonUtilsProvider.Instance.EditSmallProcedure(aSmallProcedure, entity.PatientServiceRecord.StaffID.GetValueOrDefault(0), out SmallProcedureID);
                    }
                    SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_Insert_InPtNew", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.PatientMedicalRecord.PatientID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.PtRegistrationID));
                    cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.StaffID));
                    cmd.AddParameter("@V_Behaving", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.V_Behaving));
                    cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegDetailID));
                    cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Diagnosis));
                    cmd.AddParameter("@ICD10List", SqlDbType.NChar, ConvertNullObjectToDBNull(entity.ICD10List));
                    cmd.AddParameter("@OrientedTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrientedTreatment));
                    cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DoctorComments));
                    cmd.AddParameter("@DiagnosisFinal", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisFinal));
                    cmd.AddParameter("@MDRptTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MDRptTemplateID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DeptLocationID));
                    cmd.AddParameter("@DiagnosisIcd10ListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiagnosisIcd10ListID));
                    cmd.AddParameter("@DiagnosisICD9ListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiagnosisICD9ListID));
                    cmd.AddParameter("@Treatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Treatment));
                    cmd.AddParameter("@V_DiagnosisType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DiagnosisType));
                    cmd.AddParameter("@XML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertDiagnosisIcd10ItemsToXML(XML)));
                    cmd.AddParameter("@ICD9XML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertDiagnosisICD9ItemsToXML(ICD9XML)));
                    cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)entity.PatientServiceRecord.V_RegistrationType));
                    cmd.AddParameter("@DeptIDCreated", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.Department.DeptID));
                    cmd.AddParameter("@HeartFailureType", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.HeartFailureType));
                    //==== #001
                    cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.IntPtDiagDrInstructionID));
                    cmd.AddParameter("@Pulse", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.Pulse));
                    cmd.AddParameter("@BloodPressure", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.BloodPressure));
                    cmd.AddParameter("@Temperature", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.Temperature));
                    cmd.AddParameter("@SpO2", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.SpO2));
                    //==== #001
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        ReloadInPatientDeptDetails = GetInPatientDeptDetailCollectionFromReader(reader);
                    }
                    reader.Close();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                AxLogging.AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
            return result;
        }
        
        public override bool UpdateDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML)
        {
            try
            {
                byte results = 0;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ServiceRecID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.PtRegistrationID));
                    cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.PtRegDetailID));
                    cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                    //TBL: Luu moi thi dung PatientServiceRecord.V_Behaving nhung o day lai dung PatientServiceRecord.LookupBehaving.LookupID
                    //cmd.AddParameter("@V_Behaving", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.LookupBehaving.LookupID));
                    cmd.AddParameter("@V_Behaving", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.V_Behaving));
                    cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Diagnosis));
                    cmd.AddParameter("@ICD10List", SqlDbType.NChar, ConvertNullObjectToDBNull(entity.ICD10List));
                    cmd.AddParameter("@OrientedTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrientedTreatment));
                    cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DoctorComments));
                    cmd.AddParameter("@DiagnosisFinal", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisFinal));
                    cmd.AddParameter("@MDRptTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MDRptTemplateID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DeptLocationID));
                    cmd.AddParameter("@DiagnosisIcd10ListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiagnosisIcd10ListID));
                    cmd.AddParameter("@Treatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Treatment));
                    cmd.AddParameter("@HeartFailureType", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.HeartFailureType));
                    cmd.AddParameter("@V_DiagnosisType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.V_DiagnosisType));
                    cmd.AddParameter("@XML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertDiagnosisIcd10ItemsToXML(XML)));
                    cmd.AddParameter("@KLSTriGiac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSTriGiac));
                    cmd.AddParameter("@KLSNiemMac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSNiemMac));
                    cmd.AddParameter("@KLSKetMac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSKetMac));
                    cmd.AddParameter("@KLSTuyenGiap", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSTuyenGiap));
                    cmd.AddParameter("@KLSHachBachHuyet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSHachBachHuyet));
                    cmd.AddParameter("@KLSPhoi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSPhoi));
                    cmd.AddParameter("@KLSTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSTim));
                    cmd.AddParameter("@KLSBung", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSBung));
                    cmd.AddParameter("@KLSTMH", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KLSTMH));
                    /*▼====: #003*/
                    cmd.AddParameter("@V_TreatmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_TreatmentType));
                    /*▲====: #003*/
                    SqlParameter par13 = cmd.Parameters.Add("@ErrorID", SqlDbType.TinyInt);
                    par13.Direction = ParameterDirection.Output;
                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    results = (byte)cmd.Parameters["@ErrorID"].Value;
                    return results == 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override bool UpdateDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML)
        {
            return UpdateDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, out ReloadInPatientDeptDetails, XML, DiagnosisICD9ListID, ICD9XML, null);
        }
        public override bool UpdateDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, SmallProcedure aSmallProcedure)
        {
            try
            {
                byte results = 0;
                ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    long SmallProcedureID = 0;
                    if (aSmallProcedure != null)
                    {
                        SmallProcedureID = aSmallProcedure.SmallProcedureID;
                        CommonUtilsProvider.Instance.EditSmallProcedure(aSmallProcedure, entity.PatientServiceRecord.StaffID.GetValueOrDefault(0), out SmallProcedureID);
                    }
                    SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_Update_InPt", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@DTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DTItemID));
                    cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ServiceRecID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.PtRegistrationID));
                    cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                    cmd.AddParameter("@V_Behaving", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.LookupBehaving.LookupID));
                    cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Diagnosis));
                    cmd.AddParameter("@ICD10List", SqlDbType.NChar, ConvertNullObjectToDBNull(entity.ICD10List));
                    cmd.AddParameter("@OrientedTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrientedTreatment));
                    cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DoctorComments));
                    cmd.AddParameter("@DiagnosisFinal", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisFinal));
                    cmd.AddParameter("@MDRptTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MDRptTemplateID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DeptLocationID));
                    cmd.AddParameter("@DiagnosisIcd10ListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiagnosisIcd10ListID));
                    cmd.AddParameter("@DiagnosisICD9ListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiagnosisICD9ListID));
                    cmd.AddParameter("@Treatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Treatment));
                    cmd.AddParameter("@HeartFailureType", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.HeartFailureType));
                    // Hpt 06/11/2015: vì khi cập nhật chẩn đoán có thể đổi khoa, dẫn đến thay đổi Guid của chẩn đoán nên phải truyền thêm parameter Guid xuống stored để thực hiện các kiểm tra và cập nhật lại
                    cmd.AddParameter("@InPtDeptGuid", SqlDbType.UniqueIdentifier, ConvertNullObjectToDBNull(entity.InPtDeptGuid));
                    //KMx: Dời V_DiagnosisType từ PatientServiceRecord sang DiagnosisTreatment (09/06/2015 17:18).
                    //cmd.AddParameter("@V_DiagnosisType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.V_DiagnosisType));
                    cmd.AddParameter("@V_DiagnosisType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DiagnosisType));
                    cmd.AddParameter("@DeptIDCreated", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.Department.DeptID));
                    cmd.AddParameter("@XML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertDiagnosisIcd10ItemsToXML(XML)));
                    cmd.AddParameter("@ICD9XML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertDiagnosisICD9ItemsToXML(ICD9XML)));
                    SqlParameter par13 = cmd.Parameters.Add("@ErrorID", SqlDbType.TinyInt);
                    par13.Direction = ParameterDirection.Output;
                    cn.Open();
                    //int retVal = ExecuteNonQuery(cmd);
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        ReloadInPatientDeptDetails = GetInPatientDeptDetailCollectionFromReader(reader);
                    } 
                    results = Convert.ToByte(cmd.Parameters["@ErrorID"].Value);
                    reader.Close();
                    return results == 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load_InPt(long DTItemID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisIcd10Items_Load_InPt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //KMx: Lấy IDC10 theo chẩn đoán, không lấy theo service record vì sửa lại 1 service record có thể có nhiều chẩn đoán (09/06/2015 17:20).
                //cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                cmd.AddParameter("@DTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTItemID));

                cn.Open();
                List<DiagnosisIcd10Items> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagnosisIcd10ItemsCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override IList<DiagnosisICD9Items> GetDiagnosisICD9Items_Load_InPt(long DTItemID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisICD9Items_Load_InPt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTItemID));

                cn.Open();
                List<DiagnosisICD9Items> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagnosisICD9ItemsCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        public override IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load(long? ServiceRecID, long? PatientID, bool Last)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisIcd10Items_Load", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                if (Last)
                    cmd.AddParameter("@Last", SqlDbType.Bit, 1);
                else
                    cmd.AddParameter("@Last", SqlDbType.Bit, 0);
                cn.Open();
                List<DiagnosisIcd10Items> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagnosisIcd10ItemsCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        #endregion

        #region 3.PatientMedicalRecords
        public override bool DeletePMR(PatientMedicalRecord entity)
        {
            throw new NotImplementedException();
        }

        public override bool AddPMR(long ptID, string ptRecBarCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalRecords_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptID));
                cmd.AddParameter("@PatientRecBarCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ptRecBarCode));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool UpdatePMR(PatientMedicalRecord entity)
        {
            throw new NotImplementedException();
        }

        public override IList<PatientMedicalRecord> GetPMRsByPtID(long? patientID, int? inclExpiredPMR)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalRecords_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@IncludeExpiredPMR", SqlDbType.Int, ConvertNullObjectToDBNull(inclExpiredPMR));

                cn.Open();
                List<PatientMedicalRecord> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPMRsCollectionFromReader(reader);
                reader.Close();
                return objLst;
            }

        }

        public override PatientMedicalRecord GetPMRByPtID(long? patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalRecords_ByPtID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@IncludeExpiredPMR", SqlDbType.Int, ConvertNullObjectToDBNull(1));//1: Only retrieve openning PMR

                cn.Open();
                PatientMedicalRecord obj = null;
                IDataReader reader = ExecuteReader(cmd);
                obj = GetPMRItemFromReader(reader);
                reader.Close();
                return obj;
            }
        }

        public override PatientMedicalRecord PatientMedicalRecords_ByPatientID(long patientID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalRecords_ByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));

                cn.Open();
                PatientMedicalRecord obj = null;
                IDataReader reader = ExecuteReader(cmd);
                obj = GetPMRItemFromReader(reader);
                reader.Close();
                return obj;
            }
        }



        public override bool PatientMedicalRecords_Save(long PatientRecID, long PatientID, string NationalMedicalCode, out string Error)
        {
            bool Result = false;
            Error = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalRecords_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientRecID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@NationalMedicalCode", SqlDbType.Char, ConvertNullObjectToDBNull(NationalMedicalCode));

                cmd.AddParameter("@Error", SqlDbType.NVarChar, 200, ParameterDirection.Output);
                cmd.AddParameter("@Result", SqlDbType.Bit, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Error"].Value != null)
                    Error = cmd.Parameters["@Error"].Value.ToString();

                if (cmd.Parameters["@Result"].Value != null)
                    Result = Convert.ToBoolean(cmd.Parameters["@Result"].Value);

                return Result;
            }
        }


        #endregion
        #endregion
        #region thao tac tren file
        public override PatientMedicalFile PatientMedicalFiles_ByID(long ServiceRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_CheckExists", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                cn.Open();
                PatientMedicalFile obj = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    obj = GetPatientMedicalFileFromReader(reader);
                }
                reader.Close();
                return obj;
            }
        }

        public override IList<PatientMedicalFile> PatientMedicalFiles_ByPatientRecID(long PatientRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_ByPatientRecID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientRecID));
                cn.Open();
                IList<PatientMedicalFile> obj = null;
                IDataReader reader = ExecuteReader(cmd);
                obj = GetPatientMedicalFileCollectionFromReader(reader);
                reader.Close();
                return obj;
            }
        }

        public override bool CheckExists_PatientMedicalFiles(long ServiceRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_CheckExists", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    reader.Close();
                    return true;
                }
                else
                {
                    reader.Close();
                    return false;
                }
            }
        }

        public override bool Insert_PatientMedicalFiles(PatientMedicalFile entity, out long PatientRecID)
        {
            try
            {
                PatientRecID = 0;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientID));
                    cmd.AddParameter("@PatientRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientRecID));
                    cmd.AddParameter("@FileCodeNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.FileCodeNumber));
                    cmd.AddParameter("@FileBarcodeNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.FileBarcodeNumber));
                    cmd.AddParameter("@StorageFilePath", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.StorageFilePath));
                    cmd.AddParameter("@StorageFileName", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.StorageFileName));
                    cmd.AddParameter("@PatientRecIDTemp", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    if (retVal > 0)
                    {
                        if (cmd.Parameters["@PatientRecIDTemp"].Value != DBNull.Value)
                        {
                            PatientRecID = (long)cmd.Parameters["@PatientRecIDTemp"].Value;
                        }
                    }
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                AxLogging.AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
        }

        public override bool PatientMedicalFiles_Update(PatientMedicalFile entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientMedicalFileID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientMedicalFileID));
                    cmd.AddParameter("@FileCodeNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.FileCodeNumber));
                    cmd.AddParameter("@FileBarcodeNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.FileBarcodeNumber));
                    cmd.AddParameter("@StorageFilePath", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.StorageFilePath));
                    cmd.AddParameter("@StorageFileName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.StorageFileName));

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                AxLogging.AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
        }

        public override bool PatientMedicalFiles_Delete(PatientMedicalFile entity, long staffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientMedicalFileID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientMedicalFileID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(staffID));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool PatientMedicalFiles_Active(PatientMedicalFile entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_Active", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientMedicalFileID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientMedicalFileID));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool WriteFileXML(string path, string FileName, List<StringXml> Contents)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlNode xmlRoot;
            XmlElement xmlNode;
            xmlRoot = xmldoc.CreateElement("Root");
            xmldoc.AppendChild(xmlRoot);
            if (Contents.Count > 0)
            {
                foreach (StringXml item in Contents)
                {
                    xmlNode = xmldoc.CreateElement("Page");
                    XmlAttribute newcatalogattr = xmldoc.CreateAttribute("ID");

                    // Value given for the new attribute
                    newcatalogattr.Value = item.Name;

                    // Attach the attribute to the XML element
                    xmlNode.SetAttributeNode(newcatalogattr);

                    xmlRoot.AppendChild(xmlNode);

                    xmlNode.InnerText = item.Content;
                }
            }
            xmldoc.Save(path + "/" + FileName);
            return true;
        }

        public override List<StringXml> ReadFileXML(string FullPath)
        {
            List<StringXml> lst = new List<StringXml>();
            XmlDocument xml = new XmlDocument();
            xml.Load(FullPath); // suppose that myXmlString contains "<Names>...</Names>"

            XmlNodeList xnList = xml.SelectNodes("/Root/Page");
            foreach (XmlNode xn in xnList)
            {
                XmlAttributeCollection attCol = xn.Attributes;
                lst.Add(new StringXml { Name = attCol[0].Value, Content = xn.InnerXml });
            }
            return lst;

        }
        #endregion


        //Dinh them
        #region CLS

        public override IList<PatientServiceRecord> GetAllPatientServiceRecord(long patientID)
        {
            List<PatientServiceRecord> listRs = new List<PatientServiceRecord>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientServiceRecordsGetByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetPatientServiceRecordCollectionFromReader(reader);
                reader.Close();
            }
            return listRs;
        }
        public override IList<PatientPCLRequest> GetAllPatientPCLReqServiceRecID(long ServiceRecID, long V_PCLMainCategory)
        {
            List<PatientPCLRequest> listRs = new List<PatientPCLRequest>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLReqServiceRecID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLMainCategory));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetPatientPCLRequestCollectionFromReader(reader);
                reader.Close();
            }
            return listRs;
        }

        public override InPatientAdmDisDetails GetInPatientAdmDisDetails(long PtRegistrationID)
        {
            InPatientAdmDisDetails Rs = new InPatientAdmDisDetails();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInPatientAdmDisDetails_GetByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    Rs = GetInPatientAdmissionFromReader(reader);
                }

                reader.Close();
            }
            return Rs;
        }

        public override bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInPatientAdmDisDetails_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.InPatientAdmDisDetailID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegistrationID));
                cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.AdmissionDate));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DeptID));
                cmd.AddParameter("@V_AdmissionType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_AdmissionType));
                cmd.AddParameter("@AdmissionNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AdmissionNote));
                cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.DischargeDate));
                cmd.AddParameter("@V_DischargeType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DischargeType));
                cmd.AddParameter("@DischargeNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DischargeNote));
                /*▼====: #001*/
                cmd.AddParameter("@IsConfirmEmergencyTreatment", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsConfirmEmergencyTreatment));
                /*▲====: #001*/
                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        public override bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInPatientAdmDisDetails_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@InPatientAdmDisDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.InPatientAdmDisDetailID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegistrationID));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DeptID));
                cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.AdmissionDate));
                cmd.AddParameter("@V_AdmissionType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_AdmissionType));
                cmd.AddParameter("@AdmissionNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AdmissionNote));
                cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.DischargeDate));
                cmd.AddParameter("@V_DischargeType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DischargeType));
                cmd.AddParameter("@DischargeNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DischargeNote));
                /*▼====: #001*/
                cmd.AddParameter("@IsConfirmEmergencyTreatment", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsConfirmEmergencyTreatment));
                /*▲====: #001*/
                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        #endregion

        //HPT 08/02/2017: Giấy chuyển tuyến
        public override TransferForm SaveTransferForm(TransferForm entity)
        {
            try
            {
                TransferForm Result = new TransferForm();
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSaveTransferForm", cn);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TransferFormID));
                    cmd.AddParameter("@TransferNum", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TransferNum));
                    cmd.AddParameter("@SavingNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SavingNumber));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CurPatientRegistration.PtRegistrationID));
                    cmd.AddParameter("@FromHosID", SqlDbType.BigInt, entity.TransferFromHos != null ? ConvertNullObjectToDBNull(entity.TransferFromHos.HosID) : 0);
                    cmd.AddParameter("@ToHosID", SqlDbType.BigInt, entity.TransferToHos != null ? ConvertNullObjectToDBNull(entity.TransferToHos.HosID) : 0);
                    cmd.AddParameter("@FromDate", SqlDbType.Date, ConvertNullObjectToDBNull(entity.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.Date, ConvertNullObjectToDBNull(entity.ToDate));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, entity.TransferFromDept != null ? ConvertNullObjectToDBNull(entity.TransferFromDept.DeptID) : 0);
                    cmd.AddParameter("@ClinicalSign", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ClinicalSign));
                    cmd.AddParameter("@PCLResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PCLResult));
                    cmd.AddParameter("@UsedServicesAndItems", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.UsedServicesAndItems));
                    cmd.AddParameter("@DiagnosisTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisTreatment));
                    cmd.AddParameter("@ICD10", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ICD10));
                    cmd.AddParameter("@DiagnosisTreatment_Final", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisTreatment_Final));
                    cmd.AddParameter("@ICD10Final", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ICD10Final));
                    cmd.AddParameter("@V_TransferTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_TransferTypeID));
                    cmd.AddParameter("@TransferType", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TransferType));
                    cmd.AddParameter("@V_TreatmentResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_TreatmentResultID));
                    cmd.AddParameter("@TreatmentResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TreatmentResult));
                    cmd.AddParameter("@V_TransferReasonID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_TransferReasonID));
                    cmd.AddParameter("@TransferReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TransferReason)    );
                    cmd.AddParameter("@V_CMKTID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_CMKTID));
                    cmd.AddParameter("@CMKT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.CMKT));
                    cmd.AddParameter("@V_PatientStatusID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_PatientStatusID));
                    cmd.AddParameter("@PatientStatus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PatientStatus));
                    cmd.AddParameter("@TransferDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.TransferDate));
                    cmd.AddParameter("@TransVehicle", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TransVehicle));
                    cmd.AddParameter("@TransferBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TransferBy));
                    cmd.AddParameter("@TreatmentPlan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TreatmentPlan));
                    cmd.AddParameter("@IsExistsError", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsExistsError));
                    cmd.AddParameter("@IsDiffDiagnosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsDiffDiagnosis));
                    cmd.AddParameter("@V_TransferFormType", SqlDbType.Int, ConvertNullObjectToDBNull(entity.V_TransferFormType));
                    cmd.AddParameter("@PatientFindBy", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PatientFindBy));
                    cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Note));
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null && reader.Read())
                    {
                        Result = GetTransferFormFromReader(reader);
                    }
                    reader.Close();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override IList<TransferForm> GetTransferForm(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID)
        {
            try
            {
                IList<TransferForm> Result = null;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetTransferFormByCriterial", cn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@TextCriterial", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TextCriterial));
                    cmd.AddParameter("@FindBy", SqlDbType.Int, ConvertNullObjectToDBNull(FindBy));
                    cmd.AddParameter("@V_TransferFormType", SqlDbType.Int, ConvertNullObjectToDBNull(V_TransferFormType));
                    cmd.AddParameter("@PatientFindBy", SqlDbType.Int, ConvertNullObjectToDBNull(V_PatientFindBy));
                    cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TransferFormID));/*TMA*/
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        Result = GetTransferFormCollectionFromReader(reader);
                    }
                    reader.Close();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*TMA 20/11/2017*/
        public override IList<TransferForm> GetTransferForm_Date(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                IList<TransferForm> Result = null;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetTransferFormByCriterial_Date", cn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@TextCriterial", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TextCriterial));
                    cmd.AddParameter("@FindBy", SqlDbType.Int, ConvertNullObjectToDBNull(FindBy));
                    cmd.AddParameter("@V_TransferFormType", SqlDbType.Int, ConvertNullObjectToDBNull(V_TransferFormType));
                    cmd.AddParameter("@PatientFindBy", SqlDbType.Int, ConvertNullObjectToDBNull(V_PatientFindBy));
                    cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TransferFormID));
                    cmd.AddParameter("@FromDate", SqlDbType.Date, ConvertNullObjectToDBNull(FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.Date, ConvertNullObjectToDBNull(ToDate));
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        Result = GetTransferFormCollectionFromReader(reader);
                    }
                    reader.Close();
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override bool DeleteTransferForm(long TransferFormID, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteTransferForm", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, TransferFormID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                cn.Open();
                return ExecuteNonQuery(cmd) > 0;
            }
        }
    }
}

