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
 * Modified date: 2010-10-13
 * Contents: Consultation Services
/*******************************************************************/
#endregion
/*
 * 20182510 #001 TTM: BM 0002173: Thay đổi cách lưu/ cập nhật/ lấy dữ liệu của tình trạng thể chất từ lấy theo TTTC cuối cùng => theo đăng ký
 */
namespace eHCMS.DAL
{
    public abstract class CommonRecordsProvider:DataProviderBase
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
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Consultations.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.Consultations.CommonRecords.ProviderType);
                    _instance = (CommonRecordsProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }

        public CommonRecordsProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }

        //Retrieving data
        #region Common
        public abstract List<Lookup> GetLookupVitalSignDataType();
        public abstract List<Lookup> GetLookupVitalSignContext();
        public abstract List<Lookup> GetLookupPMHStatus();
        public abstract List<Lookup> GetLookupByObjectTypeID(LookupValues objectTypeID);
        public abstract List<DiseasesReference> GetDiseasessReferences();
        public abstract List<DiseasesReference> GetDiseasessRefByICD10Code(string icd10Code);

        /// <summary>
        /// Vital Sign
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
     

        #endregion

        #region 1.VitalSigns
        
        public abstract List<VitalSign> GetAllVitalSigns();
        public abstract bool DeleteVitalSigns(byte vitalSignID);
        public abstract bool AddVitalSigns(VitalSign entity);
        public abstract bool UpdateVitalSigns(VitalSign entity);

        public abstract List<PatientVitalSign> GetVitalSignsByPtID(long patientID);
        public abstract bool DeleteItemPtVitalSigns(PatientVitalSign entity, long? staffID);
        public abstract bool AddItemPtVitalSigns(PatientVitalSign entity, long? staffID);
        public abstract bool UpdateItemPtVitalSigns(PatientVitalSign entity, long oldPtVSignID, long? staffID);
      
        #endregion

        #region 2.Medical Conditions
        public abstract List<RefMedicalConditionType> GetRefMedCondType();
        public abstract List<RefMedicalCondition> GetRefMedConditions();
        public abstract List<RefMedicalCondition> GetRefMedConditionsByType(int medCondTypeID);
        public abstract List<MedicalConditionRecord> GetMedConditionByPtID(long patientID, int mcTypeID);

  
        //Manipulate on refMedicalConditionType
        public abstract bool DeleteRefMedCondType(RefMedicalConditionType entity);
        public abstract bool AddRefMedCondType(RefMedicalConditionType entity);
        public abstract bool UpdateRefMedCondType(RefMedicalConditionType entity);

      
        //Manipulate on refMedicalConditions
        public abstract bool DeleteRefMedCond(RefMedicalCondition entity);
        public abstract bool AddRefMedCond(RefMedicalCondition entity);
        public abstract bool UpdateRefMedCond(RefMedicalCondition entity);

        public abstract bool DeleteMedReCond(long? MCRecID, long? StaffID, long? CommonMedRecID);
        public abstract bool AddNewMedReCond(long? PatientID, long? StaffID
                                                    , long? CommonMedRecID, long? MCID, bool? MCYesNo
                                                    , string MCTextValue, String MCExplainOrNotes);
        public abstract bool UpdateMedReCond(long? MCRecID, long? StaffID, long? CommonMedRecID
                                                , long? MCID, bool? MCYesNo, string MCTextValue
                                                , string MCExplainOrNotes);
        
        //Manipulate on MedicalConditionRecords
        public abstract bool DeleteMedCondRecs(MedicalConditionRecord entity);
        public abstract bool AddMedCondRecs(MedicalConditionRecord entity);
        public abstract bool UpdateMedCondRecs(MedicalConditionRecord entity);

        #endregion

        #region 3.Medical History
        public abstract List<RefMedicalHistory> GetRefMedHistory();
        public abstract List<PastMedicalConditionHistory> GetPastMedCondHisByPtID(long patientID);

        //Manipulate on RefMedicalHistory
        public abstract bool DeleteRefMedicalHistory(RefMedicalHistory entity);
        public abstract bool AddRefMedicalHistory(RefMedicalHistory entity);
        public abstract bool UpdateRefMedicalHistory(RefMedicalHistory entity);
        //Manipulate on PastMedicalHistory
        
        //dinh them
        public abstract bool DeleteMedicalHistory(long? PMHID
                                                    , long? StaffID
                                                    , long? CommonMedRecID);
        public abstract bool AddNewMedicalHistory(long? PatientID
                                                   , long? StaffID
                                                   , long? CommonMedRecID
                                                   , long? MedHistCode
                                                   , bool? PMHYesNo
                                                   , string PMHExplainReason
                                                   , long? V_PMHStatus);
        public abstract bool UpdateMedicalHistory(long? StaffID
                                                   , long? PMHID
                                                   , long? CommonMedRecID
                                                   , long? MedHistCode
                                                   , bool? PMHYesNo
                                                   , string PMHExplainReason
                                                   , long? V_PMHStatus);

        //Manipulate on PastMedicalConditionHistory
        public abstract bool DeleteMedCondHis(PastMedicalConditionHistory entity);
        public abstract bool AddMedCondHis(PastMedicalConditionHistory entity);
        public abstract bool UpdateMedCondHis(PastMedicalConditionHistory entity);

        #endregion

        #region 4.Immunization

        public abstract List<RefImmunization> GetRefImmunization(long MedServiceID);
        public abstract List<ImmunizationHistory> GetImmunizationByPtID(long patientID);
        
        //Manupulate on RefImmunization
        public abstract bool DeleteImmunization(RefImmunization entity);
        public abstract bool AddImmunization(RefImmunization entity);
        public abstract bool UpdateImmunization(RefImmunization entity);

        public abstract bool DeleteImmunization(long? IHID 
                                                    , long? StaffID
                                                    , long?  CommonMedRecID);
        public abstract bool AddNewImmunization(long? PatientID 
                                                    , long? StaffID 
                                                    , long? IHCode
                                                   , long? CommonMedRecID 
                                                   , bool? IHYesNo 
                                                   , DateTime? IHDate );
        public abstract bool UpdateImmunization(long? IHID 
                                                    , long? StaffID 
                                                    , long? IHCode
                                                   , long?  CommonMedRecID 
                                                   , bool? IHYesNo 
                                                   , DateTime? IHDate );
     
        //Manupulate on ImmunizationHistory
        public abstract bool DeleteImmuHis(ImmunizationHistory entity);
        public abstract bool AddImmuHis(ImmunizationHistory entity);
        public abstract bool UpdateImmuHis(ImmunizationHistory entity);
        #endregion

        #region 5.Hospitalization
        public abstract List<HospitalizationHistory> GetHospitalizationHistoryByPtID(long patientID);
        public abstract List<Hospital> GetHospitalByKey(string SearchKey, long V_HospitalType, int PageSize, int PageIndex);

        public abstract void HospitalizationHistory_Delete(long? HHID, long? StaffID, long? CommonMedRecID);
        
        public abstract void HospitalizationHistory_Update(
                                                            long? HHID
                                                            , long? StaffID 
                                                                , long? CommonMedRecID
                                                              , long? IDCode
                                                                ,string HDate                                                  
                                                                , DateTime? FromDate
                                                              , long? FromHosID
                                                              , long? V_AdmissionType
                                                              , string GeneralDiagnoses
                                                              , long? V_TreatmentResult
                                                              , long? V_DischargeReason                                                              
                                                              , string HHNotes);
        public abstract void HospitalizationHistory_Insert(   
                                                            long? PatientID
                                                            ,long? StaffID
                                                            ,  long? CommonMedRecID
                                                              , long? IDCode
                                                            , string HDate
                                                              , DateTime? FromDate
                                                              , long? FromHosID
                                                              , long? V_AdmissionType
                                                              , string GeneralDiagnoses
                                                              , long? V_TreatmentResult
                                                              , long? V_DischargeReason
                                                              , string HHNotes);
        
        #endregion

        #region 6.Family History
        public abstract List<FamilyHistory> GetFamilyHistoryByPtID(long patientID);

        public abstract void FamilyHistory_Insert(long? PatientID
                                                        , long? StaffID
                                                        , long? CommonMedRecID                                                          
													    ,long? IDCode 												   
												       ,string FHFullName 
												       ,long? V_FamilyRelationship
												       ,string FHNotes 
												       ,bool? Decease );
        public abstract void FamilyHistory_Update(long? PatientID
                                                        , long? StaffID
                                                        , long? CommonMedRecID
                                                        , long? IDCode
                                                       , string FHFullName
                                                       , long? V_FamilyRelationship
                                                       , string FHNotes
                                                       , bool? Decease
                                                        , DateTime? CMRModifiedDate);
        public abstract void FamilyHistory_Delete(long? StaffID
                                                    , long? FHCode
                                                    , long? CommonMedRecID);
        //Manupulate on FamilyHistory
        public abstract bool DeleteFamilyHistory(RefImmunization entity);
        public abstract bool AddFamilyHistory(RefImmunization entity);
        public abstract bool UpdateFamilyHistory(RefImmunization entity);

        #endregion

        #region 7.Physical Examination
        //References to SummaryProvider
        public abstract void PhysicalExamination_Insert(long? PatientID
                                                           , long? StaffID
                                                           , PhysicalExamination p);
        //▼====== #001
        public abstract void PhysicalExamination_Insert_V2(long? PatientID, long PtRegistrationID, long V_RegistrationType, long? StaffID, PhysicalExamination p);
        public abstract void PhysicalExamination_Update_V2(long? StaffID , PhysicalExamination p);
        //▲====== #001
        public abstract void PhysicalExamination_Delete(long? StaffID, long? PhyExamID, long? CommonMedRecID);
        public abstract void PhysicalExamination_Update(    long? StaffID
                                                            , long? PhyExamID
                                                            , PhysicalExamination p);
        public abstract PhysicalExamination PhysicalExamination_GetData(long? CommonMedRecID);
        public abstract List<PhysicalExamination> PhysicalExamination_ListData(long? CommonMedRecID);
        #endregion

        #region 8. Risk Factor
        public abstract bool RiskFactorInsert(RiskFactors p);
        public abstract bool RiskFactorDelete(long RiskFactorID);
        public abstract List<RiskFactors> RiskFactorGet(long? PatientID);
        #endregion
    }
}
