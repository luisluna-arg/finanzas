using Finance.Application.Commons;
using MediatR;

namespace Finance.Application.Queries.Base;

public abstract class GetPaginatedQuery<T> : IRequest<PaginatedResult<T>>
{
    public bool IncludeDeactivated { get; set; }

    /// <summary>
    /// Gets or sets date to filter from. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Gets or sets date to filter to. Format: YYYY-MM-DDTHH:mm:ss.sssZ.
    /// </summary>
    public DateTime? To { get; set; }

    public int Page { get; set; } // Current page number

    public int PageSize { get; set; } // Number of items per page
}