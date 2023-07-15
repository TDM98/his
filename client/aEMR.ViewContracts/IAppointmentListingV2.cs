using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Dùng để liệt kê danh sách cuộc hẹn theo 1 tiêu chuẩn tìm kiếm nào đó.
    /// </summary>
    public interface IAppointmentListingV2
    {
        void StartSearching();
        void DoubleClick(object args);

        bool IsLoading { get; set; }
        PatientAppointment SelectedAppointment { get; set; }
        AppointmentSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<PatientAppointment> Appointments { get; }
    }

    public interface IAppointmentListingKSK
    {
        void StartSearching();
        void DoubleClick(object args);

        bool IsLoading { get; set; }
        PatientAppointment SelectedAppointment { get; set; }
        AppointmentSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<PatientAppointment> Appointments { get; }
    }
}
