using System.Net.Sockets;
using System.Net;
using Shares.Helper;

namespace Instance_Service.Utils
{
    public class Helper
    {
        public static Task<int?> FindAvailablePortInMemory(int startPort, int endPort)
        {
            for (int port = startPort; port <= endPort; port++)
            {
                Logger.Info($"Checking port: {port}");
                if (IsPortAvailable(port))
                {
                    Logger.Info($"Available port found: {port}");
                    return Task.FromResult<int?>(port);
                }
            }
            Logger.Info("No available port found.");
            return Task.FromResult<int?>(null);
        }

        private static bool IsPortAvailable(int port)
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                listener.Stop();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
