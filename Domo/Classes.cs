using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domo
{
    /// <summary>
    /// Use references to make persistent links between models. 
    /// </summary>
    public struct DomainModelReference<T>
    {
        public DomainModelReference(IDomainModel<T> model)
            => Model = model;

        private IDomainModel<T> Model { get; }
        public T State => Model.Data;
    }

    public class DomainModel<T> : IDomainModel<T>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Guid Id { get; }
        public T Data { get; set; }

        object IDomainModel.Data
        {
            get => Data;
            set => Data = value;
        }

        public Type DataType { get; }
        public IRepository<T> Repository { get; }

        IRepository IDomainModel.Repository => Repository;
    }

    class DataStore : IDataStore
    {
        private IList<IRepository> _repositories = new List<IRepository>();

        public void Dispose()
        {
            RepositoryChanged = null;
            RepositoryChanging = null;
        }

        public void AddRepository(IRepository repo)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IRepository> GetRepositories()
        {
            throw new NotImplementedException();
        }

        public void DeleteRepository(IRepository repository)
        {
            throw new NotImplementedException();
        }

        public IRepository GetRepository(Type type)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<RepositoryChangeArgs> RepositoryChanging;
        public event EventHandler<RepositoryChangeArgs> RepositoryChanged;
    }

    class BaseRepository<T> : IRepository<T>
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Guid RepositoryId { get; }
        public Version Version { get; }
        public Type ModelType { get; }
        object IRepository.Read(Guid modelId)
        {
            return Read(modelId);
        }

        public bool Update(Guid modelId, Func<T, T> updateFunc)
        {
            throw new NotImplementedException();
        }

        public bool Validate(Guid modelId, T state)
        {
            throw new NotImplementedException();
        }

        public IDomainModel<T> Create(T state)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IDomainModel<T>> GetDomainModels()
        {
            throw new NotImplementedException();
        }

        public T Read(Guid modelId)
        {
            throw new NotImplementedException();
        }

        public bool Update(Guid modelId, Func<object, object> updateFunc)
        {
            throw new NotImplementedException();
        }

        public bool Validate(Guid modelId, object state)
        {
            throw new NotImplementedException();
        }

        public IDomainModel Create(object state)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Guid modelId)
        {
            throw new NotImplementedException();
        }

        IReadOnlyList<IDomainModel> IRepository.GetDomainModels()
        {
            return GetDomainModels();
        }
    }

    class AggregateRepository<T> : BaseRepository<T>, IAggregateRepository<T>
    {

    }

    class SingletonRepository<T> : BaseRepository<T>, ISingletonRepository<T>
    {

    }


    public static class DomoExtensions
    {
        public static void DeleteAllRepositories()
        {

        }
    }
}