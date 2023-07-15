using System;
using System.Collections.ObjectModel;
using System.IO;

namespace aEMR.Common.Printing
{
    public abstract class ActiveXPdfPrinterBase
    {
        /// <summary>
        /// Tên của máy in mà dữ liệu sẽ được in ra
        /// </summary>
        public string PrinterName { get; protected set; }
        public abstract void PrintPdfFile(string fileName);
        public abstract void PrintPdfData(byte[] data);
        public abstract void PrintPdfDataInBase64String(string base64String);

        public virtual void SetDefaultPrinter(string printerName)
        {
            //Do Nothing here.
        }

        public virtual bool InitPrintServer()
        {
            return false;
        }

        public byte[] GetStreamAsByteArray(Stream stream)
        {
            int streamLength = Convert.ToInt32(stream.Length);
            var fileData = new byte[streamLength];
            stream.Read(fileData, 0, streamLength);
            stream.Close();
            return fileData;
        }
    }

    /// <summary>
    /// Lớp này gọi hàm từ activeX AxPrinting (wrap thằng acrobat)
    /// </summary>
    //public class AdobeActiveXPrinter: ActiveXPdfPrinterBase
    //{
    //    public static bool PrinterServerAvailable { get; protected set; }
    //    private const string ActiveXPrinterName = "eHCMS.ActiveX.Printing.PrintEngine";
    //    private static readonly object LockObj = new object();
    //    static AdobeActiveXPrinter()
    //    {
    //        InitPrinter();
    //    }
    //    public AdobeActiveXPrinter()
    //    {
            
    //    }
    //    public AdobeActiveXPrinter(string strPrinterName)
    //    {
    //        PrinterName = strPrinterName;
    //    }
    //    public static dynamic PrintServer { get; private set; }
    //    static void InitPrinter()
    //    {
    //        lock (LockObj)
    //        {
    //            if (!PrinterServerAvailable)
    //            {
    //                if (AutomationFactory.IsAvailable)
    //                {
    //                    try
    //                    {
    //                        PrintServer = AutomationFactory.CreateObject(ActiveXPrinterName);
    //                        if (PrintServer != null)
    //                        {
    //                            PrinterServerAvailable = true;
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        ClientLoggerHelper.LogInfo(ex.ToString());
    //                    }
    //                }
    //                else
    //                {
    //                    object retCode = HtmlPage.Window.Invoke("initAdobePrinter");
    //                    if ((bool)retCode)
    //                    {
    //                        PrinterServerAvailable = true;
    //                    }
    //                } 
    //            }
    //        }
    //    }
    //    public override void PrintPdfFile(string fileName)
    //    {
    //        if (PrinterServerAvailable)
    //        {
    //            if (PrintServer != null)
    //            {
    //                PrintServer.PrintPdfFile(fileName);
    //            } 
    //            //Neu duyet bang trinh duyet thi khoi in luon.
    //        }
    //        else
    //        {
    //            throw new ActiveXPrintServerException("Không tìm thấy PrintServer");
    //        }
    //    }

    //    public override void PrintPdfData(byte[] data)
    //    {
    //        if (PrinterServerAvailable)
    //        {
    //            if (Application.Current.IsRunningOutOfBrowser)
    //            {
    //                if (PrintServer != null)
    //                {
    //                    PrintServer.PrintPdfData(data);
    //                }
    //            }
    //            else
    //            {
    //                //Chuyen sang base64 dua xuong javascript.
    //                string base64 = System.Convert.ToBase64String(data);
    //                HtmlPage.Window.Invoke("printBase64StringUseAdobePrinter", base64);
    //            } 
    //        }
    //        else
    //        {
    //            throw new ActiveXPrintServerException("Không tìm thấy PrintServer");
    //        }
    //    }

    //    public override void PrintPdfDataInBase64String(string base64String)
    //    {
    //        //TO DO: 
    //    }
    //    public override bool InitPrintServer()
    //    {
    //        return true;
    //    }

    //    public static ObservableCollection<PrinterInfo> GetAvailablePrinters()
    //    {
    //        string[] printers = null;

    //        if (PrinterServerAvailable)
    //        {
    //            if (PrintServer != null)
    //            {
    //                printers = PrintServer.GetAllPrinters();
    //            }
    //            else
    //            {
    //                var json = (string)HtmlPage.Window.Invoke("getAllPrintersInJSON");
    //                if(!string.IsNullOrWhiteSpace(json))
    //                {
    //                    try
    //                    {
    //                        var jsonArray = (JsonArray)JsonValue.Parse(json);
    //                        if(jsonArray != null && jsonArray.Count > 0)
    //                        {
    //                            printers = new string[jsonArray.Count];

    //                            for (int i = 0; i < jsonArray.Count;i++ )
    //                            {
    //                                printers[i] = jsonArray[i];
    //                            }
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        ClientLoggerHelper.LogInfo(ex.ToString());
    //                        printers = null;
    //                    }
    
    //                }
                    
    //            } 
    //        }
    //        else
    //        {
    //            throw new ActiveXPrintServerException("Không tìm thấy PrintServer");
    //        }

    //        var retVal = new ObservableCollection<PrinterInfo>();
    //        if (printers != null)
    //        {
    //            string defaultPrinter = GetDefaultPrinter();
    //            foreach (string s in printers)
    //            {
    //                var info = new PrinterInfo();
    //                info.PrinterName = s;
    //                info.IsDefaultPrinter = String.Compare(s, defaultPrinter, StringComparison.CurrentCultureIgnoreCase) == 0;

