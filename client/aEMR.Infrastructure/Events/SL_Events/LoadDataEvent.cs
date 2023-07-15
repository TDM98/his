namespace aEMR.Infrastructure.Events
{
    public class LoadPatientPCLImagingResultDataCompletedEvent
    {
    }
    public class LoadDataCompleted<T>
    {
        public T Obj { get; set; }
    }
}