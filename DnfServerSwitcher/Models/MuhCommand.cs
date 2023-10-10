using System;
using System.Windows.Input;
namespace DnfServerSwitcher.Models {
    public class MuhCommand : ICommand {
        public event EventHandler? CanExecuteChanged;
        
        private Action? _exec;
        private Func<bool>? _canExec;
        
        public bool CanExecute(object parameter) => this._exec != null && (this._canExec?.Invoke() ?? false);
        public void Execute(object parameter) => this._exec?.Invoke();

        public MuhCommand(Action execute, Func<bool> canExecute) {
            this._exec = execute;
            this._canExec = canExecute;
        }

        public void RefreshCanExecute() {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
