using System;
using System.Runtime.Serialization;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using eHCMS.Configurations;
using eHCMSLanguage;

namespace eHCMS.Services.Core
{
    public static partial class AxHelper
    {
        public static string ConvertObjectToPlainXML(object obj)
        {
            var encode = new UTF8Encoding();

            var stream = new MemoryStream();
            using (XmlWriter writer = new XmlPOCOTextWriter(stream, encode))
            {
                new XmlSerializer(obj.GetType()).Serialize(writer, obj);
            }

            string retval = encode.GetString(stream.ToArray());

            return retval;
        }

        public delegate RetryEnum OnException(Exception ex, ref bool rethrow);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Kieu du lieu tra ve</typeparam>
        /// <param name="retryFunc">delegete de retry.</param>
        /// <param name="onException">Delegate dung de xac dinh loi nao can retry</param>
        /// <param name="numTries"></param>
        /// <returns></returns>
        public static T Retry<T>(Func<T> retryFunc, OnException onException, int numTries )
        {
            var i = 0;
            while (i < numTries)
            {
                try
                {
                    return retryFunc();
                }
                catch (Exception ex)
                {
                    bool rethrow = false;
                    //Neu la lan cuoi cung thi quang loi luon.
                    if (i+1 == numTries)
                    {
                        throw;
                    }
                    if (onException(ex, ref rethrow) == RetryEnum.Retry)
                    {
                        i++;
                    }
                    else
                    {
                        if (rethrow)
                        {
                            throw ex;
                        }
                        throw;
                    }
                }
            }
            throw new Exception(eHCMSResources.Z1779_G1_RetryError);
        }

        public static void Retry(Action retryFunc, OnException onException, int numTries)
        {
            var i = 0;
            while (i < numTries)
            {
                try
                {
                    retryFunc();
                    return;
                }
                catch (Exception ex)
                {
                    bool rethrow = false;
                    //Neu la lan cuoi cung thi quang loi luon.
                    if (i + 1 == numTries)
                    {
                        throw;
                    }
                    if (onException(ex, ref rethrow) == RetryEnum.Retry)
                    {
                        i++;
                    }
                    else
                    {
                        if (rethrow)
                        {
                            throw ex;
                        }
                        throw;
                    }
                }
            }
            throw new Exception(eHCMSResources.Z1779_G1_RetryError);
        }

        public static string ConvertObjectToXml(object obj, bool omitXmlDeclaration = true)
        {
            var serializer = new DataContractSerializer(obj.GetType());
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings {OmitXmlDeclaration = omitXmlDeclaration};

            var writer = XmlWriter.Create(xmlString, settings);
            serializer.WriteObject(writer, obj);
            writer.Close();

            return xmlString.ToString();
        }

        public static string GetSequenceNumber(byte seqNoType, int seqNo)
        {
            var suffix= string.Empty;
            switch (seqNoType)
            {
                case 5:
                    suffix = Globals.AxServerSettings.Hospitals.SequenceNumberType_5;
                    break;
                case 10:
                    suffix = Globals.AxServerSettings.Hospitals.SequenceNumberType_10;
                    break;
                case 25:
                    suffix = Globals.AxServerSettings.Hospitals.SequenceNumberType_25;
                    break;
                case 30:
                    suffix = Globals.AxServerSettings.Hospitals.SequenceNumberType_30;
                    break;

                case 35:
                    suffix = Globals.AxServerSettings.Hospitals.SequenceNumberType_35;
                    break;
            }
            if (seqNo <= 0)
            {
                return string.Empty;
            }
            return !string.IsNullOrWhiteSpace(suffix) ? string.Format("{0}-{1}", seqNo, suffix) : seqNo.ToString();
        }
    }

    public enum RetryEnum
    {
        Retry,
        Cancel
    }
}
