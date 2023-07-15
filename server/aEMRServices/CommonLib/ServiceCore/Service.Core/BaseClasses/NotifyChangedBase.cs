#if SILVERLIGHT
extern alias globalsilverlight;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
#if SILVERLIGHT
using globalsilverlight::System.ComponentModel;
#endif
using Service.Core.Common;

namespace eHCMS.Services.Core.Base
{
    /// <summary>
    /// Base class for all the classes implement INotifyPropertyChanged interface
    /// </summary>
    /// 
    [DataContract(IsReference=true)]
#if SILVERLIGHT
    public abstract class NotifyChangedBase : INotifyPropertyChanged, globalsilverlight::System.ComponentModel.INotifyDataErrorInfo
#else
    public abstract class NotifyChangedBase : INotifyPropertyChanged, INotifyDataErrorInfo
#endif
    {
        private static int InstanceNo = 0;
        private int _MyID;
        public int MyID
        {
            get
            {
                return _MyID;
            }
        }
        public static Func<DateTime> GetServerDate;
        public static Action<string,string> ConstructorLogging;
        public static Action<string, string> DestructorLogging;
        public NotifyChangedBase()
        {
            _MyID = InstanceNo++;
           if(ConstructorLogging != null)
           {
               ConstructorLogging(this.GetType().Name, _MyID.ToString());
           }
        }
        ~NotifyChangedBase()
        {
            if (DestructorLogging != null)
            {
                DestructorLogging(this.GetType().Name, _MyID.ToString());
            }
        }
#region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

#endregion

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("Please assign a property name");
            VerifyPropertyName(propertyName);
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            bool validProperty = false;
            try
            {
                PropertyInfo pInfo = this.GetType().GetProperty(propertyName);
                if (pInfo != null)
                    validProperty = true;
            }
            catch
            {
            }
            if (!validProperty)
            {
                string msg = "Invalid property name: " + propertyName;
                throw new Exception(msg);
            }
        }

        protected Dictionary<string, List<string>> GetError()
        {
            if (_errors == null)
            {
                _errors = new Dictionary<string, List<string>>();
            }
            return _errors;
        }

        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        // Adds the specified error to the errors collection if it is not already 
        // present, inserting it in the first position if isWarning is false. 
        public void AddError(string propertyName, string error, bool isWarning)
        {
            if (!GetError().ContainsKey(propertyName))
                GetError()[propertyName] = new List<string>();

            if (!GetError()[propertyName].Contains(error))
            {
                if (isWarning)
                    GetError()[propertyName].Add(error);
                else
                    GetError()[propertyName].Insert(0, error);
                RaiseErrorsChanged(propertyName);
            }
        }

        // Removes the specified error from the errors collection if it is present. 
        public void RemoveError(string propertyName, string error)
        {
            if (GetError().ContainsKey(propertyName) && GetError()[propertyName].Contains(error))
            {
                GetError()[propertyName].Remove(error);
                if (GetError()[propertyName].Count == 0)
                    GetError().Remove(propertyName);
                RaiseErrorsChanged(propertyName);

            }
        }
        public void RemoveError(string propertyName)
        {
            if (GetError().ContainsKey(propertyName))
            {
                GetError().Remove(propertyName);
                RaiseErrorsChanged(propertyName);

            }
        }

        public void AddErrorFromValidationResults(string propertyName, List<ValidationResult> results)
        {
            if (!GetError().ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();
            else
                _errors[propertyName].Clear();

            foreach (ValidationResult result in results)
            {
                _errors[propertyName].Add(result.ErrorMessage);
            }
            RaiseErrorsChanged(propertyName);
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
            {
#if SILVERLIGHT
                ErrorsChanged(this, new globalsilverlight::System.ComponentModel.DataErrorsChangedEventArgs(propertyName));
#else
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
#endif
            }
        }

#region INotifyDataErrorInfo Members
#if SILVERLIGHT
        public event EventHandler<globalsilverlight::System.ComponentModel.DataErrorsChangedEventArgs> ErrorsChanged;
#else
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
#endif

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName) || !GetError().ContainsKey(propertyName))
                return null;
            return _errors[propertyName];
        }

        public bool HasErrors
        {
            get
            {
                return GetError().Count > 0;
            }
        }
        private static object lockObj = new object();
        public virtual void ValidateProperty(string propertyName,object newVal)
        {
            lock (lockObj)
            {
                List<ValidationResult> validateResults = new List<ValidationResult>();
                ValidationContext context = new ValidationContext(this, null, null) { MemberName = propertyName };
                if (!Validator.TryValidateProperty(newVal, context, validateResults))
                {
                    AddErrorFromValidationResults(propertyName, validateResults);
                }
                else
                {
                    RemoveError(propertyName);
                } 
            }
        }

        /// <summary>
        /// Validate this object
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            List<ValidationResult> validateResults = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(this, null, null);

            if (!Validator.TryValidateObject(this, context, validateResults, true))
            {
                GetError().Clear();
                foreach (ValidationResult result in validateResults)
                {
                    AddError(result.MemberNames.FirstOrDefault(), result.ErrorMessage, false);
                }
                return false;
            }
            else
            {
                GetError().Clear();
                return true;
            }
        }
#endregion
    }

    [DataContract]
    public class EntityBase : NotifyChangedBase,IEntityState
    {
        public EntityBase()
        {

        }

#region IEntityState Members
        [DataMemberAttribute()]
        public virtual EntityState EntityState
        {
            get;
            set;
        }

        private bool _canCheck = true;
        [DataMemberAttribute()]
        public virtual bool CanCheck
        {
            get { return _canCheck; }
            set
            {
                if (_canCheck != value)
                {
                    _canCheck = value;
                    RaisePropertyChanged("CanCheck");
                }
            }
        }

        private bool _isChecked;
        [DataMemberAttribute()]
        public virtual bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if(_isChecked != value)
                {
                    _isChecked = value;
                    RaisePropertyChanged("IsChecked");
                }
            }
        }

        private bool _canEdit;
        [DataMemberAttribute()]
        public virtual bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                if (_canEdit != value)
                {
                    _canEdit = value;
                    RaisePropertyChanged("CanEdit");
                }
            }
        }


        private bool _canDelete;
        [DataMemberAttribute()]
        public virtual bool CanDelete
        {
            get
            {
                //if (EntityState == EntityState.DETACHED || EntityState == EntityState.NEW)
                //{
                //    return true;
                //}
                return _canDelete;
            }
            set
            {
                _canDelete = value;
                RaisePropertyChanged("CanDelete");
            }
        }
#endregion

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
