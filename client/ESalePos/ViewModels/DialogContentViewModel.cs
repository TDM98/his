using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using Castle.Windsor;
using aEMR.Infrastructure.ViewUtils;

using aEMR.ViewContracts;
using aEMR.Common.Enums;

namespace aEMRClient.ViewModels
{
    [Export(typeof(aEMR.ViewContracts.IDialogContentViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DialogContentViewModel : Conductor<object>.Collection.OneActive, aEMR.ViewContracts.IDialogContentViewModel
    {
        private Action<aEMR.Common.Enums.MsgBoxOptions, IScreen> _callbackResult;
        
        private object _viewModel;

        public DialogContentViewModel()
        {
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

        public string Message { get; set; }

        public aEMR.Common.Enums.MsgBoxOptions Option { get; set; }

        public Action<aEMR.Common.Enums.MsgBoxOptions, IScreen> CallbackResult
        {
            get { return _callbackResult; }
            set { _callbackResult = value; }
        }

        public TViewModel DisplayContent_V2<TViewModel>(
            Action<TViewModel> initAction
            , Action<TViewModel> onClose
            , aEMR.Common.Enums.MsgBoxOptions msgBoxOptions = aEMR.Common.Enums.MsgBoxOptions.Ok
            , Action<aEMR.Common.Enums.MsgBoxOptions, IScreen> callbackResult = null)
        {
            _viewModel = IoC.Get<TViewModel>();
            GetValue(initAction, msgBoxOptions, callbackResult);
            return  (TViewModel)_viewModel;
        }

        public void DisplayContent<TViewModel>(
            Action<TViewModel> initAction
            , Action<TViewModel> onClose
            , aEMR.Common.Enums.MsgBoxOptions msgBoxOptions = aEMR.Common.Enums.MsgBoxOptions.Ok
            , Action<aEMR.Common.Enums.MsgBoxOptions, IScreen> callbackResult = null)
        {
            _viewModel = IoC.Get<TViewModel>();
            GetValue(initAction, msgBoxOptions, callbackResult);
        }

        public void DisplayContent<TViewModel>(
            IScreen viewModel
            , Action<TViewModel> initAction = null
            , aEMR.Common.Enums.MsgBoxOptions msgBoxOptions = aEMR.Common.Enums.MsgBoxOptions.Ok
            , Action<aEMR.Common.Enums.MsgBoxOptions, IScreen> callbackResult = null
            ) where TViewModel : IScreen
        {
            _viewModel = viewModel;
            Option = msgBoxOptions;
            GetValue(initAction, msgBoxOptions, callbackResult);
        }

        public void DisplayContent<TViewModel>(
            Type viewModel
            , Action<TViewModel> initAction = null
            , aEMR.Common.Enums.MsgBoxOptions msgBoxOptions = aEMR.Common.Enums.MsgBoxOptions.Ok
            , Action<aEMR.Common.Enums.MsgBoxOptions, IScreen> callbackResult = null
            ) where TViewModel : IScreen
        {
            Option = msgBoxOptions;
            GetValue(initAction, msgBoxOptions, callbackResult );
        }

        private void GetValue<TViewModel>(Action<TViewModel> initAction, aEMR.Common.Enums.MsgBoxOptions msgBoxOptions, Action<aEMR.Common.Enums.MsgBoxOptions, IScreen> callbackResult)
        {
            Option = msgBoxOptions;

            ActiveItem = _viewModel;

            ActivateItem(_viewModel);

            NotifyOfPropertyChange(() => OkVisible);
            NotifyOfPropertyChange(() => YesVisible);
            NotifyOfPropertyChange(() => CancelVisible);
            NotifyOfPropertyChange(() => NoVisible);
            NotifyOfPropertyChange(() => CloseVisible);

            if (callbackResult != null)
            {
                CallbackResult = callbackResult;
            }

            if (initAction != null)
            {
                initAction.Invoke((TViewModel) _viewModel);
            }

            if (_viewModel is IScreen)
            {
                DisplayName = (_viewModel as IScreen).DisplayName;
            }
            else
            {
                DisplayName = " aEMR - advanced Electronic Medical Records ";
            }
        }

        

        protected override void OnActivate()
        {
            base.OnActivate();
            if ( _viewModel is IScreen )
            {
                DisplayName = (_viewModel as IScreen).DisplayName;
            }
            else
            {
                DisplayName = " Point Of Sale ";    
            }

            var window = GetView() as Window;
            if ( window != null)
            {
                window.Closed += (sender, args) => ClosedWindow();    
            }
            
        }
        
        private void ClosedWindow()
        {
            Select(aEMR.Common.Enums.MsgBoxOptions.None);
        }

        public void OkCommand()
        {
            Select(aEMR.Common.Enums.MsgBoxOptions.Ok);
        }

        public bool OkVisible
        {
            get
            {
                var data = IsVisible(aEMR.Common.Enums.MsgBoxOptions.Ok);
                return data;
            }
        }

        public bool CancelVisible
        {
            get { return IsVisible(aEMR.Common.Enums.MsgBoxOptions.Cancel); }
        }

        public bool CloseVisible
        {
            get { return IsVisible(aEMR.Common.Enums.MsgBoxOptions.Close); }
        }

        public bool YesVisible
        {
            get
            {
                var data = IsVisible(aEMR.Common.Enums.MsgBoxOptions.Yes);
                return data;
            }
        }

        public bool NoVisible
        {
            get { return IsVisible(aEMR.Common.Enums.MsgBoxOptions.No); }
        }

        
        public bool WasSelected(aEMR.Common.Enums.MsgBoxOptions option)
        {
            return (Option & option) == option;
        }

        bool IsVisible(aEMR.Common.Enums.MsgBoxOptions option)
        {
            return (Option & option) == option;
        }

        public void Select(aEMR.Common.Enums.MsgBoxOptions option)
        {
            Option = option;
            if (CallbackResult != null)
            {
                CallbackResult.Invoke(option, _viewModel as IScreen);
            }
            var naviationService = IoC.Get<INavigationService>();
            naviationService.CloseCurrentWindow();
            TryClose();
           
        }


        public void CancelCommand()
        {
            Select(aEMR.Common.Enums.MsgBoxOptions.Cancel);
        }

        public void CloseCommand()
        {
            Select(aEMR.Common.Enums.MsgBoxOptions.Close);
        }

        public void YesCommand()
        {
            Select(aEMR.Common.Enums.MsgBoxOptions.Yes);   
        }

        public void NoCommand()
        {
            Select(aEMR.Common.Enums.MsgBoxOptions.No);
        }

        public void NoneCommand()
        {
            Select(aEMR.Common.Enums.MsgBoxOptions.None);
        }
    }
}
