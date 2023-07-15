using DataEntities;
/*
 * 20181217 #001 TNHX: [BM0005436] Create button to Show report PhieuMienGiam
 */
namespace aEMR.ViewContracts
{
    public interface IPromoDiscountProgramEdit
    {
        PromoDiscountProgram PromoDiscountProgramObj { get; set; }
        bool IsUpdated { get; set; }
        bool IsViewOld { get; set; }
        //▼====: #001
        bool CanBtnViewPrint { get; set; }
        long PtRegistrationID { get; set; }
        long PatientPCLReqID { get; set; }
        //▲====: #001
    }
}