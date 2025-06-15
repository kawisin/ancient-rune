using System.Text;
using RabbitMQ.Client;
using System.Text.Json;


   public class Command
   {
       public static async Task<string> SendInstanceCommand(string instanceName, InstanceCommand action)
       {
           var factory = new ConnectionFactory() { HostName = "rabbitmq" };
           using var connection = factory.CreateConnection();
           using var channel = connection.CreateModel();
           //var command = new InstanceCommand
           //{
           //    Action = action,
           //    InstanceName = instanceName
           //};
           var message = JsonSerializer.Serialize(action);
           var body = Encoding.UTF8.GetBytes(message);
           channel.QueueDeclare(queue: "instance.launch",
               durable: true,
               exclusive: false,
               autoDelete: false,
               arguments: null);
           var properties = channel.CreateBasicProperties();
           properties.Persistent = true;
           channel.BasicPublish(
               exchange: "",
               routingKey: "instance.launch",
               basicProperties: properties,
               body: body
           );
           Console.WriteLine($" [x] Sent {action} command for instance: {instanceName}");
           return $"{action} command sent for {instanceName}";
       }
   }
