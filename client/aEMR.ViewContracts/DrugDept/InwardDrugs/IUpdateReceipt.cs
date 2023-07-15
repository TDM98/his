using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IUpdateReceipt
    {
        InwardDrugMedDeptInvoice CurrentInwardDrugMedDeptInvoice { get; set; }
    }
}