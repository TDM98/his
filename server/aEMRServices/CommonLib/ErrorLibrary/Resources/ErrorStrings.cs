using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Globalization;

namespace ErrorLibrary.Resources
{
    public class ErrorStrings
    {
        private static ResourceManager resourceMan;

        private static CultureInfo resourceCulture;

        internal ErrorStrings()
        {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        public static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    ResourceManager temp = new ResourceManager("ErrorLibrary.Resources.ErrorStrings", typeof(ErrorStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        public static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        public static string GetString(string resourceKey,string defaultValue)
        {
            try
            {
                return ResourceManager.GetString(resourceKey, resourceCulture);
            }
            catch
            {
                return "";
            }
        }
    }
}
