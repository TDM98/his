
namespace aEMR.ViewContracts
{
    public interface IInPatientBillingInvoiceListingNewView
    {
        //void ShowEditColumn(bool bShow);
        //void ShowInfoColumn(bool bShow);
        //void ShowRecalcHiColumn(bool bShow);
        //void ExpandDetailsInfo();
        void ShowColumn(string columnName, bool bShow);
    }
}
