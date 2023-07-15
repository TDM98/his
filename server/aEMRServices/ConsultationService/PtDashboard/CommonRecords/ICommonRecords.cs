using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-09-29
 * Contents: Consultation Services Iterfaces
/*******************************************************************/
#endregion

/*
* 20230201 #001 DatTB: Thêm trường dữ liệu về KSNK trong phần Thông tin chung NB nội trú
 */
namespace ConsultationsService.PtDashboard.CommonRecords
{
    [ServiceContract]
    public interface ICommonRecords
    {
        #region Common

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupVitalSignDataType();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupVitalSignContext();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupPMHStatus();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DiseasesReference> GetDiseasessReferences();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DiseasesReference> GetDiseasessRefByICD10Code(string icd10Code);

        #endregion

        #region 1.VitalSigns

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<VitalSign> GetAllVitalSigns();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteVitalSigns(byte vitalSignID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddVitalSigns(VitalSign entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateVitalSigns(VitalSign entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientVitalSign> GetVitalSignsByPtID(long patientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteItemPtVitalSigns(PatientVitalSign entity, long? staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddItemPtVitalSigns(PatientVitalSign entity, long? staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateItemPtVitalSigns(PatientVitalSign entity, long oldPtVSignID, long? staffID);

        #endregion

        #region 2.Medical Conditions

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedContraIndicationTypes> GetRefMedCondType();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedContraIndicationICD> GetRefMedConditions();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedContraIndicationICD> GetRefMedConditionsByType(int medCondTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<MedicalConditionRecord> GetMedConditionByPtID(long patientID, int mcTypeID);

        //Manipulate on refMedicalConditionType
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRefMedCondType(RefMedContraIndicationTypes entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddRefMedCondType(RefMedContraIndicationTypes entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateRefMedCondType(RefMedContraIndicationTypes entity);

        //Manipulate on refMedicalConditions
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRefMedCond(RefMedContraIndicationICD entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddRefMedCond(RefMedContraIndicationICD entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateRefMedCond(RefMedContraIndicationICD entity);

        //Manipulate on MedicalConditionRecords
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteMedCondRecs(MedicalConditionRecord entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddMedCondRecs(MedicalConditionRecord entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateMedCondRecs(MedicalConditionRecord entity);

        //Dinh them
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteMedReCond(long? MCRecID, long? StaffID, long? CommonMedRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewMedReCond(MedicalConditionRecord entity,long? StaffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateMedReCond(MedicalConditionRecord entity,long? MCRecID, long? StaffID);

        #endregion

        #region 3.Medical History
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalHistory> GetRefMedHistory();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PastMedicalConditionHistory> GetPastMedCondHisByPtID(long patientID);

        //Manipulate on RefMedicalHistory
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRefMedicalHistory(RefMedicalHistory entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddRefMedicalHistory(RefMedicalHistory entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateRefMedicalHistory(RefMedicalHistory entity);

        //Manipulate on PastMedicalConditionHistory
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePastMedCondHis(PastMedicalConditionHistory entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPastMedCondHis(PastMedicalConditionHistory entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePastMedCondHis(PastMedicalConditionHistory entity);
        
        //Dinh them
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteMedicalHistory(long? PMHID, long? StaffID, long? CommonMedRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewMedicalHistory(PastMedicalConditionHistory entity, long? StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateMedicalHistory(PastMedicalConditionHistory entity, long? StaffID, long? PMHID);
        #endregion

        #region 4.Immunization
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefImmunization> GetRefImmunization(long MedSerID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ImmunizationHistory> GetImmunizationByPtID(long patientID);

        //Manupulate on RefImmunization
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool DeleteImmunization(RefImmunization entity);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddImmunization(RefImmunization entity);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool UpdateImmunization(RefImmunization entity);

        //Manupulate on ImmunizationHistory
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteImmuHis(ImmunizationHistory entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddImmuHis(ImmunizationHistory entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateImmuHis(ImmunizationHistory entity);

        //Dinh them
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteImmunization(long? IHID, long? StaffID, long? CommonMedRecID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewImmunization(ImmunizationHistory entity,long? StaffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateImmunization(ImmunizationHistory entity,long? IHID, long? StaffID);
        #endregion

        #region 5.Hospitalization
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupAdmissionType();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupAdmissionReason();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupReferralType();

        [OperationContract]
        IList<Lookup> GetLookupTreatmentResult();

        [OperationContract]
        IList<Lookup> GetLookupDischargeReason();

        [OperationContract]
        IList<Lookup> GetLookupHospitalType();

        [OperationContract]
        IList<HospitalizationHistory> GetHospitalizationHistoryByPtID(long patientID);

        //Manupulate on HospitalizationHistory
        [OperationContract]
        bool DeleteHospitalization(long? HHID, long? StaffID, long? CommonMedRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewHospitalization(HospitalizationHistory entity
                                                            , long? StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateHospitalization(HospitalizationHistory entity, long? HHID, long? StaffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Hospital> GetHospitalByKey(string SearchKey, long V_HospitalType, int PageSize, int PageIndex);

        #endregion

        #region 6.Family History
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupFamilyRelationship();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<FamilyHistory> GetFamilyHistoryByPtID(long patientID);


        //Manupulate on FamilyHistory
        //[OperationContract]
        //CRUDOperationResponse DeleteFamilyHistory(FamilyHistory entity);

        //[OperationContract]
        //CRUDOperationResponse AddFamilyHistory(FamilyHistory entity);

        //[OperationContract]
        //CRUDOperationResponse UpdateFamilyHistory(FamilyHistory entity);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteFamilyHistory(long? StaffID, long? FHCode, long? CommonMedRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewFamilyHistory(FamilyHistory entity, long? staffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateFamilyHistory(FamilyHistory entity, long? StaffID
                                                            , long? FHCode);

        #endregion

        #region 7.Physical Examination
        //refernecese SummryServices
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePhysicalExamination(PhysicalExamination entity, long? StaffID, long? PhyExamID);
        #endregion

        #region 8. Risk Factor

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RiskFactorInsert(RiskFactors p);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RiskFactorDelete(long RiskFactorID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RiskFactors> RiskFactorGet(long? PatientID);

        //▼==== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveInfectionControl(InfectionControl p);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InfectionControl> GetInfectionControlByPatientID(long? PatientID, int BacteriaType, long? InPatientAdmDisDetailID);
        //▲==== #001

        #endregion
    }
}
