using System.Collections.ObjectModel;
using System.Windows.Controls;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ISearchPatientAndRegistrationView
    {
        void SetDefaultButton(Button btn);
        void InitButtonVisibility(SearchRegButtonsVisibility values);

        Button SearchRegistrationButton { get; }
        Button SearchPatientButton { get; }
        Button SearchAppointmentsButton { get; }
        Button NewPatientButton { get; }
    }
}
