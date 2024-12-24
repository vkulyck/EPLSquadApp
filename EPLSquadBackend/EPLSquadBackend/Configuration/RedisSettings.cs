namespace EPLSquadBackend.Configuration
{
    public class RedisSettings
    {
        public required string ConnectionString { get; set; }
        public required int CacheDurationInSeconds { get; set; }
    }
}
