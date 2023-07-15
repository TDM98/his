using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Quản lý danh sách các lần trả tiền của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    public interface IPatientPayment
    {
        ObservableCollection<PatientTransactionPayment> PatientPayments { get; set; }
        bool ShowPrintColumn { get; set; }
        void InitViewForPayments(PatientRegistration aRegistration);
    }
}