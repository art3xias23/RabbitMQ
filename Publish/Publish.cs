using RabbitMQ.Client;
using System;
using System.Text;

class Publish
    {
       
            public static void Main(string[] args)
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {

                     channel.ExchangeDeclare(exchange: "logs", type: "fanout");        

                    //If this is not declared we get a randomly generated name, non-durable, exclusive, autodelete 
                    

                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);


                    channel.BasicPublish(exchange: "logs",
                                             routingKey: "",
                                             basicProperties: null,
                                             body: body);

                    //channel.QueueBind(queue: queueName,
                    //                  exchange: "logs",
                    //                  routingKey: "");

            Console.WriteLine(" [x] Sent {0}", message);
                }

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

            private static string GetMessage(string[] args)
            {
                return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
            }
        }
    

