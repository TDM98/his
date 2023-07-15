using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.ComponentModel;
using System.Collections.Generic;


/*********************************
*
* Created by: Trinh Thai Tuyen
* Date Created: Tuesday, August 31, 2010
* Last Modified Date: 
*   
*********************************/

namespace aEMR.Common.DataValidation
{
    /// <summary>
    /// This is the base class for data validation
    /// </summary>
    public partial class ErrorNotifierBase:INotifyDataErrorInfo
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        // Adds the specified error to the errors collection if it is not already 
        // present, inserting it in the first position if isWarning is false. 
        public void AddError(string propertyName, string error, bool isWarning)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                if (isWarning)
                    _errors[propertyName].Add(error);
                else
                    _errors[propertyName].Insert(0, error);
                RaiseErrorsChanged(propertyName);
            }
        }

        // Removes the specified error from the errors collection if it is present. 
        public void RemoveError(string propertyName, string error)
        {
            if (_errors.ContainsKey(propertyName) && _errors[propertyName].Contains(error))
            {
                _errors[propertyName].Remove(error);
                if (_errors[propertyName].Count == 0) 
                    _errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);

            }
        }
        public void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #region INotifyDataErrorInfo Members

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName)) 
                return null;
            return _errors[propertyName];
        }

        public bool HasErrors
        {
            get
            {
                return _errors.Count > 0; 
            }
        }

        #endregion
    }
}
