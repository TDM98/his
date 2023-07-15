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
    public interface IDrugDeptSellPriceProfitScale
    {
        long V_MedProductType { get; set; }
        string TitleForm { get; set; }
        void Init();
        bool mTim{ get; set; }
        bool mTaoMoiCTGia{ get; set; }
        bool mChinhSua { get; set; }

    }
}
