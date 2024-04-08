using GamesDomain.Model;
namespace GamesInfrastructure.Services
{
    public interface IExportService<TEntity>
   where TEntity : Entity
    {
        Task WriteToAsync(Stream stream, CancellationToken cancellationToken);
    }
}
