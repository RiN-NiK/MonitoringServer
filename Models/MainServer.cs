using System.Collections.Concurrent;

namespace MonitoringServer.Models
{
    internal class MainServer
    {
        public ConcurrentDictionary<int, Server> Servers { get; } = new();                                                       
        public void addServer(int id, Server server)
        {
            Servers.TryAdd(id, server);
        }
    }
    
}
