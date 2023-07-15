/*
 * 20170606 #001 CMN: Added variable to check HI follow TT04
*/
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using DataEntities;
using DevExpress.Xpf.Printing;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Hiển thị danh sách các item đã đăng ký của một bill trong đăng ký nội trú.
    /// </summary>
    public interface IInPatientBillingInvoiceDetailsListing
    {
        IEnumListing BillingTypeContent { get; set; }
        bool CanEditOnGrid { get; set; }
        InPatientBillingInvoice BillingInvoice { get; set; }
        bool IsLoading { get; set; }
        /// <summary>
        /// Lấy danh sách chi tiết các dịch vụ của BillingInvoice.
        /// </summary>
        /// <returns></returns>
        void LoadDetails();
        void LoadDetails(Action<InPatientBillingInvoice> callback);
        bool ShowDeleteColumn { get; set; }
        bool ShowHIAppliedColumn { get; set; }
        void ResetView();
        void AddItemToView(MedRegItemBase item);
        void RemoveItemFromView(MedRegItemBase item);

        bool CanDelete { get; set; }
        bool ValidateInfo(out ObservableCollection<ValidationResult> validationResults);

        double? HIBenefit { get; set; }
        bool CanShowPopupToModifyPrice { get; set; }

        bool ShowInPackageColumn { get; set; }

        bool IsHighTechServiceBill { get; set; }

        bool IsHICard_FiveYearsCont_NoPaid { get; set; }

        bool CheckTotalBillInvoice();

        InPatientBillingInvoice OldBillingInvoice { get; set; }

        bool CheckDifferentBillingInvoice();

        decimal CalcTotalPatientPayment();

        ObservableCollection<CharitySupportFund> CurrentSupportFunds { get; set; }

        void ModOfUpdatableItemPriceDone();

        /*==== #001 ====*/
        bool IsNotPermitted { get; set; }
        /*==== #001 ====*/
        PatientRegistration CurentRegistration { get; set; }
        void NotifyOfCanApplyIsOnPriceDiscount();
        bool IsNewCreateBill { get; set; }
        void RecacucateAllItemValues();
        bool CanEditHICount { get; set; }
        bool IsQuotationView { get; set; }
        Visibility IsShowPatientCOVID { get; set; }
        bool IsEnableCountSE { get; set; }
        decimal PaymentCeilingForTechService { get; set; }
    }
}
