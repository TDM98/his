using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPCLResultSummary
    {
        bool IsHasDescription { get; set; }
        string CurrentDescription { get; set; }
        bool IsHtmlContent { get; set; }
        void InitPatientInfo(IRegistration_DataStorage aRegistration_DataStorage);
    }
}