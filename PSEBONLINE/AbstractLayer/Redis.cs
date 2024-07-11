using StackExchange.Redis;
using System;
using static System.Web.Razor.Parser.SyntaxConstants;

using System.Net.NetworkInformation;
using System.Configuration;

namespace PSEBONLINE.AbstractLayer
{
	public class RedisManager : IDisposable
	{

		private readonly ConnectionMultiplexer _redis;
		private readonly IDatabase _db;

		public RedisManager()
		{
			string connectionString = ConfigurationManager.ConnectionStrings["RedisConnection"].ConnectionString;
			_redis = ConnectionMultiplexer.Connect(connectionString);
			_db = _redis.GetDatabase();
		}

		public string GetStringValue(string key)
		{
			return _db.StringGet(key);
		}

		public void SetStringValue(string key, string value, TimeSpan expiration)
		{
			_db.StringSet(key, value, expiration);
		}

		public bool SetKeyExpiration(string key, TimeSpan expiration)
		{
			return _db.KeyExpire(key, expiration);
		}

		public bool ResetKeyExpiration(string key)
		{
			return _db.KeyPersist(key);
		}

		public void Dispose()
		{
			_redis?.Dispose();
		}
	}

}

