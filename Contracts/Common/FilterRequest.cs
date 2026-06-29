namespace AI_genda_API.Contracts.Common;

public record FilterRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchValue { get; set; }
    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; } = "asc";

}

