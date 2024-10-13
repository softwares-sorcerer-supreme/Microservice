using System.Linq.Expressions;

namespace CartService.Domain.Abstraction.Repositories.MongoDb;

//Can be replace with IRepositoryBase, in fact it is the same, can use IRepositoryBase instead of IMongoRepository
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