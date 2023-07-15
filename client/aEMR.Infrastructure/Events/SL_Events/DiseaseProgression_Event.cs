using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aEMR.Infrastructure.Events
{
    public class DiseaseProgression_Event
    {
        public string SelectedDiseaseProgression { get; set; }
    }
    public class DiseaseProgression_Event_Save
    {
        public long FormType { get; set; }
        public object Result { get; set; }
    }
    public class DiseaseProgressionInstruction_Event
    {
        public string SelectedDiseaseProgression { get; set; }
    }
    public class DiseaseProgressionFromTicketCare_Event
    {
        public string SelectedDiseaseProgression { get; set; }
    }
}
