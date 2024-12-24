namespace EPLSquadBackend.Configuration
{
    public class FootballApiSettings
    {
        public required string BaseUrl { get; set; }
        public required string Token { get; set; }
        public required string EplTeamsEndpoint { get; set; }
        public required string TeamDetailsEndpoint { get; set; }
        public required string Season { get; set; }
    }
}
