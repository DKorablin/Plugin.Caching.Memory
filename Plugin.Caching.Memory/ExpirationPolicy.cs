using System;

namespace Plugin.Caching.Memory
{
	internal struct ExpirationPolicy
	{
		public readonly TimeSpan? _slidingExpiration;
		public readonly DateTimeOffset? _absoluteExpiration;
		public ExpirationPolicy(TimeSpan? slidingExpiration, DateTimeOffset? absoluteExpiration)
		{
			this._slidingExpiration = slidingExpiration;
			this._absoluteExpiration = absoluteExpiration;
		}
		public override Int32 GetHashCode()
			=> (this._slidingExpiration.HasValue ? this._slidingExpiration.Value.GetHashCode() : 0)
				^ (this._absoluteExpiration.HasValue ? this._absoluteExpiration.Value.GetHashCode() : 0);
	}
}