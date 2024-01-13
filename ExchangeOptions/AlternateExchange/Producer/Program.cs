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
    arguments: new Dictionary<string, object> { {"alternate-exchange", "altexchange" } });


var message = "This message is intended for alternate exchange";

var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish("mainexchange", "test", null, body);

Console.WriteLine("Sending Message");

Console.ReadKey();