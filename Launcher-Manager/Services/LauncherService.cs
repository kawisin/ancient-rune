using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Launcher_Manager.DB;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shares.Data;
using Shares.Helper;

namespace Launcher_Manager
{
    public static class LauncherService
    {
        public static void ListenToLaunchQueue()
        {
            bool connected = false;
            while (!connected)
            {
                try
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = "localhost", // หรือ "127.0.0.1"
                        Port = 5672, // พอร์ตของ RabbitMQ AMQP
                        UserName = "guest",
                        Password = "guest"
                    };
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(queue: "instance.launch",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var command = JsonSerializer.Deserialize<InstanceCommand>(message);

                        Console.WriteLine(
                            $" [x] Received {command.Action} message for instance: {command.InstanceName}");

                        if (command.Action == "Launch")
                        {
                            await LaunchInstance(command);
                        }
                        else if (command.Action == "Destroy")
                        {
                            await CloseInstance(command);
                        }
                    };

                    channel.BasicConsume(queue: "instance.launch", autoAck: true, consumer: consumer);

                    Console.WriteLine(" [*] Waiting for launch messages.");
                    Console.ReadLine();
                }
                catch
                {
                    Console.WriteLine("🔄 Waiting for RabbitMQ to be available...");
                    Thread.Sleep(5000); // รอ 5 วินาทีแล้วลองใหม่
                }
            }
        }

        public static async Task LaunchInstance(InstanceCommand command)
        {
            string exePath = @"A:\MMORPG\MMORPG\Binaries\WindowsServer\MMORPGServer.exe";
            var config = ConfigLoader.LoadInstanceConfig(command.InstanceName);
            string args = string.Join(" ", new[]
            {
                $"-ConsoleTitle=\"MMORPG Server | Port {command.Port}\"",
                "-log",
                $"-port={command.Port}",
                $"-instance_id={command.InstanceId}",
                $"-map={config.Enter.Map}",
                $"-x={config.Enter.X}",
                $"-y={config.Enter.Y}"
            });

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                }
            };

            process.Start();
            
            var status = new InstanceStatusResponse
            {
                InstanceId = command.InstanceId,
                InstanceName = command.InstanceName,
                //IsRunning = true,
                PlayerCount = 0,
                Port = command.Port,
                ProcessId = process.Id,
                StartedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            //INSTANCES[instanceId.ToString()] = (status, process);

            Logger.Info($"Create Instance : Name = {status.InstanceName} | InstanceId = {status.InstanceId} | Port = {status.Port}");

            await DBHelper.SaveInstanceAsync(status);
        }

        public static async Task CloseInstance(InstanceCommand command)
        {
            
        }
    }
}