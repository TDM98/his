using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections;

namespace aEMR.Controls
{
    public class AxComboBox : ComboBox
    {
        private bool IsProgramingCalling = false;
        //Biến xác định Control đang binding theo SelectedItem hay SelectedValue
        private ViewCase CurrentViewCase = ViewCase.ByItem;
        private enum ViewCase
        {
            ByItem,
            ByValue
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
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
        //Đăng ký thêm thuộc tính SelectedItemEx tương đương với SelectedItem
        public static readonly DependencyProperty SelectedItemExProperty = DependencyProperty.Register(
            "SelectedItemEx",
            typeof(object),
            typeof(AxComboBox),
            new PropertyMetadata((sender, e) =>
            {
                var CurrentComboBox = sender as AxComboBox;
                if (CurrentComboBox == null)
                {
                    return;
                }
                CurrentComboBox.SelectedItemEx = e.NewValue;
                //Đồng nhất giá trị SelectedItemEx tương đương với SelectedItem
                if (CurrentComboBox.ItemsSource == null || (CurrentComboBox.SelectedItem == null && e.NewValue == null) || (CurrentComboBox.SelectedItem != null && CurrentComboBox.SelectedItem.Equals(e.NewValue)))
                {
                    //Do nothing
                }
                else
                {
                    CurrentComboBox.IsProgramingCalling = true;
                    if (!string.IsNullOrEmpty(CurrentComboBox.SelectedValuePath) && (CurrentComboBox.GetMemberDisplay(CurrentComboBox.SelectedItemEx) == null || string.IsNullOrEmpty(CurrentComboBox.GetMemberDisplay(CurrentComboBox.SelectedItemEx).ToString())))
                    {
                        if (CurrentComboBox.GetMemberValue(CurrentComboBox.SelectedItemEx) != null)
                        {
                            CurrentComboBox.SelectedValue = CurrentComboBox.GetMemberValue(CurrentComboBox.SelectedItemEx);
                        }
                        else
                        {
                            CurrentComboBox.SelectedItem = e.NewValue;
                        }
                    }
                    else
                    {
                        CurrentComboBox.SelectedItem = e.NewValue;
                    }
                    CurrentComboBox.IsProgramingCalling = false;
                }
            }));
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public object SelectedItemEx
        {
            get { return GetValue(SelectedItemExProperty); }
            set { SetValue(SelectedItemExProperty, value); }
        }
        //Đăng ký thêm thuộc tính SelectedValueEx tương đương với SelectedValue
        public static readonly DependencyProperty SelectedValueExProperty = DependencyProperty.Register(
            "SelectedValueEx",
            typeof(object),
            typeof(AxComboBox),
            new PropertyMetadata((sender, e) =>
            {
                var CurrentComboBox = sender as AxComboBox;
                if (CurrentComboBox == null)
                {
                    return;
                }
                CurrentComboBox.CurrentViewCase = ViewCase.ByValue;
                CurrentComboBox.SelectedValueEx = e.NewValue;
                //Đồng nhất giá trị SelectedValueEx tương đương với SelectedValue
                if (CurrentComboBox.ItemsSource == null || string.IsNullOrEmpty(CurrentComboBox.SelectedValuePath) || (CurrentComboBox.SelectedValue == null && e.NewValue == null) || (CurrentComboBox.SelectedValue != null && CurrentComboBox.SelectedValue.Equals(e.NewValue)))
                {
                    //Do nothing
                }
                else
                {
                    CurrentComboBox.IsProgramingCalling = true;
                    CurrentComboBox.SelectedValue = e.NewValue;
                    CurrentComboBox.IsProgramingCalling = false;
                }
            }));
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public object SelectedValueEx
        {
            get { return GetValue(SelectedValueExProperty); }
            set { SetValue(SelectedValueExProperty, value); }
        }
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            //Sau khi thay đổi ItemSource nếu đã được set SelectedItemEx thì gọi lại hàm set giá trị cho SelectedItemEx
            //để cập nhật giá trị cho SelectedItem tương đương giá trị SelectedItemEx
            if (SelectedItemEx != null && CurrentViewCase == ViewCase.ByItem)
            {
                IsProgramingCalling = true;
                if (!string.IsNullOrEmpty(SelectedValuePath) && (GetMemberDisplay(SelectedItemEx) == null || string.IsNullOrEmpty(GetMemberDisplay(SelectedItemEx).ToString())))
                {
                    if (GetMemberValue(SelectedItemEx) != null)
                    {
                        SelectedValue = GetMemberValue(SelectedItemEx);
                    }
                    else
                    {
                        SelectedItem = SelectedItemEx;
                    }
                }
                else
                {
                    SelectedItem = SelectedItemEx;
                }
                IsProgramingCalling = false;
            }
            else if (SelectedValueEx != null && !string.IsNullOrEmpty(SelectedValuePath) && CurrentViewCase == ViewCase.ByValue)
            {
                IsProgramingCalling = true;
                SelectedValue = SelectedValueEx;
                IsProgramingCalling = false;
            }
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (IsProgramingCalling)
            {
                IsProgramingCalling = false;
                base.OnSelectionChanged(e);
                return;
            }
            if (CurrentViewCase == ViewCase.ByItem)
            {
                SetValue(SelectedItemExProperty, SelectedItem);
            }
            else if (CurrentViewCase == ViewCase.ByValue)
            {
                SetValue(SelectedValueExProperty, SelectedValue);
            }
            base.OnSelectionChanged(e);
        }
        private object GetMemberValue(object CurrentItem)
        {
            if (CurrentItem == null)
            {
                return null;
            }
            if (CurrentItem.GetType().GetProperty(SelectedValuePath) == null)
            {
                return null;
            }
            return CurrentItem.GetType().GetProperty(SelectedValuePath).GetValue(CurrentItem, null);
        }
        private object GetMemberDisplay(object CurrentItem)
        {
            if (CurrentItem == null)
            {
                return null;
            }
            if (CurrentItem.GetType().GetProperty(DisplayMemberPath) == null)
            {
                return null;
            }
            return CurrentItem.GetType().GetProperty(DisplayMemberPath).GetValue(CurrentItem, null);
        }
    }
}