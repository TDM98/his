using DataEntities;
using System.Collections.Generic;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System;

namespace aEMR.ViewContracts
{
    public interface IInPatientDailyDiagnosis
    {
        long IntPtDiagDrInstructionID { get; set; }
        //IEnumerator<IResult> SaveNewDiagnosis(long IntPtDiagDrInstructionID, long DeptID, bool IsUpdate = false);
        PatientRegistration PatientRegistrationContent { get; set; }
        bool CheckValidNewDiagnosis(long DeptID);
        void GetLatesDiagTrmtByPtID_InPt(long PatientID, long IntPtDiagDrInstructionID, long? V_DiagnosisType, bool IsLoadNew = false);
        void setDefaultValueWhenReNew();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void CreateNewFromOld(DiagnosisTreatment diagnosisTreatment, ObservableCollection<DiagnosisIcd10Items> ICD10lst);
        DiagnosisTreatment DiagnosisTreatmentContent { get; set; }
        ObservableCollection<DiagnosisIcd10Items> ICD10List { get; set; }
        ObservableCollection<DiagnosisIcd10Items> refIDC10ListCopy { get; set; }
        void SetValueWhenSaveAndUpdateDiag(long aIntPtDiagDrInstructionID, long DeptID);
        void GetLatestDiagnosisTreatment_InPtByInstructionID_V2(long PatientID, long IntPtDiagDrInstructionID, long? V_DiagnosisType, bool IsLoadNew = false);
        // 20210610 TNHX: 331 Dựa vào mạch, huyết áp của "y lệnh theo dõi sinh hiệu" của y lệnh gần nhất để biết có cần nhập lại DHST không
        InPatientInstruction gInPatientInstruction {get;set;}
        DateTime? MedicalInstructionDate { get; set; }
    }
}