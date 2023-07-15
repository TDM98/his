namespace aEMR.ViewContracts
{
    public interface IOutwardInternal
    {
        string TitleForm { get; set; }

        long StoreType { get; set; }

        int ViewCase { get; set; }
    }
}
