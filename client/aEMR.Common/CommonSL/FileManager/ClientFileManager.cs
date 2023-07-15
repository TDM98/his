namespace aEMR.Common
{
    public class ClientFileManager
    {
        public static void SaveFile(string clientFullPath, byte[] data)
        {
            ClientLoggerHelper.SaveFile(clientFullPath,data);
        }
        public static void DeleteFile(string fullFileName)
        {
            ClientLoggerHelper.DeleteFile(fullFileName);
        }
        public static void CreateFolder(string fullFolderName)
        {
            ClientLoggerHelper.CreateFolder(fullFolderName);
        }
        public static void CopyFile(string sourceFileName, string destFileName)
        {
            ClientLoggerHelper.CopyFile(sourceFileName, destFileName);
        }

        public static void MapDrive(string localDriveName, string shareName,string userName,string password)
        {
            ClientLoggerHelper.MapDrive(localDriveName, shareName,userName,password);
        }
        
        public static bool FolderExists(string fullFolderName)
        {
            return ClientLoggerHelper.FolderExists(fullFolderName);
        }

        public static bool FileExists(string fullFileName)
        {
            return ClientLoggerHelper.FileExists(fullFileName);
        }

        public static bool ConnectServer(string userName, string password)
        {
            return ClientLoggerHelper.ConnectServer(userName, password);
        }
    }
}
