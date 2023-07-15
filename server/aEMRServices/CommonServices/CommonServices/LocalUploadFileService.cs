using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Services;
using System.Data.Services.Common;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using eHCMS.Services.Core;
using eHCMS.DAL;
using eHCMS.Configurations;
using eHCMS.Caching;
using System.ServiceModel;
using System.IO;

namespace CommonServices
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LocalUploadFileService : ILocalUploadFileService
    {
        //how call
        //Program copy = new Program();
        //   DirectoryInfo sourcedinfo = new DirectoryInfo(@"G:\dotnet");
        //   DirectoryInfo destinfo = new DirectoryInfo(@"G:\Nytestcopy");

        //   copy.CopyAll(sourcedinfo, destinfo);
        //   Console.Read();

        public bool CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                //check if the target directory exists
                if (Directory.Exists(target.FullName) == false)
                {
                    Directory.CreateDirectory(target.FullName);
                }
                //copy all the files into the new directory
                foreach (FileInfo fi in source.GetFiles())
                {
                    //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                    //copy xong roi delete thi dung ham nay
                    //fi.Delete();
                }
                //copy all the sub directories using recursion
                foreach (DirectoryInfo diSourceDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetDir = target.CreateSubdirectory(diSourceDir.Name);
                    //lay ten folder chua trong thu muc o day
                    //Console.WriteLine(diSourceDir.Name);
                    CopyAll(diSourceDir, nextTargetDir);
                }
                return true;
                //Console.WriteLine("Success");
            }
            catch //(IOException ie)
            {
                //Console.WriteLine(ie.Message);
                return false;
            }
        }
    }
}
