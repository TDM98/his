using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Linq;

namespace aEMR.Controls
{
    public class AxAutoComplete : AutoCompleteBox
    {
        public static readonly DependencyProperty SelectedAtCloseProperty =
            DependencyProperty.Register(
            "SelectedAtClose", typeof(object), typeof(AxAutoComplete), new PropertyMetadata(null));
        public object SelectedAtClose
        {
            get
            {
                return this.GetValue(SelectedAtCloseProperty);
            }

            set
            {
                this.SetValue(SelectedAtCloseProperty, value);
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ToggleButton toggle = (ToggleButton)GetTemplateChild("DropDownToggle");
            if (toggle != null)
            {
                toggle.Click += DropDownToggle_Click;
            }
        }
        private void DropDownToggle_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            AutoCompleteBox acb = null;
            while (fe != null && acb == null)
            {
                fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
                acb = fe as AutoCompleteBox;
            }
            if (acb != null)
            {
                acb.IsDropDownOpen = !acb.IsDropDownOpen;
            }
        }
        protected override void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            base.OnDropDownClosed(e);
            this.SelectedAtClose = this.SelectedItem;
            //▼===== 20190823 TTM: Bổ sung thêm điều kiện nếu SelectedITem != null mới selectAll. Nếu không thì autocomplete cứ không tìm ra là tô đen. Khó sử dụng.
            if (IsSelectTextOnClose && this.GetChildrenByType<TextBox>() != null && this.GetChildrenByType<TextBox>().Count > 0 && this.SelectedItem != null)
            {
                this.GetChildrenByType<TextBox>().First().SelectAll();
            }
        }
        //protected override void OnTextChanged(RoutedEventArgs e)
        //{
        //    base.OnTextChanged(e);

        //    if (string.IsNullOrEmpty(this.Text))
        //    {
        //        //this.SetValue(SelectedAtCloseProperty, null);
        //        this.SelectedItem = this.SelectedAtClose;
        //    }
        //}
        /// <summary>
        /// Sets a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is set</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <param name="val">Value to set.</param>
        /// <returns>PropertyValue</returns>
        public void SetPrivatePropertyValue<T>(object obj, string propName, T val)
        {
            Type t = obj.GetType();
            if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
            {
                throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
                //return;
            }

            t.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
        }
        public void SetDisplayText(string val)
        {
            SetPrivatePropertyValue(this, "SearchText", val);
        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (this.GetChildrenByType<TextBox>() != null && this.GetChildrenByType<TextBox>().Count > 0)
            {
                this.GetChildrenByType<TextBox>().First().Focus();
            }
        }
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
            "IsSelectTextOnClose",
            typeof(bool),
            typeof(AxAutoComplete),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectTextOnCloseChanged)));
        private static void OnIsSelectTextOnCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxAutoComplete)d).IsSelectTextOnClose = (e.NewValue as bool?).GetValueOrDefault(false);
        }
        public bool IsSelectTextOnClose { get; set; } = false;
    }
}