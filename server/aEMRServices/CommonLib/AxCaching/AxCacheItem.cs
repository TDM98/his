using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eHCMS.Caching
{
    public class AxCacheItem
    {
        private object _ItemValue;
        public object ItemValue
        {
            get
            {
                return _ItemValue;
            }
            set
            {
                _ItemValue = value;
            }
        }

        private DateTime _CreatedTime = DateTime.Now;
        public DateTime CreatedTime
        {
            get
            {
                return _CreatedTime;
            }
            set
            {
                _CreatedTime = value;
            }
        }
        private DateTime _ExpirationTime = DateTime.MaxValue;
        public DateTime ExpirationTime
        {
            get
            {
                return _ExpirationTime;
            }
            set
            {
                _ExpirationTime= value;
            }
        }
        private TimeSpan _SlidingExpirationTime = new TimeSpan();
        public TimeSpan SlidingExpirationTime
        {
            get
            {
                return _SlidingExpirationTime;
            }
            set
            {
                _SlidingExpirationTime = value;
            }
        }
        private DateTime _LastAccessTime = DateTime.Now;
        public DateTime LastAccessTime
        {
            get
            {
                return _LastAccessTime;
            }
            set
            {
                _LastAccessTime = value;
            }
        }

        public AxCacheItem(object itemValue)
        {
            this._ItemValue = itemValue;
        }

        public AxCacheItem(object itemValue, DateTime expirationTime)
            : this(itemValue)
        {
            this._ExpirationTime = expirationTime;
        }

        public AxCacheItem(object itemValue, TimeSpan expirationTime)
        {
            this._ItemValue = itemValue;
            this._ExpirationTime = this._CreatedTime.Add(expirationTime);
        }
         public AxCacheItem(object itemValue, TimeSpan expirationTime, bool slidingExpiration)
        {
            this._ItemValue = itemValue;
            if (slidingExpiration)
            {
                this._SlidingExpirationTime = expirationTime;
            }
            else
            {
                this._ExpirationTime = this._CreatedTime.Add(expirationTime);
            }
        }

         public AxCacheItem(object itemValue, DateTime expirationDate, TimeSpan slidingExpirationTime)
            : this(itemValue, expirationDate)
        {
            this._SlidingExpirationTime = slidingExpirationTime;
        }
    }
}
