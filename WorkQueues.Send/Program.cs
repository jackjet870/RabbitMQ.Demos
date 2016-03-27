using System;
using RabbitMQ.Client;
using System.Text;
class Send
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            //可以使用ExchangeType.Direct这个常量    
            channel.ExchangeDeclare(exchange: "logs", type: "direct");

            channel.QueueDeclare(queue: "workqueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: "workqueue",
                              exchange: "logs",
                              routingKey: "workqueue");

            var props = channel.CreateBasicProperties();
            props.Persistent = true;

            for (int i = 0; i < 10; i++)
            {
                var message = $"Message.{i}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "logs",
                                     routingKey: "workqueue",
                                     basicProperties: props,
                                     body: body);

                Console.WriteLine("[x] Sent {0}", message);
            }
        }
        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();
    }
}
