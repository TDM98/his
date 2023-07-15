using System;
using System.Collections.Generic;


namespace aEMR.Common
{
    public class ObjectCollectionLoadingCompletedEventArgs<T> : EventArgs
    {
        public int TotalCount { get; set; }
        public IEnumerable<T> Results { get; private set; }
        public ObjectCollectionLoadingCompletedEventArgs(IEnumerable<T> objList)
        {
            Results = objList;
        }
    }
    public class ObjectLoadingCompletedEventArgs<T> : EventArgs
    {
        public T Result { get; private set; }
        public ObjectLoadingCompletedEventArgs(T obj)
        {
            Result = obj;
        }
    }
    public class DataErrorEventArgs : EventArgs
    {
        public object UserState { get; set; }
        public System.Exception InnerException { get; private set; }
        public DataErrorEventArgs()
        {
            InnerException = null;
        }
        public DataErrorEventArgs(System.Exception ex)
        {
            InnerException = ex;
        }
        public DataErrorEventArgs(System.Exception ex, object userState)
        {
            this.UserState = userState;
        }
    }

    public class CatalogOperationCompletedEventArgs : System.EventArgs
    {
        public object UserState { get; set; }

        public CatalogOperationCompletedEventArgs()
        {
            UserState = null;
        }
        public CatalogOperationCompletedEventArgs(object userState)
        {
            this.UserState = userState;
        }
    }
    public class AxEventArgs : System.EventArgs
    {
        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
        }
        public AxEventArgs()
        {
        }
        public AxEventArgs(string errorMessage)
        {
            _Message = errorMessage;
        }
    }

    public class EventArgs<T> : System.EventArgs
    {
        public EventArgs(T value)
        {
            _value = value;
        }

        private T _value;

        public T Value
        {
            get { return _value; }
        }
    }
}
