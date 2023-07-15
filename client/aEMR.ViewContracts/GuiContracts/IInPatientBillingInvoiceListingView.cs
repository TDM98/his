
namespace aEMR.ViewContracts
{
    public interface IInPatientBillingInvoiceListingView
    {
        void ShowEditColumn(bool bShow);
        void ShowInfoColumn(bool bShow);
        void ShowRecalcHiColumn(bool bShow);
        void ShowRecalcHiWithPriceListColumn(bool bShow);
        void ExpandDetailsInfo();
    }
}
