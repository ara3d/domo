using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Domo
{
    public abstract class Repository<T> : IRepository<T>
    {
        protected Repository(Guid id, Version version, Func<object, bool> validatorFunc = null)
        {
            RepositoryId = id;
            Version = version;
            _validatorFunc = validatorFunc;
        }

        public Guid RepositoryId { get; }
        public Version Version { get; }

        public Type ValueType
            => typeof(T);

        private Func<object, bool> _validatorFunc;
        private IDictionary<Guid, (T, Model<T>)> _dict = new Dictionary<Guid, (T, Model<T>)>();

        public void Dispose()
        {
            foreach (var v in _dict.Values)
                v.Item2.Dispose();
            _dict = null;
            _validatorFunc = null;
        }

        object IRepository.Read(Guid modelId)
            => Read(modelId);

        public bool Update(Guid modelId, Func<T, T> updateFunc)
        {
            var oldVal = Read(modelId);
            var newVal = updateFunc(oldVal);
            var args = new RepositoryChangeArgs
            {
                ChangeType = RepositoryChangedEvent.ModelUpdated,
                ModelId = modelId,
                NewValue = newVal,
                OldValue = oldVal
            };
            if (!Validate(newVal))
            {
                args.ChangeType = RepositoryChangedEvent.ModelInvalid;
                RepositoryChanged?.Invoke(this, args); 
                return false;
            }
            _dict[modelId] = (newVal, _dict[modelId].Item2);
            RepositoryChanged?.Invoke(this, args);
            return true;
        }

        public bool Validate(T state)
            => ((IRepository)this).Validate(state);

        public bool Validate(object state)
            => _validatorFunc(state);

        public IModel<T> Create(T state)
        {
            if (IsSingleton && _dict.Count != 0)
                throw new Exception("Singleton repository cannot have more than one model");
            var id = Guid.NewGuid();
            var model = new Model<T>(id, this);
            _dict.Add(id, (state, model));
            return model;
        }

        public IReadOnlyList<IModel<T>> GetModels()
            => _dict.Values.Select(x => x.Item2).ToList();

        public event EventHandler<RepositoryChangeArgs> RepositoryChanged;

        public T Read(Guid modelId)
            => _dict[modelId].Item1;

        public bool Update(Guid modelId, Func<object, object> updateFunc)
            => Update(modelId, (T x) => (T)updateFunc(x));

        public IModel Create(object state)
            => Create((T)state);

        public virtual void Delete(Guid id)
        {
            _dict[id].Item2.Dispose();
            _dict.Remove(id);
        }

        public bool ModelExists(Guid id)
            => _dict.ContainsKey(id);

        IReadOnlyList<IModel> IRepository.GetModels()
            => GetModels();

        public abstract bool IsSingleton { get; }
    }

    public class AggregateRepository<T> : Repository<T>, IAggregateRepository<T>
    {
        public AggregateRepository(Guid id, Version version, Func<T, bool> validatorFunc = null) 
            : base(id, version, x => validatorFunc?.Invoke((T)x) ?? true)
        { }

        public override bool IsSingleton => false;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    public class SingletonRepository<T> : Repository<T>, ISingletonRepository<T>
    {
        public SingletonRepository(Guid id, Version version, T value, Func<T, bool> validatorFunc = null)
            : base(id, version, x => validatorFunc?.Invoke((T)x) ?? true)
        {
            Model = Create(value);
        }

        public override bool IsSingleton => true;

        public IModel<T> Model { get; }
    } 
}