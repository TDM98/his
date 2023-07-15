using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IRefMedicalFileCheckIn
    {
        bool IsCheckIn { get; set; }
        int MaxWidth { get; set; }
    }
}
