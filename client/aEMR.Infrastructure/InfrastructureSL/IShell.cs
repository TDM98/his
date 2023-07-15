namespace aEMR.Infrastructure
{
    public interface IShell
    {

        bool IsEnabled { get; set; }

        object ContentItem { get; set; }
        object AppHeader { get; set; }
        bool IsLogged { get; set; }

        string BusyMessage { get; set; }
        bool IsBusy { get; set; }

        void ShowBusy(string strBusyText);
        void HideBusy();

    }
}
