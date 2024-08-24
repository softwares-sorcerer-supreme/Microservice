using Microsoft.EntityFrameworkCore;
using ReviewVerse.Shared.Models.Dtos;

namespace Shared.Extensions;

public static class LinqExtensions
{
    public static async Task<PagingDataDto<T>> ToListAsPageAsync<T>(this IQueryable<T> query, int pageNumber, int maxPerPage, CancellationToken cancelToken) where T : class
    {
        var result = new PagingDataDto<T>();
        var pagingResponse = new PagingDto
        {
            MaxPerPage = maxPerPage,
            PageNumber = pageNumber
        };
        var totalItem = await query.CountAsync(cancelToken);
        if (totalItem == 0)
        {
            result.Paging = pagingResponse;
            return result;
        }

        pagingResponse.TotalItem = totalItem;
        pagingResponse.TotalPage = (int)Math.Ceiling((float)totalItem / maxPerPage);
        var data = await query
            .Skip((pageNumber - 1) * maxPerPage)
            .Take(maxPerPage)
            .AsNoTracking()
            .ToListAsync(cancelToken);

        result.Paging = pagingResponse;
        result.Data = data;
        return result;
    }
}
