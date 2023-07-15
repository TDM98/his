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

namespace aEMR.Controls
{
    public class TabMenuItem:MenuItem
    {
        public TabMenuItem()
            : base()
        {

        }

        public static DependencyProperty NavigateURLProperty = DependencyProperty.Register("NavigateURL", typeof(string), typeof(TabMenuItem), new PropertyMetadata(null));
        public string NavigateURL
        {
            get
            {
                return (string)GetValue(NavigateURLProperty);
            }
            set
            {
                SetValue(NavigateURLProperty, value);
            }
        }
        public static DependencyProperty TargetNameProperty = DependencyProperty.Register("TargetName", typeof(string), typeof(TabMenuItem), new PropertyMetadata(null));
        public string TargetName
        {
            get
            {
                return (string)GetValue(TargetNameProperty);
            }
            set
            {
                SetValue(TargetNameProperty, value);
            }
        }
    }
    //public class HorizontalTabMenu : HeaderSlidingTabControl
    //{
        
    //}
}
