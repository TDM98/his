using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using DataEntities;
/*
 * 20180829 #001 TTM: Bổ sung HIBenefit để lấy giá trị làm điều kiện trong hàm CheckHIError trong InPatientBillingInvoiceDetailsListingViewModel.
 */
namespace aEMR.ViewContracts
{
    public interface IInPatientBillingInvoiceListingNew
    {
        //void RefreshDetailsView(InPatientBillingInvoice inv);
        bool ShowEditColumn { get; set; }
        bool ShowInfoColumn { get; set; }
        bool ShowRecalcHiColumn { get; set; }
        bool ShowRecalcHiWithPriceListColumn { get; set; }
        bool ShowCheckItemColumn { get; set; }
        bool ShowPrintBillColumn { get; set; }
        bool ShowHIAppliedColumn { get; set; }
        bool mTickSelect { get; set; }
        bool mExpanderDetail{ get; set; }
        //IInPatientBillingInvoiceDetailsListing InvoiceDetailsContent { get; set; }
        //InPatientBillingInvoice BeingViewedItem { get; set; }
        ObservableCollection<InPatientBillingInvoice> BillingInvoices { get; set; }
        bool ShowAll { get; set; }
        string GroupBy { get; set; }
        List<long> GetSelectedIds();
        List<InPatientBillingInvoice> GetSelectedItems();
        DataGridRowDetailsVisibilityMode DataGridRowDetailsVisibilityMode { get; set; }

        bool mDangKyNoiTru_SuaDV { get; set; }
        bool mDangKyNoiTru_XemChiTiet { get; set; }
        bool bShowTotalPrice { get; set; }
        //HPT: truyền RegLockFlag của đăng ký vào để ràng buộc lock các chức năng
        int RegLockFlag { get; set; }
        ObservableCollection<CharitySupportFund> CurrentSupportFund { get; set; }
        ObservableCollection<CharitySupportFund> SupportFund_ForHighTechServiceBill { get; set; }
        bool ShowSupportHpl { get; set; }
        PatientRegistration CurentRegistration { get; set; }
        //▼====== #001
        double? HIBenefit { get; set; }
        //▲====== #001
        bool IsNewCreateBill { get; set; }
    }
}