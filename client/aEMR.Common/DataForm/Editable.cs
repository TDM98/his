using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.Common
{
    /// <summary>
    /// Specifies whether users should be able to change the value of the entity property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EditableAttribute : Attribute
    {
        /// <summary>
        /// Gets a value that indicates whether a client application should allow users to change the value of the property.
        /// </summary>
        public bool AllowEdit { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether users should be able to set a value for the property when adding a new record in the data set.
        /// </summary>
        public bool AllowInitialValue { get; set; }
    }
}
