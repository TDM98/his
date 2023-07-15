
using aEMR.Infrastructure.Events;
namespace aEMR.ViewContracts
{
    public interface IConfirmHiBenefit
    {
        double OriginalHiBenefit { get; set; }
        bool OriginalIsCrossRegion { get; set; }

        double HiBenefit { get; set; }
        bool IsCrossRegion { get; set; }
        bool IsSameRegion { get; set; }
        long PatientId { get; set; }
        long HiId { get; set; }
        void OkCmd();
        void CancelCmd();
        void SetCrossRegion(bool isCrossRegion);
        ConfirmHiBenefitEvent confirmHiBenefitEvent { get; set; }
        bool CanEdit { get; set; }
        bool CanPressOKButton { get; set; }
        double RebatePercentageLevel1 { get; set; }
        bool IsAllowCrossRegion { get; set; }
        bool VisibilityCbxAllowCrossRegion { get; set; }

        int ShowOpts_To_OJR_Card { get; set; }      // Show Option Box with Radio buttons to O = Override, J = Join, R = Remove a HI Card From Registration 
        int Selected_OJR_Option { get; set; }       // Selected O or J or R of the Options above
        int PreSelected_OJR_Option { get; set; }    // Pre-selected (forced selection disallow user to select manually) of O or J or R option (thus all the OJR radio buttons are Read ONLY)
        
    }
}
