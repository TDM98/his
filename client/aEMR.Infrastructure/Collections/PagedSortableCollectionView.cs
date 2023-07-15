using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure.Collections
{
    //public class PagedSortableCollectionView<T> : SortableCollectionView<T>, IPagedCollectionView
    //{
    //    #region IPagedCollectionView Members

    //    public bool CanChangePage
    //    {
    //        get { return true; ; }
    //    }

    //    private bool _isPageChanging;

    //    public bool IsPageChanging
    //    {
    //        get { return _isPageChanging; }
    //        private set
    //        {
    //            if (_isPageChanging != value)
    //            {
    //                _isPageChanging = value;
    //                ("IsPageChanging");
    //            }
    //        }
    //    }

    //    public int ItemCount
    //    {
    //        get
    //        {
    //            return TotalItemCount;
    //        }
    //        set
    //        {
    //            TotalItemCount = value;
    //        }
    //    }

    //    public bool MoveToFirstPage()
    //    {
    //        return this.MoveToPage(0);
    //    }
    //    public bool MoveToLastPage()
    //    {
    //        return (((this.TotalItemCount != -1) && (this.PageSize > 0)) && this.MoveToPage(this.PageCount - 1));
    //    }

    //    public bool MoveToNextPage()
    //    {
    //        return MoveToPage(_pageIndex + 1);
    //    }

    //    public bool MoveToPage(int pageIndex)
    //    {
    //        if (pageIndex < -1)
    //        {
    //            return false;
    //        }
    //        if ((pageIndex == -1) && (this.PageSize > 0))
    //        {
    //            return false;
    //        }
    //        if ((pageIndex >= this.PageCount) || (this._pageIndex == pageIndex))
    //        {
    //            return false;
    //        }
    //        //
    //        try
    //        {
    //            IsPageChanging = true;
    //            if (null != PageChanging)
    //            {
    //                PageChangingEventArgs args = new PageChangingEventArgs(pageIndex);
    //                OnPageChanging(args);
    //                if (args.Cancel) return false;
    //            }
    //            _pageIndex = pageIndex;
    //            Refresh();

    //            OnPropertyChanged("PageIndex");
    //            OnPageChanged(EventArgs.Empty);
    //            return true;
    //        }
    //        finally
    //        {
    //            IsPageChanging = false;
    //        }
    //    }


    //    public bool MoveToPreviousPage()
    //    {
    //        return MoveToPage(_pageIndex - 1);
    //    }

    //    /// <summary>
    //    /// When implementing this interface, raise this event after the <see cref="P:System.ComponentModel.IPagedCollectionView.PageIndex"/> has changed.
    //    /// </summary>
    //    public event EventHandler<EventArgs> PageChanged;

    //    /// <summary>
    //    /// When implementing this interface, raise this event before changing the <see cref="P:System.ComponentModel.IPagedCollectionView.PageIndex"/>. The event handler can cancel this event.
    //    /// </summary>
    //    public event EventHandler<PageChangingEventArgs> PageChanging;

    //    public int PageCount
    //    {
    //        get
    //        {
    //            if (this._pageSize <= 0)
    //            {
    //                return 0;
    //            }
    //            return Math.Max(1, (int)Math.Ceiling(((double)this.ItemCount) / ((double)this._pageSize)));

    //        }
    //    }

    //    private int _pageIndex;

    //    public int PageIndex
    //    {
    //        get
    //        {
    //            return _pageIndex;
    //        }
    //        set
    //        {
    //            if (_pageIndex != value && value >= 0)
    //            {
    //                _pageIndex = value;
    //                OnPropertyChanged("PageIndex");
    //            }
    //        }
    //    }

    //    protected virtual void OnPageChanging(PageChangingEventArgs args)
    //    {
    //        if (null != PageChanging)
    //        {
    //            PageChanging(this, args);
    //        }
    //    }

    //    protected virtual void OnPageChanged(EventArgs args)
    //    {
    //        if (null != PageChanged)
    //        {
    //            PageChanged(this, args);
    //        }
    //    }


    //    private int _pageSize = 10;

    //    public int PageSize
    //    {
    //        get
    //        {
    //            return _pageSize;
    //        }
    //        set
    //        {
    //            if (_pageSize != value && value >= 1)
    //            {
    //                _pageSize = value;
    //                OnPropertyChanged("PageSize");
    //            }
    //        }
    //    }

    //    private int _totalItemCount;

    //    public int TotalItemCount
    //    {
    //        get
    //        {
    //            return _totalItemCount;
    //        }
    //        set
    //        {
    //            if (_totalItemCount != value)
    //            {
    //                _totalItemCount = value;
    //                OnPropertyChanged("TotalItemCount");
    //                OnPropertyChanged("ItemCount");
    //            }
    //        }
    //    }

    //    #endregion

    //}

}
