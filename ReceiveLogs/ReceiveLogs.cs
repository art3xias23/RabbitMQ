using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;


class ReceiveLogs
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {

                channel.ExchangeDeclare(exchange: "logs", type: "fanout");

                var queueName = channel.QueueDeclare().QueueName;

                channel.QueueBind(queue: queueName,
                                exchange: "logs",
                                routingKey: "");

                //Will send a message to free workers and will not bombard one after the other.
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for logs.");



                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine(" [x] {0}", message);



                    //Console.WriteLine("[x] Done");


                    ////Regarding Message acknowledgements, delivery tag is the number of aknowledgement
                    //channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);


                };

                channel.BasicConsume(
                                     queue: "task_queue",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();


            }
        }
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}

