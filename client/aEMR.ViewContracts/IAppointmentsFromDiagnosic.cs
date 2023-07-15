using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface IAppointmentsFromDiagnosic
    {
        IAppointmentListing AppointmentListingContent { get; set; }

        string TitleForm { get; set; }
    }
}
