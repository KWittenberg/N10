namespace N10.Services;

// OVO je sve što ti treba!
public class UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
{

    // Nepotrebno ???
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    // Jednostavne metode - bez previše apstrakcije
    public async Task<List<ApplicationUser>> GetUsersAsync(string? search = null)
    {
        var query = _context.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u =>
                u.Email.Contains(search) ||
                u.FirstName.Contains(search) ||
                u.LastName.Contains(search));
        }

        return await query.ToListAsync();
    }

    public async Task<ApplicationUser?> GetUserAsync(Guid id)
        => await _userManager.FindByIdAsync(id.ToString());

    // Avatar operacije
    public async Task UpdateAvatarAsync(Guid userId, string avatarPath)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.AvatarUrl = avatarPath;
            await _context.SaveChangesAsync();
        }
    }
}