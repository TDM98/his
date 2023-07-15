using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using DataEntities;
using DevExpress.Xpf.Printing;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Hiển thị danh sách các loại thuốc (hoặc y cụ, hóa chất) đã được sử dụng của một đăng ký nội trú
    /// </summary>
    public interface IInPatientUsedMedProductListing
    {
        AllLookupValues.MedProductType MedProductType { get; set; }
        ObservableCollection<RefGenMedProductSummaryInfo> AllItems { get; set; }
        PatientRegistration Registration { get; set; }
        void LoadData(long? DeptID);
        bool IsLoading { get; set; }
        bool ValidateInfo(out ObservableCollection<ValidationResult> validationResults);
        /// <summary>
        /// Lấy những item có trạng thái bị thay đổi.
        /// </summary>
        /// <returns></returns>
        List<RefGenMedProductSummaryInfo> GetModifiedItems();

        /// <summary>
        /// Lấy những item có trạng thái bị thay đổi và số lượng trả > 0
        /// </summary>
        /// <returns></returns>
        List<RefGenMedProductSummaryInfo> GetReturnItems();
    }
}
