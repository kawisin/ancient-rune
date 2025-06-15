using Instance_Service.DB;
using System.Diagnostics;
using MongoDB.Driver;
using Shares.Helper;

namespace Instance_Service.Services
{
    public class CleanupService
    {
        public static void CleanupBeforeShutdown()
        {
            //Shares.Logger.Info("Shutting down, cleaning up...");

            const String ExeName = "MMORPGServer";

            // ✅ 1. Kill processes
            foreach (var process in Process.GetProcessesByName(ExeName))
            {
                try
                {
                    process.Kill();
                    //Shares.Logger.Info($"Killed process: {process.Id}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to kill process: {ex.Message}");
                }
            }

            // ✅ 2. Delete DB instances
            try
            {
                var collection = DBHelper.GetCollection(); // ต้อง return IMongoCollection<InstanceStatusResponse>
                var result = collection.DeleteMany(_ => true); // ลบทั้งหมด
                Logger.Info($"Deleted {result.DeletedCount} instance records from DB.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error deleting instances from DB: {ex.Message}");
            }
        }
    }
}
