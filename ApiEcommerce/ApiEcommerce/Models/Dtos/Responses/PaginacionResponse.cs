using System;

namespace ApiEcommerce.Models.Dtos.Responses;

public class PaginacionResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPaginas { get; set; }

    public ICollection<T> Items { get; set; } = new List<T>();
}
