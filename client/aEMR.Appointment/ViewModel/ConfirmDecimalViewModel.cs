using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.HotKeyManagement;
using aEMR.ViewContracts;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IConfirmDecimal)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConfirmDecimalViewModel : ViewModelBase, IConfirmDecimal
    {
        #region Properties
        private int _ConfirmValue = 0;
        public int ConfirmValue
        {
            get
            {
                return _ConfirmValue;
            }
            set
            {
                if (_ConfirmValue == value)
                {
                    return;
                }
                _ConfirmValue = value;
                NotifyOfPropertyChange(() => ConfirmValue);
            }
        }
        public bool IsConfirmed { get; set; } = false;
        #endregion
        #region Events
        [ImportingConstructor]
        public ConfirmDecimalViewModel()
        {
            this.HasInputBindingCmd = true;
        }
        public void ConfirmButton()
        {
            IsConfirmed = true;
            TryClose();
        }
        #endregion
        #region KeyHandles
        public override void HandleHotKey_Action_New(object sender, LocalHotKeyEventArgs e)
        {
            foreach (var inputBindingCommand in ListInputBindingCmds)
            {
                if (inputBindingCommand.HotKey_Registered_Name == e.HotKey.Name)
                {
                    inputBindingCommand._executeDelegate.Invoke(this);
                    break;
                }
            }
        }
        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(() => ConfirmButton())
            {
                HotKey_Registered_Name = "CDConfirmButton",
                GestureModifier = ModifierKeys.None,
                GestureKey = (Key)Keys.Enter
            };
        }
        #endregion
    }
}