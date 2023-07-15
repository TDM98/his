namespace aEMR.ViewContracts
{
    public interface IStoreDeptStockTakes
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mKiemKe_Tim { get; set; }
        bool mKiemKe_ThemMoi { get; set; }
        bool mKiemKe_XuatExcel { get; set; }
        bool mKiemKe_XemIn { get; set; }

        //20210916 QTD quyen mo khoa kho
        bool mKiemKe_MoKho { get; set; }
        bool mKiemKe_KhoaKho { get; set; }
        bool mKiemKe_KhoaTatCa { get; set; }
        void InitData();
    }
}
