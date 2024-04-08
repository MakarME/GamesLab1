using ClosedXML.Excel;
using GamesInfrastructure.Services;
using GamesDomain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GamesInfrastructure.Services
{
    public class GenreImportService : IImportService<Genre>
    {
        private readonly DbgamesContext _context;

        public GenreImportService(DbgamesContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Data can not be read", nameof(stream));
            }

            using (XLWorkbook workBook = new XLWorkbook(stream))
            {
                foreach (IXLWorksheet worksheet in workBook.Worksheets)
                {
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        await AddGameAsync(row, cancellationToken);
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task AddGameAsync(IXLRow row, CancellationToken cancellationToken)
        {
            string gameName = GetGameName(row);
            DateOnly releaseDate = GetGameReleaseDate(row);

            if (_context.Games.Any(g => g.Name == gameName && g.ReleaseDate == releaseDate))
            {
                return;
            }

            Game game = new Game();
            game.Name = GetGameName(row);
            game.ReleaseDate = GetGameReleaseDate(row);
            game.Developer = await GetGameDeveloperAsync(row, cancellationToken);

            /*var ratingsText = row.Cell(5).Value.ToString();
            if (decimal.TryParse(ratingsText, out decimal ratingValue))
            {
                var rating = new Rating { Rating1 = ratingValue };
                game.Ratings.Add(rating);
            }*/

            //await GetGameCommentsAsync(row, game, cancellationToken);
            await GetGameGenresAsync(row, game, cancellationToken);
            _context.Games.Add(game);
        }

        private string GetGameName(IXLRow row)
        {
            return row.Cell(1).Value.ToString();
        }

        private DateOnly GetGameReleaseDate(IXLRow row)
        {
            return DateOnly.Parse(row.Cell(2).Value.ToString());
        }

        private async Task<Developer> GetGameDeveloperAsync(IXLRow row, CancellationToken cancellationToken)
        {
            var developerName = row.Cell(3).Value.ToString();
            var developer = await _context.Developers.FirstOrDefaultAsync(d => d.Name == developerName, cancellationToken);
            if (developer == null)
            {
                developer = new Developer { Name = developerName };
                _context.Developers.Add(developer);
            }
            return developer;
        }

        private async Task GetGameCommentsAsync(IXLRow row, Game game, CancellationToken cancellationToken)
        {
            var commentsText = row.Cell(4).Value.ToString();

            var comment = new Comment { Text = commentsText };

            game.Comments.Add(comment);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task GetGameGenresAsync(IXLRow row, Game game, CancellationToken cancellationToken)
        {
            var genresText = row.Cell(4).Value.ToString();

            var genresArray = genresText.Split(',');

            foreach (var genreName in genresArray)
            {
                var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName.Trim(), cancellationToken);
                if (genre == null)
                {
                    genre = new Genre { Name = genreName.Trim() };
                    _context.Genres.Add(genre);
                }
                game.Genres.Add(genre);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
