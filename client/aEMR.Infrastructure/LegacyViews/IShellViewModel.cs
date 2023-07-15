namespace aEMR.ViewContracts
{
    public interface IShellViewModel
    {
        bool IsBusy { get; set; }
        string BusyContent { get; set; }
        string TheCurUserName { get; set; }
        string SiteName { get; set; }
        bool IsBtnVisible { get; set; }
        bool isUserLoggedIn { get; set; }        
        void ShowBusy(string strBusyText);
        void HideBusy();
        string Message { get; set; }
        void NotifyChanged();
    }
}