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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace aEMR.Controls
{
    public class DecimalNumTextBox : TextBox
    {
        public DecimalNumTextBox() : base()
        {
            base.GotFocus += OnGotFocus;
        }
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectAll();
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch(this.Text.Insert(this.SelectionStart, e.Text));        
        }
    }

    public class AxTextBox : TextBox
    {
        public AxTextBox()
            : base()
        {
            this.KeyUp += new KeyEventHandler(AxTextBox_KeyUp);
            base.GotFocus += OnGotFocus;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectAll();
        }

        void AxTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ////e.Handled = true;
                //Type t = e.GetType();

                //FieldInfo[] fis = t.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase);
                //PropertyInfo pi = t.GetProperty("Key", BindingFlags.Instance | BindingFlags.NonPublic |BindingFlags.Public);

                //pi.SetValue(e,Key.Tab,null);
                //base.OnKeyUp(e);
            }
        }
        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
        }
        //protected override void OnTextInputStart(TextCompositionEventArgs e)
        //{
        //    base.OnTextInputStart(e);
        //}
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }


            base.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        private void SimulateTabKey()
        {

        }
    }

}
