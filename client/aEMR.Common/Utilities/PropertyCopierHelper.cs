using System.Reflection;

namespace aEMR.Common.Utilities
{
    /// <summary>
    /// This class provides some functionalities for copying data between two objects
    /// </summary>
    public class PropertyCopierHelper
    {
        /// <summary>
        /// Copy some property values from sourceObject to destObject
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="destObject"></param>
        public static void CopyPropertiesTo(object sourceObject, object destObject)
        {
            if (sourceObject == null || destObject == null)
                return;
            var sourceType = sourceObject.GetType();
            var destType = destObject.GetType();

            foreach (PropertyInfo propInfo in sourceType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var targetPropInfo = destType.GetProperty(propInfo.Name);
                if (targetPropInfo == null)
                    continue;
                if (targetPropInfo.CanWrite)
                {
                    targetPropInfo.SetValue(destObject, propInfo.GetValue(sourceObject, null), null);
                }
            }            
        }
    }
    

}
