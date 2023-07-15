using System;
using System.Net;
using System.Text.RegularExpressions;
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
using System.Globalization;
using aEMR.Infrastructure;
/*
 * 20181026 #001 TTM:   BM 0004200: Thêm điều kiện lại để người dùng nhập 6 số vẫn lấy đúng định dạng. Ví dụ nhập 010118 => 01/01/2018.
 */
namespace aEMR.Controls
{
    public class AxDateTextBox : AxTextBox
    {
        private string _dateFormat;
        public string DateFormat
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_dateFormat))
                {
                    _dateFormat = "dd/MM/yyyy";
                }
                return _dateFormat;
            }
            set { _dateFormat = value; }
        }

        public AxDateTextBox()
            : base()
        {            
            //this.KeyUp += new KeyEventHandler(AxTextBox_KeyUp);
            base.GotFocus += OnGotFocus;
            base.LostFocus += new RoutedEventHandler(AxDateTextBox_LostFocus);
        }

        void AxDateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(this.Text))
            {
                this.Text = string.Empty;
                if (DateChanged != null)
                {
                    var eventArgs = new DateTimeSelectedEventArgs { OldValue = null, NewValue = null };
                    DateChanged(this, eventArgs);
                }
                return;
            }
            //GetText();
            DateTime? dt = GetDate();
            if (dt.HasValue)
            {
                oldDateTime = newDateTime;
                newDateTime = dt;
                if (DateChanged != null)
                {
                    var eventArgs = new DateTimeSelectedEventArgs { OldValue = oldDateTime, NewValue = newDateTime };
                    DateChanged(this, eventArgs);
                }

            }
            else
            {
                //Kiểm tra trường hợp người dùng chỉ nhập vào năm mà thôi.
                
                Regex regex = new Regex(@"^(19|20)\d{2}$");
                Match match = regex.Match(this.Text);
                if(match.Success)
                {            
                    this.Text = string.Empty;
                    return;
                }
                else
                {
                    this.Text = "";
                    oldDateTime = newDateTime;
                    newDateTime = null;
                    if (DateChanged != null)
                    {
                        var eventArgs = new DateTimeSelectedEventArgs { OldValue = oldDateTime, NewValue = newDateTime };
                        DateChanged(this, eventArgs);
                    }
                }
                
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectAll();
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
        DateTime? oldDateTime = null;
        DateTime? newDateTime = null;
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //GetText();
                //base.OnKeyUp(e);
                DateTime? dt = GetDate();

                if (dt.HasValue)
                {
                    oldDateTime = newDateTime;
                    newDateTime = dt;
                    if (DateChanged != null)
                    {
                        var eventArgs = new DateTimeSelectedEventArgs { OldValue = oldDateTime, NewValue = newDateTime };
                        DateChanged(this, eventArgs);
                    }

                }
            }
        }

        private void GetText()
        {
            string currentText = this.Text.Replace('-', '/');
            //string allDigits = Regex.Replace(currentText, "[^0-9]", "", RegexOptions.IgnoreCase);
            //(allDigits.Length >=8)
            string[] dmy = currentText.Split('/');
            if (dmy.Length == 3)
            {
                if (dmy[0].Length == 1)
                {
                    dmy[0] = "0" + dmy[0];
                }
                if (dmy[1].Length == 1)
                {
                    dmy[1] = "0" + dmy[1];
                }
                if (dmy[2].Length == 2)
                {
                    dmy[2] = DateTime.Now.Year.ToString().Substring(0, 2) + dmy[2];
                }
                //currentText = string.Format("{0}/{1}/{2}", allDigits.Substring(0, 2), allDigits.Substring(2, 2), allDigits.Substring(4, 4));
                currentText = string.Format("{0}/{1}/{2}", dmy[0], dmy[1], dmy[2]);
            }
            else if (dmy.Length == 1)
            {
                if (dmy[0].Length == 8)
                {
                    currentText = string.Format("{0}/{1}/{2}", dmy[0].Substring(0, 2), dmy[0].Substring(2, 2), dmy[0].Substring(4, 4));
                }
                //▼====== #001: 
                else if (dmy[0].Length == 6)
                {                    
                    Int16 shortYear = Convert.ToInt16(dmy[0].Substring(4, 2));
                    int calcYear = calcYear = 2000 + shortYear; 
                    if (UseBothCenturies)
                    {                                               
                        if (shortYear > (Globals.GetCurServerDateTime().Year - 2000))
                        {
                            calcYear = 1900 + shortYear;
                        }                        
                    }

                    currentText = string.Format("{0}/{1}/{2}", dmy[0].Substring(0, 2),
                                                               dmy[0].Substring(2, 2),
                                                               calcYear.ToString());
                    
                }
                //▲====== #001
            }
            this.Text = currentText;
        }
        /// <summary>
        /// Trả về ngày tháng được nhập trên textbox nếu được.
        /// Nếu nhập không đúng DateFormat thì trả về null
        /// </summary>
        /// <returns></returns>
        private DateTime? GetDate()
        {
            GetText();
            DateTime dt;
            if(DateTime.TryParseExact(this.Text, DateFormat, null, DateTimeStyles.None, out dt))
            {
                return dt;
            }
            return null;
        }
        public event EventHandler<DateTimeSelectedEventArgs> DateChanged;
        private void SimulateTabKey()
        {

        }

        public DateTime? Date
        {
            get { return (DateTime?)GetValue(DateProperty); }
            set
            {
                SetValue(DateProperty, value);
            }
        }

        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
            "Date",
            typeof(DateTime?),
            typeof(AxDateTextBox),
            new PropertyMetadata(null, new PropertyChangedCallback(OnDateChanged)));

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newDate = e.NewValue as DateTime?;
            if (newDate.HasValue)
            {
                ((AxDateTextBox)d).Text = newDate.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                ((AxDateTextBox)d).Text = string.Empty;
            }
        }

        public bool UseBothCenturies
        {
            get
            {
                return (bool)GetValue(UseBothCenturiesProperty);
            }
            set
            {
                SetValue(UseBothCenturiesProperty, value);
            }
        }

        public static readonly DependencyProperty UseBothCenturiesProperty = DependencyProperty.Register(
            "UseBothCenturies",
            typeof(bool),
            typeof(AxDateTextBox),
            new PropertyMetadata(false, new PropertyChangedCallback(OnUseBothCenturies_Changed)));

        private static void OnUseBothCenturies_Changed(DependencyObject thisObj, DependencyPropertyChangedEventArgs theArgs)
        {
        }

    }
    public class DateTimeSelectedEventArgs : EventArgs
    {
        public DateTime? OldValue { get; set; }
        public DateTime? NewValue { get; set; }
        public bool YearOnly { get; set; }
    }
}
