using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPatientAppointments
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IAppointmentListing AppointmentListingContent { get; set; }
        AppointmentSearchCriteria SearchCriteria { get; set; }
        void SetCurrentPatient(Patient patient, Prescription aCurrentPrescription = null);
        bool IsPCLBookingView { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        long CurrentPtRegDetailAppointmentID { get; set; }
        bool IsShowSearchRegistrationButton { get; set; }
    }
}