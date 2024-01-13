using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

using IConnection connection = factory.CreateConnection();

using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "acceptrejectexchange", type: ExchangeType.Fanout);

channel.QueueDeclare(queue: "letterbox");


channel.QueueBind(queue: "letterbox", exchange: "acceptrejectexchange", "test");

var mainConsumer = new EventingBasicConsumer(channel);

mainConsumer.Received += (sender, args) =>
{
    var body = args.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    if(args.DeliveryTag % 5 == 0)
    {
        channel.BasicNack(deliveryTag: args.DeliveryTag, requeue: false, multiple: true);
        //channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: true);
    }  

    Console.WriteLine($"MAIN - Received new message {message}");
};

channel.BasicConsume(queue: "letterbox", consumer: mainConsumer);

Console.WriteLine("Consuming: ---- ");
Console.ReadKey();