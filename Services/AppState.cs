namespace N10.Services;

public class AppState(AuthenticationStateProvider authStateProvider,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole<int>> roleManager)
{


    #region CurrentUser
    UserDto? User;
    bool IsLoaded;

    public UserDto? CurrentUser => User;

    public async Task InitializeCurrentUserAsync()
    {
        if (IsLoaded) return;

        try
        {
            var authState = await authStateProvider.GetAuthenticationStateAsync();
            if (authState.User?.Identity?.IsAuthenticated != true)
            {
                IsLoaded = true;
                return;
            }

            var user = await userManager.GetUserAsync(authState.User);
            if (user is null)
            {
                IsLoaded = true;
                return;
            }

            List<RoleDto> roles = new();
            var roleNames = await userManager.GetRolesAsync(user);
            foreach (var roleName in roleNames)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role is not null) roles.Add(role.ToDto());
            }

            User = user.ToDto();
            User.Roles = roles;

            IsLoaded = true;
        }
        catch
        {
            IsLoaded = true;
        }
    }

    public void Clear()
    {
        User = null;
        IsLoaded = false;
    }
    #endregion



    #region Main Content Change
    public bool IsExpanded { get; set; } = false;

    public string GetContentClass() => IsExpanded ? CssClasses.Layout.Expanded : CssClasses.Layout.Container;

    public string GetContentIconClass() => IsExpanded ? CssClasses.Icons.Collapse : CssClasses.Icons.Expand;

    public void OnChangeContent() => IsExpanded = !IsExpanded;
    #endregion



    #region Notification
    public event Func<string, string, string?, Task>? OnShowNotify;

    public Task ShowNotify(string message, string type, string details)
    {
        if (OnShowNotify is not null) _ = OnShowNotify.Invoke(message, type, details);

        return Task.CompletedTask;
    }
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