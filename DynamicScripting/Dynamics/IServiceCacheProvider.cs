using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicScripting.Dynamics
{
    public interface IServiceCacheProvider
    {
        TService Get<TService>(Expression<Func<TService, bool>> expression) where TService : class;

        IEnumerable<TService> GetMany<TService>(Expression<Func<TService, bool>> expression) where TService : class;

        void Clear<TService>(Expression<Func<TService, bool>> expression = null) where TService : class;

        void Clear();
    }

    public class ServiceCacheProvider
        : IServiceCacheProvider
    {
        private readonly Func<IQueryProvider> queryProviderFunc;
        private readonly ConcurrentDictionary<Type, dynamic> cacheObjects;

        public ServiceCacheProvider(Func<IQueryProvider> queryProviderFunc)
        {
            this.queryProviderFunc = queryProviderFunc;
            this.cacheObjects = new ConcurrentDictionary<Type, dynamic>();
        }

        public TService Get<TService>(Expression<Func<TService, bool>> expression)
             where TService : class
        {
            var cache = this.GetCache<TService>();
            var service = cache.FirstOrDefault(expression.Compile());

            if (service == null)
            {
                service = this.queryProviderFunc().CreateQuery<TService>(expression).FirstOrDefault();
                cache.Add(service);
            }
                
            return service;
        }

        public IEnumerable<TService> GetMany<TService>(Expression<Func<TService, bool>> expression)
             where TService : class
        {
            var cache = this.GetCache<TService>();
            var result = cache.Where(expression.Compile());

            if (!result.Any())
            {
                result = this.queryProviderFunc().CreateQuery<TService>(expression).ToList();
                cache.AddRange(result);
            }

            return cache;
        }

        public void Clear<TService>(Expression<Func<TService, bool>> expression = null) where TService : class
        {
            var cache = this.GetCache<TService>();

            if (expression == null)
            {
                cache.Clear();
            }
            else
            {
                var exp = expression.Compile();
                cache.RemoveAll(service => exp(service));
            }
        }

        public void Clear()
        {
            this.cacheObjects.Clear();
        }

        private List<TService> GetCache<TService>()
        {
            Type type = typeof(TService);
            return this.cacheObjects.GetOrAdd(type, new List<TService>());
        }
    }
}
