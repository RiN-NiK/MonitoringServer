namespace MonitoringServer.Models
{
    internal class MainServer
    {
        public Dictionary<int, Server> Servers { get; } = new(); // достаточно обычного dictionary, т.к. нет необходимости в долгом сеансе
                                                                 // из-за специфики приложения нет необходимости использования асинхронности при подключении клиентов                                                       
        public void addServer(int id, Server server)
        {
            Servers.Add(id, server);
        }
        public void removeServer(int id)
        {
            Servers.Remove(id);
        }

    }
    
}
