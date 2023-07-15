using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ICheckedValidHICard
    {
        HealthInsurance gHealthInsurance { get; set; }
    }
}