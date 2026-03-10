using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Engines;

namespace SurveyBasket.Abstractions;

public class PaginatedList<T>
{
    public List<T> Items { get; private set; } = default!;

    public int PageNumber { get; private set; }

    public int PageSize { get; private set; }
    public int TotalPage { get; private set; }

    public bool HasPreviousePage => PageNumber > 1;
    public bool HasNextPage => TotalPage > PageNumber;

    public PaginatedList(List<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPage = (int)Math.Ceiling((double)totalCount / pageSize);
    }
    public static async Task<PaginatedList<T>> CreatePageAsync(IQueryable<T> query, int pageSize, int pageNumber, CancellationToken cancellationToken)
    {
        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, pageNumber, pageSize, count);
    }

}
