
using System.IO;
using System.Runtime.Serialization;
namespace aEMR.Common
{
    /// <summary>
    /// This class provides some functionalities for copying data between two objects
    /// </summary>
    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T DeepCopy<T>(this T ObjectToCopy)
        {
            T Copy;
            DataContractSerializer dCS = new DataContractSerializer(typeof(T));
            using(MemoryStream ms=new MemoryStream())
            {
                dCS.WriteObject(ms,ObjectToCopy);
                ms.Position = 0;
                Copy = (T)dCS.ReadObject(ms);
            }
            return Copy;
        }
    }
}
