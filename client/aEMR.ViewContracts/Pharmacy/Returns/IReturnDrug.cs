using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IReturnDrug
    {
        SearchOutwardInfo SearchCriteria { get; set; }
        void btnSearch();
        bool IsChildWindow { get; set; }
        string TitleForm { get; set; }
        //void GetCurrentInvoiceInfo(OutwardDrugInvoice Current);
        bool? bFlagStoreHI { get; set; }
        bool bFlagPaidTime { get; set; }
    }
}
