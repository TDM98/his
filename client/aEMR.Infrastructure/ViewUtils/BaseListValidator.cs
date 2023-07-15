using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Castle.Windsor;
using FluentValidation;
using FluentValidation.Results;

namespace aEMR.Infrastructure.ViewUtils
{
    public abstract class BaseListValidator<TDataTransfer, TValidator, TViewModel> : BaseListViewModel<TDataTransfer>, IDataErrorInfo
        where TDataTransfer : class 
        where TValidator : AbstractValidator<TViewModel>, new()
        where TViewModel : class
    {
        protected BaseListValidator(IWindsorContainer container) : base(container)
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
                    return null;

                var validationResults = Validate();
                if (validationResults == null) return string.Empty;
                var columnResults = validationResults.Errors
                    .FirstOrDefault(x => string.Equals(x.PropertyName, columnName));
                return columnResults != null ? columnResults.ErrorMessage : string.Empty;
            }
        }

        public bool IsValid
        {
            get { return Validate().IsValid; }
        }

        public FluentValidation.Results.ValidationResult Validate()
        {
            IValidator<TViewModel> validator = new TValidator();
            return validator.Validate(this);
        }

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
    }
}
