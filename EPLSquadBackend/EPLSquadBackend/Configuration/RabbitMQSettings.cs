namespace EPLSquadBackend.Configuration
{
    public class RabbitMQSettings
    {
        public string Hostname { get; set; } = "rabbitmq";
        public int Port { get; set; } = 5672;
        public string QueueName { get; set; } = "EPLSquadQueue";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
