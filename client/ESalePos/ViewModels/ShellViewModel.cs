using System;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Deployment.Application;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.ViewUtils;
using aEMR.ViewContracts;
using aEMR.Common.Printing;
using aEMR.Common.ConfigurationManager.Printer;
using eHCMSLanguage;
using aEMR.Infrastructure.Events;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DevExpress.Pdf;
using System.Drawing.Printing;
using System.Linq;
using aEMR.Common;
/*
* 20181208 #001 TTM: BM 0005285: Clear dữ liệu trong Globals liên quan đến bệnh nhân trước khi vào Module chẩn đoán.
*/
namespace aEMRClient.ViewModels
{
    [Export(typeof(aEMR.ViewContracts.IShellViewModel))]
    public class ShellViewModel : ConductorView<object>, aEMR.ViewContracts.IShellViewModel,
        IHandle<BusyEvent>, IHandle<ShowDialogEvent>
        , IHandle<ErrorNotification>
        , IHandle<ErrorOccurred>
        , IHandle<ErrorBoldOccurred>
        , IHandle<ValidateFailedEvent>, IHandle<LogOutEvent>
        , IHandle<ShowMessageEvent>
        , IHandle<LocationSelected_New>
        , IHandle<ActiveXPrintEvt>
        , IHandle<AuthorizationEvent>
        , IHandle<isConsultationStateEditEvent>
        , IHandle<AppCheckAndDownloadUpdateCompletedEvent>
        , IHandle<ShowWaringSegments>
    {
        private INavigationService _navigationService { get; set; }
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ShellViewModel(IWindsorContainer container, INavigationService service, IEventAggregator eventAggregator, ISalePosCaching salePosCaching) : base(container)
        {
            _navigationService = service;
            _salePosCaching = salePosCaching;
            _logger = container.Resolve<ILogger>();
            eventAggregator.Subscribe(this);
            Globals.EventAggregator = eventAggregator;
            aEMR.Common.ClientLoggerHelper.InitLogger(_logger);

            CheckValidSystemMenu();
        }
        private void CheckValidSystemMenu()
        {
            if (Globals.IseHMSSystem)
            {
                bVatu = false;
                bConfigurationMgnt = false;
                bConsultations = false;
                bCLSLaboratory = false;
                bCLSImaging = false;
                bDrugDept = false;
                bMedItemDept = false;
                bAppointmentMenuItemCmd = false;
                bUserAccountCmd = false;
                bClinicManagementCmd = false;
                bStoreDeptCmd = false;
                bSystemConfigCmd = false;
            }
        }
        private bool _isBtnVisible = false;

        private string _theCurUserName;
        public string TheCurUserName
        {
            get { return _theCurUserName; }
            set
            {
                _theCurUserName = value;
                NotifyOfPropertyChange(() => TheCurUserName);
            }
        }

        private bool _isUserLoggedIn = false;
        public bool isUserLoggedIn
        {
            get { return _isUserLoggedIn; }
            set
            {
                _isUserLoggedIn = value;
                NotifyOfPropertyChange(() => isUserLoggedIn);
            }
        }

        private string _siteName;
        public string SiteName
        {
            get { return _siteName; }
            set
            {
                _siteName = value;
                NotifyOfPropertyChange(() => SiteName);
            }
        }

        public bool IsBtnVisible
        {
            get { return _isBtnVisible; }
            set
            {
                _isBtnVisible = value;
                NotifyOfPropertyChange(() => IsBtnVisible);
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

        private string _VersionText;
        public string VersionTextStr
        {
            get { return _VersionText; }
            set
            {
                _VersionText = value;
                NotifyOfPropertyChange(() => VersionTextStr);
            }
        }

        public void LogoutCmd()
        {
            IsBtnVisible = false;
            isUserLoggedIn = false;
            //Globals.LoggedUser = null;
            SiteName = string.Empty;
            _navigationService.NavigationTo<aEMR.ViewContracts.ILoginViewModel>();
            _salePosCaching.Clear();
        }
        public void AboutCmd()
        {
            _navigationService.ShowDialog<aEMR.ViewContracts.IAboutViewModel>((vm) => { }, (opts, screen) => { }, aEMR.Common.Enums.MsgBoxOptions.Ok);
        }
        public override void Initial()
        {            
            DisplayName = " aEMR - Advanced Electronic Medical Record ";
            VersionTextStr = Assembly.GetEntryAssembly().GetName().Version.ToString();
                            //+"   "+ Assembly.GetExecutingAssembly().ImageRuntimeVersion.ToString();            

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment cd =ApplicationDeployment.CurrentDeployment;
                VersionTextStr = cd.CurrentVersion.ToString();
                // show publish version in title or About box...                
            }

            Action<ILogin> devLoginAct = (loginVM) =>
            {
                loginVM.LoginName = "kienadmin";
                loginVM.ThePassword = "2010";
                loginVM.IsDevLogin = true;
                loginVM.DevAutoLogin();
            };

            _navigationService.NavigationTo<aEMR.ViewContracts.ILogin>();
            //_navigationService.NavigationTo<aEMR.ViewContracts.ILogin>(devLoginAct);

            Globals.isAccountCheck = false;

            //if ( Globals.LoggedUser == null )
            //{
            //    _navigationService.NavigationTo<aEMR.ViewContracts.ILoginViewModel>();                              
            //}
            
        }

        public void GoHomeCmd()
        {
            _navigationService.NavigationTo<aEMR.ViewContracts.IHomeViewModel>();
        }

        public void ShowBusy(string strBusyText = "")
        {
            if (strBusyText.Length == 0)
            {
                BusyContent = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0995_G1_HThongDangXuLi);
            }
            IsBusy = true;
        }

        public void HideBusy()
        {
            IsBusy = false;
        }

        public void Handle(LogOutEvent obj)
        {
            if (obj != null)
            {
                Logout();
            }
        }
        public void Logout()
        {
            var header = Globals.GetViewModel<IAppHeader>();
            header.UserName = "";

            var loginViewModel = Globals.GetViewModel<ILogin>();
            loginViewModel.LoginName = "";
            loginViewModel.ThePassword = "";
            ActiveItem = loginViewModel;
            ActivateItem(loginViewModel);
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public void Handle(BusyEvent message)
        {
            //if (message != null)
            //{
            //    BusyMessage = string.IsNullOrEmpty(message.Message) ? "Dang xu ly..." : message.Message;
            //    IsBusy = message.IsBusy;
            //}
        }

        private IDialogManager _dialogItem;
        public IDialogManager DialogItem
        {
            get { return _dialogItem; }
            set
            {

                _dialogItem = value;
                NotifyOfPropertyChange(() => DialogItem);

            }
        }

        private string _StatusText;
        public string StatusText
        {
            get { return _StatusText; }
            set
            {
                _StatusText = value;
                NotifyOfPropertyChange(() => StatusText);
            }
        }

        public void Handle(ShowDialogEvent message)
        {

            var manager = Globals.GetViewModel<IDialogManager>();

            if (message != null)
            {

                IsEnabled = false;

                DialogItem = manager;

                if (message.DialogViewModel != null)
                {
                    manager.ShowDialog(message.Title, message.DialogViewModel as IScreen, call =>
                    {
                        IsEnabled = true;

                        if (message.Callback != null)
                        {
                            message.Callback(call);
                        }

                    });
                }
                else
                {
                    manager.ShowMessageBox(message.Message, message.Title, aEMR.Infrastructure.MessageBoxOptions.Ok, call =>
                    {
                        IsEnabled = true;
                        if (message.Callback != null)
                        {
                            message.Callback(call);
                        }
                    });
                }


            }

        }

        public void Handle(ErrorNotification message)
        {
            if (message != null)
            {
                MessageBox.Show(message.Message);
            }
        }

        public void Handle(ErrorOccurred message)
        {
            if (message != null && message.CurrentError != null)
            {
                //System.Windows.Threading.Dispatcher dispatcher = Deployment.Current.Dispatcher;
                System.Windows.Threading.Dispatcher dispatcher = Application.Current.Dispatcher;
                System.Action theAct = delegate
                {                                        
                    Action<IError> onInitDlg = (errorVm) => { errorVm.SetError(message.CurrentError); };
                    GlobalsNAV.ShowDialog<IError>(onInitDlg);
                };

                dispatcher.BeginInvoke(theAct);
                
            }
        }

        public void Handle(ErrorBoldOccurred message)
        {
            if (message != null)
            {
                //System.Windows.Threading.Dispatcher dispatcher = Deployment.Current.Dispatcher;
                System.Windows.Threading.Dispatcher dispatcher = Application.Current.Dispatcher;
                System.Action theAct = delegate
                {
                    Action<IErrorBold> onInitDlg = (errorVm) => { errorVm.SetMessage(message.message, message.checkBoxContent); };
                    GlobalsNAV.ShowDialog<IErrorBold>(onInitDlg);
                };

                dispatcher.BeginInvoke(theAct);
            }
        }

        public void Handle(ValidateFailedEvent message)
        {
            if (message != null)
            {
                //System.Windows.Threading.Dispatcher dispatcher = Deployment.Current.Dispatcher;
                System.Windows.Threading.Dispatcher dispatcher = Application.Current.Dispatcher;
                System.Action theAct = delegate
                {
                    Action<IValidationError> onInitDlg = (errorVm) => { errorVm.SetErrors(message.ValidationResults); };
                    GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                };

                dispatcher.BeginInvoke(theAct);

            }
        }

        public void Handle(ShowMessageEvent message)
        {
            if (message != null)
            {
                //System.Windows.Threading.Dispatcher dispatcher = Deployment.Current.Dispatcher;
                System.Windows.Threading.Dispatcher dispatcher = Application.Current.Dispatcher;
                System.Action theAct = delegate
                {
                    Action<IMessageBox> onInitDlg = (msgVm) => 
                    {
                        msgVm.Caption = eHCMSResources.G0442_G1_TBao;
                        msgVm.MessageBoxText = message.Message;
                    };
                    GlobalsNAV.ShowDialog<IMessageBox>(onInitDlg);
                };
                dispatcher.BeginInvoke(theAct);                
            }
        }

        public void Handle(LocationSelected_New message)
        {
            Globals.ObjRefDepartment = message.RefDepartment;
            Globals.DeptLocation = message.DeptLocation;
            Globals.DeptLocation.DeptID = message.RefDepartment != null ? message.RefDepartment.DeptID : 0;
            string strDeptAndRoomName = "";

            if (Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.Staff != null)
            {
                TheCurUserName = Globals.LoggedUserAccount.Staff.FullName + "( " + Globals.LoggedUserAccount.AccountName + " )";
            }
            
            if (Globals.ObjRefDepartment != null)
            {
                strDeptAndRoomName = "[" + Globals.ObjRefDepartment.DeptName;
                if (Globals.DeptLocation != null && Globals.DeptLocation.Location != null 
                    && Globals.DeptLocation.Location.LID > 0 && Globals.DeptLocation.Location.LocationName.Length > 0)
                {
                    strDeptAndRoomName +=  " - " + Globals.DeptLocation.Location.LocationName;
                }
                strDeptAndRoomName += "]";
            }
            if (strDeptAndRoomName.Length > 0)
            {
                if (TheCurUserName.Length > 0)
                {
                    TheCurUserName += " - " + strDeptAndRoomName;
                }
                else
                {
                    TheCurUserName = strDeptAndRoomName;
                }
            }
        }

        /************************************************************
         * Xử lý in ở đây luôn
         * 
         * *********************************************************/

        readonly IDictionary<PrinterType, AxonActiveXPrinter> _printers = new Dictionary<PrinterType, AxonActiveXPrinter>();

        //public void Handle(ActiveXPrintEvt message)
        //{
        //    if (GetView() == null || message == null) return;

        //    try
        //    {
        //        //Tìm kiếm máy in nếu không có thì báo.
        //        if (!_printers.ContainsKey(message.PrinterType))
        //        {
        //            var converter = new EnumValueToStringConverter();
        //            var printerTypeName = converter.Convert(message.PrinterType, typeof(PrinterType), null, null);
        //            MessageBox.Show(string.Format(eHCMSResources.Z0966_G1_KgTimThayMayInChoLoai0, printerTypeName));
        //            return;
        //        }

        //        var printer = _printers[message.PrinterType];

        //        if (message.Data != null)
        //        {
        //            switch (message.DataType)
        //            {
        //                case ActiveXPrintType.FileName:
        //                    if (message.NumberOfCopies <= 1)
        //                    {
        //                        printer.PrintPdfFile((string)message.Data);
        //                    }
        //                    else
        //                    {
        //                        for (int i = 0; i < message.NumberOfCopies; i++)
        //                        {
        //                            printer.PrintPdfFile((string)message.Data);
        //                        }
        //                    }
        //                    break;
        //                case ActiveXPrintType.Base64String:
        //                    if (message.NumberOfCopies <= 1)
        //                    {
        //                        printer.PrintPdfDataInBase64String((string)message.Data);
        //                    }
        //                    else
        //                    {
        //                        for (int i = 0; i < message.NumberOfCopies; i++)
        //                        {
        //                            printer.PrintPdfDataInBase64String((string)message.Data);
        //                        }
        //                    }
        //                    break;
        //                default:
        //                    if (message.NumberOfCopies <= 1)
        //                    {
        //                        printer.PrintPdfData((byte[])message.Data);
        //                    }
        //                    else
        //                    {
        //                        for (int i = 0; i < message.NumberOfCopies; i++)
        //                        {
        //                            printer.PrintPdfData((byte[])message.Data);
        //                        }
        //                    }
        //                    break;
        //            }
        //            MessageBox.Show(eHCMSResources.A0466_G1_Msg_InfoDaIn);
        //        }
        //        else
        //        {
        //            MessageBox.Show(eHCMSResources.Z0965_G1_KgCoDLieuDeIn);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(string.Format("{0} ", eHCMSResources.A0690_G1_Msg_InfoKhTheIn_Loi) + ex);
        //    }
        //}

        public void Handle(ActiveXPrintEvt message)
        {
            ClientLoggerHelper.LogInfo("Begin Handle(ActiveXPrintEvt message)");
            if (GetView() == null || message == null) return;

            try
            {
                // get printer with PrinterType
                var printerName = "";
                var printerConfigManager = new PrinterConfigurationManager();
                var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
                if (allAssignedPrinterTypes.ContainsKey(message.PrinterType) && allAssignedPrinterTypes[message.PrinterType] != "")
                {
                    printerName = allAssignedPrinterTypes[message.PrinterType];
                }
                else
                {
                    if (allAssignedPrinterTypes != null)
                    {
                        try
                        {
                            foreach (var assignedPrinterType in allAssignedPrinterTypes)
                            {
                                if (!string.IsNullOrWhiteSpace(assignedPrinterType.Value))
                                {
                                    printerName = assignedPrinterType.Value;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.ToString());
                            MessageBox.Show(eHCMSResources.Z0964_G1_KgKhoiTaoDuocMayIn);
                        }
                    }
                }

                if (message.Data != null)
                {
                    try
                    {
                        ClientLoggerHelper.LogInfo("Begin Handle(ActiveXPrintEvt message) - Start stream ");
                        System.IO.MemoryStream stream = new System.IO.MemoryStream((byte[])message.Data);
                        
                        PdfDocumentProcessor documentProcessor = new PdfDocumentProcessor();
                        documentProcessor.LoadDocument(stream);
                        ClientLoggerHelper.LogInfo("Begin Handle(ActiveXPrintEvt message) - End stream ");
                        PrinterSettings printerSettings = new PrinterSettings();
                        printerSettings.PrinterName = printerName;
                        PdfPrinterSettings pdfPrinterSettings = new PdfPrinterSettings(printerSettings);
                        if (message.PaperName != null)
                        {
                            pdfPrinterSettings.Settings.DefaultPageSettings.PaperSize = printerSettings.PaperSizes.Cast<PaperSize>().FirstOrDefault(x => x.PaperName == message.PaperName);
                        }
                        // Specify the PDF printer settings.
                        //pdfPrinterSettings.PageOrientation = PdfPrintPageOrientation.Portrait;
                        //pdfPrinterSettings.PageNumbers = new int[] { 1, 3, 4, 5 };
                        ClientLoggerHelper.LogInfo("Begin Handle(ActiveXPrintEvt message) - Start Print ");
                        // Print the document using the specified printer settings.
                        documentProcessor.Print(pdfPrinterSettings);
                        ClientLoggerHelper.LogInfo("Begin Handle(ActiveXPrintEvt message) - End Print ");
                        GlobalsNAV.ShowMessagePopup(eHCMSResources.A0466_G1_Msg_InfoDaIn);
                    }
                    catch(Exception ex)
                    {
                        GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2729_G1_KhongTimThayCauHinhMayIn);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z0965_G1_KgCoDLieuDeIn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0} ", eHCMSResources.A0690_G1_Msg_InfoKhTheIn_Loi) + ex);
            }
        }

        public void Handle(AuthorizationEvent message)
        {
            var authorizationViewModel = Globals.GetViewModel<IAuthorization>();

            ActiveItem = authorizationViewModel;
            ActivateItem(authorizationViewModel);
        }

        public void Handle(isConsultationStateEditEvent message)
        {
            if (message.isConsultationStateEdit)
            {
                StatusText = "";
            }
            else
            {
                StatusText = eHCMSResources.G2453_G1_XemLSuBAnCuaBNChiDuocXemTTin;
            }
        }

        public void Handle(AppCheckAndDownloadUpdateCompletedEvent message)
        {
            //-- do async nên chưa thấy nó xảy ra
            ActiveControl();
        }

        /// <summary>
        /// Doc tat ca cac loai may in tu config.
        /// </summary>
        public void InitPrinters()
        {
            _printers.Clear();
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
            if (allAssignedPrinterTypes != null)
            {
                try
                {
                    foreach (var assignedPrinterType in allAssignedPrinterTypes)
                    {
                        if (!string.IsNullOrWhiteSpace(assignedPrinterType.Value))
                        {
                            //Tao 1 may in tuong ung voi loai nay.

                            var printer = new AxonActiveXPrinter(assignedPrinterType.Value);
                            //printer.InitPrintServer();
                            _printers.Add(assignedPrinterType.Key, printer);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.ToString());
                    MessageBox.Show(eHCMSResources.Z0964_G1_KgKhoiTaoDuocMayIn);
                }
            }
        }

        public void ActiveControl()
        {
            var loginViewModel = Globals.GetViewModel<ILogin>();
            ActiveItem = loginViewModel;
            ActivateItem(loginViewModel);
            //((ILogin)ContentItem).SetActive();
        }

        private ObservableCollection<PanelItem> _groupMainMenuItems;
        public ObservableCollection<PanelItem> GroupMainMenuItems
        {
            get { return _groupMainMenuItems; }
            set
            {
                _groupMainMenuItems = value;
                NotifyOfPropertyChange(() => GroupMainMenuItems);
            }
        }

        private bool _mainMenuGridVisible = false;
        public bool MainMenuGridVisible
        {
            get { return _mainMenuGridVisible; }
            set
            {
                _mainMenuGridVisible = value;
                NotifyOfPropertyChange(() => MainMenuGridVisible);
            }
        }

        private Button ButtonOpenMenu = null;
        private Button ButtonCloseMenu = null;

        public void ButtonCloseMenu_Load(object source)
        {
            ButtonCloseMenu = (Button)source;
        }
        public void ButtonOpenMenu_Load(object source)
        {
            ButtonOpenMenu = (Button)source;
        }

        private int _doMoveHomeMenuMode = 0;
        public int DoMoveHomeMenuMode
        {
            get { return _doMoveHomeMenuMode;  }
            set
            {
                _doMoveHomeMenuMode = value;
                NotifyOfPropertyChange(() => DoMoveHomeMenuMode);
            }
        }

        private int _doOpenMainMenuMode = 0;
        public int DoOpenMainMenuMode
        {
            get { return _doOpenMainMenuMode; }
            set
            {
                _doOpenMainMenuMode = value;
                NotifyOfPropertyChange(() => DoOpenMainMenuMode);
            }
        }

        public void RegistrationCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eRegisMenu);           
        }
        public void ConsultationCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eConsultMenu);
        }

