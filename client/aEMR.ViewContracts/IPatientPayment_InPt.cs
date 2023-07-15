using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.PagedCollectionView;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Quản lý danh sách các lần trả tiền của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    public interface IPatientPayment_InPt
    {
        PagedCollectionView PatientPayments { get; set; }
    }
}
