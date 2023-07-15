using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using aEMR.Common.DataValidation;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.CommonRecs.Views
{

    public partial class HosHistoryView: AxUserControl
    {
        static SolidColorBrush darkColor = new SolidColorBrush(Color.FromArgb(255, 15, 37, 63));
        static SolidColorBrush lightColor = new SolidColorBrush(Color.FromArgb(255, 75, 105, 137));
        
        public HosHistoryView()
        {
            InitializeComponent();
            
            this.Loaded += new RoutedEventHandler(HosHistoryView_Loaded);
            this.Unloaded += new RoutedEventHandler(HosHistoryView_Unloaded);
        }

        void HosHistoryView_Unloaded(object sender, RoutedEventArgs e)
        {
            //grdCommonRecord.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        void HosHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

       public void InitData()
        {   //            
        }
        
        
        private void testThu()
        {
            bool flag=true;
            flag = validateDay("05/1988");
            flag = validateDay("05/88");
            flag = validateDay("1988");
            flag = validateDay("03/05/1988");
            flag = validateDay("03/05/88");

            flag = validateDay("ss/05/88");
            flag = validateDay("03/33/88");
            flag = validateDay("05/a8");
            flag = validateDay("ssssss");
            flag = validateDay("19818");
            flag = validateDay("198/18");  
        }
        private bool validateDay(string stMatch)
        {
            Regex regStr1 = new Regex(@"\d{2}/\d{2}/\d{4}");
            Regex regStr2 = new Regex(@"\d{2}/\d{4}");
            Regex regStr3 = new Regex(@"\d{4}");
            Regex regStr4 = new Regex(@"\d{2}-\d{2}-\d{4}");
            Regex regStr5 = new Regex(@"\d{2}-\d{4}");
            
            if(regStr1.IsMatch(stMatch))
            {
                return true;
            }else
                if (regStr2.IsMatch(stMatch))
                {
                    return true;
                }
                else
                    if (regStr3.IsMatch(stMatch))
                    {
                        return true;
                    }
                    else
                        if (regStr4.IsMatch(stMatch))
                        {
                            return true;
                        }
                        else
                            if (regStr5.IsMatch(stMatch))
                            {
                                return true;
                            }                           
            return false;
        }


        private void TextBox_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            string st = sender.ToString();
            if (!validateDay( e.ToString()))
            {
                ErrorNotifierBase a = new ErrorNotifierBase();
                a.RaiseErrorsChanged("Ko dung kieu");
            }  
        }


        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string st = sender.ToString();
            if (!validateDay(e.ToString()))
            {
                ErrorNotifierBase a = new ErrorNotifierBase();
                a.RaiseErrorsChanged("Not type of Date");
            }  
        }

        private void grdHosHistory_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            e.Row.Background = new SolidColorBrush(Colors.Orange);
            e.Row.BorderBrush = new SolidColorBrush(Colors.Purple);
            e.Row.Foreground = new SolidColorBrush(Colors.Magenta);
        }

    }
}
