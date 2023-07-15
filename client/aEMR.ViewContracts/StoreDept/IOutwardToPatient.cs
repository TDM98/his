using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IOutwardToPatient
    {
        AllLookupValues.MedProductType V_MedProductType { get; set; }
        // void InitializeInvoice();
        string strHienThi { get; set; }

        bool mXuatChoBenhNhan_Xem { get; set; }
        bool mXuatChoBenhNhan_PhieuMoi { get; set; }
        bool mXuatChoBenhNhan_XemIn { get; set; }
        bool mXuatChoBenhNhan_In { get; set; }
        bool IsCasualOrPreOpPt { get; set; }
        void InitData();
    }
}
