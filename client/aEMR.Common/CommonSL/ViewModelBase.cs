using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Caliburn.Micro;
using aEMR.Common.HotKeyManagement;

namespace aEMR.Common.BaseModel
{
    public static class UiElementExtensions
    {
        public static Window GetWindow(this FrameworkElement element)
        {
            if (element == null)
                return null;

            if (element is Window)
                return (Window)element;

            if (element.Parent == null)
                return null;

            return GetWindow(element.Parent as FrameworkElement);
        }
    }
    public class ViewModelBase : Conductor<object>.Collection.AllActive
    {
        // =========================================  HOT KEY BEGIN  =============================================

        // TxD 12/12/2018: The following Method is to be OVERRIDEN in the Derived class of this ViewModelBase
        //                  thus the Action is defined in the derived ViewModel
        public virtual void HandleHotKey_Action_New(object sender, LocalHotKeyEventArgs e)
        {
        }
        private HotKeyManager TheNew_HotKey_Manager = null;        
        private List<LocalHotKey> LocalHotKeysReg;
        private void Register_HotKeys_New(Window theWindow, string strHotKeyName, ModifierKeys modKey, Keys theKey)
        {
            if (TheNew_HotKey_Manager == null)
            {
                TheNew_HotKey_Manager = new HotKeyManager(theWindow);                
                LocalHotKeysReg = new List<LocalHotKey>();                
                TheNew_HotKey_Manager.LocalHotKeyPressed  += new LocalHotKeyEventHandler(HandleHotKey_Action_New);
            }

            LocalHotKey newHotKey = new LocalHotKey(strHotKeyName, modKey, theKey);
            LocalHotKeysReg.Add(newHotKey);

            TheNew_HotKey_Manager.AddLocalHotKey(newHotKey);
        }
        private void UnRegister_HotKeys_New(LocalHotKey theUnregHotKey)
        {
            if (TheNew_HotKey_Manager == null)
                return;
            TheNew_HotKey_Manager.RemoveLocalHotKey(theUnregHotKey);
        }
        // =========================================  HOT KEY END  =============================================
        public ViewModelBase()
        {
        }
        public bool _hasInputBindingCmd = false;
        public bool HasInputBindingCmd
        {
            get { return _hasInputBindingCmd; }
            set
            {
                _hasInputBindingCmd = value;
            }
        }
        private void RegisterHotKeyCommands(IEnumerable<InputBindingCommand> inputBindingCommands, Window theWindow)
        {
            foreach (var inputBindingCommand in inputBindingCommands)
            {
                inputBindingCommand.TheHostWindow = theWindow;
                ListInputBindingCmds.Add(inputBindingCommand);

                Register_HotKeys_New(theWindow, inputBindingCommand.HotKey_Registered_Name, inputBindingCommand.GestureModifier, (Keys)inputBindingCommand.GestureKey);
            }
        }
        public void DeregisterHotKeyCommands()
        {
            foreach(var theUnregHotKey in LocalHotKeysReg)
            {
                UnRegister_HotKeys_New(theUnregHotKey);
            }
            LocalHotKeysReg.Clear();
            ListInputBindingCmds.Clear();
        }
        public List<InputBindingCommand> ListInputBindingCmds = new List<InputBindingCommand>();
        private bool bHotKeysUnregistered = false;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            if (HasInputBindingCmd)
            {                
                var theWindow = (view as FrameworkElement).GetWindow();
                var helper = new WindowInteropHelper(theWindow);

                RegisterHotKeyCommands(GetInputBindingCommands(), theWindow);
            }
        }
        public void Deactivate(bool close)
        {
            OnDeactivate(close);
        }
        protected override void OnDeactivate(bool close)
        {
            if (HasInputBindingCmd)
            {                
                if (bHotKeysUnregistered == false)
                {
                    DeregisterHotKeyCommands();
                    bHotKeysUnregistered = true;
                }
            }

            base.OnDeactivate(close);
        }
        protected virtual IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield break;
        }
        public virtual string ChildWindowTitle
        {
            get
            {
                return ToString();
            }
        }
        /// <summary>
        /// Goi ham nay thong bao thuoc tinh IsProcessing va StatusText da thay doi.
        /// Can phai refresh trang thai busy indicator
        /// </summary>
        protected void NotifyWhenBusy()
        {
            NotifyOfPropertyChange(() => IsProcessing);
            NotifyOfPropertyChange(() => StatusText);
        }
        public virtual bool IsProcessing
        {
            get
            {
                return false;
            }
        }
        public virtual string StatusText
        {
            get
            {
                return string.Empty;
            }
        }
        public virtual bool HasCloseButton
        {
            get
            {
                return true;
            }
        }
        private string _dlgBusyContent = "";
        public string DlgBusyContent
        {
            get
            {
                return _dlgBusyContent;
            }
            set
            {
                _dlgBusyContent = value;
                NotifyOfPropertyChange(() => DlgBusyContent);
            }
        }
        private bool _dlgIsBusyFlag = false;
        public bool DlgIsBusyFlag
        {
            get
            {
                return _dlgIsBusyFlag;
            }
            set
            {
                _dlgIsBusyFlag = value;
                NotifyOfPropertyChange(() => DlgIsBusyFlag);
            }
        }
        public int _nDlgBusyIndicatorCnt = 0;
        public bool IsDialogView = false;
    }
}