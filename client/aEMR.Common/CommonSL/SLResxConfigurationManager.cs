using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Resources;
using System.IO;
using System.Xml.Linq;

namespace aEMR.Common
{
    /// <summary>
    /// Load thông tin trong thẻ appSettings trong file app.config của assembly gọi hàm LoadSettings.
    /// </summary>
    public static class SLResxConfigurationManager
    {
        public static Dictionary<string, string> AppSettings { get; private set; }
        static SLResxConfigurationManager()
        {
            AppSettings = new Dictionary<string, string>();
            LoadSettings();
        }
        public static void LoadSettings()
        {
            // Lay assembly dang goi -> Vao root folder kiem file app.config
            
            string assemblyName = Assembly.GetCallingAssembly().FullName;
            assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));

            string url = String.Format("{0};component/app.config", assemblyName);

            StreamResourceInfo configFile = Application.GetResourceStream(new Uri(url, UriKind.Relative));

            if (configFile != null && configFile.Stream != null)
            {
                AppSettings.Clear();

                Stream stream = configFile.Stream;

                XDocument document = XDocument.Load(stream);
                foreach (XElement element in document.Descendants("appSettings").DescendantNodes())
                {
                    AppSettings.Add(element.Attribute("key").Value, element.Attribute("value").Value);
                }
            }
        }
    }
}
