/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
