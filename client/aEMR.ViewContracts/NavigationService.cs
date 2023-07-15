using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using aEMR.Common.BaseModel;
using aEMR.Common.Enums;
using aEMR.Infrastructure;
using eHCMSLanguage;
using DataEntities;
using aEMR.Common.Converters;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using aEMR.Common;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Web.Script.Serialization;
using System.Threading;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using System.Collections.ObjectModel;

/*
* 20200728 #001 TTM:   BM 0039421: [Báo cáo BHYT] Bổ sung try catch vào hàm Get Request Stream.
* 20210118 #002 TNHX:  Thêm func gọi API truyền kết quả qua PAC
* 20210717 #003 TNHX:  Thêm func gọi API lấy STT của BN từ QMS
* 20210929 #004 TNHX:  681 Trừ đi tiền Nhà nước chi trả đối với BN điều trị COVID
* 20220103 #005 TNHX:  848 Tính lại tiền bn điều trị COVID dựa vào tích tính covid
* 20220530 #006 BLQ: Kiểm tra thời gian thao tác của bác sĩ
* 20220811 #007 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
* + Thêm ShowDialog_V5 sử dụng Dictionary để truyền thêm title cho Dialog
* 20221026 #008 QTD:   Thêm đánh dấu ưu tiên cho QMS
* 20230404 #009 QTD:   Thêm cấu hình cấp số khoa khám, tắt cấu hình cũ
* 20230619 #010 QTD:   Thêm hàm gọi API tạo dữ liệu SMS khi lưu kết quả XN
*/
namespace aEMR.ViewContracts
{
    [Export(typeof(INavigationService))]
    public class NavigationService : INavigationService
    {
       
        /// <summary>
        /// Private instance of window manager
        /// </summary>
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private IScreen _currentWindow;

        private Stack<Type> _stackScreens;
      
        [ImportingConstructor]
        public NavigationService(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _stackScreens = new Stack<Type>();
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
        }

        public IConductor ParentScreen
        {
            get
            {
                return (IoC.Get<IShellViewModel>() as IConductor); 
            }
        }

        public IScreen DefaultScreen { get; set; }

        public IScreen CurrentScreen { get; set; }

        public void NavigationTo(IScreen screen)
        {
            if ( screen != null)
            {
                _stackScreens.Push(screen.GetType());
                this.ParentScreen.ActivateItem(screen);    
            }
            
        }

        public void NavigationTo<T>() where T : class 
        {
            var screen = IoC.Get<T>();
            if ( screen is IScreen )
            {
                _stackScreens.Push(screen.GetType());
                this.ParentScreen.ActivateItem(screen);    
            }
            
        }

        public void NavigationTo(Type type)
        {
            var screen = IoC.GetInstance(type, null) ;
            if (screen is IScreen)
            {
                _stackScreens.Push(screen.GetType());
                ParentScreen.ActivateItem(screen);
            }

        }

        public void NavigationTo(IScreen screen, Action<IScreen> initAction)
        {
            NavigationTo(screen);
            initAction.Invoke(screen);
        }

        public void NavigationTo<T>(Action<T> initAction) where T : class
        {
            var screen = IoC.Get<T>();
            if (screen is IScreen)
            {
                _stackScreens.Push(screen.GetType());
                this.ParentScreen.ActivateItem(screen);
                initAction.Invoke(screen);
            }
            
        }

        public void NavigationToWithBeforeAction<T>(Action<T> beforeAction) where T : class
        {
            var screen = IoC.Get<T>();
            if (screen is IScreen)
            {
                _stackScreens.Push(screen.GetType());
                beforeAction.Invoke(screen);
                this.ParentScreen.ActivateItem(screen);                
            }
        }

        public void NavigationTo(Type type, Action<IScreen> initAction)
        {
            var screen = IoC.GetInstance(type, null);
            if (screen is IScreen)
            {
                _stackScreens.Push(screen.GetType());
                ParentScreen.ActivateItem(screen);
                initAction.Invoke(screen as IScreen);
            }
        }

        public void ShowNotification(string message)
        {
            
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        private Dictionary<string, object> BuildDialogSettings( Dictionary<string, object> paraSettings = null  )
        {
            var settings = new Dictionary<string, object>();
            //settings["WindowStyle"] = WindowStyle.None;
            settings["ShowInTaskbar"] = false;
            settings["WindowStartupLocation"] = WindowStartupLocation.CenterScreen;
            settings["StaysOpen"] = false;
            settings["Width"] = 600;
            settings["Height"] = 450;

            if ( paraSettings != null )
            {
                foreach (var setting in paraSettings)
                {
                    settings[setting.Key] = setting.Value;
                }    
            }

            return settings;
        }

        private Dictionary<string, object> BuildDialogSettingsV2(Dictionary<string, object> paraSettings = null, bool ManualSize = false)
        {
            var settings = new Dictionary<string, object>();
            settings.Add("ShowInTaskbar", false);
            settings.Add("Title", "");
            if (ManualSize)
            {
                settings.Add("SizeToContent", SizeToContent.Manual);
                if (!settings.ContainsKey("Width") && settings.ContainsKey("Height"))
                    settings.Add("WindowState", WindowState.Maximized);
            }
            if (paraSettings != null)
            {
                foreach (var setting in paraSettings)
                {
                    settings[setting.Key] = setting.Value;
                }
            }
            return settings;
        }

        private IDialogContentViewModel BuildDialogContentViewModel()
        {
            var dialogContent = IoC.Get<IDialogContentViewModel>();
            _currentWindow = dialogContent as IScreen;
            return dialogContent;
        }

        public void ShowPopup<T>(Action<T> onInitialize = null, Action<MsgBoxOptions, IScreen> onClose = null)
        {
            var dialogContent = BuildDialogContentViewModel();

            
            Execute.OnUIThread(() =>
            {
                _windowManager.ShowWindow(dialogContent, null, BuildDialogSettings());
                dialogContent.DisplayContent(onInitialize, null, MsgBoxOptions.Ok, onClose);
            });
        }

        public void ShowPopup(string message, Action<MsgBoxOptions> onClose = null)
        {
            
        }


        public void ShowPopup(string message, Action<MsgBoxOptions, IScreen> onClose = null)
        {
            var dialogContent = IoC.Get<IDialogContentViewModel>();
            _currentWindow = dialogContent as IScreen;
            dialogContent.Message = message;
            Execute.OnUIThread(() => _windowManager.ShowPopup(dialogContent, null, BuildDialogSettings()));
        }

        public T ShowDialog_V2<T>(Action<T> onInitialize, Action<MsgBoxOptions, IScreen> onClose
            , MsgBoxOptions msgBoxOptions
            , Dictionary<string, object> settings)
        {
            var dialogContent = BuildDialogContentViewModel();
            dialogContent.Option = msgBoxOptions;
            var dlgInstance = dialogContent.DisplayContent_V2<T>(onInitialize, null, msgBoxOptions, onClose);
            var defaultSettings = BuildDialogSettings(settings);
            _windowManager.ShowDialog(dialogContent, null, defaultSettings);
            return dlgInstance;
        }

        public T ShowDialogNew_V2<T>(Action<T> onInitialize, Action<MsgBoxOptions, IScreen> onClose
            , MsgBoxOptions msgBoxOptions
            , Dictionary<string, object> settings)
        {
            var dlgViewModel = IoC.Get<T>();
            if (onInitialize != null)
            {
                onInitialize.Invoke(dlgViewModel);
            }
            _windowManager.ShowDialog(dlgViewModel, null, null);
            return dlgViewModel;
        }

        public void ShowDialogNew<T>(Action<T> onInitialize, Action<MsgBoxOptions, IScreen> onClose
            , MsgBoxOptions msgBoxOptions
            , Dictionary<string, object> settings
            , bool ManualSize = false)
        {
            var dlgViewModel = IoC.Get<T>();
            if (onInitialize != null)
            {
                onInitialize.Invoke(dlgViewModel);               
            }
            Dictionary<string, object> defaultSettings = BuildDialogSettingsV2(settings, ManualSize);
            _windowManager.ShowDialog(dlgViewModel, null, defaultSettings);
            if (onClose != null && dlgViewModel is IScreen)
            {
                onClose.Invoke(MsgBoxOptions.Ok, dlgViewModel as IScreen);
            }
        }

        public T ShowDialogNew_V3<T>(T dlgViewModel, Action<T> onInitialize, Action<MsgBoxOptions, IScreen> onClose
            , MsgBoxOptions msgBoxOptions
            , Dictionary<string, object> settings
            , bool ManualSize = false
            , Size? SceenSize = null)
        {
            if (dlgViewModel is ViewModelBase)
            {
                (dlgViewModel as ViewModelBase).IsDialogView = true;
            }
            if (onInitialize != null)
            {
                onInitialize.Invoke(dlgViewModel);
            }
            Dictionary<string, object> defaultSettings = BuildDialogSettingsV2(settings, ManualSize);
            if (SceenSize != null && SceenSize.HasValue && SceenSize.Value.Width > 0 && SceenSize.Value.Width > 0)
            {
                defaultSettings.Add("Width", SceenSize.Value.Width);
                defaultSettings.Add("Height", SceenSize.Value.Height);
            }
            _windowManager.ShowDialog(dlgViewModel, null, defaultSettings);
            return dlgViewModel;
        }

        public void ShowDialogNew_V4<T>(T dlgViewModel, Action<T> onInitialize, Action<MsgBoxOptions, IScreen> onClose
            , MsgBoxOptions msgBoxOptions
            , Dictionary<string, object> settings
            , bool ManualSize = false)
        {
            if (onInitialize != null)
            {
                onInitialize.Invoke(dlgViewModel);
            }
            Dictionary<string, object> defaultSettings = BuildDialogSettingsV2(settings, ManualSize);
            Application.Current.Dispatcher.Invoke(() =>
            {
                _windowManager.ShowDialog(dlgViewModel, null, defaultSettings);
            });
        }
        

        public void ShowDialog<T>(Action<T> onInitialize, Action<MsgBoxOptions, IScreen> onClose
            , MsgBoxOptions msgBoxOptions
            , Dictionary<string, object> settings
            )
        {
            var dialogContent = BuildDialogContentViewModel();
            dialogContent.Option = msgBoxOptions;
            dialogContent.DisplayContent_V2<T>(onInitialize, null, msgBoxOptions, onClose);
            var defaultSettings = BuildDialogSettings(settings);
            _windowManager.ShowDialog(dialogContent, null, defaultSettings);            
        }

        public void ShowDialog(string message, Action<MsgBoxOptions, IScreen> onClose = null)
        {
            var dialogContent = IoC.Get<IDialogContentViewModel>();
            _currentWindow = dialogContent as IScreen;
            dialogContent.Message = message;
            dialogContent.CallbackResult = onClose;

            _windowManager.ShowWindow(dialogContent, null, BuildDialogSettings());
        }

        public void ShowDialog(string message, Action<MsgBoxOptions, IScreen> onClose
            , Dictionary<string,object> settings )
        {
            var dialogContent = IoC.Get<IDialogContentViewModel>();
            _currentWindow = dialogContent as IScreen;
            dialogContent.Message = message;
            dialogContent.CallbackResult = onClose;

            _windowManager.ShowWindow(dialogContent, null, BuildDialogSettings(settings));
        }

        public void ShowDialog(string message, Action<MsgBoxOptions, IScreen> onClose
            , MsgBoxOptions msgBoxOptions, Dictionary<string, object> settings)
        {
            var dialogContent = IoC.Get<IDialogContentViewModel>();
            _currentWindow = dialogContent as IScreen;
            dialogContent.Message = message;
            dialogContent.Option = msgBoxOptions;
            dialogContent.CallbackResult = onClose;

            _windowManager.ShowDialog(dialogContent, null, BuildDialogSettings(settings));
            
        }

        public void ShowDialog(string message, Action<MsgBoxOptions, IScreen> onClose = null
            , MsgBoxOptions msgBoxOptions = MsgBoxOptions.Ok
            )
        {
            var dialogContent = IoC.Get<IDialogContentViewModel>();
            _currentWindow = dialogContent as IScreen;
            dialogContent.Message = message;
            dialogContent.Option = msgBoxOptions;
            dialogContent.CallbackResult = onClose;

            _windowManager.ShowDialog(dialogContent, null, BuildDialogSettings());

        }

        public void CloseCurrentWindow()
        {
            if ( _currentWindow != null)
            {
                _currentWindow.TryClose();
                _currentWindow = null;
            }
        }

        public void CloseCurrentWindow( MsgBoxOptions options )
        {
            if (_currentWindow is IDialogContentViewModel)
            {
                (_currentWindow as IDialogContentViewModel).Select( options );
                if ( _currentWindow != null)
                {
                    CloseCurrentWindow();
                }
                
            }
        }

        public void ShowPopup(IScreen screen)
        {
            _windowManager.ShowWindow(screen);
        }

        public void ShowWindow(IScreen screen)
        {
            Execute.OnUIThread(() => _windowManager.ShowWindow(screen)); 
        }

        public MessageBoxResult DisplayMessageBox(string message)
        {
            return MessageBox.Show(message);
        }

        public void GoBack()
        {
            var currentScreen = _stackScreens.Pop();
            NavigationTo(currentScreen);
        }


        //public void DlgShowBusyIndicator(string busyContent)
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        int threadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        //        var curHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        //        var curWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        //        var settings = new Dictionary<string, object>();

        //        settings["ShowInTaskbar"] = false;
        //        settings["WindowStartupLocation"] = WindowStartupLocation.CenterScreen;
        //        //settings["WindowState"] = System.Windows.WindowState.Maximized;
        //        settings["Width"] = curWidth;
        //        settings["Height"] = curHeight;

        //        var busyVM = IoC.Get<IBusyIndicatorPopupView>();
        //        busyVM.strBusyMsg = busyContent;
        //        busyVM.IsPopupBusy = true;

        //        _windowManager.ShowWindow(busyVM, null, settings);
        //    });
        //}


        //public void DlgHideBusyIndicator()
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {
        //        _eventAggregator.Publish(new HideBusyIndicatorEvent());
        //    });
        //}

        public void DlgShowBusyIndicator(ViewModelBase baseVM, string busyContent)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                baseVM.DlgBusyContent = busyContent;
                baseVM.DlgIsBusyFlag = true;
            });
        }

