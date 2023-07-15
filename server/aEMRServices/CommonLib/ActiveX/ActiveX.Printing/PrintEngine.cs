//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Drawing.Printing;
//using System.Windows.Forms;
//using System.Runtime.InteropServices;

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Web.Script.Serialization;

namespace eHCMS.ActiveX.Printing
{
    [
        Guid("873355E1-2D0D-476f-9BEF-C7E645024C02"),
        ProgId("eHCMS.ActiveX.Printing.PrintEngine"),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(IPrint)),
        ComVisible(true)
    ]
    public class PrintEngine : IPrint, IObjectSafety
    {
        public PrintEngine()
        {
            _hiddenForm = new frmPrint();
        }
        #region IPrint Members

        public void PrintTest(string sInput)
        {
            MessageBox.Show(sInput);
        }

        public void PrintPdfFile(string fileName)
        {
            HiddenForm.PrintPdf(fileName);
        }
        #endregion

        private frmPrint _hiddenForm;
        public frmPrint HiddenForm
        {
            get
            {
                return _hiddenForm;
            }
        }

        #region IObjectSafety Members

        public ObjectSafetyRetVal GetInterfaceSafetyOptions(ref Guid riid, out ObjectSafetyOptions pdwSupportedOptions, out ObjectSafetyOptions pdwEnabledOptions)
        {
            ObjectSafetyOptions m_options = ObjectSafetyOptions.INTERFACESAFE_FOR_UNTRUSTED_CALLER | ObjectSafetyOptions.INTERFACESAFE_FOR_UNTRUSTED_DATA;
            pdwSupportedOptions = m_options;
            pdwEnabledOptions = m_options;

            return ObjectSafetyRetVal.S_OK;
        }

        public ObjectSafetyRetVal SetInterfaceSafetyOptions(ref Guid riid, ObjectSafetyOptions dwOptionSetMask, ObjectSafetyOptions dwEnabledOptions)
        {
            return ObjectSafetyRetVal.S_OK;
        }

        #endregion

        #region IPrint Members


        public string GetAllPrintersInJSON()
        {
            List<string> printers = new List<string>();

            foreach (var item in PrinterSettings.InstalledPrinters)
            {
                printers.Add(item.ToString());
            }
            //return printers.ToArray();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(printers);
        }

        public string[] GetAllPrinters()
        {
            List<string> printers = new List<string>();

            foreach (var item in PrinterSettings.InstalledPrinters)
            {
                printers.Add(item.ToString());
            }
            return printers.ToArray();
        }

        public string GetDefaultPrinter()
        {
            PrinterSettings p = new PrinterSettings();
            return p.PrinterName;
        }

        public void SetDefaultPrinter(string printerName)
        {
            PrinterHelper.SetDefaultPrinter(printerName);
        }

        public void PrintPdfData([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] byte[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                return;
            }

            string fName = Guid.NewGuid().ToString() + ".pdf";
            string folder = AppDomain.CurrentDomain.BaseDirectory;

            fName = Path.Combine(folder, fName);
            
            if (arr != null)
            {
                using (FileStream fs = new FileStream(fName, FileMode.Create))
                {
                    fs.Write(arr, 0, arr.Length);
                }
                
                PrintPdfFile(fName);
                File.Delete(fName);
            }
        }

        public void PrintPdfDataInBase64(string base64String)
        {
            if(string.IsNullOrWhiteSpace(base64String))
            { 
                return;
            }
            var data = System.Convert.FromBase64String(base64String);
            PrintPdfData(data);
        }
        #endregion
    }

    [
        Guid("E86A9038-368D-4e8f-B389-FDEF38935B2A"),
        InterfaceType(ComInterfaceType.InterfaceIsDual),
        ComVisible(true)
    ]
    public interface IPrint
    {
        [DispId(1)]
        void PrintTest(string sInput);

        [DispId(3)]
        void PrintPdfFile(string fileName);

        [DispId(4)]
        string GetAllPrintersInJSON();

        [DispId(5)]
        string GetDefaultPrinter();

        [DispId(6)]
        void SetDefaultPrinter(string printerName);

        [DispId(7)]
        void PrintPdfData([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] byte[] arr);

        [DispId(3)]
        void PrintPdfDataInBase64(string base64String);

        [DispId(9)]
        string[] GetAllPrinters();
    }




    public static class PrinterHelper
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);

    }

    [Flags]
    public enum ObjectSafetyOptions:int //DWORD
    {
        INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001,
        INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002,
        INTERFACE_USES_DISPEX = 0x00000004,
        INTERFACE_USES_SECURITY_MANAGER = 0x00000008
    };
    public enum ObjectSafetyRetVal : uint //HRESULT
    {
        S_OK = 0x0,
        E_NOINTERFACE = 0x80000004
    }
}
