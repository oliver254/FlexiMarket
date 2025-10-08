namespace FlexiMarket.OrderManagement.Application.DTOs;

public record PagedResult<T>(List<T> Items, int TotalCount, int PageNumber, int PageSize);
