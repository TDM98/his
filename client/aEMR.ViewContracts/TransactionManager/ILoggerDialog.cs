namespace aEMR.ViewContracts
{
    public interface ILoggerDialog
    {
        bool IsFinished { get; set; }
        void AppendLogMessage(string aLogMessage);
    }
}