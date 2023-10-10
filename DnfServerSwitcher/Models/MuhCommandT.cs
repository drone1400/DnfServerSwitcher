using System;
using System.Windows.Input;
namespace DnfServerSwitcher.Models {
    public class MuhCommand<T> : ICommand {
        public event EventHandler? CanExecuteChanged;
        
        private Action<T>? _exec;
        private Func<T,bool>? _canExec;
        
        public bool CanExecute(object parameter) => parameter is T myT && this._exec != null && (this._canExec?.Invoke(myT) ?? false);
        public void Execute(object parameter) {
            if (parameter is T myT) this._exec?.Invoke(myT);
        }

        public MuhCommand(Action<T> execute, Func<T,bool> canExecute) {
            this._exec = execute;
            this._canExec = canExecute;
        }

        public void RefreshCanExecute() {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
