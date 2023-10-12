using System;
using System.Windows.Input;
using DnfServerSwitcher.Models.Trace;
namespace DnfServerSwitcher.Models {
    public class MuhCommand<T> : ICommand {
        public event EventHandler? CanExecuteChanged;
        
        private Action<T>? _exec;
        private Func<T,bool>? _canExec;

        public bool CanExecute(object parameter) {
            try {
                return parameter is T myT && this._exec != null && (this._canExec?.Invoke(myT) ?? false);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command, ex);
                return false;
            } 
        }
        public void Execute(object parameter) {
            if (parameter is T myT) this._exec?.Invoke(myT);
        }

        public MuhCommand(Action<T> execute, Func<T,bool> canExecute) {
            this._exec = execute;
            this._canExec = canExecute;
        }

        public void RefreshCanExecute() {
            try {
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            } catch (Exception ex) {
                Glog.Error(MyTraceCategory.Command,ex);
            }
        }
    }
}
