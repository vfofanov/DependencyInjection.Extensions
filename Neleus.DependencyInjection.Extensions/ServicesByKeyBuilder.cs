using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Neleus.DependencyInjection.Extensions
{
    /// <summary>
    /// Provides easy fluent methods for building named registrations of the same interface
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TService"></typeparam>
    public class ServicesByKeyBuilder<TKey, TService>
    {
        private readonly IServiceCollection _services;

        private readonly IDictionary<TKey, Type> _registrations;

        internal ServicesByKeyBuilder(IServiceCollection services, IEqualityComparer<TKey> keyComparer = null)
        {
            _services = services;
            _registrations = new Dictionary<TKey, Type>(keyComparer ?? EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Maps name to corresponding implementation.
        /// Note that this implementation has to be also registered in IoC container so
        /// that <see cref="IServiceByKeyFactory{TKey,TService}"/> is be able to resolve it.
        /// </summary>
        public ServicesByKeyBuilder<TKey, TService> Add(TKey key, Type implemtnationType)
        {
            //TODO: Check implementationType inherits TService
            _registrations.Add(key, implemtnationType);
            return this;
        }

        /// <summary>
        /// Generic version of <see cref="Add"/>
        /// </summary>
        public ServicesByKeyBuilder<TKey,TService> Add<TImplementation>(TKey key)
            where TImplementation : TService
        {
            return Add(key, typeof(TImplementation));
        }

        /// <summary>
        /// Adds <see cref="IServiceByKeyFactory{TKey,TService}"/> to IoC container together with all registered implementations
        /// so it can be consumed by client code later. Note that each implementation has to be also registered in IoC container so
        /// <see cref="IServiceByKeyFactory{TKey,TService}"/> is be able to resolve it from the container.
        /// </summary>
        public void Build()
        {
            var registrations = _registrations;
            //Registrations are shared across all instances
            _services.AddTransient<IServiceByKeyFactory<TKey,TService>>(s => new ServiceByKeyFactory<TKey, TService>(s, registrations));
        }
    }
}