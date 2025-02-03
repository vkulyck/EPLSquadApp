using EPLSquadBackend.Services;
using EPLSquadBackend.DTO;
using EPLSquadBackend.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using EPLSquadBackend.Configuration;
using Moq.Protected;

namespace EPLSquadBackend.Tests
{
    [TestFixture]
    public class FootballApiServiceTests : IDisposable
    {
        private Mock<IOptions<FootballApiSettings>> _apiSettingsMock;
        private Mock<IOptions<RedisSettings>> _redisSettingsMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<ILogger<FootballApiService>> _loggerMock;
        private Mock<IConnectionMultiplexer> _redisMock;
        private Mock<IDatabase> _databaseMock;
        private FootballApiService _footballApiService;
        private HttpClient _httpClient;
        private Mock<IRabbitMQService> _rabbitMQServiceMock;


        [SetUp]
        public void SetUp()
        {
            _apiSettingsMock = new Mock<IOptions<FootballApiSettings>>();
            _redisSettingsMock = new Mock<IOptions<RedisSettings>>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _loggerMock = new Mock<ILogger<FootballApiService>>();
            _redisMock = new Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();
            _rabbitMQServiceMock = new Mock<IRabbitMQService>();

            _redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_databaseMock.Object);

            _apiSettingsMock.SetupGet(x => x.Value).Returns(new FootballApiSettings
            {
                BaseUrl = "https://api.football-data.org/v4",
                Token = "mock-token",
                Season = "2024",
                EplTeamsEndpoint = "/competitions/PL/teams",
                TeamDetailsEndpoint = "/teams/{id}"
            });

            _redisSettingsMock.SetupGet(x => x.Value).Returns(new RedisSettings
            {
                ConnectionString = "localhost:6379",
                CacheDurationInSeconds = 3600
            });

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _rabbitMQServiceMock.Setup(x => x.PublishMessage(It.IsAny<Team>())).Verifiable();

            _footballApiService = new FootballApiService(
                _apiSettingsMock.Object,
                _redisSettingsMock.Object,
                _httpClient,
                _loggerMock.Object,
                _redisMock.Object,
                _rabbitMQServiceMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public async Task GetTeamSquadAsync_ShouldReturnTeam_WhenTeamExistsInApi()
        {
            // Arrange
            var teamName = "The Hammers";
            var season = 2024;
            var cacheKey = $"EPLSquad:{teamName}:{season}";

            var apiTeamsResponse = new ApiResponseDto
            {
                Teams = new List<TeamDto>
                {
                    new TeamDto { Id = 1, TeamName = "West Ham United FC", ProfilePicture = "some_url" }
                }
            };

            var apiSquadResponse = new SquadDetailsDto
            {
                Crest = "https://crests.football-data.org/563.png",
                TeamName = "West Ham United FC",
                Squad = new List<PlayerDto>
                {
                    new PlayerDto { FullName = "Declan Rice", DateOfBirth = "1999-01-14", Position = "Midfielder", Nationality = "England" }
                }
            };

            _databaseMock.Setup(db => db.StringGetAsync(cacheKey, It.IsAny<CommandFlags>()))
                         .ReturnsAsync((RedisValue)string.Empty);

            _httpMessageHandlerMock.SetupRequest(
                HttpMethod.Get,
                "https://api.football-data.org/v4/competitions/PL/teams?season=2024",
                JsonSerializer.Serialize(apiTeamsResponse),
                "application/json",
                HttpStatusCode.OK);

            _httpMessageHandlerMock.SetupRequest(
                HttpMethod.Get,
                "https://api.football-data.org/v4/teams/1",
                JsonSerializer.Serialize(apiSquadResponse),
                "application/json",
                HttpStatusCode.OK);

            // Act
            var result = await _footballApiService.GetTeamSquadAsync(teamName, season);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("West Ham United FC", result.Name);
            Assert.AreEqual(1, result.Squad.Count);
            Assert.AreEqual("Declan", result.Squad[0].FirstName);
            Assert.AreEqual("Rice", result.Squad[0].Surname);
            Assert.AreEqual("Midfielder", result.Squad[0].Position);
        }

        [Test]
        public async Task GetTeamSquadAsync_ShouldReturnCachedData_WhenCacheHit()
        {
            // Arrange
            var teamName = "The Hammers";
            var season = 2024;
            var cacheKey = $"EPLSquad:{teamName}:{season}";

            var cachedTeam = new Team
            {
                Crest = "https://crests.football-data.org/563.png",
                Name = "West Ham United",
                Squad = new List<Player>
                {
                    new Player { FirstName = "Declan", Surname = "Rice", Position = "Midfielder", ProfilePicture = "some_url", DateOfBirth = new DateTime(1999, 1, 14) }
                }
            };

            _databaseMock.Setup(db => db.StringGetAsync(cacheKey, It.IsAny<CommandFlags>()))
                         .ReturnsAsync(JsonSerializer.Serialize(cachedTeam));

            // Act
            var result = await _footballApiService.GetTeamSquadAsync(teamName, season);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("West Ham United", result.Name);
            Assert.AreEqual(1, result.Squad.Count);
            Assert.AreEqual("Declan", result.Squad[0].FirstName);
            Assert.AreEqual("Rice", result.Squad[0].Surname);
            Assert.AreEqual("Midfielder", result.Squad[0].Position);
        }

        [Test]
        public void GetTeamSquadAsync_ShouldThrowException_WhenApiResponseIsInvalid()
        {
            // Arrange
            var teamName = "Invalid Team";
            var season = 2024;

            _httpMessageHandlerMock.SetupRequest(
                HttpMethod.Get,
                "https://api.football-data.org/v4/competitions/PL/teams?season=2024",
                "{}",
                "application/json",
                HttpStatusCode.OK);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _footballApiService.GetTeamSquadAsync(teamName, season));
        }

    }

    public static class HttpMessageHandlerExtensions
    {
        public static void SetupRequest(this Mock<HttpMessageHandler> handler, HttpMethod method, string url, string content, string mediaType, HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == method && req.RequestUri.ToString() == url),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }
    }
}
