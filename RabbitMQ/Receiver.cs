using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var connectionFactory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = connectionFactory.CreateConnection())
{
    using (var model = connection.CreateModel())
    {
        model.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var eventingBasicConsumer = new EventingBasicConsumer(model);
        eventingBasicConsumer.Received += (sender, e) =>
        {
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
        };

        model.BasicConsume(queue: "hello", autoAck: true, consumer: eventingBasicConsumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}