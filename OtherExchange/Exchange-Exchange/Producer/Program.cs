using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "firstexchange", type: ExchangeType.Direct);
channel.ExchangeDeclare(exchange: "secondexchange", type: ExchangeType.Fanout);

channel.ExchangeBind("secondexchange", "firstexchange", "");

var message = "This message has gone through multiple exchanges";

var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish("firstexchange", "", null, body);

Console.WriteLine("Sending Message");

Console.ReadKey();