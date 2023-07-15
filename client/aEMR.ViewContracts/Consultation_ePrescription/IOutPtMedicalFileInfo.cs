using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IOutPtMedicalFileInfo
    {
        long PatientID { get; set; }
        long PtRegDetailID { get; set; }
        long OutPtTreatmentProgramID { get; set; }
        long InPtRegistrationID { get; set; }
        long DiagConsultationSummaryID { get; set; }
        long V_RegistrationType { get; set; }
        string OrientedTreatment { get; set; }
        string Diagnosis { get; set; }
        string ReasonHospitalStay { get; set; }
        string MainDisease { get; set; }
        string IncludingDisease { get; set; }
        bool IsChronic { get; set; }
        DiagnosisTreatment DiagTrmtItem { get; set; }
    }
}