namespace Neleus.DependencyInjection.Extensions
{
    /// <summary>
    /// Provides instances of registered services by name
    /// </summary>
    /// <typeparam name="TKey"></typeparam>

    /// <typeparam name="TService"></typeparam>
    public interface IServiceByKeyFactory<in TKey, out TService>
    {
        /// <summary>
        /// Provides instance of registered service by name
        /// </summary>
        TService GetByKey(TKey key);
    }
}