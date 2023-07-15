using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using Action = System.Action;

namespace aEMR.Infrastructure.Extensions
{
    public static class ScreenExtensions
    {
        public static void TryExecute(this Screen screen, Action action, ILogger logger)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        public static void TryExecute(this Screen screen, Action action, ILogger logger, Action<Exception> errorAction = null)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                if (errorAction != null)
                {
                    errorAction.Invoke(ex);
                }
            }
        }

        public static void TryExecute(this Screen screen, Action action, Action<Exception> errorAction, ILogger logger, Action callbackAction = null)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                errorAction.Invoke(ex);
            }
            finally
            {
                if (callbackAction != null)
                {
                    callbackAction.Invoke();
                }
            }
        }

        /// <summary>
        /// Try excute action on thread 
        /// </summary>
        /// <param name="screen">The screen extensions.</param>
        /// <param name="action">The action.</param>
        /// <param name="logger">The logger.</param>
        public static void TryThreadExecute(this Screen screen, Action action, ILogger logger)
        {
            try
            {
                Task.Factory.StartNew(action);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Try execute the value from cache if available
        /// </summary>
        /// <typeparam name="TValue">The generic return type.</typeparam>
        /// <param name="screen">The screen extension object.</param>
        /// <param name="caching">The caching.</param>
        /// <param name="keyCaching">The string key caching</param>
        /// <param name="onlineAction">Online action execute.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>The caching value.</returns>
        public static TValue TryExecuteCaching<TValue>(this Screen screen, ISalePosCaching caching, string keyCaching, Func<TValue> onlineAction, ILogger logger)
            where TValue : class
        {
            var returnValue = default(TValue);

            try
            {
                if (caching != null && caching.Get<TValue>(keyCaching) != null)
                {
                    returnValue = caching.Get<TValue>(keyCaching);
                }
                else
                {
                    returnValue = onlineAction.Invoke();

                    if (caching != null)
                    {
                        caching.Cache(keyCaching, returnValue);
                    }    
                }
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
            }

            return returnValue;
        }
    }
}
