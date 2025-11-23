namespace N10.Services;

public class AppState(AuthenticationStateProvider authStateProvider,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole<Guid>> roleManager
                        // IHttpContextAccessor contextAccessor, IServiceScopeFactory scopeFactory
                        )
{
    #region CurrentUser
    public UserDto? CurrentUser => _currentUser;
    public bool IsAuthenticated => _currentUser is not null;


    private UserDto? _currentUser;
    private bool _isLoaded;


    public async Task InitializeCurrentUserAsync()
    {
        if (_isLoaded) return;

        try
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var principal = authState.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                _isLoaded = true;
                return;
            }

            var user = await userManager.GetUserAsync(principal);
            if (user is null)
            {
                _isLoaded = true;
                return;
            }

            // Dohvati role names
            var roleNames = await userManager.GetRolesAsync(user);

            // Dohvati pune role preko RoleManager-a
            var roles = new List<RoleDto>();
            foreach (var roleName in roleNames)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role is not null) roles.Add(role.ToDto());
            }

            _currentUser = user.ToDto();
            _currentUser.Roles = roles;

            _isLoaded = true;
        }
        catch
        {
            _isLoaded = true;
        }
    }

    public void Clear()
    {
        _currentUser = null;
        _isLoaded = false;
    }
    #endregion

    #region CurrentUserOLD
    //public UserDto? CurrentUser { get; private set; }

    //public async Task LoadCurrentUser()
    //{
    //    var userId = GetCurrentUserId();
    //    if (userId == Guid.Empty) return;

    //    using var scope = scopeFactory.CreateScope();
    //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    //    var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
    //    if (user is null) return;

    //    var userRoleIds = db.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
    //    var userRoles = db.Roles.Where(r => userRoleIds.Contains(r.Id)).ToList();

    //    var output = user.Adapt<UserDto>();
    //    output.Roles = userRoles.Select(r => r.Adapt<RoleDto>()).ToList();

    //    CurrentUser = output;
    //}

    //public Guid GetCurrentUserId()
    //{
    //    var userIdClaim = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    return userIdClaim != null ? Guid.Parse(userIdClaim) : Guid.Empty;
    //}
    #endregion








    #region Main Content Change
    public bool IsExpanded { get; set; } = false;

    public string GetContentClass() => IsExpanded ? CssClasses.Layout.Expanded : CssClasses.Layout.Container;

    public string GetContentIconClass() => IsExpanded ? CssClasses.Icons.Collapse : CssClasses.Icons.Expand;

    public void OnChangeContent() => IsExpanded = !IsExpanded;
    #endregion

    #region Background
    //private Background _background = new Background("Simple Gray", Placeholder.BackgroundUrl);
    //public Background Background
    //{
    //    get => _background;
    //    set => _background = value;
    //}

    //public Task<Background?> GetBackground()
    //{
    //    return Task.FromResult<Background?>(_background ?? new Background("Simple Gray", Placeholder.BackgroundUrl));
    //}

    //public void SetBackground(Background input)
    //{
    //    Background = input;
    //    hubContext.Clients.All.SendAsync("BackgroundUpdated", input);
    //}

    //public void DeleteBackground()
    //{
    //    Background = new Background("Simple Gray", Placeholder.BackgroundUrl);
    //    hubContext.Clients.All.SendAsync("BackgroundUpdated", Background);
    //}
    #endregion






    #region Toast
    public event Func<string, string, Task>? OnShowToast;

    public Task ShowToast(string message, string type = "success")
    {
        if (OnShowToast is not null) _ = OnShowToast.Invoke(message, type);

        return Task.CompletedTask;
    }


    // public event Action<string, string>? OnShowToast;  //Sinhrona koristi Func, Timer ne radi na sinhronoj!!!

    //public async Task ShowToast(string message, string type = "success")
    //{
    //    if (OnShowToast is not null) await OnShowToast.Invoke(message, type);
    //}
    #endregion

    #region Progress
    public event Func<string, Task>? OnShowProgress;

    public async Task ShowProgress(string message)
    {
        if (OnShowProgress is not null) await OnShowProgress.Invoke(message);
    }
    #endregion

    #region Confirmation
    public event Func<string, string, Task>? OnShowConfirmation;

    public async Task ShowConfirmation(string message, string type)
    {
        if (OnShowConfirmation is not null) await OnShowConfirmation.Invoke(message, type);
    }
    #endregion

    #region DeleteConfirmation
    public event Action<string?, string?, EventCallback>? OnShowDeleteConfirmation;

    public void ShowDeleteConfirmation(string? title, string? message, EventCallback onDelete)
    {
        OnShowDeleteConfirmation?.Invoke(title, message, onDelete);
    }
    #endregion
}