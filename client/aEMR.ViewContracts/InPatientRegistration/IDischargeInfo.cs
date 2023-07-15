using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using DataEntities;
/*
 * 20170927 #001 CMN: Added DeadReason
*/
namespace aEMR.ViewContracts
{
    /// <summary>
    /// Thong tin Xuat vien
    /// </summary>
    public interface IDischargeInfo
    {
        ObservableCollection<DiagnosisIcd10Items> AllExtraDiagnosisIcd10Items { get; set; }
        PatientRegistration Registration { get; set; }

        IMinHourDateControl DischargeDateContent { get; set; }


        IEnumListing DischargeTypeContent { get; set; }

        /*▼====: #001*/
        IEnumListing DeadReasonContent { get; set; }
        /*▲====: #001*/

        IEnumListing DischargeConditionContent { get; set; }

        DeceasedInfo DeceasedInfo { get; set; }
        IEnumListing ReasonOfDeceasedContent { get; set; }

        Iicd10Listing MainReasonOfDeceasedInfoContent { get; set; }
        Iicd10Listing PostMorternInfoContent { get; set; }

        bool IsMortem { get; set; }
        //string PostMortemExamDiagnosis { get; set; }
        //string PostMortemExamCode { get; set; }
        DateTime? DateOfDecease { get; set; }
        IMinHourDateControl DateOfDeceaseContent { get; set; }

        bool HasDeceaseInfo { get; set; }
        /// <summary>
        /// Tạo mới thông tin chuyển viện
        /// </summary>
        void CreateNew();
        void Reset();

        /// <summary>
        /// Phan thong tin nhap vien.
        /// </summary>
        IInPatientBedPatientAllocListing PatientAllocListingContent { get; set; }
        IInPatientDeptListing InPatientDeptListingContent { get; set; }

        //KMx: Chuyển từ View IInPatientBillingInvoiceListing -> IInPatientBillingInvoiceListingNew (13/09/2014 16:54).
        //IInPatientBillingInvoiceListing BillingInvoiceListingContent { get; set; }
        IInPatientBillingInvoiceListingNew BillingInvoiceListingContent { get; set; }
        bool isNotPayment { get; set; }

        RefDepartment SelectedDischargeDepartment { get; set; }

        void SetDischargeDeptSelection(List<RefDepartment> curInPtDepts, bool enableAllRespDepts);

        bool IsConsultation { get; set; }
        bool gIsReported { get; set; }
        TransferForm CurrentTransferForm { get; set; }
        bool HasPregnancyTermination { get; }

        bool CheckValidForDischaregePaper();
    }
}
