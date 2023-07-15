
namespace aEMR.ViewContracts
{
    public interface IPatientTree
    {
        void InitPatientInfo();
        bool IsShowSummaryContent { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        bool IsCriterion_PCLResult { get; set; }
    }
}