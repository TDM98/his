using aEMR.Controls;
using aEMR.Infrastructure;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace aEMR.Common.Views
{
    public partial class UCDateTimeHourPicker : UserControl, IAXUserControl
    {
        public int Hour;
        public int Minute;
        private DateTime? _SelectedValue;
        public DateTime? SelectedValue
        {
            get
            {
                return _SelectedValue == null ? null : new DateTime(_SelectedValue.Value.Year, _SelectedValue.Value.Month, _SelectedValue.Value.Day, Hour, Minute, 0) as DateTime?;
            }
            set
            {
                _SelectedValue = value;
            }
        }
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register("SelectedValue",
               typeof(DateTime?),
               typeof(UCDateTimeHourPicker),
               new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedDateChanged)));
        private static void OnSelectedDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var NewValue = e.NewValue as DateTime?;
            ((UCDateTimeHourPicker)sender).SelectedValue = NewValue;
            if (NewValue.HasValue && NewValue != null)
            {
                ((UCDateTimeHourPicker)sender).Hour = NewValue.Value.Hour;
                ((UCDateTimeHourPicker)sender).Minute = NewValue.Value.Minute;
                ((UCDateTimeHourPicker)sender).txtHour.Text = NewValue.Value.Hour.ToString();
                ((UCDateTimeHourPicker)sender).txtMinute.Text = NewValue.Value.Minute.ToString();
                ((UCDateTimeHourPicker)sender).txtDate.Text = NewValue.Value.ToString("dd/MM/yyyy");
            }
        }
        public UCDateTimeHourPicker()
        {
            InitializeComponent();
        }
        private void NumbericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int ASCIIKey = (int)e.Key;
            e.Handled = !(ASCIIKey >= 34 && ASCIIKey <= 43
                          || ASCIIKey >= 74 && ASCIIKey <= 83
                          || ASCIIKey == 2
                          || ASCIIKey == 3);
        }
        private void DateTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int ASCIIKey = (int)e.Key;
            e.Handled = !(ASCIIKey >= 34 && ASCIIKey <= 43
                          || ASCIIKey >= 74 && ASCIIKey <= 83
                          || ASCIIKey == 2
                          || ASCIIKey == 3
                          || ASCIIKey == 145);
        }
        private void DateTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            string CurrentText = (sender as TextBox).Text;
            DateTime CurrentDate;
            if (DateTime.TryParseExact(CurrentText, "dd/MM/yyyy", null, DateTimeStyles.None, out CurrentDate))
            {
                (sender as TextBox).Text = CurrentDate.ToString("dd/MM/yyyy");
                SelectedValue = CurrentDate;
                CallSetValue();
                return;
            }
            else if (DateTime.TryParseExact(CurrentText, "ddMMyyyy", null, DateTimeStyles.None, out CurrentDate))
            {
                (sender as TextBox).Text = CurrentDate.ToString("dd/MM/yyyy");
                SelectedValue = CurrentDate;
                CallSetValue();
                return;
            }
            else if (DateTime.TryParseExact(CurrentText, "ddMMyy", null, DateTimeStyles.None, out CurrentDate))
            {
                (sender as TextBox).Text = CurrentDate.ToString("dd/MM/yyyy");
                SelectedValue = CurrentDate;
                CallSetValue();
                return;
            }
            else if (!string.IsNullOrEmpty(CurrentText) && (CurrentText.ToLower() == "bg" || CurrentText.ToLower() == "now"))
            {
                var Now = Globals.GetCurServerDateTime();
                Hour = Now.Hour;
                Minute = Now.Minute;
                (sender as TextBox).Text = Now.ToString("dd/MM/yyyy");
                SelectedValue = Now;
                CallSetValue();
                return;
            }
            SelectedValue = null;
            (sender as TextBox).Text = "";
            CallSetValue();
        }
        private void Hour_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            string CurrentText = (sender as TextBox).Text;
            if (CurrentText == Hour.ToString())
            {
                return;
            }
            int CurrentValue = 0;
            if (int.TryParse(CurrentText, out CurrentValue))
            {
                if (!(CurrentValue >= 0 && CurrentValue <= 24))
                {
                    Hour = 0;
                    (sender as TextBox).Text = "";
                }
                else
                {
                    Hour = CurrentValue;
                }
                CallSetValue();
                return;
            }
            Hour = 0;
            (sender as TextBox).Text = "";
            CallSetValue();
        }
        private void Minute_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            string CurrentText = (sender as TextBox).Text;
            if (CurrentText == Minute.ToString())
            {
                return;
            }
            int CurrentValue = 0;
            if (int.TryParse(CurrentText, out CurrentValue))
            {
                if (!(CurrentValue >= 0 && CurrentValue <= 60))
                {
                    Minute = 0;
                    (sender as TextBox).Text = "";
                }
                else
                {
                    Minute = CurrentValue;
                }
                CallSetValue();
                return;
            }
            Minute = 0;
            (sender as TextBox).Text = "";
            CallSetValue();
        }
        private void NumbericTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
        private void CallSetValue()
        {
            SetValue(DateProperty, SelectedValue);
        }
        public bool IsLastControlEditing
        {
            get
            {
                return txtDate.IsFocused || txtDate.IsKeyboardFocused;
            }
        }
    }
}