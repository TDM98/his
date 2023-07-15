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
    public class OutstandingListBox:ListBox
    {
        public OutstandingListBox():base()
        {
            DefaultStyleKey = typeof(OutstandingListBox);
        }


        public static DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(object), typeof(OutstandingListBox), new PropertyMetadata(null));
        public object HeaderText
        {
            get
            {
                return GetValue(HeaderTextProperty);
            }
            set
            {
                SetValue(HeaderTextProperty,value);
            }
        }

        public static DependencyProperty HeaderDescriptionProperty = DependencyProperty.Register("HeaderDescription", typeof(object), typeof(OutstandingListBox), new PropertyMetadata(null));
        public object HeaderDescription
        {
            get
            {
                return GetValue(HeaderDescriptionProperty);
            }
            set
            {
                SetValue(HeaderDescriptionProperty, value);
            }
        }
    }
}
