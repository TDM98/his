using FluentValidation.Results;

namespace aEMR.Common
{
    public interface IValidatable
    {
        bool IsValid { get; }
        ValidationResult Validate();
    }
}
