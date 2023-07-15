using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IAuthorization
    {
        string LoginName { get; set; }

        string Password { get; set; }
        bool IsRemembered{ get; set; }

        bool RememberMe{ get; set; }
        DeptLocation DeptLocation { get; set; }
    }
}
