
namespace aEMR.Common.Utilities
{
    public class YieldValidationResult
    {
        private bool _isValid = false;
        public bool IsValid
        {
            get { return _isValid; }
            set { _isValid = value; }
        }
    }
}
