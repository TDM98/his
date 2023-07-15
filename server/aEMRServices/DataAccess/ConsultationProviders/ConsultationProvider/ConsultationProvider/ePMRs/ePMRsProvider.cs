using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-11-23
 * Contents: Consultation Services
/*******************************************************************/
#endregion

//20180923 #001 TBL: BM 0000066. Added out long DTItemID for AddDiagnosisTreatment
namespace eHCMS.DAL
{
    public abstract class ePMRsProvider: DataProviderBase
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
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Consultations.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.Consultations.ePMRs.ProviderType);
                    _instance = (ePMRsProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }

        public ePMRsProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }

        #region 0. Common
        public abstract List<Lookup> GetLookupBehaving();
        public abstract List<Lookup> GetLookupProcessingType();
        #endregion

        #region 1.DiagnosisTreatment

        public abstract IList<DiagnosisTreatment> GetDiagnosisTreatmentsByServiceRecID(long PatientID,long? ServiceRecID, long? PtRegistrationID,DateTime? ExamDate);
        public abstract DiagnosisTreatment GetDiagnosisTreatmentsBySerRecID(long ServiceRecID);
        public abstract IList<DiagnosisTreatment> GetAllDiagnosisTreatments();
        public abstract IList<DiagnosisTreatment> GetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID = null);
        public abstract IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt,long? V_Behaving, int PageIndex,int PageSize,out int TotalCount);

        public abstract IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID_InPt(long patientID, long? V_Behaving, int PageIndex, int PageSize, out int TotalCount);


        public abstract DiagnosisTreatment GetDiagnosisTreatmentByDTItemID(long DTItemID);
        public abstract DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest);

        public abstract DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID_InPt(long patientID, long? V_DiagnosisType);
        //==== #001
        public abstract DiagnosisTreatment GetLatestDiagnosisTreatment_InPtByInstructionID(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID);
        //==== #001

        public abstract bool CheckDiagnosisTreatmentExists_PtRegDetailID(long patientID, long PtRegDetailID);

        public abstract long? spGetPtRegistrationIDInDiagnosisTreatment_Latest(long patientID, long PtRegistrationID);

        public abstract DiagnosisTreatment DiagnosisTreatment_GetLast(long PatientID, long PtRegistrationID, long ServiceRecID);

        public abstract DiagnosisTreatment GetDiagnosisTreatment_InPt(long ServiceRecID);

        public abstract DiagnosisTreatment GetBlankDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode);
        public abstract bool DeleteDiagnosisTreatment(DiagnosisTreatment entity);
        /*▼====: #001*/
        //public abstract bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServiceRecID);
        public abstract bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServiceRecID, out long DTItemID);
        /*▲====: #001*/
        public abstract bool AddDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, out List<InPatientDeptDetail> ReloadInPatientDeptDetails);
        public abstract bool AddDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, SmallProcedure aSmallProcedure, out long SmallProcedureID);

        public abstract bool UpdateDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML);

        public abstract bool UpdateDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML);
        public abstract bool UpdateDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, SmallProcedure aSmallProcedure);

        public abstract IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load(long? ServiceRecID,long? PatientID,bool Last);

        public abstract IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load_InPt(long DTItemID);

        public abstract IList<DiagnosisICD9Items> GetDiagnosisICD9Items_Load_InPt(long DTItemID);
        //MedicalRecordTemplate
        //MedRecTemplate
        public abstract IList<MedicalRecordTemplate> GetAllMedRecTemplates();
     
        //Diagnosis Diag
        //Treatment Trmt
   

        #endregion

        #region 2.PatientMedicalRecords
        public abstract bool DeletePMR(PatientMedicalRecord entity);
        public abstract bool AddPMR(long ptID, string ptRecBarCode);
        public abstract bool UpdatePMR(PatientMedicalRecord entity);

        public abstract IList<PatientMedicalRecord> GetPMRsByPtID(long? patientID, int? inclExpiredPMR);
        public abstract PatientMedicalRecord GetPMRByPtID(long? patientID);
        public abstract PatientMedicalRecord PatientMedicalRecords_ByPatientID(long patientID);

        public abstract bool PatientMedicalRecords_Save(long PatientRecID, long PatientID, string NationalMedicalCode, out string Error);
      
        #endregion

        #region thao tac tren file

        public abstract PatientMedicalFile PatientMedicalFiles_ByID(long ServiceRecID);

        public abstract IList<PatientMedicalFile> PatientMedicalFiles_ByPatientRecID(long PatientRecID);

        public abstract bool CheckExists_PatientMedicalFiles(long ServiceRecID);

        public abstract bool Insert_PatientMedicalFiles(PatientMedicalFile entity,out long PatientRecID);

        public abstract bool PatientMedicalFiles_Update(PatientMedicalFile entity);

        public abstract bool PatientMedicalFiles_Delete(PatientMedicalFile entity, long StaffID);

        public abstract bool PatientMedicalFiles_Active(PatientMedicalFile entity);

        public abstract bool WriteFileXML(string path,string FileName,List<StringXml> Contents);

        public abstract List<StringXml> ReadFileXML(string FullPath);
        #endregion


        //Dinh them
        #region CLS

        public abstract IList<PatientServiceRecord> GetAllPatientServiceRecord(long patientID);

        protected new List<PatientServiceRecord> GetPatientServiceRecordCollectionFromReader(IDataReader reader)
        {
            List<PatientServiceRecord> lst = new List<PatientServiceRecord>();
            while (reader.Read())
            {
                lst.Add(GetPatientServiceRecordObjFromReader(reader));
            }
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

        public abstract IList<PatientPCLRequest> GetAllPatientPCLReqServiceRecID(long ServiceRecID, long V_PCLMainCategory);
        protected new List<PatientPCLRequest> GetPatientPCLRequestCollectionFromReader(IDataReader reader)
        {
            List<PatientPCLRequest> lst = new List<PatientPCLRequest>();
            while (reader.Read())
            {
                lst.Add(GetPatientPCLRequestObjFromReader(reader));
            }
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


        //---- In patient admission
        public abstract InPatientAdmDisDetails GetInPatientAdmDisDetails(long PtRegistrationID);

        protected new List<InPatientAdmDisDetails> GetInPatientAdmDisDetailsCollectionFromReader(IDataReader reader)
        {
            List<InPatientAdmDisDetails> lst = new List<InPatientAdmDisDetails>();
            while (reader.Read())
            {
                lst.Add(GetInPatientAdmissionFromReader(reader));
            }
            return lst;
        }
      
        public abstract bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity);
        public abstract bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity);

        #endregion

        //HPT 08/02/2017: Giấy chuyển tuyến
        public abstract TransferForm SaveTransferForm(TransferForm entity);

        public abstract IList<TransferForm> GetTransferForm(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID); /*TMA*/

        public abstract IList<TransferForm> GetTransferForm_Date(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID, DateTime FromDate, DateTime ToDate); /*TMA 20/11/2017*/

        public abstract bool DeleteTransferForm(long TransferFormID, long StaffID);
    }
}


