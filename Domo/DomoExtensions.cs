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
                if (args.ChangeType == RepositoryChangedEvent.ModelUpdated || args.ChangeType == RepositoryChangedEvent.ModelAdded)
                {
                    var model = (IModel<T>)args.Repository.GetModel(args.ModelId);
                    action.Invoke(model);
                }
            };

        public static void OnModelsChanged<T>(this IRepository<T> repository, Action<IReadOnlyList<IModel<T>>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                if (args.ChangeType == RepositoryChangedEvent.ModelUpdated || args.ChangeType == RepositoryChangedEvent.ModelAdded || args.ChangeType == RepositoryChangedEvent.ModelRemoved)
                {
                    var models = (IReadOnlyList<IModel<T>>)args.Repository.GetModels();
                    action.Invoke(models);
                }
            };
    }
}