namespace MonitoringServer.Models
{
    internal class Server
    {
        public int ServerId { get;} // номер сервера
        public int CPUtemperature { get; private set; }// температура центрального процессора: 20 - 90 °C
        public int CPUclockSpeed { get; private set; } // тактовая частота центрального процессора:  1 - 5 GHz
        public int GPUtemperature { get; private set; } // температура графического процессора 20 - 90 °C
        public bool IsEnabled { get; private set; } // работает ли сервер

        private readonly Random _randomizer = new(); // генератор случайных чисел для изменения значений показателей сервера
        public Server(int serverId)
        {
            this.ServerId = serverId;
            CPUtemperature = 20;
            CPUclockSpeed = 1;
            GPUtemperature = 20;
        }
        private void updateIndicator() // имитация изменения значений серверных показателей
        {
            CPUtemperature = _randomizer.Next(20,90);
            CPUclockSpeed = _randomizer.Next(1, 5);
            GPUtemperature = _randomizer.Next(20, 90);
        }
        public async void serverWork() // включение серверов и имитация их работы
        {
            try // используем try catch, так как функция будет работать в фоновом режиме
            {
                IsEnabled = true;

                while (IsEnabled)
                {
                    updateIndicator();
                    await Task.Delay(10000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ServerId} ({DateTime.UtcNow}) {ex.Message}");
            }
            finally { IsEnabled = false; }
        }
    }
}
