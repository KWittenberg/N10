namespace N10.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor contextAccessor)
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>(options)
{
    #region Note Entities
    public virtual DbSet<Note> Notes { get; set; }
    public virtual DbSet<NoteFolder> NoteFolders { get; set; }
    public virtual DbSet<NoteShare> NoteShares { get; set; }
    public virtual DbSet<NoteImage> NoteImages { get; set; }
    public virtual DbSet<NoteAttachment> NoteAttachments { get; set; }
    #endregion


    public virtual DbSet<MediaSource> MediaSources { get; set; }

    #region Movie Entities
    public virtual DbSet<Movie> Movies { get; set; }
    public virtual DbSet<MovieGenre> MovieGenres { get; set; }
    #endregion


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Auto-Apply Configurations from Assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        // builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in base.ChangeTracker.Entries<BaseAuditableEntity>().Where(q => q.State == EntityState.Added || q.State == EntityState.Modified))
        {
            entry.Entity.LastModifiedUtc = DateTime.UtcNow;
            entry.Entity.LastModifiedId = GetCurrentUserId();

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedUtc = DateTime.UtcNow;
                entry.Entity.CreatedId = GetCurrentUserId();
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    int? GetCurrentUserId()
    {
        var httpContext = contextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true) return null;

        var userIdClaim = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out int userId) ? userId : null;
    }

    // Guid GetCurrentUserId() => Guid.TryParse(contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId) ? userId : Guid.Empty;
}