using System.Collections.Specialized;
using System.ComponentModel;

namespace Domo.Sample.Services
{
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

        public bool CanExecute(object? parameter = null)
            => (bool?)CanExecuteDelegate.DynamicInvoke(parameter) != false;
        
        public void Execute(object? parameter = null) 
            => ExecuteDelegate.DynamicInvoke(parameter);

        public Delegate CanExecuteDelegate { get; }
        public Delegate ExecuteDelegate { get; }
        public event EventHandler? CanExecuteChanged;
        public string Name { get; }
    }

    public class Service
    {
        public Service(IDataStore store)
            => Store = store;
        public IDataStore Store { get; }

        public Dictionary<string, INamedCommand> Commands = new();

        public INamedCommand RegisterCommand(Delegate execute, Delegate canExecute, IRepository repository)
        {
            var r = new NamedCommand(execute, canExecute, repository);
            Commands.Add(r.Name, r);
            return r;
        }

        public INamedCommand GetCommand(string name)
            => Commands[name];
    }

    public class SingletonModelBackedService<T> : Service, ISingletonModelBackedService<T>
    {
        public SingletonModelBackedService(IDataStore store)
            : base(store)
        {
            Repository = store.GetSingletonRepository<T>();
            Repository.RepositoryChanged += OnRepositoryChanged;
        }

        protected virtual void OnRepositoryChanged(object? sender, RepositoryChangeArgs e)
        { }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add => Model.PropertyChanged += value;
            remove => Model.PropertyChanged -= value;
        }

        public ISingletonRepository<T> Repository { get; }
        public IModel<T> Model => Repository.Model;

        public T Value
        {
            get => Model.Value;
            set => Model.Value = value;
        }
    }

    public class AggregateModelBackedService<T> : Service, IAggregateModelBackedService<T>
    {
        public AggregateModelBackedService(IDataStore store)
            : base(store)
        {
            Repository = store.GetAggregateRepository<T>();
            Repository.RepositoryChanged += OnRepositoryChanged;
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => Repository.CollectionChanged += value;
            remove => Repository.CollectionChanged -= value;
        }

        protected virtual void OnRepositoryChanged(object? sender, RepositoryChangeArgs e)
        { }

        public IAggregateRepository<T> Repository { get; }
        public IReadOnlyList<IModel<T>> Models => Repository.GetModels();
    }


}
