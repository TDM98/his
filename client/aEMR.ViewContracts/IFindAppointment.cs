using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using DataEntities;
using aEMR.Common.Collections;
//using eHCMS.Services.Core;

namespace aEMR.ViewContracts
{
    public interface IFindAppointment
    {
        IEnumListing AppointmentStatusContent { get; set; }
        IAppointmentListingV2 AppointmentListingContent { get; set; }

        void CancelCmd();
        void OkCmd();
        void SearchCmd();
        void ResetFilterCmd();
        void DoubleClick(object args);

        bool IsLoading { get; set; }
        PatientAppointment SelectedAppointment { get; set; }
        AppointmentSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<PatientAppointment> Appointments{get;}

        void SelectPatientAndClose(object context);
        void CopyExistingAppointmentList(IList<PatientAppointment> items, AppointmentSearchCriteria criteria, int total);
    }

    public interface IFindAppointmentKSK
    {
        IAppointmentListingKSK AppointmentListingContent { get; set; }

        void CancelCmd();
        void OkCmd();
        void SearchCmd();
        void DoubleClick(object args);

        bool IsLoading { get; set; }
        PatientAppointment SelectedAppointment { get; set; }
        AppointmentSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<PatientAppointment> Appointments { get; }

        void SelectPatientAndClose(object context);
        void CopyExistingAppointmentList(IList<PatientAppointment> items, AppointmentSearchCriteria criteria, int total);
    }
}
