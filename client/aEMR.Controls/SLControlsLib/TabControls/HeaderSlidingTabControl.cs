using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace eHCMS.Controls
{
    public class HeaderSlidingTabItem:System.Windows.Controls.TabItem
    {

    }

    [StyleTypedProperty(Property = "ScrollLeftButtonStyle", StyleTargetType = typeof(RepeatButton)),
     StyleTypedProperty(Property = "ScrollRightButtonStyle", StyleTargetType = typeof(RepeatButton)),
     TemplatePart(Name = "ScrollLeftButton", Type = typeof(RepeatButton)),
     TemplatePart(Name = "ScrollRightButton", Type = typeof(RepeatButton)),
     TemplatePart(Name = "TabScrollViewerTop", Type = typeof(ScrollViewer)),
     TemplatePart(Name = "TabPanelTop", Type = typeof(TabPanel))]
    public class HeaderSlidingTabControl : System.Windows.Controls.TabControl
    {
        private RepeatButton _tabLeftButton;
        private RepeatButton _tabRightButton;
        private ScrollViewer _tabScrollViewer;
        private TabPanel _tabPanelTop;
        private double _curHorizontalOffset = 0;
        private double _maxHorizontalOffset = 0;

        private const int SCROLL_STEP = 10;
        /// <summary>
        /// Tab Top Left Button style
        /// </summary>
        public static readonly DependencyProperty ScrollLeftButtonStyleProperty = DependencyProperty.Register(
            "ScrollLeftButtonStyle",
            typeof(Style),
            typeof(HeaderSlidingTabControl),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ScrollRightButtonStyleProperty = DependencyProperty.Register(
            "ScrollRightButtonStyle",
            typeof(Style),
            typeof(HeaderSlidingTabControl),
            new PropertyMetadata(null));

        public HeaderSlidingTabControl()
        {
            this.DefaultStyleKey = typeof(HeaderSlidingTabControl);
            this.SelectionChanged += new SelectionChangedEventHandler(HeaderSlidingTabControl_SelectionChanged);
        }

        [Description("Gets or sets the tab top left button style")]
        [Category("ScrollButton")]
        public Style ScrollLeftButtonStyle
        {
            get { return (Style)GetValue(ScrollLeftButtonStyleProperty); }
            set { SetValue(ScrollLeftButtonStyleProperty, value); }
        }

        [Description("Gets or sets the tab top right button style")]
        [Category("ScrollButton")]
        public Style ScrollRightButtonStyle
        {
            get { return (Style)GetValue(ScrollRightButtonStyleProperty); }
            set { SetValue(ScrollRightButtonStyleProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

           
            //if (null != this._tabLeftButton)
            //{
            //    this._tabLeftButton.Click -= this.tabLeftButton_Click;
            //}
            //if (null != this._tabRightButton)
            //{
            //    this._tabRightButton.Click -= this.tabRightButton_Click;
            //}

    
            this._tabLeftButton = GetTemplateChild("ScrollLeftButton") as RepeatButton;
            this._tabRightButton = GetTemplateChild("ScrollRightButton") as RepeatButton;
            this._tabScrollViewer = GetTemplateChild("TabScrollViewerTop") as ScrollViewer;
            this._tabPanelTop = GetTemplateChild("TabPanelTop") as TabPanel;
            if (null != this._tabLeftButton)
            {
                this._tabLeftButton.Click += new RoutedEventHandler(tabLeftButton_Click);
            }
            if (null != this._tabRightButton)
            {
                this._tabRightButton.Click += new RoutedEventHandler(tabRightButton_Click);
            }

        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            if (this._tabPanelTop != null && this._tabScrollViewer != null)
            {
                if (this._tabPanelTop.ActualWidth > this._tabScrollViewer.ActualWidth)
                {
                    _maxHorizontalOffset = this._tabPanelTop.ActualWidth - this._tabScrollViewer.ActualWidth;
                }
                SetScrollButtonVisibility();
            }

            return size;
        }
        private void HeaderSlidingTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HeaderSlidingTabControl tabControl = sender as HeaderSlidingTabControl;
            tabControl.Dispatcher.BeginInvoke(new Action(() => UpdateZIndex(sender as HeaderSlidingTabControl)));
        }
        private void tabRightButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != this._tabScrollViewer && null != this._tabPanelTop)
            {
                _curHorizontalOffset = this._tabScrollViewer.HorizontalOffset + SCROLL_STEP;
                if (_curHorizontalOffset > _maxHorizontalOffset)
                    _curHorizontalOffset = _maxHorizontalOffset;
                this._tabScrollViewer.ScrollToHorizontalOffset(_curHorizontalOffset);
                SetScrollButtonVisibility();
            }
        }
        private void tabLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != this._tabScrollViewer)
            {

                _curHorizontalOffset = this._tabScrollViewer.HorizontalOffset - SCROLL_STEP;
                if (_curHorizontalOffset < 0)
                    _curHorizontalOffset = 0;

                this._tabScrollViewer.ScrollToHorizontalOffset(_curHorizontalOffset);
                SetScrollButtonVisibility();
            }

        }
        private void SetScrollButtonVisibility()
        {
            if (null != this._tabScrollViewer)
            {
                if (null != this._tabLeftButton)
                {
                    if (this._curHorizontalOffset > 0)
                        this._tabLeftButton.Visibility = Visibility.Visible;
                    else
                        this._tabLeftButton.Visibility = Visibility.Collapsed;

                }
                if (null != this._tabRightButton)
                {
                    if (_curHorizontalOffset >= _maxHorizontalOffset)
                    {
                        this._tabRightButton.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        this._tabRightButton.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }
        private void UpdateZIndex(HeaderSlidingTabControl tc)
        {
            if (tc != null)
            {
                foreach (TabItem tabItem in tc.Items)
                {
                    tabItem.SetValue(Canvas.ZIndexProperty, (tabItem == tc.SelectedItem ? tc.Items.Count : (tc.Items.Count - 1) - tc.Items.IndexOf(tabItem)));
                }
            }
        }


    }
}
