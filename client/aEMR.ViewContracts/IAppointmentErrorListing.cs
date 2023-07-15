using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IAppointmentErrorListing
    {
        ObservableCollection<PatientApptServiceDetails> RequestSeqNoFailedList{ get; set; }
        ObservableCollection<PatientApptServiceDetails> InsertFailedList { get; set; }
        ObservableCollection<PatientApptServiceDetails> DeleteFailedList { get; set; }
        
    }
}
