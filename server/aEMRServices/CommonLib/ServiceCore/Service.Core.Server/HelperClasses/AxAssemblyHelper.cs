using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Service.Core.Common;

namespace Service.Core.HelperClasses
{
    public class AxAssemblyHelper
    {
        private static T CustomAttributes<T>(Assembly assembly) where T : Attribute
        {
            object[] customAttributes = assembly.GetCustomAttributes(typeof(T), false);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                return ((T)customAttributes[0]);
            }
            throw new InvalidOperationException();
        }

        public static AxAssemblyInfo GetAssemblyCustomInfo(Assembly assembly)
        {
            AxAssemblyInfo info = new AxAssemblyInfo();

            info.AssemblyName = assembly.GetName().FullName;
            info.AssemblyVersion = assembly.GetName().Version.ToString();
            info.Company = CustomAttributes<AssemblyCompanyAttribute>(assembly).Company;
            info.Copyright = CustomAttributes<AssemblyCopyrightAttribute>(assembly).Copyright;
            info.Description = CustomAttributes<AssemblyDescriptionAttribute>(assembly).Description;
            info.FileName = FileVersionInfo.GetVersionInfo(assembly.Location).OriginalFilename;
            info.FilePath = FileVersionInfo.GetVersionInfo(assembly.Location).FileName;
            info.FileVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            info.Guid = CustomAttributes<System.Runtime.InteropServices.GuidAttribute>(assembly).Value;
            info.Product = CustomAttributes<AssemblyProductAttribute>(assembly).Product;

            return info;
        }
    }
}