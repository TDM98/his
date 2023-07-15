//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Drawing.Printing;
//using System.Windows.Forms;
//using System.Runtime.InteropServices;

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using AxLogging;
using aejw.Network;
using eHCMSLanguage;

namespace eHCMS.ActiveX.Logging
{
    [
        Guid("655498E0-5EB1-4EE7-9142-6B5C85BEA0A6"),
        ProgId("eHCMS.ActiveX.Logging.AxClientLogger"),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(IClientLogging)),
        ComVisible(true)
    ]
    public class ClientLogger : IClientLogging, IObjectSafety
    {
        private Thread _myThread;
        public ClientLogger()
        {
            _myThread = System.Threading.Thread.CurrentThread;
            try
            {
                using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("ActiveX.Logger.App.config"))
                {
                    log4net.Config.XmlConfigurator.Configure(s);
                } 
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
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

        public void LogInfo(string sInput,string userName=null)
        {
            AxLogger.Instance.LogInfo(sInput,userName);
        }
        public void LogWarn(string sInput, string userName = null)
        {
            AxLogger.Instance.LogWarn(sInput, userName);
        }
        public void LogDebug(string sInput, string userName = null)
        {
            AxLogger.Instance.LogDebug(sInput, userName);
        }
        public void LogError(string sInput, string userName = null)
        {
            AxLogger.Instance.LogError(sInput, userName);
        }
        public void LogFatal(string sInput, string userName = null)
        {
            AxLogger.Instance.LogFatal(sInput, userName);
        }
      
        public void SaveFile(string fullFileName, byte[] data)
        {
            if(string.IsNullOrWhiteSpace(fullFileName))
            {
                throw new Exception(eHCMSResources.Z1674_G1_PleaseSpecifyAFileName);
            }
            if (data == null)
            {
                throw new Exception(eHCMSResources.Z1675_G1_DataInvalid);
            }
            if(File.Exists(fullFileName))
            {
                throw new Exception(string.Format("{0}.",eHCMSResources.Z1676_G1_FileExists));
            }
            using (FileStream stream = new FileStream(fullFileName, FileMode.CreateNew))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(data);
                    writer.Close();
                }
            }
        }

        public void SaveFileInBase64(string fullFileName, string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                return;
            }
            var data = System.Convert.FromBase64String(base64String);
            SaveFile(fullFileName, data);
        }

        public bool DeleteFile(string fullFileName)
        {
            if(File.Exists(fullFileName))
            {
                try
                {
                    File.Delete(fullFileName);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                return true;
            }
            return false;
        }
        public void CreateFolder(string fullFolderName)
        {
            if(Directory.Exists(fullFolderName))
            {
                throw new Exception(eHCMSResources.Z1677_G1_FolderExists);
            }
            DirectoryInfo dir = Directory.CreateDirectory(fullFolderName);
        }
        public void CopyFile(string sourceFileName, string destFileName)
        {
            if(File.Exists(destFileName))
            {
                throw new Exception(string.Format("{0}.",eHCMSResources.Z1678_G1_DestinationFileExists));
            }
            try
            {
                File.Copy(sourceFileName, destFileName);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void MapDrive(string localDriveName, string shareName,string userName,string password)
        {
            NetworkDrive oNetDrive = new NetworkDrive();
            
            //set propertys
            oNetDrive.Force = false;
            oNetDrive.Persistent = false;
            oNetDrive.LocalDrive = localDriveName;
            oNetDrive.PromptForCredentials = false;
            oNetDrive.ShareName = shareName;
            oNetDrive.SaveCredentials = true;
            //match call to options provided
            if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(userName))
            {
                oNetDrive.MapDrive();
            }
            else if (string.IsNullOrEmpty(userName))
            {
                oNetDrive.MapDrive(password);
            }
            else
            {
                oNetDrive.MapDrive(userName, password);
            }
        }
        public bool ConnectServer(string userName, string password)
        {
            try
            {
                NetworkDrive oNetDrive = new NetworkDrive();

                //set propertys
                oNetDrive.Force = false;
                oNetDrive.Persistent = false;
                oNetDrive.PromptForCredentials = false;
                oNetDrive.SaveCredentials = true;
                //match call to options provided
                if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(userName))
                {
                    oNetDrive.MapDrive();
                }
                else if (string.IsNullOrEmpty(userName))
                {
                    oNetDrive.MapDrive(password);
                }
                else
                {
                    oNetDrive.MapDrive(userName, password);
                }
            }
            catch(Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return false;
            }
            return true;
        }
        public bool FolderExists(string fullFolderName)
        {
            return Directory.Exists(fullFolderName);
        }

        public bool FileExists(string fullFileName)
        {
            return File.Exists(fullFileName);
        }
    }

    [
        Guid("E86A9038-368D-4e8f-B389-FDEF38935B2F"),
        InterfaceType(ComInterfaceType.InterfaceIsDual),
        ComVisible(true)
    ]
    public interface IClientLogging
    {
        [DispId(1)]
        void LogInfo(string sInput,string userName=null);

        [DispId(2)]
        void LogWarn(string sInput, string userName = null);

        [DispId(3)]
        void LogDebug(string sInput, string userName = null);

        [DispId(4)]
        void LogError(string sInput, string userName = null);

        [DispId(5)]
        void LogFatal(string sInput, string userName = null);

        [DispId(6)]
        void SaveFile(string fullFileName, byte[] data);

        [DispId(7)]
        void SaveFileInBase64(string fullFileName, string base64String);

        [DispId(8)]
        bool DeleteFile(string fullFileName);

        [DispId(9)]
        void CreateFolder(string fullFolderName);

        [DispId(10)]
        void CopyFile(string sourceFileName, string destFileName);

        [DispId(11)]
        void MapDrive(string localDriveName, string shareName,string userName,string password);

        [DispId(12)]
        bool FolderExists(string fullFolderName);

        [DispId(13)]
        bool FileExists(string fullFileName);

        [DispId(14)]
        bool ConnectServer(string userName, string password);
    }
    [Serializable,
    ComVisible(true)]
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
