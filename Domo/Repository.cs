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

        IModel IRepository.GetModel(Guid modelId)
            => GetModel(modelId);

        object IRepository.GetValue(Guid modelId)
            => GetModel(modelId);

        public bool Update(Guid modelId, Func<T, T> updateFunc)
        {
            var model = GetModel(modelId);
            var oldVal = model.Value;
            var newVal = updateFunc(oldVal);
            if (oldVal.Equals(newVal))
            {
                // When there is no difference in the values there is no need to trigger a change
                return false;
            }
            var args = new RepositoryChangeArgs
            {
                ChangeType = RepositoryChangeType.ModelUpdated,
                ModelId = modelId,
                NewValue = newVal,
                OldValue = oldVal
            };
            if (!Validate(newVal))
            {
                args.ChangeType = RepositoryChangeType.ModelInvalid;
                RepositoryChanged?.Invoke(this, args); 
                return false;
            }
            _dict[modelId] = (newVal, _dict[modelId].Item2);
            model.TriggerChangeNotification();
            RepositoryChanged?.Invoke(this, args);
            return true;
        }

        public bool Validate(T state)
            => ((IRepository)this).Validate(state);

        public bool Validate(object state)
            => _validatorFunc(state);

        public IModel<T> Add(T state)
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

        public IModel<T> GetModel(Guid modelId)
            => _dict[modelId].Item2;

        public T GetValue(Guid modelId)
            => _dict[modelId].Item1;

        public bool Update(Guid modelId, Func<object, object> updateFunc)
            => Update(modelId, (T x) => (T)updateFunc(x));

        public IModel Add(object state)
            => Add((T)state);

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
            Model = Add(value);
        }

        public override bool IsSingleton => true;

        public IModel<T> Model { get; }
    } 
}