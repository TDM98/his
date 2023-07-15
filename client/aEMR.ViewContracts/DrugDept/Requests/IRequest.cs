using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IRequest
    {
        long V_MedProductType { get; set; }
        void RefeshRequest();
        string strHienThi { get; set; }

        bool mDuyetPhieuLinhHang_Tim { get; set; }
        bool mDuyetPhieuLinhHang_PhieuMoi { get; set; }
        bool mDuyetPhieuLinhHang_XuatHang { get; set; }
        bool mDuyetPhieuLinhHang_XemInTH { get; set; }
        bool mDuyetPhieuLinhHang_XemInCT { get; set; }
    }
}
