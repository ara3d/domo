using System;
using System.Collections.Generic;
using System.IO;

namespace Domo
{
    public static class DomoExtensions
    {
        public static IRepository<T> GetRepository<T>(this IDataStore store)
            => (IRepository<T>)store.GetRepository(typeof(T));

        public static IAggregateRepository<T> GetAggregateRepository<T>(this IDataStore store)
            => (IAggregateRepository<T>)store.GetRepository(typeof(T));

        public static ISingletonRepository<T> GetSingletonRepository<T>(this IDataStore store)
            => (ISingletonRepository<T>)store.GetRepository(typeof(T));

        public static void DeleteAllRepositories(this IDataStore store)
        {
            foreach (var r in store.GetRepositories())
                store.DeleteRepository(r);
        }

        public static void OnModelChanged<T>(this IRepository<T> repository, Action<IModel<T>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                if (args.ChangeType == RepositoryChangeType.ModelUpdated || args.ChangeType == RepositoryChangeType.ModelAdded)
                {
                    var model = (IModel<T>)args.Repository.GetModel(args.ModelId);
                    action.Invoke(model);
                }
            };

        public static void OnModelsChanged<T>(this IRepository<T> repository, Action<IReadOnlyList<IModel<T>>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                if (args.ChangeType == RepositoryChangeType.ModelUpdated || args.ChangeType == RepositoryChangeType.ModelAdded || args.ChangeType == RepositoryChangeType.ModelRemoved)
                {
                    var models = (IReadOnlyList<IModel<T>>)args.Repository.GetModels();
                    action.Invoke(models);
                }
            };

        public static IRepository<T> AddTypedRepository<T>(this IDataStore store, IRepository<T> repository)
            => (IRepository<T>)store.AddRepository(repository);

        public static IRepository<T> CreateAggregateRepository<T>(Guid id, Version version = null, Func<T, bool> validator = null)
            => new AggregateRepository<T>(id, version ?? new Version(), validator);

        public static IRepository<T> CreateSingletonRepository<T>(Guid id, Version version = null, T value = default, Func<T, bool> validator = null)
            where T : new()
            => new SingletonRepository<T>(id, version ?? new Version(), value == null ? new T() : value, validator);

        public static ISingletonRepository<T> AddSingletonRepository<T>(this IDataStore store, T value = default) where T : new()
            => (ISingletonRepository<T>)store.AddTypedRepository<T>(CreateSingletonRepository(Guid.NewGuid(), new Version(), value == null ? new T() : value, null));

        public static IAggregateRepository<T> AddAggregateRepository<T>(this IDataStore store)
            => (IAggregateRepository<T>)store.AddTypedRepository(CreateAggregateRepository<T>(Guid.NewGuid(), new Version(), null));

        public static bool Update<T>(this IModel<T> model, Func<T, T> updateFunc)
            => model.Repository.Update(model.Id, updateFunc);

        public static string ToDebugString(this IModel model)
            => model == null ? "null" : $"{model.Id} {model.ValueType?.Name} {model.Value}";
    }
}