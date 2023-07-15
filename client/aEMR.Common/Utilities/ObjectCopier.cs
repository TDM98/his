using System.IO;
using System.Runtime.Serialization;

namespace aEMR.Common.Utilities
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
        public static T DeepCopy<T>(this T source)
        {
            T copy;
            var dCS = new DataContractSerializer(typeof(T));
            using(var ms=new MemoryStream())
            {
                dCS.WriteObject(ms,source);
                ms.Position = 0;
                copy = (T)dCS.ReadObject(ms);
            }
            return copy;
        }
    }
}
