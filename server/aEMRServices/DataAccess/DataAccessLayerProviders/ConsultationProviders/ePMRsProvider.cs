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
using System.Xml;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-11-23
 * Contents: Consultation Services
/*******************************************************************/
#endregion
/*
 * 20180923 #001 TBL:    BM 0000066. Added out long DTItemID for AddDiagnosisTreatment
 * 20190929 #002 TTM:    BM 0017381: [aEMR] Y lệnh: Bổ sung huyết áp dưới, bổ sung chức năng cập nhật dấu hiệu sinh tồn khi cập nhật y lệnh.
 * 20191006 #003 TTM:    BM 0017421: [Ra toa] Thêm tên thư ký y khoa thực hiện toa 
 * 20191010 #004 TTM:    BM 0017443: [Kiểm soát nhiễm khuẩn]: Bổ sung màn hình hội chẩn.
 * 20210422 #005 TNHX:  Thêm chẩn đoán khác
 * 20210428 #006 TNHX:  Lấy danh sách ICD sao kèm theo
 * 20210611 #007 TNHX:  Lấy danh sách ICD quy tắc
 * 20210701 #008 TNHX:  260 Lưu thông tin bsi mượn user
 * 20210809 #009 BLQ:  Thêm trường lý do vào ngoại trú
 * 20220309 #010 QTD:   Thêm trường Cho điều trị tại khoa, Ghi chú, Lý do vào viện
 * 20220602 #011 DatTB: IssueID: 1619 | [CSKH] Thêm màn hình nhập kết quả cho KSK
 * 20220622 #012 QTD:   Thêm biến theo dõi bệnh nặng khi tạo y lệnh nội trú
 * 20220725 #013 DatTB: Thêm thông tin nhịp thở RespiratoryRate vào DiagnosisTreatment_InPt.
 * 20230102 #014 BLQ: Thêm thông tin tờ tự khai
 * 20230307 #015 BLQ: Thêm định mức bàn khám 
 * 20230331 #016 BLQ: Thêm trường giai đoạn bệnh
 * 20230330 #016 QTD: 
    + Lưu thêm ICD9 cho ngoại trú
    + Dữ liệu 130
 * 20230502 #017 QTD: Thêm trương cho Phiếu sơ kết 15 ngày
* 20230517 #018 DatTB: Chỉnh sửa service xóa giấy chuyển tuyến
* 20230518 #019 DatTB: Chỉnh service lưu thêm người thêm,sửa giấy chuyển tuyến
* 20230527 #020 DatTB: Thêm service xóa kết quả KSK
 */
