using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "simplehashing", type: "x-consistent-hash");

channel.QueueDeclare(queue: "letterbox1");
channel.QueueDeclare(queue: "letterbox2");

channel.QueueBind(queue: "letterbox1", exchange: "simplehashing", "1"); //25%
channel.QueueBind(queue: "letterbox2", exchange: "simplehashing", "3"); //75%

var consumer1 = new EventingBasicConsumer(channel);

consumer1.Received += (sender, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Queue 1 Received new message {message}");
};

channel.BasicConsume(queue: "letterbox1", autoAck: true, consumer: consumer1);

var consumer2 = new EventingBasicConsumer(channel);

consumer2.Received += (sender, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Queue 2 Received new message {message}");
};

channel.BasicConsume(queue: "letterbox2", autoAck: true, consumer: consumer2);

Console.ReadKey();