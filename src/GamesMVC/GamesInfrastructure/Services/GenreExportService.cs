using ClosedXML.Excel;
using GamesDomain.Model;
using GamesInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GamesInfrastructure.Services
{

    public class GenreExportService : IExportService<Genre>
    {
        private const string RootWorksheetName = "";

        private static readonly IReadOnlyList<string> HeaderNames =
            new string[]
            {
                "Name",
                "Release date",
                "Developer",
                //"Comments",
                //"Ratings",
                "Genres",
            };
        private readonly DbgamesContext _context;

        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value = HeaderNames[columnIndex];
            }
            worksheet.Row(1).Style.Font.Bold = true;
        }

        private void WriteGame(IXLWorksheet worksheet, Game game, int rowIndex)
        {
            var columnIndex = 1;
            worksheet.Cell(rowIndex, columnIndex++).Value = game.Name;
            worksheet.Cell(rowIndex, columnIndex++).Value = game.ReleaseDate.ToString();
            worksheet.Cell(rowIndex, columnIndex++).Value = game.Developer?.Name;

            //worksheet.Cell(rowIndex, columnIndex++).Value = string.Join(", ", game.Comments.Select(c => c.Text));
            //worksheet.Cell(rowIndex, columnIndex++).Value = string.Join(", ", game.Ratings.Select(r => r.Rating1));
            worksheet.Cell(rowIndex, columnIndex++).Value = string.Join(", ", game.Genres.Select(g => g.Name.Trim()));
        }

        private void WriteGames(IXLWorksheet worksheet, ICollection<GamesDomain.Model.Game> games)
        {
            WriteHeader(worksheet);
            int rowIndex = 2;
            foreach (var game in games)
            {
                WriteGame(worksheet, game, rowIndex);
                rowIndex++;
            }
        }

        public GenreExportService(DbgamesContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("Input stream is not writable");
            }

            var games = await _context.Games
                .Include(g => g.Developer)
                .Include(g => g.Comments)
                .Include(g => g.Genres)
                .ToListAsync(cancellationToken);

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Games");

            WriteGames(worksheet, games);
            workbook.SaveAs(stream);
        }

    }
}
