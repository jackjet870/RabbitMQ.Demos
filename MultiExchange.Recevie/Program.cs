using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public static class Recevie
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            //多线程处理方案
            //DoByMultiThread(channel);

            //队列处理方案
            DoByQueue(channel);

        }
    }

    private static void DoByQueue(IModel channel)
    {
        var consumer = new QueueingBasicConsumer(channel);
        channel.BasicConsume(queue: "PBS.Queue.Log", noAck: true, consumer: consumer);
        do
        {
            var lst = new List<BasicDeliverEventArgs>();
            //先拿到一批
            for (int i = 0; i < 10; i++)
            {
                var ea = consumer.Queue.DequeueNoWait(null);
                if (ea != null && ea.RoutingKey.Contains("Business"))
                {
                    lst.Add(ea);
                }
            }
            if (lst.Count > 0)
            {
                //统一批处理，如写DB，可能比一次一条的写要快。
                Console.WriteLine("\r\n[x] " + string.Join("\r\n[x] ", lst.Select(p => Encoding.UTF8.GetString(p.Body))));
            }
            else
            {
                Thread.Sleep(1000);
            }
        } while (true);
    }

    private static void DoByMultiThread(IModel channel)
    {
        //设置CPU每核最少处理10个线程。
        ThreadPool.SetMinThreads(Environment.ProcessorCount * 10, Environment.ProcessorCount * 20);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            //只管开线程
            //当然如果线程池里有空闲的就直接用
            //如果没有空闲线程 且 没超过最小线程数则直接创建新线程
            //否则，以每秒不超过两个的速度创建新线程
            Task.Run(() =>
            {
                System.Threading.Thread.Sleep(1000);
                var message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine("[x] Received {0}", message);
            });
        };

        channel.BasicConsume(queue: "PBS.Queue.Log", noAck: true, consumer: consumer);


        Console.WriteLine("Press [enter] to exit.");
        Console.ReadLine();//挂起线程，否则直接退出了。

        Console.ReadLine();
    }
}
