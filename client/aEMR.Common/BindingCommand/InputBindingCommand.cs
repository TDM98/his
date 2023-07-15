using System;
using System.Windows;
using System.Windows.Input;

namespace aEMR.Common
{
    public class InputBindingCommand : ICommand
    {        
        public event EventHandler CanExecuteChanged;

        public int HotKey_Registered_ID = 0;
        public string HotKey_Registered_Name;
        public Window TheHostWindow = null;

        public  Action<object> _executeDelegate;
        private Func<object, bool> _canExecutePredicate; 

        public Key GestureKey { get; set; }
        public ModifierKeys GestureModifier { get; set; }
        public MouseAction MouseGesture { get; set; }
        
        public bool CanExecute(object parameter)
        {
            return _canExecutePredicate(parameter);
        }

        public InputBindingCommand(Action executeDelegate)
        {
            _executeDelegate = x => executeDelegate();
            _canExecutePredicate = x => true;
        }

        public InputBindingCommand(Action<object> executeDelegate)
        {
            _executeDelegate = executeDelegate;
            _canExecutePredicate = x => true;
        }

        public void Execute(object parameter)
        {
            _executeDelegate(parameter);
        }

        public InputBindingCommand If(Func<bool> canExecutePredicate)
        {
            _canExecutePredicate = x => canExecutePredicate();

            return this;
        }

        public InputBindingCommand If(Func<object, bool> canExecutePredicate)
        {
            _canExecutePredicate = canExecutePredicate;

            return this;
        }
    }
}