using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using aEMR.DataContracts;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Windows.Controls;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IError)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ErrorViewModel : Conductor<object>, IError
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ErrorViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ErrorTitle = string.Format("{0}:", eHCMSResources.G0449_G1_TBaoLoi);
        }

        private string _errorTitle;
        public string ErrorTitle
        {
            get { return _errorTitle; }
            set
            {
                _errorTitle = value;
                NotifyOfPropertyChange(()=>ErrorTitle);
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(()=>ErrorMessage);
            }
        }

        private string _errorHeader;

        public string ErrorHeader
        {
            get { return _errorHeader; }
            set
            {
                _errorHeader = value;
                NotifyOfPropertyChange(()=>ErrorHeader);
            }
        }

        public void SetError(AxErrorEventArgs axError)
        {
            AxErrorEventArgs args = axError as AxErrorEventArgs;
            if (args == null)
            {
                return;
            }
            if (args.ClientError != null)
            {
                ConvertExceptionToMessage(args.ClientError);
            }
            else if (args.ServerError != null)
            {
                ConvertServerErrorToMessage(args.ServerError);
            }
        }
        private void ConvertServerErrorToMessage(FaultException<AxException> faultException)
        {
            ErrorHeader = faultException.Reason == null ? "" : faultException.Reason.ToString();
            ErrorMessage = "";
            if (faultException.Detail != null)
            {
                //ErrorMessage += "Module: " + faultException.Detail.ModuleName;
                //ErrorMessage += "\nMethodName: " + faultException.Detail.MethodName;
                //ErrorMessage += "\nErrorCode: " + faultException.Detail.ErrorCode;
                ErrorMessage += "\nMessage: " + faultException.Detail.ErrorMessage;
            }
        }
        private void ConvertExceptionToMessage(Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        public void OkCmd()
        {
            TryClose();
        }

        public object LayoutRoot{ get; set;}
        public void LayoutRoot_Loaded(object sender) 
        {
            LayoutRoot = sender as Grid;
        }

        public void SetControlTheme(int width, int height ) 
        {
            ((Grid)LayoutRoot).Width = width;
            ((Grid)LayoutRoot).Height = height;
            
        }
    }
}
