using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace eHCMSCommon.Utilities
{
    public class Helpers
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes
              (typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        public static string GetRootPath()
        {
            //return Application.Current.Host.Source.AbsoluteUri.Substring(0,
            //                                                      Application.Current.Host.Source.AbsoluteUri.
            //                                                          LastIndexOf("/") + 1);
            // TxD 23/05/2018 Replaced the above with the following:
            return System.AppDomain.CurrentDomain.BaseDirectory;
            // or
            // return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }
    }
}
