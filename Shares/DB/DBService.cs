using MongoDB.Driver;
using DotNetEnv;
using System.Data.Common;
using System.Runtime.CompilerServices;
using MongoDB.Driver.Core.Configuration;
using System.Xml.Linq;

namespace Shares.DB
{
    public static class DBService
    {
        private static IMongoDatabase? _database;

        public static void Initialize()
        {
            //var envPath = Path.GetFullPath(@"..\Shares\.env");
            //var envPath = Path.Combine(AppContext.BaseDirectory, "Shares", ".env");
            //Console.WriteLine($"test :,{envPath}");
            //Env.Load(Path.GetFullPath(envPath));
            var isDocker = Environment.GetEnvironmentVariable("DOCKER_ON") == "true";
            var connectionString = "";

            if (isDocker)
            {
                connectionString = Environment.GetEnvironmentVariable("DB_DOCKER_CONN"); //?? "mongodb://localhost:27017";
            }
            else
            {
                var envPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"../../../.env"));
                Env.Load(envPath);
                connectionString = Environment.GetEnvironmentVariable("DB_CONN"); //?? "mongodb://localhost:27017";
            }
            var databaseName = Environment.GetEnvironmentVariable("DB_NAME"); //?? "mmorpg";
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            Console.WriteLine($"c {connectionString}");
            Console.WriteLine($"db {databaseName}");
        }

        public static IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            if (_database == null)
                throw new InvalidOperationException("DBService is not initialized.");
            return _database.GetCollection<T>(collectionName);
        }
    }
}