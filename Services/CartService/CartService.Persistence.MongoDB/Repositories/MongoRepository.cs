using System.Linq.Expressions;
using CartService.Domain.Abstraction.Repositories.MongoDb;
using MongoDB.Driver;

namespace CartService.Persistence.MongoDB.Repositories;

public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : class
{
    private readonly IMongoCollection<TDocument> _collection;

    public MongoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<TDocument>(typeof(TDocument).Name);
    }

    public virtual IQueryable<TDocument> AsQueryable()
    {
        return _collection.AsQueryable();
    }

    public virtual IEnumerable<TDocument> FilterBy(
        Expression<Func<TDocument, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).ToEnumerable();
    }

    public virtual IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression)
    {
        return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
    }

    public async Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression,
        CancellationToken cancellationToken)
    {
        return await _collection.Find(filterExpression).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task InsertOneAsync(TDocument document, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
    }

    public async Task InsertManyAsync(ICollection<TDocument> documents, CancellationToken cancellationToken)
    {
        await _collection.InsertManyAsync(documents, cancellationToken: cancellationToken);
    }

    public async Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression,
        CancellationToken cancellationToken)
    {
        await _collection.FindOneAndDeleteAsync(filterExpression, cancellationToken: cancellationToken);
    }

    public async Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression,
        CancellationToken cancellationToken)
    {
        await _collection.DeleteManyAsync(filterExpression, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}