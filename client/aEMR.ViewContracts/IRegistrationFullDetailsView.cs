using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMS.Services.Core;
using eHCMSCommon.Utilities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Hiển thị chi tiết, đầy đủ các thông tin về 1 đăng ký
    /// </summary>
    public interface IRegistrationFullDetailsView
    {
        /// <summary>
        /// Content chứa các dịch vụ Thuốc
        /// </summary>
        IOutPatientDrugManage DrugContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ CLS 
        /// </summary>
        IOutPatientPclRequestManage PclContent { get; set; }
        
        /// <summary>
        /// Content chứa các dịch vụ 
        /// </summary>
        IOutPatientServiceManage ServiceContent { get; set; }

        /// <summary>
        /// Chứa thông tin các lần trả tiền.
        /// </summary>
        IPatientPayment PaymentContent { get; set; }
        
        PatientRegistration CurrentRegistration { get; set; }

        /// <summary>
        /// Đang lấy thông tin đăng ký từ server
        /// </summary>
        bool RegistrationLoading { get; set; }

        void SetRegistration(PatientRegistration registrationInfo);

        void LoadRegistrationById(RegistrationSummaryInfo RegistrationSummaryInfo);
        /// <summary>
        /// Có đang sử dụng bảo hiểm hay không.
        /// </summary>
        bool HiServiceBeingUsed { get; set; }

        bool ShowButtonList { get; set; }
        bool ShowCheckBoxColumn { get; set; }
    }
}
