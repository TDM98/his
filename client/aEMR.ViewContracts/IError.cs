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
    public interface IError
    {
        string ErrorTitle { get; set; }
        string ErrorMessage { get; }

        string ErrorHeader { get; set; }

        void SetError(AxErrorEventArgs axError);
    }
}
