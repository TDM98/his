using System;
using DataEntities;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Su dung cho autocomplete box
    /// </summary>
    public interface IPatientRegistrationDetailsListing
    {
        PatientRegistration SelectedRegistration { get; set; }
        ObservableCollection<PatientRegistrationDetail> RegistrationDetails { get; }
        PatientRegistrationDetail SelectedRegistrationDetails { get; }
        Int64 PtRegistrationID { get; set; }
    }
}
