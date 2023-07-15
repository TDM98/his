using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientBedRegDetailListing
    {
        ObservableCollection<BedPatientRegDetail> AllItems { get;}
        long BedPatientID { get; set; }
        bool IsLoading { get; set; }
        void LoadData();
    }
}
