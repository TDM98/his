namespace aEMR.ViewContracts
{
    public interface IConsultingDiagnosysReport
    {
        string TitleForm { get; set; }
        bool IsWaitOnly { get; set; }
    }
}
