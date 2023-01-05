using System.Text;
using RabbitMQ.Client;

var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = connectionFactory.CreateConnection())
{
    using (var model = connection.CreateModel())
    {
        model.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);

        model.BasicPublish(exchange: string.Empty, routingKey: "hello", basicProperties: null, body: body);

        Console.WriteLine(" [x] Sent {0}", message);
    }
}