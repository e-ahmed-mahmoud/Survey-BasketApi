using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyBasket.Contracts.Common;

public class RequestFilters
{
    public int PageSize { get; init; } = 10;
    public int PageNumber { get; init; } = 1;
    public string? Search { get; init; }

    public string? Sort { get; set; }
    public string? SortDir { get; set; } = "ASC";
}
