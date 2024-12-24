using EPLSquadBackend.Configuration;
using EPLSquadBackend.DTO;
using EPLSquadBackend.Mappers;
using EPLSquadBackend.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Text.Json;

namespace EPLSquadBackend.Services
{
    public class FootballApiService
    {
        private readonly HttpClient _httpClient;
        private readonly FootballApiSettings _apiSettings;
        private readonly RedisSettings _redisSettings;
        private readonly ILogger<FootballApiService> _logger;
        private readonly IDatabase _cache;
        

        public FootballApiService(IOptions<FootballApiSettings> apiOptions, IOptions<RedisSettings> redisOptions, HttpClient httpClient, ILogger<FootballApiService> logger, IConnectionMultiplexer redis)
        {
            try
            {
                _apiSettings = apiOptions.Value;
                _redisSettings = redisOptions.Value;
                _httpClient = httpClient;
                _logger = logger;
                _cache = redis.GetDatabase();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing FootballApiService: {Message}", ex.Message);
                throw;
            }

        }

        public async Task<Team> GetTeamSquadAsync(string teamNameOrNickname, int? year = null)
        {
            try
            {
                // Step 1: Generate cache key
                var seasonYear = year ?? int.Parse(_apiSettings.Season);
                var cacheKey = $"EPLSquad:{teamNameOrNickname}:{seasonYear}";

                // Step 2: Check cache
                var cachedTeam = await _cache.StringGetAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedTeam))
                {
                    _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                    return JsonSerializer.Deserialize<Team>(cachedTeam);
                }

                _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching from API.", cacheKey);

                // Step 3: Fetch data from API
                var eplTeamsUrl = $"{_apiSettings.BaseUrl}{_apiSettings.EplTeamsEndpoint}?season={seasonYear}";
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", _apiSettings.Token);

                var teamsResponse = await _httpClient.GetAsync(eplTeamsUrl);
                if (!teamsResponse.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error fetching EPL teams: {teamsResponse.StatusCode}");

                var teamsContent = await teamsResponse.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponseDto>(teamsContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse == null || apiResponse.Teams == null || !apiResponse.Teams.Any())
                    throw new Exception("No teams found in the EPL API response.");

                // Match team by nickname
                if (TeamNicknameMapper.NicknameMap.TryGetValue(teamNameOrNickname, out var resolvedTeamName))
                {
                    teamNameOrNickname = resolvedTeamName;
                }

                var matchingTeam = apiResponse.Teams.FirstOrDefault(team =>team.TeamName.IndexOf(teamNameOrNickname, StringComparison.OrdinalIgnoreCase) >= 0);
                if (matchingTeam == null)
                {
                    throw new Exception($"Team '{teamNameOrNickname}' not found in the English Premier League.");
                }
                    

                // Fetch squad details
                var teamDetailsUrl = $"{_apiSettings.BaseUrl}{_apiSettings.TeamDetailsEndpoint.Replace("{id}", matchingTeam.Id.ToString())}";
                var teamDetailsResponse = await _httpClient.GetAsync(teamDetailsUrl);
                if (!teamDetailsResponse.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error fetching squad details: {teamDetailsResponse.StatusCode}");

                var teamDetailsContent = await teamDetailsResponse.Content.ReadAsStringAsync();
                var squadDetails = JsonSerializer.Deserialize<SquadDetailsDto>(teamDetailsContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (squadDetails == null || squadDetails.Squad == null || !squadDetails.Squad.Any())
                    throw new Exception("No squad details found for the specified team.");

                // Map to Team model
                var team = new Team
                {
                    Crest = squadDetails.Crest,
                    Name = squadDetails.TeamName,
                    Squad = squadDetails.Squad.Select(playerDto => new Player
                    {
                        ProfilePicture = string.Empty,
                        FirstName = playerDto.FullName.Contains(" ") ? playerDto.FullName.Split(' ')[0] : playerDto.FullName,
                        Surname = playerDto.FullName.Contains(" ") ? playerDto.FullName.Split(' ')[^1] : string.Empty,
                        DateOfBirth = DateTime.TryParse(playerDto.DateOfBirth, out var dob) ? dob : DateTime.MinValue,
                        Position = playerDto.Position
                    }).ToList()
                };

                // Step 4: Cache the result
                var serializedTeam = JsonSerializer.Serialize(team);
                await _cache.StringSetAsync(cacheKey, serializedTeam, TimeSpan.FromSeconds(_redisSettings.CacheDurationInSeconds));

                return team;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while fetching data for team: {TeamNameOrNickname}", teamNameOrNickname);
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error while parsing API response for team: {TeamNameOrNickname}", teamNameOrNickname);
                throw new Exception("Failed to parse the API response. Please check the API data format.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, teamNameOrNickname);
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
