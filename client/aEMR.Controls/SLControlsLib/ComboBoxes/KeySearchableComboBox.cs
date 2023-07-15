using System.Collections;
using System.Windows.Controls;
using System.Linq;
using aEMR.Infrastructure;
using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows;

namespace aEMR.Controls
{
    public class KeySearchableComboBox : ComboBox
    {
        public static readonly DependencyProperty ItemSourceExProperty =
            DependencyProperty.Register(
                "ItemSourceEx",
                typeof(object),
                typeof(KeySearchableComboBox),
                new PropertyMetadata((s, e) =>
                {
                    var mComboBox = s as KeySearchableComboBox;
                    mComboBox.gItemSource = null;
                    mComboBox.ItemsSource = (IEnumerable)e.NewValue;
                }));
        //[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public object ItemSourceEx
        {
            get { return (object)GetValue(ItemSourceExProperty); }
            set { SetValue(ItemSourceExProperty, value); }
        }
        public IEnumerable gItemSource { get; set; }
        TextBox gEditableTextBox { get; set; }
        public KeySearchableComboBox() : base()
        {
            this.IsTextSearchEnabled = false;
            this.IsEditable = true;
            this.Loaded += KeySearchableComboBox_Loaded;
            this.KeyUp += KeySearchableComboBox_KeyUp;
            this.PreviewKeyUp += KeySearchableComboBox_PreviewKeyUp;
        }
        private void KeySearchableComboBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            gEditableTextBox = (TextBox)this.Template.FindName("PART_EditableTextBox", this);
        }
        System.Windows.Input.Key LastKeyPressed { get; set; }
        private async void KeySearchableComboBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            await Task.Delay(300);
            if (LastKeyPressed != e.Key)
            {
                return;
            }
            if (gItemSource == null)
            {
                gItemSource = this.ItemsSource;
            }
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control && this.Text != null && (e.Key.ToString().Length == 1 || e.Key == Key.Back || e.Key == Key.Space || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9)) && char.IsLetter(e.Key.ToString()[0]) && gEditableTextBox != null)
            {
                int mIndex = gEditableTextBox.CaretIndex;
                string mSearchText = Globals.RemoveVietnameseString(this.Text).ToLower().Trim();
                if (!this.IsDropDownOpen)
                {
                    string CurrentText = this.Text;
                    this.IsDropDownOpen = true;
                    this.SelectedItem = null;
                    this.Text = CurrentText;
                }
                if (gItemSource != null)
                {
                    var mSourceCollection = gItemSource.Cast<object>().ToList();
                    if (mSourceCollection == null)
                    {
                        this.IsDropDownOpen = false;
                        return;
                    }
                    if (string.IsNullOrEmpty(mSearchText))
                    {
                        this.ItemsSource = mSourceCollection;
                    }
                    else if (gItemSource.Cast<object>().ToList().Count > 0 && !string.IsNullOrEmpty(DisplayMemberPath) && DisplayMemberPath.IndexOf('.') < 0)
                    {
                        var DisplayProperty = gItemSource.Cast<object>().ToList().First().GetType().GetProperty(DisplayMemberPath);
                        if (DisplayProperty == null)
                        {
                            this.ItemsSource = mSourceCollection.Where(x => Globals.RemoveVietnameseString(x.ToString()).ToLower().Contains(mSearchText) || string.Join("", x.ToString().ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(c => c[0]).ToArray()).Contains(mSearchText)).ToList();
                        }
                        else
                        {
                            this.ItemsSource = mSourceCollection.Where(x => DisplayProperty.GetValue(x) != null && Globals.RemoveVietnameseString(DisplayProperty.GetValue(x).ToString()).ToLower().Contains(mSearchText) || string.Join("", DisplayProperty.GetValue(x).ToString().ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(c => c[0]).ToArray()).Contains(mSearchText)).ToList();
                        }
                    }
                    else
                    {
                        this.ItemsSource = mSourceCollection.Where(x => Globals.RemoveVietnameseString(x.ToString()).ToLower().Contains(mSearchText) || string.Join("", x.ToString().ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(c => c[0]).ToArray()).Contains(mSearchText)).ToList();
                    }
                    if (this.ItemsSource == null || this.ItemsSource.Cast<object>().Count() == 0)
                    {
                        this.IsDropDownOpen = false;
                        return;
                    }
                }
                gEditableTextBox.Select(mIndex, 0);
            }
            else if (e.Key == Key.Enter && this.SelectedValue != null)
            {
                if (!this.IsDropDownOpen)
                {
                    this.IsDropDownOpen = true;
                }
                if (Keyboard.FocusedElement != null && Keyboard.FocusedElement is System.Windows.UIElement)
                {
                    (Keyboard.FocusedElement as System.Windows.UIElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
            else
            {
                if (gItemSource != null)
                {
                    this.ItemsSource = gItemSource;
                }
                this.IsDropDownOpen = false;
                this.SelectedItem = null;
                this.SelectedValue = null;
                if (gItemSource == null)
                {
                    return;
                }
                if (Nullable.GetUnderlyingType(gItemSource.Cast<object>().FirstOrDefault().GetType()) != null)
                {
                    this.SelectedValue = null;
                }
                else
                {
                    var mValueExpression = this.GetBindingExpression(ComboBox.SelectedValueProperty);
                    if (mValueExpression != null && mValueExpression.ParentBinding != null
                        && mValueExpression.ParentBinding.Path != null
                        && !string.IsNullOrEmpty(mValueExpression.ParentBinding.Path.Path))
                    {
                        Type mValueType = null;
                        foreach (string item in mValueExpression.ParentBinding.Path.Path.Split('.'))
                        {
                            if (mValueType == null && this.DataContext.GetType().GetProperty(item) == null)
                            {
                                this.SelectedValue = null;
                                return;
                            }
                            else if (mValueType != null && mValueType.GetProperty(item) == null)
                            {
                                this.SelectedValue = null;
                                return;
                            }
                            if (mValueType == null)
                            {
                                mValueType = this.DataContext.GetType().GetProperty(item).PropertyType;
                            }
                            else
                            {
                                mValueType = mValueType.GetProperty(item).PropertyType;
                            }
                        }
                        if (!mValueType.IsValueType)
                        {
                            this.SelectedValue = null;
                            return;
                        }
                        this.SelectedValue = Activator.CreateInstance(mValueType);
                    }
                }
            }
        }
        private void KeySearchableComboBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.IsDropDownOpen && (e.Key == System.Windows.Input.Key.Up || e.Key == System.Windows.Input.Key.Down))
            {
                e.Handled = true;
                if (this.ItemsSource == null || this.ItemsSource.Cast<object>().Count() == 0) return;
                if (this.SelectedItem == null) this.SelectedIndex = 0;
            }
            LastKeyPressed = e.Key;
        }
    }
}