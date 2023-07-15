using aEMR.Infrastructure.Events;
using DataEntities;

/*
 * 20230502 QTD #001: Thêm mới View
 */

namespace aEMR.ViewContracts
{
    public interface ITreatmentProcessSummary
    {
        TreatmentProcess CurrentTreatmentProcess { get; set; }
        void SetCurrentTreatmentProcess(TreatmentProcess item);
        bool IsThisViewDialog { get; set; }
        void SetCurrentInformation(TreatmentProcessEvent item);
        bool IsNeedTreatmentSummary { get; set; }
    }
}