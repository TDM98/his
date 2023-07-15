using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.Toolkit;

namespace aEMR.Controls
{
    public class AxReportChildWindow : ChildWindow
    {
        private static int InstanceNo = 0;
        private int _MyID;
        public int MyID
        {
            get
            {
                return _MyID;
            }
        }

        public static Action<string, string> ConstructorLogging;
        public static Action<string, string> DestructorLogging;

        readonly Grid rootGrid;

        ~AxReportChildWindow()
        {
            if (DestructorLogging != null)
            {
                DestructorLogging(this.GetType().Name, _MyID.ToString());
            }
        }
        public AxReportChildWindow()
            : base()
        {
            this.MouseRightButtonDown += new MouseButtonEventHandler(AxReportChildWindow_MouseRightButtonDown);
            var allGrids = Application.Current.MainWindow.Descendents().OfType<Grid>();
            //var allGrids = Application.Current.RootVisual.Descendents().OfType<Grid>();
            rootGrid = allGrids.FirstOrDefault();
            if (rootGrid == null)
                throw new Exception("Can not find root Grid to add to its visual tree.");

            _MyID = InstanceNo++;
            if (ConstructorLogging != null)
            {
                ConstructorLogging(this.GetType().Name, _MyID.ToString());
            }

            var binding = new Binding();
            if (!String.IsNullOrEmpty("ChildWindowTitle"))
            {
                binding.Path = new PropertyPath("ChildWindowTitle");
            }
            binding.Source = this.DataContext;
            binding.Mode = BindingMode.OneWay;
            SetBinding(CaptionProperty, binding);

            var hasCloseButtonBinding = new Binding();
            if (!String.IsNullOrEmpty("HasCloseButton"))
            {
                hasCloseButtonBinding.Path = new PropertyPath("HasCloseButton");
            }
            hasCloseButtonBinding.Source = this.DataContext;
            hasCloseButtonBinding.Mode = BindingMode.OneWay;
            SetBinding(CloseButtonStyleProperty, hasCloseButtonBinding);

            this.Style = Application.Current.Resources["ChildWindowStyle1"] as Style; 
        }

        void AxReportChildWindow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        // TxD 25/05/2018 Commented out the following because OnOpened NOT EXIST in WPF Toolkit ChildWindow
        //protected override void OnOpened()
        //{
        //    base.OnOpened();
        //    try
        //    {
        //        rootGrid.Children.Add((UIElement)Parent);
        //    }
        //    catch
        //    {
        //    }
        //}

   
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            rootGrid.Children.Remove((UIElement)Parent);
            Application.Current.MainWindow.SetValue(Control.IsEnabledProperty, true);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }

    public static class VisualTreeEnumeration
    {
        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                foreach (var descendent in Descendents(child))
                    yield return descendent;
            }
        }
    }

}