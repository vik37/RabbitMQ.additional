using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.QueueDeclare(queue: "request-queue", exclusive: false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, args) =>
{
    Console.WriteLine($"Received Request {args.BasicProperties.CorrelationId}");

    var replyMessage = $"This is your reply {args.BasicProperties.CorrelationId}";

    var body = Encoding.UTF8.GetBytes(replyMessage);

    channel.BasicPublish("", args.BasicProperties.ReplyTo, null, body);
};

channel.BasicConsume(queue: "request-queue",autoAck: true,consumer: consumer);

Console.ReadKey();