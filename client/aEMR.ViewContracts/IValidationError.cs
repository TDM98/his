using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Data;
using System.Windows.Input;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface IValidationError
    {
        string ErrorTitle { get; set; }

        ObservableCollection<ValidationResult> ValidationErrors { get; set; }


        void SetErrors(ObservableCollection<ValidationResult> validationErrorCollection);
    }
}
