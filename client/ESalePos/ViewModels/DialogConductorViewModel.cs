using System;

using System.ComponentModel.Composition;

using System.Windows;

using aEMR.Infrastructure;

using Caliburn.Micro;

namespace aEMRClient.ViewModels
{
    [Export(typeof(IDialogManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DialogConductorViewModel : Conductor<object>, IDialogManager
    {
        readonly Func<IMessageBox> _createMessageBox;

        [ImportingConstructor]
        public DialogConductorViewModel(Func<IMessageBox> messageBoxFactory)
        {
            _createMessageBox = messageBoxFactory;
        }

        private string _dialogTitle;
        public string DialogTitle
        {
            get { return _dialogTitle; }
            set
            {
                _dialogTitle = value;
                NotifyOfPropertyChange(()=>DialogTitle);
            }
        }
        private string _busyContent;
        public string BusyContent
        {
            get { return _busyContent; }
            set
            {
                _busyContent = value;
                NotifyOfPropertyChange(() => BusyContent);
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }


        public void ShowDialog(IScreen dialogModel)
        {
            ActiveItem = null;
            ActivateItem(dialogModel);
        }

        public void ShowMessageBox(string message, string title = null, aEMR.Infrastructure.MessageBoxOptions options = aEMR.Infrastructure.MessageBoxOptions.Ok, Action<IMessageBox> callback = null)
        {
            var box = _createMessageBox();

            box.Caption = title ?? "";
            box.Options = options;
            box.MessageBoxText = message;

            //if (callback != null)
            //    box.Deactivated += delegate { callback(box); };

            ActiveItem = null;
            ActivateItem(box);
        }


        public void ShowDialog(string title ,IScreen dialogModel, Action<object> callback = null)
        {
            var shell = Globals.GetViewModel<IShell>();
            shell.IsEnabled = true;
      
            ShowDialog( dialogModel);
        }



    }

}
