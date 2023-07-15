using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ITransmissionMonitorEdit
    {
        TransmissionMonitor CurTransmissionMonitor { get; set; }
        void InitTransmissionMonitor(bool IsNew);
    }
}