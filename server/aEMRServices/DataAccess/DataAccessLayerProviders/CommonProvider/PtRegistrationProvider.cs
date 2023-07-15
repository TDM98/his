using System;
using System.Collections.Generic;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using eHCMS.Configurations;
using eHCMS.DAL;

namespace aEMR.DataAccessLayer.Providers
{
    public class PtRegistrationProvider : DataProviderBase
    {
           static private PtRegistrationProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public PtRegistrationProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PtRegistrationProvider();
                }
                return _instance;
            }
        }

        public PtRegistrationProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }


        public  DateTime GetMaxExamDate()
        {
            DateTime MaxExam = DateTime.Now;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetMaxExamDate", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                try
                {
                    MaxExam = (DateTime)cmd.ExecuteScalar();
                }
                catch
                {
                    MaxExam = DateTime.Now;
                }

                CleanUpConnectionAndCommand(cn, cmd);
                return MaxExam;
            }

        }

        protected override Patient GetPatientFromReader(IDataReader reader)
        {
            Patient p = base.GetPatientFromReader(reader);
            try
            {
                p.EthnicName = reader["EthnicName"].ToString();
                p.EthnicCode = reader["EthnicCode"].ToString();
            }
            catch
            {
            }
            return p;
        }
        public  List<Patient> SearchPatients(PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("proc_SearchPatients", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramFullName = new SqlParameter("@FullName", SqlDbType.NVarChar, 100);
                paramFullName.Value = ConvertNullObjectToDBNull(criteria.FullName);
                SqlParameter paramPatientCode = new SqlParameter("@PatientCode", SqlDbType.Char, 20);/*Do chua 2 gia tri: PatientCode va PMR nen khai lai la 20, luc truoc chi la 8*/
                paramPatientCode.Value = ConvertNullObjectToDBNull(criteria.PatientCode);

                SqlParameter paramEntryDateEnabled = new SqlParameter("@EntryDateEnabled", SqlDbType.Bit);
                paramEntryDateEnabled.Value = criteria.EntryDateEnabled;
                SqlParameter paramEntryDateBegin = new SqlParameter("@EntryDateBegin", SqlDbType.DateTime);
                paramEntryDateBegin.Value = ConvertNullObjectToDBNull(criteria.EntryDateBegin);
                SqlParameter paramEntryDateEnd = new SqlParameter("@EntryDateEnd", SqlDbType.DateTime);
                paramEntryDateEnd.Value = ConvertNullObjectToDBNull(criteria.EntryDateEnd);

                SqlParameter paramReleaseDateEnabled = new SqlParameter("@ReleaseDateEnabled", SqlDbType.Bit);
                paramReleaseDateEnabled.Value = criteria.ReleaseDateEnabled;
                SqlParameter paramReleaseDateBegin = new SqlParameter("@ReleaseDateBegin", SqlDbType.DateTime);
                paramReleaseDateBegin.Value = ConvertNullObjectToDBNull(criteria.ReleaseDateBegin);
                SqlParameter paramReleaseDateEnd = new SqlParameter("@ReleaseDateEnd", SqlDbType.DateTime);
                paramReleaseDateEnd.Value = ConvertNullObjectToDBNull(criteria.ReleaseDateEnd);

                SqlParameter paramBirthDateEnabled = new SqlParameter("@BirthDateEnabled", SqlDbType.Bit);
                paramBirthDateEnabled.Value = criteria.BirthDateEnabled;
                SqlParameter paramBirthDateBegin = new SqlParameter("@BirthDateBegin", SqlDbType.DateTime);
                paramBirthDateBegin.Value = ConvertNullObjectToDBNull(criteria.BirthDateBegin);
                SqlParameter paramBirthDateEnd = new SqlParameter("@BirthDateEnd", SqlDbType.DateTime);
                paramBirthDateEnd.Value = ConvertNullObjectToDBNull(criteria.BirthDateEnd);

                SqlParameter paramGenderEnabled = new SqlParameter("@GenderEnabled", SqlDbType.Bit);
                paramGenderEnabled.Value = criteria.GenderEnabled;
                SqlParameter paramGender = new SqlParameter("@Gender", SqlDbType.Char, 1);
                if (criteria.Gender != null)
                    paramGender.Value = criteria.Gender.ID;
                else
                    paramGender.Value = DBNull.Value;

                SqlParameter paramHealthInsuranceCard = new SqlParameter("@HealthInsuranceCard", SqlDbType.VarChar, 15);
                paramHealthInsuranceCard.Value = ConvertNullObjectToDBNull(criteria.InsuranceCard);
                SqlParameter paramIncludeInactivePatients = new SqlParameter("@IncludeInactivePatients", SqlDbType.Bit);
                paramIncludeInactivePatients.Value = false;

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.Int);
                paramPageSize.Value = pageSize;
                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.Int);
                paramPageIndex.Value = pageIndex;

                SqlParameter paramOrderBy = new SqlParameter("@OrderBy", SqlDbType.NVarChar);
                paramOrderBy.Value = ConvertNullObjectToDBNull(criteria.OrderBy);

                SqlParameter paramCountTotal = new SqlParameter("@CountTotal", SqlDbType.Bit);
                paramCountTotal.Value = bCountTotal;
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramFullName);
                cmd.Parameters.Add(paramPatientCode);
                cmd.Parameters.Add(paramEntryDateEnabled);
                cmd.Parameters.Add(paramEntryDateBegin);
                cmd.Parameters.Add(paramEntryDateEnd);
                cmd.Parameters.Add(paramReleaseDateEnabled);
                cmd.Parameters.Add(paramReleaseDateBegin);
                cmd.Parameters.Add(paramReleaseDateEnd);
                cmd.Parameters.Add(paramBirthDateEnabled);
                cmd.Parameters.Add(paramBirthDateBegin);
                cmd.Parameters.Add(paramBirthDateEnd);
                cmd.Parameters.Add(paramGenderEnabled);
                cmd.Parameters.Add(paramGender);
                cmd.Parameters.Add(paramHealthInsuranceCard);
                cmd.Parameters.Add(paramIncludeInactivePatients);
                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);

                cmd.Parameters.Add(paramOrderBy);
                cmd.Parameters.Add(paramCountTotal);
                cmd.Parameters.Add(paramTotal);
                cmd.AddParameter("@IsShowHICardNo", SqlDbType.Bit, ConvertNullObjectToDBNull(criteria.IsShowHICardNo));
                cn.Open();
                List<Patient> patients = null;

                IDataReader reader = ExecuteReader(cmd);

                patients = GetPatientCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                    totalCount = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return patients;
            }
        }

        protected override PatientRegistration GetPatientRegistrationFromReader(IDataReader reader)
        {
            PatientRegistration p = new PatientRegistration();

            if (reader.HasColumn("PtRegistrationID") && reader["PtRegistrationID"] != DBNull.Value)
            {
                p.PtRegistrationID = (long)reader["PtRegistrationID"];
            }

            if (reader.HasColumn("PtRegistrationCode") && reader["PtRegistrationCode"] != DBNull.Value)
            {
                p.PtRegistrationCode = reader["PtRegistrationCode"].ToString();
            }
            if (reader.HasColumn("PatientClassID") && reader["PatientClassID"] != DBNull.Value)
            {
                p.PatientClassID = reader["PatientClassID"] as long?;
            }
            if (reader.HasColumn("PtInsuranceBenefit") && reader["PtInsuranceBenefit"] != DBNull.Value)
            {
                p.PtInsuranceBenefit = reader["PtInsuranceBenefit"] as double?;
            }
            if (reader.HasColumn("RegTypeID") && reader["RegTypeID"] != DBNull.Value)
            {
                p.RegTypeID = reader["RegTypeID"] as byte?;
            }
            if (reader.HasColumn("DeptID") && reader["DeptID"] != DBNull.Value)
            {
                p.DeptID = reader["DeptID"] as long?;
            }
            if (reader.HasColumn("PatientID") && reader["PatientID"] != DBNull.Value)
            {
                p.PatientID = reader["PatientID"] as long?;
            }
            if (reader.HasColumn("EmergRecID") && reader["EmergRecID"] != DBNull.Value)
            {
                p.EmergRecID = reader["EmergRecID"] as long?;
            }

            if (reader.HasColumn("IsForeigner") && reader["IsForeigner"] != DBNull.Value)
            {
                p.IsForeigner = (reader["IsForeigner"] as bool?).GetValueOrDefault(false);
            }

            if (reader.HasColumn("EmergInPtReExamination") && reader["EmergInPtReExamination"] != DBNull.Value)
            {
                p.EmergInPtReExamination = (reader["EmergInPtReExamination"] as bool?).GetValueOrDefault(false);
            }
            //HPT 20/08/2015 BEGIN:Câu lệnh IF dưới đây thực hiện đọc từ reader giá trị và bỏ vào cột IsHICard_FiveYearsCont (được thêm mới)trong table PatientRegistration và PatientRegistration_InPt
            if (reader.HasColumn("IsHICard_FiveYearsCont") && reader["IsHICard_FiveYearsCont"] != DBNull.Value)
            {
                p.IsHICard_FiveYearsCont = Convert.ToBoolean(reader["IsHICard_FiveYearsCont"]);
            }
            if (reader.HasColumn("IsChildUnder6YearsOld") && reader["IsChildUnder6YearsOld"] != DBNull.Value)
            {
                p.IsChildUnder6YearsOld = Convert.ToBoolean(reader["IsChildUnder6YearsOld"]);
            }
            //IsAllowCrossRegion: Thông tuyến (11/01/2017)
            if (reader.HasColumn("IsAllowCrossRegion") && reader["IsAllowCrossRegion"] != DBNull.Value)
            {
                p.IsAllowCrossRegion = Convert.ToBoolean(reader["IsAllowCrossRegion"]);
            }
            //HPT 20/08/2015 END
            if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
            {
                p.StaffID = reader["StaffID"] as long?;
            }
            if (reader.HasColumn("ExamDate") && reader["ExamDate"] != DBNull.Value)
            {
                p.ExamDate = (DateTime)reader["ExamDate"];
            }

            if (reader.HasColumn("AdmissionDate") && reader["AdmissionDate"] != DBNull.Value)
            {
                p.ExamDate = (DateTime)reader["AdmissionDate"];
            }

            if (reader.HasColumn("V_DocumentTypeOnHold") && reader["V_DocumentTypeOnHold"] != DBNull.Value)
            {
                p.V_DocumentTypeOnHold = reader["V_DocumentTypeOnHold"] as long?;
            }
            if (reader.HasColumn("SequenceNo") && reader["SequenceNo"] != DBNull.Value)
            {
                p.SequenceNo = (int)reader["SequenceNo"];
            }

            p.RecordState = RecordState.UNCHANGED;
            if (reader.HasColumn("V_RegistrationStatus") && reader["V_RegistrationStatus"] != DBNull.Value)
            {
                p.V_RegistrationStatus = (long)reader["V_RegistrationStatus"];
            }

            if (reader.HasColumn("HIReportID") && reader["HIReportID"] != DBNull.Value)
            {
                p.HIReportID = Convert.ToInt32(reader["HIReportID"]);
                p.RegLockFlag = 1;
            }
            if (Enum.IsDefined(typeof(AllLookupValues.RegistrationStatus), (int)p.V_RegistrationStatus))
            {
                p.RegistrationStatus = (AllLookupValues.RegistrationStatus)p.V_RegistrationStatus;
            }
            else
            {
                p.RegistrationStatus = AllLookupValues.RegistrationStatus.INVALID;
            }

            if (reader.HasColumn("MedServiceNames"))
            {
                p.MedServiceNames = reader["MedServiceNames"] as string;
            }
            try
            {
                if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                {
                    p.FullName = reader["FullName"].ToString();
                }
                if (reader.HasColumn("PatientCode") && reader["PatientCode"] != DBNull.Value)
                {
                    p.PatientCode = reader["PatientCode"].ToString();
                }
            }
            catch
            {
            }

            try
            {
                p.Patient = GetPatientFromReader(reader);
                /*▼====: #001*/
                p.Patient.GenerateAgeString(p.ExamDate);
                /*▲====: #001*/
            }
            catch
            {
            }
            if (reader.HasColumn("ConfirmHIStaffID") && reader["ConfirmHIStaffID"] != DBNull.Value)
            {
                p.ConfirmHIStaffID = (long)reader["ConfirmHIStaffID"];
            }
            if (reader.HasColumn("ChangedLog") && reader["ChangedLog"] != DBNull.Value)
            {
                p.ChangedLog = reader["ChangedLog"].ToString();
            }
            if (reader.HasColumn("OutPtTreatmentProgramID") && reader["OutPtTreatmentProgramID"] != DBNull.Value)
            {
                p.OutPtTreatmentProgramID = (long)reader["OutPtTreatmentProgramID"];
            }
            if (reader.HasColumn("DTDTReportID") && reader["DTDTReportID"] != DBNull.Value)
            {
                p.DTDTReportID = (long)reader["DTDTReportID"];
            }
            return p;
        }
        public  bool UpdatePatientRegistration(long PtRegistrationID, long V_RegistrationStatus)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientRegistration_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_RegistrationStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationStatus));

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                cn.Open();
                int val = cmd.ExecuteNonQuery();

                CleanUpConnectionAndCommand(cn, cmd);

                if (val > 0)
                    return true;
                else
                    return false;

            }
        }
        public  bool UpdatePatientRegistrationPayStatus(long PtRegistrationID, long V_RegistrationPaymentStatus)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientRegistration_UpdatePaymentStatus", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_RegistrationPaymentStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationPaymentStatus));

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                cn.Open();
                int val = cmd.ExecuteNonQuery();

                CleanUpConnectionAndCommand(cn, cmd);
                if (val > 0)                
                    return true;                
                else
                    return false;
            }
        }


        public  bool PatientRegistration_Close(long PtRegistrationID, int FindPatient, long V_RegistrationClosingStatus)
        {
            string Result = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientRegistration_Close", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@FindPatient", SqlDbType.Int, ConvertNullObjectToDBNull(FindPatient));
                cmd.AddParameter("@V_RegistrationClosingStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationClosingStatus));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                {
                    Result = cmd.Parameters["@Result"].Value.ToString();
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }

            if (Result == "0")
                return false;
            return true;

        }

        public  List<PatientRegistration> SearchPtRegistration(SeachPtRegistrationCriteria criteria, int pageSize, int pageIndex, bool bcount, out int Totalcount)
        {
            Totalcount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchPatientRegistration", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramFullName = new SqlParameter("@FullName", SqlDbType.NVarChar, 100);
                paramFullName.Value = ConvertNullObjectToDBNull(criteria.FullName);
                SqlParameter paramPatientCode = new SqlParameter("@PatientCode", SqlDbType.VarChar);
                paramPatientCode.Value = ConvertNullObjectToDBNull(criteria.PatientCode);

                SqlParameter paramFromdate = new SqlParameter("@FromDate", SqlDbType.DateTime);
                paramFromdate.Value = ConvertNullObjectToDBNull(criteria.FromDate);
                SqlParameter paramTodate = new SqlParameter("@Todate", SqlDbType.DateTime);
                paramTodate.Value = ConvertNullObjectToDBNull(criteria.ToDate);

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.Int);
                paramPageSize.Value = pageSize;
                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.Int);
                paramPageIndex.Value = pageIndex;

                SqlParameter paramCountTotal = new SqlParameter("@CountTotal", SqlDbType.Bit);
                paramCountTotal.Value = bcount;
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramFullName);
                cmd.Parameters.Add(paramPatientCode);
                cmd.Parameters.Add(paramFromdate);
                cmd.Parameters.Add(paramTodate);

                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);
                cmd.Parameters.Add(paramCountTotal);
                cmd.Parameters.Add(paramTotal);

                cn.Open();
                List<PatientRegistration> lst = null;

                IDataReader reader = ExecuteReader(cmd);

                lst = GetPatientRegistrationCollectionFromReader(reader);
                reader.Close();

                if (bcount && paramTotal.Value != null)
                {
                    Totalcount = (int)paramTotal.Value;
                }
                else
                {
                    Totalcount = -1;
                }

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        //Ny create Mr Tuyen not delete nhe!
        public  Patient GetPatientInfoByPtRegistration(long? PtRegistrationID, long? PatientID, int FindPatient)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPaytientInfo_ByPtReg", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@FindPatient", SqlDbType.BigInt, ConvertNullObjectToDBNull(FindPatient));

                cn.Open();
                Patient p = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader != null && reader.Read())
                {
                    //Lay thong tin general info.
                    p = GetPatientFromReader(reader);
                    if (!reader.NextResult())
                    {
                        return p;
                    }
                    if (reader.Read())
                    {
                        p.LatestRegistration = GetPatientRegistrationFromReader(reader);
                    }
                    if (!reader.NextResult())
                    {
                        return p;
                    }
                    if (reader.Read())
                    {
                        p.CurrentClassification = GetPatientClassificationFromReader(reader);
                    }
                    if (!reader.NextResult())
                    {
                        return p;
                    }
                    if (reader.Read())
                    {
                        p.CurrentHealthInsurance = GetHealthInsuranceFromReader(reader);
                    }
                    if (!reader.NextResult())
                    {
                        return p;
                    }
                    if (reader.Read())
                    {
                        p.CurrentHealthInsurance.InsuranceBenefit = new InsuranceBenefit();
                        p.CurrentHealthInsurance.InsuranceBenefit = GetInsuranceBenefitFromReader(reader);
                    }
                }

                if (reader != null)
                {
                    reader.Close();
                }

                CleanUpConnectionAndCommand(cn, cmd);
                return p;
            }
        }
        //Dinh create Mr Tuyen not delete nhe!
        public  List<PatientRegistration> GetAllPatientRegistration_ByRegType(long PatientID, long V_RegistrationType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientRegistration_GetByRegType", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cn.Open();
                List<PatientRegistration> lst = null;

                IDataReader reader = ExecuteReader(cmd);

                lst = GetPatientRegistrationCollectionFromReader(reader);

                if (reader != null)
                {
                    reader.Close();
                }

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

    }
}
