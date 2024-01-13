using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

var replyQueue = channel.QueueDeclare(queue: "", exclusive: true);

channel.QueueDeclare(queue: "request-queue", exclusive: false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += MessageReceiver;

channel.BasicConsume(queue: replyQueue.QueueName, autoAck: true, consumer: consumer);

var message = "Can I Request an Reply?";
var body = Encoding.UTF8.GetBytes(message);

var properties = channel.CreateBasicProperties();
properties.ReplyTo = replyQueue.QueueName;
properties.CorrelationId = Guid.NewGuid().ToString();

channel.BasicPublish("", "request-queue", properties, body);

Console.WriteLine($"Sending Request {properties.CorrelationId}");

Console.WriteLine("Started CLient");

void MessageReceiver(object? sender, BasicDeliverEventArgs args)
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Reply Received {message}");
}
