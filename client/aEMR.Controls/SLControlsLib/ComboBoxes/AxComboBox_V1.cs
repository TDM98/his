//using System;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using System.ComponentModel;
//using aEMR.Common;
//using System.Reflection;

//namespace aEMR.Controls
//{
//    /// <summary>
//    /// Fix bug: Khi itemsource được update lại, combobox không thể chọn được selecteditem.
//    /// Bug của Silverlight.
//    /// </summary>
//    public class AxComboBox : ComboBox
//    {
//        public AxComboBox() : base()
//        {
//            SelectionChanged += new SelectionChangedEventHandler(AxComboBox_SelectionChanged);
//        }

//        void AxComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            //TODO:
//            //Neu da bind roi thoi.
//            if (_selectedItemRebind)
//                return;

//            //Neu khong co item nao duoc chon thi thoi. Neu khong no gan gia tri null thi khung luon.
//            //Co chon item moi gan gia tri.
//            if(e.AddedItems != null && e.AddedItems.Count > 0)
//            {
//                //if (this.SelectedValue == null)
//                //{
//                //    ClientLoggerHelper.LogInfo("AxComboBox =====> AxComboBox_SelectionChanged 1st this.SelectedValue = NULL");
//                //}
//                //else
//                //{
//                //    ClientLoggerHelper.LogInfo("AxComboBox =====> AxComboBox_SelectionChanged 1st this.SelectedValue = " + this.SelectedValue.ToString());
//                //}
//                SelectedValueEx = this.SelectedValue;
//                SelectedItemEx = this.SelectedItem;

//                if (this.ItemsSource is ICollectionView)
//                {
//                    (this.ItemsSource as ICollectionView).MoveCurrentTo(this.SelectedItem);
//                }
//            }

//        }
//        private bool _selectedItemRebind = false;

//        public void ResetSelectedItem(object newItem)
//        {
//            try
//            {
//                _selectedItemRebind = true;
//                SelectedItem = newItem;
//            }
//            catch (System.Exception ex)
//            {
//                ClientLoggerHelper.LogInfo(ex.ToString());
//            }
//            finally
//            {
//                _selectedItemRebind = false;
//            }
//        }

//        protected override void OnKeyUp(KeyEventArgs e)
//        {
//            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
//            {
//                //base.OnKeyUp(e);
//                if (this.IsDropDownOpen)
//                {
//                    base.OnKeyUp(e);
//                }
//                else
//                {
//                    if (this.SelectedIndex < 0)
//                    {
//                        this.IsDropDownOpen = true;
//                        e.Handled = true;
//                        base.OnKeyUp(e);
//                    }
//                    else
//                    {
//                        base.OnKeyUp(e);
//                    }
//                } 
//            }
//            else
//            {
//                base.OnKeyUp(e);
//            }
//        }
//        protected override void OnKeyDown(KeyEventArgs e)
//        {
//            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
//            {
//                if (this.IsDropDownOpen)
//                {
//                    base.OnKeyDown(e);
//                }
//                else
//                {
//                    if (this.SelectedIndex < 0)
//                    {
//                        e.Handled = true;
//                    }
//                    else
//                    {
//                        base.OnKeyDown(e);
//                    }
//                }
//            }
//            else
//            {
//                base.OnKeyDown(e);
//            }

//        }


//        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
//        {
//            base.OnItemsChanged(e);
//            //ResetSelectedItem(SelectedItemEx);
//            if (this.GetBindingExpression(AxComboBox.SelectedItemExProperty) != null)
//            {
//                //if (SelectedItemEx != null && !string.IsNullOrEmpty(DisplayMemberPath) && GetMemberDisplay(SelectedItemEx) == null && !string.IsNullOrEmpty(SelectedValuePath))
//                //{
//                //    this.SelectedValue = GetMemberValue(SelectedItemEx);
//                //}
//                ResetSelectedItem(SelectedItemEx);
//            }
//            else if (this.GetBindingExpression(AxComboBox.SelectedValueExProperty) != null)
//            {
//                ResetSelectedValue(SelectedValueEx);
//            }
//        }
//        public static readonly DependencyProperty SelectedItemExProperty =
//            DependencyProperty.Register(
//                "SelectedItemEx",
//                typeof(object),
//                typeof(AxComboBox),
//                new PropertyMetadata((o, dp) =>
//                {
//                    var comboBoxEx = o as AxComboBox;
//                    if (comboBoxEx == null)
//                        return;

//                    comboBoxEx.ResetSelectedItem(dp.NewValue);

//                    //if (comboBoxEx.SelectedItemEx != null
//                    //    && !string.IsNullOrEmpty(comboBoxEx.DisplayMemberPath)
//                    //    && !string.IsNullOrEmpty(comboBoxEx.SelectedValuePath)
//                    //    && (comboBoxEx.GetMemberDisplay(comboBoxEx.SelectedItemEx) == null
//                    //        || (comboBoxEx.GetMemberDisplay(comboBoxEx.SelectedItemEx).GetType() == typeof(string)
//                    //        && string.IsNullOrEmpty(comboBoxEx.GetMemberDisplay(comboBoxEx.SelectedItemEx).ToString()))))
//                    //{
//                    //    comboBoxEx.SelectedItem = null;
//                    //    comboBoxEx.SelectedValue = comboBoxEx.GetMemberValue(comboBoxEx.SelectedItemEx);
//                    //}
//                }));

//        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
//        public object SelectedItemEx
//        {
//            get { return (object)GetValue(SelectedItemExProperty); }
//            set { SetValue(SelectedItemExProperty, value); }
//        }

//        public static readonly DependencyProperty SelectedValueExProperty =
//            DependencyProperty.Register(
//                "SelectedValueEx",
//                typeof(object),
//                typeof(AxComboBox),
//                new PropertyMetadata((o, dp) =>
//                {
//                    var comboBoxEx = o as AxComboBox;
//                    if (comboBoxEx == null)
//                        return;

//                    comboBoxEx.ResetSelectedValue(dp.NewValue);
//                }));

//        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]



//        public object SelectedValueEx
//        {
//            get { return (object)GetValue(SelectedValueExProperty); }
//            set { SetValue(SelectedValueExProperty, value); }
//        }
//        public void ResetSelectedValue(object newItem)
//        {
//            try
//            {
//                SelectedValue = newItem;
//            }
//            catch (System.Exception ex)
//            {
//                ClientLoggerHelper.LogInfo(ex.ToString());
//            }
//        }

//        private object GetMemberValue(object item)
//        {
//            if (item == null)
//                return null;
//            if (item.GetType().GetProperty(SelectedValuePath) == null)
//                return null;
//            return item.GetType().GetProperty(SelectedValuePath).GetValue(item, null);
//        }
//        private object GetMemberDisplay(object item)
//        {
//            if (item == null)
//                return null;
//            if (item.GetType().GetProperty(DisplayMemberPath) == null)
//                return null;
//            return item.GetType().GetProperty(DisplayMemberPath).GetValue(item, null);
//        }
//    }
//}