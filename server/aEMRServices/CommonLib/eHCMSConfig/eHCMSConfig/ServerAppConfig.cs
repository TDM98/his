using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace eHCMS.Configurations
{
    public static class ServerAppConfig
    {
        private static bool? _cachingEnabled;

        public static bool CachingEnabled
        {
            get
            {
                if (_cachingEnabled == null)
                {
                    string keyStrVal = ConfigurationManager.AppSettings["CachingEnabled"] as string;
                    if (!string.IsNullOrEmpty(keyStrVal))
                    {
                        _cachingEnabled = keyStrVal.ToLower() == "true";
                    }
                    else
                    {
                        //Gán giá trị mặc định.
                        _cachingEnabled = true;
                    }
                }
              
                return _cachingEnabled.Value;
            }
        }

        private static int? _slidingExpirationTime;
        /// <summary>
        /// Tính bằng giây
        /// </summary>
        public static int SlidingExpirationTime
        {
            get
            {
                if (_slidingExpirationTime == null)
                {
                    string keyStrVal = ConfigurationManager.AppSettings["SlidingExpirationTime"] as string;
                    int temp;
                    if (int.TryParse(keyStrVal,out temp) && temp > 0)
                    {
                        _slidingExpirationTime = temp;
                    }
                    else
                    {
                        //Gán giá trị mặc định.
                        _slidingExpirationTime = 30;
                    }
                }

                return _slidingExpirationTime.Value;
            }
        }
    }
}
