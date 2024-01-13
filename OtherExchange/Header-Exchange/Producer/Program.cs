using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "headersexchange", type: ExchangeType.Headers);

var message = "This message will be sent through headers";

var body = Encoding.UTF8.GetBytes(message);

var properties = channel.CreateBasicProperties();
properties.Headers = new Dictionary<string, object>
{
    {"name","brian" }
};

channel.BasicPublish("headersexchange", "", basicProperties: properties,body: body);

Console.WriteLine($"Sending Message: {message}");

Console.ReadKey();