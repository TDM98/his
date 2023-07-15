using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface ISearchAppointments
    {
        IAppointmentListing AppointmentListingContent { get; set; }

        void SearchAppointmentsCmd();
        AppointmentSearchCriteria SearchCriteria { get; set; }
        bool IsConsultation { get; set; }
        bool IsStaff { get; set; }
        string TitleForm { get; set; }

    }
}
