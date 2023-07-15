using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IHospitalClientEdit
    {
        HospitalClient CurrentHospitalClient { get; set; }
        bool IsCompleted { get; set; }
    }
}