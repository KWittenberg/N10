namespace N10.DTOs;

// RESULT |Simple non-generic result
public record Result(bool Success, string Message)
{
    public static Result Ok(string message = "Success") => new(true, message);
    public static Result Error(string message) => new(false, message);
}

// RESULT |Generic result wrapper
public record Result<T>(bool Success, string Message, T? Data = default)
{
    public static Result<T> Ok(T data, string message = "Success") => new(true, message, data);
    public static Result<T> Error(string message) => new(false, message, default);
}


// QuickGrid |query pagination DTO (page is zero-based)
public record QPaginationDto(int TotalRecords, int Page, int PageSize, string? SortBy = null, bool SortDescending = false);

// RESULT QuickGridPagination
public record ResultQPagination<T>(bool Success, string Message, List<T>? Data, QPaginationDto? Pagination)
{
    public static ResultQPagination<T> Ok(List<T> data, QPaginationDto pagination, string message = "Success") => new(true, message, data, pagination);

    public static ResultQPagination<T> Empty(QPaginationDto pagination, string message = "No items") => new(true, message, new List<T>(), pagination);

    public static ResultQPagination<T> Error(string message) => new(false, message, null, null);
}

// CATEGORY
public record CategoryDto(Guid Id, string Name, string? Description, string? IconHtml);

// MAP
public record MapDto(Guid Id, string Name, string? IconHtml);

// DASHBOARD
public record DashboardDto(int? RoleCount, int? UserCount);