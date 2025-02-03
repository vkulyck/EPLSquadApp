using EPLSquadBackend.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace EPLSquadBackend.Services
{
    public class RabbitMQService
    {
        private readonly RabbitMQSettings _rabbitSettings;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(IOptions<RabbitMQSettings> rabbitOptions)
        {
            _rabbitSettings = rabbitOptions.Value;

            var factory = new ConnectionFactory()
            {
                HostName = _rabbitSettings.Hostname,
                Port = _rabbitSettings.Port,
                UserName = _rabbitSettings.Username,
                Password = _rabbitSettings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _rabbitSettings.QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void PublishMessage<T>(T message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(exchange: "",
                                  routingKey: _rabbitSettings.QueueName,
                                  basicProperties: null,
                                  body: body);
        }
    }
}
