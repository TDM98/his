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
using System.Xml;
using System.Linq;

namespace aEMR.Common
{
    /// <summary>
    /// Load thông tin trong thẻ appSettings trong file ServiceReferences.clientconfig của assembly gọi hàm LoadSettings.
    /// </summary>
    public static class ClientConfigurationManager
    {
        public static Dictionary<string, string> AppSettings { get; private set; }
        static ClientConfigurationManager()
        {
            AppSettings = new Dictionary<string, string>();
            LoadSettings();
        }
        public static void LoadSettings()
        {            
            XmlReaderSettings settings = new XmlReaderSettings();
            // TxD 23/05/2018 Commented out the following BECAUSE of XmlXapResolver no longer relevant
            //settings.XmlResolver = new XmlXapResolver();

            XmlReader reader = XmlReader.Create("ServiceReferences.ClientConfig");
            //reader.MoveToContent();
            XDocument document = XDocument.Load(reader);
            var doc = XDocument.Load("ServiceReferences.ClientConfig");

            IEnumerable<XElement> descendants = doc.Descendants("appSettings");

            Dictionary<string, string> dictionary = descendants.Descendants("add").ToDictionary
            (
                e => e.Attribute("key").Value,
                e => e.Attribute("value").Value
            );

            AppSettings = dictionary;
        }
    }
}
