using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "altexchange", type: ExchangeType.Fanout);

channel.ExchangeDeclare(exchange: "mainexchange", type: ExchangeType.Direct,
    arguments: new Dictionary<string, object> { { "alternate-exchange", "altexchange" } });

channel.QueueDeclare(queue: "altexchangequeue");
channel.QueueBind(queue: "altexchangequeue", exchange: "altexchange", "");

var altConsumer = new EventingBasicConsumer(channel);

altConsumer.Received += (sender, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"ALT - Received new message {message}");
};

channel.BasicConsume(queue: "altexchangequeue", autoAck: true, consumer: altConsumer);

channel.QueueDeclare(queue: "mainexchangequeue");
channel.QueueBind(queue: "mainexchangequeue", exchange: "mainexchange", "test");

var mainConsumer = new EventingBasicConsumer(channel);

mainConsumer.Received += (sender, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"MAIN - Received new message {message}");
};

channel.BasicConsume(queue: "mainexchangequeue", autoAck: true, consumer: mainConsumer);

Console.ReadKey();