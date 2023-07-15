namespace aEMR.ViewContracts
{
    public interface IXuatHangKyGoi
    {
        long V_MedProductType { get; set; }
       // void InitializeInvoice();
        string strHienThi { get; set; }
        bool mXuatHangKyGui_Tim { get; set; }
        bool mXuatHangKyGui_PhieuMoi { get; set; }
        bool mXuatHangKyGui_Save { get; set; }
        bool mXuatHangKyGui_ThuTien { get; set; }
        bool mXuatHangKyGui_XemIn { get; set; }
        bool mXuatHangKyGui_In { get; set; }
        bool mXuatHangKyGui_DeleteInvoice { get; set; }
        bool mXuatHangKyGui_PrintReceipt { get; set; }

        bool mIsInputTemp { get; set; }/*Xuất Chưa Nhập Hàng*/

        void LoadRefOutputType();
    }
}
