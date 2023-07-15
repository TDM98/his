namespace aEMR.ViewContracts
{
    public interface IVisitor
    {
        string TitleForm { get; set; }
        bool IsPrescriptionCollect { get; set; }
    }
}