using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;



    class Worker
        {
            public static void Main()
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {

                        channel.QueueDeclare(queue: "task_queue", //queue name
                                             durable: true, //true if we are declaring a durable queue, one that will survive a server restart
                                             exclusive: false, //true if we decalring a queue restricted to this connection
                                             autoDelete: false, //when true the server will delete the queue when its no longer in use
                                             arguments: null); //other properties, constructional arguments

                //Requesting Specific quallity of service settings. They restrict the amount of data which is sent
                //prefetchsize: maximum amount of data that the server will send 0 if unlimited, prefetchCount: maximum number of messages that the server will deliver 0 if unlimited, global: true if settings should be applied to entire channel and not each consumer
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

               

                var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {

                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);

                            Console.WriteLine(" [x] Received {0}", message);

                            int dots = message.Split('.').Length - 1;
                            Thread.Sleep(dots * 1000);

                            Console.WriteLine("[x] Done");


                            //Acknowledge one or several received messages
                            //multiple: true to acknowledge all messages, false to acknowledge only deliverytag
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        };

                        channel.BasicConsume(
                                             queue: "task_queue",
                                             autoAck: true, //true if the server should consider messages acknowledged once delivered; false if the server should expect explicit acknowledgements
                                             consumer: consumer);

                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();


            }
                }
            }
        }


    


