using System;
using System.ComponentModel.Design;

namespace Domo
{
    public class ServiceRegistry : IServiceContainer
    {
        private readonly ServiceContainer _container = new ServiceContainer();

        public void AddService(Type serviceType, ServiceCreatorCallback callback)
            => ((IServiceContainer)_container).AddService(serviceType, callback);

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
            => ((IServiceContainer)_container).AddService(serviceType, callback, promote);

        public void AddService(Type serviceType, object serviceInstance)
            => ((IServiceContainer)_container).AddService(serviceType, serviceInstance);

        public void AddService(Type serviceType, object serviceInstance, bool promote)
            => ((IServiceContainer)_container).AddService(serviceType, serviceInstance, promote);

        public object GetService(Type serviceType)
            => ((IServiceProvider)_container).GetService(serviceType);

        public void RemoveService(Type serviceType)
            => ((IServiceContainer)_container).RemoveService(serviceType);

        public void RemoveService(Type serviceType, bool promote)
            => ((IServiceContainer)_container).RemoveService(serviceType, promote);
    }
}