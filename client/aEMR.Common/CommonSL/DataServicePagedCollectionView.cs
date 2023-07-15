using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Collections;
using System.Reflection.Emit;
using System.Reflection;
using System.Text.RegularExpressions;
using aEMR.Common.PagedCollectionView;
/*
 * 20181107 #001 CMN: Remove ICollectionView out of SortableCollectionView cause of not show correct on grid by double inherit
*/
namespace aEMR.Common.Collections
{
    public class DataServicePagedCollectionView<T> : ObservableCollection<T>, IPagedCollectionView, INotifyPropertyChanged
       where T : INotifyPropertyChanged, new()
    {
        private IPagedCollectionView _pagingView;

        public DataServicePagedCollectionView(IPagedCollectionView delegatePagingView)
        {
            _pagingView = delegatePagingView;

            _pagingView.PageChanging += PageChanging;
            _pagingView.PageChanged += PageChanged;

            INotifyPropertyChanged propertyChanged = _pagingView as INotifyPropertyChanged;

            if (propertyChanged != null)
            {
                propertyChanged.PropertyChanged += (s, e) => RaisePropertyChanged(e);
            }
        }

        #region IPagedCollectionView Members

        public bool CanChangePage
        {
            get { return _pagingView.CanChangePage; }
        }

        public bool IsPageChanging
        {
            get { return _pagingView.IsPageChanging; }
        }
        public int ItemCount
        {
            get { return _pagingView.ItemCount; }

        }

        public bool MoveToFirstPage()
        {
            return _pagingView.MoveToFirstPage();
        }

        public bool MoveToLastPage()
        {
            return _pagingView.MoveToLastPage();
        }

        public bool MoveToNextPage()
        {
            return _pagingView.MoveToNextPage();
        }

        public bool MoveToPage(int pageIndex)
        {
            return _pagingView.MoveToPage(pageIndex);
        }

        public bool MoveToPreviousPage()
        {
            return _pagingView.MoveToPreviousPage();
        }

        public event EventHandler<System.EventArgs> PageChanged = delegate { };
        public event EventHandler<PageChangingEventArgs> PageChanging = delegate { };

        public int PageIndex
        {
            get { return _pagingView.PageIndex; }
        }

        public int PageSize
        {
            get { return _pagingView.PageSize; }
            set { _pagingView.PageSize = value; }
        }

        public int TotalItemCount
        {
            get { return _pagingView.TotalItemCount; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public new event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged(this, args);
        }

        #endregion
    }
    public static class CollectionExtension
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            if (enumerableList != null)
            {
                var observableCollection = new ObservableCollection<T>();
                foreach (var item in enumerableList)
                    observableCollection.Add(item);
                return observableCollection;
            }
            return null;
        }
    }


    public abstract class AxPagedCollectionView<T> : ObservableCollection<T>, IPagedCollectionView
    {
        #region IPagedCollectionView Members

        public bool CanChangePage
        {
            get { return true; }
        }

        bool _isPageChanging;
        public bool IsPageChanging
        {
            get { return _isPageChanging; }
            private set
            {
                if (_isPageChanging != value)
                {
                    _isPageChanging = value;
                    RaisePropertyChanged("IsPageChanging");
                }
            }
        }

        int _itemCount = 0;
        public int ItemCount
        {
            get { return TotalItemCount; }
            set
            {
                if (_itemCount != value)
                {
                    TotalItemCount = value;
                }
            }
        }

        //int _itemCount;
        //public int ItemCount
        //{
        //    get { return _itemCount; }
        //    set
        //    {
        //        if (_itemCount != value)
        //        {
        //            _itemCount = value;
        //            RaisePropertyChanged("ItemCount");
        //        }
        //    }
        //}

        public bool MoveToFirstPage()
        {
            return MoveToPage(0);
        }

        public bool MoveToLastPage()
        {
            return MoveToPage((TotalItemCount - 1) / PageSize);
        }

        public bool MoveToNextPage()
        {
            return MoveToPage(PageIndex + 1);
        }
        public abstract void LoadData();
        public bool MoveToPage(int pageIdx)
        {
            if (pageIdx == PageIndex || pageIdx < 0 || pageIdx > (TotalItemCount - 1) / PageSize)
            {
                return false;
            }
            PageChangingEventArgs args = new PageChangingEventArgs(pageIdx);

            try
            {
                IsPageChanging = true;
                if (PageChanging != null)
                {
                    PageChanging(this, args);
                }

                if (!args.Cancel)
                {
                    _pageIndex = pageIdx;
                    LoadData();

                    RaisePropertyChanged("PageIndex");
                    PageChanged(this, System.EventArgs.Empty);

                    return true;
                }

                return false;
            }
            finally
            {
                IsPageChanging = false;
            }
        }

        public bool MoveToPreviousPage()
        {
            return MoveToPage(PageIndex - 1);
        }

        public event EventHandler<System.EventArgs> PageChanged = delegate { };
        public event EventHandler<PageChangingEventArgs> PageChanging = delegate { };

        private int _pageIndex;
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                if (value < 0 || value > _totalItemCount / PageSize)
                {
                    throw new ArgumentOutOfRangeException("PageIndex must be greater than or equal to 0 and less than the page count");
                }
                _pageIndex = value;
                RaisePropertyChanged("PageIndex");
                MoveToPage(value);
            }
        }
        private int _pageSize = 10;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    RaisePropertyChanged("PageSize");
                }
            }
        }

        private int _totalItemCount;
        public int TotalItemCount
        {
            get { return _totalItemCount; }
            set
            {
                if (_totalItemCount != value)
                {
                    _totalItemCount = value;
                    RaisePropertyChanged("TotalItemCount");
                }
            }
        }

        #endregion

        protected void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        public event EventHandler<PagedViewModelEventArgs> ErrorRaising = delegate { };
    }
    public class PagedViewModelEventArgs : System.EventArgs
    {
        public PagedViewModelEventArgs(Exception exception)
        {
            Exception = exception;
        }
        public Exception Exception { get; set; }
    }

    public class RefreshEventArgs : System.EventArgs
    {
        public SortDescriptionCollection SortDescriptions { get; set; }
    }

    public class SortableCollectionView<T> : ObservableCollection<T>
        //▼====: #001
        //, ICollectionView
        //▲====: #001
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SortableCollectionView&lt;T&gt;"/> class.
        /// </summary>
        public SortableCollectionView()
        {
            this._currentItem = null;
            this._currentPosition = -1;
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, T item)
        {
            //if (null != this.Filter && !this.Filter(item)) {
            //    return;
            //}
            base.InsertItem(index, item);
            if (0 == index || null == this._currentItem)
            {
                _currentItem = item;
                _currentPosition = index;
            }
        }

        /// <summary>
        /// Gets the item at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>item if found; otherwise, null</returns>
        public virtual object GetItemAt(int index)
        {
            if ((index >= 0) && (index < this.Count))
            {
                return this[index];
            }
            return null;
        }

        #region ICollectionView Members

        /// <summary>
        /// Gets a value that indicates whether this view supports filtering by way of the <see cref="P:System.ComponentModel.ICollectionView.Filter"/> property.
        /// </summary>
        /// <value></value>
        /// <returns>true if this view supports filtering; otherwise, false.
        /// </returns>
        public bool CanFilter
        {
            get
            {
                //return true;
                return false;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this view supports grouping by way of the <see cref="P:System.ComponentModel.ICollectionView.GroupDescriptions"/> property.
        /// </summary>
        /// <value></value>
        /// <returns>true if this view supports grouping; otherwise, false.
        /// </returns>
        public bool CanGroup
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicates whether this view supports sorting by way of the <see cref="P:System.ComponentModel.ICollectionView.SortDescriptions"/> property.
        /// </summary>
        /// <value></value>
        /// <returns>true if this view supports sorting; otherwise, false.
        /// </returns>
        public bool CanSort
        {
            get { return true; }
        }

        /// <summary>
        /// Indicates whether the specified item belongs to this collection view.
        /// </summary>
        /// <param name="item">The object to check.</param>
        /// <returns>
        /// true if the item belongs to this collection view; otherwise, false.
        /// </returns>
        public bool Contains(object item)
        {
            if (!IsValidType(item))
            {
                return false;
            }
            //return this.Contains((T)item);
            return true;
        }

        /// <summary>
        /// Determines whether the specified item is of valid type
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if the specified item is of valid type; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidType(object item)
        {
            return item is T;
        }

        private CultureInfo _culture;

        /// <summary>
        /// Gets or sets the cultural information for any operations of the view that may differ by culture, such as sorting.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The culture information to use during culture-sensitive operations.
        /// </returns>
        public System.Globalization.CultureInfo Culture
        {
            get
            {
                return this._culture;
            }
            set
            {
                if (this._culture != value)
                {
                    this._culture = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Culture"));
                }

            }
        }

        /// <summary>
        /// Occurs after the current item has been changed.
        /// </summary>
        public event EventHandler CurrentChanged;

        /// <summary>
        /// Occurs before the current item changes.
        /// </summary>
        public event CurrentChangingEventHandler CurrentChanging;

        private object _currentItem;

        /// <summary>
        /// Gets the current item in the view.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The current item in the view or null if there is no current item.
        /// </returns>
        public object CurrentItem
        {
            get { return this._currentItem; }
        }

        private int _currentPosition;

        /// <summary>
        /// Gets the ordinal position of the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> in the view.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The ordinal position of the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> in the view.
        /// </returns>
        public int CurrentPosition
        {
            get
            {
                return this._currentPosition;
            }
        }

        /// <summary>
        /// Enters a defer cycle that you can use to merge changes to the view and delay automatic refresh.
        /// </summary>
        /// <returns>
        /// The typical usage is to create a using scope with an implementation of this method 
        /// and then include multiple view-changing calls within the scope. 
        /// The implementation should delay automatic refresh until after the using scope exits.
        /// </returns>
        public IDisposable DeferRefresh()
        {            
            return new DeferRefreshHelper(() => Refresh());
        }

        private Predicate<object> _filter;

        /// <summary>
        /// Gets or sets a callback that is used to determine whether an item is appropriate for inclusion in the view.
        /// </summary>
        /// <value></value>
        /// <returns>A method that is used to determine whether an item is appropriate for inclusion in the view.</returns>
        public Predicate<object> Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                //if (value == _filter) return;
                _filter = value;
                //this.Refresh();
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="T:System.ComponentModel.GroupDescription"/> objects that describe how the items in the collection are grouped in the view.
        /// </summary>
        /// <value></value>
        /// <returns>A collection of objects that describe how the items in the collection are grouped in the view. </returns>
        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the top-level groups.
        /// </summary>
        /// <value></value>
        /// <returns>A read-only collection of the top-level groups or null if there are no groups.</returns>
        public ReadOnlyObservableCollection<object> Groups
        {
            get
            {
                return null;//throw new NotImplementedException(); 
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the end of the collection.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the end of the collection; otherwise, false.
        /// </returns>
        public bool IsCurrentAfterLast
        {
            get
            {
                if (!this.IsEmpty)
                {
                    return (this.CurrentPosition >= this.Count);
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the start of the collection.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> of the view is beyond the start of the collection; otherwise, false.
        /// </returns>
        public bool IsCurrentBeforeFirst
        {
            get
            {
                if (!this.IsEmpty)
                {
                    return (this.CurrentPosition < 0);
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is current in sync.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is current in sync; otherwise, <c>false</c>.
        /// </value>
        protected bool IsCurrentInSync
        {
            get
            {
                if (this.IsCurrentInView)
                {
                    return (this.GetItemAt(this.CurrentPosition) == this.CurrentItem);
                }
                return (this.CurrentItem == null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is current in view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is current in view; otherwise, <c>false</c>.
        /// </value>
        private bool IsCurrentInView
        {
            get
            {
                return ((0 <= this.CurrentPosition) && (this.CurrentPosition < this.Count));
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the view is empty.
        /// </summary>
        /// <value></value>
        /// <returns>true if the view is empty; otherwise, false.
        /// </returns>
        public bool IsEmpty
        {
            get
            {
                return (this.Count == 0);
            }
        }

        /// <summary>
        /// Sets the specified item in the view as the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.
        /// </summary>
        /// <param name="item">The item to set as the current item.</param>
        /// <returns>
        /// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item in the view; otherwise, false.
        /// </returns>
        public bool MoveCurrentTo(object item)
        {
            if (!IsValidType(item))
            {
                return false;
            }
            if (object.Equals(this.CurrentItem, item) && ((item != null) || this.IsCurrentInView))
            {
                return this.IsCurrentInView;
            }
            int index = this.IndexOf((T)item);
            return this.MoveCurrentToPosition(index);
        }

        /// <summary>
        /// Sets the first item in the view as the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.
        /// </summary>
        /// <returns>
        /// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item in the view; otherwise, false.
        /// </returns>
        public bool MoveCurrentToFirst()
        {
            return this.MoveCurrentToPosition(0);
        }

        /// <summary>
        /// Sets the last item in the view as the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.
        /// </summary>
        /// <returns>
        /// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item in the view; otherwise, false.
        /// </returns>
        public bool MoveCurrentToLast()
        {
            return this.MoveCurrentToPosition(this.Count - 1);
        }

        /// <summary>
        /// Sets the item after the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> in the view as the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.
        /// </summary>
        /// <returns>
        /// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item in the view; otherwise, false.
        /// </returns>
        public bool MoveCurrentToNext()
        {
            return ((this.CurrentPosition < this.Count) && this.MoveCurrentToPosition(this.CurrentPosition + 1));
        }

        /// <summary>
        /// Sets the item before the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> in the view to the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/>.
        /// </summary>
        /// <returns>
        /// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item in the view; otherwise, false.
        /// </returns>
        public bool MoveCurrentToPrevious()
        {
            return ((this.CurrentPosition >= 0) && this.MoveCurrentToPosition(this.CurrentPosition - 1));
        }

        /// <summary>
        /// Sets the item at the specified index to be the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> in the view.
        /// </summary>
        /// <param name="position">The index to set the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> to.</param>
        /// <returns>
        /// true if the resulting <see cref="P:System.ComponentModel.ICollectionView.CurrentItem"/> is an item in the view; otherwise, false.
        /// </returns>
        public bool MoveCurrentToPosition(int position)
        {
            if ((position < -1) || (position > this.Count))
            {
                throw new ArgumentOutOfRangeException("position");
            }
            if (((position != this.CurrentPosition) || !this.IsCurrentInSync) && this.IsOKToChangeCurrent())
            {
                bool isCurrentAfterLast = this.IsCurrentAfterLast;
                bool isCurrentBeforeFirst = this.IsCurrentBeforeFirst;
                ChangeCurrentToPosition(position);
                OnCurrentChanged();
                if (this.IsCurrentAfterLast != isCurrentAfterLast)
                {
                    this.OnPropertyChanged("IsCurrentAfterLast");
                }
                if (this.IsCurrentBeforeFirst != isCurrentBeforeFirst)
                {
                    this.OnPropertyChanged("IsCurrentBeforeFirst");
                }
                this.OnPropertyChanged("CurrentPosition");
                this.OnPropertyChanged("CurrentItem");
            }
            return this.IsCurrentInView;
        }

        /// <summary>
        /// Changes the current to position.
        /// </summary>
        /// <param name="position">The position.</param>
        private void ChangeCurrentToPosition(int position)
        {
            if (position < 0)
            {
                this._currentItem = null;
                this._currentPosition = -1;
            }
            else if (position >= this.Count)
            {
                this._currentItem = null;
                this._currentPosition = this.Count;
            }
            else
            {
                this._currentItem = this[position];
                this._currentPosition = position;
            }
        }

        /// <summary>
        /// Determines whether it is OK to change current item.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if is OK to change current item; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsOKToChangeCurrent()
        {
            CurrentChangingEventArgs args = new CurrentChangingEventArgs();
            this.OnCurrentChanging(args);
            return !args.Cancel;
        }

        /// <summary>
        /// Called when current item has changed.
        /// </summary>
        protected virtual void OnCurrentChanged()
        {
            if (this.CurrentChanged != null)
            {
                this.CurrentChanged(this, System.EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CurrentChanging"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.ComponentModel.CurrentChangingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCurrentChanging(CurrentChangingEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }
            if (this.CurrentChanging != null)
            {
                this.CurrentChanging(this, args);
            }
        }

        /// <summary>
        /// Called when the current item is changing.
        /// </summary>
        protected void OnCurrentChanging()
        {
            this._currentPosition = -1;
            this.OnCurrentChanging(new CurrentChangingEventArgs(false));
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            OnCurrentChanging();
            base.ClearItems();
        }

        /// <summary>
        /// Called when a property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when data needs to be refreshed.
        /// </summary>
        public event EventHandler<RefreshEventArgs> OnRefresh;

        /// <summary>
        /// Recreates the view, by firing OnRefresh event.
        /// </summary>
        public void Refresh()
        {
            // sort and refersh
            if (null != OnRefresh)
            {
                OnRefresh(this, new RefreshEventArgs() { SortDescriptions = SortDescriptions });
            }
        }
        private SortDescriptionCollection _sortDescriptions;

        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                if (this._sortDescriptions == null)
                {
                    this._sortDescriptions = new SortDescriptionCollection();
                }

                return this._sortDescriptions;
            }
        }

        public IEnumerable<T> SourceCollection
        {
            get
            {
                return this;
            }
            set
            {
                List<T> data = value.ToList<T>();
                this.Clear();
                foreach (T item in data)
                {
                    this.Add(item);
                }
            }
        }
        #endregion
        
        //▼====: #001
        //#region ICollectionView Members
        //System.Collections.IEnumerable ICollectionView.SourceCollection
        //{
        //    get { return this; }
        //}
        //#endregion
        //▲====: #001

        #region Defer Refresh helper
        private class DeferRefreshHelper : IDisposable
        {
            private Action _callback;

            public DeferRefreshHelper(Action callback)
            {
                _callback = callback;
            }

            public void Dispose()
            {
                // TxD 29/07/2018: Why _callback is called upon Dispose , commented out for now BECAUSE it caused a Reloading of Data unnecessarily and Slow down operation. 
                //                 Keep monitoring if some abnormal things happens then To Be Reviewed
                //_callback();
            }
        }
        #endregion
    }

    public class PagedSortableCollectionView<T> : SortableCollectionView<T>, IPagedCollectionView
    {
        public PagedSortableCollectionView() { }
        public PagedSortableCollectionView(IList<T> aCollection)
        {
            this.SourceCollection = aCollection;
        }
        #region IPagedCollectionView Members

        public bool CanChangePage
        {
            get { return true; ; }
        }

        private bool _isPageChanging;

        public bool IsPageChanging
        {
            get { return _isPageChanging; }
            private set
            {
                if (_isPageChanging != value)
                {
                    _isPageChanging = value;
                    OnPropertyChanged("IsPageChanging");
                }
            }
        }

        public int ItemCount
        {
            get
            {
                return TotalItemCount;
            }
            set
            {
                TotalItemCount = value;
            }
        }

        public bool MoveToFirstPage()
        {
            return this.MoveToPage(0);
        }
        public bool MoveToLastPage()
        {
            return (((this.TotalItemCount != -1) && (this.PageSize > 0)) && this.MoveToPage(this.PageCount - 1));
        }

        public bool MoveToNextPage()
        {
            return MoveToPage(_pageIndex + 1);
        }

        public bool MoveToPage(int pageIndex)
        {
            if (pageIndex < -1)
            {
                return false;
            }
            if ((pageIndex == -1) && (this.PageSize > 0))
            {
                return false;
            }
            if ((pageIndex >= this.PageCount) || (this._pageIndex == pageIndex))
            {
                return false;
            }
            //
            try
            {
                IsPageChanging = true;
                if (null != PageChanging)
                {
                    PageChangingEventArgs args = new PageChangingEventArgs(pageIndex);
                    OnPageChanging(args);
                    if (args.Cancel) return false;
                }
                _pageIndex = pageIndex;
                Refresh();

                OnPropertyChanged("PageIndex");
                OnPageChanged(System.EventArgs.Empty);
                return true;
            }
            finally
            {
                IsPageChanging = false;
            }
        }


        public bool MoveToPreviousPage()
        {
            return MoveToPage(_pageIndex - 1);
        }

        /// <summary>
        /// When implementing this interface, raise this event after the <see cref="P:System.ComponentModel.IPagedCollectionView.PageIndex"/> has changed.
        /// </summary>
        public event EventHandler<System.EventArgs> PageChanged;

        /// <summary>
        /// When implementing this interface, raise this event before changing the <see cref="P:System.ComponentModel.IPagedCollectionView.PageIndex"/>. The event handler can cancel this event.
        /// </summary>
        public event EventHandler<PageChangingEventArgs> PageChanging;

        public int PageCount
        {
            get
            {
                if (this._pageSize <= 0)
                {
                    return 0;
                }
                return Math.Max(1, (int)Math.Ceiling(((double)this.ItemCount) / ((double)this._pageSize)));

            }
        }

        private int _pageIndex;

        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                if (_pageIndex != value && value >= 0)
                {
                    _pageIndex = value;
                    OnPropertyChanged("PageIndex");
                }
            }
        }

        protected virtual void OnPageChanging(PageChangingEventArgs args)
        {
            if (null != PageChanging)
            {
                PageChanging(this, args);
            }
        }

        protected virtual void OnPageChanged(System.EventArgs args)
        {
            if (null != PageChanged)
            {
                PageChanged(this, args);
            }
        }


        private int _pageSize = 10;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (_pageSize != value && value >= 1)
                {
                    _pageSize = value;
                    OnPropertyChanged("PageSize");
                }
            }
        }

        private int _totalItemCount;

        public int TotalItemCount
        {
            get
            {
                return _totalItemCount;
            }
            set
            {
                if (_totalItemCount != value)
                {
                    _totalItemCount = value;
                    OnPropertyChanged("TotalItemCount");
                    OnPropertyChanged("ItemCount");
                }
            }
        }

        #endregion
    }

    public static class DataSourceCreator
    {

        //public void btnSearch()
        //{
        //    //this.theGrid.Columns.Add(
        //    //            new DataGridTextColumn
        //    //            {
        //    //                Header = eHCMSResources.T3185_G1_Ma,
        //    //                Binding= new Binding("ID")
        //    //            });
        //    //this.theGrid.Columns.Add(
        //    //            new DataGridTextColumn
        //    //            {
        //    //                Header = eHCMSResources.T0829_G1_Ten,
        //    //                Binding = new Binding("Name")
        //    //            });
        //    //this.theGrid.Columns.Add(
        //    //            new DataGridTextColumn
        //    //            {
        //    //                Header = "Index",
        //    //                Binding = new Binding("Index")
        //    //            });
        //    //this.theGrid.Columns.Add(
        //    //            new DataGridCheckBoxColumn
        //    //            {
        //    //                Header = "Is Even",
        //    //                Binding = new Binding("IsEven")
        //    //            });

        //    this.theGrid.ItemsSource = GenerateData().ToObservableCollection();//.ToDataSource();
        //}
        private static readonly Regex PropertNameRegex =
                new Regex(@"^[A-Za-z]+[A-Za-z1-9_]*$", RegexOptions.Singleline);

        public static object[] ToDataSource(this IEnumerable<IDictionary> list)
        {
            IDictionary firstDict = null;
            bool hasData = false;
            foreach (IDictionary currentDict in list)
            {
                hasData = true;
                firstDict = currentDict;
                break;
            }
            if (!hasData)
            {
                return new object[] { };
            }
            if (firstDict == null)
            {
                throw new ArgumentException("IDictionary entry cannot be null");
            }

            Type objectType = null;

            TypeBuilder tb = GetTypeBuilder(list.GetHashCode());

            ConstructorBuilder constructor =
                        tb.DefineDefaultConstructor(
                                    MethodAttributes.Public |
                                    MethodAttributes.SpecialName |
                                    MethodAttributes.RTSpecialName);

            foreach (DictionaryEntry pair in firstDict)
            {
                //khong can dung
                //if (PropertNameRegex.IsMatch(Convert.ToString(pair.Key), 0))
                //{
                CreateProperty(tb,
                                Convert.ToString(pair.Key),
                                pair.Value == null ?
                                            typeof(object) :
                                            pair.Value.GetType());
                //                }
                //                else
                //                {
                //                    throw new ArgumentException(
                //                                @"Each key of IDictionary must be
                //                                alphanumeric and start with character.");
                //                }
            }
            objectType = tb.CreateType();
            return GenerateArray(objectType, list, firstDict);
        }

        private static object[] GenerateArray(Type objectType, IEnumerable<IDictionary> list, IDictionary firstDict)
        {
            var itemsSource = new List<object>();
            foreach (var currentDict in list)
            {
                if (currentDict == null)
                {
                    throw new ArgumentException("IDictionary entry cannot be null");
                }
                object row = Activator.CreateInstance(objectType);
                foreach (DictionaryEntry pair in firstDict)
                {
                    if (currentDict.Contains(pair.Key))
                    {
                        PropertyInfo property =
                            objectType.GetProperty(Convert.ToString(pair.Key));
                        property.SetValue(
                            row,
                            Convert.ChangeType(
                                    currentDict[pair.Key],
                                    property.PropertyType,
                                    null),
                            null);
                    }
                }
                itemsSource.Add(row);
            }
            return itemsSource.ToArray();
        }

        private static TypeBuilder GetTypeBuilder(int code)
        {
            AssemblyName an = new AssemblyName("TempAssembly" + code);
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                    an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            TypeBuilder tb = moduleBuilder.DefineType("TempType" + code
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , typeof(object));
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName,
                                                        propertyType,
                                                        FieldAttributes.Private);


            PropertyBuilder propertyBuilder =
                tb.DefineProperty(
                    propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr =
                tb.DefineMethod("get_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    propertyType, Type.EmptyTypes);

            ILGenerator getIL = getPropMthdBldr.GetILGenerator();

            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getIL.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new Type[] { propertyType });

            ILGenerator setIL = setPropMthdBldr.GetILGenerator();

            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fieldBuilder);
            setIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
