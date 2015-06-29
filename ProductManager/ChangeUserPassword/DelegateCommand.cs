using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ChangeUserPassword
{
    class DelegateCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (FuncCanExecute == null)
                return true;

            return FuncCanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (ActionExecute == null)
                return;

            ActionExecute(parameter);
        }

        public Func<object, bool> FuncCanExecute { get; set; }
        public Action<object> ActionExecute { get; set; }
    }
}
