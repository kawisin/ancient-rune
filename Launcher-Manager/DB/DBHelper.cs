using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Launcher_Manager.DB
{
    public static class DBHelper
    {
        private static readonly IMongoCollection<InstanceStatusResponse> _collection;

        static DBHelper()
        {
            Shares.DB.DBService.Initialize();
            _collection = Shares.DB.DBService.GetCollection<InstanceStatusResponse>("instances");
        }
        
        public static async Task SaveInstanceAsync(InstanceStatusResponse instance)
        {
            await _collection.InsertOneAsync(instance);
        }

        public static IMongoCollection<InstanceStatusResponse> GetCollection()
        {
            return _collection;
        }

        public static async Task<List<InstanceStatusResponse>> GetAllInstancesAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
    }
}
