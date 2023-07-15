using System.Windows.Controls;
using aEMR.Common;

namespace aEMR.ViewContracts
{
    public interface IRegistrationLeftMenu
    {
        //void SetHyperlinkSelectedStyle(HyperlinkButton lnk);
        void InPatientProcessPaymentCmd(object source);
        void InPatientRegisterCmd(object source);
        eLeftMenuByPTType LeftMenuByPTType { get; set; }
    }

    public interface IRegistrationTopMenu
    {
        //void SetHyperlinkSelectedStyle(HyperlinkButton lnk);
        void InPatientProcessPaymentCmd(object source);
        void InPatientRegisterCmd(object source);
        eLeftMenuByPTType LeftMenuByPTType { get; set; }
    }

}
