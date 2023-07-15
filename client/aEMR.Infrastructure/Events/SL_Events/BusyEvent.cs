namespace aEMR.Infrastructure.Events
{
    public class BusyEvent
    {

        public bool IsBusy { get; set; }

        public string Message { get; set; }
        
    }
    public class AppCheckAndDownloadUpdateCompletedEvent 
    {

    }

    public class HideBusyIndicatorEvent
    {

    }

}