namespace aEMR.DataAccessLayer.Providers
{
    public class ePMRsProvider : DataProviderBase
    {
        static private ePMRsProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public ePMRsProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ePMRsProvider();
                }
                return _instance;
            }
        }
        public ePMRsProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }
        protected new List<PatientServiceRecord> GetPatientServiceRecordCollectionFromReader(IDataReader reader)
        {
            List<PatientServiceRecord> lst = new List<PatientServiceRecord>();
            while (reader.Read())
            {
                lst.Add(GetPatientServiceRecordObjFromReader(reader));
            }
            reader.Close();
            return lst;
        }
        protected virtual PatientServiceRecord GetPatientServiceRecordObjFromReader(IDataReader reader)
        {
            PatientServiceRecord p = new PatientServiceRecord();
            try
            {

                if (reader.HasColumn("ServiceRecID") && reader["ServiceRecID"] != DBNull.Value)
                {
                    p.ServiceRecID = (long)(reader["ServiceRecID"]);
                }

                if (reader.HasColumn("PtRegistrationID") && reader["PtRegistrationID"] != DBNull.Value)
                {
                    p.PtRegistrationID = (long)(reader["PtRegistrationID"]);
                }

                if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                {
                    p.StaffID = (long)(reader["StaffID"]);
                }

                if (reader.HasColumn("PatientRecID") && reader["PatientRecID"] != DBNull.Value)
                {
                    p.PatientRecID = (long)(reader["PatientRecID"]);
                }

                if (reader.HasColumn("ExamDate") && reader["ExamDate"] != DBNull.Value)
                {
                    p.ExamDate = (DateTime)(reader["ExamDate"]);
                }

                if (reader.HasColumn("V_ProcessingType") && reader["V_ProcessingType"] != DBNull.Value)
                {
                    p.V_ProcessingType = (long)(reader["V_ProcessingType"]);
                }

                if (reader.HasColumn("V_Behaving") && reader["V_Behaving"] != DBNull.Value)
                {
                    p.V_Behaving = (long)(reader["V_Behaving"]);
                }

                //if (reader.HasColumn("DateModified") && reader["DateModified"] != DBNull.Value)
                //{
                //    p.DateModified = Convert.ToInt32(reader["DateModified"]);
                //}
            }
            catch
            { return null; }
            return p;

        }
        protected new List<PatientPCLRequest> GetPatientPCLRequestCollectionFromReader(IDataReader reader)
        {
            List<PatientPCLRequest> lst = new List<PatientPCLRequest>();
            while (reader.Read())
            {
                lst.Add(GetPatientPCLRequestObjFromReader(reader));
            }
            reader.Close();
            return lst;
        }
        protected virtual PatientPCLRequest GetPatientPCLRequestObjFromReader(IDataReader reader)
        {
            PatientPCLRequest p = new PatientPCLRequest();
            try
            {

                if (reader.HasColumn("PatientPCLReqID") && reader["PatientPCLReqID"] != DBNull.Value)
                {
                    p.PatientPCLReqID = (long)reader["PatientPCLReqID"];

                }


                if (reader.HasColumn("ServiceRecID") && reader["ServiceRecID"] != DBNull.Value)
                {
                    p.ServiceRecID = (long)reader["ServiceRecID"];

                }


                if (reader.HasColumn("ReqFromDeptLocID") && reader["ReqFromDeptLocID"] != DBNull.Value)
                {
                    p.ReqFromDeptLocID = (long)reader["ReqFromDeptLocID"];

                }


                if (reader.HasColumn("PCLRequestNumID") && reader["PCLRequestNumID"] != DBNull.Value)
                {
                    p.PCLRequestNumID = (string)reader["PCLRequestNumID"];

                }


                if (reader.HasColumn("TestSampleID") && reader["TestSampleID"] != DBNull.Value)
                {
                    //p.TestSampleID = (string)reader["TestSampleID"];

                }


                if (reader.HasColumn("Diagnosis") && reader["Diagnosis"] != DBNull.Value)
                {
                    p.Diagnosis = (string)reader["Diagnosis"];

                }


                if (reader.HasColumn("DoctorComments") && reader["DoctorComments"] != DBNull.Value)
                {
                    p.DoctorComments = (string)reader["DoctorComments"];

                }


                if (reader.HasColumn("IsExternalExam") && reader["IsExternalExam"] != DBNull.Value)
                {
                    p.IsExternalExam = (bool)reader["IsExternalExam"];

                }


                if (reader.HasColumn("IsImported") && reader["IsImported"] != DBNull.Value)
                {
                    p.IsImported = (bool)reader["IsImported"];

                }


                if (reader.HasColumn("IsCaseOfEmergency") && reader["IsCaseOfEmergency"] != DBNull.Value)
                {
                    p.IsCaseOfEmergency = (bool)reader["IsCaseOfEmergency"];

                }


                if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                {
                    p.StaffID = (long)reader["StaffID"];

                }


                if (reader.HasColumn("MarkedAsDeleted") && reader["MarkedAsDeleted"] != DBNull.Value)
                {
                    p.MarkedAsDeleted = (bool)reader["MarkedAsDeleted"];

                }

                if (reader.HasColumn("V_PCLRequestType") && reader["V_PCLRequestType"] != DBNull.Value)
                {
                    p.V_PCLRequestType = (AllLookupValues.V_PCLRequestType)reader["V_PCLRequestType"];

                }


                if (reader.HasColumn("V_PCLRequestStatus") && reader["V_PCLRequestStatus"] != DBNull.Value)
                {
                    p.V_PCLRequestStatus = (AllLookupValues.V_PCLRequestStatus)reader["V_PCLRequestStatus"];

                }


                if (reader.HasColumn("PaidTime") && reader["PaidTime"] != DBNull.Value)
                {
                    p.PaidTime = (Nullable<DateTime>)reader["PaidTime"];

                }


                if (reader.HasColumn("RefundTime") && reader["RefundTime"] != DBNull.Value)
                {
                    p.RefundTime = (Nullable<DateTime>)reader["RefundTime"];

                }


                if (reader.HasColumn("CreatedDate") && reader["CreatedDate"] != DBNull.Value)
                {
                    p.CreatedDate = (DateTime)reader["CreatedDate"];

                }


                if (reader.HasColumn("AgencyID") && reader["AgencyID"] != DBNull.Value)
                {
                    p.AgencyID = (long)reader["AgencyID"];

                }


                if (reader.HasColumn("InPatientBillingInvID") && reader["InPatientBillingInvID"] != DBNull.Value)
                {
                    p.InPatientBillingInvID = (long)reader["InPatientBillingInvID"];

                }

                //Main
                if (reader.HasColumn("V_PCLMainCategory") && reader["V_PCLMainCategory"] != DBNull.Value)
                {
                    p.V_PCLMainCategory = Convert.ToInt64(reader["V_PCLMainCategory"]);
                }
                else
                {
                    p.V_PCLMainCategory = 0;
                }

                p.ObjV_PCLMainCategory = new Lookup();
                try
                {
                    p.ObjV_PCLMainCategory.LookupID = p.V_PCLMainCategory;
                    if (reader.HasColumn("V_PCLMainCategoryName"))
                    {
                        p.ObjV_PCLMainCategory.ObjectValue = reader["V_PCLMainCategoryName"] == null ? "" : reader["V_PCLMainCategoryName"].ToString().Trim();
                    }
                }
                catch
                {
                }
                //Main
                if (reader.HasColumn("PCLResultParamImpID") && reader["PCLResultParamImpID"] != DBNull.Value)
                {
                    p.PCLResultParamImpID = reader["PCLResultParamImpID"] as long?;
                }

                p.ObjPCLResultParamImpID = new PCLResultParamImplementations();
                try
                {
                    p.ObjPCLResultParamImpID = GetPCLResultParamImplementationsFromReader(reader);
                }
                catch
                {

                }

                if (reader.HasColumn("PCLExamTypeName") && reader["PCLExamTypeName"] != DBNull.Value)
                {
                    p.PCLExamTypeName = reader["PCLExamTypeName"].ToString();
                }
            }
            catch
            { return null; }
            return p;

        }
        protected new List<InPatientAdmDisDetails> GetInPatientAdmDisDetailsCollectionFromReader(IDataReader reader)
        {
            List<InPatientAdmDisDetails> lst = new List<InPatientAdmDisDetails>();
            while (reader.Read())
            {
                lst.Add(GetInPatientAdmissionFromReader(reader));
            }
            reader.Close();
            return lst;
        }
        public List<Lookup> GetLookupBehaving()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.BEHAVING);
            return objLst;
        }

        public List<Lookup> GetLookupProcessingType()
        {
            List<Lookup> objLst = null;
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.PROCESSING_TYPE);
            return objLst;
        }
        #region  methods
        #region 1.MedcalRecordTemplate
        public IList<MedicalRecordTemplate> GetAllMedRecTemplates()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        #endregion

        #region 2.DiagnosisTreatment
        public IList<DiagnosisTreatment> GetDiagnosisTreatmentsByServiceRecID(long PatientID, long? ServiceRecID, long? PtRegistrationID, DateTime? ExamDate)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public IList<DiagnosisTreatment> GetDiagnosisTreatment_InPt_ByPtRegID(long PtRegistrationID, long? DTItemID, long? DeptID)
        {
            using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spGetDiagnosisTreatment_InPt_ByPtRegID", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                mCommand.AddParameter("@DTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTItemID));
                mCommand.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                mConnection.Open();
                List<DiagnosisTreatment> mCollection = null;
                IDataReader reader = ExecuteReader(mCommand);
                mCollection = GetDiagTrmtCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return mCollection;
            }
        }
        public DiagnosisTreatment GetDiagnosisTreatmentsBySerRecID(long ServiceRecID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }
        public IList<DiagnosisTreatment> GetAllDiagnosisTreatments()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, long? V_Behaving, int PageIndex, int PageSize, out int TotalCount)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }


        public IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID_InPt(long patientID, long? V_Behaving, int PageIndex, int PageSize, out int TotalCount)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }


        public DiagnosisTreatment GetDiagnosisTreatmentByDTItemID(long DTItemID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }

        public IList<DiagnosisTreatment> GetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID)
        {
            return GetDiagnosisTreatmentByPtID_V2(patientID, PtRegistrationID, nationalMedCode, opt, latest, V_RegistrationType, ServiceRecID, null);
        }
        public IList<DiagnosisTreatment> GetDiagnosisTreatmentByPtID_V2(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID
            , long? PtRegDetailID)
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
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                cn.Open();
                List<DiagnosisTreatment> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagTrmtCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }


        public DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID_InPt(long patientID, long? V_DiagnosisType)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DiagnosisTreatment GetLatestDiagnosisTreatment_InPtByInstructionID(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID)
        {
            return GetAllDiagnosisTreatment_InPt(patientID, V_DiagnosisType, IntPtDiagDrInstructionID);
        }
        //==== #001
        public DiagnosisTreatment GetLatestDiagnosisTreatment_InPtByInstructionID_V2(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID, out List<DiagnosisIcd10Items> ICD10List)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetLatestDiagnosisTreatment_InPtByInstructionID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                    cmd.AddParameter("@V_DiagnosisType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_DiagnosisType));
                    cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IntPtDiagDrInstructionID));
                    cn.Open();
                    DiagnosisTreatment obj = new DiagnosisTreatment();
                    ICD10List = new List<DiagnosisIcd10Items>();
                    IDataReader reader = ExecuteReader(cmd);
                    obj = GetDiagTrmtItemFromReader(reader);
                    if (reader.NextResult())
                    {
                        ICD10List = GetDiagnosisIcd10ItemsCollectionFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public long? spGetPtRegistrationIDInDiagnosisTreatment_Latest(long patientID, long PtRegistrationID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return result;
            }
        }

        public bool CheckDiagnosisTreatmentExists_PtRegDetailID(long patientID, long PtRegDetailID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return results <= 0;
            }
        }
        public bool CheckStatusInPtRegistration_InPtRegistrationID(long InPtRegistrationID)
        {
            int results = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckStatusInPtRegistration_InPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
         
                cmd.AddParameter("@InPtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPtRegistrationID));
                SqlParameter par13 = cmd.Parameters.Add("@IsExists", SqlDbType.TinyInt);
                par13.Direction = ParameterDirection.Output;
                cn.Open();
                ExecuteNonQuery(cmd);
                if (cmd.Parameters["@IsExists"].Value != DBNull.Value)
                {
                    results = Convert.ToInt32(cmd.Parameters["@IsExists"].Value);
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return results <= 0;
            }
        }
        public void UpdateInPtRegistrationID_PtRegistration(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateInPtRegistrationID_PtRegistration", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public DiagnosisTreatment DiagnosisTreatment_GetLast(long PatientID, long PtRegistrationID, long ServiceRecID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }

        public DiagnosisTreatment GetDiagnosisTreatment_InPt(long ServiceRecID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DiagnosisTreatment GetBlankDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }

        public bool DeleteDiagnosisTreatment(DiagnosisTreatment entity)
        {
            throw new NotImplementedException();
        }

        /*▼====: #002*/
        //public  bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServiceRecID)
        public bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServiceRecID, out long DTItemID)
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
                    cmd.AddParameter("@DiagnosisOther", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisOther));
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

                    //▼===== #003
                    cmd.AddParameter("@MedSecretaryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MedSecretaryID));
                    //▲===== #003
                    //▼===== #008
                    cmd.AddParameter("@UserOfficialAccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UserOfficialAccountID));
                    //▲===== #008
                    //▼===== #009
                    cmd.AddParameter("@ReasonHospitalStay", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ReasonHospitalStay));
                    //▲===== #009
                    //▼===== #016
                    cmd.AddParameter("@DiseaseStage", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiseaseStage));
                    //▲===== #016

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
                    CleanUpConnectionAndCommand(cn, cmd);
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

        public bool AddDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, out List<InPatientDeptDetail> ReloadInPatientDeptDetails)
        {
            long SmallProcedureID;
            return AddDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, XML, DiagnosisICD9ListID, ICD9XML, new List<Resources>(), out ReloadInPatientDeptDetails, null, out SmallProcedureID);
        }
        public bool AddDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML
            //▼====: #016
            , IList<Resources> ResourceList
            //▲====: #016
            , out List<InPatientDeptDetail> ReloadInPatientDeptDetails, SmallProcedure aSmallProcedure, out long SmallProcedureID)
        {
            //entity.PatientServiceRecord.ExamDate = DateTime.Now;
            ReloadInPatientDeptDetails = null;
            bool result = false;
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SmallProcedureID = 0;
                   
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

                    //▼===== #002
                    cmd.AddParameter("@LowerBloodPressure", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.LowerBloodPressure));
                    //▲===== #002
                    //▼===== #005
                    cmd.AddParameter("@DiagnosisOther", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisOther));
                    //▲===== #005
                    //▼===== #012
                    cmd.AddParameter("@IsSevereIllness", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsSevereIllness));
                    //▼===== #012
                    //▼==== #013
                    cmd.AddParameter("@RespiratoryRate", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.RespiratoryRate));
                    //▲==== #013
                    //▼==== #016
                    cmd.AddParameter("@DiseaseStage", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiseaseStage));
                    //▲==== #016
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        ReloadInPatientDeptDetails = GetInPatientDeptDetailCollectionFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (aSmallProcedure != null)
                    {
                        SmallProcedureID = aSmallProcedure.SmallProcedureID;
                        //▼====: #016
                        CommonUtilsProvider.Instance.EditSmallProcedure_V2(aSmallProcedure, entity.PatientServiceRecord.StaffID.GetValueOrDefault(0), 0, new List<DiagnosisICD9Items>(), ResourceList, out SmallProcedureID);
                        //CommonUtilsProvider.Instance.EditSmallProcedure(aSmallProcedure, entity.PatientServiceRecord.StaffID.GetValueOrDefault(0), out SmallProcedureID);
                        //▲====: #016
                    }
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

        public bool UpdateDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out int VersionNumberOut, bool IsUpdateWithoutChangeDoctorIDAndDatetime = false)
        {
            try
            {
                VersionNumberOut = 0;
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
                    cmd.AddParameter("@DiagnosisOther", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisOther));
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
                    cmd.AddParameter("@IsUpdateWithoutChangeDoctorIDAndDatetime", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateWithoutChangeDoctorIDAndDatetime));

                    //▼===== #003
                    cmd.AddParameter("@MedSecretaryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MedSecretaryID));
                    //▲===== #003
                    cmd.AddParameter("@VersionNumber", SqlDbType.Int, ConvertNullObjectToDBNull(entity.VersionNumber));
                    //▼===== #009
                    cmd.AddParameter("@ReasonHospitalStay", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ReasonHospitalStay));
                    //▲===== #009
                    //▼===== #016
                    cmd.AddParameter("@DiseaseStage", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiseaseStage));
                    //▲===== #016

                    SqlParameter par13 = cmd.Parameters.Add("@ErrorID", SqlDbType.TinyInt);
                    par13.Direction = ParameterDirection.Output;
                    cmd.AddParameter("@VersionNumberOut", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    results = (byte)cmd.Parameters["@ErrorID"].Value;
                    if (cmd.Parameters["@VersionNumberOut"].Value != DBNull.Value)
                    {
                        VersionNumberOut = Convert.ToInt32(cmd.Parameters["@VersionNumberOut"].Value);
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                    return results == 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool UpdateDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, bool IsUpdateDiagConfirmInPT, out int VersionNumberOut)
        {
            VersionNumberOut = 0;
            return UpdateDiagnosisTreatment_InPt_V2(entity, DiagnosisIcd10ListID, out ReloadInPatientDeptDetails, XML, DiagnosisICD9ListID, ICD9XML, new List<Resources>(), null, IsUpdateDiagConfirmInPT, out VersionNumberOut);
        }
        public bool UpdateDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML
            //▼====: #016
            , IList<Resources> ResourceList
            //▲====: #016
            , SmallProcedure aSmallProcedure, bool IsUpdateDiagConfirmInPT, out int VersionNumberOut)
        {
            try
            {
                VersionNumberOut = 0;
                byte results = 0;
                ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    long SmallProcedureID = 0;
                    
                    SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_Update_InPt", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@DTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DTItemID));
                    cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ServiceRecID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.PtRegistrationID));
                    cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                    cmd.AddParameter("@V_Behaving", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientServiceRecord.V_Behaving));
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

                    //▼===== #002
                    cmd.AddParameter("@Pulse", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.Pulse));
                    cmd.AddParameter("@BloodPressure", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.BloodPressure));
                    cmd.AddParameter("@Temperature", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.Temperature));
                    cmd.AddParameter("@SpO2", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.SpO2));
                    cmd.AddParameter("@LowerBloodPressure", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.LowerBloodPressure));
                    //▲===== #002
                    cmd.AddParameter("@VersionNumber", SqlDbType.Int, ConvertNullObjectToDBNull(entity.VersionNumber));
                    cmd.AddParameter("@VersionNumberOut", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                    cmd.AddParameter("@IsUpdateDiagConfirmInPT", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdateDiagConfirmInPT));
                    //▼===== #005
                    cmd.AddParameter("@DiagnosisOther", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisOther));
                    //▲===== #005
                    cmd.AddParameter("@ReasonHospitalStay", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ReasonHospitalStay));
                    cmd.AddParameter("@AdmissionCriteriaList", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AdmissionCriteriaList));
                    cmd.AddParameter("@IsTreatmentCOVID", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsTreatmentCOVID));
                    //▼===== #012
                    cmd.AddParameter("@IsSevereIllness", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsSevereIllness));
                    //▼===== #012
                    //▼==== #013
                    cmd.AddParameter("@RespiratoryRate", SqlDbType.Decimal, ConvertNullObjectToDBNull(entity.RespiratoryRate));
                    //▲==== #013 
                    cmd.AddParameter("@DiseaseStage", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiseaseStage));
                    SqlParameter par13 = cmd.Parameters.Add("@ErrorID", SqlDbType.TinyInt);
                    par13.Direction = ParameterDirection.Output;
                    cn.Open();
                    //int retVal = ExecuteNonQuery(cmd);
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        ReloadInPatientDeptDetails = GetInPatientDeptDetailCollectionFromReader(reader);
                    }
                    //20191024 TBL: Vì sử dụng ExecuteReader nên khi output thì value đều = null nên phải reader.NextResult
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            if (reader.HasColumn("@ErrorID") && reader["@ErrorID"] != DBNull.Value)
                            {
                                results = Convert.ToByte(reader["@ErrorID"]);
                            }
                            if (reader.HasColumn("@VersionNumberOut") && reader["@VersionNumberOut"] != DBNull.Value)
                            {
                                VersionNumberOut = Convert.ToInt32(reader["@VersionNumberOut"]);
                            }
                        }
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (aSmallProcedure != null)
                    {
                        SmallProcedureID = aSmallProcedure.SmallProcedureID;
                        //▼====: #016
                        CommonUtilsProvider.Instance.EditSmallProcedure_V2(aSmallProcedure, entity.PatientServiceRecord.StaffID.GetValueOrDefault(0), 0, new List<DiagnosisICD9Items>(), ResourceList, out SmallProcedureID);
                        //CommonUtilsProvider.Instance.EditSmallProcedure(aSmallProcedure, entity.PatientServiceRecord.StaffID.GetValueOrDefault(0), out SmallProcedureID);
                        //▲====: #016
                    }
                    return results == 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool CheckValidProcedure(long PtRegistrationID)
        {
            try
            {
                int tempCount = 0;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                   
                    SqlCommand cmd = new SqlCommand("spCheckValidProcedure", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                 
                    cn.Open();
                    //int retVal = ExecuteNonQuery(cmd);
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            if (reader.HasColumn("CountDV") && reader["CountDV"] != DBNull.Value)
                            {
                                tempCount = Convert.ToInt32(reader["CountDV"]);
                                
                            }
                            
                        }
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (tempCount == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load_InPt(long DTItemID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<DiagnosisICD9Items> GetDiagnosisICD9Items_Load_InPt(long DTItemID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load(long? ServiceRecID, long? PatientID, bool Last)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public bool UpdateDiseaseProgression(DiagnosisTreatment entity, bool IsUpdate)
        {
            try
            {
                byte results = 0;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateDiseaseProgression", cn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DTItemID));
                    cmd.AddParameter("@OrientedTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrientedTreatment));
                    cmd.AddParameter("@DoctorComments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DoctorComments));
                    cmd.AddParameter("@IsUpdate", SqlDbType.Bit, ConvertNullObjectToDBNull(IsUpdate));
                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    return results == 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 3.PatientMedicalRecords
        public bool DeletePMR(PatientMedicalRecord entity)
        {
            throw new NotImplementedException();
        }

        public bool AddPMR(long ptID, string ptRecBarCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalRecords_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptID));
                cmd.AddParameter("@PatientRecBarCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ptRecBarCode));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool UpdatePMR(PatientMedicalRecord entity)
        {
            throw new NotImplementedException();
        }

        public IList<PatientMedicalRecord> GetPMRsByPtID(long? patientID, int? inclExpiredPMR)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }

        }

        public PatientMedicalRecord GetPMRByPtID(long? patientID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }

        public PatientMedicalRecord PatientMedicalRecords_ByPatientID(long patientID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }

        public bool PatientMedicalRecords_Save(long PatientRecID, long PatientID, string NationalMedicalCode, out string Error)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return Result;
            }
        }


        #endregion
        #endregion
        #region thao tac tren file
        public PatientMedicalFile PatientMedicalFiles_ByID(long ServiceRecID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }

        public IList<PatientMedicalFile> PatientMedicalFiles_ByPatientRecID(long PatientRecID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }

        public bool CheckExists_PatientMedicalFiles(long ServiceRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_CheckExists", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
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

        public bool Insert_PatientMedicalFiles(PatientMedicalFile entity, out long PatientRecID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                AxLogging.AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
        }

        public bool PatientMedicalFiles_Update(PatientMedicalFile entity)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                AxLogging.AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
        }

        public bool PatientMedicalFiles_Delete(PatientMedicalFile entity, long staffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientMedicalFileID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientMedicalFileID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(staffID));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool PatientMedicalFiles_Active(PatientMedicalFile entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalFiles_Active", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientMedicalFileID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientMedicalFileID));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool WriteFileXML(string path, string FileName, List<StringXml> Contents)
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

        public List<StringXml> ReadFileXML(string FullPath)
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

        #region TreatmentProcess
        public TreatmentProcess SaveTreatmentProcess(TreatmentProcess entity)
        {
            try
            {
                TreatmentProcess Result = new TreatmentProcess();
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSaveTreatmentProcess", cn);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@TreatmentProcessID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TreatmentProcessID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.CurPatientRegistration.PtRegistrationID));
                    cmd.AddParameter("@PathologicalProcess", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PathologicalProcess));
                    cmd.AddParameter("@PCLResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PCLResult));
                    cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Diagnosis));
                    cmd.AddParameter("@Treatments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Treatments));
                    cmd.AddParameter("@DischargedCondition", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DischargedCondition));
                    cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Note));
                    //▼====: #017
                    cmd.AddParameter("@TreatmentsProcess", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TreatmentsProcess));
                    cmd.AddParameter("@ResultsEvaluation", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ResultsEvaluation));
                    cmd.AddParameter("@Prognosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Prognosis));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                    cmd.AddParameter("@HeadOfDepartmentDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.HeadOfDepartmentDoctorStaffID));
                    cmd.AddParameter("@DeptName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DeptName));
                    cmd.AddParameter("@LocationName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LocationName));
                    cmd.AddParameter("@BedNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(entity.BedNumber));
                    //▲====: #017
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null && reader.Read())
                    {
                        Result = GetTreatmentProcessFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public TreatmentProcess GetTreatmentProcessByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                TreatmentProcess Result = new TreatmentProcess();
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetTreatmentProcessByPtRegistrationID", cn);

                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null && reader.Read())
                    {
                        Result = GetTreatmentProcessFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteTreatmentProcess(long TreatmentProcessID, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteTreatmentProcess", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@TreatmentProcessID", SqlDbType.BigInt, TreatmentProcessID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                cn.Open();
                int result = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }
        #endregion

        //Dinh them
        #region CLS

        public IList<PatientServiceRecord> GetAllPatientServiceRecord(long patientID,DateTime FromDate, DateTime ToDate)
        {
            List<PatientServiceRecord> listRs = new List<PatientServiceRecord>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientServiceRecordsGetByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(patientID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetPatientServiceRecordCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public IList<PatientPCLRequest> GetAllPatientPCLReqServiceRecID(long ServiceRecID, long V_PCLMainCategory)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }

        public InPatientAdmDisDetails GetInPatientAdmDisDetails(long PtRegistrationID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return Rs;
        }

        public bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        public bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        #endregion

        //HPT 08/02/2017: Giấy chuyển tuyến
        public TransferForm SaveTransferForm(TransferForm entity, long StaffID) //==== #019
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
                    cmd.AddParameter("@TuNgay", SqlDbType.Date, ConvertNullObjectToDBNull(entity.TuNgay));
                    cmd.AddParameter("@DenNgay", SqlDbType.Date, ConvertNullObjectToDBNull(entity.DenNgay));
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
                    cmd.AddParameter("@TransferReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TransferReason));
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
                    //▼==== #019
                    cmd.AddParameter("@StaffID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(StaffID));
                    //▲==== #019
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null && reader.Read())
                    {
                        Result = GetTransferFormFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IList<TransferForm> GetTransferForm(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*TMA 20/11/2017*/
        public IList<TransferForm> GetTransferForm_Date(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID, DateTime FromDate, DateTime ToDate)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeleteTransferForm(long TransferFormID, long StaffID, string DeletedReason) //==== #018
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteTransferForm", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, TransferFormID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                //▼==== #018
                cmd.AddParameter("@DeletedReason", SqlDbType.NVarChar, DeletedReason);
                //▲==== #018
                cn.Open();
                int result = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }
        public TransferForm GetTransferFormByPtRegistrationID( long PtRegistrationID)
        {
            try
            {
                TransferForm Result = new TransferForm();
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetTransferFormByPtRegistrationID", cn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null && reader.Read())
                    {
                        Result = GetTransferFormFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #region Other Func
        public DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(long? InPtRegistrationID, long? V_DiagnosisType, long IntPtDiagDrInstructionID = 0)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDiagnosisTreatment_ByPtID_InPt_ForDiag", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@InPtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(InPtRegistrationID));
                    cmd.AddParameter("@V_DiagnosisType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_DiagnosisType));
                    cmd.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IntPtDiagDrInstructionID));
                    cn.Open();
                    DiagnosisTreatment obj = new DiagnosisTreatment();
                    IDataReader reader = ExecuteReader(cmd);
                    obj = GetDiagTrmtItemFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
        public MedicalExaminationResult GetMedicalExaminationResultByPtRegDetailID(long PtRegDetailID)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetMedicalExaminationResultByPtRegDetailID", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                    mConnection.Open();
                    MedicalExaminationResult mItem = new MedicalExaminationResult();
                    IDataReader mReader = ExecuteReader(mCommand);
                    if (mReader.Read())
                    {
                        mItem = GetMedicalExaminationResultFromReader(mReader);
                    }
                    mReader.Close();
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return mItem;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public MedicalExaminationResult GetMedicalExaminationResultByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetMedicalExaminationResultByPtRegistrationID", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    mConnection.Open();
                    MedicalExaminationResult mItem = new MedicalExaminationResult();
                    IDataReader mReader = ExecuteReader(mCommand);
                    if (mReader.Read())
                    {
                        mItem = GetMedicalExaminationResultFromReader(mReader);
                    }
                    mReader.Close();
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return mItem;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void UpdateMedicalExaminationResult(MedicalExaminationResult aMedicalExaminationResult)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spUpdateMedicalExaminationResult", mConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    mCommand.AddParameter("@MedicalExaminationResultXML", SqlDbType.Xml, ConvertNullObjectToDBNull(aMedicalExaminationResult.ToXML()));
                    mConnection.Open();
                    MedicalExaminationResult mItem = new MedicalExaminationResult();
                    ExecuteNonQuery(mCommand);
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //▼==== #020
        public void DeleteMedicalExaminationResult(MedicalExaminationResult aMedicalExaminationResult)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spDeleteMedicalExaminationResult", mConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    mCommand.AddParameter("@MedicalExaminationResultXML", SqlDbType.Xml, ConvertNullObjectToDBNull(aMedicalExaminationResult.ToXML()));
                    mConnection.Open();
                    MedicalExaminationResult mItem = new MedicalExaminationResult();
                    ExecuteNonQuery(mCommand);
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲==== #020

        public IList<TreatmentHistory> GetTreatmentHistoriesByPatientID(long PatientID, bool IsCriterion_PCLResult, DateTime? ToDate, DateTime? FromDate)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetTreatmentHistoriesByPatientID", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                    mCommand.AddParameter("@IsCriterion_PCLResult", SqlDbType.Bit, ConvertNullObjectToDBNull(IsCriterion_PCLResult));
                    mCommand.AddParameter("@ToDate", SqlDbType.DateTime, ToDate);
                    mCommand.AddParameter("@FromDate", SqlDbType.DateTime, FromDate);
                    mConnection.Open();
                    IList<TreatmentHistory> ItemCollection = new List<TreatmentHistory>();
                    IDataReader reader = ExecuteReader(mCommand);
                    if (reader != null)
                    {
                        ItemCollection = GetTreatmentHistoryCollectionFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return ItemCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IList<TreatmentHistory> GetTreatmentHistoriesByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetTreatmentHistoriesByPtRegistrationID", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    mConnection.Open();
                    IList<TreatmentHistory> ItemCollection = new List<TreatmentHistory>();
                    IDataReader reader = ExecuteReader(mCommand);
                    if (reader != null)
                    {
                        ItemCollection = GetTreatmentHistoryCollectionFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return ItemCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▼===== #004
        public bool AddDiagnosysConsultation(DiagnosysConsultationSummary gDiagConsultation, List<Staff> SurgeryDoctorCollection, List<DiagnosisIcd10Items> refICD10List, out long DiagConsultationSummaryID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAddDiagnosysConsultation", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DiagConsultationSummaryID", SqlDbType.BigInt, 16 , ParameterDirection.Output);
                cmd.AddParameter("@ConsultationDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationDate));
                cmd.AddParameter("@ConsultationDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationDiagnosis));
                cmd.AddParameter("@ConsultationResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationResult));
                cmd.AddParameter("@RecCreateDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(gDiagConsultation.RecCreateDate));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.StaffID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.V_RegistrationType));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.PatientID));
                cmd.AddParameter("@StaffXML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertListStaffToXml(SurgeryDoctorCollection)));
                cmd.AddParameter("@ICD10XML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertICD10ToXml(refICD10List)));
                cmd.AddParameter("@Title", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.Title));
                cmd.AddParameter("@V_DiagnosysConsultation", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.V_DiagnosysConsultation));
                cmd.AddParameter("@ConsultationSummary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationSummary));
                cmd.AddParameter("@ConsultationTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationTreatment));
                cmd.AddParameter("@PresiderStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.PresiderStaffID));
                cmd.AddParameter("@SecretaryStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.SecretaryStaffID));
                cn.Open();
                DiagConsultationSummaryID = 0;
                int retVal = ExecuteNonQuery(cmd);
                if (cmd.Parameters["@DiagConsultationSummaryID"].Value != DBNull.Value)
                {
                    DiagConsultationSummaryID = Convert.ToInt32(cmd.Parameters["@DiagConsultationSummaryID"].Value);
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public bool UpdateDiagnosysConsultation(DiagnosysConsultationSummary gDiagConsultation, List<Staff> SurgeryDoctorCollection, List<DiagnosisIcd10Items> refICD10List, out long DiagConsultationSummaryID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateDiagnosysConsultation", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DiagConsultationSummaryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.DiagConsultationSummaryID));
                cmd.AddParameter("@ConsultationDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationDiagnosis));
                cmd.AddParameter("@ConsultationResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationResult));
                cmd.AddParameter("@ModifiedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(gDiagConsultation.ModifiedDate));
                cmd.AddParameter("@ModifiedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.ModifiedStaffID));
                cmd.AddParameter("@StaffXML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertListStaffToXml(SurgeryDoctorCollection)));
                cmd.AddParameter("@ICD10XML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertICD10ToXml(refICD10List)));
                cmd.AddParameter("@ConsultationDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationDate));
                cmd.AddParameter("@V_DiagnosysConsultation", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.V_DiagnosysConsultation));
                cmd.AddParameter("@Title", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.Title));
                cmd.AddParameter("@DiagConsultationSummaryID_New", SqlDbType.BigInt,16, ParameterDirection.Output);
                cmd.AddParameter("@ConsultationSummary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationSummary));
                cmd.AddParameter("@ConsultationTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(gDiagConsultation.ConsultationTreatment));
                cmd.AddParameter("@PresiderStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.PresiderStaffID));
                cmd.AddParameter("@SecretaryStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(gDiagConsultation.SecretaryStaffID));
                cn.Open();
                DiagConsultationSummaryID = 0;
                int retVal = ExecuteNonQuery(cmd);
                if (cmd.Parameters["@DiagConsultationSummaryID_New"].Value != DBNull.Value)
                {
                    DiagConsultationSummaryID = Convert.ToInt32(cmd.Parameters["@DiagConsultationSummaryID_New"].Value);
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public DiagnosysConsultationSummary LoadDiagnosysConsultationSummary(long DiagConsultationSummaryID, out List<Staff> StaffList, out List<DiagnosisIcd10Items> ICD10List)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spLoadDiagnosysConsultation", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DiagConsultationSummaryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiagConsultationSummaryID));
                cn.Open();
                DiagnosysConsultationSummary objDiagConsultationSummary = null;
                StaffList = new List<Staff>();
                ICD10List = new List<DiagnosisIcd10Items>();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    objDiagConsultationSummary = GetDiagnosysConsultationSummaryFromReader(reader);
                    if (reader.NextResult())
                    {
                        StaffList = GetStaffCollectionFromReader(reader);
                    }
                    if (reader.NextResult())
                    {
                        ICD10List = GetDiagnosisIcd10ItemsCollectionFromReader(reader);
                    }
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objDiagConsultationSummary;
            }
        }
        //▲===== #004

        public TransferForm GetTransferFormByID(long transferFormID, int V_PatientFindBy)
        {
            try
            {
                TransferForm Result = null;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetTransferFormByID", cn);

                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.AddParameter("@TransferFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(transferFormID));
                    cmd.AddParameter("@V_PatientFindBy", SqlDbType.Int, ConvertNullObjectToDBNull(V_PatientFindBy));

                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader.Read())
                    {
                        Result = GetTransferFormFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //▼====: #006
        public IList<RequiredSubDiseasesReferences> GetListRequiredSubDiseasesReferences(string MainICD10)
        {
            using (SqlConnection mConnection = new SqlConnection(ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spGetListSubRequireICDFromMainICD", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@MainICD", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MainICD10));
                mConnection.Open();
                IList<RequiredSubDiseasesReferences> ItemCollection = new List<RequiredSubDiseasesReferences>();
                IDataReader reader = ExecuteReader(mCommand);
                if (reader != null)
                {
                    ItemCollection = GetRequiredSubDiseasesReferencesDataCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return ItemCollection;
            }
        }
        //▲====: #006
        //▼====: #007
        public IList<RuleDiseasesReferences> GetListRuleDiseasesReferences(string ICD10)
        {
            using (SqlConnection mConnection = new SqlConnection(ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spGetListRuleFromICD", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@ICD", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ICD10));
                mConnection.Open();
                IList<RuleDiseasesReferences> ItemCollection = new List<RuleDiseasesReferences>();
                IDataReader reader = ExecuteReader(mCommand);
                if (reader != null)
                {
                    ItemCollection = GetRuleDiseasesReferencesDataCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return ItemCollection;
            }
        }
        //▲====: #007
        public IList<PCLExamAccordingICD> GetListPCLExamAccordingICD(long PatientID, long V_SpecialistType, long PtRegistrationID)
        {
            using (SqlConnection mConnection = new SqlConnection(ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spGetListPCLExamAccordingICD", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                mCommand.AddParameter("@V_SpecialistType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_SpecialistType));
                mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                mConnection.Open();
                IList<PCLExamAccordingICD> ItemCollection = new List<PCLExamAccordingICD>();
                IDataReader reader = ExecuteReader(mCommand);
                if (reader != null)
                {
                    //ItemCollection = GetPatientPCLRequestCollectionFromReader(reader);
                    ItemCollection = GetPCLExamAccordingICDCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return ItemCollection;
            }
        }
        public AdmissionExamination GetAdmissionExamination(long PtRegistrationID, long V_RegistrationType)
        {
            using (SqlConnection mConnection = new SqlConnection(ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spGetAdmissionExamination", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                mCommand.AddParameter("V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
            
                mConnection.Open();
                AdmissionExamination Item = new AdmissionExamination();
                IDataReader reader = ExecuteReader(mCommand);
                if (reader != null && reader.Read())
                {
                   
                    Item = GetAdmissionExaminationFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return Item;
            }
        }
        public bool SaveAdmissionExamination(AdmissionExamination admissionExamination, out long AdmissionExaminationID_New)
        {
            using (SqlConnection mConnection = new SqlConnection(ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spSaveAdmissionExamination", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@AdmissionExaminationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(admissionExamination.AdmissionExaminationID));
                mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(admissionExamination.PtRegistrationID));
                mCommand.AddParameter("@ReferralDiagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.ReferralDiagnosis));
                mCommand.AddParameter("@PathologicalProcess", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.PathologicalProcess));
                mCommand.AddParameter("@MedicalHistory", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.MedicalHistory));
                mCommand.AddParameter("@FamilyMedicalHistory", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.FamilyMedicalHistory));
                mCommand.AddParameter("@FullBodyExamination", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.FullBodyExamination));
                mCommand.AddParameter("@PartialExamination", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.PartialExamination));
                mCommand.AddParameter("@PclResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.PclResult));
                mCommand.AddParameter("@DrugTreatment", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.DrugTreatment));
                mCommand.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(admissionExamination.CreatedStaffID));
                mCommand.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(admissionExamination.CreatedDate));
                mCommand.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(admissionExamination.IsDeleted));
                //▼====: #010
                mCommand.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(admissionExamination.DeptID));
                mCommand.AddParameter("@ReasonAdmission", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.ReasonAdmission));
                mCommand.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.Notes));
                mCommand.AddParameter("@DiagnosisResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(admissionExamination.DiagnosisResult));
                mCommand.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(admissionExamination.V_RegistrationType));
                //▲====: #010
                mCommand.AddParameter("@AdmissionExaminationID_New", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
            
                mConnection.Open();
                int retVal = ExecuteNonQuery(mCommand);
                if (mCommand.Parameters["@AdmissionExaminationID_New"].Value != null)
                {
                    AdmissionExaminationID_New = Convert.ToInt64(mCommand.Parameters["@AdmissionExaminationID_New"].Value);
                }
                else
                {
                    AdmissionExaminationID_New = 0;
                }
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return retVal > 0;
            }
        }
        public IList<DiagnosisTreatment> GetDiagAndDoctorInstruction_InPt_ByPtRegID(long PtRegistrationID, long? DTItemID, long? DeptID)
        {
            using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spGetDiagAndDoctorInstruction_InPt_ByPtRegID", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                mCommand.AddParameter("@DTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTItemID));
                mCommand.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                mConnection.Open();
                List<DiagnosisTreatment> mCollection = null;
                IDataReader reader = ExecuteReader(mCommand);
                mCollection = GetDiagTrmtCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(mConnection, mCommand);
                return mCollection;
            }
        }
        public void GetDataFromWebForInstruction_ByIntPtDiagDrInstructionID(long IntPtDiagDrInstructionID, out string Disease_Progression, out string Diet, out long V_LevelCare
            , out string PCLExamTypeList, out IList<RefMedicalServiceGroupItem> MedServiceList, out string DrugList)
        {
            using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand mCommand = new SqlCommand("spGetDataFromWebForInstruction_ByIntPtDiagDrInstructionID", mConnection);
                mCommand.CommandType = CommandType.StoredProcedure;
                mCommand.AddParameter("@IntPtDiagDrInstructionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(IntPtDiagDrInstructionID));

                mCommand.AddParameter("@Disease_Progression", SqlDbType.NVarChar, 512, DBNull.Value, ParameterDirection.Output);
                mCommand.AddParameter("@Diet", SqlDbType.NVarChar, 12, DBNull.Value, ParameterDirection.Output);
                mCommand.AddParameter("@V_LevelCare", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                mCommand.AddParameter("@PCLExamTypeList", SqlDbType.NVarChar, 512, DBNull.Value, ParameterDirection.Output);
                mCommand.AddParameter("@MedServiceList", SqlDbType.NVarChar, 512, DBNull.Value, ParameterDirection.Output);
                mCommand.AddParameter("@DrugList", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                mConnection.Open();
                ExecuteNonQuery(mCommand);
                if (mCommand.Parameters["@Disease_Progression"].Value != DBNull.Value)
                {
                    Disease_Progression = Convert.ToString(mCommand.Parameters["@Disease_Progression"].Value);
                }
                else
                {
                    Disease_Progression = null;
                }
                if (mCommand.Parameters["@Diet"].Value != DBNull.Value)
                {
                    Diet = Convert.ToString(mCommand.Parameters["@Diet"].Value);
                }
                else
                {
                    Diet = null;
                }
                if (mCommand.Parameters["@V_LevelCare"].Value != DBNull.Value)
                {
                    V_LevelCare = Convert.ToInt64(mCommand.Parameters["@V_LevelCare"].Value);
                }
                else
                {
                    V_LevelCare = 0;
                }
                if (mCommand.Parameters["@PCLExamTypeList"].Value != DBNull.Value)
                {
                    PCLExamTypeList = Convert.ToString(mCommand.Parameters["@PCLExamTypeList"].Value);
                }
                else
                {
                    PCLExamTypeList = null;
                }
                MedServiceList = null;
                if (mCommand.Parameters["@MedServiceList"].Value != DBNull.Value)
                {
                    string stringMedServiceList = Convert.ToString(mCommand.Parameters["@MedServiceList"].Value);
                    MedServiceList = new List<RefMedicalServiceGroupItem>();
                    foreach (var item in stringMedServiceList.Split(','))
                    {
                        List<RefMedicalServiceGroupItem> tempMedServiceList = aEMR.DataAccessLayer.Providers.CommonProvider_V2.Instance.GetRefMedicalServiceGroupItemsByID(Convert.ToInt64(item));
                        foreach (var service in tempMedServiceList)
                        {
                            MedServiceList.Add(service);
                        }
                    }
                }
              
                if (mCommand.Parameters["@DrugList"].Value != DBNull.Value)
                {
                    DrugList = Convert.ToString(mCommand.Parameters["@DrugList"].Value);
                }
                else
                {
                    DrugList = null;
                }

                CleanUpConnectionAndCommand(mConnection, mCommand);
            }
        }

        //▼==== #011
        public void UpdateConclusionMedicalExaminationResult(MedicalExaminationResult Result)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spUpdateConclusionMedicalExaminationResult", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Result.PtRegistrationID));
                    mCommand.AddParameter("@HealthClassification", SqlDbType.Int, ConvertNullObjectToDBNull(Result.HealthClassification));
                    mCommand.AddParameter("@Diseases", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Result.Diseases));
                    mCommand.AddParameter("@Record", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Result.Record));
                    mCommand.AddParameter("@CurrentHealth", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Result.CurrentHealth));
                    mCommand.AddParameter("@HealthCheckUpDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Result.HealthCheckUpDate));
                    mCommand.AddParameter("@ExpiryDateHealthCertificate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Result.ExpiryDateHealthCertificate));
                    mConnection.Open();
                    ExecuteReader(mCommand);
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲==== #011
        //▼==== #014
        public SelfDeclaration GetSelfDeclarationByPatientID(long PatientID)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetSelfDeclarationByPatientID", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));

                    mConnection.Open();
                    SelfDeclaration Item = new SelfDeclaration();
                    IDataReader reader = ExecuteReader(mCommand);
                    if (reader != null && reader.Read())
                    {
                        Item = GetSelfDeclarationFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return Item;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SelfDeclaration GetSelfDeclarationByPtRegistrationID(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spGetSelfDeclarationByPtRegistrationID", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    mCommand.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                    mConnection.Open();
                    SelfDeclaration Item = new SelfDeclaration();
                    IDataReader reader = ExecuteReader(mCommand);
                    if (reader != null && reader.Read())
                    {
                        Item = GetSelfDeclarationFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(mConnection, mCommand);
                    return Item;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool SaveSelfDeclaration(SelfDeclaration Obj, long StaffID)
        {
            try
            {
                using (SqlConnection mConnection = new SqlConnection(ConnectionString))
                {
                    SqlCommand mCommand = new SqlCommand("spSaveSelfDeclaration", mConnection);
                    mCommand.CommandType = CommandType.StoredProcedure;
                    mCommand.AddParameter("@SelfDeclarationSheetID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.SelfDeclarationSheetID));
                    mCommand.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PtRegistrationID));
                    mCommand.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PatientID));
                    mCommand.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RegistrationType));
                    mCommand.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    mCommand.AddParameter("@Answer1", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.Answer1));
                    mCommand.AddParameter("@Answer1_Count", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.Answer1_Count));
                    mCommand.AddParameter("@Answer1_Drug", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer1_Drug));
                    mCommand.AddParameter("@Answer1_Solve", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer1_Solve));
                    mCommand.AddParameter("@Answer2", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.Answer2));
                    mCommand.AddParameter("@Answer2_Count", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.Answer2_Count));
                    mCommand.AddParameter("@Answer2_Drug", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer2_Drug));
                    mCommand.AddParameter("@Answer2_Solve", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer2_Solve));
                    mCommand.AddParameter("@Answer3", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.Answer3));
                    mCommand.AddParameter("@Answer3_Count", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.Answer3_Count));
                    mCommand.AddParameter("@Answer3_Drug", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer3_Drug));
                    mCommand.AddParameter("@Answer3_Solve", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer3_Solve));
                    mCommand.AddParameter("@Answer4", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.Answer4));
                    mCommand.AddParameter("@Answer4_Count", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.Answer4_Count));
                    mCommand.AddParameter("@Answer4_Drug", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer4_Drug));
                    mCommand.AddParameter("@Answer4_Solve", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer4_Solve));
                    mCommand.AddParameter("@Answer5", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.Answer5));
                    mCommand.AddParameter("@Answer5_Count", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.Answer5_Count));
                    mCommand.AddParameter("@Answer5_Drug", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer5_Drug));
                    mCommand.AddParameter("@Answer5_Solve", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer5_Solve));
                    mCommand.AddParameter("@Answer6", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.Answer6));
                    mCommand.AddParameter("@Answer6_Count", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.Answer6_Count));
                    mCommand.AddParameter("@Answer6_Drug", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer6_Drug));
                    mCommand.AddParameter("@Answer6_Solve", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Answer6_Solve));
                    //▼==== #015
                    mCommand.AddParameter("@RelativeFullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.RelativeFullName));
                    mCommand.AddParameter("@RelativeAge", SqlDbType.Char, ConvertNullObjectToDBNull(Obj.RelativeAge));
                    mCommand.AddParameter("@Relationship", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Relationship));
                    mCommand.AddParameter("@RelativePhone", SqlDbType.Char, ConvertNullObjectToDBNull(Obj.RelativePhone));
                    mCommand.AddParameter("@TotalCost", SqlDbType.Decimal, ConvertNullObjectToDBNull(Obj.TotalCost));
                    //▲==== #015
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
        //▲==== #014
        //▼==== #015
        public bool CheckBeforeUpdateDiagnosisTreatment(long PtRegDetailID, long DoctorStaffID, out string errorMessages, out string confirmMessages)
        {
            try
            {
                errorMessages = "";
                confirmMessages = "";
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCheckBeforeUpdateDiagnosisTreatment", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, PtRegDetailID);
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, DoctorStaffID);
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader == null)
                    {
                        return false;
                    }
                    while (reader.Read())
                    {
                        if (reader.HasColumn("Message") && reader["Message"] != DBNull.Value && !string.IsNullOrEmpty(reader["Message"].ToString()))
                        {
                            if (reader.HasColumn("MessageType") && reader["MessageType"] != DBNull.Value && (byte)reader["MessageType"] == 1)
                            {
                                confirmMessages += reader["Message"].ToString() + Environment.NewLine;
                            }
                            else
                            {
                                errorMessages += reader["Message"].ToString() + Environment.NewLine;
                            }
                        }
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲==== #015

        //▼====: #016
        public IList<DiagnosisICD9Items> GetDiagnosisICD9Items_Load(long DTItemID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiagnosisICD9Items_Load", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DTItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DTItemID));

                cn.Open();
                List<DiagnosisICD9Items> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetDiagnosisICD9ItemsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        //▲====: #016

        //▼====: #017
        public IList<TreatmentProcess> GetAllTreatmentProcessByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetTreatmentProcessByPtRegistrationID", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                    cn.Open();
                    List<TreatmentProcess> objLst = null;
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null)
                    {
                        objLst = GetTreatmentProcessCollectionFromReader(reader);
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objLst;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲====: #017
        public bool AddNewDiagDetal_KhoaSan(long PtRegistrationID, int? SoConChet, DateTime? NgayConChet)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddNewDiagDetal_KhoaSan", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@SoConChet", SqlDbType.Int, ConvertNullObjectToDBNull(SoConChet));
                    cmd.AddParameter("@NgayConChet", SqlDbType.DateTime, ConvertNullObjectToDBNull(NgayConChet));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool GetSoConChet_KhoaSan(long PtRegistrationID, out int? SoConChet, out DateTime? NgayConChet)
        {
            try
            {
                SoConChet = null;
                NgayConChet = null;
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetSoConChet_KhoaSan", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    int retVal = 0;
                    if (reader == null)
                    {
                        return false;
                    }
                    while (reader.Read())
                    {
                        if (reader.HasColumn("SoConChet") && reader["SoConChet"] != DBNull.Value)
                        {
                            SoConChet = (int?)reader["SoConChet"];
                        }
                        if (reader.HasColumn("NgayConChet") && reader["NgayConChet"] != DBNull.Value)
                        {
                            NgayConChet = (DateTime?)reader["NgayConChet"];
                        }
                        retVal++;
                    }
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}