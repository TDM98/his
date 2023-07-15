using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMS.Services.Core;
/*
 * 20201012 #001 TNHX: Init
 */
namespace aEMR.ViewContracts
{
    /// <summary>
    /// Hiển thị cửa sổ PatientDetails để Thêm BN, Quản lý BH của BN.
    /// </summary>
    public interface IPatientPaymentCardManagement
    {
        Patient CurrentPatient { get; set; }
    }
}
