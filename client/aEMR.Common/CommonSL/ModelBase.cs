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
using System.Reflection;
using DataEntities;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace aEMR.Common.BaseModel
{
    public class ModelBase:INotifyPropertyChanged
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

        public static Action<string, string> ConstructorLogging;
        public static Action<string, string> DestructorLogging;
      
        public ModelBase()
        {
            _MyID = InstanceNo++;
//#if DEBUG
//            System.Diagnostics.Debug.WriteLine(string.Format("\"{0}({1}) created.\"", this.GetType().Name, _MyID));
//#endif
            if (ConstructorLogging != null)
            {
                ConstructorLogging(this.GetType().Name, _MyID.ToString());
            }
        }
        ~ModelBase()
        {
//#if DEBUG
//            System.Diagnostics.Debug.WriteLine(string.Format("\"{0}({1}) deleted.\"", this.GetType().Name, _MyID));
//#endif
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

    }
}
