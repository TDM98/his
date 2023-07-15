using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.ComponentModel.DataAnnotations;
using System.Windows.Data;
using System.Windows.Input;
using aEMR.DataContracts;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface IErrorBold
    {
        bool isCheckBox { get; set; }
        string ErrorTitle { get; set; }
        string ErrorMessage { get; }

        string ErrorHeader { get; set; }

        void SetError(AxErrorEventArgs axError);

        void SetMessage(string message, string _checkBoxContent);
        bool IsAccept { get; set; }

        void SetDeActivateCallback(System.Action<bool> deActivateCallback);
        bool FireOncloseEvent { get; set; }
        bool IsShowReason { get; set; }
        string Reason { get; set; }
        string TitleOkBtn { get; set; }
        string ErrorColor { get; set; }
    }
}
