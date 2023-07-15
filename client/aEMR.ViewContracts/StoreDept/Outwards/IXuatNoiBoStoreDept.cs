using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IXuatNoiBoStoreDept
    {
        AllLookupValues.MedProductType V_MedProductType { get; set; }
        // void InitializeInvoice();
        string strHienThi { get; set; }


        bool mXuatTraHang_Xem { get; set; }
        bool mXuatTraHang_PhieuMoi { get; set; }
        bool mXuatTraHang_XemIn { get; set; }
        bool mXuatTraHang_In { get; set; }

        void InitData();
        int ViewCase { get; set; }
    }
}
