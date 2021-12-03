using System;
using System.Windows.Input;

namespace Domo
{
    public interface INamedCommand : ICommand
    {
        string Name { get; }
    }

    public interface INamedCommand<T> : INamedCommand
    {
        bool CanExecute(T parameter);
        void Execute(T parameter);
    }

    public class NamedCommand : INamedCommand
    {
        public NamedCommand(Delegate execute, Delegate canExecute, IRepository repository)
        {
            Name = execute.Method.Name;
            ExecuteDelegate = execute;
            CanExecuteDelegate = canExecute;
            repository.RepositoryChanged +=
                (_, _) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter = null)
            => (bool?)CanExecuteDelegate.DynamicInvoke(parameter) != false;

        public void Execute(object parameter = null)
            => ExecuteDelegate.DynamicInvoke(parameter);

        public Delegate CanExecuteDelegate { get; }
        public Delegate ExecuteDelegate { get; }
        public event EventHandler CanExecuteChanged;
        public string Name { get; }
    }
}