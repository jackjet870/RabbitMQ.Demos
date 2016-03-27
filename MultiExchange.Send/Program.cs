using System;
using RabbitMQ.Client;
using System.Text;


public static class Send
{
    public static void Main(string[] args)
    {

        var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("PBS.Exchange.Entry", ExchangeType.Fanout, durable: true);

            channel.ExchangeDeclare("PBS.Exchange.4Log", ExchangeType.Topic, durable: true);
            channel.ExchangeDeclare("PBS.Exchange.4Business", ExchangeType.Topic, durable: true);

            channel.ExchangeBind("PBS.Exchange.4Log", "PBS.Exchange.Entry", "fanout会忽略此设置");
            channel.ExchangeBind("PBS.Exchange.4Business", "PBS.Exchange.Entry", "fanout会忽略此设置");



            channel.QueueDeclare("PBS.Queue.Log", durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueDeclare("PBS.Queue.Business.SearchTax", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare("PBS.Queue.Business.CheckChange", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueDeclare("PBS.Queue.Business.GenPnr", durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.QueueBind("PBS.Queue.Log", "PBS.Exchange.4Log", "#");
            channel.QueueBind("PBS.Queue.Business.SearchTax", "PBS.Exchange.4Business", "PBS.Queue.Business.SearchTax");
            channel.QueueBind("PBS.Queue.Business.CheckChange", "PBS.Exchange.4Business", "PBS.Queue.Business.CheckChange");
            channel.QueueBind("PBS.Queue.Business.GenPnr", "PBS.Exchange.4Business", "PBS.Queue.Business.GenPnr");


            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.DeliveryMode = 2;

            var body = Encoding.UTF8.GetBytes("查税" + DateTime.Now);
            channel.BasicPublish(exchange: "PBS.Exchange.Entry",
                                            routingKey: "PBS.Queue.Business.SearchTax",
                                            basicProperties: properties,
                                            body: body);
            Console.WriteLine(" [x] Sent PBS.Queue.Business.SearchTax");

            body = Encoding.UTF8.GetBytes("检测变价" + DateTime.Now);
            channel.BasicPublish(exchange: "PBS.Exchange.Entry",
                                            routingKey: "PBS.Queue.Business.CheckChange",
                                            basicProperties: properties,
                                            body: body);
            Console.WriteLine(" [x] Sent PBS.Queue.Business.CheckChange");

            body = Encoding.UTF8.GetBytes("生成PNR" + DateTime.Now);
            channel.BasicPublish(exchange: "PBS.Exchange.Entry",
                                            routingKey: "PBS.Queue.Business.GenPnr",
                                            basicProperties: properties,
                                            body: body);
            Console.WriteLine(" [x] Sent PBS.Queue.Business.GenPnr");


            Console.ReadLine();

        }

    }
}