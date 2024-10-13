using System.Linq.Expressions;

namespace CartService.Domain.Abstraction.Repositories.MongoDb;

//Can be replaced with IRepositoryBase, in real case clean architecture, we can use IRepositoryBase as a base interface for all repositories and for all database
//But for this example, I will use IMongoRepository as a base interface for all mongo repositories
public interface IMongoRepository<TDocument>: IDisposable where TDocument : class
{
    IQueryable<TDocument> AsQueryable();

    IEnumerable<TDocument> FilterBy(
        Expression<Func<TDocument, bool>> filterExpression);

    IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression);

    Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken);

    Task InsertOneAsync(TDocument document, CancellationToken cancellationToken);

    Task InsertManyAsync(ICollection<TDocument> documents, CancellationToken cancellationToken);

    Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken);

    Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken);
}