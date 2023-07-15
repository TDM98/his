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
    public class AxTextBoxFilter:TextBox
    {
        public enum TextBoxFilterType
        {
            None,
            PositiveInteger,
            PositiveDecimal,
            Integer,
            Decimal,
            Alpha,
            DateKey,
        }
        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = d as TextBox;
            if (TextBoxFilterType.None != (TextBoxFilterType)e.OldValue)
            {
                textBox.KeyDown -= new KeyEventHandler(textBox_KeyDown);
            }
            if (TextBoxFilterType.None != (TextBoxFilterType)e.NewValue)
            {
                textBox.KeyDown += new KeyEventHandler(textBox_KeyDown);
            }
        }
        // Filter Attached Dependency Property
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.RegisterAttached("Filter", typeof(TextBoxFilterType), typeof(AxTextBoxFilter),
                                                new PropertyMetadata(OnFilterChanged));
        // Gets the Filter property.

        public static TextBoxFilterType GetFilter(DependencyObject d)
        {
            return (TextBoxFilterType)d.GetValue(FilterProperty);
        }
        // Sets the Filter property.
        public static void SetFilter(DependencyObject d, TextBoxFilterType value)
        {
            d.SetValue(FilterProperty, value);

        }
        // Handles the KeyDown event of the textBox control. 
        private static void textBox_KeyDown(object sender, KeyEventArgs e) 
        { 
            // bypass other keys! 
            if (IsValidOtherKey(e.Key))             
            { 
                return; 
            } 
            TextBoxFilterType filterType = GetFilter((DependencyObject)sender); 
            TextBox textBox = sender as TextBox;

            if (null == textBox) 
            {
                textBox = e.OriginalSource as TextBox;
            }

            switch (filterType) 
            { 
                case TextBoxFilterType.PositiveInteger: e.Handled = !IsValidIntegerKey(textBox, e.Key, false); 
                break; 
                case TextBoxFilterType.Integer: e.Handled = !IsValidIntegerKey(textBox, e.Key, true);
                    break; 
                case TextBoxFilterType.PositiveDecimal: e.Handled = !IsValidDecmialKey(textBox, e.Key, false);
                    break;
                case TextBoxFilterType.Decimal: e.Handled = !IsValidDecmialKey(textBox, e.Key, true); 
                    break; 
                case TextBoxFilterType.Alpha: e.Handled = !IsValidAlphaKey(e.Key); 
                    break;
                case TextBoxFilterType.DateKey: e.Handled = !IsValidDateKey(textBox, e.Key, true);
                    break;
            } 
        }

        private static bool IsValidOtherKey(Key key)
        {
            // allow control keys
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                return true;
            }
            // allow
            // Back, Tab, Enter, Shift, Ctrl, Alt, CapsLock, Escape, PageUp, PageDown
            // End, Home, Left, Up, Right, Down, Insert, Delete 
            // except for space!
            // allow all Fx keys
            if (
                (key < Key.D0 && key != Key.Space)
                || (key > Key.Z && key < Key.NumPad0))
            {
                return true;
            }
            // we need to examine all others!
            return false;
        }
        private static bool IsValidDateKey(TextBox textBox, Key key, bool negativeAllowed)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                return false;
            }
            if (Key.D0 <= key && key <= Key.D9)
            {
                return true;
            }
            if (Key.NumPad0 <= key && key <= Key.NumPad9)  
            {
                return true;
            }
            if (key.Equals('/') || key.Equals('-'))
            {
                return true;
            }
            return false;
        }
        private static bool IsValidIntegerKey(TextBox textBox, Key key, bool negativeAllowed)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                return false;
            }
            if (Key.D0 <= key && key <= Key.D9)
            {
                return true;
            }
            if (Key.NumPad0 <= key && key <= Key.NumPad9)
            {
                return true;
            }
            if (negativeAllowed && key == Key.Subtract)
            {
                return 0 == textBox.Text.Length;
            }
            return false;
        }
        // Determines whether the specified key is valid decmial key for the specified text box.
        private static bool IsValidDecmialKey(TextBox textBox, Key key, bool negativeAllowed)
        {
            if (IsValidIntegerKey(textBox, key, negativeAllowed))
            {
                return true;
            }
            if (key == Key.Decimal)
            {
                return !textBox.Text.Contains(".");
            }
            return false;
        }
        // Determines whether the specified key is valid alpha key.
        private static bool IsValidAlphaKey(Key key)
        {
            if (Key.A <= key && key <= Key.Z)
            {
                return true;
            }
            return false;
        }
    }

    public class AxTextBoxHICard : TextBox
    {
        public object TextLength
        {
            get 
            {
                return GetValue(TextLenghtProperty); 
            }
            set 
            {
                SetValue(TextLenghtProperty,value);
            }
        }

        //protected override void OnKeyUp(KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {

        //    }
        //    base.OnKeyUp(e);
        //}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            int textLength = Convert.ToInt32(TextLength) -1 ;
            base.OnKeyDown(e);
            TextBox textBox = this as TextBox;
            if (null == textBox)
            {
                textBox = e.OriginalSource as TextBox;                
            }
            
            if (textBox.Text.Length < textLength)
            {
                textBox.Foreground = new SolidColorBrush(Colors.Black);
                e.Handled = MakeUpperCase((TextBox)this, e);  
            }
            else if (textBox.Text.Length == textLength)
            {
                textBox.Foreground = new SolidColorBrush(Colors.Green);
            }
            else if (textBox.Text.Length > textLength
               && !IsValidOtherKey(e.Key))
            {
                e.Handled = true;
            }
            
            
        }
        bool MakeUpperCase(TextBox txt, KeyEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.None || (e.Key < Key.A) || (e.Key > Key.Z))  //do not handle ModifierKeys (work for shift key)
            {
                return false;
            }
            else
            {
                //19072018 TTM
                //Bi loi ma ASCII nen chuyen thanh nhan key roi convert sang string.
                //string n = new string(new char[] { (char)e.Key });
                string n = e.Key.ToString().ToUpper();
                int nSelStart = txt.SelectionStart;

                txt.Text = txt.Text.Remove(nSelStart, txt.SelectionLength); //remove character from the start to end selection
                txt.Text = txt.Text.Insert(nSelStart, n); //insert value n
                txt.Select(nSelStart + 1, 0); //for cursortext

                return true; //stop to write in txt2
            }

        }
        protected override void OnKeyUp(KeyEventArgs e)
        {

            int textLength = Convert.ToInt32(TextLength);
            base.OnKeyUp(e);
            TextBox textBox = this as TextBox;
            if (null == textBox)
            {
                textBox = e.OriginalSource as TextBox;
            }
            textBox.Text=textBox.Text.ToUpper();            
            if (textBox.Text.Length < textLength)
            {
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (textBox.Text.Length == textLength)
            {
                textBox.Foreground = new SolidColorBrush(Colors.Green);
            }            
        }

        
        
        private static void OnTextLengthChanged(DependencyObject d,DependencyPropertyChangedEventArgs e) 
        {
            
        }
        // Filter Attached Dependency Property
        
        public static readonly DependencyProperty TextLenghtProperty =
            DependencyProperty.Register("TextLength", typeof(object), typeof(AxTextBoxHICard)
            , new PropertyMetadata(OnTextLengthChanged));
        // Gets the Filter property.

        // Handles the KeyDown event of the textBox control. 
        
        
        private static bool IsValidOtherKey(Key key)
        {
            // allow control keys
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                return true;
            }
            // allow
            // Back, Tab, Enter, Shift, Ctrl, Alt, CapsLock, Escape, PageUp, PageDown
            // End, Home, Left, Up, Right, Down, Insert, Delete 
            // except for space!
            // allow all Fx keys
            if (
                (key < Key.D0 && key != Key.Space)
                || (key > Key.Z && key < Key.NumPad0))
            {
                return true;
            }
            // we need to examine all others!
            return false;
        }
        private static bool IsValidDateKey(TextBox textBox, Key key, bool negativeAllowed)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                return false;
            }
            if (Key.D0 <= key && key <= Key.D9)
            {
                return true;
            }
            if (Key.NumPad0 <= key && key <= Key.NumPad9)
            {
                return true;
            }
            if (key.Equals('/') || key.Equals('-'))
            {
                return true;
            }
            if (negativeAllowed && key == Key.Subtract)
            {
                return 0 == textBox.Text.Length;
            }
            return false;
        }
        private static bool IsValidIntegerKey(TextBox textBox, Key key, bool negativeAllowed)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                return false;
            }
            if (Key.D0 <= key && key <= Key.D9)
            {
                return true;
            }
            if (Key.NumPad0 <= key && key <= Key.NumPad9)
            {
                return true;
            }
            if (negativeAllowed && key == Key.Subtract)
            {
                return 0 == textBox.Text.Length;
            }
            return false;
        }
        // Determines whether the specified key is valid decmial key for the specified text box.
        private static bool IsValidDecmialKey(TextBox textBox, Key key, bool negativeAllowed)
        {
            if (IsValidIntegerKey(textBox, key, negativeAllowed))
            {
                return true;
            }
            if (key == Key.Decimal)
            {
                return !textBox.Text.Contains(".");
            }
            return false;
        }
        // Determines whether the specified key is valid alpha key.
        private static bool IsValidAlphaKey(Key key)
        {
            if (Key.A <= key && key <= Key.Z)
            {
                return true;
            }
            return false;
        }
    }

    public class AxAutoCompleteBox : AutoCompleteBox 
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            AutoCompleteBox auc = this as AutoCompleteBox;
            if (auc==null)
            {
                auc = e.OriginalSource as AutoCompleteBox;
            }

            e.Handled = MakeUpperCase((AutoCompleteBox)this, e);            
        }
        bool MakeUpperCase(AutoCompleteBox auc, KeyEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.None || (e.Key < Key.A) || (e.Key > Key.Z))  //do not handle ModifierKeys (work for shift key)
            {
                return false;
            }
            else
            {
                //string n = new string(new char[] { (char)e.PlatformKeyCode });
                auc.Text = auc.Text.ToUpper();                

                return true; //stop to write in txt2
            }

        }
    }
}
