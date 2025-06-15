using MongoDB.Driver;
using System.Threading.Tasks;

namespace Instance_Service.DB
{
    public static class DBHelper
    {
        private static readonly IMongoCollection<InstanceStatusResponse> _collection;

        static DBHelper()
        {
            Shares.DB.DBService.Initialize();
            _collection = Shares.DB.DBService.GetCollection<InstanceStatusResponse>("instances");
            //var client = new MongoClient("mongodb://localhost:27017");
            //var database = client.GetDatabase("mmorpg");
            //_collection = database.GetCollection<InstanceStatusResponse>("instances");
        }

        public static async Task SaveInstanceAsync(InstanceStatusResponse instance)
        {
            await _collection.InsertOneAsync(instance);
        }

        public static async Task<UpdateResult> UpdateOneAsync(
            FilterDefinition<InstanceStatusResponse> filter,
            UpdateDefinition<InstanceStatusResponse> update)
        {
            return await _collection.UpdateOneAsync(filter, update);
        }

        public static async Task<InstanceStatusResponse?> GetInstanceByIdAsync(int instanceId)
        {
            var filter = Builders<InstanceStatusResponse>.Filter.Eq(x => x.InstanceId, instanceId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public static async Task<InstanceStatusResponse?> GetLastStartedInstanceAsync()
        {
            return await _collection
                .Find(_ => true)
                .SortByDescending(x => x.StartedAt)
                .Limit(1)
                .FirstOrDefaultAsync();
        }

        public static IMongoCollection<InstanceStatusResponse> GetCollection()
        {
            return _collection;
        }
    }
}
