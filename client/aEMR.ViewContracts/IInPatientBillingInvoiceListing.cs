using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientBillingInvoiceListing
    {
        void RefreshDetailsView(InPatientBillingInvoice inv);
        bool ShowEditColumn { get; set; }
        bool ShowInfoColumn { get; set; }
        bool ShowRecalcHiColumn { get; set; }
        bool ShowRecalcHiWithPriceListColumn { get; set; }
        bool ShowHIAppliedColumn { get; set; }
        bool IsLoading { get; set; }
        bool mTickSelect { get; set; }
        bool mExpanderDetail{ get; set; }
        IInPatientBillingInvoiceDetailsListing InvoiceDetailsContent { get; set; }
        InPatientBillingInvoice BeingViewedItem { get; set; }
        ObservableCollection<InPatientBillingInvoice> BillingInvoices { get; set; }
        bool ShowAll { get; set; }
        string GroupBy { get; set; }
        List<long> GetSelectedIds();
        List<InPatientBillingInvoice> GetSelectedItems();
        DataGridRowDetailsVisibilityMode DataGridRowDetailsVisibilityMode { get; set; }

        bool mDangKyNoiTru_SuaDV { get; set; }
        bool mDangKyNoiTru_XemChiTiet { get; set; }
    }
}
