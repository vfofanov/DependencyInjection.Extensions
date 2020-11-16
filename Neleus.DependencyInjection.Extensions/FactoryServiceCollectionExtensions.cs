using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Neleus.DependencyInjection.Extensions
{
    public static class FactoryServiceCollectionExtensions
    {
        /// <summary>
        /// Entry point for name-based registrations. This method should be called in order to start building
        /// named registrations for <typeparamref name="TService"/>"/>
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="keyComparer"></param>
        /// <returns><see cref="ServicesByKeyBuilder{TKey,TService}"/> which is used to build multiple named registrations</returns>
        public static ServicesByKeyBuilder<string, TService> AddByName<TService>(this IServiceCollection services, IEqualityComparer<string> keyComparer=null)
        {
            return AddByKey<string,TService>(services, keyComparer);
        }

        /// <summary>
        /// Entry point for name-based registrations. This method should be called in order to start building
        /// named registrations for <typeparamref name="TService"/>"/>
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="keyComparer"></param>
        /// <returns><see cref="ServicesByKeyBuilder{TKey,TService}"/> which is used to build multiple named registrations</returns>
        public static ServicesByKeyBuilder<TKey, TService> AddByKey<TKey, TService>(this IServiceCollection services, IEqualityComparer<TKey> keyComparer=null)
        {
            return new ServicesByKeyBuilder<TKey,TService>(services, keyComparer);
        }

        /// <summary>
        /// Provides instances of named registration. It is intended to be used in factory registrations, see example.
        /// </summary>
        /// <code>
        /// _container.AddTransient&lt;ClientA&gt;(s =&gt; new ClientA(s.GetByName&lt;IEnumerable&lt;int&gt;&gt;(&quot;list&quot;)));
        /// _container.AddTransient&lt;ClientB&gt;(s =&gt; new ClientB(s.GetByName&lt;IEnumerable&lt;int&gt;&gt;(&quot;hashSet&quot;)));
        /// </code>
        /// <returns></returns>
        public static TService GetServiceByName<TService>(this IServiceProvider provider, string name)
        {
            return provider.GetServiceByKey<string, TService>(name);
        }

        /// <summary>
        /// Provides instances of named registration. It is intended to be used in factory registrations, see example.
        /// </summary>
        /// <code>
        /// _container.AddTransient&lt;ClientA&gt;(s =&gt; new ClientA(s.GetByName&lt;IEnumerable&lt;int&gt;&gt;(&quot;list&quot;)));
        /// _container.AddTransient&lt;ClientB&gt;(s =&gt; new ClientB(s.GetByName&lt;IEnumerable&lt;int&gt;&gt;(&quot;hashSet&quot;)));
        /// </code>
        /// <returns></returns>
        public static TService GetServiceByKey<TKey, TService>(this IServiceProvider provider, TKey key)
        {
            var factory = provider.GetService<IServiceByKeyFactory<TKey, TService>>();
            if (factory == null)
            {
                throw new InvalidOperationException($"The factory {typeof(IServiceByKeyFactory<TKey, TService>)} is not registered. Please use {nameof(FactoryServiceCollectionExtensions)}.{nameof(AddByKey)}() to register names.");
            }
            return factory.GetByKey(key);
        }
    }
}