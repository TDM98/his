using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientRegistration_HighTechService
    {
        bool IsLoadingBill { get; }
        string StatusText { get; }
        bool IsProcessing { get;}
        bool IsEditing { get; set; }
        string EditingBillingInvoiceTitle { get; }
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        IInPatientSelectService InPatientSelectServiceContent { get; set; }
        IInPatientSelectPcl SelectPCLContent { get; set; }
        IDrugListing SelectDrugContent { get; set; }
        IMedItemListing MedItemContent { get; set; }
        IChemicalListing ChemicalItemContent { get; set; }

        PatientRegistration CurRegistration { get; set; }
        IInPatientBillingInvoiceDetailsListing EditingInvoiceDetailsContent { get; set; }
        
        //KMx: Chuyển từ View IInPatientBillingInvoiceListing -> IInPatientBillingInvoiceListingNew (13/09/2014 16:54).
        //IInPatientBillingInvoiceListing OldBillingInvoiceContent { get; set; }
        IInPatientBillingInvoiceListingNew OldBillingInvoiceContent { get; set; }

        string DeptLocTitle { get; set; }

        bool UsedByTaiVuOffice { get; set; }
    }
}
