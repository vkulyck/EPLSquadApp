using System.Text.Json.Serialization;

namespace EPLSquadBackend.DTO
{
    public class PlayerDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public required string FullName { get; set; }

        [JsonPropertyName("position")]
        public required string Position { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public required string DateOfBirth { get; set; }

        [JsonPropertyName("nationality")]
        public required string Nationality { get; set; }
    }
}
