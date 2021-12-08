using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Domo
{
    public enum RepositoryChangeType
    {
        RepositoryAdded,
        RepositoryDeleted,
        ModelAdded,
        ModelRemoved,
        ModelUpdated,
        ModelInvalid,
    }

    public class RepositoryChangeArgs : EventArgs
    {
        public IRepository Repository { get; set; }
        public Guid ModelId { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public RepositoryChangeType ChangeType { get; set; }
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
        IRepository AddRepository(IRepository repository);

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
        /// Called after a change to a repository 
        /// </summary>
        event EventHandler<RepositoryChangeArgs> RepositoryChanged;
    }

    /// <summary>
    /// A repository is a container for either zero or more domain models (an IAggregateRepository)
    /// or a single domain model (ISingletonRepository).
    /// A repository is responsible for managing the actual state of the domain model, and
    /// supports Create, GetModel, Update, and Delete (CRUD) operations. 
    /// Repositories are stored in a Value Store. A Repository's Guid is a compile-time constant that
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
        Type ValueType { get; }

        /// <summary>
        /// Returns the model stored in the repository
        /// </summary>
        IModel GetModel(Guid modelId);

        /// <summary>
        /// Returns the value stored in the repository. 
        /// </summary>
        object GetValue(Guid modelId);

        /// <summary>
        /// Call this function to attempt a change in the state of particular repository. 
        /// </summary>
        bool Update(Guid modelId, Func<object, object> updateFunc);

        /// <summary>
        /// Returns true if the state is valid, or false otherwise.
        /// </summary>
        bool Validate(object state);

        /// <summary>
        /// Creates a new domain model given the existing state and adds it to the repository.
        /// </summary>
        IModel Add(object state);

        /// <summary>
        /// Deletes the specified domain model.  
        /// </summary>
        void Delete(Guid modelId);

        /// <summary>
        /// Returns all of the managed domain models at the current moment in time. 
        /// </summary>
        IReadOnlyList<IModel> GetModels();

        /// <summary>
        /// Returns true if the model exists, or false otherwise
        /// </summary>
        bool ModelExists(Guid modelId);

        /// <summary>
        /// Called after a change to a repository 
        /// </summary>
        event EventHandler<RepositoryChangeArgs> RepositoryChanged;

        /// <summary>
        /// Removes all of the models from the repository. 
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Strongly typed repository. The T object can be any C# type, but it is strongly recommended to be immutable.
    /// Reference between data models should be created via ModelReference classes.
    /// </summary>
    public interface IRepository<TValue>
        : IRepository
    {
        /// <summary>
        /// Returns the model stored in the repository. 
        /// </summary>
        new IModel<TValue> GetModel(Guid modelId);

        /// <summary>
        /// Returns the value stored in the repository. 
        /// </summary>
        new TValue GetValue(Guid modelId);

        bool Update(Guid modelId, Func<TValue,TValue> updateFunc);

        bool Validate(TValue value);

        IModel<TValue> Add(TValue value);

        new IReadOnlyList<IModel<TValue>> GetModels();
    }

    /// <summary>
    /// An aggregate repository manages a collection of domain models. 
    /// </summary>
    public interface IAggregateRepository<T> : 
        IRepository<T>, INotifyCollectionChanged
    {
        int Count { get; }

        IModel<T> this[int index] { get; }
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
        IModel<T> Model { get; }

        T Value { get; set; }
    }

    /// <summary>
    /// A Domain Model is a wrapper around a single state value assumed to be immutable.
    /// If the state value is replaced with a new one, it triggers a PropertyChanged
    /// event, however the parameter name will all be String.Empty.
    /// This allows Views or View Models to respond to changes in a domain model. 
    /// Domain models can refer 
    /// (a guid) to identify the model across different states.
    /// The state type (T) can be any C# type but is strongly recommended to be immutable.
    /// If the state changes the INotifyPropertyChanged will always be triggered, with a null
    /// parameter name.
    /// This enables domain models to support data binding to views or view models as desired.
    /// When Disposed all events handlers are removed. 
    /// </summary>
    public interface IModel :
        INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Represents this particular domain model. Is persistent, and does not change
        /// if the underlying value changed. Useful for creating serializable references 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The underlying value or entity of the model. The actual value
        /// is stored in a repository usign the Guid as a key. 
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// The type of the value or entity.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// The in-memory backing storage for the model values
        /// </summary>
        IRepository Repository { get; }

        /// <summary>
        /// Called by the repository to identify when changes happen.
        /// This invokes the INotifyPropertyChanged.PropertyChanged event
        /// with the parameter name set to string.Empty.
        /// </summary>
        void TriggerChangeNotification();
    }

    /// <summary>
    /// Type safe model. The type parameter can be a class or struct. It is recommended that the
    /// type parameter is type-safe.
    /// Do not derive your classes from this class. 
    /// </summary>
    public interface IModel<TValue> 
        : IModel 
    {
        new TValue Value { get; set; }
        new IRepository<TValue> Repository { get; }
    }
}
