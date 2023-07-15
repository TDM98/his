using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Castle.Windsor;
using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ILoggerDialog)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoggerDialogViewModel : ViewModelBase, ILoggerDialog
    {
        [ImportingConstructor]
        public LoggerDialogViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
        }
        #region Properties
        private TextBox txtLogContent;
        private string _LogMessage;
        private bool _IsFinished = false;
        public string LogMessage
        {
            get => _LogMessage; set
            {
                _LogMessage = value;
                NotifyOfPropertyChange(() => LogMessage);
            }
        }
        public bool IsFinished
        {
            get => _IsFinished; set
            {
                _IsFinished = value;
                NotifyOfPropertyChange(() => IsFinished);
                if (IsFinished)
                {
                    bOkCancelButtonPressed = IsFinished;
                }
            }
        }
        #endregion
        #region HookedEvents
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001;
        private const uint SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        protected static void DisableCloseButton(IntPtr windowHandle)
        {
            IntPtr menuHandle = GetSystemMenu(windowHandle, false);
            if (menuHandle != IntPtr.Zero)
            {
                EnableMenuItem(menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }
        }

        public void SelectLocation_Loaded(object view, RoutedEventArgs e)
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

        private bool bOkCancelButtonPressed = false;

        public override void CanClose(Action<bool> callback)
        {
            if (!bOkCancelButtonPressed)
            {
                callback(false);
                return;
            }
            base.CanClose(callback);
        }
        #endregion
        #region Events
        public void btnClose()
        {
            bOkCancelButtonPressed = true;
            TryClose();
        }
        public void txtLogContent_Loaded(object sender)
        {
            txtLogContent = sender as TextBox;
        }
        #endregion
        #region Methods
        public void AppendLogMessage(string aLogMessage)
        {
            LogMessage = LogMessage + aLogMessage + Environment.NewLine;
            if (txtLogContent != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    txtLogContent.CaretIndex = txtLogContent.Text.Length;
                    txtLogContent.ScrollToEnd();
                });
            }
        }
        #endregion
    }
}