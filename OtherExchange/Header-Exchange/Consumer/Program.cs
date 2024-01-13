using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "headersexchange", type: ExchangeType.Headers);
channel.QueueDeclare(queue: "letterbox");

var bindingArguments = new Dictionary<string, object>
{
    {"x-match", "any" },
    {"name", "brian" },
    {"age", "21" }
};

channel.QueueBind(queue: "letterbox", exchange: "headersexchange", routingKey: "", arguments: bindingArguments);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received new message {message}");
};

channel.BasicConsume(queue: "letterbox", autoAck: true, consumer: consumer);

Console.ReadKey();