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
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPharmacySellingPriceList_AddEdit
    {
        string TitleForm { get; set; }

        void InitializeNewItem();
        PharmacySellingPriceList ObjPharmacySellingPriceList_Current { get; set; }

        //Visibility dgCellTemplate0_Visible { get; set; }/* Ẩn nút xóa, vì BG khi tạo xong thì ko cho phép Xóa */

        //bool btChooseItemFromDelete_IsEnabled { get; set; }
        //bool dpEffectiveDate_IsEnabled { get; set; }
        //bool btSave_IsEnabled { get; set; }
        DateTime EndDate { get; set; }
        DateTime BeginDate { get; set; }
        bool dpEffectiveDate_IsEnabled { get; set; }
    }
}
