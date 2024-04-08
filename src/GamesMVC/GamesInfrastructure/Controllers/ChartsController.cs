using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GamesDomain.Model;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace GamesInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly DbgamesContext _dbgamesContext;

        public ChartsController(DbgamesContext dbgamesContext)
        {
            _dbgamesContext = dbgamesContext;
        }

        [HttpGet("getAverageRatingPerGame")]
        public async Task<IActionResult> GetAverageRatingPerGame(CancellationToken cancellationToken)
        {
            var ratings = await _dbgamesContext.Ratings
                .Include(r => r.Game)
                .GroupBy(r => new { r.GameId, r.Game.Name })
                .Select(group => new
                {
                    GameId = group.Key.GameId,
                    GameName = group.Key.Name,
                    AverageRating = group.Average(r => r.Rating1)
                })
                .ToListAsync(cancellationToken);
            if(ratings.Count() > 0)
            {
                return Ok(ratings);
            }
            else
            {
                return BadRequest();
            }
        }

        private record AverageRatingResponseItem(int GameId, decimal AverageRating);

        [HttpGet("getGamesReleasedPerYear")]
        public async Task<IActionResult> GetGamesReleasedPerYear(CancellationToken cancellationToken)
        {
            var gamesReleasedPerYear = await _dbgamesContext.Games
                .GroupBy(g => g.ReleaseDate.Year)
                .Select(group => new { Year = group.Key, Count = group.Count() })
                .ToListAsync(cancellationToken);

            return Ok(gamesReleasedPerYear);
        }
    }
}