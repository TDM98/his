using System;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.Common.Controls
{
    public partial class WatermarkedTextBox : TextBox
    {
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Watermark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkedTextBox), new UIPropertyMetadata(String.Empty));


        public WatermarkedTextBox()
        {
            InitializeComponent();
        }
    }
}
