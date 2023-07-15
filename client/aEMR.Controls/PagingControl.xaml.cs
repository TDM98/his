using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Controls
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for PagingControl.xaml
    /// </summary>
    [TemplatePart(Name = "NumItemTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "GoFirstButton", Type = typeof(Button))]
    [TemplatePart(Name = "GoNextButton", Type = typeof(Button))]
    [TemplatePart(Name = "GoPreviousButton", Type = typeof(Button))]
    [TemplatePart(Name = "GoLastButton", Type = typeof(Button))]
    public partial class PagingControl : UserControl
    {
        //public event Action<int> PopulatePagedData;
        public event EventHandler PopulatePagedData;
        private TextBox _txtNumItem;
        private Button _firstButton;
        private Button _nextButton;
        private Button _previousButton;
        private Button _lastButton;

        private int _totalPage = 0;

        #region Dependency Property

        #region Deletegate command

        /// <summary>
        /// Gets or sets the <see cref="GoFirst"/> for control.
        /// </summary>
        public ICommand GetDataPage
        {
            get
            {
                return (ICommand)GetValue(GetDataPageProperty);
            }
            set
            {
                SetValue(GetDataPageProperty, value);
            }
        }
        public static readonly DependencyProperty GetDataPageProperty = DependencyProperty.Register("GetDataPage",
            typeof(ICommand), typeof(PagingControl), new FrameworkPropertyMetadata());

        #endregion

        #region Dependence Property

        private int _numItemOnPage = 0;
        /// <summary>
        /// Gets or sets the num item on page.
        /// </summary>
        /// <value>
        /// The num item on page.
        /// </value>
        public int NumItemOnPage
        {
            get
            {
                if (DesignerProperties.GetIsInDesignMode(this)) return 10;
                return (int)GetValue(NumItemOnPageProperty);
            }
            set
            {
                SetValue(NumItemOnPageProperty, value);

                if (_numItemOnPage == 0)
                    _numItemOnPage = value;
            }
        }
        public static readonly DependencyProperty NumItemOnPageProperty = DependencyProperty.Register("NumItemOnPage",
            typeof(int), typeof(PagingControl), new UIPropertyMetadata(0, OnItemsSourceChanged));


        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage
        {
            get
            {
                return (int)GetValue(CurrentPageProperty);
            }
            set
            {
                SetValue(CurrentPageProperty, value);
            }
        }
        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register("CurrentPage",
            typeof(int), typeof(PagingControl), new UIPropertyMetadata(0, OnItemsSourceChanged));

        /// <summary>
        /// Gets or sets the total page.
        /// </summary>
        /// <value>
        /// The total page.
        /// </value>
        public int TotalItems
        {
            get
            {
                return (int)GetValue(TotalItemsProperty);
            }
            set
            {
                SetValue(TotalItemsProperty, value);
            }
        }
        public static readonly DependencyProperty TotalItemsProperty = DependencyProperty.Register("TotalItems",
            typeof(int), typeof(PagingControl), new UIPropertyMetadata(0, OnItemsSourceChanged));


        /// <summary>
        /// Gets or sets the display page result.
        /// </summary>
        /// <value>
        /// The display page result.
        /// </value>
        public string DisplayPageResult
        {
            get
            {
                return (string)GetValue(DisplayPageResultProperty);
            }
            set
            {
                SetValue(DisplayPageResultProperty, value);
            }
        }
        public static readonly DependencyProperty DisplayPageResultProperty = DependencyProperty.Register("DisplayPageResult",
            typeof(string), typeof(PagingControl), new FrameworkPropertyMetadata());

        #endregion

        /// <summary>
        /// Called when [items source changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                var pagingControl = d as PagingControl;
                if (pagingControl == null) return;
                pagingControl.OnItemsSourceChanged(e.NewValue as string);
            }
        }

        /// <summary>
        /// Called when [items source changed].
        /// </summary>
        /// <param name="newValue">The new value.</param>
        protected void OnItemsSourceChanged(string newValue)
        {
            if (_lastButton != null && NumItemOnPage > 0)
            {
                //Calculate total page
                if ((TotalItems % NumItemOnPage) > 0)
                    _totalPage = (TotalItems / NumItemOnPage) + 1;
                else
                    _totalPage = (TotalItems / NumItemOnPage);

                if (TotalItems > 0)
                    DisplayPageResult = string.Format("{0}/{1}", CurrentPage + 1, _totalPage);
                else
                    DisplayPageResult = string.Format("{0}/{1}", CurrentPage, _totalPage);

                EnablePagingButton(CurrentPage, _totalPage);
            }
            else
            {
                DisplayPageResult = "0/0";
            }
        }

        #endregion

        #region Contructor

        /// <summary>
        /// Initializes the <see cref="PagingControl"/> class.
        /// </summary>
        static PagingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PagingControl),
                new FrameworkPropertyMetadata(typeof(PagingControl))
            );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingControl"/> class.
        /// </summary>
        public PagingControl()
        {
            Initialized += new EventHandler(PagingControl_Initialized);
            Loaded += new RoutedEventHandler(PagingControl_Loaded);

            InitializeComponent();
        }

        /// <summary>
        /// On initialize verify that the template is applied to the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void PagingControl_Initialized(object sender, EventArgs e)
        {
            ApplyTemplate();
        }

        /// <summary>
        /// Refresh the Available Item list according to the Asssign Item list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PagingControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_txtNumItem != null && string.IsNullOrEmpty(_txtNumItem.Text))
                _txtNumItem.Text = "20";
        }

        /// <summary>
        /// Occurs when the template is applied. Sets the different references to the
        /// template parts, the Available Items default view.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            TextBox txtNumItem = Template.FindName("NumItemTextBox", this) as TextBox;
            if (txtNumItem != null)
                _txtNumItem = txtNumItem;


            Button btnFirst = Template.FindName("GoFirstButton", this) as Button;
            if (btnFirst != null)
            {
                btnFirst.Click += new RoutedEventHandler(BtnFirst_Click);
                _firstButton = btnFirst;
            }

            Button btnNext = Template.FindName("GoNextButton", this) as Button;
            if (btnNext != null)
            {
                btnNext.Click += new RoutedEventHandler(BtnNext_Click);
                _nextButton = btnNext;
            }

            Button btnPrevious = Template.FindName("GoPreviousButton", this) as Button;
            if (btnPrevious != null)
            {
                btnPrevious.Click += new RoutedEventHandler(BtnPrevious_Click);
                _previousButton = btnPrevious;
            }

            Button btnLast = Template.FindName("GoLastButton", this) as Button;
            if (btnLast != null)
            {
                btnLast.Click += new RoutedEventHandler(BtnLast_Click);
                _lastButton = btnLast;
            }

            OnItemsSourceChanged(string.Empty);
        }

        private void FirePopulatePagedDataEvent(int pageIndex)
        {
            if(PopulatePagedData != null)
            {
                PopulatePagedData(this, null);
            }
        }
        /// <summary>
        /// Handles the Click event of the goFirst control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BtnFirst_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 0;
            //TotalItems = 0;
            FirePopulatePagedDataEvent(CurrentPage);
        }

        /// <summary>
        /// Handles the Click event of the goLast control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (_totalPage > 1)
            {
                CurrentPage = _totalPage - 1;
                FirePopulatePagedDataEvent(CurrentPage);
            }
        }

        /// <summary>
        /// Handles the Click event of the goNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            int newPage = CurrentPage + 1;
            if (newPage < _totalPage)
            {
                CurrentPage = newPage;
                FirePopulatePagedDataEvent(CurrentPage);
            }
            
            //TotalItems = 0;
        }

        /// <summary>
        /// Handles the Click event of the goPrevious control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            int newPage = CurrentPage - 1;
            if (newPage >= 0)
            {
                CurrentPage = newPage;
                FirePopulatePagedDataEvent(CurrentPage);
            }

            //TotalItems = 0;
        }

        #endregion

        #region Util Function

        /// <summary>
        /// Enables the paging button.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="sumPage">The sum page.</param>
        private void EnablePagingButton(int page, int sumPage)
        {
            // Disable or _enable button
            if (page == 0 && sumPage > 1)
            {
                _firstButton.IsEnabled = false;
                _previousButton.IsEnabled = false;

                _lastButton.IsEnabled = true;
                _nextButton.IsEnabled = true;
            }
            else
            {
                if ((page == 0 && sumPage == 1) || (page == 0 && sumPage == 0))
                {
                    _firstButton.IsEnabled = false;
                    _previousButton.IsEnabled = false;

                    _lastButton.IsEnabled = false;
                    _nextButton.IsEnabled = false;
                }
                else if (page + 1 == sumPage)
                {
                    _firstButton.IsEnabled = true;
                    _previousButton.IsEnabled = true;

                    _lastButton.IsEnabled = false;
                    _nextButton.IsEnabled = false;
                }
                else
                {
                    _firstButton.IsEnabled = true;
                    _previousButton.IsEnabled = true;

                    _lastButton.IsEnabled = true;
                    _nextButton.IsEnabled = true;
                }
            }
        }

        #endregion
    }
}
