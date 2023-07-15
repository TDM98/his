using aEMR.Common;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IConsultationOutstandingTask
    {
        void SearchRegistrationListForOST(long ExamRegStatus = (long)V_ExamRegStatus.mDangKyKham, long V_OutPtEntStatus =0);
        bool IsMedicalInstruction { get; set; }
    }

    public interface IInPatientOutstandingTask
    {
        SetOutStandingTask WhichVM { get; set; }
        //bool IsInstructionVisible { get; set; }
        bool IsShowListInPatientReturnRecord { get; set; }
        bool IsShowListInPatient { get; set; }
        bool IsShowListOutPatientList { get; set; }
        bool IsEnableDepartmentContent { get; set; }
        bool IsShowListPatientCashAdvance { get; set; }
        bool IsSearchForListPatientCashAdvance { get; set; }
    }
    public interface IInPatientAdmissionOutstandingTask
    {
    }
    public interface IInPatientInstructionOutstandingTask
    {
    }

    public interface IPatientsInTreatmentListViewModel
    {
    }
}