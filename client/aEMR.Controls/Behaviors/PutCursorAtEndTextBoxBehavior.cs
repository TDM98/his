using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace aEMR.Controls
{
    public class PutCursorAtEndTextBoxBehavior : Behavior<UIElement>
    {
        private TextBox _textBox;

        protected override void OnAttached()
        {
            base.OnAttached();

            _textBox = AssociatedObject as TextBox;

            if (_textBox == null)
            {
                return;
            }
            _textBox.GotFocus += TextBoxGotFocus;
        }

        protected override void OnDetaching()
        {
            if (_textBox == null)
            {
                return;
            }
            _textBox.GotFocus -= TextBoxGotFocus;

            base.OnDetaching();
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            _textBox.CaretIndex = _textBox.Text.Length;
        }
    }
}
