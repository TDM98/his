using System;
using System.Reflection;
using System.ComponentModel;
using aEMR.Common.Utilities;
using aEMR.Common.BaseModel;
/*
 * 20221007 #001 DatTB: Thêm cấu hình máy in cho soạn thuốc trước
 */
namespace aEMR.Common.Printing
{
    public class PrinterInfo :ModelBase
    {
        new public event PropertyChangedEventHandler PropertyChanged;
        new public void VerifyPropertyName(string propertyName)
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
        new protected void RaisePropertyChanged(string propertyName)
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
        [Description("Máy in khác")]
        IN_KHAC = 2,
        [Description("Máy in nhiệt")]
        IN_NHIET = 3,
        //▼==== #001
        [Description("Máy in phiếu soạn thuốc")]
        IN_SOAN_THUOC = 4,
        [Description("Máy in nhiệt (Hướng dẫn dùng thuốc)")]
        IN_NHIET_HDDT = 5
        //▲==== #001
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
