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
using System.Collections.ObjectModel;
/*
 * 20171002 #001 CMN: Added GetAllSugeriesByPtRegistrationID
*/
namespace eHCMS.DAL
{
    public class SqlCommonUtilsProvider : CommonUtilsProvider
    {
        public SqlCommonUtilsProvider()
            : base()
        {

        }

        #region Override methods
        #region 1.Drugs
        protected override RefGenericDrugDetail GetRefDrugGenericDetailFromReader(IDataReader reader)
        {
            RefGenericDrugDetail p = new RefGenericDrugDetail();
            p.DrugID = Convert.ToInt64(reader["DrugID"]);
            p.BrandName = reader["BrandName"].ToString();
            p.GenericName = reader["GenericName"].ToString();
            return p;
        }

        public override List<RefGenericDrugDetail> SearchRefDrugNames(string brandName, long pageIndex, long pageSize, byte type)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefGenericDrugName_SearchAutoComplete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramBrandName = new SqlParameter("@BrandName", SqlDbType.NVarChar, 128);
                paramBrandName.Value = ConvertNullObjectToDBNull(brandName);

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.BigInt);
                paramPageSize.Value = pageSize;

                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.BigInt);
                paramPageIndex.Value = pageIndex;

                SqlParameter paramType = new SqlParameter("@Type", SqlDbType.Int);
                paramType.Value = type;

                cmd.Parameters.Add(paramBrandName);
                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);
                cmd.Parameters.Add(paramType);

                cn.Open();
                List<RefGenericDrugDetail> drugs = null;

                IDataReader reader = ExecuteReader(cmd);

                drugs = GetRefDrugGenericCollectionFromReader(reader);
                reader.Close();
                return drugs;
            }
        }
        #endregion

        #region 2.DiseasesReference
        public override List<DiseasesReference> SearchRefDiseases(string searchKey, long pageIndex, long pageSize, byte type,out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiseasesReferences_SearchAutoComplete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramSearchKey = new SqlParameter("@SearchKey", SqlDbType.NVarChar);
                paramSearchKey.Value = ConvertNullObjectToDBNull(searchKey);

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.BigInt);
                paramPageSize.Value = pageSize;

                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.BigInt);
                paramPageIndex.Value = pageIndex;

                SqlParameter paramType = new SqlParameter("@Type", SqlDbType.Int);
                //@Type:0 get by ICD10Code, 1: get by DiseaseNameVN, 2: get by DiseaseName
                paramType.Value = type;

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramSearchKey);
                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);
                cmd.Parameters.Add(paramType);

                cmd.Parameters.Add(paramTotal);
                cn.Open();
                List<DiseasesReference> idc10s = null;

                IDataReader reader = ExecuteReader(cmd);

                idc10s = GetRefDiseasesCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != DBNull.Value)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
                return idc10s;
            }
        }

        #endregion

        #region RefICD9
        public override List<RefICD9> SearchRefICD9(string searchKey, long pageIndex, long pageSize, byte ICD9SearchType, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefICD9_SearchAutoComplete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SearchKey", SqlDbType.NVarChar, ConvertNullObjectToDBNull(searchKey));
                cmd.AddParameter("@PageSize", SqlDbType.BigInt, ConvertNullObjectToDBNull(pageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.BigInt, ConvertNullObjectToDBNull(pageIndex));
                //@Type:0 get by ICD9Code, 1: get by ProcedureName
                cmd.AddParameter("@ICD9SearchType", SqlDbType.Int, ConvertNullObjectToDBNull(ICD9SearchType));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();
                List<RefICD9> ICD9List = null;

                IDataReader reader = ExecuteReader(cmd);

                ICD9List = GetRefICD9CollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != DBNull.Value)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
                return ICD9List;
            }
        }

        #endregion


        #region 3.Staff
        public override List<Staff> SearchStaffFullName(string searchKey, long pageIndex, long pageSize)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffs_SearchAutoComplete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramSearchKey = new SqlParameter("@SearchKey", SqlDbType.VarChar, 10);
                paramSearchKey.Value = ConvertNullObjectToDBNull(searchKey);

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.BigInt);
                paramPageSize.Value = pageSize;

                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.BigInt);
                paramPageIndex.Value = pageIndex;

                cmd.Parameters.Add(paramSearchKey);
                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);

                cn.Open();
                List<Staff> staffs = null;

                IDataReader reader = ExecuteReader(cmd);

                staffs = GetStaffFullNameCollectionFromReader(reader);
                reader.Close();
                return staffs;
            }
        }

        public override List<Staff> SearchStaffCat(Staff Staff, long pageIndex, long pageSize)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffsSearchAutoComplete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramSearchKey = new SqlParameter("@SearchKey", SqlDbType.VarChar, 10);
                paramSearchKey.Value = ConvertNullObjectToDBNull(Staff.FullName);

                cmd.AddParameter("@V_StaffCatType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Staff.RefStaffCategory.V_StaffCatType));


                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.BigInt);
                paramPageSize.Value = pageSize;

                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.BigInt);
                paramPageIndex.Value = pageIndex;

                cmd.Parameters.Add(paramSearchKey);
                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);

                cn.Open();
                List<Staff> staffs = null;

                IDataReader reader = ExecuteReader(cmd);

                staffs = GetStaffFullNameCollectionFromReader(reader);
                reader.Close();
                return staffs;
            }
        }


        public override List<Staff> GetAllStaffs()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetAllStaff", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cn.Open();
                    List<Staff> staffs = null;

                    IDataReader reader = ExecuteReader(cmd);

                    staffs = GetStaffFullNameCollectionFromReader(reader);
                    reader.Close();
                    return staffs;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override List<Staff> GetAllStaffs_FromStaffID(long nFromStaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetAllStaff", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@FromStaffID", SqlDbType.BigInt, nFromStaffID);
                    cn.Open();
                    List<Staff> staffList = null;

                    IDataReader reader = ExecuteReader(cmd);

                    staffList = GetStaffFullNameCollectionFromReader(reader);
                    reader.Close();

                    if (staffList != null && nFromStaffID == 0) // TxD 07/02/2018: First half of Staff list
                    {
                        int nCount = staffList.Count();
                        if (nCount > 400)
                        {
                            int nHalfCount = nCount / 2;
                            List<Staff> halfStaffList = new List<Staff>();
                            for (int nIdx = 0; nIdx < nHalfCount; ++nIdx)
                            {
                                halfStaffList.Add(staffList[nIdx]);
                            }
                            return halfStaffList;
                        }
                    }
                    
                    return staffList;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 4.Departments
      
        public override RefDepartment GetRefDepartmentByLID(long locationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_ByLID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@LID", SqlDbType.BigInt, ConvertNullObjectToDBNull(locationID));

                cn.Open();
                RefDepartment obj = null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    obj = GetDepartmentFromReader(reader);
                }

                reader.Close();
                return obj;
            }
        }

        #endregion

        #region 5.Lookup
        #endregion

        #region 7.PhysicalExamination

        public override void PhysicalExamination_Insert(PhysicalExamination p, long? staffID)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("PhysicalExamination_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.CommonMedRecID));
                //cmd.AddParameter("@RecordDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(p.RecordDate));
                cmd.AddParameter("@Height", SqlDbType.Float, ConvertNullObjectToDBNull(p.Height));
                cmd.AddParameter("@Weight", SqlDbType.Float, ConvertNullObjectToDBNull(p.Weight));
                cmd.AddParameter("@SystolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.SystolicPressure));
                cmd.AddParameter("@DiastolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(p.DiastolicPressure));
                cmd.AddParameter("@Pulse", SqlDbType.Float, ConvertNullObjectToDBNull(p.Pulse));
                cmd.AddParameter("@Cholesterol", SqlDbType.Float, ConvertNullObjectToDBNull(p.Cholesterol));
                cmd.AddParameter("@Smoke_EveryDay", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.Smoke_EveryDay));
                cmd.AddParameter("@Smoke_OnOccasion", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.Smoke_OnOccasion));

                cmd.AddParameter("@Smoke_Never", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.Smoke_Never));
                cmd.AddParameter("@Alcohol_CurrentHeavy", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.Alcohol_CurrentHeavy));
                cmd.AddParameter("@Alcohol_HeavyInThePast", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.Alcohol_HeavyInThePast));
                cmd.AddParameter("@Alcohol_CurrentLight", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.Alcohol_CurrentLight));
                cmd.AddParameter("@Alcohol_Never", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.Alcohol_Never));
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
        public override void PhysicalExamination_Delete(long CommonMedRecID)
        {
        }
        public override void PhysicalExamination_Update(long CommonMedRecID
                                                            , DateTime RecordDate
                                                            , float Height
                                                            , float Weight
                                                            , float SystolicPressure
                                                            , float DiastolicPressure
                                                            , float Pulse
                                                            , float Cholesterol
                                                            , int Smoke_EveryDay
                                                            , int Smoke_OnOccasion
                                                            , int Smoke_Never
                                                            , int Alcohol_CurrentHeavy
                                                            , int Alcohol_HeavyInThePast
                                                            , int Alcohol_CurrentLight
                                                            , int Alcohol_Never

                                                            , float CVRisk)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("PhysicalExamination_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@CommonMedRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CommonMedRecID));
                cmd.AddParameter("@RecordDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(RecordDate));
                cmd.AddParameter("@Height", SqlDbType.Float, ConvertNullObjectToDBNull(Height));
                cmd.AddParameter("@Weight", SqlDbType.Float, ConvertNullObjectToDBNull(Weight));
                cmd.AddParameter("@SystolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(SystolicPressure));
                cmd.AddParameter("@DiastolicPressure", SqlDbType.Float, ConvertNullObjectToDBNull(DiastolicPressure));
                cmd.AddParameter("@Pulse", SqlDbType.Float, ConvertNullObjectToDBNull(Pulse));
                cmd.AddParameter("@Cholesterol", SqlDbType.Float, ConvertNullObjectToDBNull(Cholesterol));
                cmd.AddParameter("@Smoke_EveryDay", SqlDbType.BigInt, ConvertNullObjectToDBNull(Smoke_EveryDay));
                cmd.AddParameter("@Smoke_OnOccasion", SqlDbType.BigInt, ConvertNullObjectToDBNull(Smoke_OnOccasion));

                cmd.AddParameter("@Smoke_Never", SqlDbType.BigInt, ConvertNullObjectToDBNull(Smoke_Never));
                cmd.AddParameter("@Alcohol_CurrentHeavy", SqlDbType.BigInt, ConvertNullObjectToDBNull(Alcohol_CurrentHeavy));
                cmd.AddParameter("@Alcohol_HeavyInThePast", SqlDbType.BigInt, ConvertNullObjectToDBNull(Alcohol_HeavyInThePast));
                cmd.AddParameter("@Alcohol_CurrentLight", SqlDbType.BigInt, ConvertNullObjectToDBNull(Alcohol_CurrentLight));
                cmd.AddParameter("@Alcohol_Never", SqlDbType.BigInt, ConvertNullObjectToDBNull(Alcohol_Never));

                cmd.AddParameter("@CVRisk", SqlDbType.Float, ConvertNullObjectToDBNull(CVRisk));

                //cmd.AddParameter("@RespiratoryRate", SqlDbType.Float, ConvertNullObjectToDBNull(RespiratoryRate));
                //cmd.AddParameter("@SpO2", SqlDbType.Float, ConvertNullObjectToDBNull(SpO2));
                //cmd.AddParameter("@Temperature", SqlDbType.Float, ConvertNullObjectToDBNull(Temperature));
                cn.Open();
                cmd.ExecuteNonQuery();
            }


        }
        protected virtual PhysicalExamination GetPhysicalExamFromReader(IDataReader reader)
        {
            PhysicalExamination p = new PhysicalExamination();
            try
            {
                p.CommonMedRecID = (long)reader["CommonMedRecID"];
                p.RecordDate = (DateTime)reader["RecordDate"];
                p.Height = (float)reader["Height"];
                p.Weight = (float)reader["Weight"];
                p.SystolicPressure = (float)reader["SystolicPressure"];
                p.DiastolicPressure = (float)reader["DiastolicPressure"];
                p.Pulse = (float)reader["Pulse"];
                p.Cholesterol = (float)reader["Cholesterol"];
                p.Smoke_EveryDay = (bool)reader["Smoke_EveryDay"];
                p.Smoke_OnOccasion = (bool)reader["Smoke_OnOccasion"];
                p.Smoke_Never = (bool)reader["Smoke_Never"];
                p.Alcohol_CurrentHeavy = (bool)reader["Alcohol_CurrentHeavy"];
                p.Alcohol_HeavyInThePast = (bool)reader["Alcohol_HeavyInThePast"];
                p.Alcohol_CurrentLight = (bool)reader["Alcohol_CurrentLight"];
                p.Alcohol_Never = (bool)reader["Alcohol_Never"];
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
        public override PhysicalExamination PhysicalExamination_GetData(long CommonMedRecID)
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
        public override List<PhysicalExamination> PhysicalExamination_ListData(long CommonMedRecID)
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
        #endregion
        #region PatientServiceRecord

        //KMx: Sửa lại từ function cũ (09/10/2014 16:45).
        //Lý do: Function cũ: 1 Service Record chứa 2 DiagnosisTreatment của 2 Service Record khác nhau, dẫn đến lưu toa xuất viện sai. 
        public override List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh(PatientServiceRecord entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientServiceRecordsGetForKhamBenh", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegistrationID));
                cmd.AddParameter("@V_ProcessingType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ProcessingType));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)entity.V_RegistrationType));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegDetailID));

                cn.Open();

                List<PatientServiceRecord> retVal = new List<PatientServiceRecord>();

                IDataReader reader = ExecuteReader(cmd);

                ObservableCollection<DiagnosisTreatment> DiagnosisTreatmentList = new ObservableCollection<DiagnosisTreatment>(GetDiagTrmtCollectionFromReader(reader));

                reader.Close();
                reader.Dispose();

                if (DiagnosisTreatmentList == null || DiagnosisTreatmentList.Count <= 0)
                {
                    return retVal;
                }

                foreach (var item in DiagnosisTreatmentList)
                {
                    if (item.ServiceRecID <= 0 || item.PatientServiceRecord == null)
                    {
                        continue;
                    }

                    PatientServiceRecord p = new PatientServiceRecord();

                    p.ServiceRecID = item.ServiceRecID.Value;
                    p.PtRegistrationID = entity.PtRegistrationID;
                    p.PtRegDetailID = item.PtRegDetailID;
                    p.V_DiagnosisType = item.PatientServiceRecord.V_DiagnosisType;

                    p.DiagnosisTreatments = new ObservableCollection<DiagnosisTreatment>();
                    p.DiagnosisTreatments.Add(item);

                    retVal.Add(p);
                }

                foreach (var aServiceRecID in DiagnosisTreatmentList.Select(x => x.ServiceRecID).Distinct())
                {
                    SqlCommand cmdPre = new SqlCommand("spPrescriptionIssueHistoryGetBySerRecID", cn);

                    cmdPre.CommandType = CommandType.StoredProcedure;
                    cmdPre.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aServiceRecID));

                    IDataReader readerPre = ExecuteReader(cmdPre);
                    List<PrescriptionIssueHistory> mPrescriptionIssueHistory = GetPtPrescriptIssueHisCollectionFromReader(readerPre);
                    readerPre.Close();
                    readerPre.Dispose();

                    foreach (var item in retVal.Where(x => x.ServiceRecID == aServiceRecID))
                    {
                        item.PrescriptionIssueHistories = new ObservableCollection<PrescriptionIssueHistory>(mPrescriptionIssueHistory);
                    }
                }

                return retVal;
            }
        }


        //public override List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh_InPt(PatientServiceRecord entity)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientServiceRecordsGetForKhamBenh_InPt", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegistrationID));
        //        cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)entity.V_RegistrationType));

        //        cn.Open();

        //        List<PatientServiceRecord> retVal = new List<PatientServiceRecord>();

        //        IDataReader reader = ExecuteReader(cmd);

        //        ObservableCollection<DiagnosisTreatment> DiagnosisTreatmentList = new ObservableCollection<DiagnosisTreatment>(GetDiagTrmtCollectionFromReader(reader));

        //        reader.Close();
        //        reader.Dispose();

        //        if (DiagnosisTreatmentList == null || DiagnosisTreatmentList.Count <= 0)
        //        {
        //            return retVal;
        //        }

        //        foreach (var item in DiagnosisTreatmentList)
        //        {
        //            if (item.ServiceRecID <= 0 || item.PatientServiceRecord == null)
        //            {
        //                continue;
        //            }

        //            PatientServiceRecord p = new PatientServiceRecord();

        //            p.ServiceRecID = item.ServiceRecID.Value;
        //            p.PtRegistrationID = entity.PtRegistrationID;
        //            p.PtRegDetailID = item.PtRegDetailID;
        //            p.V_DiagnosisType = item.PatientServiceRecord.V_DiagnosisType;

        //            p.DiagnosisTreatments = new ObservableCollection<DiagnosisTreatment>();
        //            p.DiagnosisTreatments.Add(item);

        //            SqlCommand cmdPre = new SqlCommand("spPrescriptionIssueHistoryInPtGetBySerRecID", cn);

        //            cmdPre.CommandType = CommandType.StoredProcedure;
        //            cmdPre.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(item.ServiceRecID));

        //            IDataReader readerPre = ExecuteReader(cmdPre);
        //            p.PrescriptionIssueHistories = new ObservableCollection<PrescriptionIssueHistory>(GetPtPrescriptIssueHisCollectionFromReader(readerPre));
        //            readerPre.Close();
        //            readerPre.Dispose();
        //            retVal.Add(p);
        //        }

        //        return retVal;
        //    }
        //}


        public override List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh_InPt(PatientServiceRecord entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientServiceRecordsGetForKhamBenh_InPt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)entity.V_RegistrationType));

                cn.Open();

                List<PatientServiceRecord> retVal = new List<PatientServiceRecord>();

                PatientServiceRecord p = new PatientServiceRecord();

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    p = GetPatientServiceRecordFromReader(reader);
                    if (reader.NextResult())
                    {

                        ObservableCollection<DiagnosisTreatment> DiagnosisTreatmentList = new ObservableCollection<DiagnosisTreatment>(GetDiagTrmtCollectionFromReader(reader));

                        if (DiagnosisTreatmentList != null)
                        {
                            p.DiagnosisTreatments = DiagnosisTreatmentList;
                        }
                    }
                }

                reader.Close();
                reader.Dispose();

                if (p != null && p.ServiceRecID > 0)
                {
                    SqlCommand cmdPre = new SqlCommand("spPrescriptionIssueHistoryInPtGetBySerRecID", cn);

                    cmdPre.CommandType = CommandType.StoredProcedure;
                    cmdPre.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.ServiceRecID));

                    IDataReader readerPre = ExecuteReader(cmdPre);
                    p.PrescriptionIssueHistories = new ObservableCollection<PrescriptionIssueHistory>(GetPtPrescriptIssueHisCollectionFromReader(readerPre));
                    readerPre.Close();
                    readerPre.Dispose();
                }

                retVal.Add(p);

                return retVal;
            }
        }
        //public override List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh(PatientServiceRecord entity, int findPatient)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPatientServiceRecordsGetForKhamBenh", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegistrationID));
        //        cmd.AddParameter("@V_ProcessingType", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ProcessingType));
        //        cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)entity.V_RegistrationType));
        //        cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PtRegDetailID));

        //        cn.Open();
        //        PatientServiceRecord p = new PatientServiceRecord();

        //        List<PatientServiceRecord> retVal = new List<PatientServiceRecord>();
        //        IDataReader reader = ExecuteReader(cmd);
        //        p.DiagnosisTreatments = new ObservableCollection<DiagnosisTreatment>(GetDiagTrmtCollectionFromReader(reader));

        //        reader.Close();
        //        reader.Dispose();
        //        if (p != null
        //            && p.DiagnosisTreatments != null && p.DiagnosisTreatments.Count>0)
        //        {
        //            foreach (var item in p.DiagnosisTreatments)
        //            {
        //                if(item.ServiceRecID > 0)
        //                {
        //                    p.ServiceRecID = item.ServiceRecID.Value;
        //                    p.PtRegistrationID = entity.PtRegistrationID;
        //                    p.PtRegDetailID = item.PtRegDetailID;
        //                    p.V_DiagnosisType = item.PatientServiceRecord.V_DiagnosisType;
        //                    //SqlCommand cmdPre = new SqlCommand("spPrescriptionIssueHistoryGetBySerRecID", cn);
        //                    SqlCommand cmdPre = new SqlCommand();
        //                    cmdPre.Connection = cn;
        //                    if (findPatient == (int)AllLookupValues.PatientFindBy.NGOAITRU)
        //                    {
        //                        cmdPre.CommandText = "spPrescriptionIssueHistoryGetBySerRecID";
                                
        //                    }
        //                    else
        //                    {
        //                        cmdPre.CommandText = "spPrescriptionIssueHistoryInPtGetBySerRecID";
        //                    }
        //                    cmdPre.CommandType = CommandType.StoredProcedure;
        //                    cmdPre.AddParameter("@ServiceRecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(item.ServiceRecID));

        //                    IDataReader readerPre = ExecuteReader(cmdPre);
        //                    p.PrescriptionIssueHistories = new ObservableCollection<PrescriptionIssueHistory>
        //                        (GetPtPrescriptIssueHisCollectionFromReader(readerPre));
        //                    readerPre.Close();
        //                    readerPre.Dispose();
        //                    retVal.Add(p.EntityDeepCopy());
        //                }
        //            }
                    
        //        }
                
        //        return retVal;
        //    }
        //}

        #endregion
        #region ConsultingDiagnosys
        public override bool UpdateConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys, out long ConsultingDiagnosysID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateConsultingDiagnosys", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.PatientID));
                cmd.AddParameter("@FirstExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.FirstExamDate));
                cmd.AddParameter("@NextExamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.NextExamDate));
                cmd.AddParameter("@ConsultingDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.ConsultingDate));
                cmd.AddParameter("@ConsultingDoctor", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.ConsultingDoctor));
                cmd.AddParameter("@ConsultingResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.ConsultingResult));
                cmd.AddParameter("@OutPtConsultingDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.OutPtConsultingDate));
                cmd.AddParameter("@OutPtConsultingDoctor", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.OutPtConsultingDoctor));
                cmd.AddParameter("@OutPtConsultingResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.OutPtConsultingResult));
                cmd.AddParameter("@V_DiagnosticType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.V_DiagnosticType == null ? null : (long?)aConsultingDiagnosys.V_DiagnosticType.LookupID));
                cmd.AddParameter("@V_TreatmentMethod", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.V_TreatmentMethod == null ? null : (long?)aConsultingDiagnosys.V_TreatmentMethod.LookupID));
                cmd.AddParameter("@V_HeartSurgicalType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.V_HeartSurgicalType == null ? null : (long?)aConsultingDiagnosys.V_HeartSurgicalType.LookupID));
                cmd.AddParameter("@ValveIncluded", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.ValveIncluded));
                cmd.AddParameter("@ValveQty", SqlDbType.SmallInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.ValveQty));
                cmd.AddParameter("@V_ValveType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.V_ValveType == null ? null : (long?)aConsultingDiagnosys.V_ValveType.LookupID));
                cmd.AddParameter("@RingIncluded", SqlDbType.SmallInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.RingIncluded));
                cmd.AddParameter("@RingQty", SqlDbType.SmallInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.RingQty));
                cmd.AddParameter("@CoronaryArtery", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.CoronaryArtery));
                cmd.AddParameter("@MitralIncompetence", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.MitralIncompetence));
                cmd.AddParameter("@BloodDonorNumber", SqlDbType.SmallInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.BloodDonorNumber));
                cmd.AddParameter("@EstimationPrice", SqlDbType.Money, ConvertNullObjectToDBNull(aConsultingDiagnosys.EstimationPrice));
                cmd.AddParameter("@AdminProcessExpDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.AdminProcessExpDate));
                cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.Note));
                cmd.AddParameter("@CoronarographyExpPaidTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.CoronarographyExpPaidTime));
                cmd.AddParameter("@SurgeryExpPaidTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.SurgeryExpPaidTime));
                cmd.AddParameter("@AdditionalItemsPaidTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.AdditionalItemsPaidTime));
                cmd.AddParameter("@IsEnoughBloodDonor", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.IsEnoughBloodDonor));
                cmd.AddParameter("@TMH_ExamDate", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.TMH_ExamDate));
                cmd.AddParameter("@RHM_ExamDate", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.RHM_ExamDate));
                cmd.AddParameter("@Transferred_KT_Date", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.Transferred_KT_Date));
                cmd.AddParameter("@CoronarographyDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.CoronarographyDate));
                cmd.AddParameter("@AngioDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.AngioDate));
                cmd.AddParameter("@RVPDate", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.RVPDate));
                cmd.AddParameter("@Remark", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.Remark));
                cmd.AddParameter("@ExpAdmissionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.ExpAdmissionDate));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.PtRegistrationID));
                cmd.AddParameter("@Createdby", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.Createdby));
                cmd.AddParameter("@ConsultingDiagnosysID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.ConsultingDiagnosysID));
                cmd.AddParameter("@OutConsultingDiagnosysID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                //cmd.AddParameter("@ProcessRejectedReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.SurgeryHiTechProcStatusHistories != null && aConsultingDiagnosys.SurgeryHiTechProcStatusHistories.Any(x => x.SurgeryHiTechProcStatusHistoryID == 0 && x.V_ProcessStep != null && x.V_ProcessStep.LookupID == (long)AllLookupValues.V_ProcessStep.Rejected) ? aConsultingDiagnosys.SurgeryHiTechProcStatusHistories.Where(x => x.SurgeryHiTechProcStatusHistoryID == 0 && x.V_ProcessStep != null && x.V_ProcessStep.LookupID == (long)AllLookupValues.V_ProcessStep.Rejected).FirstOrDefault().Note : null));
                cmd.AddParameter("@AdminProcessApprovedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.AdminProcessApprovedDate));
                cmd.AddParameter("@SurgeryDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.SurgeryDate));
                cmd.AddParameter("@IsAllExamCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.IsAllExamCompleted));


                if (aConsultingDiagnosys.SupportCharityOrganization != null)
                    cmd.AddParameter("@CharityOrgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.SupportCharityOrganization.CharityOrgID));
                cmd.AddParameter("@PrevSugeryDiagnostic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.PrevSugeryDiagnostic));
                cmd.AddParameter("@FinalConsultingDiagnosys", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.FinalConsultingDiagnosys));
                cmd.AddParameter("@Intervention", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.Intervention));
                cmd.AddParameter("@ShortIntervention", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.ShortIntervention));
                cmd.AddParameter("@PlastieMitrale", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.PlastieMitrale));
                cmd.AddParameter("@PTMaze", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.PTMaze));
                cmd.AddParameter("@DuraGraft", SqlDbType.Bit, ConvertNullObjectToDBNull(aConsultingDiagnosys.DuraGraft));
                cmd.AddParameter("@PCLExamStartDated", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.PCLExamStartDated));
                cmd.AddParameter("@PrevSugeryNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aConsultingDiagnosys.PrevSugeryNote));
                cmd.AddParameter("@BloodInfoGettedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.BloodInfoGettedDate));

                if (aConsultingDiagnosys.SurgeryScheduleDetail != null)
                {
                    cmd.AddParameter("@SurgeryScheduleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.SurgeryScheduleDetail.SurgeryScheduleID));
                    cmd.AddParameter("@SSD_Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(aConsultingDiagnosys.SurgeryScheduleDetail.SSD_Date));
                    cmd.AddParameter("@SSD_Room", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.SurgeryScheduleDetail.SSD_Room));
                    cmd.AddParameter("@OpSeqNum", SqlDbType.SmallInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.SurgeryScheduleDetail.OpSeqNum));
                    cmd.AddParameter("@SurgeryScheduleDetail_TeamMember", SqlDbType.Xml, SurgeryScheduleDetail_TeamMemberToXML(aConsultingDiagnosys.SurgeryScheduleDetail.SurgeryScheduleDetail_TeamMember));
                }
                if (aConsultingDiagnosys.DoctorStaff != null)
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.DoctorStaff.StaffID));
                cn.Open();
                int Affected = ExecuteNonQuery(cmd);
                ConsultingDiagnosysID = (long)cmd.Parameters.Cast<SqlParameter>().Where(x => x.Direction == ParameterDirection.Output && x.ParameterName == "@OutConsultingDiagnosysID").FirstOrDefault().Value;
                return Affected > 0;
            }
        }
        public override ConsultingDiagnosys GetConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetConsultingDiagnosys", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.PatientID));
                if (aConsultingDiagnosys.ConsultingDiagnosysID > 0)
                    cmd.AddParameter("@ConsultingDiagnosysID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aConsultingDiagnosys.ConsultingDiagnosysID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                ConsultingDiagnosys retVal = GetConsultingDiagnosysFromReader(reader).FirstOrDefault();
                reader.Close();
                reader.Dispose();
                return retVal;
            }
        }
        public override List<ConsultingDiagnosys> GetReportConsultingDiagnosys(ConsultingDiagnosysSearchCriteria aSearchCriteria, out int TotalItemCount)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRptGetConsultingDiagnosys", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@IsApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(aSearchCriteria.IsApproved));
                cmd.AddParameter("@IsLated", SqlDbType.Bit, ConvertNullObjectToDBNull(aSearchCriteria.IsLated));
                cmd.AddParameter("@IsAllExamCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(aSearchCriteria.IsAllExamCompleted));
                cmd.AddParameter("@IsSurgeryCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(aSearchCriteria.IsSurgeryCompleted));
                cmd.AddParameter("@IsWaitSurgery", SqlDbType.Bit, ConvertNullObjectToDBNull(aSearchCriteria.IsWaitSurgery));
                cmd.AddParameter("@IsDuraGraft", SqlDbType.Bit, ConvertNullObjectToDBNull(aSearchCriteria.IsDuraGraft));
                cmd.AddParameter("@IsWaitingExamCompleted", SqlDbType.Bit, ConvertNullObjectToDBNull(aSearchCriteria.IsWaitingExamCompleted));

                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSearchCriteria.FullName));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSearchCriteria.ToDate));
                cmd.AddParameter("@ConsultingDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSearchCriteria.ConsultingDoctorStaffID));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(aSearchCriteria.PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(aSearchCriteria.PageSize));

                cmd.AddParameter("@IsConsultingHistoryView", SqlDbType.Bit, ConvertNullObjectToDBNull(aSearchCriteria.IsConsultingHistoryView));

                SqlParameter pTotal = new SqlParameter("@TotalItemCount", SqlDbType.Int);
                pTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pTotal);

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                List<ConsultingDiagnosys> retVal = GetConsultingDiagnosysFromReader(reader);
                reader.Close();
                reader.Dispose();
                if (pTotal.Value != DBNull.Value && pTotal.Value != null)
                    TotalItemCount = (int)pTotal.Value;
                else
                    TotalItemCount = -1;
                return retVal;
            }
        }
        public override List<SurgerySchedule> GetSurgerySchedules()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetSurgerySchedules", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                List<SurgerySchedule> retVal = GetSurgeryScheduleCollectionFromReader(reader);
                reader.Close();
                reader.Dispose();
                return retVal;
            }
        }
        public override List<SurgeryScheduleDetail> GetSurgeryScheduleDetails(long ConsultingDiagnosysID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSurgeryScheduleDetails", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ConsultingDiagnosysID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultingDiagnosysID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                List<SurgeryScheduleDetail> retVal = GetSurgeryScheduleDetailCollectionFromReader(reader);
                reader.Close();
                reader.Dispose();
                return retVal;
            }
        }
        public override List<SurgeryScheduleDetail_TeamMember> GetSurgeryScheduleDetail_TeamMembers(long ConsultingDiagnosysID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetSurgeryScheduleDetail_TeamMember", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ConsultingDiagnosysID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultingDiagnosysID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                List<SurgeryScheduleDetail_TeamMember> retVal = GetSurgeryScheduleDetail_TeamMemberCollectionFromReader(reader);
                reader.Close();
                reader.Dispose();
                return retVal;
            }
        }
        public override bool EditSurgerySchedule(SurgerySchedule aEditSurgerySchedule, out long SurgeryScheduleID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spEditSurgerySchedule", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SurgeryScheduleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aEditSurgerySchedule.SurgeryScheduleID));
                cmd.AddParameter("@SSName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aEditSurgerySchedule.SSName));
                cmd.AddParameter("@SSFromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aEditSurgerySchedule.SSFromDate));
                cmd.AddParameter("@SSToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aEditSurgerySchedule.SSToDate));
                cmd.AddParameter("@SSCreatorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aEditSurgerySchedule.SSCreatorStaffID));
                cmd.AddParameter("@SSNote", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aEditSurgerySchedule.SSNote));
                SqlParameter pEditSurgeryScheduleID = new SqlParameter("@EditSurgeryScheduleID", SqlDbType.BigInt);
                pEditSurgeryScheduleID.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pEditSurgeryScheduleID);
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (pEditSurgeryScheduleID.Value != DBNull.Value && pEditSurgeryScheduleID.Value != null)
                    SurgeryScheduleID = (long)pEditSurgeryScheduleID.Value;
                else
                    SurgeryScheduleID = -1;
                return retVal > 0;
            }
        }
        /*▼====: #001*/
        public override List<RefMedicalServiceItem> GetAllSugeriesByPtRegistrationID(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllSugeriesByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                List<RefMedicalServiceItem> mMedicalServiceItems = GetMedicalServiceItemCollectionFromReader(reader);
                reader.Close();
                reader.Dispose();
                return mMedicalServiceItems;
            }
        }
        public override bool SaveCatastropheByPtRegDetailID(long PtRegDetailID, long V_CatastropheType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveCatastropheByPtRegDetailID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                cmd.AddParameter("@V_CatastropheType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_CatastropheType));
                cn.Open();
                int Affected = ExecuteNonQuery(cmd);
                return Affected > 0;
            }
        }
        /*▲====: #001*/
        public override void GetFirstExamDate(long PatientID, out DateTime? MinExamDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetFirstExamDate", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
                cmd.AddParameter("@MinExamDate", SqlDbType.DateTime, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                ExecuteNonQuery(cmd);
                MinExamDate = cmd.Parameters.Cast<SqlParameter>().Where(x => x.Direction == ParameterDirection.Output && x.ParameterName == "@MinExamDate").FirstOrDefault().Value == null || cmd.Parameters.Cast<SqlParameter>().Where(x => x.Direction == ParameterDirection.Output && x.ParameterName == "@MinExamDate").FirstOrDefault().Value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(cmd.Parameters.Cast<SqlParameter>().Where(x => x.Direction == ParameterDirection.Output && x.ParameterName == "@MinExamDate").FirstOrDefault().Value);
            }
        }
        public override void GetNextAppointment(long PatientID, long MedServiceID, DateTime CurentDate, out DateTime? ApptDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetNextAppointment", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, PatientID);
                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, MedServiceID);
                cmd.AddParameter("@CurentDate", SqlDbType.DateTime, CurentDate);
                cmd.AddParameter("@ApptDate", SqlDbType.DateTime, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                ExecuteNonQuery(cmd);
                ApptDate = cmd.Parameters.Cast<SqlParameter>().Where(x => x.Direction == ParameterDirection.Output && x.ParameterName == "@ApptDate").FirstOrDefault().Value == null || cmd.Parameters.Cast<SqlParameter>().Where(x => x.Direction == ParameterDirection.Output && x.ParameterName == "@ApptDate").FirstOrDefault().Value == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(cmd.Parameters.Cast<SqlParameter>().Where(x => x.Direction == ParameterDirection.Output && x.ParameterName == "@ApptDate").FirstOrDefault().Value);
            }
        }
        #endregion
        public override bool EditSmallProcedure(SmallProcedure aSmallProcedure, long StaffID, out long SmallProcedureID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spEditSmallProcedure", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SmallProcedureID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.SmallProcedureID));
                cmd.AddParameter("@ProcedureDateTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSmallProcedure.ProcedureDateTime));
                cmd.AddParameter("@Diagnosis", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSmallProcedure.Diagnosis));
                cmd.AddParameter("@ProcedureMethod", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSmallProcedure.ProcedureMethod));
                cmd.AddParameter("@NarcoticMethod", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSmallProcedure.NarcoticMethod));
                cmd.AddParameter("@ProcedureDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.ProcedureDoctorStaffID));
                cmd.AddParameter("@ProcedureDoctorStaffID2", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.ProcedureDoctorStaffID2));
                cmd.AddParameter("@NarcoticDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.NarcoticDoctorStaffID));
                cmd.AddParameter("@NarcoticDoctorStaffID2", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.NarcoticDoctorStaffID2));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.PtRegDetailID));
                cmd.AddParameter("@NurseStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.NurseStaffID));
                cmd.AddParameter("@NurseStaffID2", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.NurseStaffID2));
                cmd.AddParameter("@NurseStaffID3", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.NurseStaffID3));
                cmd.AddParameter("@CheckRecordDoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.CheckRecordDoctorStaffID));
                cmd.AddParameter("@TrinhTu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSmallProcedure.TrinhTu));
                cmd.AddParameter("@OutSmallProcedureID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSmallProcedure.V_RegistrationType));
                cn.Open();
                int Affected = ExecuteNonQuery(cmd);
                SmallProcedureID = (long)cmd.Parameters.Cast<SqlParameter>().Where(x => x.Direction == ParameterDirection.Output && x.ParameterName == "@OutSmallProcedureID").FirstOrDefault().Value;
                return Affected > 0;
            }
        }
        public override SmallProcedure GetSmallProcedure(long PtRegDetailID, long? V_RegistrationType = null)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetSmallProcedure", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                List<SmallProcedure> mMedicalServiceItems = GetSmallProcedureCollectionFromReader(reader);
                reader.Close();
                reader.Dispose();
                return mMedicalServiceItems == null ? new SmallProcedure() : mMedicalServiceItems.FirstOrDefault();
            }
        }
        public override bool ChangeStatus(long PtRegDetailID, long StaffChangeStatus, string ReasonChangeStatus)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spChangeStatusForPatientRegistrationDetails", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                cmd.AddParameter("@StaffChangeStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffChangeStatus));
                cmd.AddParameter("@ReasonChangeStatus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ReasonChangeStatus));
                cn.Open();
                int result = ExecuteNonQuery(cmd);
                return result > 0;
            }
        }
    }
}