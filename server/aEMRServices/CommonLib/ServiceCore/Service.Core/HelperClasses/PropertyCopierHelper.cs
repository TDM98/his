using System;
using System.Net;
using System.Reflection;

namespace eHCMS.Services.Core
{
    /// <summary>
    /// This class provides some functionalities for copying data between two objects
    /// </summary>
    public class PropertyCopierHelper
    {
        /// <summary>
        /// Copy some property values from sourceObject to destObject
        /// </summary>
        /// <param name="p"></param>
        public static void CopyPropertiesTo(object sourceObject, object destObject)
        {
            if (sourceObject == null || destObject == null)
                return;
            Type sourceType = sourceObject.GetType();
            Type destType = destObject.GetType();

            PropertyInfo targetPropInfo = null;
            foreach (PropertyInfo propInfo in sourceType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                targetPropInfo = destType.GetProperty(propInfo.Name);
                if (targetPropInfo == null)
                    continue;
                if(targetPropInfo.CanWrite && propInfo.CanRead)
                    targetPropInfo.SetValue(destObject, propInfo.GetValue(sourceObject, null), null);
            }
        }
    }
}
