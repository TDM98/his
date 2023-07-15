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

using System.Windows.Data;
namespace aEMR.Controls
{
    public class EventConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool boolVal = (bool)value;
            return boolVal ? new SolidColorBrush(Color.FromArgb(255, 250, 150, 150)) :
                new SolidColorBrush(Color.FromArgb(255, 250, 250, 150));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public EventHandler Test
        {
            get 
            { return new EventHandler(TestOp); }
        }
        public void TestOp(object sender,EventArgs args)
        {

        }
    }
}
