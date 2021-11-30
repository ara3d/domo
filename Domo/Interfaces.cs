using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Domo
{
    public enum RepositoryChangedEvent
    {
        RepositoryAdded,
        RepositoryRemoved,
        DomainModelAdded,
        DomainModelRemoved,
        DomainModelUpdated,
    }

    public class RepositoryChangeArgs : EventArgs
    {
        public IRepository Repository { get; set; }
        public Guid DomainModelId { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public RepositoryChangedEvent ChangeType { get; set; }
    }

    /// <summary>
    /// A data store is the owning collection of repositories.
    /// When disposed, all repositories are deleted (disposed).
    /// Provides hooks for responding to changes to repositories. 
    /// </summary>
    public interface IDataStore
        : IDisposable
    {
        /// <summary>
        /// Adds a repository to the store. The DataStore is now
        /// responsible for disposing the repository
        /// </summary>
        void AddRepository(IRepository repo);

        /// <summary>
        /// Geta a shallow copy of all of the repositories managed
        /// by the store at the current moment. 
        /// </summary>
        IReadOnlyList<IRepository> GetRepositories();

        /// <summary>
        /// Removes the specified repository from the store, and disposes it. 
        /// </summary>
        void DeleteRepository(IRepository repository);

        /// <summary>
        /// Retrieves a repository based on the type.  
        /// </summary>
        IRepository GetRepository(Type type);

        /// <summary>
        /// Called prior to a change to a repository
        /// </summary>
        event EventHandler<RepositoryChangeArgs> RepositoryChanging;

        /// <summary>
        /// Called after a change to a repository 
        /// </summary>
        event EventHandler<RepositoryChangeArgs> RepositoryChanged;
    }

    /// <summary>
    /// A repository is a container for either zero or more domain models (an IAggregateRepository)
    /// or a single domain model (ISingletonRepository).
    /// A repository is responsible for managing the actual state of the domain model, and
    /// supports Create, Read, Update, and Delete (CRUD) operations. 
    /// Repositories are stored in a Data Store. A Repository's Guid is a compile-time constant that
    /// defines its identity across processes, and versions. This is useful for serialization
    /// of repositories, and having different versions of a repsitory. 
    /// When disposed, all domain models are disposed.
    /// </summary>
    public interface IRepository 
        : IDisposable
    {
        /// <summary>
        /// Should be consistent across processes and versions.
        /// Two repositories with the same Guid but different versions could be registered with a data store. 
        /// </summary>
        Guid RepositoryId { get; }

        /// <summary>
        /// The version of the repository. Should be changed when the layout of the state type changes. 
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// The type of the model objects stored in in this particular repository 
        /// </summary>
        Type ModelType { get; }

        /// <summary>
        /// Returns the model 
        /// </summary>
        object Read(Guid modelId);

        /// <summary>
        /// Call this function to attempt a change in the state of particular repository. 
        /// </summary>
        bool Update(Guid modelId, Func<object, object> updateFunc);

        /// <summary>
        /// Returns true if the state is a valid transition from the current state. 
        /// </summary>
        bool Validate(Guid modelId, object state);

        /// <summary>
        /// Creates a new domain model given the existing state. 
        /// </summary>
        IDomainModel Create(object state);

        /// <summary>
        /// Deletes the specified domain model. 
        /// </summary>
        bool Delete(Guid modelId);

        /// <summary>
        /// Returns all of the managed domain models at the current moment in time. 
        /// </summary>
        IReadOnlyList<IDomainModel> GetDomainModels();
    }

    /// <summary>
    /// Strongly typed repository. The T object can be any C# type, but it is strongly recommended to be immutable.
    /// Reference between data models should be created via DomainModelReference classes.
    /// </summary>
    public interface IRepository<T>
        : IRepository
    {
        /// <summary>
        /// Returns the concrete model stored in the repository. 
        /// </summary>
        T Read(Guid modelId);

        /// <summary>
        /// 
        /// </summary>
        bool Update(Guid modelId, Func<T,T> updateFunc);

        bool Validate(Guid modelId, T state);

        IDomainModel<T> Create(T state);

        new IReadOnlyList<IDomainModel<T>> GetDomainModels();
    }

    /// <summary>
    /// An aggregate repository manages a collection of domain models. 
    /// </summary>
    public interface IAggregateRepository<T> : 
        IRepository<T>, INotifyCollectionChanged, IList<IDomainModel<T>>
    {
    }

    /// <summary>
    /// In a singleton repository, the Guid of the DomainModel is the same Guid as that of the Repository.
    /// To be informed of changes to an underlying data model subscriptions should be made to the model
    /// itself.
    /// </summary>
    public interface ISingletonRepository<T> 
        : IRepository<T>
    {
        /// <summary>
        /// The domain model associated with the repository 
        /// </summary>
        IDomainModel<T> Model { get; }
    }

    /// <summary>
    /// A Domain Model is a wrapper around a single state value assumed to be immutable.
    /// If the state value is replaced with a new one, it triggers a PropertyChanged
    /// event with a null parameter. This allows Views or View Models
    /// to respond to changes in a domain model. 
    /// Domain models can refer 
    /// (a guid) to identify the model across different states.
    /// The state type (T) can be any C# type but is strongly recommended to be immutable.
    /// If the state changes the INotifyPropertyChanged will always be triggered, with a null
    /// parameter name.
    /// This enables domain models to support data binding to views or view models as desired.
    /// When Disposed all events handlers are removed. 
    /// </summary>
    public interface IDomainModel :
        INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Represents this particular domain model. Used for creating serializable references 
        /// </summary>
        Guid Id { get; }

        object Data { get; set; }

        Type DataType { get; }

        IRepository Repository { get; }
    }

    /// <summary>
    /// Type safe domain model
    /// </summary>
    public interface IDomainModel<T> 
        : IDomainModel
    {
        new T Data { get; set; }
        new IRepository<T> Repository { get; }
    }
}
