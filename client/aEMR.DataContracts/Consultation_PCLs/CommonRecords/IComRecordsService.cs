using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using System.Collections.ObjectModel;
/*
 * 20230201 #001 DatTB: Thêm trường dữ liệu về KSNK trong phần Thông tin chung NB nội trú
 */
namespace CommonRecordsService
{
    [ServiceContract]
    public interface ICommonRecords
    {
        #region Common

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupVitalSignDataType(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupVitalSignDataType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupVitalSignContext(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupVitalSignContext(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupPMHStatus(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupPMHStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiseasessReferences(AsyncCallback callback, object state);
        List<DiseasesReference> EndGetDiseasessReferences(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiseasessRefByICD10Code(string icd10Code, AsyncCallback callback, object state);
        List<DiseasesReference> EndGetDiseasessRefByICD10Code(IAsyncResult asyncResult);

        #endregion

        #region 1.VitalSigns

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllVitalSigns(AsyncCallback callback, object state);
        IList<VitalSign> EndGetAllVitalSigns(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteVitalSigns(byte vitalSignID, AsyncCallback callback, object state);
        bool EndDeleteVitalSigns(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddVitalSigns(VitalSign entity, AsyncCallback callback, object state);
        bool EndAddVitalSigns(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateVitalSigns(VitalSign entity, AsyncCallback callback, object state);
        bool EndUpdateVitalSigns(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetVitalSignsByPtID(long patientID, AsyncCallback callback, object state);
        IList<PatientVitalSign> EndGetVitalSignsByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteItemPtVitalSigns(PatientVitalSign entity, long? staffID, AsyncCallback callback, object state);
        bool EndDeleteItemPtVitalSigns(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddItemPtVitalSigns(PatientVitalSign entity, long? staffID, AsyncCallback callback, object state);
        bool EndAddItemPtVitalSigns(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateItemPtVitalSigns(PatientVitalSign entity, long oldPtVSignID, long? staffID, AsyncCallback callback, object state);
        bool EndUpdateItemPtVitalSigns(IAsyncResult asyncResult);

        #endregion

        #region 2.Medical Conditions

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefMedCondType(AsyncCallback callback, object state);
        IList<RefMedContraIndicationTypes> EndGetRefMedCondType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefMedConditions(AsyncCallback callback, object state);
        IList<RefMedContraIndicationICD> EndGetRefMedConditions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefMedConditionsByType(int medCondTypeID, AsyncCallback callback, object state);
        IList<RefMedContraIndicationICD> EndGetRefMedConditionsByType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMedConditionByPtID(long patientID, int mcTypeID, AsyncCallback callback, object state);
        IList<MedicalConditionRecord> EndGetMedConditionByPtID(IAsyncResult asyncResult);

        //Manipulate on refMedicalConditionType
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRefMedCondType(RefMedContraIndicationTypes entity, AsyncCallback callback, object state);
        bool EndDeleteRefMedCondType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddRefMedCondType(RefMedContraIndicationTypes entity, AsyncCallback callback, object state);
        bool EndAddRefMedCondType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRefMedCondType(RefMedContraIndicationTypes entity, AsyncCallback callback, object state);
        bool EndUpdateRefMedCondType(IAsyncResult asyncResult);

        //Manipulate on refMedicalConditions
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRefMedCond(RefMedContraIndicationICD entity, AsyncCallback callback, object state);
        bool EndDeleteRefMedCond(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddRefMedCond(RefMedContraIndicationICD entity, AsyncCallback callback, object state);
        bool EndAddRefMedCond(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRefMedCond(RefMedContraIndicationICD entity, AsyncCallback callback, object state);
        bool EndUpdateRefMedCond(IAsyncResult asyncResult);

        //Manipulate on MedicalConditionRecords
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteMedCondRecs(MedicalConditionRecord entity, AsyncCallback callback, object state);
        bool EndDeleteMedCondRecs(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddMedCondRecs(MedicalConditionRecord entity, AsyncCallback callback, object state);
        bool EndAddMedCondRecs(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateMedCondRecs(MedicalConditionRecord entity, AsyncCallback callback, object state);
        bool EndUpdateMedCondRecs(IAsyncResult asyncResult);

        //Dinh them
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteMedReCond(long? MCRecID, long? StaffID, long? CommonMedRecID, AsyncCallback callback, object state);
        bool EndDeleteMedReCond(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewMedReCond(MedicalConditionRecord entity, long? StaffID, AsyncCallback callback, object state);
        bool EndAddNewMedReCond(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateMedReCond(MedicalConditionRecord entity, long? MCRecID, long? StaffID, AsyncCallback callback, object state);
        bool EndUpdateMedReCond(IAsyncResult asyncResult);

        #endregion

        #region 3.Medical History
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefMedHistory(AsyncCallback callback, object state);
        IList<RefMedicalHistory> EndGetRefMedHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPastMedCondHisByPtID(long patientID, AsyncCallback callback, object state);
        IList<PastMedicalConditionHistory> EndGetPastMedCondHisByPtID(IAsyncResult asyncResult);

        //Manipulate on RefMedicalHistory
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRefMedicalHistory(RefMedicalHistory entity, AsyncCallback callback, object state);
        bool EndDeleteRefMedicalHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddRefMedicalHistory(RefMedicalHistory entity, AsyncCallback callback, object state);
        bool EndAddRefMedicalHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRefMedicalHistory(RefMedicalHistory entity, AsyncCallback callback, object state);
        bool EndUpdateRefMedicalHistory(IAsyncResult asyncResult);

        //Manipulate on PastMedicalConditionHistory
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePastMedCondHis(PastMedicalConditionHistory entity, AsyncCallback callback, object state);
        bool EndDeletePastMedCondHis(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddPastMedCondHis(PastMedicalConditionHistory entity, AsyncCallback callback, object state);
        bool EndAddPastMedCondHis(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePastMedCondHis(PastMedicalConditionHistory entity, AsyncCallback callback, object state);
        bool EndUpdatePastMedCondHis(IAsyncResult asyncResult);

        //Dinh them
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteMedicalHistory(long? PMHID, long? StaffID, long? CommonMedRecID, AsyncCallback callback, object state);
        bool EndDeleteMedicalHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewMedicalHistory(PastMedicalConditionHistory entity, long? StaffID, AsyncCallback callback, object state);
        bool EndAddNewMedicalHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateMedicalHistory(PastMedicalConditionHistory entity, long? StaffID, long? PMHID, AsyncCallback callback, object state);
        bool EndUpdateMedicalHistory(IAsyncResult asyncResult);
        #endregion

        #region 4.Immunization
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefImmunization(long MedSerID, AsyncCallback callback, object state);
        IList<RefImmunization> EndGetRefImmunization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetImmunizationByPtID(long patientID, AsyncCallback callback, object state);
        IList<ImmunizationHistory> EndGetImmunizationByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteImmuHis(ImmunizationHistory entity, AsyncCallback callback, object state);
        bool EndDeleteImmuHis(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddImmuHis(ImmunizationHistory entity, AsyncCallback callback, object state);
        bool EndAddImmuHis(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateImmuHis(ImmunizationHistory entity, AsyncCallback callback, object state);
        bool EndUpdateImmuHis(IAsyncResult asyncResult);

        //Dinh them
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteImmunization(long? IHID, long? StaffID, long? CommonMedRecID, AsyncCallback callback, object state);
        bool EndDeleteImmunization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewImmunization(ImmunizationHistory entity, long? StaffID, AsyncCallback callback, object state);
        bool EndAddNewImmunization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateImmunization(ImmunizationHistory entity, long? IHID, long? StaffID, AsyncCallback callback, object state);
        bool EndUpdateImmunization(IAsyncResult asyncResult);
        #endregion

        #region 5.Hospitalization
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupAdmissionType(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupAdmissionType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupAdmissionReason(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupAdmissionReason(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupReferralType(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupReferralType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupTreatmentResult(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupTreatmentResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupDischargeReason(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupDischargeReason(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupHospitalType(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupHospitalType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetHospitalizationHistoryByPtID(long patientID, AsyncCallback callback, object state);
        IList<HospitalizationHistory> EndGetHospitalizationHistoryByPtID(IAsyncResult asyncResult);

        //Manupulate on HospitalizationHistory
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteHospitalization(long? HHID, long? StaffID, long? CommonMedRecID, AsyncCallback callback, object state);
        bool EndDeleteHospitalization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewHospitalization(HospitalizationHistory entity, long? StaffID,AsyncCallback callback, object state);
        bool EndAddNewHospitalization(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateHospitalization(HospitalizationHistory entity, long? HHID, long? StaffID, AsyncCallback callback, object state);
        bool EndUpdateHospitalization(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetHospitalByKey(string SearchKey, long V_HospitalType, int PageSize, int PageIndex, AsyncCallback callback, object state);
        IList<Hospital> EndGetHospitalByKey(IAsyncResult asyncResult);

        #endregion

        #region 6.Family History
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupFamilyRelationship(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupFamilyRelationship(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetFamilyHistoryByPtID(long patientID, AsyncCallback callback, object state);
        IList<FamilyHistory> EndGetFamilyHistoryByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteFamilyHistory(long? StaffID, long? FHCode, long? CommonMedRecID, AsyncCallback callback, object state);
        bool EndDeleteFamilyHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewFamilyHistory(FamilyHistory entity, long? staffID, AsyncCallback callback, object state);
        bool EndAddNewFamilyHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateFamilyHistory(FamilyHistory entity, long? StaffID
                                                            , long? FHCode,AsyncCallback callback, object state);
        bool EndUpdateFamilyHistory(IAsyncResult asyncResult);

        #endregion

        #region 7.Physical Examination
        //refernecese SummryServices
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePhysicalExamination(PhysicalExamination entity, long? StaffID
                                                            , long? PhyExamID,AsyncCallback callback, object state);
        bool EndUpdatePhysicalExamination(IAsyncResult asyncResult);

        #endregion

        #region 8. Risk Factor

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRiskFactorInsert(RiskFactors p, AsyncCallback callback, object state);
        bool EndRiskFactorInsert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRiskFactorDelete(long RiskFactorID, AsyncCallback callback, object state);
        bool EndRiskFactorDelete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRiskFactorGet(long? PatientID, AsyncCallback callback, object state);
        List<RiskFactors> EndRiskFactorGet(IAsyncResult asyncResult);

        //▼==== #001
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInfectionControlByPatientID(long? PatientID, int BacteriaType, long? InPatientAdmDisDetailID, AsyncCallback callback, object state);
        List<InfectionControl> EndGetInfectionControlByPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveInfectionControl(InfectionControl p, AsyncCallback callback, object state);
        bool EndSaveInfectionControl(IAsyncResult asyncResult);
        //▲==== #001
        #endregion
    }
}
