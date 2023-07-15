using System.ComponentModel.Composition;
using Microsoft.Practices.EnterpriseLibrary.Caching;

namespace aEMR.Infrastructure.CachingUtils
{
    [Export(typeof(ISalePosCaching))]
    public class SalePosCaching : ISalePosCaching
    {
        private readonly ICacheManager _cacheManager;
        
        [ImportingConstructor]
        public SalePosCaching(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// The caching with key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value object.</param>
        public void Cache(string key, object value)
        {
            if (_cacheManager.Contains(key))
            {
                _cacheManager.Remove(key);
            }
            
            _cacheManager.Add(key, value);
        }

        /// <summary>
        /// Get object value
        /// </summary>
        /// <typeparam name="T">The generic key value.</typeparam>
        /// <param name="key">The key value.</param>
        /// <returns>The generic value.</returns>
        public T Get<T>(string key)
        {
            if (_cacheManager.Contains(key))
            {
                return (T)_cacheManager[key];
            }

            return default(T);
        }

        /// <summary>
        /// Gets caching object 
        /// </summary>
        /// <param name="key">The caching key.</param>
        /// <returns>The caching object value.</returns>
        public object Get(string key)
        {
            if (_cacheManager.Contains(key))
            {
                return _cacheManager[key];
            }

            return null;
        }

        /// <summary>
        /// Remove caching object by key 
        /// </summary>
        /// <param name="key">The key value.</param>
        public void Remove(string key)
        {
            _cacheManager.Remove(key);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public void Clear()
        {
            _cacheManager.Flush();           
        }
    }
}
