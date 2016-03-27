//使用方法：先开多个等着接收信息
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;


class Recevie
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            //var queueName = channel.QueueDeclare().QueueName;//生成随机队列
            channel.QueueDeclare(queue: "workqueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine("[*] Waiting for messages.");

            //此场景，可以同时接收5条消息，但事件是串行的一次只能处理一个。
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                System.Threading.Thread.Sleep(1000);
                var message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine("[x] Received {0}", message);
                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "workqueue", noAck: false,
                                 consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();//挂起线程，否则直接退出了。
        }
    }
}
