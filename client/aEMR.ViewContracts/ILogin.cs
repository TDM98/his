using System.Windows.Controls;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ILogin
    {
        string LoginName { get; set; }
        void SetActive();

        string ThePassword { get; set; }
        bool IsRemembered{ get; set; }

        PasswordBox PwdBoxCtrl { get; set; }

        bool RememberMe{ get; set; }
        DeptLocation DeptLocation { get; set; }

        bool IsDevLogin { get; set; }
        void DevAutoLogin();
        bool IsConfirmForSecretary { get; set; }
    }
}
