using System.Collections.Specialized;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using System;

namespace aEMR.Common
{
    /// <summary>
    /// Default CollectionChanged weak-event wrapper for INotifyCollectionChanged event source.
    /// </summary>
    public class CollectionChangedWeakEventSource : WeakEventSourceBase<INotifyCollectionChanged>
    {
        protected override WeakEventListenerBase CreateWeakEventListener(INotifyCollectionChanged eventObject)
        {
            var weakListener = new WeakEventListener<CollectionChangedWeakEventSource,
                                                     INotifyCollectionChanged,
                                                     NotifyCollectionChangedEventArgs>(this, eventObject);
            weakListener.OnDetachAction = (listener, source) =>
            {
                source.CollectionChanged -= listener.OnEvent;
            };
            weakListener.OnEventAction = (instance, source, e) =>
            {
                // fire event
                if (instance.CollectionChanged != null)
                    instance.CollectionChanged(source, e);
            };
            eventObject.CollectionChanged += weakListener.OnEvent;

            return weakListener;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

    }


    public class PropertyChangedWeakEventSource : WeakEventSourceBase<NotifyChangedBase>
    {
        protected override WeakEventListenerBase CreateWeakEventListener(NotifyChangedBase eventObject)
        {
            var weakListener = new WeakEventListener<PropertyChangedWeakEventSource,
                                                     NotifyChangedBase,
                                                     PropertyChangedEventArgs>(this, eventObject);
            weakListener.OnDetachAction = (listener, source) =>
            {
                source.PropertyChanged -= listener.OnEvent;
            };
            weakListener.OnEventAction = (instance, source, e) =>
            {
                if (instance.PropertyChanged != null)
                    instance.PropertyChanged(source, e);
            };
            eventObject.PropertyChanged += weakListener.OnEvent;

            return weakListener;
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }

    //public class WeakEventSource<TSource,TEventArgs> : WeakEventSourceBase<TSource> where TSource : class where TEventArgs:EventArgs
    //{
    //    protected override WeakEventListenerBase CreateWeakEventListener(TSource eventObject)
    //    {
    //        var weakListener = new WeakEventListener<WeakEventSource<TSource, TEventArgs>,
    //                                                 TSource,
    //                                                 TEventArgs>(this, eventObject);
    //        //weakListener.OnDetachAction = (listener, source) =>
    //        //{
    //        //    //source -= listener.OnEvent;
    //        //    listener.OnDetachAction(listener, source);
                
    //        //};
    //        weakListener.OnDetachAction = OnDetachAction;

    //        weakListener.OnEventAction = (instance, source, e) =>
    //        {
    //            if (instance.EventFired != null)
    //                instance.EventFired(source, e);
    //        };
    //        //this.SetEventSource(eventObject);

    //        if (AddEvent != null)
    //        {
    //            AddEvent(weakListener, eventObject);
    //        }
            
    //        //eventObject += weakListener.OnEvent;
    //        return weakListener;
    //    }
    //    public Action<WeakEventListener<WeakEventSource<TSource, TEventArgs>, TSource,TEventArgs>, TSource> OnDetachAction;
    //    public event EventHandler<TEventArgs> EventFired;
    //    public Action<WeakEventListener<WeakEventSource<TSource, TEventArgs>,
    //                                                 TSource,
    //                                                 TEventArgs>, TSource> AddEvent;

    //}





    public class NewWeakEventSource<TSource, TEventArgs> : WeakEventSourceBase<TSource>
        where TSource : class
        where TEventArgs : System.EventArgs
    {
        protected override WeakEventListenerBase CreateWeakEventListener(TSource eventObject)
        {
            var weakListener = new WeakEventListener<NewWeakEventSource<TSource, TEventArgs>,
                                                     TSource,
                                                     TEventArgs>(this, eventObject);
            weakListener.OnDetachAction = OnDetachAction;
            weakListener.OnEventAction = (instance, source, e) =>
            {
                if (instance.EventFired != null)
                    instance.EventFired(source, e);
            };
            //this.SetEventSource(eventObject);

            if (OnAttachEvent != null)
            {
                OnAttachEvent(weakListener, eventObject);
            }

            //eventObject += weakListener.OnEvent;
            return weakListener;
        }
        public Action<WeakEventListener<NewWeakEventSource<TSource, TEventArgs>, TSource, TEventArgs>, TSource> OnDetachAction;
        public event EventHandler<TEventArgs> EventFired;
        public Action<WeakEventListener<NewWeakEventSource<TSource, TEventArgs>,
                                                     TSource,
                                                     TEventArgs>, TSource> OnAttachEvent;

    }

    public class WeakEventSource<TSource, TEventArgs> : WeakEventSourceBase<TSource>
        where TSource : class
        where TEventArgs : EventArgs
    {
        protected override WeakEventListenerBase CreateWeakEventListener(TSource eventObject)
        {
            var weakListener = new WeakEventListener<WeakEventSource<TSource, TEventArgs>,
                                                     TSource,
                                                     TEventArgs>(this, eventObject);
            weakListener.OnDetachAction = OnDetachAction;

            weakListener.OnEventAction = (instance, source, e) =>
            {
                if (instance.EventFired != null)
                    instance.EventFired(source, e);
            };
            
            if (OnAttachAction != null)
            {
                OnAttachAction(weakListener, eventObject);
            }
            return weakListener;
        }
        public Action<WeakEventListener<WeakEventSource<TSource, TEventArgs>, TSource, TEventArgs>, TSource> OnDetachAction;
        public event EventHandler<TEventArgs> EventFired;
        public Action<WeakEventListener<WeakEventSource<TSource, TEventArgs>,
                                                     TSource,
                                                     TEventArgs>, TSource> OnAttachAction;


        public WeakEventSource(TSource source, Action<WeakEventListener<WeakEventSource<TSource, TEventArgs>,
                                                     TSource,
                                                     TEventArgs>, TSource> AttachAction,
            Action<WeakEventListener<WeakEventSource<TSource, TEventArgs>, TSource, TEventArgs>, TSource> DetachAction)
        {
            this.OnAttachAction = AttachAction;
            this.OnDetachAction = DetachAction;

            if (source != null)
            {
                this.SetEventSource(source);
            }
        }
        public WeakEventSource()
        { }
    }

}
