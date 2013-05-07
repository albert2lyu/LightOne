using System;
using System.Linq;
using MongoDB.Driver;

namespace Common.Data
{
	public class DatabaseFactory
	{
        public static IDatabase CreateDatabase(string connectionStringName = "*")
		{
			return new ConnectionDatabase(connectionStringName);
		}

        public static MongoDatabase CreateMongoDatabase(string host = "localhost") {
            var connectionString = string.Format("mongodb://{0}/?safe=true", host);
            var server = MongoServer.Create(connectionString);
            return server.GetDatabase("queen");
        }
	}
}
