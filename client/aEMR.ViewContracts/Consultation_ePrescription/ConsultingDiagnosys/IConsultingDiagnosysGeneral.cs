using DataEntities;
using System.Collections.Generic;
namespace aEMR.ViewContracts
{
    public interface IConsultingDiagnosysGeneral
    {
        ConsultingDiagnosys gConsultingDiagnosys { get; set; }
        List<Lookup> DiagnosticTypeArray { get; set; }
        List<Lookup> TreatmentMethodArray { get; set; }
        List<Lookup> HeartSurgicalTypeArray { get; set; }
        List<Lookup> ValveTypeArray { get; set; }
        void InitDataView();
    }
}