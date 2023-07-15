using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Castle.Windsor;
using aEMR.Common;
using FluentValidation;
using FluentValidation.Results;

namespace aEMR.Infrastructure.ViewUtils
{
    public abstract class BaseFormValidator<TDataTransfer> : BaseFormViewModel<TDataTransfer>, IDataErrorInfo, IValidatable
    {

        protected BaseFormValidator(IWindsorContainer container) : base(container)
        {
        }

        public bool ValidationEnabled { get; set; }
       
        public string Error
        {
            get { return GetError(Validate()); }
        }

        public string this[string columnName]
        {
            get
            {
                if (!ValidationEnabled)
                    return string.Empty;

                var validationResults = Validate();
                if (validationResults == null) return string.Empty;
                var columnResults = validationResults.Errors
                    .FirstOrDefault(x => string.Equals(x.PropertyName, columnName));
                return columnResults != null ? columnResults.ErrorMessage : string.Empty;
            }
        }

        public bool IsValid
        {
            get
            {
                var temp = ValidationEnabled;
                ValidationEnabled = true;
                var isValid = Validate().IsValid;
                ValidationEnabled = temp;
                return isValid;
            }
        }

        public abstract ValidationResult Validate();

        public static string GetError(ValidationResult result)
        {
            var validationErrors = new StringBuilder();
            foreach (var validationFailure in result.Errors)
            {
                validationErrors.Append(validationFailure.ErrorMessage);
                validationErrors.Append(Environment.NewLine);
            }
            return validationErrors.ToString();
        }

        public virtual void NotifyAllValidatedProperties()
        {
            
        }
    }
}
