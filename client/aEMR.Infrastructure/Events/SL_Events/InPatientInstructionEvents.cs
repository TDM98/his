using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.Infrastructure.Events
{
    public class CreateNewFromOld_ForInPatientInstruction
    {
        public DiagnosisTreatment DiagnosisTreatmentContent { get; set; }
        public ObservableCollection<DiagnosisIcd10Items> ICD10List { get; set; }
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> DrugList  { get; set; }
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> IntravenousDetails { get; set; }
        public long DoctorStaffID { get; set; }
}
    public class LoadInstructionFromWebsite
    {
        public long IntPtDiagDrInstructionID { get; set; }
    }
}