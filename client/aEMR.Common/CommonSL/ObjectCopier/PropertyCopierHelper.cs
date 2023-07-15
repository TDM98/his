using System;
using System.Reflection;

namespace aEMR.Common
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
                if (targetPropInfo.CanWrite)
                {
                    targetPropInfo.SetValue(destObject, propInfo.GetValue(sourceObject, null), null);
                }
            }
        }
    }
    public class PropertyHelper
    {
        public static bool CompareObject<T>(T aObj, T bObj)
        {
            foreach (var aPropInf in aObj.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if ((aPropInf.GetValue(aObj) == null && aPropInf.GetValue(bObj) == null) || aPropInf.DeclaringType.Equals(typeof(eHCMS.Services.Core.Base.NotifyChangedBase)))
                {
                    continue;
                }
                if (aPropInf.GetValue(aObj) == null && aPropInf.GetValue(bObj) != null)
                {
                    return false;
                }
                if (!aPropInf.GetValue(aObj).Equals(aPropInf.GetValue(bObj)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}