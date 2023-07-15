using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPay
    {
        IRegistrationSummaryV2 RegistrationInfoContent { get; set; }
        PatientRegistration Registration { get; set; }
        PaymentFormMode FormMode { get; set; }

        void PayCmd();
        bool CanPayCmd { get;}

        void SaveAndPayCmd();
        void SetRegistration(object registrationInfo);
        void LoadRegistration(long registrationID);
    }
}
