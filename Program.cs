using MonitoringServer.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MonitoringServer
{ 
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var ipAddress = IPAddress.Parse("127.0.0.1"); // local host
            var port = 8888;

            var mainServer = new MainServer();
            var server_1 = new Server(1);
            var server_2 = new Server(2);

            mainServer.addServer(server_1.ServerId, server_1);
            mainServer.addServer(server_2.ServerId, server_2);

            foreach (var server in mainServer.Servers.Values) // запуск серверов в фоновом режиме (имитация работы серверов)
            {
                _ = Task.Run(server.serverWork); 
            }
            Console.WriteLine("Сервера начали свою работу");

            var endPoint = new IPEndPoint(ipAddress, port);

            TcpListener tcpListener = new(endPoint); // расположение мониторинг сервера

            try
            {
                tcpListener.Start(); // запуск мониторинг сервера
                Console.WriteLine($"Мониторинг сервер начал работу {ipAddress.ToString()}:{port} ({DateTime.UtcNow})\n");

                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync(); // мониторинг сервер ожидает подключения клиента

                    try
                    {
                        Console.WriteLine($"Подключился клиент: ({DateTime.UtcNow}) {tcpClient.Client.RemoteEndPoint}");

                        StringBuilder message = new StringBuilder(); // используется для формирования ответа сервера
                        bool isStopped = false; // переменная, которая позволяет клиенту отключиться 

                        using (NetworkStream networkStream = tcpClient.GetStream()) // установка соединения клиента с мониторинг сервером для обмена байтами
                        {
                            while (!isStopped)
                            {
                                byte[] buffer = new byte[1024];

                                int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead); // расшифровываем массив байт от клиента

                                switch (receivedMessage) // ядро программы (функционал сервера): формируется ответ сервера на запрос, который записывается в stringBuilder
                                {
                                    case "Show information about all servers": // вывод полной информации о всех серверах

                                        message.AppendLine($"{DateTime.UtcNow}{Environment.NewLine}");

                                        foreach (var server in mainServer.Servers.Values)
                                        {
                                           message.AppendLine($"ID сервера: {server.ServerId}{Environment.NewLine}" + // Environment.NewLine позволяет корректно отобразить переход на новую строку
                                                               $"температура CPU - {server.CPUtemperature} °C{Environment.NewLine}" +
                                                               $"температура GPU - {server.GPUtemperature} °C{Environment.NewLine}" +
                                                               $"тактовая частота CPU - {server.CPUclockSpeed} GHz{Environment.NewLine}" +
                                                               $"рабочее состояние сервера - {server.IsEnabled}{Environment.NewLine}" +
                                                               $"-----------------------------------------{Environment.NewLine}");
                                        }
                                            break;
                                 
                                    case "Exit": // завершить соединение клиента с мониторинг сервером (следующий клиент может подключиться)
                                        isStopped = true;
                                        message.AppendLine("Приложение отключено.");
                                        break;
                                }

                                byte[] responseByte = Encoding.UTF8.GetBytes(message.ToString()); // преобразование ответа сервера в массив байт с дальнейшей отправкой
                                await networkStream.WriteAsync(responseByte);
                                message.Clear(); // зачищаем переменную, чтобы данные не перемешались с предыдущими
                            }

                            Console.WriteLine($"Клиент был отключен: ({DateTime.UtcNow}) {tcpClient.Client.RemoteEndPoint}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Произошла ошибка: ({DateTime.UtcNow}) {ex.Message}");
                    }
                    finally
                    {
                        tcpClient?.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: ({DateTime.UtcNow}) {ex.Message}");
            }
            finally
            {
                tcpListener.Stop();
                Console.WriteLine($"Сервер закончил работу ({DateTime.UtcNow}) {ipAddress.ToString()}:{port}");
            }

        }
    }
}
