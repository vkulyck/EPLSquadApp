using System.Text.Json.Serialization;

namespace EPLSquadBackend.DTO
{
    public class ApiResponseDto
    {
        [JsonPropertyName("teams")]
        public required List<TeamDto> Teams { get; set; }
    }
}