        public void AppointmentCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eApptMenu);
        }

        public void LaboratoryCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eLabPCLMenu);
        }
        public void PCLDepartmentCmd_Click(object sender)
        {
            CloseTheMainMenu();
            //▼====== #001
            //ClearDataForChangeModule();
            //▲====== #001
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eImagePCLMenu);
        }

        public void PharmacyCmd_Clicked(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.ePharmMenu);
        }
        public void DrugDeptCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eDrugDeptMenu);
        }
        public void MedItemDeptCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eMedItemDeptMenu);
        }
        public void StoreDeptCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eStoreMenu);
        }

        public void TransactionCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eTranRepMenu);
        }

        public void ConfigurationMgnt_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eConfigManMenu);
        }

        public void GeneralEnquireCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eGenEnqMenu);

        }

        public void GotoVatTu_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eResourceManMenu);
        }

        public void ClinicManagementCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eClinicDeptMenu);

        }
        public void UserAccountCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eUserAccMenu);
        }

        public void SystemConfigurationCmd_Click(object sender)
        {
            CloseTheMainMenu();
            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.MainMenuItem_Click(sender, MainMenuItemType.eSystemConfigMenu);
        }


        public void ButtonOpenMenu_Click(object sender)
        {
            OpenTheMainMenu();
        }

        private void OpenTheMainMenu()
        {
            if (!isUserLoggedIn)
                return;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
            DoMoveHomeMenuMode = 1;
            MainMenuGridVisible = true;
            DoOpenMainMenuMode = 1;

            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.HomeMenuPanelBusy = true;
        }

        private void CloseTheMainMenu()
        {
            if (!isUserLoggedIn)
                return;
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            DoMoveHomeMenuMode = 2;
            DoOpenMainMenuMode = 2;
            MainMenuGridVisible = false;

            IHome homeVM = Globals.GetViewModel<IHome>();
            homeVM.HomeMenuPanelBusy = false;
        }
        public void ButtonCloseMenu_Click(object sender)
        {
            CloseTheMainMenu();
        }
        private string _Message;
        public string Message
        {
            get => _Message; set
            {
                _Message = value;
                NotifyOfPropertyChange(() => Message);
                if (!string.IsNullOrEmpty(Message) && MessagePopup != null)
                {
                    MessagePopup.Visibility = Visibility.Hidden;
                    MessagePopup.Visibility = Visibility.Visible;
                }
            }
        }
        Border MessagePopup;
        public void MessagePopup_Loaded(object source)
        {
            MessagePopup = (Border)source;
        }

        public void ProfileCmd()
        {
            Action<ISelectLocation> onInitDlg = delegate (ISelectLocation locationVm)
            {
                locationVm.mCancel = true;
            };

            GlobalsNAV.ShowDialog<ISelectLocation>(onInitDlg);
        }

        //▼====== #001: Cần phải test kĩ mặc dù chính xác là ra khỏi Module là phải clear dữ liệu đi, nhưng vì lq đến Globals nên không biết có phát sinh ảnh hưởng gì không.
        //private void ClearDataForChangeModule()
        //{
        //    //Globals.PatientAllDetails.PatientInfo = null;
        //    //Globals.PatientAllDetails.PtRegistrationDetailInfo = null;
        //    //Globals.PatientAllDetails.PtRegistrationInfo = null;
        //    //Globals.PatientAllDetails.curDiagnosisTreatmentByPtDetailID = null;
        //    //Globals.PatientAllDetails.curPrecriptionsByPtDetailID = null;
        //    //Globals.PatientAllDetails.allPrescriptionIssueHistory = null;
        //}
        //▲====== #001
        #region Properties
        private bool _bVatu = true;
        private bool _bConfigurationMgnt = true;
        private bool _bConsultations = true;
        private bool _bCLSLaboratory = true;
        private bool _bCLSImaging = true;
        private bool _bPharmacies = true;
        private bool _bRegisterMenuItemCmd = true;
        private bool _bDrugDept = true;
        private bool _bMedItemDept = true;
        private bool _bAppointmentMenuItemCmd = true;
        private bool _bTransactionCmd = true;
        private bool _bUserAccountCmd = true;
        private bool _bClinicManagementCmd = true;
        private bool _bStoreDeptCmd = true;
        private bool _bSystemConfigCmd = true;
        public bool bVatu
        {
            get
            {
                return _bVatu;
            }
            set
            {
                if (_bVatu == value)
                    return;
                _bVatu = value;
                NotifyOfPropertyChange(() => bVatu);
            }
        }
        public bool bConfigurationMgnt
        {
            get
            {
                return _bConfigurationMgnt;
            }
            set
            {
                if (_bConfigurationMgnt == value)
                    return;
                _bConfigurationMgnt = value;
                NotifyOfPropertyChange(() => bConfigurationMgnt);
            }
        }
        public bool bConsultations
        {
            get
            {
                return _bConsultations;
            }
            set
            {
                if (_bConsultations == value)
                    return;
                _bConsultations = value;
                NotifyOfPropertyChange(() => bConsultations);
            }
        }
        public bool bCLSLaboratory
        {
            get
            {
                return _bCLSLaboratory;
            }
            set
            {
                if (_bCLSLaboratory == value)
                    return;
                _bCLSLaboratory = value;
                NotifyOfPropertyChange(() => bCLSLaboratory);
            }
        }
        public bool bCLSImaging
        {
            get
            {
                return _bCLSImaging;
            }
            set
            {
                if (_bCLSImaging == value)
                    return;
                _bCLSImaging = value;
            }
        }
        public bool bPharmacies
        {
            get
            {
                return _bPharmacies;
            }
            set
            {
                if (_bPharmacies == value)
                    return;
                _bPharmacies = value;
                NotifyOfPropertyChange(() => bPharmacies);
            }
        }
        public bool bRegisterMenuItemCmd
        {
            get
            {
                return _bRegisterMenuItemCmd;
            }
            set
            {
                if (_bRegisterMenuItemCmd == value)
                    return;
                _bRegisterMenuItemCmd = value;
                NotifyOfPropertyChange(() => bRegisterMenuItemCmd);
            }
        }
        public bool bDrugDept
        {
            get
            {
                return _bDrugDept;
            }
            set
            {
                if (_bDrugDept == value)
                {
                    return;
                }
                _bDrugDept = value;
                NotifyOfPropertyChange(() => bDrugDept);
            }
        }
        public bool bMedItemDept
        {
            get
            {
                return _bMedItemDept && Globals.ServerConfigSection != null && Globals.ServerConfigSection.MedDeptElements.UseDrugDeptAs2DistinctParts;
            }
            set
            {
                if (_bMedItemDept == value)
                {
                    return;
                }
                _bMedItemDept = value;
                NotifyOfPropertyChange(() => bMedItemDept);
            }
        }
        public bool bAppointmentMenuItemCmd
        {
            get
            {
                return _bAppointmentMenuItemCmd;
            }
            set
            {
                if (_bAppointmentMenuItemCmd == value)
                    return;
                _bAppointmentMenuItemCmd = value;
                NotifyOfPropertyChange(() => bAppointmentMenuItemCmd);
            }
        }
        public bool bTransactionCmd
        {
            get
            {
                return _bTransactionCmd;
            }
            set
            {
                if (_bTransactionCmd == value)
                    return;
                _bTransactionCmd = value;
                NotifyOfPropertyChange(() => bTransactionCmd);
            }
        }
        public bool bUserAccountCmd
        {
            get
            {
                return _bUserAccountCmd;
            }
            set
            {
                if (_bUserAccountCmd == value)
                    return;
                _bUserAccountCmd = value;
                NotifyOfPropertyChange(() => bUserAccountCmd);
            }
        }
        public bool bClinicManagementCmd
        {
            get
            {
                return _bClinicManagementCmd;
            }
            set
            {
                if (_bClinicManagementCmd == value)
                    return;
                _bClinicManagementCmd = value;
                NotifyOfPropertyChange(() => bClinicManagementCmd);
            }
        }
        public bool bStoreDeptCmd
        {
            get
            {
                return _bStoreDeptCmd;
            }
            set
            {
                if (_bStoreDeptCmd == value)
                    return;
                _bStoreDeptCmd = value;
                NotifyOfPropertyChange(() => bStoreDeptCmd);
            }
        }
        public bool bSystemConfigCmd
        {
            get
            {
                return _bSystemConfigCmd;
            }
            set
            {
                if (_bSystemConfigCmd == value)
                    return;
                _bSystemConfigCmd = value;
                NotifyOfPropertyChange(() => bSystemConfigCmd);
            }
        }
        #endregion
        #region Methods
        public void NotifyChanged()
        {
            NotifyOfPropertyChange(() => bMedItemDept);
        }
        #endregion

        private bool _IsWarningTestVisible;
        public bool IsWarningTestVisible
        {
            get { return _IsWarningTestVisible; }
            set
            {
                _IsWarningTestVisible = value;
                NotifyOfPropertyChange(() => IsWarningTestVisible);
            }
        }
        public void Handle(ShowWaringSegments message)
        {
            IsWarningTestVisible = true;
        }

    }
}