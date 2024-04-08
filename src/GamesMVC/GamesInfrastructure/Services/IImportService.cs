using GamesDomain.Model;
namespace GamesInfrastructure.Services
{
    public interface IImportService<TEntity>
         where TEntity : Entity
    {
        Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
    }
}
