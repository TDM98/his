namespace aEMR.ViewContracts
{
    public interface ICollectionDrug
    {
        string TitleForm { get; set; }
        bool? bFlagStoreHI { get; set; }
        bool bFlagPaidTime { get; set; }
    }
}
