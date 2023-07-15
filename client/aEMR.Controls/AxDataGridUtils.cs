using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;


namespace aEMR.Controls
{
    public class AxDataGridTextColumn : DataGridTextColumn
    {
        public AxDataGridTextColumn()
            : base()
        {
            _Notifier = new AxNotifier();
            _Notifier.PropertyChanged += new PropertyChangedEventHandler(_Notifier_PropertyChanged);
        }

        void _Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                this.Visibility = _Notifier.AxVisibility;
            }
        }

        private class AxNotifier : FrameworkElement, INotifyPropertyChanged
        {

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            public static readonly DependencyProperty AxVisibilityProperty = DependencyProperty.Register("AxVisibility", typeof(Visibility), typeof(AxNotifier), new PropertyMetadata(ToggleColumnVisibility));
            private static void ToggleColumnVisibility(object sender, DependencyPropertyChangedEventArgs args)
            {
                var notifier = sender as AxNotifier;
                if (notifier != null)
                {
                    notifier.AxVisibility = (Visibility)args.NewValue;
                    notifier.PropertyChanged(notifier, new PropertyChangedEventArgs("Visibility"));
                }
            }

            public Visibility AxVisibility
            {
                get
                {
                    return (Visibility)GetValue(AxVisibilityProperty);
                }
                set
                {
                    SetValue(AxVisibilityProperty, value);
                }
            }

        }
        private Binding _VisibilityBinding;
        public Binding VisibilityBinding
        {
            get
            {
                return _VisibilityBinding;
            }
            set
            {
                _VisibilityBinding = value;
                _Notifier.SetBinding(AxNotifier.AxVisibilityProperty, _VisibilityBinding);
            }
        }
        private AxNotifier _Notifier;

    }
    public class AxDataGridTemplateColumn : DataGridTemplateColumn
    {
        public AxDataGridTemplateColumn()
            : base()
        {
            _Notifier = new AxNotifier();
            _Notifier.PropertyChanged += new PropertyChangedEventHandler(_Notifier_PropertyChanged);
        }

        void _Notifier_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                this.Visibility = _Notifier.AxVisibility;
            }
        }

        private class AxNotifier : FrameworkElement, INotifyPropertyChanged
        {

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

            public static readonly DependencyProperty AxVisibilityProperty = DependencyProperty.Register("AxVisibility", typeof(Visibility), typeof(AxNotifier), new PropertyMetadata(ToggleColumnVisibility));
            private static void ToggleColumnVisibility(object sender, DependencyPropertyChangedEventArgs args)
            {
                var notifier = sender as AxNotifier;
                if (notifier != null)
                {
                    notifier.AxVisibility = (Visibility)args.NewValue;
                    notifier.PropertyChanged(notifier, new PropertyChangedEventArgs("Visibility"));
                }
            }

            public Visibility AxVisibility
            {
                get
                {
                    return (Visibility)GetValue(AxVisibilityProperty);
                }
                set
                {
                    SetValue(AxVisibilityProperty, value);
                }
            }

        }
        private Binding _VisibilityBinding;
        public Binding VisibilityBinding
        {
            get
            {
                return _VisibilityBinding;
            }
            set
            {
                _VisibilityBinding = value;
                _Notifier.SetBinding(AxNotifier.AxVisibilityProperty, _VisibilityBinding);
            }
        }
        private AxNotifier _Notifier;


        //Dinh them cho nay ne
        public static readonly DependencyProperty IsReadOnlyExProperty = DependencyProperty.Register("IsReadOnlyEx",
            typeof(bool),
            typeof(AxDataGridTemplateColumn),
            new PropertyMetadata(true, Callback));

        public bool IsReadOnlyEx
        {
            get
            {
                return (bool)GetValue(IsReadOnlyExProperty);
            }
            set
            {
                SetValue(IsReadOnlyExProperty, value);
            }
        }

        public static void SetIsReadOnlyEx(DependencyObject obj, bool IsReadOnlyEx)
        {
            obj.SetValue(IsReadOnlyExProperty, IsReadOnlyEx);
        }

        public static bool GetIsReadOnlyEx(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsReadOnlyExProperty);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxDataGridTemplateColumn)d).IsReadOnly = (bool)e.NewValue;
        }
    }
    public class DataGridHyperlinkColumn : DataGridTemplateColumn
    {
        protected override object PrepareCellForEdit(FrameworkElement editingElement,
                                                                RoutedEventArgs editingEventArgs)
        {
            editingElement.IsHitTestVisible = true;
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

    }

    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}
