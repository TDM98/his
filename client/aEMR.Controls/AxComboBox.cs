using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace aEMR.Controls
{
    /// <summary>
    /// Fix bug: Khi itemsource được update lại, combobox không thể chọn được selecteditem.
    /// Bug của Silverlight.
    /// </summary>
    public class AxComboBox : ComboBox
    {
        public AxComboBox()
        {
            SelectionChanged += AxComboBox_SelectionChanged;
        }

        void AxComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Neu da bind roi thoi.
            if (_selectedItemRebind)
                return;

            //Neu khong co item nao duoc chon thi thoi. Neu khong no gan gia tri null thi khung luon.
            //Co chon item moi gan gia tri.
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                SelectedValueEx = SelectedValue;
                SelectedItemEx = SelectedItem;

                if (ItemsSource is ICollectionView)
                {
                    (ItemsSource as ICollectionView).MoveCurrentTo(SelectedItem);
                }
            }

        }
        private bool _selectedItemRebind;

        public void ResetSelectedItem(object newItem)
        {
            try
            {
                _selectedItemRebind = true;
                SelectedItem = newItem;
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                _selectedItemRebind = false;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                //base.OnKeyUp(e);
                if (IsDropDownOpen)
                {
                    base.OnKeyUp(e);
                }
                else
                {
                    if (SelectedIndex < 0)
                    {
                        IsDropDownOpen = true;
                        e.Handled = true;
                        base.OnKeyUp(e);
                    }
                    else
                    {
                        base.OnKeyUp(e);
                    }
                }
            }
            else
            {
                base.OnKeyUp(e);
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                if (IsDropDownOpen)
                {
                    base.OnKeyDown(e);
                }
                else
                {
                    if (SelectedIndex < 0)
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        base.OnKeyDown(e);
                    }
                }
            }
            else
            {
                base.OnKeyDown(e);
            }

        }


        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            //ResetSelectedItem(SelectedItemEx);
            if (GetBindingExpression(SelectedItemExProperty) != null)
            {
                ResetSelectedItem(SelectedItemEx);
            }
            else if (GetBindingExpression(SelectedValueExProperty) != null)
            {
                ResetSelectedValue(SelectedValueEx);
            }
        }
        public static readonly DependencyProperty SelectedItemExProperty =
            DependencyProperty.Register(
                "SelectedItemEx",
                typeof(object),
                typeof(AxComboBox),
                new PropertyMetadata((o, dp) =>
                {
                    var comboBoxEx = o as AxComboBox;
                    if (comboBoxEx == null)
                        return;

                    comboBoxEx.ResetSelectedItem(dp.NewValue);
                }));

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public object SelectedItemEx
        {
            get { return GetValue(SelectedItemExProperty); }
            set { SetValue(SelectedItemExProperty, value); }
        }

        public static readonly DependencyProperty SelectedValueExProperty =
            DependencyProperty.Register(
                "SelectedValueEx",
                typeof(object),
                typeof(AxComboBox),
                new PropertyMetadata((o, dp) =>
                {
                    var comboBoxEx = o as AxComboBox;
                    if (comboBoxEx == null)
                        return;

                    comboBoxEx.ResetSelectedValue(dp.NewValue);
                }));

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]



        public object SelectedValueEx
        {
            get { return GetValue(SelectedValueExProperty); }
            set { SetValue(SelectedValueExProperty, value); }
        }
        public void ResetSelectedValue(object newItem)
        {
            try
            {
                SelectedValue = newItem;
            }
            catch (System.Exception ex)
            {

            }
        }
    }
}
