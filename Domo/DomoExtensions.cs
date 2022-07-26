using System;
using System.Collections.Generic;
using System.Linq;

namespace Domo
{
    public static class DomoExtensions
    {
        public static void OnModelAdded<T>(this IAggregateRepository<T> repository, Action<IModel<T>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                if (args.ChangeType != RepositoryChangeType.ModelAdded) return;
                action.Invoke((IModel<T>)args.Repository.GetModel(args.ModelId));
            };

        public static void OnModelRemoved<T>(this IAggregateRepository<T> repository, Action<IModel<T>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                if (args.ChangeType != RepositoryChangeType.ModelRemoved) return;
                action.Invoke((IModel<T>)args.Repository.GetModel(args.ModelId));
            };

        public static void OnModelUpdated<T>(this IAggregateRepository<T> repository, Action<IModel<T>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                if (args.ChangeType != RepositoryChangeType.ModelUpdated) return;
                action.Invoke((IModel<T>)args.Repository.GetModel(args.ModelId));
            };

        public static void OnModelChanged<T>(this IRepository<T> repository, Action<IModel<T>> action)
            => repository.RepositoryChanged += (sender, args) =>
            {
                var model = (IModel<T>)args.Repository.GetModel(args.ModelId);
                action.Invoke(model);
            };

        public static void OnModelsChanged<T>(this IRepository<T> repository, Action<IReadOnlyList<IModel<T>>> action)
            => repository.RepositoryChanged += (sender, args) =>
                action.Invoke((IReadOnlyList<IModel<T>>)args.Repository.GetModels());

        public static bool Update<T>(this IModel<T> model, Func<T, T> updateFunc)
            => model.Repository.Update(model.Id, updateFunc);

        public static bool Update<T>(this ISingletonRepository<T> repo, Func<T, T> updateFunc)
            => repo.Model.Update(updateFunc);

        public static string ToDebugString(this IModel model)
            => model == null ? "null" : $"{model.Id} {model.Value}";

        public static string GetTypeName(this object x)
            => x?.GetType().Name;

        public static IReadOnlyDictionary<Guid, object> GetModelDictionary(this IRepository r)
            => r.GetModels().ToDictionary(m => m.Id, m => m.Value);

        public static IModel<T> Add<T>(this IRepository<T> repo, T value)
            => repo.Add(Guid.NewGuid(), value);
    }
}   