
namespace aEMR.Infrastructure.CachingUtils
{
    /// <summary>
    /// Provide the caching interface
    /// </summary>
    public interface ISalePosCaching
    {
        /// <summary>
        /// Caching value with key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Cache(string key, object value);

        /// <summary>
        /// Gets caching value
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="key">The key</param>
        /// <returns>The generic value.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets caching object
        /// </summary>
        /// <param name="key">The caching key.</param>
        /// <returns>The caching value object.</returns>
        object Get(string key);

        /// <summary>
        /// Remove caching value by key
        /// </summary>
        /// <param name="key">The key</param>
        void Remove(string key);

        /// <summary>
        /// Clear cache
        /// </summary>
        void Clear();
    }
}