    //                retVal.Add(info);
    //            }
    //        }
    //        return retVal;
    //    }

    //    public static string GetDefaultPrinter()
    //    {
    //        string defaultPrinter = string.Empty;
    //        if (PrinterServerAvailable)
    //        {
    //            if (PrintServer != null)
    //            {
    //                defaultPrinter = PrintServer.GetDefaultPrinter();
    //            }
    //            else
    //            {
    //                defaultPrinter = (string)HtmlPage.Window.Invoke("getDefaultPrinter");
    //            }
    //        }
    //        else
    //        {
    //            throw new ActiveXPrintServerException("Không tìm thấy PrintServer");
    //        }

    //        return defaultPrinter;
    //    }
    //}

    /// <summary>
    /// Lớp này gọi hàm từ activeX anh Tuấn viết.
    /// </summary>
    public class AxonActiveXPrinter : ActiveXPdfPrinterBase
    {
        public static bool PrinterServerAvailable { get; protected set; }
        private const string ActiveXPrinterName = "eHCMS.PrintToolComServer.PrintPdfTool";
        private const int PrinterMode = 1;
        static AxonActiveXPrinter()
        {
            InitPrinter();
        }
        public AxonActiveXPrinter(string strPrinterName)
        {
            PrinterName = strPrinterName;
        }
        //public static dynamic PrintServer { get; private set; }

        private static PrintToolComServerLib.PrintPdfTool PrintServer { get; set; }

        //Initialize may in luon.
        static void InitPrinter()
        {
            lock (typeof(AxonActiveXPrinter))
            {
                try
                {
                    int initRetCode = -1;
                    if (!PrinterServerAvailable)
                    {
                        var autoPrintType = Type.GetTypeFromProgID(ActiveXPrinterName);
                        if (autoPrintType != null)
                        {
                            PrintServer = new PrintToolComServerLib.PrintPdfTool();
                            
                            if (PrintServer != null)
                            {
                                PrinterServerAvailable = true;
                                PrintServer.InitPrintServer(string.Empty, out initRetCode);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    PrinterServerAvailable = false;
                    throw new ActiveXPrintServerException("Không tìm thấy PrintServer hoặc không init được PrintServer");
                }
            }
        }

        /// <summary>
        /// Hàm này dùng để gọi InitPrintServer trong dll anh Tuấn
        /// </summary>
        public override bool InitPrintServer()
        {
            int retCode = -1;
            try
            {
                if (PrinterServerAvailable && PrintServer != null)
                {
                    PrintServer.InitPrintServer(PrinterName, out retCode);
                }
                else
                {
                    throw new ActiveXPrintServerException("Không tìm thấy PrintServer");
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
            return retCode == 0;
        }
        public override void PrintPdfFile(string fileName)
        {
            if (PrinterServerAvailable && PrintServer != null)
            {
                PrintServer.PrintFile(fileName, PrinterName, PrinterMode);
            }
            else
            {
                throw new ActiveXPrintServerException("Không tìm thấy PrintServer");
            }
        }

        public override void PrintPdfData(byte[] data)
        {
            if (PrinterServerAvailable && PrintServer != null)
            {
                PrintServer.PrintByteArray(data, PrinterName, PrinterMode);
            }
            else
            {
                throw new ActiveXPrintServerException("Không tìm thấy PrintServer");
            }
        }

        public override void PrintPdfDataInBase64String(string base64String)
        {
            if (PrinterServerAvailable && PrintServer != null)
            {
                PrintServer.PrintBase64Stream(base64String, PrinterName, PrinterMode);                    
            }
            else
            {
                throw new ActiveXPrintServerException("Không tìm thấy PrintServer");
            }
        }

        public static ObservableCollection<PrinterInfo> GetAvailablePrinters()
        {
            string[] printers = null;

            if (PrinterServerAvailable)
            {
                string json;
                if (PrintServer != null)
                {
                    json = PrintServer.GetPrinterInfoListRetString();

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        json = json.Replace("\\", "");
                        try
                        {
                            printers = json.Split(' ');
                        }
                        catch (Exception ex)
                        {
                            ClientLoggerHelper.LogInfo(ex.ToString());
                            printers = null;
                        }
                    }
                }
            }
            else
            {
                throw new ActiveXPrintServerException("Không tìm thấy PrintServer");
            }

            var retVal = new ObservableCollection<PrinterInfo>();
            if (printers != null)
            {
                string defaultPrinter = GetDefaultPrinter();
                foreach (string s in printers)
                {
                    var info = new PrinterInfo();
                    info.PrinterName = s;
                    info.IsDefaultPrinter = String.Compare(s, defaultPrinter, StringComparison.CurrentCultureIgnoreCase) == 0;

                    retVal.Add(info);
                }
            }
            return retVal;
        }

        public static string GetDefaultPrinter()
        {
            var defaultPrinter = string.Empty;
            //Tam thoi ben nay chua lay duoc Default Printer (anh Tuan lay)
            return defaultPrinter;
        }
    }

    public class ActiveXPrintServerException:Exception
    {
        public ActiveXPrintServerException():base(){}
        public ActiveXPrintServerException(string message) : base(message) { }
    }
}
