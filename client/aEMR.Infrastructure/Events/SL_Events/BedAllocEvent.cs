using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.Infrastructure.Events
{
    public class BedAllocEvent 
    {

    }
    public class BedPatientDischarge
    {
        public BedPatientAllocs BedPatientDischargeItem { get; set; }
    }

    public class SetNewBedForUCBedAllocGridViewModel
    {
        public ObservableCollection<BedAllocation> BedAllocationsList { get; set; }
    }
}
