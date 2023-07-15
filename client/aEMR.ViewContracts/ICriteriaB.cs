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

namespace aEMR.ViewContracts
{
    public interface ICriteriaB
    {
        string TextbtViewPrint { get; set; }
        string HienThi1 { get; set; }
        string HienThi2 { get; set; }
        long V_MedProductType { get; set; }
        void GetStore();
    }
}
