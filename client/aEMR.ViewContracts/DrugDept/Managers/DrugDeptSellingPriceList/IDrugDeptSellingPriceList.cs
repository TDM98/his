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
    public interface IDrugDeptSellingPriceList
    {
        long V_MedProductType { get; set; }
        string TitleForm { get; set; }
        void Init();

        bool mXem { get; set; }
        bool mChinhSua { get; set; }
        bool mTaoBangGia { get; set; }
        bool mPreView { get; set; }
        bool mIn { get; set; }
    }
}
