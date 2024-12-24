using System.Text.Json.Serialization;

namespace EPLSquadBackend.DTO
{
    public class SquadDetailsDto
    {
        [JsonPropertyName("crest")]
        public required string Crest { get; set; }

        [JsonPropertyName("name")]
        public required string TeamName { get; set; }

        [JsonPropertyName("squad")]
        public required List<PlayerDto> Squad { get; set; }
    }
}
