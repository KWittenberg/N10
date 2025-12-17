namespace N10.Repository.Interfaces;

public interface IUserRepository
{
    int? GetCurrentUserId();
    //Task<UserDto> GetCurrentUser();
    Task<Result<UserDto>> GetCurrentUserAsync();



    //Task<Result<List<UserDto>>> GetAllToListAsync();
    //Task<Result<IQueryable<UserDto>>> GetAllAsync();


    Task<ResultQPagination<UserDto>> GetPagedAsync(int page, int pageSize, string? search = null, string? sortBy = null, bool sortDescending = false, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetByIdAsync(int id);

    Task<Result> AddAsync(UserInput input);
    Task<Result> UpdateAsync(int id, UserInput input);
    Task<Result> DeleteAsync(int id);



    Task<Result> UpdateAvatarAsync(int id, AvatarInput input, bool isOriginal);
    Task<Result> DeleteAvatarAsync(int id);



    Task<Result<List<RoleDto>>> GetRolesByUserIdAsync(int userId);
    Task<Result<List<RoleDto>>> GetRolesNotAssignedToUserAsync(int userId);
    Task<Result<RoleDto>> GetRoleByIdAsync(int userId, int roleId);
    Task<Result> AddRoleAsync(int userId, int roleId);
    Task<Result> DeleteRoleAsync(int userId, int roleId);




    Task<bool> IsUserAvailableAsync(string email);
}