using GamesInfrastructure.Services;
using GamesDomain.Model;
using GamesInfrastructure;
namespace GamesInfrastructure.Services
{
    public class GenreDataPortServiceFactory
        : IDataPortServiceFactory<Genre>
    {
        private readonly DbgamesContext _context;
        public GenreDataPortServiceFactory(DbgamesContext context)
        {
            _context = context;
        }
        public IImportService<Genre> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new GenreImportService(_context);
            }
            throw new NotImplementedException($"No import service implemented for movies with content type {contentType}");
        }
        public IExportService<Genre> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new GenreExportService(_context);
            }
            throw new NotImplementedException($"No export service implemented for movies with content type {contentType}");
        }
    }
}