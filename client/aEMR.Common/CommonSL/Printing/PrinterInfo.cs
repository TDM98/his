using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using eHCMSLanguage;
using System.ServiceModel;
using System.ComponentModel;
using DataEntities;
using System.Runtime.InteropServices.Automation;
using Microsoft.CSharp.RuntimeBinder;
using System.Net;
using eHCMS.Common.BaseModel;
using eHCMSCommon.Utilities;

namespace eHCMS.Common.Printing
{
    public class PrinterInfo //:ModelBase
    {
        public event PropertyChangedEventHandler PropertyChanged;
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
        protected void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("Please assign a property name");
            VerifyPropertyName(propertyName);
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _PrinterName;
        public string PrinterName
        {
            get
            {
                return _PrinterName;
            }
            set
            {
                _PrinterName = value;
                RaisePropertyChanged("PrinterName");
            }
        }
        private bool _IsDefaultPrinter;
        public bool IsDefaultPrinter
        {
            get
            {
                return _IsDefaultPrinter;
            }
            set
            {
                _IsDefaultPrinter = value;
                RaisePropertyChanged("IsDefaultPrinter");
            }
        }
        public override string ToString()
        {
            return _PrinterName;
        }
    }

    public enum PrinterType : int
    {
        [Description("Máy in phiếu yêu cầu")]
        IN_PHIEU = 0,
        [Description("Máy in hóa đơn cho bệnh nhân")]
        IN_HOA_DON = 1,
        [Description("Máy in hóa đơn cho BHYT")]
        IN_HOA_DON_BHYT = 2
    }

    /// <summary>
    /// Gan 1 ten may in cho 1 loai may in
    /// </summary>
    public class PrinterTypePrinterAssignment //:ModelBase
    {
        public event PropertyChangedEventHandler PropertyChanged;
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
        protected void RaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("Please assign a property name");
            VerifyPropertyName(propertyName);
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _printerTypeName;
        public string PrinterTypeName
        {
            get
            {
                return _printerTypeName;
            }
            private set
            {
                _printerTypeName = value;
                RaisePropertyChanged("PrinterTypeName");
            }
        }
        private PrinterType _printerType;
        public PrinterType PrinterType
        {
            get { return _printerType; }
            set
            {
                _printerType = value;
                RaisePropertyChanged("PrinterType");
                PrinterTypeName = Helpers.GetEnumDescription(_printerType);
            }
        }

        private string _assignedPrinterName;
        public string AssignedPrinterName
        {
            get { return _assignedPrinterName; }
            set
            {
                _assignedPrinterName = value;
                RaisePropertyChanged("AssignedPrinterName");
            }
        }
    }
}
