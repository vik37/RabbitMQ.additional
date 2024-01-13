using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "simplehashing", type: "x-consistent-hash");


var message = "Hello hash the routing key and passs me on please";

var body = Encoding.UTF8.GetBytes(message);

var routingKey = "hash me112w3ewe rewq";

channel.BasicPublish(exchange: "simplehashing", routingKey: routingKey, null, body);

Console.WriteLine("Sending Message");

//Console.ReadKey();