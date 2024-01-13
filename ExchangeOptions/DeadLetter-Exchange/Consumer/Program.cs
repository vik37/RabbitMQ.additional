using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "dlx", type: ExchangeType.Fanout);

channel.ExchangeDeclare(exchange: "mainexchange", type: ExchangeType.Direct);

channel.QueueDeclare(queue: "mainexchangequeue", 
    arguments: new Dictionary<string, object> { { "x-dead-letter-exchange", "dlx" },
                                                { "x-message-ttl", 1000 } });

channel.QueueBind(queue: "mainexchangequeue", exchange: "mainexchange", "test");

var mainConsumer = new EventingBasicConsumer(channel);

mainConsumer.Received += (sender, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"MAIN - Received new message {message}");
};

channel.BasicConsume(queue: "mainexchangequeue", autoAck: true, consumer: mainConsumer);

channel.QueueDeclare(queue: "dlxexchangequeue");

channel.QueueBind(queue: "dlxexchangequeue", exchange: "dlx", "");

var dlxConsumer = new EventingBasicConsumer(channel);

dlxConsumer.Received += (sender, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"DEAD LETTER - Received new message {message}");
};

channel.BasicConsume(queue: "dlxexchangequeue", autoAck: true, consumer: dlxConsumer);

Console.WriteLine("Consuming: ------ ");

Console.ReadKey();