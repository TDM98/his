using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface IIPCLResultGraph
    {
        IList<PCLExamTestItems> PCLExamTestItemCollection { get; set; }
    }
}