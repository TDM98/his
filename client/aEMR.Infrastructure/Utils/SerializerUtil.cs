using System;
using System.Data;
using System.IO;
using System.Xml.Serialization;

namespace aEMR.Infrastructure.Utils
{
    public static class SerializerUtil
    {
        /// <summary>
        /// Convert from object to dataset
        /// </summary>
        /// <param name="dataTransferObject">The extension data transfer object.</param>
        /// <returns>return the dataset if object is avaible otherwise return null.</returns>
        public static DataSet ToDataSet<T>(object dataTransferObject, Type[] extraTypes = null)
        {
            XmlSerializer xmlSerializer = null;

            xmlSerializer = extraTypes == null ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), extraTypes);

            using (Stream memStream = new MemoryStream())
            {
                xmlSerializer.Serialize(memStream, dataTransferObject);
                memStream.Seek(0, SeekOrigin.Begin);
                DataSet ds = new DataSet();
                ds.ReadXml(memStream);
                return ds;
            }
        }
    }
}
