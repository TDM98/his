using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;



namespace aEMR.Common.DataValidation
{
    public class ValidationExtensions
    {
        public static bool ValidateObject<T>(T item, out ObservableCollection<ValidationResult> ValidationResults)
        {
            ValidationContext vc = new ValidationContext(item, null, null) { };
            ValidationResults = new ObservableCollection<ValidationResult>();
            bool isValid = Validator.TryValidateObject(item, vc, ValidationResults, true);
            return isValid;
        }
    }
}
