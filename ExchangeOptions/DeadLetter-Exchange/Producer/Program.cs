using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "mainexchange", type: ExchangeType.Direct);


var message = "This message might expire";

var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish("mainexchange", "test", null, body);

Console.WriteLine($"Sending Message: {message}");

Console.ReadKey();