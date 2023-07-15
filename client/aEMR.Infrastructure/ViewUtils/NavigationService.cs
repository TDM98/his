using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using aEMR.Common.Enums;
using aEMR.ViewContracts;

namespace aEMR.Infrastructure.ViewUtils
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

        public void ShowDialog<T>(Action<T> onInitialize, Action<MsgBoxOptions, IScreen> onClose 
            , MsgBoxOptions msgBoxOptions
            , Dictionary<string, object> settings 
            )
        {
            var dialogContent = BuildDialogContentViewModel();
            dialogContent.Option = msgBoxOptions;
            dialogContent.DisplayContent<T>(onInitialize, null, msgBoxOptions, onClose);
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

        public void ShowBusy()
        {
            ShowBusy("Processing...");
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

    // TxD 17/08/2015 added GenericCoroutineTask
    public class GenericCoRoutineTask : IResult
    {
        private object _objParam1 = null;
        private object _objParam2 = null;
        private object _objParam3 = null;

        private Exception _error = null;
        public Exception Error
        {
            get { return _error; }
            set
            {
                _error = value;
            }
        }

        private List<object> _listResultObjs;

        private System.Action<GenericCoRoutineTask> _executeAction;
        private System.Action<GenericCoRoutineTask, object> _executeActionOneParam;
        private System.Action<GenericCoRoutineTask, object, object> _executeActionTwoParams;
        private System.Action<GenericCoRoutineTask, object, object, object> _executeActionThreeParams;

        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask> exeAction)
        {
            this._executeAction = new System.Action<GenericCoRoutineTask>(exeAction);
            _listResultObjs = new List<object>();
        }
        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask, object> exeAction, object param1)
        {
            this._executeActionOneParam = new System.Action<GenericCoRoutineTask, object>(exeAction);
            _listResultObjs = new List<object>();
            _objParam1 = param1;
        }
        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask, object, object> exeAction, object param1, object param2)
        {
            this._executeActionTwoParams = new System.Action<GenericCoRoutineTask, object, object>(exeAction);
            _listResultObjs = new List<object>();
            _objParam1 = param1;
            _objParam2 = param2;
        }
        public GenericCoRoutineTask(System.Action<GenericCoRoutineTask, object, object, object> exeAction, object param1, object param2, object param3)
        {
            this._executeActionThreeParams = new System.Action<GenericCoRoutineTask, object, object, object>(exeAction);
            _listResultObjs = new List<object>();
            _objParam1 = param1;
            _objParam2 = param2;
            _objParam3 = param3;
        }


        public void AddResultObj(object objRes)
        {
            _listResultObjs.Add(objRes);
        }

        public object GetResultObj(int nIdx)
        {
            if (nIdx >= _listResultObjs.Count)
            {
                return null;
            }

            return _listResultObjs[nIdx];

        }

        public void ActionComplete(bool bContinue)
        {
            Completed(this, new ResultCompletionEventArgs
            {
                Error = null,
                WasCancelled = !bContinue
            });
        }

        public void Execute(ActionExecutionContext context)
        {
            if (_objParam3 != null)
            {
                _executeActionThreeParams(this, _objParam1, _objParam2, _objParam3);
            }
            else if (_objParam2 != null)
            {
                _executeActionTwoParams(this, _objParam1, _objParam2);
            }
            else if (_objParam1 != null)
            {
                _executeActionOneParam(this, _objParam1);
            }
            else
            {
                _executeAction(this);
            }
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public static IResult StartTask(System.Action<GenericCoRoutineTask> exeAction)
        {
            return new GenericCoRoutineTask(exeAction);
        }
        public static IResult StartTask(System.Action<GenericCoRoutineTask, object> exeActionOneParam, object param1)
        {
            return new GenericCoRoutineTask(exeActionOneParam, param1);
        }
        public static IResult StartTask(System.Action<GenericCoRoutineTask, object, object> exeActionTwoParams, object param1, object param2)
        {
            return new GenericCoRoutineTask(exeActionTwoParams, param1, param2);
        }
        public static IResult StartTask(System.Action<GenericCoRoutineTask, object, object, object> exeActionThreeParams, object param1, object param2, object param3)
        {
            return new GenericCoRoutineTask(exeActionThreeParams, param1, param2, param3);
        }

    }
}
