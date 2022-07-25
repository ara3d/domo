using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Domo
{
    public interface IService
    {
        //string Name { get; }
        //Version Version { get; }
        //IReadOnlyList<INamedCommand> GetCommands();
    }

    public interface ISingletonModelBackedService<T> : IService, INotifyPropertyChanged
    {
        ISingletonRepository<T> Repository { get; }
        IModel<T> Model { get; }
    }

    public interface IAggregateModelBackedService<T> : IService, INotifyCollectionChanged
    {
        IAggregateRepository<T> Repository { get; }
        IReadOnlyList<IModel<T>> Models { get; }
    }

    public class Service : IService
    {
        public Service(IRepositoryManager store)
            => Store = store;

        public IRepositoryManager Store { get; }

        public Dictionary<string, INamedCommand> Commands = new Dictionary<string, INamedCommand>();

        public INamedCommand RegisterCommand(Delegate execute, Delegate canExecute, IRepository repository)
        {
            var r = new NamedCommand(execute, canExecute, repository);
            Commands.Add(r.Name, r);
            return r;
        }

        public INamedCommand GetCommand(string name)
            => Commands[name];

        public IReadOnlyList<INamedCommand> GetCommands()
            => Commands.Values.ToList();
    }

    public class SingletonModelBackedService<T> : Service, ISingletonModelBackedService<T>
    {
        public SingletonModelBackedService(IRepositoryManager store)
            : base(store)
        {
            Repository = store.GetSingletonRepository<T>();
            Repository.RepositoryChanged += OnRepositoryChanged;
        }

        protected virtual void OnRepositoryChanged(object sender, RepositoryChangeArgs e)
        { }

        public event PropertyChangedEventHandler PropertyChanged
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
        public AggregateModelBackedService(IRepositoryManager store)
            : base(store)
        {
            Repository = store.GetAggregateRepository<T>();
            Repository.RepositoryChanged += OnRepositoryChanged;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => Repository.CollectionChanged += value;
            remove => Repository.CollectionChanged -= value;
        }

        protected virtual void OnRepositoryChanged(object sender, RepositoryChangeArgs e)
        { }

        public IAggregateRepository<T> Repository { get; }
        public IReadOnlyList<IModel<T>> Models => Repository.GetModels();
    }

}