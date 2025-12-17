namespace N10.Services;

public class CurrentUserService(AuthenticationStateProvider authStateProvider,
                                UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole<int>> roleManager) : ICurrentUserService
{
    private UserDto? _cachedUser;
    private bool _isLoaded;

    public UserDto? CurrentUser => _cachedUser;

    public async Task<UserDto?> GetUserAsync()
    {
        if (_isLoaded) return _cachedUser;

        try
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            var principal = authState.User;

            if (principal?.Identity?.IsAuthenticated != true)
            {
                _isLoaded = true;
                return null;
            }

            var user = await userManager.GetUserAsync(principal);
            if (user is null)
            {
                _isLoaded = true;
                return null;
            }

            // Dohvati role names
            var roleNames = await userManager.GetRolesAsync(user);

            // Dohvati pune role preko RoleManager-a
            var roles = new List<RoleDto>();
            foreach (var roleName in roleNames)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role is not null)
                {
                    roles.Add(role.ToDto());
                }
            }

            _cachedUser = user.ToDto();
            _cachedUser.Roles = roles;

            _isLoaded = true;
            return _cachedUser;
        }
        catch
        {
            _isLoaded = true;
            return null;
        }
    }
}