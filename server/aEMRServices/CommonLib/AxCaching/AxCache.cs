using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace eHCMS.Caching
{
    public class AxCache
    {
        private Hashtable _items = new Hashtable();
        private Timer _timer = null;

        private ReaderWriterLockSlim _itemsLock = new ReaderWriterLockSlim();

        public int CachedItemsCount
        {
            get
            {
                _itemsLock.EnterReadLock();
                try
                {
                    return _items.Count;
                }
                finally
                {
                    _itemsLock.ExitReadLock();
                }
            }
        }

        private object _cacheRefreshFrequencyLock = new object();
        private TimeSpan _cacheRefreshFrequency = new TimeSpan(0, 0, 60);
        public TimeSpan CacheRefreshFrequency
        {
            get
            {
                TimeSpan res;

                lock (_cacheRefreshFrequencyLock)
                {
                    res = _cacheRefreshFrequency;
                }

                return res;
            }
            set
            {
                lock (_cacheRefreshFrequencyLock)
                {
                    _cacheRefreshFrequency = value;
                }

                int refreshFrequencyMilliseconds = (int)CacheRefreshFrequency.TotalMilliseconds;
                this._timer.Change(refreshFrequencyMilliseconds, refreshFrequencyMilliseconds);
            }
        }

        private AxCache()
        {
            int refreshFrequencyMilliseconds = (int)CacheRefreshFrequency.TotalMilliseconds;
            this._timer = new System.Threading.Timer(new TimerCallback(CacheRefresh),
                       null, refreshFrequencyMilliseconds, refreshFrequencyMilliseconds);
        }

        public static AxCache Current
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly AxCache instance = new AxCache();
        }

        private void CacheRefresh(object state)
        {
            // TxD Testing
            //int x = 88;
            _itemsLock.EnterUpgradeableReadLock();
            try
            {
                Dictionary<object, AxCacheItem> delItems = new Dictionary<object, AxCacheItem>();
                DateTime now = DateTime.Now;
                foreach (DictionaryEntry de in _items)
                {
                    AxCacheItem curItem = (AxCacheItem)de.Value;
                    if(curItem != null)
                    {
                        if(curItem.ExpirationTime < now)
                        {
                            delItems.Add(de.Key, curItem);
                        }
                        else
                        {
                            if(curItem.SlidingExpirationTime.TotalMilliseconds > 0)
                            {
                                if(now.Subtract(curItem.LastAccessTime) > curItem.SlidingExpirationTime)
                                {
                                    delItems.Add(de.Key, curItem);
                                }
                            }
                        }
                    }
                    else
                    {
                        delItems.Add(de.Key, curItem);
                    }
                }
                if(delItems.Count > 0)
                {
                    _itemsLock.EnterWriteLock();
                    try
                    {
                        foreach (KeyValuePair<object, AxCacheItem> delItem in delItems)
                        {
                            if (_items.ContainsKey(delItem.Key))
                            {
                                _items.Remove(delItem.Key);
                            }
                        }
                    }
                    finally
                    {
                        _itemsLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _itemsLock.ExitUpgradeableReadLock();
            }
        }
        public object this[object key]
        {
            get
            {
                //Nhiều thread cùng đọc.
                _itemsLock.EnterUpgradeableReadLock();
                try
                {
                    AxCacheItem curItem = (AxCacheItem)_items[key];
                    if(curItem != null)
                    {
                        if(curItem.SlidingExpirationTime.TotalMilliseconds > 0)
                        {
                            //Cập nhật lại thời gian timeout.
                            _itemsLock.EnterWriteLock();
                            try
                            {
                                curItem.LastAccessTime = DateTime.Now;
                            }
                            finally
                            {
                                _itemsLock.ExitWriteLock();
                            }
                        }
                        return curItem.ItemValue;
                    }
                    else
                    {
                        return null;
                    }
                }
                finally
                {
                    _itemsLock.ExitUpgradeableReadLock();
                }
            }
            set
            {
                //Chỉ một thread được quyền set.
                _itemsLock.EnterWriteLock();
                try
                {
                    _items[key] = new AxCacheItem(value);
                }
                finally
                {
                    _itemsLock.ExitWriteLock();
                }
            }
        }

        public void Insert(object key, object val)
        {
            _itemsLock.EnterWriteLock();
            try
            {
                _items[key] = new AxCacheItem(val);
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }
        }

        public void Insert(object key, object value, DateTime expirationDate)
        {
            _itemsLock.EnterWriteLock();
            try
            {
                _items[key] = new AxCacheItem(value, expirationDate);
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }
        }

        public void Insert(object key, object value, TimeSpan expirationTime)
        {
            _itemsLock.EnterWriteLock();
            try
            {
                _items[key] = new AxCacheItem(value, expirationTime);
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }
        }

        public void Insert(object key, object value, TimeSpan expirationTime, bool slidingExpiration)
        {
            _itemsLock.EnterWriteLock();
            try
            {
                _items[key] = new AxCacheItem(value, expirationTime, slidingExpiration);
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }
        }

        public void Insert(object key, object value, DateTime expirationDate, TimeSpan slidingExpirationTime)
        {
            _itemsLock.EnterWriteLock();
            try
            {
                _items[key] = new AxCacheItem(value, expirationDate, slidingExpirationTime);
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }
        }

        public void Remove(object key)
        {
            _itemsLock.EnterWriteLock();
            try
            {
                _items.Remove(key);
            }
            finally
            {
                _itemsLock.ExitWriteLock();
            }
        }
    }
}
