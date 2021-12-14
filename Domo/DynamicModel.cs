using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace Domo
{
    /// <summary>
    /// A Dynamic Model provides getters and setters in a dynamic context
    /// for a model. This can be useful for data binding, or to reduce
    /// boiler plate code. 
    /// </summary>
    public class DynamicModel : DynamicObject, INotifyPropertyChanged
    {
        public IModel Model { get; }

        private readonly MethodInfo _cloneMethod;
        private readonly IDictionary<string, PropertyInfo> _props 
            = new Dictionary<string, PropertyInfo>();

        public DynamicModel(IModel model)
        {
            Model = model;
            _cloneMethod = model.ValueType.GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in model.ValueType.GetProperties())
            {
                _props.Add(prop.Name, prop);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => Model.PropertyChanged += value;
            remove => Model.PropertyChanged -= value;
        }

        public void SetProperty(string name, object value)
        {
            var newState = _cloneMethod.Invoke(Model.Value, Array.Empty<object>());
            var prop = _props[name];
            prop.SetValue(newState, value);
            Model.Value = newState;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _props[binder.Name].GetValue(Model.Value);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetProperty(binder.Name, value);
            return true;
        }
    }
}
