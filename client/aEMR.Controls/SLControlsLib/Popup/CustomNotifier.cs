using System;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.Controls
{
    public class CustomNotificator : ContentControl
    {
        public CustomNotificator()
        {
            this.DefaultStyleKey = typeof(CustomNotificator);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(CustomNotificator),
            new PropertyMetadata(OnHeaderPropertyChanged));

        public string Header
        {
            get
            {
                return (string)GetValue(CustomNotificator.HeaderProperty);
            }

            set
            {
                SetValue(CustomNotificator.HeaderProperty, value);
            }
        }

        private static void OnHeaderPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(CustomNotificator),
            new PropertyMetadata(OnTextPropertyChanged));

        public string Text
        {
            get
            {
                return (string)GetValue(CustomNotificator.TextProperty);
            }

            set
            {
                SetValue(CustomNotificator.TextProperty, value);
            }
        }

        private static void OnTextPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button closeButton = GetTemplateChild("closeButton") as Button;
            if (closeButton != null)
            {
                closeButton.Click += new RoutedEventHandler(closeButton_Click);
            }

            Button btClose = GetTemplateChild("btClose") as Button;
            if (btClose != null)
            {
                btClose.Click += new RoutedEventHandler(closeButton_Click);
            }
        }


        public event EventHandler<EventArgs> Closed;

        void closeButton_Click(object sender, RoutedEventArgs e)
        {
            EventHandler<EventArgs> handler = this.Closed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
