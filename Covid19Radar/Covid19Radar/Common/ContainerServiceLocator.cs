using System;
using System.Collections.Generic;
using CommonServiceLocator;
using DryIoc;

namespace Covid19Radar.Common
{
    public class ContainerServiceLocator : ServiceLocatorImplBase
    {
        private readonly Container _container;

        internal IContainer CopyContainerWithRegistrations()
        {
            return _container.WithRegistrationsCopy();
        }

        public ContainerServiceLocator(Container container)
        {
            _container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (_container == null) throw new ObjectDisposedException("container");
            return _container.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (_container == null) throw new ObjectDisposedException("container");
            return _container.ResolveMany(serviceType);
        }
    }
}
