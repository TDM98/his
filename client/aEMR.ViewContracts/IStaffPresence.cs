using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IStaffPresence
    {
        bool IsUpdateRequiredNumber { get; set; }
    }
}
