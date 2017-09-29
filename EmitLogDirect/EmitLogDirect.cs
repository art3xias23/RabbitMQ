using System;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;



class EmitLogDirect
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs",
                                        type: "direct");

                var severity = (args.Length > 0) ? args[0] : "info"; 

                var message = (args.Length > 1) ? args.Join(" ").Skip(1).ToArray()) : "Hello World!";

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "direct_logs",
                                     routingKey: severity,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] sent Severity {0} : Message {1}", message, severity);
            }
        }

    }
}

