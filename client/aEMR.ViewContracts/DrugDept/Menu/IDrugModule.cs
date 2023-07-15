namespace aEMR.ViewContracts
{
    public interface IDrugModule
    {
        object MainContent { get; set; }
        bool[] MenuVisibleCollection { get; set; }
    }
}