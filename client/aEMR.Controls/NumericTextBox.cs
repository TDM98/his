using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.Controls
{
    /// <summary>
    /// Customize text box to only accept number
    /// </summary>
    public class NumericTextBox
    {
        #region MinimumValue Property

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static double GetMinimumValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MinimumValueProperty);
        }

        /// <summary>
        /// Sets the minimum value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetMinimumValue(DependencyObject obj, double value)
        {
            obj.SetValue(MinimumValueProperty, value);
        }

        public static readonly DependencyProperty MinimumValueProperty =
            DependencyProperty.RegisterAttached(
                "MinimumValue",
                typeof(double),
                typeof(NumericTextBox),
                new FrameworkPropertyMetadata(double.NaN, MinimumValueChangedCallback)
                );

        /// <summary>
        /// Minimums the value changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MinimumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = (d as TextBox);
            ValidateTextBox(_this);
        }

        #endregion MinimumValue Property

        #region MaximumValue Property

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static double GetMaximumValue(DependencyObject obj)
        {
            return (double)obj.GetValue(MaximumValueProperty);
        }

        /// <summary>
        /// Sets the maximum value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetMaximumValue(DependencyObject obj, double value)
        {
            obj.SetValue(MaximumValueProperty, value);
        }

        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.RegisterAttached(
                "MaximumValue",
                typeof(double),
                typeof(NumericTextBox),
                new FrameworkPropertyMetadata(double.NaN, MaximumValueChangedCallback)
                );

        /// <summary>
        /// Maximums the value changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MaximumValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox _this = (d as TextBox);
            ValidateTextBox(_this);
        }

        #endregion MaximumValue Property

        #region Mask Property

        /// <summary>
        /// Gets the mask.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static NumericType GetMask(DependencyObject obj)
        {
            return (NumericType)obj.GetValue(MaskProperty);
        }

        /// <summary>
        /// Sets the mask.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetMask(DependencyObject obj, NumericType value)
        {
            obj.SetValue(MaskProperty, value);
        }

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.RegisterAttached(
                "Mask",
                typeof(NumericType),
                typeof(NumericTextBox),
                new FrameworkPropertyMetadata(MaskChangedCallback)
                );

        /// <summary>
        /// Masks the changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void MaskChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is TextBox)
            {
                (e.OldValue as TextBox).PreviewTextInput -= TextBox_PreviewTextInput;
                DataObject.RemovePastingHandler((e.OldValue as TextBox), (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            TextBox _this = (d as TextBox);
            if (_this == null)
                return;

            if ((NumericType)e.NewValue != NumericType.Any)
            {
                _this.PreviewTextInput += TextBox_PreviewTextInput;
                DataObject.AddPastingHandler(_this, (DataObjectPastingEventHandler)TextBoxPastingEventHandler);
            }

            ValidateTextBox(_this);
        }

        #endregion Mask Property

        #region Private Static Methods

        private static void ValidateTextBox(TextBox _this)
        {
            if (GetMask(_this) != NumericType.Any || GetMask(_this) != NumericType.PhoneNumber)
            {
                _this.Text = ValidateValue(GetMask(_this), _this.Text, GetMinimumValue(_this), GetMaximumValue(_this));
            }
        }

        private static void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            string clipboard = e.DataObject.GetData(typeof(string)) as string;
            clipboard = ValidateValue(GetMask(_this), clipboard, GetMinimumValue(_this), GetMaximumValue(_this));
            if (!string.IsNullOrEmpty(clipboard))
            {
                _this.Text = clipboard;
            }
            e.CancelCommand();
            e.Handled = true;
        }

        private static void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox _this = (sender as TextBox);
            if(_this.IsReadOnly) return;

            bool isValid = IsSymbolValid(GetMask(_this), e.Text);
            e.Handled = !isValid;
            if (isValid)
            {
                int caret = _this.CaretIndex;
                string text = _this.Text;
                bool textInserted = false;
                int selectionLength = 0;

                if (_this.SelectionLength > 0)
                {
                    text = text.Substring(0, _this.SelectionStart) +
                            text.Substring(_this.SelectionStart + _this.SelectionLength);
                    caret = _this.SelectionStart;
                }

                if (e.Text == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                {
                    while (true)
                    {
                        int ind = text.IndexOf(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
                        if (ind == -1)
                            break;

                        text = text.Substring(0, ind) + text.Substring(ind + 1);
                        if (caret > ind)
                            caret--;
                    }

                    if (caret == 0)
                    {
                        text = "0" + text;
                        caret++;
                    }
                    else
                    {
                        if (caret == 1 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign)
                        {
                            text = NumberFormatInfo.CurrentInfo.NegativeSign + "0" + text.Substring(1);
                            caret++;
                        }
                    }

                    if (caret == text.Length)
                    {
                        selectionLength = 1;
                        textInserted = true;
                        text = text + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0";
                        caret++;
                    }
                }
                else if (e.Text == NumberFormatInfo.CurrentInfo.NegativeSign)
                {
                    textInserted = true;
                    if (_this.Text.Contains(NumberFormatInfo.CurrentInfo.NegativeSign))
                    {
                        text = text.Replace(NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty);
                        if (caret != 0)
                            caret--;
                    }
                    else
                    {
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + _this.Text;
                        caret++;
                    }
                }

                if (!textInserted)
                {
                    text = text.Substring(0, caret) + e.Text +
                        ((caret < _this.Text.Length) ? text.Substring(caret) : string.Empty);

                    caret++;
                }

                if (GetMask(_this).Equals(NumericType.Integer) || GetMask(_this).Equals(NumericType.Single))
                {
                    try
                    {
                        double val = Convert.ToDouble(text);
                        double newVal = ValidateLimits(GetMinimumValue(_this), GetMaximumValue(_this), val);
                        if (val != newVal)
                        {
                            text = newVal.ToString();
                        }
                        else if (val == 0)
                        {
                            if (!text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)) text = "0";
                        }
                    }
                    catch
                    {
                        text = "0";
                    }

                    while (text.Length > 1 && text[0] == '0'
                           && string.Empty + text[1] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    {
                        text = text.Substring(1);
                        if (caret > 0) caret--;
                    }

                    while (text.Length > 2 && string.Empty + text[0] == NumberFormatInfo.CurrentInfo.NegativeSign
                           && text[1] == '0'
                           && string.Empty + text[2] != NumberFormatInfo.CurrentInfo.NumberDecimalSeparator)
                    {
                        text = NumberFormatInfo.CurrentInfo.NegativeSign + text.Substring(2);
                        if (caret > 1) caret--;
                    }
                }
                else if (GetMask(_this).Equals(NumericType.PhoneNumber))
                {
                    text = text.Trim();
                    //caret -= text.Count(x => x.Equals(' '));
                    //text = text.Replace("  ", " ");
                }

                if (caret > text.Length) caret = text.Length;

                if (text.Length > _this.MaxLength && _this.MaxLength > 0 )
                {
                    _this.Text = text.Substring(0, _this.MaxLength);
                }
                else
                {
                    _this.Text = text;
                }
                _this.CaretIndex = caret;
                _this.SelectionStart = caret;
                _this.SelectionLength = selectionLength;
                e.Handled = true;
            }
        }

        private static string ValidateValue(NumericType mask, string value, double min, double max)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            value = value.Trim();
            switch (mask)
            {
                case NumericType.Integer:
                    try
                    {
                        Convert.ToInt64(value);
                        return value;
                    }
                    catch
                    {
                    }
                    return string.Empty;

                case NumericType.Single:
                    try
                    {
                        Convert.ToDouble(value);

                        return value;
                    }
                    catch
                    {
                    }
                    return string.Empty;
            }

            return value;
        }

        private static double ValidateLimits(double min, double max, double value)
        {
            if (!min.Equals(double.NaN))
            {
                if (value < min)
                    return min;
            }

            if (!max.Equals(double.NaN))
            {
                if (value > max)
                    return max;
            }

            return value;
        }

        private static bool IsSymbolValid(NumericType mask, string str)
        {
            switch (mask)
            {
                case NumericType.Any:
                    return true;

                case NumericType.Integer:
                    if (str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;

                case NumericType.Single:
                    if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator ||
                        str == NumberFormatInfo.CurrentInfo.NegativeSign)
                        return true;
                    break;
            }

            if (mask.Equals(NumericType.Integer) || mask.Equals(NumericType.Single))
            {
                foreach (char ch in str)
                {
                    if (!char.IsDigit(ch))
                        return false;
                }

                return true;
            }
            else if (mask.Equals(NumericType.PhoneNumber))
            {
                foreach (char ch in str)
                {
                    if (!(char.IsDigit(ch) || ch == ' '))
                        return false;
                }

                return true;
            }
            return false;
        }

        #endregion Private Static Methods
    }

    /// <summary>
    /// Enum to specify type of number
    /// </summary>
    public enum NumericType
    {
        /// <summary>
        /// Any type
        /// </summary>
        Any,

        /// <summary>
        /// Integer number
        /// </summary>
        Integer,

        /// <summary>
        /// Single number
        /// </summary>
        Single,

        /// <summary>
        /// Phone number
        /// </summary>
        PhoneNumber
    }
}