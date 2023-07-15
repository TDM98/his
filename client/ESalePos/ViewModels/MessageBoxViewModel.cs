using System;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Windsor;

namespace aEMRClient.ViewModels
{
    [Export(typeof(IMessageBox)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class MessageBoxViewModel : Conductor<object>, IMessageBox
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MessageBoxViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        public aEMR.Infrastructure.MessageBoxOptions Options { get; set; }

        public void CloseUsingXButton()
        {
            OnClosed();
        }

        public void Ok()
        {
            Select(aEMR.Infrastructure.MessageBoxOptions.Ok);
            CloseForm();
        }

        public void Cancel()
        {
            Select(aEMR.Infrastructure.MessageBoxOptions.Cancel);
            CloseForm();
        }

        public void Yes()
        {
            Select(aEMR.Infrastructure.MessageBoxOptions.Yes);
            CloseForm();
        }

        public void No()
        {
            Select(aEMR.Infrastructure.MessageBoxOptions.No);
            CloseForm();
        }

        bool IsVisible(aEMR.Infrastructure.MessageBoxOptions opt)
        {
            return (Options & opt) == opt;
        }

        void Select(aEMR.Infrastructure.MessageBoxOptions opt)
        {
            if(Enum.IsDefined(typeof(AxMessageBoxResult),(int)opt))
            {
                _result = (AxMessageBoxResult) (int) opt;
            }
            else
            {
                _result = AxMessageBoxResult.Unknown;
            }

            //CloseForm();
        }

        private void CloseForm()
        {
            TryClose();
            OnClosed();
        }

        private void OnClosed()
        {
            //var shell = Globals.GetViewModel<IShell>();
            //shell.IsEnabled = true;
            Globals.EventAggregator.Publish(new ItemSelected<IMessageBox, AxMessageBoxResult>() {Sender = this, Item = _result});
        }


        private AxMessageBoxResult _result = AxMessageBoxResult.Unknown;
        public bool OkVisible
        {
            get { return IsVisible(aEMR.Infrastructure.MessageBoxOptions.Ok); }
        }

        public bool CancelVisible
        {
            get { return IsVisible(aEMR.Infrastructure.MessageBoxOptions.Cancel); }
        }

        public bool YesVisible
        {
            get { return IsVisible(aEMR.Infrastructure.MessageBoxOptions.Yes); }
        }

        public bool NoVisible
        {
            get { return IsVisible(aEMR.Infrastructure.MessageBoxOptions.No); }
        }


        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                if(_caption != value)
                {
                    _caption = value;
                    NotifyOfPropertyChange(()=>Caption);
                }
            }
        }

        private string _messageBoxText;
        public string MessageBoxText
        {
            get
            {
                return _messageBoxText;
            }
            set
            {
                if(_messageBoxText != value)
                {
                    _messageBoxText = value;
                    NotifyOfPropertyChange(()=>MessageBoxText);
                }
            }
        }

        public AxMessageBoxResult Result
        {
            get { return _result; }
        }
    }
}
