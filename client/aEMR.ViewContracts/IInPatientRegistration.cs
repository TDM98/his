using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientRegistration
    {
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

        bool IsHighTechServiceBill { get; set; }

        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

        bool ShowInPackageColumn { get; set; }
        bool IsNewCreateBill { get; set; }
    }
}
