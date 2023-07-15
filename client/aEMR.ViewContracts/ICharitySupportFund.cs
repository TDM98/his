/*
#001: 20161216: CMN Begin: Add variables interface
*/
using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface ICharitySupportFund
    {
        decimal TotalBillToSupport { get;}
        bool? IsHighTechServiceBill { get; set; }
        long PtRegistrationID { get; set; }
        ObservableCollection<CharitySupportFund> SupportFunds { get; set; } // các dòng hỗ trợ cho bill thường (trong màn hình quyết toán) hoặc giải phẫu + kỹ thuật cao (trong màn hình tạo bill DVKTC)
        //==== #001
        ObservableCollection<CharitySupportFund> AllSupportFunds { get; set; }
        //==== #001
        //IInPatientBillingInvoiceDetailsListing EditingInvoiceDetailsContent { get; set; }
        IInPatientBillingInvoiceListingNew OldBillingInvoiceContent { get; set; }
        IInPatientBillingInvoiceListingNew BillingInvoiceListingContent { get; set; }
        IInPatientSettlement IParentScreen { get; set; }
        decimal TotalSupported { get; set; }
        decimal TotalSupported_HighTech { get; set; }
        void GetAllCharityOrganization();
        void GetCharitySupportFunds();
        void SaveCharitySupportFundForInPt(bool CanShowMessage = true);
        void RefreshFundInfo();
        bool IsModifiedSupportFund();
        bool CheckAndCalBeforeSave();
        InPatientBillingInvoice SelectedBillingInv { get; set; }
    }
    public interface ICharityOrganization
    {

    }
}
