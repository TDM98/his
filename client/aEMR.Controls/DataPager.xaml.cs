using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace aEMR.Controls
{
    /// <summary>
    /// Interaction logic for DataPager.xaml
    /// </summary>
    public partial class DataPager : UserControl, INotifyPropertyChanged
    {
        private bool gManualUpdate = false;
        public DataPager()
        {
            InitializeComponent();
            GeneratePages();
        }

        private void GeneratePages()
        {
            if (_ItemsSource != null)
            {
                PageCount = (int)Math.Ceiling(_ItemsSource.Count / (double)PageSize);
                Pages = new ObservableCollection<ObservableCollection<object>>();
                for (int i = 0; i < PageCount; i++)
                {
                    ObservableCollection<object> page = new ObservableCollection<object>();
                    for (int j = 0; j < PageSize; j++)
                    {
                        if (i * PageSize + j > _ItemsSource.Count - 1) break;
                        page.Add(_ItemsSource[i * PageSize + j]);
                    }
                    Pages.Add(page);
                }
                CurrentPage = Pages[0];
                CurrentPageNumber = 1;
            }
        }
        private void GeneratePages(int TotalItemCount)
        {
            if (Source.GetType().GetProperty("PageSize") != null)
            {
                var mPageSize = Convert.ToInt32(Source.GetType().GetProperty("PageSize").GetValue(Source));
                if (mPageSize > 0)
                    PageSize = mPageSize;
            }
            PageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);
            Pages = new ObservableCollection<ObservableCollection<object>>();
            for (int i = 0; i < PageCount; i++)
            {
                Pages.Add(new ObservableCollection<object>());
            }
            if (Pages.Count == 1)
                CurrentPage = Pages[0];
            gManualUpdate = true;
            CurrentPageNumber = 1;
        }

        private int _PageCount = 1;
        public int PageCount
        {
            get => _PageCount; set
            {
                _PageCount = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PageCount"));
            }
        }
        //public int PageCount;
        private int _CurrentPageNumber = 1;
        private ObservableCollection<ObservableCollection<Object>> Pages;
        private ObservableCollection<Object> _ItemsSource;
        private ObservableCollection<Object> _CurrentPage;

        public ObservableCollection<Object> CurrentPage
        {
            get { return _CurrentPage; }
            set
            {
                _CurrentPage = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentPage"));
            }
        }

        //public int PageSize
        //{
        //    get { return (int)GetValue(PageSizeProperty); }
        //    set { SetValue(PageSizeProperty, value); }
        //}
        //public static readonly DependencyProperty PageSizeProperty =
        //    DependencyProperty.Register("PageSize", typeof(int), typeof(DataPager), new UIPropertyMetadata(10));

        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(DataPager), new PropertyMetadata(null, OnSourceChanged));
        private static void OnSourceChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            DataPager mDataPager = s as DataPager;
            mDataPager.Source = e.NewValue;
            if (mDataPager.Source.GetType().GetProperty("TotalItemCount") != null)
            {
                mDataPager.Source.GetType().GetProperty("TotalItemCount").SetValue(mDataPager.Source, 1);
            }
            ((INotifyPropertyChanged)mDataPager.Source).PropertyChanged += new PropertyChangedEventHandler(((DataPager)s).OnSourcePropertyChanged);
        }
        public void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TotalItemCount")
            {
                if (Source.GetType().GetProperty("TotalItemCount") != null)
                {
                    GeneratePages(Convert.ToInt32(Source.GetType().GetProperty("TotalItemCount").GetValue(Source)));
                }
            }
        }
        public int CurrentPageNumber
        {
            get { return _CurrentPageNumber; }
            set
            {
                _CurrentPageNumber = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentPageNumber"));
                if (gManualUpdate)
                {
                    gManualUpdate = false;
                    return;
                }
                else if (Source.GetType().GetMethod("Refresh") != null && Source.GetType().GetProperty("PageIndex") != null)
                {
                    Source.GetType().GetProperty("PageIndex").SetValue(Source, CurrentPageNumber - 1);
                    Source.GetType().GetMethod("Refresh").Invoke(Source, null);
                }
            }
        }

        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(DataPager), new UIPropertyMetadata(10));

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            if (Pages != null && Pages.Count > 0)
            {
                CurrentPage = Pages[0];
                CurrentPageNumber = 1;
            }
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (Pages != null)
            {
                CurrentPageNumber = (CurrentPageNumber - 1) < 1 ? 1 : CurrentPageNumber - 1;
                if (Pages.Count() < CurrentPageNumber - 1)
                    CurrentPage = Pages[CurrentPageNumber - 1];
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (Pages != null)
            {
                CurrentPageNumber = (CurrentPageNumber + 1) > PageCount ? PageCount : CurrentPageNumber + 1;
                if (Pages.Count() < CurrentPageNumber - 1)
                    CurrentPage = Pages[CurrentPageNumber - 1];
            }
        }

        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            if (Pages != null && Pages.Count > 0)
            {
                CurrentPage = Pages[PageCount - 1];
                CurrentPageNumber = PageCount;
            }
        }

        private void GoPage_Click(object sender, RoutedEventArgs e)
        {
            if (Pages != null && Pages.Count > 0 && Pages.Count() < CurrentPageNumber - 1)
            {
                CurrentPage = Pages[CurrentPageNumber - 1];
            }
        }

        private void page_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int PageNumber = 0;
                int.TryParse(page.Text, out PageNumber);
                if (PageNumber > 0)
                {
                    CurrentPageNumber = PageNumber;
                }
                GoPage_Click(null, null);
            }
        }
    }
}