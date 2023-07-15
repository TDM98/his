using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TxD 17/05/2018 Commented out the following System namespaces 
//  and specifying them directly to the 2 types below:
//  1. ValidationAttribute
//  2. ValidationResult
// Because ValidationResult exists in both System.Windows.Controls and System.ComponentModel.DataAnnotations
// thus will lead to ambiguilty in resolving which namespace should be used 
//using System.Windows.Controls;
//using System.ComponentModel.DataAnnotations;

namespace aEMR.Common
{
    class AttributeValidationRule : System.Windows.Controls.ValidationRule
    {        
        private System.ComponentModel.DataAnnotations.ValidationAttribute attribute;
        private string propName;

        public AttributeValidationRule(System.ComponentModel.DataAnnotations.ValidationAttribute attribute, string propName)
        {
            this.attribute = attribute;
            this.propName = propName;
        }

        public override System.Windows.Controls.ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            try
            {
                this.attribute.Validate(value, propName);

                return new System.Windows.Controls.ValidationResult(true, null);
            }
            catch (FormatException fex)
            {
                return new System.Windows.Controls.ValidationResult(false, fex.Message);
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                return new System.Windows.Controls.ValidationResult(false, ex.Message);
            }
        }
    }
}
