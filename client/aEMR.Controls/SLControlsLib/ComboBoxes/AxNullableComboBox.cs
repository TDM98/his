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
using System.Reflection;
using System.Windows.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace aEMR.Controls
{
    public class AxNullableComboBox : AxComboBox
    {
        #region ctor
        public AxNullableComboBox()
        {
            this.SelectionChanged += this.OnSelectionChanged;
        }
        #endregion

        #region Properties

        #region NullValueContent

        #region NullValueContentProperty
        /// <summary>
        /// Identifies the NullValueContent dependency property.
        /// </summary>
        public static DependencyProperty NullValueContentProperty = DependencyProperty.Register("NullValueContent", typeof(object), typeof(AxNullableComboBox), new PropertyMetadata(new PropertyChangedCallback(AxNullableComboBox.OnNullValueContentPropertyChanged)));
        #endregion

        #region NullValueContent
        /// <summary>
        /// Gets or sets the NullValueContent value.
        /// </summary>
        public object NullValueContent
        {
            get { return (object)base.GetValue(AxNullableComboBox.NullValueContentProperty); }
            set { base.SetValue(AxNullableComboBox.NullValueContentProperty, value); }
        }
        #endregion

        #region OnNullValueContentPropertyChanged

        private static void OnNullValueContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxNullableComboBox)d).OnNullValueContentChanged((object)e.OldValue, (object)e.NewValue);
        }
        #endregion

        #endregion

        #region NullValueContentTemplate

        #region NullValueContentTemplateProperty
        /// <summary>
        /// Identifies the NullValueContentTemplate dependency property.
        /// </summary>
        public static DependencyProperty NullValueContentTemplateProperty = DependencyProperty.Register("NullValueContentTemplate", typeof(DataTemplate), typeof(AxNullableComboBox), new PropertyMetadata(new PropertyChangedCallback(AxNullableComboBox.OnNullValueContentTemplatePropertyChanged)));
        #endregion

        #region NullValueContentTemplate
        /// <summary>
        /// Gets or sets the NullValueContentTemplate value.
        /// </summary>
        public DataTemplate NullValueContentTemplate
        {
            get { return (DataTemplate)base.GetValue(AxNullableComboBox.NullValueContentTemplateProperty); }
            set { base.SetValue(AxNullableComboBox.NullValueContentTemplateProperty, value); }
        }
        #endregion

        #region OnNullValueContentTemplatePropertyChanged
        private static void OnNullValueContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AxNullableComboBox)d).OnNullValueContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }
        #endregion

        #endregion

        #region NullValueListBoxItem
        protected ListBoxItem NullValueListBoxItem { get; set; }
        protected object NullValueItem { get; set; }
        #endregion
        
        #endregion

        #region Methods

        #region OnItemsChanged
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    this.NullValueListBoxItem = null;
                    this.NullValueItem = null;
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems.Contains(null))
                    {
                        this.NullValueListBoxItem = null;
                        this.NullValueItem = null;
                    }
                    break;
            }
            object obj = this.SelectedItem;
            base.OnItemsChanged(e);
            this.SelectedItem = obj;
        }
        #endregion

        #region OnNullValueContentChanged
        private void OnNullValueContentChanged(object oldValue, object newValue)
        {
            if (this.NullValueListBoxItem != null)
            {
                this.NullValueListBoxItem.Content = newValue;
                this.OnApplyTemplate();
            }
        }
        #endregion

        #region OnNullValueContentTemplateChanged
        private void OnNullValueContentTemplateChanged(DataTemplate oldValue, DataTemplate newValue)
        {
            if (this.NullValueListBoxItem != null)
            {
                this.NullValueListBoxItem.ContentTemplate = newValue;
                this.OnApplyTemplate();
            }
        }
        #endregion

        #region OnSelectionChanged
        protected virtual void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedItemEx == this.NullValueItem)
            {
                this.SetValue(AxNullableComboBox.SelectedItemExProperty, null);
                //this.SelectedItemEx = null;
            }
        }
        #endregion

        #region PrepareContainerForItemOverride
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            ListBoxItem item2 = element as ListBoxItem;
            if (item == null && item2 != null)
            {
                item2.Content = this.NullValueContent;
                item2.ContentTemplate = this.NullValueContentTemplate;
                this.NullValueListBoxItem = item2;
                this.NullValueItem = item2.DataContext;
            }
        }
        #endregion

        #endregion
    }

    public class NullableCollectionConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return null;
            }
            IList list1 = value as IList;
            if (list1 == null) throw new ArgumentException("Only collections implementing the IList interface are supported");
            IList list2 = Activator.CreateInstance(value.GetType()) as IList;
            list2.Add(null);
            foreach (object obj1 in list1) list2.Add(obj1);
            INotifyCollectionChanged ncc = value as INotifyCollectionChanged;
            if (ncc != null)
            {
                ncc.CollectionChanged += (s, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Reset) list2.Clear();
                    else
                    {
                        if (e.NewItems != null) foreach (object obj2 in e.NewItems) list2.Add(obj2);
                        if (e.OldItems != null) foreach (object obj2 in e.OldItems) list2.Remove(obj2);
                    }
                };
            }
            return list2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
