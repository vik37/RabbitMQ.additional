using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "acceptrejectexchange", type: ExchangeType.Fanout);

while (true)
{
    var message = "Lets send this message";

    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("acceptrejectexchange", "test", null, body);

    Console.WriteLine("Sending Message");

    Console.WriteLine("Press any key to continue");

    Console.ReadKey();
}



