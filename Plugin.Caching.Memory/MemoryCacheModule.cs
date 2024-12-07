using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using SAL.Interface.Caching;

namespace Plugin.Caching.Memory
{
	public class MemoryCacheModule : ICacheModule, IDisposable
	{
		#region Fields
		private static readonly Object _policyLock = new Object();
		private static readonly Dictionary<ExpirationPolicy, CacheItemPolicy> _policy = new Dictionary<ExpirationPolicy, CacheItemPolicy>();

		/// <summary>Локер вызовов методов для получения данных из источника данных</summary>
		private readonly Dictionary<String, Object> _lockMethodCall = new Dictionary<String, Object>();
		#endregion Fields

		#region Properties
		public String Name { get; }

		internal MemoryCache Cache { get; }
		#endregion Properties

		#region Methods
		internal MemoryCacheModule(String name)
		{
			this.Name = name;
			this.Cache = new MemoryCache(this.Name);
		}

		public T Get<T>(String key) where T : class
			=> (T)this.Cache.Get(key, null);

		public T Get<T>(String key, Func<T> fallback, TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration) where T : class
		{
			T result = this.Get<T>(key);
			if(result == null && fallback != null)
			{
				if(!this._lockMethodCall.TryGetValue(key, out Object lockMethod))//Лок на метод получения данных
					lock(this._lockMethodCall.Keys)
						if(!this._lockMethodCall.TryGetValue(key, out lockMethod))
							this._lockMethodCall.Add(key, lockMethod = new Object());

				lock(lockMethod)
				{
					result = this.Get<T>(key);
					if(result == null)
					{
						result = fallback();
						this.Add(key, result, slidingExpiration, absoluteExpiration);
					}
				}
			}
			return result;
		}

		public void Add<T>(String key, T value, TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration)
		{
			CacheItemPolicy policy = MemoryCacheModule.GetPolicy(slidingExpiration, absoluteExpiration);
			this.Cache.Set(key, value, policy);
		}

		public void Remove(String key)
			=> this.Cache.Remove(key);

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.Cache.Dispose();
		}

		private static CacheItemPolicy GetPolicy(TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration)
		{
			if(slidingExpiration == null && absoluteExpiration == null)
				throw new ArgumentNullException($"{nameof(slidingExpiration)} or {nameof(absoluteExpiration)} are required arguments");

			ExpirationPolicy expPolicy = new ExpirationPolicy(slidingExpiration, absoluteExpiration);
			if(!_policy.TryGetValue(expPolicy, out CacheItemPolicy result))
			{
				lock(_policyLock)
				{
					if(!_policy.TryGetValue(expPolicy, out result))
					{
						result = new CacheItemPolicy();

						if(expPolicy._slidingExpiration.HasValue)
							result.SlidingExpiration = expPolicy._slidingExpiration.Value;
						else if(expPolicy._absoluteExpiration.HasValue)
							result.AbsoluteExpiration = expPolicy._absoluteExpiration.Value;
						//result.RemovedCallback = new CacheEntryRemovedCallback(RemovedFromCacheCallback);
						_policy.Add(expPolicy, result);
					}
				}
			}
			return result;
		}
		#endregion Methods
	}
}