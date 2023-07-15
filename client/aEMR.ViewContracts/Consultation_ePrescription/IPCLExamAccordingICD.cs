using DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aEMR.ViewContracts.Consultation_ePrescription
{
    public interface IPCLExamAccordingICD
    {
        List<PCLExamAccordingICD> ListPCLExamAccordingICD { get; set; }
    }
}
