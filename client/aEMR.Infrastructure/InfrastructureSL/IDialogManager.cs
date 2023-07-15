using System;

using Caliburn.Micro;

namespace aEMR.Infrastructure
{
    public interface IDialogManager
    {
        string BusyContent { get; set;}
        bool IsBusy { get; set; }
        void ShowDialog( string title, IScreen dialogModel, Action<object> callback = null);
        void ShowMessageBox(string message, string title = null, MessageBoxOptions options = MessageBoxOptions.Ok, Action<IMessageBox> callback = null);
    }
    
}
