using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IcwdDaysCheck
    {
        ConsultationRoomTarget curConsultationRoomTarget { get; set; }
        bool isUpdate { get; set; }
    }
}
