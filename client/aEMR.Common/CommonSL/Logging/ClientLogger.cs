using Castle.Core.Logging;


namespace aEMR.Common
{
    public static class ClientLoggerHelper
    {        
        public static dynamic ClientLogger { get; private set; }
        
        private static ILogger _thelogger;

        static ClientLoggerHelper()
        {            
        }

        public static void InitLogger(ILogger theLogger)
        {
            _thelogger = theLogger;
        }

        //static void InitLogger()
        //{
        //    nInitThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        //    _clientLoggerAvailable = false;
        //    bRunOutOfBrowser = Application.Current.IsRunningOutOfBrowser;

        //    if (AutomationFactory.IsAvailable)
        //    {
        //        try
        //        {
        //            ClientLogger = AutomationFactory.CreateObject("eHCMS.ActiveX.Logging.AxClientLogger");
        //            if (ClientLogger != null)
        //            {
        //                _clientLoggerAvailable = true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ClientLoggerHelper.LogInfo(ex.ToString());
        //        }
        //    }
        //    else
        //    {
        //        var retCode = HtmlPage.Window.Invoke("initLogger");
        //        if ((bool)retCode)
        //        {
        //            _clientLoggerAvailable = true;
        //        }
        //    }
        //    if (!_clientLoggerAvailable)
        //    {
        //        //Thong bao download ClientLogger activeX ve su dung.
        //    }
        //}

        public static void LogDebug(string sInput, string userName = null)
        {
            if (userName != null && userName.Length > 1)
            {
                _thelogger.Info(sInput + " - " + userName);
            }
            else
            {
                _thelogger.Info(sInput);
            }
        }
        //public static void LogDebug(string sInput, string userName = null)
        //{
        //    if (userName == null && Globals.LoggedUserAccount != null)
        //    {
        //        userName = Globals.LoggedUserAccount.AccountName;
        //    }
        //    if (_clientLoggerAvailable)
        //    {
        //        var callingMethodInfo = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
        //        string strCallingMethodClassInfo = " [" + callingMethodInfo.ReflectedType.Name + "." + callingMethodInfo.ToString() + "] ";

        //        if (bRunOutOfBrowser)
        //        {
        //            if (ClientLogger != null)
        //            {
        //                if (nInitThreadID != System.Threading.Thread.CurrentThread.ManagedThreadId)
        //                {
        //                    Deployment.Current.Dispatcher.BeginInvoke(() => { ClientLogger.LogDebug(strCallingMethodClassInfo + sInput, userName); });
        //                }
        //                else
        //                {
        //                    ClientLogger.LogDebug(strCallingMethodClassInfo + sInput, userName);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (nInitThreadID != System.Threading.Thread.CurrentThread.ManagedThreadId)
        //            {
        //                Deployment.Current.Dispatcher.BeginInvoke(() => { HtmlPage.Window.Invoke("logDebug", strCallingMethodClassInfo + sInput, userName); });
        //            }
        //            else
        //            {
        //                HtmlPage.Window.Invoke("logDebug", strCallingMethodClassInfo + sInput, userName);
        //            }
        //        }
        //    }
        //    /*
        //    else
        //    {
        //        throw new Exception("Không tìm thấy ClientLogger");
        //    }
        //     * */
        //}

        public static void LogInfo(string sInput, string userName = null)
        {
            if (userName != null && userName.Length > 1)
            {
                _thelogger.Info(sInput + " - " + userName);
            }
            else
            {
                _thelogger.Info(sInput);
            }
        }

        //public static void LogInfo(string sInput, string userName = null)
        //{
        //    if (userName == null && Globals.LoggedUserAccount != null)
        //    {
        //        userName = Globals.LoggedUserAccount.AccountName;
        //    }

        //    if (_clientLoggerAvailable)
        //    {
        //        var callingMethodInfo = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
        //        string strCallingMethodClassInfo = " [" + callingMethodInfo.ReflectedType.Name + "." + callingMethodInfo.ToString() + "] ";

        //        if (bRunOutOfBrowser)
        //        {
        //            if (ClientLogger != null)
        //            {
        //                if (nInitThreadID != System.Threading.Thread.CurrentThread.ManagedThreadId)
        //                {
        //                    Deployment.Current.Dispatcher.BeginInvoke(() => { ClientLogger.LogInfo(strCallingMethodClassInfo + sInput, userName); });
        //                }
        //                else
        //                {
        //                    ClientLogger.LogInfo(strCallingMethodClassInfo + sInput, userName);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (nInitThreadID != System.Threading.Thread.CurrentThread.ManagedThreadId)
        //            {
        //                Deployment.Current.Dispatcher.BeginInvoke(() => { HtmlPage.Window.Invoke("logInfo", strCallingMethodClassInfo + sInput, userName); });
        //            }
        //            else
        //            {
        //                HtmlPage.Window.Invoke("logInfo", strCallingMethodClassInfo + sInput, userName);
        //            }
        //        }
        //    }
        //    /*
        //    else
        //    {
        //        throw new Exception("Không tìm thấy ClientLogger");
        //    }
        //     * */
        //}

        public static void LogError(string sInput, string userName = null)
        {
            if (userName != null && userName.Length > 1)
            {
                _thelogger.Error(sInput + " - " + userName);
            }
            else
            {
                _thelogger.Error(sInput);
            }
        }

        //public static void LogError(string sInput, string userName = null)
        //{
        //    if (userName == null && Globals.LoggedUserAccount != null)
        //    {
        //        userName = Globals.LoggedUserAccount.AccountName;
        //    }

