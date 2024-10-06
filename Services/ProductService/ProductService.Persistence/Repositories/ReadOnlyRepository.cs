using System.Data;
using Dapper;
using ProductService.Domain.Abstraction.Repositories;

namespace ProductService.Persistence.Repositories;

public class ReadOnlyRepository : IReadOnlyRepository, IDisposable
{
    private readonly ApplicationDbContext _context;

    public ReadOnlyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
    {
        return (await _context.Connection.QueryAsync<T>(sql, param, transaction)).AsList();
    }

    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.Connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
    {
        return await _context.Connection.QuerySingleAsync<T>(sql, param, transaction);
    }

    public void Dispose()
    {
        _context.Connection.Dispose();
    }
}