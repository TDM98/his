using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using Service.Core.Common;
using eHCMS.Services.Core;

namespace Service.Core.HelperClasses
{
    public class RetryOnDatabaseDeadlock
    {
        public static T RetryUntil<T>(Func<T> retryFunc, int numTries = 1, Action callback = null)
        {
            var retVal = AxHelper.Retry(retryFunc, OnException, numTries);
            if(callback != null)
            {
                callback();
            }
            return retVal;
        }
        public static void RetryUntil(Action retryFunc, int numTries = 1, Action callback = null)
        {
            AxHelper.Retry(retryFunc, OnException, numTries);
            if(callback != null)
            {
                callback();
            }
        }
        private static RetryEnum OnException(Exception exception, ref bool rethrow)
        {
            //return RetryEnum.Retry;
            //Tam thoi bo qua khuc nay.
            //Test truoc.
            var ex = exception as SqlException;
            if(ex != null)
            {
                if(ex.Number == 1205)
                {
                    return RetryEnum.Retry;
                }
            }
            return RetryEnum.Cancel;
        }
    }
}