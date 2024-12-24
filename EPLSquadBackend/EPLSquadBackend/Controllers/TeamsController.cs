using EPLSquadBackend.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EPLSquadBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly FootballApiService _footballApiService;

        public TeamsController(FootballApiService footballApiService)
        {
            _footballApiService = footballApiService;
        }

        [HttpGet("{teamName}")]
        public async Task<IActionResult> GetTeamSquad(string teamName,[FromQuery] int? season)
        {
            try
            {
                var team = await _footballApiService.GetTeamSquadAsync(teamName, season);
                return Ok(team);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
