using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IMonitoringVitalSigns
    {
        InPatientInstruction gInPatientInstruction { get; set; }
        bool EnableSaveCmd { get; set; }
        // 20210610 TNHX: 331 Dựa vào mạch, huyết áp của "y lệnh theo dõi sinh hiệu" của y lệnh gần nhất để biết có cần nhập lại DHST không
        ObservableCollection<Lookup> ListV_ReconmendTime { get; set; }
    }
}