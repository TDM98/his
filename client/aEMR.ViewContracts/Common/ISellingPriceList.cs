
namespace aEMR.ViewContracts
{
    public interface ISellingPriceList
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
