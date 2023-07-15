using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace aEMR.Controls
{
    public class AxMultilineTextBox : TextBox
    {
        public AxMultilineTextBox()
            : base()
        {
            base.GotFocus += OnGotFocus;

            this.AcceptsReturn = false;
            this.TextWrapping = TextWrapping.Wrap;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectAll();
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            bool controlKeyPressed = (Keyboard.Modifiers & ModifierKeys.Control) != 0;

            if (e.Key == Key.Enter)
            {
                if (controlKeyPressed)
                {
                    base.OnKeyUp(e);

                    string curText = this.Text;
                    BindingExpression textBinding = this.GetBindingExpression(TextBox.TextProperty);
                    if(textBinding != null)
                    {
                        this.SetValue(TextBox.TextProperty,curText + Environment.NewLine);
                    }
                    else
                    {
                        this.Text = curText + Environment.NewLine;
                    }
                    this.SelectionStart = this.Text.Length;
                    e.Handled = true;
                }
                else
                {
                    //Khong lam gi het
                }
            }
            else
            {
                base.OnKeyUp(e);
            }
        }

        private void SimulateTabKey()
        {

        }
    }
}
