using System;
using System.ComponentModel;

namespace Domo
{
    public class Model<TValue> : IModel<TValue>
    {
        public Model(Guid id, IRepository<TValue> repo)
            => (Id, Repository) = (id, repo);

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void Dispose()
        {
            PropertyChanged = null;
        }

        public Guid Id { get; }

        public TValue Value
        {
            get => Repository.Read(Id);
            set
            {
                if (Repository.Update(Id, _ => value))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                }
            }
        }

        object IModel.Value
        {
            get => Value;
            set => Value = (TValue)value;
        }

        public Type ValueType 
            => typeof(TValue);

        public IRepository<TValue> Repository { get; }

        IRepository IModel.Repository 
            => Repository;
    }
}