namespace N10.Services.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardDto>> GetAllAsync();
}