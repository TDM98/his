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
namespace aEMR.Controls
{
    public class VerticalScrollPanel:ContentControl
    {
        
        public VerticalScrollPanel()
        {
            this.DefaultStyleKey = typeof(VerticalScrollPanel);
          
        }
        private RepeatButton _tabTopButton;
        private RepeatButton _tabBottomButton;
        private ScrollViewer _tabScrollViewer;
        private StackPanel _tabMainPanel;
        private double _curVerticalOffset = 0;
        private double _maxVerticalOffset = 0;

        private const int SCROLL_STEP = 10;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._tabTopButton = GetTemplateChild("ScrollTopButton") as RepeatButton;
            this._tabBottomButton = GetTemplateChild("ScrollDownButton") as RepeatButton;
            this._tabScrollViewer = GetTemplateChild("MainScrollView") as ScrollViewer;
            this._tabMainPanel = GetTemplateChild("MainPanel") as StackPanel;
            if (null != this._tabTopButton)
            {
                this._tabTopButton.Click += new RoutedEventHandler(_tabTopButton_Click);
            }
            if (null != this._tabBottomButton)
            {
                this._tabBottomButton.Click += new RoutedEventHandler(_tabBottomButton_Click);
            }
            this._tabMainPanel.SizeChanged += new SizeChangedEventHandler(_tabMainPanel_SizeChanged);
        }

        void _tabMainPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this._curVerticalOffset = this._tabScrollViewer.VerticalOffset;
            if (this._tabMainPanel != null && this._tabScrollViewer != null)
            {
                if (this._tabMainPanel.ActualHeight >= this._tabScrollViewer.ActualHeight + GetUpAndDownButtonHeight())
                {
                    //_maxVerticalOffset = this._tabMainPanel.ActualHeight - this._tabScrollViewer.ActualHeight;
                    _maxVerticalOffset = CalculateMaxOffset();
                    if(_maxVerticalOffset <= 0)
                    {
                        this._curVerticalOffset = 0;
                    }
                }
                
                SetScrollButtonVisibility();
            }
        }

        void _tabBottomButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != this._tabScrollViewer && null != this._tabMainPanel)
            {
                _maxVerticalOffset = CalculateMaxOffset();

                _curVerticalOffset = this._tabScrollViewer.VerticalOffset + SCROLL_STEP;
                if (_curVerticalOffset > _maxVerticalOffset)
                    _curVerticalOffset = _maxVerticalOffset;
                this._tabScrollViewer.ScrollToVerticalOffset(_curVerticalOffset);
                SetScrollButtonVisibility();
            }
        }

        void _tabTopButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != this._tabScrollViewer)
            {
                _maxVerticalOffset = CalculateMaxOffset();

                _curVerticalOffset = this._tabScrollViewer.VerticalOffset - SCROLL_STEP;
                if (_curVerticalOffset < 0)
                    _curVerticalOffset = 0;

                this._tabScrollViewer.ScrollToVerticalOffset(_curVerticalOffset);
                SetScrollButtonVisibility();
            }
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            if (this._tabMainPanel != null && this._tabScrollViewer != null)
            {
                if (this._tabMainPanel.ActualHeight >= this._tabScrollViewer.ActualHeight)
                {
                    //_maxVerticalOffset = this._tabMainPanel.ActualHeight - this._tabScrollViewer.ActualHeight;
                    _maxVerticalOffset = CalculateMaxOffset();
                }
                SetScrollButtonVisibility();
            }

            return size;
        }
        private double GetUpAndDownButtonHeight()
        {
            double height = 0.0;

            if (null != this._tabTopButton && this._tabTopButton.Visibility == Visibility.Visible)
            {
                height += this._tabTopButton.ActualHeight;
            }
            if (null != this._tabBottomButton && this._tabBottomButton.Visibility == Visibility.Visible)
            {
                height += this._tabBottomButton.ActualHeight;
            }
            return height;
        }
        private double CalculateMaxOffset()
        {
            if (this._tabMainPanel != null && this._tabScrollViewer != null)
            {
                if (this._tabMainPanel.ActualHeight > this._tabScrollViewer.ActualHeight + GetUpAndDownButtonHeight()+2)
                {
                    return this._tabMainPanel.ActualHeight - this._tabScrollViewer.ActualHeight - GetUpAndDownButtonHeight()-2;
                }
                else
                {
                    this._curVerticalOffset = this._tabScrollViewer.VerticalOffset;
                }
            }
            return 0;
        }
        private void SetScrollButtonVisibility()
        {
            if (null != this._tabScrollViewer)
            {
                if (null != this._tabTopButton)
                {
                    if (this._curVerticalOffset > 0)
                        this._tabTopButton.Visibility = Visibility.Visible;
                    else
                        this._tabTopButton.Visibility = Visibility.Collapsed;

                }
                if (null != this._tabBottomButton)
                {
                    if (_curVerticalOffset >= _maxVerticalOffset)
                    {
                        this._tabBottomButton.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        this._tabBottomButton.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
        }
        public double GetViewportHeight()
        {
            return this._tabScrollViewer.ActualHeight;
        }
    }
}
