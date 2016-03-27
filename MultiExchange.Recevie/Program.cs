using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Collections.Generic;

public static class Recevie
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            var consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queue: "PBS.Queue.Log", noAck: true, consumer: consumer);

            Do(consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();//挂起线程，否则直接退出了。

            Console.ReadLine();

        }
    }

    private static void Do(QueueingBasicConsumer consumer)
    {
        do
        {
            var lst = new List<BasicDeliverEventArgs>();
            for (int i = 0; i < 10; i++)
            {
                var ea = consumer.Queue.Dequeue();
                if (ea.RoutingKey.Contains("Business"))
                {
                    lst.Add(ea);
                }
            }
            if (lst.Count > 0)
            {
                Console.WriteLine(string.Join("\r\n", lst));
            }
            else
            {
                Thread.Sleep(1000);
            }
        } while (true);
    }
}
