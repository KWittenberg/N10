namespace N10.Services;

public class UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
{
    public async Task<List<ApplicationUser>> GetUsersAsync(string? search = null)
    {
        var query = context.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u =>
                u.Email.Contains(search) ||
                u.FirstName.Contains(search) ||
                u.LastName.Contains(search));
        }

        return await query.ToListAsync();
    }

    public async Task<ApplicationUser?> GetUserAsync(int id) => await userManager.FindByIdAsync(id.ToString());

    // Avatar operacije
    public async Task UpdateAvatarAsync(int userId, string avatarPath)
    {
        var user = await context.Users.FindAsync(userId);
        if (user != null)
        {
            user.AvatarUrl = avatarPath;
            await context.SaveChangesAsync();
        }
    }
}