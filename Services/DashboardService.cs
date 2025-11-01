namespace N10.Services;

public class DashboardService(IDbContextFactory<ApplicationDbContext> dbFactory) : IDashboardService
{
    public async Task<Result<DashboardDto>> GetAllAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var roleCount = await db.Roles.AsNoTracking().CountAsync();
        var userCount = await db.Users.AsNoTracking().CountAsync();

        var noteFolderCount = await db.NoteFolders.AsNoTracking().CountAsync();
        var noteCount = await db.Notes.AsNoTracking().CountAsync();


        //var ordersChart = await db.Orders.AsNoTracking().OrderByDescending(x => x.CreatedUtc).Select(o => new OrderChartDto(o.CreatedUtc, o.Total)).ToListAsync();
        //var ordersCount = ordersChart.Count();

        //var manufacturersCount = await db.Manufacturers.AsNoTracking().CountAsync();
        //var brandsCount = await db.Brands.AsNoTracking().CountAsync();
        //var productsCount = await db.Products.AsNoTracking().CountAsync();

        //var categoriesCount = await db.Categories.AsNoTracking().CountAsync();
        //var authorsCount = await db.Authors.AsNoTracking().CountAsync();
        //var couponsCount = await db.Coupons.AsNoTracking().CountAsync();

        var output = new DashboardDto(roleCount, userCount, noteFolderCount, noteCount);

        return Result<DashboardDto>.Ok(output);
    }
}