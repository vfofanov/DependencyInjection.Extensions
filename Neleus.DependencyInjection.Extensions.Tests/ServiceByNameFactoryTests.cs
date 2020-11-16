using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neleus.DependencyInjection.Extensions.Tests
{
    [TestClass]
    public class ServiceByNameFactoryTests
    {
        private ServiceCollection _container;

        [TestInitialize]
        public void Init()
        {
            _container = new ServiceCollection();
        }

        [TestMethod]
        public void ServiceByNameFactory_GetByName_ResolvesToAppropriateTypes()
        {
            _container.AddTransient<List<int>>();
            _container.AddTransient<HashSet<int>>();

            _container.AddByName<IEnumerable<int>>()
                .Add<List<int>>("list")
                .Add<HashSet<int>>("hashSet")
                .Build();

            var serviceProvider = _container.BuildServiceProvider();

            // direct resolution by calling GetByName
            var list = serviceProvider.GetService<IServiceByKeyFactory<string, IEnumerable<int>>>()
                .GetByKey("list");
            var hashSet = serviceProvider.GetService<IServiceByKeyFactory<string, IEnumerable<int>>>()
                .GetByKey("hashSet");

            Assert.AreEqual(typeof(List<int>), list.GetType());
            Assert.AreEqual(typeof(HashSet<int>), hashSet.GetType());
        }

        [TestMethod]
        public void ResolutionLogicInContainerRegistration_ResolvesToAppropriateTypes()
        {
            _container.AddTransient<List<int>>();
            _container.AddTransient<HashSet<int>>();

            _container.AddByKey<string, IEnumerable<int>>()
                .Add<List<int>>("list")
                .Add<HashSet<int>>("hashSet")
                .Build();

            //The named services are resolved by IoC container and the client is abstracted from the
            //dependency resolution
            _container.AddTransient<ClientA>(s => new ClientA(s.GetServiceByKey<string, IEnumerable<int>>("list")));
            _container.AddTransient<ClientB>(s => new ClientB(s.GetServiceByKey<string, IEnumerable<int>>("hashSet")));

            var serviceProvider = _container.BuildServiceProvider();

            var a = serviceProvider.GetService<ClientA>();
            var b = serviceProvider.GetService<ClientB>();

            Assert.AreEqual(typeof(List<int>), a.Dependency.GetType());
            Assert.AreEqual(typeof(HashSet<int>), b.Dependency.GetType());
        }

        [TestMethod]
        public void ResolutionLogicInClient_ResolvesToAppropriateTypes()
        {
            _container.AddTransient<List<int>>();
            _container.AddTransient<HashSet<int>>();

            _container.AddByKey<string,IEnumerable<int>>()
                .Add<List<int>>("list")
                .Add<HashSet<int>>("hashSet")
                .Build();

            //The named service is resolved by the client itself
            _container.AddTransient<ClientZ>();

            var serviceProvider = _container.BuildServiceProvider();

            var z = serviceProvider.GetService<ClientZ>();

            Assert.AreEqual(typeof(HashSet<int>), z.Dependency.GetType());
        }

        [TestMethod]
        public void ServiceByNameFactory_GetByName_CaseInsensitive()
        {
            _container.AddTransient<List<int>>();
            _container.AddTransient<HashSet<int>>();

            _container.AddByKey<string,IEnumerable<int>>(StringComparer.OrdinalIgnoreCase)
                .Add<List<int>>("list")
                .Add<HashSet<int>>("hashSet")
                .Build();

            var serviceProvider = _container.BuildServiceProvider();

            // direct resolution by calling GetByName
            var list = serviceProvider.GetService<IServiceByKeyFactory<string,IEnumerable<int>>>()
                .GetByKey("LiSt");
            var hashSet = serviceProvider.GetService<IServiceByKeyFactory<string,IEnumerable<int>>>()
                .GetByKey("HASHSET");

            Assert.AreEqual(typeof(List<int>), list.GetType());
            Assert.AreEqual(typeof(HashSet<int>), hashSet.GetType());
        }
    }
}
