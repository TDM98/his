using DataEntities;
namespace aEMR.Infrastructure.Events
{
    public class TreatmentProcessEvent
    {
        public TreatmentProcess Item { get; set; }

        public class OnChangedTreatmentProcess
        {
            public TreatmentProcess TreatmentProcess { get; set; }
        }
    }
}