        //    if (_clientLoggerAvailable)
        //    {
        //        var callingMethodInfo = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
        //        string strCallingMethodClassInfo = " [" + callingMethodInfo.ReflectedType.Name + "." + callingMethodInfo.ToString() + "] ";

        //        if (bRunOutOfBrowser)
        //        {
        //            if (ClientLogger != null)
        //            {
        //                if (nInitThreadID != System.Threading.Thread.CurrentThread.ManagedThreadId)
        //                {
        //                    Deployment.Current.Dispatcher.BeginInvoke(() => { ClientLogger.LogError(strCallingMethodClassInfo + sInput, userName); });
        //                }
        //                else
        //                {
        //                    ClientLogger.LogError(strCallingMethodClassInfo + sInput, userName);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (nInitThreadID != System.Threading.Thread.CurrentThread.ManagedThreadId)
        //            {
        //                Deployment.Current.Dispatcher.BeginInvoke(() => { HtmlPage.Window.Invoke("logError", strCallingMethodClassInfo + sInput, userName); });
        //            }
        //            else
        //            {
        //                HtmlPage.Window.Invoke("logError", strCallingMethodClassInfo + sInput, userName);
        //            }
        //        }
        //    }
        //    /*
        //    else
        //    {
        //        throw new Exception("Không tìm thấy ClientLogger");
        //    }
        //     * */
        //}

        public static void SaveFile(string clientFullPath, byte[] data)
        {
        }

        //public static void SaveFile(string clientFullPath, byte[] data)
        //{

        //    if (Application.Current.IsRunningOutOfBrowser)
        //    {
        //        if (ClientLogger != null)
        //        {
        //            ClientLogger.SaveFile(clientFullPath, data);
        //        }
        //    }
        //    else
        //    {
        //        //Chuyen sang base64 dua xuong javascript.
        //        string base64 = System.Convert.ToBase64String(data);
        //        HtmlPage.Window.Invoke("saveFile", clientFullPath, base64);
        //    }
        //}

        public static void DeleteFile(string fullFileName)
        {
        }

        //public static void DeleteFile(string fullFileName)
        //{

        //    if (Application.Current.IsRunningOutOfBrowser)
        //    {
        //        if (ClientLogger != null)
        //        {
        //            ClientLogger.DeleteFile(fullFileName);
        //        }
        //    }
        //    else
        //    {
        //        HtmlPage.Window.Invoke("deleteFile", fullFileName);
        //    }
        //}

        public static void CreateFolder(string fullFolderName)
        {
        }

        //public static void CreateFolder(string fullFolderName)
        //{

        //    if (Application.Current.IsRunningOutOfBrowser)
        //    {
        //        if (ClientLogger != null)
        //        {
        //            ClientLogger.CreateFolder(fullFolderName);
        //        }
        //    }
        //    else
        //    {
        //        HtmlPage.Window.Invoke("createFolder", fullFolderName);
        //    }
        //}

        public static void CopyFile(string sourceFileName, string destFileName)
        {
        }

        //public static void CopyFile(string sourceFileName, string destFileName)
        //{

        //    if (Application.Current.IsRunningOutOfBrowser)
        //    {
        //        if (ClientLogger != null)
        //        {
        //            ClientLogger.CopyFile(sourceFileName, destFileName);
        //        }
        //    }
        //    else
        //    {
        //        HtmlPage.Window.Invoke("copyFile", sourceFileName, destFileName);
        //    }
        //}

        public static void MapDrive(string localDriveName, string shareName, string userName, string password)
        {
        }
        //public static void MapDrive(string localDriveName, string shareName,string userName,string password)
        //{

        //    if (Application.Current.IsRunningOutOfBrowser)
        //    {
        //        if (ClientLogger != null)
        //        {
        //            ClientLogger.MapDrive(localDriveName, shareName,userName,password);
        //        }
        //    }
        //    else
        //    {
        //        HtmlPage.Window.Invoke("mapDrive", localDriveName, shareName, userName, password);
        //    }
        //}

        public static bool ConnectServer(string userName, string password)
        {
            return true;
        }

        //public static bool ConnectServer(string userName, string password)
        //{
        //    if (Application.Current.IsRunningOutOfBrowser)
        //    {
        //        if (ClientLogger != null)
        //        {
        //            dynamic retVal = ClientLogger.ConnectServer(userName, password);
        //            return (bool)retVal;
        //        }
        //    }
        //    else
        //    {
        //        object obj = HtmlPage.Window.Invoke("ConnectServer", userName, password);
        //        return (bool)obj;
        //    }
        //    return false;
        //}

        public static bool FolderExists(string fullFolderName)
        {
            return true;
        }

        //public static bool FolderExists(string fullFolderName)
        //{
        //    if (Application.Current.IsRunningOutOfBrowser)
        //    {
        //        if (ClientLogger != null)
        //        {
        //            dynamic retVal = ClientLogger.FolderExists(fullFolderName);
        //            return (bool)retVal;
        //        }
        //    }
        //    else
        //    {
        //        object obj = HtmlPage.Window.Invoke("folderExists", fullFolderName);
        //        return (bool)obj;
        //    }
        //    return false;
        //}

        public static bool FileExists(string fullFileName)
        {
            return false;
        }

        //public static bool FileExists(string fullFileName)
        //{
        //    if (Application.Current.IsRunningOutOfBrowser)
        //    {
        //        if (ClientLogger != null)
        //        {
        //            dynamic retVal = ClientLogger.FileExists(fullFileName);
        //            return (bool)retVal;
        //        }
        //    }
        //    else
        //    {
        //        object obj = HtmlPage.Window.Invoke("FileExists", fullFileName);
        //        return (bool)obj;
        //    }
        //    return false;
        //}
    }
}
