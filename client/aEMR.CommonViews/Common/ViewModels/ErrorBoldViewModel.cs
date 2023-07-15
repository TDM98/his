using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using aEMR.DataContracts;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Windows.Controls;
using eHCMSLanguage;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using aEMR.Common.BaseModel;
/*
 * 20200831 #001 TTM: BM 0040435: Fix lỗi toa thuốc xuất viện
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IErrorBold)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ErrorBoldViewModel : Conductor<object>, IErrorBold
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ErrorBoldViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ErrorTitle = eHCMSResources.K1576_G1_CBao;
            isCheckBox = true;
        }

        //~ErrorBoldViewModel()
        //{
        //    int y = 2;
        //    ClientLoggerHelper.LogInfo("ErrorBoldViewModel DESTRUCTOR : " + y.ToString());
        //}

        private string _TitleOkBtn = "Đồng ý";
        public string TitleOkBtn
        {
            get { return _TitleOkBtn; }
            set
            {
                _TitleOkBtn = value;
                NotifyOfPropertyChange(() => TitleOkBtn);
            }
        }

        private string _ErrorColor = "Crimson";
        public string ErrorColor
        {
            get { return _ErrorColor; }
            set
            {
                _ErrorColor = value;
                NotifyOfPropertyChange(() => ErrorColor);
            }
        }

        private bool _fireOncloseEvent = false;
        public bool FireOncloseEvent
        {
            get { return _fireOncloseEvent; }
            set { _fireOncloseEvent = value; }
        }

        private bool _isCheckBox=true;
        public bool isCheckBox
        {
            get { return _isCheckBox; }
            set
            {
                _isCheckBox = value;
                IsBlock = !_isCheckBox;
                NotifyOfPropertyChange(() => isCheckBox);
            }
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

        private bool _IsAccept;
        public bool IsAccept
        {
            get { return _IsAccept; }
            set
            {
                _IsAccept = value;
                NotifyOfPropertyChange(() => IsAccept);
            }
        }

        private string _CheckBoxContent=eHCMSResources.K3847_G1_DongY;
        public string CheckBoxContent
        {
            get { return _CheckBoxContent; }
            set
            {
                _CheckBoxContent = value;
                NotifyOfPropertyChange(() => CheckBoxContent);
            }
        }

        private bool _IsBlock;
        public bool IsBlock
        {
            get { return _IsBlock; }
            set
            {
                _IsBlock = value;
                NotifyOfPropertyChange(() => IsBlock);
            }
        }
        private bool _IsShowReason;
        public bool IsShowReason
        {
            get { return _IsShowReason; }
            set
            {
                _IsShowReason = value;
                NotifyOfPropertyChange(() => IsShowReason);
            }
        }

        private string _Reason;
        public string Reason
        {
            get { return _Reason; }
            set
            {
                _Reason = value;
                NotifyOfPropertyChange(() => Reason);
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

        public void SetMessage(string message, string _checkBoxContent)
        {
            ErrorMessage = message;
            if (!string.IsNullOrEmpty(_checkBoxContent))
            {
                CheckBoxContent = _checkBoxContent;
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

        //private Action<bool> OnDeActivateCallback = null;

        public event Action<bool> DeActivateEvent = null;

        public void SetDeActivateCallback(Action<bool> deActivateCallback)
        {
            DeActivateEvent += deActivateCallback;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            //20191023 TBL: Khi tick vào đồng ý lưu nhưng lại bấm dấu X để thoát thì set IsAccept = false để không đi lưu
            if (!ClickOK)
            {
                IsAccept = false;
            }
            else
            {
                ClickOK = false;
            }
        }

        public void OkCmd()
        {
            //ClientLoggerHelper.LogInfo("ErrorBoldViewModel OkCmd BEGIN.");
            //(GetView() as ChildWindow).Visibility = System.Windows.Visibility.Collapsed;

            //TryClose();
            if (IsShowReason && string.IsNullOrEmpty(Reason) && IsAccept)
            {
                MessageBox.Show(eHCMSResources.Z2860_G1_ChuaNhapLyDo);
                return;
            }
            
            if (DeActivateEvent != null)
            {
                DeActivateEvent(IsAccept);
            }
            
            if (FireOncloseEvent)
            {
                //ClientLoggerHelper.LogInfo("ErrorBoldViewModel FireOncloseEvent.");
                Globals.EventAggregator.Publish(new WarningConfirmMsgBoxClose() { IsConfirmed = IsAccept });
            }
            //ClientLoggerHelper.LogInfo("ErrorBoldViewModel OkCmd END.");
            ClickOK = true;
            TryClose();
        }
        bool ClickOK;

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
        #region Disable Remove button
        //▼===== #001
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        public void ErrorBold_Loaded(object view, RoutedEventArgs e)
        {
            base.OnViewLoaded(view);
            var theWindow = (view as FrameworkElement).GetWindow();
            IntPtr hnwdSelPopup = new WindowInteropHelper(theWindow).Handle;

            if (hnwdSelPopup != null)
            {
                IntPtr menuHandle = GetSystemMenu(hnwdSelPopup, false);
                if (menuHandle != IntPtr.Zero)
                {
                    EnableMenuItem(menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
                }
            }
        }
        //▲===== #001
        #endregion
    }
}