        public void DlgHideBusyIndicator(ViewModelBase baseVM)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                baseVM.DlgIsBusyFlag = false;
            });
        }


        public void ShowBusy(string busyContent)
        {
            if ( _currentWindow != null && _currentWindow is IDialogContentViewModel )
            {
                (_currentWindow as IDialogContentViewModel).BusyContent = busyContent;
                (_currentWindow as IDialogContentViewModel).IsBusy = true;
            }
            else
            {
                var shellviewModel = IoC.Get<IShellViewModel>();
                shellviewModel.BusyContent = busyContent;
                shellviewModel.IsBusy = true;                    
            }
        }

        public void ShowMessagePopup(string Message)
        {
            var shellviewModel = IoC.Get<IShellViewModel>();
            if (shellviewModel != null)
            {
                shellviewModel.Message = Message;
            }
        }

        public void HideBusy()
        {
            if (_currentWindow != null && _currentWindow is IDialogContentViewModel)
            {
                (_currentWindow as IDialogContentViewModel).IsBusy = false;
            }
            else
            {
                var shellviewModel = IoC.Get<IShellViewModel>();
                shellviewModel.IsBusy = false;
            }
        }
    }

    public static class GlobalsNAV
    {
        // Nhiệt độ
        public static readonly float WARNING_TEMPERATURE_TOP = 39;
        public static readonly float WARNING_TEMPERATURE_BOTTOM = 36;
        public static readonly float BLOCK_TEMPERATURE_TOP = 45;
        public static readonly float BLOCK_TEMPERATURE_BOTTOM = 30;
        // SPO2
        public static readonly float WARNING_SPO2_TOP = 100;
        public static readonly float WARNING_SPO2_BOTTOM = 92;
        public static readonly float BLOCK_SPO2_TOP = 100;
        public static readonly float BLOCK_SPO2_BOTTOM = 0;
        // Cân nặng
        public static readonly float WARNING_WEIGHT_TOP = 120;
        public static readonly float WARNING_WEIGHT_BOTTOM = 1;
        public static readonly float BLOCK_WEIGHT_TOP = 250;
        public static readonly float BLOCK_WEIGHT_BOTTOM = 0;
        // Mạch
        public static readonly float WARNING_PULSE_TOP = 120;
        public static readonly float WARNING_PULSE_BOTTOM = 50;
        public static readonly float BLOCK_PULSE_TOP = 250;
        public static readonly float BLOCK_PULSE_BOTTOM = 0;
        // Huyết áp
        public static readonly float WARNING_UPPER_PRESSURE_TOP = 200;
        public static readonly float WARNING_UPPER_PRESSURE_BOTTOM = 90;
        public static readonly float WARNING_LOWER_PRESSURE_TOP = 120;
        public static readonly float WARNING_LOWER_PRESSURE_BOTTOM = 30;
        public static readonly float BLOCK_UPPER_PRESSURE_TOP = 300;
        public static readonly float BLOCK_LOWER_PRESSURE_TOP = 200;
        public static readonly float BLOCK_PRESSURE_BOTTOM = 0;
        //Nhịp thở
        public static readonly float BLOCK_RESPIRATORY_RATE_BOTTOM = 0;
        public static readonly float BLOCK_RESPIRATORY_RATE_TOP = 100;
        public static readonly float WARNING_RESPIRATORY_RATE_BOTTOM = 10;
        public static readonly float WARNING_RESPIRATORY_RATE_TOP = 30;
        
        public static T GetViewModel<T>()
        {
            return IoC.Get<T>();
        }
        private static IEnumerator<IResult> LoadDynamicXap<T>(string uri, string message)
        {
            yield return new ShowScreen(typeof(T));
        }

        public static void LoadDynamicModule<T>(string xapUri, string busymessage = null, Action<object> callback = null)
        {
            Coroutine.BeginExecute(LoadDynamicXap<T>(xapUri, busymessage), null, (o, e) =>
            {
                if (callback != null)
                {
                    callback(e);
                }
            });
        }
        // TxD 02/06/2018: The following ShowDialog methods has been moved from Infrastucture Globals and rewritten to work with the Navigation Service and new version of Caliburn Micro's ShowDialog
        //                  All Existing codes that were using the old method of ShowDialog above (commented out) will have to be modified accordingly
        public static void ShowDialog<VM_Interface_Type>(Action<VM_Interface_Type> onInitDialog = null, Action<MsgBoxOptions, IScreen> onCloseCallback = null, bool bHasCloseBtn = false, bool ManualSize = false, Size? SceenSize = null)
        {
            INavigationService _navService = IoC.Get<INavigationService>();
            //_navService.ShowDialog<VM_Interface_Type>(onInitDialog, onCloseCallback, MsgBoxOptions.None, settings);
            Dictionary<string, object> settings = null;
            if (SceenSize != null && SceenSize.HasValue && SceenSize.Value.Width > 0 && SceenSize.Value.Width > 0)
            {
                ManualSize = true;
                if (settings == null) settings = new Dictionary<string, object>();
                settings.Add("Width", SceenSize.Value.Width);
                settings.Add("Height", SceenSize.Value.Height);
            }
            _navService.ShowDialogNew<VM_Interface_Type>(onInitDialog, onCloseCallback, MsgBoxOptions.None, settings, ManualSize);
        }
        public static VM_Interface_Type ShowDialog_V2<VM_Interface_Type>(bool bHasCloseBtn = false, Action<MsgBoxOptions, IScreen> onCloseCallback = null, Action<VM_Interface_Type> onInitDialog = null)
        {
            INavigationService _navService = IoC.Get<INavigationService>();
            //return _navService.ShowDialog_V2<VM_Interface_Type>(onInitDialog, onCloseCallback, MsgBoxOptions.None);
            return _navService.ShowDialogNew_V2<VM_Interface_Type>(onInitDialog, onCloseCallback, MsgBoxOptions.None);
        }
        public static void ShowDialog_V3<VM_Interface_Type>(VM_Interface_Type dlgViewModel, Action<VM_Interface_Type> onInitDialog = null, Action<MsgBoxOptions, IScreen> onCloseCallback = null, bool bHasCloseBtn = false, bool ManualSize = false, Size? SceenSize = null)
        {
            INavigationService _navService = IoC.Get<INavigationService>();
            _navService.ShowDialogNew_V3<VM_Interface_Type>(dlgViewModel, onInitDialog, onCloseCallback, MsgBoxOptions.None, null, ManualSize, SceenSize);
        }

        public static void ShowDialog_V4<VM_Interface_Type>(VM_Interface_Type dlgViewModel, Action<VM_Interface_Type> onInitDialog = null, Action<MsgBoxOptions, IScreen> onCloseCallback = null, bool bHasCloseBtn = false, bool ManualSize = false)
        {
            INavigationService _navService = IoC.Get<INavigationService>();
            _navService.ShowDialogNew_V4<VM_Interface_Type>(dlgViewModel, onInitDialog, onCloseCallback, MsgBoxOptions.None, null, ManualSize);
        }

        //▼==== #007
        public static void ShowDialog_V5<VM_Interface_Type>(VM_Interface_Type dlgViewModel, Action<VM_Interface_Type> onInitDialog = null, Action<MsgBoxOptions, IScreen> onCloseCallback = null, Dictionary<string, object> settings = null, bool ManualSize = false, Size? SceenSize = null)
        {
            INavigationService _navService = IoC.Get<INavigationService>();
            _navService.ShowDialogNew_V3<VM_Interface_Type>(dlgViewModel, onInitDialog, onCloseCallback, MsgBoxOptions.None, settings, ManualSize, SceenSize);
        }
        //▲==== #007

        //private static int _nDlgBusyIndicatorCnt = 0;

        public static void DlgShowBusyIndicator<GEN_VM>(this GEN_VM genVM, string strBusyText = "")
        {
            var mView = genVM as ViewModelBase;
            System.Threading.Interlocked.Increment(ref mView._nDlgBusyIndicatorCnt);
            if (strBusyText == "")
            {
                strBusyText = eHCMSResources.K2887_G1_DangXuLy;
            }
            if (mView._nDlgBusyIndicatorCnt == 1)
            {
                INavigationService _navService = IoC.Get<INavigationService>();
                _navService.DlgShowBusyIndicator(genVM as ViewModelBase, strBusyText);
            }
        }

        public static void DlgHideBusyIndicator<GEN_VM>(this GEN_VM genVM)
        {
            var mView = genVM as ViewModelBase;
            System.Threading.Interlocked.Decrement(ref mView._nDlgBusyIndicatorCnt);
            if (mView._nDlgBusyIndicatorCnt <= 0)
            {
                mView._nDlgBusyIndicatorCnt = 0;
                INavigationService _navService = IoC.Get<INavigationService>();
                _navService.DlgHideBusyIndicator(genVM as ViewModelBase);
            }
        }

        private static int _nBusyIndicatorCnt = 0;
        public static void ShowBusyIndicator<GEN_VM>(this GEN_VM genVM, string strBusyText = "")
        {
            System.Threading.Interlocked.Increment(ref _nBusyIndicatorCnt);
            if (strBusyText == "")
            {
                strBusyText = eHCMSResources.K2887_G1_DangXuLy;
            }
            if (_nBusyIndicatorCnt == 1)
            {
                INavigationService _navService = IoC.Get<INavigationService>();
                _navService.ShowBusy(strBusyText);
            }
        }
        public static void ShowMessagePopup(string Message)
        {
            INavigationService _navService = IoC.Get<INavigationService>();
            _navService.ShowMessagePopup(Message);
        }
        public static void HideBusyIndicator<GEN_VM>(this GEN_VM genVM)
        {
            System.Threading.Interlocked.Decrement(ref _nBusyIndicatorCnt);
            INavigationService _navService = IoC.Get<INavigationService>();
            if (_nBusyIndicatorCnt <= 0)
            {
                _nBusyIndicatorCnt = 0;
                _navService.HideBusy();
            }
        }

        private static string _TitleForm;
        public static string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
            }
        }

        private static string _HIRegistrationForm;
        public static string HIRegistrationForm
        {
            get { return _HIRegistrationForm; }
            set
            {
                _HIRegistrationForm = value;
            }
        }

        public static MessageBoxTask msgb = null;
        public static IEnumerator<IResult> DoMessageBox()
        {
            msgb = new MessageBoxTask(string.Format(eHCMSResources.Z0465_G1_BanCoMuonQuaTrangHayKg, TitleForm), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
            yield return msgb;
            yield break;
        }
        public static IEnumerator<IResult> DoMessageBoxHIRegis()
        {
            //msgb = new MessageBoxTask(HIRegistrationForm, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
            //20180717 TBL: Globals.HIRegistrationForm de show thong bao
            msgb = new MessageBoxTask(Globals.HIRegistrationForm, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
            yield return msgb;
            yield break;
        }
        public static void CalcInvoiceItem(MedRegItemBase aRegItem, bool IsHighTechServiceBill, PatientRegistration aCurrentRegistration)
        {
            //▼====: #004
            bool IsCountPatientCOVID = false;
            bool IsRegItemCOVID = false;
            if (aCurrentRegistration.AdmissionInfo != null)
            {
                IsCountPatientCOVID = aCurrentRegistration.AdmissionInfo.IsTreatmentCOVID;
            }
            //▲====: #004
            //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng rồi mới tính tổng(02/08/2014 18:24).
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;
            bool isHighTechServiceDetail = false;
            if (aRegItem is PatientRegistrationDetail)
            {
                var detailsItem = aRegItem as PatientRegistrationDetail;
                if (detailsItem.RefMedicalServiceItem != null && detailsItem.RefMedicalServiceItem.RefMedicalServiceType != null
                    && detailsItem.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KYTHUATCAO)
                {
                    isHighTechServiceDetail = true;
                }
                //▼====: #004
                if (detailsItem.RefMedicalServiceItem.InCategoryCOVID)
                {
                    IsRegItemCOVID = true;
                }
                //▲====: #004
            }

            //▼====: #004
            if (aRegItem is PatientPCLRequestDetail)
            {
                var detailsItem = aRegItem as PatientPCLRequestDetail;
                if (detailsItem.PCLExamType.InCategoryCOVID)
                {
                    IsRegItemCOVID = true;
                }
            }
            //▲====: #004
            //double? hiBenefit = PatientSummaryInfoContent.HiBenefit;
            // TxD 02/03/2018 HiBenefit has been removed from interface of PatientSummaryInfoContent so use CurRegistration.PtInsuranceBenefit
            //item.HIBenefit = PatientSummaryInfoContent.HiBenefit;
            //▼====: #004
            if (IsCountPatientCOVID)
            {
                if (aRegItem.IsCountPatientCOVID)
                {
                    //aRegItem.HIBenefit = aCurrentRegistration.PtInsuranceBenefit;
                    aRegItem.InvoicePrice = (decimal)aRegItem.ChargeableItem.HIAllowedPrice;
                    aRegItem.HIAllowedPrice = aRegItem.ChargeableItem.HIAllowedPrice;
                    aRegItem.PriceDifference = aRegItem.InvoicePrice - aRegItem.HIAllowedPrice.GetValueOrDefault(0);
                    aRegItem.TotalPriceDifference = aRegItem.PriceDifference * aRegItem.Qty;
                    aRegItem.OtherAmt = aRegItem.InvoicePrice;
                    //aRegItem.IsCountPatientCOVID = true;
                }
                else
                {
                    aRegItem.HIBenefit = aCurrentRegistration.PtInsuranceBenefit;
                    aRegItem.InvoicePrice = aRegItem.HIBenefit.HasValue ? aRegItem.ChargeableItem.HIPatientPrice : aRegItem.ChargeableItem.NormalPrice;
                    aRegItem.HIAllowedPrice = aRegItem.ChargeableItem.HIAllowedPrice;
                    aRegItem.PriceDifference = aRegItem.InvoicePrice - aRegItem.HIAllowedPrice.GetValueOrDefault(0);
                    aRegItem.TotalPriceDifference = aRegItem.PriceDifference * aRegItem.Qty;
                    aRegItem.OtherAmt = 0;
                }
            }
            else
            {
                aRegItem.HIBenefit = aCurrentRegistration.PtInsuranceBenefit;
                aRegItem.InvoicePrice = aRegItem.HIBenefit.HasValue ? aRegItem.ChargeableItem.HIPatientPrice : aRegItem.ChargeableItem.NormalPrice;
                aRegItem.HIAllowedPrice = aRegItem.ChargeableItem.HIAllowedPrice;
                aRegItem.PriceDifference = aRegItem.InvoicePrice - aRegItem.HIAllowedPrice.GetValueOrDefault(0);
                aRegItem.TotalPriceDifference = aRegItem.PriceDifference * aRegItem.Qty;
                aRegItem.OtherAmt = 0;
            }
            //▲====: #004

            if (aRegItem is PatientRegistrationDetail && aRegItem.ChargeableItem is RefMedicalServiceItem && (aRegItem.ChargeableItem as RefMedicalServiceItem).VATRate.GetValueOrDefault(0) > 0)
            {
                (aRegItem as PatientRegistrationDetail).VATRate = Convert.ToDecimal((aRegItem.ChargeableItem as RefMedicalServiceItem).VATRate);
            }

            if (!onlyRoundResultForOutward)
            {
                aRegItem.TotalHIPayment = MathExt.Round(aRegItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)aRegItem.HIBenefit.GetValueOrDefault(0.0) * aRegItem.Qty, Common.Converters.MidpointRounding.AwayFromZero);
            }
            else
            {
                aRegItem.TotalHIPayment = aRegItem.HIAllowedPrice.GetValueOrDefault(0) * (decimal)aRegItem.HIBenefit.GetValueOrDefault(0.0) * aRegItem.Qty;
            }

            //KMx: Nếu đang tạo Bill DVKTC và loại DV đó KHÔNG phải DVKTC thì mặc định tick "Trong Gói" (11/12/2015 09:38).
            if (IsHighTechServiceBill && !isHighTechServiceDetail)
            {
                aRegItem.IsInPackage = true;
                aRegItem.TotalInvoicePrice = 0;
                aRegItem.TotalPatientPayment = 0;
            }
            else
            {
                aRegItem.IsInPackage = false;
                aRegItem.TotalInvoicePrice = aRegItem.InvoicePrice * (decimal)aRegItem.Qty;
                //▼====: #004
                aRegItem.TotalPatientPayment = aRegItem.TotalInvoicePrice - aRegItem.TotalHIPayment - aRegItem.OtherAmt;
                //▲====: #004
            }

            if (aRegItem.HIBenefit.GetValueOrDefault() > 0 && aRegItem.HIAllowedPrice.GetValueOrDefault() > 0)
            {
                aRegItem.IsCountHI = true;
            }
            else
            {
                aRegItem.IsCountHI = false;
            }
        }
        public static HIAPILogin gLoggedHIAPIUser { get; set; }
        public static string CalculateMD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            var a = new UTF8Encoding().GetBytes(input);
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));
            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString().ToLower();
        }
        public static T ConvertJsonToObject<T>(string JsonString)
        {
            if (string.IsNullOrEmpty(JsonString)) return default(T);
            return JsonConvert.DeserializeObject<T>(JsonString);
        }
        public static string ConvertObjectToJson(object Obj)
        {
            if (Obj == null) return null;
            return JsonConvert.SerializeObject(Obj);
        }
        public static string GetRESTServiceJSon(string Url, string InputJson, string BearerAccessToken = null)
        {
            Nullable<HttpStatusCode> aStatusCode;
            return GetRESTServiceJSon(Url, InputJson, BearerAccessToken, out aStatusCode);
        }
        public static string GetWebContent(string Url)
        {
            var request = WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = @"text/plain;charset=UTF-8";
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetRESTServiceJSon(string Url, string InputJson, string BearerAccessToken, out Nullable<HttpStatusCode> aStatusCode)
        {
            var request = WebRequest.Create(Url);
            request.Method = "POST";
            if (!string.IsNullOrEmpty(BearerAccessToken))
            {
                request.PreAuthenticate = true;
                request.Headers.Add("Authorization", "Bearer " + BearerAccessToken);
            }
            request.ContentType = @"application/json;charset=UTF-8";
            //▼===== #001
            //var mRequestStream = request.GetRequestStream();
            //if (!string.IsNullOrEmpty(InputJson))
            //{
            //    using (var streamWriter = new StreamWriter(mRequestStream))
            //    {
            //        streamWriter.Write(InputJson);
            //        streamWriter.Flush();
            //        streamWriter.Close();
            //    }
            //}
            try
            {
                var mRequestStream = request.GetRequestStream();
                if (!string.IsNullOrEmpty(InputJson))
                {
                    using (var streamWriter = new StreamWriter(mRequestStream))
                    {
                        streamWriter.Write(InputJson);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError(ex.ToString());
                throw ex;
            }
            //▲===== #001
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        if (response is HttpWebResponse)
                        {
                            aStatusCode = (response as HttpWebResponse).StatusCode;
                        }
                        else
                        {
                            aStatusCode = null;
                        }
                        return result;
                    }
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var streamReader = new StreamReader(stream))
                {
                    var result = streamReader.ReadToEnd();
                    throw new Exception(string.Format("[{0}] {1}", ((ex.Response as HttpWebResponse)?.StatusCode.ToString() ?? ex.Status.ToString()), result));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static readonly string MEDIA_TYPE = "application/json";

        /// <summary>
        /// GET method request
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static string RequestGet(string uri, string requestUri)
        {
            if (String.IsNullOrEmpty(uri) || String.IsNullOrEmpty(requestUri))
            {
                return null;
            }

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE));
                    var response = client.GetAsync(requestUri).Result;

                    if (HttpStatusCode.OK != response.StatusCode
                        || !response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        return null;
                    }

                    return response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Common function for the requesting POST method
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestUri"></param>
        /// <param name="contentAsJson"></param>
        /// <returns></returns>
        public static string RequestPOST(string uri, string requestUri, string contentAsJson, TimeSpan timeSpan = new TimeSpan(), string BearerAccessToken = "")
        {
            //if (String.IsNullOrEmpty(uri) || String.IsNullOrEmpty(requestUri))
            //{
            //    return null;
            //}
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE));
                    if (!string.IsNullOrEmpty(BearerAccessToken))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + BearerAccessToken);
                    }
                    if (timeSpan.TotalSeconds != 0)
                    {
                        client.Timeout = timeSpan;
                    }
                    var response = client.PostAsync(requestUri, new StringContent(contentAsJson,
                        System.Text.Encoding.UTF8, MEDIA_TYPE)).Result;
                    if (response.StatusCode == (HttpStatusCode)422)
                    {
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }
                    if (HttpStatusCode.OK != response.StatusCode
                        || !response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        return null;
                    }

                    return response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.Message);
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// Common function for the requesting PUST method
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestUri"></param>
        /// <param name="contentAsJson"></param>
        /// <returns></returns>
        public static string RequestPUT(string uri, string requestUri, string contentAsJson)
        {
            if (String.IsNullOrEmpty(uri) || String.IsNullOrEmpty(requestUri))
            {
                return null;
            }

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(uri);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE));
                    var response = client.PutAsync(requestUri, new StringContent(contentAsJson,
                        System.Text.Encoding.UTF8, MEDIA_TYPE)).Result;

                    if (HttpStatusCode.OK != response.StatusCode
                        || !response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        return null;
                    }
                    return response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.Message);
                    return null;
                }
            }
        }

        public static readonly long EXCEPT_ORDER_NUMBER = 999999999;

        /// <summary>
        /// Validate a new order
        /// </summary>
        /// <param name="refDepId"></param>
        /// <param name="orderNumber"></param>
        /// <param name="patientCode"></param>
        /// <returns></returns>
        public static bool ValidOrder(long refDepId, long orderNumber, string patientCode)
        {
            if (0 == refDepId || 0 == orderNumber)
            {
                MessageBox.Show("Thông tin khoa hoặc STT không hợp lệ. Vui lòng kiểm tra lại!", "Thông báo");
                return false;
            }

            try
            {
                string result = RequestGet(Globals.ServerConfigSection.CommonItems.QMS_API_Url,
                    OrderDTO.GetOrderByOrderNumberAndDeptIdUrl(refDepId, orderNumber));
                if (String.IsNullOrEmpty(result))
                {
                    MessageBox.Show("Thông tin khoa hoặc STT không hợp lệ. Vui lòng kiểm tra lại!", "Thông báo");
                    return false;
                }
                OrderDTO order = OrderDTO.ToDTO(result);
                if (null == order)
                {
                    MessageBox.Show("Thông tin khoa hoặc STT không hợp lệ. Vui lòng kiểm tra lại!", "Thông báo");
                    return false;
                }
                if (String.IsNullOrEmpty(order.patientCode)
                    || (!String.IsNullOrEmpty(patientCode) && patientCode.Equals(order.patientCode)))
                {
                    return true;
                }

                MessageBox.Show("STT không hợp lệ. Vui lòng kiểm tra lại!", "Thông báo");
                return false;
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Init the order information
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="patientPCLRequestDetail"></param>
        /// <param name="status"></param>
        /// <param name="hightPriority"></param>
        /// <returns></returns>
        public static OrderDTO InitOrder(Patient patient,
            PatientPCLRequest patientPCLRequest, string status)
        {
            if (patient == null
                || patientPCLRequest == null)
            {
                return null;
            }

            return new OrderDTO
            {
                patientId = patient.PatientID,
                patientCode = patient.PatientCode,
                patientName = patient.FullName,
                patientDOB = patient.DOB.Value.ToString(OrderDTO.DEFAULT_DATE_FORMAT),
                description = patient.PatientNotes,
                orderStatus = status,
                //ptRegDetailId = patientPCLRequestDetail.PatientPCLReqID,
                refDeptId = patientPCLRequest.DeptID.Value,
                refLocationId = patientPCLRequest.DeptLocID
            };
        }

        /// <summary>
        /// Init the order information
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="ptRegistrationDetail"></param>
        /// <param name="status"></param>
        /// <param name="hightPriority"></param>
        /// <returns></returns>
        public static OrderDTO InitOrder(Patient patient,
            PatientRegistrationDetail ptRegistrationDetail, string status)
        {
            if (patient == null || ptRegistrationDetail == null)
            {
                return null;
            }

            string orderDescription = String.Format(ORDER_DESCRIPTION_PATTERN,
                ptRegistrationDetail.MedServiceID, patient.PatientCode);

            return new OrderDTO
            {
                patientId = patient.PatientID,
                patientCode = patient.PatientCode,
                patientName = patient.FullName,
                patientDOB = patient.DOB.Value.ToString(OrderDTO.DEFAULT_DATE_FORMAT),
                description = orderDescription,
                orderStatus = status,
                ptRegDetailId = ptRegistrationDetail.PtRegDetailID,
                refDeptId = ptRegistrationDetail.DeptLocation.DeptID,
                refLocationId = ptRegistrationDetail.DeptLocation.DeptLocationID,
                hightPriority = ptRegistrationDetail.IsPriority
            };
        }

        /// <summary>
        /// Update the last order
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static OrderDTO UpdateLastOrder(Patient patient, string status)
        {
            if (null == patient)
            {
                return null;
            }

            OrderDTO orderDTO = new OrderDTO()
            {
                orderNumber = patient.OrderNumber,
                patientId = patient.PatientID,
                patientCode = patient.PatientCode,
                patientName = patient.FullName,
                patientDOB = patient.DOB.Value.ToString(OrderDTO.DEFAULT_DATE_FORMAT),
                orderStatus = status,
                startedServiceAt = Globals.GetCurServerDateTime().ToString(OrderDTO.DEFAULT_DATE_TIME_FORMAT),
                refDeptId = Globals.DeptLocation.DeptID,
                refLocationId = Globals.DeptLocation.DeptLocationID
            };

            orderDTO = GlobalsNAV.UpdateOrder(
                OrderDTO.UpdateLastOrderUrl(status, patient.PatientID, Globals.DeptLocation.DeptID),
                orderDTO);
            return orderDTO;
        }

        /// <summary>
        /// Update order
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static OrderDTO UpdateOrder(Patient patient, string status)
        {
            if (null == patient)
            {
                return null;
            }

            OrderDTO orderDTO = new OrderDTO()
            {
                orderNumber = patient.OrderNumber,
                patientId = patient.PatientID,
                patientCode = patient.PatientCode,
                patientName = patient.FullName,
                patientDOB = patient.DOB.Value.ToString(OrderDTO.DEFAULT_DATE_FORMAT),
                orderStatus = status,
                startedServiceAt = Globals.GetCurServerDateTime().ToString(OrderDTO.DEFAULT_DATE_TIME_FORMAT),
                refDeptId = Globals.DeptLocation.DeptID,
                refLocationId = Globals.DeptLocation.DeptLocationID
            };

            orderDTO = GlobalsNAV.UpdateOrder(OrderDTO.PUBLISH_ORDER_URI, orderDTO);
            return orderDTO;
        }

        public static OrderDTO InitOrder(Patient patient, string firstCreatedAt, string status)
        {
            if (null == patient || String.IsNullOrEmpty(status)
                || String.IsNullOrEmpty(firstCreatedAt))
            {
                return null;
            }

            OrderDTO orderDTO = new OrderDTO
            {
                patientId = patient.PatientID,
                patientCode = patient.PatientCode,
                patientName = patient.FullName,
                patientDOB = patient.DOB.Value.ToString(OrderDTO.DEFAULT_DATE_FORMAT),
                description = patient.PatientNotes,
                orderStatus = status,
                refDeptId = Globals.DeptLocation.DeptID,
                refLocationId = Globals.DeptLocation.DeptLocationID,
                createdAt = firstCreatedAt

            };
            return orderDTO;
        }

        /// <summary>
        /// Initialize order
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="deptId"></param>
        /// <param name="deptLocId"></param>
        /// <param name="status"></param>
        /// <param name="hightPriority"></param>
        /// <returns></returns>
        public static OrderDTO InitOrder(Patient patient, long deptId, long deptLocId, string status,
            long? serviceID = null, bool isPriority = false)
        {
            if (null == patient || (0 == deptId && 0 == deptLocId)
                || String.IsNullOrEmpty(status))
            {
                return null;
            }

            string orderDescription = null != serviceID && serviceID > 0
                ? String.Format(ORDER_DESCRIPTION_PATTERN, serviceID, patient.PatientCode) : null;

            OrderDTO orderDTO = new OrderDTO
            {
                patientId = patient.PatientID,
                patientCode = patient.PatientCode,
                patientName = patient.FullName,
                patientDOB = patient.DOB.Value.ToString(OrderDTO.DEFAULT_DATE_FORMAT),
                description = orderDescription,
                orderStatus = status,
                refDeptId = deptId,
                hightPriority = isPriority
            };
            if (0 != deptLocId)
            {
                orderDTO.refLocationId = deptLocId;
            }
            return orderDTO;
        }

        /// <summary>
        /// Update the order
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="orderDTO"></param>
        /// <returns></returns>
        public static OrderDTO UpdateOrder(string uri, OrderDTO orderDTO)
        {
            if (null == orderDTO
                || String.IsNullOrEmpty(orderDTO.ToJSON()))
            {
                return null;
            }
            try
            {
                string result = GlobalsNAV.RequestPUT(Globals.ServerConfigSection.CommonItems.QMS_API_Url,
                    uri, orderDTO.ToJSON());
                if (String.IsNullOrEmpty(result))
                {
                    return null;
                }
                return OrderDTO.ToDTO(result);
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Update the order status
        /// </summary>
        /// <param name="requestOrderDTO"></param>
        /// <returns></returns>
        public static OrderDTO UpdateOrder(OrderDTO requestOrderDTO)
        {
            if (null == requestOrderDTO
                || String.IsNullOrEmpty(requestOrderDTO.ToJSON()))
            {
                return null;
            }
            try
            {
                var result = GlobalsNAV.RequestPUT(Globals.ServerConfigSection.CommonItems.QMS_API_Url,
                    OrderDTO.PUBLISH_ORDER_URI, requestOrderDTO.ToJSON());
                if (String.IsNullOrEmpty(result))
                {
                    return null;
                }
                return OrderDTO.ToDTO(result);
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.Message);
                return null;
            }
        }

        //▼====: #003
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="refDeptId"></param>
        /// <param name="patientCode"></param>
        /// <returns></returns>
        public static OrderDTO GetOrderByDeptIdAndPatientCodeAndExcludeOrder(long refDeptId, string patientCode, string excludeStatus)
        {
            if (0 == refDeptId || String.IsNullOrEmpty(patientCode) || String.IsNullOrEmpty(excludeStatus))
            {
                return null;
            }

            try
            {
                string dataResponse = RequestGet(Globals.ServerConfigSection.CommonItems.QMS_API_Url,
                    OrderDTO.GetOrderByOrderNumberAndDeptIdAndExcludeStatusUrl(refDeptId, patientCode, excludeStatus));
                if (String.IsNullOrEmpty(dataResponse))
                {
                    return null;
                }

                OrderDTO order = OrderDTO.ToDTO(dataResponse);
                if (null != order)
                {
                    return order;
                }
                return null;
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.Message);
                return null;
            }
        }
        //▲====: #003

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="refDeptId"></param>
        /// <param name="patientCode"></param>
        /// <returns></returns>
        public static OrderDTO GetOrderByDeptIdAndPatientCode(long refDeptId, string patientCode)
        {
            if (0 == refDeptId || String.IsNullOrEmpty(patientCode))
            {
                MessageBox.Show("Vui lòng chọn khoa hoặc nhập mã bệnh nhân!", "Thông báo", MessageBoxButton.OK);
                return null;
            }

            try
            {
                string dataResponse = RequestGet(Globals.ServerConfigSection.CommonItems.QMS_API_Url,
                    OrderDTO.GetOrderByPatientCodeAndDeptIdUri(refDeptId, patientCode));
                if (String.IsNullOrEmpty(dataResponse))
                {
                    return null;
                }

                OrderDTO order = OrderDTO.ToDTO(dataResponse);
                if (null != order)
                {
                    return order;
                }
                return null;
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get order by patient code and location
        /// </summary>
        /// <param name="refLocationId"></param>
        /// <param name="patientCode"></param>
        /// <returns></returns>
        public static OrderDTO GetOrderByPatientCode(long refLocationId, string patientCode)
        {
            if (0 == refLocationId || String.IsNullOrEmpty(patientCode))
            {
                MessageBox.Show("Vui lòng chọn khoa hoặc nhập mã bệnh nhân!", "Thông báo", MessageBoxButton.OK);
                return null;
            }

            try
            {
                string dataResponse = RequestGet(Globals.ServerConfigSection.CommonItems.QMS_API_Url,
                    OrderDTO.GetOrderByPatientCodeUri(refLocationId, patientCode));
                if (String.IsNullOrEmpty(dataResponse))
                {
                    return null;
                }

                OrderDTO order = OrderDTO.ToDTO(dataResponse);
                if (null != order)
                {
                    return order;
                }
                return null;
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get the next waiting order
        /// </summary>
        /// <param name="refLocationId"></param>
        /// <returns></returns>
        public static OrderDTO GetNextWaitingOrder(long refLocationId)
        {
            if (0 == refLocationId)
            {
                return null;
            }

            try
            {
                string curDept = String.Format(DEPT_PATTERN, Globals.DeptLocation.DeptLocationID);
                bool isOutpatientDept = curDept.Equals(Globals.ServerConfigSection.CommonItems.OutpatientDept);

                string dataResponse = RequestGet(Globals.ServerConfigSection.CommonItems.QMS_API_Url,
                    OrderDTO.GetNextWaitingOrderUri(refLocationId, isOutpatientDept));
                if (String.IsNullOrEmpty(dataResponse))
                {
                    return null;
                }

                OrderDTO order = OrderDTO.ToDTO(dataResponse);
                if (null != order)
                {
                    return order;
                }
                return null;
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Checking the existed FloorDeptLocation configurations
        /// </summary>
        /// <returns></returns>
        public static bool HasFloorConfiguration()
        {
            return !(String.IsNullOrEmpty(Globals.ServerConfigSection.CommonItems.FloorDeptLocation_0)
                && String.IsNullOrEmpty(Globals.ServerConfigSection.CommonItems.FloorDeptLocation_1)
                && String.IsNullOrEmpty(Globals.ServerConfigSection.CommonItems.FloorDeptLocation_2));
        }

        /// <summary>
        /// Find the floor number of the cashier
        /// </summary>
        /// <returns></returns>
        public static int? GetRequestCashier()
        {
            if (!HasFloorConfiguration()
                || 0 == Globals.DeptLocation.DeptLocationID)
            {
                return null;
            }

            string deptLocationAsStr = "|" + Globals.DeptLocation.DeptLocationID + "|";

            if (!String.IsNullOrEmpty(Globals.ServerConfigSection.CommonItems.FloorDeptLocation_0)
                && (Globals.ServerConfigSection.CommonItems.FloorDeptLocation_0.Contains(deptLocationAsStr)))
            {
                return 0;
            }

            if (!String.IsNullOrEmpty(Globals.ServerConfigSection.CommonItems.FloorDeptLocation_1)
                && (Globals.ServerConfigSection.CommonItems.FloorDeptLocation_1.Contains(deptLocationAsStr)))
            {
                return 1;
            }

            if (!String.IsNullOrEmpty(Globals.ServerConfigSection.CommonItems.FloorDeptLocation_2)
                && (Globals.ServerConfigSection.CommonItems.FloorDeptLocation_2.Contains(deptLocationAsStr)))
            {
                return 2;
            }

            return null;
        }

        /// <summary>
        /// Get a new order with the waitting status
        /// </summary>
        /// <param name="requestOrderDTO"></param>
        /// <param name="PtRegDetailID"></param>
        /// <param name="PCLReqItemID"></param>
        public static OrderDTO GetOrder(OrderDTO orderDTO, long PtRegDetailID = 0, long PCLReqItemID = 0)
        {
            if (null == orderDTO)
            {
                return null;
            }

            try
            {
                // find the floor number for the cashier
                orderDTO.floorNumber = GetRequestCashier();

                string dataResponse = RequestPOST(Globals.ServerConfigSection.CommonItems.QMS_API_Url,
                    eHCMSResources.Z3115_G1_PublishOrder, orderDTO.ToJSON());
                if (String.IsNullOrEmpty(dataResponse))
                {
                    return null;
                }

                OrderDTO order = OrderDTO.ToDTO(dataResponse);
                if (null != order)
                {
                    return order;
                }

                return null;
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get setting value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSettingValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// Check the current department that is the cashier
        /// </summary>
        /// <returns></returns>
        public static bool IsCashier()
        {
            if (0 == Globals.DeptLocation.DeptID)
            {
                return false;
            }

            return (Globals.DeptLocation.DeptID == Globals.ServerConfigSection.CommonItems.Cashier1
                || Globals.DeptLocation.DeptID == Globals.ServerConfigSection.CommonItems.Cashier2);
        }

        /// <summary>
        /// VuTTM - Create new order at the cashier.
        /// </summary>
        public static void CreateCashierOrderNumber(Patient patient)
        {
            if (!(GlobalsNAV.IsApplyingQMS() && !GlobalsNAV.IsCashier())
                || null == patient)
            {
                return;
            }
            OrderDTO order = GlobalsNAV.GetOrder(GlobalsNAV.InitOrder(
                patient, Globals.ServerConfigSection.CommonItems.Cashier2, 0, OrderDTO.WAITING_STATUS));

            if (null == order)
            {
                return;
            }
            //20220111 BLQ : Bỏ thông báo khi cấp số thừ tự
            //MessageBox.Show(String.Format("Số thứ tự của bệnh nhân [{0}] tại quầy kế toán thu là [{1}].",
            //  order.patientName, order.orderNumber));
        }

        /// <summary>
        /// Author: VuTTM
        /// Description: Creating the med department order number
        /// </summary>
        /// <param name="patient"></param>
        public static OrderDTO CreateMedOrderNumber(Patient patient)
        {
            if (!(GlobalsNAV.IsApplyingQMS()) || null == patient)
            {
                return null;
            }
            OrderDTO order = GlobalsNAV.GetOrder(GlobalsNAV.InitOrder(
                patient, Globals.ServerConfigSection.CommonItems.MedDepartment, 0, OrderDTO.WAITING_STATUS));

            if (null == order)
            {
                return null;
            }
            MessageBox.Show(String.Format("Số thứ tự của bệnh nhân [{0}] tại quầy duyệt toa là [{1}].",
                order.patientName, order.orderNumber));
            return order;
        }

        /// <summary>
        /// Author: VuTTM
        /// Description: Creating the pharmacy department order number
        /// </summary>
        /// <param name="patient"></param>
        public static OrderDTO CreatePharmacyOrderNumber(Patient patient)
        {
            if (!(GlobalsNAV.IsApplyingQMS()) || null == patient)
            {
                return null;
            }
            OrderDTO order = GlobalsNAV.GetOrder(GlobalsNAV.InitOrder(
                patient, Globals.ServerConfigSection.CommonItems.PharmacyDepartment, 0, OrderDTO.WAITING_STATUS));

            if (null == order)
            {
                return null;
            }
            MessageBox.Show(String.Format("Số thứ tự của bệnh nhân [{0}] tại nhà thuốc là [{1}].",
                order.patientName, order.orderNumber));
            return order;
        }

        public static bool IsQMSEnable2()
        {
            string depLocIdAsStr = "[" + Globals.DeptLocation.DeptLocationID + "]";
            return (Globals.ServerConfigSection.CommonItems.ApplyQMSAPI
                && Globals.ServerConfigSection.CommonItems.MedDepartment == Globals.DeptLocation.DeptID
                && !Globals.ServerConfigSection.CommonItems.Excluded_Room.Contains(depLocIdAsStr));
        }

        /// <summary>
        /// Check the department is applying queue
        /// </summary>
        /// <returns></returns>
        public static bool IsQMSEnable()
        {
            string depIdAsStr = "[" + Globals.DeptLocation.DeptID + "]";
            string depLocIdAsStr = "[" + Globals.DeptLocation.DeptLocationID + "]";
            return (Globals.ServerConfigSection.CommonItems.ApplyQMSAPI
                && Globals.ServerConfigSection.CommonItems.QMSDepts.Contains(depIdAsStr)
                && !Globals.ServerConfigSection.CommonItems.Excluded_Room.Contains(depLocIdAsStr));
        }

        /// <summary>
        /// Check for applying QMS service
        /// </summary>
        /// <returns></returns>
        public static bool IsApplyingQMS()
        {
            string depIdAsStr = "[" + Globals.DeptLocation.DeptID + "]";
            return (Globals.ServerConfigSection.CommonItems.ApplyQMSAPI
                && Globals.ServerConfigSection.CommonItems.ApplyingQMSDepts.Contains(depIdAsStr));
        }

        public static string PostRESTServiceUsingParams(string Url, out HttpStatusCode? aStatusCode)
        {
            var request = WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = @"application/json;charset=UTF-8";
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        if (response is HttpWebResponse)
                        {
                            aStatusCode = (response as HttpWebResponse).StatusCode;
                        }
                        else
                        {
                            aStatusCode = null;
                        }
                        return result;
                    }
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var streamReader = new StreamReader(stream))
                {
                    var result = streamReader.ReadToEnd();
                    throw new Exception(string.Format("[{0}] {1}", ((ex.Response as HttpWebResponse)?.StatusCode.ToString() ?? ex.Status.ToString()), result));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GetRESTServiceJSon(string UrlAddress, string UrlParams, byte[] ReportStream)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(UrlAddress);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.MaxResponseContentBufferSize = 2000005000L;
                    var serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = int.MaxValue;
                    HttpResponseMessage response = client.PostAsync(UrlParams,
                        new StringContent(serializer.Serialize(ReportStream), Encoding.UTF8, "application/json")).Result;
                    if (HttpStatusCode.OK != response.StatusCode
                        || !response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        return null;
                    }

                    return response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.Message);
                    return null;
                }
            }
        }
        private const string HIAPILoginAddress = "http://egw.baohiemxahoi.gov.vn/api/token/take";
        public static void LoginHIAPI()
        {
            string gHIAPILoginPassword = GlobalsNAV.CalculateMD5Hash(EncryptExtension.Decrypt(Globals.ServerConfigSection.Hospitals.HIAPILoginPassword, Globals.AxonKey, Globals.AxonPass));
            string mLoginData = string.Format("{{\"username\":\"{0}\",\"password\":\"{1}\"}}", Globals.ServerConfigSection.Hospitals.HIAPILoginAccount, gHIAPILoginPassword);
            string mRestJson = GlobalsNAV.GetRESTServiceJSon(HIAPILoginAddress, mLoginData);
            GlobalsNAV.gLoggedHIAPIUser = GlobalsNAV.ConvertJsonToObject<HIAPILogin>(mRestJson);
            GlobalsNAV.gLoggedHIAPIUser.password = gHIAPILoginPassword;
        }
        public static PharmacyAPILogin gLoggedPharmacyAPIUser { get; set; }
        public static PharmacyAPILogin gLoggedPharmacyAPIHUser { get; set; }
        private const string PharmacyAPILoginAddress = "https://duocquocgia.com.vn/api/tai_khoan/dang_nhap";
        private const string PharmacyAPIInwardInvoiceAddress = "https://duocquocgia.com.vn/api/lien_thong/phieu_nhap";
        private const string PharmacyAPIPrescriptionAddress = "https://duocquocgia.com.vn/api/lien_thong/don_thuoc";
        private const string PharmacyAPIOutwardPrescriptionAddress = "https://duocquocgia.com.vn/api/lien_thong/hoa_don";
        private const string PharmacyAPIOutwardInvoiceAddress = "https://duocquocgia.com.vn/api/lien_thong/phieu_xuat";
        public static void LoginPharmacyAPI()
        {
            if (GlobalsNAV.gLoggedPharmacyAPIUser != null && GlobalsNAV.gLoggedPharmacyAPIUser.data != null && !string.IsNullOrEmpty(GlobalsNAV.gLoggedPharmacyAPIUser.data.token))
            {
                return;
            }
            gLoggedPharmacyAPIUser = new PharmacyAPILogin(Globals.ServerConfigSection.CommonItems.DQGUsername, Globals.ServerConfigSection.CommonItems.DQGPassword);
            string mLoginData = string.Format("{{\"usr\":\"{0}\",\"pwd\":\"{1}\"}}", gLoggedPharmacyAPIUser.usr, gLoggedPharmacyAPIUser.pwd);
            string mRestJson = GlobalsNAV.GetRESTServiceJSon(PharmacyAPILoginAddress, mLoginData);
            GlobalsNAV.gLoggedPharmacyAPIUser = GlobalsNAV.ConvertJsonToObject<PharmacyAPILogin>(mRestJson);
        }
        public static void LoginPharmacyAPIHUser()
        {
            if (GlobalsNAV.gLoggedPharmacyAPIHUser != null && GlobalsNAV.gLoggedPharmacyAPIHUser.data != null && !string.IsNullOrEmpty(GlobalsNAV.gLoggedPharmacyAPIHUser.data.token))
            {
                return;
            }
            gLoggedPharmacyAPIHUser = new PharmacyAPILogin(Globals.ServerConfigSection.CommonItems.DQGHUsername, Globals.ServerConfigSection.CommonItems.DQGHPassword);
            string mLoginData = string.Format("{{\"usr\":\"{0}\",\"pwd\":\"{1}\"}}", gLoggedPharmacyAPIHUser.usr, gLoggedPharmacyAPIHUser.pwd);
            string mRestJson = GlobalsNAV.GetRESTServiceJSon(PharmacyAPILoginAddress, mLoginData);
            GlobalsNAV.gLoggedPharmacyAPIHUser = GlobalsNAV.ConvertJsonToObject<PharmacyAPILogin>(mRestJson);
        }
        public static GlobalDrugsSystemAPIResultCode PharmacyAPIImportInwardDrugInvoice(InwardDrugInvoice CurrentInwardDrugInvoice, List<InwardDrug> InwardDrugList)
        {
            try
            {
                LoginPharmacyAPI();
                PharmacyAPIInwardInvoice mImportItem = new PharmacyAPIInwardInvoice(Globals.ServerConfigSection.CommonItems.DQGUnitcode);
                mImportItem.ma_phieu = CurrentInwardDrugInvoice.InvID;
                mImportItem.ngay_nhap = CurrentInwardDrugInvoice.DSPTModifiedDate.ToString("yyyyMMdd");
                mImportItem.ghi_chu = CurrentInwardDrugInvoice.Notes;
                mImportItem.ten_co_so_cung_cap = CurrentInwardDrugInvoice.SelectedSupplier.SupplierName;
                mImportItem.chi_tiet = new List<PharmacyAPIInwardInvoiceDetail>();
                foreach (var item in InwardDrugList)
                {
                    PharmacyAPIInwardInvoiceDetail mDetailItem = new PharmacyAPIInwardInvoiceDetail();
                    mDetailItem.ma_thuoc = item.SelectedDrug.RefGeneralReportCode;
                    mDetailItem.ten_thuoc = item.SelectedDrug.BrandName;
                    mDetailItem.so_lo = item.InBatchNumber;
                    if (item.InProductionDate.HasValue && item.InProductionDate != null)
                    {
                        mDetailItem.ngay_san_xuat = item.InProductionDate.Value.ToString("yyyyMMdd");
                    }
                    if (item.InExpiryDate.HasValue && item.InExpiryDate != null)
                    {
                        mDetailItem.han_dung = item.InExpiryDate.Value.ToString("yyyyMMdd");
                    }
                    mDetailItem.so_dklh = item.SelectedDrug.Visa;
                    mDetailItem.so_luong = Convert.ToInt32(item.InQuantity);
                    mDetailItem.don_gia = Convert.ToInt32(item.InBuyingPrice);
                    mDetailItem.don_vi_tinh = item.SelectedDrug.SeletedUnit.UnitName;
                    mImportItem.chi_tiet.Add(mDetailItem);
                }
                if (GlobalsNAV.gLoggedPharmacyAPIUser.data != null && !string.IsNullOrEmpty(GlobalsNAV.gLoggedPharmacyAPIUser.data.token))
                {
                    string mJsonData = GlobalsNAV.ConvertObjectToJson(mImportItem);
                    try
                    {
                        return GlobalsNAV.ConvertJsonToObject<GlobalDrugsSystemAPIResultCode>(GlobalsNAV.GetRESTServiceJSon(PharmacyAPIInwardInvoiceAddress, mJsonData, GlobalsNAV.gLoggedPharmacyAPIUser.data.token));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static GlobalDrugsSystemAPIResultCode PharmacyAPIImportInwardDrugInvoice(DQG_phieu_nhap aDQG_phieu_nhap)
        {
            try
            {
                LoginPharmacyAPI();
                PharmacyAPIInwardInvoice mImportItem = new PharmacyAPIInwardInvoice(Globals.ServerConfigSection.CommonItems.DQGUnitcode);
                mImportItem.ma_phieu = aDQG_phieu_nhap.ma_phieu_nhap;
                mImportItem.ngay_nhap = aDQG_phieu_nhap.ngay_nhap;
                mImportItem.ghi_chu = aDQG_phieu_nhap.ghi_chu;
                mImportItem.ten_co_so_cung_cap = aDQG_phieu_nhap.ten_co_so_cung_cap;
                mImportItem.chi_tiet = new List<PharmacyAPIInwardInvoiceDetail>();
                foreach (var item in aDQG_phieu_nhap.chi_tiet)
                {
                    PharmacyAPIInwardInvoiceDetail mDetailItem = new PharmacyAPIInwardInvoiceDetail();
                    mDetailItem.ma_thuoc = item.ma_thuoc;
                    mDetailItem.ten_thuoc = item.ten_thuoc;
                    mDetailItem.so_lo = item.so_lo;
                    mDetailItem.ngay_san_xuat = item.ngay_san_xuat;
                    mDetailItem.han_dung = item.han_dung;
                    mDetailItem.so_dklh = item.so_dklh;
                    mDetailItem.so_luong = item.so_luong;
                    mDetailItem.don_gia = Convert.ToInt32(item.don_gia);
                    mDetailItem.don_vi_tinh = item.don_vi_tinh;
                    mImportItem.chi_tiet.Add(mDetailItem);
                }
                if (GlobalsNAV.gLoggedPharmacyAPIUser.data != null && !string.IsNullOrEmpty(GlobalsNAV.gLoggedPharmacyAPIUser.data.token))
                {
                    string mJsonData = GlobalsNAV.ConvertObjectToJson(mImportItem);
                    try
                    {
                        return GlobalsNAV.ConvertJsonToObject<GlobalDrugsSystemAPIResultCode>(GlobalsNAV.GetRESTServiceJSon(PharmacyAPIInwardInvoiceAddress, mJsonData, GlobalsNAV.gLoggedPharmacyAPIUser.data.token));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return new GlobalDrugsSystemAPIResultCode { mess = ex.Message };
            }
        }
        public static GlobalDrugsSystemAPIResultCode PharmacyAPIImportPrescription(DQG_don_thuoc aDQG_don_thuoc)
        {
            try
            {
                LoginPharmacyAPIHUser();
                PharmacyAPIPrescription mImportItem = new PharmacyAPIPrescription();
                mImportItem.ma_don_thuoc_co_so_kcb = aDQG_don_thuoc.ma_don_thuoc_co_so_kcb;
                mImportItem.ngay_ke_don = aDQG_don_thuoc.ngay_ke_don;
                mImportItem.nguoi_ke_don = aDQG_don_thuoc.nguoi_ke_don;
                mImportItem.thong_tin_don_vi = new PharmacyAPIUnit();
                mImportItem.thong_tin_don_vi.ma_co_so_kcb = aDQG_don_thuoc.ma_co_so;
                mImportItem.thong_tin_don_vi.ten_co_so_kcb = aDQG_don_thuoc.ten_co_so;
                mImportItem.thong_tin_benh_nhan = new PharmacyAPIPatient();
                mImportItem.thong_tin_benh_nhan.ma_benh_nhan = aDQG_don_thuoc.ma_benh_nhan;
                mImportItem.thong_tin_benh_nhan.ho_ten = aDQG_don_thuoc.ho_ten;
                mImportItem.thong_tin_benh_nhan.tuoi = aDQG_don_thuoc.tuoi;
                mImportItem.thong_tin_benh_nhan.gioi_tinh = aDQG_don_thuoc.gioi_tinh;
                mImportItem.thong_tin_benh_nhan.dia_chi = aDQG_don_thuoc.dia_chi;
                mImportItem.thong_tin_benh = new PharmacyAPIICD10();
                mImportItem.thong_tin_benh.ma_benh = aDQG_don_thuoc.ma_benh;
                mImportItem.thong_tin_benh.ten_benh = aDQG_don_thuoc.ten_benh;
                mImportItem.thong_tin_don_thuoc = new List<PharmacyAPIPrescriptionDetail>();
                foreach (var item in aDQG_don_thuoc.chi_tiet)
                {
                    PharmacyAPIPrescriptionDetail mDetailItem = new PharmacyAPIPrescriptionDetail();
                    mDetailItem.ma_thuoc = item.ma_thuoc;
                    mDetailItem.ten_thuoc = item.ten_thuoc;
                    mDetailItem.don_vi_tinh = item.don_vi_tinh;
                    mDetailItem.ham_luong = item.ham_luong;
                    mDetailItem.duong_dung = item.duong_dung;
                    mDetailItem.lieu_dung = item.lieu_dung;
                    mDetailItem.so_dang_ky = item.so_dang_ky;
                    mDetailItem.so_luong = item.so_luong;
                    mImportItem.thong_tin_don_thuoc.Add(mDetailItem);
                }
                if (GlobalsNAV.gLoggedPharmacyAPIHUser.data != null && !string.IsNullOrEmpty(GlobalsNAV.gLoggedPharmacyAPIHUser.data.token))
                {
                    string mJsonData = GlobalsNAV.ConvertObjectToJson(mImportItem);
                    try
                    {
                        return GlobalsNAV.ConvertJsonToObject<GlobalDrugsSystemAPIResultCode>(GlobalsNAV.GetRESTServiceJSon(PharmacyAPIPrescriptionAddress, mJsonData, GlobalsNAV.gLoggedPharmacyAPIHUser.data.token));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return new GlobalDrugsSystemAPIResultCode { mess = ex.Message };
            }
        }
        public static GlobalDrugsSystemAPIResultCode PharmacyAPIImportOutwardPrescription(DQG_hoa_don aDQG_hoa_don)
        {
            try
            {
                LoginPharmacyAPI();
                PharmacyAPIOutwardPrescription mImportItem = new PharmacyAPIOutwardPrescription();
                mImportItem.ma_hoa_don = aDQG_hoa_don.ma_hoa_don;
                mImportItem.ma_co_so = aDQG_hoa_don.ma_co_so;
                mImportItem.ma_don_thuoc_quoc_gia = aDQG_hoa_don.ma_don_thuoc_quoc_gia;
                mImportItem.ngay_ban = aDQG_hoa_don.ngay_ban;
                mImportItem.ho_ten_nguoi_ban = aDQG_hoa_don.ho_ten_nguoi_ban;
                mImportItem.ho_ten_khach_hang = aDQG_hoa_don.ho_ten_khach_hang;
                mImportItem.hoa_don_chi_tiet = new List<PharmacyAPIOutwardPrescriptionDetail>();
                foreach (var item in aDQG_hoa_don.chi_tiet)
                {
                    PharmacyAPIOutwardPrescriptionDetail mDetailItem = new PharmacyAPIOutwardPrescriptionDetail();
                    mDetailItem.ma_thuoc = item.ma_thuoc;
                    mDetailItem.ten_thuoc = item.ten_thuoc;
                    mDetailItem.so_lo = item.so_lo;
                    mDetailItem.ngay_san_xuat = item.ngay_san_xuat;
                    mDetailItem.han_dung = item.han_dung;
                    mDetailItem.don_vi_tinh = item.don_vi_tinh;
                    mDetailItem.ham_luong = item.ham_luong;
                    mDetailItem.duong_dung = item.duong_dung;
                    mDetailItem.lieu_dung = item.lieu_dung;
                    mDetailItem.so_dang_ky = item.so_dang_ky;
                    mDetailItem.so_luong = item.so_luong;
                    mDetailItem.don_gia = Convert.ToInt32(item.don_gia);
                    mDetailItem.thanh_tien = Convert.ToInt32(item.thanh_tien);
                    mDetailItem.ty_le_quy_doi = item.ty_le_quy_doi;
                    mImportItem.hoa_don_chi_tiet.Add(mDetailItem);
                }
                if (GlobalsNAV.gLoggedPharmacyAPIUser.data != null && !string.IsNullOrEmpty(GlobalsNAV.gLoggedPharmacyAPIUser.data.token))
                {
                    string mJsonData = GlobalsNAV.ConvertObjectToJson(mImportItem);
                    try
                    {
                        return GlobalsNAV.ConvertJsonToObject<GlobalDrugsSystemAPIResultCode>(GlobalsNAV.GetRESTServiceJSon(PharmacyAPIOutwardPrescriptionAddress, mJsonData, GlobalsNAV.gLoggedPharmacyAPIUser.data.token));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return new GlobalDrugsSystemAPIResultCode { mess = ex.Message };
            }
        }
        public static GlobalDrugsSystemAPIResultCode PharmacyAPIImportOutwardInvoice(DQG_phieu_xuat aDQG_phieu_xuat)
        {
            try
            {
                LoginPharmacyAPI();
                PharmacyAPIOutwardInvoice mImportItem = new PharmacyAPIOutwardInvoice();
                mImportItem.ma_phieu = aDQG_phieu_xuat.ma_phieu_xuat;
                mImportItem.ma_co_so = aDQG_phieu_xuat.ma_co_so;
                mImportItem.ngay_xuat = aDQG_phieu_xuat.ngay_xuat;
                mImportItem.loai_phieu_xuat = aDQG_phieu_xuat.loai_phieu_xuat;
                mImportItem.ghi_chu = aDQG_phieu_xuat.ghi_chu;
                mImportItem.ten_co_so_nhan = aDQG_phieu_xuat.ten_co_so_nhan;
                mImportItem.chi_tiet = new List<PharmacyAPIOutwardInvoiceDetail>();
                foreach (var item in aDQG_phieu_xuat.chi_tiet)
                {
                    PharmacyAPIOutwardInvoiceDetail mDetailItem = new PharmacyAPIOutwardInvoiceDetail();
                    mDetailItem.ma_thuoc = item.ma_thuoc;
                    mDetailItem.ten_thuoc = item.ten_thuoc;
                    mDetailItem.so_lo = item.so_lo;
                    mDetailItem.ngay_san_xuat = item.ngay_san_xuat;
                    mDetailItem.han_dung = item.han_dung;
                    mDetailItem.so_dklh = item.so_dklh;
                    mDetailItem.so_luong = item.so_luong;
                    mDetailItem.don_gia = Convert.ToInt32(item.don_gia);
                    mDetailItem.don_vi_tinh = item.don_vi_tinh;
                    mImportItem.chi_tiet.Add(mDetailItem);
                }
                if (GlobalsNAV.gLoggedPharmacyAPIUser.data != null && !string.IsNullOrEmpty(GlobalsNAV.gLoggedPharmacyAPIUser.data.token))
                {
                    string mJsonData = GlobalsNAV.ConvertObjectToJson(mImportItem);
                    try
                    {
                        return GlobalsNAV.ConvertJsonToObject<GlobalDrugsSystemAPIResultCode>(GlobalsNAV.GetRESTServiceJSon(PharmacyAPIOutwardInvoiceAddress, mJsonData, GlobalsNAV.gLoggedPharmacyAPIUser.data.token));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return new GlobalDrugsSystemAPIResultCode { mess = ex.Message };
            }
        }
        public static void AddNewPCLRequestToRIS(HL7_INP aHL7_INP)
        {
            string mJsonData = GlobalsNAV.ConvertObjectToJson(aHL7_INP);
            try
            {
                Nullable<HttpStatusCode> mStatusCode;
                GlobalsNAV.GetRESTServiceJSon(Globals.ServerConfigSection.Pcls.RISAPIAddress, mJsonData, null, out mStatusCode);
                if (mStatusCode == null || !mStatusCode.HasValue || mStatusCode.Value != HttpStatusCode.OK)
                {
                    throw new Exception(eHCMSResources.Z1684_G1_WCFError);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //private string gPACSAPISendWorkListParams = "&MaChiDinh={0}&ThoiGianChiDinh={1}&MaBenhNhan={2}&password={3}&loaiHoSo=3&maTinh={4}&maCSKCB={5}";
        public static void AddNewPCLRequestToPAC(HL7_ITEM aHL7_INP)
        {
            string mJsonData = ConvertObjectToQueryParams(aHL7_INP);
            try
            {
                //string gHIAPILoginPassword = CalculateMD5Hash(EncryptExtension.Decrypt(Globals.ServerConfigSection.Pcls.PACPassword, Globals.AxonKey, Globals.AxonPass));
                string mLoginData = string.Format("/?user_name={0}&password={1}", Globals.ServerConfigSection.Pcls.PACUserName, Globals.ServerConfigSection.Pcls.PACPassword);
                HttpStatusCode? mStatusCode;
                PostRESTServiceUsingParams(Globals.ServerConfigSection.Pcls.PACSAPIAddress + mLoginData + mJsonData, out mStatusCode);
                if (mStatusCode == null || !mStatusCode.HasValue || mStatusCode.Value != HttpStatusCode.OK)
                {
                    throw new Exception(eHCMSResources.Z1684_G1_WCFError);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ConvertObjectToQueryParams(HL7_ITEM temp)
        {
            string strTemp = "";
            foreach (var p in temp.GetType().GetProperties())
            {
                if (p.GetValue(temp, null) != null)
                {
                    strTemp += "&" + p.Name + "=" + p.GetValue(temp, null);
                }
            }
            return strTemp;
        }

        //▼====: #002
        public static void AddPCLResultToPAC(PACResult aPACResult)
        {
            string mJsonData = ConvertObjectToQueryParams(aPACResult);
            try
            {
                string mLoginData = string.Format("/?user_name={0}&password={1}", Globals.ServerConfigSection.Pcls.PACUserName, Globals.ServerConfigSection.Pcls.PACPassword);
                HttpStatusCode? mStatusCode;
                PostRESTServiceUsingParams(Globals.ServerConfigSection.Pcls.PACSAPIAddress + mLoginData + mJsonData, out mStatusCode);
                if (mStatusCode == null || !mStatusCode.HasValue || mStatusCode.Value != HttpStatusCode.OK)
                {
                    throw new Exception(eHCMSResources.Z1684_G1_WCFError);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ConvertObjectToQueryParams(PACResult temp)
        {
            string strTemp = "";
            foreach (var p in temp.GetType().GetProperties())
            {
                if (p.GetValue(temp, null) != null)
                {
                    strTemp += "&" + p.Name + "=" + p.GetValue(temp, null);
                }
            }
            return strTemp;
        }
        //▲====: #002

        /// <summary>
        /// Creating orders
        /// Author: VuTTM
        /// </summary>
        /// <param name="isPaid"></param>
        /// <param name="hasCashierOrder"></param>
        /// <param name="patient"></param>
        /// <param name="SavedRegistrationDetailList"></param>
        /// <param name="SavedPclRequestList"></param>
        public static void CreateOrders(bool isPaid, bool hasCashierOrder,
            Patient patient, ref IList<PatientRegistrationDetail> SavedRegistrationDetailList,
            ref IList<PatientPCLRequest> SavedPclRequestList, bool canUpdateSeqNumber = false,
            List<PatientRegistrationDetail> deletedServiceList = null, List<PatientPCLRequest> deletedPclRequestList = null)
        {
            List<PatientRegistrationDetail> patientRegistrationDetails = null;
            if (null != SavedRegistrationDetailList)
            {
                patientRegistrationDetails = SavedRegistrationDetailList.ToList<PatientRegistrationDetail>();
            }

            List<PatientPCLRequest> patientPCLRequests = null;
            if (null != SavedPclRequestList)
            {
                patientPCLRequests = SavedPclRequestList.ToList<PatientPCLRequest>();
            }
            CreateOrders(isPaid, hasCashierOrder, patient, ref patientRegistrationDetails, ref patientPCLRequests, canUpdateSeqNumber,
                deletedServiceList, deletedPclRequestList);

            SavedRegistrationDetailList = patientRegistrationDetails;
            SavedPclRequestList = patientPCLRequests;
        }

        /// <summary>
        /// Creating orders
        /// Author: VuTTM
        /// </summary>
        /// <param name="isPaid"></param>
        /// <param name="patient"></param>
        /// <param name="paidRegDetailsList"></param>
        /// <param name="paidPclRequestList"></param>
        public static List<string> CreateOrders(bool isPaid, bool hasCashierOrder, Patient patient,
            ref List<PatientRegistrationDetail> regDetailsList, ref List<PatientPCLRequest> pclRequestList, bool canUpdateSeqNumber = false,
            List<PatientRegistrationDetail> deletedServiceList = null, List<PatientPCLRequest> deletedPclRequestList = null, bool IsSimplePayView = false)
        {
            //▼====: #009
            //if (!IsQMSEnable())
            if (!IsQMSEnableForMedicalExamination())
            {
                return null;
            }
            //▲====: #009

            // Create a new cashier order
            if (hasCashierOrder && GlobalsNAV.IsApplyingQMS() && !GlobalsNAV.IsCashier())
            {
                GlobalsNAV.CreateCashierOrderNumber(patient);
            }

            List<string> orderNumberLst = new List<string>();

            if (null != regDetailsList && regDetailsList.Count > 0)
            {
                ProcessRegistration(isPaid, patient, ref regDetailsList, ref orderNumberLst, canUpdateSeqNumber, deletedServiceList);
            }

            List<string> pclOrderNumberLst = new List<string>();
            //20220330 QTD Thêm cấu hình bật QMS cho CLS
            if (null != pclRequestList && pclRequestList.Count > 0 && Globals.ServerConfigSection.CommonItems.IsEnableQMSForPCL)
            {
                ProcessPCL(isPaid, patient, ref pclRequestList, ref pclOrderNumberLst, canUpdateSeqNumber, deletedPclRequestList);
            }

            if (null != pclOrderNumberLst && pclOrderNumberLst.Count > 0)
            {
                foreach (string pclOrderNumber in pclOrderNumberLst)
                {
                    orderNumberLst.Add(pclOrderNumber);
                }
            }

            if (null != orderNumberLst && orderNumberLst.Count > 0 && !IsSimplePayView)
            {
                UpdateOrderNumber(orderNumberLst);
                return null;
            }
            return orderNumberLst;
        }

        /// <summary>
        /// Create prescription orders
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="prescriptionDetails"></param>
        public static void CreatePrescriptionOrders(Patient patient, long PrescriptID,
            ObservableCollection<PrescriptionDetail> prescriptionDetails)
        {
            if (null == patient || null == prescriptionDetails)
            {
                return;
            }
            HasPrescriptionOrders(prescriptionDetails, out bool hasPharmacyOrder, out bool hasPrescriptionOrder);
            if (hasPharmacyOrder)
            {
                CreatePharmacyOrderNumber(patient);
            }

            OrderDTO order = null;
            if (hasPrescriptionOrder)
            {
                order = CreateMedOrderNumber(patient);
            }

            if (null == order)
            {
                order = UpdateOrder(InitOrder(patient,
                    Globals.ServerConfigSection.CommonItems.MedDepartment, 0, OrderDTO.WAITING_STATUS));
            }

            List<string> orderNumLst = new List<string>();
            long orderNumber = 0;
            long roomID = 0;
            if (null != order)
            {
                orderNumber = null == order.orderNumber ? 0 : order.orderNumber.Value;
                roomID = null == order.roomId ? 0 : order.roomId.Value;
                orderNumLst.Add(
                    String.Format(UPDATED_PRE_ORDER_NUMBER_PATTERN, PrescriptID, orderNumber, roomID));
            }
            if (null != orderNumLst && orderNumLst.Count > 0)
            {
                UpdateOrderNumber(orderNumLst);
            }
        }

        private static readonly string UPDATED_PCL_ORDER_NUMBER_PATTERN = "PCL-{0}-{1}-{2}";
        private static readonly string UPDATED_REG_ORDER_NUMBER_PATTERN = "REG-{0}-{1}-{2}";
        private static readonly string UPDATED_PRE_ORDER_NUMBER_PATTERN = "PRE-{0}-{1}-{2}";

        /// <summary>
        /// QMS Service
        /// Creating the registration orders
        /// Author: VuTTM
        /// </summary>
        /// <param name="paidRegDetailsList"></param>
        /// <returns></returns>
        public static bool ProcessRegistration(bool isPaid, Patient patient,
            ref List<PatientRegistrationDetail> regDetailsList, ref List<string> orderNumLst, bool canUpdateSeqNumber = false,
            List<PatientRegistrationDetail> deletedServiceList = null)
        {
            long orderNumber = 0;
            long roomID = 0;

            if (null != deletedServiceList && deletedServiceList.Count > 0)
            {
                foreach (var detail in regDetailsList)
                {
                    UpdateOrder(InitOrder(patient, detail.DeptLocation.DeptID,
                        detail.DeptLocation.DeptLocationID, OrderDTO.DONE_STATUS));
                }
            }

            if (null == patient
                || (regDetailsList == null || regDetailsList.Count == 0))
            {
                return false;
            }

            foreach (var detail in regDetailsList)
            {
                if (checkDepLocIDIsApplyQMS(detail.DeptLocID) || (detail.DeptLocation != null && checkDepLocIDIsApplyQMS(detail.DeptLocation.DeptLocationID)))
                {
                    if ((detail.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                        || detail.RecordState == RecordState.DELETED
                        || detail.RecordState == RecordState.DETACHED
                        || (detail.RecordState == RecordState.MODIFIED && detail.PaidStaffID == 0)))
                    {
                        // Update order status for canceling the registration
                        UpdateOrder(InitOrder(patient, detail.DeptLocation.DeptID,
                            detail.DeptLocation.DeptLocationID, OrderDTO.DONE_STATUS));
                    }
                    else if (detail.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM
                        || detail.RecordState == RecordState.ADDED
                        || detail.RecordState == RecordState.DETACHED)
                    {
                        //20220329 QTD Không dùng status này
                        //string curOrderStatus = (isPaid && detail.PaidStaffID >= 0) ? OrderDTO.WAITING_FOR_VITAL_SIGNS : OrderDTO.NOT_PAY_YET_STATUS;
                        string curOrderStatus = (isPaid && detail.PaidStaffID >= 0) ? OrderDTO.WAITING_STATUS : OrderDTO.NOT_PAY_YET_STATUS;
                        //string curOrderStatus = OrderDTO.WAITING_STATUS;

                        // Get a new order number
                        OrderDTO order = GetOrder(InitOrder(patient, detail, curOrderStatus));
                        if (null == order)
                        {
                            order = UpdateOrder(InitOrder(patient, detail.DeptLocation.DeptID,
                                detail.DeptLocation.DeptLocationID, curOrderStatus, detail.MedServiceID));
                        }
                        // Update the order number for the HIS registration
                        if ((null != order && isPaid)
                            || canUpdateSeqNumber)
                        {
                            detail.ServiceSeqNum = (int)order.orderNumber.Value;
                            orderNumber = null == order.orderNumber ? 0 : order.orderNumber.Value;
                            roomID = null == order.roomId ? 0 : order.roomId.Value;

                            orderNumLst.Add(
                                String.Format(UPDATED_REG_ORDER_NUMBER_PATTERN, detail.PtRegDetailID, orderNumber, roomID));
                        }
                    }
                }
            }

            return true;
        }

        public static void UpdateDoneStatus(Patient patient, List<PatientPCLRequest> deletedPclRequestList)
        {
            if (null == deletedPclRequestList || deletedPclRequestList.Count < 1)
            {
                return;
            }

            foreach (var item in deletedPclRequestList)
            {
                foreach (var detail in item.PatientPCLRequestIndicators)
                {
                    GetLocation(detail.DeptLocation, item.DeptLocID, out long curDeptId, out long curDeptLocId);
                    UpdateOrder(InitOrder(patient, curDeptId, curDeptLocId, OrderDTO.DONE_STATUS));
                }

            }
        }

        /// <summary>
        /// QMS Service
        /// Creating the PCL request orders
        /// Author: VuTTM
        /// </summary>
        /// <param name="paidPclRequestList"></param>
        /// <returns></returns>
        public static bool ProcessPCL(bool isPaid, Patient patient,
            ref List<PatientPCLRequest> pclRequestList, ref List<string> orderNumLst, bool canUpdateSeqNumber = false,
            List<PatientPCLRequest> deletedPclRequestList = null)
        {
            UpdateDoneStatus(patient, deletedPclRequestList);
            long orderNumber = 0;
            long roomID = 0;

            if (null == patient
                || (null == pclRequestList || pclRequestList.Count == 0))
            {
                return false;
            }

            foreach (var item in pclRequestList)
            {
                if (checkDepLocIDIsApplyQMS(item.DeptLocID))
                {
                    foreach (var detail in item.PatientPCLRequestIndicators
                        .Where(x => ((x.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                        || x.RecordState == RecordState.DELETED
                                        || x.RecordState == RecordState.MODIFIED))))
                    {
                        GetLocation(detail.DeptLocation, item.DeptLocID, out long curDeptId, out long curDeptLocId);
                        UpdateOrder(InitOrder(patient, curDeptId, curDeptLocId, OrderDTO.DONE_STATUS));
                    }

                    foreach (var detail in item.PatientPCLRequestIndicators
                        .Where(x => (x.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM
                                    || x.RecordState == RecordState.ADDED
                                    || x.RecordState == RecordState.DETACHED)))
                    {
                        if (null == detail.DeptLocation && 0 == item.DeptLocID)
                        {
                            MessageBox.Show(UNSPECIFIED_DEPT_LOC_MSG);
                            return false;
                        }

                        //20220329 QTD Chưa dùng Status này
                        string curOrderStatus = (isPaid && detail.PaidStaffID >= 0) ? OrderDTO.WAITING_STATUS : OrderDTO.NOT_PAY_YET_STATUS;
                        //string curOrderStatus = OrderDTO.WAITING_STATUS;

                        GetLocation(detail.DeptLocation, item.DeptLocID, out long curDeptId, out long curDeptLocId);

                        // Get a new order number
                        OrderDTO order = GetOrder(InitOrder(patient, curDeptId, curDeptLocId, curOrderStatus,
                            detail.PCLExamTypeID, detail.IsPriority));
                        if (null == order)
                        {
                            order = UpdateOrder(InitOrder(patient, curDeptId, curDeptLocId, curOrderStatus,
                                detail.PCLExamTypeID));
                        }
                        // Update the order number for the HIS registration
                        if ((null != order && isPaid)
                            || canUpdateSeqNumber)
                        {
                            detail.ServiceSeqNum = (int)order.orderNumber.Value;
                            orderNumber = null == order.orderNumber ? 0 : order.orderNumber.Value;
                            roomID = null == order.roomId ? 0 : order.roomId.Value;
                            orderNumLst.Add(
                                String.Format(UPDATED_PCL_ORDER_NUMBER_PATTERN, detail.PCLReqItemID, orderNumber, roomID));
                        }
                    }
                }
            }

            return true;
        }

        private static void GetLocation(DeptLocation deptLocation, long deptLocId,
            out long curDeptId, out long curDeptLocId)
        {
            curDeptId = null != deptLocation ? deptLocation.DeptID : 0;
            curDeptLocId = null != deptLocation ? deptLocation.DeptLocationID : deptLocId;
        }

        /// <summary>
        /// QMS Service
        /// Update order number
        /// Author: BaoLQ, VuTTM
        /// </summary>
        /// <param name="PtRegDetailID"></param>
        /// <param name="PCLReqItemID"></param>
        /// <param name="orderDTO"></param>
        private static void UpdateOrderNumber(List<string> orderNumLst)
        {
            if (null == orderNumLst || orderNumLst.Count < 1)
            {
                MessageBox.Show(CANNOT_CREATE_ORDER_MSG, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }

            var t = new Thread(() =>
            {
                using (var mFactory = new CommonService_V2Client())
                {
                    try
                    {
                        var mContract = mFactory.ServiceInstance;

                        mContract.BeginUpdateOrderNumberRegistration(orderNumLst,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool IsOK = mContract.EndUpdateOrderNumberRegistration(asyncResult);
                                    if (!IsOK)
                                    {
                                        MessageBox.Show(CANNOT_CREATE_ORDER_MSG);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                            }), null);
                    }
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                    }
                }
            });
            t.Start();
        }

        /// <summary>
        /// Check for existence of the pharmacy and prescription orders 
        /// </summary>
        /// <param name="prescriptionDetails"></param>
        /// <param name="hasPharmacyOrder"></param>
        /// <param name="hasPrescriptionOrder"></param>
        public static void HasPrescriptionOrders(ObservableCollection<PrescriptionDetail> prescriptionDetails,
            out bool hasPharmacyOrder, out bool hasPrescriptionOrder)
        {
            hasPharmacyOrder = false;
            hasPrescriptionOrder = false;

            if (null == prescriptionDetails || prescriptionDetails.ToList<PrescriptionDetail>().Count < 1)
            {
                return;
            }

            List<PrescriptionDetail> prescriptions = prescriptionDetails.ToList<PrescriptionDetail>();
            foreach (PrescriptionDetail prescription in prescriptions)
            {
                if (prescription.BeOfHIMedicineList)
                {
                    hasPrescriptionOrder = true;
                }
                else
                {
                    hasPharmacyOrder = true;
                }
            }
        }

        public static readonly string CANNOT_CREATE_ORDER_MSG = "Không thể cấp số thứ tự!";
        public static readonly string CREATED_ORDER_MSG = "Số thứ tự của bệnh nhân là {0}.";
        public static readonly string UNSPECIFIED_DEPT_LOC_MSG = "Bạn chưa chọn phòng cho CLS. Vui lòng xóa và chọn lại!";
        public static readonly string ORDER_DESCRIPTION_PATTERN = "{0} - {1}"; // ServiceId - PatientCode
        public static readonly string DEPT_PATTERN = "[{0}]";

        public static bool checkDepLocIDIsApplyQMS (long? deptLocID)
        {
            if (deptLocID == null || deptLocID == 0)
            {
                return false;
            }    
            string strDeptLocID = "|" + deptLocID + "|";
            return (!String.IsNullOrEmpty(Globals.ServerConfigSection.CommonItems.FloorDeptLocation_0)
                        && Globals.ServerConfigSection.CommonItems.FloorDeptLocation_0.Contains(strDeptLocID))
                    || (!String.IsNullOrEmpty(Globals.ServerConfigSection.CommonItems.FloorDeptLocation_1)
                        && Globals.ServerConfigSection.CommonItems.FloorDeptLocation_1.Contains(strDeptLocID))
                    || (!String.IsNullOrEmpty(Globals.ServerConfigSection.CommonItems.FloorDeptLocation_2)
                        && Globals.ServerConfigSection.CommonItems.FloorDeptLocation_2.Contains(strDeptLocID));
        }
        //▼====: #006
        public static void AddUpdateDoctorContactPatientTimeAction(long PatientID, long PtRegistrationID, long PtRegDetailID, DateTime StartDatetime,string Log, long DoctorStaffID)
        {
            DoctorContactPatientTime doctorContactPatientTime = new DoctorContactPatientTime
            {
                PatientID = PatientID,
                PtRegistrationID = PtRegistrationID,
                PtRegDetailID = PtRegDetailID,
                StartDatetime = StartDatetime,
                EndDatetime = Globals.GetCurServerDateTime(),
                Log = Log,
                DoctorStaffID = DoctorStaffID
            };
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var aContract = serviceFactory.ServiceInstance;
                        aContract.BeginAddUpdateDoctorContactPatientTime(doctorContactPatientTime, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var result = aContract.EndAddUpdateDoctorContactPatientTime(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.Message);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.Message);
                }
            });

            t.Start();
        }
        //▲====: #006

        //▼====: #009
        public static bool IsQMSEnableForMedicalExamination()
        {
            string join = ";";
            string depIdAsStr = "[" + Globals.DeptLocation.DeptID + "]";
            string depLocIdAsStr = string.Concat(join, Globals.DeptLocation.DeptLocationID, join);
            string temp = string.Concat(join, Globals.ServerConfigSection.CommonItems.DeptLocIDApplyQMS, join);
            return Globals.ServerConfigSection.CommonItems.ApplyQMSAPI
                && ((Globals.ServerConfigSection.CommonItems.QMSDepts.Contains(depIdAsStr) && temp.Contains(depLocIdAsStr)) || IsCashier());
        }
        //▲====: #009

        //▼====: #010
        public static bool SaveDataForSmsLab(long PatientPCLReqID, long PatientID, string XML)
        {
            if ((PatientPCLReqID == 0 || PatientID == 0) && string.IsNullOrEmpty(XML))
            {
                return false;
            }

            try
            {
                string paramURL = "";
                string dataResponse = "";
                if (string.IsNullOrEmpty(XML))
                {
                    paramURL = string.Format("sms-info?PatientPCLReqID={0}&PatientID={1}", PatientPCLReqID, PatientID);
                    dataResponse = RequestPOST(Globals.ServerConfigSection.CommonItems.SMS_API_Url, paramURL, "");
                }
                else
                {
                    dataResponse = RequestPOST(Globals.ServerConfigSection.CommonItems.SMS_API_Url, "sms-info/save-all", XML);
                }

                if (string.IsNullOrEmpty(dataResponse))
                {
                    return false;
                }
                return Convert.ToBoolean(dataResponse);
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.Message);
                return false;
            }
        }
        //▲====: #010
    }
}
