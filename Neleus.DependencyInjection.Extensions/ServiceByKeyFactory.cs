using System;
using System.Collections.Generic;

namespace Neleus.DependencyInjection.Extensions
{
    internal class ServiceByKeyFactory<TKey, TService> : IServiceByKeyFactory<TKey, TService>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<TKey, Type> _registrations;

        public ServiceByKeyFactory(IServiceProvider serviceProvider, IDictionary<TKey, Type> registrations)
        {
            _serviceProvider = serviceProvider;
            _registrations = registrations;
        }

        public TService GetByKey(TKey name)
        {
            if (!_registrations.TryGetValue(name, out var implementationType))
            {
                throw new ArgumentException($"Service name '{name}' is not registered");
            }
            return (TService)_serviceProvider.GetService(implementationType);
        }
    }
}
