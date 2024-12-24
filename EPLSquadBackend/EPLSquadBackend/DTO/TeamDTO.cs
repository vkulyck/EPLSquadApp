using System.Text.Json.Serialization;

namespace EPLSquadBackend.DTO
{
    public class TeamDto
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("name")]
        public required string TeamName { get; set; }

        [JsonPropertyName("crest")]
        public required string ProfilePicture { get; set; }
    }
}
