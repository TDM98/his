using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPatientAppointments_PCL_InConsultation
    {
        IAppointmentListing AppointmentListingContent { get; set; }
        AppointmentSearchCriteria SearchCriteria { get; set; }
        void SetCurrentPatient(Patient patient);
        bool IsCreateApptFromConsultation { get; set; }
    }
}